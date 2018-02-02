using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Ink;
using System.Text;

namespace WPFInk.WZL
{
    /// <summary>
    /// TEST的值代表是聚类还是独立的一个曲线或者直线
    /// </summary>
    public enum TESTRESULT
    {
        GROUP,
        SEPERATE
    }

    /// <summary>
    /// class Tool 定义了一些完成数学计算的函数
    /// </summary>
    public class ToolForStroke
    {
        public ToolForStroke()
        {

        }

        /// <summary>
        /// 将多边形按照其中心旋转角度angel
        /// angel为度数
        /// </summary>
        /// <param name="points">多边形的顶点</param>
        /// <param name="angel">需要旋转的角度</param>
        /// <returns>旋转后的多边形顶点</returns>
        public Point[] rotatePolygon(Point[] points, float angel)
        {
            Point center = getCenter(points);

            Point[] result = new Point[points.Length];
            angel = (float)(angel * Math.PI / 180);
            double cos = Math.Cos(angel);
            double sin = Math.Sin(angel);

            for (int i = 0; i < points.Length; i++)
            {
                Point offset = new Point(points[i].X - center.X, points[i].Y - center.Y);
                double xoffset = offset.X * cos - offset.Y * sin;
                double yoffset = offset.X * sin + offset.Y * cos;
                result[i] = new Point(center.X + (int)xoffset, center.Y + (int)yoffset);
            }

            return result;
        }

        /// <summary>
        /// 将多边形平移
        /// </summary>
        /// <param name="points"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Point[] movePolygon(Point[] points, Point offset)
        {
            Point[] result = new Point[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                result[i] = new Point(points[i].X + offset.X, points[i].Y + offset.Y);
            }
            return result;
        }

        /// <summary>
        /// 计算有points组成的多边形的中心
        /// </summary>
        /// <param name="points">多边形的顶点</param>
        /// <returns>多边形的中心坐标</returns>
        public Point getCenter(Point[] points)
        {
            int sumx = 0, sumy = 0;
            foreach (Point p in points)
            {
                sumx += p.X;
                sumy += p.Y;
            }
            Point center = new Point(sumx / points.Length, sumy / points.Length);
            return center;
        }

        /// <summary>
        /// 线性测试
        /// 判断stroke是直线或者曲线
        /// 如果是直线，将直线方程的参数写入到改stroke对象中
        /// 并且计算出改直线上每个点到其直线方程的垂足
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public Linearity LinearityTest(MyStroke stroke)
        {
            double linearity = 0;
            Point[] points = stroke.points;
            Point start = stroke.startPoint;
            Point end = stroke.endPoint;

            double sumofp2p = 0.0;
            for (int i = 1, j = points.Length; i < j; i++)
            {
                sumofp2p += distanceP2P(points[i], points[i - 1]);
            }

            linearity = distanceP2P(end, start) / sumofp2p;

            if (linearity >= THRESHOLD_LINEARITY)
            {
                stroke.linearity = Linearity.Line;//设置stroke的m和c属性
                computeMC(stroke);//为stroke添加m和c
                return Linearity.Line;
            }
            else
            {
                stroke.linearity = Linearity.Curve;
                return Linearity.Curve;
            }
        }

        /// <summary>
        /// distancetest判断两个stroke的距离
        /// 当且仅当两个stroke的都是线性的
        /// </summary>
        /// <param name="newstroke"></param>
        /// <returns></returns>
        public TESTRESULT DistanceTest(MyStroke newstroke, MyStroke prestroke)
        {
            if (prestroke == null || prestroke.linearity == Linearity.Curve)
                return TESTRESULT.SEPERATE;

            Point start1 = newstroke.adjustedPoints[0];
            Point end1 = newstroke.adjustedPoints[newstroke.adjustedPoints.Length - 1];
            Point start2 = prestroke.adjustedPoints[0];
            Point end2 = prestroke.adjustedPoints[prestroke.adjustedPoints.Length - 1];

            if (distanceP2P(start1, start2) < THRESHOLD_DISTANCE ||
                distanceP2P(start1, end2) < THRESHOLD_DISTANCE ||
                distanceP2P(end1, start2) < THRESHOLD_DISTANCE ||
                distanceP2P(end1, end2) < THRESHOLD_DISTANCE)
                return TESTRESULT.GROUP;
            else if
               (distanceP2L(start2, newstroke.m, -1, newstroke.c) < THRESHOLD_DISTANCE ||
                distanceP2L(end2, newstroke.m, -1, newstroke.c) < THRESHOLD_DISTANCE ||
                distanceP2L(start1, prestroke.m, -1, prestroke.c) < THRESHOLD_DISTANCE ||
                distanceP2L(end1, prestroke.m, -1, prestroke.c) < THRESHOLD_DISTANCE)
                return TESTRESULT.GROUP;
            else
                return TESTRESULT.SEPERATE;

        }

