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
    public class InkState_AddKeyFrameAnnotation : InkState
    {
        #region 私有变量
        private Point _startPoint;
        private Point _currPoint;
        private int startIndex = 0;
        private ThumbPlayer thumbPlayer = null;
        Image preImage = null;
        //视频摘要
        private VideoSummarization videoSummarization;

        public VideoSummarization VideoSummarization
        {
            get { return videoSummarization; }
            set { videoSummarization = value; }
        }
        private KeyFrame selectKeyFrame;
        private Point selectKeyFrameCenterPoint;
        private Thickness inkCanvasSpiralSummarizationMargin;
        #endregion

        #region 构造函数
        public InkState_AddKeyFrameAnnotation(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            this.videoSummarization = inkCollector.VideoSummarization;
        }

        
        #endregion

        #region 封装变量
        
        #endregion

        #region 事件
        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.CaptureMouse();
            _startPoint = e.GetPosition(_inkCanvas);
            //记录操作类型与持续时间
            
            //this.spiralSummarization = _inkCollector.SpiralSummarization;
            //_inkCollector.KeyFrames = _inkCollector.SpiralSummarization.KeyFrames;
            inkCanvasSpiralSummarizationMargin = ((InkCanvas)_inkCanvas.Children[0]).Margin;
            //纠正，在全屏时不纠正就会出错
            _startPoint.X -= inkCanvasSpiralSummarizationMargin.Left;
            _startPoint.Y -= inkCanvasSpiralSummarizationMargin.Top;
            startIndex = videoSummarization.getSelectedKeyFrameIndex(_startPoint);//, spiralSummarization);
            if (startIndex != int.MinValue)
            {
                this.selectKeyFrameCenterPoint = videoSummarization.ShowKeyFrameCenterPoints[startIndex];
                selectKeyFrame = videoSummarization.ShowKeyFrames[startIndex];
            }
        }



        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (videoSummarization != null && _startPoint != null && _inkCanvas.Strokes.Count > 0)
            {
                _currPoint = e.GetPosition(_inkCanvas);
                Stroke lastStroke = _inkCanvas.Strokes.Last();
                _inkCanvas.Strokes.Remove(lastStroke);
                if ( _inkCollector.KeyFramesAnnotation.relatedKeyFrameIndexes.IndexOf(startIndex)==-1&&
                    startIndex!=int.MinValue&& isInAnnotationBox(_currPoint))
                {
                    Stroke linkLine;
                    if (_inkCollector.KeyFrameAnnotation.HorizontalAlignment == HorizontalAlignment.Left)
                    {
                        linkLine = InkTool.getInstance().DrawLine(selectKeyFrameCenterPoint.X + inkCanvasSpiralSummarizationMargin.Left,
                                selectKeyFrameCenterPoint.Y + inkCanvasSpiralSummarizationMargin.Top,
                                _inkCollector.KeyFrameAnnotation.Margin.Left + _inkCollector.KeyFrameAnnotation.Width / 2,
                                _inkCollector.KeyFrameAnnotation.Margin.Top + _inkCollector.KeyFrameAnnotation.Height / 2,
                                _inkCanvas, Color.FromArgb(180, 0, 255, 0));
                    }
                    else
                    {
                        linkLine = InkTool.getInstance().DrawLine(selectKeyFrameCenterPoint.X + inkCanvasSpiralSummarizationMargin.Left,
                                selectKeyFrameCenterPoint.Y + inkCanvasSpiralSummarizationMargin.Top,
                               _inkCanvas.ActualWidth - _inkCollector.KeyFrameAnnotation.Width / 2,
                               _inkCollector.KeyFrameAnnotation.Margin.Top + _inkCollector.KeyFrameAnnotation.Height / 2,
                               _inkCanvas, Color.FromArgb(180, 0, 255, 0));
                    }
                    _inkCollector.KeyFramesAnnotation.relatedKeyFrameIndexes.Add(startIndex);
                    selectKeyFrame.Annotations.Add(linkLine, _inkCollector.KeyFramesAnnotation);
                    //videoSummarization.AddPoint2Track(startIndex, Colors.Red, 0);
                    ((SpiralSummarization)videoSummarization).AddPoints2ShowSpiral(startIndex, Colors.Red, 0);
                }
                _startPoint.X = 0;
                _startPoint.Y = 0;
            }
        }
        #endregion 

        #region 成员函数    

        /// <summary>
        /// 查找选中的关键帧编号
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int getSelectImageIndex(Point point)
        {
            return videoSummarization.getSelectedKeyFrameIndex(point);//, spiralSummarization);
        }

        /// <summary>
        /// 判断笔迹的终点是否在注释框内
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool isInAnnotationBox(Point point)
        {
            Thickness margin = _inkCollector.KeyFrameAnnotation.Margin;
            if (_inkCollector.KeyFrameAnnotation.HorizontalAlignment == HorizontalAlignment.Left)
            {
                if (point.X > margin.Left &&
                    point.X < margin.Left + _inkCollector.KeyFrameAnnotation.Width
                    && point.Y > margin.Top
                    && point.Y < margin.Top + _inkCollector.KeyFrameAnnotation.Height)
                    return true;
            }
            else
            {
                if (point.X > _inkCanvas.ActualWidth- _inkCollector.KeyFrameAnnotation.Width&&
                    point.X < _inkCanvas.ActualWidth  
                    && point.Y > margin.Top
                    && point.Y < margin.Top + _inkCollector.KeyFrameAnnotation.Height)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
