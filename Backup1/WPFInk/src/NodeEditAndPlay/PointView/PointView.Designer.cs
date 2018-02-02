namespace WPFInk
{
    partial class PointView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.inkPictureNode = new System.Windows.Forms.PictureBox();//Microsoft.Ink.InkPicture();
            ((System.ComponentModel.ISupportInitialize)(this.inkPictureNode)).BeginInit();
            this.SuspendLayout();
            // 
            // inkPictureNode
            // 
            //7.29this.inkPictureNode.CollectionMode = Microsoft.Ink.CollectionMode.GestureOnly;
            this.inkPictureNode.Location = new System.Drawing.Point(3, 0);
            //7.29this.inkPictureNode.MarginX = -2147483648;
            //7.29this.inkPictureNode.MarginY = -2147483648;
            this.inkPictureNode.Name = "inkPictureNode";
            this.inkPictureNode.Size = new System.Drawing.Size(563, 386);
            this.inkPictureNode.TabIndex = 0;
            this.inkPictureNode.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.inkPictureNode_MouseDoubleClick);
            this.inkPictureNode.Paint += new System.Windows.Forms.PaintEventHandler(this.inkPictureNode_Paint);
            this.inkPictureNode.MouseClick += new System.Windows.Forms.MouseEventHandler(this.inkPictureNode_MouseClick);
            this.inkPictureNode.MouseUp += new System.Windows.Forms.MouseEventHandler(this.inkPictureNode_MouseUp);
            //7.29this.inkPictureNode.Gesture += new Microsoft.Ink.InkCollectorGestureEventHandler(this.inkPictureNode_Gesture);
            this.inkPictureNode.MouseMove += new System.Windows.Forms.MouseEventHandler(this.inkPictureNode_MouseMove);
            this.inkPictureNode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.inkPictureNode_MouseDown);
            // 
            // PointView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.inkPictureNode);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "PointView";
            this.Size = new System.Drawing.Size(566, 386);
            this.Load += new System.EventHandler(this.PointView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inkPictureNode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        //public Microsoft.Ink.InkPicture inkPictureNode;
        public System.Windows.Forms.PictureBox inkPictureNode;

    }
}
