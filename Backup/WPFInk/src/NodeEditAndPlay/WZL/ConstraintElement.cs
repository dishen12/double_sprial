using System;
using System.Collections.Generic;
using System.Text;

namespace WPFInk.WZL
{
    public enum ConstraintType
    {
        Intersect,
        Parallel,
        Vertical,
        Tangent,
        In,
        Horizontal
    }
    public class ConstraintElement
    {
        public StrokeGroup strokeGroup1;
        public StrokeGroup strokeGroup2;
        public ConstraintType ctype;

        public List<ConstraintElement> m_pNextforfirst = new List<ConstraintElement>();
        public List<ConstraintElement> m_pNextforsecond = new List<ConstraintElement>();
    }
}
