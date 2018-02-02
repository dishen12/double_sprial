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
    /// 图片移动
    /// </summary>
    public class ButtonMoveCommand : Command
    {
        private MyButton myButton;
        private double offset_x, offset_y;
        private InkCanvas _inkCanvas;
        private InkCollector _inkCollector;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="offsetx">移动x距离</param>
        /// <param name="offsety">移动y距离</param>
        public ButtonMoveCommand(MyButton myButton, double offsetx, double offsety, InkCollector _inkCollector)
        {
            this.myButton = myButton;
            this.offset_x = offsetx;
            this.offset_y = offsety;
            this._inkCanvas = _inkCollector._mainPage._inkCanvas;
            this._inkCollector = _inkCollector;
        }

       

        /// <summary>
        /// 执行移动的过程
        /// </summary>
        public void execute()
        {
            myButton.Left += offset_x;
            myButton.Top += offset_y;
            myButton.setLocation((int)myButton.Left, (int)myButton.Top);
            //更新时间段显示
            _inkCanvas.Children.Remove(myButton.TextBoxTime);
            myButton.adjustTextBoxTime();
			myButton.updateRectangles(_inkCanvas,_inkCollector);
            _inkCanvas.Children.Add(myButton.TextBoxTime);
            myButton.updateArrow(_inkCanvas, _inkCollector);
        }

        

        /// <summary>
        /// 撤销
        /// </summary>
        public void undo()
        {
            myButton.Left -= offset_x;
            myButton.Top -= offset_y;
            myButton.setLocation((int)myButton.Left, (int)myButton.Top);
            //更新时间段显示
            _inkCanvas.Children.Remove(myButton.TextBoxTime);
			myButton.adjustTextBoxTime();
			myButton.updateRectangles(_inkCanvas,_inkCollector);
            _inkCanvas.Children.Add(myButton.TextBoxTime);
            myButton.updateArrow(_inkCanvas, _inkCollector);
        }
    }
}