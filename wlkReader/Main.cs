//======================================================================
//  wlkReader - reads *.wlk data files from Davis Instruments Weatherlink
//
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace wlkReader
{


    //   Data is stored in monthly files.  Each file has the following header.
    //
    public struct DayIndex
    {
        public short recordsInDay;      // includes any daily summary records
        public long startPos;       // The index (starting at 0) of the first daily summary record
    };

    //======================================================================
    // Main form
    //======================================================================
    public partial class Main : Form
    {
        //======================================================================
        // global data
        //======================================================================

        // misc
        //
        private ListViewColumnSorter lvwColumnSorter;

        // Header Block
        //
        public char[] idCode = new char[16]; // = {'W', 'D', 'A', 'T', '5', '.', '0', 0, 0, 0, 0, 0, 0, 0, 5, 0}
        public long totalRecords;
        public DayIndex[] dayIndex = new DayIndex[32];

        // interval data
        //
        public string fpWLK;
        public int dataYear;
        public int dataMonth;
        public List<clsWeatherData> lstRecords;

        //======================================================================
        // METHODS/ROUTINES
        //======================================================================

        //----------------------------------------------------
        // Main
        //----------------------------------------------------
        public Main()
        {
            InitializeComponent();

            // init
            //
            lstRecords = new List<clsWeatherData>();

            // setup ListView
            //
            lvRecords.HeaderStyle = ColumnHeaderStyle.Clickable;
            lvRecords.Columns.Add("Time");          // 0
            lvRecords.Columns.Add("TempOut");       // 1
            lvRecords.Columns.Add("TempIn");        // 2
            lvRecords.Columns.Add("HumOut");        // 3
            lvRecords.Columns.Add("HumIn");         // 4
            lvRecords.Columns.Add("Baro");          // 5
            lvRecords.Columns.Add("Rain");          // 6
            lvRecords.Columns.Add("Rate");          // 7
            lvRecords.Columns.Add("Wind");          // 8
            lvRecords.Columns.Add("WindDir");       // 9
            lvRecords.Columns.Add("Gust");          // 10
            lvRecords.Columns.Add("GustDir");       // 11

            lvRecords.View = View.Details;

            lvwColumnSorter = new ListViewColumnSorter();
            this.lvRecords.ListViewItemSorter = lvwColumnSorter;

        }  // end of main

        //----------------------------------------------------
        // Open a WLK File
        //----------------------------------------------------
        private void openWLKFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fsWLK;
            BinaryReader brWLK;
            byte dataType;
            byte[] buff;
            clsWeatherData oRecord;
            int headerSize;
            string strTmp;

            //**********************
            // Ask user for file
            //
            openFileDialog1.Title = "Select WLK file";
            openFileDialog1.Filter = "WLK | *.wlk";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            fpWLK = openFileDialog1.FileName;

            //*******************
            //  parse filename into Year and Month
            //
            strTmp = Path.GetFileName(fpWLK);
            if ((strTmp.Substring(4, 1) != "_") && (strTmp.Substring(4, 1) != "-"))
            {
                MessageBox.Show("error: Filename is not in YYYY_MM format");
                return;
            }
            if (!int.TryParse(strTmp.Substring(0,4), out dataYear))
            {
                MessageBox.Show("error parsing Year from filename.");
                return;
            }
            if (!int.TryParse(strTmp.Substring(5,2), out dataMonth))
            {
                MessageBox.Show("error parsing Mont from filename.");
                return;
            }

            //*******************
            // Open file
            //
            try
            {
                fsWLK = File.Open(fpWLK, FileMode.Open, FileAccess.Read, FileShare.Read);
                brWLK = new BinaryReader(fsWLK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening WLK file <" + ex.Message + ">.");
                return;
            }
            toolStripStatusLabel1.Text = "Reading data from WLK file...";
            Application.DoEvents();

            //*****************
            // now read data from file
            //
            lstRecords.Clear();
            lvRecords.Items.Clear();
            try
            {

                // Read file header block
                //
                idCode = brWLK.ReadChars(16);
                totalRecords = brWLK.ReadInt32();
                for(int i = 0; i < 32; i++)
                {
                    dayIndex[i].recordsInDay = brWLK.ReadInt16();
                    dayIndex[i].startPos = brWLK.ReadInt32();
                }
                headerSize = 16 + 4 + 32 * (2 + 4);     // size of header record

                // Read records one day at a time...
                //
                for( int iDay = 1; iDay < 32; iDay++)
                {
                    // seek to the star of this day
                    //
                    fsWLK.Seek(headerSize + dayIndex[iDay].startPos*88, SeekOrigin.Begin);

                    // Read all data records for this day
                    //   but just save the standard archive records
                    //
                    for (int i = 0; i < dayIndex[iDay].recordsInDay; i++)
                    {

                        // read first byte to ID the record type
                        //
                        dataType = brWLK.ReadByte();

                        // now handle it...
                        //
                        if (dataType != 1)
                        {
                            // not a standard archive record, read the 87 remaining bytes and move on
                            //
                            buff = brWLK.ReadBytes(87);
                        }
                        else
                        {
                            // we have a standard archive record, read it and keep the data
                            //
                            oRecord = new clsWeatherData();
                            oRecord.ReadRecord(brWLK);          // errors will propagate out...
                            oRecord.dteTime = new DateTime(dataYear,dataMonth, iDay).AddMinutes(oRecord.packedTime);            // set date/time

                            lstRecords.Add(oRecord);
                        }

                    } // end of loop for reading records for a day

                    toolStripStatusLabel1.Text = string.Format("{0}", iDay);
                    Application.DoEvents();

                }  // end of loop through days of month


            }
            catch( Exception ex)
            {
                MessageBox.Show("Error opening WLK file <" + ex.Message + ">.");
                return;
            }
            finally
            {
                fsWLK.Close();
                brWLK.Close();
            }

            // update the list
            //
            UpdateList();
        
            // all done
            //
            toolStripStatusLabel1.Text = "WLK file has been read. " + lstRecords.Count.ToString() + " records imported.";

        } // end of open WLK file

        //----------------------------------------------------
        // DisplayLine - return ListView item for listview control
        //----------------------------------------------------
        private ListViewItem DisplayLine(clsWeatherData oRecord)
        {
            ListViewItem lviTmp;
            int rainCollecterType;
            int rainClicks;
            double rainFactor;
            double rainVal;
            int iTmp;

            //init
            //
            lviTmp = new ListViewItem();        // note: this creates the first subitem

            // date/time in python format %Y-%m-%d %H:%M:%S : 2020 Apr 02 07:47
            //
            lviTmp.SubItems[0].Text = (string.Format("{0:yyyy-MM-dd HH:mm:ss}", oRecord.dteTime));

            // outside temp
            //
            if (oRecord.outsideTemp < -1000)
            {
                lviTmp.SubItems.Add("");
            }
            else
            {
                lviTmp.SubItems.Add(string.Format("{0:0.0}", (double)oRecord.outsideTemp * 0.1));
            }

            // inside temp
            //
            if (oRecord.insideTemp < -1000)
            {
                lviTmp.SubItems.Add("");
            }
            else
            {
                lviTmp.SubItems.Add(string.Format("{0:0.0}", (double)oRecord.insideTemp * 0.1));
            }

            // outside humidity
            //
            if (oRecord.outsideHum < -1)
            {
                lviTmp.SubItems.Add("");
            }
            else
            {
                lviTmp.SubItems.Add(string.Format("{0:0.0}", (double)oRecord.outsideHum * 0.1));
            }

            // inside humidty
            //
            if (oRecord.insideHum < -1)
            {
                lviTmp.SubItems.Add("");
            }
            else
            {
                lviTmp.SubItems.Add(string.Format("{0:0.0}", (double)oRecord.insideHum * 0.1));
            }

            // barometer
            //
            if (oRecord.barometer == 0)
            {
                lviTmp.SubItems.Add("");
            }
            else
            {
                lviTmp.SubItems.Add(string.Format("{0:0.000}", (double)oRecord.barometer * 0.001));
            }

            // rain
            //
            rainCollecterType = oRecord.rain & 0xF000;
            rainClicks = oRecord.rain & 0x0FFF;
            if (rainCollecterType == 0x0000)
            {
                rainFactor = 0.1;
            }
            else if (rainCollecterType == 0x1000)
            {
                rainFactor = 0.01;
            }
            else if (rainCollecterType == 0x2000)
            {
                rainFactor = 0.2 / 25.4;        // mm => inches
            }
            else if (rainCollecterType == 0x3000)
            {
                rainFactor = 1.0 / 25.4;        // mm => inches
            }
            else if (rainCollecterType == 0x6000)
            {
                rainFactor = 0.1 / 25.4;        // mm => inches
            }
            else
            {
                throw new Exception("invalid value for rain 0x" + oRecord.rain.ToString("X4"));
            }
            rainVal = (double)rainClicks * rainFactor;
            lviTmp.SubItems.Add(string.Format("{0:0.00}", rainVal));

            // rain rate
            //
            rainVal = (double)oRecord.hiRainRate * rainFactor;
            lviTmp.SubItems.Add(string.Format("{0:0.00}", rainVal));

            // wind speed
            //
            lviTmp.SubItems.Add(string.Format("{0:0.0}", (double)oRecord.windSpeed * 0.1));

            // wind direction
            //
            iTmp = oRecord.windDirection;
            if (iTmp == 255)
            {
                lviTmp.SubItems.Add("");
            }
            else
            {
                lviTmp.SubItems.Add(string.Format("{0:0}", iTmp * 24));
            }

            // gust
            //
            lviTmp.SubItems.Add(string.Format("{0:0.0}", (double)oRecord.hiWindSpeed * 0.1));

            // Gust direction
            //
            iTmp = oRecord.hiWindDirection;
            if (iTmp == 255)
            {
                lviTmp.SubItems.Add("");
            }
            else
            {
                lviTmp.SubItems.Add(string.Format("{0:0}", iTmp * 24));
            }

            return lviTmp;

        } // end of DisplayLine

        //----------------------------------------------------
        // UpdateList - update listview control
        //----------------------------------------------------
        private void UpdateList()
        {
            //*******************
            //  populate the list view box
            //
            lvRecords.ListViewItemSorter = null;            // turn off sorter while adding items!

            lvRecords.Items.Clear();
            lvRecords.BeginUpdate();
            foreach (clsWeatherData oR in lstRecords)
            {
                lvRecords.Items.Add(DisplayLine(oR));       // add this item
            }
            lvRecords.EndUpdate();
            lvRecords.ListViewItemSorter = lvwColumnSorter;     // now turn it back on

        } // end of update list

        //----------------------------------------------------
        // ColumnClick
        //----------------------------------------------------
        private void lvRecords_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lvRecords.Sort();

        } // end of ColumnClick handler

        //----------------------------------------------------
        // Edit the selected record(s)
        //----------------------------------------------------
        private void selectedRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool blnChanged;

            // sanity check
            //
            if (lvRecords.SelectedIndices.Count == 0)
            {
                return;
            }

            // walk through all selected records
            //
            blnChanged = false;
            foreach( int iSel in lvRecords.SelectedIndices)
            {
                // startup dialog
                //
                dlgRecord dlgR = new dlgRecord(lstRecords[iSel], lvRecords.Items[iSel]);
                if ( dlgR.ShowDialog() == DialogResult.OK )
                {
                    blnChanged = true;
                }

            } // end of for loop through all selected records

            // if anything changed, update the list
            if (blnChanged)
            {
                UpdateList();
            }

        } // end of Edit Record

        //----------------------------------------------------
        // Edit All Records to CSV format
        //----------------------------------------------------
        private void toCSVFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fpCSV;
            StringBuilder sbTmp;

            // sanity check
            //
            if (lvRecords.Items.Count == 0)
            {
                MessageBox.Show("No records to export.");
                return;
            }

            // filename = YYYY-MM.csv
            //
            fpCSV = Path.ChangeExtension(fpWLK,"csv");

            // start writing
            //
            toolStripStatusLabel1.Text = "Writing data to CSV...";
            try
            {
                using (StreamWriter sw = new StreamWriter(fpCSV))
                {
                    // write header
                    //
                    sw.WriteLine("Time,TempOut,TempIn,HumOut,HumIn,Baro,Rain,Rate,Wind,WindDir,Gust,GustDir");

                    // write data lines
                    //
                    foreach( ListViewItem lvi in lvRecords.Items)
                    {
                        sbTmp = new StringBuilder(lvi.SubItems[0].Text);
                        for(int j = 1; j< 12; j++)
                        {
                            sbTmp.Append(string.Format(",{0}", lvi.SubItems[j].Text));
                        }
                        sw.WriteLine(sbTmp.ToString());
                    }

                } // end of using streamwrite
            }
            catch ( Exception ex)
            {
                MessageBox.Show("Error exporting CSV <" + ex.Message + ">");
                return;
            }

            // All done
            //
            toolStripStatusLabel1.Text = "CSV file written.";

        }  // end of Export CSV

    } // end of form class
}
