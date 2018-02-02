using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFInk
{
    public class Link
    {
        public int StartPtIndex;
        public int EndPtIndex;
        public float radius;
        public int index = 0;
        public int weight = 0;
        public Point farthestPoint;


        public Color Color = Color.Red;

        public Link(int startIndex, int endIndex, float r)
        {
            StartPtIndex = startIndex;
            EndPtIndex = endIndex;
            radius = r;
        }
    }
}
