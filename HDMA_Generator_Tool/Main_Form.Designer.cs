namespace HDMA_Generator_Tool
{
    partial class Main_Form
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("BG Gradient");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("FG Gradient");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Brightness");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Gradients", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Parallax");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Waves");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Scrolling", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Windows");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Mosaic");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Color Math");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Others", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Form));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewMultilayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aSMOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.standardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tvwEffects = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblNoSelect = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolToolStripMenuItem,
            this.screenshotToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(696, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolToolStripMenuItem
            // 
            this.toolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewMultilayerToolStripMenuItem,
            this.toolStripSeparator1,
            this.aSMOptionsToolStripMenuItem});
            this.toolToolStripMenuItem.Name = "toolToolStripMenuItem";
            this.toolToolStripMenuItem.Size = new System.Drawing.Size(50, 24);
            this.toolToolStripMenuItem.Text = "Tool";
            // 
            // addNewMultilayerToolStripMenuItem
            // 
            this.addNewMultilayerToolStripMenuItem.Name = "addNewMultilayerToolStripMenuItem";
            this.addNewMultilayerToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.addNewMultilayerToolStripMenuItem.Text = "Manage Multilayer";
            this.addNewMultilayerToolStripMenuItem.Click += new System.EventHandler(this.addNewMultilayerToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(213, 6);
            // 
            // aSMOptionsToolStripMenuItem
            // 
            this.aSMOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.standardToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.expertToolStripMenuItem});
            this.aSMOptionsToolStripMenuItem.Name = "aSMOptionsToolStripMenuItem";
            this.aSMOptionsToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.aSMOptionsToolStripMenuItem.Text = "Channel Options";
            // 
            // standardToolStripMenuItem
            // 
            this.standardToolStripMenuItem.Checked = true;
            this.standardToolStripMenuItem.CheckOnClick = true;
            this.standardToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.standardToolStripMenuItem.Name = "standardToolStripMenuItem";
            this.standardToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.standardToolStripMenuItem.Text = "Standard";
            this.standardToolStripMenuItem.Click += new System.EventHandler(this.asmModeToolStripMenuItem_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.CheckOnClick = true;
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.advancedToolStripMenuItem.Text = "All";
            this.advancedToolStripMenuItem.Click += new System.EventHandler(this.asmModeToolStripMenuItem_Click);
            // 
            // expertToolStripMenuItem
            // 
            this.expertToolStripMenuItem.CheckOnClick = true;
            this.expertToolStripMenuItem.Name = "expertToolStripMenuItem";
            this.expertToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.expertToolStripMenuItem.Text = "Expert";
            this.expertToolStripMenuItem.Visible = false;
            this.expertToolStripMenuItem.Click += new System.EventHandler(this.asmModeToolStripMenuItem_Click);
            // 
            // screenshotToolStripMenuItem
            // 
            this.screenshotToolStripMenuItem.Name = "screenshotToolStripMenuItem";
            this.screenshotToolStripMenuItem.Size = new System.Drawing.Size(93, 24);
            this.screenshotToolStripMenuItem.Text = "Screenshot";
            this.screenshotToolStripMenuItem.Click += new System.EventHandler(this.screenshotToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Visible = false;
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.lblNoSelect);
            this.splitContainer1.Size = new System.Drawing.Size(696, 381);
            this.splitContainer1.SplitterDistance = 193;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tvwEffects);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.groupBox1.Size = new System.Drawing.Size(193, 375);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Effects";
            // 
            // tvwEffects
            // 
            this.tvwEffects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwEffects.ImageIndex = 6;
            this.tvwEffects.ImageList = this.imageList1;
            this.tvwEffects.Location = new System.Drawing.Point(7, 21);
            this.tvwEffects.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tvwEffects.Name = "tvwEffects";
            treeNode1.Name = "BG Gradient";
            treeNode1.Text = "BG Gradient";
            treeNode2.Name = "FG Gradient";
            treeNode2.Text = "FG Gradient";
            treeNode3.Name = "Brightness";
            treeNode3.Text = "Brightness";
            treeNode4.ImageIndex = 0;
            treeNode4.Name = "Gradients";
            treeNode4.SelectedImageIndex = 0;
            treeNode4.Text = "Gradients";
            treeNode5.Name = "Parallax";
            treeNode5.Text = "Parallax";
            treeNode6.Name = "Waves";
            treeNode6.Text = "Waves";
            treeNode7.ImageIndex = 0;
            treeNode7.Name = "Scrolling";
            treeNode7.SelectedImageIndex = 0;
            treeNode7.Text = "Scrolling";
            treeNode8.Name = "Windows";
            treeNode8.Text = "Windows";
            treeNode9.Name = "Mosaic";
            treeNode9.Text = "Mosaic";
            treeNode10.Name = "Color Math";
            treeNode10.Text = "Color Math";
            treeNode11.ImageIndex = 0;
            treeNode11.Name = "Others";
            treeNode11.SelectedImageIndex = 0;
            treeNode11.Text = "Others";
            this.tvwEffects.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode7,
            treeNode11});
            this.tvwEffects.SelectedImageIndex = 6;
            this.tvwEffects.Size = new System.Drawing.Size(179, 348);
            this.tvwEffects.TabIndex = 1;
            this.tvwEffects.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tvwEffects_AfterExpandColaps);
            this.tvwEffects.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvwEffects_AfterExpandColaps);
            this.tvwEffects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvwEffects_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imageList1.Images.SetKeyName(0, "VSFolder_closed.bmp");
            this.imageList1.Images.SetKeyName(1, "VSFolder_closed_hidden.bmp");
            this.imageList1.Images.SetKeyName(2, "VSFolder_closed_virtual.bmp");
            this.imageList1.Images.SetKeyName(3, "VSFolder_open.bmp");
            this.imageList1.Images.SetKeyName(4, "VSFolder_open_hidden.bmp");
            this.imageList1.Images.SetKeyName(5, "VSFolder_open_virtual.bmp");
            this.imageList1.Images.SetKeyName(6, "Control_Box.bmp");
            // 
            // lblNoSelect
            // 
            this.lblNoSelect.AutoSize = true;
            this.lblNoSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoSelect.Location = new System.Drawing.Point(12, 78);
            this.lblNoSelect.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNoSelect.Name = "lblNoSelect";
            this.lblNoSelect.Size = new System.Drawing.Size(418, 50);
            this.lblNoSelect.TabIndex = 0;
            this.lblNoSelect.Text = "Please select the effect you want to create\r\non the list to the left";
            this.lblNoSelect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Main_Form
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 409);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "Main_Form";
            this.Text = "Effect Tool";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TreeView tvwEffects;
		private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label lblNoSelect;
        private System.Windows.Forms.ToolStripMenuItem toolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSMOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem standardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewMultilayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem screenshotToolStripMenuItem;

    }
}

