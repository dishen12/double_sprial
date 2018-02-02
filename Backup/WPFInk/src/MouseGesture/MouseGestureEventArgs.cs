using System;
using System.Collections.Generic;
using System.Text;

namespace WPFInk.mouseGesture
{
	/// <summary>
	/// 鼠标手势事件参数
	/// </summary>
	public class MouseGestureEventArgs : EventArgs
	{
		/// <summary>
		/// 包含的数据
		/// </summary>
		public string Present { get; set; }
		/// <summary>
		/// 有效性
		/// </summary>
		public int Fiability { get; set; }
	}
}
