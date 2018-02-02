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
	/// Interaction logic for ThumbVideoPlayer.xaml
	/// </summary>
	public partial class ThumbVideoPlayer : UserControl
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private int startTime;//起始播放时间,以毫秒为单位
        private bool isLoop = true;//是否循环播放视频片段，true代表循环
		public ThumbVideoPlayer()
		{
			this.InitializeComponent();
		}
        public void InitVideoPlayer(string videoName, int startTime, int duration, bool isLoop)//,InkCanvas _inkCanvas)
        {
            //videoPlayer = new MediaElement();
            //videoPlayer.LoadedBehavior = MediaState.Manual;
            videoPlayer.Source = new Uri(videoName);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, startTime);
            videoPlayer.Position = ts;
            timer.Interval = duration;
            timer.Tick += new System.EventHandler(this.timer_Tick);
            this.isLoop = isLoop;
            this.startTime = startTime;
            videoPlayer.Play();
            timer.Start();
        }

        #region 成员函数
        private void timer_Tick(object sender, EventArgs e)
        {
            if (isLoop)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, startTime);
                videoPlayer.Position = ts;
                videoPlayer.Play();
            }
            else
            {
                timer.Stop();
                videoPlayer.Pause();
            }

        }
        #endregion
	}
}