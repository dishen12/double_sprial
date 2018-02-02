using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFInk
{
    /// <summary>
    /// PlayerPolyTimeline.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerPolyTimeline : Window
    {
        public PlayerPolyTimeline()
        {
            InitializeComponent();
            _myTimeLine.setVideoPlayer(videoPlayer);
        }

        private void videoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (videoPlayer.NaturalDuration.HasTimeSpan)//
            {
                _myTimeLine.KeyframeLineUnit = (_myTimeLine.Width - 4 )/ videoPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                _myTimeLine.exec();
            }
        }
    }
}
