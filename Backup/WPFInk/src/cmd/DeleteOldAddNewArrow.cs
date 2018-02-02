using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;         
using WPFInk.ink;        
using WPFInk.video;

namespace WPFInk.cmd
{
    public class DeleteOldAddNewArrow:Command
    {
        private MyArrow _myArrowOld;
        private MyArrow _myArrowNew;
        private InkCollector _inkCollector;
        public DeleteOldAddNewArrow(InkCollector _inkCollector, MyArrow myArrowOld, MyArrow myArrowNew)
        {
            this._myArrowOld = myArrowOld;
            this._myArrowNew = myArrowNew;
            this._inkCollector = _inkCollector;
        }
        public void execute()
        {
            _inkCollector.RemoveArrow(_myArrowOld);
            _inkCollector.AddArrow(_myArrowNew);
        }

        public void undo()
        {

            _inkCollector.AddArrow(_myArrowOld);
            _inkCollector.RemoveArrow(_myArrowNew);
        }
    }
}
