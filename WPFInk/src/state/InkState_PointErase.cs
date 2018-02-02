using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows;
using WPFInk.cmd;
using WPFInk.tool;

namespace WPFInk.state
{
    public class InkState_PointErase : InkState
    {

        private bool isButtonDown = false;
        public InkState_PointErase(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
        }

        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.CaptureMouse();
            isButtonDown = true;
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isButtonDown)
            {
                Point pointErasePoint = e.GetPosition(_inkCanvas);
                StrokeCollection hitStrokes = _inkCanvas.Strokes.HitTest(pointErasePoint,3);
                if (hitStrokes.Count > 0)
                {
                    foreach (Stroke hitStroke in hitStrokes)
                    {
                        ////For each intersecting stroke, split the stroke into two while removing the intersecting points.
                        ProcessPointErase(hitStroke, pointErasePoint);
                    }
                }
            }
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.ReleaseMouseCapture();
            isButtonDown = false;
        }


        /// <summary>
        /// 将stroke以点迹的模式擦除，拆分成两个stroke
        /// </summary>
        /// <param name="stroke">被擦除的stroke</param>
        /// <param name="pointErasePoints">擦除的点</param>
        void ProcessPointErase(Stroke stroke, Point pointErasePoint)
        {
            if (stroke.StylusPoints.Count > 1)
            {
                Stroke hitTestStroke;
                StylusPointCollection splitStrokePoints1=new StylusPointCollection();
                StylusPointCollection splitStrokePoints2 = new StylusPointCollection(); 

                StylusPointCollection spc = new StylusPointCollection();
                foreach (StylusPoint point in stroke.StylusPoints)
                {
                    spc.Add(point);
                }
                hitTestStroke = new Stroke(spc);
                hitTestStroke.DrawingAttributes = stroke.DrawingAttributes;

                while (true)
                {
                    if (hitTestStroke.StylusPoints.Count > 0)
                    {
                        if (hitTestStroke.StylusPoints.Count > 1)
                        {         
                            StylusPoint sp = hitTestStroke.StylusPoints[hitTestStroke.StylusPoints.Count - 1];
                            hitTestStroke.StylusPoints.RemoveAt(hitTestStroke.StylusPoints.Count - 1);
                            if (!hitTestStroke.HitTest(pointErasePoint, 3)) break;
                            splitStrokePoints2.Add(sp);
                        }
                        else
                        {
                            break;
                        }
                    }
                }


                StylusPointCollection spc2 = new StylusPointCollection();
                foreach (StylusPoint point in stroke.StylusPoints)
                {
                    spc2.Add(point);
                }
                hitTestStroke = new Stroke(spc2);
                hitTestStroke.DrawingAttributes = stroke.DrawingAttributes;
                //从下标为0的位置开始遍历点迹，直到第一个被擦除的点 
                while (true)
                {
                    if (hitTestStroke.StylusPoints.Count > 0)
                    {
                        if (hitTestStroke.StylusPoints.Count > 1)
                        {
                            StylusPoint sp = hitTestStroke.StylusPoints[0];
                            hitTestStroke.StylusPoints.RemoveAt(0);
                            if (!hitTestStroke.HitTest(pointErasePoint, 3)) break;
                            splitStrokePoints1.Add(sp);
                        }
                        else
                        {
                            break;
                        }
                    }
                }


                // 将原来的stroke删除掉，然后添加两个新的stroke
                if (splitStrokePoints1.Count > 1)
                {
                    Stroke splitStroke1 = new Stroke(splitStrokePoints1);
                    splitStroke1.DrawingAttributes = InkTool.getInstance().DrawingAttributesCopy(stroke.DrawingAttributes);
                    MyStroke mystroke = new MyStroke(splitStroke1);
                    AddStrokeCommand cmd = new AddStrokeCommand(_inkCollector, mystroke);
                    _inkCollector.CommandStack.Push(cmd);
                    cmd.execute();
                }


                if (splitStrokePoints2.Count > 1)
                {
                    Stroke splitStroke2 = new Stroke(splitStrokePoints2);
                    splitStroke2.DrawingAttributes = InkTool.getInstance().DrawingAttributesCopy(stroke.DrawingAttributes);
                    MyStroke mystroke = new MyStroke(splitStroke2);
                    AddStrokeCommand cmd = new AddStrokeCommand(_inkCollector, mystroke);
                    _inkCollector.CommandStack.Push(cmd);
                    cmd.execute();
                }


            }
            DeleteStrokeCommand dsc = new DeleteStrokeCommand(_inkCollector, stroke);
            _inkCollector.CommandStack.Push(dsc);
            dsc.execute();

        }
    }
}