        /// <summary>
        /// angular test 判断两条直线的角度差别是否在threshold范围之内，
        /// 以判断他们是否可以分到一组
        /// </summary>
        /// <param name="newstroke"></param>
        /// <param name="prestroke"></param>
        /// <returns></returns>
        public TESTRESULT AngleTest(MyStroke newstroke, MyStroke prestroke)
        {
            if (prestroke == null || prestroke.linearity == Linearity.Curve)
                return TESTRESULT.SEPERATE;

            if (getAngleBetweenLine(newstroke.m, prestroke.m) < THRESHOLD_ANGULAR)
                return TESTRESULT.GROUP;
            else
                return TESTRESULT.SEPERATE;
        }

        /// <summary>
        /// 修正了bug
        /// 计算笔迹ms的方程y = mx + c的参数m和c
        /// </summary>
        /// <param name="ms"></param>
        public void computeMC(MyStroke ms)
        {
            ///只有ms是直线的时候才可以计算
            if (ms.linearity == Linearity.Curve)
                return;
            Point[] points = ms.points;
            double m = 0;
            double c = 0;
            double
                sumx2 = 0,
                sumx = 0,
                sumy = 0,
                sumxy = 0;

            Point pre = points[0];
            foreach (Point p in points)
            {
                int x = p.X, y = p.Y;
                sumx += x;
                sumy += y;
                sumx2 += x * x;
                sumxy += x * y;
            }

            m = (points.Length * sumxy - sumx * sumy) / (points.Length * sumx2 - sumx * sumx);
            c = (sumx2 * sumy - sumx * sumxy) / (points.Length * sumx2 - sumx * sumx);
            ms.m = m;
            ms.c = c;

            Point[] foots = new Point[points.Length];

            //拟合到直线上，计算每个点的垂足
            for (int i = 0; i < foots.Length; i++)
            {
                foots[i] = getFoot(points[i], m, -1, c);
            }
            ms.adjustedPoints = foots;

        }

        /// <summary>
        /// 计算group的skeleton方程 y = mx +c 
        /// 将结果m和c保存到strokegroup数据结构中
        /// </summary>
        /// <param name="group"></param>
        public void computeSkeleton(StrokeGroup group)
        {
            LinkedList<MyStroke> sts = group.strokeList;
            List<Point> ptlist = new List<Point>();
            foreach (MyStroke ms in sts)
            {
                Point[] points = ms.points;
                foreach (Point p in points)
                {
                    ptlist.Add(p);
                }
            }
            Point[] pts = ptlist.ToArray();

            double m = 0;
            double c = 0;
            double
                sumx2 = 0,
                sumx = 0,
                sumy = 0,
                sumxy = 0;

            foreach (Point p in pts)
            {
                int x = p.X, y = p.Y;
                sumx += x;
                sumy += y;
                sumx2 += x * x;
                sumxy += x * y;
            }

            m = (pts.Length * sumxy - sumx * sumy) / (pts.Length * sumx2 - sumx * sumx);
            c = (sumx2 * sumy - sumx * sumxy) / (pts.Length * sumx2 - sumx * sumx);

            group.m = m;
            group.c = c;
        }

        /// <summary>
        /// 返回两点之间的距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double distanceP2P(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }


        /// <summary>
        /// 计算点p到直线AX+BX+C=0的距离
        /// </summary>
        /// <param name="p"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public double distanceP2L(Point p, double A, double B, double C)
        {
            int x = p.X;
            int y = p.Y;
            return Math.Abs(A * x + B * y + C) / Math.Sqrt(A * A + B * B);
        }


        /// <summary>
        /// 计算两条直线之间的最短距离
        /// </summary>
        /// <param name="stroke1"></param>
        /// <param name="stroke2"></param>
        /// <returns></returns>
        public double distanceL2L(MyStroke stroke1, MyStroke stroke2)
        {
            Point[] ps1 = stroke1.adjustedPoints;
            Point[] ps2 = stroke2.adjustedPoints;

            Point start1 = ps1[0];
            Point end1 = ps1[ps1.Length - 1];
            Point start2 = ps2[0];
            Point end2 = ps2[ps2.Length - 1];

            double distance = distanceP2L(start1, stroke2.m, -1, stroke2.c);
            double distancetemp = 0;

            Point[] points = { start1, end1, start2, end2 };
            MyStroke[] strokes = { stroke1, stroke1, stroke2, stroke2 };
            for (int i = 0; i < points.Length; i++)
            {
                distancetemp = distanceP2L(points[i], strokes[i].m, -1, strokes[i].c);
                if (distancetemp < distance)
                    distance = distancetemp;
            }

            return distance;
        }

        /// <summary>
        /// 计算点p到直线Ax+By+C=0的垂足
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public Point getFoot(Point p, double A, double B, double C)
        {
            int x = p.X, y = p.Y;
            double x1, y1;
            x1 = (B * B * x - A * B * y - A * C) / (A * A + B * B);
            y1 = (-A * B * x + A * A * y - B * C) / (A * A + B * B);
            return new Point((int)x1, (int)y1);
        }

