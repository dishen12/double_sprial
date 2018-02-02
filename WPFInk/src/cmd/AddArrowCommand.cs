using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;            
using WPFInk.video;
using System.Windows.Shapes;

namespace WPFInk.cmd
{
    public class AddArrowCommand : Command
    {
        private MyArrow _myArrow;
        private InkCollector _inkCollector;
        public AddArrowCommand(InkCollector _inkCollector,MyArrow myArrow)
        {
            this._myArrow = myArrow;
            this._inkCollector = _inkCollector;
        }
        public void execute()
        {
            _inkCollector.AddArrow(_myArrow);
        }

        public void undo()
        {
            _inkCollector.RemoveArrow(_myArrow);
        }
    }
}
