using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;   
using System.Windows;
using System.Windows.Shapes;
using WPFInk.tool;
using System.Windows.Ink;
using WPFInk.ink;
using System.Drawing.Drawing2D;

namespace WPFInk.video
{
    public class ThumbConnector
    {
        private MyButton source;
        private MyButton target;
        public Path arrow;
        public Point startPoint;
        public Point endPoint;

        public ThumbConnector(MyButton source, MyButton target)
        {
            this.source = source;
            this.target = target;
            Connector c = new Connector();

            arrow = c.getThumbConnector(source, target);
            startPoint = c.StartPoint;
            endPoint = c.EndPoint;
        }
    }
}
