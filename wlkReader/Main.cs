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
using System.Threading;
using System.Globalization;

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

        // settings
        //
        clsSettings gSettings;

        // misc
        //
        private ListViewColumnSorter lvwColumnSorter;
        private string strHeaderLine =  "Time,TempOut,TempIn,HumOut,HumIn,Baro,Rain,Rate,Wind,WindDir,Gust,GustDir," +
                                        "Rad,hiRad,UV,hiUV,LTemp0,LTemp1,LTemp2,LTemp3,xRad,new0,new1,new2,new3,new4,new5," +
                                        "forecast,ET,STemp0,STemp1,STemp2,STemp3,STemp4,STemp5,SMoist0,SMoist1,SMoist2," +
                                        "SMoist3,SMoist4,SMoist5,LWet0,LWet1,LWet2,LWet3,XTemp0,XTemp1,XTemp2,XTemp3," +
                                        "XTemp4,XTemp5,XTemp6,XHum0,XHum1,XHum2,XHum3,XHum4,XHum5,XHum6";
        private int lvFieldCount = 59;

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
            gSettings = new clsSettings();

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
            lvRecords.Columns.Add("Rad");           // 12
            lvRecords.Columns.Add("hiRad");         // 13
            lvRecords.Columns.Add("UV");            // 14
            lvRecords.Columns.Add("hiUV");          // 15
            lvRecords.Columns.Add("LTemp0");            // 16
            lvRecords.Columns.Add("LTemp1");            // 17
            lvRecords.Columns.Add("LTemp2");            // 18
            lvRecords.Columns.Add("LTemp3");            // 19
            lvRecords.Columns.Add("xRad");            // 20
            lvRecords.Columns.Add("new0");            // 22
            lvRecords.Columns.Add("new1");            // 23
            lvRecords.Columns.Add("new2");            // 24
            lvRecords.Columns.Add("new3");            // 25
            lvRecords.Columns.Add("new4");            // 26
            lvRecords.Columns.Add("new5");            // 27
            lvRecords.Columns.Add("forecast");            // 28
            lvRecords.Columns.Add("ET");            // 29
            lvRecords.Columns.Add("STemp0");            // 30
            lvRecords.Columns.Add("STemp1");            // 31
            lvRecords.Columns.Add("STemp2");            // 32
            lvRecords.Columns.Add("STemp3");            // 33
            lvRecords.Columns.Add("STemp4");            // 34
            lvRecords.Columns.Add("STemp5");            // 35
            lvRecords.Columns.Add("SMoist0");            // 36
            lvRecords.Columns.Add("SMoist1");            // 37
            lvRecords.Columns.Add("SMoist2");            // 38
            lvRecords.Columns.Add("SMoist3");            // 39
            lvRecords.Columns.Add("SMoist4");            // 40
            lvRecords.Columns.Add("SMoist5");            // 41
            lvRecords.Columns.Add("LWet0");            // 42
            lvRecords.Columns.Add("LWet1");            // 43
            lvRecords.Columns.Add("LWet2");            // 44
            lvRecords.Columns.Add("LWet3");            // 45
            lvRecords.Columns.Add("XTemp0");            // 46
            lvRecords.Columns.Add("XTemp1");            // 47
            lvRecords.Columns.Add("XTemp2");            // 48
            lvRecords.Columns.Add("XTemp3");            // 49
            lvRecords.Columns.Add("XTemp4");            // 50
            lvRecords.Columns.Add("XTemp5");            // 51
            lvRecords.Columns.Add("XTemp6");            // 52
            lvRecords.Columns.Add("XHum0");            // 53
            lvRecords.Columns.Add("XHum1");            // 54
            lvRecords.Columns.Add("XHum2");            // 55
            lvRecords.Columns.Add("XHum3");            // 56
            lvRecords.Columns.Add("XHum4");            // 57
            lvRecords.Columns.Add("XHum5");            // 58
            lvRecords.Columns.Add("XHum6");            // 59


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
            double dTmp;

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
                lviTmp.SubItems.Add(string.Format("{0:0.0}", iTmp * 22.5));
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
                lviTmp.SubItems.Add(string.Format("{0:0.0}", iTmp * 22.5));
            }


            // Solar Radiation
            //
            iTmp = oRecord.solarRad;
            lviTmp.SubItems.Add(string.Format("{0:0}", iTmp));

            // Hi Solar Radiation
            //
            iTmp = oRecord.hisolarRad;
            lviTmp.SubItems.Add(string.Format("{0:0}", iTmp));

            // UV
            //
            dTmp = (double)oRecord.UV * 0.1;
            lviTmp.SubItems.Add(string.Format("{0:0.000}", dTmp));

            // hiUV
            //
            dTmp = (double)oRecord.hiUV * 0.1;
            lviTmp.SubItems.Add(string.Format("{0:0.000}", dTmp));

            // leaf Temp
            //      value of 255 => no value
            //
            for ( int i=0; i< 4; i++)
            {
                iTmp = (int)oRecord.leafTemp[i];
                if (iTmp == 255)
                {
                    lviTmp.SubItems.Add("");
                }
                else
                {
                    lviTmp.SubItems.Add(string.Format("{0:0}", iTmp-90));
                }
            }

            // extra Rad
            //
            iTmp = oRecord.extraRad;
            lviTmp.SubItems.Add(string.Format("{0:0}", iTmp));

            // new Sensors
            //      NOTE:
            //          0x8000 = -32768 => no valid data present
            //
            for (int i = 0; i < 6; i++)
            {
                iTmp = (int)oRecord.newSensors[i];
                if (iTmp == -32768)
                {
                    lviTmp.SubItems.Add("");
                }
                else
                {
                    lviTmp.SubItems.Add(string.Format("{0:0}", iTmp));
                }
            }

            // forecast code
            //
            iTmp = oRecord.forecast;
            lviTmp.SubItems.Add(string.Format("{0:0}", iTmp));

            // ET
            //
            dTmp = (double)oRecord.ET * 0.001;
            lviTmp.SubItems.Add(string.Format("{0:0.000}", dTmp));

            // soil temp
            //
            for (int i = 0; i < 6; i++)
            {
                iTmp = (int)oRecord.soilTemp[i];
                if (iTmp == 255)
                {
                    lviTmp.SubItems.Add("");
                }
                else
                {
                    lviTmp.SubItems.Add(string.Format("{0:0}", iTmp - 90));
                }
            }

            // soil Moisture
            //
            for (int i = 0; i < 6; i++)
            {
                iTmp = (int)oRecord.soilMoisture[i];
                if (iTmp == 255)
                {
                    lviTmp.SubItems.Add("");
                }
                else
                {
                    lviTmp.SubItems.Add(string.Format("{0:0}", iTmp));
                }
            }

            // leaf Wetness
            //
            for (int i = 0; i < 4; i++)
            {
                iTmp = (int)oRecord.leafWetness[i];
                if (iTmp == 255)
                {
                    lviTmp.SubItems.Add("");
                }
                else
                {
                    lviTmp.SubItems.Add(string.Format("{0:0}", iTmp));
                }
            }

            // extraTemp
            //
            for (int i = 0; i < 7; i++)
            {
                iTmp = (int)oRecord.extraTemp[i];
                if (iTmp == 255)
                {
                    lviTmp.SubItems.Add("");
                }
                else
                {
                    lviTmp.SubItems.Add(string.Format("{0:0}", iTmp - 90));
                }
            }

            // extraHum
            //
            for (int i = 0; i < 7; i++)
            {
                iTmp = (int)oRecord.extraHum[i];
                if (iTmp == 255)
                {
                    lviTmp.SubItems.Add("");
                }
                else
                {
                    lviTmp.SubItems.Add(string.Format("{0:0}", iTmp));
                }
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
        // Export All Records to CSV format
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
                    //  and update field delimiter
                    //
                    sw.WriteLine(strHeaderLine.Replace(',',gSettings.cDelimiter));

                    // write data lines
                    //
                    foreach( ListViewItem lvi in lvRecords.Items)
                    {
                        sbTmp = new StringBuilder(lvi.SubItems[0].Text);
                        for(int j = 1; j< lvFieldCount; j++)
                        {
                            sbTmp.Append(string.Format("{0}{1}",gSettings.cDelimiter, lvi.SubItems[j].Text));
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

        //----------------------------------------------------------------
        //  CSV Export settings dialog
        //----------------------------------------------------------------
        private void cSVExportSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgCSVsettings dlgCSV = new dlgCSVsettings(gSettings);
            dlgCSV.ShowDialog();

        }
    } // end of form class
}
