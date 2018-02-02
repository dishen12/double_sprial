using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


namespace WPFInk
{
    public partial class PanelNormalFrame : UserControl
    {
        int curFrameNum = 0;
        /// <summary>
        /// 当前帧号
        /// </summary>
        public int CurFrameNum
        {
            get { return curFrameNum; }
            set { curFrameNum = value; }
        }

        List<Annotation> annos = null;

        internal List<Annotation> Annos
        {
            get { return annos; }
            set { annos = value; }
        }
        public PanelNormalFrame()
        {
            InitializeComponent();
        }

        private void PanelNormalFrame_Paint(object sender, PaintEventArgs e)
        {
            if (null == this.Annos)
                return;
            foreach (Annotation anno in this.Annos)
            {
                if (anno.StartFrame <= curFrameNum && anno.EndFrame >= curFrameNum)
                {
                    foreach (CJStroke stroke in anno)
                    {
                        stroke.Draw(e.Graphics, this.Width, this.Height);
                    }
                }
            }
        }

        private void PanelNormalFrame_Load(object sender, EventArgs e)
        {

        }
    }
}
