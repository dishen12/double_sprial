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
using System.Windows.Shapes;
using System.IO;
using WPFInk.videoSummarization;
using System.Windows.Ink;
using WPFInk.Global;
using WPFInk.ink;
using WPFInk.tool;
using WPFInk.state;
using WPFInk.Multitouch;
using Windows7.Multitouch.WPF;
namespace WPFInk
{
    /// <summary>
    /// Interaction logic for VideoSummarizationControl.xaml
    /// </summary>
    public partial class VideoSummarizationControl : Window
    {
        #region 构造函数
        public VideoSummarizationControl()
        {
            try
            {
                this.InitializeComponent();            
                summarization.InkCollector._mainPage.VideoSummarizationControl = this;
                summarization.setSummarization(this);
                summarization.rectangle1.Visibility = Visibility.Collapsed;
                summarization._inkCanvas.Background = new SolidColorBrush(GlobalValues.color);
                _userStudySettings.setVideoSummarizationControl(this);

                //非测试情况下
                _userStudySettings.Visibility = Visibility.Collapsed;
                SpiralButton.Visibility = Visibility.Visible;
                TileButton.Visibility = Visibility.Collapsed;
                TapestryButton.Visibility = Visibility.Collapsed; 
                summarization.InkCollector.addImages();
                setFileStream(@"resource\Record\非测试.txt");

                TaskInitialization();
            }
            catch(Exception e)
            {
                Console.Write(e.StackTrace);
                MessageBox.Show("VideoSummarizationControl初始化失败");
            }
        }
        public void setFileStream(string recordTxtPath)
        {
            this.filePath = recordTxtPath;
            myStream = new FileStream(recordTxtPath, FileMode.Append, FileAccess.Write);
        }
        #endregion
        #region 私有变量
        public FileStream myStream;
        public StreamWriter sWriter;
        protected string filePath;// = System.IO.Path.GetFullPath(GlobalValues.FilesPath + @"\WPFInk\SpiralTrace.txt");
        private SpiralSummarization spiralSummarizationLeft;
        private SpiralSummarization spiralSummarizationRight;
        private SpiralSummarization spiralSummarizationBottom;
        private InkCanvas spiralInkCanvasLeft;
        private InkCanvas spiralInkCanvasRight;
        private InkCanvas spiralInkCanvasBottom;
        private InkCanvas spiralInkCanvas = new InkCanvas();
        private InkCanvas tileInkCanvas = new InkCanvas();
        private InkCanvas tapestryInkCanvas = new InkCanvas();
        private SpiralSummarization spiralSummarization;
        private TileSummarization tileSummarization;
        private TapestrySummarization tapestrySummarization;
        private PictureTrackerManager _pictureTrackerManager;
        private bool isSpiralScreen = false;//螺旋摘要是否全屏
        //System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private TimeBar timeBar;
        #endregion

        #region 封装字段
        public bool IsSpiralScreen
        {
            get { return isSpiralScreen; }
            set { isSpiralScreen = value; }
        }
        public SpiralSummarization SpiralSummarization
        {
            get { return spiralSummarization; }
            set { spiralSummarization = value; }
        }
        public SpiralSummarization SpiralSummarizationLeft
        {
            get { return spiralSummarizationLeft; }
            set { spiralSummarizationLeft = value; }
        }


        public SpiralSummarization SpiralSummarizationRight
        {
            get { return spiralSummarizationRight; }
            set { spiralSummarizationRight = value; }
        }


