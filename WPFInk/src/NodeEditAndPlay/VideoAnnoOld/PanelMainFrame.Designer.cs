namespace WPFInk
{
    partial class PanelMainFrame
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
            this.SuspendLayout();
            // 
            // PanelMainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "PanelMainFrame";
            this.Load += new System.EventHandler(this.PanelMainFrame_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMainFrame_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PanelMainFrame_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelMainFrame_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PanelMainFrame_MouseUp);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
