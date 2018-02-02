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
using WPFInk.ink;

namespace WPFInk.cmd
{
    /// <summary>
    /// 删除stroke
    /// 要删除sketch和inkpresenter中相对应的stroke
    /// 同时，还要删除stroke所在group中的所有stroke
    /// </summary>
    public class DeleteStrokeCommand:Command
    {
        private InkCollector _inkcollector;
        private Sketch _sketch;
        private MyStroke _mystroke;
        private Stroke _stroke;

        public DeleteStrokeCommand(InkCollector inkcollector,MyStroke stroke)
        {
            _inkcollector = inkcollector;
            _sketch = _inkcollector.Sketch;
            _mystroke = stroke;
            _stroke = stroke.Stroke;
        }

        public DeleteStrokeCommand(InkCollector inkcollector, Stroke stroke)
        {
            _inkcollector = inkcollector;
            _sketch = inkcollector.Sketch;
            _stroke = stroke;
            _mystroke = _sketch.GetStroke(stroke);
        }

        public void execute()
        {
            _inkcollector.RemoveStroke(_mystroke);
        }

        public void undo()
        {
            _inkcollector.AddStroke(_mystroke);
        }
    }
}
