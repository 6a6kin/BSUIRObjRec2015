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
            this.OtsuFilter = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusBarText = new System.Windows.Forms.ToolStripStatusLabel();
            this.medianButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.sobelButton = new System.Windows.Forms.Button();
            this.sharpButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sourcePic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedPic)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.sourcePic.Size = new System.Drawing.Size(652, 571);
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
            this.processedPic.Location = new System.Drawing.Point(661, 43);
            this.processedPic.Name = "processedPic";
            this.processedPic.Size = new System.Drawing.Size(652, 571);
            this.processedPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.processedPic.TabIndex = 0;
            this.processedPic.TabStop = false;
            // 
            // filenameTextbox
            // 
            this.filenameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.filenameTextbox.Location = new System.Drawing.Point(3, 10);
            this.filenameTextbox.Name = "filenameTextbox";
            this.filenameTextbox.Size = new System.Drawing.Size(652, 20);
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
            // OtsuFilter
            // 
            this.OtsuFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.OtsuFilter.Location = new System.Drawing.Point(84, 3);
            this.OtsuFilter.Name = "OtsuFilter";
            this.OtsuFilter.Size = new System.Drawing.Size(75, 23);
            this.OtsuFilter.TabIndex = 3;
            this.OtsuFilter.Text = "Otsu";
            this.OtsuFilter.UseVisualStyleBackColor = true;
            this.OtsuFilter.Click += new System.EventHandler(this.OtsuFilter_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarText});
            this.statusStrip.Location = new System.Drawing.Point(0, 632);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1340, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusBarText
            // 
            this.statusBarText.Name = "statusBarText";
            this.statusBarText.Size = new System.Drawing.Size(39, 17);
            this.statusBarText.Text = "Ready";
            // 
            // medianButton
            // 
            this.medianButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.medianButton.Location = new System.Drawing.Point(165, 3);
            this.medianButton.Name = "medianButton";
            this.medianButton.Size = new System.Drawing.Size(79, 23);
            this.medianButton.TabIndex = 5;
            this.medianButton.Text = "Median";
            this.medianButton.UseVisualStyleBackColor = true;
            this.medianButton.Click += new System.EventHandler(this.medianButton_Click);
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1316, 617);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.loadFileButton);
            this.flowLayoutPanel1.Controls.Add(this.OtsuFilter);
            this.flowLayoutPanel1.Controls.Add(this.medianButton);
            this.flowLayoutPanel1.Controls.Add(this.sobelButton);
            this.flowLayoutPanel1.Controls.Add(this.sharpButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(661, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(652, 34);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // sobelButton
            // 
            this.sobelButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.sobelButton.Location = new System.Drawing.Point(250, 3);
            this.sobelButton.Name = "sobelButton";
            this.sobelButton.Size = new System.Drawing.Size(75, 23);
            this.sobelButton.TabIndex = 6;
            this.sobelButton.Text = "Sobel";
            this.sobelButton.UseVisualStyleBackColor = true;
            this.sobelButton.Click += new System.EventHandler(this.sobelButton_Click);
            // 
            // sharpButton
            // 
            this.sharpButton.Location = new System.Drawing.Point(331, 3);
            this.sharpButton.Name = "sharpButton";
            this.sharpButton.Size = new System.Drawing.Size(75, 23);
            this.sharpButton.TabIndex = 7;
            this.sharpButton.Text = "Sharpness";
            this.sharpButton.UseVisualStyleBackColor = true;
            this.sharpButton.Click += new System.EventHandler(this.sharpButton_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1340, 654);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox sourcePic;
        private System.Windows.Forms.PictureBox processedPic;
        private System.Windows.Forms.TextBox filenameTextbox;
        private System.Windows.Forms.Button loadFileButton;
        private System.Windows.Forms.Button OtsuFilter;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusBarText;
        private System.Windows.Forms.Button medianButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button sobelButton;
        private System.Windows.Forms.Button sharpButton;
    }
}

