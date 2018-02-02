using System;
using System.Collections.Generic;
using System.Text;

namespace WPFInk.mouseGesture
{
	class GenericRect
	{
		public int MinX { get; set; }
		public int MaxX { get; set; }
		public int MinY { get; set; }
		public int MaxY { get; set; }
		public GenericRect(int minx, int maxx, int miny, int maxy)
		{
			MinX = minx;
			MaxX = maxx;
			MinY = miny;
			MaxY = maxy;
		}
	}
}
