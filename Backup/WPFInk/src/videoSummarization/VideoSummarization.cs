using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Text;
using WPFInk.ink;
using WPFInk.tool;
using System.Windows;
using System.Windows.Media;


namespace WPFInk.videoSummarization
{
    public abstract class VideoSummarization
    {
        #region 私有变量
        //protected List<Image> images = new List<Image>();//视频摘要关键帧图片列表  
        protected List<KeyFrame> keyFrames = new List<KeyFrame>();//视频摘要关键帧列表  
        private InkCanvas _inkCanvas;//显示螺旋式摘要的画板
        protected StylusPointCollection keyPoints = new StylusPointCollection();//关键点列表
        protected List<int> keyPointIndexes = new List<int>();
        protected Stroke trackStroke;//用户自定义的视频摘要轨迹
        protected double trackWidth;//轨迹宽度，在螺旋摘要中就是螺宽
        protected int keyFrameCount;//关键帧个数
        protected List<KeyFrame> showKeyFrames = new List<KeyFrame>();//显示出来的关键帧
        protected List<Point> showKeyFrameCenterPoints = new List<Point>();///显示出来的关键帧中心点列表
        protected double showWidth = 150;
        protected double showHeight = 100;
        private StylusPoint center;//如果是螺旋摘要，则表示螺旋线的中心点，否则表示平铺摘要的左上角点
        protected SolidArrow solidArrow = null;
        #endregion

        public SolidArrow SolidArrow
        {
            get { return solidArrow; }
            set { solidArrow = value; }
        }

        #region 构造函数
        public VideoSummarization(Stroke trackStroke,double trackWidth, List<KeyFrame> keyFrames, InkCanvas _inkCanvas)
        {
            this.trackStroke = trackStroke;
            this._inkCanvas = _inkCanvas;
            this.keyFrames = keyFrames;
            this.trackWidth = trackWidth;
            this.keyFrameCount = keyFrames.Count;
            //for (int i = 0; i < keyFrames.Count; i++)
            //{
            //    //this.images.Add(InkConstants.getImageFromName(keyFrames[i].ImageName));
            //    MathTool.resizeImage(keyFrames[i], showWidth, showHeight);
            //}

        }
        #endregion
        
        #region 抽象函数
        /// <summary>
        /// 向轨迹中添加图片
        /// </summary>
        public abstract void addImages2Track();
        /// <summary>
        /// 获取当前选择帧的索引值
        /// </summary>
        /// <param name="point">选择点</param>
        /// <returns></returns>
        public abstract int getSelectedKeyFrameIndex(Point point);//获取当前选择的关键帧
        /// <summary>
        /// 向显示轨迹上添加圆点代表播放到该关键帧,表明播放进度
        /// </summary>
        /// <param name="index">关键帧索引</param>
        /// <param name="color">原点颜色</param>
        /// <param name="strokeWidthOffset">原点宽度相对应轨迹的差异</param>
        /// <returns></returns>
        public abstract Stroke AddPoint2Track(int index, Color color, double strokeWidthOffset);//向摘要中添加点表示播放进度
        #endregion

        #region 封装变量

        public StylusPoint Center
        {
            get { return center; }
            set { center = value; }
        }

        public double ShowHeight
        {
            get { return showHeight; }
            set { showHeight = value; }
        }

        public double ShowWidth
        {
            get { return showWidth; }
            set { showWidth = value; }
        }
        public List<Point> ShowKeyFrameCenterPoints
        {
            get { return showKeyFrameCenterPoints; }
            set { showKeyFrameCenterPoints = value; }
        }
        public List<KeyFrame> ShowKeyFrames
        {
            get { return showKeyFrames; }
            set { showKeyFrames = value; }
        }
        public InkCanvas InkCanvas
        {
            get { return _inkCanvas; }
            set { _inkCanvas = value; }
        }

        public List<int> KeyPointIndexes
        {
            get { return keyPointIndexes; }
            set { keyPointIndexes = value; }
        }
        public StylusPointCollection KeyPoints
        {
            get { return keyPoints; }
            set { keyPoints = value; }
        }
        /// <summary>
        /// 关键帧
        /// </summary>
        public List<KeyFrame> KeyFrames
        {
            get { return keyFrames; }
            set { keyFrames = value; }
        }

        /// <summary>
        /// 用户自定义的视频摘要轨迹
        /// </summary>
        public Stroke TrackStroke
        {
            get { return trackStroke; }
            set { trackStroke = value; }
        }

        public double TrackWidth
        {
            get { return trackWidth; }
            set { trackWidth = value; }
        }

        ///// <summary>
        ///// 还原图片
        ///// </summary>
        ///// <param name="index">图片索引值</param>
        //public void restoreImage(int index)
        //{
        //    images[index] = InkConstants.getImageFromName(keyFrames[index].ImageName);
        //}
        #endregion

        #region 普通函数
        //hide Tilesummarization
        public void hideSummarization()
        {
            this.InkCanvas.Visibility = Visibility.Collapsed;
        }
        //show Tilesummarization
        public void showSummarization()
        {
            this.InkCanvas.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
