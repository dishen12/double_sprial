using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Ink;
using WPFInk.ink;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WPFInk.videoSummarization
{
    public class KeyFrame
    {
        #region 构造函数
        public KeyFrame(string videoName, string imageName, int time)
        {
            this.videoName = videoName;
            this.imageName = imageName;
            this.time = time;
            //this.bitmap =new System.Drawing.Bitmap(imageName);
        }
        #endregion

        #region 私有变量
        private string imageName = null;//关键帧图片路径名称        
        private int time = 0;//关键帧对应的视频时间,以毫秒为单位
        private KeyFramesAnnotation keyFramesAnnotation = new KeyFramesAnnotation();
        private Dictionary<Stroke, KeyFramesAnnotation> annotations = new Dictionary<Stroke, KeyFramesAnnotation>();//关键帧注释信息
        private string videoName = null;
        private Image image = null;
        private BitmapSource bitmapSource;
        private KeyFrame hyperLink = null;
        private SpiralSummarization hyperLinkSpiralSummarization = null;
        private Stroke spiralStrokeClip = null;
        private int pointIndexInSpiral = 0;//对应螺旋线中点的下标值
        private bool isVisible = true;

        private Image counterPartImage = null;
        private bool level = false;
        private bool isVisibleInTile = false;
        private int id = 0;
        private List<int> similarKeyFrames = new List<int>();//相似关键帧，用作语义理解
        private List<int> distance1ToKeyFrames = new List<int>();//聚类中距离为1的关键帧集合
        private int bunchNo = 0;//关键帧所在的聚类编号
        private Image grayImage = null;
        //以下用作折线时间轴
        private int bunchLayer = 0;//聚类所在层,从1开始，1代表是中心聚类
        private int bigBunchNo = 0;//对聚类结果进行再次聚类得到大聚类，大聚类的编号

        //private StrokeCollection line = new StrokeCollection();
        //private StrokeCollection counterPartLine = new StrokeCollection();

        #endregion
        #region 封装变量
        /// <summary>
        /// 对聚类结果进行再次聚类得到大聚类，大聚类的编号
        /// </summary>
        public int BigBunchNo
        {
            get { return bigBunchNo; }
            set { bigBunchNo = value; }
        }
        /// <summary>
        /// 聚类所在层
        /// </summary>
        public int BunchLayer
        {
            get { return bunchLayer; }
            set { bunchLayer = value; }
        }
        /// <summary>
        /// 关键帧所在的聚类编号
        /// </summary>
        public int BunchNo
        {
            get { return bunchNo; }
            set { bunchNo = value; }
        }
        
        /// <summary>
        /// 聚类中距离为1的关键帧集合
        /// </summary>
        public List<int> Distance1ToKeyFrames
        {
            get { return distance1ToKeyFrames; }
            set { distance1ToKeyFrames = value; }
        }
        /// <summary>
        /// 对应黑白图片
        /// </summary>
        public Image GrayImage
        {
            get { return grayImage; }
            set { grayImage = value; }
        }
        /// <summary>
        /// 相似关键帧，用作语义理解
        /// </summary>
        public List<int> SimilarKeyFrames
        {
            get { return similarKeyFrames; }
            set { similarKeyFrames = value; }
        }
        //public StrokeCollection CounterPartLine
        //{
        //    get { return counterPartLine; }
        //    set { counterPartLine = value; }
        //}


        //public StrokeCollection Line
        //{
        //    get { return line; }
        //    set { line = value; }
        //}

        public Image CounterPartImage
        {
            get { return counterPartImage; }
            set { counterPartImage = value; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        public bool IsVisibleInTile
        {
            get { return isVisibleInTile; }
            set { isVisibleInTile = value; }
        }

        public bool Level
        {
            get { return level; }
            set { level = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public BitmapSource BitmapSource
        {
            get { return bitmapSource; }
            set { bitmapSource = value; }
        }
       
        /// <summary>
        /// 对应螺旋线中点的下标值
        /// </summary>
        public int PointIndexInSpiral
        {
            get { return pointIndexInSpiral; }
            set { pointIndexInSpiral = value; }
        }
        /// <summary>
        /// 该关键帧对应的螺旋轨迹
        /// </summary>
        public Stroke SpiralStrokeClip
        {
            get { return spiralStrokeClip; }
            set { spiralStrokeClip = value; }
        }

        public SpiralSummarization HyperLinkSpiralSummarization
        {
            get { return hyperLinkSpiralSummarization; }
            set { hyperLinkSpiralSummarization = value; }
        }
        public KeyFrame HyperLink
        {
            get { return hyperLink; }
            set { hyperLink = value; }
        }
        //public List<KeyFrame> HyperLinks
        //{
        //    get { return hyperLinks; }
        //    set { hyperLinks = value; }
        //}

        /// <summary>
        /// 关键帧图片的高度
        /// </summary>
        public double Height
        {
            get { return image.Height; }
        }
        /// <summary>
        /// 关键帧图片的宽度
        /// </summary>
        public double Width
        {
            get { return image.Width; }
        }
        /// <summary>
        /// 关键帧图片的上边距
        /// </summary>
        public double Top
        {
            get { return image.Margin.Top; }
        }
        /// <summary>
        /// 关键帧图片的左边距
        /// </summary>
        public double Left
        {
            get { return image.Margin.Left; }
        }
        /// <summary>
        /// 关键帧图片
        /// </summary>
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        /// <summary>
        /// 关键帧图片路径名称
        /// </summary>
        public string ImageName
        {
            get { return imageName; }
            set { imageName = value; }
        }

        /// <summary>
        /// 关键帧对应的视频时间
        /// </summary>
        public int Time
        {
            get { return time; }
            set { time = value; }
        }

        /// <summary>
        /// 关键帧注释
        /// </summary>

        public Dictionary<Stroke, KeyFramesAnnotation> Annotations
        {
            get { return annotations; }
            set { annotations = value; }
        }
        /// <summary>
        /// 视频路径名称
        /// </summary>
        public string VideoName
        {
            get { return videoName; }
            set { videoName = value; }
        }
        public Visibility Visibility
        {
            get { return image.Visibility; }
        }
        #endregion

        #region 成员函数
        public KeyFrame convertToNewKeyFrame()
        {
            KeyFrame keyFrame = new KeyFrame(this.videoName, this.imageName, this.time);
            keyFrame.Image = InkConstants.getImageFromName(this.imageName);
            return keyFrame;
        }
        public void reLoadImage()
        {
            this.image = InkConstants.getImageFromName(imageName);
        }
        public void hiddenImage()
        {
            this.image.Visibility = Visibility.Collapsed;
        }
        public void showImage()
        {
            this.image.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 渐隐
        /// </summary>
        public void fadeOut()
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 1;
            opacityAnimation.To = 0;
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            Storyboard.SetTarget(opacityAnimation, this.image);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Image.OpacityProperty));
            Storyboard fadeout = new Storyboard();
            fadeout.Children.Add(opacityAnimation);
            fadeout.Begin();

        }
        /// <summary>
        /// 渐现
        /// </summary>
        public void fadeIn()
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0;
            opacityAnimation.To = 1;
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            Storyboard.SetTarget(opacityAnimation, this.image);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Image.OpacityProperty));
            Storyboard fadeout = new Storyboard();
            fadeout.Children.Add(opacityAnimation);
            fadeout.Begin();

        }

        #endregion
    }
}
