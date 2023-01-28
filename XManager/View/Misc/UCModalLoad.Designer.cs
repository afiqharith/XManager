namespace XManager
{
    partial class UCModalLoad
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
            this.PanelBase = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LabelPercentage = new System.Windows.Forms.Label();
            this.ProgressBarPercentage = new System.Windows.Forms.ProgressBar();
            this.RichTextBoxStatus = new System.Windows.Forms.RichTextBox();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.PanelBase.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelBase
            // 
            this.PanelBase.AutoSize = true;
            this.PanelBase.Controls.Add(this.StatusStrip);
            this.PanelBase.Controls.Add(this.panel1);
            this.PanelBase.Controls.Add(this.RichTextBoxStatus);
            this.PanelBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelBase.Location = new System.Drawing.Point(0, 0);
            this.PanelBase.Name = "PanelBase";
            this.PanelBase.Size = new System.Drawing.Size(519, 278);
            this.PanelBase.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.LabelPercentage);
            this.panel1.Controls.Add(this.ProgressBarPercentage);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 29);
            this.panel1.TabIndex = 2;
            // 
            // LabelPercentage
            // 
            this.LabelPercentage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LabelPercentage.AutoSize = true;
            this.LabelPercentage.BackColor = System.Drawing.Color.Transparent;
            this.LabelPercentage.Location = new System.Drawing.Point(211, 8);
            this.LabelPercentage.Name = "LabelPercentage";
            this.LabelPercentage.Size = new System.Drawing.Size(72, 13);
            this.LabelPercentage.TabIndex = 2;
            this.LabelPercentage.Text = "percentage %";
            this.LabelPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressBarPercentage
            // 
            this.ProgressBarPercentage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressBarPercentage.Location = new System.Drawing.Point(0, 0);
            this.ProgressBarPercentage.Name = "ProgressBarPercentage";
            this.ProgressBarPercentage.Size = new System.Drawing.Size(494, 29);
            this.ProgressBarPercentage.TabIndex = 1;
            // 
            // RichTextBoxStatus
            // 
            this.RichTextBoxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RichTextBoxStatus.Location = new System.Drawing.Point(7, 50);
            this.RichTextBoxStatus.Name = "RichTextBoxStatus";
            this.RichTextBoxStatus.Size = new System.Drawing.Size(504, 203);
            this.RichTextBoxStatus.TabIndex = 3;
            this.RichTextBoxStatus.Text = "";
            // 
            // StatusStrip
            // 
            this.StatusStrip.Location = new System.Drawing.Point(0, 256);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(519, 22);
            this.StatusStrip.TabIndex = 4;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // UCComponentLoadModal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PanelBase);
            this.Name = "UCComponentLoadModal";
            this.Size = new System.Drawing.Size(519, 278);
            this.PanelBase.ResumeLayout(false);
            this.PanelBase.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel PanelBase;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LabelPercentage;
        private System.Windows.Forms.ProgressBar ProgressBarPercentage;
        private System.Windows.Forms.RichTextBox RichTextBoxStatus;
        private System.Windows.Forms.StatusStrip StatusStrip;
    }
}
