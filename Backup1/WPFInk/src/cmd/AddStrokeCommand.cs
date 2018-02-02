using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;

namespace WPFInk.cmd
{
    public class AddStrokeCommand:Command
    {
        private InkCollector _inkCollector;
        private MyStroke _myStroke;

        public AddStrokeCommand(InkCollector inkCollector, MyStroke stroke)
        {
            _inkCollector = inkCollector;
            _myStroke = stroke;
        }

        #region Command Members

        public void execute()
        {
            _inkCollector.AddStroke(_myStroke);
        }

        public void undo()
        {
            _inkCollector.RemoveStroke(_myStroke);
        }

        #endregion
    }
}
