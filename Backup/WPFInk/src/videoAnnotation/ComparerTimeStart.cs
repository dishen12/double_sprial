using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;

namespace WPFInk.video
{
	public class ComparerTimeStart : IComparer<MyButton>

    {
        public int Compare(MyButton x, MyButton y)
        {
            return x.TimeStart.CompareTo(y.TimeStart);
        }

    }
}
