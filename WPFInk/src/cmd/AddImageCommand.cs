using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;

namespace WPFInk.cmd
{
    public class AddImageCommand : Command
    {
        private InkCollector _inkCollector;
        private MyImage _image;

        public AddImageCommand(InkCollector inkcollector, MyImage image)
        {
            this._inkCollector = inkcollector;
            this._image = image;
        }

        public void execute()
        {
            _inkCollector.AddImage(_image);
        }

        public void undo()
        {
            _inkCollector.RemoveImage(_image);
        }
    }
}
