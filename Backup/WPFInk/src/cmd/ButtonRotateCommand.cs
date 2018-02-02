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
    /// 图片旋转
    /// </summary>
    public class ButtonRotateCommand : Command
    {
        private MyButton myButton;
        private double preAngle;

        /// <summary>
        /// 图片image旋转angle角度
        /// </summary>
        /// <param name="image">被旋转的图片</param>
        /// <param name="preAngle">旋转的角度</param>
        public ButtonRotateCommand(MyButton myButton, double preAngle)
        {
            this.myButton = myButton;
            this.preAngle = preAngle;
        }

       
        /// <summary>
        /// 执行
        /// </summary>
        public void execute()
        {
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.RotateAt(-myButton.Angle, myButton.Width / 2, myButton.Height / 2);
            this.myButton.Button.RenderTransform = new MatrixTransform(m);
        }

        

        /// <summary>
        /// 撤销
        /// </summary>
        public void undo()
        {
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.RotateAt(preAngle, myButton.Width / 2, myButton.Height / 2);
            this.myButton.Button.RenderTransform = new MatrixTransform(m);
        }
    }
}
