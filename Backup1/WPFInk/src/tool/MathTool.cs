using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using WPFInk.ink;
//using WPFInk.recognizer;
using System.IO;
using System.Windows.Media.Imaging;
using WPFInk.videoSummarization;

namespace WPFInk.tool
{
    /// <summary>
    /// 为笔迹处理提供数学计算函数
    /// </summary>
    public class MathTool
    {
        private static MathTool Singleton = null;

        /// <summary>
        /// 不可以从外部调用构造函数
        /// </summary>
        private MathTool()
        {

        }

        public static MathTool getInstance()
        {
            if (Singleton == null)
                Singleton = new MathTool();
            return Singleton;
        }

		/// <summary>
        /// 将多边形按照其中心旋转角度angel
        /// angel为度数
        /// </summary>
        /// <param name="points">多边形的顶点</param>
        /// <param name="angel">需要旋转的角度</param>
        /// <returns>旋转后的多边形顶点</returns>
        public Point[] rotatePolygon(Point[] points, float angle)
        {
            Point center = getCenter(points);

            Point[] result = new Point[points.Length];
            angle = (float)(angle * Math.PI / 180);
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

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
        /// 将点point绕center旋转angle度数
        /// </summary>
        /// <param name="point">需要旋转的点</param>
        /// <param name="center">中心点</param>
        /// <param name="angle">旋转度数</param>
        /// <returns></returns>
        public StylusPoint rotatePoint(StylusPoint point, StylusPoint center, double angle)
        {
            angle = (double)(angle * Math.PI / 180);
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            StylusPoint offset = new StylusPoint(point.X - center.X, point.Y - center.Y);
            double xoffset = offset.X * cos - offset.Y * sin;
            double yoffset = offset.X * sin + offset.Y * cos;
            return new StylusPoint(center.X + xoffset, center.Y + yoffset);
        }
        /// <summary>
        /// 获取直线上距离sp1距离为distance2Sp1的点
        /// </summary>
        /// <param name="sp1">直线上点1</param>
        /// <param name="sp2">直线上点2</param>
        /// <param name="distance2Sp1">所求点距离点1的距离</param>
        /// <returns></returns>
        public StylusPoint getPointInLine(StylusPoint sp1, StylusPoint sp2, double distance2Sp1)
        {
            StylusPoint result = new StylusPoint();
            double distance2P = distanceP2P(sp1, sp2);
            if (sp1.Y == sp2.Y)
            {
                if (sp1.X > sp2.X)
                {
                    result.X = sp1.X - distance2Sp1;
                }
                else
                {
                    result.X = sp1.X + distance2Sp1;
                }
                result.Y = sp1.Y;
            }
            else
            {
                if (sp1.X > sp2.X)
                {
                    double k = (sp2.Y - sp1.Y) / (sp2.X - sp1.X);
                    result.X = sp1.X - (sp1.X - sp2.X) * distance2Sp1 / distance2P;
                    result.Y = (result.X - sp1.X) * k + sp1.Y;
                }
                else if (sp1.X == sp2.X)
                {
                    if (sp1.Y > sp2.Y)
                    {
                        result.Y = sp1.Y - distance2Sp1;
                    }
                    else
                    {
                        result.Y = sp1.Y + distance2Sp1;
                    }
                    result.X = sp1.X;

                }
                else
                {
                    double k = (sp2.Y - sp1.Y) / (sp2.X - sp1.X);
                    result.X = sp1.X + (sp1.X - sp2.X) * distance2Sp1 / distance2P;
                    result.Y = (result.X - sp1.X) * k + sp1.Y;
                }
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
            double sumx = 0, sumy = 0;
            foreach (Point p in points)
            {
                sumx += p.X;
                sumy += p.Y;
            }
            Point center = new Point(sumx / points.Length, sumy / points.Length);
            return center;
        }
        /// <summary>
        /// 计算有StylusPointCollection组成的多边形的中心
        /// </summary>
        /// <param name="points">多边形的顶点</param>
        /// <returns>多边形的中心坐标</returns>
        public StylusPoint getCenter(StylusPointCollection points)
        {
            double sumx = 0, sumy = 0;
            foreach (Point p in points)
            {
                sumx += p.X;
                sumy += p.Y;
            }
            StylusPoint center = new StylusPoint(sumx / points.Count, sumy / points.Count);
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
        //public Linearity LinearityTest(MyStroke stroke)
        //{
        //    double linearity = 0;
        //    StylusPointCollection points = stroke.StylusPoints;
        //    ///如果点数少于2，直线
        //    if (stroke.StylusPoints.Count <= 2)
        //        return Linearity.Line;
        //    StylusPoint start = points[0];
        //    StylusPoint end = points[points.Count-1];

        //    double sumofp2p = 0.0;
        //    for (int i = 1, j = points.Count; i < j; i++)
        //    {
        //        sumofp2p += distanceP2P(points[i], points[i - 1]);
        //    }

        //    linearity = distanceP2P(end, start) / sumofp2p;

        //    if (linearity >= THRESHOLD_LINEARITY)
        //    {
        //        stroke.linearity = Linearity.Line;//设置stroke的m和c属性
        //        computeMC(stroke);//为stroke添加m和c
        //        return Linearity.Line;
        //    }
        //    else
        //    {
        //        stroke.linearity = Linearity.Curve;
        //        return Linearity.Curve;
        //    }
        //}

        /// <summary>
        /// 方法重载，参数为stroke类型
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        //public Linearity LinearityTest(Stroke stroke)
        //{
        //    double linearity = 0;
        //    StylusPointCollection points = stroke.StylusPoints;
        //    ///如果点数少于2，直线
        //    if (stroke.StylusPoints.Count <= 2)
        //        return Linearity.Line;
        //    StylusPoint start = points[0];
        //    StylusPoint end = points[points.Count - 1];

        //    double sumofp2p = 0.0;
        //    for (int i = 1, j = points.Count; i < j; i++)
        //    {
        //        sumofp2p += distanceP2P(points[i], points[i - 1]);
        //    }

        //    linearity = distanceP2P(end, start) / sumofp2p;

        //    if (linearity >= THRESHOLD_LINEARITY)
        //    {
        //        return Linearity.Line;
        //    }
        //    else
        //    {
        //        return Linearity.Curve;
        //    }
        //}

        /// <summary>
        /// distancetest判断两个stroke的距离
        /// 当且仅当两个stroke的都是线性的
        /// </summary>
        /// <param name="newstroke"></param>
        /// <returns></returns>
        //public TESTRESULT DistanceTest(MyStroke newstroke, MyStroke prestroke)
        //{
        //    if (prestroke == null || prestroke.linearity == Linearity.Curve)
        //        return TESTRESULT.SEPERATE;

        //    StylusPoint start1 = newstroke.adjustedPoints[0];
        //    StylusPoint end1 = newstroke.adjustedPoints[newstroke.adjustedPoints.Count - 1];
        //    StylusPoint start2 = prestroke.adjustedPoints[0];
        //    StylusPoint end2 = prestroke.adjustedPoints[prestroke.adjustedPoints.Count - 1];

        //    if (distanceP2P(start1, start2) < THRESHOLD_DISTANCE ||
        //        distanceP2P(start1, end2) < THRESHOLD_DISTANCE ||
        //        distanceP2P(end1, start2) < THRESHOLD_DISTANCE ||
        //        distanceP2P(end1, end2) < THRESHOLD_DISTANCE)
        //        return TESTRESULT.GROUP;
        //    else if
        //       (distanceP2L(start2, newstroke.m, -1, newstroke.c, newstroke.startPoint, newstroke.endPoint) < THRESHOLD_DISTANCE ||
        //        distanceP2L(end2, newstroke.m, -1, newstroke.c, newstroke.startPoint, newstroke.endPoint) < THRESHOLD_DISTANCE ||
        //        distanceP2L(start1, prestroke.m, -1, prestroke.c, prestroke.startPoint, prestroke.endPoint) < THRESHOLD_DISTANCE ||
        //        distanceP2L(end1, prestroke.m, -1, prestroke.c, prestroke.startPoint, prestroke.endPoint) < THRESHOLD_DISTANCE)
        //        return TESTRESULT.GROUP;
        //    else
        //        return TESTRESULT.SEPERATE;
        //}

        /// <summary>
        /// angular test 判断两条直线的角度差别是否在threshold范围之内，
        /// 以判断他们是否可以分到一组
        /// </summary>
        /// <param name="newstroke"></param>
        /// <param name="prestroke"></param>
        /// <returns></returns>
        //public TESTRESULT AngleTest(MyStroke newstroke, MyStroke prestroke)
        //{
        //    if (prestroke == null || prestroke.linearity == Linearity.Curve)
        //        return TESTRESULT.SEPERATE;

        //    if (getAngleBetweenLine(newstroke.m, prestroke.m) < THRESHOLD_ANGULAR)
        //        return TESTRESULT.GROUP;
        //    else
        //        return TESTRESULT.SEPERATE;
        //}

        /// <summary>
        /// 修正了bug
        /// 计算笔迹ms的方程y = mx + c的参数m和c
        /// </summary>
        /// <param name="ms"></param>
        //public void computeMC(MyStroke ms)
        //{
        //    ///只有ms是直线的时候才可以计算
        //    if (ms.linearity == Linearity.Curve)
        //        return;
        //    StylusPointCollection points = ms.StylusPoints;
        //    double m = 0;
        //    double c = 0;
        //    double
        //        sumx2 = 0,
        //        sumx = 0,
        //        sumy = 0,
        //        sumxy = 0;

        //    StylusPoint pre = points[0];
        //    foreach (StylusPoint p in points)
        //    {
        //        double x = p.X, y = p.Y;
        //        sumx += x;
        //        sumy += y;
        //        sumx2 += x * x;
        //        sumxy += x * y;
        //    }

        //    m = (points.Count * sumxy - sumx * sumy) / (points.Count * sumx2 - sumx * sumx);
        //    c = (sumx2 * sumy - sumx * sumxy) / (points.Count * sumx2 - sumx * sumx);
        //    ms.m = m;
        //    ms.c = c;

        //    ms.adjustedPoints = new StylusPointCollection();
        //    //拟合到直线上，计算每个点的垂足
        //    for (int i = 0; i < points.Count; i++)
        //    {
        //        ms.adjustedPoints.Add(getFoot(points[i], m, -1, c));
        //    }
        //}

        /// <summary>
        /// 计算group的skeleton方程 y = mx +c 
        /// 将结果m和c保存到strokegroup数据结构中
        /// </summary>
        /// <param name="group"></param>
        public void computeSkeleton(StrokeGroup group)
        {
            List<MyStroke> sts = group.strokeList;
            List<StylusPoint> ptlist = new List<StylusPoint>();
            foreach (MyStroke ms in sts)
            {
                StylusPointCollection points = ms.StylusPoints;
                foreach (StylusPoint p in points)
                {
                    ptlist.Add(p);
                }
            }
            StylusPoint[] pts = ptlist.ToArray();

            double m = 0;
            double c = 0;
            double
                sumx2 = 0,
                sumx = 0,
                sumy = 0,
                sumxy = 0;

            foreach (StylusPoint p in pts)
            {
                double x = p.X, y = p.Y;
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
        public double distanceP2P(StylusPoint p1, StylusPoint p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        /// <summary>
        /// 返回两点之间距离
        /// 方法重载
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double distanceP2P(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        /// <summary>
        /// 返回两点的距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double distanceSP2SP(StylusPoint p1, StylusPoint p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        /// <summary>
        /// 计算两个矩形的垂直距离
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public double verticalDistanceR2R(Rect r1, Rect r2)
        {
            if (r1.Top > r2.Top)
            {
                return verticalDistanceR2R(r2, r1);
            }
            return r2.Top - r1.Top - r1.Height;
        }
        /// <summary>
        /// 计算两个矩形的水平距离
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public double horizontalDistanceR2R(Rect r1, Rect r2)
        {
            if (r1.Left > r2.Left)
            {
                return horizontalDistanceR2R(r2, r1);
            }
            return r2.Left - r1.Left - r1.Width;
        }
        /// <summary>
        /// 计算两个矩形的距离
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public double distanceR2R(Rect r1, Rect r2)
        {
            double horizontalDistance = horizontalDistanceR2R(r1, r2);
            double verticalDistance = verticalDistanceR2R(r1, r2);
            if (horizontalDistance > 0 && verticalDistance > 0)
            {
                return Math.Sqrt(horizontalDistance * horizontalDistance + verticalDistance * verticalDistance);
            }
            else if (horizontalDistance <= 0 && verticalDistance > 0)
            {
                return verticalDistance;
            }
            else if (horizontalDistance > 0 && verticalDistance <= 0)
            {
                return horizontalDistance;
            }
            
            return -Math.Sqrt(horizontalDistance * horizontalDistance + verticalDistance * verticalDistance);
            
        }
        /// <summary>
        /// 计算点p到直线AX+BX+C=0的距离
        /// </summary>
        /// <param name="p"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public double distanceP2L(StylusPoint p, double A, double B, double C)
        {
            double x = p.X;
            double y = p.Y;
            return Math.Abs(A * x + B * y + C) / Math.Sqrt(A * A + B * B);
        }
        /// <summary>
        /// 计算点p到笔迹S最近的点的距离
        /// </summary>
        /// <param name="p"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public double distanceP2S(StylusPoint p, Stroke s)
        {
            int index=InkTool.getInstance().getNearestPointInStroke(s, p);
            return distanceP2P(p, s.StylusPoints[index]);
        }

        /// <summary>
        /// 计算点p到直线AX+BX+C=0上距离p最近的点的距离
        /// </summary>
        /// <param name="p"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double distanceP2L(StylusPoint p, double A, double B, double C, StylusPoint start, StylusPoint end)
        {
            double x = p.X;
            double y = p.Y;
            double result;
            StylusPoint foot = getFoot(p, A, B, C);
            if ((foot.X - start.X) * (foot.X - end.X) > 0)
                result =  Math.Min(distanceP2P(p, start), distanceP2P(p, end));
            else
                result =  Math.Abs(A * x + B * y + C) / Math.Sqrt(A * A + B * B);
            return result;
        }

        /// <summary>
        /// 计算点p到直线(start-end)的距离
        /// </summary>
        /// <param name="p"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double distanceP2L(StylusPoint p, StylusPoint start, StylusPoint end)
        {
            double dist = 0;
            /*
             * 特殊情况：start和end的横坐标相等
             */
            if (start.X == end.X)
                dist = start.X - p.X;
            else
            {
                double m, c;
                m = (end.Y - start.Y) / (end.X - start.X);
                c = start.Y - m * start.X;
                dist = distanceP2L(p, m, -1, c);
            }

            return Math.Abs(dist);
        }

        /// <summary>
        /// 计算两条直线之间的最短距离
        /// </summary>
        /// <param name="stroke1"></param>
        /// <param name="stroke2"></param>
        /// <returns></returns>
        public double distanceL2L(MyStroke stroke1, MyStroke stroke2)
        {
            StylusPointCollection ps1 = stroke1.StylusPoints;
            StylusPointCollection ps2 = stroke2.StylusPoints;

            StylusPoint start1 = ps1[0];
            StylusPoint end1 = ps1[ps1.Count - 1];
            StylusPoint start2 = ps2[0];
            StylusPoint end2 = ps2[ps2.Count - 1];

            double distance = distanceP2L(start1, stroke2.m, -1, stroke2.c);
            double distancetemp = 0;

            StylusPoint[] points = { start1, end1, start2, end2 };
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
        /// 计算两条直线笔迹之间的最短距离
        /// </summary>
        /// <param name="stroke1"></param>
        /// <param name="stroke2"></param>
        /// <returns></returns>
        public double distanceL2L(Stroke stroke1, Stroke stroke2)
        {
            StylusPointCollection ps1 = stroke1.StylusPoints;
            StylusPointCollection ps2 = stroke2.StylusPoints;

            StylusPoint start1 = ps1[0];
            StylusPoint end1 = ps1[ps1.Count - 1];
            StylusPoint start2 = ps2[0];
            StylusPoint end2 = ps2[ps2.Count - 1];

            double distance1 = distanceP2L(start1, start2, end2);
            double distance2 = distanceP2L(end1, start2, end2);
            double distance3 = distanceP2L(start2, start1, end1);
            double distance4 = distanceP2L(end2, start1, end1);
            

            return Math.Min(Math.Min(distance1,distance2),Math.Min(distance3,distance4));
        }

        /// <summary>
        /// 计算点p到直线Ax+By+C=0的垂足
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public StylusPoint getFoot(StylusPoint p, double A, double B, double C)
        {
            double x = p.X, y = p.Y;
            double x1, y1;
            x1 = (B * B * x - A * B * y - A * C) / (A * A + B * B);
            y1 = (-A * B * x + A * A * y - B * C) / (A * A + B * B);
            return new StylusPoint((int)x1, (int)y1);
        }

        /// <summary>
        /// 建立以给定两点为端点的直线的点序列
        /// 坐标为ink空间坐标
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public StylusPoint[] getStrokeFromEnds(StylusPoint start, StylusPoint end)
        {
            List<StylusPoint> result = new List<StylusPoint>();
            if (end.X == start.X)
            {
                if (start.Y < end.Y)
                {
                    for (double i = start.Y; i < end.Y; i += 60)
                        result.Add(new StylusPoint(start.X, i));
                }
                else
                {
                    for (double i = end.Y; i < start.Y; i += 60)
                        result.Add(new StylusPoint(start.X, i));
                }
                return result.ToArray();
            }

            double m = (double)(end.Y - start.Y) / (double)(end.X - start.X);
            double c = end.Y - m * end.X;

            if (Math.Abs(m) < 1)
            {
                if (start.X < end.X)
                {
                    for (double x = start.X; x <= end.X; x += 60)
                    {
                        int y = (int)(m * x + c);
                        StylusPoint midPoint = new StylusPoint(x, y);
                        result.Add(midPoint);
                    }
                }
                else
                {
                    for (double x = end.X; x <= start.X; x += 60)
                    {
                        int y = (int)(m * x + c);
                        StylusPoint midPoint = new StylusPoint(x, y);
                        result.Add(midPoint);
                    }
                }
            }
            else
            {
                if (start.Y < end.Y)
                {
                    for (double y = start.Y; y <= end.Y; y += 60)
                    {
                        int x = (int)((y - c) / m);
                        StylusPoint midPoint = new StylusPoint(x, y);
                        result.Add(midPoint);
                    }
                }
                else
                {
                    for (double y = end.Y; y <= start.Y; y += 60)
                    {
                        int x = (int)((y - c) / m);
                        StylusPoint midPoint = new StylusPoint(x, y);
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
        /// 根据起始点，计算向量的角度
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double getAngleP2P(StylusPoint start, StylusPoint end)
        {
            double x1 = start.X,
                y1 = start.Y,
                x2 = end.X,
                y2 = end.Y;
            double angle = 0;
            if (x1 == x2)
            {
                if (y1 < y2)
                    angle = 90;
                else
                    angle = 270;
            }
            else
            {
                double slope = (y2 - y1) / (x2 - x1);
                if (y2 > y1) //小于180
                {
                    if (x2 > x1)
                        angle = Math.Atan(slope) * (180 / Math.PI);
                    else
                        angle = 180 + Math.Atan(slope) * (180 / Math.PI);
                }
                else
                {
                    if (x2 > x1)
                        angle = 360 + Math.Atan(slope) * (180 / Math.PI);
                    else
                        angle = 180 + Math.Atan(slope) * (180 / Math.PI);
                }
            }

            return 360-angle;
        } 
        /// <summary>
        /// 根据起始点，计算向量的角度
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double getAngleP2P(StylusPoint start, double x2, double y2)
        {
            double x1 = start.X,
                y1 = start.Y;
            double angle = 0;
            if (x1 == x2)
            {
                if (y1 < y2)
                    angle = 90;
                else
                    angle = 270;
            }
            else
            {
                double slope = (y2 - y1) / (x2 - x1);
                if (y2 > y1) //小于180
                {
                    if (x2 > x1)
                        angle = Math.Atan(slope) * (180 / Math.PI);
                    else
                        angle = 180 + Math.Atan(slope) * (180 / Math.PI);
                }
                else
                {
                    if (x2 > x1)
                        angle = 360 + Math.Atan(slope) * (180 / Math.PI);
                    else
                        angle = 180 + Math.Atan(slope) * (180 / Math.PI);
                }
            }

            return 360 - angle;
        }
        /// <summary>
        /// 根据起始点，计算向量的角度
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double getAngleP2P(double x1,double y1,double x2,double y2)
        {
            double angle = 0;
            if (x1 == x2)
            {
                if (y1 < y2)
                    angle = 90;
                else
                    angle = 270;
            }
            else
            {
                double slope = (y2 - y1) / (x2 - x1);
                if (y2 > y1) //小于180
                {
                    if (x2 > x1)
                        angle = Math.Atan(slope) * (180 / Math.PI);
                    else
                        angle = 180 + Math.Atan(slope) * (180 / Math.PI);
                }
                else
                {
                    if (x2 > x1)
                        angle = 360 + Math.Atan(slope) * (180 / Math.PI);
                    else
                        angle = 180 + Math.Atan(slope) * (180 / Math.PI);
                }
            }

            return 360 - angle;
        }
        /// <summary>
        /// 计算两条直线之间的角度,角度去锐角
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public double getAngleL2L(Stroke s1, Stroke s2)
        {
            StylusPointCollection ps1 = s1.StylusPoints;
            StylusPointCollection ps2 = s2.StylusPoints;
            int s1PointCount = ps1.Count;
            int s2PointCount = ps2.Count;
            double angle1 = MathTool.getInstance().getAngleP2P(s1.StylusPoints[0], s1.StylusPoints[s1PointCount - 1]);
            double angle2 = MathTool.getInstance().getAngleP2P(s2.StylusPoints[0], s2.StylusPoints[s2PointCount - 1]);
            double angle = Math.Abs(angle1 - angle2);
            StylusPoint start1 = ps1[0];
            StylusPoint end1 = ps1[ps1.Count - 1];
            StylusPoint start2 = ps2[0];
            StylusPoint end2 = ps2[ps2.Count - 1];
            if (start1.X == end1.X)
            {
                return (int)angle2 % 90;
            }
            if (start2.X == end2.X)
            {
                return (int)angle1 % 90;
            }
            double tan1 = (end1.Y - start1.Y) / (end1.X - start1.X);
            double tan2 = (end2.Y - start2.Y) / (end2.X - start2.X);
            angle = Math.Atan(Math.Abs(tan1 - tan2) / (1 + tan1 * tan2));
            angle = Math.Abs(angle * 180 / Math.PI);
            return angle;
        }
        /// <summary>
        /// 求两交叉直线之间的角度
        /// </summary>
        /// <param name="p1">直线1上某点</param>
        /// <param name="p2">直线2上某点</param>
        /// <param name="intersectionPoint">交点，不等于p1，p2</param>
        /// <returns></returns>
        public double getAngleL2L(StylusPoint p1, StylusPoint p2, StylusPoint intersectionPoint)
        {           
            double angle1 = MathTool.getInstance().getAngleP2P(p1, intersectionPoint);
            double angle2 = MathTool.getInstance().getAngleP2P(p2, intersectionPoint);
            double angle=0;
            if(angle1>angle2)//使得angle1<angle2
            {
                double temp=angle1;
                angle1=angle2;
                angle2=temp;
            }
            if (angle2 - angle1 < 180)
            {
                angle = angle2 - angle1;
            }
            else
            {
                angle = 360 - (angle2 - angle1);
            }
            return angle;
        }
        /// <summary>
		/// 计算两个点之间的角
        /// </summary>
        /// <param name="ptStart">起始点</param>
        /// <param name="ptEnd">终止点</param>
        /// <param name="criticalValue">临界值</param>
        /// <returns></returns>
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

        /// <summary>
        /// 将角度转换成度数
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public double radianToDegree(double rad)
        {
            return rad * 180.0 / Math.PI;
        }

        /// <summary>
        /// 将度数转化成角度
        /// </summary>
        /// <param name="ang"></param>
        /// <returns></returns>
        public double degreeToRadian(double ang)
        {
            return Math.PI * ang / 180.0;
        }

        /// <summary>
        /// 返回图片的中心点
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public StylusPoint getImageCenter(MyImage image)
        {
            Rect bounds = new Rect(image.Image.Margin.Left, image.Image.Margin.Top, image.Image.Width, image.Image.Height);
            Rect r = image.Image.RenderTransform.TransformBounds(bounds);
            return new StylusPoint((r.Left + r.Right) / 2, (r.Top + r.Bottom) / 2);
        }

        /// <summary>
        /// 返回图片的bounds
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Rect getImageBounds(MyImage image)
        {
            Rect bounds = new Rect(image.Left, image.Top, image.Width, image.Height);
            Rect r = image.Image.RenderTransform.TransformBounds(bounds);
            return r;
        }

        /// <summary>
        /// 返回图片的中心点
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public StylusPoint getMyButtonCenter(MyButton myButton)
        {
            return new StylusPoint(myButton.Left + myButton.Width / 2, myButton.Top + myButton.Height / 2);
        }

        /// <summary>
        /// 返回button的bounds
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Rect getMyButtonBounds(MyButton myButton)
        {
            Rect bounds = new Rect(myButton.Left, myButton.Top, myButton.Width, myButton.Height);
            Rect r = myButton.Button.RenderTransform.TransformBounds(bounds);
            return r;
        }


        /// <summary>
		/// 判断一个点是否在mybutton周围
        /// </summary>
        /// <param name="point"></param>
        /// <param name="mybutton"></param>
        /// <param name="offset">距离Mybutton的边界距离</param>
        /// <returns></returns>
        public bool isCloseMyButton(Point point, MyButton mybutton,int offset)
        {
            bool flag = false;
            if (point.X >= mybutton.Left - offset && point.X <= mybutton.Left + mybutton.Width + offset && point.Y >= mybutton.Top - offset && point.Y < mybutton.Top + mybutton.Height + offset)
            {
                flag = true;
            }
            return flag;
        }

		/// <summary>
		/// 判断一个点是否在MyButton外的某个环里面
		/// </summary>
		/// <param name="point"></param>
		/// <param name="mybutton"></param>
		/// <param name="offsetIn">内边界</param>
		/// <param name="offsetOut">外边界</param>
		/// <returns></returns>
		public bool isEncircleMyButton(Point point, MyButton mybutton,int offsetIn, int offsetOut)
		{
			bool flag = false;
			if ((((point.X >= mybutton.Left - offsetOut && point.X <= mybutton.Left - offsetIn) || (point.X <= mybutton.Left + mybutton.Width + offsetOut && point.X >= mybutton.Left + mybutton.Width + offsetIn)) && (point.Y >= mybutton.Top - offsetOut && point.Y <= mybutton.Top + mybutton.Height + offsetOut)) || (((point.Y <= mybutton.Top - offsetIn && point.Y >= mybutton.Top - offsetOut) || (point.Y <= mybutton.Top + mybutton.Height + offsetIn && point.Y >= mybutton.Top + mybutton.Height + offsetOut)) && (point.X >= mybutton.Left - offsetIn && point.X <= mybutton.Left + mybutton.Width + offsetIn)))
			{
				flag = true;
			}
			return flag;
		}


        /// <summary>
		/// 判断一个两个矩形是否有交集
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public bool isHitRects(Rect rect1, Rect rect2)
        {
            if (rect1.BottomRight.Y < rect2.TopLeft.Y || rect1.TopLeft.Y > rect2.BottomLeft.Y || rect1.TopRight.X < rect2.TopLeft.X || rect1.TopLeft.X > rect2.TopRight.X)
                return false;
            return true;
        }
		/// <summary>
		/// 判断小矩形是否在大矩形内
		/// </summary>
		/// <param name="rectBig">大矩形</param>
		/// <param name="rectSmall">小矩形</param>
		/// <returns></returns>
		public bool isContainRectangle(Rectangle rectBig,Rectangle rectSmall)
		{
			if (rectSmall.Margin.Left >= rectBig.Margin.Left && rectSmall.Margin.Top >= rectBig.Margin.Top && rectSmall.Margin.Left + rectSmall.Width <= rectBig.Margin.Left + rectBig.Width && rectSmall.Margin.Top + rectSmall.Height <= rectBig.Margin.Top + rectBig.Height)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 判断小矩形是否在大矩形内
		/// </summary>
		/// <param name="rectBig">大矩形</param>
		/// <param name="rectSmall">小矩形</param>
		/// <returns></returns>
		public bool isContainRect(Rect rectBig, Rect rectSmall)
		{
			if (rectSmall.Left >= rectBig.Left && rectSmall.Top >= rectBig.Top && rectSmall.TopRight.X <= rectBig.TopRight.X && rectSmall.BottomRight.Y <= rectBig.BottomRight.Y)
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// 判断一个点是否在矩形内
		/// </summary>
		/// <param name="point"></param>
		/// <param name="rect"></param>
		/// <param name="offset">边界误差</param>
		/// <returns></returns>
		public bool isCloseRectangle(Point point, Rectangle rect, int offset)
		{
			bool flag = false;
			if (point.X >= rect.Margin.Left - offset && point.X <= rect.Margin.Left + rect.Width + offset && point.Y >= rect.Margin.Top - offset && point.Y < rect.Margin.Top + rect.Height + offset)
			{
				flag = true;
			}
			return flag;
		}

		/// <summary>
		/// 判断一个点是否在rect外的某个环里面
		/// </summary>
		/// <param name="point"></param>
		/// <param name="rect"></param>
		/// <param name="offsetIn"></param>
		/// <param name="offsetOut"></param>
		/// <returns></returns>
		public bool isEncircleRectangle(Point point, Rectangle rect, int offsetIn, int offsetOut)
		{
			bool flag = false;
			if ((((point.X >= rect.Margin.Left - offsetOut && point.X <= rect.Margin.Left - offsetIn) || (point.X <= rect.Margin.Left + rect.Width + offsetOut && point.X >= rect.Margin.Left + rect.Width + offsetIn)) && (point.Y >= rect.Margin.Top - offsetOut && point.Y <= rect.Margin.Top + rect.Height + offsetOut)) || (((point.Y <= rect.Margin.Top - offsetIn && point.Y >= rect.Margin.Top - offsetOut) || (point.Y <= rect.Margin.Top + rect.Height + offsetIn && point.Y >= rect.Margin.Top + rect.Height + offsetOut)) && (point.X >= rect.Margin.Left - offsetIn && point.X <= rect.Margin.Left + rect.Width + offsetIn)))
			{
				flag = true;
			}
			return flag;
		}
		/// <summary>
		/// Rect转化为Rectangle
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public Rectangle RectToRectangle(Rect rect)
		{
			Rectangle rectangle = new Rectangle();
			rectangle.HorizontalAlignment = HorizontalAlignment.Left;
			rectangle.VerticalAlignment = VerticalAlignment.Top;
			rectangle.Margin = new Thickness(rect.Left, rect.Top, 0, 0);
			rectangle.Width = rect.Width;
			rectangle.Height = rect.Height;
			return rectangle;
		}

		public void getFrameByPosition(MediaElement mediaElement,int position,string imageName)
		{
			TimeSpan ts = new TimeSpan(0, 0, 0, 0, position);
			mediaElement.Position = ts;
			mediaElement.Play();
			FileStream stream = File.Open("c://"+imageName+".png", FileMode.Create);

			RenderTargetBitmap bmp = new RenderTargetBitmap((int)mediaElement.ActualWidth,

				(int)mediaElement.ActualHeight, 96, 96, PixelFormats.Pbgra32);

			bmp.Render(mediaElement);

			PngBitmapEncoder coder = new PngBitmapEncoder();

			coder.Interlace = PngInterlaceOption.Off;

			coder.Frames.Add(BitmapFrame.Create(bmp));

			coder.Save(stream);

			stream.Close();
		}

		/// <summary>
		/// Levenshtein Distance算法，计算两个字符串的相异程度
		/// </summary>
		/// <param name="s"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public int Levenshtein(string s,string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			// Step 1
			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			// Step 2
			for (int i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (int j = 0; j <= m; d[0, j] = j++)
			{
			}

			// Step 3
			for (int i = 1; i <= n; i++)
			{
				//Step 4
				for (int j = 1; j <= m; j++)
				{
					// Step 5
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					// Step 6
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			// Step 7
			return d[n, m];

		}
		/// <summary>
		///移动Stroke
		/// </summary>
		/// <param name="stroke"></param>
		public void MoveStroke(Stroke stroke, double offset_x, double offset_y)
		{
            if (stroke != null)
            {
                Matrix moveMatrix = new Matrix(1, 0, 0, 1, offset_x, offset_y);

                stroke.Transform(moveMatrix, false);
            }
		}
		/// <summary>
		/// 移动StrokeCollection
		/// </summary>
		/// <param name="strokes"></param>
		public void MoveStrokes(StrokeCollection strokes, double offset_x, double offset_y)
		{
            if (strokes!=null&&strokes.Count > 0)
            {
                Matrix moveMatrix = new Matrix(1, 0, 0, 1, offset_x, offset_y);
                strokes.Transform(moveMatrix, false);
            }
		}

        /// <summary>
        /// 判断点是否在矩形内
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool isPointInRectangle(Rectangle rectangle, Point point)
        {
            if (point.X >= rectangle.Margin.Left && point.X <= rectangle.Margin.Left + rectangle.Width && point.Y >= rectangle.Margin.Top && point.Y <= rectangle.Margin.Top + rectangle.Height)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断点是否在矩形内
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool isPointInRect(Rect rect, StylusPoint point)
        {
            if (point.X >= rect.Left && point.X <= rect.Left + rect.Width && point.Y >= rect.Top && point.Y <= rect.Top + rect.Height)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断坐标为（pointX，pointY）点是否在矩形内
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool isPointXYinRectangle(Rectangle rectangle, double pointX, double pointY)
        {
            if (pointX >= rectangle.Margin.Left && pointX <= rectangle.Margin.Left + rectangle.Width && pointY >= rectangle.Margin.Top && pointY <= rectangle.Margin.Top + rectangle.Height)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 去除字符串中的空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string removeSpace(string str)
        {
            string Str1 = str.Replace(" ", "");
            return Str1;
        }
        /// <summary>
        /// 去除字符串中的换行
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string removeRN(string str)
        {
            string Str1 = str.Replace("\r\n", "");
            return Str1;
        }
        /// <summary>
        /// 去除字符串中的空格和换行
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string removeSpaceRN(string str)
        {
            string Str1 = str.Replace(" ", "");
            string Str2 = Str1.Replace("\r\n", "");
            return Str2;
        }
        /// <summary>
        /// 画直线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void DrawLine(Point start, Point end)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
            line.X1 = start.X;
            line.Y1 = start.Y;
            line.X2 = end.X;
            line.Y2 = end.Y;
            line.Stroke = new SolidColorBrush(Colors.Red);
            line.Fill = new SolidColorBrush(Colors.Red);
            line.StrokeThickness = 2;
        }

        /// <summary>
        /// 获取笔迹边框的中心点
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Point getStrokeCenterPoint(Stroke s)
        {
            Rect bound = s.GetBounds();
            Point center = new Point(bound.Left + bound.Width / 2, bound.Top + bound.Height / 2);
            return center;
        }
        /// <summary>
        /// 获取多条笔迹边框的中心点
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Point getStrokesCenterPoint(StrokeCollection sc)
        {
            Rect bound = sc.GetBounds();
            Point center = new Point(bound.Left + bound.Width / 2, bound.Top + bound.Height / 2);
            return center;
        }

        /// <summary>
        /// 清空画板
        /// </summary>
        /// <param name="inkFrame"></param>
        public void ClearAllStrokesAndChildren(InkFrame inkFrame)
        {

            inkFrame.InkCollector.InkCanvas.Children.Clear();
            inkFrame.InkCollector.InkCanvas.Strokes.Clear();
            inkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
            inkFrame.InkCollector.Sketch.Images.Clear();
            inkFrame.InkCollector.Sketch.MyStrokes.Clear();
            inkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
            inkFrame.InkCollector.Sketch.MyGraphics.Clear();
            inkFrame.InkCollector.Sketch.GraphicLinkNodes.Clear();
        }
        /// <summary>
        /// 将stroke以点迹的模式擦除，拆分成两个stroke
        /// </summary>
        /// <param name="stroke">被擦除的stroke</param>
        /// <param name="pointErasePoints">擦除的点</param>
        public StrokeCollection ProcessPointErase(Stroke stroke, StylusPoint pointErasePoint)
        {
            StrokeCollection strokes = new StrokeCollection();
            if (stroke.StylusPoints.Count > 1)
            {
                StylusPointCollection splitStrokePoints1 = new StylusPointCollection();
                StylusPointCollection splitStrokePoints2 = new StylusPointCollection();
                int index = 0;
                foreach (StylusPoint sp in stroke.StylusPoints)
                {
                    if ((int)sp.X == (int)pointErasePoint.X && (int)sp.Y == (int)pointErasePoint.Y)
                    {
                        break;
                    }
                    index++;
                }
                for (int i = 0; i <= index-3; i++)
                {
                    splitStrokePoints1.Add(stroke.StylusPoints[i]);
                }

                for (int i = index-3 ; i <= stroke.StylusPoints.Count - 1; i++)
                {
                    splitStrokePoints2.Add(stroke.StylusPoints[i]);
                }
                if (splitStrokePoints1.Count > 1)
                {
                    Stroke splitStroke1 = new Stroke(splitStrokePoints1);
                    splitStroke1.DrawingAttributes = stroke.DrawingAttributes;
                    strokes.Add(splitStroke1);
                }


                if (splitStrokePoints2.Count > 1)
                {
                    Stroke splitStroke2 = new Stroke(splitStrokePoints2);
                    splitStroke2.DrawingAttributes = stroke.DrawingAttributes;
                    strokes.Add(splitStroke2);
                }
            }
            return strokes;   
        }
        /// <summary>
        /// 从整型数组中查找到某个整数在数组中的最后一个位置下标，下标从0开始
        /// </summary>
        /// <param name="search">要查找的整数</param>
        /// <param name="intList">整型数组</param>
        /// <returns></returns>
        public int getLastLocationInIntList(int search, List<int> intList)
        {
            int lastIndex = -1;
            for (int i = intList.Count - 1; i >= 0; i--)
            {
                if (intList[i] == search)
                {
                    return i;
                }
            }
            return lastIndex;
        }

        /// <summary>
        /// 获取圆弧的圆心
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public StylusPoint getArcCenter(Stroke arcStroke)
        {
            StylusPointCollection sps=arcStroke.StylusPoints;
            int pointsCount = sps.Count;
            int midIndex = pointsCount / 2;
            double centerX = 0;
            double centerY = 0;
            if (GraphicMathTool.getInstance().isArc(arcStroke))
            {
                if (GraphicMathTool.getInstance().isCloseStroke(arcStroke))
                {
                    return getCenter(arcStroke.StylusPoints);
                }
                else
                {
                    double k1 = 0;
                    double k2 = 0;
                    if (sps[midIndex].Y != sps[0].Y && sps[pointsCount - 1].Y != sps[midIndex].Y)
                    {
                        k1 = (sps[0].X - sps[midIndex].X) / (sps[midIndex].Y - sps[0].Y);
                        k2 = (sps[midIndex].X - sps[pointsCount - 1].X) / (sps[pointsCount - 1].Y - sps[midIndex].Y);
                        centerX = (sps[pointsCount - 1].Y - sps[0].Y - k2 * (sps[midIndex].X + sps[pointsCount - 1].X)
                            + k1 * (sps[0].X + sps[midIndex].X)) / (k1 - k2) / 2;
                        centerY = k1 * (centerX - (sps[0].X + sps[midIndex].X) / 2) + (sps[0].Y + sps[midIndex].Y) / 2;
                    }
                    else if (sps[midIndex].Y == sps[0].Y)
                    {
                        k2 = (sps[midIndex].X - sps[pointsCount - 1].X) / (sps[pointsCount - 1].Y - sps[midIndex].Y);
                        centerX = (sps[midIndex].X + sps[pointsCount - 1].X - sps[pointsCount - 1].Y + sps[0].Y) / (2 * k2);
                        centerY = (sps[0].Y + sps[midIndex].Y) / 2;
                    }
                    else if (sps[pointsCount - 1].Y == sps[midIndex].Y)
                    {
                        k1 = (sps[0].X - sps[midIndex].X) / (sps[midIndex].Y - sps[0].Y);
                        centerX = (sps[midIndex].X + sps[0].X + sps[pointsCount - 1].Y - sps[0].Y) / (2 * k1);
                        centerY = (sps[pointsCount - 1].Y + sps[midIndex].Y) / 2;
                    }
                }
            }
            StylusPoint pointCenter = new StylusPoint(centerX,centerY);
            return pointCenter;

        }
        /// <summary>
        /// 计算两条平行弧线的垂直距离，即半径差的绝对值
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public double distance2ParallelArc(Stroke s1, Stroke s2)
        {
            StylusPoint centerPoint1 = getArcCenter(s1);
            StylusPoint centerPoint2 = getArcCenter(s2);
            StylusPoint centerPoint=new StylusPoint((centerPoint1.X+centerPoint2.X)/2,(centerPoint1.Y+centerPoint2.Y)/2);
            int midIndex1 = s1.StylusPoints.Count / 2;
            int midIndex2 = s2.StylusPoints.Count / 2;
            double radius1 = distanceP2P(s1.StylusPoints[midIndex1], centerPoint);
            double radius2 = distanceP2P(s2.StylusPoints[midIndex2], centerPoint);
            return Math.Abs(radius1 - radius2);
        }
        /// <summary>
        /// 修改图片大小
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void resizeImage(KeyFrame keyFrame, double width, double height)
        {
            double kx = (width + 5) / keyFrame.Image.Width;//宽度比例
            double ky = (height + 5) / keyFrame.Image.Height;//高度比例
            if (kx < 1 && ky > 1)
            {
                keyFrame.Image.Width *= ky;
                keyFrame.Image.Height *= ky;
            }
            else if (kx > 1 && ky < 1)
            {
                keyFrame.Image.Width *= kx;
                keyFrame.Image.Height *= kx;
            }
            else if (kx < 1 && ky < 1)
            {
                if (kx > ky)
                {
                    keyFrame.Image.Width *= kx;
                    keyFrame.Image.Height *= kx;
                }
                else
                {
                    keyFrame.Image.Width *= ky;
                    keyFrame.Image.Height *= ky;
                }
            }
            else //if (kx > 1 && ky > 1)
            {
                if (kx < ky)
                {
                    keyFrame.Image.Width *= ky;
                    keyFrame.Image.Height *= ky;
                }
                else
                {
                    keyFrame.Image.Width *= kx;
                    keyFrame.Image.Height *= kx;
                }
            }
        }
        public void resizeWidthHeight(ref double  fwidth,ref double  fheight, double twidth, double theight)
        {
            double kx = (twidth + 5) / fwidth;//宽度比例
            double ky = (theight + 5) / fheight;//高度比例
            if (kx < 1 && ky > 1)
            {
                fwidth *= ky;
                fheight *= ky;
            }
            else if (kx > 1 && ky < 1)
            {
                fwidth *= kx;
                fheight *= kx;
            }
            else if (kx < 1 && ky < 1)
            {
                if (kx > ky)
                {
                    fwidth *= kx;
                    fheight *= kx;
                }
                else
                {
                    fwidth *= ky;
                    fheight *= ky;
                }
            }
            else //if (kx > 1 && ky > 1)
            {
                if (kx < ky)
                {
                    fwidth *= ky;
                    fheight *= ky;
                }
                else
                {
                    fwidth *= kx;
                    fheight *= kx;
                }
            }
        } 
        /// 修改图片大小
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void resizeImage(Image image, double width, double height)
        {
            double kx = (width + 5) / image.Width;//宽度比例
            double ky = (height + 5) / image.Height;//高度比例
            if (kx < 1 && ky > 1)
            {
                image.Width *= ky;
                image.Height *= ky;
            }
            else if (kx > 1 && ky < 1)
            {
                image.Width *= kx;
                image.Height *= kx;
            }
            else if (kx < 1 && ky < 1)
            {
                if (kx > ky)
                {
                    image.Width *= kx;
                    image.Height *= kx;
                }
                else
                {
                    image.Width *= ky;
                    image.Height *= ky;
                }
            }
            else //if (kx > 1 && ky > 1)
            {
                if (kx < ky)
                {
                    image.Width *= ky;
                    image.Height *= ky;
                }
                else
                {
                    image.Width *= kx;
                    image.Height *= kx;
                }
            }
        }
        /// <summary>
        /// 修改图片大小
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public List<double> getResizeImageWidthHeight(KeyFrame keyFrame, double width, double height)
        {
            List<double> widthHeight = new List<double>();
            double w = 0;
            double h = 0;
            double kx = (width + 5) / keyFrame.Image.Width;//宽度比例
            double ky = (height + 5) / keyFrame.Image.Height;//高度比例
            if (kx < 1 && ky > 1)
            {
                w=keyFrame.Image.Width * ky;
                h=keyFrame.Image.Height * ky;
            }
            else if (kx > 1 && ky < 1)
            {
                w = keyFrame.Image.Width * kx;
                h = keyFrame.Image.Height * kx;
            }
            else if (kx < 1 && ky < 1)
            {
                if (kx > ky)
                {
                    w = keyFrame.Image.Width * kx;
                    h = keyFrame.Image.Height * kx;
                }
                else
                {
                    w = keyFrame.Image.Width * ky;
                    h = keyFrame.Image.Height * ky;
                }
            }
            else //if (kx > 1 && ky > 1)
            {
                if (kx < ky)
                {
                    w = keyFrame.Image.Width * ky;
                    h = keyFrame.Image.Height * ky;
                }
                else
                {
                    w = keyFrame.Image.Width * kx;
                    h = keyFrame.Image.Height * kx;
                }
            }
            widthHeight.Add(w);
            widthHeight.Add(h);
            return widthHeight;
        }
        /// <summary>
        /// 一些常量
        /// </summary>
        public double THRESHOLD_LINEARITY = 0.95;
        public double THRESHOLD_DISTANCE = 5;
        public double THRESHOLD_ANGULAR = 12 * Math.PI / 180;
    }
}
