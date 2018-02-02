using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using WPFInk.videoSummarization;
using WPFInk.ink;
using WPFInk.tool;
using WPFInk.mouseGesture;
using System.Diagnostics;
using WPFInk.Global;

namespace WPFInk.videoSummarization
{
    public class VideoSummarizationTool
    {
        
        /// <summary>
        /// 关键帧原帧toImage
        /// </summary>
        /// <param name="images"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Image KeyFrameToImage(KeyFrame keyFrame)
        {
            Image image = InkConstants.getImageFromName(keyFrame.ImageName);
            image.Width = 120;
            image.Height = 80;
            return image;
        }
        /// <summary>
        /// 定位视频
        /// </summary>
        /// <param name="videoTime"></param>
        public static void locateMediaPlayer(MediaElement videoPlayer,KeyFrame keyFrame)
        {
            videoPlayer.Source = new Uri(keyFrame.VideoName);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, keyFrame.Time);
            videoPlayer.Position = ts;
            videoPlayer.Play();
        }
        /// <summary>
        /// 定位视频
        /// </summary>
        /// <param name="videoTime"></param>
        public static void locateMediaPlayer(MediaElement videoPlayer,string VideoName, int videoTime)
        {
            videoPlayer.Source = new Uri(VideoName);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, videoTime);
            videoPlayer.Position = ts;
            videoPlayer.Play();
        }
    }
}
