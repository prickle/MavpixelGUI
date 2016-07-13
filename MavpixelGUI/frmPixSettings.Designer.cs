namespace MavpixelGUI
{
    partial class frmPixSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPixSettings));
            this.pnlMain = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxSoftserial = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkAnim = new System.Windows.Forms.CheckBox();
            this.cbxMavlink = new System.Windows.Forms.ComboBox();
            this.trkBright = new System.Windows.Forms.TrackBar();
            this.pnlBase = new System.Windows.Forms.Panel();
            this.comTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chkHeartBeat = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFactory = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.divider4 = new MavpixelGUI.Divider();
            this.txtSysid = new MavpixelGUI.NumericTextBox();
            this.txtDeadband = new MavpixelGUI.NumericTextBox();
            this.txtMinsats = new MavpixelGUI.NumericTextBox();
            this.txtLowcell = new MavpixelGUI.NumericTextBox();
            this.txtLowpct = new MavpixelGUI.NumericTextBox();
            this.comms = new MavpixelGUI.Communicator();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkBright)).BeginInit();
            this.pnlBase.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMain.ColumnCount = 2;
            this.pnlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 137F));
            this.pnlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlMain.Controls.Add(this.divider4, 0, 7);
            this.pnlMain.Controls.Add(this.chkHeartBeat, 1, 9);
            this.pnlMain.Controls.Add(this.label5, 0, 9);
            this.pnlMain.Controls.Add(this.txtSysid, 1, 8);
            this.pnlMain.Controls.Add(this.label3, 0, 8);
            this.pnlMain.Controls.Add(this.txtDeadband, 1, 5);
            this.pnlMain.Controls.Add(this.label2, 0, 5);
            this.pnlMain.Controls.Add(this.txtMinsats, 1, 4);
            this.pnlMain.Controls.Add(this.label1, 0, 4);
            this.pnlMain.Controls.Add(this.cbxSoftserial, 1, 11);
            this.pnlMain.Controls.Add(this.label16, 0, 11);
            this.pnlMain.Controls.Add(this.label12, 0, 10);
            this.pnlMain.Controls.Add(this.label11, 0, 1);
            this.pnlMain.Controls.Add(this.label10, 0, 0);
            this.pnlMain.Controls.Add(this.label9, 0, 3);
            this.pnlMain.Controls.Add(this.label4, 0, 2);
            this.pnlMain.Controls.Add(this.chkAnim, 1, 1);
            this.pnlMain.Controls.Add(this.cbxMavlink, 1, 10);
            this.pnlMain.Controls.Add(this.txtLowcell, 1, 2);
            this.pnlMain.Controls.Add(this.txtLowpct, 1, 3);
            this.pnlMain.Controls.Add(this.trkBright, 1, 0);
            this.pnlMain.Controls.Add(this.label7, 0, 6);
            this.pnlMain.Location = new System.Drawing.Point(7, 5);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.RowCount = 12;
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.999859F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.999854F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.999854F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.999854F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.999854F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.999854F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00052F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.999516F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00042F));
            this.pnlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.00042F));
            this.pnlMain.Size = new System.Drawing.Size(339, 291);
            this.pnlMain.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 26);
            this.label2.TabIndex = 10;
            this.label2.Text = "Stick center deadband:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 26);
            this.label1.TabIndex = 8;
            this.label1.Text = "Minimum GPS satellites:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbxSoftserial
            // 
            this.cbxSoftserial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxSoftserial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSoftserial.FormattingEnabled = true;
            this.cbxSoftserial.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "19200",
            "38400"});
            this.cbxSoftserial.Location = new System.Drawing.Point(140, 267);
            this.cbxSoftserial.Name = "cbxSoftserial";
            this.cbxSoftserial.Size = new System.Drawing.Size(196, 21);
            this.cbxSoftserial.TabIndex = 15;
            this.cbxSoftserial.SelectedIndexChanged += new System.EventHandler(this.cbxSoftserial_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Location = new System.Drawing.Point(3, 264);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(131, 27);
            this.label16.TabIndex = 14;
            this.label16.Text = "Configuration baud rate:";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Location = new System.Drawing.Point(3, 238);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(131, 26);
            this.label12.TabIndex = 12;
            this.label12.Text = "Mavlink baud rate:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(3, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(131, 26);
            this.label11.TabIndex = 2;
            this.label11.Text = "LED strip animation:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 26);
            this.label10.TabIndex = 0;
            this.label10.Text = "LED strip brightness:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(3, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 26);
            this.label9.TabIndex = 6;
            this.label9.Text = "Low battery percentage:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 26);
            this.label4.TabIndex = 4;
            this.label4.Text = "Low battery cell voltage:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkAnim
            // 
            this.chkAnim.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkAnim.Location = new System.Drawing.Point(140, 31);
            this.chkAnim.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.chkAnim.Name = "chkAnim";
            this.chkAnim.Size = new System.Drawing.Size(111, 17);
            this.chkAnim.TabIndex = 3;
            this.chkAnim.Text = "Enabled";
            this.chkAnim.UseVisualStyleBackColor = true;
            this.chkAnim.CheckedChanged += new System.EventHandler(this.chkAnim_CheckedChanged);
            // 
            // cbxMavlink
            // 
            this.cbxMavlink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxMavlink.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMavlink.FormattingEnabled = true;
            this.cbxMavlink.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cbxMavlink.Location = new System.Drawing.Point(140, 241);
            this.cbxMavlink.Name = "cbxMavlink";
            this.cbxMavlink.Size = new System.Drawing.Size(196, 21);
            this.cbxMavlink.TabIndex = 13;
            this.cbxMavlink.SelectedIndexChanged += new System.EventHandler(this.cbxMavlink_SelectedIndexChanged);
            // 
            // trkBright
            // 
            this.trkBright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.trkBright.LargeChange = 10;
            this.trkBright.Location = new System.Drawing.Point(140, 3);
            this.trkBright.Maximum = 100;
            this.trkBright.Name = "trkBright";
            this.trkBright.Size = new System.Drawing.Size(196, 20);
            this.trkBright.TabIndex = 1;
            this.trkBright.Scroll += new System.EventHandler(this.trkBright_Scroll);
            // 
            // pnlBase
            // 
            this.pnlBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBase.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBase.Controls.Add(this.btnFactory);
            this.pnlBase.Controls.Add(this.btnSend);
            this.pnlBase.Location = new System.Drawing.Point(1, 302);
            this.pnlBase.Name = "pnlBase";
            this.pnlBase.Size = new System.Drawing.Size(353, 40);
            this.pnlBase.TabIndex = 0;
            // 
            // comTimer
            // 
            this.comTimer.Interval = 2000;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.sendToolStripMenuItem,
            this.toolStripSeparator1,
            this.allDefaultToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(156, 76);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 26);
            this.label3.TabIndex = 16;
            this.label3.Text = "Mavlink system id:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 26);
            this.label5.TabIndex = 18;
            this.label5.Text = "Mavlink heartbeat:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkHeartBeat
            // 
            this.chkHeartBeat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkHeartBeat.Location = new System.Drawing.Point(140, 217);
            this.chkHeartBeat.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.chkHeartBeat.Name = "chkHeartBeat";
            this.chkHeartBeat.Size = new System.Drawing.Size(111, 17);
            this.chkHeartBeat.TabIndex = 19;
            this.chkHeartBeat.Text = "Enabled";
            this.chkHeartBeat.UseVisualStyleBackColor = true;
            this.chkHeartBeat.CheckedChanged += new System.EventHandler(this.chkHeartBeat_CheckedChanged);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 163);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Advanced settings";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Image = global::MavpixelGUI.Properties.Resources.arrow_refresh;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.refreshToolStripMenuItem.Text = "&Reload Settings";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // sendToolStripMenuItem
            // 
            this.sendToolStripMenuItem.Image = global::MavpixelGUI.Properties.Resources.page_go;
            this.sendToolStripMenuItem.Name = "sendToolStripMenuItem";
            this.sendToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.sendToolStripMenuItem.Text = "S&end";
            this.sendToolStripMenuItem.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // allDefaultToolStripMenuItem
            // 
            this.allDefaultToolStripMenuItem.Image = global::MavpixelGUI.Properties.Resources.control_rewind_blue;
            this.allDefaultToolStripMenuItem.Name = "allDefaultToolStripMenuItem";
            this.allDefaultToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.allDefaultToolStripMenuItem.Text = "&All Default";
            this.allDefaultToolStripMenuItem.Click += new System.EventHandler(this.allDefaultToolStripMenuItem_Click);
            // 
            // btnFactory
            // 
            this.btnFactory.BackColor = System.Drawing.Color.Salmon;
            this.btnFactory.FlatAppearance.BorderColor = System.Drawing.Color.DarkRed;
            this.btnFactory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Tomato;
            this.btnFactory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFactory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFactory.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnFactory.Image = global::MavpixelGUI.Properties.Resources.cancel;
            this.btnFactory.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnFactory.Location = new System.Drawing.Point(6, 8);
            this.btnFactory.Name = "btnFactory";
            this.btnFactory.Size = new System.Drawing.Size(231, 24);
            this.btnFactory.TabIndex = 0;
            this.btnFactory.Text = "Reset Mavpixel to Factory Default";
            this.btnFactory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFactory.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFactory.UseVisualStyleBackColor = false;
            this.btnFactory.Click += new System.EventHandler(this.btnFactory_Click);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.LimeGreen;
            this.btnSend.Enabled = false;
            this.btnSend.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            this.btnSend.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lime;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btnSend.Image = global::MavpixelGUI.Properties.Resources.page_go;
            this.btnSend.Location = new System.Drawing.Point(260, 8);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(85, 24);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSend.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // divider4
            // 
            this.divider4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.divider4.BackColor = System.Drawing.Color.Transparent;
            this.pnlMain.SetColumnSpan(this.divider4, 2);
            this.divider4.ForeColor = System.Drawing.Color.Green;
            this.divider4.Location = new System.Drawing.Point(3, 179);
            this.divider4.Name = "divider4";
            this.divider4.Size = new System.Drawing.Size(333, 1);
            this.divider4.TabIndex = 44;
            // 
            // txtSysid
            // 
            this.txtSysid.AllowSpace = false;
            this.txtSysid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSysid.Location = new System.Drawing.Point(140, 189);
            this.txtSysid.Name = "txtSysid";
            this.txtSysid.Size = new System.Drawing.Size(196, 20);
            this.txtSysid.TabIndex = 17;
            this.txtSysid.Terminator = "";
            this.txtSysid.TextChanged += new System.EventHandler(this.txtSysid_TextChanged);
            // 
            // txtDeadband
            // 
            this.txtDeadband.AllowSpace = false;
            this.txtDeadband.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeadband.Location = new System.Drawing.Point(140, 133);
            this.txtDeadband.Name = "txtDeadband";
            this.txtDeadband.Size = new System.Drawing.Size(196, 20);
            this.txtDeadband.TabIndex = 11;
            this.txtDeadband.Terminator = "us";
            this.txtDeadband.Text = "us";
            this.txtDeadband.TextChanged += new System.EventHandler(this.txtDeadband_TextChanged);
            // 
            // txtMinsats
            // 
            this.txtMinsats.AllowSpace = false;
            this.txtMinsats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMinsats.Location = new System.Drawing.Point(140, 107);
            this.txtMinsats.Name = "txtMinsats";
            this.txtMinsats.Size = new System.Drawing.Size(196, 20);
            this.txtMinsats.TabIndex = 9;
            this.txtMinsats.Terminator = "";
            this.txtMinsats.TextChanged += new System.EventHandler(this.txtMinsats_TextChanged);
            // 
            // txtLowcell
            // 
            this.txtLowcell.AllowSpace = false;
            this.txtLowcell.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLowcell.Location = new System.Drawing.Point(140, 55);
            this.txtLowcell.Name = "txtLowcell";
            this.txtLowcell.Size = new System.Drawing.Size(196, 20);
            this.txtLowcell.TabIndex = 5;
            this.txtLowcell.Terminator = "v";
            this.txtLowcell.Text = "v";
            this.txtLowcell.TextChanged += new System.EventHandler(this.txtLowcell_TextChanged);
            // 
            // txtLowpct
            // 
            this.txtLowpct.AllowSpace = false;
            this.txtLowpct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLowpct.Location = new System.Drawing.Point(140, 81);
            this.txtLowpct.Name = "txtLowpct";
            this.txtLowpct.Size = new System.Drawing.Size(196, 20);
            this.txtLowpct.TabIndex = 7;
            this.txtLowpct.Terminator = "%";
            this.txtLowpct.Text = "%";
            this.txtLowpct.TextChanged += new System.EventHandler(this.txtLowpct_TextChanged);
            // 
            // comms
            // 
            this.comms.CommandsToRead = 11;
            this.comms.CommandsToSend = 11;
            this.comms.DataName = "Settings";
            this.comms.MavlinkParameterCount = 13;
            this.comms.MavlinkParameterStart = 0;
            this.comms.MavlinkParameterTotal = 83;
            this.comms.Timeout = 1000;
            this.comms.ProgressChanged += new MavpixelGUI.Communicator.ProgressChangedEventHandler(this.comms_ProgressChanged);
            this.comms.WriteCommand += new MavpixelGUI.Communicator.WriteCommandEventHandler(this.comms_WriteCommand);
            this.comms.GotMavlinkParam += new MavpixelGUI.Communicator.GotMavlinkEventHandler(this.comms_GotMavlinkParam);
            this.comms.ComStarted += new MavpixelGUI.Communicator.ComStartedEventHandler(this.comms_ComStarted);
            this.comms.WriteMavlink += new MavpixelGUI.Communicator.WriteMavlinkEventHandler(this.comms_WriteMavlink);
            this.comms.GotData += new MavpixelGUI.Communicator.GotDataEventHandler(this.comms_GotData);
            this.comms.ReadCommand += new MavpixelGUI.Communicator.ReadCommandEventHandler(this.comms_ReadCommand);
            this.comms.Completed += new MavpixelGUI.Communicator.CompletedEventHandler(this.comms_Completed);
            // 
            // frmPixSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(354, 342);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.pnlBase);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPixSettings";
            this.Text = "Mavpixel Settings";
            this.Load += new System.EventHandler(this.frmPixSettings_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPixSettings_FormClosing);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkBright)).EndInit();
            this.pnlBase.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnlMain;
        private System.Windows.Forms.ComboBox cbxSoftserial;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkAnim;
        private System.Windows.Forms.ComboBox cbxMavlink;
        private NumericTextBox txtLowcell;
        private NumericTextBox txtLowpct;
        private System.Windows.Forms.Panel pnlBase;
        private NumericTextBox txtMinsats;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFactory;
        private System.Windows.Forms.TrackBar trkBright;
        public System.Windows.Forms.Timer comTimer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private Communicator comms;
        private NumericTextBox txtDeadband;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ToolStripMenuItem sendToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem allDefaultToolStripMenuItem;
        private NumericTextBox txtSysid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkHeartBeat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private Divider divider4;
    }
}