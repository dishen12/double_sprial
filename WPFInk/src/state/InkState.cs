using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using WPFInk.ink;
using System.Windows.Ink;

namespace WPFInk.state
{
    public abstract class InkState
    {
        protected InkCollector _inkCollector;
        protected InkCanvas _inkCanvas;

        public InkState(InkCollector inkCollector)
        {
            this._inkCollector = inkCollector;
            this._inkCanvas = inkCollector.InkCanvas;
        }

        public abstract void _presenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e);
        public abstract void _presenter_MouseMove(object sender, MouseEventArgs e);
        public abstract void _presenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e);

    }
}
