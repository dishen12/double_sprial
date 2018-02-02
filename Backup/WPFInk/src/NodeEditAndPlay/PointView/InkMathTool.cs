using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ink;
using System.Drawing;

namespace WPFInk
{
    class InkMathTool
    {
        /// <summary>
        /// 将inkspace中的rectangle转换成像素的坐标
        /// </summary>
        /// <param name="inkpicture"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectangle InkSpaceToPixelRect(InkPicture inkpicture, Rectangle rect)
        {
            Graphics g = inkpicture.CreateGraphics();
            Point p1 = rect.Location;
            Point p2 = new Point(rect.Width, rect.Height);
            inkpicture.Renderer.InkSpaceToPixel(g, ref p1);
            inkpicture.Renderer.InkSpaceToPixel(g, ref p2);
            return new Rectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        public Rectangle _InkSpaceToPixelRect(System.Windows.Forms.PictureBox pictureBox, InkCollector inkpicture, Rectangle rect)
        {
            //Graphics g = inkpicture.CreateGraphics();
            Graphics g = pictureBox.CreateGraphics();
            Point p1 = rect.Location;
            Point p2 = new Point(rect.Width, rect.Height);
            inkpicture.Renderer.InkSpaceToPixel(g, ref p1);
            inkpicture.Renderer.InkSpaceToPixel(g, ref p2);
            g.Dispose();
            return new Rectangle(p1.X, p1.Y, p2.X, p2.Y);            
        }


        /// <summary>
        /// 将inkspace中的Point转换成像素坐标
        /// </summary>
        /// <param name="inkpicture"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Point InkSpaceToPixelPoint(InkPicture inkpicture, Point pt)
        {
            Graphics g = inkpicture.CreateGraphics();
            Point p = new Point(pt.X, pt.Y);
            inkpicture.Renderer.InkSpaceToPixel(g, ref p);
            return p;
        }
    }
}
