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
using System.Reflection;
using System.Diagnostics;
using OpenNETCF.IO.Serial;
using OpenNETCF.IO.Ports.Streams;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Collections;


namespace MavpixelGUI
{
    public partial class Form1 : Form
    {
        //Global variables
        Ports availablePorts;
        //public SerialPort port { get { return serialControl1.Port; } set { serialControl1.Port = value; } }
        //Name of currently open port
        public string PortName { get { if (serialControl1.IsOpen) return serialControl1.PortName; else return ""; } }
        //Programmer
        AVRDude avrDude;
        //Mavlink interpreter
        //MavlinkWorker mavWorker;
        public MAVLink.MavlinkParse Mavlink;

        //Status string
        char[] trimChars = new char[] { '\r', '\n', ':' };
        public string Status
        {
            get { return lblStatus.Text; }
            set
            {
                string line = value;
                if (line.Contains('\n')) line = line.Substring(0, line.IndexOf('\n'));
                lblStatus.Text = line.Trim(trimChars);
                //termControl1.Append(value);
            }
        }

        //---------------------------------------------------------
        // LED editor vars

        //Image of what the LEDs currently are on the Mavpixel
        // used to check for changes to LED data
        List<LedStore> storedLeds = new List<LedStore>();

        //button states
        bool warningState;
        bool modeState;
        bool indicatorState;
        bool armState;
        bool throttleState;
        bool ringState;
        bool colorState;
        bool gpsState;
        bool northState;
        bool southState;
        bool eastState;
        bool westState;
        bool upState;
        bool downState;
        bool[] solidColorStates = new bool[16];

        //Default index->color mapping for solid LED colors
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

        //Wire ordering mode
        bool wireOrderMode = false;
        //Flag to notify refresh to do all windows 
        bool refreshAllWindows;
        //List of the colour buttons
        Button[] colorButtons;
        //List of LEDs in the current selection
        List<LedControl> selectedLeds;
        //Modification state
        bool modified = false;
        public bool Modified
        {
            get { return modified; }
            set
            {
                modified = value;
                updateTitle();
            }
        }
        //Current file
        string fileName;

        //--------------------------------------------------------------
        // Constructor, destructor and housekeeping

        public Form1()
        {
            InitializeComponent();
            colorButtons = new Button[16];
            getColorButtons();
            solidColorStates = new bool[16];
            clearButtonStates();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            comms.Setup();
            //mavWorker = new MavlinkWorker();
            Mavlink = new MAVLink.MavlinkParse();
            Mavlink.MavMessageHandler += new MAVLink.MavlinkParse.MavMessageEventHandler(MessageHandler);
            ledArray1.Enabled = false;
            btnSend.Enabled = false;
            availablePorts = new Ports();   //Create RS232 port interface
            avrDude = new AVRDude();
            restoreWindow(this, "pgmWindowState", "pgmWindowLocation", "pgmWindowSize");
            this.Opacity = 100;
            terminal = new frmTerminal(serialControl1);
            startWindow(terminal, "termWindowState", "termWindowLocation", "termWindowSize", "termShowing");
            modeColorDialog = new frmModeColor(serialControl1);
            startWindow(modeColorDialog, "modeWindowState", "modeWindowLocation", "", "modeShowing");
            pixSettingsDialog = new frmPixSettings(serialControl1);
            startWindow(pixSettingsDialog, "pixsetWindowState", "pixsetWindowLocation", "", "pixsetShowing");
            paletteDialog = new frmPalette(serialControl1);
            startWindow(paletteDialog, "paletteWindowState", "paletteWindowLocation", "", "paletteShowing");
            if (Properties.Settings.Default.setUpdateCheck) checkForUpdates();
            Modified = false;
            fileName = "";
            updateTitle();
        }

        //Update the main window title
        private void updateTitle()
        {
            string file = Path.GetFileName(fileName);
            this.Text = (modified ? "*" : "") + file + (file == "" ? "" : " - ")
                + "Mavpixel GUI Configurator"
                + (Communicator.CurrentMode == CommMode.CLI ? " - CLI Mode" : "")
                + (Communicator.CurrentMode == CommMode.MAVLINK ? " - Mavlink Mode" : "");
        }

        //Read the colour buttons into a list
        // this is to allow them to be accessed by index
        private void getColorButtons()
        {
            int index;
            foreach (Control ctl in panel1.Controls)
                if (ctl is Button)
                    if (ctl.Tag != null && (string)ctl.Tag != "")
                    {
                        index = int.Parse((string)ctl.Tag);
                        colorButtons[index] = (Button)ctl;
                        frmPalette.SetButtonColor(colorButtons[index], solidColors[index]);
                    }
        }

        //Check for updated version
        private void checkForUpdates()
        {
            Status = "Checking for updates";
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int Epoch = (int)t.TotalSeconds;
            string xmlURL = "http://raw.githubusercontent.com/prickle/MavpixelGUI/master/CurrentVersion.xml";
            UpdateWorker worker = new UpdateWorker("MavpixelGUI");
            worker.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(checkUpdates_RunWorkerCompleted);
            worker.worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.checkForUpdate(xmlURL);
            //continues from worker_runWorkerCompleted
        }

        //Update check has something to say
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (Status.Contains("updates"))
                Status += ".";
        }

        frmUpdate updateForm = null;
        //Update check has results
        void checkUpdates_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string result = (string)e.Result;
            if (result.Contains(" is up to date.")) Status = "MavpixelGUI is up to date.";
            else if (!result.Contains("Error during update")) //Ignore errors here
            {
                updateForm = new frmUpdate(result);
                updateForm.Show();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO: check modified and warn
            if (terminal != null)
                SaveWindow(terminal, "termWindowState", "termWindowLocation", "termWindowSize", "termShowing");
            if (paletteDialog != null)
                SaveWindow(paletteDialog, "paletteWindowState", "paletteWindowLocation", "", "paletteShowing");
            if (modeColorDialog != null)
                SaveWindow(modeColorDialog, "modeWindowState", "modeWindowLocation", "", "modeShowing");
            if (pixSettingsDialog != null)
                SaveWindow(pixSettingsDialog, "pixsetWindowState", "pixsetWindowLocation", "", "pixsetShowing");
            SaveWindow(this, "pgmWindowState", "pgmWindowLocation", "pgmWindowSize");
            serialControl1.ForceClose();
            Properties.Settings.Default.Save();
        }

        //--------------------------------------------------------------
        // Serial port events

        private void serialControl1_PortOpening(object sender, EventArgs e)
        {
            if (serialControl1.LinkType == serialControl.Type.UDP)
                Status = serialControl1.LinkName + ": Waiting for " + serialControl1.PortName + " Client..";
            else            
                Status = serialControl1.LinkName + ": Opening " + serialControl1.PortName + " Port..";
            boxProgress.Visible = !ledArray1.Enabled;
            showRoundboxInfo(!ledArray1.Enabled && !Properties.Settings.Default.pgmGetOnConnect, "Waiting to Reload, Open file or New..");
            boxProgressLabel.Text = "Waiting for " + serialControl1.PortName + " connection..";
            prgReading.Style = ProgressBarStyle.Marquee;
            Application.DoEvents();
        }

        private void serialControl1_PortOpened(object sender, EventArgs e)
        {
            boxProgress.Visible = false;
            prgReading.Style = ProgressBarStyle.Continuous;
            if (serialControl1.LinkType == serialControl.Type.UDP)
                Status = serialControl1.LinkName + ": Client connected.";
            else
                Status = serialControl1.LinkName + ": Opened.";
            saveButtonsEnabled(true);
            enableTerminal(true);
            Application.DoEvents();
            if (serialControl1.LinkType == serialControl.Type.UDP) comms.DoModeChange(CommMode.MAVLINK);
            if (!ledArray1.Enabled && Properties.Settings.Default.pgmGetOnConnect) initialGetData();
            else comms.ConnectAndGetPrompt();
        }

        private void serialControl1_PortError(object sender, PortErrorEventArgs e)
        {
            boxProgress.Visible = false;
            prgReading.Style = ProgressBarStyle.Continuous;
            if (e.Message.IndexOf("Framing") >= 0 || 
                e.Message.IndexOf("Overrun") >= 0 ||
                e.Message.IndexOf("Parity") >= 0 ||
                e.Message.IndexOf("Receive Overflow") >= 0 ||
                e.Message.IndexOf("Transmit Overflow") >= 0)
                e.Message = e.Message + " Error.";
            if (e.Message.IndexOf("Framing") >= 0) e.Message += " Perhaps the baud rate is wrong?";
            if (!this.Disposing && !this.IsDisposed && this.IsHandleCreated)
                this.Invoke((MethodInvoker)delegate { Status = serialControl1.LinkName + ": " + e.Message; });
        }

