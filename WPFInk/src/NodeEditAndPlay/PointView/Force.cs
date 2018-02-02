using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk
{
    public class Force
    {
        private double x = 0.0;
        private double y = 0.0;
        public Force(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public void addAForce(Force fToAdd)
        {
            x += fToAdd.x;
            y += fToAdd.y;
        }
    }
}
