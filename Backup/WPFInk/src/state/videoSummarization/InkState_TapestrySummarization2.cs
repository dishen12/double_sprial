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

namespace WPFInk.state
{
    public class InkState_TapestrySummarization2 : InkState
    {
        #region 私有变量
        private Point _startPoint;
        private Point _currPoint;
        private Point _prePoint;
        private int startIndex = 0;
        private int endIndex = 0;
        private ThumbPlayer thumbPlayer = null;
        int preIndex = int.MinValue;
        Image preImage = null;
        //视频摘要
        private VideoSummarization videoSummarization = null;
        //视频播放变量
        public System.Windows.Forms.Timer VideoPlayTimer;//播放视频过程中的Timer，用于超链接和摘要的显示
        public string videoSource = null;
        //记录操作时间变量
        private List<MyStrokeData> trackRecord = new List<MyStrokeData>();
        private MyStrokeData myStrokeData;
        private System.DateTime currentTime = new System.DateTime();
        private System.DateTime downTime;
        private System.DateTime upTime;
        private int currIndex = 0;
        //private List<KeyFrame> keyFrames;
        private VideoSummarizationControl VideoSummarizationControl;
        private Thickness inkCanvasSpiralSummarizationMargin;
        #endregion

        #region 封装变量
        public VideoSummarization VideoSummarization
        {
            get { return videoSummarization; }
            set { videoSummarization = value; }
        }
        public List<MyStrokeData> TrackRecord
        {
            get { return trackRecord; }
            set { trackRecord = value; }
        }
        #endregion
        #region 构造函数
        public InkState_TapestrySummarization2(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;          
        }

        
        #endregion

        

        #region 事件
        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.CaptureMouse();
            _startPoint = e.GetPosition(_inkCanvas);
            _prePoint = _startPoint;
            //记录操作类型与持续时间
            recordOperateTrace("TapestryDOWN");
            downTime = System.DateTime.Now;
            startIndex = VideoSummarization.getSelectedKeyFrameIndex(_startPoint);
        }



        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {                       
            if (this.VideoSummarizationControl == null)
            {
                this.VideoSummarizationControl = _inkCollector._mainPage.VideoSummarizationControl;
            }
            if (_startPoint.X != 0 && _startPoint.Y != 0 && startIndex!=int.MinValue)
            {
                _currPoint = e.GetPosition(_inkCanvas);
                double left=videoSummarization.InkCanvas.Margin.Left + _currPoint.X - _prePoint.X;
                if (left <= 0)
                {
                    videoSummarization.InkCanvas.Margin = new Thickness(left,
                        videoSummarization.InkCanvas.Margin.Top, 0, 0);
                }
                _prePoint = _currPoint;
                //记录操作类型与持续时间
                recordOperateTrace("TapestryMOVE");
            }
        }
        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (videoSummarization != null && _startPoint != null)// && _inkCanvas.Strokes.Count>0)
            {
                //记录操作类型与持续时间
                recordOperateTrace("TapestryUP");
                upTime = System.DateTime.Now;                
                _startPoint.X = 0;
                _startPoint.Y = 0;

            }
        }
        #endregion 

        #region 成员函数
        /// <summary>
        /// //记录操作类型与持续时间
        /// </summary>
        private void recordOperateTrace(string OperateType)
        {
            myStrokeData = new MyStrokeData();
            myStrokeData.Point = _currPoint;
            currentTime = System.DateTime.Now;
            myStrokeData.CurrentTime = currentTime.ToString("s") + ":" + currentTime.Millisecond.ToString();
            myStrokeData.OperateType = OperateType;
            trackRecord.Add(myStrokeData);
        }

        /// <summary>
        /// 记录操作事件与持续时间
        /// </summary>
        /// <param name="operateEvent"></param>
        private void recordOperateEvent(string operateEvent)
        {
            myStrokeData = new MyStrokeData();
            myStrokeData.CurrentTime = (upTime - downTime).ToString();
            myStrokeData.OperateType = operateEvent;
            trackRecord.Add(myStrokeData);
        }        
        #endregion
    }
}
