using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WPFInk
{
    class CJVideoStaticStroke : CJStroke
    {
        public int startFrame = 0;
        public int endFrame = int.MaxValue;

        public void Draw(Graphics g, Pen pen)
        {
            GraphicsPath path = new GraphicsPath();
            for (int i = 1; i < this.Count; i++)
            {
                Point startPt = new Point((int)this[i - 1].X, (int)this[i - 1].Y);
                Point endPt = new Point((int)this[i].X, (int)this[i].Y);
                path.AddLine(startPt, endPt);
            }
            g.DrawPath(pen, path);
        }
    }
}
