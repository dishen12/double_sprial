using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.state;
using System.Windows;
using WPFInk.ink;
using WPFInk.tool;
using System.Windows.Shapes;
using WPFInk.video;
using WPFInk.cmd;

namespace WPFInk.state
{
    public class InkState_DrawArrow  :InkState
    {
        private InkFrame _inkFrame;
        private Point startPoint;
        private MyButton source;
        private MyButton target;
        public InkState_DrawArrow(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            this._inkFrame = inkCollector._mainPage;
        }
        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            _inkCanvas.CaptureMouse();//捕捉鼠标
            startPoint = e.GetPosition(_inkCanvas);
            foreach (MyButton myButton in _inkCollector.Sketch.MyButtons)
            {
                if (MathTool.getInstance().isCloseMyButton(startPoint, myButton,20) == true)
                {
                    source = myButton;
                    break;
                }
            }
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {


        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (startPoint != null)
            {
                Point endPoint = e.GetPosition(_inkCanvas);
                foreach (MyButton myButton in _inkCollector.Sketch.MyButtons)
                {
                    if (MathTool.getInstance().isCloseMyButton(endPoint, myButton,20) == true)
                    {
                        target = myButton;
                        break;
                    }
                }
            
                if (source != target&&source!=null&&target!=null)
                {
                    ThumbConnector thumbConnector = new ThumbConnector(source, target);
                    MyArrow ma = new MyArrow(thumbConnector.arrow);
                    ma.PreMyButton = source;
                    ma.NextMyButton = target;
                    ma.StartPoint = startPoint;
                    ma.EndPoint = endPoint;
                    int i = 0;
                    foreach (MyArrow myArrow in _inkCollector.Sketch.MyArrows)
                    {
                        if (myArrow.IsDeleted == false&& myArrow.PreMyButton.IsDeleted == false && myArrow.PreMyButton==source && _inkCanvas.Children.IndexOf(myArrow.Arrow) > -1 )
                        {                                                           
                            Command doana = new DeleteOldAddNewArrow(_inkCollector,myArrow, ma);
                            doana.execute();
                            _inkCollector.CommandStack.Push(doana);
                            i = -1;
                            break;
                        }
                        i++;
                    }
                    if (i >= _inkCollector.Sketch.MyArrows.Count)
                    {
                        Command aac = new AddArrowCommand(_inkCollector, ma);
                        aac.execute();
                        _inkCollector.CommandStack.Push(aac);
                        
                    }
                }
            }
            _inkCanvas.ReleaseMouseCapture();

        }
    }
}
