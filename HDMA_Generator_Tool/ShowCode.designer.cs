namespace HDMA_Generator_Tool
{
    partial class ShowCode
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
            this.rtbInit = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.spcCode = new System.Windows.Forms.SplitContainer();
            this.rtbMain = new System.Windows.Forms.RichTextBox();
            this.btnMainToClip = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btnInitToClip = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcCode)).BeginInit();
            this.spcCode.Panel1.SuspendLayout();
            this.spcCode.Panel2.SuspendLayout();
            this.spcCode.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbInit
            // 
            this.rtbInit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInit.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbInit.Location = new System.Drawing.Point(0, 0);
            this.rtbInit.Name = "rtbInit";
            this.rtbInit.ReadOnly = true;
            this.rtbInit.Size = new System.Drawing.Size(188, 220);
            this.rtbInit.TabIndex = 0;
            this.rtbInit.Text = "LDA #$01\nSTA $something\n\n.table\n   db $01";
            this.rtbInit.WordWrap = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.spcCode);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnMainToClip);
            this.splitContainer1.Panel2.Controls.Add(this.btn_Save);
            this.splitContainer1.Panel2.Controls.Add(this.btnInitToClip);
            this.splitContainer1.Size = new System.Drawing.Size(394, 262);
            this.splitContainer1.SplitterDistance = 220;
            this.splitContainer1.TabIndex = 1;
            // 
            // spcCode
            // 
            this.spcCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcCode.Location = new System.Drawing.Point(0, 0);
            this.spcCode.Name = "spcCode";
            // 
            // spcCode.Panel1
            // 
            this.spcCode.Panel1.Controls.Add(this.rtbInit);
            // 
            // spcCode.Panel2
            // 
            this.spcCode.Panel2.Controls.Add(this.rtbMain);
            this.spcCode.Size = new System.Drawing.Size(394, 220);
            this.spcCode.SplitterDistance = 188;
            this.spcCode.TabIndex = 1;
            // 
            // rtbMain
            // 
            this.rtbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMain.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbMain.Location = new System.Drawing.Point(0, 0);
            this.rtbMain.Name = "rtbMain";
            this.rtbMain.ReadOnly = true;
            this.rtbMain.Size = new System.Drawing.Size(202, 220);
            this.rtbMain.TabIndex = 1;
            this.rtbMain.Text = "Main:\n   LDA #$01\n   STA $something\n\n   .table\n      db $01";
            this.rtbMain.WordWrap = false;
            // 
            // btnMainToClip
            // 
            this.btnMainToClip.Location = new System.Drawing.Point(240, 3);
            this.btnMainToClip.Name = "btnMainToClip";
            this.btnMainToClip.Size = new System.Drawing.Size(137, 23);
            this.btnMainToClip.TabIndex = 2;
            this.btnMainToClip.Text = "Copy MAIN to Clipboard";
            this.btnMainToClip.UseVisualStyleBackColor = true;
            this.btnMainToClip.Click += new System.EventHandler(this.btnMainToClip_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(145, 2);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(89, 23);
            this.btn_Save.TabIndex = 1;
            this.btn_Save.Text = "Save as ASM";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_AsASM_Click);
            // 
            // btnInitToClip
            // 
            this.btnInitToClip.Location = new System.Drawing.Point(12, 2);
            this.btnInitToClip.Name = "btnInitToClip";
            this.btnInitToClip.Size = new System.Drawing.Size(127, 23);
            this.btnInitToClip.TabIndex = 0;
            this.btnInitToClip.Text = "Copy INIT to Clipboard";
            this.btnInitToClip.UseVisualStyleBackColor = true;
            this.btnInitToClip.Click += new System.EventHandler(this.btnInitToClip_Click);
            // 
            // ShowCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 262);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(256, 165);
            this.Name = "ShowCode";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Code";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.spcCode.Panel1.ResumeLayout(false);
            this.spcCode.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcCode)).EndInit();
            this.spcCode.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbInit;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnInitToClip;
        private System.Windows.Forms.Button btn_Save;
		private System.Windows.Forms.SplitContainer spcCode;
		private System.Windows.Forms.RichTextBox rtbMain;
        private System.Windows.Forms.Button btnMainToClip;
    }
}