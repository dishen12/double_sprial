using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFInk
{
    class Frame
    {
        public Bitmap frameBmp = null;
        public Bitmap sketchBmp = null;
        public double time = 0;


        public Bitmap nextFrameBmp = null;
        public Frame(Bitmap bmp, Bitmap nextBmp,double time)
        {
            frameBmp = bmp;
            nextFrameBmp = nextBmp;
            this.time = time;
        }
    }
}
