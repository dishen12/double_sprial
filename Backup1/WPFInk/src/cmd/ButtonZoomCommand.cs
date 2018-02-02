using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFInk.ink;

namespace WPFInk.cmd
{
    /// <summary>
    /// 图像缩放命令
    /// </summary>
    public class ButtonZoomCommand : Command
    {
        private MyButton myButton;
        private double scaling;
        private InkCanvas _inkCanvas;
        private InkCollector _inkCollector;
        private double _prescaling;

        public ButtonZoomCommand(MyButton myButton, double scaling, InkCollector _inkCollector,double preScaling)
        {
            this.myButton = myButton;
            this.scaling = scaling;
            this._inkCanvas = _inkCollector._mainPage._inkCanvas;
            this._inkCollector = _inkCollector;
            this._prescaling = preScaling;
        }

        
        public void execute()
        {
            myButton.Width *= scaling;
            myButton.Height *= scaling;
            InkConstants.InkCanvasTransform(myButton.InkFrame._inkCanvas, 1, 1, myButton.Width / myButton.InkFrame.Width, myButton.Height / myButton.InkFrame.Height);
            myButton.updateArrow(_inkCanvas, _inkCollector);
			myButton.updateRectangles(_inkCanvas, _inkCollector);
        }

        public void undo()
        {
            myButton.Width *= 1 / scaling;
            myButton.Height *= 1 / scaling;
            InkConstants.InkCanvasTransform(myButton.InkFrame._inkCanvas, 1, 1, _prescaling, _prescaling);
			myButton.updateArrow(_inkCanvas, _inkCollector);
			myButton.updateRectangles(_inkCanvas, _inkCollector);
        }
    }

}