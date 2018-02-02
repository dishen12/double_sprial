using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPFInk.ink;
using WPFInk.tool;

namespace WPFInk.video
{
    class Connector
    {
        private static Connector connector;
        //定义弧的跨度
        //int DIS_SEP = 80;

        //定义弧的高度
        //int H = 50;
        private Point startPoint;
        private Point endPoint;

        public Point EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        public Point StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }



        public Connector()
        {

        }

        public static Connector getInstance()
        {
            if (connector == null)
                connector = new Connector();
            return connector;
        }

        public Path getThumbConnector(MyButton myButton1, MyButton myButton2)
        {
            int arrowHeadWidth = 20;
            int arrowHeadAngle = 15;
            StylusPoint center1 = MathTool.getInstance().getMyButtonCenter(myButton1);
            StylusPoint center2 = MathTool.getInstance().getMyButtonCenter(myButton2);
            Point LeftPoint1 = new Point((int)(center1.X - myButton1.Width / 2), (int)center1.Y);
            Point TopPoint1 = new Point((int)center1.X, (int)(center1.Y - myButton1.Height / 2));
            Point RightPoint1 = new Point((int)(center1.X + myButton1.Width / 2), (int)center1.Y);
            Point BottomPoint1 = new Point((int)center1.X, (int)(center1.Y + myButton1.Height / 2));

            Point LeftPoint2 = new Point((int)(center2.X - myButton2.Width / 2), (int)center2.Y);
            Point TopPoint2 = new Point((int)center2.X, (int)(center2.Y - myButton2.Height / 2));
            Point RightPoint2 = new Point((int)(center2.X + myButton2.Width / 2), (int)center2.Y);
            Point BottomPoint2 = new Point((int)center2.X, (int)(center2.Y + myButton2.Height / 2));
            double angle = MathTool.getInstance().getAngleP2P(center1, center2);
            double angleStartEnd=0;
            Path arrow = new Path();
            GeometryGroup lineGroup = new GeometryGroup();
            LineGeometry line = new LineGeometry();
            LineGeometry line2 = new LineGeometry();
            LineGeometry line3 = new LineGeometry();
			
            if ((angle >=0 && angle < 45) || (angle >= 315 && angle <= 360))
            {
                line=new LineGeometry(RightPoint1, LeftPoint2);
                startPoint = RightPoint1;
                endPoint = LeftPoint2;
                angleStartEnd = MathTool.getInstance().getAngleP2P(new StylusPoint(RightPoint1.X, RightPoint1.Y), new StylusPoint(LeftPoint2.X, LeftPoint2.Y));
                
                Point point = getMiddlePoint(angleStartEnd, LeftPoint2, arrowHeadWidth);
                line2 = new LineGeometry(LeftPoint2, point);
                RotateTransform xform2 = new RotateTransform();
                xform2.CenterX= LeftPoint2.X;
                xform2.CenterY=LeftPoint2.Y;
                xform2.Angle = arrowHeadAngle;
                line2.Transform = xform2;

                line3 = new LineGeometry(LeftPoint2, point);
                RotateTransform xform3 = new RotateTransform();
                xform3.CenterX = LeftPoint2.X;
                xform3.CenterY = LeftPoint2.Y;
                xform3.Angle = -arrowHeadAngle;
                line3.Transform = xform3;
            }
            if (angle >= 45 && angle < 135)
            {
                line = new LineGeometry(TopPoint1, BottomPoint2);
                startPoint = TopPoint1;
                endPoint = BottomPoint2;
                angleStartEnd = MathTool.getInstance().getAngleP2P(new StylusPoint(TopPoint1.X, TopPoint1.Y), new StylusPoint(BottomPoint2.X, BottomPoint2.Y));
                
                Point point = getMiddlePoint(angleStartEnd, BottomPoint2, arrowHeadWidth);
                line2 = new LineGeometry(BottomPoint2, point);
                RotateTransform xform2 = new RotateTransform();
                xform2.CenterX = BottomPoint2.X;
                xform2.CenterY = BottomPoint2.Y;
                xform2.Angle = arrowHeadAngle;
                line2.Transform = xform2;

                line3 = new LineGeometry(BottomPoint2, point);
                RotateTransform xform3 = new RotateTransform();
                xform3.CenterX = BottomPoint2.X;
                xform3.CenterY = BottomPoint2.Y;
                xform3.Angle = -arrowHeadAngle;
                line3.Transform = xform3;
            }
            if (angle >= 135 && angle < 225)
            {
                line = new LineGeometry(LeftPoint1, RightPoint2);
                startPoint = LeftPoint1;
                endPoint = RightPoint2;
                angleStartEnd = MathTool.getInstance().getAngleP2P(new StylusPoint(LeftPoint1.X, LeftPoint1.Y), new StylusPoint(RightPoint2.X, RightPoint2.Y));
                
                Point point = getMiddlePoint(angleStartEnd, RightPoint2, arrowHeadWidth);
                line2 = new LineGeometry(RightPoint2, point);
                RotateTransform xform2 = new RotateTransform();
                xform2.CenterX = RightPoint2.X;
                xform2.CenterY = RightPoint2.Y;
                xform2.Angle = arrowHeadAngle;
                line2.Transform = xform2;

                line3 = new LineGeometry(RightPoint2, point);
                RotateTransform xform3 = new RotateTransform();
                xform3.CenterX = RightPoint2.X;
                xform3.CenterY = RightPoint2.Y;
                xform3.Angle = -arrowHeadAngle;
                line3.Transform = xform3;
            }
            if (angle >=225&& angle < 315)
            {

                line = new LineGeometry(BottomPoint1, TopPoint2);
                startPoint = BottomPoint1;
                endPoint = TopPoint2;
                angleStartEnd = MathTool.getInstance().getAngleP2P(new StylusPoint(BottomPoint1.X, BottomPoint1.Y), new StylusPoint(TopPoint2.X, TopPoint2.Y));
                
                Point point = getMiddlePoint(angleStartEnd, TopPoint2, arrowHeadWidth);
                line2 = new LineGeometry(TopPoint2, point);
                RotateTransform xform2 = new RotateTransform();
                xform2.CenterX = TopPoint2.X;
                xform2.CenterY = TopPoint2.Y;
                xform2.Angle = arrowHeadAngle;
                line2.Transform = xform2;

                line3 = new LineGeometry(TopPoint2, point);
                RotateTransform xform3 = new RotateTransform();
                xform3.CenterX = TopPoint2.X;
                xform3.CenterY = TopPoint2.Y;
                xform3.Angle = -arrowHeadAngle;
                line3.Transform = xform3;
            }
            lineGroup.Children.Add(line);
            lineGroup.Children.Add(line2);
            lineGroup.Children.Add(line3);
            
            arrow.Stroke = System.Windows.Media.Brushes.Black;
            arrow.Fill = System.Windows.Media.Brushes.MediumSlateBlue;
            arrow.StrokeThickness = 2;
            arrow.HorizontalAlignment = HorizontalAlignment.Left;
            arrow.VerticalAlignment = VerticalAlignment.Center;
            arrow.Data = lineGroup;
            return arrow;
        }

        

        //获取距离某点point为lineLength的位置，angle是角度
        private Point getMiddlePoint(double angle, Point point, double lineLength)
        {
            Point pointResult = new Point();
            pointResult.X = point.X - lineLength * Math.Cos(angle * Math.PI / 180);
            pointResult.Y = point.Y + lineLength * Math.Sin(angle * Math.PI / 180);
            return pointResult;

        }
    }
}
