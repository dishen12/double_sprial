using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WPFInk.graphic;
using System.Windows.Ink;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;
using WPFInk.ink;
using WPFInk.cmd;


namespace WPFInk.tool
{
	public class GraphicMathTool
	{
		private static GraphicMathTool Singleton = null;
        #region 常量
        /// <summary>
        /// 箭头和图形相连的外边界阈值,默认值为10
        /// </summary>
        private const int MARGINDISTANCE = 10;
        /// <summary>
        /// 消除伪折点的阈值,默认值为20
        /// </summary>
        private const int removeFalsePolyPointDistance = 20;
        /// <summary>
        /// //是否是直线的长度比例阈值,默认值为0.9
        /// </summary>
        private const double isLineLengthThreshold = 0.9;
        /// <summary>
        /// //是否是直线的落在直线周围点的比例阈值,默认值为0.9
        /// </summary>
        private const double isLinePointsThreshold = 0.9;
        /// <summary>
        /// 是否是直线的落在直线周围点的比例阈值,默认值为20
        /// </summary>
        private const double distanceP2LThreshold = 15;//
        /// <summary>
        /// 是否闭合的角度阈值,默认值为Math.PI / 6
        /// </summary>
        private const double isCloseAngleThreshold = Math.PI / 6;//
        /// <summary>
        /// 是否闭合的直线阈值,默认值为0.2
        /// </summary>
        private const double isCloseLineThreshold = 0.2;//
        /// <summary>
        /// 落在椭圆或圆弧范围内的点的比例预置,默认值为0.9
        /// </summary>
        private const double isEllipsePointsThershold = 0.9;//
        /// <summary>
        /// 落在圆弧范围外的点的比例预置,默认值为0.7
        /// </summary>
        private const double isArcPointsThershold = 0.7;//
        /// <summary>
        /// 鼠标方向扇区,默认值为8
        /// </summary>
        public const int DEFAULT_SECTORS = 8;
        /// <summary>
        /// 两条直线平行角度偏差，值为20
        /// </summary>
        private const double parallelThreshold = 20;

        /// <summary>
        /// 两条直线垂直角度偏差，值为20
        /// </summary>
        private const double verticalThreshold = 10;
        /// <summary>
        /// 矩形四边约束长度阈值，值为20
        /// </summary>
        private const double rectangleDistanceThreshold = 40;
        /// <summary>
        /// 箭头头部夹角正弦阈值，值为20
        /// </summary>
        private const double arrowAngleThreshold = 20;
        /// <summary>
        /// 根据密度获取折点时，圆形的半径，默认为20
        /// </summary>
        private const double polyPointRadiusThreshold = 3;
        #endregion

        #region 构造函数及实例化函数
        /// <summary>
        /// 不可以从外部调用构造函数
        /// </summary>
		private GraphicMathTool()
        {

        }
		/// <summary>
		/// 实例化
		/// </summary>
		/// <returns></returns>
		public static GraphicMathTool getInstance()
        {
            if (Singleton == null)
				Singleton = new GraphicMathTool();
            return Singleton;
        }
        #endregion

        #region 图形关系函数
        /// <summary>
		/// 计算两个Graphic之间的关系
		/// </summary>
		/// <param name="myGraphic1">Graphic1</param>
		/// <param name="myGraphic2">Graphic2</param>
		/// <returns></returns>
		public GraphicRule GraphicAndGraphic(MyGraphic myGraphic1, MyGraphic myGraphic2,InkCollector _inkCollector)
		{
			string myGraphicType1 = myGraphic1.ShapeType;
            string myGraphicType2 = myGraphic2.ShapeType;
            GraphicRule rule = GraphicRule.None;
			switch (myGraphicType1 +","+ myGraphicType2)
			{
                case "arrow,ellipse"://直线与椭圆
                case "polylineArrow,ellipse"://直线与椭圆
                case "loopArc,ellipse"://循环弧线与椭圆
                    rule = LineAndEllipse((System.Windows.Shapes.Line)myGraphic1.Shape, (Ellipse)myGraphic2.Shape);
                    addRelationsHeadTail(myGraphic1, myGraphic2, _inkCollector, rule);
					break;
                case "ellipse,arrow"://直线与椭圆
                case "ellipse,polylineArrow"://直线与椭圆
                case "ellipse,loopArc"://循环弧线与椭圆
                    rule = LineAndEllipse((System.Windows.Shapes.Line)myGraphic2.Shape, (Ellipse)myGraphic1.Shape);
                    addRelationsHeadTail(myGraphic1, myGraphic2, _inkCollector, rule);
					break;
				case "arrow,rectangle"://直线与矩形
                case "polylineArrow,rectangle"://直线与矩形
                case "loopArc,rectangle"://循环弧线与矩形
                    rule = LineAndRectangle((System.Windows.Shapes.Line)myGraphic1.Shape, (Rectangle)myGraphic2.Shape);
                    addRelationsHeadTail(myGraphic1, myGraphic2, _inkCollector, rule);
                    break;
                case "rectangle,arrow"://直线与矩形
                case "rectangle,polylineArrow"://直线与矩形
                case "rectangle,loopArc"://循环弧线与矩形
                    rule = LineAndRectangle((System.Windows.Shapes.Line)myGraphic2.Shape, (Rectangle)myGraphic1.Shape);
                    addRelationsHeadTail(myGraphic1, myGraphic2, _inkCollector, rule);
                    break;
                case "loopArcSelf,ellipse"://自循环弧线与椭圆
                    rule = LoopSelfAndEllipse((System.Windows.Shapes.Line)myGraphic1.Shape, (Ellipse)myGraphic2.Shape);
                    addRelationsHeadTail(myGraphic1, myGraphic2, _inkCollector, rule);
                    break;
                case "loopArcSelf,rectangle"://自循环弧线与矩形
                    rule = LoopSelfAndRectangle((System.Windows.Shapes.Line)myGraphic1.Shape, (Rectangle)myGraphic2.Shape);
                    addRelationsHeadTail(myGraphic1, myGraphic2, _inkCollector, rule);
                    break;
                case "ellipse,loopArcSelf"://自循环弧线与椭圆
                    rule = LoopSelfAndEllipse((System.Windows.Shapes.Line)myGraphic2.Shape, (Ellipse)myGraphic1.Shape);
                    addRelationsHeadTail(myGraphic1, myGraphic2, _inkCollector, rule);
                    break;
                case "rectangle,loopArcSelf"://自循环弧线与矩形
                    rule = LoopSelfAndRectangle((System.Windows.Shapes.Line)myGraphic2.Shape, (Rectangle)myGraphic1.Shape);
                    addRelationsHeadTail(myGraphic1, myGraphic2, _inkCollector, rule);
                    break;
                case "rectangle,rectangle"://矩形与矩形
                    rule = RectangleAndRectangle((Rectangle)myGraphic2.Shape, (System.Windows.Shapes.Rectangle)myGraphic1.Shape);
                    if (rule == GraphicRule.HeadIntersect)
                    {
                        AddRelation(myGraphic1, myGraphic2, "HeadIntersect", _inkCollector);
                        AddRelation(myGraphic2, myGraphic1, "HeadIntersect", _inkCollector);
                    }            
                    break;
			}
            if (rule != GraphicRule.None)
            {
                setStrokesColor(myGraphic1, Colors.Red);
                setStrokesColor(myGraphic2, Colors.Red);
                
            }
            return rule;
		}
        /// <summary>
        /// 添加两个图形之间的关系
        /// </summary>
        /// <param name="myGraphic1"></param>
        /// <param name="myGraphic2"></param>
        /// <param name="_inkCollector"></param>
        /// <param name="rule"></param>
        private void addRelationsHeadTail(MyGraphic myGraphic1, MyGraphic myGraphic2, InkCollector _inkCollector, GraphicRule rule)
        {
            if (rule == GraphicRule.HeadIntersect)
            {
                AddRelation(myGraphic2, myGraphic1, "HeadIntersect", _inkCollector);
                AddRelation(myGraphic1, myGraphic2, "HeadIntersect", _inkCollector);
            }
            else if (rule == GraphicRule.TailIntersect)
            {
                AddRelation(myGraphic2, myGraphic1, "TailIntersect", _inkCollector);
                AddRelation(myGraphic1, myGraphic2, "TailIntersect", _inkCollector);
            }
            else if (rule == GraphicRule.LoopSelf)
            {
                AddRelation(myGraphic2, myGraphic1, "LoopSelf", _inkCollector);
                AddRelation(myGraphic1, myGraphic2, "LoopSelf", _inkCollector);
            }
        }

