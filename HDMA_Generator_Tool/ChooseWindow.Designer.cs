namespace HDMA_Generator_Tool
{
    partial class ChooseWindow
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
			this.btnDone = new System.Windows.Forms.Button();
			this.rdbWinOne = new System.Windows.Forms.RadioButton();
			this.rdbWinTwo = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// btnDone
			// 
			this.btnDone.Location = new System.Drawing.Point(48, 35);
			this.btnDone.Name = "btnDone";
			this.btnDone.Size = new System.Drawing.Size(74, 22);
			this.btnDone.TabIndex = 5;
			this.btnDone.Text = "Done";
			this.btnDone.UseVisualStyleBackColor = true;
			this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
			// 
			// rdbWinOne
			// 
			this.rdbWinOne.AutoSize = true;
			this.rdbWinOne.Checked = true;
			this.rdbWinOne.Location = new System.Drawing.Point(12, 12);
			this.rdbWinOne.Name = "rdbWinOne";
			this.rdbWinOne.Size = new System.Drawing.Size(73, 17);
			this.rdbWinOne.TabIndex = 6;
			this.rdbWinOne.TabStop = true;
			this.rdbWinOne.Text = "Window 1";
			this.rdbWinOne.UseVisualStyleBackColor = true;
			this.rdbWinOne.CheckedChanged += new System.EventHandler(this.rdbWinOne_CheckedChanged);
			// 
			// rdbWinTwo
			// 
			this.rdbWinTwo.AutoSize = true;
			this.rdbWinTwo.Location = new System.Drawing.Point(91, 12);
			this.rdbWinTwo.Name = "rdbWinTwo";
			this.rdbWinTwo.Size = new System.Drawing.Size(73, 17);
			this.rdbWinTwo.TabIndex = 7;
			this.rdbWinTwo.Text = "Window 2";
			this.rdbWinTwo.UseVisualStyleBackColor = true;
			this.rdbWinTwo.CheckedChanged += new System.EventHandler(this.rdbWinTwo_CheckedChanged);
			// 
			// ChoseWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(182, 72);
			this.Controls.Add(this.rdbWinTwo);
			this.Controls.Add(this.rdbWinOne);
			this.Controls.Add(this.btnDone);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChoseWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Choose Window";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button btnDone;
		private System.Windows.Forms.RadioButton rdbWinOne;
		private System.Windows.Forms.RadioButton rdbWinTwo;
    }
}