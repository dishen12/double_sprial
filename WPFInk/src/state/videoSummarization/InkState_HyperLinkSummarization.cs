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
    public class InkState_HyperLinkSummarization : InkState
    {
        private Point _startPoint;
        private Point _currPoint;
        private int startIndex = int.MinValue;
        private int endIndex = int.MinValue;
        private KeyFrame startKeyFrame = null;//链接源
        private KeyFrame endKeyFrame = null;//链接目标
        private SpiralSummarization endSpiralSummarization = null;//链接目标所在inkcanvas
        //private ThumbPlayer thumbPlayer = null;
        //Image preImage = null;
        private VideoSummarization videoSummarization = null;
        private WPFInk.mouseGesture.MouseGesture mouseGesture = null;
        private List<MyStrokeData> trackRecord = new List<MyStrokeData>();

        private System.DateTime downTime;
        private List<KeyFrame> selectKeyFrames = new List<KeyFrame>();//选中的关键帧，用来合并新的螺旋摘要
        private Grid TableGrid;

        public VideoSummarization VideoSummarization
        {
            get { return videoSummarization; }
            set { videoSummarization = value; }
        }

        public InkState_HyperLinkSummarization(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
        }

        public List<MyStrokeData> TrackRecord
        {
            get { return trackRecord; }
            set { trackRecord = value; }
        }

        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.CaptureMouse();
            _startPoint = e.GetPosition(_inkCanvas);
            downTime = System.DateTime.Now;
            startKeyFrame = null;
            endKeyFrame = null;
            if (TableGrid == null)
            {
                TableGrid = _inkCollector._mainPage.VideoSummarizationControl.TableGrid;
            }
            if (_startPoint.Y < TableGrid.ActualHeight / 2)
            {
                if (_startPoint.X < TableGrid.ActualWidth*0.5)
                {
                    _startPoint.X -= ((InkCanvas)_inkCanvas.Children[0]).Margin.Left;
                    _startPoint.Y -= ((InkCanvas)_inkCanvas.Children[0]).Margin.Top;
                    startIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.getSelectedKeyFrameIndex(_startPoint);
                    if (startIndex != int.MinValue)
                    {
                        startKeyFrame = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.KeyFrames[startIndex];
                    }
                    _startPoint.X += ((InkCanvas)_inkCanvas.Children[0]).Margin.Left;
                    _startPoint.Y += ((InkCanvas)_inkCanvas.Children[0]).Margin.Top;
                }
                else
                {
                    Point startPointRight = e.GetPosition((InkCanvas)_inkCanvas.Children[1]);
                    startIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.getSelectedKeyFrameIndex(startPointRight);
                    if (startIndex != int.MinValue)
                    {
                        startKeyFrame = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.KeyFrames[startIndex];
                    }
                }
            }
            else
            {
                Point startPointBottom = e.GetPosition((InkCanvas)_inkCanvas.Children[2]);
                startIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationBottom.getSelectedKeyFrameIndex(startPointBottom);
                if (startIndex != int.MinValue)
                {
                    startKeyFrame = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationBottom.KeyFrames[startIndex];
                }
            }
            if (mouseGesture == null)
            {
                createGesture();
            }
            mouseGesture.StartCapture((int)_startPoint.X, (int)_startPoint.Y);

        }



        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!(_startPoint.X == 0 && _startPoint.Y == 0))
            {
                _currPoint = e.GetPosition(_inkCanvas);
                mouseGesture.Capturing((int)_currPoint.X, (int)_currPoint.Y);
            }

        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Stroke lastStroke = _inkCanvas.Strokes.Last();
            if (!(_startPoint.X == 0 && _startPoint.Y == 0))
            {
                //先清空原来选中的关键帧序列
                //clearPreMessage();
                _currPoint = e.GetPosition(_inkCanvas);
                if (_currPoint.Y < TableGrid.ActualHeight / 2)
                {
                    if (_currPoint.X < TableGrid.ActualWidth / 2)
                    {
                        _currPoint.X -= ((InkCanvas)_inkCanvas.Children[0]).Margin.Left;
                        _currPoint.Y -= ((InkCanvas)_inkCanvas.Children[0]).Margin.Top;
                        endIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.getSelectedKeyFrameIndex(_currPoint);
                        if (startIndex != int.MinValue && endIndex != int.MinValue)
                        {
                            endKeyFrame = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.KeyFrames[endIndex];
                            endSpiralSummarization = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft;
                        }
                        _currPoint.X += ((InkCanvas)_inkCanvas.Children[0]).Margin.Left;
                        _currPoint.Y += ((InkCanvas)_inkCanvas.Children[0]).Margin.Top;
                    }
                    //右边的螺旋摘要
                    else
                    {
                        Point endPointRight = e.GetPosition((InkCanvas)_inkCanvas.Children[1]);
                        endIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.getSelectedKeyFrameIndex(endPointRight);
                        if (startIndex != int.MinValue && endIndex != int.MinValue)
                        {
                            endKeyFrame = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.KeyFrames[endIndex];
                            endSpiralSummarization = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight;
                        }
                    }
                }
                else
                {
                    Point endPointBottom = e.GetPosition((InkCanvas)_inkCanvas.Children[2]);
                    endIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationBottom.getSelectedKeyFrameIndex(endPointBottom);
                    if (startIndex != int.MinValue && endIndex != int.MinValue)
                    {
                        endKeyFrame = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationBottom.KeyFrames[endIndex];
                        endSpiralSummarization = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationBottom;
                    }
                }
                if (startKeyFrame != null && endKeyFrame != null&&startKeyFrame!=endKeyFrame)
                {
                    startKeyFrame.HyperLink = endKeyFrame;
                    _inkCollector.HyperLinkKeyFrames.Add(startKeyFrame);
                    startKeyFrame.HyperLinkSpiralSummarization = endSpiralSummarization;
                    _inkCanvas.Strokes.Remove(_inkCanvas.Strokes.Last());
                    InkTool.getInstance().DrawLine(_startPoint.X, _startPoint.Y, _currPoint.X, _currPoint.Y, _inkCanvas,Colors.Red);
                    InkTool.getInstance().drawPoint(_startPoint.X, _startPoint.Y, 8, Colors.Blue, _inkCanvas);

                }
                else
                {
                    mouseGesture.StopCapture();
                    _inkCanvas.Strokes.Remove(lastStroke);
                }
                _startPoint.X = 0;
                _startPoint.Y = 0;

            }
        }

        /// <summary>
        /// 创建手势库
        /// </summary>
        public void createGesture()
        {
            mouseGesture = new WPFInk.mouseGesture.MouseGesture();
            //把选出来的关键帧生成螺旋摘要
            //mouseGesture.AddGesture("generateSpiralSummarization", "70123456701234567", null);
            //mouseGesture.AddGesture("generateSpiralSummarization", "701234567012345670123456", null);
            //进入视频播放状态
            mouseGesture.AddGesture("spiralSummarization", "0", null);
            mouseGesture.GestureMatchEvent += new WPFInk.mouseGesture.MouseGesture.GestureMatchDelegate(gesture_GestureMatchEvent);
        }
        /// <summary>
        /// 方向和笔序识别结果匹配和处理
        /// </summary>
        /// <param name="args"></param>
        void gesture_GestureMatchEvent(MouseGestureEventArgs args)
        {
            switch (args.Present)
            {
                case "spiralSummarization":
                    if (startIndex == int.MinValue && endIndex == int.MinValue)
                    {
                        //先清除原来的两个螺旋摘要
                        //_inkCollector._mainPage.VideoSummarizationControl.keyFrameListScrollViewer.Visibility = Visibility.Visible;
                        _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.hiddenSpiralSummarization();
                        _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationBottom.hiddenSpiralSummarization();

                        _inkCollector._mainPage.VideoSummarizationControl.resetTableGrid();
                        _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.InkCanvas.Margin = new Thickness(0, 0, 0, 0);
                        _inkCanvas.Strokes.Clear();
                        _inkCollector.VideoSummarization = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft;
                        _inkCollector._mainPage.VideoSummarizationControl._timeBar.Visibility = Visibility.Visible;
                        _inkCollector._mainPage.VideoSummarizationControl.VideoProgressText.Visibility = Visibility.Visible;
                        _inkCollector.Mode = InkMode.VideoSummarization;
                    }
                    break;
                default:
                    //Console.WriteLine("default");

                    break;
            }
        }

    }
}
