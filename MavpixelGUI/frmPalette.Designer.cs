namespace MavpixelGUI
{
    partial class frmPalette
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPalette));
            this.pnlBase = new System.Windows.Forms.Panel();
            this.btnColor0 = new System.Windows.Forms.Button();
            this.btnColor5 = new System.Windows.Forms.Button();
            this.btnColor4 = new System.Windows.Forms.Button();
            this.btnColor6 = new System.Windows.Forms.Button();
            this.btnColor3 = new System.Windows.Forms.Button();
            this.btnColor7 = new System.Windows.Forms.Button();
            this.btnColor15 = new System.Windows.Forms.Button();
            this.btnColor2 = new System.Windows.Forms.Button();
            this.btnColor8 = new System.Windows.Forms.Button();
            this.btnColor14 = new System.Windows.Forms.Button();
            this.btnColor1 = new System.Windows.Forms.Button();
            this.btnColor9 = new System.Windows.Forms.Button();
            this.btnColor13 = new System.Windows.Forms.Button();
            this.btnColor10 = new System.Windows.Forms.Button();
            this.btnColor11 = new System.Windows.Forms.Button();
            this.btnColor12 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.pnlColors = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.btnAllDefault = new System.Windows.Forms.Button();
            this.rbxPrompt = new MavpixelGUI.RoundBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comms = new MavpixelGUI.Communicator();
            this.pnlBase.SuspendLayout();
            this.pnlColors.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.rbxPrompt.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBase
            // 
            this.pnlBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBase.BackColor = System.Drawing.SystemColors.Control;
            this.pnlBase.Controls.Add(this.btnSend);
            this.pnlBase.Controls.Add(this.btnDefault);
            this.pnlBase.Controls.Add(this.btnAllDefault);
            this.pnlBase.Location = new System.Drawing.Point(0, 124);
            this.pnlBase.Name = "pnlBase";
            this.pnlBase.Size = new System.Drawing.Size(306, 41);
            this.pnlBase.TabIndex = 2;
            // 
            // btnColor0
            // 
            this.btnColor0.BackColor = System.Drawing.Color.Black;
            this.btnColor0.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor0.FlatAppearance.BorderSize = 2;
            this.btnColor0.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor0.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor0.ForeColor = System.Drawing.Color.White;
            this.btnColor0.Location = new System.Drawing.Point(12, 3);
            this.btnColor0.Name = "btnColor0";
            this.btnColor0.Size = new System.Drawing.Size(30, 30);
            this.btnColor0.TabIndex = 0;
            this.btnColor0.Tag = "0";
            this.btnColor0.Text = "0";
            this.btnColor0.UseVisualStyleBackColor = false;
            this.btnColor0.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor5
            // 
            this.btnColor5.BackColor = System.Drawing.Color.GreenYellow;
            this.btnColor5.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor5.FlatAppearance.BorderSize = 2;
            this.btnColor5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor5.Location = new System.Drawing.Point(192, 3);
            this.btnColor5.Name = "btnColor5";
            this.btnColor5.Size = new System.Drawing.Size(30, 30);
            this.btnColor5.TabIndex = 5;
            this.btnColor5.Tag = "5";
            this.btnColor5.Text = "5";
            this.btnColor5.UseVisualStyleBackColor = false;
            this.btnColor5.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor4
            // 
            this.btnColor4.BackColor = System.Drawing.Color.Yellow;
            this.btnColor4.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor4.FlatAppearance.BorderSize = 2;
            this.btnColor4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor4.Location = new System.Drawing.Point(156, 3);
            this.btnColor4.Name = "btnColor4";
            this.btnColor4.Size = new System.Drawing.Size(30, 30);
            this.btnColor4.TabIndex = 4;
            this.btnColor4.Tag = "4";
            this.btnColor4.Text = "4";
            this.btnColor4.UseVisualStyleBackColor = false;
            this.btnColor4.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor6
            // 
            this.btnColor6.BackColor = System.Drawing.Color.LimeGreen;
            this.btnColor6.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor6.FlatAppearance.BorderSize = 2;
            this.btnColor6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor6.Location = new System.Drawing.Point(228, 3);
            this.btnColor6.Name = "btnColor6";
            this.btnColor6.Size = new System.Drawing.Size(30, 30);
            this.btnColor6.TabIndex = 6;
            this.btnColor6.Tag = "6";
            this.btnColor6.Text = "6";
            this.btnColor6.UseVisualStyleBackColor = false;
            this.btnColor6.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor3
            // 
            this.btnColor3.BackColor = System.Drawing.Color.Orange;
            this.btnColor3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor3.FlatAppearance.BorderSize = 2;
            this.btnColor3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor3.Location = new System.Drawing.Point(120, 3);
            this.btnColor3.Name = "btnColor3";
            this.btnColor3.Size = new System.Drawing.Size(30, 30);
            this.btnColor3.TabIndex = 3;
            this.btnColor3.Tag = "3";
            this.btnColor3.Text = "3";
            this.btnColor3.UseVisualStyleBackColor = false;
            this.btnColor3.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor7
            // 
            this.btnColor7.BackColor = System.Drawing.Color.PaleGreen;
            this.btnColor7.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor7.FlatAppearance.BorderSize = 2;
            this.btnColor7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor7.Location = new System.Drawing.Point(264, 3);
            this.btnColor7.Name = "btnColor7";
            this.btnColor7.Size = new System.Drawing.Size(30, 30);
            this.btnColor7.TabIndex = 7;
            this.btnColor7.Tag = "7";
            this.btnColor7.Text = "7";
            this.btnColor7.UseVisualStyleBackColor = false;
            this.btnColor7.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor15
            // 
            this.btnColor15.BackColor = System.Drawing.Color.Black;
            this.btnColor15.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor15.FlatAppearance.BorderSize = 2;
            this.btnColor15.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor15.ForeColor = System.Drawing.Color.White;
            this.btnColor15.ImageKey = "(none)";
            this.btnColor15.Location = new System.Drawing.Point(264, 39);
            this.btnColor15.Name = "btnColor15";
            this.btnColor15.Size = new System.Drawing.Size(30, 30);
            this.btnColor15.TabIndex = 15;
            this.btnColor15.Tag = "15";
            this.btnColor15.Text = "15";
            this.btnColor15.UseCompatibleTextRendering = true;
            this.btnColor15.UseVisualStyleBackColor = false;
            this.btnColor15.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor2
            // 
            this.btnColor2.BackColor = System.Drawing.Color.Red;
            this.btnColor2.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor2.FlatAppearance.BorderSize = 2;
            this.btnColor2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor2.Location = new System.Drawing.Point(84, 3);
            this.btnColor2.Name = "btnColor2";
            this.btnColor2.Size = new System.Drawing.Size(30, 30);
            this.btnColor2.TabIndex = 2;
            this.btnColor2.Tag = "2";
            this.btnColor2.Text = "2";
            this.btnColor2.UseVisualStyleBackColor = false;
            this.btnColor2.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor8
            // 
            this.btnColor8.BackColor = System.Drawing.Color.Cyan;
            this.btnColor8.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor8.FlatAppearance.BorderSize = 2;
            this.btnColor8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor8.Location = new System.Drawing.Point(12, 39);
            this.btnColor8.Name = "btnColor8";
            this.btnColor8.Size = new System.Drawing.Size(30, 30);
            this.btnColor8.TabIndex = 8;
            this.btnColor8.Tag = "8";
            this.btnColor8.Text = "8";
            this.btnColor8.UseVisualStyleBackColor = false;
            this.btnColor8.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor14
            // 
            this.btnColor14.BackColor = System.Drawing.Color.Black;
            this.btnColor14.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor14.FlatAppearance.BorderSize = 2;
            this.btnColor14.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor14.ForeColor = System.Drawing.Color.White;
            this.btnColor14.Location = new System.Drawing.Point(228, 39);
            this.btnColor14.Name = "btnColor14";
            this.btnColor14.Size = new System.Drawing.Size(30, 30);
            this.btnColor14.TabIndex = 14;
            this.btnColor14.Tag = "14";
            this.btnColor14.Text = "14";
            this.btnColor14.UseCompatibleTextRendering = true;
            this.btnColor14.UseVisualStyleBackColor = false;
            this.btnColor14.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor1
            // 
            this.btnColor1.BackColor = System.Drawing.Color.White;
            this.btnColor1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor1.FlatAppearance.BorderSize = 2;
            this.btnColor1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor1.Location = new System.Drawing.Point(48, 3);
            this.btnColor1.Name = "btnColor1";
            this.btnColor1.Size = new System.Drawing.Size(30, 30);
            this.btnColor1.TabIndex = 1;
            this.btnColor1.Tag = "1";
            this.btnColor1.Text = "1";
            this.btnColor1.UseVisualStyleBackColor = false;
            this.btnColor1.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor9
            // 
            this.btnColor9.BackColor = System.Drawing.Color.LightCyan;
            this.btnColor9.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor9.FlatAppearance.BorderSize = 2;
            this.btnColor9.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor9.Location = new System.Drawing.Point(48, 39);
            this.btnColor9.Name = "btnColor9";
            this.btnColor9.Size = new System.Drawing.Size(30, 30);
            this.btnColor9.TabIndex = 9;
            this.btnColor9.Tag = "9";
            this.btnColor9.Text = "9";
            this.btnColor9.UseVisualStyleBackColor = false;
            this.btnColor9.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor13
            // 
            this.btnColor13.BackColor = System.Drawing.Color.DeepPink;
            this.btnColor13.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor13.FlatAppearance.BorderSize = 2;
            this.btnColor13.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor13.Location = new System.Drawing.Point(192, 39);
            this.btnColor13.Name = "btnColor13";
            this.btnColor13.Size = new System.Drawing.Size(30, 30);
            this.btnColor13.TabIndex = 13;
            this.btnColor13.Tag = "13";
            this.btnColor13.Text = "13";
            this.btnColor13.UseCompatibleTextRendering = true;
            this.btnColor13.UseVisualStyleBackColor = false;
            this.btnColor13.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor10
            // 
            this.btnColor10.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnColor10.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor10.FlatAppearance.BorderSize = 2;
            this.btnColor10.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor10.Location = new System.Drawing.Point(84, 39);
            this.btnColor10.Name = "btnColor10";
            this.btnColor10.Size = new System.Drawing.Size(30, 30);
            this.btnColor10.TabIndex = 10;
            this.btnColor10.Tag = "10";
            this.btnColor10.Text = "10";
            this.btnColor10.UseCompatibleTextRendering = true;
            this.btnColor10.UseVisualStyleBackColor = false;
            this.btnColor10.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor11
            // 
            this.btnColor11.BackColor = System.Drawing.Color.DarkViolet;
            this.btnColor11.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor11.FlatAppearance.BorderSize = 2;
            this.btnColor11.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor11.Location = new System.Drawing.Point(120, 39);
            this.btnColor11.Name = "btnColor11";
            this.btnColor11.Size = new System.Drawing.Size(30, 30);
            this.btnColor11.TabIndex = 11;
            this.btnColor11.Tag = "11";
            this.btnColor11.Text = "11";
            this.btnColor11.UseCompatibleTextRendering = true;
            this.btnColor11.UseVisualStyleBackColor = false;
            this.btnColor11.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // btnColor12
            // 
            this.btnColor12.BackColor = System.Drawing.Color.Fuchsia;
            this.btnColor12.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnColor12.FlatAppearance.BorderSize = 2;
            this.btnColor12.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnColor12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnColor12.Location = new System.Drawing.Point(156, 39);
            this.btnColor12.Name = "btnColor12";
            this.btnColor12.Size = new System.Drawing.Size(30, 30);
            this.btnColor12.TabIndex = 12;
            this.btnColor12.Tag = "12";
            this.btnColor12.Text = "12";
            this.btnColor12.UseCompatibleTextRendering = true;
            this.btnColor12.UseVisualStyleBackColor = false;
            this.btnColor12.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // colorDialog1
            // 
            this.colorDialog1.AnyColor = true;
            this.colorDialog1.FullOpen = true;
            this.colorDialog1.SolidColorOnly = true;
            // 
            // pnlColors
            // 
            this.pnlColors.Controls.Add(this.btnColor0);
            this.pnlColors.Controls.Add(this.btnColor12);
            this.pnlColors.Controls.Add(this.btnColor5);
            this.pnlColors.Controls.Add(this.btnColor11);
            this.pnlColors.Controls.Add(this.btnColor4);
            this.pnlColors.Controls.Add(this.btnColor10);
            this.pnlColors.Controls.Add(this.btnColor6);
            this.pnlColors.Controls.Add(this.btnColor13);
            this.pnlColors.Controls.Add(this.btnColor3);
            this.pnlColors.Controls.Add(this.btnColor9);
            this.pnlColors.Controls.Add(this.btnColor7);
            this.pnlColors.Controls.Add(this.btnColor1);
            this.pnlColors.Controls.Add(this.btnColor15);
            this.pnlColors.Controls.Add(this.btnColor14);
            this.pnlColors.Controls.Add(this.btnColor2);
            this.pnlColors.Controls.Add(this.btnColor8);
            this.pnlColors.Location = new System.Drawing.Point(0, 50);
            this.pnlColors.Name = "pnlColors";
            this.pnlColors.Size = new System.Drawing.Size(306, 77);
            this.pnlColors.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.sendToolStripMenuItem,
            this.toolStripSeparator1,
            this.defaultToolStripMenuItem,
            this.allDefaultToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(148, 98);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(144, 6);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Image = global::MavpixelGUI.Properties.Resources.arrow_refresh;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.refreshToolStripMenuItem.Text = "&Reload Colors";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // sendToolStripMenuItem
            // 
            this.sendToolStripMenuItem.Image = global::MavpixelGUI.Properties.Resources.page_go;
            this.sendToolStripMenuItem.Name = "sendToolStripMenuItem";
            this.sendToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.sendToolStripMenuItem.Text = "S&end";
            this.sendToolStripMenuItem.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // defaultToolStripMenuItem
            // 
            this.defaultToolStripMenuItem.Image = global::MavpixelGUI.Properties.Resources.control_play_back_blue;
            this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            this.defaultToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.defaultToolStripMenuItem.Text = "&Default";
            this.defaultToolStripMenuItem.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // allDefaultToolStripMenuItem
            // 
            this.allDefaultToolStripMenuItem.Image = global::MavpixelGUI.Properties.Resources.control_rewind_blue;
            this.allDefaultToolStripMenuItem.Name = "allDefaultToolStripMenuItem";
            this.allDefaultToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.allDefaultToolStripMenuItem.Text = "&All Default";
            this.allDefaultToolStripMenuItem.Click += new System.EventHandler(this.btnAllDefault_Click);
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
            this.btnSend.Location = new System.Drawing.Point(213, 9);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(85, 24);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSend.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnDefault
            // 
            this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDefault.Image = global::MavpixelGUI.Properties.Resources.control_play_back_blue;
            this.btnDefault.Location = new System.Drawing.Point(8, 9);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(75, 24);
            this.btnDefault.TabIndex = 0;
            this.btnDefault.Text = "Default";
            this.btnDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDefault.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // btnAllDefault
            // 
            this.btnAllDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAllDefault.Image = global::MavpixelGUI.Properties.Resources.control_rewind_blue;
            this.btnAllDefault.Location = new System.Drawing.Point(89, 9);
            this.btnAllDefault.Name = "btnAllDefault";
            this.btnAllDefault.Size = new System.Drawing.Size(106, 24);
            this.btnAllDefault.TabIndex = 1;
            this.btnAllDefault.Text = "ALL Default";
            this.btnAllDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAllDefault.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAllDefault.UseVisualStyleBackColor = true;
            this.btnAllDefault.Click += new System.EventHandler(this.btnAllDefault_Click);
            // 
            // rbxPrompt
            // 
            this.rbxPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rbxPrompt.BackColor = System.Drawing.Color.Transparent;
            this.rbxPrompt.BorderColor = System.Drawing.Color.Yellow;
            this.rbxPrompt.Controls.Add(this.label11);
            this.rbxPrompt.FillColor = System.Drawing.Color.MistyRose;
            this.rbxPrompt.Location = new System.Drawing.Point(12, 12);
            this.rbxPrompt.Name = "rbxPrompt";
            this.rbxPrompt.Size = new System.Drawing.Size(282, 32);
            this.rbxPrompt.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.MistyRose;
            this.label11.Location = new System.Drawing.Point(10, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(249, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Alter the true color of Mavpixel\'s LED color indexes.";
            // 
            // comms
            // 
            this.comms.CommandsToRead = 1;
            this.comms.CommandsToSend = 16;
            this.comms.DataName = "palette";
            this.comms.MavlinkParameterCount = 16;
            this.comms.MavlinkParameterStart = 67;
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
            // frmPalette
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(306, 165);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.pnlColors);
            this.Controls.Add(this.pnlBase);
            this.Controls.Add(this.rbxPrompt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPalette";
            this.Text = "Color Palette";
            this.Load += new System.EventHandler(this.frmPalette_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPalette_FormClosing);
            this.pnlBase.ResumeLayout(false);
            this.pnlColors.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.rbxPrompt.ResumeLayout(false);
            this.rbxPrompt.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private RoundBox rbxPrompt;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel pnlBase;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Button btnAllDefault;
        private System.Windows.Forms.Button btnColor0;
        private System.Windows.Forms.Button btnColor5;
        private System.Windows.Forms.Button btnColor4;
        private System.Windows.Forms.Button btnColor6;
        private System.Windows.Forms.Button btnColor3;
        private System.Windows.Forms.Button btnColor7;
        private System.Windows.Forms.Button btnColor15;
        private System.Windows.Forms.Button btnColor2;
        private System.Windows.Forms.Button btnColor8;
        private System.Windows.Forms.Button btnColor14;
        private System.Windows.Forms.Button btnColor1;
        private System.Windows.Forms.Button btnColor9;
        private System.Windows.Forms.Button btnColor13;
        private System.Windows.Forms.Button btnColor10;
        private System.Windows.Forms.Button btnColor11;
        private System.Windows.Forms.Button btnColor12;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel pnlColors;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private Communicator comms;
        public System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ToolStripMenuItem sendToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allDefaultToolStripMenuItem;
    }
}