using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WPFInk.ink;

namespace WPFInk.cmd
{
    class AddTextCommand: Command
    {
        private InkCollector _inkCollector;
		private MyRichTextBox _myRichTextBox;

		public AddTextCommand(InkCollector inkcollector, MyRichTextBox myRichTextBox)
        {
            this._inkCollector = inkcollector;
			this._myRichTextBox = myRichTextBox;
        }

        public void execute()
        {
			_inkCollector.AddText(_myRichTextBox);
        }

        public void undo()
        {
			_inkCollector.RemoveText(_myRichTextBox);
        }
    }
}
