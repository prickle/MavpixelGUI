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
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace MavpixelGUI
{
    class FirmwareDownloader
    {
        public BackgroundWorker worker = new BackgroundWorker();
        public WebClient web = new WebClient();

        public FirmwareDownloader()
        {
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }
        
        public void getVersions(string url)
        {
            worker.RunWorkerAsync(url);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FirmwareVersion currentVersion = null;
            FirmwareVersions versions = new FirmwareVersions();
            e.Result = versions;
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader((string)e.Argument);
                reader.MoveToContent();
                string elementName = "";
                if ((reader.NodeType == XmlNodeType.Element) &&
                    (reader.Name == "Mavpixel"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Release")
                        {
                            currentVersion = new FirmwareVersion();
                            currentVersion.Filename = "";
                            versions.Versions.Add(currentVersion);
                        }
                        else if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) &&
                                (reader.HasValue) && (currentVersion != null))
                            {
                                // we check what the name of the node was  
                                switch (elementName)
                                {
                                    case "name":
                                        currentVersion.Name = reader.Value;
                                        break;
                                    case "version":
                                        currentVersion.Version = new Version(reader.Value);
                                        break;
                                    case "url":
                                        currentVersion.DownloadUrl = reader.Value;
                                        break;
                                    case "notes":
                                        //fix line endings
                                        currentVersion.ReleaseNotes = reader.Value.Replace("\r", "").Replace("\n", "\r\n");
                                        break;
                                }
                            }
                        }
                    }
                }
                if (currentVersion == null) throw new Exception("Version information not found in versions.xml!");
            }
            catch (Exception ex)
            {
                versions.Result = "Error fetching version list: " + ex.Message + "\r\n";
            }
            finally
            {
                if (reader != null) reader.Close();
            }

        }

        //Firmware downloader entry point
        // Attach events to 'web' before calling
        public string download(FirmwareVersion version)
        {
            Uri uri = new Uri(version.DownloadUrl);
            string file = Application.UserAppDataPath + "\\" + uri.Segments[uri.Segments.Length - 1];
            web.DownloadFileAsync(uri, file);
            return file;
        }
    }

    class FirmwareVersions
    {
        public string Result = "";
        public List<FirmwareVersion> Versions;
        public FirmwareVersions()
        {
            Versions = new List<FirmwareVersion>();
        }
    }

    class FirmwareVersion
    {
        public string Name;
        public Version Version;
        public string DownloadUrl;
        public string ReleaseNotes;
        public string Filename;
    }
}
