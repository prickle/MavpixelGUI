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
using System.IO;
using System.Reflection;

//Portions extracted from BitBurner AVR programmer

namespace MavpixelGUI
{
    public partial class frmFlasher : Form
    {
        AVRDude avrDude;
        Ports availablePorts;
        FirmwareDownloader downloader;

        Form1 form1;
        string portName { get { return form1.PortName; } }

        public frmFlasher(AVRDude dude)
        {
            InitializeComponent();
            availablePorts = new Ports();   //Create RS232 port interface
            availablePorts.ScanDevices(portScan_RunCompleted);
            avrDude = dude;
        }

        private void frmFlasher_Load(object sender, EventArgs e)
        {
            cbxFlashFile.Text = Properties.Settings.Default.pgmFlash;
            chkEraseEeprom.Checked = Properties.Settings.Default.pgmEraseEeprom;
            cbxFlashFile_updateList(cbxFlashFile.Text);
            form1 = (Form1)Application.OpenForms["Form1"];
            downloader = new FirmwareDownloader();
            txtConsole.AppendText("Looking for AVRDude..");
            txtConsole.AppendText(avrDude.checkAvrDude());
            if (avrDude.AVRdudeOK)
            {
                txtConsole.AppendText("found.");
                avrDude.getAvrdudeVersion(new AVRDude.GotAVRdudeVersionEventHandler(avrDude_GotAVRdudeVersionEvent));
            }
            else txtConsole.AppendText("not found!\r\nChoose a working avrdude.exe in Program Options.");
            downloader.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(downloader_GotVersionsEvent);
            downloader.web.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(web_DownloadProgressChanged);
            downloader.web.DownloadFileCompleted += new AsyncCompletedEventHandler(web_DownloadFileCompleted);
            getVersions();
        }

        //-----------------------------------------------------------------------
        //Initialisation events

        void avrDude_GotAVRdudeVersionEvent(object sender, AVRDude.AVRdudeVersionEventArgs e)
        {
            if (!txtConsole.IsDisposed) txtConsole.AppendText("\r\n" + e.Version);
        }

        //Callback from serial port scan
        private void portScan_RunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cbxPort.Enabled = true;
            if (((List<cbxPortEntry>)e.Result).Count == 0)
            {
                cbxPort.Text = "None";
                return;
            }
            cbxPort.Items.AddRange(((List<cbxPortEntry>)e.Result).ToArray());
            //No selection? Choose something reasonable
            if (Properties.Settings.Default.flashPort == null || Properties.Settings.Default.flashPort == "")
            {
                //Use first available com port
                foreach (cbxPortEntry port in cbxPort.Items)
                    if (port.Name.Contains("COM"))
                    {
                        cbxPort.SelectedItem = port;
                        break;
                    }
            }
            //Restore previous selection
            else cbxPort.Text = Properties.Settings.Default.flashPort;
        }

        //-----------------------------------------------------------------------
        //UI events