        private static void setStrokesColor(MyGraphic myGraphic,Color c)
        {
            foreach (Stroke s in myGraphic.Strokes)
            {
                s.DrawingAttributes.Color = c;
            }
        }
		/// <summary>
		/// 计算直线与圆形的关系
		/// </summary>
		/// <param name="circleCenter">圆心</param>
		/// <param name="radius">圆的半径</param>
		/// <param name="linePoint1">直线上点1</param>
		/// <param name="linePoint2">直线上点2</param>
		/// <returns></returns>
		public GraphicRule LineAndCircle(Point circleCenter,double radius, Point linePoint1, Point linePoint2)
		{
			double distanceCenterToLine=distancePointToLine(circleCenter,linePoint1,linePoint2);
			if(distanceCenterToLine==radius)
			{
				return GraphicRule.Tangent;
			}
			else if (distanceCenterToLine > radius)
			{
				return GraphicRule.Intersect;
			}
			else
			{
				return GraphicRule.Deviation;
			}
		}
		/// <summary>
		/// 判断直线和椭圆是否相交
		/// </summary>
		/// <param name="ellipse">椭圆</param>
		/// <param name="linePoint1">直线上点1</param>
		/// <param name="linePoint2">直线上点2</param>
		/// <returns></returns>
        public GraphicRule LineAndEllipse(System.Windows.Shapes.Line line, System.Windows.Shapes.Ellipse ellipse)
		{
			Point ellipseCenter = new Point(ellipse.Margin.Left + ellipse.Width / 2, ellipse.Margin.Top + ellipse.Height / 2);
			Point linePoint1 = new Point(line.X1 + line.Margin.Left, line.Y1 + line.Margin.Top);
			Point linePoint2 = new Point(line.X2 + line.Margin.Left, line.Y2 + line.Margin.Top);
			double distanceCenterToLinePoint1 = MathTool.getInstance().distanceP2P(ellipseCenter, linePoint1);
			double distanceCenterToLinePoint2 = MathTool.getInstance().distanceP2P(ellipseCenter, linePoint2);
			if (distanceCenterToLinePoint1 < distanceCenterToLinePoint2)
			{
                if (linePoint1.X >= ellipse.Margin.Left - MARGINDISTANCE && linePoint1.X <= ellipse.Margin.Left + ellipse.Width + MARGINDISTANCE && linePoint1.Y >= ellipse.Margin.Top - MARGINDISTANCE && linePoint1.Y <= ellipse.Margin.Top + ellipse.Height + MARGINDISTANCE)
				{
                    return GraphicRule.HeadIntersect;
				}
			}
			else
			{
                if (linePoint2.X >= ellipse.Margin.Left - MARGINDISTANCE && linePoint2.X <= ellipse.Margin.Left + ellipse.Width + MARGINDISTANCE && linePoint2.Y >= ellipse.Margin.Top - MARGINDISTANCE && linePoint2.Y <= ellipse.Margin.Top + ellipse.Height + MARGINDISTANCE)
				{
                    return GraphicRule.TailIntersect;
				}
			}
            return GraphicRule.None;
		}
        /// <summary>
        /// 判断自循环与Ellipse的关系
        /// </summary>
        /// <param name="line"></param>
        /// <param name="ellipse"></param>
        /// <returns></returns>
        public GraphicRule LoopSelfAndEllipse(System.Windows.Shapes.Line line, System.Windows.Shapes.Ellipse ellipse)
		{
			Point ellipseCenter = new Point(ellipse.Margin.Left + ellipse.Width / 2, ellipse.Margin.Top + ellipse.Height / 2);
			Point linePoint1 = new Point(line.X1 + line.Margin.Left, line.Y1 + line.Margin.Top);
			Point linePoint2 = new Point(line.X2 + line.Margin.Left, line.Y2 + line.Margin.Top);
			double distanceCenterToLinePoint1 = MathTool.getInstance().distanceP2P(ellipseCenter, linePoint1);
			double distanceCenterToLinePoint2 = MathTool.getInstance().distanceP2P(ellipseCenter, linePoint2);
			if (distanceCenterToLinePoint1 < distanceCenterToLinePoint2)
			{
                if (linePoint1.X >= ellipse.Margin.Left - 2 * MARGINDISTANCE && linePoint1.X <= ellipse.Margin.Left + ellipse.Width + 2 * MARGINDISTANCE && linePoint1.Y >= ellipse.Margin.Top - 2 * MARGINDISTANCE && linePoint1.Y <= ellipse.Margin.Top + ellipse.Height + 2 * MARGINDISTANCE)
				{
                    return GraphicRule.LoopSelf;
				}
			}
			else
			{
                if (linePoint2.X >= ellipse.Margin.Left - 2 * MARGINDISTANCE && linePoint2.X <= ellipse.Margin.Left + ellipse.Width + 2 * MARGINDISTANCE && linePoint2.Y >= ellipse.Margin.Top - 2 * MARGINDISTANCE && linePoint2.Y <= ellipse.Margin.Top + ellipse.Height + 2 * MARGINDISTANCE)
				{
                    return GraphicRule.LoopSelf;
				}
			}
            return GraphicRule.None;
		}
        /// <summary>
        /// 判断自循环与矩形的关系
        /// </summary>
        /// <param name="line"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        private GraphicRule LoopSelfAndRectangle(System.Windows.Shapes.Line line, System.Windows.Shapes.Rectangle rectangle)
        {
            Point rectangleCenter = new Point(rectangle.Margin.Left + rectangle.Width / 2, rectangle.Margin.Top + rectangle.Height / 2);
            Point linePoint1 = new Point(line.X1 + line.Margin.Left, line.Y1 + line.Margin.Top);
            Point linePoint2 = new Point(line.X2 + line.Margin.Left, line.Y2 + line.Margin.Top);
            double distanceCenterToLinePoint1 = MathTool.getInstance().distanceP2P(rectangleCenter, linePoint1);
            double distanceCenterToLinePoint2 = MathTool.getInstance().distanceP2P(rectangleCenter, linePoint2);
            if (distanceCenterToLinePoint1 < distanceCenterToLinePoint2)
            {
                if (MathTool.getInstance().isCloseRectangle(linePoint1, rectangle, 2*MARGINDISTANCE))// linePoint1.X >= rectangle.Margin.Left && linePoint1.X <= rectangle.Margin.Left + rectangle.Width && linePoint1.Y >= rectangle.Margin.Top && linePoint1.Y <= rectangle.Margin.Top + rectangle.Height)
                {
                    return GraphicRule.LoopSelf;
                }
            }
            else
            {
                if (MathTool.getInstance().isCloseRectangle(linePoint2, rectangle, 2*MARGINDISTANCE))//linePoint2.X >= rectangle.Margin.Left && linePoint2.X <= rectangle.Margin.Left + rectangle.Width && linePoint2.Y >= rectangle.Margin.Top && linePoint2.Y <= rectangle.Margin.Top + rectangle.Height)
                {
                    return GraphicRule.LoopSelf;
                }
            }
            return GraphicRule.None;
        }
		/// <summary>
		/// 判断直线和矩形是否相交
		/// </summary>
		/// <param name="rectangle">矩形</param>
		/// <param name="linePoint1">直线上点1</param>
		/// <param name="linePoint2">直线上点2</param>
		/// <returns></returns>
        private GraphicRule LineAndRectangle(System.Windows.Shapes.Line line, System.Windows.Shapes.Rectangle rectangle)
		{
			Point rectangleCenter = new Point(rectangle.Margin.Left + rectangle.Width / 2, rectangle.Margin.Top + rectangle.Height / 2);
			Point linePoint1 = new Point(line.X1 + line.Margin.Left, line.Y1 + line.Margin.Top);
			Point linePoint2 = new Point(line.X2 + line.Margin.Left, line.Y2 + line.Margin.Top);
			double distanceCenterToLinePoint1 = MathTool.getInstance().distanceP2P(rectangleCenter, linePoint1);
			double distanceCenterToLinePoint2 = MathTool.getInstance().distanceP2P(rectangleCenter, linePoint2);
			if (distanceCenterToLinePoint1 < distanceCenterToLinePoint2)
			{
                if (MathTool.getInstance().isCloseRectangle(linePoint1, rectangle, MARGINDISTANCE))// linePoint1.X >= rectangle.Margin.Left && linePoint1.X <= rectangle.Margin.Left + rectangle.Width && linePoint1.Y >= rectangle.Margin.Top && linePoint1.Y <= rectangle.Margin.Top + rectangle.Height)
				{
                    return GraphicRule.HeadIntersect;
				}
			}
			else
			{
                if (MathTool.getInstance().isCloseRectangle(linePoint2, rectangle, MARGINDISTANCE))//linePoint2.X >= rectangle.Margin.Left && linePoint2.X <= rectangle.Margin.Left + rectangle.Width && linePoint2.Y >= rectangle.Margin.Top && linePoint2.Y <= rectangle.Margin.Top + rectangle.Height)
				{
                    return GraphicRule.TailIntersect;
				}
            }
            return GraphicRule.None;
		}
        /// <summary>
        /// 判断直线和菱形是否相交
        /// </summary>
        /// <param name="line">直线</param>
        /// <param name="rhombus">菱形</param>
        /// <returns></returns>
        private GraphicRule LineAndRhombus(System.Windows.Shapes.Line line, System.Windows.Shapes.Polygon rhombus)
        {
            Point rhombusCenter = new Point((rhombus.Points[0].X + rhombus.Points[2].X) / 2, (rhombus.Points[1].Y + rhombus.Points[3].Y) / 2);
            Point linePoint1 = new Point(line.X1 + line.Margin.Left, line.Y1 + line.Margin.Top);
            Point linePoint2 = new Point(line.X2 + line.Margin.Left, line.Y2 + line.Margin.Top);
            double distanceCenterToLinePoint1 = MathTool.getInstance().distanceP2P(rhombusCenter, linePoint1);
            double distanceCenterToLinePoint2 = MathTool.getInstance().distanceP2P(rhombusCenter, linePoint2);
            if (distanceCenterToLinePoint1 < distanceCenterToLinePoint2)
            {
                foreach (Point p in rhombus.Points)
                {
                    double d=MathTool.getInstance().distanceP2P(new Point(p.X+rhombus.Margin.Left,p.Y+rhombus.Margin.Top), linePoint1);
                    if ( d<= 20)
                    {
                        return GraphicRule.HeadIntersect;
                    }
                }
            }
            else
            {
                foreach (Point p in rhombus.Points)
                {
                    double d = MathTool.getInstance().distanceP2P(new Point(p.X + rhombus.Margin.Left, p.Y + rhombus.Margin.Top), linePoint2);
                    if (d <= 20)
                    {
                        return GraphicRule.TailIntersect;
                    }
                }
            }
            return GraphicRule.None;
        }
        /// <summary>
        /// 判断菱形和三边矩形的关系
        /// </summary>
        /// <param name="rhombus"></param>
        /// <param name="tRectangle"></param>
        /// <returns></returns>
        private GraphicRule RhombusAndTRectangle(System.Windows.Shapes.Polygon rhombus, System.Windows.Shapes.Polyline tRectangle)
        {
            Point p0 = new Point(tRectangle.Points[0].X+tRectangle.Margin.Left,tRectangle.Points[0].Y+tRectangle.Margin.Top);
            double rhombusLeft = rhombus.Margin.Left;
            double rhombusTop = rhombus.Margin.Top;
            double rhombusWidth = rhombus.Points[0].X + rhombus.Points[2].X - 2 * Math.Min(rhombus.Points[0].X < rhombus.Points[1].X ? rhombus.Points[0].X : rhombus.Points[1].X, rhombus.Points[2].X);
            double rhombusHeight = rhombus.Points[0].Y + rhombus.Points[2].Y - 2 * Math.Min(rhombus.Points[0].Y < rhombus.Points[1].Y ? rhombus.Points[0].Y : rhombus.Points[1].Y, rhombus.Points[2].Y);
            foreach (Point p in rhombus.Points)
            {
                double d = MathTool.getInstance().distanceP2P(new Point(p.X + rhombus.Margin.Left, p.Y + rhombus.Margin.Top), p0);
                if (d <= 20)
                {
                    return GraphicRule.HeadIntersect;
                }
            }
            return GraphicRule.None;
        }
        /// <summary>
        /// 判断三边矩形和椭圆是否相交
        /// </summary>
        /// <param name="ellipse"></param>
        /// <param name="tRectangle"></param>
        /// <returns></returns>
        private GraphicRule EllipseAndTRectangle(System.Windows.Shapes.Ellipse ellipse, System.Windows.Shapes.Polyline tRectangle)
        {
            if (tRectangle.Points[3].X >= ellipse.Margin.Left && tRectangle.Points[3].X <= ellipse.Margin.Left + ellipse.Width && tRectangle.Points[3].Y >= ellipse.Margin.Top && tRectangle.Points[3].Y <= ellipse.Margin.Top + ellipse.Height)
            {
                return GraphicRule.TailIntersect;
            }
            return GraphicRule.None;
        }
        /// <summary>
        /// 判断三边矩形和矩形是否相交
        /// </summary>
        /// <param name="rectagnle"></param>
        /// <param name="tRectangle"></param>
        /// <returns></returns>
        private GraphicRule RectangleAndTRectangle(System.Windows.Shapes.Rectangle rectagnle, System.Windows.Shapes.Polyline tRectangle)
        {
            if (tRectangle.Points[3].X >= rectagnle.Margin.Left && tRectangle.Points[3].X <= rectagnle.Margin.Left + rectagnle.Width && tRectangle.Points[3].Y >= rectagnle.Margin.Top && tRectangle.Points[3].Y <= rectagnle.Margin.Top + rectagnle.Height)
            {
                return GraphicRule.TailIntersect;
            }
            return GraphicRule.None;
        }
		/// <summary>
        /// 计算两个椭圆的关系
		/// </summary>
		/// <param name="ellipse1"></param>
		/// <param name="ellipse2"></param>
		/// <returns></returns>
        public GraphicRule CircleAndCircle(System.Windows.Shapes.Ellipse ellipse1, System.Windows.Shapes.Ellipse ellipse2)
        {            
            Rect rect1 = new Rect(ellipse1.Margin.Left, ellipse1.Margin.Top, ellipse1.Width, ellipse1.Height);
            Rect rect2 = new Rect(ellipse2.Margin.Left, ellipse2.Margin.Top, ellipse1.Width, ellipse2.Height);
            if (MathTool.getInstance().isHitRects(rect1, rect2))
            {
                return GraphicRule.Intersect;
            }
            else
            {
                return GraphicRule.None;
            }
		}
        /// <summary>
        /// 判断两个矩形是否有关
        /// </summary>
        /// <param name="rectangle1"></param>
        /// <param name="rectangle2"></param>
        /// <returns></returns>
        public GraphicRule RectangleAndRectangle(System.Windows.Shapes.Rectangle rectangle1, System.Windows.Shapes.Rectangle rectangle2)
        {
            Rect rect1 = new Rect(rectangle1.Margin.Left, rectangle1.Margin.Top, rectangle1.Width, rectangle1.Height);
            Rect rect2 = new Rect(rectangle2.Margin.Left, rectangle2.Margin.Top, rectangle2.Width, rectangle2.Height);
            if (rect1.Top * rect1.Top + rect1.Left * rect1.Left > rect2.Top * rect2.Top + rect2.Left * rect2.Left)
            {
                RectangleAndRectangle(rectangle2, rectangle1);
            }
            double verticalDistance = MathTool.getInstance().verticalDistanceR2R(rect1, rect2);
            double horizontalDistance = MathTool.getInstance().horizontalDistanceR2R(rect1, rect2);
            if (verticalDistance <= 30 && verticalDistance > 0&&horizontalDistance<0)
            {
                return GraphicRule.HeadIntersect;
            }
            else
            {
                return GraphicRule.None;
            }
        }

