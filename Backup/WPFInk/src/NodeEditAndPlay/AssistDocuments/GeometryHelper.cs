using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFInk
{
    class GeometryHelper
    {
        public static double DistanceBetweenRectangles(Rectangle rect1, Rectangle rect2)
        {
            return DistanceBetweenPoints(
                CenterPointOfRectangle(rect1),
                CenterPointOfRectangle(rect2));
        }

        public static double DistanceBetweenPoints(Point pt1, Point pt2)
        {
            return Math.Sqrt((pt1.X - pt2.X) * (pt1.X - pt2.X) + (pt1.Y - pt2.Y) * (pt1.Y - pt2.Y));
        }

        public static Point CenterPointOfRectangle(Rectangle rect)
        {
            return new Point(
                rect.Left + rect.Width / 2,
                rect.Top + rect.Height / 2
                );
        }

        public static double DistanceBeteenRecPt(Rectangle rect, Point pt)
        {
            return DistanceBetweenPoints(CenterPointOfRectangle(rect), pt);
        }
    }
}