        //Serial port description tooltip
        private void cbxPort_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; } // added this line thanks to Andrew's comment
            cbxPortEntry item = (cbxPortEntry)cbxPort.Items[e.Index];
            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            { e.Graphics.DrawString(item.Name, e.Font, br, e.Bounds); }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            { toolTip1.Show(item.Description, cbxPort, e.Bounds.Right, e.Bounds.Bottom); }
            e.DrawFocusRectangle();
        }

        //Re-scan ports when the Ports dropdown is opened
        private void cbxPort_DropDown(object sender, EventArgs e)
        {
            cbxPort.Enabled = false;
            cbxPort.Items.Clear();
            cbxPort.Text = "Scanning..";
            availablePorts.ScanDevices(portScan_RunCompleted);
        }

        private void cbxPort_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(cbxPort);
        }

        private void cbxFlashFile_Enter(object sender, EventArgs e)
        {
            rbnLocalFile.Checked = true;
        }

        private void cbxVersion_Enter(object sender, EventArgs e)
        {
            rbnAvailable.Checked = true;
        }

        public void setFlashFile(string fileName)
        {
            Properties.Settings.Default.pgmFlash = fileName;
            cbxFlashFile.Text = fileName;
            cbxFlashFile_updateList(fileName);
        }

        internal void cbxFlashFile_updateList(string fileName)
        {
            if (Properties.Settings.Default.pgmFlashList == null)
                Properties.Settings.Default.pgmFlashList = new System.Collections.Specialized.StringCollection();
            while (Properties.Settings.Default.pgmFlashList.Count >= Properties.Settings.Default.recentItems)
                Properties.Settings.Default.pgmFlashList.RemoveAt((int)Properties.Settings.Default.recentItems - 1);
            if (fileName != null && fileName != "" && !Properties.Settings.Default.pgmFlashList.Contains(fileName))
                Properties.Settings.Default.pgmFlashList.Insert(0, fileName);
            cbxFlashFile.Items.Clear();
            foreach (string item in Properties.Settings.Default.pgmFlashList)
                cbxFlashFile.Items.Add(item);
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            rbnLocalFile.Checked = true;
            if (Properties.Settings.Default.pgmFlash == null || Properties.Settings.Default.pgmFlash == "")
                Properties.Settings.Default.pgmFlash = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.pgmFlash);
            openFileDialog.FileName = Path.GetFileName(Properties.Settings.Default.pgmFlash);
            openFileDialog.Title = "Select Flash File";
            openFileDialog.Filter = "Known Types|*.hex;*.s;*.s19;*.bin|Intel Hex|*.hex|Motorola S-Record|*.s;*.s19|Binary|*.bin|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                setFlashFile(openFileDialog.FileName);
        }

        private void cbxVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            //bool hasFile = versions.Versions[cbxVersion.SelectedIndex].Filename != "";
            btnFlash.Enabled = (cbxPort.Text != "None");
            txtReleaseNotes.Text = versions.Versions[cbxVersion.SelectedIndex].Name;
            //if (hasFile) txtReleaseNotes.Text += " downloaded, ready to flash.";
            //else txtReleaseNotes.Text += " not yet downloaded.";
            txtReleaseNotes.Text += "\r\n\r\n" + versions.Versions[cbxVersion.SelectedIndex].ReleaseNotes;
            downloadedFile = versions.Versions[cbxVersion.SelectedIndex].Filename;
        }

        private void chkEraseEeprom_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pgmEraseEeprom = chkEraseEeprom.Checked;
        }

        private void rbnAvailable_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnAvailable.Checked) btnFlash.Enabled = (cbxPort.Text != "None");
        }

        private void rbnLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnLocalFile.Checked) 
                btnFlash.Enabled = (cbxFlashFile.Text != "" && cbxPort.Text != "None");
        }

        private void cbxFlashFile_TextChanged(object sender, EventArgs e)
        {
            if (rbnLocalFile.Checked) 
                btnFlash.Enabled = (cbxFlashFile.Text != "" && cbxPort.Text != "None");
        }

        private void btnFlash_Click(object sender, EventArgs e)
        {
            string file;
            btnFlash.Enabled = false;
            if (rbnAvailable.Checked)
            {
                if (cbxVersion.SelectedIndex >= 0)
                {
                    file = versions.Versions[cbxVersion.SelectedIndex].Filename;
                    if (file == "")
                    {
                        //Need to download file
                        downloadedFile = downloader.download(versions.Versions[cbxVersion.SelectedIndex]);
                        Status(LogLevel.Low, "\r\nDownloading " + Path.GetFileName(downloadedFile) + "..");
                        txtConsole.AppendText("\r\nDownloading firmware file '" + Path.GetFileName(downloadedFile) + "'..");
                        return;
                    }
                }
                else
                {
                    Status(LogLevel.Low, "\r\nNothing selected!");
                    btnFlash.Enabled = true;
                    return;
                }
            }
            else file = cbxFlashFile.Text;
            flash(file);
        }

        //-----------------------------------------------------------------------
        //Utilities

        //Get version database
        private void getVersions()
        {
            string url = "https://raw.githubusercontent.com/prickle/Mavpixel/master/versions.xml";
            downloader.getVersions(url);
        }

        FirmwareVersions versions = null;
        void downloader_GotVersionsEvent(object sender, RunWorkerCompletedEventArgs e)
        {
            versions = (FirmwareVersions)e.Result;
            if (versions.Result != "")
            {
                txtConsole.AppendText("\r\n" + versions.Result);
                cbxVersion.Text = "Not Available";
                rbnLocalFile.Checked = true;
            }
            else
            {
                cbxVersion.Enabled = true;
                cbxVersion.Items.Clear();
                foreach (FirmwareVersion version in versions.Versions)
                    cbxVersion.Items.Add(version.Name);
                if (cbxVersion.Items.Count > 0) cbxVersion.SelectedIndex = 0;
            }
            txtReleaseNotes.Enabled = true;
            btnFlash.Enabled = (cbxPort.Text != "None");
        }

        //Download firmware file
        string downloadedFile = "";
        private void download()
        {
            if (cbxVersion.SelectedIndex >= 0)
            {
                cbxVersion.Focus();
                Application.DoEvents();
                downloadedFile = downloader.download(versions.Versions[cbxVersion.SelectedIndex]);
                Status(LogLevel.Low, "\r\nDownloading " + Path.GetFileName(downloadedFile) + "..");
                txtConsole.AppendText("\r\nDownloading firmware file '" + Path.GetFileName(downloadedFile) + "'..");
            }
            else
            {
                downloadedFile = "";
                btnFlash.Enabled = true;
            }
        }

        void web_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (!prgBar.IsDisposed) prgBar.Value = e.ProgressPercentage;
        }

        void web_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (this.IsDisposed) return;        //user closed the window?
            prgBar.Value = 0;
            if (e.Error != null)
            {
                Status(LogLevel.Low, e.Error.Message);
                downloadedFile = "";
                btnFlash.Enabled = true;
            }
            else
            {
                Status(LogLevel.Low, "Download complete.");
                txtConsole.AppendText("done.");
            }
            versions.Versions[cbxVersion.SelectedIndex].Filename = downloadedFile;
            cbxVersion_SelectedIndexChanged(null, null);
            if (e.Error == null) flash(downloadedFile);
        }

        //Programming action entry point.
        private void flash(string file)
        {
            string command;
            if (!File.Exists(file))
            {
                Status(LogLevel.Low, "Error: Flash file not found!");
                btnFlash.Enabled = true;
                return;
            }
            if (cbxPort.Text.Replace(":", "").ToLower() == portName.Replace(":", "").ToLower())
            {
                Status(LogLevel.Low, "Error: Port in use.");
                txtConsole.AppendText("\r\nError: Cannot flash Mavpixel while in use. Please disconnect in the main window and try again.");
                btnFlash.Enabled = true;
                return;
            }
            //Chip erase
            if (chkEraseEeprom.Checked && !eepromErase()) return;
            btnFlash.Enabled = false;                               //will have been reenabled by eepromErase()
            //Flash
            Status(LogLevel.Low, "Flashing firmware..");
            command = avrDudeCommandArgs(file, "flash", avrDudeBasicArgs(""));
            avrDudeRun(command, programmingComplete);
        }

        //Write a blanking file over the EEPROM.
        private bool eepromErase()
        {
            string command;
            string blankFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\blank.eep";
            if (File.Exists(blankFile))
            {
                Status(LogLevel.Low, "Erase EEPROM..");
                command = avrDudeCommandArgs(blankFile, "eeprom", avrDudeBasicArgs(""));
                avrDudeRun(command, programmingComplete);
                avrDude.WaitForAVRdude();
                return avrDude.Errors.Count == 0;
            }
            Status(LogLevel.Low, "Error: blank file not found.");
            return false;
        }

        //-----------------------------------------------------------------------
        //AVRdude control

        //Basic current AVRdude args, with programmer, device etc. Returns null if any unselected.
        // modified to only program Aduino Pro Mini right now.
        public string avrDudeBasicArgs(string command)
        {
            return " -c arduino -p m328p -b 57600" +
                (Properties.Settings.Default.pgmNoVerify ? " -V" : "") +
                " -P " + ((string)cbxPort.Text).Replace(":", "").ToLower() + command;
        }

        //Construct the core of a -U command fragment
        private string avrDudeCommandArgs(string file, string memSpace, string command)
        {
            if (command == null) return null;
            command += " -U \"" + memSpace + ":w:";
            if (file != null && file != "") command += file;
            return command + ":i\"";
        }

        //AVRdude executor entry point
        public void avrDudeRun(string command, RunWorkerCompletedEventHandler runCompleted)
        {
            if (command != null)
            {
                txtConsole.Text += "\r\nAVRdude" + command + "\r\n";
                avrDude.RunAvrDude(command, stdOutToLog, runCompleted);
            }
        }

        //Logging, error detection and progress
        //To be fixed..
        public enum LogLevel { Low = 0, Medium = 1, High = 2, Critical = 3 }
        void Status(LogLevel level, string info)
        {
            info = info.Trim('\n').Trim();
            lblStatus.Text = info; ;
        }

        //Error management
        string stdOutLine = "";                 //Line accumulator
        bool InErrorMessage = false;            //Capture entire multi-line error messages
        string lastChar = "";
        void stdOutToLog(object sender, ProgressChangedEventArgs e)
        {
            if (this.IsDisposed) return;
            bool ErrorFirstLine = false;        //first line of error message (resets itself)
            string ch = (string)e.UserState;
            //Strip repeated crlfs between avrdude output lines in console
            if (ch != "\n")
            {
                if (!(lastChar == "\r" && ch == "\r"))
                {
                    if (ch == "\r") 
                        txtConsole.AppendText("\r\n");
                    else txtConsole.AppendText(ch);
                } lastChar = ch;
            }
            if (ch != "\r")  //line not complete? accumulate..
                stdOutLine += ch;
            else  //Whole new line arrived
            {
                //Parse avrdude responses
                stdOutLine = stdOutLine.Trim('\n') + "\n"; //fix CR
                if (stdOutLine.StartsWith(avrDude.AVRdudeName))
                {
                    //Lots of responses to check for...
                    //These messages are not errors. Ignore them.
                    #region Standard Messages
                    //Ignore Device Ready and standard stuff
                    if (stdOutLine.Contains("AVR device initialized and ready to accept instructions"))
                        Status(LogLevel.Low, "Ready.");
                    else if (stdOutLine.Contains(" Version ")) { }
                    #endregion
                    #region Parameter readjustments
                    //Ignore parameter size readjustments
                    else if (stdOutLine.Contains(" too large, using "))
                        Status(LogLevel.Low, "Parameter Too Large.");
                    else if (stdOutLine.Contains(" too small, using "))
                        Status(LogLevel.Low, "Parameter Too Small.");
                    else if (stdOutLine.Contains(" failure: Frequency too ")) { }
                    else if (stdOutLine.Contains(" bytes requested, but memory region is only ")) { }
                    #endregion
                    #region Power cycle event
                    //Ignore successful power cycle event
                    else if (stdOutLine.Contains(" this device must be powered off and back on to continue")) { }
                    else if (stdOutLine.Contains(" attempting to do this now ..."))
                        Status(LogLevel.Medium, "Reinitializing Chip..");
                    else if (stdOutLine.Contains(" device was successfully re-initialized")) { }
                    #endregion
                    #region Warnings and infos
                    //Ignore warnings & info
                    else if (stdOutLine.Contains(": WARNING: ")) { }
                    else if (stdOutLine.Contains(": warning: ")) { }
                    else if (stdOutLine.Contains(": Warning: ")) { }
                    else if (stdOutLine.Contains(": info: ")) { }
                    else if (stdOutLine.Contains(", serial number: ")) { }
                    else if (stdOutLine.Contains(" Using device VID:PID ")) { }
                    else if (stdOutLine.Contains(" Type C/D found. Setting interface to A")) { }
                    else if (stdOutLine.Contains(" Using USB Interface ")) { }
                    else if (stdOutLine.Contains(" for bitbang delays")) { }
                    else if (stdOutLine.Contains(" Calibrating delay loop...")) { }
                    else if (stdOutLine.Contains(" serial_baud_lookup(): Using non-standard baud rate: ")) { }
                    else if (stdOutLine.Contains(": Send: ")) { }
                    else if (stdOutLine.Contains(": Recv: ")) { }
                    else if (stdOutLine.Contains(" ser_recv(): programmer is not responding,reselecting")) { }
                    #endregion
                    #region BusPirate
                    //Ignore BusPirate info messages
                    else if (stdOutLine.Contains(" buspirate_send_bin():")) { }
                    else if (stdOutLine.Contains(" buspirate_recv_bin():")) { }
                    else if (stdOutLine.Contains(" buspirate_readline():")) { }
                    else if (stdOutLine.Contains(" buspirate_send():")) { }
                    #endregion
                    #region Jtag MKI
                    //Ignore Jtag MKI info messages
                    else if (stdOutLine.Contains(" jtagmkI_send(): sending ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_resync()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkI_resync(): Sending sign-on command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_getsync(): Sending sign-on command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_chip_erase(): Sending chip erase command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_set_devdescr(): Sending ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_reset(): Sending reset command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_set_devdescr(): Sending ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_program_enable(): Sending ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_program_disable(): Sending")) { }
                    else if (stdOutLine.Contains(" jtagmkI_initialize(): trying ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_open()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkI_open(): trying to sync at baud rate ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_open(): succeeded")) { }
                    else if (stdOutLine.Contains(" jtagmkI_close()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkI_close(): trying")) { }
                    else if (stdOutLine.Contains(" jtagmkI_paged_write(..,")) { }
                    else if (stdOutLine.Contains(" jtagmkI_paged_write(): block_size")) { }
                    else if (stdOutLine.Contains(" jtagmkI_close(): trying")) { }
                    else if (stdOutLine.Contains(" jtagmkI_paged_write(): Sending write memory command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_paged_load(..,")) { }
                    else if (stdOutLine.Contains(" jtagmkI_paged_load(): block size")) { }
                    else if (stdOutLine.Contains(" jtagmkI_paged_load(): Sending read memory command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkI_read_byte(..,")) { }
                    else if (stdOutLine.Contains(" jtagmkI_write_byte(..,")) { }
                    else if (stdOutLine.Contains(" jtagmkI_getparm()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkI_setparm()\n")) { }
                    #endregion
                    #region Jtag MKII
                    //Ignore Jtag MKII info messages
                    else if (stdOutLine.Contains(" jtagmkII_send(): sending ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_recv():\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_recv(): msglen ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_recv(): CRC OK")) { }
                    else if (stdOutLine.Contains(" jtagmkII_recv(): Got message")) { }
                    else if (stdOutLine.Contains(" jtagmkII_recv(): got asynchronous event")) { }
                    else if (stdOutLine.Contains(" jtagmkII_getsync()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_getsync(): Sending sign-on command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_getsync(): sign-on command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_getsync(): S_MCU firmware version")) { }
                    else if (stdOutLine.Contains(" jtagmkII_getsync(): ISP activation failed, trying debugWire")) { }
                    else if (stdOutLine.Contains(" Target prepared for ISP, signed off.")) { }
                    else if (stdOutLine.Contains(" Please restart ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_getsync(): Sending get sync command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_set_devdescr(): Sending set device descriptor ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_program_enable(): Sending enter progmode command:")) { }
                    else if (stdOutLine.Contains(" jtagmkII_program_disable(): Sending leave progmode command")) { }
                    else if (stdOutLine.Contains(" jtagmkII_initialize(): trying to set baudrate")) { }
                    else if (stdOutLine.Contains(" jtagmkII_initialize(): trying to set JTAG clock period")) { }
                    else if (stdOutLine.Contains(" jtagmkII_parseextparms(): JTAG chain parsed")) { }
                    else if (stdOutLine.Contains(" jtagmkII_open()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_open_dw()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_open_pdi()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_dragon_open()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_dragon_open_dw()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_dragon_open_pdi()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_close()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_close(): Sending GO command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_close(): Sending sign-off command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_write(.., ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_write(): block_size")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_write(): Sending write memory command:")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_load(..,")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_load(): block_size")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_load(): Sending read memory command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_read_byte(..,")) { }
                    else if (stdOutLine.Contains(" jtagmkII_read_byte(): Sending read memory command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_write_byte(..,")) { }
                    else if (stdOutLine.Contains(" jtagmkII_write_byte(): Sending write memory command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_getparm()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_getparm(): Sending get parameter command")) { }
                    else if (stdOutLine.Contains(" jtagmkII_setparm()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_setparm(): Sending set parameter command")) { }
                    else if (stdOutLine.Contains(" jtagmkII_open()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_avr32_reset(")) { }
                    else if (stdOutLine.Contains(" jtagmkII_reset32(")) { }
                    else if (stdOutLine.Contains(" jtagmkII_chip_erase32()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_read_SABaddr(): OCD Register ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_write_SABaddr(): OCD Register ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_open32()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_close32()\n")) { }
                    else if (stdOutLine.Contains(" jtagmkII_close(): Sending sign-off command: ")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_load32(..,")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_load32(): block_size")) { }
                    else if (stdOutLine.Contains(" jtagmkII_paged_write32(): block_size")) { }
                    #endregion
                    #region stk500
                    //Ignore stk500 info messages
                    else if (stdOutLine.Contains(" stk500_set_vtarget(): reducing V[aref] from ")) { }
                    else if (stdOutLine.Contains(" stk500_set_fosc(): f = ")) { }
                    else if (stdOutLine.Contains(" stk500_set_sck_period(): p = ")) { }
                    else if (stdOutLine.Contains(" stk500_initialize(): programmer not in sync, ")) { }
                    else if (stdOutLine.Contains(" please define PAGEL and BS2 signals in the configuration")) { }
                    else if (stdOutLine.Contains(" successfully opened stk500v1 device")) { }
                    else if (stdOutLine.Contains(" successfully opened stk500v2 device")) { }
                    else if (stdOutLine.Contains(" stk500v2_jtagmkII_recv(): got ")) { }
                    else if (stdOutLine.Contains(" stk500v2_getsync(): got response from unknown programmer ")) { }
                    else if (stdOutLine.Contains(" stk500v2_getsync(): found ")) { }
                    else if (stdOutLine.Contains(" stk500hv_read_byte(..,")) { }
                    else if (stdOutLine.Contains(" stk500hv_read_byte(): Sending read memory command: ")) { }
                    else if (stdOutLine.Contains(" stk500hv_write_byte(..,")) { }
                    else if (stdOutLine.Contains(" stk500hv_write_byte(): Sending write memory command: ")) { }
                    else if (stdOutLine.Contains(" stk500v2_set_vtarget(): reducing V[aref]")) { }
                    else if (stdOutLine.Contains(" stk500v2_set_fosc(): f = ")) { }
                    else if (stdOutLine.Contains(" stk500v2_set_sck_period(): p = ")) { }
                    else if (stdOutLine.Contains(" Skipping paramter write; parameter value already set.")) { }
                    else if (stdOutLine.Contains(" stk500v2_jtagmkII_open()\n")) { }
                    else if (stdOutLine.Contains(" stk500v2_jtagmkII_close()\n")) { }
                    else if (stdOutLine.Contains(" stk500v2_dragon_isp_open()\n")) { }
                    else if (stdOutLine.Contains(" stk500v2_dragon_hv_open()\n")) { }
                    #endregion
                    #region Other programmers
                    else if (stdOutLine.Contains(" seen device from vendor ->")) { }
                    else if (stdOutLine.Contains(" seen product ->")) { }
                    else if (stdOutLine.Contains(" auto set sck period ")) { }
                    else if (stdOutLine.Contains(" try to set SCK period to ")) { }
                    else if (stdOutLine.Contains(" set SCK frequency to ")) { }
                    else if (stdOutLine.Contains(" retries during ")) { }
                    else if (stdOutLine.Contains(" usbdev_open(): Found USBtinyISP, bus:device: ")) { }
                    else if (stdOutLine.Contains(" Warning: cannot open USB device: ")) { }
                    else if (stdOutLine.Contains(" Setting SCK period to ")) { }
                    else if (stdOutLine.Contains(" Using SCK period of ")) { }
                    else if (stdOutLine.Contains(" wiring_open(): snoozing for ")) { }
                    else if (stdOutLine.Contains(" wiring_open(): done snoozing")) { }
                    else if (stdOutLine.Contains(" wiring_open(): releasing DTR/RTS")) { }
                    else if (stdOutLine.Contains(" wiring_open(): asserting DTR/RTS")) { }
                    #endregion
                    #region failures
                    //Ignore non-fatal failures
                    else if (stdOutLine.Contains(" failure: pin list has pins out of range ")) { }
                    else if (stdOutLine.Contains(" failure: conflicting pins in pin list: ")) { }
                    else if (stdOutLine.Contains(" failure: pinning for FTDI MPSSE must be:")) { }
                    else if (stdOutLine.Contains(" failure: RESET pin clashes with data pin or is not set.")) { }
                    else if (stdOutLine.Contains(" Invalid interface ")) { }
                    #endregion
                    #region Signature check & safemode
                    //Ignore signature check and safemode
                    else if (stdOutLine.Contains(" Device signature = ")) { }
                    else if (stdOutLine.Contains(" safemode: Fuse reading not support by programmer.")) { }
                    else if (stdOutLine.Contains(" safemode: fuse changed! Was ")) { }
                    else if (stdOutLine.Contains(" safemode: and is now rescued")) { }
                    else if (stdOutLine.Contains(" safemode: lfuse changed! Was ")) { }
                    else if (stdOutLine.Contains(" safemode: hfuse changed! Was ")) { }
                    else if (stdOutLine.Contains(" safemode: efuse changed! Was ")) { }
                    else if (stdOutLine.Contains(" safemode: lfuse reads as ")) { }
                    else if (stdOutLine.Contains(" safemode: hfuse reads as ")) { }
                    else if (stdOutLine.Contains(" safemode: efuse reads as ")) { }
                    else if (stdOutLine.Contains(" safemode: Fuses OK"))
                        Status(LogLevel.Low, "Fuses OK.");
                    #endregion
                    #region Flash Erase
                    //Ignore flash erase
                    else if (stdOutLine.Contains(" NOTE: FLASH memory has been specified, an erase cycle will be performed")) { }
                    else if (stdOutLine.Contains(" conflicting -e and -n options specified, NOT erasing chip")) { }
                    else if (stdOutLine.Contains(" erasing chip\n"))
                        Status(LogLevel.Low, "Erasing Chip..");
                    #endregion
                    #region Cycle tracking
                    //Ignore cycle tracking message
                    else if (stdOutLine.Contains(" current erase-rewrite cycle count is ")) { }
                    else if (stdOutLine.Contains(" setting erase-rewrite cycle count to ")) { }
                    else if (stdOutLine.Contains(" erase-rewrite cycle count is now "))
                        Status(LogLevel.Medium, "Cycle Count =" + stdOutLine.Substring(stdOutLine.LastIndexOf(' ')));
                    #endregion
                    #region Reading and Writing
                    //Ignore reading and writing messages
                    else if (stdOutLine.Contains(" reading input file "))
                        Status(LogLevel.Low, "Reading File..");
                    else if (stdOutLine.Contains(" reading "))
                        Status(LogLevel.Medium, "Reading " + stdOutLine.Substring(stdOutLine.IndexOf(" reading ") + 9).Replace(":", ".."));
                    else if (stdOutLine.Contains(" writing output file "))
                    { if (!stdOutLine.Contains("<stdout>")) Status(LogLevel.Low, "Writing File.."); }
                    else if (stdOutLine.Contains(" writing "))
                        Status(LogLevel.Medium, "Writing " + stdOutLine.Substring(stdOutLine.IndexOf(" writing ") + 9).Replace(":", ".."));
                    else if (stdOutLine.Contains(" verifying ...")) { }
                    else if (stdOutLine.Contains(" verifying "))
                    {
                        string line = stdOutLine.Substring(stdOutLine.IndexOf(" verifying ") + 11);
                        line = line.Substring(0, line.IndexOf(" against"));
                        Status(LogLevel.Medium, "Verifying " + line + "..");
                    }
                    else if (stdOutLine.Contains(" contains ") && stdOutLine.Contains(" bytes")) { }
                    else if (stdOutLine.Contains(" load data ")) { }
                    else if (stdOutLine.Contains(" verified"))
                        Status(LogLevel.Low, "Verified OK.");
                    else if (stdOutLine.Contains(" auto detected as ")) { }
                    else if (stdOutLine.Contains(" written")) { }
                    #endregion
                    #region Completed message
                    //Ignore completed message
                    else if (stdOutLine.Contains(" done.  Thank you."))
                        Status(LogLevel.Low, "Done.");
                    #endregion
                    //Anything else prefixed with "avrdude.exe" is probably an error.
                    else
                    {
                        InErrorMessage = ErrorFirstLine = true;
                        //Annotate incomprehensible errors
                        if (stdOutLine.Contains("stk500_getsync(): not in sync"))
                            stdOutLine += "No response from Mavpixel. Check the connections and port is correct.";
                    }
                }
                //Errors not prefixed with "avrdude.exe" but worth reporting matched below
                else if (stdOutLine.Contains(" memory type not defined "))
                    avrDude.Errors.Add(stdOutLine);

                //Error message being written, gather..
                if (InErrorMessage)
                    if (!ErrorFirstLine)
                    {
                        if (InErrorMessage = stdOutLine.StartsWith(" "))
                            avrDude.Errors[avrDude.Errors.Count - 1] += "\n  " + stdOutLine.Trim('\n').Trim();
                    }
                    else avrDude.Errors.Add(stdOutLine.Replace(avrDude.AVRdudeName + ": ", "").Trim());

                //Clear line
                stdOutLine = "";
            }
            //Progress bar
            if (stdOutLine.Contains('|'))
            {
                if (stdOutLine.StartsWith("\nReading") || stdOutLine.StartsWith("\nWriting"))
                    prgBar.Value = (stdOutLine.LastIndexOf('#') - stdOutLine.IndexOf('#')) * 2;
            }
        }

        //AVRdude programming operation returns here for housekeeping
        public void programmingComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.IsDisposed) return;
            prgBar.Value = 0;      //clear progress bar
            //if (logItAll) txtLog.Text += "\r\nAVRdude completed, " + avrDude.Errors.Count.ToString() + " errors.";
            //report success or otherwise
            if ((int)e.Result == AVRDude.RESULT_SUCCESS) Status(LogLevel.High, "Flashing successful.");
            else
            {
                Status(LogLevel.High, "Flashing Failed.");
                //pop up the error dialog if appropriate & desireable
                if (avrDude.Errors.Count > 0)// && !Properties.Settings.Default.setIgnoreErrors)
                {
                    frmErrors errors = new frmErrors(avrDude.AVRdudeName, avrDude.Errors);
                    errors.ShowDialog();
                }
            }
            //re-enable programming controls
            btnFlash.Enabled = true;
        }

    }
}
