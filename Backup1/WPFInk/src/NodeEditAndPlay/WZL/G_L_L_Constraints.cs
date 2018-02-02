using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk.WZL
{
    public class G_L_L_Constraints : ConstraintElement
    {
        //YHY-090408

        public bool L_L_Vertical(G_Line pFirst, G_Line pSecond)
        {
            if (L_L_Parallel(pFirst, pSecond))
                return false;

            if ((System.Math.Abs(pFirst.angle - 90) < 0.1) && (System.Math.Abs(pSecond.angle - 0) < 0.1))
                return true;
            if ((System.Math.Abs(pFirst.angle - 0) < 0.1) && (System.Math.Abs(pSecond.angle - 90) < 0.1))
                return true;

            double tmp = pFirst.k * pSecond.k;

            if (System.Math.Abs(tmp + 1) < VerticalTreshold)
                return true;
            return false;
        }

        public bool L_L_Intersect(G_Line pFirst, G_Line pSecond)
        {
            if (!L_L_Parallel(pFirst, pSecond))
            {
                double tmp = (pSecond.b - pFirst.b) / (pFirst.k - pSecond.k);
                double ld;
                double rd;
                double mld;
                double mrd;
                if (pFirst.startPt.X < pFirst.endPt.X)
                {
                    ld = pFirst.startPt.X;
                    rd = pFirst.endPt.X;
                }
                else
                {
                    ld = pFirst.endPt.X;
                    rd = pFirst.startPt.X;
                }
                mld = ld;
                mrd = rd;
                if (pSecond.startPt.X < pSecond.endPt.X)
                {
                    ld = pSecond.startPt.X;
                    rd = pSecond.endPt.X;
                }
                else
                {
                    ld = pSecond.endPt.X;
                    rd = pSecond.startPt.X;
                }
                if (ld > mld)
                    mld = ld;
                if (rd < mrd)
                    mrd = rd;
                if ((tmp > mld) && (tmp < mrd))
                    return true;
            }
            return false;
        }

        public bool L_L_IntersectVertex(G_Line pFirst, G_Line pSecond)
        {
            double dist = new ToolForStroke().distanceP2P(pFirst.startPt, pSecond.startPt);
            if (dist < 500)
                return true;

            dist = new ToolForStroke().distanceP2P(pFirst.startPt, pSecond.endPt);
            if (dist < 500)
                return true;

            dist = new ToolForStroke().distanceP2P(pFirst.endPt, pSecond.startPt);
            if (dist < 500)
                return true;

            dist = new ToolForStroke().distanceP2P(pFirst.endPt, pSecond.endPt);
            if (dist < 500)
                return true;

            return false;
        }

        public bool L_L_Angel()
        {

            return false;
        }

        public bool L_L_Parallel(G_Line pFirst, G_Line pSecond)
        {
            //TODO::目前是两个直线的平行判定，不是两个平行线的连动约束判断；
            if (System.Math.Abs(pFirst.k - pSecond.k) < ParallelThreshold)
                return true;
            return false;
        }

        public double ParallelThreshold = 0.5; //TODO:平行阈值确定
        public double VerticalTreshold = 2;
    }
}
