namespace HDMA_Generator_Tool
{
    partial class ChooseChannel
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
            this.cmbHigh = new System.Windows.Forms.ComboBox();
            this.cmbLow = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbLeast = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmbHigh
            // 
            this.cmbHigh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHigh.FormattingEnabled = true;
            this.cmbHigh.Items.AddRange(new object[] {
            "3",
            "4",
            "5"});
            this.cmbHigh.Location = new System.Drawing.Point(156, 12);
            this.cmbHigh.Name = "cmbHigh";
            this.cmbHigh.Size = new System.Drawing.Size(47, 21);
            this.cmbHigh.TabIndex = 0;
            this.cmbHigh.SelectedIndexChanged += new System.EventHandler(this.cmbHigh_SelectedIndexChanged);
            // 
            // cmbLow
            // 
            this.cmbLow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLow.FormattingEnabled = true;
            this.cmbLow.Items.AddRange(new object[] {
            "4",
            "5"});
            this.cmbLow.Location = new System.Drawing.Point(156, 39);
            this.cmbLow.Name = "cmbLow";
            this.cmbLow.Size = new System.Drawing.Size(47, 21);
            this.cmbLow.TabIndex = 1;
            this.cmbLow.SelectedIndexChanged += new System.EventHandler(this.cmbLow_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Channel with higher priority:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Channel with lower priority:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(73, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 22);
            this.button1.TabIndex = 4;
            this.button1.Text = "Done";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Channel with least priority:";
            // 
            // cmbLeast
            // 
            this.cmbLeast.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLeast.FormattingEnabled = true;
            this.cmbLeast.Items.AddRange(new object[] {
            "4",
            "5"});
            this.cmbLeast.Location = new System.Drawing.Point(156, 66);
            this.cmbLeast.Name = "cmbLeast";
            this.cmbLeast.Size = new System.Drawing.Size(47, 21);
            this.cmbLeast.TabIndex = 5;
            // 
            // ChooseChannel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 122);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbLeast);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbLow);
            this.Controls.Add(this.cmbHigh);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseChannel";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Choose Channel";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChooseChannel_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbHigh;
        private System.Windows.Forms.ComboBox cmbLow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbLeast;
    }
}