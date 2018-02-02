using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFInk 
{
    public class Annotation : List<CJStroke>
    {
        int startFrame = 0;

        public int StartFrame
        {
            get { return startFrame; }
            set { startFrame = value; }
        }
        int endFrame = 0;

        public int EndFrame
        {
            get { return endFrame; }
            set { endFrame = value; }
        }

        // 导出为bmp图片
        public Bitmap GetBmp()
        {
            if (this.Count == 0)
                return null;

            // 获取该annotation 的boundingbox
            double left = double.MaxValue;
            double right = double.MinValue;
            double top = double.MaxValue;
            double bottom = double.MinValue;
            foreach (CJStroke stroke in this)
            {

                foreach (CJPoint pt in stroke)
                {
                    left = pt.X < left ? pt.X : left;
                    right = pt.X > right ? pt.X : right;
                    top = pt.Y < top ? pt.Y : top;
                    bottom = pt.Y > bottom ? pt.Y : bottom;
                }
            }

            double width = right - left;
            double height = bottom - top;
            Bitmap bmp = new Bitmap((int)(width * 100 / height), 100);

            Graphics g = Graphics.FromImage(bmp);
            foreach (CJStroke stroke in this)
            {
                stroke.DrawWithOffset(g, new Offset(-left * bmp.Width, -top * bmp.Height), bmp.Width, bmp.Height);
            }
            g.Dispose();
            return bmp;

        }
    }
}
