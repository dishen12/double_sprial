using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFInk.ShotCut
{
    class MathTool
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
            Point center = new Point((int)(sumx / points.Length), (int)(sumy / points.Length));
            return center;
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
            double x = p.X;
            double y = p.Y;
            return Math.Abs(A * x + B * y + C) / Math.Sqrt(A * A + B * B);
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
        public double distanceP2L(Point p, double A, double B, double C, Point start, Point end)
        {
            double x = p.X;
            double y = p.Y;
            double result;
            Point foot = getFoot(p, A, B, C);
            if ((foot.X - start.X) * (foot.X - end.X) > 0)
                result = Math.Min(distanceP2P(p, start), distanceP2P(p, end));
            else
                result = Math.Abs(A * x + B * y + C) / Math.Sqrt(A * A + B * B);
            return result;
        }

        /// <summary>
        /// 计算点p到直线(start-end)的距离
        /// </summary>
        /// <param name="p"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double distanceP2L(Point p, Point start, Point end)
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

            return dist;
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
            double x = p.X, y = p.Y;
            double x1, y1;
            x1 = (B * B * x - A * B * y - A * C) / (A * A + B * B);
            y1 = (-A * B * x + A * A * y - B * C) / (A * A + B * B);
            return new Point((int)x1, (int)y1);
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
        public double getAngle(Point start, Point end)
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

            return angle;
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
        /// 一些常量
        /// </summary>
        public double THRESHOLD_LINEARITY = 0.95;
        public double THRESHOLD_DISTANCE = 5;
        public double THRESHOLD_ANGULAR = 12 * Math.PI / 180;
    }
}
