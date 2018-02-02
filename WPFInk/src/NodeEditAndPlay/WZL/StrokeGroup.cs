using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Ink;
using System.Text;

namespace WPFInk.WZL
{
    /// <summary>
    /// GroupType 定义StrokeGroup的不同类型
    /// Overlapping    ---多笔重描
    /// Joint             ---多笔连续
    /// Overlooping    ---多笔来回
    /// Hatching        ---影线
    /// </summary>
    public enum GroupType
    {
        Overlapping,
        Joint,
        Overlooping,
        Hatching,
        Mark,
        DEFAULT
    }

    public enum Graphic
    {
        Line,
        Circle,
        Curve,
        DEFAULT
    }

    /// <summary>
    /// 定义StrokeGroup的类，一个StrokeGroup可能由一条或多条Stroke组成
    /// </summary>
    public class StrokeGroup
    {
        /// <summary>
        /// 构造函数中初始化strokegroup的创建时间
        /// </summary>
        /// <param name="type"></param>
        public StrokeGroup(GroupType type)
        {
            this.TYPE = type;
            this.StartTime = DateTime.Now;
            strokeList = new LinkedList<MyStroke>();
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// operations
        /// </summary>
        /// 

        ///<summary>
        /// 在group中增加一个stroke
        /// 将stroke的时间设置为相同 这样通过时间轴回放的时候 会同时绘制group中的笔画
        /// </summary>
        ///
        public void addStroke(MyStroke myStroke)
        {
            strokeList.AddLast(myStroke);
            myStroke.isInGroup = true;
            myStroke.endTime = this.StartTime;
            myStroke.group = this;

            if (myStroke.linearity == Linearity.Curve)
            {
                return;
            }

            ///设置startPoint和endPoint
            Point p1 = myStroke.startPoint;
            Point p2 = myStroke.endPoint;
            if (strokeList.Count == 1)
            {
                if (p1.X < p2.X)
                {
                    startPoint = new Point(p1.X, p1.Y);
                    endPoint = new Point(p2.X, p2.Y);
                }
                else
                {
                    startPoint = new Point(p2.X, p2.Y);
                    endPoint = new Point(p1.X, p1.Y);
                }
            }
            else if (strokeList.Count > 1)
            {
                if (p1.X < p2.X)
                {
                    if (p1.X < startPoint.X)
                    {
                        startPoint.X = p1.X;
                        startPoint.Y = p1.Y;
                    }
                    if (p2.X > endPoint.X)
                    {
                        endPoint.X = p2.X;
                        endPoint.Y = p2.Y;
                    }
                }
                else
                {
                    if (p2.X < startPoint.X)
                    {
                        startPoint.X = p2.X;
                        startPoint.Y = p2.Y;
                    }
                    if (p1.X > endPoint.X)
                    {
                        endPoint.X = p1.X;
                        endPoint.Y = p1.Y;
                    }
                }
            }
            tool.computeSkeleton(this);
            computeSkeletonFromEnds();
        }

        /// <summary>
        /// 从strokegroup中删除stroke
        /// </summary>
        /// <param name="myStroke"></param>
        public void removeStroke(MyStroke myStroke)
        {
            strokeList.Remove(myStroke);
        }


        /// <summary>
        /// 前提条件：已经计算出y=mx+c
        /// </summary>
        /// 
        /*
        public void computeSkeletonFromEnds()
        {
            if (startPoint == null || endPoint == null|| m == 0.12d)
                return;
            
            ToolForStroke t = new ToolForStroke();
            Point s = t.getFoot(startPoint, m, -1, c);
            Point e = t.getFoot(endPoint, m, -1, c);
            //Point s = startPoint;
            //Point e = endPoint;
            List<Point> foots = new List<Point>();
            if(Math.Abs(m)<1)
            {
                for(int x=s.X;x<e.X;x+=20)
                {
                    int y =(int) (x*m+c);
                    Point midPoint = new Point(x,y);
                    foots.Add(midPoint);
                }
            }
            else
            {
                //斜率很大的时候，以y为增量
                if(s.Y<e.Y)
                {
                    for(int y = s.Y;y<e.Y;y+=20)
                    {
                        int x =(int) ((y-c)/m);
                        Point midPoint = new Point(x,y);
                        foots.Add(midPoint);
                    }
                }
                else
                {
                    for(int y = e.Y;y<s.Y;y+=20)
                    {
                        int x =(int) ((y-c)/m);
                        Point midPoint = new Point(x,y);
                        foots.Add(midPoint);
                    }
                }
                
            }

            SkeletonPoints = foots.ToArray();
        }*/

        public void computeSkeletonFromEnds()
        {
            if (startPoint == null || endPoint == null || m == 0.12d)
                return;
            SkeletonPoints = tool.getStrokeFromEnds(startPoint, endPoint);
        }

        //YHY-090411
        public void setBoundingBox()
        {
            int topLeft_x = 1000000;
            int topLeft_y = 1000000;
            int bottomRight_x = -1000000;
            int bottomRight_y = -1000000;

            foreach (MyStroke stroke in strokeList)
            {
                if (stroke.boundingBox.Left < topLeft_x)
                    topLeft_x = stroke.boundingBox.Left;
                if (stroke.boundingBox.Top < topLeft_y)
                    topLeft_y = stroke.boundingBox.Top;
                if (stroke.boundingBox.Right > bottomRight_x)
                    bottomRight_x = stroke.boundingBox.Right;
                if (stroke.boundingBox.Bottom > bottomRight_y)
                    bottomRight_y = stroke.boundingBox.Bottom;
            }

            boundingBox = new Rectangle(topLeft_x, topLeft_y, System.Math.Abs(bottomRight_x - topLeft_x), System.Math.Abs(topLeft_y - bottomRight_y));

        }

        public Rectangle getBoundingBox()
        {
            return boundingBox;
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// attributes
        /// </summary>
        public GroupType TYPE = 0;
        public DateTime StartTime;
        public LinkedList<MyStroke> strokeList = null;
        public List<StrokeGroup> groupList = null;
        public Point[] SkeletonPoints = null;
        public Stroke Skeleton = null;
        public Point startPoint, endPoint;
        public double m = 0.12, c = 0.12;

        //YHY-090408
        public Graphic GRAPH = 0; //skeleton的类型，即基本图元：目前支持 Line|Circle

        //保存group的包围盒，用于距离计算
        public Rectangle boundingBox = new Rectangle();
        public int groupID;


        //YHY-090415
        //暂时令这里的strokeGroup为语义上的单位元素，即，一个strokegroup虽然可能有多个stroke组成，
        //但是一个strokeGroup代表一个基本图元，目前只支持Line 和circle
        public G_Geometry geometry;

        private ToolForStroke tool = new ToolForStroke();///用于计算
    }
}
