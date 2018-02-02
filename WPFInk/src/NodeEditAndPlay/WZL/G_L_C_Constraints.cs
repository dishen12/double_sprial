using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk.WZL
{
    public class G_L_C_Constraints
    {
        public bool L_C_Tangent()
        {

            return false;
        }

        public bool L_C_Intesect(G_Line L1, G_Cricle C1)
        {
            bool isStartPtInCircle = false;
            bool isEndPtInCircle = false;
            double dist;

            dist = Math.Sqrt(Math.Pow(C1.pCenter.X - L1.startPt.X, 2) + Math.Pow(C1.pCenter.Y - L1.startPt.Y, 2));
            if (dist < C1.radius)
                isStartPtInCircle = true;

            dist = Math.Sqrt(Math.Pow(C1.pCenter.X - L1.endPt.X, 2) + Math.Pow(C1.pCenter.Y - L1.endPt.Y, 2));
            if (dist < C1.radius)
                isEndPtInCircle = true;

            if (isStartPtInCircle && !isEndPtInCircle)
                return true;
            if (!isStartPtInCircle && isEndPtInCircle)
                return true;

            //穿过circle的情况
            if (!isEndPtInCircle && !isStartPtInCircle)
            {
                dist = System.Math.Sqrt(System.Math.Abs(L1.k * C1.pCenter.X - C1.pCenter.Y + L1.b) / (System.Math.Pow(L1.k, 2) + 1));
                if (dist < C1.radius)
                    return true;
            }

            return false;
        }

        public bool L_C_LineInCircle(G_Line L1, G_Cricle C1)
        {
            bool isStartPtInCircle = false;
            bool isEndPtInCircle = false;
            double dist;

            dist = Math.Sqrt(Math.Pow(C1.pCenter.X - L1.startPt.X, 2) + Math.Pow(C1.pCenter.Y - L1.startPt.Y, 2));
            if (dist < C1.radius)
                isStartPtInCircle = true;

            dist = Math.Sqrt(Math.Pow(C1.pCenter.X - L1.endPt.X, 2) + Math.Pow(C1.pCenter.Y - L1.endPt.Y, 2));
            if (dist < C1.radius)
                isEndPtInCircle = true;

            if (isStartPtInCircle && isEndPtInCircle)
                return true;

            return false;
        }
    }
}
