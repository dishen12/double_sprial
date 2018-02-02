using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Shapes;

namespace WPFInk.mouseGesture
{
	/// <summary>
	/// 鼠标手势数据
	/// </summary>
	public class GestureData
	{
		private int[] moves;
        private System.Windows.Input.StylusPoint[] points;
        private System.Windows.Input.StylusPoint lastPoint;
		private Rectangle rect;

        public GestureData(int[] moves, System.Windows.Input.StylusPoint[] points, System.Windows.Input.StylusPoint lastPoint, Rectangle rect)
		{
			this.moves = moves;
			this.points = points;
			this.lastPoint = lastPoint;
			this.rect = rect;
		}
		/// <summary>
		/// 获取或设置鼠标手势
		/// </summary>
		public int[] Moves
		{
			get
			{
				return this.moves;
			}
			set
			{
				this.moves = value;
			}
		}
		/// <summary>
		/// 获取或设置鼠标点
		/// </summary>
        public System.Windows.Input.StylusPoint[] Points
		{
			get
			{
				return this.points;
			}
			set
			{
				this.points = value;
			}
		}
		/// <summary>
		/// 获取或设置最后一次鼠标位置
		/// </summary>
        public System.Windows.Input.StylusPoint LastPoint
		{
			get
			{
				return this.lastPoint;
			}
			set
			{
				this.lastPoint = value;
			}
		}
		/// <summary>
		/// 获取或设置鼠标手势轨迹大小
		/// </summary>
		public Rectangle Rect
		{
			get
			{
				return this.rect;
			}
			set
			{
				this.rect = value;
			}
		}
	}
}
