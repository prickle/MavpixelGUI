namespace MavpixelGUI
{
    partial class frmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.label1 = new System.Windows.Forms.Label();
            this.txtAvrdude = new System.Windows.Forms.TextBox();
            this.chkDashCapV = new System.Windows.Forms.CheckBox();
            this.chkRestorePosition = new System.Windows.Forms.CheckBox();
            this.chkUpdateCheck = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnFindAvrDude = new System.Windows.Forms.Button();
            this.btnAvrdudeFile = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkGetConnect = new System.Windows.Forms.CheckBox();
            this.txtTimeout = new MavpixelGUI.NumericTextBox();
            this.numRetries = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRetries)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Location of avrdude.exe:";
            // 
            // txtAvrdude
            // 
            this.txtAvrdude.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAvrdude.Location = new System.Drawing.Point(9, 42);
            this.txtAvrdude.Name = "txtAvrdude";
            this.txtAvrdude.Size = new System.Drawing.Size(269, 20);
            this.txtAvrdude.TabIndex = 3;
            this.txtAvrdude.Leave += new System.EventHandler(this.txtAvrdude_Leave);
            // 
            // chkDashCapV
            // 
            this.chkDashCapV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkDashCapV.AutoSize = true;
            this.chkDashCapV.Location = new System.Drawing.Point(15, 71);
            this.chkDashCapV.Name = "chkDashCapV";
            this.chkDashCapV.Size = new System.Drawing.Size(214, 17);
            this.chkDashCapV.TabIndex = 4;
            this.chkDashCapV.Text = "Disable verification check after flashing.";
            this.chkDashCapV.UseVisualStyleBackColor = true;
            this.chkDashCapV.CheckedChanged += new System.EventHandler(this.chkDashCapV_CheckedChanged);
            // 
            // chkRestorePosition
            // 
            this.chkRestorePosition.AutoSize = true;
            this.chkRestorePosition.Checked = true;
            this.chkRestorePosition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRestorePosition.Location = new System.Drawing.Point(15, 42);
            this.chkRestorePosition.Name = "chkRestorePosition";
            this.chkRestorePosition.Size = new System.Drawing.Size(180, 17);
            this.chkRestorePosition.TabIndex = 1;
            this.chkRestorePosition.Text = "Restore windows to last position.";
            this.chkRestorePosition.UseVisualStyleBackColor = true;
            this.chkRestorePosition.CheckedChanged += new System.EventHandler(this.chkRestorePosition_CheckedChanged);
            // 
            // chkUpdateCheck
            // 
            this.chkUpdateCheck.AutoSize = true;
            this.chkUpdateCheck.Checked = true;
            this.chkUpdateCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpdateCheck.Location = new System.Drawing.Point(15, 19);
            this.chkUpdateCheck.Name = "chkUpdateCheck";
            this.chkUpdateCheck.Size = new System.Drawing.Size(116, 17);
            this.chkUpdateCheck.TabIndex = 0;
            this.chkUpdateCheck.Text = "Check for updates.";
            this.chkUpdateCheck.UseVisualStyleBackColor = true;
            this.chkUpdateCheck.CheckedChanged += new System.EventHandler(this.chkUpdateCheck_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkRestorePosition);
            this.groupBox1.Controls.Add(this.chkUpdateCheck);
            this.groupBox1.Location = new System.Drawing.Point(12, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 70);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Program startup";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnFindAvrDude);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtAvrdude);
            this.groupBox2.Controls.Add(this.chkDashCapV);
            this.groupBox2.Controls.Add(this.btnAvrdudeFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 196);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(284, 100);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Firmware Flasher settings";
            // 
            // btnFindAvrDude
            // 
            this.btnFindAvrDude.Image = global::MavpixelGUI.Properties.Resources.zoom;
            this.btnFindAvrDude.Location = new System.Drawing.Point(154, 14);
            this.btnFindAvrDude.Name = "btnFindAvrDude";
            this.btnFindAvrDude.Size = new System.Drawing.Size(59, 24);
            this.btnFindAvrDude.TabIndex = 1;
            this.btnFindAvrDude.Text = "Find";
            this.btnFindAvrDude.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFindAvrDude.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFindAvrDude.UseVisualStyleBackColor = true;
            this.btnFindAvrDude.Click += new System.EventHandler(this.btnFindAvrDude_Click);
            // 
            // btnAvrdudeFile
            // 
            this.btnAvrdudeFile.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAvrdudeFile.Image = ((System.Drawing.Image)(resources.GetObject("btnAvrdudeFile.Image")));
            this.btnAvrdudeFile.Location = new System.Drawing.Point(219, 14);
            this.btnAvrdudeFile.Name = "btnAvrdudeFile";
            this.btnAvrdudeFile.Size = new System.Drawing.Size(59, 24);
            this.btnAvrdudeFile.TabIndex = 2;
            this.btnAvrdudeFile.Text = "Open";
            this.btnAvrdudeFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAvrdudeFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAvrdudeFile.UseVisualStyleBackColor = true;
            this.btnAvrdudeFile.Click += new System.EventHandler(this.btnAvrdudeFile_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkGetConnect);
            this.groupBox3.Controls.Add(this.txtTimeout);
            this.groupBox3.Controls.Add(this.numRetries);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(12, 85);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(284, 105);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Communication settings";
            // 
            // chkGetConnect
            // 
            this.chkGetConnect.AutoSize = true;
            this.chkGetConnect.Location = new System.Drawing.Point(15, 20);
            this.chkGetConnect.Name = "chkGetConnect";
            this.chkGetConnect.Size = new System.Drawing.Size(254, 17);
            this.chkGetConnect.TabIndex = 0;
            this.chkGetConnect.Text = "Get configuration from Mavpixel on first connect.";
            this.chkGetConnect.UseVisualStyleBackColor = true;
            this.chkGetConnect.CheckedChanged += new System.EventHandler(this.chkGetConnect_CheckedChanged);
            // 
            // txtTimeout
            // 
            this.txtTimeout.AllowSpace = false;
            this.txtTimeout.Location = new System.Drawing.Point(154, 73);
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(123, 20);
            this.txtTimeout.TabIndex = 4;
            this.txtTimeout.Terminator = "ms";
            this.txtTimeout.Text = "ms";
            this.txtTimeout.Leave += new System.EventHandler(this.txtTimeout_Leave);
            // 
            // numRetries
            // 
            this.numRetries.Location = new System.Drawing.Point(154, 46);
            this.numRetries.Name = "numRetries";
            this.numRetries.Size = new System.Drawing.Size(123, 20);
            this.numRetries.TabIndex = 2;
            this.numRetries.ValueChanged += new System.EventHandler(this.numRetries_Leave);
            this.numRetries.Leave += new System.EventHandler(this.numRetries_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Com failure timeout:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Number of com error retries:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(310, 309);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmOptions";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.frmGuiSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRetries)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAvrdude;
        private System.Windows.Forms.Button btnAvrdudeFile;
        private System.Windows.Forms.CheckBox chkDashCapV;
        private System.Windows.Forms.CheckBox chkRestorePosition;
        private System.Windows.Forms.CheckBox chkUpdateCheck;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnFindAvrDude;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private NumericTextBox txtTimeout;
        private System.Windows.Forms.NumericUpDown numRetries;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox chkGetConnect;
    }
}