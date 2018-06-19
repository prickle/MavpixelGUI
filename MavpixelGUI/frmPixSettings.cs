/*
 * MavpixelGUI - Front end for Mavpixel Mavlink Neopixel bridge
 * (c) 2016 Nick Metcalfe
 *
 * Mavpixel is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Please read licensing, redistribution, modifying, authors and 
 * version numbering from main sketch file. This file contains only
 * a minimal header.
 *
 * MavpixelGUI is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Mavpixel.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenNETCF.IO.Ports;
using System.IO;
using System.Threading;

namespace MavpixelGUI
{
    public partial class frmPixSettings : Form
    {
        bool isEnabled = false;
        bool isReady = false;
        public static frmPixSettings Instance;
        serialControl serial;
        //SerialPort port { get { return serial.Port; } set { serial.Port = value; } }
        Form1 form1;
        ToolStripProgressBar prgBar;
        string Status{ get { return form1.Status; } set { form1.Status = value; } }
        ParamStore storedParams;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                pnlMain.Enabled = value;
                pnlBase.Enabled = value;
                if (!value)
                {
                    comms.Cancel();
                    Clear();
                }
                Invalidate();
            }
        }

        public bool IsReady
        {
            get { return isEnabled; }
            set { isReady = value; }
        }
            
        string[] paramList = {
             "version",
             "sysid",
             "heartbeat",
             "brightness",
             "animation",
             "lowcell",
             "lowpct",
             "minsats",
             "deadband",
             "baud",
             "softbaud"};

        const int VERSION = 0;
        const int SYSID = 1;
        const int HEARTBEAT = 2;
        const int BRIGHTNESS = 3;
        const int ANIMATION = 4;
        const int LOWCELL = 5;
        const int LOWPCT = 6;
        const int MINSATS = 7;
        const int DEADBAND = 8;
        const int BAUD = 9;
        const int SOFTBAUD = 10;
        const int PARAM_COUNT = 11;

        string paramNotAvailable = "Not available";
        
        //Culture settings to fix decimal problems in OS languages different from english
        System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowCurrencySymbol;
        System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        bool Modified { set { form1.Modified = value; } }


        //--------------------------------------------------------------
        // Constructor and setup

        public frmPixSettings(serialControl serial)
        {
            isReady = false;
            InitializeComponent();
            this.serial = serial;
            Instance = this;
        }

        private void initParamStore()
        {
            storedParams = new ParamStore(PARAM_COUNT);
        }

        private void frmPixSettings_Load(object sender, EventArgs e)
        {
            initParamStore();
            form1 = (Form1)Application.OpenForms["Form1"];
            prgBar = form1.prgBar;
            IsEnabled = (serial.IsOpen);
        }

        private void clearSettings()
        {
            updateTitle(-1);
            holdEvents = true;
            txtSysid.Text = "";
            chkHeartBeat.Checked = false;
            trkBright.Value = 0;
            chkAnim.Checked = false;
            txtLowcell.Text = "";
            txtLowpct.Text = "";
            txtMinsats.Text = "";
            txtDeadband.Text = "";
            cbxMavlink.SelectedIndex = -1;
            cbxSoftserial.SelectedIndex = -1;
            holdEvents = false;
        }

        private void frmPixSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            //Restore current settings
            for (int setting = 0; setting < PARAM_COUNT; setting++)
                loadSetting(setting);
        }

        //--------------------------------------------------------------
        // Utility functions

        public void Clear()
        {
            clearSettings();
            initParamStore();
        }

        public void refreshSettings()
        {
            //if (Communicator.CurrentMode == CommMode.MAVLINK)
            //    form1.refreshAll();
            //else 
            if (serial.IsOpen)
            {
                updateTitle(true);
                comms.GetData();
            }
        }

        public void LoadDefaults()
        {
            txtSysid.Text = "2";
            chkHeartBeat.Checked = false;
            trkBright.Value = 10;
            chkAnim.Checked = false;
            txtLowcell.Text = "3.3";
            txtLowpct.Text = "20";
            txtMinsats.Text = "6";
            txtDeadband.Text = "40";
            cbxMavlink.SelectedIndex = 5;
            cbxSoftserial.SelectedIndex = 0;
        }

        public void Save(StreamWriter file)
        {
            if (!isEnabled) return;
            for (int index = 0; index < PARAM_COUNT; index++)
            {
                string command = constructSettingsCommand(index);
                if (index != SOFTBAUD || getBaud(cbxSoftserial.SelectedIndex, SOFTBAUD) != 0)
                    file.WriteLine(command);
            }
        }

        public void WaitReady()
        {
            while (!isReady)
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }

        public void updateStoredSettings(bool changed)
        {
            storedParams.update(changed);
        }

        public void updateStoredSettings(bool changed, bool fetched)
        {
            storedParams.update(changed, fetched);
        }

        public static Version MavpixelVersion;

        string mavpixelVersion = "";
        bool settingsReading = false;
        private void updateTitle(float version)
        {
            if (version <= 0)
            {
                mavpixelVersion = "";
                MavpixelVersion = null;
            }
            else
            {
                mavpixelVersion = version.ToString(culture);
                MavpixelVersion = new Version(mavpixelVersion);
            }
            updateTitle();
        }

        private void updateTitle(bool reading)
        {
            settingsReading = reading;
            updateTitle();
        }

        private void updateTitle()
        {
            if (mavpixelVersion == "")
                Text = "Mavpixel Settings";
            else
                Text = "Mavpixel v" + mavpixelVersion + " Settings";
            if (settingsReading) Text += " - Reading..";
        }

        bool holdEvents = false;
        private void loadSetting(int fetchingParam)
        {
            holdEvents = true;
            if (fetchingParam == VERSION) //Version
            {
                updateTitle(storedParams.get(VERSION));
            }
            if (fetchingParam == SYSID) //Sysid
            {
                txtSysid.Text = ((int)storedParams.get(SYSID)).ToString();
            }
            if (fetchingParam == HEARTBEAT) //Heartbeat
            {
                chkHeartBeat.Checked = (storedParams.get(HEARTBEAT) > 0);
            }
            if (fetchingParam == BRIGHTNESS) //Brightness
            {
                trkBright.Value = (int)storedParams.get(BRIGHTNESS);
            }
            if (fetchingParam == ANIMATION) //Animation
            {
                chkAnim.Checked = (storedParams.get(ANIMATION) > 0);
            }
            if (fetchingParam == LOWCELL) //Lowcell
            {
                txtLowcell.Text = storedParams.get(LOWCELL).ToString();
            }
            if (fetchingParam == LOWPCT) //Lowpct
            {
                txtLowpct.Text = ((int)storedParams.get(LOWPCT)).ToString();
            }
            if (fetchingParam == MINSATS) //Minsats
            {
                txtMinsats.Text = ((int)storedParams.get(MINSATS)).ToString();
            }
            if (fetchingParam == DEADBAND) //Deadband
            {
                txtDeadband.Text = ((int)storedParams.get(DEADBAND)).ToString();
            }
            if (fetchingParam == BAUD) //Baud
            {
                cbxMavlink.SelectedIndex = getBaudIndex((int)storedParams.get(BAUD));
            }
            if (fetchingParam == SOFTBAUD) //Softbaud
            {
                loadSoftBaud();
            }
            holdEvents = false;
        }

        public void MavlinkStarted()
        {
            updateTitle(true);
            IsEnabled = false;
            btnSend.Enabled = false;
        }

        public void MavlinkParameter(int index, float value)
        {
            holdEvents = true;
            if (index == Form1.MAV_VERSION) //Version
            {
                storedParams.load(VERSION, value);
                updateTitle(value);
            }
            if (index == Form1.MAV_SYSID) //Sysid
            {
                storedParams.load(SYSID, value);
                txtSysid.Text = ((int)value).ToString();
            }
            if (index == Form1.MAV_HEARTBEAT) //Heartbeat
            {
                storedParams.load(HEARTBEAT, value);
                chkHeartBeat.Checked = (value > 0);
            }
            if (index == Form1.MAV_BRIGHTNESS) //Brightness
            {
                storedParams.load(BRIGHTNESS, value);
                trkBright.Value = (int)value;
            }
            if (index == Form1.MAV_ANIMATION) //Animation
            {
                storedParams.load(ANIMATION, value);
                chkAnim.Checked = (value > 0);
            }
            if (index == Form1.MAV_LOWBATT) //Lowcell
            {
                storedParams.load(LOWCELL, value);
                txtLowcell.Text = value.ToString();
            }
            if (index == Form1.MAV_LOWPCT) //Lowpct
            {
                storedParams.load(LOWPCT, value);
                txtLowpct.Text = ((int)value).ToString();
            }
            if (index == Form1.MAV_MINSATS) //Minsats
            {
                storedParams.load(MINSATS, value);
                txtMinsats.Text = ((int)value).ToString();
            }
            if (index == Form1.MAV_DEADBAND) //Deadband
            {
                storedParams.load(DEADBAND, value);
                txtDeadband.Text = ((int)value).ToString();
            }
            if (index == Form1.MAV_BAUD) //Baud
            {
                storedParams.load(BAUD, value);
                cbxMavlink.SelectedIndex = getBaudIndex((int)value);
            }
            if (index == Form1.MAV_SOFTBAUD) //Softbaud
            {
                storedParams.load(SOFTBAUD, value);
                loadSoftBaud();
                MavlinkCompleted();
            }
            holdEvents = false;
        }

        public void MavlinkCompleted()
        {
            btnSend.Enabled = true;
            updateTitle(false);
            IsEnabled = true;
            Application.DoEvents();
        }
        
        public void UpdateSoftBaud()
        {
            holdEvents = true;
            loadSoftBaud();
            holdEvents = false;
        }

        private void loadSoftBaud()
        {
            int index = getBaudIndex((int)storedParams.get(SOFTBAUD));
            cbxSoftserial.Enabled = (index != -1);
            if (index == -1)
            {
                if (!cbxSoftserial.Items.Contains(paramNotAvailable))
                    cbxSoftserial.Items.Add(paramNotAvailable);
                cbxSoftserial.SelectedItem = paramNotAvailable;
            }
            else
            {
                if (cbxSoftserial.Items.Contains(paramNotAvailable))
                    cbxSoftserial.Items.Remove(paramNotAvailable);
                cbxSoftserial.SelectedIndex = index;
            }
        }

        private int getBaudIndex(int baud)
        {
            if (baud == 2400) return 0;
            if (baud == 4800) return 1;
            if (baud == 9600) return 2;
            if (baud == 19200) return 3;
            if (baud == 38400) return 4;
            if (baud == 57600) return 5;
            if (baud == 115200) return 6;
            return -1;
        }

        int getBaud(int index, int portType)
        {
            if (index == 0) return 2400;
            if (index == 1) return 4800;
            if (index == 2) return 9600;
            if (index == 3) return 19200;
            if (index == 4) return 38400;
            if (portType == SOFTBAUD) return 0;
            if (index == 5) return 57600;
            if (index == 6) return 115200;
            return 0;
        }

        private void updateStoredParams()
        {
            float f;
            int i;
            if (int.TryParse(txtSysid.Text, out i))
                storedParams.load(SYSID, i);
            storedParams.load(HEARTBEAT, chkHeartBeat.Checked ? 1 : 0);
            storedParams.load(BRIGHTNESS, trkBright.Value);
            storedParams.load(ANIMATION, chkAnim.Checked ? 1 : 0);
            if (float.TryParse(txtLowcell.PlainText, out f))
                storedParams.load(LOWCELL, f);
            if (int.TryParse(txtLowpct.PlainText, out i))
                storedParams.load(LOWPCT, i);
            if (int.TryParse(txtMinsats.Text, out i))
                storedParams.load(MINSATS, i);
            if (int.TryParse(txtDeadband.PlainText, out i))
                storedParams.load(DEADBAND, i);
            storedParams.load(BAUD, getBaud(cbxMavlink.SelectedIndex, BAUD));
            storedParams.load(SOFTBAUD, getBaud(cbxSoftserial.SelectedIndex, SOFTBAUD));
        }

        //--------------------------------------------------------------
        // Control events

        private void txtSysid_TextChanged(object sender, EventArgs e)
        {
            int val;
            if (holdEvents) return;
            if (int.TryParse(txtSysid.Text, out val))
            {
                storedParams.changed[SYSID] = (val != (int)storedParams.get(SYSID));
                Modified = true;
            }
        }

        private void chkHeartBeat_CheckedChanged(object sender, EventArgs e)
        {
            if (holdEvents) return;
            storedParams.changed[HEARTBEAT] = (chkHeartBeat.Checked != (int)storedParams.get(HEARTBEAT) > 0);
            Modified = true;
        }

        private void trkBright_Scroll(object sender, EventArgs e)
        {
            if (holdEvents) return;
            storedParams.changed[BRIGHTNESS] = (trkBright.Value != (int)storedParams.get(BRIGHTNESS));
            Modified = true;
        }

        private void chkAnim_CheckedChanged(object sender, EventArgs e)
        {
            if (holdEvents) return;
            storedParams.changed[ANIMATION] = (chkAnim.Checked != (int)storedParams.get(ANIMATION) > 0);
            Modified = true;
        }

        private void txtLowcell_TextChanged(object sender, EventArgs e)
        {
            float val;
            if (holdEvents) return;
            if (float.TryParse(txtLowcell.Text.Replace("v", ""), out val))
            {
                storedParams.changed[LOWCELL] = (val != storedParams.get(LOWCELL));
                Modified = true;
            }
        }

        private void txtLowpct_TextChanged(object sender, EventArgs e)
        {
            int val;
            if (holdEvents) return;
            if (int.TryParse(txtLowpct.Text.Replace("%", ""), out val))
            {
                storedParams.changed[LOWPCT] = (val != (int)storedParams.get(LOWPCT));
                Modified = true;
            }
        }

        private void txtMinsats_TextChanged(object sender, EventArgs e)
        {
            int val;
            if (holdEvents) return;
            if (int.TryParse(txtMinsats.Text, out val))
            {
                storedParams.changed[MINSATS] = (val != (int)storedParams.get(MINSATS));
                Modified = true;
            }
        }

        private void txtDeadband_TextChanged(object sender, EventArgs e)
        {
            int val;
            if (holdEvents) return;
            if (int.TryParse(txtDeadband.Text, out val))
            {
                storedParams.changed[DEADBAND] = (val != (int)storedParams.get(DEADBAND));
                Modified = true;
            }
        }

        private void cbxMavlink_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (holdEvents) return;
            storedParams.changed[BAUD] = (getBaud(cbxMavlink.SelectedIndex, BAUD) != storedParams.get(BAUD));
            Modified = true;
        }

        private void cbxSoftserial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (holdEvents) return;
            storedParams.changed[SOFTBAUD] = (getBaud(cbxSoftserial.SelectedIndex, SOFTBAUD) != storedParams.get(SOFTBAUD));
            Modified = true;
        }

        private void btnFactory_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                @"Are you sure?

Saying yes will reset ALL settings, LEDS, modes and colors to default.
This will take effect on next reboot. Please reset Mavpixel to finish.

Remember - Baud rates are also reset to default 2400 and 57600.",
                "Warning - Full Factory Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                if (Communicator.CurrentMode == CommMode.CLI)
                    comms.SendCommand("factory");
                else if (Communicator.CurrentMode == CommMode.MAVLINK)
                    comms.SendParameter("factory", 1);
                Modified = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!isEnabled) return;
            form1.Send();
        }

        public void Send()
        {
            comms.SendCommand();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshSettings();
        }

        private void allDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDefaults();
        }

        //--------------------------------------------------------------
        // Communications

        private void comms_ComStarted(object sender, ComStartedEventArgs e)
        {
            btnSend.Enabled = false;
            prgBar.Value = 0;
            prgBar.Visible = true;
            if (e.Type == CommType.RECEIVING) IsEnabled = false;
        }

        private void comms_ProgressChanged(object sender, ProgressEventArgs e)
        {
            if (e.Event != "") Status = e.Event;
            prgBar.ValueFast(e.Progress);
        }

        private void comms_Completed(object sender, CompletedEventArgs e)
        {
            btnSend.Enabled = true;
            prgBar.Visible = false;
            updateTitle(false);
            if (e.Type == CommType.SENDING)
            {
                if (e.Success || e.LastIndex == BAUD || e.LastIndex == SOFTBAUD) updateStoredParams();
                if (!e.Success && (e.LastIndex == BAUD || e.LastIndex == SOFTBAUD))
                    Status = "Configuration - Sent OK, but baudrate seems to have changed. Please reconnect at new rate.";
            }
            else if (e.Type == CommType.RECEIVING && e.Success) IsEnabled = true;
            isReady = true;
        }


        //--------------------------------------------------------------
        // Command parsers and generators

        private string comms_ReadCommand(object sender, CommandEventArgs e)
        {
            return paramList[e.Index];
        }

        private string comms_WriteCommand(object sender, CommandEventArgs e)
        {
            if (storedParams.changed[e.Index])
                return constructSettingsCommand(e.Index);
            else return "";
        }

        private void comms_GotMavlinkParam(object sender, MavParameterEventArgs e)
        {
            MavlinkParameter(e.Param.param_index, e.Param.param_value);
        }

        private MAVLink.mavlink_param_set_t comms_WriteMavlink(object sender, CommandEventArgs e)
        {
            float f;
            int i;
            MAVLink.mavlink_param_set_t param = new MAVLink.mavlink_param_set_t();
            if (!storedParams.changed[e.Index]) return param;
            param.target_system = comms.sysid;
            param.target_component = comms.compid;
            form1.SetIdString(ref param.param_id, paramList[e.Index]);
            param.param_type = (byte)MAVLink.MAV_PARAM_TYPE.REAL32;
            if (e.Index == SYSID && int.TryParse(txtSysid.Text, out i))
                param.param_value = i;
            else if (e.Index == HEARTBEAT)
                param.param_value = (chkHeartBeat.Checked ? 1 : 0);
            else if (e.Index == BRIGHTNESS)
                param.param_value = trkBright.Value;
            else if (e.Index == ANIMATION)
                param.param_value = (chkAnim.Checked ? 1 : 0);
            else if (e.Index == LOWCELL && float.TryParse(txtLowcell.PlainText, out f))
                param.param_value = f;
            else if (e.Index == LOWPCT && int.TryParse(txtLowpct.PlainText, out i))
                param.param_value = i;
            else if (e.Index == MINSATS && int.TryParse(txtMinsats.Text, out i))
                param.param_value = i;
            else if (e.Index == DEADBAND && int.TryParse(txtDeadband.PlainText, out i))
                param.param_value = i;
            else if (e.Index == BAUD)
                param.param_value = getBaud(cbxMavlink.SelectedIndex, BAUD);
            else if (e.Index == SOFTBAUD)
                param.param_value = getBaud(cbxSoftserial.SelectedIndex, SOFTBAUD);
            else
            {
                param.target_system = 0;
                param.target_component = 0;
            }
            //SYSID changed? Need to change our current sysid
            if (e.Index == SYSID)
                form1.ChangeSysid((byte)param.param_value);
            return param;
        }

        private string constructSettingsCommand(int index)
        {
            float f;
            int i;
            if (index == VERSION) return "";
            string command = paramList[index] + " ";
            if (index == SYSID && int.TryParse(txtSysid.Text, out i))
                return command + i.ToString();
            else if (index == HEARTBEAT)
                return command + (chkHeartBeat.Checked ? "YES" : "NO");
            else if (index == BRIGHTNESS)
                return command + trkBright.Value.ToString();
            else if (index == ANIMATION)
                return command + (chkAnim.Checked ? "YES" : "NO");
            else if (index == LOWCELL && float.TryParse(txtLowcell.PlainText, out f))
                return command + f.ToString();
            else if (index == LOWPCT && int.TryParse(txtLowpct.PlainText, out i))
                return command + i.ToString();
            else if (index == MINSATS && int.TryParse(txtMinsats.Text, out i))
                return command + i.ToString();
            else if (index == DEADBAND && int.TryParse(txtDeadband.PlainText, out i))
                return command + i.ToString();
            else if (index == BAUD)
                return command + getBaud(cbxMavlink.SelectedIndex, BAUD).ToString();
            else if (index == SOFTBAUD)
                return command + getBaud(cbxSoftserial.SelectedIndex, SOFTBAUD);
            else return command + "parsing error";
        }

        private void comms_GotData(object sender, GotDataEventArgs e)
        {
            float value;
            bool success = false;
            if (e.Data.Contains("Unknown")) return; //Fail silently
            string txt = e.Data.Split(':')[1].Split(' ')[1].Split('\r')[0];
            txt = txt.Replace("%", "");
            txt = txt.Replace("v", "");
            if (txt.Contains("YES"))
            {
                storedParams.load(e.Index, 1);
                success = true;
            }
            else if (txt.Contains("NO"))
            {
                storedParams.load(e.Index, 0);
                success = true;
            }

            else if (float.TryParse(txt, style, culture, out value))
            {
                storedParams.load(e.Index, value);
                success = true;
            }
            if (success) loadSetting(e.Index);
            else comms.Retry();
        }

        public bool ParseUpdate(string data)
        {
            float value;
            bool success = false;
            data = data.Replace("\n", "");
            string[] line = data.Split(' ');
            int index;
            for (index = 0; index < paramList.Length; index++)
                if (line[0].ToLower() == paramList[index]) break;
            if (index < paramList.Length)
            {
                string txt = line[1].Replace("%", "").Replace("v", "").Replace("\r", "");
                if (txt.Contains("YES"))
                {
                    storedParams.set(index, 1);
                    success = true;
                }
                else if (txt.Contains("NO"))
                {
                    storedParams.set(index, 0);
                    success = true;
                }

                else if (float.TryParse(txt, style, culture, out value))
                {
                    storedParams.set(index, value);
                    success = true;
                }
            }
            if (success) loadSetting(index);
            return success;
        }

    }
}
