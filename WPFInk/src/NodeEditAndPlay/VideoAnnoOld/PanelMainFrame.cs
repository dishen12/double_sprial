using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace WPFInk
{
    public partial class PanelMainFrame : UserControl
    {
        #region variables
        /// <summary>
        /// 该帧上的annotation
        /// </summary>
        List<Annotation> listannos = new List<Annotation>();

        internal List<Annotation> ListAnnos
        {
            get { return listannos; }
            set { listannos = value; }
        }

        Annotation curAnno = null;

        internal Annotation CurAnno
        {
            get { return curAnno; }
            set { curAnno = value; }
        }

        FormAnnotationOld formAnnotationOld = null;

        /// <summary>
        /// 左键是否按下
        /// </summary>
        bool bBtnDown = false;

        /// <summary>
        /// 当前正在绘制的Stroke
        /// </summary>
        CJStroke curStroke = null;

        int curFrameNum = 0;
        /// <summary>
        /// 当前帧号
        /// </summary>
        public int CurFrameNum
        {
            get { return curFrameNum; }
            set { curFrameNum = value; }
        }

        #endregion
        public PanelMainFrame(FormAnnotationOld formAO)
        {
            formAnnotationOld = formAO;
            InitializeComponent();
        }

        private void PanelMainFrame_MouseDown(object sender, MouseEventArgs e)
        {
            bBtnDown = true;

            if (null == curAnno)
            {
                curAnno = new Annotation();
                curAnno.StartFrame = CurFrameNum;
                curAnno.EndFrame = int.MaxValue;
                ListAnnos.Add(curAnno);
            }
            curStroke = new CJStroke();
            // todo 
            curStroke.color = formAnnotationOld.GetPenColor();
            curStroke.penWidth = (float)formAnnotationOld.GetPenWidth();
            curStroke.Add(new CJPoint((double)e.X / this.Width, (double)e.Y / this.Height));
            curAnno.Add(curStroke);
        }

        private void PanelMainFrame_MouseMove(object sender, MouseEventArgs e)
        {
            if (true == bBtnDown)
            {
                if (null != curStroke)
                {
                    curStroke.Add(new CJPoint((double)e.X / this.Width, (double)e.Y / this.Height));
                    formAnnotationOld.DrawFrames();
                    this.Refresh();
                }
            }
        }

        private void PanelMainFrame_MouseUp(object sender, MouseEventArgs e)
        {
            bBtnDown = false;
            curStroke = null;
        }

        private void PanelMainFrame_Paint(object sender, PaintEventArgs e)
        {
            foreach (Annotation anno in this.ListAnnos)
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

        private void PanelMainFrame_Load(object sender, EventArgs e)
        {

        }

        public void SaveSketchToImg(string filepath)
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));
            foreach (Annotation anno in this.ListAnnos)
            {
                if (anno.StartFrame <= curFrameNum && anno.EndFrame >= curFrameNum)
                {
                    foreach (CJStroke stroke in anno)
                    {
                        stroke.Draw(g, this.Width, this.Height, 6);
                    }
                }
            }
            //DeleteBlank(bmp).Save(filepath);
            bmp.Save(filepath);
            g.Dispose();
        }


        public static Bitmap DeleteBlank(Bitmap bmp)
        {
            int left = 0;
            int right = bmp.Width;
            int top = 0;
            int bottom = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            int bytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[bytes];

            IntPtr ptr = data.Scan0;
            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // detect left boundary
            for (int i = 0; i < bmp.Width; i++)
            {
                bool bBlank = true;
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = 3 * (data.Width * j + i);
                    //if (rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255)
                    //    ;
                    //else
                    //{
                    //    bBlank = false;
                    //    break;
                    //}
                    //2011.4.1修改为下面的语句
                    if (!(rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255))
                    {
                        bBlank = false;
                        break;
                    }
                }
                if (true == bBlank)
                    left = i;
                else
                    break;
            }

            // detect right boundary
            for (int i = bmp.Width - 1; i >= 0; i--)
            {
                bool bBlank = true;
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = 3 * (data.Width * j + i);
                    //if (rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255)
                    //    ;
                    //else
                    //{
                    //    bBlank = false;
                    //    break;
                    //}
                    if (!(rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255))
                    {
                        bBlank = false;
                        break;
                    }
                }

                if (true == bBlank)
                    right = i;
                else
                    break;
            }


            // detect top boundary
            for (int j = 0; j < bmp.Height; j++)
            {
                bool bBlank = true;
                for (int i = 0; i < bmp.Width; i++)
                {
                    int index = 3 * (data.Width * j + i);
                    //if (rgbValues[index] > 255 && rgbValues[index + 1] > 255 && rgbValues[index + 2] > 255)
                    //    ;
                    //else
                    //{
                    //    bBlank = false;
                    //    break;
                    //}
                    if (!(rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255))
                    {
                        bBlank = false;
                        break;
                    }
                }

                if (true == bBlank)
                    top = j;
                else
                    break;
            }

            // detect bottom boundary
            for (int j = bmp.Height - 1; j >= 0; j--)
            {
                bool bBlank = true;
                for (int i = 0; i < bmp.Width; i++)
                {
                    int index = 3 * (data.Width * j + i);
                    if (!(rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255))
                    {
                        bBlank = false;
                        break;
                    }
                }

                if (true == bBlank)
                    bottom = j;
                else
                    break;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(data);

            Bitmap newBmp = new Bitmap(right - left, bottom - top);
            Graphics g = Graphics.FromImage(newBmp);
            g.DrawImage(bmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height), new Rectangle(left, top, right - left, bottom - top), GraphicsUnit.Pixel);
            g.Dispose();
            return newBmp;
        }
    }
}
