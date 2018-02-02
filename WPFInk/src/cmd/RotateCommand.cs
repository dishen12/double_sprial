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
using WPFInk.tool;
using WPFInk.ink;

namespace WPFInk.cmd
{
    /// <summary>
    /// 完成旋转命令
    /// 使得stroke集合可以围绕自身旋转一定的度数
    /// </summary>
    public class RotateCommand:Command
    {
        //private StrokeCollection collection;
		private MyStroke _myStroke;
        private double angle;
        StylusPoint center;

        public RotateCommand(MyStroke myStroke, double angle, StylusPoint center)
        {
			this._myStroke = myStroke;
            this.angle = angle/180;
            this.center = center;
        }

        public void execute()
        {
           
            Matrix rotatingMatrix = new Matrix();
            rotatingMatrix.RotateAt(-angle, center.X, center.Y);
			_myStroke.Stroke.Transform(rotatingMatrix, false);

        }

        public void undo()
        {
                        
            Matrix rotatingMatrix = new Matrix();
			rotatingMatrix.RotateAt(angle, center.X, center.Y);
			_myStroke.Stroke.Transform(rotatingMatrix, false);
        }
    }
}
