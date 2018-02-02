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
	/// 播放按钮的类型
	/// </summary>
	public enum StrokePlayButton
	{
		StartPlay,
		GoOnPlay,
		StopPlay
	}

	/// <summary>
	/// Interaction logic for StrokePlayer.xaml
	/// </summary>
	public partial class StrokePlayer : UserControl
	{
        //观看某一时刻的草图
       // public delegate void timeOperate(double propotion);
       // public event timeOperate OnTimeChanged;
        //观看草图的播放过程
        public delegate void startPlay();
		public event startPlay OnStart;
		
		public delegate void timeBar_OnSuspend();
		
        
		//结束播放
		public event startPlay OnEnd;
		//暂停播放
		public event startPlay OnStop;
        //指示时间轴调整是否按下
        //private bool isTimeSelect = false;
		
		private StrokePlayButton _playbutton;

		public StrokePlayer()
		{
			this.InitializeComponent();
		}
        
        public StrokePlayButton StrokePlayButton
		{
			get
			{
				return _playbutton;
			}
			set
			{
				_playbutton = value;
				switch(value)
				{
					case StrokePlayButton.StartPlay:
						this.PlayButton.Visibility = Visibility.Visible;
            			this.StopButton.Visibility = Visibility.Collapsed;
						this.GoOnButton.Visibility = Visibility.Collapsed;   
						break;
					case StrokePlayButton.GoOnPlay:
						this.GoOnButton.Visibility = Visibility.Visible;
            			this.StopButton.Visibility = Visibility.Collapsed;
						this.PlayButton.Visibility = Visibility.Collapsed;
						break;
					case StrokePlayButton.StopPlay:
						this.StopButton.Visibility = Visibility.Visible;
            			this.GoOnButton.Visibility = Visibility.Collapsed;
						this.PlayButton.Visibility = Visibility.Collapsed;
						break;
					
				}						
			}
		}

		/// <summary>
		/// 设置播放进度
		/// </summary>
		/// <param name="x"></param>
		public void setProgress(double x)
		{
			this.ProgressSlider.Value = x;
		}

		private void ProgressButton_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			//OnTimeChanged(this.ProgressButton.Value);
		}

		/// <summary>
		/// 结束播放
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EndButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.StopButton.Visibility = Visibility.Collapsed;
            this.PlayButton.Visibility = Visibility.Visible;
			OnEnd();
		}

		

		//开始播放
		private void PlayButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OnStart();	
		}
		//继续播放
		private void GoonButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OnStop();	
		}

		//暂停播放
		private void StopButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{   			
			OnStop();
		}
	}
}


