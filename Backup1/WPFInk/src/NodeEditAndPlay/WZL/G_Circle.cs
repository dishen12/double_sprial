using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFInk.WZL
{
    public class G_Cricle : G_Geometry
    {
        public Point pCenter;
        public double radius;
        public double m_dMaxDistance;

        public G_Cricle(Point p, double r)
        {
            pCenter.X = p.X;
            pCenter.Y = p.Y;
            radius = r;
            m_dMaxDistance = 0;
        }

    }
}
