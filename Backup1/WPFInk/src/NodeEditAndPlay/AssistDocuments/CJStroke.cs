using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WPFInk
{
    public class CJStroke : List<CJPoint>
    {
        public Color color = Color.Red;

        public float penWidth = 8;

        public void Draw(Graphics g)
        {
            if (this.Count < 2)
                return;
            for (int i = 1; i < this.Count; i++)
            {
                g.DrawLine(new Pen(color), (float)this[i - 1].X, (float)this[i - 1].Y, (float)this[i].X, (float)this[i].Y);
            }
        }

        public void DrawWithOffset(Graphics g, Offset offset)
        {
            if (this.Count < 2)
                return;
            for (int i = 1; i < this.Count; i++)
            {
                g.DrawLine(new Pen(color, 2.0F), (float)(this[i - 1].X - offset.X), (float)(this[i - 1].Y - offset.Y), (float)(this[i].X - offset.X), (float)(this[i].Y - offset.Y));
            }
        }

        public void Draw(Graphics g, int width, int height)
        {
            if (this.Count < 2)
                return;
            GraphicsPath path = new GraphicsPath();
            for (int i = 1; i < this.Count; i++)
            {
                path.AddLine((float)this[i - 1].X * width, (float)this[i - 1].Y * height, (float)this[i].X * width, (float)this[i].Y * height);

            }
            g.DrawPath(new Pen(color, penWidth), path);
        }


        public void Draw(Graphics g, int width, int height, float penWidth)
        {
            if (this.Count < 2)
                return;
            GraphicsPath path = new GraphicsPath();
            for (int i = 1; i < this.Count; i++)
            {
                path.AddLine((float)this[i - 1].X * width, (float)this[i - 1].Y * height, (float)this[i].X * width, (float)this[i].Y * height);

            }
            g.DrawPath(new Pen(color, penWidth), path);
        }

        public void DrawWithOffset(Graphics g, Offset offset, int width, int height)
        {
            if (this.Count < 2)
                return;
            GraphicsPath path = new GraphicsPath();
            for (int i = 1; i < this.Count; i++)
            {
                path.AddLine((float)(this[i - 1].X * width - offset.X), (float)(this[i - 1].Y * height - offset.Y),
                    (float)(this[i].X * width - offset.X), (float)(this[i].Y * height - offset.Y));
            }
            g.DrawPath(new Pen(color, penWidth), path);
        }


        public void AddPoint(double x, double y)
        {
            this.Add(new CJPoint(x, y));
        }
    }
}
