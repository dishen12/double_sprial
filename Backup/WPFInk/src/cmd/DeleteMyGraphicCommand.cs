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
using WPFInk.graphic;
using WPFInk.tool;

namespace WPFInk.cmd
{
    /// <summary>
    /// 删除stroke
    /// 要删除sketch和inkpresenter中相对应的stroke
    /// 同时，还要删除stroke所在group中的所有stroke
    /// </summary>
    public class DeleteMyGraphicCommand:Command
    {
        private InkCollector _inkcollector;
        private Sketch _sketch;
        private MyGraphic _myGraphic;
        private StrokeCollection _strokes;

        public DeleteMyGraphicCommand(InkCollector inkcollector,MyGraphic myGraphic)
        {
            _inkcollector = inkcollector;
            _sketch = _inkcollector.Sketch;
            _myGraphic = myGraphic;
            _strokes = myGraphic.Strokes;
        }

        

        public void execute()
        {
            removeRelation();
            _inkcollector.RemoveMyGraphic(_myGraphic);

        }

        
        public void undo()
        {
            removeRelation();
            _inkcollector.AddMyGraphic(_myGraphic);
        }
        private void removeRelation()
        {
            _inkcollector.Sketch.RemoveGraphicLinkNode(_inkcollector.Sketch.getGraphicLinkNodeByNextGraphicLinkNodeID(_myGraphic.MyGraphicID));
            foreach (GraphicLinkNode gln in _inkcollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(_myGraphic.MyGraphicID))
            {
                _inkcollector.Sketch.RemoveGraphicLinkNode(gln);
            }
        }

    }
}
