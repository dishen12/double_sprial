using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;            
using WPFInk.video;
using System.Windows.Shapes;
using WPFInk.graphic;

namespace WPFInk.cmd
{
    public class AddMyGraphicCommand : Command
    {
		private MyGraphic _myGraphic;
        private InkCollector _inkCollector;
		public AddMyGraphicCommand(InkCollector _inkCollector, MyGraphic myGraphic)
        {
			this._myGraphic = myGraphic;
            this._inkCollector = _inkCollector;
        }
        public void execute()
        {
			_inkCollector.AddMyGraphic(_myGraphic);
        }

        public void undo()
        {
			_inkCollector.RemoveMyGraphic(_myGraphic);
        }
    }
}
