using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WPFInk.ink;

namespace WPFInk.cmd
{
    class DeleteTextCommand: Command
    {
        private InkCollector _inkCollector;
		private MyRichTextBox _myRichTextBox;

		public DeleteTextCommand(InkCollector inkcollector, MyRichTextBox myRichTextBox)
        {
            this._inkCollector = inkcollector;
			this._myRichTextBox = myRichTextBox;
        }

        public void execute()
        {
			_inkCollector.RemoveText(_myRichTextBox);
        }

        public void undo()
        {
			_inkCollector.AddText(_myRichTextBox);
        }
    }
}
