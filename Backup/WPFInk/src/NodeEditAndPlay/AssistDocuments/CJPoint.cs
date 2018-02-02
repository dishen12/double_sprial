using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk
{
    public class CJPoint
    {
        double x;

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        double y;

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public CJPoint(double dx, double dy)
        {
            x = dx;
            y = dy;
        }
    }
}
