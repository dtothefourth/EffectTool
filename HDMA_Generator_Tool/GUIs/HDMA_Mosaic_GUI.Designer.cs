namespace HDMA_Generator_Tool
{
    partial class HDMA_Mosaic_GUI
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
			this.tbc = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.btnWhlCod = new System.Windows.Forms.Button();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.cmbWhlScnSel = new System.Windows.Forms.ComboBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chbWhlBg4 = new System.Windows.Forms.CheckBox();
			this.chbWhlBg3 = new System.Windows.Forms.CheckBox();
			this.chbWhlBg2 = new System.Windows.Forms.CheckBox();
			this.chbWhlBg1 = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.trbWhlPix = new System.Windows.Forms.TrackBar();
			this.lblWhlPix = new System.Windows.Forms.Label();
			this.pcbWhlMainPic = new System.Windows.Forms.PictureBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.btnLinCod = new System.Windows.Forms.Button();
			this.grpLinChnAdv = new System.Windows.Forms.GroupBox();
			this.cmbLinChn = new System.Windows.Forms.ComboBox();
			this.grpLinChnStd = new System.Windows.Forms.GroupBox();
			this.rdbLinCh5 = new System.Windows.Forms.RadioButton();
			this.rdbLinCh4 = new System.Windows.Forms.RadioButton();
			this.rdbLinCh3 = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.btnLinDwn = new System.Windows.Forms.Button();
			this.btnLinUp = new System.Windows.Forms.Button();
			this.btnLinDel = new System.Windows.Forms.Button();
			this.btnLinNew = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.dgvLinVal = new System.Windows.Forms.DataGridView();
			this.colScanline = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPixel = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colBG = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.grpLinCur = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chbLinBg4 = new System.Windows.Forms.CheckBox();
			this.trbLinPix = new System.Windows.Forms.TrackBar();
			this.chbLinBg3 = new System.Windows.Forms.CheckBox();
			this.chbLinBg2 = new System.Windows.Forms.CheckBox();
			this.chbLinBg1 = new System.Windows.Forms.CheckBox();
			this.nudLinScnLin = new System.Windows.Forms.NumericUpDown();
			this.cmbLinScnSel = new System.Windows.Forms.ComboBox();
			this.pcbLinMainPic = new System.Windows.Forms.PictureBox();
			this.tbc.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trbWhlPix)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcbWhlMainPic)).BeginInit();
			this.tabPage2.SuspendLayout();
			this.grpLinChnAdv.SuspendLayout();
			this.grpLinChnStd.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvLinVal)).BeginInit();
			this.grpLinCur.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trbLinPix)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudLinScnLin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcbLinMainPic)).BeginInit();
			this.SuspendLayout();
			// 
			// tbc
			// 
			this.tbc.Controls.Add(this.tabPage1);
			this.tbc.Controls.Add(this.tabPage2);
			this.tbc.Location = new System.Drawing.Point(12, 12);
			this.tbc.Name = "tbc";
			this.tbc.SelectedIndex = 0;
			this.tbc.Size = new System.Drawing.Size(575, 291);
			this.tbc.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage1.Controls.Add(this.btnWhlCod);
			this.tabPage1.Controls.Add(this.groupBox6);
			this.tabPage1.Controls.Add(this.cmbWhlScnSel);
			this.tabPage1.Controls.Add(this.groupBox3);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Controls.Add(this.pcbWhlMainPic);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(567, 265);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Whole";
			// 
			// btnWhlCod
			// 
			this.btnWhlCod.Location = new System.Drawing.Point(132, 236);
			this.btnWhlCod.Name = "btnWhlCod";
			this.btnWhlCod.Size = new System.Drawing.Size(57, 21);
			this.btnWhlCod.TabIndex = 31;
			this.btnWhlCod.Text = "Code";
			this.btnWhlCod.UseVisualStyleBackColor = true;
			this.btnWhlCod.Click += new System.EventHandler(this.btnWhlCod_Click);
			// 
			// groupBox6
			// 
			this.groupBox6.Location = new System.Drawing.Point(406, 6);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(155, 253);
			this.groupBox6.TabIndex = 14;
			this.groupBox6.TabStop = false;
			// 
			// cmbWhlScnSel
			// 
			this.cmbWhlScnSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbWhlScnSel.FormattingEnabled = true;
			this.cmbWhlScnSel.Location = new System.Drawing.Point(6, 236);
			this.cmbWhlScnSel.Name = "cmbWhlScnSel";
			this.cmbWhlScnSel.Size = new System.Drawing.Size(120, 21);
			this.cmbWhlScnSel.TabIndex = 11;
			this.cmbWhlScnSel.SelectedIndexChanged += new System.EventHandler(this.cmbWhlScnSel_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Location = new System.Drawing.Point(340, 129);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(60, 130);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chbWhlBg4);
			this.groupBox2.Controls.Add(this.chbWhlBg3);
			this.groupBox2.Controls.Add(this.chbWhlBg2);
			this.groupBox2.Controls.Add(this.chbWhlBg1);
			this.groupBox2.Location = new System.Drawing.Point(340, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(60, 117);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Layers";
			// 
			// chbWhlBg4
			// 
			this.chbWhlBg4.AutoSize = true;
			this.chbWhlBg4.Location = new System.Drawing.Point(6, 88);
			this.chbWhlBg4.Name = "chbWhlBg4";
			this.chbWhlBg4.Size = new System.Drawing.Size(50, 17);
			this.chbWhlBg4.TabIndex = 7;
			this.chbWhlBg4.Text = "BG 4";
			this.chbWhlBg4.UseVisualStyleBackColor = true;
			this.chbWhlBg4.CheckedChanged += new System.EventHandler(this.chbWhlBGs_CheckedChanged);
			// 
			// chbWhlBg3
			// 
			this.chbWhlBg3.AutoSize = true;
			this.chbWhlBg3.Location = new System.Drawing.Point(6, 65);
			this.chbWhlBg3.Name = "chbWhlBg3";
			this.chbWhlBg3.Size = new System.Drawing.Size(50, 17);
			this.chbWhlBg3.TabIndex = 6;
			this.chbWhlBg3.Text = "BG 3";
			this.chbWhlBg3.UseVisualStyleBackColor = true;
			this.chbWhlBg3.CheckedChanged += new System.EventHandler(this.chbWhlBGs_CheckedChanged);
			// 
			// chbWhlBg2
			// 
			this.chbWhlBg2.AutoSize = true;
			this.chbWhlBg2.Checked = true;
			this.chbWhlBg2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbWhlBg2.Location = new System.Drawing.Point(6, 42);
			this.chbWhlBg2.Name = "chbWhlBg2";
			this.chbWhlBg2.Size = new System.Drawing.Size(50, 17);
			this.chbWhlBg2.TabIndex = 5;
			this.chbWhlBg2.Text = "BG 2";
			this.chbWhlBg2.UseVisualStyleBackColor = true;
			this.chbWhlBg2.CheckedChanged += new System.EventHandler(this.chbWhlBGs_CheckedChanged);
			// 
			// chbWhlBg1
			// 
			this.chbWhlBg1.AutoSize = true;
			this.chbWhlBg1.Checked = true;
			this.chbWhlBg1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbWhlBg1.Location = new System.Drawing.Point(6, 19);
			this.chbWhlBg1.Name = "chbWhlBg1";
			this.chbWhlBg1.Size = new System.Drawing.Size(50, 17);
			this.chbWhlBg1.TabIndex = 4;
			this.chbWhlBg1.Text = "BG 1";
			this.chbWhlBg1.UseVisualStyleBackColor = true;
			this.chbWhlBg1.CheckedChanged += new System.EventHandler(this.chbWhlBGs_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.trbWhlPix);
			this.groupBox1.Controls.Add(this.lblWhlPix);
			this.groupBox1.Location = new System.Drawing.Point(268, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(66, 253);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pixelation";
			// 
			// trbWhlPix
			// 
			this.trbWhlPix.Location = new System.Drawing.Point(9, 19);
			this.trbWhlPix.Maximum = 16;
			this.trbWhlPix.Minimum = 1;
			this.trbWhlPix.Name = "trbWhlPix";
			this.trbWhlPix.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trbWhlPix.Size = new System.Drawing.Size(45, 205);
			this.trbWhlPix.TabIndex = 1;
			this.trbWhlPix.Value = 1;
			this.trbWhlPix.Scroll += new System.EventHandler(this.trbWhlPix_Scroll);
			// 
			// lblWhlPix
			// 
			this.lblWhlPix.Location = new System.Drawing.Point(6, 227);
			this.lblWhlPix.Name = "lblWhlPix";
			this.lblWhlPix.Size = new System.Drawing.Size(48, 17);
			this.lblWhlPix.TabIndex = 2;
			this.lblWhlPix.Text = "1x1";
			this.lblWhlPix.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// pcbWhlMainPic
			// 
			this.pcbWhlMainPic.BackColor = System.Drawing.Color.Black;
			this.pcbWhlMainPic.Location = new System.Drawing.Point(6, 6);
			this.pcbWhlMainPic.Name = "pcbWhlMainPic";
			this.pcbWhlMainPic.Size = new System.Drawing.Size(256, 224);
			this.pcbWhlMainPic.TabIndex = 0;
			this.pcbWhlMainPic.TabStop = false;
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage2.Controls.Add(this.btnLinCod);
			this.tabPage2.Controls.Add(this.grpLinChnAdv);
			this.tabPage2.Controls.Add(this.grpLinChnStd);
			this.tabPage2.Controls.Add(this.groupBox4);
			this.tabPage2.Controls.Add(this.groupBox5);
			this.tabPage2.Controls.Add(this.grpLinCur);
			this.tabPage2.Controls.Add(this.cmbLinScnSel);
			this.tabPage2.Controls.Add(this.pcbLinMainPic);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(567, 265);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Lines";
			// 
			// btnLinCod
			// 
			this.btnLinCod.Location = new System.Drawing.Point(132, 236);
			this.btnLinCod.Name = "btnLinCod";
			this.btnLinCod.Size = new System.Drawing.Size(57, 21);
			this.btnLinCod.TabIndex = 31;
			this.btnLinCod.Text = "Code";
			this.btnLinCod.UseVisualStyleBackColor = true;
			this.btnLinCod.Click += new System.EventHandler(this.btnLinCod_Click);
			// 
			// grpLinChnAdv
			// 
			this.grpLinChnAdv.Controls.Add(this.cmbLinChn);
			this.grpLinChnAdv.Location = new System.Drawing.Point(568, 151);
			this.grpLinChnAdv.Name = "grpLinChnAdv";
			this.grpLinChnAdv.Size = new System.Drawing.Size(67, 106);
			this.grpLinChnAdv.TabIndex = 23;
			this.grpLinChnAdv.TabStop = false;
			this.grpLinChnAdv.Text = "Channel";
			this.grpLinChnAdv.Visible = false;
			// 
			// cmbLinChn
			// 
			this.cmbLinChn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLinChn.FormattingEnabled = true;
			this.cmbLinChn.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
			this.cmbLinChn.Location = new System.Drawing.Point(6, 22);
			this.cmbLinChn.Name = "cmbLinChn";
			this.cmbLinChn.Size = new System.Drawing.Size(55, 21);
			this.cmbLinChn.TabIndex = 0;
			this.cmbLinChn.SelectedIndexChanged += new System.EventHandler(this.cmbLinChn_SelectedIndexChanged);
			// 
			// grpLinChnStd
			// 
			this.grpLinChnStd.Controls.Add(this.rdbLinCh5);
			this.grpLinChnStd.Controls.Add(this.rdbLinCh4);
			this.grpLinChnStd.Controls.Add(this.rdbLinCh3);
			this.grpLinChnStd.Location = new System.Drawing.Point(495, 151);
			this.grpLinChnStd.Name = "grpLinChnStd";
			this.grpLinChnStd.Size = new System.Drawing.Size(67, 106);
			this.grpLinChnStd.TabIndex = 22;
			this.grpLinChnStd.TabStop = false;
			this.grpLinChnStd.Text = "Channel";
			// 
			// rdbLinCh5
			// 
			this.rdbLinCh5.AutoSize = true;
			this.rdbLinCh5.Location = new System.Drawing.Point(6, 65);
			this.rdbLinCh5.Name = "rdbLinCh5";
			this.rdbLinCh5.Size = new System.Drawing.Size(49, 17);
			this.rdbLinCh5.TabIndex = 24;
			this.rdbLinCh5.TabStop = true;
			this.rdbLinCh5.Text = "CH 5";
			this.rdbLinCh5.UseVisualStyleBackColor = true;
			this.rdbLinCh5.CheckedChanged += new System.EventHandler(this.chbLinChn_CheckedChanged);
			// 
			// rdbLinCh4
			// 
			this.rdbLinCh4.AutoSize = true;
			this.rdbLinCh4.Location = new System.Drawing.Point(6, 42);
			this.rdbLinCh4.Name = "rdbLinCh4";
			this.rdbLinCh4.Size = new System.Drawing.Size(49, 17);
			this.rdbLinCh4.TabIndex = 23;
			this.rdbLinCh4.TabStop = true;
			this.rdbLinCh4.Text = "CH 4";
			this.rdbLinCh4.UseVisualStyleBackColor = true;
			this.rdbLinCh4.CheckedChanged += new System.EventHandler(this.chbLinChn_CheckedChanged);
			// 
			// rdbLinCh3
			// 
			this.rdbLinCh3.AutoSize = true;
			this.rdbLinCh3.Checked = true;
			this.rdbLinCh3.Location = new System.Drawing.Point(6, 19);
			this.rdbLinCh3.Name = "rdbLinCh3";
			this.rdbLinCh3.Size = new System.Drawing.Size(49, 17);
			this.rdbLinCh3.TabIndex = 22;
			this.rdbLinCh3.TabStop = true;
			this.rdbLinCh3.Text = "CH 3";
			this.rdbLinCh3.UseVisualStyleBackColor = true;
			this.rdbLinCh3.CheckedChanged += new System.EventHandler(this.chbLinChn_CheckedChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.btnLinDwn);
			this.groupBox4.Controls.Add(this.btnLinUp);
			this.groupBox4.Controls.Add(this.btnLinDel);
			this.groupBox4.Controls.Add(this.btnLinNew);
			this.groupBox4.Location = new System.Drawing.Point(495, 6);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(67, 139);
			this.groupBox4.TabIndex = 21;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Controls";
			// 
			// btnLinDwn
			// 
			this.btnLinDwn.Location = new System.Drawing.Point(6, 105);
			this.btnLinDwn.Name = "btnLinDwn";
			this.btnLinDwn.Size = new System.Drawing.Size(53, 23);
			this.btnLinDwn.TabIndex = 24;
			this.btnLinDwn.Text = "Down";
			this.btnLinDwn.UseVisualStyleBackColor = true;
			this.btnLinDwn.Click += new System.EventHandler(this.btnLinDwn_Click);
			// 
			// btnLinUp
			// 
			this.btnLinUp.Location = new System.Drawing.Point(6, 76);
			this.btnLinUp.Name = "btnLinUp";
			this.btnLinUp.Size = new System.Drawing.Size(53, 23);
			this.btnLinUp.TabIndex = 23;
			this.btnLinUp.Text = "Up";
			this.btnLinUp.UseVisualStyleBackColor = true;
			this.btnLinUp.Click += new System.EventHandler(this.btnLinUp_Click);
			// 
			// btnLinDel
			// 
			this.btnLinDel.Location = new System.Drawing.Point(6, 47);
			this.btnLinDel.Name = "btnLinDel";
			this.btnLinDel.Size = new System.Drawing.Size(53, 23);
			this.btnLinDel.TabIndex = 22;
			this.btnLinDel.Text = "Delete";
			this.btnLinDel.UseVisualStyleBackColor = true;
			this.btnLinDel.Click += new System.EventHandler(this.btnLinDel_Click);
			// 
			// btnLinNew
			// 
			this.btnLinNew.Location = new System.Drawing.Point(6, 18);
			this.btnLinNew.Name = "btnLinNew";
			this.btnLinNew.Size = new System.Drawing.Size(53, 23);
			this.btnLinNew.TabIndex = 21;
			this.btnLinNew.Text = "New";
			this.btnLinNew.UseVisualStyleBackColor = true;
			this.btnLinNew.Click += new System.EventHandler(this.btnLinNew_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.dgvLinVal);
			this.groupBox5.Location = new System.Drawing.Point(268, 106);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(221, 151);
			this.groupBox5.TabIndex = 20;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Values";
			// 
			// dgvLinVal
			// 
			this.dgvLinVal.AllowUserToAddRows = false;
			this.dgvLinVal.AllowUserToResizeRows = false;
			this.dgvLinVal.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvLinVal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvLinVal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colScanline,
            this.colPixel,
            this.colBG});
			this.dgvLinVal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvLinVal.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.dgvLinVal.Location = new System.Drawing.Point(3, 16);
			this.dgvLinVal.MultiSelect = false;
			this.dgvLinVal.Name = "dgvLinVal";
			this.dgvLinVal.RowHeadersVisible = false;
			this.dgvLinVal.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvLinVal.Size = new System.Drawing.Size(215, 132);
			this.dgvLinVal.TabIndex = 6;
			this.dgvLinVal.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvLinVal_RowsAdded);
			this.dgvLinVal.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvLinVal_RowsRemoved);
			this.dgvLinVal.SelectionChanged += new System.EventHandler(this.dgvLinVal_SelectionChanged);
			// 
			// colScanline
			// 
			this.colScanline.HeaderText = "Scanlines";
			this.colScanline.Name = "colScanline";
			// 
			// colPixel
			// 
			this.colPixel.HeaderText = "Pixel";
			this.colPixel.Name = "colPixel";
			// 
			// colBG
			// 
			this.colBG.HeaderText = "BGs";
			this.colBG.Name = "colBG";
			// 
			// grpLinCur
			// 
			this.grpLinCur.Controls.Add(this.label2);
			this.grpLinCur.Controls.Add(this.chbLinBg4);
			this.grpLinCur.Controls.Add(this.trbLinPix);
			this.grpLinCur.Controls.Add(this.chbLinBg3);
			this.grpLinCur.Controls.Add(this.chbLinBg2);
			this.grpLinCur.Controls.Add(this.chbLinBg1);
			this.grpLinCur.Controls.Add(this.nudLinScnLin);
			this.grpLinCur.Enabled = false;
			this.grpLinCur.Location = new System.Drawing.Point(268, 6);
			this.grpLinCur.Name = "grpLinCur";
			this.grpLinCur.Size = new System.Drawing.Size(221, 94);
			this.grpLinCur.TabIndex = 19;
			this.grpLinCur.TabStop = false;
			this.grpLinCur.Text = "Current";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(162, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 13);
			this.label2.TabIndex = 19;
			this.label2.Text = "Scanlines:";
			// 
			// chbLinBg4
			// 
			this.chbLinBg4.AutoSize = true;
			this.chbLinBg4.Location = new System.Drawing.Point(165, 24);
			this.chbLinBg4.Name = "chbLinBg4";
			this.chbLinBg4.Size = new System.Drawing.Size(47, 17);
			this.chbLinBg4.TabIndex = 18;
			this.chbLinBg4.Text = "BG4";
			this.chbLinBg4.UseVisualStyleBackColor = true;
			this.chbLinBg4.CheckedChanged += new System.EventHandler(this.chbLinBGs_CheckedChanged);
			// 
			// trbLinPix
			// 
			this.trbLinPix.Location = new System.Drawing.Point(6, 47);
			this.trbLinPix.Maximum = 16;
			this.trbLinPix.Minimum = 1;
			this.trbLinPix.Name = "trbLinPix";
			this.trbLinPix.Size = new System.Drawing.Size(150, 45);
			this.trbLinPix.TabIndex = 10;
			this.trbLinPix.Value = 1;
			this.trbLinPix.Scroll += new System.EventHandler(this.trbLinPix_Scroll);
			// 
			// chbLinBg3
			// 
			this.chbLinBg3.AutoSize = true;
			this.chbLinBg3.Location = new System.Drawing.Point(112, 24);
			this.chbLinBg3.Name = "chbLinBg3";
			this.chbLinBg3.Size = new System.Drawing.Size(47, 17);
			this.chbLinBg3.TabIndex = 17;
			this.chbLinBg3.Text = "BG3";
			this.chbLinBg3.UseVisualStyleBackColor = true;
			this.chbLinBg3.CheckedChanged += new System.EventHandler(this.chbLinBGs_CheckedChanged);
			// 
			// chbLinBg2
			// 
			this.chbLinBg2.AutoSize = true;
			this.chbLinBg2.Checked = true;
			this.chbLinBg2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbLinBg2.Location = new System.Drawing.Point(59, 24);
			this.chbLinBg2.Name = "chbLinBg2";
			this.chbLinBg2.Size = new System.Drawing.Size(47, 17);
			this.chbLinBg2.TabIndex = 16;
			this.chbLinBg2.Text = "BG2";
			this.chbLinBg2.UseVisualStyleBackColor = true;
			this.chbLinBg2.CheckedChanged += new System.EventHandler(this.chbLinBGs_CheckedChanged);
			// 
			// chbLinBg1
			// 
			this.chbLinBg1.AutoSize = true;
			this.chbLinBg1.Checked = true;
			this.chbLinBg1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chbLinBg1.Location = new System.Drawing.Point(6, 24);
			this.chbLinBg1.Name = "chbLinBg1";
			this.chbLinBg1.Size = new System.Drawing.Size(47, 17);
			this.chbLinBg1.TabIndex = 15;
			this.chbLinBg1.Text = "BG1";
			this.chbLinBg1.UseVisualStyleBackColor = true;
			this.chbLinBg1.CheckedChanged += new System.EventHandler(this.chbLinBGs_CheckedChanged);
			// 
			// nudLinScnLin
			// 
			this.nudLinScnLin.Location = new System.Drawing.Point(165, 60);
			this.nudLinScnLin.Maximum = new decimal(new int[] {
            112,
            0,
            0,
            0});
			this.nudLinScnLin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudLinScnLin.Name = "nudLinScnLin";
			this.nudLinScnLin.Size = new System.Drawing.Size(50, 20);
			this.nudLinScnLin.TabIndex = 14;
			this.nudLinScnLin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudLinScnLin.ValueChanged += new System.EventHandler(this.nudLinScnLin_ValueChanged);
			// 
			// cmbLinScnSel
			// 
			this.cmbLinScnSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLinScnSel.FormattingEnabled = true;
			this.cmbLinScnSel.Location = new System.Drawing.Point(6, 236);
			this.cmbLinScnSel.Name = "cmbLinScnSel";
			this.cmbLinScnSel.Size = new System.Drawing.Size(120, 21);
			this.cmbLinScnSel.TabIndex = 13;
			this.cmbLinScnSel.SelectedIndexChanged += new System.EventHandler(this.cmbLinScnSel_SelectedIndexChanged);
			// 
			// pcbLinMainPic
			// 
			this.pcbLinMainPic.BackColor = System.Drawing.Color.Black;
			this.pcbLinMainPic.Location = new System.Drawing.Point(6, 6);
			this.pcbLinMainPic.Name = "pcbLinMainPic";
			this.pcbLinMainPic.Size = new System.Drawing.Size(256, 224);
			this.pcbLinMainPic.TabIndex = 12;
			this.pcbLinMainPic.TabStop = false;
			// 
			// HDMA_Mosaic_GUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(851, 318);
			this.Controls.Add(this.tbc);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "HDMA_Mosaic_GUI";
			this.Text = "HDMA_Mosaic_GUI";
			this.tbc.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trbWhlPix)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcbWhlMainPic)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.grpLinChnAdv.ResumeLayout(false);
			this.grpLinChnStd.ResumeLayout(false);
			this.grpLinChnStd.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvLinVal)).EndInit();
			this.grpLinCur.ResumeLayout(false);
			this.grpLinCur.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trbLinPix)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudLinScnLin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcbLinMainPic)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbc;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TrackBar trbWhlPix;
        private System.Windows.Forms.PictureBox pcbWhlMainPic;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chbWhlBg4;
        private System.Windows.Forms.CheckBox chbWhlBg3;
        private System.Windows.Forms.CheckBox chbWhlBg2;
        private System.Windows.Forms.CheckBox chbWhlBg1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblWhlPix;
        private System.Windows.Forms.ComboBox cmbWhlScnSel;
        private System.Windows.Forms.GroupBox grpLinCur;
        private System.Windows.Forms.CheckBox chbLinBg4;
        private System.Windows.Forms.TrackBar trbLinPix;
        private System.Windows.Forms.CheckBox chbLinBg3;
        private System.Windows.Forms.CheckBox chbLinBg2;
        private System.Windows.Forms.CheckBox chbLinBg1;
        private System.Windows.Forms.NumericUpDown nudLinScnLin;
        private System.Windows.Forms.ComboBox cmbLinScnSel;
		private System.Windows.Forms.PictureBox pcbLinMainPic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView dgvLinVal;
        private System.Windows.Forms.Button btnLinUp;
        private System.Windows.Forms.Button btnLinDel;
        private System.Windows.Forms.Button btnLinNew;
        private System.Windows.Forms.Button btnLinDwn;
        private System.Windows.Forms.GroupBox grpLinChnStd;
        private System.Windows.Forms.GroupBox grpLinChnAdv;
        private System.Windows.Forms.ComboBox cmbLinChn;
        private System.Windows.Forms.RadioButton rdbLinCh5;
        private System.Windows.Forms.RadioButton rdbLinCh4;
        private System.Windows.Forms.RadioButton rdbLinCh3;
		private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScanline;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPixel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBG;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnWhlCod;
        private System.Windows.Forms.Button btnLinCod;
    }
}