		/// <summary>
		/// 计算直线外一点到直线的距离
		/// </summary>
		/// <param name="point">直线外一点</param>
		/// <param name="linePoint1">直线上点1</param>
		/// <param name="linePoint2">直线上点2</param>
		public double distancePointToLine(Point point, Point linePoint1, Point linePoint2)
		{
			if (linePoint1.X == linePoint2.X)
			{
				//所给的直线是垂直线
				return Math.Abs(point.X - linePoint1.X);
			}

			if (linePoint1.Y == linePoint2.Y)
			{
				//所给的直线是水平线
				return Math.Abs(point.Y - linePoint1.Y);
			}

			//计算直线的k,b值
			double k = (double)(linePoint1.Y - linePoint2.Y) / (linePoint1.X - linePoint2.X);
			double b = (double)(linePoint2.Y * linePoint1.X - linePoint2.X * linePoint1.Y) / (linePoint1.X - linePoint2.X);

			return Math.Abs(point.Y - k * point.X - b) / Math.Abs(1 + k * k);
			
		}

		/// <summary>
		/// 通过位置检测任意两个MyGraphic之间的关系
		/// </summary>
		/// <param name="myGraphic"></param>
		/// <param name="myGraphics"></param>
		public void SearchRelationByPosition(MyGraphic myGraphic, List<MyGraphic> myGraphics, InkCollector _inkCollector)
		{
			foreach (MyGraphic mg in myGraphics)
			{
				if (mg != myGraphic)
				{
					string relation = GraphicAndGraphic(mg, myGraphic,_inkCollector).ToString();
                    if (relation == "None")
					{
						RemoveRelatioin(mg, myGraphic, _inkCollector);
						RemoveRelatioin(myGraphic, mg, _inkCollector);
					}
				}
			}
		}
        /// <summary>
        /// 在数据结构中检测任意两个MyGraphic之间的关系
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="myGraphics"></param>
        /// <param name="_inkCollector"></param>
        /// <returns></returns>
        public List<GraphicLinkNode> SearchRelationByGraphicLinkNodes(MyGraphic myGraphic, List<MyGraphic> myGraphics, InkCollector _inkCollector)
        {
            List<GraphicLinkNode> glns = new List<GraphicLinkNode>();
            foreach (MyGraphic mg in myGraphics)
            {
                if (mg != myGraphic)
                {
                    GraphicLinkNode gln = _inkCollector.Sketch.getGraphicLinkNodeByMyGraphicIDAndSelfMyGraphicID(myGraphic.MyGraphicID, mg.MyGraphicID);
                    glns.Add(gln);
                }
            }
            return glns;
        }
        /// <summary>
        /// 查找关联并删除关联
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="myGraphics"></param>
        /// <param name="_inkCollector"></param>
        public void searchExistRelationAndRemove(MyGraphic myGraphic, List<MyGraphic> myGraphics, InkCollector _inkCollector)
        {
            foreach (MyGraphic mg in myGraphics)
            {
                if (mg != myGraphic)
                {
                    string relation = GraphicAndGraphic(mg, myGraphic,_inkCollector).ToString();
                    if (relation != "None")
                    {
                        RemoveExistRelation(mg, myGraphic, _inkCollector);
                        RemoveExistRelation(myGraphic, mg, _inkCollector);
                    }
                }
            }
        }
		/// <summary>
		/// 创建两个MyGraphic之间的关系
		/// </summary>
		/// <param name="mg">from</param>
		/// <param name="myGraphic">to</param>
		/// <param name="relation"></param>
		private void AddRelation(MyGraphic mg, MyGraphic myGraphic, string relation,InkCollector _inkCollector)
		{
			//先查找两者是否已经存在同样的关联			
			GraphicLinkNode CheckGln = _inkCollector.Sketch.getGraphicLinkNodeByMyGraphicIDAndSelfMyGraphicID(myGraphic.MyGraphicID, mg.MyGraphicID);
			if (CheckGln == null)
			{
				//Console.WriteLine("mg.GraphicLinkNodeID:" + mg.GraphicLinkNodeID);
				//Console.WriteLine("mg.LastGraphicLinkNodeID:" + mg.LastGraphicLinkNodeID);
				GraphicLinkNode graphicLinkNode = new GraphicLinkNode(_inkCollector.Sketch.GraphicLinkNodes.Count == 0 ? 1 : _inkCollector.Sketch.GraphicLinkNodes[_inkCollector.Sketch.GraphicLinkNodes.Count - 1].GraphicLinkNodeID + 1, mg.MyGraphicID, myGraphic.MyGraphicID, relation);
				if (mg.GraphicLinkNodeID == 0)//没有相关图形
				{
					mg.GraphicLinkNodeID = mg.LastGraphicLinkNodeID = graphicLinkNode.GraphicLinkNodeID;
				}
				else if(mg.LastGraphicLinkNodeID!=0)//已经有相关图形的情况
				{
					GraphicLinkNode gln=_inkCollector.Sketch.getGraphicLinkNodeByID(mg.LastGraphicLinkNodeID);
                    if (gln != null)
                    {
                        gln.NextGraphicLinkNodeID = graphicLinkNode.GraphicLinkNodeID;
                    }
					mg.LastGraphicLinkNodeID = graphicLinkNode.GraphicLinkNodeID;

				}
				_inkCollector.Sketch.AddGraphicLinkNode(graphicLinkNode);
			}
		}
		/// <summary>
		/// 解除两个MyGraphic之间的关系
		/// </summary>
		/// <param name="myGraphic1">from</param>
		/// <param name="myGraphic2">to</param>
		private void RemoveRelatioin(MyGraphic myGraphic1, MyGraphic myGraphic2, InkCollector _inkCollector)
		{
			GraphicLinkNode Delgln = _inkCollector.Sketch.getGraphicLinkNodeByMyGraphicIDAndSelfMyGraphicID(myGraphic2.MyGraphicID, myGraphic1.MyGraphicID);//查找到要删除的GraphicLinkNode
			if (Delgln != null && Delgln.Rule != "None")
			{
				GraphicLinkNode preDelGln = _inkCollector.Sketch.getGraphicLinkNodeByNextGraphicLinkNodeID(Delgln.GraphicLinkNodeID);//查找到要删除的GraphicLinkNode的上一节点
				if (preDelGln == null)//如果上一节点不存在，即上一节点是MyGraphic
				{
					myGraphic1.GraphicLinkNodeID = Delgln.NextGraphicLinkNodeID;
					myGraphic1.LastGraphicLinkNodeID = 0;//如果删除的是和本MyGraphic相关的最后结点且无上一节点，修改LastGraphicLinkNodeID
				}
				else//如果上一节点存在
				{
					preDelGln.NextGraphicLinkNodeID = Delgln.NextGraphicLinkNodeID;
					myGraphic1.LastGraphicLinkNodeID = preDelGln.GraphicLinkNodeID;//如果删除的是和本MyGraphic相关的最后结点，修改LastGraphicLinkNodeID
				}
				if (_inkCollector.Sketch.GraphicLinkNodes.IndexOf(Delgln) == -1)
				{
					_inkCollector.Sketch.RemoveGraphicLinkNode(Delgln);
				}
				_inkCollector.Sketch.RemoveGraphicLinkNode(Delgln);
				//Console.WriteLine("_inkCollector.Sketch.GraphicLinkNodes.Count:" + _inkCollector.Sketch.GraphicLinkNodes.Count);
			}
		}

        private void RemoveExistRelation(MyGraphic myGraphic1, MyGraphic myGraphic2, InkCollector _inkCollector)
        {
            GraphicLinkNode Delgln = _inkCollector.Sketch.getGraphicLinkNodeByMyGraphicIDAndSelfMyGraphicID(myGraphic2.MyGraphicID, myGraphic1.MyGraphicID);//查找到要删除的GraphicLinkNode
            if (Delgln != null && Delgln.Rule != "None")
            {
                GraphicLinkNode preDelGln = _inkCollector.Sketch.getGraphicLinkNodeByNextGraphicLinkNodeID(Delgln.GraphicLinkNodeID);//查找到要删除的GraphicLinkNode的上一节点
                if (preDelGln == null)//如果上一节点不存在，即上一节点是MyGraphic
                {
                    myGraphic1.GraphicLinkNodeID = Delgln.NextGraphicLinkNodeID;
                    myGraphic1.LastGraphicLinkNodeID = Delgln.NextGraphicLinkNodeID;//如果删除的是和本MyGraphic相关的最后结点且无上一节点，修改LastGraphicLinkNodeID
                }
                else//如果上一节点存在
                {
                    preDelGln.NextGraphicLinkNodeID = Delgln.NextGraphicLinkNodeID;
                    myGraphic1.LastGraphicLinkNodeID = preDelGln.GraphicLinkNodeID;//如果删除的是和本MyGraphic相关的最后结点，修改LastGraphicLinkNodeID
                }
                if (_inkCollector.Sketch.GraphicLinkNodes.IndexOf(Delgln) == -1)
                {
                    _inkCollector.Sketch.RemoveGraphicLinkNode(Delgln);
                }
                _inkCollector.Sketch.RemoveGraphicLinkNode(Delgln);
            }
        }
        #endregion

        #region 查找函数
        /// <summary>
        /// 查找相关图形(不包括本身)
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="_inkCollector"></param>
        /// <param name="relativeMyGraphics"></param>
        /// <returns></returns>
        public List<MyGraphic> getDirectRelativeMyGraphicNoSelf(MyGraphic myGraphic, InkCollector _inkCollector, List<MyGraphic> relativeMyGraphics)
        {
            List<GraphicLinkNode> glns = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(myGraphic.MyGraphicID);//获取与myGraphic直接关联的GraphicLinkNode列表
            foreach (GraphicLinkNode gln in glns)
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (relativeMyGraphics.IndexOf(relativeMyGraphic) == -1)
                {
                    setStrokesColor(myGraphic,Colors.Red);
                    relativeMyGraphics.Add(relativeMyGraphic);
                    getDirectRelativeMyGraphicNoSelf(relativeMyGraphic, _inkCollector, relativeMyGraphics);
                }
                if (gln.NextGraphicLinkNodeID == 0)
                {
                    break;
                }
            }
            return relativeMyGraphics;
        }

