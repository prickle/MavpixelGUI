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
    public partial class frmPalette : Form
    {
        bool isReady = false;
        public static frmPalette Instance;
        public int Timeout { get { return comms.Timeout; } set { comms.Timeout = value; } }
        Form1 form1;
        string Status { get { return form1.Status; } set { form1.Status = value; } }
        serialControl serial;
        //SerialPort port { get { return serial.Port; } set { serial.Port = value; } }
        Button[] colorButtons;
        bool[] solidColorStates;
        List<ColorStore> storedColors;
        bool isEnabled = false;
        ToolStripProgressBar prgBar;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                pnlColors.Enabled = value;
                pnlBase.Enabled = value;
                if (!value)
                {
                    Clear();
                    clearColorButtons();
                    comms.Cancel();
                }
                Invalidate();
            }
        }

        public bool IsReady
        {
            get { return isEnabled; }
            set { isReady = value; }
        }
/*
        Color[] solidColors = {
            Color.FromArgb(255, 0, 0, 0),
            Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 255, 128, 0),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 128, 255, 0),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 255, 128),
            Color.FromArgb(255, 0, 255, 255),
            Color.FromArgb(255, 0, 128, 255),
            Color.FromArgb(255, 0, 0, 255),
            Color.FromArgb(255, 128, 0, 255),
            Color.FromArgb(255, 255, 0, 255),
            Color.FromArgb(255, 255, 0, 128),
            Color.FromArgb(255, 0, 0, 0),
            Color.FromArgb(255, 0, 0, 0)};
*/
        HsvColor[] defaultHsv = {
            new HsvColor(0, 0, 0),
            new HsvColor(0, 255, 255),
            new HsvColor(0, 0, 255),
            new HsvColor(30, 0, 255),
            new HsvColor(60, 0, 255),
            new HsvColor(90, 0, 255),
            new HsvColor(120, 0, 255),
            new HsvColor(150, 0, 255),
            new HsvColor(180, 0, 255),
            new HsvColor(210, 0, 255),
            new HsvColor(240, 0, 255),
            new HsvColor(270, 0, 255),
            new HsvColor(300, 0, 255),
            new HsvColor(330, 0, 255),
            new HsvColor(0, 0, 0),
            new HsvColor(0, 0, 0)};

        public frmPalette(serialControl serial)
        {
            isReady = false;
            InitializeComponent();
            this.serial = serial;
            solidColorStates = new bool[16];
            colorButtons = new Button[16];
            getColorButtons();
            initColorStore();
            Instance = this;
        }

        private void initColorStore()
        {
            storedColors = new List<ColorStore>();
            for (int i = 0; i < 16; i++) storedColors.Add(new ColorStore());
        }

        private void frmPalette_Load(object sender, EventArgs e)
        {
            clearColorButtons();
            form1 = (Form1)Application.OpenForms["Form1"];
            IsEnabled = (serial.IsOpen);
            prgBar = form1.prgBar;
        }

        public void LoadDefaults()
        {
            for (int i = 0; i < 16; i++)
                if (!storedColors[i].compare(defaultHsv[i]))
                {
                    storedColors[i].set(defaultHsv[i]);
                    storedColors[i].changed = true;
                    form1.Modified = true;
                }
            updateAllButtons();
        }

        public void Save(StreamWriter file)
        {
            if (!isEnabled) return;
            for (int index = 0; index < 16; index++)
            {
                if (storedColors[index].fetched)
                {
                    string command = constructColorCommand(index);
                    file.WriteLine(command);
                }
            }
        }

        private void getColorButtons()
        {
            foreach (Control ctl in pnlColors.Controls)
                if (ctl is Button)
                    if (ctl.Tag != null && (string)ctl.Tag != "")
                        colorButtons[int.Parse((string)ctl.Tag)] = (Button)ctl;
        }

        private void frmPalette_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void Clear()
        {
            initColorStore();
            clearColorButtons();
        }

        public void refreshColors()
        {
            //if (Communicator.CurrentMode == CommMode.MAVLINK)
            //    form1.refreshAll();
            //else 
            if (serial.IsOpen)
            {
                Text = "Color Palette - Reading..";
                comms.GetData();
            }
        }

        bool refreshSettings = false;
        public void refreshAllColors()
        {
            refreshSettings = true;
            refreshColors();
        }

        bool initSettings = false;
        public void initAllColors()
        {
            initSettings = true;
            refreshColors();
        }

        public void WaitReady()
        {
            while (!isReady)
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }

        private void clearSolidColorStates()
        {
            for (int i = 0; i < 16; i++)
                solidColorStates[i] = false;
        }

        private void clearColorButtons()
        {
            clearSolidColorStates();
            updateAllButtons();
        }

        private void updateAllButtons()
        {
            for (int i = 0; i < 16; i++)
                updateButtonState(colorButtons[i], solidColorStates[i], storedColors[i].get());
        }

        void updateButtonState(Button b, bool state, HsvColor hsv)
        {
            Color c = hsv.rgb();
            b.BackColor = c;
            if (backgroundDark(c)) b.ForeColor = Color.White;
            else b.ForeColor = Color.Black;
            if (state)
                b.FlatAppearance.BorderColor = Color.Black;
            else
                b.FlatAppearance.BorderColor = Color.Gray;
        }

        public void updateStoredColors(bool changed)
        {
            foreach (ColorStore color in storedColors)
                color.changed = changed;
        }

        public void updateStoredColors(bool changed, bool fetched)
        {
            foreach (ColorStore color in storedColors)
            {
                color.changed = changed;
                color.fetched = fetched;
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            clearSolidColorStates();
            int i = int.Parse((string)((Button)sender).Tag);
            solidColorStates[i] = true;
            updateAllButtons();
            colorDialog1.Color = colorButtons[i].BackColor;
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                HsvColor hsv = new HsvColor(colorDialog1.Color);
                if (!storedColors[i].compare(hsv))
                {
                    storedColors[i].set(hsv);
                    storedColors[i].changed = true;
                    form1.Modified = true;
                }
                SetButtonColor(colorButtons[i], colorDialog1.Color);
            }
        }


        private void btnDefault_Click(object sender, EventArgs e)
        {
            if (!isEnabled) return;
            for (int i = 0; i < 16; i++)
                if (solidColorStates[i])
                    if (storedColors[i].compare(defaultHsv[i]))
                    {
                        storedColors[i].set(defaultHsv[i]);
                        storedColors[i].changed = true;
                        form1.Modified = true;
                    }
            updateAllButtons();
        }

        private void btnAllDefault_Click(object sender, EventArgs e)
        {
            if (!isEnabled) return;
            LoadDefaults();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            form1.Send();
        }

        public void Send()
        {
            comms.SendCommand();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshColors();
        }

        public void MavlinkStarted()
        {
            Text = "Color Palette - Reading..";
            IsEnabled = false;
            btnSend.Enabled = false;
            clearStore();
        }

        public void MavlinkParameter(int index, float value)
        {
            index -= Form1.MAV_COLOR_START;
            byte[] bytes = BitConverter.GetBytes(value);
            storedColors[index] = new ColorStore(bytes[1] * 256 + bytes[0], bytes[2], bytes[3], true);
            updateButtonState(colorButtons[index], solidColorStates[index], storedColors[index].get());
            if (index == 15) MavlinkCompleted();
        }

        public void MavlinkCompleted()
        {
            btnSend.Enabled = true;
            IsEnabled = true;
            //updateAllButtons();
            Text = "Color Palette";
            Application.DoEvents();
        }


        //--------------------------------------------------------------
        // Communications


        private void comms_ComStarted(object sender, ComStartedEventArgs e)
        {
            btnSend.Enabled = false;
            prgBar.Visible = true;
            prgBar.Value = 0;
            if (e.Type == CommType.RECEIVING) IsEnabled = false;
        }

        void comms_ProgressChanged(object sender, ProgressEventArgs e)
        {
            if (e.Event != "") Status = e.Event;
            prgBar.ValueFast(e.Progress);
        }

        void comms_Completed(object sender, CompletedEventArgs e)
        {
            btnSend.Enabled = true;
            prgBar.Visible = false;
            prgBar.Value = 0;
            if (e.Type == CommType.SENDING)
            {
                if (e.Success) updateStoredColors(false);
                if (frmPixSettings.Instance != null) frmPixSettings.Instance.Send();
            }
            else if (e.Type == CommType.RECEIVING && e.Success)
            {
                //Trigger the next collection step
                if (refreshSettings && frmPixSettings.Instance != null)
                {
                    frmPixSettings.Instance.refreshSettings();
                    refreshSettings = false;
                }

                //Trigger the next initialisation step
                if (initSettings && frmPixSettings.Instance != null && !frmPixSettings.Instance.IsReady)
                {
                    frmPixSettings.Instance.refreshSettings();
                    initSettings = false;
                }

            }
            isReady = true;


        }

        //--------------------------------------------------------------
        // Command parsers and generators

        string comms_ReadCommand(object sender, CommandEventArgs e)
        {
            return "color";
        }

        string comms_WriteCommand(object sender, CommandEventArgs e)
        {
            if (storedColors[e.Index].changed)
                return constructColorCommand(e.Index);
            else return "";
        }

        private string constructColorCommand(int index)
        {
            //int h, s, v;
            HsvColor hsv = storedColors[index].get();
            return "color " + index.ToString() + " " + hsv.hue.ToString() + "," + hsv.sat.ToString() + "," + hsv.val.ToString();
        }

        private void comms_GotMavlinkParam(object sender, MavParameterEventArgs e)
        {
            MavlinkParameter(e.Param.param_index, e.Param.param_value);
        }

        private MAVLink.mavlink_param_set_t comms_WriteMavlink(object sender, CommandEventArgs e)
        {
            MAVLink.mavlink_param_set_t param = new MAVLink.mavlink_param_set_t();
            if (!storedColors[e.Index].changed) return param;
            param.target_system = comms.sysid;
            param.target_component = comms.compid;
            form1.SetIdString(ref param.param_id, "color_" + e.Index.ToString());
            param.param_type = (byte)MAVLink.MAV_PARAM_TYPE.UINT32;
            ColorStore color = storedColors[e.Index];
            param.param_value = BitConverter.ToSingle(new byte[] {
                (byte)(color.hsv.hue & 0xff),
                (byte)((color.hsv.hue >> 8) & 0xff),
                (byte)(color.hsv.sat),
                (byte)(color.hsv.val)}, 0);
            return param;
        }

        void comms_GotData(object sender, GotDataEventArgs e)
        {
            bool success;
            clearStore();
            if (e.Data.Contains("color "))
            {
                success = Parse(e.Data, false);
                if (success)
                {
                    Status = "Configuration: Got color palette OK.";
                    IsEnabled = true;
                    updateAllButtons();
                    Text = "Color Palette";
                }
                else comms.Retry();
            }
            else
            {
                Status = "Configuration: color palette empty.";
                initColorStore();
                updateAllButtons();
            }
        }

        public void clearStore()
        {
            initColorStore();
        }

        public void UpdateAll()
        {
            updateAllButtons();
        }

        public bool Parse(string data, bool updating)
        {
            string[] colors;
            bool success = true;
            data = data.Replace("\r", "");
            if (data.Contains('\n')) colors = data.Split('\n');
            else colors = new string[] { data };
            for (int count = 0; count < colors.Length; count++)
                if (colors[count].Contains("color ") && colors[count].Contains(","))
                {
                    bool colorOK = false;
                    string[] line = colors[count].Split(' ');
                    int index, h, s, v;
                    if (line.Length == 3 && int.TryParse(line[1], out index))
                    {
                        string[] parms = line[2].Split(',');
                        if (parms.Length == 3)
                            if (int.TryParse(parms[0], out h) &&
                                int.TryParse(parms[1], out s) &&
                                int.TryParse(parms[2], out v))
                                if (h >= 0 && h < 360 && s >= 0 && s < 256 && v >= 0 && v < 256)
                                {
                                    if (updating)
                                    {
                                        HsvColor hsv = new HsvColor(h, s, v);
                                        if (!storedColors[index].compare(hsv))
                                        {
                                            storedColors[index].set(hsv);
                                            storedColors[index].fetched = true;
                                            storedColors[index].changed = true;
                                        }
                                    }
                                    else storedColors[index] = new ColorStore(h, s, v, true);
                                    colorOK = true;
                                }
                    }
                    if (!colorOK) success = false;
                }
            return success;
        }


        //--------------------------------------------------------------
        // Colour utilities

        //Algorithm found here:
        //https://www.splitbrain.org/blog/2008-09/18-calculating_color_contrast_with_php
        static public bool backgroundDark(Color color)
        {
            double L = 0.2126 * Math.Pow((double)color.R / 255, 2.2) +
                        0.7152 * Math.Pow((double)color.G / 255, 2.2) +
                        0.0722 * Math.Pow((double)color.B / 255, 2.2);
            return (L < 0.07);
        }

        static public void SetButtonColor(Button button, Color color)
        {
            button.BackColor = color;
            //Black or white text?
            if (backgroundDark(color))
                button.ForeColor = Color.White;
            else button.ForeColor = Color.Black;
        }


    }
}
