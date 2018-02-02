using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFInk.WZL
{
    /// <summary>
    /// stroketype定义了stroke的类型
    /// starMark --- 五角星
    /// checkmark --- 勾号
    /// </summary>
    public enum StrokeType
    {
        Single,
        Skeleton,
        Addtional,
        StarMark,
        CheckMark
    }

    //判断线性与否
    public enum Linearity
    {
        Line,
        Curve
    }
    public class MyStroke
    {
        //constructor
        public MyStroke(Microsoft.Ink.Stroke stroke)
        {
            this.endTime = DateTime.Now;//set time to the current time
            this.points = stroke.GetPoints();//record the points of stroke
            this.startPoint = stroke.GetPoint(0);
            this.endPoint = stroke.GetPoint(stroke.GetPoints().Length - 1);
            this.direction = new Point(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            this.slope = direction.Y / direction.X;///if direction.X = 0 , ... 
            this.inflexions = null; //////////////////////////////////////////////////////////////////////////
            this.DrawingAttributes = stroke.DrawingAttributes;//record the drawingAttributes
            this.inkstroke = stroke;
        }

        //////////////////////////////////////////////////////////////////////////
        ///operations
        ///
        //////////////////////////////////////////////////////////////////////////

        //YHY-090411
        public void setBoundingBox()
        {
            int topLeft_x = 1000000;
            int topLeft_y = 1000000;
            int bottomRight_x = -1000000;
            int bottomRight_y = -1000000;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X < topLeft_x)
                    topLeft_x = points[i].X;
                if (points[i].Y < topLeft_y)
                    topLeft_y = points[i].Y;
                if (points[i].X > bottomRight_x)
                    bottomRight_x = points[i].X;
                if (points[i].Y > bottomRight_y)
                    bottomRight_y = points[i].Y;
            }
            boundingBox = new Rectangle(topLeft_x, topLeft_y, System.Math.Abs(bottomRight_x - topLeft_x), System.Math.Abs(bottomRight_y - topLeft_y));

        }

        public Rectangle getBoundingBox()
        {
            return boundingBox;
        }

        public void setLenth()
        {
            Point p1 = startPoint;
            lenth = 0;
            for (int i = 1; i < points.Length; i++)
            {
                lenth += new ToolForStroke().distanceP2P(points[i - 1], points[i]);
            }
        }


        /// <summary>
        /// attributes
        /// </summary>
        public DateTime startTime;            //stroke开始绘制的时间
        public DateTime endTime;              //stroke建立时间
        public Point[] points;                    //stroke上的点坐标数组 
        public Point[] adjustedPoints;         //stroke所对应的直线的points
        public Point startPoint;                  //起始点坐标
        public Point endPoint;                    //终点坐标
        public Point direction;                    //stroke的方向向量（Point.x,Point.y)
        public double slope;                      //stroke的斜率
        public double m, c;                       //直线方程的两个参数
        public double velocity;                   //绘制的速度。。。点
        public StrokeType stroketype;         //skeleton or additional
        public Linearity linearity;                //直线还是曲线
        //笔锋
        public LinkedList<Point> inflexions;   //拐点构成的双向链表，其中第一个点为起点，最后一个点为终点
        //geometry , skeleton , style 
        public Microsoft.Ink.DrawingAttributes DrawingAttributes;//stroke的属性
        public Microsoft.Ink.Stroke inkstroke = null;                   //对应的ink中的stroke
        public bool isInGroup = false;
        public StrokeGroup group = null;

        //YHY-090411
        //stroke的包围盒
        public Rectangle boundingBox;
        public double lenth;
    }
}
