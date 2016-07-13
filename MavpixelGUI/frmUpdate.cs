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
using System.Diagnostics;
using System.Reflection;

namespace MavpixelGUI
{
    public partial class frmUpdate : Form
    {
        string externalText;
        string xmlURL;
        UpdateWorker worker;

        public frmUpdate(string gotResults)
        {
            InitializeComponent();
            externalText = gotResults;
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            if (externalText != "")
                textBox.Rtf = externalText;
            else
            {
                textBox.Text += "Checking for updates to MavpixelGUI application (v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")..";
                Application.DoEvents();
                //Download CurrentVersion.xml from here
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int Epoch = (int)t.TotalSeconds;
                xmlURL = "http://downloads.sourceforge.net/project/bitburner/CurrentVersion.xml?r=&ts=" + Epoch.ToString() + "&use_mirror=master";
                worker = new UpdateWorker("MavpixelGUI");
                worker.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.checkForUpdate(xmlURL);
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try { Process.Start(e.LinkText); }
            catch { }
        }

        FirmwareDownloader firmwareWorker;
        string txtBox_Text;
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtBox_Text = (string)e.Result;
            textBox.Rtf = txtBox_Text;
            if (frmPixSettings.MavpixelVersion != null)
            {
                textBox.Rtf = txtBox_Text.Trim('}') + UpdateWorker.horizontalBarRtf + @"\par Checking for updates to Mavpixel firmware (v" + frmPixSettings.MavpixelVersion.ToString() + @")..\par}";
                firmwareWorker = new FirmwareDownloader();
                firmwareWorker.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(firmwareWorkerCompleted);
                string url = "https://raw.githubusercontent.com/prickle/Mavpixel/master/versions.xml";
                firmwareWorker.getVersions(url);
            }
            else textBox.Rtf = txtBox_Text.Trim('}') + UpdateWorker.horizontalBarRtf + @"\par Please connect to Mavpixel to also check for firmware updates.\par}";
        }

        void firmwareWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FirmwareVersions versions = (FirmwareVersions)e.Result;
            if (versions.Result != "")
            {
                textBox.Rtf = txtBox_Text.Trim('}') + @"\par " + versions.Result.Replace("\n", @"\par ") + "}";
            }
            else
            {
                FirmwareVersion newVersion = null;
                string result = @"\par ";
                foreach (FirmwareVersion version in versions.Versions)
                    if (version.Version >= frmPixSettings.MavpixelVersion
                        && (newVersion == null || newVersion.Version < version.Version))
                        newVersion = version;
                if (newVersion != null)
                {
                    result += @"{\qc\b\fs20" + UpdateWorker.imageRtf + @" A newer version of Mavpixel (v" + newVersion.Version.ToString() + @") is available.\b0\par\ql" +
                        UpdateWorker.horizontalBarRtf + @"\fs16\par Use the Firmware Flasher to upgrade to the new version.\par\par The firmware image is available from: " + newVersion.DownloadUrl + @"\par";
                    if (newVersion.ReleaseNotes != null) result += @"\par\b Notes for v" + newVersion.Version.ToString() + @":\b0\par " + newVersion.ReleaseNotes.Replace("\n", @"\par ") + "}";
                    else result += "}";
                }
                else result += @"Mavpixel firmware is up to date.}";
                textBox.Rtf = txtBox_Text.Trim('}') + result;
            }
        }
    }
}
