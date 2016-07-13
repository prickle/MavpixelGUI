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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using OpenNETCF.IO.Ports;
using System.Net.Sockets;
using System.Net;


namespace MavpixelGUI
{
    public partial class serialControl : UserControl
    {
        SerialPort Port = null;
        UdpClient Client = null;
        Ports availablePorts;
        string portName;
        
        /// <summary>
        /// Serial Port is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if ((Port == null && Client != null && remoteIpEndPoint.Port != 0)
                    || (Port != null && Port.IsOpen)) return true;
                else return false;
            }
        }

        /// <summary>
        /// Name of open port or "" if closed
        /// </summary>
        public string PortName
        {
            get
            {
                if (Client != null || Port != null) return portName.Trim(':');
                else return "";
            }
        }

        /// <summary>
        /// Name of communications link or "" if none established
        /// </summary>
        public string LinkName
        {
            get
            {
                if (Port != null) return "Serial Port";
                else if(Client != null) return "Network";
                else return "Communications";
            }
        }

        public enum Type { NONE, SERIAL, UDP };
        /// <summary>
        /// Type of communications link
        /// </summary>
        public Type LinkType
        {
            get
            {
                if (Port != null) return Type.SERIAL;
                else if (Client != null) return Type.UDP;
                else return Type.NONE;
            }
        }

        /// <summary>
        /// Number of bytes to read
        /// </summary>
        public int BytesToRead
        {
            get
            {
                if (Port == null && Client != null && UdpBuffer.Count > 0)
                    return UdpBuffer[0].Length;
                else if (Port != null && Port.IsOpen) return Port.BytesToRead;
                else return 0;
            }
        }


        public serialControl()
        {
            InitializeComponent();
            availablePorts = new Ports();   //Create RS232 port interface
            availablePorts.ScanDevices(portScan_RunCompleted);
            cbxBaud.Text = Properties.Settings.Default.baudRate;
            Font = new Font("Microsoft Sans Serif", 8.25f); //prevent switch to Segoe UI
        }

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

        //Callback from serial port scan
        private void portScan_RunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cbxPort.Enabled = true;
            //if (((List<cbxPortEntry>)e.Result).Count == 0)
            //{
            //    cbxPort.Text = "None";
            //    chkOpen.Enabled = false;
            //    return;
            //}
            chkOpen.Enabled = true;
            cbxPort.Items.AddRange(((List<cbxPortEntry>)e.Result).ToArray());
            cbxPort.Items.Add(new cbxPortEntry("UDP:", "Network server"));
            //No selection? Choose something reasonable
            if (Properties.Settings.Default.comPort == null || Properties.Settings.Default.comPort == "")
            {
                //Use first available com port
                foreach (cbxPortEntry port in cbxPort.Items)
                    if (port.Name.StartsWith("COM"))
                    {
                        cbxPort.SelectedItem = port;
                        break;
                    }
                //Still nothing? Use UDP.
                if (cbxPort.SelectedIndex == -1) cbxPort.SelectedIndex = 0;
            }
            //Restore previous selection
            else cbxPort.Text = Properties.Settings.Default.comPort;
        }

        //Port opening event
        public delegate void PortOpeningHandler(object sender, EventArgs e);
        public event PortOpeningHandler PortOpening;
        public void OnPortOpening()
        {
            if (PortOpening != null)
            {
                PortOpening(this, EventArgs.Empty);
            }
        }

        //Port opened event
        public delegate void PortOpenedHandler(object sender, EventArgs e);
        public event PortOpenedHandler PortOpened;
        public void OnPortOpened()
        {
            if (PortOpened != null)
            {
                PortOpened(this, EventArgs.Empty);
            }
        }

        //Port closed event
        public delegate void PortClosedHandler(object sender, EventArgs e);
        public event PortClosedHandler PortClosed;
        public void OnPortClosed()
        {
            if (PortClosed != null)
            {
                PortClosed(this, EventArgs.Empty);
            }
        }


        //Data received event
        public delegate void DataReceivedEventHandler(object sender, SerialReceivedEventArgs e);
        public event DataReceivedEventHandler DataReceived;
        public void OnDataReceived(object sender, SerialReceivedEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        //Port error event
        public delegate void PortErrorEventHandler(object sender, PortErrorEventArgs e);
        public event PortErrorEventHandler PortError;
        public void OnPortError(string Message)
        {
            if (PortError != null)
            {
                PortError(this, new PortErrorEventArgs(Message));
            }
        }

        //Open/Close communication port
        private void chkOpen_CheckedChanged(object sender, EventArgs e)
        {
            //Close UDP connection if open
            if (Client != null)
            {
                if (Client.Client.Connected) Client.Client.Disconnect(true);
                Client.Client.Shutdown(SocketShutdown.Both);
                Client.Close();
                Client = null;
                chkOpen.Text = "Connect";
                chkOpen.Image = Properties.Resources.disconnect;
                remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                OnPortClosed();
            }
            //Close Serial port if open
            if (Port != null)
            {
                Thread CloseDown = new Thread(new ThreadStart(ForceClose)); //close port in new thread to avoid hang
                CloseDown.Start(); //close port in new thread to avoid hang
                chkOpen.Text = "Connect";
                chkOpen.Image = Properties.Resources.disconnect;
                comTimer.Enabled = false;
                OnPortClosed();
            }
            //Opening..
            if (chkOpen.Checked)
            {
                int baud = 0;
                int.TryParse(cbxBaud.Text, out baud);
                portName = cbxPort.Text;
                //Open UDP connection
                if (portName.ToUpper().StartsWith("UDP"))
                {
                    try
                    {
                        Client = new UdpClient(baud);
                        chkOpen.Text = "Connecting..";
                        chkOpen.Image = Properties.Resources.connect;
                        OnPortOpening();
                        Port = null;
                        Client.BeginReceive(UdpReceived, Client);
                        baudListAdd(cbxPort.Text, baud);
                    }
                    catch (Exception ex)
                    {
                        chkOpen.Checked = false;
                        OnPortError(ex.Message);
                        chkOpen.Text = "Connect";
                        chkOpen.Image = Properties.Resources.disconnect;
                    }
                }
                //Open Serial port
                else
                {
                    try
                    {
                        string name = @"\\.\" + portName.Trim(':');
                        Port = new SerialPort(name, baud);
                        Port.ErrorEvent += new SerialErrorEventHandler(Port_ErrorEvent);
                        Port.ReceivedEvent += new SerialReceivedEventHandler(OnDataReceived);
                        chkOpen.Text = "Connecting..";
                        OnPortOpening();
                        chkOpen.Image = Properties.Resources.connect;
                        Application.DoEvents();
                        Thread.Sleep(100);
                        Port.Open();
                        chkOpen.Text = "Disconnect";
                        comTimer.Enabled = true;
                        OnPortOpened();
                        baudListAdd(cbxPort.Text, baud);
                    }
                    catch (Exception ex)
                    {
                        chkOpen.Checked = false;
                        OnPortError(ex.Message);
                        chkOpen.Text = "Connect";
                        chkOpen.Image = Properties.Resources.disconnect;
                        comTimer.Enabled = false;
                    }
                }
            }
        }

        private void baudListAdd(string name, int baud)
        {
            if (Properties.Settings.Default.comBaudList == null)
                Properties.Settings.Default.comBaudList = new System.Collections.Specialized.StringCollection();
            string entry = name.Trim(':').ToLower() + " " + baud.ToString();
            int find = findPort(name);
            if (find == -1) Properties.Settings.Default.comBaudList.Add(entry);
            else Properties.Settings.Default.comBaudList[find] = entry;
        }

        private int findPort(string name)
        {
            int index;
            if (Properties.Settings.Default.comBaudList == null) return -1;
            for (index = 0; index < Properties.Settings.Default.comBaudList.Count; index++)
            {
                string[] entry = Properties.Settings.Default.comBaudList[index].Split(' ');
                if (name.Trim(':').ToLower() == entry[0]) return index;
            }
            return -1;
        }

        List<byte[]> UdpBuffer = new List<byte[]>();

        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private void UdpReceived(IAsyncResult ar)
        {
            bool justConnected = false;
            if (remoteIpEndPoint.Port == 0) justConnected = true;
            UdpClient c = (UdpClient)ar.AsyncState;
            try
            {
                if (Client != null)
                {
                    if (c.Client != null) UdpBuffer.Add(c.EndReceive(ar, ref remoteIpEndPoint));
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (justConnected)
                        {
                            chkOpen.Text = "Disconnect";
                            OnPortOpened();
                        }
                        OnDataReceived(this, new SerialReceivedEventArgs(SerialReceived.ReceivedChars));
                    });
                    if (c.Client != null) c.BeginReceive(UdpReceived, ar.AsyncState);
                }
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException || ex is SocketException) return;
                OnPortError(ex.Message);
            }
        }

        //public communication port Close() method
        /// <summary>
        /// Close serial port.
        /// </summary>
        public void Close()
        {
            if (chkOpen.Checked) chkOpen.Checked = false;
        }

        //Port error handler
        void Port_ErrorEvent(object sender, SerialErrorEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    OnPortError(e.Description);
                });
            }
            catch { }
        }

        //Manually close serial port
        /// <summary>
        /// Force serial port to close in current thread.
        /// </summary>
        public void ForceClose()
        {
            try
            {
                if (Port.IsOpen)
                {
                    Port.DiscardInBuffer();
                    Port.Close(); //close the serial port
                }
                Port = null;
            }
            catch (Exception ex)
            {
                //catch any serial port closing error messages
                this.Invoke((MethodInvoker)delegate
                {
                    OnPortError(ex.Message);
                });
            }
        }

        private void comTimer_Tick(object sender, EventArgs e)
        {
            //Check if port closed unexpectedly
            if (Port != null && !Port.IsOpen)
            {
                chkOpen.Checked = false;
                chkOpen.Text = "Open";
                comTimer.Enabled = false;
                OnPortClosed();
            }
        }

        private void cbxPort_TextChanged(object sender, EventArgs e)
        {
            if (cbxPort.Text != "Scanning.." && cbxPort.Text != "None")
            {
                Properties.Settings.Default.comPort = cbxPort.Text;
                int index = findPort(cbxPort.Text);
                if (index != -1)
                {
                    cbxBaud.Text = Properties.Settings.Default.comBaudList[index].Split(' ')[1];
                }
                else if (cbxPort.Text.ToUpper().StartsWith("UDP")) cbxBaud.Text = "14550";
                else cbxBaud.Text = Properties.Settings.Default.baudRate;
                if (cbxPort.Text.ToUpper().StartsWith("UDP")) populateUdpPortList();
                else populateSerialBaudList();
            }
        }

        private void cbxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar)) e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void populateUdpPortList()
        {
            cbxBaud.Items.Clear();
            cbxBaud.Items.AddRange(new string[] {
                        "14550",
                        "14551",
                        "14552"});
            
        }
        private void populateSerialBaudList()
        {
            cbxBaud.Items.Clear();
            cbxBaud.Items.AddRange(new string[] {
                        "2400",
                        "4800",
                        "9600",
                        "19200",
                        "38400",
                        "57600",
                        "115200"});
        }

        private void cbxBaud_TextChanged(object sender, EventArgs e)
        {
            if (!cbxPort.Text.ToUpper().StartsWith("UDP"))
                Properties.Settings.Default.baudRate = cbxBaud.Text;
        }

        //-----------------------------------------------------------------------
        //Low-level communication

        //IntFloat extracts bytes from single precision float
        [StructLayout(LayoutKind.Explicit)]
        struct IntFloat
        {
            [FieldOffset(0)]
            public float FloatValue;

            [FieldOffset(0)]
            public uint IntValue;
        }

        /// <summary>
        /// Write 'count' bytes from byte buffer to communication port
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int count)
        {
            try
            {
                if (Port == null && Client != null && remoteIpEndPoint.Port != 0)
                    //UDP connection
                    Client.Send(buffer, count, remoteIpEndPoint);
                else if (Port != null && Port.IsOpen)
                    //Serial connection
                    Port.Write(buffer, 0, count);
            }
            catch (IOException) { } //Ignore leaky pipes
        }

        /// <summary>
        /// Write a string to communication port
        /// </summary>
        /// <param name="line"></param>
        public void WriteLine(string line)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(line + "\n");
            Write(bytes, bytes.Length);
        }

        /// <summary>
        /// Read all existing data from communication port
        /// </summary>
        public string ReadExisting()
        {
            return Encoding.ASCII.GetString(ReadExistingBytes());
        }


        /// <summary>
        /// Read all existing bytes from communication port
        /// </summary>
        public byte[] ReadExistingBytes()
        {
            if (Port == null && Client != null && UdpBuffer.Count > 0)
            {
                //UDP connection
                byte[] entry = UdpBuffer[0];
                UdpBuffer.RemoveAt(0);
                return entry;
            }
            else if (Port != null && Port.IsOpen)
                return Port.ReadExistingBytes();
            else return new byte[] {};
        }



    }
    //Outgoing new message event
    public class PortErrorEventArgs : EventArgs
    {
        public string Message;
        public PortErrorEventArgs(string Message)
        {
            this.Message = Message;
        }
    }

    public class PortReceivedEventArgs : EventArgs
    {
        public string Data;
        public PortReceivedEventArgs(string Data)
        {
            this.Data = Data;
        }
    }

    public class BytesReceivedEventArgs : EventArgs
    {
        public byte[] Bytes;
        public BytesReceivedEventArgs(byte[] Bytes)
        {
            this.Bytes = Bytes;
        }
    }


}