        /// <summary>
        /// 建立以给定两点为端点的直线的点序列
        /// 坐标为ink空间坐标
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Point[] getStrokeFromEnds(Point start, Point end)
        {
            List<Point> result = new List<Point>();
            if (end.X == start.X)
            {
                if (start.Y < end.Y)
                {
                    for (int i = start.Y; i < end.Y; i += 60)
                        result.Add(new Point(start.X, i));
                }
                else
                {
                    for (int i = end.Y; i < start.Y; i += 60)
                        result.Add(new Point(start.X, i));
                }
                return result.ToArray();
            }

            double m = (double)(end.Y - start.Y) / (double)(end.X - start.X);
            double c = end.Y - m * end.X;

            if (Math.Abs(m) < 1)
            {
                if (start.X < end.X)
                {
                    for (int x = start.X; x <= end.X; x += 60)
                    {
                        int y = (int)(m * x + c);
                        Point midPoint = new Point(x, y);
                        result.Add(midPoint);
                    }
                }
                else
                {
                    for (int x = end.X; x <= start.X; x += 60)
                    {
                        int y = (int)(m * x + c);
                        Point midPoint = new Point(x, y);
                        result.Add(midPoint);
                    }
                }
            }
            else
            {
                if (start.Y < end.Y)
                {
                    for (int y = start.Y; y <= end.Y; y += 60)
                    {
                        int x = (int)((y - c) / m);
                        Point midPoint = new Point(x, y);
                        result.Add(midPoint);
                    }
                }
                else
                {
                    for (int y = end.Y; y <= start.Y; y += 60)
                    {
                        int x = (int)((y - c) / m);
                        Point midPoint = new Point(x, y);
                        result.Add(midPoint);
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 取得 y = m1x +c 与 y = m2x + c的夹角
        /// 计算(1,m1)与(1,m2)两个向量的夹角
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        private double getAngleBetweenLine(double m1, double m2)
        {
            return Math.Acos(Math.Abs(m1 * m2 + 1) / (Math.Sqrt(m1 * m1 + 1) * Math.Sqrt(m2 * m2 + 1)));
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

        /// <summary>
        /// 将inkspace中的Points转换成为像素空间的坐标
        /// </summary>
        /// <param name="inkpicture"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public Point[] InkSpaceToPixelPoints(InkPicture inkpicture, Point[] points)
        {
            Graphics g = inkpicture.CreateGraphics();
            Point[] pts = (Point[])(points.Clone());
            inkpicture.Renderer.InkSpaceToPixel(g, ref pts);

            return pts;
        }

        /// <summary>
        /// 将inkspace中的rectangle转换成像素的坐标
        /// </summary>
        /// <param name="inkpicture"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectangle InkSpaceToPixelRect(IntPtr handle, Renderer renderer, Rectangle rect)
        {
            Graphics g = Graphics.FromHwnd(handle);
            Point p1 = rect.Location;
            Point p2 = new Point(rect.Width, rect.Height);
            renderer.InkSpaceToPixel(g, ref p1);
            renderer.InkSpaceToPixel(g, ref p2);
            return new Rectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        //YHY-090415
        public double radianToDegree(double rad)
        {
            return rad * 180.0 / PI;
        }
        public double degreeToRadian(double ang)
        {
            return PI * ang / 180.0;
        }

        //计算两个点之间的角
        public double angleOfPoint(Point ptStart, Point ptEnd, double criticalValue)
        {
            double temp;
            double angle;
            double XLen = ptEnd.X - ptStart.X;
            double YLen = ptEnd.Y - ptStart.Y;

            if (System.Math.Abs(XLen) <= criticalValue) return 90;
            else
            {
                temp = (double)YLen / XLen;
                angle = System.Math.Atan(temp);
                return radianToDegree(angle);
            }
        }


        //---------------------------------------------------------------------------------------
        //YHY-090421
        //判断一个stroke是否是近似圆
        public bool JudgeCircle(MyStroke stroke)
        {
            double dist = distanceP2P(stroke.startPoint, stroke.endPoint);

            double rationWtoH = 0;
            if (stroke.boundingBox.Width > stroke.boundingBox.Height && stroke.boundingBox.Height != 0)
                rationWtoH = stroke.boundingBox.Width / stroke.boundingBox.Height;
            else if (stroke.boundingBox.Height > stroke.boundingBox.Width && stroke.boundingBox.Width != 0)
                rationWtoH = stroke.boundingBox.Height / stroke.boundingBox.Width;

            double rationLenthtoBoundingbox;
            stroke.setLenth();
            rationLenthtoBoundingbox = stroke.lenth / ((stroke.boundingBox.Height + stroke.boundingBox.Width) * 2);

            if (dist < 1000 && rationWtoH < 1.5 && rationLenthtoBoundingbox < 0.9)
                return true;

            return false;
        }
        //---------------------------------------------------------------------------------------

        public double THRESHOLD_LINEARITY = 0.95;
        public double THRESHOLD_DISTANCE = 350;
        public double THRESHOLD_ANGULAR = 15 * Math.PI / 180;
        public double PI = 3.14159265358979;
    }
}
