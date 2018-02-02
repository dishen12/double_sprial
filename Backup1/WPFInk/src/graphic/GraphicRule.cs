using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk.graphic
{	
	public enum GraphicRule
	{
		None=0,
		/// <summary>
		/// 相切
		/// </summary>
		Tangent=1,//相切

		/// <summary>
		/// 平行
		/// </summary>
		Parallel=2,//平行
		/// <summary>
		/// 头相交
		/// </summary>
		HeadIntersect=3,
        /// <summary>
        /// 尾相交
        /// </summary>
        TailIntersect=4,
		/// <summary>
		/// 相交
		/// </summary>
        Intersect = 5,
        /// <summary>
        /// 自循环
        /// </summary>
        LoopSelf=6,
        /// <summary>
        /// 想离
        /// </summary>
        Deviation
	}
}
