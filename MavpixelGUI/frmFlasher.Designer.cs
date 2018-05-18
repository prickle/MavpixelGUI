namespace MavpixelGUI
{
    partial class frmFlasher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFlasher));
            this.pnlBase = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.prgBar = new System.Windows.Forms.ProgressBar();
            this.btnFlash = new System.Windows.Forms.Button();
            this.rbnLocalFile = new System.Windows.Forms.RadioButton();
            this.rbnAvailable = new System.Windows.Forms.RadioButton();
            this.txtReleaseNotes = new System.Windows.Forms.TextBox();
            this.cbxVersion = new System.Windows.Forms.ComboBox();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rbxPrompt = new MavpixelGUI.RoundBox();
            this.label11 = new System.Windows.Forms.Label();
            this.chkEraseEeprom = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxFlashFile = new System.Windows.Forms.ComboBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxPort = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.pnlBase.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.rbxPrompt.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBase
            // 
            this.pnlBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBase.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBase.Controls.Add(this.lblStatus);
            this.pnlBase.Controls.Add(this.prgBar);
            this.pnlBase.Controls.Add(this.btnFlash);
            this.pnlBase.Location = new System.Drawing.Point(0, 352);
            this.pnlBase.Name = "pnlBase";
            this.pnlBase.Size = new System.Drawing.Size(498, 40);
            this.pnlBase.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(185, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(168, 23);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // prgBar
            // 
            this.prgBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.prgBar.Location = new System.Drawing.Point(12, 13);
            this.prgBar.Name = "prgBar";
            this.prgBar.Size = new System.Drawing.Size(167, 16);
            this.prgBar.TabIndex = 0;
            // 
            // btnFlash
            // 
            this.btnFlash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFlash.BackColor = System.Drawing.Color.Salmon;
            this.btnFlash.Enabled = false;
            this.btnFlash.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            this.btnFlash.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Tomato;
            this.btnFlash.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFlash.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFlash.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnFlash.Image = global::MavpixelGUI.Properties.Resources.chipwrite;
            this.btnFlash.Location = new System.Drawing.Point(359, 8);
            this.btnFlash.Name = "btnFlash";
            this.btnFlash.Size = new System.Drawing.Size(131, 24);
            this.btnFlash.TabIndex = 2;
            this.btnFlash.Text = "Flash Firmware";
            this.btnFlash.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFlash.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFlash.UseVisualStyleBackColor = false;
            this.btnFlash.Click += new System.EventHandler(this.btnFlash_Click);
            // 
            // rbnLocalFile
            // 
            this.rbnLocalFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbnLocalFile.AutoSize = true;
            this.rbnLocalFile.Location = new System.Drawing.Point(9, 160);
            this.rbnLocalFile.Name = "rbnLocalFile";
            this.rbnLocalFile.Size = new System.Drawing.Size(70, 17);
            this.rbnLocalFile.TabIndex = 3;
            this.rbnLocalFile.Text = "Local file:";
            this.rbnLocalFile.UseVisualStyleBackColor = true;
            this.rbnLocalFile.CheckedChanged += new System.EventHandler(this.rbnLocalFile_CheckedChanged);
            // 
            // rbnAvailable
            // 
            this.rbnAvailable.AutoSize = true;
            this.rbnAvailable.Checked = true;
            this.rbnAvailable.Location = new System.Drawing.Point(9, 19);
            this.rbnAvailable.Name = "rbnAvailable";
            this.rbnAvailable.Size = new System.Drawing.Size(114, 17);
            this.rbnAvailable.TabIndex = 0;
            this.rbnAvailable.TabStop = true;
            this.rbnAvailable.Text = "Available Versions:";
            this.rbnAvailable.UseVisualStyleBackColor = true;
            this.rbnAvailable.CheckedChanged += new System.EventHandler(this.rbnAvailable_CheckedChanged);
            // 
            // txtReleaseNotes
            // 
            this.txtReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReleaseNotes.Enabled = false;
            this.txtReleaseNotes.Location = new System.Drawing.Point(9, 47);
            this.txtReleaseNotes.Multiline = true;
            this.txtReleaseNotes.Name = "txtReleaseNotes";
            this.txtReleaseNotes.ReadOnly = true;
            this.txtReleaseNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReleaseNotes.Size = new System.Drawing.Size(463, 102);
            this.txtReleaseNotes.TabIndex = 2;
            this.txtReleaseNotes.Text = "Release Notes";
            // 
            // cbxVersion
            // 
            this.cbxVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxVersion.Enabled = false;
            this.cbxVersion.FormattingEnabled = true;
            this.cbxVersion.Location = new System.Drawing.Point(129, 17);
            this.cbxVersion.Name = "cbxVersion";
            this.cbxVersion.Size = new System.Drawing.Size(343, 21);
            this.cbxVersion.TabIndex = 1;
            this.cbxVersion.Text = "Fetching, please wait..";
            this.cbxVersion.SelectedIndexChanged += new System.EventHandler(this.cbxVersion_SelectedIndexChanged);
            this.cbxVersion.Enter += new System.EventHandler(this.cbxVersion_Enter);
            // 
            // txtConsole
            // 
            this.txtConsole.BackColor = System.Drawing.Color.Black;
            this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsole.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.Color.White;
            this.txtConsole.Location = new System.Drawing.Point(0, 0);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(498, 78);
            this.txtConsole.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rbxPrompt);
            this.splitContainer1.Panel1.Controls.Add(this.chkEraseEeprom);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.cbxPort);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtConsole);
            this.splitContainer1.Size = new System.Drawing.Size(498, 349);
            this.splitContainer1.SplitterDistance = 265;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            // 
            // rbxPrompt
            // 
            this.rbxPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbxPrompt.BackColor = System.Drawing.Color.Transparent;
            this.rbxPrompt.BorderColor = System.Drawing.Color.Yellow;
            this.rbxPrompt.Controls.Add(this.label11);
            this.rbxPrompt.FillColor = System.Drawing.Color.MistyRose;
            this.rbxPrompt.Location = new System.Drawing.Point(198, 13);
            this.rbxPrompt.Name = "rbxPrompt";
            this.rbxPrompt.Size = new System.Drawing.Size(287, 43);
            this.rbxPrompt.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.BackColor = System.Drawing.Color.MistyRose;
            this.label11.Location = new System.Drawing.Point(10, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(267, 38);
            this.label11.TabIndex = 0;
            this.label11.Text = "Use the Mavpixel\'s Mavlink serial port to flash firmware.";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkEraseEeprom
            // 
            this.chkEraseEeprom.AutoSize = true;
            this.chkEraseEeprom.Location = new System.Drawing.Point(19, 39);
            this.chkEraseEeprom.Name = "chkEraseEeprom";
            this.chkEraseEeprom.Size = new System.Drawing.Size(152, 17);
            this.chkEraseEeprom.TabIndex = 3;
            this.chkEraseEeprom.Text = "Erase settings in EEPROM";
            this.chkEraseEeprom.UseVisualStyleBackColor = true;
            this.chkEraseEeprom.CheckedChanged += new System.EventHandler(this.chkEraseEeprom_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbxFlashFile);
            this.groupBox1.Controls.Add(this.rbnAvailable);
            this.groupBox1.Controls.Add(this.txtReleaseNotes);
            this.groupBox1.Controls.Add(this.rbnLocalFile);
            this.groupBox1.Controls.Add(this.btnOpen);
            this.groupBox1.Controls.Add(this.cbxVersion);
            this.groupBox1.Location = new System.Drawing.Point(9, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 190);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Firmware";
            // 
            // cbxFlashFile
            // 
            this.cbxFlashFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxFlashFile.FormattingEnabled = true;
            this.cbxFlashFile.Location = new System.Drawing.Point(85, 158);
            this.cbxFlashFile.Name = "cbxFlashFile";
            this.cbxFlashFile.Size = new System.Drawing.Size(295, 21);
            this.cbxFlashFile.TabIndex = 4;
            this.cbxFlashFile.Enter += new System.EventHandler(this.cbxFlashFile_Enter);
            this.cbxFlashFile.TextChanged += new System.EventHandler(this.cbxFlashFile_TextChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Image = global::MavpixelGUI.Properties.Resources.folderopen;
            this.btnOpen.Location = new System.Drawing.Point(386, 156);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(86, 23);
            this.btnOpen.TabIndex = 5;
            this.btnOpen.Text = "Open";
            this.btnOpen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // cbxPort
            // 
            this.cbxPort.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxPort.Enabled = false;
            this.cbxPort.FormattingEnabled = true;
            this.cbxPort.Location = new System.Drawing.Point(61, 12);
            this.cbxPort.Name = "cbxPort";
            this.cbxPort.Size = new System.Drawing.Size(104, 21);
            this.cbxPort.TabIndex = 1;
            this.cbxPort.Text = "Scanning..";
            this.cbxPort.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxPort_DrawItem);
            this.cbxPort.DropDownClosed += new System.EventHandler(this.cbxPort_DropDownClosed);
            this.cbxPort.DropDown += new System.EventHandler(this.cbxPort_DropDown);
            // 
            // openFileDialog
            // 
            this.openFileDialog.CheckFileExists = false;
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // frmFlasher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(498, 392);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlBase);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(340, 300);
            this.Name = "frmFlasher";
            this.Text = "Firmware Flasher";
            this.Load += new System.EventHandler(this.frmFlasher_Load);
            this.pnlBase.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.rbxPrompt.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBase;
        private System.Windows.Forms.Button btnFlash;
        private System.Windows.Forms.ComboBox cbxVersion;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtReleaseNotes;
        private System.Windows.Forms.RadioButton rbnLocalFile;
        private System.Windows.Forms.RadioButton rbnAvailable;
        private System.Windows.Forms.ProgressBar prgBar;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxPort;
        private System.Windows.Forms.CheckBox chkEraseEeprom;
        private RoundBox rbxPrompt;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ComboBox cbxFlashFile;
        private System.Windows.Forms.Label lblStatus;
    }
}