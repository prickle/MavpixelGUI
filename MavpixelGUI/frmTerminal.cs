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

//------------------------------------------------------------------------------------------------
// Terminal window

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

namespace MavpixelGUI
{
    public partial class frmTerminal : Form
    {

        bool isEnabled = false;
        serialControl serial;
        //SerialPort port { get { return serial.Port; } set { serial.Port = value; } }
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                txtTerminal.Enabled = value;
                if (value)
                {
                    txtTerminal.BackColor = Color.Black;
                    Text = "Terminal - Connected";
                }
                else
                {
                    //txtTerminal.Clear();
                    txtTerminal.BackColor = System.Drawing.SystemColors.Control;
                    if (serial == null || !serial.IsOpen)
                        Text = "Terminal - Not Connected";
                    else
                        Text = "Terminal - Disabled in Mavlink Mode";
                }
                Invalidate();
            }
        }

        int maxTerminalLines = 0;
        int rollBackIndex = -1;
        string displayedRollBack = "";
        List<string> rollBackList = new List<string>();
        StringBuilder keyPressAccumulator = new StringBuilder();

        public frmTerminal(serialControl serial)
        {
            InitializeComponent();
            this.serial = serial;
            rollBackList.Add("");
        }

        private void frmTerminal_Load(object sender, EventArgs e)
        {
            maxTerminalLines = Properties.Settings.Default.termLines;
            txtLines.Text = maxTerminalLines.ToString();
            txtTerminal.Enabled = false;
            txtTerminal.Text = "Please connect first.";
        }

        public void Clear()
        {
            txtTerminal.Clear();
        }

        StringBuilder command = new StringBuilder();
        public void clearCommand()
        {
            command.Remove(0, command.Length);
        }

        public void termOut(string str)
        {
            int z;
            if ((z = str.IndexOf('\0')) >= 0) str = str.Remove(z);
            txtTerminal.Suspend();
            //Handle lines containing backspace
            if ((z = str.IndexOf('\b')) >= 0)
            {
                for (int count = 0; count < str.Length; count++)
                {
                    char c = str[count];
                    if (c == 8 && txtTerminal.Text.Length > 1)
                    {
                        //remove character from terminal
                        txtTerminal.Text = txtTerminal.Text.Substring(0, txtTerminal.Text.Length - 1);
                        //Remove character from accumulator
                        if (keyPressAccumulator.Length > 0) keyPressAccumulator.Remove(keyPressAccumulator.Length - 1, 1);
                    }
                    else txtTerminal.AppendText(c.ToString());
                }
            }
            //Regular lines
            else txtTerminal.AppendText(str);
            if (maxTerminalLines != txtTerminal.Lines.Length)
            {
                while (txtTerminal.Lines.Length > (int)Properties.Settings.Default.termLines)
                    txtTerminal.Text = txtTerminal.Text.Substring(txtTerminal.Text.IndexOf("\n") + 1);
                maxTerminalLines = txtTerminal.Lines.Length;
            }
            ScrollToBottom(txtTerminal);
            txtTerminal.SelectionStart = txtTerminal.Text.Length;
            txtTerminal.Resume();
        }

        void displayRollBack(string command)
        {
            txtTerminal.Suspend();
            //Remove existing rollback if any
            if (displayedRollBack != "")
            {
                if (txtTerminal.Text.EndsWith(displayedRollBack))
                {
                    int start = txtTerminal.Text.LastIndexOf(displayedRollBack);
                    txtTerminal.Text = txtTerminal.Text.Remove(start, displayedRollBack.Length);
                }
            }
            //Add the new rollback
            txtTerminal.Text += command;
            ScrollToBottom(txtTerminal);
            txtTerminal.SelectionStart = txtTerminal.Text.Length;
            txtTerminal.Resume();
            displayedRollBack = command;
        }

        void writeRollBack()
        {
            if (displayedRollBack == "") return;
            string command = displayedRollBack;
            this.keyPressAccumulator.Append(command);
            displayRollBack("");
            foreach (char c in command)
            {
                byte[] data = { (byte)c };
                serial.Write(data, 1);
            }
            rollBackIndex = -1;
        }

        //Up and down arrows for rollback
        private void txtTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (rollBackList.Count > 0 && rollBackIndex < rollBackList.Count - 1)
                {
                    rollBackIndex++;
                    displayRollBack(rollBackList[rollBackIndex]);
                }
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                if (rollBackIndex > -1)
                {
                    rollBackIndex--;
                    if (rollBackIndex == -1) displayRollBack("");
                    else displayRollBack(rollBackList[rollBackIndex]);
                }
                e.Handled = true;
            }
        }

        private void txtTerminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (serial.IsOpen)
            {
                writeRollBack();
                if (e.KeyChar == '\r')
                {
                    byte[] data = { (byte)'\r', (byte)'\n' };
                    serial.Write(data, 2);
                    //New entry in rollback
                    if (keyPressAccumulator.Length > 0)
                    {
                        rollBackList.Insert(0, keyPressAccumulator.ToString());
                        keyPressAccumulator = new StringBuilder();
                    }
                }
                else if (e.KeyChar == 3) //ctl-c
                {
                    txtTerminal.Copy();
                }
                else if (e.KeyChar == 22) //ctl-v
                {
                    if (Clipboard.ContainsText())
                        txtTerminal_PasteEvent(this, new PasteEventArgs(Clipboard.GetText()));
                }
                else
                {
                    byte[] data = { (byte)e.KeyChar };
                    serial.Write(data, 1);
                    //Add char to accumulator
                    if (e.KeyChar >= 32) keyPressAccumulator.Append(e.KeyChar);
                }
            }
            e.Handled = true;   //no local echo
        }


        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SendMessage(System.IntPtr hWnd, int wMsg, System.IntPtr wParam, System.IntPtr lParam);

        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        /// <summary>
        /// Scrolls the vertical scroll bar of a multi-line text box to the bottom.
        /// </summary>
        /// <param name="tb">The text box to scroll</param>
        public static void ScrollToBottom(System.Windows.Forms.TextBox tb)
        {
            if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                SendMessage(tb.Handle, WM_VSCROLL, new System.IntPtr(SB_BOTTOM), System.IntPtr.Zero);
        }

        private void frmTerminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.termFile == null || Properties.Settings.Default.termFile == "")
                Properties.Settings.Default.termFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.termFile);
            saveFileDialog1.FileName = "log.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.termFile = saveFileDialog1.FileName;
                File.WriteAllText(saveFileDialog1.FileName, txtTerminal.Text);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                Paste(Clipboard.GetText());
            }
        }

        public void Copy()
        {
            txtTerminal.Copy();
        }

        private void txtLines_TextChanged(object sender, EventArgs e)
        {
            int value;
            if (int.TryParse(txtLines.Text, out value))
            {
                if (value < 1) value = 1;
                if (value > 10000) value = 10000;
                Properties.Settings.Default.termLines = value;
            }
        }

        private void txtTerminal_PasteEvent(object sender, PasteEventArgs e)
        {
            Paste(e.Data);
        }

        public void Paste(string data)
        {
            if (serial.IsOpen)
                serial.WriteLine(data);
        }

    }

    class TermTextBox : TextBox
    {
        //Paste event
        public delegate void PasteEventHandler(object sender, PasteEventArgs e);
        public event PasteEventHandler PasteEvent;
        public void OnPaste(PasteEventArgs e)
        {
            if (PasteEvent != null) PasteEvent(this, e);
        }

        protected override void WndProc(ref Message m)
        {
            // Trap WM_PASTE:
            if (m.Msg == 0x302 && Clipboard.ContainsText())
            {
                OnPaste(new PasteEventArgs(Clipboard.GetText()));
                return;
            }
            base.WndProc(ref m);
        }
    }

    public class PasteEventArgs : EventArgs
    {
        public string Data;
        public PasteEventArgs(string Data)
        {
            this.Data = Data;
        }
    }

}
namespace System.Windows.Forms
{
    public static class ControlExtensions
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void Suspend(this Control control)
        {
            LockWindowUpdate(control.Handle);
        }

        public static void Resume(this Control control)
        {
            LockWindowUpdate(IntPtr.Zero);
        }

    }
}