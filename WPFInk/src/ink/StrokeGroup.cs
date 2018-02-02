using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using WPFInk.tool;

namespace WPFInk.ink
{
    /// <summary>
    /// 定义了group的不同类型
    /// </summary>
    public enum GroupType
    {
        Overtracing,
        Default
    }

    /// <summary>
    /// 定义了识别后的形状
    /// </summary>
    public enum Graphic
    {
        Line,
        Circle
    }

    /// <summary>
    /// 定义了Strokegroup类
    /// 多个stroke组成一个group
    /// </summary>
    public class StrokeGroup
    {
        /// <summary>
        /// 创建group‘的时候确定group的类型，并记录开始时间
        /// </summary>
        /// <param name="type"></param>
        public StrokeGroup(GroupType type)
        {
            this.TYPE = type;
            this.StartTime = MyTimer.getInstance().getTime();
        }

        /// <summary>
        /// 在group中增加一个Stroke
        /// </summary>
        /// <param name="stroke"></param>
        public void addStroke(MyStroke stroke)
        {
            this.strokeList.Add(stroke);
            stroke.IsInGroup = true;
            stroke.Group = this;
        }

        /// <summary>
        /// 将stroke从group中删除
        /// </summary>
        /// <param name="stroke"></param>
        public void removeStroke(MyStroke stroke)
        {
            this.strokeList.Remove(stroke);
            stroke.Group = null;
            stroke.IsInGroup = false;
        }

        public GroupType TYPE = 0;
        public int StartTime;
        public List<MyStroke> strokeList = new List<MyStroke>();

        public double m, c;//skeleton的两个参数
    }
}
