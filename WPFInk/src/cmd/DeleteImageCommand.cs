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
    public class DeleteImageCommand:Command
    {
        private InkCollector _inkcollector;
        private Sketch _sketch;
        private MyImage _myImage;
        

        public DeleteImageCommand(InkCollector inkcollector,MyImage myImage)
        {
            _inkcollector = inkcollector;
            _sketch = _inkcollector.Sketch;
            _myImage = myImage;
        }

        

        public void execute()
        {
            
            _inkcollector.RemoveImage(_myImage);
        }

        public void undo()
        {
            _inkcollector.AddImage(_myImage);
        }
    }
}
