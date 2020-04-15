using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wlkReader
{
    public partial class dlgRecord : Form
    {

        clsWeatherData g_oRecord;
        ListViewItem g_lviRecord;

        public dlgRecord(clsWeatherData oRecord, ListViewItem lviRecord)
        {
            InitializeComponent();

            // save these
            //
            g_oRecord = oRecord;
            g_lviRecord = lviRecord;

            // use date/time as form label
            //
            this.Text = lviRecord.SubItems[0].Text;

            // Init text boxes
            //
            tb_TempOut.Text = lviRecord.SubItems[1].Text;
            tb_TempIn.Text = lviRecord.SubItems[2].Text;
            tb_HumOut.Text = lviRecord.SubItems[3].Text;
            tb_HumIn.Text = lviRecord.SubItems[4].Text;
            tb_Barometer.Text = lviRecord.SubItems[5].Text;
            tb_Rain.Text = lviRecord.SubItems[6].Text;
            tb_RainRate.Text = lviRecord.SubItems[7].Text;
            tb_Wind.Text = lviRecord.SubItems[8].Text;
            tb_WindDir.Text = lviRecord.SubItems[9].Text;
            tb_Gust.Text = lviRecord.SubItems[10].Text;
            tb_GustDir.Text = lviRecord.SubItems[11].Text;

        }  // end of constructor

        //-------------------------------------------------
        // OK => accept changes
        //-------------------------------------------------
        private void cbOK_Click(object sender, EventArgs e)
        {
            bool blnChanged = false;
            int rainCollecterType;
            double rainFactor;
            short rainVal;
            double dblTmp;
            string strTB;
            int lvi;

            //********************
            // look for changes
            //
            //  **** Question: how are empty values denoted in the binary data? e.g no tempOut?
            //

            // TempOut
            //
            strTB = tb_TempOut.Text.Trim();
            lvi = 1;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = -1000;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing TempOut value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                g_oRecord.outsideTemp = (short)(dblTmp * 10);
            }

            // TempIn
            //
            strTB = tb_TempIn.Text.Trim();
            lvi = 2;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = -1000;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing TempIn value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                g_oRecord.insideTemp = (short)(dblTmp * 10);
            }

            // HumOut
            //
            strTB = tb_HumOut.Text.Trim();
            lvi = 3;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = -1;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing HumOut value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                g_oRecord.outsideHum = (short)(dblTmp * 10);
            }

            // HumIn
            //
            strTB = tb_HumIn.Text.Trim();
            lvi = 4;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = -1;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing HumIn value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                g_oRecord.insideHum = (short)(dblTmp * 10);
            }

            // Barometer
            //
            strTB = tb_Barometer.Text.Trim();
            lvi = 5;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = 0;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing Barometer value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                g_oRecord.barometer = (short)(dblTmp * 1000);
            }

            // Rain
            //

            // will use this info for rain rate as well
            //
            rainCollecterType = g_oRecord.rain & 0xF000;
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
                throw new Exception("invalid value for rain collector type");  // not likely, but ...
            }

            strTB = tb_Rain.Text.Trim();
            lvi = 6;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = 0;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing Rain value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                if (dblTmp > 0)
                {
                    dblTmp = dblTmp / rainFactor;       // dblTmp should now be the number of clicks for the given collector type
                }
                rainVal = (short)dblTmp;

                g_oRecord.rain = (ushort)( (int)rainVal & rainCollecterType);
            }

            // rain rate
            //
            strTB = tb_RainRate.Text.Trim();
            lvi = 7;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = 0;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing RainRate value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                if (dblTmp > 0)
                {
                    dblTmp = dblTmp / rainFactor;       // dblTmp should now be the number of clicks for the given collector type
                }
                rainVal = (short)dblTmp;

                g_oRecord.hiRainRate = rainVal;
            }

            // Wind
            //
            strTB = tb_Wind.Text.Trim();
            lvi = 8;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = -1;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing Wind value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                g_oRecord.windSpeed = (short)(dblTmp * 10);
            }

            // Wind Direction
            //
            strTB = tb_WindDir.Text.Trim();
            lvi = 9;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    g_oRecord.windDirection = 0xFF;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing WindDir value.");
                        return;
                    }
                    g_lviRecord.SubItems[lvi].Text = strTB;
                    g_oRecord.windDirection = (byte)(dblTmp / 15);
                }
            }

            // Gust
            //
            strTB = tb_Gust.Text.Trim();
            lvi = 10;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    dblTmp = -1;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing Gust value.");
                        return;
                    }
                }
                g_lviRecord.SubItems[lvi].Text = strTB;
                g_oRecord.hiWindSpeed = (short)(dblTmp * 10);
            }

            // Wind Direction
            //
            strTB = tb_GustDir.Text.Trim();
            lvi = 11;
            if (strTB != g_lviRecord.SubItems[lvi].Text.Trim())
            {
                // something changed
                //
                blnChanged = true;

                // was it cleared?
                //
                if (strTB == string.Empty)
                {
                    g_oRecord.windDirection = 0xFF;      // flag it as no value
                }
                else
                {
                    if (!double.TryParse(strTB, out dblTmp))
                    {
                        MessageBox.Show("error parsing GustDir value.");
                        return;
                    }
                    g_lviRecord.SubItems[lvi].Text = strTB;
                    g_oRecord.hiWindDirection = (byte)(dblTmp / 15);
                }
            }

            //******************
            // all Done
            //
            if (blnChanged)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }

            this.Close();

        }       // end of OK /accept

        //-------------------------------------------------
        // Cancel => reject changes
        //-------------------------------------------------
        private void cbCancel_Click(object sender, EventArgs e)
        {
            // do nothing
            //
            this.DialogResult = DialogResult.Cancel;
            this.Close();

        } // end of Cancel changes
    }
}
