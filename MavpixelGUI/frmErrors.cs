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
using System.Runtime.InteropServices;

namespace MavpixelGUI
{
    public partial class frmErrors : Form
    {
        #region Win32
        [DllImport("user32.dll")]
        static extern void MessageBeep(uint uType);

        const uint MB_OK = 0x00000000;
        const uint MB_ICONHAND = 0x00000010;
        const uint MB_ICONQUESTION = 0x00000020;
        const uint MB_ICONEXCLAMATION = 0x00000030;
        const uint MB_ICONASTERISK = 0x00000040;

        #endregion

        public BindingList<ErrorEntry> list;
        public frmErrors(string avrDudeName, List<string> errorList)
        {
            InitializeComponent();
            this.Text = avrDudeName + " Errors";
            //checkBox1.Checked = Properties.Settings.Default.setIgnoreErrors;
            list = new BindingList<ErrorEntry>();
            for (int index = 0; index < errorList.Count; index++)
                list.Add(new ErrorEntry(index, errorList[index]));
            dataGridView1.AutoGenerateColumns = false;
            DataGridViewTextBoxColumn numColumn = new DataGridViewTextBoxColumn();
            numColumn.DataPropertyName = "Number";
            numColumn.HeaderText = "Error";
            numColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            numColumn.Width = 40;

            DataGridViewTextBoxColumn msgColumn = new DataGridViewTextBoxColumn();
            msgColumn.DataPropertyName = "Message";
            msgColumn.HeaderText = "Message";
            msgColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridView1.Columns.Add(numColumn);
            dataGridView1.Columns.Add(msgColumn);

            dataGridView1.DataSource = list;
        }

        public class ErrorEntry
        {
            public int Number { get; set; }
            public string Message { get; set; }
            public ErrorEntry(int number, string message)
            {
                Number = number;
                Message = message;
            }
        }

        private void frmErrors_Load(object sender, EventArgs e)
        {
            MessageBeep(MB_ICONEXCLAMATION);
        }
    }
}
