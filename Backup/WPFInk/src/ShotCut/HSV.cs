using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk.ShotCut
{
    class HSV
    {
        public float h = 0;
        public float s = 0;
        public float v = 0;

        public HSV(float fh, float fs, float fv)
        {
            h = fh;
            s = fs;
            v = fv;
        }
    }
}
