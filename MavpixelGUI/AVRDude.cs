/*
    BitBurner AVR Programmer - a frontend for AVRdude
    Copyright (C) 2012 Nick Metcalfe

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading;

namespace MavpixelGUI
{
    public class AVRDude
    {
        BackgroundWorker AVRdudeWorker;
        public bool AVRdudeOK;
        public string AVRdudeName;
        Form form1;

        public AVRDude()
        {
            AVRdudeOK = false;
            AVRdudeName = "";
            AVRdudeWorker = new BackgroundWorker();
            AVRdudeWorker.WorkerReportsProgress = true;
            AVRdudeWorker.WorkerSupportsCancellation = true;
            AVRdudeWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
            form1 = (Form)Application.OpenForms["Form1"];
            if (form1 == null) form1 = (Form)Application.OpenForms["frmFlasher"];
        }

        //Error management
        List<string> avrDudeErrors; //One error per entry, can contain multiple lines

        public List<string> Errors
        {
            get { return avrDudeErrors; }
        }

        //Call this early to init AVRdude
        public string checkAvrDude()
        {
            string response = "";
            //No AVRdude registered
            if (Properties.Settings.Default.avrDude == null || Properties.Settings.Default.avrDude == "")
            {
                //find embedded AVRdude
                string programLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (File.Exists(programLocation + "\\avrdude.exe"))
                {
                    response += "\r\nUsing internal AVRdude.";
                    Properties.Settings.Default.avrDude = programLocation + "\\avrdude.exe";
                    Properties.Settings.Default.avrConfig = programLocation + "\\avrdude.conf";
                    AVRdudeOK = true;
                }
                else //not found?
                {
                    //Look for WinAVR
                    response += "\r\nChecking for WinAVR..";
                    string winAvrPath = null;
                    RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WinAVR");
                    if (key == null) key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\WinAVR");
                    if (key != null)
                    {
                        string[] names = key.GetValueNames();
                        if (names.Length == 1)
                        {
                            response += "WinAVR-" + names[0];
                            winAvrPath = (string)key.GetValue(names[0]);
                            Properties.Settings.Default.avrDude = winAvrPath + "\\bin\\avrdude.exe";
                            if (Properties.Settings.Default.avrConfig == null || Properties.Settings.Default.avrConfig == "")
                                Properties.Settings.Default.avrConfig = winAvrPath + "\\bin\\avrdude.conf";
                            //Check AVRdude
                            response += "\r\nChecking for AVRdude executable..";
                            if (!File.Exists(Properties.Settings.Default.avrDude)) response += "Not Found!";
                            else
                            {
                                response += "Found.";
                                AVRdudeOK = true;
                            }
                        }
                    }
                    else response += "WinAVR Not installed!";
                }
            }
            //Something is registered, check it still exists
            else if (File.Exists(Properties.Settings.Default.avrDude))
                AVRdudeOK = true;
            else AVRdudeOK = false;
            if (AVRdudeOK) AVRdudeName = Path.GetFileName(Properties.Settings.Default.avrDude);
            return response;
        }

        public class AVRdudeVersionEventArgs : EventArgs
        {
            public string Version;
            public AVRdudeVersionEventArgs(string version)
            {
                Version = version;
            }
        }
        public delegate void GotAVRdudeVersionEventHandler(object sender, AVRdudeVersionEventArgs e);
        public event GotAVRdudeVersionEventHandler GotAVRdudeVersionEvent;
        public void OnGotAVRdudeVersionEvent(string version)
        {
            if (GotAVRdudeVersionEvent != null)
                GotAVRdudeVersionEvent(this, new AVRdudeVersionEventArgs(version));
        }
        GotAVRdudeVersionEventHandler currentGotAvrDudeVersionEventHandler = null;

        public void getAvrdudeVersion(GotAVRdudeVersionEventHandler gotVersionHandler)
        {
            if (currentGotAvrDudeVersionEventHandler != null)
                GotAVRdudeVersionEvent -= currentGotAvrDudeVersionEventHandler;
            GotAVRdudeVersionEvent += gotVersionHandler;
            avrdudeVersion = "";
            RunAvrDude("-v", AVRdudeWorker_ReadVersion, AVRdudeWorker_ProcessVersion);
        }

        string avrdudeLine;
        string avrdudeVersion;
        //bool gotVersion;
        void AVRdudeWorker_ReadVersion(object sender, ProgressChangedEventArgs e)
        {
            avrdudeLine += (string)e.UserState;
            if (((string)e.UserState) == "\n")
            {
                if (avrdudeLine.Contains("Version") && avrdudeLine.Contains(", compiled on"))
                    avrdudeVersion = avrdudeLine;
                avrdudeLine = "";
            }
        }
        void AVRdudeWorker_ProcessVersion(object sender, RunWorkerCompletedEventArgs e)
        {
            //Check version for errors
            if (!AVRdudeOK) avrdudeVersion = "Cannot execute AVRdude, location not known.";
            else if (avrdudeVersion == null || avrdudeVersion == "")
                avrdudeVersion = "Executable does not appear to be AVRdude.";
            //Keep name of avrdude executable
            if (AVRdudeOK) AVRdudeName = Path.GetFileName(Properties.Settings.Default.avrDude);
            else AVRdudeName = "";
            OnGotAVRdudeVersionEvent(avrdudeVersion.Replace("\r\n", ""));
        }


        //***************************************************************************************
        // AVRdude Executor

        public StringBuilder StandardOutput;
        void ReadStdOut()
        {
            string buffer = "";
            while ((buffer = process.StandardOutput.ReadLine()) != null)
                StandardOutput.Append(buffer + "\r\n");
        }

        delegate void ReportProgress(int percentage, object state);
        void ReadStdErr()
        {
            var buffer = new char[1];
            ReportProgress progress = new ReportProgress(AVRdudeWorker.ReportProgress);
            while (process.StandardError.Read(buffer, 0, 1) > 0 && AVRdudeWorker.IsBusy)
                try { form1.Invoke(progress, new object[] { 0, new string(buffer) }); }
                catch { Errors.Add("Program output truncated!"); }
        }

        Process process;
        public static int RESULT_SUCCESS = 0;
        public static int RESULT_FAILURE = 1;
        public static int FILE_ERROR = 10;

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StandardOutput = new StringBuilder();
            // Start a new process for the cmd
            process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = Properties.Settings.Default.avrDude;
            process.StartInfo.Arguments = (string)e.Argument;
            try
            {
                process.Start();
                // Invoke stdOut and stdErr readers
                new MethodInvoker(ReadStdOut).BeginInvoke(null, null);
                new MethodInvoker(ReadStdErr).BeginInvoke(null, null);
                // Wait for the process to end, or cancel it
                while (!process.HasExited || !process.StandardError.EndOfStream || !process.StandardOutput.EndOfStream)
                {
                    Thread.Sleep(50); // sleep
                    if (AVRdudeWorker.CancellationPending)
                    {
                        process.Kill();
                        break;
                    }
                }
                e.Result = process.ExitCode;
                Thread.Sleep(100);  //Make sure all finished
                process.StandardOutput.DiscardBufferedData(); //Really make sure
                process.StandardError.DiscardBufferedData();
                Thread.Sleep(100);  //really really sure
            }
            catch { e.Result = FILE_ERROR; }
        }

        ProgressChangedEventHandler oldStdOutHandler = null;
        RunWorkerCompletedEventHandler oldCompletedHandler = null;

        public void RunAvrDude(string args, ProgressChangedEventHandler stdOutHandler, RunWorkerCompletedEventHandler completedHandler)
        {
            avrDudeErrors = new List<string>();
            if (AVRdudeOK)
            {
                if (oldStdOutHandler != null) AVRdudeWorker.ProgressChanged -= oldStdOutHandler;
                if (oldCompletedHandler != null) AVRdudeWorker.RunWorkerCompleted -= oldCompletedHandler;
                AVRdudeWorker.ProgressChanged += oldStdOutHandler = stdOutHandler;
                AVRdudeWorker.RunWorkerCompleted += oldCompletedHandler = completedHandler;
                DoRunWorkerStarted();
                AVRdudeWorker.RunWorkerAsync(args);
            }
            //else Errors.Add("AVRdude not found.");
        }

        public bool IsBusy()
        {
            return AVRdudeWorker.IsBusy;
        }

        public void CancelOperation()
        {
            if (AVRdudeWorker.IsBusy) AVRdudeWorker.CancelAsync();
        }

        public void WaitForAVRdude()
        {
            while (IsBusy())
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }
        }

        public void AddCompletedHandler(RunWorkerCompletedEventHandler handler)
        {
            AVRdudeWorker.RunWorkerCompleted += handler;
        }

        public delegate void RunWorkerStartedEventHandler(object sender, EventArgs e);
        public event RunWorkerStartedEventHandler RunWorkerStarted;
        public void DoRunWorkerStarted()
        {
            if (RunWorkerStarted != null) RunWorkerStarted(this, EventArgs.Empty);
        }

    }
}
