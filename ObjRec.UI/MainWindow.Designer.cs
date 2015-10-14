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
            ((System.ComponentModel.ISupportInitialize)(this.sourcePic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedPic)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // sourcePic
            // 
            this.sourcePic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sourcePic.Location = new System.Drawing.Point(12, 39);
            this.sourcePic.Name = "sourcePic";
            this.sourcePic.Size = new System.Drawing.Size(640, 480);
            this.sourcePic.TabIndex = 0;
            this.sourcePic.TabStop = false;
            // 
            // processedPic
            // 
            this.processedPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.processedPic.Location = new System.Drawing.Point(658, 39);
            this.processedPic.Name = "processedPic";
            this.processedPic.Size = new System.Drawing.Size(640, 480);
            this.processedPic.TabIndex = 0;
            this.processedPic.TabStop = false;
            // 
            // filenameTextbox
            // 
            this.filenameTextbox.Location = new System.Drawing.Point(12, 12);
            this.filenameTextbox.Name = "filenameTextbox";
            this.filenameTextbox.Size = new System.Drawing.Size(640, 20);
            this.filenameTextbox.TabIndex = 1;
            this.filenameTextbox.Click += new System.EventHandler(this.filenameTextbox_Click);
            // 
            // loadFileButton
            // 
            this.loadFileButton.Location = new System.Drawing.Point(656, 10);
            this.loadFileButton.Name = "loadFileButton";
            this.loadFileButton.Size = new System.Drawing.Size(75, 23);
            this.loadFileButton.TabIndex = 2;
            this.loadFileButton.Text = "Load file";
            this.loadFileButton.UseVisualStyleBackColor = true;
            this.loadFileButton.Click += new System.EventHandler(this.loadFileButton_Click);
            // 
            // OtsuFilter
            // 
            this.OtsuFilter.Location = new System.Drawing.Point(737, 10);
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
            this.statusStrip.Location = new System.Drawing.Point(0, 548);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1310, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusBarText
            // 
            this.statusBarText.Name = "statusBarText";
            this.statusBarText.Size = new System.Drawing.Size(39, 17);
            this.statusBarText.Text = "Ready";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1310, 570);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.OtsuFilter);
            this.Controls.Add(this.loadFileButton);
            this.Controls.Add(this.filenameTextbox);
            this.Controls.Add(this.processedPic);
            this.Controls.Add(this.sourcePic);
            this.Name = "MainWindow";
            this.Text = "Object Recognition";
            ((System.ComponentModel.ISupportInitialize)(this.sourcePic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processedPic)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
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
    }
}

