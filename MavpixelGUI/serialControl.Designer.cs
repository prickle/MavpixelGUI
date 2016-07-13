namespace MavpixelGUI
{
    partial class serialControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chkOpen = new System.Windows.Forms.CheckBox();
            this.cbxBaud = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbxPort = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.comTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(264, 24);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.28571F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.71429F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.chkOpen, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbxBaud, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbxPort, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(264, 24);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // chkOpen
            // 
            this.chkOpen.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkOpen.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkOpen.Image = global::MavpixelGUI.Properties.Resources.disconnect;
            this.chkOpen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkOpen.Location = new System.Drawing.Point(172, 0);
            this.chkOpen.Margin = new System.Windows.Forms.Padding(0);
            this.chkOpen.Name = "chkOpen";
            this.chkOpen.Size = new System.Drawing.Size(91, 24);
            this.chkOpen.TabIndex = 20;
            this.chkOpen.Text = "Connect";
            this.chkOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkOpen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.chkOpen.UseVisualStyleBackColor = true;
            this.chkOpen.CheckedChanged += new System.EventHandler(this.chkOpen_CheckedChanged);
            // 
            // cbxBaud
            // 
            this.cbxBaud.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxBaud.FormattingEnabled = true;
            this.cbxBaud.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cbxBaud.Location = new System.Drawing.Point(109, 1);
            this.cbxBaud.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.cbxBaud.Name = "cbxBaud";
            this.cbxBaud.Size = new System.Drawing.Size(61, 21);
            this.cbxBaud.TabIndex = 21;
            this.cbxBaud.TextChanged += new System.EventHandler(this.cbxBaud_TextChanged);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(0, 5);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Port";
            // 
            // cbxPort
            // 
            this.cbxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxPort.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxPort.FormattingEnabled = true;
            this.cbxPort.Location = new System.Drawing.Point(30, 1);
            this.cbxPort.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.cbxPort.Name = "cbxPort";
            this.cbxPort.Size = new System.Drawing.Size(75, 21);
            this.cbxPort.TabIndex = 19;
            this.cbxPort.Text = "Scanning..";
            this.cbxPort.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbxPort_DrawItem);
            this.cbxPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbxPort_KeyPress);
            this.cbxPort.DropDownClosed += new System.EventHandler(this.cbxPort_DropDownClosed);
            this.cbxPort.DropDown += new System.EventHandler(this.cbxPort_DropDown);
            this.cbxPort.TextChanged += new System.EventHandler(this.cbxPort_TextChanged);
            // 
            // comTimer
            // 
            this.comTimer.Tick += new System.EventHandler(this.comTimer_Tick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(264, 90);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // serialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.MinimumSize = new System.Drawing.Size(264, 28);
            this.Name = "serialControl";
            this.Size = new System.Drawing.Size(264, 90);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbxBaud;
        private System.Windows.Forms.CheckBox chkOpen;
        private System.Windows.Forms.ComboBox cbxPort;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer comTimer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

    }
}
