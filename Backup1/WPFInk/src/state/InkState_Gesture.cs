using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Ink;
using WPFInk.ink;
using WPFInk.cmd;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Text;

namespace WPFInk.state
{
    public class InkState_Gesture : InkState
    {

        private InkFrame _inkFrame;
        public InkState_Gesture(InkCollector inkCollector)
            : base(inkCollector)
        {

            this._inkCollector = inkCollector;
            this._inkFrame = inkCollector._mainPage;
            
            //initApp();
            
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
