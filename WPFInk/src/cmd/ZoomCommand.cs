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
    /// 将选中的笔迹按照比例放缩
    /// </summary>
    public class ZoomCommand:Command
    {
		private MyStroke _myStroke;
		private double scaling;
		private StylusPoint center;

        public ZoomCommand(MyStroke myStroke, double scaling)
        {
			_myStroke = myStroke;
			Rect bounds = myStroke.Stroke.GetBounds();
            center = new StylusPoint(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
			this.scaling = scaling;
        }

        public void execute()
        {
			for (int j = 0; j < _myStroke.Stroke.StylusPoints.Count; j++)
            {
				StylusPoint point = _myStroke.Stroke.StylusPoints[j];
                double offsetx = point.X - center.X;
                double offsety = point.Y - center.Y;

                point.X = center.X + offsetx * scaling;
                point.Y = center.Y + offsety * scaling;

				_myStroke.Stroke.StylusPoints[j] = point;
            }
            
        }

        public void undo()
        {
			for (int j = 0; j < _myStroke.Stroke.StylusPoints.Count; j++)
            {
				StylusPoint point = _myStroke.Stroke.StylusPoints[j];
                double offsetx = point.X - center.X;
                double offsety = point.Y - center.Y;

                point.X = center.X + offsetx / scaling;
                point.Y = center.Y + offsety / scaling;

				_myStroke.Stroke.StylusPoints[j] = point;
            }
            
        }
    }
}
