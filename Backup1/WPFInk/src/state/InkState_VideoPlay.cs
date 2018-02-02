using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using WPFInk.ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFInk.tool;
using WPFInk.cmd;
using System.Collections.Generic;

namespace WPFInk.state
{
    public class InkState_VideoPlay : InkState
    {
        
        public InkState_VideoPlay(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            this._inkCanvas = inkCollector.InkCanvas;
        }

        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }
    }
}
