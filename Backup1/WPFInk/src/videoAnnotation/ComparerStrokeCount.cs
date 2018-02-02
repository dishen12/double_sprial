using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;

namespace WPFInk.video
{
	public class ComparerStrokeCount : IComparer<MyButton>

    {
        public int Compare(MyButton x, MyButton y)
        {
			return (x.InkFrame._inkCanvas.Strokes.Count + x.InkFrame._inkCanvas.Children.Count).CompareTo(y.InkFrame._inkCanvas.Strokes.Count + y.InkFrame._inkCanvas.Children.Count);
        }

    }
}
