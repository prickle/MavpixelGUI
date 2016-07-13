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

namespace MavpixelGUI
{
    public partial class frmOptions : Form
    {
        AVRDude avrDude;
        Form1 form1;
        public frmOptions(AVRDude dude)
        {
            InitializeComponent();
            avrDude = dude;
        }

        private void frmGuiSettings_Load(object sender, EventArgs e)
        {
            form1 = (Form1)Application.OpenForms["Form1"];
            chkUpdateCheck.Checked = Properties.Settings.Default.setUpdateCheck;
            chkRestorePosition.Checked = Properties.Settings.Default.setRestorePosition;
            chkGetConnect.Checked = Properties.Settings.Default.pgmGetOnConnect;
            chkDashCapV.Checked = Properties.Settings.Default.pgmNoVerify;
            avrDude.checkAvrDude();
            txtAvrdude.Text = Properties.Settings.Default.avrDude;
            numRetries.Value = Properties.Settings.Default.comRetries;
            txtTimeout.Text = Properties.Settings.Default.comTimeout.ToString();
        }

        private void txtTimeout_Leave(object sender, EventArgs e)
        {
            form1.setTimeout(txtTimeout.IntValue);
        }

        private void numRetries_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.comRetries = (int)numRetries.Value;
        }

        private void chkUpdateCheck_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.setUpdateCheck = chkUpdateCheck.Checked;
        }

        private void chkRestorePosition_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.setRestorePosition = chkRestorePosition.Checked;
        }

        private void chkGetConnect_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pgmGetOnConnect = chkGetConnect.Checked;
        }

        private void btnFindAvrDude_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.avrDude = "";
            avrDude.checkAvrDude();
            txtAvrdude.Text = Properties.Settings.Default.avrDude;
        }

        private void btnAvrdudeFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select AVRdude executable";
            openFileDialog1.Filter = "Executable files|*.exe|All Files|*.*";
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(txtAvrdude.Text);
            openFileDialog1.FileName = Path.GetFileName(txtAvrdude.Text);
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtAvrdude.Text = openFileDialog1.FileName;
                Properties.Settings.Default.avrDude = openFileDialog1.FileName;
            }
        }

        private void txtAvrdude_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.avrDude = txtAvrdude.Text;
        }

        private void validateAvrDude()
        {
            throw new NotImplementedException();
        }

        private void chkDashCapV_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pgmNoVerify = chkDashCapV.Checked;
        }

    }
}
