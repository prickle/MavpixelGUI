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
    public partial class frmModeColor : Form
    {
        bool isReady = false;
        public static frmModeColor Instance;
        public int Timeout { get { return comms.Timeout; } set { comms.Timeout = value; } }
        bool isEnabled = false;
        serialControl serial;
        //SerialPort port { get { return serial.Port; } set { serial.Port = value; } }
        ToolStripProgressBar prgBar;
        int currentMode = -1;
        bool sendWholeModes = false;  //MCI threshold crossed flag

        bool northState;
        bool southState;
        bool eastState;
        bool westState;
        bool upState;
        bool downState;
        bool[] solidColorStates;

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

        static int
            COLOR_WHITE = 1,
            COLOR_RED = 2,
            COLOR_ORANGE = 3,
            COLOR_YELLOW = 4,
            COLOR_LIME_GREEN = 5,
            COLOR_GREEN = 6,
            COLOR_MINT_GREEN = 7,
            COLOR_CYAN = 8,
            COLOR_LIGHT_BLUE = 9,
            COLOR_BLUE = 10,
            COLOR_DARK_VIOLET = 11,
            COLOR_MAGENTA = 12,
            COLOR_DEEP_PINK = 13;

        //We currently handle 21 different flight modes.
        // Should cope with anything up to APM:copter 3.4 and APM:plane 3.5
        int NUMBER_OF_MODES = 21;

        // Note, the color index used for the mode colors below refer to the default colors.
        int[][] defaultModeColors = new int[][]{
            //Stabilize  
            new int[]{COLOR_GREEN, COLOR_DARK_VIOLET, COLOR_GREEN, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Acro
            new int[]{COLOR_LIME_GREEN, COLOR_DARK_VIOLET, COLOR_GREEN, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Althold
            new int[]{COLOR_BLUE, COLOR_DARK_VIOLET, COLOR_GREEN, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Auto
            new int[]{COLOR_CYAN, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Guided
            new int[]{COLOR_MINT_GREEN, COLOR_DARK_VIOLET, COLOR_ORANGE, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Loiter
            new int[]{COLOR_LIGHT_BLUE, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //RTL
            new int[]{COLOR_ORANGE, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Circle
            new int[]{COLOR_DEEP_PINK, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Land
            new int[]{COLOR_MAGENTA, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Drift
            new int[]{COLOR_DARK_VIOLET, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Sport
            new int[]{COLOR_YELLOW, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Flip
            new int[]{COLOR_RED, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Autotune
            new int[]{COLOR_WHITE, COLOR_ORANGE, COLOR_RED, COLOR_ORANGE, COLOR_BLUE, COLOR_ORANGE},
            //Poshold
            new int[]{COLOR_BLUE, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Brake
            new int[]{COLOR_RED, COLOR_RED, COLOR_RED, COLOR_RED, COLOR_BLUE, COLOR_ORANGE},
            //Throw
            new int[]{COLOR_YELLOW, COLOR_YELLOW, COLOR_YELLOW, COLOR_YELLOW, COLOR_BLUE, COLOR_ORANGE},
            //Manual
            new int[]{COLOR_YELLOW, COLOR_DARK_VIOLET, COLOR_GREEN, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //FBWA
            new int[]{COLOR_BLUE, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //FBWB
            new int[]{COLOR_MAGENTA, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Training
            new int[]{COLOR_RED, COLOR_DARK_VIOLET, COLOR_ORANGE, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
            //Cruise
            new int[]{COLOR_DARK_VIOLET, COLOR_DARK_VIOLET, COLOR_RED, COLOR_DEEP_PINK, COLOR_BLUE, COLOR_ORANGE},
        };

        //How many changed colors beyond which mode_color lists are used instead of single MCIs.
        int MCI_THRESHOLD = 6;

        Button[] colorButtons;

        Form1 form1;
        string Status { get { return form1.Status; } set { form1.Status = value; } }

        List<ModeColorStore> storedModes;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                cbxMode.Enabled = value;
                gbxColor.Enabled = value;
                gbxOrientation.Enabled = value;
                pnlBase.Enabled = value;
                if (!value)
                {
                    comms.Cancel();
                    Clear();
                }
                if (colorButtons != null) updateAllButtons();
                Invalidate();
            }
        }

        bool Modified { set { form1.Modified = value; } }

        public bool IsReady
        {
            get { return isEnabled; }
            set { isReady = value; }
        }

        //--------------------------------------------------------------
        // Constructor and setup

        public frmModeColor(serialControl serial)
        {
            isReady = false;
            InitializeComponent();
            this.serial = serial;
            Instance = this;
        }

        private void initModeStore()
        {
            storedModes = new List<ModeColorStore>();
            for (int i = 0; i < NUMBER_OF_MODES; i++) storedModes.Add(new ModeColorStore());
        }

        private void frmModeColor_Load(object sender, EventArgs e)
        {
            initModeStore();
            solidColorStates = new bool[16];
            colorButtons = new Button[16];
            getColorButtons();
            clearStates();
            form1 = (Form1)Application.OpenForms["Form1"];
            IsEnabled = (serial.IsOpen);
            prgBar = form1.prgBar;
        }

        private void getColorButtons()
        {
            int index;
            foreach (Control ctl in gbxColor.Controls)
                if (ctl is Button)
                    if (ctl.Tag != null && (string)ctl.Tag != "")
                    {
                        index = int.Parse((string)ctl.Tag);
                        colorButtons[index] = (Button)ctl;
                        frmPalette.SetButtonColor(colorButtons[index], getButtonColor(index));
                    }
        }

        private Color getButtonColor(int index)
        {
            if (!isEnabled && solidColors[index].ToArgb() == Color.Black.ToArgb())
                return Color.Gray;
            else return solidColors[index];
        }

        private void frmModeColor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        //--------------------------------------------------------------
        // Utility functions

        public void Clear()
        {
            cbxMode.SelectedIndex = -1;
            currentMode = -1;
            clearDirectionButtons();
            initModeStore();
        }

        bool refreshPalette = false;
        public void refreshAllModes()
        {
            refreshPalette = true;
            refreshModes();
        }


        public void refreshModes()
        {
            //if (Communicator.CurrentMode == CommMode.MAVLINK)
            //    form1.refreshAll();
            //else 
            if (serial.IsOpen)
            {
                Text = "Mode Colors - Reading..";
                comms.GetData();
            }
        }

        bool initPalette = false;
        public void initAllModes()
        {
            initPalette = true;
            refreshModes();
        }

        public void LoadDefaults()
        {
            for (int i = 0; i < NUMBER_OF_MODES; i++)
                storedModes[i].setAll(defaultModeColors[i]);
            updateAllButtons();
        }

        public void Save(StreamWriter file)
        {
            if (!isEnabled) return;
            for (int index = 0; index < NUMBER_OF_MODES; index++)
            {
                if (storedModes[index].fetched)
                {
                    string command = constructModeCommand(index);
                    file.WriteLine(command);
                }
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

        private void clearStates()
        {
            clearDirectionStates();
            clearSolidColorStates();
        }

        private void clearDirectionStates()
        {
            northState = false;
            southState = false;
            eastState = false;
            westState = false;
            upState = false;
            downState = false;
        }

        private void clearSolidColorStates()
        {
            for (int i = 0; i < 16; i++)
                solidColorStates[i] = false;
        }

        private void updateAllButtons()
        {
            updateDirectionButtons();
            for (int i = 0; i < 16; i++)
                updateButtonState(colorButtons[i], solidColorStates[i], i);
        }

        private void updateDirectionButtons()
        {
            if (currentMode == -1) clearDirectionButtons();
            else
            {
                updateButtonState(btnNorth, northState, storedModes[currentMode].get(0));
                updateButtonState(btnEast, eastState, storedModes[currentMode].get(1));
                updateButtonState(btnSouth, southState, storedModes[currentMode].get(2));
                updateButtonState(btnWest, westState, storedModes[currentMode].get(3));
                updateButtonState(btnUp, upState, storedModes[currentMode].get(4));
                updateButtonState(btnDown, downState, storedModes[currentMode].get(5));
            }
        }

        private void clearDirectionButtons()
        {
            clearDirectionStates();
            updateButtonState(btnNorth, northState, -1);
            updateButtonState(btnEast, eastState, -1);
            updateButtonState(btnSouth, southState, -1);
            updateButtonState(btnWest, westState, -1);
            updateButtonState(btnUp, upState, -1);
            updateButtonState(btnDown, downState, -1);
        }

        void updateButtonState(Button b, bool state)
        {
            if (state)
                b.FlatAppearance.BorderColor = Color.Black;
            else
                b.FlatAppearance.BorderColor = Color.Gray;
        }

        void updateButtonState(Button b, bool state, int colorIndex)
        {
            Color c = System.Drawing.SystemColors.Control;
            if (colorIndex >= 0) c = solidColors[colorIndex];
            if (!isEnabled && c.ToArgb() == Color.Black.ToArgb()) c = Color.Gray;
            if (frmPalette.backgroundDark(c)) b.ForeColor = Color.White;
            else b.ForeColor = Color.Black;
            if (state)
            {
                b.FlatAppearance.BorderColor = Color.Black;
                b.BackColor = c;
            }
            else
            {
                b.FlatAppearance.BorderColor = Color.Gray;
                b.BackColor = c;
            }
        }

        public void updateStoredModes(bool changed)
        {
            foreach (ModeColorStore mode in storedModes)
                mode.update(changed);
        }

        public void updateStoredModes(bool changed, bool fetched)
        {
            foreach (ModeColorStore mode in storedModes)
                mode.update(changed, fetched);
        }

        private void invalidateStoredModes()
        {
            foreach (ModeColorStore mode in storedModes)
                mode.fetched = false;
        }

        //--------------------------------------------------------------
        // Control events

        int selectedIndex = -1;
        private void btnNorth_Click(object sender, EventArgs e)
        {
            selectedIndex = 0;
            clearStates();
            northState = true;
            solidColorStates[storedModes[currentMode].get(selectedIndex)] = true;
            updateAllButtons();
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            selectedIndex = 2;
            clearStates();
            southState = true;
            solidColorStates[storedModes[currentMode].get(selectedIndex)] = true;
            updateAllButtons();
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            selectedIndex = 3;
            clearStates();
            westState = true;
            solidColorStates[storedModes[currentMode].get(selectedIndex)] = true;
            updateAllButtons();
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            selectedIndex = 1;
            clearStates();
            eastState = true;
            solidColorStates[storedModes[currentMode].get(selectedIndex)] = true;
            updateAllButtons();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            selectedIndex = 4;
            clearStates();
            upState = true;
            solidColorStates[storedModes[currentMode].get(selectedIndex)] = true;
            updateAllButtons();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            selectedIndex = 5;
            clearStates();
            downState = true;
            solidColorStates[storedModes[currentMode].get(selectedIndex)] = true;
            updateAllButtons();
        }


        private void btnSolidColor_Click(object sender, EventArgs e)
        {
            clearSolidColorStates();
            int i = int.Parse((string)((Button)sender).Tag);
            solidColorStates[i] = true;
            for (int b = 0; b < 16; b++)
                updateButtonState(colorButtons[b], solidColorStates[b]);
            if (selectedIndex >= 0)
            {
                storedModes[currentMode].set(selectedIndex, i);
                updateDirectionButtons();
                Modified = true;
            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            form1.Send();
        }

        int countChanged() {
            int c = 0;
            for (int i = 0; i < storedModes.Count; i++)
                c += storedModes[i].countChanged();
            return c;
        }

        public void Send()
        {
            sendWholeModes = (Communicator.CurrentMode == CommMode.MAVLINK) || (countChanged() > MCI_THRESHOLD);
            comms.SendCommand();
        }

        public void MavlinkParameter(int index, float value)
        {
            index -= Form1.MAV_MODE_START;
            cbxMode.SelectedIndex = index;
            Application.DoEvents();
            byte[] bytes = BitConverter.GetBytes(value);
            int[] colors = new int[] {
                bytes[0] & 0x1f, 
                ((bytes[0] & 0xe0) >> 5) + ((bytes[1] & 0xe0) >> 2),
                bytes[1] & 0x1f,
                bytes[2] & 0x1f,
                ((bytes[2] & 0xe0) >> 5) + ((bytes[3] & 0xe0) >> 2),
                bytes[3] & 0x1f};
            storedModes[index].load(colors);
            if (index == 20) MavlinkCompleted();
        }

        public void MavlinkStarted()
        {
            IsEnabled = false;
            btnSend.Enabled = false;
            cbxModeDisable();
            Text = "Mode Colors - Reading..";
        }

        public void MavlinkCompleted()
        {
            btnSend.Enabled = true;
            cbxModeEnable();
            Text = "Mode Colors";
            IsEnabled = true;
            if (cbxMode.SelectedIndex == -1) cbxMode.SelectedIndex = 0;
            Application.DoEvents();
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            if (!isEnabled) return;
            storedModes[currentMode].setAll(defaultModeColors[currentMode]);
            updateAllButtons();
            Modified = true;
        }

        private void btnAllDefault_Click(object sender, EventArgs e)
        {
            if (!isEnabled) return;
            LoadDefaults();
            Modified = true;
        }

        bool cbxModeDisableSelection = false;
        int cbxMode_savedIndex;
        void cbxModeDisable()
        {
            cbxMode_savedIndex = cbxMode.SelectedIndex;
            cbxModeDisableSelection = true;
        }

        void cbxModeEnable()
        {
            cbxModeDisableSelection = false;
            cbxMode.SelectedIndex = cbxMode_savedIndex;
        }

        private void cbxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxModeDisableSelection) return;
            clearStates();
            updateAllButtons();
            if (cbxMode.SelectedIndex >= 0)
            {
                currentMode = cbxMode.SelectedIndex;
                //if (!storedModes[currentMode].fetched) comms.GetData();
                //else 
                updateAllButtons();
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshModes();
        }

        //--------------------------------------------------------------
        // Communications

        private void comms_ComStarted(object sender, ComStartedEventArgs e)
        {
            if (sendWholeModes) comms.CommandsToSend = 21;
            else comms.CommandsToSend = 126;
            btnSend.Enabled = false;
            prgBar.Visible = true;
            prgBar.Value = 0;
            cbxModeDisable();
            if (e.Type == CommType.RECEIVING) IsEnabled = false;
        }

        private void comms_ProgressChanged(object sender, ProgressEventArgs e)
        {
            if (e.Event != "") Status = e.Event;
            prgBar.ValueFast(e.Progress);
            if (e.Type == CommType.RECEIVING)
                cbxMode.SelectedIndex = ((int)(e.Progress / 1.6f));
        }

        private void comms_Completed(object sender, CompletedEventArgs e)
        {
            prgBar.Visible = false;
            prgBar.Value = 0;
            btnSend.Enabled = true;
            cbxModeEnable();
            Text = "Mode Colors";
            if (e.Type == CommType.SENDING)
            {
                if (e.Success) updateStoredModes(false);
                if (frmPalette.Instance != null) frmPalette.Instance.Send();
                else if (frmPixSettings.Instance != null) frmPixSettings.Instance.Send();
            }
            else if (e.Type == CommType.RECEIVING && e.Success)
            {
                IsEnabled = true;
                if (cbxMode.SelectedIndex == -1) cbxMode.SelectedIndex = 0;
                
                //Trigger the next collection step
                if (refreshPalette)
                {
                    if (frmPalette.Instance != null)
                        frmPalette.Instance.refreshAllColors();
                    else if (frmPixSettings.Instance != null)
                        frmPixSettings.Instance.refreshSettings();
                    refreshPalette = false;
                }
                
                //Trigger the next initialisation step
                if (initPalette)
                {
                    if (frmPalette.Instance != null && !frmPalette.Instance.IsReady)
                        frmPalette.Instance.refreshAllColors();
                    else if (frmPixSettings.Instance != null && !frmPixSettings.Instance.IsReady)
                        frmPixSettings.Instance.refreshSettings();
                    initPalette = false;
                }

            }
            isReady = true;
        }


        //--------------------------------------------------------------
        // Command parsers and generators

        private string comms_ReadCommand(object sender, CommandEventArgs e)
        {
            return "mode_color";// + currentMode.ToString();
        }

        private string comms_WriteCommand(object sender, CommandEventArgs e)
        {
            if (sendWholeModes && storedModes[e.Index].isChanged())
                return constructModeCommand(e.Index);
            else
            {

                int mode = e.Index / 6;
                int color = e.Index % 6;
                if (storedModes[mode].changed[color])
                {
                    return "mode_color " + mode.ToString() + "," + storedModes[mode].get(color) + "," + color.ToString();
                }
                else return "";
            }
        }

        private void comms_GotMavlinkParam(object sender, MavParameterEventArgs e)
        {
            MavlinkParameter(e.Param.param_index, e.Param.param_value);
        }

        private MAVLink.mavlink_param_set_t comms_WriteMavlink(object sender, CommandEventArgs e)
        {
            MAVLink.mavlink_param_set_t param = new MAVLink.mavlink_param_set_t();
            if (!storedModes[e.Index].isChanged()) return param;
            param.target_system = comms.sysid;
            param.target_component = comms.compid;
            form1.SetIdString(ref param.param_id, "mode_" + e.Index.ToString());
            param.param_type = (byte)MAVLink.MAV_PARAM_TYPE.UINT32;
            ModeColorStore mode = storedModes[e.Index];
            param.param_value = BitConverter.ToSingle(new byte[] {
                (byte)(mode.get(0) + ((mode.get(1) << 5) & 0xe0)),
                (byte)(mode.get(2) + ((mode.get(1) << 2) & 0xe0)),
                (byte)(mode.get(3) + ((mode.get(4) << 5) & 0xe0)),
                (byte)(mode.get(5) + ((mode.get(4) << 2) & 0xe0))}, 0);
            return param;
        }

        private string constructModeCommand(int mode)
        {
            string command = "mode_color " + mode.ToString() + " ";
            for (int index = 0; index < 6; index++)
            {
                command += storedModes[mode].get(index);
                if (index < 5) command += ",";
            }
            return command;
        }

        private void comms_GotData(object sender, GotDataEventArgs e)
        {
            bool success = Parse(e.Data, false);
            if (success) Status = "Configuration: Got Mode data OK.";
            else comms.Retry();
            updateAllButtons();
        }

        public void UpdateAll()
        {
            updateAllButtons();
            if (cbxMode.SelectedIndex == -1) cbxMode.SelectedIndex = 0;
        }

        public bool Parse(string data, bool updating)
        {
            string[] entries;
            data = data.Replace("\r", "");
            if (data.Contains('\n')) entries = data.Split('\n');
            else entries = new string[] { data };
            bool success = true;
            for (int count = 0; count < entries.Length; count++)
            {
                if (entries[count].Contains("mode_color ") && entries[count].Contains(","))
                {
                    string[] line = entries[count].Split(' ');
                    int index, value;
                    if (line.Length == 3 && int.TryParse(line[1], out index))
                    {
                        string[] colors = line[2].Split(',');
                        if (colors.Length == 6)
                            for (int color = 0; color < colors.Length; color++)
                            {
                                if (int.TryParse(colors[color], out value))
                                {
                                    if (updating)
                                    {
                                        if (storedModes[index].get(color) != value)
                                        {
                                            storedModes[index].set(color, value);
                                            storedModes[index].changed[color] = true;
                                        }
                                    }
                                    else
                                    {
                                        storedModes[index].set(color, value);
                                        storedModes[index].changed[color] = false;
                                    }
                                    storedModes[index].fetched = true;
                                }
                                else success = false;
                            }
                        else success = false;
                    }
                    else success = false;
                }
            }
            return success;
        }

    }
}
