using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace WPFInk.videoSummarization
{
    /// <summary>
    /// 缩略播放器，和摘要同时使用
    /// </summary>
    public class ThumbPlayer
    {
        #region 构造函数
        /// <summary>
        /// 缩略播放器类，和摘要同时使用
        /// </summary>
        /// <param name="videoName">视频路径名称</param>
        /// <param name="width">播放器宽度</param>
        /// <param name="height">播放器高度</param>
        /// <param name="left">播放器左边距</param>
        /// <param name="top">播放器右边距</param>
        /// <param name="startTime">起始播放时间,以毫秒为单位</param>
        /// <param name="duration">播放持续时间</param>
        /// <param name="isLoop">是否循环播放视频片段，true代表循环</param>
        /// <param name="_inkCanvas">显示画板</param>
        public ThumbPlayer(string videoName,double width,double height,double left,double top,int startTime,int duration,bool isLoop)//,InkCanvas _inkCanvas)
        {
            videoPlayer = new MediaElement();
            videoPlayer.LoadedBehavior = MediaState.Manual;
            videoPlayer.Width = width;
            videoPlayer.Height = height;
            videoPlayer.Margin=new System.Windows.Thickness(left,top,0,0);
            videoPlayer.Source = new Uri(videoName);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, startTime);
            videoPlayer.Position = ts;
            //_inkCanvas.Children.Add(videoPlayer);
            timer.Interval = duration;
            timer.Tick += new System.EventHandler(this.timer_Tick);
            this.isLoop = isLoop;
            this.startTime = startTime;
            videoPlayer.Play();
            timer.Start();
        }
        #endregion

        #region 私有变量
        private MediaElement videoPlayer;//视频播放器
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        //private string videoName;//视频路径名称
        //private double width;//播放器宽度
        //private double height;//播放器高度
        //private double left;//播放器左边距
        //private double top;//播放器右边距
        private int startTime;//起始播放时间,以毫秒为单位
        //private int endTime;//终止播放时间以毫秒为单位
        private bool isLoop=true;//是否循环播放视频片段，true代表循环

        #endregion

        #region 封装变量
        public MediaElement VideoPlayer
        {
            get { return videoPlayer; }
            set { videoPlayer = value; }
        }

        //public string VideoName
        //{
        //    get { return videoName; }
        //    set { videoName = value; }
        //}

        //public double Width
        //{
        //    get { return width; }
        //    set { width = value; }
        //}

        //public double Height
        //{
        //    get { return height; }
        //    set { height = value; }
        //}

        //public double Left
        //{
        //    get { return left; }
        //    set { left = value; }
        //}

        //public double Top
        //{
        //    get { return top; }
        //    set { top = value; }
        //}
 

        #endregion

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
