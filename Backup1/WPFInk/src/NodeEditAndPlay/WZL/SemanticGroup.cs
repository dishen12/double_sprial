using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFInk.WZL
{
    public class SemanticGroup
    {

        public List<StrokeGroup> groupList = new List<StrokeGroup>();
        public int semanticGroupID;
        public Rectangle boundingBox = new Rectangle();
        public int nCountLine = 0;
        public int nCountCircle = 0;
        public int nCountCurve = 0;




        public void setBoundingBox()
        {
            boundingBox = new Rectangle(1000000, 1000000, -2000000, -2000000);
            int Left = boundingBox.Left;
            int Right = boundingBox.Right;
            int Top = boundingBox.Top;
            int Bottom = boundingBox.Bottom;

            foreach (StrokeGroup sg in groupList)
            {
                Rectangle rect = sg.boundingBox;
                if (rect.Left < Left)
                    Left = rect.Left;
                if (rect.Right > Right)
                    Right = rect.Right;
                if (rect.Top < Top)
                    Top = rect.Top;
                if (rect.Bottom > Bottom)
                    Bottom = rect.Bottom;
            }
            boundingBox = new Rectangle(Left, Top, System.Math.Abs(Right - Left), System.Math.Abs(Bottom - Top));
        }

    }
}
