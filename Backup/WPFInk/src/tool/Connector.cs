using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Ink;
using System.Drawing;

namespace WPFInk.tool
{
    class Connector
    {
        //private MyImage image1;
        //private MyImage image2;
        //private MyStroke stroke;
        private static Connector connector;
        //定义弧的跨度
        int DIS_SEP = 80;

        //定义弧的高度
        int H = 50;

        

        public Connector()
        {
            //this.image1 = image1;
            //this.image2 = image2;

        }

        public static Connector getInstance()
        {
            if (connector == null)
                connector = new Connector();
            return connector;
        }

        public StylusPointCollection getImageConnector(MyImage image1, MyImage image2)
        {
            StylusPoint center1 = MathTool.getInstance().getImageCenter(image1);
            StylusPoint center2 = MathTool.getInstance().getImageCenter(image2);

            //Console.WriteLine(center1.X.ToString());
            if (center1.X > center2.X)
                return getImageConnector(image2, image1);
            Rect bounds1 = MathTool.getInstance().getImageBounds(image1);
            Rect bounds2 = MathTool.getInstance().getImageBounds(image2);
            double angel = MathTool.getInstance().getAngleP2P(center1, center2);
            //Console.WriteLine("angel:"+angel);
            StylusPointCollection collection = null;
            if (angel > 45 & angel <= 90)
            {
                collection = getCurve(new StylusPoint(center1.X, bounds1.Top),
                                        new StylusPoint(center2.X, bounds2.Bottom));
            }
            else if (angel >= 0 & angel <= 45)
            {
                collection = getCurve(new StylusPoint(bounds1.Right, center1.Y),
                                        new StylusPoint(bounds2.Left, center2.Y));
            }
            else if (angel >= 270 & angel <= 315)
            {
                collection = getCurve(new StylusPoint(center1.X, bounds1.Bottom),
                                        new StylusPoint(center2.X, bounds2.Top));
            }
            else if (angel > 315 & angel <= 360)
            {
                collection = getCurve(new StylusPoint(bounds1.Right, center1.Y),
                                        new StylusPoint(bounds2.Left, center2.Y));
            }
            return collection;
        }

        public StylusPointCollection getCurve(StylusPoint start, StylusPoint end)
        {

            StylusPointCollection collection = new StylusPointCollection();

            if (start.X > end.X)
            {
                StylusPoint temp = start;
                start = end;
                end = temp;
            }

            StylusPoint[] pts = getMidPoints(start, end);
            for (int i = 1; i < pts.Length; i++)
            {
                StylusPoint p1 = pts[i - 1];
                StylusPoint p2 = pts[i];
                int num = (int)(MathTool.getInstance().distanceP2P(p1, p2));

                StylusPoint b = new StylusPoint();
                StylusPoint temp = new StylusPoint(p1.X + (p2.X - p1.X) / 3, p1.Y + (p2.Y - p1.Y) / 3);

                double angle = MathTool.getInstance().getAngleP2P(p1, p2);
                double angle1 = 90 + angle;
                double kk = Math.Tan(angle1 / 180);
                b.X = (int)(temp.X - H / kk);
                b.Y = temp.Y + H;

                for (int k = 0; k < num; k++)
                {
                    double t = k * 1.0 / num;
                    double a, e, c;
                    a = (1 - t) * (1 - t);
                    e = 2 * t * (1 - t);
                    c = t * t;
                    int x1 = (int)(p1.X * a + b.X * e + p2.X * c);
                    int y1 = (int)(p1.Y * a + b.Y * e + p2.Y * c);
                    collection.Add(new StylusPoint(x1, y1));
                }

            }

            return collection;
        }

        /// <summary>
        /// 返回起始点和结束点中间的所有中间点的坐标
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private StylusPoint[] getMidPoints(StylusPoint start, StylusPoint end)
        {
            StylusPoint[] results = { start, end };
            double distance = MathTool.getInstance().distanceP2P(start, end);

            int numofmidpoints = (int)(distance / DIS_SEP) - 1;

            if (numofmidpoints <= 0)
                return results;
            StylusPoint[] midpoints = new StylusPoint[numofmidpoints];
            int offx = (int)(end.X - start.X);
            int offy = (int)(end.Y - start.Y);
            for (int i = 0; i < numofmidpoints; i++)
            {
                midpoints[i] = new StylusPoint();
                midpoints[i].X = start.X + offx * (i + 1) / (numofmidpoints + 1);
                midpoints[i].Y = start.Y + offy * (i + 1) / (numofmidpoints + 1);
            }
            results = new StylusPoint[2 + numofmidpoints];
            results[0] = start;
            for (int i = 1; i < numofmidpoints + 1; i++)
                results[i] = midpoints[i - 1];
            results[1 + numofmidpoints] = end;
            return results;
        }
    }
}
