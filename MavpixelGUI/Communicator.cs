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
using System.ComponentModel;
using OpenNETCF.IO.Ports;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace MavpixelGUI
{
    class Communicator : Component
    {
        int timeout = 1000;
        public int Timeout { get { return timeout; } set { timeout = comTimer.Interval = value; } }

        string rootDataName = "";
        public string DataName { get { return rootDataName; } set { rootDataName = value; } }

        int commandsToSend = 1;
        public int CommandsToSend { get { return commandsToSend; } set { commandsToSend = value; } }

        int commandsToRead = 1;
        public int CommandsToRead { get { return commandsToRead; } set { commandsToRead = value; } }

        int mavlinkParameterCount = 1;
        public int MavlinkParameterCount { get { return mavlinkParameterCount; } set { mavlinkParameterCount = value; } }

        int mavlinkParameterStart = 0;
        public int MavlinkParameterStart { get { return mavlinkParameterStart; } set { mavlinkParameterStart = value; } }

        int mavlinkParameterTotal = 1;
        public int MavlinkParameterTotal { get { return mavlinkParameterTotal; } set { mavlinkParameterTotal = value; } }

        //List of received flags for each parameter
        bool[] mavlinkParameterReceived;

        //Global message counter
        public static int MessageCounter;

        //Mode flag
        public static CommMode CurrentMode = CommMode.NONE;

        Form1 form1;
        serialControl serial;

        //Communicator states
        bool waitForBoot = false;
        bool waitForPrompt = false;
        bool sendingCommands = false;
        bool collectingReply = false;
        bool checkEmpty = false;
        bool getAllMavlinkParams = false;

        //Communicator vars
        string singleCommand;
        float mavlinkSingleValue;
        int mavlinkStart;
        int mavlinkCount;
        string dataName;
        string mavPixReply;
        int sentCommandIndex;
        int readCommandIndex;
        CommType commType;
        int lastReply = -1;
        int comAttempts = 0;

        private System.Windows.Forms.Timer comTimer;
        private IContainer components;

        public Communicator()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Runtime)
            {
                // Don't do this in the designer - very bad form
                form1 = (Form1)Application.OpenForms["Form1"];
                if (form1 != null) form1.DataReceived += new Form1.DataReceivedEventHandler(DataReceived);
                if (form1 != null) serial = form1.serialControl1;
                if (form1 != null) form1.MavMessageEvent += new Form1.MavMessageEventHandler(MavMessageEvent);
                if (form1 != null) form1.CommsClosed += new Form1.CommsClosedEventHandler(CommsClosed);
            }
            comTimer.Interval = timeout;
        }

        //--------------------------------------------------------------
        // Public functions

        //Attach event handlers
        //Only needed for main form communicator
        public void Setup()
        {
            form1 = (Form1)Application.OpenForms["Form1"];
            form1.DataReceived += new Form1.DataReceivedEventHandler(DataReceived);
            serial = form1.serialControl1;
            form1.MavMessageEvent += new Form1.MavMessageEventHandler(MavMessageEvent);
            form1.CommsClosed += new Form1.CommsClosedEventHandler(CommsClosed);
        }

        int bootCounter;
        //Just connected, expecting "Press <Enter> 3 times for CLI." message to arrive.
        // or, if Mavlink is active, wait for heartbeat
        public void ConnectAndGetPrompt()
        {
            commType = CommType.BOOTING;
            if (checkPort()) return;
            comAttempts = 0;
            bootCounter = 0;
            dataName = "prompt/heartbeat";
            mavPixReply = "";
            startTimer(ref waitForBoot, timeout + 3000);
        }

        //Just connected, expecting "Press <Enter> 3 times for CLI." message to arrive.
        //Proceed to collexting data
        public void ConnectAndGetAllData()
        {
            commType = CommType.RECEIVING;
            if (checkPort()) return;
            comAttempts = 0;
            bootCounter = 0;
            readCommandIndex = 0;
            dataName = DataName;
            mavPixReply = "";
            getAllMavlinkParams = true;
            initMavlinkParameterReceived(0, mavlinkParameterTotal);
            startTimer(ref waitForBoot);
        }

        //Trigger fetching of all parameters
        public void GetAllData()
        {
            getAllMavlinkParams = true;
            initMavlinkParameterReceived(0, mavlinkParameterTotal);
            comAttempts = 0;
            readCommandIndex = 0;
            getData();
        }

        //Regular fetching of data.
        public void GetData()
        {
            getAllMavlinkParams = false;
            initMavlinkParameterReceived(mavlinkParameterStart, mavlinkParameterCount);
            comAttempts = 0;
            readCommandIndex = 0;
            getData();
        }

        //Common GetData entry point
        void getData()
        {
            commType = CommType.RECEIVING;
            if (checkPort()) return;
            dataName = rootDataName;            
            mavPixReply = "";
            if (CurrentMode == CommMode.CLI)
            {
                crlfPress();
                startTimer(ref waitForPrompt);
            }
            else if (CurrentMode == CommMode.MAVLINK)
            {
                OnComStarted(commType);
                if (getAllMavlinkParams)
                    OnProgressChanged(commType, 0, "Configuration: Fetching Mavlink parameters..");
                else
                    OnProgressChanged(commType, 0, "Configuration: Fetching " + dataName + "..");
                mavlinkNextParamFetch();
            }
        }

        //Sending of new data.
        public void SendCommand()
        {
            commType = CommType.SENDING;
            if (checkPort()) return;
            singleCommand = "";
            dataName = rootDataName;
            OnComStarted(commType);
            for (sentCommandIndex = 0; sentCommandIndex < commandsToSend; sentCommandIndex++)
            {
                if (CurrentMode == CommMode.MAVLINK && mavlinkWriteCommand(sentCommandIndex)) return;
                else if (CurrentMode == CommMode.CLI && serialWriteCommand(sentCommandIndex)) return;
            }
            //No commands to send
            if (MessageCounter == 0)    //Have any messages been sent since the global counter was reset?
                OnProgressChanged(commType, 0, "Configuration: No changes, nothing to send.");
            OnCompleted(commType, true, 0);
        }

        //Send a single command
        public void SendCommand(string Command, bool ExpectReply)
        {
            commType = CommType.SINGLE;
            if (checkPort()) return;
            singleCommand = Command;
            dataName = Command;
            sentCommandIndex = 0;
            comAttempts = 0;
            OnComStarted(commType);
            OnProgressChanged(commType, 0, "Configuration: Sent " + dataName + ".");
            mavPixReply = "";
            serial.WriteLine(Command);
            if (ExpectReply) startTimer(ref sendingCommands);
        }

        //Send a single mavlink parameter
        public void SendParameter(string Command, float value)
        {
            commType = CommType.SINGLE;
            if (checkPort()) return;
            singleCommand = Command;
            dataName = Command;
            mavlinkSingleValue = value;
            OnComStarted(commType);
            mavlinkWriteCommand(-1);
        }

        //Send a single command and wait for the prompt.
        public void SendCommand(string Command)
        {
            SendCommand(Command, true);
        }

        //Try to repeat the last command (use when CLI parsing fails)
        public void Retry()
        {
            int progress = (int)((float)sentCommandIndex / (float)commandsToSend * 32);
            if (comAttempts <= Properties.Settings.Default.comRetries)
            {
                OnProgressChanged(commType, progress, "Configuration - attempt " + comAttempts.ToString() + ": Parse error fetching " + dataName + ".");
                comAttempts++;
                crlfPress();
                startTimer(ref waitForPrompt);
            }
            else
            {
                OnProgressChanged(commType, progress, "Configuration: Parse error fetching " + dataName + ".");
                OnCompleted(commType, false, sentCommandIndex);
            }
        }

        public void Cancel()
        {
            if (waitForBoot || waitForPrompt || collectingReply || sendingCommands || checkEmpty)
            {
                stopTimer();
                OnProgressChanged(commType, 32, "Configuration - Cancelled.");
                OnCompleted(commType, false, sentCommandIndex);
            }
        }

        //--------------------------------------------------------------
        // Events

        //Boot completed - use after connect or reset
        public delegate void BootCompletedEventHandler(object sender, EventArgs e);
        public event BootCompletedEventHandler BootCompleted;
        public void OnBootCompleted()
        {
            if (BootCompleted != null) BootCompleted(this, EventArgs.Empty);
        }

        //Communication started - use to disable UI portions
        public delegate void ComStartedEventHandler(object sender, ComStartedEventArgs e);
        public event ComStartedEventHandler ComStarted;
        public void OnComStarted(CommType Type)
        {
            if (ComStarted != null) ComStarted(this, new ComStartedEventArgs(Type));
        }

        //Got a line of data event - use to feed to a parser
        public delegate void GotDataEventHandler(object sender, GotDataEventArgs e);
        public event GotDataEventHandler GotData;
        public void OnGotData(int Index, string Data)
        {
            if (GotData != null) GotData(this, new GotDataEventArgs(Index, Data));
        }

        //Got a Mavlink message event
        public delegate void GotMavlinkEventHandler(object sender, MavParameterEventArgs e);
        public event GotMavlinkEventHandler GotMavlinkParam;
        public void OnGotMavlinkParam(MAVLink.mavlink_param_value_t param, bool GetAllMavlinkParams)
        {
            if (GotMavlinkParam != null) GotMavlinkParam(this, new MavParameterEventArgs(param, GetAllMavlinkParams));
        }

        //Get the command used for reading data
        public delegate string ReadCommandEventHandler(object sender, CommandEventArgs e);
        public event ReadCommandEventHandler ReadCommand;
        public string OnReadCommand(int Index)
        {
            if (ReadCommand != null) return ReadCommand(this, new CommandEventArgs(Index));
            return "";
        }

        //Get the CLI command used to write and update data
        public delegate string WriteCommandEventHandler(object sender, CommandEventArgs e);
        public event WriteCommandEventHandler WriteCommand;
        public string OnWriteCommand(int Index)
        {
            if (WriteCommand != null) return WriteCommand(this, new CommandEventArgs(Index));
            return "";
        }

        //Get the Mavlink command used to write and update data
        public delegate MAVLink.mavlink_param_set_t WriteMavlinkEventHandler(object sender, CommandEventArgs e);
        public event WriteMavlinkEventHandler WriteMavlink;
        public MAVLink.mavlink_param_set_t OnWriteMavlink(int Index)
        {
            if (WriteMavlink != null) return WriteMavlink(this, new CommandEventArgs(Index));
            return new MAVLink.mavlink_param_set_t();
        }

        //Report progress and events like retries
        public delegate void ProgressChangedEventHandler(object sender, ProgressEventArgs e);
        public event ProgressChangedEventHandler ProgressChanged;
        public void OnProgressChanged(CommType Type, int Progress, string Event)
        {
            if (Progress < 0) Progress = 0;         //Avoid errors from malformed data
            if (Progress > 32) Progress = 32;
            if (ProgressChanged != null) ProgressChanged(this, new ProgressEventArgs(Type, Progress, Event));
        }

        //Task completed - use to report success or failure, reenable UI, update storage..
        public delegate void CompletedEventHandler(object sender, CompletedEventArgs e);
        public event CompletedEventHandler Completed;
        public void OnCompleted(CommType Type, bool Success, int lastIndex)
        {
            if (Completed != null) Completed(this, new CompletedEventArgs(Type, Success, lastIndex));
        }

        //Task completed - use to report success or failure, reenable UI, update storage..
        public delegate void ModeChangedEventHandler(object sender, ModeChangedEventArgs e);
        public event ModeChangedEventHandler ModeChanged;
        public void DoModeChange(CommMode Mode)
        {
            if (CurrentMode != Mode)
            {
                CurrentMode = Mode;
                if (ModeChanged != null) ModeChanged(this, new ModeChangedEventArgs(Mode));
            }
        }

        //--------------------------------------------------------------
        // Serial communications

        private bool checkPort()
        {
            if (!serial.IsOpen)
            {
                OnProgressChanged(commType, 32, "Configuration: Serial port not open.");
                return true;
            }
            else return false;
        }

        //Ping for a prompt
        private void crlfPress()
        {
            byte[] data = { (byte)'\r', (byte)'\n' };
            if (serial.IsOpen) serial.Write(data, 2);
        }

        //Retrieve and dispatch the CLI write data command
        private bool serialWriteCommand(int sentCommandIndex)
        {
            string command = OnWriteCommand(sentCommandIndex);
            if (command != "")
            {
                int progress = (int)((float)sentCommandIndex / (float)commandsToSend * 32);
                comAttempts = 0;
                OnProgressChanged(commType, progress, "Configuration: Sending " + dataName + "..");
                mavPixReply = "";
                serial.WriteLine(command);
                startTimer(ref sendingCommands);
                MessageCounter++;
                return true;
            }
            return false;
        }

        //--------------------------------------------------------------
        // Mavlink communications

        //Init the parameter received flag list to all false
        // Also set up the mavlink parameter index window
        void initMavlinkParameterReceived(int start, int count)
        {
            mavlinkParameterReceived = Enumerable.Repeat<bool>(false, count).ToArray();
            mavlinkStart = start;
            mavlinkCount = count;
        }

        private MAVLink.mavlink_param_set_t mavlinkSingleParameter()
        {
            MAVLink.mavlink_param_set_t param = new MAVLink.mavlink_param_set_t();
            param.target_system = sysid;
            param.target_component = compid;
            form1.SetIdString(ref param.param_id, singleCommand);
            param.param_type = (byte)MAVLink.MAV_PARAM_TYPE.REAL32;
            param.param_value = mavlinkSingleValue;
            return param;
        }

        //Retrieve and dispatch the Mavlink write data command
        private bool mavlinkWriteCommand(int sentCommandIndex)
        {
            MAVLink.mavlink_param_set_t param;
            if (sentCommandIndex == -1) param = mavlinkSingleParameter();
            else param = OnWriteMavlink(sentCommandIndex);
            if (param.target_component != 0 && param.target_system != 0)
            {
                int progress = (int)((float)sentCommandIndex / (float)commandsToSend * 32);
                comAttempts = 0;
                OnProgressChanged(commType, progress, "Configuration: Sending " + dataName + "..");
                byte[] p = form1.Mavlink.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.PARAM_SET, param);
                serial.Write(p, p.Length);
                startTimer(ref sendingCommands);
                MessageCounter++;
                return true;
            }
            return false;
        }

        //Send a Mavlink ping
        private void mavlinkSendPing(int seq)
        {
            byte[] p = form1.Mavlink.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.PING,
                new MAVLink.mavlink_ping_t()
                {
                    seq = (uint)seq,
                    time_usec = (ulong)GetCurrentUnixTimestampMillis(),
                    target_component = 0,
                    target_system = 0
                });
            serial.Write(p, p.Length);
        }

        //Fetch the next Mavlink parameter or the whole list
        private void mavlinkNextParamFetch()
        {
            if (getAllMavlinkParams)
            {
                byte[] p = form1.Mavlink.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST,
                    new MAVLink.mavlink_param_request_list_t()
                    {
                        target_component = compid,
                        target_system = sysid
                    });
                serial.Write(p, p.Length);
                startTimer(ref collectingReply);
            }
            else
            {
                for (int index = 0; index < mavlinkCount; index++)
                {
                    if (!mavlinkParameterReceived[index])
                    {
                        mavlinkSingleParamFetch(mavlinkStart + index);
                        break;
                    }
                }
            }
        }

        //Fetch a single Mavlink parameter
        private void mavlinkSingleParamFetch(int index)
        {
            byte[] p = form1.Mavlink.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_READ,
                new MAVLink.mavlink_param_request_read_t()
                {
                    target_component = compid,
                    target_system = sysid,
                    param_index = (short)index
                });
            serial.Write(p, p.Length);
            startTimer(ref collectingReply);
        }

        //--------------------------------------------------------------
        // Timing control

        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //Get a UNIX-style timestamp for Mavlink ping command
        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }
        
        //Begin timing the activity state
        void startTimer(ref bool activityFlag, int time)
        {
            comTimer.Interval = time;
            activityFlag = true;
            comTimer.Start();
        }
        void startTimer(ref bool activityFlag)
        {
            startTimer(ref activityFlag, timeout);
        }

        //Stop the timer and clear all states
        void stopTimer()
        {
            comTimer.Stop();
            waitForBoot = false;
            waitForPrompt = false;
            collectingReply = false;
            sendingCommands = false;
            checkEmpty = false;
        }

        //Timeout - retry or fail..
        private void comTimer_Timeout(object sender, EventArgs e)
        {
            //Waiting for "<Enter>" boot message
            if (waitForBoot)
            {
                stopTimer();
                if (CurrentMode == CommMode.MAVLINK)
                {
                    //Mavlink - Send ping
                    if (bootCounter == 0)
                        OnProgressChanged(commType, 0, "Discovery - Sending ping.");
                    else
                        OnProgressChanged(commType, 0, "Discovery - attempt " + bootCounter.ToString() + ": Timeout waiting for ping, Mavpixel not responding.");
                    mavlinkSendPing(bootCounter);
                }
                else crlfPress();   //CLI - Press enter
                if (++bootCounter < 10) startTimer(ref waitForBoot);
                //Timed out waiting for boot message. Try anyway.
                else startTimer(ref waitForPrompt);
            }

            //Waiting to see if any more data will arrive after finding a single prompt
            else if (checkEmpty)
            {
                stopTimer();
                OnGotData(readCommandIndex, mavPixReply);
                OnCompleted(commType, true, sentCommandIndex);
            }

            //Timed out waiting for a prompt
            else if (waitForPrompt || sendingCommands)
            {
                bool sending = sendingCommands;
                stopTimer();
                //timed out waiting for a new prompt
                if (sending)
                {
                    int progress = (int)((float)sentCommandIndex / (float)commandsToSend * 32);
                    if (comAttempts <= Properties.Settings.Default.comRetries)
                    {
                        string command;
                        OnProgressChanged(commType, progress, "Configuration - attempt " + comAttempts.ToString() + ": Timeout sending " + dataName + ", Mavpixel not responding.");
                        comAttempts++;
                        if (CurrentMode == CommMode.CLI)
                        {
                            //CLI - try pressing enter and sending command again
                            crlfPress();
                            if (singleCommand != "") command = singleCommand;
                            else command = OnWriteCommand(sentCommandIndex);
                            serial.WriteLine(command);
                            mavPixReply = "";
                        }
                        else if (CurrentMode == CommMode.MAVLINK)
                        {
                            //Mavlink - try sending parameter again
                            MAVLink.mavlink_param_set_t param = OnWriteMavlink(sentCommandIndex);
                            byte[] p = form1.Mavlink.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.PARAM_SET, param);
                            serial.Write(p, p.Length);
                        }
                        startTimer(ref sendingCommands);
                        MessageCounter++;
                    }
                    else
                    {
                        OnProgressChanged(commType, progress, "Configuration: Timeout sending " + dataName + ", Mavpixel not responding.");
                        OnCompleted(commType, false, sentCommandIndex);
                    }
                }
                else //timed out waiting for initial prompt
                {
                    if (comAttempts <= Properties.Settings.Default.comRetries)
                    {
                        // CLI / Mavlink - last resort, try waiting for CLI prompt 
                        OnProgressChanged(commType, 0, "Configuration - attempt " + comAttempts.ToString() + ": Timeout fetching " + dataName + ", Mavpixel not responding.");
                        crlfPress();
                        mavPixReply = "";
                        comAttempts++;
                        startTimer(ref waitForPrompt);
                    }
                    else
                    {
                        OnProgressChanged(commType, 0, "Configuration: Timeout fetching " + dataName + ", Mavpixel not responding.");
                        OnCompleted(commType, false, 0);
                    }
                }
            }

            //Timed out waiting for a reply to a command
            else if (collectingReply)
            {
                stopTimer();
                if (comAttempts <= Properties.Settings.Default.comRetries)
                {
                    OnProgressChanged(commType, 0, "Configuration - attempt " + comAttempts.ToString() + ": Timeout fetching " + dataName + ", Lost contact with Mavpixel.");
                    comAttempts++;
                    if (CurrentMode == CommMode.MAVLINK)
                    {
                        //Mavlink - Request parameter(s) again
                        getAllMavlinkParams = false;
                        mavlinkNextParamFetch();
                        startTimer(ref collectingReply);
                    }
                    else
                    {
                        //CLI - Press enter and wait for prompt
                        crlfPress();
                        mavPixReply = "";
                        startTimer(ref waitForPrompt);
                    }
                }
                else
                {
                    //Lost contact with Mavpixel
                    OnProgressChanged(commType, 0, "Configuration: Timeout fetching " + dataName + ", Lost contact with Mavpixel.");
                    OnCompleted(commType, false, sentCommandIndex);
                }
            }
        }

        //--------------------------------------------------------------
        // Communications state machine

        // our target sysid
        public byte sysid;
        // our target compid
        public byte compid;
        void MavMessageEvent(object sender, MAVLink.MavMessageEventArgs e)
        {
            DoModeChange(CommMode.MAVLINK);

            //Respond to Mavpixel heartbeat or ping reply
            if (e.Message.data.GetType() == typeof(MAVLink.mavlink_heartbeat_t)
                || e.Message.data.GetType() == typeof(MAVLink.mavlink_ping_t))
            {
                //Discovered our current Mavpixel, save reply id
                sysid = e.Message.sysid;
                compid = e.Message.compid;
                //Discovery complete, trigger next action
                if (waitForBoot || waitForPrompt)
                {
                    stopTimer();
                    if (commType == CommType.BOOTING)
                    {
                        //Just booting? Ready.
                        OnProgressChanged(commType, 0, "Configuration: Mavpixel ready.");
                        OnBootCompleted();
                        OnCompleted(commType, true, 0);
                    }
                    else
                    {
                        //Collecting parameters? Start fetch.
                        OnComStarted(commType);
                        OnProgressChanged(commType, 0, "Configuration: Fetching " + dataName + "..");
                        mavlinkNextParamFetch();
                    }
                }
            }

            //Respond to a Mavlink parameter
            if (e.Message.data.GetType() == typeof(MAVLink.mavlink_param_value_t))
            {
                if (collectingReply)
                {
                    stopTimer();
                    MAVLink.mavlink_param_value_t param = (MAVLink.mavlink_param_value_t)e.Message.data;
                    //Send parameter to host's mavlink message parser
                    OnGotMavlinkParam(param, getAllMavlinkParams);
                    MessageCounter++;
                    //Update parameter received flags
                    mavlinkParameterReceived[param.param_index - mavlinkStart] = true;
                    //Send progress report
                    int progress = (int)((float)(param.param_index - mavlinkStart) / (float)mavlinkCount * 32);
                    OnProgressChanged(commType, progress, "");
                    //Fetch next parameter
                    if (getAllMavlinkParams && param.param_index < param.param_count - 1)
                        //Retrieving all Mavlink parameters list.. no reply needed
                        startTimer(ref collectingReply);
                    else
                    {
                        //Any parameters not yet received?
                        if (mavlinkParameterReceived.Contains(false))
                        {
                            getAllMavlinkParams = false;    //Ensure we dont request all parameters
                            mavlinkNextParamFetch();        //Fetch next unreceived parameter
                            startTimer(ref collectingReply);
                        }
                        else
                        {
                            OnProgressChanged(commType, 32, "Configuration: Read all data OK.");
                            OnCompleted(commType, true, mavlinkCount);
                        }
                    }
                }
                else if (sendingCommands)
                {
                    stopTimer();
                    //Look for a modified parameter to send
                    for (sentCommandIndex++; sentCommandIndex < commandsToSend; sentCommandIndex++)
                        if (mavlinkWriteCommand(sentCommandIndex)) break;
                    //All parameters sent?
                    if (sentCommandIndex == commandsToSend)
                    {
                        //All parameters sent successfully
                        stopTimer();
                        OnProgressChanged(commType, 32, "Configuration: Sent all data OK.");
                        OnCompleted(commType, true, sentCommandIndex - 1);
                    }

                }


            }

        }


        //Incoming data event - wait for a prompt or gather/send a line and trigger events
        void DataReceived(object sender, PortReceivedEventArgs e)
        {
            DoModeChange(CommMode.CLI);
            mavPixReply += e.Data;

            //Respond to boot message - "Press <Enter> 3 times for CLI."
            if (waitForBoot)
            {
                if (mavPixReply.Contains("<Enter>"))
                {
                    stopTimer();
                    crlfPress();
                    crlfPress();
                    crlfPress();
                    startTimer(ref waitForPrompt);
                    return;
                }
                else if (mavPixReply.EndsWith("#")) 
                    waitForPrompt = true;               //Drop through to prompt
            }

            //Respond to a prompt "#"
            if (waitForPrompt)
            {
                if (mavPixReply.EndsWith("#"))
                {
                    stopTimer();
                    if (commType == CommType.BOOTING)
                    {
                        OnProgressChanged(commType, 32, "Configuration: Mavpixel ready.");
                        OnBootCompleted();
                        OnCompleted(commType, true, 0);
                    }
                    else
                    {
                        OnComStarted(commType);
                        OnProgressChanged(commType, 0, "Configuration: Fetching " + dataName + "..");
                        serial.WriteLine(OnReadCommand(readCommandIndex));
                        mavPixReply = "";
                        startTimer(ref collectingReply);
                    }
                }
            }

            //Gathering a reply
            else if (collectingReply)
            {
                //Scan the (multiline) reply for command indexes and use to update progress.
                int index = -1;
                for (int c = mavPixReply.Length - 1; c >= 0; c--)
                    if (char.IsLower(mavPixReply, c))
                    {
                        index = c;
                        break;
                    }
                if (index >= 0 && mavPixReply.Length >= index + 4)
                {
                    int gotReply;
                    if (int.TryParse(mavPixReply.Substring(index + 1, 3), out gotReply) && lastReply != gotReply)
                    {
                        stopTimer();
                        lastReply = gotReply;
                        OnProgressChanged(commType, lastReply, "");
                        startTimer(ref collectingReply);
                    }
                }
                //Got a complete reply. Send it to be parsed.
                if (mavPixReply.EndsWith("#"))
                {
                    if (mavPixReply.Contains(":") || mavPixReply.Contains(","))
                    {
                        stopTimer();
                        OnGotData(readCommandIndex, mavPixReply);
                        int progress = (int)((float)readCommandIndex / (float)commandsToRead * 32);
                        MessageCounter++;
                        if (++readCommandIndex < commandsToRead)
                        {
                            serial.WriteLine(OnReadCommand(readCommandIndex));
                            mavPixReply = "";
                            OnProgressChanged(commType, progress, "");
                            startTimer(ref collectingReply);
                        }
                        else
                        {
                            OnProgressChanged(commType, progress, "Configuration: Read all data OK.");
                            OnCompleted(commType, true, readCommandIndex - 1);
                        }
                    }
                    else checkEmpty = true;
                }
            }

            //Sending a command
            else if (sendingCommands)
            {
                if (mavPixReply.EndsWith("#"))  //wait for the prompt
                {
                    stopTimer();
                    //Check for errors or unrecognised responses
                    if (mavPixReply.Contains("error") || mavPixReply.Contains("Unknown"))
                    {
                        string error = mavPixReply.Split('\r')[1].Trim();
                        comAttempts++;
                        int progress = (int)((float)sentCommandIndex / (float)commandsToSend * 32);
                        if (comAttempts <= Properties.Settings.Default.comRetries)
                        {
                            string command;
                            OnProgressChanged(commType, progress, "Configuration - attempt " + comAttempts.ToString() + ": " + error + " error sending " + dataName + ".");
                            if (singleCommand != "") command = singleCommand;
                            else command = OnWriteCommand(sentCommandIndex);
                            serial.WriteLine(command);
                            startTimer(ref sendingCommands);
                        }
                        else
                        {
                            OnProgressChanged(commType, progress, "Configuration: " + error + " error sending " + dataName + ".");
                            OnCompleted(commType, false, sentCommandIndex);
                        }
                    }
                    //Send the next command if there is one
                    else if (commType == CommType.SENDING)
                    {
                        for (sentCommandIndex++; sentCommandIndex < commandsToSend; sentCommandIndex++)
                            if (serialWriteCommand(sentCommandIndex)) break;
                    }
                    //Single command completed
                    else sentCommandIndex = commandsToSend;
                    mavPixReply = "";
                }
                //All commands sent?
                if (sentCommandIndex == commandsToSend)
                {
                    //All commands sent successfully
                    stopTimer();
                    OnProgressChanged(commType, 32, "Configuration: Sent all data OK.");
                    OnCompleted(commType, true, sentCommandIndex - 1);
                }
            }
        }

        void CommsClosed(object sender, EventArgs e)
        {
            DoModeChange(CommMode.NONE);
        }

        //--------------------------------------------------------------
        // And the rest..

        //Windows designer fluff
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.comTimer = new System.Windows.Forms.Timer(this.components);
            // 
            // comTimer
            // 
            this.comTimer.Interval = 1000;
            this.comTimer.Tick += new System.EventHandler(this.comTimer_Timeout);

        }

    }
    //Event args stuff

    public class ComStartedEventArgs : EventArgs
    {
        public CommType Type;
        public ComStartedEventArgs(CommType Type)
        {
            this.Type = Type;
        }
    }
    public class GotDataEventArgs : EventArgs
    {
        public string Data;
        public int Index;
        public GotDataEventArgs(int Index, string Data)
        {
            this.Index = Index;
            this.Data = Data;
        }
    }
    public class CommandEventArgs : EventArgs
    {
        public int Index;
        public CommandEventArgs(int Index)
        {
            this.Index = Index;
        }
    }
    public class MavParameterEventArgs : EventArgs
    {
        public bool GetAllMavlinkParams;
        public MAVLink.mavlink_param_value_t Param;
        public MavParameterEventArgs(MAVLink.mavlink_param_value_t param, bool getAllMavlinkParams)
        {
            Param = param;
            GetAllMavlinkParams = getAllMavlinkParams;
        }
    }
    public enum CommType { SENDING = 0, RECEIVING = 1, BOOTING = 2, SINGLE = 3 };
    public enum CommMode { NONE = 0, CLI = 1, MAVLINK = 2 };
    public class ProgressEventArgs : EventArgs
    {
        public CommType Type;
        public int Progress;
        public string Event;
        public ProgressEventArgs(CommType Type, int Progress, string Event)
        {
            this.Type = Type;
            this.Progress = Progress;
            this.Event = Event;
        }
    }
    public class CompletedEventArgs : EventArgs
    {
        public CommType Type;
        public bool Success;
        public int LastIndex;
        public CompletedEventArgs(CommType Type, bool Success, int LastIndex)
        {
            this.Type = Type;
            this.Success = Success;
            this.LastIndex = LastIndex;
        }
    }

    public class ModeChangedEventArgs : EventArgs
    {
        public CommMode Mode;
        public ModeChangedEventArgs(CommMode Mode)
        {
            this.Mode = Mode;
        }
    }

}


