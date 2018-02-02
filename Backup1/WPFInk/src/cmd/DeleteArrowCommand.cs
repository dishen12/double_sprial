using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;        
using WPFInk.video;

namespace WPFInk.cmd
{
    class DeleteArrowCommand:Command
    {

        private MyArrow _myArrow;
        private InkCollector _inkCollector;
        public DeleteArrowCommand(InkCollector _inkCollector, MyArrow myArrow)
        {
            this._myArrow = myArrow;
            this._inkCollector = _inkCollector;
        }
        public void execute()
        {
            _inkCollector.RemoveArrow(_myArrow);
        }

        public void undo()
        {                        
            _inkCollector.AddArrow(_myArrow);
			foreach (MyArrow ma in _inkCollector.Sketch.MyArrows)
			{
				if (ma.IsDeleted == false && ma.PreMyButton == _myArrow.PreMyButton)
				{
					_inkCollector.RemoveArrow(ma);
				}
			}
        }
    }
}
