using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for TimeBar.xaml
	/// </summary>
	public partial class TimeBar : UserControl
	{
		public TimeBar()
		{
            this.InitializeComponent();
        }

        #region 变量
        //接收外部参数的变量
        private double totalTime;　　　　　　　　 //接收视频总时间
        private double show_StartTime;　　　　　　//接收屏幕呈现区域的视频起点
        private double show_EndTime;　　　　　　　//接收屏幕呈现区域的视频终点
        #endregion 

        #region 封装变量
        public double TotalTime
        {
            get { return totalTime; }
            set { totalTime = value; }
        }

        public double Show_StartTime
        {
            get { return show_StartTime; }
            set { show_StartTime = value; }
        }

        public double Show_EndTime
        {
            get { return show_EndTime; }
            set { show_EndTime = value; }
        }
        #endregion

        public void computeLocation()                                                   //动态刷新移动轴标位置与大小
        {　　                                  
            double rate = (show_EndTime-show_StartTime) / totalTime;                     //计算时间与长度的比例因子
            ShowTimeBar.Width = Math.Abs(this.Width * rate);　　　　　//移动轴标的长度
            double left=show_StartTime / totalTime*this.Width;
            left = left < 0 ? 0 : left;
            left = left > this.Width - ShowTimeBar.Width ? this.Width - ShowTimeBar.Width : left;
            ShowTimeBar.Margin = new Thickness(left, 9, 0, 0); 　　　 //移动轴标的位置
        }

    }
}