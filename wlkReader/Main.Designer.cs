namespace wlkReader
{
    partial class Main
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWLKFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllRecordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toCSVFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectedRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lvRecords = new System.Windows.Forms.ListView();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1255, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openWLKFileToolStripMenuItem,
            this.exportAllRecordsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openWLKFileToolStripMenuItem
            // 
            this.openWLKFileToolStripMenuItem.Name = "openWLKFileToolStripMenuItem";
            this.openWLKFileToolStripMenuItem.Size = new System.Drawing.Size(206, 26);
            this.openWLKFileToolStripMenuItem.Text = "Open WLK file...";
            this.openWLKFileToolStripMenuItem.Click += new System.EventHandler(this.openWLKFileToolStripMenuItem_Click);
            // 
            // exportAllRecordsToolStripMenuItem
            // 
            this.exportAllRecordsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toCSVFormatToolStripMenuItem});
            this.exportAllRecordsToolStripMenuItem.Name = "exportAllRecordsToolStripMenuItem";
            this.exportAllRecordsToolStripMenuItem.Size = new System.Drawing.Size(206, 26);
            this.exportAllRecordsToolStripMenuItem.Text = "Export All Records";
            // 
            // toCSVFormatToolStripMenuItem
            // 
            this.toCSVFormatToolStripMenuItem.Name = "toCSVFormatToolStripMenuItem";
            this.toCSVFormatToolStripMenuItem.Size = new System.Drawing.Size(188, 26);
            this.toCSVFormatToolStripMenuItem.Text = "To CSV format...";
            this.toCSVFormatToolStripMenuItem.Click += new System.EventHandler(this.toCSVFormatToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedRecordToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // selectedRecordToolStripMenuItem
            // 
            this.selectedRecordToolStripMenuItem.Name = "selectedRecordToolStripMenuItem";
            this.selectedRecordToolStripMenuItem.Size = new System.Drawing.Size(205, 26);
            this.selectedRecordToolStripMenuItem.Text = "Selected Record ...";
            this.selectedRecordToolStripMenuItem.Click += new System.EventHandler(this.selectedRecordToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 529);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1255, 25);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(151, 20);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // lvRecords
            // 
            this.lvRecords.HideSelection = false;
            this.lvRecords.Location = new System.Drawing.Point(17, 50);
            this.lvRecords.Margin = new System.Windows.Forms.Padding(4);
            this.lvRecords.Name = "lvRecords";
            this.lvRecords.Size = new System.Drawing.Size(1217, 421);
            this.lvRecords.TabIndex = 2;
            this.lvRecords.UseCompatibleStateImageBehavior = false;
            this.lvRecords.View = System.Windows.Forms.View.Details;
            this.lvRecords.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvRecords_ColumnClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1255, 554);
            this.Controls.Add(this.lvRecords);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Main";
            this.Text = "wlkReader v1.3";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWLKFileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ListView lvRecords;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectedRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllRecordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toCSVFormatToolStripMenuItem;
    }
}

