using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using WPFInk.ink;
using WPFInk.tool;
using System.Windows.Ink;
using WPFInk.cmd;
using System.Windows;
using WPFInk.videoSummarization;
using WPFInk.Global;
using System.Diagnostics;

namespace WPFInk.state
{
    public class InkState_Ink : InkState
    {
        private MyStroke _mystroke; 
        private int _starttime;
        private Nullable<Point> _startpoint = null;

        public InkState_Ink(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
        }

        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _starttime = MyTimer.getInstance().getTime();
            _inkCanvas.CaptureMouse();
            _startpoint = e.GetPosition(_inkCanvas);
            
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            //Point current = e.GetPosition(_inkCanvas);
            //Console.WriteLine("move:" + current.X + "," + current.Y);
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_startpoint != null)
            {
                if (_inkCanvas.Strokes.Count - 1 > _inkCollector._sketch.MyStrokes.Count)
                {
                    _inkCollector._sketch.MyStrokes.Clear();
                    foreach (Stroke stroke in _inkCanvas.Strokes)
                    {
                        _mystroke = new MyStroke(stroke);
                        _mystroke.StartTime = _starttime;
                        _mystroke.EndTime = MyTimer.getInstance().getTime();

                        Command addStrokeCommand = new AddStrokeCommand(_inkCollector, _mystroke);
                        _inkCollector.CommandStack.Push(addStrokeCommand);
                        addStrokeCommand.execute();
                    }
                }
                else
                {
                    //Point current = e.GetPosition(_inkCanvas);
                    //Console.WriteLine("up:"+current.X + "," + current.Y);
                    if (_inkCanvas.Strokes.Count > 0)
                    {
                        Stroke lastStroke = _inkCanvas.Strokes.Last();

                        _mystroke = new MyStroke(lastStroke);
                        _mystroke.StartTime = _starttime;
                        _mystroke.EndTime = MyTimer.getInstance().getTime();

                        Command addStrokeCommand = new AddStrokeCommand(_inkCollector, _mystroke);
                        _inkCollector.CommandStack.Push(addStrokeCommand);
                        addStrokeCommand.execute();
                    }
                }
            }
        }
    }
}
