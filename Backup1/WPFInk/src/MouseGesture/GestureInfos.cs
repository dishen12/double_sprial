﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;

namespace WPFInk.mouseGesture
{
	/// <summary>
	/// 手势信息，用于MatchHandler回调
	/// </summary>
	public class GestureInfos
	{
		private int cost;
		private GestureData data;
		private string present;
		public GestureInfos(GestureData data)
		{
			this.data = data;
		}
		/// <summary>
		/// 鼠标手势显示
		/// </summary>
		public string Present
		{
			get
			{
				return this.present;
			}
			set
			{
				this.present = value;
			}
		}
		/// <summary>
		/// 获取或设置鼠标手势数据
		/// </summary>
		public GestureData Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}
		public int Cost
		{
			get
			{
				return this.cost;
			}
			set
			{
				this.cost = value;
			}
		}


	}
}
