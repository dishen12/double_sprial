using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;

namespace WPFInk.cmd
{
    public class AddButtonCommand:Command
    {
        private MyButton myButton;
        private InkCollector _inkCollector;
        public AddButtonCommand(InkCollector _inkCollector, MyButton myButton)
        {
            this.myButton = myButton;
            this._inkCollector = _inkCollector;
        }
        public void execute()
        {
            _inkCollector.AddButton(myButton);
        }

        public void undo()
        {
            _inkCollector.RemoveButton(myButton);
        }
    }
}
