using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;
using System.Windows.Ink;
using System.Windows.Input;
using WPFInk.cmd;
using System.Windows;

namespace WPFInk.state
{
    public class InkState_Erase : InkState
    {
        private bool isButtonDown = false;

        public InkState_Erase(InkCollector inkCollector) : base(inkCollector)
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
                StrokeCollection hitStrokes = _inkCanvas.Strokes.HitTest(e.GetPosition(_inkCanvas),5);
                if (hitStrokes.Count > 0)
                {
                    foreach (Stroke hitStroke in hitStrokes)
                    {
                        DeleteStrokeCommand dsc = new DeleteStrokeCommand(_inkCollector,hitStroke);
                        _inkCollector.CommandStack.Push(dsc);
                        dsc.execute();
                    }
                }
            }
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.ReleaseMouseCapture();
            isButtonDown = false;
        }
    }
}
