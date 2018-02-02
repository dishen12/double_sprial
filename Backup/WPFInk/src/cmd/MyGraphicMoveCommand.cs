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
using WPFInk.graphic;
using WPFInk.ink;
using WPFInk.tool;

namespace WPFInk.cmd
{
    /// <summary>
    /// 移动命令
    /// </summary>
	public class MyGraphicMoveCommand : Command
    {
		private MyGraphic _myGraphic;
        private double offset_x, offset_y;
		private List<MyGraphic> MyGraphics;
		private InkCollector _inkCollector;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strokeCollection"></param>
        /// <param name="offset_x"></param>
        /// <param name="offset_y"></param>
        public MyGraphicMoveCommand(MyGraphic myGraphic,double offset_x,double offset_y,InkCollector inkCollector)
        {
			this._myGraphic = myGraphic;
            this.offset_x = offset_x;
            this.offset_y = offset_y;
            MyGraphics = inkCollector.Sketch.MyGraphics;
			this._inkCollector = inkCollector;
        }

        public void execute()
        {
            if (_myGraphic.ShapeType == "ellipse" || _myGraphic.ShapeType == "rectangle")
            {
                MathTool.getInstance().MoveStrokes(_myGraphic.Strokes, offset_x, offset_y);
                MathTool.getInstance().MoveStrokes(_myGraphic.textStrokeCollection, offset_x, offset_y);
                MathTool.getInstance().MoveStrokes(_myGraphic.PentagramStrokes, offset_x, offset_y);
                _myGraphic.Shape.Margin = new Thickness(_myGraphic.Shape.Margin.Left + offset_x, _myGraphic.Shape.Margin.Top + offset_y, 0, 0);
                //GraphicMathTool.getInstance().SearchRelationByPosition(_myGraphic, MyGraphics, _inkCollector);


                //修改直线
                List<GraphicLinkNode> glns = _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(_myGraphic.MyGraphicID);
                foreach (GraphicLinkNode gln in glns)
                {
                    MyGraphic relativeGraphic = _inkCollector.Sketch.getMyGraphicByID(gln.MyGraphicID);
                    if (relativeGraphic != null)
                    {
                        if (relativeGraphic.ShapeType == "arrow" || relativeGraphic.ShapeType == "loopArc" || relativeGraphic.ShapeType == "polylineArrow")
                        {
                            ZoomAndRotate(gln, relativeGraphic);
                        }
                        else if (relativeGraphic.ShapeType == "loopArcSelf")
                        {
                            ((System.Windows.Shapes.Line)relativeGraphic.Shape).X1 = ((System.Windows.Shapes.Line)relativeGraphic.Shape).X1 + offset_x;
                            ((System.Windows.Shapes.Line)relativeGraphic.Shape).Y1 = ((System.Windows.Shapes.Line)relativeGraphic.Shape).Y1 + offset_y;
                            ((System.Windows.Shapes.Line)relativeGraphic.Shape).X2 = ((System.Windows.Shapes.Line)relativeGraphic.Shape).X2 + offset_x;
                            ((System.Windows.Shapes.Line)relativeGraphic.Shape).Y2 = ((System.Windows.Shapes.Line)relativeGraphic.Shape).Y2 + offset_y;
                            MathTool.getInstance().MoveStrokes(relativeGraphic.Strokes, offset_x, offset_y);
                        }
                    }
                }
            }
        }
        public void searchRelation()
        {
            GraphicMathTool.getInstance().SearchRelationByPosition(_myGraphic, MyGraphics, _inkCollector);
        }
        private void ZoomAndRotate(GraphicLinkNode gln, MyGraphic relativeGraphic)
        {
            Matrix m1 = new Matrix();
            Matrix m2 = new Matrix();
            Matrix m3 = new Matrix();
            double zoomScale = 1;
            double preX1 = ((System.Windows.Shapes.Line)relativeGraphic.Shape).X1;
            double preY1 = ((System.Windows.Shapes.Line)relativeGraphic.Shape).Y1;
            double preX2 = ((System.Windows.Shapes.Line)relativeGraphic.Shape).X2;
            double preY2 = ((System.Windows.Shapes.Line)relativeGraphic.Shape).Y2;
            double nowX1 = preX1 + offset_x;
            double nowY1 = preY1 + offset_y;
            double nowX2 = preX2 + offset_x;
            double nowY2 = preY2 + offset_y;
            double preLength = MathTool.getInstance().distanceP2P(new StylusPoint(preX2, preY2), new StylusPoint(preX1, preY1));
            double nowLength;
            if (gln.Rule == "HeadIntersect")//修改第一个点
            {
                ((System.Windows.Shapes.Line)relativeGraphic.Shape).X1 = nowX1;
                ((System.Windows.Shapes.Line)relativeGraphic.Shape).Y1 = nowY1;

                if (relativeGraphic.ShapeType == "arrow" || relativeGraphic.ShapeType == "loopArc")
                {
                    double angle1 = MathTool.getInstance().getAngleP2P(new StylusPoint(preX1, preY1), new StylusPoint(preX2, preY2));
                    double angle2 = MathTool.getInstance().getAngleP2P(new StylusPoint(nowX1, nowY1), new StylusPoint(preX2, preY2));
                    double angle = angle2 - angle1;//旋转角度

                    nowLength = MathTool.getInstance().distanceP2P(new StylusPoint(preX2, preY2), new StylusPoint(nowX1, nowY1));

                    if (preLength == 0||nowLength==0)
                    {
                        zoomScale = 1;
                    }
                    else
                    {
                        zoomScale = nowLength / preLength;
                    }
                    m1.RotateAt(-angle, preX2, preY2);
                    m2.ScaleAt(zoomScale, zoomScale, preX2, preY2);
                    Matrix m = m1 * m2;
                
                    relativeGraphic.Strokes[0].Transform(m, true);
                    relativeGraphic.Strokes[0].DrawingAttributes.StylusTipTransform = new Matrix(1, 0, 0, 1, 0, 0);
                    relativeGraphic.Strokes[1].Transform(m1, true);

                    if (relativeGraphic.Strokes.Count == 3)//笔迹数为3时
                    {
                        relativeGraphic.Strokes[2].Transform(m1, true);
                    }
                }               
            }
            else if (gln.Rule == "TailIntersect")//修改最后点
            {
                ((System.Windows.Shapes.Line)relativeGraphic.Shape).X2 = nowX2;
                ((System.Windows.Shapes.Line)relativeGraphic.Shape).Y2 = nowY2;
                       
                if (relativeGraphic.ShapeType == "arrow"||relativeGraphic.ShapeType == "loopArc")
                {
                    double angle1 = MathTool.getInstance().getAngleP2P(new StylusPoint(preX2, preY2), new StylusPoint(preX1, preY1));
                    double angle2 = MathTool.getInstance().getAngleP2P(new StylusPoint(nowX2, nowY2), new StylusPoint(preX1, preY1));
                    double angle = angle2 - angle1;//旋转角度

                    nowLength = MathTool.getInstance().distanceP2P(new StylusPoint(nowX2, nowY2), new StylusPoint(preX1, preY1));

                    if (preLength == 0||nowLength==0)
                    {
                        zoomScale = 1;
                    }
                    else
                    {
                        zoomScale = nowLength / preLength;
                    }

                    m1.RotateAt(-angle, preX1, preY1);
                    relativeGraphic.Strokes[0].Transform(m1, false);
                    m2.ScaleAt(zoomScale, zoomScale, preX1, preY1);
                    relativeGraphic.Strokes[0].Transform(m2, false);

                    //************************************
                    //用于计算箭头的头部移动位置
                    StylusPoint p = new StylusPoint(preX2, preY2);
                    StylusPointCollection ps = new StylusPointCollection();
                    ps.Add(p);
                    Stroke s = new Stroke(ps);
                    //*************************************
                    //箭的头部只旋转和移动，不缩放
                    if (relativeGraphic.Strokes.Count == 2)//笔迹数为2时
                    {
                        relativeGraphic.Strokes[1].Transform(m1, false);                        
                        s.Transform(m1, false);
                        m3.Translate(nowX2 - s.StylusPoints[0].X, nowY2 - s.StylusPoints[0].Y);
                        relativeGraphic.Strokes[1].Transform(m3, false);
                    }
                    else if (relativeGraphic.Strokes.Count == 3)//笔迹数为3时
                    {
                        relativeGraphic.Strokes[1].Transform(m1, false); 
                        relativeGraphic.Strokes[2].Transform(m1, false);
                        s.Transform(m1, false);
                        m3.Translate(nowX2 - s.StylusPoints[0].X, nowY2 - s.StylusPoints[0].Y);
                        relativeGraphic.Strokes[1].Transform(m3, false);
                        relativeGraphic.Strokes[2].Transform(m3, false);
                    }
                   
                }
                
            }
        }
		
        public void undo()
        {
            if (_myGraphic.ShapeType != "relativeGraphic")
            {
                MathTool.getInstance().MoveStrokes(_myGraphic.Strokes, -offset_x, -offset_y);
                MathTool.getInstance().MoveStrokes(_myGraphic.textStrokeCollection, -offset_x, -offset_y);
                MathTool.getInstance().MoveStrokes(_myGraphic.PentagramStrokes, -offset_x, -offset_y);
                _myGraphic.Shape.Margin = new Thickness(_myGraphic.Shape.Margin.Left - offset_x, _myGraphic.Shape.Margin.Top - offset_y, 0, 0);
                GraphicMathTool.getInstance().SearchRelationByPosition(_myGraphic, MyGraphics, _inkCollector);
            }
        }
		
    }
}
