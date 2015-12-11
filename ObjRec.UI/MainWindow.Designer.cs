namespace ObjRec.UI
{
    partial class MainWindow
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
            this.sourcePic = new System.Windows.Forms.PictureBox();
            this.processedPic = new System.Windows.Forms.PictureBox();
            this.filenameTextbox = new System.Windows.Forms.TextBox();
            this.loadFileButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.siftButton = new System.Windows.Forms.Button();
            this.siftDescSize = new System.Windows.Forms.NumericUpDown();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            listView1 = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.sourcePic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedPic)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.siftDescSize)).BeginInit();
            this.SuspendLayout();
            // 
            // sourcePic
            // 
            this.sourcePic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourcePic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sourcePic.Location = new System.Drawing.Point(3, 43);
            this.sourcePic.Name = "sourcePic";
            this.sourcePic.Size = new System.Drawing.Size(659, 310);
            this.sourcePic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.sourcePic.TabIndex = 0;
            this.sourcePic.TabStop = false;
            // 
            // processedPic
            // 
            this.processedPic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processedPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.processedPic.Location = new System.Drawing.Point(668, 43);
            this.processedPic.Name = "processedPic";
            this.processedPic.Size = new System.Drawing.Size(659, 310);
            this.processedPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.processedPic.TabIndex = 0;
            this.processedPic.TabStop = false;
            // 
            // filenameTextbox
            // 
            this.filenameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.filenameTextbox.Location = new System.Drawing.Point(3, 10);
            this.filenameTextbox.Name = "filenameTextbox";
            this.filenameTextbox.Size = new System.Drawing.Size(659, 20);
            this.filenameTextbox.TabIndex = 1;
            this.filenameTextbox.Click += new System.EventHandler(this.filenameTextbox_Click);
            // 
            // loadFileButton
            // 
            this.loadFileButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.loadFileButton.Location = new System.Drawing.Point(3, 3);
            this.loadFileButton.Name = "loadFileButton";
            this.loadFileButton.Size = new System.Drawing.Size(75, 23);
            this.loadFileButton.TabIndex = 2;
            this.loadFileButton.Text = "Reload file";
            this.loadFileButton.UseVisualStyleBackColor = true;
            this.loadFileButton.Click += new System.EventHandler(this.loadFileButton_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarText});
            this.statusStrip.Location = new System.Drawing.Point(0, 688);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1354, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusBarText
            // 
            this.statusBarText.Name = "statusBarText";
            this.statusBarText.Size = new System.Drawing.Size(39, 17);
            this.statusBarText.Text = "Ready";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.processedPic, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.filenameTextbox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.sourcePic, 0, 1);
            this.tableLayoutPanel1.Controls.Add(listView1, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1330, 673);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.loadFileButton);
            this.flowLayoutPanel1.Controls.Add(this.siftButton);
            this.flowLayoutPanel1.Controls.Add(this.siftDescSize);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(668, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(659, 34);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // siftButton
            // 
            this.siftButton.Location = new System.Drawing.Point(84, 3);
            this.siftButton.Name = "siftButton";
            this.siftButton.Size = new System.Drawing.Size(75, 23);
            this.siftButton.TabIndex = 7;
            this.siftButton.Text = "SIFT";
            this.siftButton.UseVisualStyleBackColor = true;
            this.siftButton.Click += new System.EventHandler(this.siftButton_Click);
            // 
            // siftDescSize
            // 
            this.siftDescSize.Location = new System.Drawing.Point(165, 3);
            this.siftDescSize.Maximum = new decimal(new int[] {
            1004,
            0,
            0,
            0});
            this.siftDescSize.Name = "siftDescSize";
            this.siftDescSize.Size = new System.Drawing.Size(49, 20);
            this.siftDescSize.TabIndex = 8;
            this.siftDescSize.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.siftDescSize.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.tableLayoutPanel1.SetColumnSpan(listView1, 2);
            listView1.Location = new System.Drawing.Point(3, 359);
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(1324, 311);
            listView1.TabIndex = 3;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.SmallIcon;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 200;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1354, 710);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip);
            this.Name = "MainWindow";
            this.Text = "Object Recognition";
            ((System.ComponentModel.ISupportInitialize)(this.sourcePic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedPic)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.siftDescSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.PictureBox sourcePic;
        private System.Windows.Forms.PictureBox processedPic;
        private System.Windows.Forms.TextBox filenameTextbox;
        private System.Windows.Forms.Button loadFileButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusBarText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button siftButton;
        private System.Windows.Forms.NumericUpDown siftDescSize;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}