        private void serialControl1_PortClosed(object sender, EventArgs e)
        {
            boxProgress.Visible = false;
            prgReading.Style = ProgressBarStyle.Continuous;
            comms.Cancel();
            comms.DoModeChange(CommMode.NONE);
            this.Cursor = Cursors.Default;
            Status = serialControl1.LinkName + ": Closed.";
            showRoundboxInfo(!ledArray1.Enabled, "Waiting to Connect, Open file or New..");
            //port = null;
            enableTerminal(false);
            saveButtonsEnabled(false);
            OnCommsClosed();
            AllMavIds.Clear();
            clearMavpixelSysid();
            updateBeats();
        }

        //--------------------------------------------------------------
        // Helper routines

        //Set the timeout on all communicators
        public void setTimeout(int timeout)
        {
            comms.Timeout = timeout;
            if (modeColorDialog != null) modeColorDialog.Timeout = timeout;
            if (pixSettingsDialog != null) pixSettingsDialog.comTimer.Interval = timeout;
            if (paletteDialog != null) paletteDialog.Timeout = timeout;
            Properties.Settings.Default.comTimeout = timeout;
        }

        //Turn on the terminal
        private void enableTerminal(bool enabled)
        {
            if (terminal != null)
            {
                terminal.IsEnabled = enabled;
                if (enabled) terminal.Clear();
            }
        }

        //Control the visibility of the info boxes
        private void showRoundboxInfo(bool show, string info)
        {
            rbxInfo.Visible = show;
            rbxPrompt.Visible = show;
            if (info != "") lblWaiting.Text = info;
        }

        private void showRoundboxInfo(string info)
        {
            lblWaiting.Text = info;
        }

        //Turn the save buttons on all windows on or off
        private void saveButtonsEnabled(bool enabled)
        {
            btnSend.Enabled = enabled;
            if (pixSettingsDialog != null) pixSettingsDialog.btnSend.Enabled = enabled;
            if (modeColorDialog != null) modeColorDialog.btnSend.Enabled = enabled;
            if (paletteDialog != null) paletteDialog.btnSend.Enabled = enabled;
        }

        public void refreshAll()
        {
            refreshAllWindows = true;
            if (serialControl1.IsOpen)
                comms.GetAllData();
        }

        bool heartBeatState;
        private void setHeart(bool state)
        {
            heartBeatState = state;
            if (state) lblBeats.Image = Properties.Resources.heart;
            else lblBeats.Image = Properties.Resources.heart_grey;
        }

        bool heartBeating;
        bool heartBeatTrigger;
        private void heartBeat()
        {
            if (!heartBeating)
            {
                heartBeating = true;
                setHeart(true);
            }
            else heartBeatTrigger = true;
            if (!hbTimer.Enabled) hbTimer.Start();
        }

        private void hbTimer_Tick(object sender, EventArgs e)
        {
            if (heartBeatState) setHeart(false);
            else if (heartBeatTrigger)
            {
                setHeart(true);
                heartBeatTrigger = false;
            }
            else
            {
                heartBeating = false;
                hbTimer.Stop();
            }
        }



        #region LED Strip editor

        //--------------------------------------------------------------
        // Control states

        //Clear all leds in the array and all buttons
        private void clearLedArray()
        {
            clearButtonStates();
            updateAllButtons();
            ledArray1.Clear();
            ledArray1.ClearWiring();
            lblRemaining.Text = ledArray1.CountRemaining().ToString();
        }

        //Clear all button states
        private void clearButtonStates()
        {
            warningState = false;
            modeState = false;
            indicatorState = false;
            armState = false;
            throttleState = false;
            ringState = false;
            colorState = false;
            gpsState = false;
            northState = false;
            southState = false;
            eastState = false;
            westState = false;
            upState = false;
            downState = false;
            clearColorButtonStates();
        }

        //Clear just the colour button states
        private void clearColorButtonStates()
        {
            for (int i = 0; i < 16; i++)
                solidColorStates[i] = false;
        }

        //LED selection event has taken place
        private void ledArray1_Selected(object sender, SelectedEventArgs e)
        {
            selectedLeds = e.SelectedLeds;
            clearButtonStates();
            foreach (LedControl led in selectedLeds)
            {
                if (led.Mode.WARNING) warningState = true;
                if (led.Mode.MODE) modeState = true;
                if (led.Mode.INDICATOR) indicatorState = true;
                if (led.Mode.ARM) armState = true;
                if (led.Mode.THROTTLE) throttleState = true;
                if (led.Mode.RING) ringState = true;
                if (led.Mode.COLOR) colorState = true;
                if (led.Mode.GPS) gpsState = true;
                if (led.Direction.NORTH) northState = true;
                if (led.Direction.SOUTH) southState = true;
                if (led.Direction.EAST) eastState = true;
                if (led.Direction.WEST) westState = true;
                if (led.Direction.UP) upState = true;
                if (led.Direction.DOWN) downState = true;
                solidColorStates[led.ColorIndex] = true;
            }
            updateAllButtons();
        }

        //Update the UI to reflect the current button states
        private void updateAllButtons()
        {
            updateButtonState(btnWarnings, warningState, Color.Red);
            updateButtonState(btnModes, modeState, Color.LimeGreen);
            updateButtonState(btnIndicator, indicatorState, Color.Yellow);
            updateButtonState(btnArm, armState, Color.Blue);
            updateButtonState(btnThrottle, throttleState, Color.Orange);
            updateButtonState(btnRing, ringState, Color.DarkGray);
            updateButtonState(btnColor, colorState, Properties.Resources.rainbow);
            updateButtonState(btnGps, gpsState, Color.Green);
            updateButtonState(btnNorth, northState, SystemColors.ControlLightLight);
            updateButtonState(btnSouth, southState, SystemColors.ControlLightLight);
            updateButtonState(btnEast, eastState, SystemColors.ControlLightLight);
            updateButtonState(btnWest, westState, SystemColors.ControlLightLight);
            updateButtonState(btnUp, upState, SystemColors.ControlLightLight);
            updateButtonState(btnDown, downState, SystemColors.ControlLightLight);
            for (int i = 0; i < 15; i++) 
                updateButtonState(colorButtons[i], solidColorStates[i]); 
        }

        //Update a button to match a state
        void updateButtonState(Button b, bool state)
        {
            if (state)
                b.FlatAppearance.BorderColor = Color.Black;
            else
                b.FlatAppearance.BorderColor = Color.Gray;
        }
        void updateButtonState(Button b, bool state, Bitmap bitmap)
        {
            if (state)
            {
                b.FlatAppearance.BorderColor = Color.Black;
                b.BackgroundImage = bitmap;
            }
            else
            {
                b.FlatAppearance.BorderColor = Color.Gray;
                b.BackgroundImage = null;
            }
        }
        void updateButtonState(Button b, bool state, Color color)
        {
            if (state)
            {
                b.FlatAppearance.BorderColor = Color.Black;
                b.BackColor = color;
            }
            else
            {
                b.FlatAppearance.BorderColor = Color.Gray;
                b.BackColor = SystemColors.Control;
            }
        }

        //Callback providing remaining LED count
        private void ledArray1_RemainingChanged(object sender, RemainingChangedEventArgs e)
        {
            lblRemaining.Text = e.Remaining.ToString();
        }

        //--------------------------------------------------------------
        // Wiring buttons

        private void btnWireOrder_Click(object sender, EventArgs e)
        {
            wireOrderMode = !wireOrderMode;
            updateButtonState(btnWireOrder, wireOrderMode, Color.Green);
            ledArray1.OrderMode = wireOrderMode;
        }