        /// <summary>
        /// 查找相关图形(包括本身)
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="_inkCollector"></param>
        /// <param name="relativeMyGraphics"></param>
        /// <returns></returns>
        public List<MyGraphic> getDirectRelativeMyGraphicHasSelf(MyGraphic myGraphic, InkCollector _inkCollector, List<MyGraphic> relativeMyGraphics)
        {
            if (relativeMyGraphics.IndexOf(myGraphic) == -1)
            {
                relativeMyGraphics.Add(myGraphic);
            }
            List<GraphicLinkNode> glns = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(myGraphic.MyGraphicID);//获取与myGraphic直接关联的GraphicLinkNode列表
            foreach (GraphicLinkNode gln in glns)
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (relativeMyGraphics.IndexOf(relativeMyGraphic) == -1)
                {
                    setStrokesColor(myGraphic, Colors.Red);
                    getDirectRelativeMyGraphicHasSelf(relativeMyGraphic, _inkCollector, relativeMyGraphics);
                }
                if (gln.NextGraphicLinkNodeID == 0)
                {
                    break;
                }
            }
            return relativeMyGraphics;
        }
        /// <summary>
        /// 查找相关联的Arrow图形
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="_inkCollector"></param>
        /// <param name="relativeMyGraphics"></param>
        /// <returns></returns>
        public List<MyGraphic> getDirectRelativeArrowHasSelf(MyGraphic myGraphic, InkCollector _inkCollector, List<MyGraphic> relativeMyGraphics)
        {
            if (relativeMyGraphics.IndexOf(myGraphic) == -1)
            {
                relativeMyGraphics.Add(myGraphic);
            }
            List<GraphicLinkNode> glns = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(myGraphic.MyGraphicID);//获取与myGraphic直接关联的GraphicLinkNode列表
            foreach (GraphicLinkNode gln in glns)
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (((relativeMyGraphic.ShapeType == "arrow") || relativeMyGraphic.ShapeType == "loopArc" || relativeMyGraphic.ShapeType == "loopArcSelf"||(relativeMyGraphic.ShapeType == "polylineArrow")) && relativeMyGraphics.IndexOf(relativeMyGraphic) == -1)
                {
                    setStrokesColor(myGraphic, Colors.Red);
                    relativeMyGraphics.Add(relativeMyGraphic);
                }
            }
            return relativeMyGraphics;
        }
        /// <summary>
        /// 获取和myGraphic相关的图形，直到直线为止
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="_inkCollector"></param>
        /// <param name="relativeMyGraphics"></param>
        /// <returns></returns>
        public List<MyGraphic> getDirectRelativeMyGraphicLineHasSelf(MyGraphic myGraphic, InkCollector _inkCollector, List<MyGraphic> relativeMyGraphics)
        {
            if (relativeMyGraphics.IndexOf(myGraphic) == -1)
            {
                relativeMyGraphics.Add(myGraphic);
            }
            List<GraphicLinkNode> glns = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(myGraphic.MyGraphicID);//获取与myGraphic直接关联的GraphicLinkNode列表
            foreach (GraphicLinkNode gln in glns)
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (relativeMyGraphic != null)
                {
                    if (relativeMyGraphic.ShapeType == "arrow" || relativeMyGraphic.ShapeType == "loopArc" || relativeMyGraphic.ShapeType == "polylineArrow" || relativeMyGraphic.ShapeType == "loopArcSelf")
                    {
                        if (relativeMyGraphics.IndexOf(relativeMyGraphic) == -1)
                        {
                            setStrokesColor(myGraphic, Colors.Red);
                            //relativeMyGraphics.Add(myGraphic);
                        }
                    }
                    else
                    {
                        if (relativeMyGraphics.IndexOf(relativeMyGraphic) == -1)
                        {
                            setStrokesColor(myGraphic, Colors.Red);
                            getDirectRelativeMyGraphicLineHasSelf(relativeMyGraphic, _inkCollector, relativeMyGraphics);
                        }
                    }
                }
            }
            return relativeMyGraphics;
        }
        /// <summary>
        /// 查找向下相关的图形
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="_inkCollector"></param>
        /// <param name="relativeMyGraphics"></param>
        /// <returns></returns>
        public List<MyGraphic> getDirectDownRelativeMyGraphicNoSelf(MyGraphic myGraphic, InkCollector _inkCollector, List<MyGraphic> relativeMyGraphics)
        {
            List<GraphicLinkNode> glns = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(myGraphic.MyGraphicID);//获取与myGraphic直接关联的GraphicLinkNode列表
            foreach (GraphicLinkNode gln in glns)
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (relativeMyGraphic!=null&&relativeMyGraphic.Strokes.GetBounds().Top > myGraphic.Strokes.GetBounds().Top && relativeMyGraphics.IndexOf(relativeMyGraphic) == -1)
                {
                    setStrokesColor(myGraphic, Colors.Red);
                    relativeMyGraphics.Add(relativeMyGraphic);
                    getDirectDownRelativeMyGraphicNoSelf(relativeMyGraphic, _inkCollector, relativeMyGraphics);
                }
                if (gln.NextGraphicLinkNodeID == 0)
                {
                    break;
                }
            }
            return relativeMyGraphics;
        }
        #endregion

        #region 识别图形结构关系
        /// <summary>
        /// 识别图形之间的关系
        /// </summary>
        /// <param name="myGraphics"></param>
        /// <param name="linkNodes"></param>
        /// <returns></returns>
        public List<int> getGraphicStructure(MyGraphic mg,InkCollector _inkCollector,List<int> ids)
        {
            ids.Add(mg.MyGraphicID);
            List<GraphicLinkNode> glns = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(mg.MyGraphicID);//获取与myGraphic直接关联的GraphicLinkNode列表
            List<MyGraphic> loopMyGraphics = new List<MyGraphic>();
            foreach (GraphicLinkNode gln in glns)//查找是否有自循环
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (relativeMyGraphic.ShapeType == "loopArcSelf")
                {
                    ids.Add(mg.MyGraphicID);
                }
            }
            foreach (GraphicLinkNode gln in glns)//查找是否有循环
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (gln.Rule == "HeadIntersect")
                {
                    if(relativeMyGraphic.ShapeType == "loopArc")
                    {
                        List<GraphicLinkNode> glns_loop = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(relativeMyGraphic.MyGraphicID);
                        foreach (GraphicLinkNode gln_loop in glns_loop)
                        {
                            if (gln_loop.Rule == "TailIntersect")
                            {
                                int loopFirstMyGraphicId = gln_loop.MyGraphicID;
                                int lastIndex = MathTool.getInstance().getLastLocationInIntList(loopFirstMyGraphicId, ids);
                                int idsCount=ids.Count;
                                if (lastIndex != -1)
                                {
                                    for (int i = lastIndex; i < idsCount; i++)
                                    {
                                        if ((i > lastIndex && ids[i - 1] != ids[i])||i == lastIndex)
                                        {
                                            ids.Add(ids[i]);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            foreach (GraphicLinkNode gln in glns)//查找是否有箭头
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (gln.Rule == "HeadIntersect")
                {
                    if (relativeMyGraphic.ShapeType == "arrow" || relativeMyGraphic.ShapeType == "polylineArrow")
                    {
                        List<GraphicLinkNode> glnsNext = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(relativeMyGraphic.MyGraphicID);
                        foreach (GraphicLinkNode glnNext in glnsNext)
                        {
                            if (glnNext.Rule == "TailIntersect")
                            {
                                if (ids.IndexOf(glnNext.MyGraphicID) == -1)//不存在环
                                {
                                    getGraphicStructure(_inkCollector.Sketch.getMyGraphicByID(glnNext.MyGraphicID), _inkCollector, ids);
                                }
                                else//存在环的情况下删除形成死循环的箭头
                                {
                                    DeleteMyGraphicCommand dmgc = new DeleteMyGraphicCommand(_inkCollector, relativeMyGraphic);
                                    dmgc.execute();
                                }
                            }
                        }
                    }
                }
                if (gln.NextGraphicLinkNodeID == 0)
                {
                    break;
                }
            }
            foreach (GraphicLinkNode gln in glns)//层次图检查，矩形
            {
                MyGraphic relativeMyGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                if (mg.ShapeType == "rectangle"&&relativeMyGraphic.ShapeType=="rectangle"&&mg.Shape.Margin.Top<relativeMyGraphic.Shape.Margin.Top)
                {
                    getGraphicStructure(relativeMyGraphic, _inkCollector, ids);                    
                }
            }
            return ids;
        }
        #endregion

        #region 删除图形
        /// <summary>
        /// 删除图形
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="_inkCollector"></param>
        public void deleteMyGraphic(MyGraphic myGraphic,InkCollector _inkCollector)
        {
            if (myGraphic.ShapeType != "arrow" && myGraphic.ShapeType != "polylineArrow" && myGraphic.ShapeType != "loopArc" && myGraphic.ShapeType != "loopArcSelf")
            {
                //查找相关图形
                List<MyGraphic> mg2MoveListDelete = GraphicMathTool.getInstance().getDirectDownRelativeMyGraphicNoSelf(myGraphic, _inkCollector, new List<MyGraphic>());
                if (mg2MoveListDelete.Count > 0)
                {
                    if (mg2MoveListDelete[0].ShapeType == "arrow" || mg2MoveListDelete[0].ShapeType == "loopArc" || mg2MoveListDelete[0].ShapeType == "polylineArrow" || mg2MoveListDelete[0].ShapeType == "loopArcSelf")
                    {
                        DeleteMyGraphicCommand dmgcline = new DeleteMyGraphicCommand(_inkCollector, mg2MoveListDelete[0]);
                        dmgcline.execute();
                        _inkCollector.CommandStack.Push(dmgcline);
                        if (mg2MoveListDelete.Count > 1)
                        {
                            Command mgsmc = new MyGraphicsMoveCommand(mg2MoveListDelete, 0, myGraphic.Strokes.GetBounds().Top - mg2MoveListDelete[1].Strokes.GetBounds().Top, _inkCollector);
                            mgsmc.execute();
                            _inkCollector.CommandStack.Push(mgsmc);
                        }
                    }
                    else
                    {
                        Command mgsmc = new MyGraphicsMoveCommand(mg2MoveListDelete, 0, myGraphic.Strokes.GetBounds().Top - mg2MoveListDelete[0].Strokes.GetBounds().Top,  _inkCollector);
                        mgsmc.execute();
                        _inkCollector.CommandStack.Push(mgsmc);
                    }
                }
            }

            DeleteMyGraphicCommand dmgc = new DeleteMyGraphicCommand(_inkCollector, myGraphic);
            dmgc.execute();
            _inkCollector.CommandStack.Push(dmgc);
        }
        #endregion

        #region 识别图形函数

        /// <summary>
        /// 查找笔迹集中的折点
        /// </summary>
        /// <param name="strokes"></param>
        /// <returns></returns>
        public System.Windows.Input.StylusPointCollection getPolyPointsByStrokes(StrokeCollection strokes)
        {
            System.Windows.Input.StylusPointCollection spcPoints = new System.Windows.Input.StylusPointCollection();
            foreach (Stroke s in strokes)
            {
                spcPoints.Add(getPolyPointsByStroke(s));
            }
            return spcPoints;
        }

        /// <summary>
        /// 查找单笔迹中的折点
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public System.Windows.Input.StylusPointCollection getPolyPointsByStroke(Stroke stroke)
        {
            System.Windows.Input.StylusPointCollection spcPoints = new System.Windows.Input.StylusPointCollection();
            List<int> polyPointsIndex = getPolyPointsIndexByStroke(stroke);
            foreach (int i in polyPointsIndex)
            {
                spcPoints.Add(stroke.StylusPoints[i]);
            }
            return spcPoints;
        }

        /// <summary>
        /// 获取笔迹中折点的下标
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public List<int> getPolyPointsIndexByStroke(Stroke stroke)
        {
            //获取笔迹点集
            System.Windows.Input.StylusPointCollection points = stroke.StylusPoints;
            //获取笔迹点数
            int pointCount = points.Count;
            List<double> slopeList = new List<double>();//斜率列表
            List<int> polyPointsIndex = new List<int>();
            bool isline = false;
            if (pointCount > 7)
            {
                polyPointsIndex.Add(0);
                isline = isLine(stroke);
                if (!isline)
                {
                    //按照间隔长度获取折点
                    List<double> distanceList = new List<double>();
                    List<int> countList = new List<int>();
                    for (int i = 3; i <= pointCount - 4; i++)
                    {
                        double distance = MathTool.getInstance().distanceP2P(points[i - 3], points[i + 3]);
                        distanceList.Add(distance);
                    }

                    for (int i = 3; i <= distanceList.Count - 4; i++)
                    {
                        if (distanceList[i - 3] > distanceList[i - 2] && distanceList[i - 2] > distanceList[i - 1]
                            && distanceList[i - 1] > distanceList[i] && distanceList[i] < distanceList[i + 1]
                            && distanceList[i + 1] < distanceList[i + 2] && distanceList[i + 2] < distanceList[i + 3])
                        {
                            polyPointsIndex.Add(i+3);
                        }
                    }                   
                   
                }
                polyPointsIndex.Add(pointCount - 1);
                //消除伪折点
                if (!isline&&polyPointsIndex.Count >= 3)
                {
                    //Console.WriteLine("polyPointsIndex.Count" + polyPointsIndex.Count.ToString());
                    for (int i = 1; i < polyPointsIndex.Count - 1; i++)
                    {
                        //去除折线上的伪折点
                        double dis = MathTool.getInstance().distanceP2L(points[polyPointsIndex[i]], points[polyPointsIndex[i - 1]], points[polyPointsIndex[i + 1]]);
                        double angle1 = MathTool.getInstance().getAngleL2L(points[polyPointsIndex[i]], points[polyPointsIndex[i - 1]], points[polyPointsIndex[i + 1]]);
                        double angle2 = MathTool.getInstance().getAngleL2L(points[polyPointsIndex[i]], points[polyPointsIndex[i + 1]], points[polyPointsIndex[i - 1]]);
                        if (dis < removeFalsePolyPointDistance && angle1 < 90 && angle2 < 90)
                        {
                            polyPointsIndex.Remove(polyPointsIndex[i]);
                            i--;
                        }
                        else
                        {
                            //去除弧线上的伪折点
                            System.Windows.Input.StylusPointCollection spc1 = new System.Windows.Input.StylusPointCollection();
                            for (int k = polyPointsIndex[i - 1]; k <= polyPointsIndex[i]; k++)
                            {
                                spc1.Add(points[k]);
                            }
                            Stroke s1 = new Stroke(spc1);
                            System.Windows.Input.StylusPointCollection spc2 = new System.Windows.Input.StylusPointCollection();
                            for (int k = polyPointsIndex[i]; k <= polyPointsIndex[i + 1]; k++)
                            {
                                spc2.Add(points[k]);
                            }
                            Stroke s2 = new Stroke(spc2);
                            bool bool1 = GraphicMathTool.getInstance().isArc(s1);
                            bool bool2 = GraphicMathTool.getInstance().isArc(s2);
                            System.Windows.Input.StylusPointCollection spc3 = new System.Windows.Input.StylusPointCollection();
                            for (int k = polyPointsIndex[i]-1; k <= polyPointsIndex[i]+1; k++)
                            {
                                spc3.Add(points[k]);
                            }
                            Stroke s3 = new Stroke(spc3);
                            ArrayList nos = BuildDirectionNumber(s3);
                            int no = Math.Abs((int)nos[1] - (int)nos[0]);
                            bool bool3 = false;
                            if (no <= 2||no==7)
                            {
                                bool3 = true;
                            }
                            //Console.WriteLine("bool1" + bool1.ToString()+",bool2" + bool2.ToString()+",bool3" + bool3.ToString());
                            if(bool1&&bool2&&bool3)
                            {
                                polyPointsIndex.Remove(polyPointsIndex[i]);
                                i--;
                            }
                        }
                    }
                }
            }
            return polyPointsIndex;
        }

        /// <summary>
        /// 画笔迹点，用作测试
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="width"></param>
        /// <param name="c"></param>
        public void drawPoint(System.Windows.Input.StylusPoint sp, int width, Color c, InkCanvas _inkCanvas)
        {
            System.Windows.Input.StylusPointCollection sps = new System.Windows.Input.StylusPointCollection();
            sps.Add(sp);
            Stroke s = new Stroke(sps);
            s.DrawingAttributes.Color = c;
            s.DrawingAttributes.Width = width;
            s.DrawingAttributes.Height = width;
            _inkCanvas.Strokes.Add(s);
        }

        /// <summary>
        /// 判断笔迹是否是直线
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public bool isLine(Stroke stroke)
        {
            //获取笔迹点集
            System.Windows.Input.StylusPointCollection points = stroke.StylusPoints;
            if (points.Count > 1&&!isCloseStroke(stroke))
            {
                //长度比例判断因子
                double distanceF2L = MathTool.getInstance().distanceP2P(points[0], points[points.Count - 1]);//首尾点距离
                double distanceA = 0;//笔迹轨迹长
                for (int i = 0; i < points.Count - 1; i++)
                {
                    distanceA += MathTool.getInstance().distanceSP2SP(points[i], points[i+ 1]);
                }
                double k1 = distanceF2L / distanceA;//长度比例判断因子

                //落在直线周围点的比例
                int surroundingPointCount=0;
                foreach (System.Windows.Input.StylusPoint sp in points)
                {
                    double p2L = MathTool.getInstance().distanceP2L(sp, points[0], points[points.Count - 1]);
                    if (p2L < distanceP2LThreshold)
                    {
                        surroundingPointCount++;
                    }
                }
                double k2 = surroundingPointCount / points.Count;

                if (k1 > isLineLengthThreshold&&k2>isLinePointsThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断笔迹是否闭合
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public bool isCloseStroke(Stroke stroke)
        {
            //获取笔迹点集
            System.Windows.Input.StylusPointCollection points = stroke.StylusPoints;
            if (points.Count > 1)
            {
                //直线闭合判断因子
                double distanceF2L = MathTool.getInstance().distanceSP2SP(points[0], points[points.Count - 1]);
                double distanceA = 0;
                for (int i = 0; i < points.Count - 1; i++)
                {
                    distanceA += MathTool.getInstance().distanceSP2SP(points[i], points[i + 1]);
                }
                double kLine = distanceF2L / distanceA;

                //角度闭合判断因子
                Point centerPoint = MathTool.getInstance().getStrokeCenterPoint(stroke);//中心点
                //落笔点到中心点的直线角度
                double angleS2C = MathTool.getInstance().getAngleP2P(points[0], new System.Windows.Input.StylusPoint(centerPoint.X, centerPoint.Y));
                //起笔点到中心点的直线角度
                double angleE2C = MathTool.getInstance().getAngleP2P(points[points.Count - 1], 
                    new System.Windows.Input.StylusPoint(centerPoint.X, centerPoint.Y));
                double kAngle=Math.Abs(angleS2C-angleE2C)/180;
                if (kLine <= isCloseLineThreshold && kAngle <= isCloseAngleThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根据拐点拆分笔迹
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public StrokeCollection inciseStrokeByPolyPoints(Stroke stroke)
        {
            StrokeCollection strokes = new StrokeCollection();
            List<int> polyPointsIndex=getPolyPointsIndexByStroke(stroke);
            if (polyPointsIndex.Count > 0)
            {
                for(int i=polyPointsIndex.Count-1;i>1;i--)//拆分笔迹
                {
                    strokes.Add(subStrokeByIndex(stroke, polyPointsIndex[i-1], polyPointsIndex[i]));
                    removeStylusPointsFromStroke(stroke, polyPointsIndex[i-1]+1, polyPointsIndex[i]);
                }
            }
            return strokes;
        }

        /// <summary>
        /// 取笔迹的一部分，start 小于 end
        /// </summary>
        /// <param name="stroke"></param>
        /// <param name="start">起始下标</param>
        /// <param name="end">终止下标</param>
        /// <returns></returns>
        public Stroke subStrokeByIndex(Stroke stroke, int start, int end)
        {
            System.Windows.Input.StylusPointCollection sps = new System.Windows.Input.StylusPointCollection();
            for (int i = start; i <= end; i++)
            {
                sps.Add(stroke.StylusPoints[i]);
            }
            Stroke s = new Stroke(sps);
            s.DrawingAttributes = stroke.DrawingAttributes;
            //s.DrawingAttributes.Color = Colors.Blue;
            //s.DrawingAttributes.Width = 2;
            //s.DrawingAttributes.Height = 2;
            return s;
        }

        /// <summary>
        /// 删除笔迹上的点集
        /// </summary>
        /// <param name="stroke"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void removeStylusPointsFromStroke(Stroke stroke, int start, int end)
        {
            int i = 0;
            while(i<=end-start)
            {
                stroke.StylusPoints.Remove(stroke.StylusPoints[start]);
                i++;
            }
        }

        /// <summary>
        /// 判断是否是椭圆或圆弧
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public bool isEllipse(Stroke stroke)
        {
            //获取笔迹点集
            System.Windows.Input.StylusPointCollection points = stroke.StylusPoints;
            ArrayList nos = BuildDirectionNumber(stroke);
            if (nos.Count > 1)
            {
                int outNoCount = 0;
                for (int i = 0; i < nos.Count - 1; i++)
                {
                    int noCha = Math.Abs((int)nos[i + 1] - (int)nos[i]);
                    if (noCha > 1 && noCha < 7)
                    {
                        outNoCount++;
                    }
                }
                double k = (double)outNoCount / (double)nos.Count;

                //计算落在首尾连线周围的点的比例
                int surroundingPointCount = 0;
                foreach (System.Windows.Input.StylusPoint sp in points)
                {
                    double p2L = MathTool.getInstance().distanceP2L(sp, points[0], points[points.Count - 1]);
                    if (p2L < distanceP2LThreshold)
                    {
                        surroundingPointCount++;
                    }
                }
                double k2 = (double)surroundingPointCount / (double)points.Count;
                if (isCloseStroke(stroke)&&k <= isEllipsePointsThershold&& k2 >= 1-isLinePointsThreshold)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断圆弧
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public bool isArc(Stroke stroke)
        {
            //获取笔迹点集
            System.Windows.Input.StylusPointCollection points = stroke.StylusPoints;
            ArrayList nos = BuildDirectionNumber(stroke);
            if (nos.Count > 1)
            {
                int outNoCount = 0;
                for (int i = 0; i < nos.Count - 1; i++)
                {
                    int noCha = Math.Abs((int)nos[i + 1] - (int)nos[i]);
                    if (noCha > 1 && noCha < 7)
                    {
                        outNoCount++;
                    }
                }
                double k = (double)outNoCount / (double)nos.Count;

                //看是否是直线
                bool isline = isLine(stroke);
                if (k <= isEllipsePointsThershold && !isline)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 建立方向编号序列
        /// </summary>
        /// <returns></returns>
        public ArrayList BuildDirectionNumber(Stroke stroke)
        {
            ArrayList nos = new ArrayList();
            //获取笔迹点集
            System.Windows.Input.StylusPointCollection points = stroke.StylusPoints;
            if (points.Count > 1)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    double angle = Math.Atan2(points[i + 1].Y - points[i].Y, points[i + 1].X - points[i].X) + Math.PI * 2 / DEFAULT_SECTORS / 2;
                    if (angle < 0)
                        angle += Math.PI * 2;
                    int no = (int)Math.Floor(angle / (Math.PI * 2) * 100);//计算在弧度表中对应的向量编号
                    nos.Add(BuildAngleMap()[no]);
                }
            }
            return nos;
        }

        /// <summary>
        /// 建立弧度表
        /// </summary>
        private ArrayList BuildAngleMap()
        {
            double sectorRad = Math.PI * 2 / DEFAULT_SECTORS;
            ArrayList anglesMap = new ArrayList();

            //精度步进，100
            double step = Math.PI * 2 / 100;

            int sector;
            for (double i = -sectorRad / 2; i <= Math.PI * 2 - sectorRad / 2; i += step)
            {
                sector = (int)Math.Floor((i + sectorRad / 2) / sectorRad);
                anglesMap.Add(sector);
            }
            return anglesMap;
        }
        /// <summary>
        /// 判断是否是矩形
        /// </summary>
        /// <param name="strokes"></param>
        /// <returns></returns>
        public bool isRectangle(StrokeCollection strokes)
        {
            if (strokes.Count == 4)
            {
                //查找有没有平行的两条直线
                double angle0 = MathTool.getInstance().getAngleP2P(strokes[0].StylusPoints[0]
                    , strokes[0].StylusPoints[strokes[0].StylusPoints.Count - 1]);
                double angle1 = MathTool.getInstance().getAngleP2P(strokes[1].StylusPoints[0]
                    , strokes[1].StylusPoints[strokes[1].StylusPoints.Count - 1]);
                double angle2 = MathTool.getInstance().getAngleP2P(strokes[2].StylusPoints[0]
                    , strokes[2].StylusPoints[strokes[2].StylusPoints.Count - 1]);
                double angle3 = MathTool.getInstance().getAngleP2P(strokes[3].StylusPoints[0]
                    , strokes[3].StylusPoints[strokes[3].StylusPoints.Count - 1]);

                angle0 = angle0 > 180 ? angle0 - 180 : angle0;
                angle1 = angle1 > 180 ? angle1 - 180 : angle1;
                angle2 = angle2 > 180 ? angle2 - 180 : angle2;
                angle3 = angle3 > 180 ? angle3 - 180 : angle3;

                double distance0=MathTool.getInstance().distanceP2P(strokes[0].StylusPoints[0]
                    ,strokes[0].StylusPoints[strokes[0].StylusPoints.Count - 1]);
                double distance1=MathTool.getInstance().distanceP2P(strokes[1].StylusPoints[0]
                    ,strokes[1].StylusPoints[strokes[1].StylusPoints.Count - 1]);
                double distance2=MathTool.getInstance().distanceP2P(strokes[2].StylusPoints[0]
                    ,strokes[2].StylusPoints[strokes[2].StylusPoints.Count - 1]);
                double distance3=MathTool.getInstance().distanceP2P(strokes[3].StylusPoints[0]
                    ,strokes[3].StylusPoints[strokes[3].StylusPoints.Count - 1]);

                double angle10 = Math.Abs(angle1 - angle0);
                double angle20 = Math.Abs(angle2 - angle0);
                double angle30 = Math.Abs(angle3 - angle0);
                double minAngle = Math.Min(angle10, Math.Min(angle20, angle30));
                if (minAngle <= parallelThreshold)//存在平行线
                {
                    if (angle10 == minAngle)
                    {
                        double angle23 = Math.Abs(angle2 - angle3);
                        if (angle23 <= parallelThreshold)//存在另一对平行线
                        {
                            double angle12 = Math.Abs(angle1 - angle2);
                            if ((angle12 % 90) >= 90-verticalThreshold || (angle12 % 90) <= verticalThreshold)//两对平行线垂直
                            {
                                double distance01 = MathTool.getInstance().distanceP2L(strokes[0].StylusPoints[0]
                                    , strokes[1].StylusPoints[0], strokes[1].StylusPoints[strokes[1].StylusPoints.Count - 1]);
                                
                                double distance23 = MathTool.getInstance().distanceP2L(strokes[2].StylusPoints[0]
                                    , strokes[3].StylusPoints[0], strokes[3].StylusPoints[strokes[3].StylusPoints.Count - 1]);
                                if (Math.Abs(distance01 - distance2) <= rectangleDistanceThreshold 
                                    && Math.Abs(distance23 - distance1) <= rectangleDistanceThreshold)//对边距离在一定范围内
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else if (angle20 == minAngle)
                    {
                        double angle13 = Math.Abs(angle1 - angle3);
                        if (angle13 <= parallelThreshold)//存在另一对平行线
                        {
                            double angle01 = Math.Abs(angle0 - angle1);
                            if ((angle01 % 90) >= 90 - verticalThreshold || (angle01 % 90) <= verticalThreshold)//两对平行线垂直
                            {
                                double distance02 = MathTool.getInstance().distanceP2L(strokes[0].StylusPoints[0]
                                    , strokes[2].StylusPoints[0], strokes[2].StylusPoints[strokes[2].StylusPoints.Count - 1]);

                                double distance13 = MathTool.getInstance().distanceP2L(strokes[1].StylusPoints[0]
                                    , strokes[3].StylusPoints[0], strokes[3].StylusPoints[strokes[3].StylusPoints.Count - 1]);
                                if (Math.Abs(distance02 - distance1) <= rectangleDistanceThreshold
                                    && Math.Abs(distance13 - distance2) <= rectangleDistanceThreshold)//对边距离在一定范围内
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else if (angle30 == minAngle)
                    {
                        double angle12 = Math.Abs(angle1 - angle2);
                        if (angle12 <= parallelThreshold)//存在另一对平行线
                        {
                            double angle01 = Math.Abs(angle0 - angle1);
                            if ((angle01 % 90) >= 90 - verticalThreshold || (angle01 % 90) <= verticalThreshold)//两对平行线垂直
                            {
                                double distance03 = MathTool.getInstance().distanceP2L(strokes[0].StylusPoints[0]
                                    , strokes[3].StylusPoints[0], strokes[3].StylusPoints[strokes[3].StylusPoints.Count - 1]);

                                double distance12 = MathTool.getInstance().distanceP2L(strokes[1].StylusPoints[0]
                                    , strokes[2].StylusPoints[0], strokes[2].StylusPoints[strokes[2].StylusPoints.Count - 1]);
                                if (Math.Abs(distance03 - distance1) <= rectangleDistanceThreshold
                                    && Math.Abs(distance12 - distance3) <= rectangleDistanceThreshold)//对边距离在一定范围内
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 判断是否是循环箭头
        /// </summary>
        /// <param name="strokes"></param>
        /// <returns></returns>
        public System.Windows.Input.StylusPointCollection isLoop(StrokeCollection strokes)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();//箭头的头方向点和尾方向点
            if (strokes.Count == 3)
            {
                System.Windows.Input.StylusPointCollection firstPoints = new System.Windows.Input.StylusPointCollection();
                System.Windows.Input.StylusPointCollection lastPoints = new System.Windows.Input.StylusPointCollection();
                firstPoints.Add(strokes[0].StylusPoints[0]);//第0笔首点
                firstPoints.Add(strokes[1].StylusPoints[0]);//第1笔首点
                firstPoints.Add(strokes[2].StylusPoints[0]);//第2笔首点
                lastPoints.Add(strokes[0].StylusPoints[strokes[0].StylusPoints.Count - 1]);//第0笔尾点
                lastPoints.Add(strokes[1].StylusPoints[strokes[1].StylusPoints.Count - 1]);//第1笔尾点
                lastPoints.Add(strokes[2].StylusPoints[strokes[2].StylusPoints.Count - 1]);//第2笔尾点
                //直线0的首点到直线1的距离
                double length001 = MathTool.getInstance().distanceP2L(firstPoints[0], firstPoints[1], lastPoints[1]);
                //直线0的尾点到直线1的距离
                double length011 = MathTool.getInstance().distanceP2L(lastPoints[0], firstPoints[1], lastPoints[1]);
                if (length001 <= length011)//直线0的首点在箭头方向
                {
                    //笔迹1的首点到笔迹0的首点的距离
                    double length1000 = MathTool.getInstance().distanceP2P(firstPoints[1], firstPoints[0]);
                    //笔迹1的尾点到笔迹0的首点的距离
                    double length1100 = MathTool.getInstance().distanceP2P(lastPoints[1], firstPoints[0]);
                    //笔迹2的首点到笔迹0的首点的距离
                    double length2000 = MathTool.getInstance().distanceP2P(firstPoints[2], firstPoints[0]);
                    //笔迹2的尾点到笔迹0的首点的距离
                    double length2100 = MathTool.getInstance().distanceP2P(lastPoints[2], firstPoints[0]);

                    if (length1000 <= length1100)//直线1的首点在箭头方向
                    {
                        if (length2000 <= length2100)//直线2的首点在箭头方向(首首首)
                        {
                            return fff(strokes, firstPoints, lastPoints);

                        }
                        else//直线2的尾点点在箭头方向(首首尾)
                        {
                            return ffl(strokes, firstPoints, lastPoints);
                        }
                    }
                    else//直线1的尾点点在箭头方向
                    {
                        if (length2000 <= length2100)//直线2的首点在箭头方向(首尾首)
                        {
                            return flf(strokes, firstPoints, lastPoints);

                        }
                        else//直线2的尾点点在箭头方向(首尾尾)
                        {
                            return fll(strokes, firstPoints, lastPoints);
                        }
                    }
                }
                else//直线0的尾点在箭头方向
                {
                    //笔迹1的首点到笔迹0的尾点的距离
                    double length1001 = MathTool.getInstance().distanceP2P(firstPoints[1], lastPoints[0]);
                    //笔迹1的尾点到笔迹0的尾点的距离
                    double length1101 = MathTool.getInstance().distanceP2P(lastPoints[1], lastPoints[0]);
                    //笔迹2的首点到笔迹0的尾点的距离
                    double length2001 = MathTool.getInstance().distanceP2P(firstPoints[2], lastPoints[0]);
                    //笔迹2的尾点到笔迹0的尾点的距离
                    double length2101 = MathTool.getInstance().distanceP2P(lastPoints[2], lastPoints[0]);

                    if (length1001 <= length1101)//直线1的首点在箭头方向
                    {
                        if (length2001 <= length2101)//直线2的首点在箭头方向(首首首)
                        {
                            return lff(strokes, firstPoints, lastPoints);

                        }
                        else//直线2的尾点点在箭头方向(首首尾)
                        {
                            return lfl(strokes, firstPoints, lastPoints);
                        }
                    }
                    else//直线1的尾点点在箭头方向
                    {
                        if (length2001 <= length2101)//直线2的首点在箭头方向(首尾首)
                        {
                            return llf(strokes, firstPoints, lastPoints);

                        }
                        else//直线2的尾点点在箭头方向(首尾尾)
                        {
                            return lll(strokes, firstPoints, lastPoints);
                        }
                    }
                }


            }
            return points;
        }
        /// <summary>
        /// 判断是否是箭头
        /// </summary>
        /// <param name="strokes"></param>
        /// <returns></returns>
        public System.Windows.Input.StylusPointCollection isArrow(StrokeCollection strokes)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();//箭头的头方向点和尾方向点
            if (strokes.Count == 3)
            {
                System.Windows.Input.StylusPointCollection firstPoints = new System.Windows.Input.StylusPointCollection();
                System.Windows.Input.StylusPointCollection lastPoints = new System.Windows.Input.StylusPointCollection();
                firstPoints.Add(strokes[0].StylusPoints[0]);//第0笔首点
                firstPoints.Add(strokes[1].StylusPoints[0]);//第1笔首点
                firstPoints.Add(strokes[2].StylusPoints[0]);//第2笔首点
                lastPoints.Add(strokes[0].StylusPoints[strokes[0].StylusPoints.Count - 1]);//第0笔尾点
                lastPoints.Add(strokes[1].StylusPoints[strokes[1].StylusPoints.Count - 1]);//第1笔尾点
                lastPoints.Add(strokes[2].StylusPoints[strokes[2].StylusPoints.Count - 1]);//第2笔尾点
                //直线0的首点到直线1的距离
                double length001 = MathTool.getInstance().distanceP2L(firstPoints[0], firstPoints[1], lastPoints[1]);
                //直线0的尾点到直线1的距离
                double length011 = MathTool.getInstance().distanceP2L(lastPoints[0], firstPoints[1], lastPoints[1]);
                if (length001 <= length011)//直线0的首点在箭头方向
                {
                    //笔迹1的首点到笔迹0的首点的距离
                    double length1000 = MathTool.getInstance().distanceP2P(firstPoints[1], firstPoints[0]);
                    //笔迹1的尾点到笔迹0的首点的距离
                    double length1100 = MathTool.getInstance().distanceP2P(lastPoints[1], firstPoints[0]);
                    //笔迹2的首点到笔迹0的首点的距离
                    double length2000 = MathTool.getInstance().distanceP2P(firstPoints[2], firstPoints[0]);
                    //笔迹2的尾点到笔迹0的首点的距离
                    double length2100 = MathTool.getInstance().distanceP2P(lastPoints[2], firstPoints[0]);

                    if (length1000 <= length1100)//直线1的首点在箭头方向
                    {
                        if (length2000 <= length2100)//直线2的首点在箭头方向(首首首)
                        {
                            return fff(strokes,firstPoints, lastPoints);

                        }
                        else//直线2的尾点点在箭头方向(首首尾)
                        {
                            return ffl(strokes, firstPoints, lastPoints);
                        }
                    }
                    else//直线1的尾点点在箭头方向
                    {
                        if (length2000 <= length2100)//直线2的首点在箭头方向(首尾首)
                        {
                            return flf(strokes, firstPoints, lastPoints);

                        }
                        else//直线2的尾点点在箭头方向(首尾尾)
                        {
                            return fll(strokes, firstPoints, lastPoints);
                        }
                    }
                }
                else//直线0的尾点在箭头方向
                {
                    //笔迹1的首点到笔迹0的尾点的距离
                    double length1001 = MathTool.getInstance().distanceP2P(firstPoints[1], lastPoints[0]);
                    //笔迹1的尾点到笔迹0的尾点的距离
                    double length1101 = MathTool.getInstance().distanceP2P(lastPoints[1], lastPoints[0]);
                    //笔迹2的首点到笔迹0的尾点的距离
                    double length2001 = MathTool.getInstance().distanceP2P(firstPoints[2], lastPoints[0]);
                    //笔迹2的尾点到笔迹0的尾点的距离
                    double length2101 = MathTool.getInstance().distanceP2P(lastPoints[2], lastPoints[0]);

                    if (length1001 <= length1101)//直线1的首点在箭头方向
                    {
                        if (length2001 <= length2101)//直线2的首点在箭头方向(首首首)
                        {
                            return lff(strokes, firstPoints, lastPoints);

                        }
                        else//直线2的尾点点在箭头方向(首首尾)
                        {
                            return lfl(strokes, firstPoints, lastPoints);
                        }
                    }
                    else//直线1的尾点点在箭头方向
                    {
                        if (length2001 <= length2101)//直线2的首点在箭头方向(首尾首)
                        {
                            return llf(strokes, firstPoints, lastPoints);

                        }
                        else//直线2的尾点点在箭头方向(首尾尾)
                        {
                            return lll(strokes, firstPoints, lastPoints);
                        }
                    }
                }
                

            }
            return points;
        }
        /// <summary>
        /// 箭头中三条笔迹的首点都在箭头的情况
        /// </summary>
        /// <param name="points">返回箭头起始点和终止点</param>
        /// <param name="firstPoints">首点集合</param>
        /// <param name="lastPoints">尾点集合</param>
        private System.Windows.Input.StylusPointCollection fff(StrokeCollection strokes, System.Windows.Input.StylusPointCollection firstPoints, System.Windows.Input.StylusPointCollection lastPoints)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();
            double Stroke1_offsetX = firstPoints[0].X - firstPoints[1].X;
            double Stroke1_offsetY = firstPoints[0].Y - firstPoints[1].Y;
            double Stroke2_offsetX = firstPoints[0].X - firstPoints[2].X;
            double Stroke2_offsetY = firstPoints[0].Y - firstPoints[2].Y;
            //笔迹0与笔迹1的角度
            double angle01 = MathTool.getInstance().getAngleL2L(lastPoints[0],
                new System.Windows.Input.StylusPoint(lastPoints[1].X + Stroke1_offsetX, lastPoints[1].Y + Stroke1_offsetY),
                firstPoints[0]);

            //笔迹0与笔迹2的角度
            double angle02 = MathTool.getInstance().getAngleL2L(lastPoints[0],
                new System.Windows.Input.StylusPoint(lastPoints[2].X + Stroke2_offsetX, lastPoints[2].Y + Stroke2_offsetY),
                firstPoints[0]);

            //笔迹1与笔迹2的角度
            double angle12 = MathTool.getInstance().getAngleL2L(
                new System.Windows.Input.StylusPoint(lastPoints[1].X + Stroke1_offsetX, lastPoints[1].Y + Stroke1_offsetY),
                new System.Windows.Input.StylusPoint(lastPoints[2].X + Stroke2_offsetX, lastPoints[2].Y + Stroke2_offsetY),
                firstPoints[0]);
            double maxAngle = Math.Max(angle01, Math.Max(angle02, angle12));
            if (maxAngle < 180 - arrowAngleThreshold && maxAngle > arrowAngleThreshold)
            {
                if (angle01 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke01 = new StrokeCollection();
                    stroke01.Add(strokes[0]);
                    stroke01.Add(strokes[1]);
                    Rect bound = stroke01.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[2]))
                    {
                        points.Add(lastPoints[2]);
                        points.Add(firstPoints[2]);
                    }
                }
                else if (angle02 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke02 = new StrokeCollection();
                    stroke02.Add(strokes[0]);
                    stroke02.Add(strokes[2]);
                    Rect bound = stroke02.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[1]))
                    {
                        points.Add(lastPoints[1]);
                        points.Add(firstPoints[1]);
                    }
                }
                else if (angle12 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke12 = new StrokeCollection();
                    stroke12.Add(strokes[1]);
                    stroke12.Add(strokes[2]);
                    Rect bound = stroke12.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[0]))
                    {
                        points.Add(lastPoints[0]);
                        points.Add(firstPoints[0]);
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// 箭头中三条笔迹的（首首尾）在箭头的情况
        /// </summary>
        /// <param name="points">返回箭头起始点和终止点</param>
        /// <param name="firstPoints">首点集合</param>
        /// <param name="lastPoints">尾点集合</param>
        private System.Windows.Input.StylusPointCollection ffl(StrokeCollection strokes, 
            System.Windows.Input.StylusPointCollection firstPoints, 
            System.Windows.Input.StylusPointCollection lastPoints)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();
            double Stroke1_offsetX = firstPoints[0].X - firstPoints[1].X;
            double Stroke1_offsetY = firstPoints[0].Y - firstPoints[1].Y;
            double Stroke2_offsetX = firstPoints[0].X - lastPoints[2].X;
            double Stroke2_offsetY = firstPoints[0].Y - lastPoints[2].Y;
            //笔迹0与笔迹1的角度
            double angle01 = MathTool.getInstance().getAngleL2L(lastPoints[0],
                new System.Windows.Input.StylusPoint(lastPoints[1].X + Stroke1_offsetX, lastPoints[1].Y + Stroke1_offsetY),
                firstPoints[0]);

            //笔迹0与笔迹2的角度
            double angle02 = MathTool.getInstance().getAngleL2L(lastPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                firstPoints[0]);

            //笔迹1与笔迹2的角度
            double angle12 = MathTool.getInstance().getAngleL2L(
                new System.Windows.Input.StylusPoint(lastPoints[1].X + Stroke1_offsetX, lastPoints[1].Y + Stroke1_offsetY),
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                firstPoints[0]);
            double maxAngle = Math.Max(angle01, Math.Max(angle02, angle12));
            if (maxAngle < 180 - arrowAngleThreshold && maxAngle > arrowAngleThreshold)
            {
                if (angle01 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke01 = new StrokeCollection();
                    stroke01.Add(strokes[0]);
                    stroke01.Add(strokes[1]);
                    Rect bound = stroke01.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[2]))
                    {
                        points.Add(firstPoints[2]);
                        points.Add(lastPoints[2]);
                    }
                }
                else if (angle02 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke02 = new StrokeCollection();
                    stroke02.Add(strokes[0]);
                    stroke02.Add(strokes[2]);
                    Rect bound = stroke02.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[1]))
                    {
                        points.Add(lastPoints[1]);
                        points.Add(firstPoints[1]);
                    }
                }
                else if (angle12 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke12 = new StrokeCollection();
                    stroke12.Add(strokes[1]);
                    stroke12.Add(strokes[2]);
                    Rect bound = stroke12.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[0]))
                    {
                        points.Add(lastPoints[0]);
                        points.Add(firstPoints[0]);
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// 箭头中三条笔迹的（首尾首）在箭头的情况
        /// </summary>
        /// <param name="points">返回箭头起始点和终止点</param>
        /// <param name="firstPoints">首点集合</param>
        /// <param name="lastPoints">尾点集合</param>
        private System.Windows.Input.StylusPointCollection flf(StrokeCollection strokes, 
            System.Windows.Input.StylusPointCollection firstPoints,
            System.Windows.Input.StylusPointCollection lastPoints)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();
            double Stroke1_offsetX = firstPoints[0].X - lastPoints[1].X;
            double Stroke1_offsetY = firstPoints[0].Y - lastPoints[1].Y;
            double Stroke2_offsetX = firstPoints[0].X - firstPoints[2].X;
            double Stroke2_offsetY = firstPoints[0].Y - firstPoints[2].Y;
            //笔迹0与笔迹1的角度
            double angle01 = MathTool.getInstance().getAngleL2L(lastPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[1].X + Stroke1_offsetX, firstPoints[1].Y + Stroke1_offsetY),
                firstPoints[0]);

            //笔迹0与笔迹2的角度
            double angle02 = MathTool.getInstance().getAngleL2L(lastPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                firstPoints[0]);

            //笔迹1与笔迹2的角度
            double angle12 = MathTool.getInstance().getAngleL2L(
                new System.Windows.Input.StylusPoint(firstPoints[1].X + Stroke1_offsetX, firstPoints[1].Y + Stroke1_offsetY),
                new System.Windows.Input.StylusPoint(lastPoints[2].X + Stroke2_offsetX, lastPoints[2].Y + Stroke2_offsetY),
                firstPoints[0]);
            double maxAngle = Math.Max(angle01, Math.Max(angle02, angle12));
            if (maxAngle < 180 - arrowAngleThreshold && maxAngle > arrowAngleThreshold)
            {
                if (angle01 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke01 = new StrokeCollection();
                    stroke01.Add(strokes[0]);
                    stroke01.Add(strokes[1]);
                    Rect bound = stroke01.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[2]))
                    {
                        points.Add(lastPoints[2]);
                        points.Add(firstPoints[2]);
                    }
                }
                else if (angle02 == maxAngle)//笔迹2是箭尾
                {
                    
                    StrokeCollection stroke02 = new StrokeCollection();
                    stroke02.Add(strokes[0]);
                    stroke02.Add(strokes[2]);
                    Rect bound = stroke02.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[1]))
                    {
                        points.Add(firstPoints[1]);
                        points.Add(lastPoints[1]);
                    }
                }
                else if (angle12 == maxAngle)//笔迹2是箭尾
                {
                    
                    StrokeCollection stroke12 = new StrokeCollection();
                    stroke12.Add(strokes[1]);
                    stroke12.Add(strokes[2]);
                    Rect bound = stroke12.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[0]))
                    {
                        points.Add(lastPoints[0]);
                        points.Add(firstPoints[0]);
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// 箭头中三条笔迹的（首尾尾）在箭头的情况
        /// </summary>
        /// <param name="points">返回箭头起始点和终止点</param>
        /// <param name="firstPoints">首点集合</param>
        /// <param name="lastPoints">尾点集合</param>
        private System.Windows.Input.StylusPointCollection fll(StrokeCollection strokes,
            System.Windows.Input.StylusPointCollection firstPoints,
            System.Windows.Input.StylusPointCollection lastPoints)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();
            double Stroke1_offsetX = firstPoints[0].X - lastPoints[1].X;
            double Stroke1_offsetY = firstPoints[0].Y - lastPoints[1].Y;
            double Stroke2_offsetX = firstPoints[0].X - lastPoints[2].X;
            double Stroke2_offsetY = firstPoints[0].Y - lastPoints[2].Y;
            //笔迹0与笔迹1的角度
            double angle01 = MathTool.getInstance().getAngleL2L(lastPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[1].X + Stroke1_offsetX, firstPoints[1].Y + Stroke1_offsetY),
                firstPoints[0]);

            //笔迹0与笔迹2的角度
            double angle02 = MathTool.getInstance().getAngleL2L(lastPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                firstPoints[0]);

            //笔迹1与笔迹2的角度
            double angle12 = MathTool.getInstance().getAngleL2L(
                new System.Windows.Input.StylusPoint(firstPoints[1].X + Stroke1_offsetX, firstPoints[1].Y + Stroke1_offsetY),
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                firstPoints[0]);
            double maxAngle = Math.Max(angle01, Math.Max(angle02, angle12));
            if (maxAngle < 180 - arrowAngleThreshold && maxAngle > arrowAngleThreshold)
            {
                if (angle01 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke01 = new StrokeCollection();
                    stroke01.Add(strokes[0]);
                    stroke01.Add(strokes[1]);
                    Rect bound = stroke01.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[2]))
                    {
                        points.Add(firstPoints[2]);
                        points.Add(lastPoints[2]);
                    }
                }
                else if (angle02 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke02 = new StrokeCollection();
                    stroke02.Add(strokes[0]);
                    stroke02.Add(strokes[2]);
                    Rect bound = stroke02.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[1]))
                    {
                        points.Add(firstPoints[1]);
                        points.Add(lastPoints[1]);
                    }
                }
                else if (angle12 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke12 = new StrokeCollection();
                    stroke12.Add(strokes[1]);
                    stroke12.Add(strokes[2]);
                    Rect bound = stroke12.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[0]))
                    {
                        points.Add(lastPoints[0]);
                        points.Add(firstPoints[0]);
                    }
                }
            }
            return points;
        }








        /// <summary>
        /// 箭头中三条笔迹的(尾首首)都在箭头的情况
        /// </summary>
        /// <param name="points">返回箭头起始点和终止点</param>
        /// <param name="firstPoints">首点集合</param>
        /// <param name="lastPoints">尾点集合</param>
        private System.Windows.Input.StylusPointCollection lff(StrokeCollection strokes,
            System.Windows.Input.StylusPointCollection firstPoints, 
            System.Windows.Input.StylusPointCollection lastPoints)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();
            double Stroke1_offsetX = lastPoints[0].X - firstPoints[1].X;
            double Stroke1_offsetY = lastPoints[0].Y - firstPoints[1].Y;
            double Stroke2_offsetX = lastPoints[0].X - firstPoints[2].X;
            double Stroke2_offsetY = lastPoints[0].Y - firstPoints[2].Y;
            //笔迹0与笔迹1的角度
            double angle01 = MathTool.getInstance().getAngleL2L(firstPoints[0],
                new System.Windows.Input.StylusPoint(lastPoints[1].X + Stroke1_offsetX, lastPoints[1].Y + Stroke1_offsetY),
                lastPoints[0]);

            //笔迹0与笔迹2的角度
            double angle02 = MathTool.getInstance().getAngleL2L(firstPoints[0],
                new System.Windows.Input.StylusPoint(lastPoints[2].X + Stroke2_offsetX, lastPoints[2].Y + Stroke2_offsetY),
                lastPoints[0]);

            //笔迹1与笔迹2的角度
            double angle12 = MathTool.getInstance().getAngleL2L(
                new System.Windows.Input.StylusPoint(lastPoints[1].X + Stroke1_offsetX, lastPoints[1].Y + Stroke1_offsetY),
                new System.Windows.Input.StylusPoint(lastPoints[2].X + Stroke2_offsetX, lastPoints[2].Y + Stroke2_offsetY),
                lastPoints[0]);
            double maxAngle = Math.Max(angle01, Math.Max(angle02, angle12));
            if (maxAngle < 180 - arrowAngleThreshold && maxAngle > arrowAngleThreshold)
            {
                if (angle01 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke01 = new StrokeCollection();
                    stroke01.Add(strokes[0]);
                    stroke01.Add(strokes[1]);
                    Rect bound = stroke01.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[2]))
                    {
                        points.Add(lastPoints[2]);
                        points.Add(firstPoints[2]);
                    }
                }
                else if (angle02 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke02 = new StrokeCollection();
                    stroke02.Add(strokes[0]);
                    stroke02.Add(strokes[2]);
                    Rect bound = stroke02.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[1]))
                    {
                        points.Add(lastPoints[1]);
                        points.Add(firstPoints[1]);
                    }
                }
                else if (angle12 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke12 = new StrokeCollection();
                    stroke12.Add(strokes[1]);
                    stroke12.Add(strokes[2]);
                    Rect bound = stroke12.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[0]))
                    {
                        points.Add(firstPoints[0]);
                        points.Add(lastPoints[0]);
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// 箭头中三条笔迹的（尾首尾）在箭头的情况
        /// </summary>
        /// <param name="points">返回箭头起始点和终止点</param>
        /// <param name="firstPoints">首点集合</param>
        /// <param name="lastPoints">尾点集合</param>
        private System.Windows.Input.StylusPointCollection lfl(StrokeCollection strokes,
            System.Windows.Input.StylusPointCollection firstPoints,
            System.Windows.Input.StylusPointCollection lastPoints)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();
            double Stroke1_offsetX = lastPoints[0].X - firstPoints[1].X;
            double Stroke1_offsetY = lastPoints[0].Y - firstPoints[1].Y;
            double Stroke2_offsetX = lastPoints[0].X - lastPoints[2].X;
            double Stroke2_offsetY = lastPoints[0].Y - lastPoints[2].Y;
            //笔迹0与笔迹1的角度
            double angle01 = MathTool.getInstance().getAngleL2L(firstPoints[0],
                new System.Windows.Input.StylusPoint(lastPoints[1].X + Stroke1_offsetX, lastPoints[1].Y + Stroke1_offsetY),
                lastPoints[0]);

            //笔迹0与笔迹2的角度
            double angle02 = MathTool.getInstance().getAngleL2L(firstPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                lastPoints[0]);

            //笔迹1与笔迹2的角度
            double angle12 = MathTool.getInstance().getAngleL2L(
                new System.Windows.Input.StylusPoint(lastPoints[1].X + Stroke1_offsetX, lastPoints[1].Y + Stroke1_offsetY),
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                lastPoints[0]);
            double maxAngle = Math.Max(angle01, Math.Max(angle02, angle12));
            if (maxAngle < 180 - arrowAngleThreshold && maxAngle > arrowAngleThreshold)
            {
                if (angle01 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke01 = new StrokeCollection();
                    stroke01.Add(strokes[0]);
                    stroke01.Add(strokes[1]);
                    Rect bound = stroke01.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[2]))
                    {
                        points.Add(firstPoints[2]);
                        points.Add(lastPoints[2]);
                    }
                }
                else if (angle02 == maxAngle)//笔迹2是箭尾
                {
                    
                    StrokeCollection stroke02 = new StrokeCollection();
                    stroke02.Add(strokes[0]);
                    stroke02.Add(strokes[2]);
                    Rect bound = stroke02.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[1]))
                    {
                        points.Add(lastPoints[1]);
                        points.Add(firstPoints[1]);
                    }
                }
                else if (angle12 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke12 = new StrokeCollection();
                    stroke12.Add(strokes[1]);
                    stroke12.Add(strokes[2]);
                    Rect bound = stroke12.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[0]))
                    {
                        points.Add(firstPoints[0]);
                        points.Add(lastPoints[0]);
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// 箭头中三条笔迹的（尾尾首）在箭头的情况
        /// </summary>
        /// <param name="points">返回箭头起始点和终止点</param>
        /// <param name="firstPoints">首点集合</param>
        /// <param name="lastPoints">尾点集合</param>
        private System.Windows.Input.StylusPointCollection llf(StrokeCollection strokes,
            System.Windows.Input.StylusPointCollection firstPoints,
            System.Windows.Input.StylusPointCollection lastPoints)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();
            double Stroke1_offsetX = lastPoints[0].X - lastPoints[1].X;
            double Stroke1_offsetY = lastPoints[0].Y - lastPoints[1].Y;
            double Stroke2_offsetX = lastPoints[0].X - firstPoints[2].X;
            double Stroke2_offsetY = lastPoints[0].Y - firstPoints[2].Y;
            //笔迹0与笔迹1的角度
            double angle01 = MathTool.getInstance().getAngleL2L(firstPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[1].X + Stroke1_offsetX, firstPoints[1].Y + Stroke1_offsetY),
                lastPoints[0]);

            //笔迹0与笔迹2的角度
            double angle02 = MathTool.getInstance().getAngleL2L(firstPoints[0],
                new System.Windows.Input.StylusPoint(lastPoints[2].X + Stroke2_offsetX, lastPoints[2].Y + Stroke2_offsetY),
                lastPoints[0]);

            //笔迹1与笔迹2的角度
            double angle12 = MathTool.getInstance().getAngleL2L(
                new System.Windows.Input.StylusPoint(firstPoints[1].X + Stroke1_offsetX, firstPoints[1].Y + Stroke1_offsetY),
                new System.Windows.Input.StylusPoint(lastPoints[2].X + Stroke2_offsetX, lastPoints[2].Y + Stroke2_offsetY),
                lastPoints[0]);
            double maxAngle = Math.Max(angle01, Math.Max(angle02, angle12));
            if (maxAngle < 180 - arrowAngleThreshold && maxAngle > arrowAngleThreshold)
            {
                if (angle01 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke01 = new StrokeCollection();
                    stroke01.Add(strokes[0]);
                    stroke01.Add(strokes[1]);
                    Rect bound = stroke01.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, firstPoints[2]))
                    {
                        points.Add(lastPoints[2]);
                        points.Add(firstPoints[2]);
                    }
                }
                else if (angle02 == maxAngle)//笔迹2是箭尾
                {
                    
                    StrokeCollection stroke02 = new StrokeCollection();
                    stroke02.Add(strokes[0]);
                    stroke02.Add(strokes[2]);
                    Rect bound = stroke02.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[1]))
                    {
                        points.Add(firstPoints[1]);
                        points.Add(lastPoints[1]);
                    }
                }
                else if (angle12 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke12 = new StrokeCollection();
                    stroke12.Add(strokes[1]);
                    stroke12.Add(strokes[2]);
                    Rect bound = stroke12.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[0]))
                    {
                        points.Add(firstPoints[0]);
                        points.Add(lastPoints[0]);
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// 箭头中三条笔迹的（尾尾尾）在箭头的情况
        /// </summary>
        /// <param name="points">返回箭头起始点和终止点</param>
        /// <param name="firstPoints">首点集合</param>
        /// <param name="lastPoints">尾点集合</param>
        private System.Windows.Input.StylusPointCollection lll(StrokeCollection strokes,
            System.Windows.Input.StylusPointCollection firstPoints,
            System.Windows.Input.StylusPointCollection lastPoints)
        {
            System.Windows.Input.StylusPointCollection points = new System.Windows.Input.StylusPointCollection();
            double Stroke1_offsetX = lastPoints[0].X - lastPoints[1].X;
            double Stroke1_offsetY = lastPoints[0].Y - lastPoints[1].Y;
            double Stroke2_offsetX = lastPoints[0].X - lastPoints[2].X;
            double Stroke2_offsetY = lastPoints[0].Y - lastPoints[2].Y;
            //笔迹0与笔迹1的角度
            double angle01 = MathTool.getInstance().getAngleL2L(firstPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[1].X + Stroke1_offsetX, firstPoints[1].Y + Stroke1_offsetY),
                lastPoints[0]);

            //笔迹0与笔迹2的角度
            double angle02 = MathTool.getInstance().getAngleL2L(firstPoints[0],
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                lastPoints[0]);

            //笔迹1与笔迹2的角度
            double angle12 = MathTool.getInstance().getAngleL2L(
                new System.Windows.Input.StylusPoint(firstPoints[1].X + Stroke1_offsetX, firstPoints[1].Y + Stroke1_offsetY),
                new System.Windows.Input.StylusPoint(firstPoints[2].X + Stroke2_offsetX, firstPoints[2].Y + Stroke2_offsetY),
                lastPoints[0]);
            double maxAngle = Math.Max(angle01, Math.Max(angle02, angle12));
            if (maxAngle < 180 - arrowAngleThreshold && maxAngle > arrowAngleThreshold)
            {
                if (angle01 == maxAngle)//笔迹2是箭尾
                {
                    StrokeCollection stroke01 = new StrokeCollection();
                    stroke01.Add(strokes[0]);
                    stroke01.Add(strokes[1]);
                    Rect bound = stroke01.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[2]))
                    {
                        points.Add(firstPoints[2]);
                        points.Add(lastPoints[2]);
                    }
                }
                else if (angle02 == maxAngle)//笔迹2是箭尾
                {                    
                    StrokeCollection stroke02 = new StrokeCollection();
                    stroke02.Add(strokes[0]);
                    stroke02.Add(strokes[2]);
                    Rect bound = stroke02.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[1]))
                    {
                        points.Add(firstPoints[1]);
                        points.Add(lastPoints[1]);
                    }
                }
                else if (angle12 == maxAngle)//笔迹2是箭尾
                {
                    
                    StrokeCollection stroke12 = new StrokeCollection();
                    stroke12.Add(strokes[1]);
                    stroke12.Add(strokes[2]);
                    Rect bound = stroke12.GetBounds();
                    if (MathTool.getInstance().isPointInRect(bound, lastPoints[0]))
                    {
                        points.Add(firstPoints[0]);
                        points.Add(lastPoints[0]);
                    }
                }
            }
            return points;
        }
        #endregion
    }
}
