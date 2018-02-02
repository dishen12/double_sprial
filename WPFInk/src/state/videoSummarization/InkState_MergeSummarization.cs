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
    public class InkState_MergeSummarization : InkState
    {
        private Point _startPoint;
        private Point _currPoint;
        private int startIndex = 0;
        private int endIndex = 0;
        private ThumbPlayer thumbPlayer = null;
        Image preImage = null;
        private VideoSummarization videoSummarization = null;
        private WPFInk.mouseGesture.MouseGesture mouseGesture = null;
        private List<MyStrokeData> trackRecord = new List<MyStrokeData>();
        //private MyStrokeData myStrokeData; 

        private System.DateTime downTime;
        private List<KeyFrame> selectKeyFrames = new List<KeyFrame>();//选中的关键帧，用来合并新的螺旋摘要
        private Grid TableGrid;

        public VideoSummarization VideoSummarization
        {
            get { return videoSummarization; }
            set { videoSummarization = value; }
        }

        public InkState_MergeSummarization(InkCollector inkCollector)
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
            if (TableGrid == null)
            {
                TableGrid = _inkCollector._mainPage.VideoSummarizationControl.TableGrid;
            }
            if (_startPoint.X < (TableGrid.ActualWidth-300)*0.5)
            {
                _startPoint.X -= ((InkCanvas)_inkCanvas.Children[0]).Margin.Left;
                _startPoint.Y -= ((InkCanvas)_inkCanvas.Children[0]).Margin.Top;
                startIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.getSelectedKeyFrameIndex(_startPoint);
            }
            else
            {
                Point startPointRight = e.GetPosition((InkCanvas)_inkCanvas.Children[1]);
                startIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.getSelectedKeyFrameIndex(startPointRight);
                
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
                if (_startPoint.X < (TableGrid.ActualWidth - 300) * 0.5)
                {
                    _currPoint.X -= ((InkCanvas)_inkCanvas.Children[0]).Margin.Left;
                    _currPoint.Y -= ((InkCanvas)_inkCanvas.Children[0]).Margin.Top;
                    endIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.getSelectedKeyFrameIndex(_currPoint);
                }
                //右边的螺旋摘要
                else
                {
                    Point endPointRight = e.GetPosition((InkCanvas)_inkCanvas.Children[1]);
                    endIndex = _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.getSelectedKeyFrameIndex(endPointRight);
                }
                if (startIndex != int.MinValue && endIndex != int.MinValue && startIndex != endIndex)
                {
                    if (startIndex > endIndex)
                    {
                        int temp = startIndex;
                        startIndex = endIndex;
                        endIndex = temp;
                    }
                    if (_startPoint.X < TableGrid.ActualWidth / 2)
                    {
                        for (int i = startIndex; i <= endIndex; i++)
                        {
                            if (i < _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.KeyFrames.Count)
                            {
                                Image image = VideoSummarizationTool.KeyFrameToImage(_inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.KeyFrames[i]);

                                image.Margin = new Thickness(2, _inkCollector._mainPage.VideoSummarizationControl.keyFrameList._inkCanvas.Children.Count * 84, 0, 0);
                                _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.KeyFrames[i].Image =
                                    VideoSummarizationTool.KeyFrameToImage(_inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.KeyFrames[i]);
                                selectKeyFrames.Add(_inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationLeft.KeyFrames[i].convertToNewKeyFrame());
                                _inkCollector._mainPage.VideoSummarizationControl.keyFrameList._inkCanvas.Children.Add(image);
                            }
                        }
                    }
                    //右边的螺旋摘要
                    else
                    {
                        for (int i = startIndex; i <= endIndex; i++)
                        {
                            Image image = VideoSummarizationTool.KeyFrameToImage(_inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.KeyFrames[i]);
                            image.Margin = new Thickness(2, _inkCollector._mainPage.VideoSummarizationControl.keyFrameList2._inkCanvas.Children.Count * 84, 0, 0);
                            _inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.KeyFrames[i].Image =
                                VideoSummarizationTool.KeyFrameToImage(_inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.KeyFrames[i]);
                            selectKeyFrames.Add(_inkCollector._mainPage.VideoSummarizationControl.SpiralSummarizationRight.KeyFrames[i].convertToNewKeyFrame());
                            _inkCollector._mainPage.VideoSummarizationControl.keyFrameList2._inkCanvas.Children.Add(image);
                        }

                    }
                }
                else
                {
                    mouseGesture.StopCapture();
                }
                _startPoint.X = 0;
                _startPoint.Y = 0;
                _inkCanvas.Strokes.Remove(lastStroke);
               
            }
        }
        /// <summary>
        /// 记录操作事件与持续时间
        /// </summary>
        /// <param name="operateEvent"></param>
        private void recordOperateEvent(string operateEvent)
        {
            //myStrokeData = new MyStrokeData();
            //myStrokeData.CurrentTime = (upTime - downTime).ToString();
            //myStrokeData.OperateType = operateEvent;
            //trackRecord.Add(myStrokeData);
        }
        /// <summary>
        /// 清除原来的事件和操作信息
        /// </summary>
        private void clearPreMessage()
        {            
            _inkCollector.SelectKeyFrames.Clear();
            if (thumbPlayer != null)
            {
                _inkCanvas.Children.Remove(thumbPlayer.VideoPlayer);
            }
            _inkCollector.KeyFrameAnnotation.InkCanvasAnnotation.Strokes.Clear();
            _inkCollector.KeyFrameAnnotation.Visibility = Visibility.Collapsed;
            _inkCollector._mainPage._thumbVideoPlayer.videoPlayer.Source = null;
            _inkCollector._mainPage._thumbVideoPlayer.Visibility = Visibility.Collapsed;
            if (preImage != null)
            {
                _inkCanvas.Children.Remove(preImage);
            }
        }
       

        /// <summary>
        /// 创建手势库
        /// </summary>
        public void createGesture()
        {
            mouseGesture = new WPFInk.mouseGesture.MouseGesture();
            //把选出来的关键帧生成螺旋摘要
            mouseGesture.AddGesture("generateSpiralSummarization", "70123456701234567", null);
            mouseGesture.AddGesture("generateSpiralSummarization", "701234567012345670123456", null);
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
            //记录操作事件与持续时间
            recordOperateEvent(args.Present);
            switch (args.Present)
            {
                case "generateSpiralSummarization":
                    //生成新的螺旋摘要
                    InkCanvas newSpiralInkCanvas = new InkCanvas();
                    newSpiralInkCanvas.Width = TableGrid.ActualWidth*0.75;
                    newSpiralInkCanvas.Height = TableGrid.ActualHeight;
                    newSpiralInkCanvas.Background = new SolidColorBrush(Colors.Transparent);
                    newSpiralInkCanvas.Margin = new Thickness(0, TableGrid.ActualHeight*0.25, 0, 0);
                    double spiralWidth = 0;
                    spiralWidth = 54;
                    MySpiral mySpiral = new MySpiral(spiralWidth, Colors.Transparent, new StylusPoint((double)(int)(newSpiralInkCanvas.Width / 2), (double)(int)(newSpiralInkCanvas.Height / 2)), 3, 10, newSpiralInkCanvas, false);
                    SpiralSummarization newSpiralSummarization = new SpiralSummarization(_inkCollector, mySpiral, selectKeyFrames,false);
                    //SpiralSummarization newSpiralSummarization = new SpiralSummarization(mySpiral, _inkCollector.TileKeyFrames);
                    _inkCanvas.Children.Add(newSpiralInkCanvas);
                    _inkCollector.VideoSummarization = newSpiralSummarization;
                    break;
                case "spiralSummarization":
                    if (startIndex == int.MinValue && endIndex == int.MinValue)
                    {
                        //先清除原来的两个螺旋摘要
                        _inkCollector._mainPage.VideoSummarizationControl.keyFrameListScrollViewer.Visibility = Visibility.Visible;
                        ((InkCanvas)_inkCanvas.Children[0]).Children.Clear();
                        ((InkCanvas)_inkCanvas.Children[0]).Strokes.Clear();
                        ((InkCanvas)_inkCanvas.Children[1]).Children.Clear();
                        ((InkCanvas)_inkCanvas.Children[1]).Strokes.Clear();
                        _inkCanvas.Children.RemoveAt(0);
                        _inkCanvas.Children.RemoveAt(0);
                        ((InkCanvas)_inkCanvas.Children[0]).Margin = new Thickness(0);
                        _inkCollector._mainPage.VideoSummarizationControl.resetTableGrid();
                        _inkCollector._mainPage.VideoSummarizationControl.keyFrameList._inkCanvas.Children.Clear();
                        _inkCollector._mainPage.VideoSummarizationControl.keyFrameList2._inkCanvas.Children.Clear();
                        _inkCollector._mainPage.VideoSummarizationControl.keyFrameListScrollViewer.Visibility = Visibility.Collapsed;
                        _inkCollector._mainPage.VideoSummarizationControl.keyFrameListScrollViewer2.Visibility = Visibility.Collapsed;
                        _inkCollector._mainPage.VideoSummarizationControl.GridBtn.Visibility = Visibility.Visible;
                        _inkCollector._mainPage.VideoSummarizationControl._timeBar.Visibility = Visibility.Visible;
                        _inkCollector._mainPage.VideoSummarizationControl.VideoProgressText.Visibility = Visibility.Visible;
                        _inkCollector._mainPage.VideoSummarizationControl.BtnMerge.Visibility = Visibility.Collapsed;
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