        private void btnClearSelectedWiring_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                foreach (LedControl led in selectedLeds)
                    if (led.Index >= 0)
                    {
                        led.Index = -1;
                        led.Invalidate();
                        Modified = true;
                    }
                lblRemaining.Text = ledArray1.CountRemaining().ToString();
            }
        }

        private void btnClearAllWiring_Click(object sender, EventArgs e)
        {
            ledArray1.ClearWiring();
            lblRemaining.Text = ledArray1.CountRemaining().ToString();
            Modified = true;
        }

        private void btnClearSelected_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                clearButtonStates();
                updateAllButtons();
                foreach (LedControl led in selectedLeds)
                {
                    if (led.Mode.Get() != 0 || led.Direction.Get() != 0 || led.ColorIndex != 0)
                    {
                        led.Mode.Set(0);
                        led.Direction.Set(0);
                        led.ColorIndex = 0;
                        led.Invalidate();
                        Modified = true;
                    }
                }
                lblRemaining.Text = ledArray1.CountRemaining().ToString();
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            clearButtonStates();
            updateAllButtons();
            ledArray1.Clear();
            lblRemaining.Text = ledArray1.CountRemaining().ToString();
            Modified = true;
        }

        //--------------------------------------------------------------
        // Mode buttons

        private void btnWarnings_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                warningState = !warningState;
                updateButtonState(btnWarnings, warningState, Color.Red);
                foreach (LedControl led in selectedLeds)
                    if (led.Mode.WARNING != warningState)
                    {
                        led.Mode.WARNING = warningState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnModes_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                modeState = !modeState;
                updateButtonState(btnModes, modeState, Color.LimeGreen);
                foreach (LedControl led in selectedLeds)
                    if (led.Mode.MODE != modeState)
                    {
                        led.Mode.MODE = modeState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnIndicator_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                indicatorState = !indicatorState;
                updateButtonState(btnIndicator, indicatorState, Color.Yellow);
                foreach (LedControl led in selectedLeds)
                    if (led.Mode.INDICATOR != indicatorState)
                    {
                        led.Mode.INDICATOR = indicatorState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnArm_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                armState = !armState;
                updateButtonState(btnArm, armState, Color.Blue);
                foreach (LedControl led in selectedLeds)
                    if (led.Mode.ARM != armState)
                    {
                        led.Mode.ARM = armState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnThrottle_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                throttleState = !throttleState;
                updateButtonState(btnThrottle, throttleState, Color.Orange);
                foreach (LedControl led in selectedLeds)
                    if (led.Mode.THROTTLE != throttleState)
                    {
                        led.Mode.THROTTLE = throttleState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnRing_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                ringState = !ringState;
                updateButtonState(btnRing, ringState, Color.DarkGray);
                foreach (LedControl led in selectedLeds)
                    if (led.Mode.RING != ringState)
                    {
                        led.Mode.RING = ringState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                colorState = !colorState;
                updateButtonState(btnColor, colorState, Properties.Resources.rainbow);
                foreach (LedControl led in selectedLeds)
                    if (led.Mode.COLOR != colorState)
                    {
                        led.Mode.COLOR = colorState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnGps_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                gpsState = !gpsState;
                updateButtonState(btnGps, gpsState, Color.Green);
                foreach (LedControl led in selectedLeds)
                    if (led.Mode.GPS != gpsState)
                    {
                        led.Mode.GPS = gpsState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        //--------------------------------------------------------------
        // Direction buttons

        private void btnNorth_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                northState = !northState;
                updateButtonState(btnNorth, northState, SystemColors.ControlLightLight);
                foreach (LedControl led in selectedLeds)
                    if (led.Direction.NORTH != northState)
                    {
                        led.Direction.NORTH = northState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                southState = !southState;
                updateButtonState(btnSouth, southState, SystemColors.ControlLightLight);
                foreach (LedControl led in selectedLeds)
                    if (led.Direction.SOUTH != southState)
                    {
                        led.Direction.SOUTH = southState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                westState = !westState;
                updateButtonState(btnWest, westState, SystemColors.ControlLightLight);
                foreach (LedControl led in selectedLeds)
                    if (led.Direction.WEST != westState)
                    {
                        led.Direction.WEST = westState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                eastState = !eastState;
                updateButtonState(btnEast, eastState, SystemColors.ControlLightLight);
                foreach (LedControl led in selectedLeds)
                    if (led.Direction.EAST != eastState)
                    {
                        led.Direction.EAST = eastState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                upState = !upState;
                updateButtonState(btnUp, upState, SystemColors.ControlLightLight);
                foreach (LedControl led in selectedLeds)
                    if (led.Direction.UP != upState)
                    {
                        led.Direction.UP = upState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                downState = !downState;
                updateButtonState(btnDown, downState, SystemColors.ControlLightLight);
                foreach (LedControl led in selectedLeds)
                    if (led.Direction.DOWN != downState)
                    {
                        led.Direction.DOWN = downState;
                        led.Invalidate();
                        Modified = true;
                    }
            }
        }


        private void btnSolidColor_Click(object sender, EventArgs e)
        {
            if (selectedLeds != null && selectedLeds.Count > 0)
            {
                clearColorButtonStates();
                int i = int.Parse((string)((Button)sender).Tag);
                solidColorStates[i] = true;
                for (int b = 0; b < 16; b++)
                    updateButtonState(colorButtons[b], solidColorStates[b]);
                foreach (LedControl led in selectedLeds)
                    if (led.ColorIndex != i)
                    {
                        led.ColorIndex = i;
                        Modified = true;
                    }
            }
        }
        #endregion

        #region Menu item events

        private void btnSend_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void mavpixelOnGithubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { Process.Start("http://github.com/prickle/Mavpixel/"); }
            catch { }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshAllWindows = false;
            if (serialControl1.IsOpen)
                comms.GetData();
        }
        private void reloadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshAll();
        }

        private void resetMavpixelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Communicator.CurrentMode == CommMode.CLI)
            {
                comms.SendCommand("reboot", false);
                comms.ConnectAndGetPrompt();
            }
            else if (Communicator.CurrentMode == CommMode.MAVLINK)
                comms.SendParameter("reboot", 1);
        }

        private void fullClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnClearAll_Click(sender, e);
            btnClearAllWiring_Click(sender, e);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearLedArray();
            ledArray1.Enabled = true;
            showRoundboxInfo(false, "");
            btnSend.Enabled = (serialControl1.IsOpen);
            if (pixSettingsDialog != null)
            {
                pixSettingsDialog.Clear();
                pixSettingsDialog.LoadDefaults();
                pixSettingsDialog.updateStoredSettings(false, true);
                pixSettingsDialog.IsEnabled = true;
            }
            if (modeColorDialog != null)
            {
                modeColorDialog.Clear();
                modeColorDialog.LoadDefaults();
                modeColorDialog.updateStoredModes(false, true);
                modeColorDialog.IsEnabled = true;
            }
            if (paletteDialog != null)
            {
                paletteDialog.Clear();
                paletteDialog.LoadDefaults();
                paletteDialog.updateStoredColors(false, true);
                paletteDialog.IsEnabled = true;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.pgmFileName == null || Properties.Settings.Default.pgmFileName == "")
                Properties.Settings.Default.pgmFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.pgmFileName);
            openFileDialog.FileName = "*.txt"; //Path.GetFileName(Properties.Settings.Default.pgmFlash);
            openFileDialog.Title = "Select Input File";
            openFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                updateTitle();
                loadFile();
                Properties.Settings.Default.pgmFileName = fileName;
                //setFlashFile(openFileDialog.FileName);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.pgmFileName == null || Properties.Settings.Default.pgmFileName == "")
                Properties.Settings.Default.pgmFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.pgmFileName);
            if (fileName == "") fileName = "*.txt";
            saveFileDialog.FileName = fileName; //Path.GetFileName(Properties.Settings.Default.pgmFlash);
            saveFileDialog.Title = "Save Output File";
            saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                updateTitle();
                saveFile();
                Properties.Settings.Default.pgmFileName = fileName;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileName == "") return;
            saveFile();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearLedArray();
            ledArray1.Enabled = false;
            btnSend.Enabled = false;
            if (pixSettingsDialog != null) pixSettingsDialog.IsEnabled = false;
            if (modeColorDialog != null) modeColorDialog.IsEnabled = false;
            if (paletteDialog != null) paletteDialog.IsEnabled = false;
            showRoundboxInfo(true, "Waiting to Connect, Open file or New..");
            fileName = "";
            Modified = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Sysid handlers
        //------------------------------------------------------------------------------------------------
        // Mavlink sysid handlers

        //Mavlink device sysid/compid pair
        class mavId
        {
            public int sysid;
            public int compid;
            public mavId(int sys, int comp)
            {
                sysid = sys;
                compid = comp;
            }
        }

        //All seen Mavlink devices
        List<mavId> AllMavIds = new List<mavId>();

        // List of Mavpixel sysids
        List<byte> MavpixelSysIds = new List<byte>();

        //Currently selected target Mavpixel
        int currentSysId = 0;

        //Add a Mavlink device if not already seen
        private void addMavId(MAVLink.MAVLinkMessage MAVLinkMessage)
        {
            mavId id = new mavId(MAVLinkMessage.sysid, MAVLinkMessage.compid);
            //Search for ID and reset expiry counter if found
            bool found = false;
            foreach (mavId stored in AllMavIds)
            {
                if (id.compid == stored.compid && id.sysid == stored.sysid)
                {
                    found = true;
                    break;
                }
            }
            //New ID, add to list
            if (!found)
            {
                AllMavIds.Add(id);
                updateBeats();
            }
        }

        //Update the statusBar Mavlink device list
        private void updateBeats()
        {
            string beats = "";
            foreach (mavId mav in AllMavIds)
            {
                if (mav.compid == 1) beats += "<Vehicle " + mav.sysid.ToString() + ">";
                if (mav.compid == 160) beats += "<Mavpixel " + mav.sysid.ToString() + ">";
                if (mav.compid == 190) beats += "<Ground Station " + mav.sysid.ToString() + ">";
                if (mav.compid == 0) beats += "<Ground Station " + mav.sysid.ToString() + ">";
            }
            lblBeats.Text = beats;
        }

        //Add a Mavpixel if not found and update cbxSysid
        private void addMavpixelSysid(byte newSysid)
        {
            if (MavpixelSysIds.Contains(newSysid)) return;
            MavpixelSysIds.Add(newSysid);
            int selection = cbxSysid.SelectedIndex;
            cbxSysid.Items.Clear();
            cbxSysid.Visible = true;
            lblDevice.Visible = true;
            foreach (int sysid in MavpixelSysIds)
            {
                cbxSysid.Items.Add("Mavpixel " + sysid.ToString());
            }
            if (selection == -1) selection = 0;
            cbxSysid.SelectedIndex = selection;
        }

        //Clear Mavpixel sysid list and hide cbxSysid
        private void clearMavpixelSysid()
        {
            MavpixelSysIds.Clear();
            cbxSysid.Items.Clear();
            cbxSysid.Visible = false;
            lblDevice.Visible = false;
            cbxSysid.SelectedIndex = -1;
        }

        //Select a new Mavpixel
        private void cbxSysid_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentSysId = cbxSysid.SelectedIndex;
            comms.sysid = MavpixelSysIds[currentSysId];
        }

        //Force a change to a new Mavpixel sysid
        public void ChangeSysid(byte sysid)
        {
            addMavpixelSysid(sysid);
            cbxSysid.SelectedIndex = MavpixelSysIds.IndexOf(sysid);
        }
        #endregion

        #region Window management
        //------------------------------------------------------------------------------------------------
        // Window management

        //Trigger an initialisation cascade for windows that are not ready
        void initWaitingWindows()
        {
            if (modeColorDialog != null && !modeColorDialog.IsReady)
                modeColorDialog.initAllModes();
            else if (paletteDialog != null && !paletteDialog.IsReady)
                paletteDialog.initAllColors();
            else if (pixSettingsDialog != null && !pixSettingsDialog.IsReady)
                pixSettingsDialog.refreshSettings();
        }

        frmTerminal terminal = null;
        private void terminalWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (terminal == null)
            {
                terminal = new frmTerminal(serialControl1);
                terminal.IsEnabled = (serialControl1.IsOpen && Communicator.CurrentMode != CommMode.MAVLINK);
                startWindow(terminal, "termWindowState", "termWindowLocation", "termWindowSize", "");
            }
            else
            {
                terminal.IsEnabled = (serialControl1.IsOpen && Communicator.CurrentMode != CommMode.MAVLINK);
                terminal.Show();
                if (terminal.WindowState == FormWindowState.Minimized)
                    terminal.WindowState = FormWindowState.Normal;
                terminal.Focus();
            }
        }

        frmModeColor modeColorDialog = null;
        private void modeColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (modeColorDialog == null)
            {
                modeColorDialog = new frmModeColor(serialControl1);
                modeColorDialog.IsEnabled = false;// (port != null);
                startWindow(modeColorDialog, "modeWindowState", "modeWindowLocation", "", "");
                if (Properties.Settings.Default.pgmGetOnConnect) modeColorDialog.refreshModes();
            }
            else
            {
                modeColorDialog.Show();
                if (modeColorDialog.WindowState == FormWindowState.Minimized)
                    modeColorDialog.WindowState = FormWindowState.Normal;
                modeColorDialog.Focus();
            }
        }

        frmPixSettings pixSettingsDialog = null;
        private void mavpixelSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pixSettingsDialog == null)
            {
                pixSettingsDialog = new frmPixSettings(serialControl1);
                pixSettingsDialog.IsEnabled = false;//(port != null);
                startWindow(pixSettingsDialog, "pixsetWindowState", "pixsetWindowLocation", "", "");
                if (Properties.Settings.Default.pgmGetOnConnect) pixSettingsDialog.refreshSettings();
            }
            else
            {
                pixSettingsDialog.Show();
                if (pixSettingsDialog.WindowState == FormWindowState.Minimized)
                    pixSettingsDialog.WindowState = FormWindowState.Normal;
                pixSettingsDialog.Focus();
            }
        }

        frmPalette paletteDialog = null;
        private void colorPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (paletteDialog == null)
            {
                paletteDialog = new frmPalette(serialControl1);
                paletteDialog.IsEnabled = false;// (port != null);
                startWindow(paletteDialog, "paletteWindowState", "paletteWindowLocation", "", "");
                if (Properties.Settings.Default.pgmGetOnConnect) paletteDialog.refreshColors();
            }
            else
            {
                paletteDialog.Show();
                if (paletteDialog.WindowState == FormWindowState.Minimized)
                    paletteDialog.WindowState = FormWindowState.Normal;
                paletteDialog.Focus();
            }
        }

        frmFlasher flashDialog = null;
        private void firmwareFlasherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (flashDialog == null || flashDialog.IsDisposed)
                flashDialog = new frmFlasher(avrDude);
            flashDialog.Show();
            if (flashDialog.WindowState == FormWindowState.Minimized)
                flashDialog.WindowState = FormWindowState.Normal;
            flashDialog.Focus();
        }

        frmOptions settingsDialog = null;
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settingsDialog == null || settingsDialog.IsDisposed)
                settingsDialog = new frmOptions(avrDude);
            settingsDialog.Show();
            if (settingsDialog.WindowState == FormWindowState.Minimized)
                settingsDialog.WindowState = FormWindowState.Normal;
            settingsDialog.Focus();
        }

        frmAbout aboutDialog = null;
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (aboutDialog == null || aboutDialog.IsDisposed)
                aboutDialog = new frmAbout();
            aboutDialog.Show();
            if (aboutDialog.WindowState == FormWindowState.Minimized)
                aboutDialog.WindowState = FormWindowState.Normal;
            aboutDialog.Focus();
        }
        #endregion

        #region File management
        //------------------------------------------------------------------------------------------------
        // Save and load file

        void saveFile()
        {
            Status = "File: Saving..";
            string line = "";
            About about = new About();
            try
            {
                using (StreamWriter file = new StreamWriter(fileName))
                {
                    file.WriteLine("/ Mavpixel configuration file");
                    file.WriteLine("/ " + about.AssemblyTitle + " " + about.AssemblyVersion);
                    //Leds
                    file.WriteLine("\r\n/ --- LED Configuration ---");
                    LedControl led = null;
                    for (int index = 0; index < 32; index++)
                    {
                        int arrayIndex = -1;
                        for (int ai = 0; ai < 256; ai++)
                            if (ledArray1.Leds[ai].Index == index) arrayIndex = ai;
                        if (arrayIndex >= 0)
                        {
                            led = ledArray1.Leds[arrayIndex];
                            line = constructLedCommand(index, arrayIndex, led);
                            file.WriteLine(line);
                        }
                    }
                    //Settings
                    if (pixSettingsDialog == null && serialControl1.IsOpen)
                    {
                        pixSettingsDialog = new frmPixSettings(serialControl1);
                        startWindow(pixSettingsDialog, "pixsetWindowState", "pixsetWindowLocation", "", "pixsetShowing");
                        pixSettingsDialog.refreshSettings();
                        pixSettingsDialog.WaitReady();
                    }
                    if (pixSettingsDialog != null && pixSettingsDialog.IsReady)
                    {
                        file.WriteLine("\r\n/ --- Mavpixel settings ---");
                        pixSettingsDialog.Save(file);
                    }
                    //Modes
                    if (modeColorDialog == null && serialControl1.IsOpen)
                    {
                        modeColorDialog = new frmModeColor(serialControl1);
                        startWindow(modeColorDialog, "modeWindowState", "modeWindowLocation", "", "modeShowing");
                        modeColorDialog.refreshModes();
                        modeColorDialog.WaitReady();
                    }
                    if (modeColorDialog != null && modeColorDialog.IsReady)
                    {
                        file.WriteLine("\r\n/ --- Mode Colors ---");
                        modeColorDialog.Save(file);
                    }
                    //Palette
                    if (paletteDialog == null && serialControl1.IsOpen)
                    {
                        paletteDialog = new frmPalette(serialControl1);
                        startWindow(paletteDialog, "paletteWindowState", "paletteWindowLocation", "", "paletteShowing");
                        paletteDialog.refreshColors();
                        paletteDialog.WaitReady();
                    }
                    if (paletteDialog != null && paletteDialog.IsReady)
                    {
                        file.WriteLine("\r\n/ --- Color Palette ---");
                        paletteDialog.Save(file);
                    }
                    Modified = false;
                    Status = "File: Saved OK.";
                }
            }
            catch (Exception ex)
            {
                Status = "File: Error during save: " + ex.Message;
            }
        }

        private void loadFile()
        {
            Status = "File: Loading..";
            //Modes
            if (modeColorDialog == null)
            {
                modeColorDialog = new frmModeColor(serialControl1);
                startWindow(modeColorDialog, "modeWindowState", "modeWindowLocation", "", "modeShowing");
                if (serialControl1.IsOpen)
                {
                    modeColorDialog.refreshModes();
                    modeColorDialog.WaitReady();
                }
            }
            //Palette
            if (paletteDialog == null)
            {
                paletteDialog = new frmPalette(serialControl1);
                startWindow(paletteDialog, "paletteWindowState", "paletteWindowLocation", "", "paletteShowing");
                if (serialControl1.IsOpen)
                {
                    paletteDialog.refreshColors();
                    paletteDialog.WaitReady();
                }
            }
            //Settings
            if (pixSettingsDialog == null)
            {
                pixSettingsDialog = new frmPixSettings(serialControl1);
                startWindow(pixSettingsDialog, "pixsetWindowState", "pixsetWindowLocation", "", "pixsetShowing");
                if (serialControl1.IsOpen)
                {
                    pixSettingsDialog.refreshSettings();
                    pixSettingsDialog.WaitReady();
                }
            }
            try
            {
                bool leds = false;
                bool modes = false;
                bool colors = false;
                bool settings = false;
                using (StreamReader file = new StreamReader(fileName))
                {
                    while (!file.EndOfStream)
                    {
                        string line = file.ReadLine();
                        if (line.Contains("led"))
                        {
                            if (!leds)
                            {
                                ledArray1.Clear();
                                ledArray1.ClearWiring();
                            }
                            leds = Parse(line, true);
                        }
                        else if (line.Contains("mode_color") && modeColorDialog.Parse(line, true)) modes = true;
                        else if (line.Contains("color") && paletteDialog.Parse(line, true)) colors = true;
                        else if (pixSettingsDialog.ParseUpdate(line)) settings = true;
                    }
                    if (modes)
                    {
                        modeColorDialog.UpdateAll();
                        modeColorDialog.IsEnabled = true;
                    }
                    if (colors)
                    {
                        paletteDialog.UpdateAll();
                        paletteDialog.IsEnabled = true;
                    }
                    if (settings)
                    {
                        pixSettingsDialog.UpdateSoftBaud();
                        pixSettingsDialog.IsEnabled = true;
                    }
                    if (leds)
                        ledArray1.Enabled = true;
                    showRoundboxInfo(!ledArray1.Enabled, "Waiting to Connect or New..");
                    btnSend.Enabled = (serialControl1.IsOpen);
                }
                if (!leds && !modes && !colors && !settings)
                    Status = "File: Empty file, no data found.";
                else
                    Status = "File: Loaded OK.";
            }
            catch (Exception ex)
            {
                Status = "File: Error during load: " + ex.Message;
            }
            Modified = false;
        }
        #endregion

        #region Communication
        //------------------------------------------------------------------------------------------------
        // Low-level serial comms

        //Process raw data from serialControl
        private void serialControl1_DataReceived(object sender, OpenNETCF.IO.Ports.SerialReceivedEventArgs e)
        {
            while (serialControl1.IsOpen && serialControl1.BytesToRead > 0)
            {
                byte[] bytes = serialControl1.ReadExistingBytes();
                if (!this.Disposing && !this.IsDisposed)
                    this.Invoke((MethodInvoker)delegate
                    {
                        //Feed it through the Mavlink parser
                        string data = Mavlink.CollectPacket(bytes);
                        if (data != "")
                        {
                            //If the Mavlink parser did not eat it, feed it to the CLI parser
                            if (terminal != null) terminal.termOut(data);
                            OnDataReceived(new PortReceivedEventArgs(data));
                        }
                    });
            }
        }

        //Process Mavlink messages from Mavlink parser
        void MessageHandler(object sender, MAVLink.MavMessageEventArgs e)
        {
            // check its valid
            if (e.Message == null || e.Message.data == null) return;
            comms.DoModeChange(CommMode.MAVLINK);
            heartBeat();
            // save the sysid and compid of the seen MAV
            if (e.Message.compid == 160)
                addMavpixelSysid(e.Message.sysid);
            addMavId(e.Message);
            // from here we should check the the message is addressed to us
            if (e.Message.compid == 160 && MavpixelSysIds.Count > currentSysId && MavpixelSysIds[currentSysId] == e.Message.sysid)
                OnMavMessageData(e.Message);
        }

        //Data received event
        public delegate void DataReceivedEventHandler(object sender, PortReceivedEventArgs e);
        public event DataReceivedEventHandler DataReceived;
        public void OnDataReceived(PortReceivedEventArgs e)
        {
            if (DataReceived != null) DataReceived(this, e);
        }

        //Got a packet event
        public delegate void MavMessageEventHandler(object sender, MAVLink.MavMessageEventArgs e);
        public event MavMessageEventHandler MavMessageEvent;
        public void OnMavMessageData(MAVLink.MAVLinkMessage message)
        {
            if (MavMessageEvent != null) MavMessageEvent(this, new MAVLink.MavMessageEventArgs(message));
        }

        //Comms closed event
        public delegate void CommsClosedEventHandler(object sender, EventArgs e);
        public event CommsClosedEventHandler CommsClosed;
        public void OnCommsClosed()
        {
            if (CommsClosed != null) CommsClosed(this, EventArgs.Empty);
        }

        //------------------------------------------------------------------------------------------------
        // Mavpixel LED comms

        //Read all on first open
        private void initialGetData()
        {
            refreshAllWindows = true;
            boxProgress.Visible = true;
            Status += boxProgressLabel.Text = " Waiting for response..";
            prgReading.Style = ProgressBarStyle.Marquee;
            prgBar.Visible = true;
            prgBar.Style = ProgressBarStyle.Marquee;
            comms.ConnectAndGetAllData();
        }

        //Send all changed data
        public void Send()
        {
            if (serialControl1.IsOpen)
                comms.SendCommand();
        }

        //------------------------------------------------------------------------------------------------
        // Communicator callbacks

        //Communication established, prepare UI
        private void comms_ComStarted(object sender, ComStartedEventArgs e)
        {
            Communicator.MessageCounter = 0;
            if (e.Type != CommType.SINGLE)
            {
                if (e.Type == CommType.RECEIVING)
                {
                    showRoundboxInfo(false, "");
                    ledArray1.Enabled = false;
                    ledArray1.Clear();
                    ledArray1.ClearWiring();
                    if (Communicator.CurrentMode == CommMode.MAVLINK)
                        boxProgressLabel.Text = "Reading Mavlink Parameters..";
                    else
                        boxProgressLabel.Text = "Reading LED Data..";
                }
                else boxProgressLabel.Text = "Writing LED Data..";
                prgBar.Style = ProgressBarStyle.Continuous;
                prgBar.Value = 0;
                prgBar.Visible = true;
                prgReading.Style = ProgressBarStyle.Continuous;
                prgReading.Value = 0;
            }
            btnSend.Enabled = false;
        }

        //Communications completed. Update storage, trigger cascades and finalise UI
        private void comms_Completed(object sender, CompletedEventArgs e)
        {
            btnSend.Enabled = true;
            if (e.Type == CommType.SENDING)
            {
                if (e.Success)
                    for (int index = 0; index < 32; index++)
                        ledUpdate(index);
                if (modeColorDialog != null) modeColorDialog.Send();
                else if (paletteDialog != null) paletteDialog.Send();
                else if (pixSettingsDialog != null) pixSettingsDialog.Send();
            }
            else if (e.Type == CommType.BOOTING && e.Success && Properties.Settings.Default.pgmGetOnConnect) 
                initWaitingWindows();
            else if (e.Type == CommType.RECEIVING && e.Success)
            {
                ledArray1.Enabled = true;
                if (refreshAllWindows && Communicator.CurrentMode == CommMode.CLI)
                {
                    if (modeColorDialog != null) modeColorDialog.refreshAllModes();
                    else if (paletteDialog != null) paletteDialog.refreshAllColors();
                    else if (pixSettingsDialog != null) pixSettingsDialog.refreshSettings();
                }
            }
            prgBar.Visible = false;
            boxProgress.Visible = false;
            Modified = false;
        }

        //Communications progress changed
        private void comms_ProgressChanged(object sender, ProgressEventArgs e)
        {
            if (e.Progress != 0)
            {
                boxProgress.Visible = true;
                prgReading.ValueFast(e.Progress);
            }
            if (e.Event != "") Status = e.Event;
            prgBar.ValueFast(e.Progress);
        }

        //Communicator wants the CLI command used to read LED data
        private string comms_ReadCommand(object sender, CommandEventArgs e)
        {
            return "led";
        }

        //Communicator wants the CLI command used to update LED data
        // Return empty string if LED entry has not been modified 
        private string comms_WriteCommand(object sender, CommandEventArgs e)
        {
            LedControl led = null;
            int arrayIndex = -1;
            for (int ai = 0; ai < 256; ai++)
                if (ledArray1.Leds[ai].Index == e.Index) arrayIndex = ai;
            if (arrayIndex == -1 && getStoredLed(e.Index).isEmpty()) return "";
            if (arrayIndex >= 0)
            {
                led = ledArray1.Leds[arrayIndex];
                if (getStoredLed(e.Index).Compare(led, arrayIndex)) return "";
            } 
            else led = null;  //Not empty, delete
            return constructLedCommand(e.Index, arrayIndex, led);
        }

        //Communicator notification that the current mode (CLI/Mavlink) has changed
        // Used to update the window title and dis/enable the terminal
        private void comms_ModeChanged(object sender, ModeChangedEventArgs e)
        {
            updateTitle();
            if (e.Mode == CommMode.CLI) lblBeats.Image = Properties.Resources.application_xp_terminal;
            if (terminal != null)
            {
                if (e.Mode == CommMode.MAVLINK)
                    terminal.IsEnabled = false;
            }
        }

        //CLI response to a read command
        private void comms_GotData(object sender, GotDataEventArgs e)
        {
            ledArray1.Clear();
            ledArray1.ClearWiring();
            clearStore();

            if (e.Data.Contains("led "))
            {
                if (Parse(e.Data, false))
                    Status = "Configuration: Got LED data OK.";
                else comms.Retry();
            }
            else
            {
                Status = "Configuration: LED data empty.";
                clearStore();
            }
        }

        //Mavlink response to a read command
        // This is special as it may receive results from a full parameter list fetch.
        // Needs to dispatch parameters for other data groups to the other windows.
        bool readingMavlinkParams = false;  //One-shot flag
        private void comms_GotMavlinkParam(object sender, MavParameterEventArgs e)
        {
            MAVLink.mavlink_param_value_t param = e.Param;
            //Trigger the one-shot flag on first pass
            if (e.GetAllMavlinkParams && !readingMavlinkParams)
            {
                readingMavlinkParams = true;
                //Ensure all data windows are started
                //Modes
                if (modeColorDialog == null)
                {
                    modeColorDialog = new frmModeColor(serialControl1);
                    startWindow(modeColorDialog, "modeWindowState", "modeWindowLocation", "", "modeShowing");
                }
                //Palette
                if (paletteDialog == null)
                {
                    paletteDialog = new frmPalette(serialControl1);
                    startWindow(paletteDialog, "paletteWindowState", "paletteWindowLocation", "", "paletteShowing");
                }
                //Settings
                if (pixSettingsDialog == null)
                {
                    pixSettingsDialog = new frmPixSettings(serialControl1);
                    startWindow(pixSettingsDialog, "pixsetWindowState", "pixsetWindowLocation", "", "pixsetShowing");
                }
                //Start notification
                pixSettingsDialog.MavlinkStarted();
                modeColorDialog.MavlinkStarted();
                paletteDialog.MavlinkStarted();
            }
            if (param.param_index < MAV_LED_START)
                pixSettingsDialog.MavlinkParameter(param.param_index, param.param_value);
            if (param.param_index >= MAV_LED_START && param.param_index < MAV_MODE_START)
                MavlinkParameter(param.param_index, param.param_value);
            if (param.param_index >= MAV_MODE_START && param.param_index < MAV_COLOR_START)
                modeColorDialog.MavlinkParameter(param.param_index, param.param_value);
            if (param.param_index >= MAV_COLOR_START && param.param_index <= MAV_PARAM_LAST)
                paletteDialog.MavlinkParameter(param.param_index, param.param_value);

            if (param.param_index == MAV_PARAM_LAST)
                readingMavlinkParams = false;
        }


        private MAVLink.mavlink_param_set_t comms_WriteMavlink(object sender, CommandEventArgs e)
        {
            LedControl led = null;
            MAVLink.mavlink_param_set_t param = new MAVLink.mavlink_param_set_t();
            int arrayIndex = -1;
            for (int ai = 0; ai < 256; ai++)
                if (ledArray1.Leds[ai].Index == e.Index) arrayIndex = ai;
            if (arrayIndex == -1 && getStoredLed(e.Index).isEmpty()) return param;
            if (arrayIndex >= 0)
            {
                int x = arrayIndex % 16;
                int y = arrayIndex / 16;
                int flags = 0;
                led = ledArray1.Leds[arrayIndex];
                if (getStoredLed(e.Index).Compare(led, arrayIndex)) return param;
                if (led.Direction.NORTH) flags += LED_DIR_NORTH;
                if (led.Direction.SOUTH) flags += LED_DIR_SOUTH;
                if (led.Direction.EAST) flags += LED_DIR_EAST;
                if (led.Direction.WEST) flags += LED_DIR_WEST;
                if (led.Direction.UP) flags += LED_DIR_UP;
                if (led.Direction.DOWN) flags += LED_DIR_DOWN;
                if (led.Mode.INDICATOR) flags += LED_FUNC_INDICATOR;
                if (led.Mode.WARNING) flags += LED_FUNC_WARNING;
                if (led.Mode.MODE) flags += LED_FUNC_FLIGHT_MODE;
                if (led.Mode.ARM) flags += LED_FUNC_ARM_STATE;
                if (led.Mode.THROTTLE) flags += LED_FUNC_THROTTLE;
                if (led.Mode.RING) flags += LED_FUNC_THRUST_RING;
                if (led.Mode.COLOR) flags += LED_FUNC_COLOR;
                if (led.Mode.GPS) flags += LED_FUNC_GPS;
                param.target_system = comms.sysid;
                param.target_component = comms.compid;
                SetIdString(ref param.param_id, "led_" + e.Index.ToString());
                param.param_type = (byte)MAVLink.MAV_PARAM_TYPE.UINT32;
                param.param_value = BitConverter.ToSingle(new byte[] {
                    (byte)((x << 4) + y),
                    (byte)(led.ColorIndex),
                    (byte)(flags & 0xff),
                    (byte)(flags >> 8)}, 0);
            }
            else
            {  //Not empty, delete
                param.target_system = comms.sysid;
                param.target_component = comms.compid;
                SetIdString(ref param.param_id, "led_" + e.Index.ToString());
                param.param_value = BitConverter.ToSingle(new byte[] { 0, 0, 0, 0 }, 0);
            }            
            return param;
        }

        public void SetIdString(ref byte[] id, string str)
        {
            int i;
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            id = new byte[16];
            for (i = 0; i < bytes.Length && i < 15; i++)
                id[i] = bytes[i];
            id[i] = 0;
        }

        #endregion


        //------------------------------------------------------------------------------------------------
        // Parsers and generators

        private string constructLedCommand(int index, int arrayIndex, LedControl led)
        {
            string command;
            command = "led " + index.ToString() + " ";
            if (led == null || arrayIndex == -1) return command + "0,0:::0";
            int x = arrayIndex % 16;
            int y = arrayIndex / 16;
            command += x.ToString() + "," + y.ToString() + ":";
            if (led.Direction.NORTH) command += "N";
            if (led.Direction.EAST) command += "E";
            if (led.Direction.SOUTH) command += "S";
            if (led.Direction.WEST) command += "W";
            if (led.Direction.UP) command += "U";
            if (led.Direction.DOWN) command += "D";
            command += ":";
            if (led.Mode.INDICATOR) command += "I";
            if (led.Mode.WARNING) command += "W";
            if (led.Mode.MODE) command += "F";
            if (led.Mode.ARM) command += "A";
            if (led.Mode.THROTTLE) command += "T";
            if (led.Mode.RING) command += "R";
            if (led.Mode.COLOR) command += "C";
            if (led.Mode.GPS) command += "G";
            command += ":" + led.ColorIndex.ToString();
            return command;
        }

        private bool Parse(string data, bool updating)
        {
            string[] leds;
            data = data.Replace("\r", "");
            if (data.Contains('\n')) leds = data.Split('\n');
            else leds = new string[] { data };
            bool success = true;
            for (int count = 0; count < leds.Length; count++)
            {
                if (leds[count].Contains("led ") && leds[count].Contains(":"))
                {
                    string[] line = leds[count].Split(' ');
                    int index, x, y, color;
                    if (line.Length == 3 && int.TryParse(line[1], out index))
                    {
                        string[] parms = line[2].Split(':');
                        if (parms.Length == 4)
                        {
                            string[] coords = parms[0].Split(',');
                            if (int.TryParse(coords[0], out x) && int.TryParse(coords[1], out y) && int.TryParse(parms[3], out color))
                            {
                                if (x >= 0 && x < 16 && y >= 0 && y < 16)
                                {
                                    int address = y * 16 + x;
                                    //null entry?
                                    if (address == 0 && parms[1].Length == 0 && parms[2].Length == 0 && color == 0)
                                    {
                                        index = -1;
                                        storedLeds.Add(new LedStore());     //null entry
                                    }
                                    else
                                    {
                                        LedControl lc = ledArray1.Leds[address];
                                        lc.ColorIndex = color;
                                        lc.Direction.Set(0);
                                        if (parms[1].Contains('N')) lc.Direction.NORTH = true;
                                        if (parms[1].Contains('S')) lc.Direction.SOUTH = true;
                                        if (parms[1].Contains('E')) lc.Direction.EAST = true;
                                        if (parms[1].Contains('W')) lc.Direction.WEST = true;
                                        if (parms[1].Contains('U')) lc.Direction.UP = true;
                                        if (parms[1].Contains('D')) lc.Direction.DOWN = true;
                                        if (parms[2].Contains('I')) lc.Mode.INDICATOR = true;
                                        if (parms[2].Contains('W')) lc.Mode.WARNING = true;
                                        if (parms[2].Contains('F')) lc.Mode.MODE = true;
                                        if (parms[2].Contains('A')) lc.Mode.ARM = true;
                                        if (parms[2].Contains('T')) lc.Mode.THROTTLE = true;
                                        if (parms[2].Contains('R')) lc.Mode.RING = true;
                                        if (parms[2].Contains('C')) lc.Mode.COLOR = true;
                                        if (parms[2].Contains('G')) lc.Mode.GPS = true;
                                        lc.Index = index;
                                        if (!updating) storedLeds.Add(new LedStore(lc, address));
                                        ledArray1.UpdateRemaining();
                                        Application.DoEvents();
                                    }
                                }
                                else success = false;
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

        private void MavlinkParameter(int index, float value)
        {
            index -= MAV_LED_START;
            byte[] bytes = BitConverter.GetBytes(value);
            int address = (bytes[0] & 0xf) * 16 + ((bytes[0] >> 4) & 0xf);
            int color = bytes[1];
            int flags = bytes[2] + (bytes[3] * 256);
            //null entry?
            if (address == 0 && flags == 0 && color == 0)
            {
                index = -1;
                storedLeds.Add(new LedStore());     //null entry
            }
            else
            {
                LedControl lc = ledArray1.Leds[address];
                lc.ColorIndex = color;
                lc.Direction.Set(0);
                if ((flags & LED_DIR_NORTH) > 0) lc.Direction.NORTH = true;
                if ((flags & LED_DIR_SOUTH) > 0) lc.Direction.SOUTH = true;
                if ((flags & LED_DIR_EAST) > 0) lc.Direction.EAST = true;
                if ((flags & LED_DIR_WEST) > 0) lc.Direction.WEST = true;
                if ((flags & LED_DIR_UP) > 0) lc.Direction.UP = true;
                if ((flags & LED_DIR_DOWN) > 0) lc.Direction.DOWN = true;
                if ((flags & LED_FUNC_INDICATOR) > 0) lc.Mode.INDICATOR = true;
                if ((flags & LED_FUNC_WARNING) > 0) lc.Mode.WARNING = true;
                if ((flags & LED_FUNC_FLIGHT_MODE) > 0) lc.Mode.MODE = true;
                if ((flags & LED_FUNC_ARM_STATE) > 0) lc.Mode.ARM = true;
                if ((flags & LED_FUNC_THROTTLE) > 0) lc.Mode.THROTTLE = true;
                if ((flags & LED_FUNC_THRUST_RING) > 0) lc.Mode.RING = true;
                if ((flags & LED_FUNC_COLOR) > 0) lc.Mode.COLOR = true;
                if ((flags & LED_FUNC_GPS) > 0) lc.Mode.GPS = true;
                lc.Index = index;
                storedLeds.Add(new LedStore(lc, address));
                ledArray1.UpdateRemaining();
                Application.DoEvents();
            }
        }

        //------------------------------------------------------------------------------------------------
        // LED storage

        //Clear LED storage
        private void clearStore()
        {
            storedLeds.Clear();
        }

        private LedStore getStoredLed(int index)
        {
            if (index < storedLeds.Count)
                return storedLeds[index];
            else
                return new LedStore();      //Null
        }

        private void ledUpdate(int index)
        {
            LedControl led = null;
            int arrayIndex = -1;
            for (int ai = 0; ai < 256; ai++)
                if (ledArray1.Leds[ai].Index == index) arrayIndex = ai;
            if (arrayIndex == -1 && getStoredLed(index).isEmpty()) return;
            if (arrayIndex >= 0)
            {
                led = ledArray1.Leds[arrayIndex];
                if (getStoredLed(index).Compare(led, arrayIndex)) return;
                if (index >= storedLeds.Count)
                {
                    while (index > storedLeds.Count) storedLeds.Add(new LedStore());
                    storedLeds.Add(new LedStore(led, arrayIndex));
                }
                else storedLeds[index].update(led, arrayIndex);
            }
            else storedLeds[index].nullOut();
        }

        //--------------------------------------------------------------
        // Dull housekeeping chores below

        private void startWindow(Form form, string state, string location, string size, string showing)
        {
            form.Opacity = 0;
            form.Show();
            restoreWindow(form, state, location, size);
            if (showing != "" && !((bool)Properties.Settings.Default[showing])) form.Hide();
            form.Opacity = 100;
        }

        //Save location/size of main window to properties
        private void SaveWindow(Form form, string state, string location, string size)
        {
            Properties.Settings.Default[state] = form.WindowState.ToString();
            if (form.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default[location] = form.Location;
                if (size != "") Properties.Settings.Default[size] = form.Size;
            }
            else
            {
                Properties.Settings.Default[location] = form.RestoreBounds.Location;
                if (size != "") Properties.Settings.Default[size] = form.RestoreBounds.Size;
            }
        }

        private void SaveWindow(Form form, string state, string location, string size, string showing)
        {
            Properties.Settings.Default[showing] = form.Visible;
            SaveWindow(form, state, location, size);
        }
        
        //Restore main window location/size
        private void restoreWindow(Form form, string state, string location, string size)
        {
            if (((size != "" && ((Size)Properties.Settings.Default[size]).Height != 0 
                && ((Size)Properties.Settings.Default[size]).Width != 0) || size == "")
                && Properties.Settings.Default.setRestorePosition)
            {
                if (((string)Properties.Settings.Default[state]) == "Maximized") form.WindowState = FormWindowState.Maximized;
                else if (((string)Properties.Settings.Default[state]) == "Minimized") form.WindowState = FormWindowState.Minimized;
                else form.WindowState = FormWindowState.Normal;
                form.Location = ((Point)Properties.Settings.Default[location]);
                if (size != "") form.Size = ((Size)Properties.Settings.Default[size]);
            }
            Application.DoEvents();
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (updateForm != null && !updateForm.IsDisposed) updateForm.Close(); 
            updateForm = new frmUpdate("");
            updateForm.Show();
        }

        //Mavlink parameter ids
        public static int MAV_VERSION = 0;
        public static int MAV_SYSID = 1;
        public static int MAV_HEARTBEAT = 2;
        public static int MAV_BRIGHTNESS = 3;
        public static int MAV_ANIMATION = 4;
        public static int MAV_LOWBATT = 5;
        public static int MAV_LOWPCT = 6;
        public static int MAV_MINSATS = 7;
        public static int MAV_DEADBAND = 8;
        public static int MAV_BAUD = 9;
        public static int MAV_SOFTBAUD = 10;
        public static int MAV_LAMPTEST = 11;
        public static int MAV_FACTORY = 12;
        public static int MAV_REBOOT = 13;
        public static int MAV_SET_LAST = MAV_REBOOT;
        public static int MAV_LED_START = 14;
        public static int MAV_LED_0 = MAV_LED_START;
        public static int MAV_LED_1 = MAV_LED_START + 1;
        public static int MAV_LED_2 = MAV_LED_START + 2;
        public static int MAV_LED_3 = MAV_LED_START + 3;
        public static int MAV_LED_4 = MAV_LED_START + 4;
        public static int MAV_LED_5 = MAV_LED_START + 5;
        public static int MAV_LED_6 = MAV_LED_START + 6;
        public static int MAV_LED_7 = MAV_LED_START + 7;
        public static int MAV_LED_8 = MAV_LED_START + 8;
        public static int MAV_LED_9 = MAV_LED_START + 9;
        public static int MAV_LED_10 = MAV_LED_START + 10;
        public static int MAV_LED_11 = MAV_LED_START + 11;
        public static int MAV_LED_12 = MAV_LED_START + 12;
        public static int MAV_LED_13 = MAV_LED_START + 13;
        public static int MAV_LED_14 = MAV_LED_START + 14;
        public static int MAV_LED_15 = MAV_LED_START + 15;
        public static int MAV_LED_16 = MAV_LED_START + 16;
        public static int MAV_LED_17 = MAV_LED_START + 17;
        public static int MAV_LED_18 = MAV_LED_START + 18;
        public static int MAV_LED_19 = MAV_LED_START + 19;
        public static int MAV_LED_20 = MAV_LED_START + 20;
        public static int MAV_LED_21 = MAV_LED_START + 21;
        public static int MAV_LED_22 = MAV_LED_START + 22;
        public static int MAV_LED_23 = MAV_LED_START + 23;
        public static int MAV_LED_24 = MAV_LED_START + 24;
        public static int MAV_LED_25 = MAV_LED_START + 25;
        public static int MAV_LED_26 = MAV_LED_START + 26;
        public static int MAV_LED_27 = MAV_LED_START + 27;
        public static int MAV_LED_28 = MAV_LED_START + 28;
        public static int MAV_LED_29 = MAV_LED_START + 29;
        public static int MAV_LED_30 = MAV_LED_START + 30;
        public static int MAV_LED_31 = MAV_LED_START + 31;
        public static int MAV_LED_LAST = MAV_LED_31;
        public static int MAV_MODE_START = MAV_LED_START + 32;
        public static int MAV_MODE_0 = MAV_MODE_START;
        public static int MAV_MODE_1 = MAV_MODE_START + 1;
        public static int MAV_MODE_2 = MAV_MODE_START + 2;
        public static int MAV_MODE_3 = MAV_MODE_START + 3;
        public static int MAV_MODE_4 = MAV_MODE_START + 4;
        public static int MAV_MODE_5 = MAV_MODE_START + 5;
        public static int MAV_MODE_6 = MAV_MODE_START + 6;
        public static int MAV_MODE_7 = MAV_MODE_START + 7;
        public static int MAV_MODE_8 = MAV_MODE_START + 8;
        public static int MAV_MODE_9 = MAV_MODE_START + 9;
        public static int MAV_MODE_10 = MAV_MODE_START + 10;
        public static int MAV_MODE_11 = MAV_MODE_START + 11;
        public static int MAV_MODE_12 = MAV_MODE_START + 12;
        public static int MAV_MODE_13 = MAV_MODE_START + 13;
        public static int MAV_MODE_14 = MAV_MODE_START + 14;
        public static int MAV_MODE_15 = MAV_MODE_START + 15;
        public static int MAV_MODE_16 = MAV_MODE_START + 16;
        public static int MAV_MODE_17 = MAV_MODE_START + 17;
        public static int MAV_MODE_18 = MAV_MODE_START + 18;
        public static int MAV_MODE_19 = MAV_MODE_START + 19;
        public static int MAV_MODE_20 = MAV_MODE_START + 20;
        public static int MAV_MODE_LAST = MAV_MODE_20;
        public static int MAV_COLOR_START = MAV_MODE_START + 21;
        public static int MAV_COLOR_0 = MAV_COLOR_START;
        public static int MAV_COLOR_1 = MAV_COLOR_START + 1;
        public static int MAV_COLOR_2 = MAV_COLOR_START + 2;
        public static int MAV_COLOR_3 = MAV_COLOR_START + 3;
        public static int MAV_COLOR_4 = MAV_COLOR_START + 4;
        public static int MAV_COLOR_5 = MAV_COLOR_START + 5;
        public static int MAV_COLOR_6 = MAV_COLOR_START + 6;
        public static int MAV_COLOR_7 = MAV_COLOR_START + 7;
        public static int MAV_COLOR_8 = MAV_COLOR_START + 8;
        public static int MAV_COLOR_9 = MAV_COLOR_START + 9;
        public static int MAV_COLOR_10 = MAV_COLOR_START + 10;
        public static int MAV_COLOR_11 = MAV_COLOR_START + 11;
        public static int MAV_COLOR_12 = MAV_COLOR_START + 12;
        public static int MAV_COLOR_13 = MAV_COLOR_START + 13;
        public static int MAV_COLOR_14 = MAV_COLOR_START + 14;
        public static int MAV_COLOR_15 = MAV_COLOR_START + 15;
        public static int MAV_COLOR_LAST = MAV_COLOR_15;
        public static int MAV_PARAM_LAST = MAV_COLOR_LAST;

        //LED flags mask
        public static int LED_DIR_NORTH = 1;
        public static int LED_DIR_EAST = 2;
        public static int LED_DIR_SOUTH = 4;
        public static int LED_DIR_WEST = 8;
        public static int LED_DIR_UP = 16;
        public static int LED_DIR_DOWN = 32;
        public static int LED_FUNC_INDICATOR = 64;
        public static int LED_FUNC_WARNING = 128;
        public static int LED_FUNC_FLIGHT_MODE = 256;
        public static int LED_FUNC_ARM_STATE = 512;
        public static int LED_FUNC_THROTTLE = 1024;
        public static int LED_FUNC_THRUST_RING = 2048;
        public static int LED_FUNC_COLOR = 4096;
        public static int LED_FUNC_GPS = 8192;


    }

    public static class Extensions
    {
        public static void ValueFast(this ToolStripProgressBar progressBar, int value)
        {
            if (value < progressBar.Maximum)    // prevent ArgumentException error on value = 100
            {
                progressBar.Value = value + 1;    // set the value +1
            }

            progressBar.Value = value;    // set the actual value
        }
        public static void ValueFast(this ProgressBar progressBar, int value)
        {
            if (value < progressBar.Maximum)    // prevent ArgumentException error on value = 100
            {
                progressBar.Value = value + 1;    // set the value +1
            }

            progressBar.Value = value;    // set the actual value
        }
    }

}