        public SpiralSummarization SpiralSummarizationBottom
        {
            get { return spiralSummarizationBottom; }
            set { spiralSummarizationBottom = value; }
        }
        #endregion
        #region 事件
        private int tagSpiralCount = 0;
        private void SpiralButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sWriter == null)
            {
                sWriter = new StreamWriter(myStream);
            }     
            if ( null ==spiralSummarization)
            {
                //生成螺旋视频摘要
                spiralInkCanvas.Background = new SolidColorBrush(GlobalValues.color);
                spiralInkCanvas.DefaultDrawingAttributes.Color = Colors.Transparent;
                spiralInkCanvas.Width = summarization.ActualWidth;
                spiralInkCanvas.Height = summarization.ActualHeight;
                spiralInkCanvas.HorizontalAlignment = HorizontalAlignment.Stretch;
                spiralInkCanvas.VerticalAlignment = VerticalAlignment.Stretch;
                double spiralWidth = 0;
                spiralWidth = 70;
                Color spiralLineColor = Colors.Blue;
                //Color spiralLineColor = GlobalValues.color;
                MySpiral mySpiral = new MySpiral(spiralWidth, spiralLineColor, new StylusPoint((double)(int)(summarization._inkCanvas.ActualWidth / 2 + 30),
                    (double)(int)(summarization._inkCanvas.ActualHeight / 2 - 15)), 3, 10, spiralInkCanvas, false);
                spiralSummarization = new SpiralSummarization(summarization.InkCollector, mySpiral, summarization.InkCollector.KeyFramesSpiral[summarization.InkCollector.VideoNum],GlobalValues.isShowHalf);
                summarization._inkCanvas.Children.Add(spiralInkCanvas);
                summarization._inkCanvas.MouseWheel += new MouseWheelEventHandler(_inkCanvas_MouseWheel);

                ////多点触摸设置
                //if (!Windows7.Multitouch.TouchHandler.DigitizerCapabilities.IsMultiTouchReady)
                //{
                //    MessageBox.Show("Multitouch is not availible");
                //    Environment.Exit(1);
                //}
                //Enable stylus events
                Factory.EnableStylusEvents(this);
                _pictureTrackerManager = new PictureTrackerManager(spiralSummarization, this);
                //Register for stylus (touch) events
                summarization.InkCollector._mainPage.StylusDown += _pictureTrackerManager.ProcessDown;
                summarization.InkCollector._mainPage.StylusUp += _pictureTrackerManager.ProcessUp;
                summarization.InkCollector._mainPage.StylusMove += _pictureTrackerManager.ProcessMove;
            }
            spiralInkCanvas.Visibility = Visibility.Visible;
            tileInkCanvas.Visibility = Visibility.Hidden;
            tapestryInkCanvas.Visibility = Visibility.Hidden;
            _timeBar.Visibility = Visibility.Visible;
            VideoProgressText.Visibility = Visibility.Visible;
            ClearPreSummarizationFrameAndThumbVideo();
            if (timeBar != null)
            {
                timeBar.Visibility = Visibility.Hidden;
            }
            tagSpiralCount++;
            summarization.InkCollector.VideoSummarization = spiralSummarization;
            summarization.InkCollector.IsShowRedPoint = true;
            summarization.InkCollector.Mode = InkMode.VideoSummarization;
            summarization.InkCollector.DefaultSummarizationNum = 0;
            InkCanvasShowRate.Visibility = Visibility.Visible;
            BtnStart.IsEnabled = true;
            Record("Spiral");
            
        }
        //生成平铺视频摘要
        private void TileButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sWriter == null)
            {
                sWriter = new StreamWriter(myStream);
            }
            summarization.InkCollector.DefaultSummarizationNum = 1;
            if (null == tileSummarization)
            {
                tileInkCanvas.Background = new SolidColorBrush(Colors.Black);
                tileInkCanvas.Width = summarization._inkCanvas.ActualWidth;
                tileInkCanvas.Height = summarization._inkCanvas.ActualHeight*3;
                tileSummarization = new TileSummarization(tileInkCanvas, summarization.InkCollector.KeyFramesTile[summarization.InkCollector.VideoNum],new StylusPoint(-5,10),true);//最后一个参数是控制是否有隐藏
                summarization._inkCanvas.Children.Add(tileInkCanvas);
                summarization._inkCanvas.MouseWheel += new MouseWheelEventHandler(_inkCanvas_MouseWheel);
            }
            tileSummarization.InkCollector = summarization.InkCollector;
            tileInkCanvas.Visibility = Visibility.Visible;
            spiralInkCanvas.Visibility = Visibility.Hidden;
            tapestryInkCanvas.Visibility = Visibility.Hidden;
            ClearPreSummarizationFrameAndThumbVideo();
            if (timeBar != null)
            {
                timeBar.Visibility = Visibility.Hidden;
            }
            summarization.InkCollector.VideoSummarization = tileSummarization;
            summarization.InkCollector.IsShowRedPoint = false;
            summarization.InkCollector.Mode = InkMode.VideoSummarization;
            InkCanvasShowRate.Visibility = Visibility.Collapsed;
            BtnStart.IsEnabled = true;
            Record("Tile");
            
        }

        /// <summary>
        /// 生成织锦摘要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapestryButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sWriter == null)
            {
                sWriter = new StreamWriter(myStream);
            }
            summarization.InkCollector.DefaultSummarizationNum = 2;
            if (null == tapestrySummarization)
            {
                tapestryInkCanvas.Background = new SolidColorBrush(Colors.Black);
                tapestryInkCanvas.Width = 60000;
                tapestryInkCanvas.Height = 160;
                tapestryInkCanvas.Margin = new Thickness(0,summarization._inkCanvas.ActualHeight *0.75 - tapestryInkCanvas.Height / 2,0,0);
                tapestrySummarization = new TapestrySummarization(tapestryInkCanvas, summarization.InkCollector.KeyFramesTapestry[summarization.InkCollector.VideoNum]);
                summarization._inkCanvas.Children.Add(tapestryInkCanvas);
                summarization._inkCanvas.MouseWheel += new MouseWheelEventHandler(_inkCanvas_MouseWheel);
                timeBar = new TimeBar();
                timeBar.Width = summarization._inkCanvas.ActualWidth;// -20;
                timeBar.TotalTime = tapestrySummarization.Width;
                timeBar.Show_StartTime = 0;
                timeBar.Show_EndTime = summarization._inkCanvas.ActualWidth;
                timeBar.computeLocation();
                timeBar.HorizontalAlignment = HorizontalAlignment.Stretch;
                timeBar.Margin = new Thickness(0, tapestryInkCanvas.Margin.Top + tapestryInkCanvas.Height + 20, 0, 0);
                summarization._inkCanvas.Children.Add(timeBar);
            }
            
            tapestrySummarization.ParentInkcanvas = summarization._inkCanvas;
            tapestrySummarization.InkCollector = summarization.InkCollector;
            tapestrySummarization.setTimebar(timeBar);
            tapestryInkCanvas.Visibility = Visibility.Visible;
            timeBar.Visibility = Visibility.Visible;
            spiralInkCanvas.Visibility = Visibility.Hidden;
            tileInkCanvas.Visibility = Visibility.Hidden;
            ClearPreSummarizationFrameAndThumbVideo();
            summarization.InkCollector.VideoSummarization = tapestrySummarization;
            summarization.InkCollector.IsShowRedPoint = false;
            summarization.InkCollector.Mode = InkMode.TapestrySummarization;
            InkCanvasShowRate.Visibility = Visibility.Collapsed;
            BtnStart.IsEnabled = true;        
            Record("Tapestry");
        }
        private void ClearPreSummarizationFrameAndThumbVideo()
        {
            summarization._thumbVideoPlayer.Visibility = Visibility.Collapsed;
        }
        private double zoomRate = 1;
        void _inkCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point point = e.GetPosition(summarization._inkCanvas);
            int currIndex;
            switch (summarization.InkCollector.DefaultSummarizationNum)
            {
                case 0:
                    if (!spiralSummarization.IsZooming)
                    {
                        currIndex = spiralSummarization.getSelectedKeyFrameIndex(point);
                        if (Global.GlobalValues.templates.Count > 0 && (spiralSummarization.IsFocus || spiralSummarization.IsShowHalf))
                        {
                            if (e.Delta < 0)
                            {
                                if (currIndex == int.MinValue)
                                {
                                    InkTool.getInstance().InkCanvasZoom(spiralInkCanvas, zoomRate, zoomRate + 0.1 <= 3 ? zoomRate = zoomRate + 0.1 : zoomRate, 0.5, 0.5);
                                    InkCanvasShowRate.Content = (zoomRate * 100).ToString() + "%";
                                }
                                else
                                {
                                    ((InkState_Summarization)summarization.InkCollector._state).clearPreMessage();
                                    if (spiralSummarization.IsFocus)
                                    {
                                        if (!spiralSummarization.IsZoomOut)
                                        {
                                            spiralSummarization.ZoomInOut();
                                        }
                                    }
                                    else
                                    {
                                        if (currIndex == 0)
                                        {
                                            spiralSummarization.InsertKeyFrames(0, spiralSummarization.KeyFrames.Count - 2);
                                        }
                                        else
                                        {
                                            currIndex = spiralSummarization.KeyFrames.IndexOf(spiralSummarization.ShowKeyFrames[currIndex]);
                                            int startIndex = currIndex - 6;
                                            startIndex = startIndex < 0 ? 0 : startIndex;
                                            int endIndex = currIndex + 6;
                                            endIndex = endIndex > spiralSummarization.KeyFrames.Count ? spiralSummarization.KeyFrames.Count - 2 : endIndex;
                                            spiralSummarization.InsertKeyFrames(startIndex, endIndex);
                                        }
                                    }
                                    ((InkState_Summarization)summarization.InkCollector._state).MoveTimerSecond = 0;
                                }
                            }
                            else
                            {
                                if (currIndex == int.MinValue)
                                {
                                    InkTool.getInstance().InkCanvasZoom(spiralInkCanvas, zoomRate, zoomRate > 0.15 ? zoomRate = zoomRate - 0.1 : zoomRate, 0.5, 0.5);
                                    InkCanvasShowRate.Content = (zoomRate * 100).ToString() + "%";
                                }
                                else
                                {
                                    ((InkState_Summarization)summarization.InkCollector._state).clearPreMessage();
                                    if (spiralSummarization.IsFocus)
                                    {
                                        if (spiralSummarization.IsZoomOut)
                                        {
                                            spiralSummarization.ZoomInOut();
                                        }
                                    }
                                    else
                                    {
                                        if (currIndex == 0)
                                        {
                                            spiralSummarization.HiddenKeyFrames(0, spiralSummarization.KeyFrames.Count - 2);
                                        }
                                        else
                                        {
                                            currIndex = spiralSummarization.KeyFrames.IndexOf(spiralSummarization.ShowKeyFrames[currIndex]);
                                            int startIndex = currIndex - 6;
                                            startIndex = startIndex < 0 ? 0 : startIndex;
                                            int endIndex = currIndex + 6;
                                            endIndex = endIndex > spiralSummarization.KeyFrames.Count ? spiralSummarization.KeyFrames.Count - 2 : endIndex;
                                            spiralSummarization.HiddenKeyFrames(startIndex, endIndex);
                                        }
                                    }
                                }
                                ((InkState_Summarization)summarization.InkCollector._state).MoveTimerSecond = 0;
                            }
                        }
                    }
                    else
                    {
                       // Console.WriteLine("spiralSummarization.IsZooming = true;");
                    }
                    break;
                case 1:
                    //((InkState_Summarization)summarization.InkCollector._state).clearPreMessage();
                    //if (e.Delta < 0)
                    //{
                    //    tileSummarization.ScrollViewer.ScrollToVerticalOffset(tileSummarization.ScrollViewer.VerticalOffset + 100);
                    //}
                    //else
                    //{
                    //    tileSummarization.ScrollViewer.ScrollToVerticalOffset(tileSummarization.ScrollViewer.VerticalOffset - 100);
                    //}
                    //((InkState_Summarization)summarization.InkCollector._state).MoveTimerSecond = 0;

                    if (true == tileSummarization.Protect)
                    {
                        tileSummarization.Protect = false;
                        ((InkState_Summarization)summarization.InkCollector._state).clearPreMessage();
                        int selectedIndex = tileSummarization.getSelectedKeyFrameIndex(point);
                        if (selectedIndex < 0 || selectedIndex >= tileSummarization.ShowKeyFrames.Count ||
                            int.MinValue == selectedIndex || tileSummarization.ShowKeyFrames[selectedIndex].Level == false)
                        {
                            tileSummarization.Protect = true;
                            return;
                        }

                        if (e.Delta < 0)
                        {
                            //MessageBox.Show(tileSummarization.InkCanvas.ActualWidth.ToString());
                            tileSummarization.SelectInsertKeyFrames(selectedIndex);
                            if (tileSummarization.StartInsert == -1 && tileSummarization.EndInsert == -1)
                            {
                                tileSummarization.Protect = true;
                                return;
                            }
                            tileSummarization.TimerOut.Stop();
                            tileSummarization.TimerOut.Start();
                        }
                        else if (e.Delta > 0)
                        {
                            tileSummarization.selectedDeleteKeyFrames(selectedIndex);
                            if (tileSummarization.StartDelete == -1 && tileSummarization.EndDelete == -1)
                            {
                                tileSummarization.Protect = true;
                                return;
                            }
                            tileSummarization.TimerIn.Stop();
                            tileSummarization.TimerIn.Start();
                            //tileSummarization.deleteZoomInKeyFrames(tileSummarization.StartDelete, 
                            //    tileSummarization.PreSelectedCount + tileSummarization.AfterSelectedCount);
                        }
                        ((InkState_Summarization)summarization.InkCollector._state).MoveTimerSecond = 0;
                        for (int i = 0; i < tileSummarization.KeyFrames.Count; i++)
                        {
                            tileSummarization.KeyFrames[i].CounterPartImage.Visibility = Visibility.Hidden;
                        }
                    }
                    break;
                case 2:
                    currIndex = tapestrySummarization.getSelectedKeyFrameIndex(point);
                    if (e.Delta > 0)
                    {
                        tapestrySummarization.ZoomOut(currIndex);
                    }
                    else
                    {
                        tapestrySummarization.ZoomIn(currIndex);
                    }
                    ((InkState_TapestrySummarization)summarization.InkCollector._state).MoveTimerSecond = 0;
                    break;
            }
        }
        /// <summary>
        /// 合并螺旋摘要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMerge_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BtnMerge.Visibility = Visibility.Collapsed;
            TableGrid.RowDefinitions[0].Height = new GridLength(0);
            TableGrid.RowDefinitions[1].Height = new GridLength(TableGrid.ActualHeight);
            TableGrid.ColumnDefinitions[0].Width = new GridLength(150);
            TableGrid.ColumnDefinitions[1].Width = new GridLength(TableGrid.ActualWidth - 300);
            TableGrid.ColumnDefinitions[2].Width = new GridLength(150);
            summarization._inkCanvas.Children.Clear();
            keyFrameListScrollViewer.Visibility = Visibility.Visible;
            keyFrameListScrollViewer2.Visibility = Visibility.Visible;
            spiralInkCanvas.Visibility = Visibility.Hidden;
            tileInkCanvas.Visibility = Visibility.Hidden;
            tapestryInkCanvas.Visibility = Visibility.Hidden;
            VideoProgressText.Visibility = Visibility.Hidden;
            _timeBar.Visibility = Visibility.Hidden;
            ClearPreSummarizationFrameAndThumbVideo();
            if (timeBar != null)
            {
                timeBar.Visibility = Visibility.Hidden;
            }
            GridBtn.Visibility = Visibility.Collapsed;
            //_tabControl.Visibility = Visibility.Collapsed;
            summarization.InkCollector.IsShowRedPoint = false;
            //第一个螺旋摘要
            addNewSpiral(0.125, 75, summarization.InkCollector.keyFramesSpiralMerge[0]);
            //第二个螺旋摘要
            addNewSpiralRight(0.375, 225, summarization.InkCollector.keyFramesSpiralMerge[1]);
            summarization.InkCollector.Mode = InkMode.MergeSummarization;
        }

        private void addNewSpiralRight(double leftRate, double offset, List<KeyFrame> keyFrames)
        {
            spiralInkCanvasRight = new InkCanvas();
            spiralInkCanvasRight.Width = TableGrid.ActualWidth * 0.75;
            spiralInkCanvasRight.Height = TableGrid.ActualHeight;
            spiralInkCanvasRight.Margin = new Thickness(TableGrid.ActualWidth * leftRate - offset, -TableGrid.ActualHeight * 0.25 + 20, 0, 0);
            spiralInkCanvasRight.Background = new SolidColorBrush(Colors.Transparent);
            MySpiral mySpiral2 = new MySpiral(54, Colors.Transparent, new StylusPoint((double)(int)(spiralInkCanvasRight.Width / 2), (double)(int)(spiralInkCanvasRight.Height / 2)), 3, 10, spiralInkCanvasRight, false);

            spiralSummarizationRight = new SpiralSummarization(summarization.InkCollector, mySpiral2, keyFrames, false);
            summarization._inkCanvas.Children.Add(spiralInkCanvasRight);
        }
        /// <summary>
        /// 添加螺旋摘要，在左上角位置
        /// </summary>
        /// <param name="widthRate">新添加到螺旋摘要画布宽度</param>
        /// <returns></returns>
        private void addNewSpiral(double leftRate,double offset,List<KeyFrame> keyFrames)
        {
            spiralInkCanvasLeft = new InkCanvas();
            spiralInkCanvasLeft.Width = TableGrid.ActualWidth * 0.75;
            spiralInkCanvasLeft.Height = TableGrid.ActualHeight;
            spiralInkCanvasLeft.Margin = new Thickness(-TableGrid.ActualWidth * leftRate - offset, -TableGrid.ActualHeight * 0.25 + 20, 0, 0);
            spiralInkCanvasLeft.Background = new SolidColorBrush(Colors.Transparent);
            MySpiral mySpiral = new MySpiral(54, GlobalValues.color, new StylusPoint((double)(int)(spiralInkCanvasLeft.Width / 2), (double)(int)(spiralInkCanvasLeft.Height / 2)), 3, 10, spiralInkCanvasLeft, false);
            spiralSummarizationLeft = new SpiralSummarization(summarization.InkCollector, mySpiral, keyFrames, false);
            summarization._inkCanvas.Children.Add(spiralInkCanvasLeft);
        }

        /// <summary>
        /// 添加超链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHyperLink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TableGrid.RowDefinitions[0].Height = new GridLength(0);
            TableGrid.RowDefinitions[1].Height = new GridLength(TableGrid.ActualHeight);
            TableGrid.ColumnDefinitions[0].Width = new GridLength(0);
            TableGrid.ColumnDefinitions[1].Width = new GridLength(TableGrid.ActualWidth);
            TableGrid.ColumnDefinitions[2].Width = new GridLength(0);
            summarization._inkCanvas.Children.Clear();
            spiralInkCanvas.Visibility = Visibility.Hidden;
            tileInkCanvas.Visibility = Visibility.Hidden;
            tapestryInkCanvas.Visibility = Visibility.Hidden;
            _timeBar.Visibility = Visibility.Hidden;
            VideoProgressText.Visibility = Visibility.Hidden;
            ClearPreSummarizationFrameAndThumbVideo();
            if (timeBar != null)
            {
                timeBar.Visibility = Visibility.Hidden;
            }
            //第一个螺旋摘要
            addNewSpiral(0.125, 0, summarization.InkCollector.keyFramesSpiralLink[0]);
            //第二个螺旋摘要
            addNewSpiralRight(0.375, 0, summarization.InkCollector.keyFramesSpiralLink[1]);
            //第三个螺旋摘要
            spiralInkCanvasBottom = new InkCanvas();
            spiralInkCanvasBottom.Width = TableGrid.ActualWidth * 0.75;
            spiralInkCanvasBottom.Height = TableGrid.ActualHeight;
            spiralInkCanvasBottom.Background = new SolidColorBrush(Colors.Transparent);
            spiralInkCanvasBottom.Margin = new Thickness(TableGrid.ActualWidth * 0.125, TableGrid.ActualHeight * 0.25, 0, 0);
            MySpiral mySpiral = new MySpiral(54,Colors.Transparent,
                new StylusPoint((double)(int)(spiralInkCanvasBottom.Width / 2), 
                    (double)(int)(spiralInkCanvasBottom.Height / 2)), 3, 6, spiralInkCanvasBottom, false);
            spiralSummarizationBottom = new SpiralSummarization(summarization.InkCollector, mySpiral, summarization.InkCollector.keyFramesSpiralLink[2], false);
            summarization._inkCanvas.Children.Add(spiralInkCanvasBottom);
            summarization.InkCollector.IsShowRedPoint = false;
            summarization.InkCollector.Mode = InkMode.HyperLinkSummarization;
        }
        /// <summary>
        /// 全屏播放视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFullScreen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TableGrid.RowDefinitions[1].Height = new GridLength(0);
            TableGrid.RowDefinitions[0].Height = new GridLength(TableGrid.ActualWidth * 0.67);
            TableGrid.ColumnDefinitions[0].Width = new GridLength(TableGrid.ActualWidth);
            TableGrid.ColumnDefinitions[1].Width = new GridLength(0);
            TableGrid.ColumnDefinitions[2].Width = new GridLength(0);
            AnnotationInkCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(AnnotationInkCanvas_MouseRightButtonUp);
            summarization._thumbVideoPlayer.Visibility = Visibility.Collapsed;
        }
       
       
        /// <summary>
        /// 还原视频大小
        /// </summary>
        private void AnnotationInkCanvas_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            resetTableGrid();
        }
        /// <summary>
        /// 恢复默认布局
        /// </summary>
        public void resetTableGrid()
        {
            TableGrid.RowDefinitions[0].Height = new GridLength(TableGrid.ActualHeight * 0.2);
            TableGrid.RowDefinitions[1].Height = new GridLength(TableGrid.ActualHeight * 0.8);
            TableGrid.ColumnDefinitions[0].Width = new GridLength(TableGrid.ActualWidth * 0.25);
            TableGrid.ColumnDefinitions[1].Width = new GridLength(TableGrid.ActualWidth * 0.50);
            TableGrid.ColumnDefinitions[2].Width = new GridLength(TableGrid.ActualWidth * 0.25);
        }

        private void hyperLinkPlayer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void BtnCut_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TableGrid.RowDefinitions[0].Height = new GridLength(0);
            TableGrid.RowDefinitions[1].Height = new GridLength(TableGrid.ActualHeight);
            TableGrid.ColumnDefinitions[0].Width = new GridLength(150);
            TableGrid.ColumnDefinitions[1].Width = new GridLength(TableGrid.ActualWidth - 150);
            TableGrid.ColumnDefinitions[2].Width = new GridLength(0);
            summarization._inkCanvas.Children.Clear();
            keyFrameListScrollViewer.Visibility = Visibility.Visible;
            keyFrameListScrollViewer2.Visibility = Visibility.Visible;
            GridBtn.Visibility = Visibility.Collapsed;
            //_tabControl.Visibility = Visibility.Collapsed;
            spiralInkCanvas.Visibility = Visibility.Hidden;
            tileInkCanvas.Visibility = Visibility.Hidden;
            tapestryInkCanvas.Visibility = Visibility.Hidden;
            _timeBar.Visibility = Visibility.Hidden;
            VideoProgressText.Visibility = Visibility.Hidden;
            ClearPreSummarizationFrameAndThumbVideo();
            if (timeBar != null)
            {
                timeBar.Visibility = Visibility.Hidden;
            }
            //第一个螺旋摘要
            addNewSpiral(0.125, 75, summarization.InkCollector.keyFramesSpiralMerge[0]);
            //第二个螺旋摘要
            addNewSpiralRight(0.375, 225, summarization.InkCollector.keyFramesSpiralMerge[1]);
            spiralSummarizationRight.hiddenSpiralSummarization();
            summarization.InkCollector.IsShowRedPoint = false;
            summarization.InkCollector.Mode = InkMode.MergeSummarization;
        }

        private void mediaPlayer_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                _timeBar.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                string videoSummarizationType = summarization.InkCollector._state.GetType().ToString();
                switch (videoSummarizationType)
                {
                    case "WPFInk.state.InkState_Summarization":
                        InkState_Summarization state = (InkState_Summarization)summarization.InkCollector._state;
                        state.VideoPlayTimer = new System.Windows.Forms.Timer();
                        state.VideoPlayTimer.Interval = 1000;
                        state.VideoPlayTimer.Tick += new System.EventHandler(state.VideoPlayTimer_Tick);
                        state.VideoSummarizationControl = this;
                        state.VideoPlayTimer.Start();
                        List<string> timeTotal = ConvertClass.getInstance().MsToHMS(_timeBar.Maximum);
                        ((InkState_Summarization)summarization.InkCollector._state).TimeTotalString = timeTotal[0] + ":" + timeTotal[1] + ":" + timeTotal[2];
                        break;
                    case "WPFInk.state.InkState_TapestrySummarization":
                        InkState_TapestrySummarization stateT = (InkState_TapestrySummarization)summarization.InkCollector._state;
                        stateT.VideoPlayTimer = new System.Windows.Forms.Timer();
                        stateT.VideoPlayTimer.Interval = 1000;
                        stateT.VideoPlayTimer.Tick += new System.EventHandler(stateT.VideoPlayTimer_Tick);
                        stateT.VideoSummarizationControl = this;
                        stateT.VideoPlayTimer.Start();
                        List<string> timeTotal2 = ConvertClass.getInstance().MsToHMS(_timeBar.Maximum);
                        ((InkState_TapestrySummarization)summarization.InkCollector._state).TimeTotalString = timeTotal2[0] + ":" + timeTotal2[1] + ":" + timeTotal2[2];
                        break;
                }

            }
        }
        private static int LineCount = 0;  //双任务输入文件中任务的个数（即文件中数据的行数）
        private string SingleTaskPath = @"Release\resource\Task\SingleTask1.txt";
        private string DualTaskPath = @"resource\Task\DualTask1.txt";
        private string SingleTaskResultPath = @"resource\Task\SingleTask1_result.txt";
        private string DualTaskResultPath = @"resource\Task\DualTask1_result.txt";

        private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private int SecondaryValue = 1;    //随机值对3取模所得的值。

        private struct SingleTaskResultStruct
        {
            public string SingleCheck;
            public int sign;
        }
        private SingleTaskResultStruct[] SingleTaskResult = new SingleTaskResultStruct[3];


        private void TaskInitialization()
        {
            //this.timer.Tick += new System.EventHandler(OnTimedEvent);
            //File.Create(DualTaskResultPath);

        }

        #region singleTask code region
        //private void NewTaskButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.

        //    if (File.Exists(DualTaskPath))
        //    {
        //        string[] stringlines = File.ReadAllLines(DualTaskPath, Encoding.Default);
        //        if (LineCount < stringlines.Length)
        //        {
        //            this.PrimaryTextBlock.Text = stringlines[LineCount];
        //            LineCount++;
        //            this.NewTaskButton.IsEnabled = false;
        //            this.StartButton.IsEnabled = true;
        //        }
        //        else
        //            this.PrimaryTextBlock.Text = "No more tasks,thanks!";

        //    }
        //    //if (!(File.Exists(ResultPath)))
        //    //    File.Create(ResultPath);
        //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(DualTaskResultPath, true))
        //    {
        //        file.WriteLine("任务" + LineCount + "的测验结果:");
        //    }

        //}

        //private void StartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    //StartTime = "开始时间："+System.DateTime.Now.ToString();
        //    sw.Reset();
        //    sw.Start();
        //    timer.Interval = 2000;
        //    timer.Start();
        //    this.StartButton.IsEnabled = false;
        //    this.YesButton.IsEnabled = true;
        //    this.NoButton.IsEnabled = true;

        //}

        //private void DoneButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    // TODO: Add event handler implementation here.
        //    sw.Stop();
        //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(DualTaskResultPath, true))
        //    {
        //        file.WriteLine("任务" + LineCount + "需要时间：" + sw.ElapsedMilliseconds + "ms");
        //        file.WriteLine(" ");
        //    }
        //    this.SecondaryTextBlock.Text = "";
        //    this.DoneButton.IsEnabled = false;
        //    this.NewTaskButton.IsEnabled = true;
        //}

        //private void YesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    if (0 == SecondaryValue)
        //        using (System.IO.StreamWriter file = new System.IO.StreamWriter(DualTaskResultPath, true))
        //        {
        //            file.WriteLine("次任务" + LineCount + "回答正确！");
        //        }
        //    else
        //        using (System.IO.StreamWriter file = new System.IO.StreamWriter(DualTaskResultPath, true))
        //        {
        //            file.WriteLine("次任务" + LineCount + "回答错误！");
        //        }
        //    this.YesButton.IsEnabled = false;
        //    this.NoButton.IsEnabled = false;
        //    this.DoneButton.IsEnabled = true;
        //}

        //private void NoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    if (0 == SecondaryValue)
        //        using (System.IO.StreamWriter file = new System.IO.StreamWriter(DualTaskResultPath, true))
        //        {
        //            file.WriteLine("次任务" + LineCount + "回答错误！");
        //        }
        //    else
        //        using (System.IO.StreamWriter file = new System.IO.StreamWriter(DualTaskResultPath, true))
        //        {
        //            file.WriteLine("次任务" + LineCount + "回答正确！");
        //        }
        //    this.YesButton.IsEnabled = false;
        //    this.NoButton.IsEnabled = false;
        //    this.DoneButton.IsEnabled = true;
        //}

        //private void OnTimedEvent(object sender, EventArgs e)
        //{
        //    Random R = new Random();
        //    int r1 = R.Next(0, 30);
        //    int r2 = R.Next(0, 10);
        //    SecondaryValue = (r1 * r2) % 3;
        //    this.SecondaryTextBlock.Text = r1.ToString() + "plus" + r2 + ", and to responde whether the sum is divisible by 3？";
        //    timer.Stop();

        //}
        //#endregion


        //#region singleTask code region

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (this.S_StartB.Content.Equals("START"))
            //{
            //    this.QuestionGrid.IsEnabled = true;
            //    this.S_StartB.Content = "FINISH";
            //    this.S_StartB.IsEnabled = false;
            //    if (File.Exists(SingleTaskPath))
            //    {
            //        string[] stringlines = File.ReadAllLines(SingleTaskPath, Encoding.Default);
            //        this.S_TextBlock1.Text = stringlines[0];
            //        this.S_TextBlock2.Text = stringlines[1];
            //        this.S_TextBlock3.Text = stringlines[2];
            //    }
            //    File.Create(SingleTaskResultPath);

            //}
            //else
            //{
            //    this.S_StartB.Content = "START";
            //    this.QuestionGrid.IsEnabled = false;
            //    this.S_TextBlock1.Text = "";
            //    this.S_TextBlock2.Text = "";
            //    this.S_TextBlock3.Text = "";
            //    this.YesB1.IsChecked = false;
            //    this.YesB2.IsChecked = false;
            //    this.YesB3.IsChecked = false;
            //    this.NoB1.IsChecked = false;
            //    this.NoB2.IsChecked = false;
            //    this.NoB3.IsChecked = false;
            //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(SingleTaskResultPath, true))
            //    {
            //        file.WriteLine("问题1的结果:" + SingleTaskResult[0].SingleCheck);
            //        file.WriteLine("问题2的结果:" + SingleTaskResult[1].SingleCheck);
            //        file.WriteLine("问题3的结果:" + SingleTaskResult[2].SingleCheck);
            //    }
            //    for (int i = 0; i < 3; i++)
            //    {
            //        SingleTaskResult[i].sign = 0;
            //    }
            //}
        }

        //private void YesB1_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    SingleTaskResult[0].SingleCheck = "YES";
        //    SingleTaskResult[0].sign = 1;
        //    if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
        //        this.S_StartB.IsEnabled = true;
        //}

        //private void NoB1_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    SingleTaskResult[0].SingleCheck = "NO";
        //    SingleTaskResult[0].sign = 1;
        //    if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
        //        this.S_StartB.IsEnabled = true;
        //}

        //private void YesB2_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    SingleTaskResult[1].SingleCheck = "YES";
        //    SingleTaskResult[1].sign = 1;
        //    if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
        //        this.S_StartB.IsEnabled = true;
        //}

        //private void NoB2_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    SingleTaskResult[1].SingleCheck = "NO";
        //    SingleTaskResult[1].sign = 1;
        //    if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
        //        this.S_StartB.IsEnabled = true;
        //}

        //private void YesB3_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    SingleTaskResult[2].SingleCheck = "YES";
        //    SingleTaskResult[2].sign = 1;
        //    if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
        //        this.S_StartB.IsEnabled = true;
        //}

        //private void NoB3_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    SingleTaskResult[2].SingleCheck = "NO";
        //    SingleTaskResult[2].sign = 1;
        //    if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
        //        this.S_StartB.IsEnabled = true;
        //}

        private void BtnNavigation_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            summarization._inkCanvas.Children.Clear();
            SpiralButton.IsEnabled = true;
            TileButton.IsEnabled = true;
            TapestryButton.IsEnabled = true;
            BtnVideoFullScreen.IsEnabled = true;
            BtnMerge.IsEnabled = false;
            BtnCut.IsEnabled = false;
            BtnHyperLink.IsEnabled = false;
            //_tabControl.Visibility = Visibility.Visible;
        }

        private void BtnAutoring_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            summarization._inkCanvas.Children.Clear();
            BtnMerge.IsEnabled = true;
            BtnCut.IsEnabled = true;
            BtnHyperLink.IsEnabled = true;
            TileButton.IsEnabled = false;
            TapestryButton.IsEnabled = false;
            BtnVideoFullScreen.IsEnabled = false;
            //_tabControl.Visibility = Visibility.Collapsed;
        }

        List<Question> questiones = new List<Question>();
        //private double StartTime;
        //private double EndTime;
        private void BtnStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (TitleLbl.Content.ToString())
            {
                case "Comprehension":
                    switch (BtnStart.Content.ToString())
                    {
                        case "Start":
                            RecordCurrOperationAndTime("ComprehensionStart");
                            string questionTitlePath = @"resource\Task\" + GlobalValues.videoName + @"\Comprehension.txt";
                            if (File.Exists(questionTitlePath))
                            {
                                string[] stringlines = File.ReadAllLines(questionTitlePath, Encoding.Default);
                                for (int i = 0; i < stringlines.Length; i++)
                                {
                                    string[] line = stringlines[i].Split(' ');
                                    //添加问题
                                    Question question = new Question();
                                    question.setQuestion(line[0]);
                                    //添加选项
                                    for (int j = 1; j < line.Length; j++)
                                    {
                                        question.addRadio(line[j] + "    ", "groupName" + i);
                                    }
                                    QuestionContentGrid.Children.Add(question);
                                    Grid.SetRow(question, i);
                                    questiones.Add(question);
                                }
                                //固定题目
                                //Question question1 = new Question();
                                //question1.setQuestion("1. What was the prince and yoyo riding");
                                //question1.addRadio("A.black horse ", "groupName" + 1);
                                //question1.addRadio("B.white horse ", "groupName" + 2);
                                //question1.addRadio("C.elk   ", "groupName" + 3);
                                //QuestionContentGrid.Children.Add(question1);
                                //Grid.SetRow(question1, 0);
                                //questiones.Add(question1);

                                //Question question2 = new Question();
                                //question2.setQuestion("2. Did the prince jump into the flower sea?");
                                //question2.addRadio("A.Yes  ", "groupName" + 1);
                                //question2.addRadio("B.No", "groupName" + 2);
                                //QuestionContentGrid.Children.Add(question2);
                                //Grid.SetRow(question2, 1);
                                //questiones.Add(question2);

                                //Question question3 = new Question();
                                //question3.setQuestion("3. Did the Minister lead the troops into mountain?");
                                //question3.addRadio("A.Yes  ", "groupName" + 1);
                                //question3.addRadio("B.No", "groupName" + 2);
                                //QuestionContentGrid.Children.Add(question3);
                                //Grid.SetRow(question3, 2);
                                //questiones.Add(question3);

                                //Question question4 = new Question();
                                //question4.setQuestion("4. Did the Milu group jump into the water?");
                                //question4.addRadio("A.Yes  ", "groupName" + 1);
                                //question4.addRadio("B.No", "groupName" + 2);
                                //QuestionContentGrid.Children.Add(question4);
                                //Grid.SetRow(question4, 3);
                                //questiones.Add(question4);
                            }
                            BtnStart.Content = "Done";
                            break;
                        case "Done":
                            //判断题目有没有全部完成
                            for (int i = 0; i < questiones.Count; i++)
                            {
                                if (int.MinValue == questiones[i].SelectIndex)
                                {
                                    MessageBox.Show("请完成所有题目！谢谢！^-^");
                                    return;
                                }

                            }
                            RecordCurrOperationAndTime("ComprehensionDown");
                            //RecordString();

                            string questionAnswerTitlePath = @"resource\Task\" + GlobalValues.videoName + @"\Comprehension_answer.txt";
                            if (File.Exists(questionAnswerTitlePath))
                            {
                                string[] stringlinesAnswer = File.ReadAllLines(questionAnswerTitlePath, Encoding.Default);
                                string[] line = stringlinesAnswer[0].Split(' ');
                                int rightCount = 0;
                                for (int i = 0; i < line.Length; i++)
                                {
                                    if (line[i] == questiones[i].SelectIndex.ToString())
                                    {
                                        rightCount++;
                                    }
                                    RecordString("第" + (i.ToString()) + "题的答案是：" + line[i] + ",你选择的答案是：" + questiones[i].SelectIndex.ToString());
                                }
                                RecordString("你的正确比例是" + rightCount.ToString()+"/"+line.Length);
                            }
                            switch (GlobalValues.summarizationTypeNo)
                            {
                                case 0:
                                    Record("Spiral");
                                    break;
                                case 1:
                                    Record("Tile");
                                    break;
                                case 2:
                                    Record("Tapestry");
                                    break;
                            }
                            MessageBox.Show("恭喜你完成此项任务！请完成此任务的定位功能。");
                            //sWriter.Close();
                            //myStream.Close();
                            //Environment.Exit(1);
                            QuestionContentGrid.Children.Clear();
                            TitleLbl.Content = "Location";
                            BtnStart.Content = "Start";
                            break;
                    }
                    break;
                case "Location":
                    RecordCurrOperationAndTime("LocationStart");
                    string questionLocationTitlePath = @"resource\Task\" + GlobalValues.videoName + @"\location.txt";
                    if (File.Exists(questionLocationTitlePath))
                    {
                        string[] stringlines = File.ReadAllLines(questionLocationTitlePath, Encoding.Default);
                        for (int i = 0; i < stringlines.Length; i++)
                        {
                            string[] line = stringlines[i].Split(' ');
                            //添加问题
                            if (i == 0)
                            {
                                Question LocationQuestion = new Question();
                                LocationQuestion.setQuestion(line[0]);
                                QuestionContentGrid.Children.Add(LocationQuestion);
                                Grid.SetRow(LocationQuestion, i);
                                GlobalValues.LocationQuestion = LocationQuestion;
                            }
                            GlobalValues.locationQuestions.Add(line[0]);
                            List<int> answers = new List<int>();
                            for (int j = 1; j < line.Length; j++)
                            {
                                answers.Add(int.Parse(line[j]));
                            }
                            GlobalValues.locationAnswers.Add(answers);
                            GlobalValues.CurrLocationId = 0;
                        }
                        GlobalValues.LocationQuestionBackGrouds.Add(Colors.White);
                        GlobalValues.LocationQuestionBackGrouds.Add(Colors.LightBlue);
                        GlobalValues.LocationQuestionBackGrouds.Add(Colors.LightGreen);
                        GlobalValues.LocationQuestionBackGrouds.Add(Colors.LightPink);
                        GlobalValues.LocationQuestionBackGrouds.Add(Colors.LightSkyBlue);
                    }
                    BtnStart.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        #endregion   
        #region 记录测试数据

        public void Record(string summarizationName)
        {
            List<MyStrokeData> myStrokeDatas = new List<MyStrokeData>();
            if (summarizationName == "Spiral" || summarizationName == "Tile")
            {
                myStrokeDatas = summarization.InkCollector.InkState_Summarization.TrackRecord;
            }
            else
            {
                myStrokeDatas = summarization.InkCollector.InkState_TapestrySummarization.TrackRecord;

            }
            foreach (MyStrokeData myStrokeData in myStrokeDatas)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        sWriter.WriteLine(myStrokeData.OperateType + ": (" + myStrokeData.Point.X.ToString() + "," + myStrokeData.Point.Y.ToString() + ")" + " " + myStrokeData.CurrentTime);
                    }
                    catch
                    {
                        MessageBox.Show("写文件出错！");
                    }
                }
            }
            if (File.Exists(filePath))
            {
                try
                {
                    sWriter.WriteLine("*************************" + summarizationName + "***********************");
                }
                catch
                {
                    MessageBox.Show("写文件出错！");
                }
            }
        }
        public void RecordCurrOperationAndTime(string operation)
        {
            sWriter.WriteLine(operation + ":" + System.DateTime.Now.ToString("s") + ":" + System.DateTime.Now.Millisecond.ToString());
            
            sWriter.Flush();
        }
        private void RecordString(string record)
        {
            sWriter.WriteLine(record);
            sWriter.Flush();
        }
        #endregion

        private void summarization_Loaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}