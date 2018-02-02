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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using WPFInk.tool;
using WPFInk.video;
using WPFInk.ink;
using WPFInk.cmd;
using System.Windows.Media.Animation;
using System.Windows.Ink;
using WPFInk.Global;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for VideoAnnotation.xaml
	/// </summary>
	public partial class VideoAnnotation : UserControl
    {
        private static VideoAnnotation single = null;
        private InkCollector _inkCollector;
        public ThumbInk _thumbInk;
		private VideoOperation _videoOperation;
        public TitleInk _titleInk;
        private List<string> timeTotal = new List<string>();//视频总时间
        private string timeTotalString;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private MyButton _thumbMyButton;
        private MyButton _thumbMyButtonD;
        private InkFrame _thumbInkFrame;
        private InkFrame _thumbInkFrameD;
        private Button _thumbButton;
        private Button _thumbButtonD;
		//private string videoFileName;

        private bool isStaticAnnotation = false;
        private double scaling = 1;//缩放比例
        private double _thumbWidth = 127; //缩略图宽度
        private double _thumbInterval = 0;//两个缩略图之间的水平间隙
        private double _thumbVerticalInterval = 18;//两个缩略图之间的垂直间隙
        private int _thumbCountPerLine = 10;//每行显示个数

        public double ThumbWidth
        {
            get { return _thumbWidth; }
            set { _thumbWidth = value; }
        }

        public double ThumbInterval
        {
            get { return _thumbInterval; }
            set { _thumbInterval = value; }
        }

        public double ThumbVerticalInterval
        {
            get { return _thumbVerticalInterval; }
            set { _thumbVerticalInterval = value; }
        }

        public int ThumbCountPerLine
        {
            get { return _thumbCountPerLine; }
            set { _thumbCountPerLine = value; }
        }
        private VideoCollector _videoCollector = new VideoCollector();
		public List<MyVideo> _myVideoList = new List<MyVideo>();
		public List<Storyboard> StoryboardInList = new List<Storyboard>();
		private List<Storyboard> StoryboardOutList = new List<Storyboard>();

        public static VideoAnnotation getInstance()
        {
            if (single == null)
                single = new VideoAnnotation();
            return single;
        }

		public VideoAnnotation()
		{
			this.InitializeComponent();
            this.InitTimer();
            InitApp();
        }

        private void InitApp()
        {
            //将controlpanel和inkframe关联
            Annotation_ControlPanel.Height = 20;
            Annotation_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            Annotation_ControlPanel.MaxButton.Visibility = Visibility.Visible;
            Annotation_InkFrame._inkCanvas.Background = null;
            Annotation_InkFrame._inkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
            Annotation_InkFrame._inkCanvas.DefaultDrawingAttributes.Width = 5;
            Annotation_InkFrame._inkCanvas.DefaultDrawingAttributes.Height = 5;
            Annotation_InkFrame.rectangle1.Visibility = Visibility.Collapsed;
            Annotation_ControlPanel.setInkFrame(Annotation_InkFrame);
            _inkCollector = Annotation_InkFrame.InkCollector;
            _inkCollector.Mode = InkMode.None;
            Annotation_InkFrame._inkCanvas.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(InkCanvasLeft_MouseLeftButtonUp);
			ButtonDynamicAdd.IsEnabled = false;

			Storyboard sbIn0 = this.Resources["videoGradientStroryBoardIn"] as Storyboard;//渐入
			Storyboard sbIn1 = this.Resources["RightToLeftStoryboardIn"] as Storyboard;//从右向左入
			Storyboard sbIn2 = this.Resources["LeftToRightStoryboardIn"] as Storyboard;//从左向右入
			Storyboard sbIn3 = this.Resources["BottomToTopStoryboardIn"] as Storyboard;//从下向上入
			Storyboard sbIn4 = this.Resources["TopToBottomStoryboardIn"] as Storyboard;//从上向下入
			//Storyboard sbIn5 = this.Resources["RotateStoryboardIn"] as Storyboard;//旋转进入
			StoryboardInList.Add(sbIn0);
            //StoryboardInList.Add(sbIn1);
            //StoryboardInList.Add(sbIn2);
            //StoryboardInList.Add(sbIn3);
            //StoryboardInList.Add(sbIn4);
			//StoryboardInList.Add(sbIn5);

			Storyboard sbOut0 = this.Resources["videoGradientStroryBoardOut"] as Storyboard;//渐出
			Storyboard sbOut1 = this.Resources["LeftToRightStoryboardOut"] as Storyboard;//从左向右出
			Storyboard sbOut2 = this.Resources["RightToLeftStoryboardOut"] as Storyboard;//从右向左出
			Storyboard sbOut3 = this.Resources["TopToBottomStoryboardOut"] as Storyboard;//从上向下出
			Storyboard sbOut4 = this.Resources["BottomToTopStoryboardOut"] as Storyboard;//从下向上出
			StoryboardOutList.Add(sbOut0);
            //StoryboardOutList.Add(sbOut1);
            //StoryboardOutList.Add(sbOut2);
            //StoryboardOutList.Add(sbOut3);
            //StoryboardOutList.Add(sbOut4);

        }

        

        int dynamicStrokes = 0;
        bool isStop = false;
        /// <summary>
        /// 注释框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkCanvasLeft_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mediaPlayer.Source == null)
            {
                this.Annotation_InkFrame.InkCollector.Mode = InkMode.None;
                OpenAndPlayVideo();
            }
            else
            {
                InkCanvas _inkCanvas = this.Annotation_InkFrame._inkCanvas;
                Stroke lastStroke = _inkCanvas.Strokes.Last();
                int lastStrokePointCount = lastStroke.StylusPoints.Count;
                if (lastStrokePointCount == 1)//只是单击事件，视频暂停播放，进行buttonStaricAdd操作
                {
                    Annotation_InkFrame.InkCollector.Sketch.MyStrokes.RemoveAt(_inkCanvas.Strokes.Count - 1);
                    _inkCanvas.Strokes.Remove(lastStroke);
                    if (!isStop)
                    {
                        _thumbButton = new Button();
                        _thumbButton.Width = _thumbWidth;
                        _videoCollector.ThumbHeightWidthRate = this.Annotation_InkFrame._inkCanvas.ActualHeight / this.Annotation_InkFrame._inkCanvas.ActualWidth;
                        _thumbButton.Height = _thumbWidth * (_videoCollector.ThumbHeightWidthRate);
                        _thumbMyButton = new MyButton(_thumbButton);
                        string videoPath = mediaPlayer.Source.ToString();
                        _thumbMyButton.VideoPath = videoPath;
                        foreach (MyVideo myVideo in _myVideoList)
                        {
                            if (myVideo.VideoPath == videoPath)
                            {
                                _thumbButton.Background = myVideo.Background;
                            }
                        }
                        _thumbMyButton.TimeStart = mediaPlayer.Position.TotalMilliseconds; //记录当前时间
                        _thumbInkFrame = new InkFrame();
                        _thumbInkFrame.Width = Annotation_InkFrame.ActualWidth;
                        _thumbInkFrame.Height = Annotation_InkFrame.ActualHeight;
                        _thumbMyButton.InkFrameWidth = Annotation_InkFrame.ActualWidth;
                        _thumbMyButton.InkFrameHeight = Annotation_InkFrame.ActualHeight;
                        _thumbInkFrame.InkCollector.Mode = InkMode.None;
                        _thumbInkFrame._inkCanvas.Margin = new Thickness(0);

                        _thumbMyButton.IsStaticAnnotion = true;
                        PauseVideo();       //暂停播放视频
                        isStaticAnnotation = true;
                        ButtonDynamicAdd.IsEnabled = false;
                        isStop = true;
                    }
                    else
                    {
                        PlayVideo();
                        isStop = false;
                    }

                }
                else//否则，进行添加注释操作
                {
                    Point lastPoint = e.GetPosition(_inkCanvas);
                    if (lastPoint.Y > this.ActualHeight)//通过拖拽动作表示停止添加注释，并把添加的注释添加到下面的注释库中
                    {
                        Annotation_InkFrame.InkCollector.Sketch.MyStrokes.RemoveAt(Annotation_InkFrame.InkCollector.Sketch.MyStrokes.Count - 1);
                        _inkCanvas.Strokes.Remove(lastStroke);
                       // Console.WriteLine("1" + lastPoint.Y + "," + this.ActualHeight);
                        if (isStaticAnnotation)
                        {
                            ButtonStatic();
                        }
                        else
                        {
                            if (dynamicStrokes > 0)
                            {
                                //ButtonDynamicAdd.IsEnabled = true;
                                //_thumbButtonD = new Button();
                                //_thumbButtonD.Width = _thumbWidth;
                                //_videoCollector.ThumbHeightWidthRate = this.Annotation_InkFrame._inkCanvas.ActualHeight / this.Annotation_InkFrame._inkCanvas.ActualWidth;
                                //_thumbButtonD.Height = _thumbWidth * (_videoCollector.ThumbHeightWidthRate);
                                //_thumbMyButtonD = new MyButton(_thumbButtonD);
                                //string videoPath = mediaPlayer.Source.ToString();
                                //_thumbMyButtonD.VideoPath = videoPath;
                                //foreach (MyVideo myVideo in _myVideoList)
                                //{
                                //    if (myVideo.VideoPath == videoPath)
                                //    {
                                //        _thumbButtonD.Background = myVideo.Background;
                                //    }
                                //}
                                ////_thumbMyButtonD.TimeStart = mediaPlayer.Position.TotalMilliseconds;

                                //_thumbInkFrameD = new InkFrame();
                                //_thumbInkFrameD.Width = Annotation_InkFrame.ActualWidth;
                                //_thumbInkFrameD.Height = Annotation_InkFrame.ActualHeight;
                                //_thumbMyButtonD.InkFrameWidth = Annotation_InkFrame.ActualWidth;
                                //_thumbMyButtonD.InkFrameHeight = Annotation_InkFrame.ActualHeight;
                                //_thumbInkFrameD.InkCollector.Mode = InkMode.None;
                                //_thumbInkFrameD._inkCanvas.Margin = new Thickness(0);
                                //ButtonStaticAdd.IsEnabled = false;
                                //isStaticAnnotation = false;
                                ButtonDynamicAddOk();
                            }
                        }
                    }
                    else//继续添加注释
                    {
                        //Console.WriteLine("2" + lastPoint.Y + "," + this.ActualHeight);

                        switch (isStaticAnnotation)
                        {
                            case false:
                                if (dynamicStrokes == 0)
                                {
                                    ButtonDynamicAdd.IsEnabled = true;
                                    _thumbButtonD = new Button();
                                    _thumbButtonD.Width = _thumbWidth;
                                    _videoCollector.ThumbHeightWidthRate = this.Annotation_InkFrame._inkCanvas.ActualHeight / this.Annotation_InkFrame._inkCanvas.ActualWidth;
                                    _thumbButtonD.Height = _thumbWidth * (_videoCollector.ThumbHeightWidthRate);
                                    _thumbMyButtonD = new MyButton(_thumbButtonD);
                                    string videoPath = mediaPlayer.Source.ToString();
                                    _thumbMyButtonD.VideoPath = videoPath;
                                    foreach (MyVideo myVideo in _myVideoList)
                                    {
                                        if (myVideo.VideoPath == videoPath)
                                        {
                                            _thumbButtonD.Background = myVideo.Background;
                                        }
                                    }
                                    _thumbMyButtonD.TimeStart = mediaPlayer.Position.TotalMilliseconds;

                                    _thumbInkFrameD = new InkFrame();
                                    _thumbInkFrameD.Width = Annotation_InkFrame.ActualWidth;
                                    _thumbInkFrameD.Height = Annotation_InkFrame.ActualHeight;
                                    _thumbMyButtonD.InkFrameWidth = Annotation_InkFrame.ActualWidth;
                                    _thumbMyButtonD.InkFrameHeight = Annotation_InkFrame.ActualHeight;
                                    _thumbInkFrameD.InkCollector.Mode = InkMode.None;
                                    _thumbInkFrameD._inkCanvas.Margin = new Thickness(0);
                                    //ButtonStaticAdd.IsEnabled = false;
                                    isStaticAnnotation = false;
                                }

                                dynamicStrokes++;
                                break;
                            case true:
                                break;

                        }
                    }
                }
            }


        }

        public void setThumbInk(ThumbInk thumbInk,TitleInk titleInk)
        {
            this._thumbInk = thumbInk;
            this._titleInk = titleInk;
        }

		public void setVideoOperation(VideoOperation v)
		{
			this._videoOperation = v;
		}

        private void InitTimer()
        {
            timer.Interval = 1000;
            timer.Tick += new System.EventHandler(this.timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {                   
            //修改时间轴的值
            timelineSlider.Value = mediaPlayer.Position.TotalMilliseconds;
            
            //修改显示播放进度的值的textbox
            List<string> timeCurr=new List<string>();
            timeCurr=ConvertClass.getInstance().MsToHMS(timelineSlider.Value);
            this.VideoProgress.Text = timeCurr[0] + ":" + timeCurr[1] + ":" + timeCurr[2] + "/" + timeTotalString;

            OnPlaySketch();

        }

		int storyBoardOutRandomIndex = 0;
        private void OnPlaySketch()
        {
            int i = 0;
            foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
            {
                if (myButtonLocal.IsDeleted==false&&myButtonLocal.VideoPath == mediaPlayer.Source.ToString())
                {

                    if ((myButtonLocal == _thumbMyButton && (ButtonStaticAdd.Visibility == Visibility.Visible && myButtonLocal.IsStaticAnnotion == true) || (myButtonLocal.IsStaticAnnotion == false)) || (myButtonLocal != _thumbMyButtonD))
                    {                                      
                        if (_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Count > i)
                        {
							double currentTime=mediaPlayer.Position.TotalMilliseconds;
							if ((int)(myButtonLocal.TimeStart / 1000) == (int)(currentTime / 1000) && myButtonLocal.VideoPath == mediaPlayer.Source.ToString())
							{
								if (myButtonLocal.IsGlobal == false)
								{
									Stream streamSave = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Create);
									WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(myButtonLocal.InkFrame.InkCollector, streamSave);
									Stream streamOpen = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Open);
									WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(Annotation_InkFrame.InkCollector, streamOpen);
								}
								myButtonLocal.InkFrame._inkCanvas.Background = Brushes.LemonChiffon;
								foreach (MyButton mb in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
								{
									if ((int)(myButtonLocal.TimeStart / 1000) != (int)(currentTime / 1000) && mb.InkFrame._inkCanvas.Background == Brushes.LemonChiffon)
									{
										myButtonLocal.InkFrame._inkCanvas.Background = Brushes.White;
									}
								}
							}
							//渐出效果
							if ((int)(myButtonLocal.TimeEnd / 1000) - (int)(mediaPlayer.Position.TotalMilliseconds / 1000) == 3 && myButtonLocal.VideoPath == mediaPlayer.Source.ToString())
							{
								foreach (MyArrow myArrow in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyArrows)
								{
									if (myArrow.IsDeleted == false && myArrow.PreMyButton.IsDeleted == false && myArrow.PreMyButton == myButtonLocal && _thumbInk.Thumb_InkFrame._inkCanvas.Children.IndexOf(myArrow.Arrow) > -1)
									{
										Random randomOut = new Random(unchecked((int)DateTime.Now.Ticks));
										storyBoardOutRandomIndex = randomOut.Next(0, StoryboardOutList.Count);
										storyBoardOutRandomIndex = storyBoardOutRandomIndex == StoryboardOutList.Count ? storyBoardOutRandomIndex-- : storyBoardOutRandomIndex;
				
										StoryboardOutList[storyBoardOutRandomIndex].Begin();
										selectInkCanvasStoryboard(storyBoardOutRandomIndex,"Out").Begin(Annotation_InkFrame._inkCanvas);
										break;
									}
								}
							}
							if ((int)(myButtonLocal.TimeEnd / 1000) == (int)(mediaPlayer.Position.TotalMilliseconds / 1000) && myButtonLocal.VideoPath == mediaPlayer.Source.ToString())
                            {
                                foreach (MyArrow myArrow in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyArrows)
                                {
                                    if (myArrow.IsDeleted == false && myArrow.PreMyButton.IsDeleted == false && myArrow.PreMyButton == myButtonLocal && _thumbInk.Thumb_InkFrame._inkCanvas.Children.IndexOf(myArrow.Arrow) > -1)
                                    {
                                        OpenVideoBySource(new Uri(myArrow.NextMyButton.VideoPath));
										setPositon((int)myArrow.NextMyButton.TimeStart);
										//渐入效果,与渐出效果对应
										StoryboardInList[storyBoardOutRandomIndex].Begin();
										selectInkCanvasStoryboard(storyBoardOutRandomIndex, "In").Begin(Annotation_InkFrame._inkCanvas);
										break;
										
                                    }
                                }
								if (myButtonLocal.IsGlobal == false)
								{
									//下面是在视频上删除草图
									Annotation_InkFrame.InkCollector.InkCanvas.Children.Clear();
									Annotation_InkFrame.InkCollector.InkCanvas.Strokes.Clear();
									Annotation_InkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
									Annotation_InkFrame.InkCollector.Sketch.Images.Clear();
									Annotation_InkFrame.InkCollector.Sketch.MyStrokes.Clear();
									Annotation_InkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
									
									Annotation_InkFrame.InkCollector.Mode = InkMode.Ink;
								}
								myButtonLocal.InkFrame._inkCanvas.Background = Brushes.White;

                            }
                        }

                    }
                }
                i++;
            }   
        }
		
       

        private void Element_MediaOpened(object sender, EventArgs e)
        {
            timelineSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            timeTotal = ConvertClass.getInstance().MsToHMS(timelineSlider.Maximum);
            timeTotalString = timeTotal[0] + ":" + timeTotal[1] + ":" + timeTotal[2];

            _inkCollector.Mode = InkMode.Ink;
        }

        

        private void Element_MediaOpenedFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            _myVideoList.RemoveAt(_myVideoList.Count - 1);
            mediaPlayer.Source = null;
            MessageBox.Show(e.ErrorException.Message);
        }

        private void OpenFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenAndPlayVideo();
        }

        private void OpenAndPlayVideo()
        {
            String fileName = null;
            System.Windows.Forms.OpenFileDialog openfile = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Avi Files (*.avi)|*.avi|Wmv Files (*.wmv)|*.wmv| All Files(*.*)|*.*",
                Multiselect = false
            };

            if (openfile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				//videoFileName = openfile.SafeFileName;
				_titleInk.Title_InkFrame.InkCollector.Mode = InkMode.None;
				//先保存Title inkFrame
                SaveTitleInkFrame();

				//打开新视频
                fileName = openfile.FileName;
                if (fileName != null)
                {
                    mediaPlayer.Source = new Uri(fileName);
					mediaPlayer.LoadedBehavior = MediaState.Manual;
					_titleInk.Title_InkFrame.InkCollector.VideoPath = mediaPlayer.Source.ToString();
				
                }
                MyVideo myVideo = new MyVideo();
                myVideo.VideoPath = mediaPlayer.Source.ToString();
                switch (_myVideoList.Count)
                {
                    case 0:
                        myVideo.Background = Brushes.PaleTurquoise;
                        break;
                    case 1:
                        myVideo.Background = Brushes.Fuchsia;
                        break;
                    case 2:
                        myVideo.Background = Brushes.Thistle;
                        break;
                    case 3:
                        myVideo.Background = Brushes.DarkSalmon;
                        break;
                    case 4:
                        myVideo.Background = Brushes.PaleGreen;
                        break;
                    case 5:
                        myVideo.Background = Brushes.ForestGreen;
                        break;
                    case 6:
                        myVideo.Background = Brushes.Crimson;
                        break;
                    case 7:
                        myVideo.Background = Brushes.DarkOrange;
                        break;
                    case 8:
                        myVideo.Background = Brushes.Blue;
                        break;
                    case 9:
                        myVideo.Background = Brushes.DarkOrchid;
                        break;
                    case 10:
                        myVideo.Background = Brushes.Red;
						break;
					case 11:
						myVideo.Background = Brushes.AliceBlue;
						break;
					case 12:
						myVideo.Background = Brushes.Chocolate;
						break;
					case 13:
						myVideo.Background = Brushes.Cyan;
						break;
					case 14:
						myVideo.Background = Brushes.DarkKhaki;
						break;
					case 15:
						myVideo.Background = Brushes.LawnGreen;
						break;
					case 16:
						myVideo.Background = Brushes.Silver;
						break;
					case 17:
						myVideo.Background = Brushes.SkyBlue;
						break;
					case 18:
						myVideo.Background = Brushes.Tomato;
						break;
					case 19:
						myVideo.Background = Brushes.YellowGreen;
						break;
					case 20:
						myVideo.Background = Brushes.LightPink;
						break;
					default:
						myVideo.Background = Brushes.White;
						break;

                }
                bool isExist = false;
                foreach (MyVideo mv in _myVideoList)
                {
                    if (mv.VideoPath == myVideo.VideoPath)
                    {
                        isExist = true;
                        break;
                    }
                }
                if (isExist == false)
                {
                    _myVideoList.Add(myVideo);
                }
                foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
                {
                    myButtonLocal.InkFrame._inkCanvas.Background = Brushes.White;
                    Annotation_InkFrame._inkCanvas.Strokes.Clear();
                    Annotation_InkFrame._inkCanvas.Children.Clear();
                }

                PlayVideo();
            }
            
        }

        public void SaveTitleInkFrame()
        {
            if (mediaPlayer.Source != null && ((_titleInk.Title_InkFrame._inkCanvas.Children.Count != 0) || (_titleInk.Title_InkFrame._inkCanvas.Strokes.Count != 0)))
            {
                Button _titleButton = new Button();
                _titleButton.Width = _thumbWidth;
                _videoCollector.ThumbHeightWidthRate = this._titleInk.Title_InkFrame._inkCanvas.ActualHeight / this._titleInk.Title_InkFrame._inkCanvas.ActualWidth;
                _titleButton.Height = _thumbWidth * (_videoCollector.ThumbHeightWidthRate);
                string videoPath = mediaPlayer.Source.ToString();
                foreach (MyVideo mv in _myVideoList)
                {
                    if (mv.VideoPath == videoPath)
                    {
                        _titleButton.Background = mv.Background;
                    }
                }

                InkFrame _titleInkFrame = new InkFrame();
                _titleInkFrame.Width = _titleInk.Title_InkFrame.ActualWidth;
                _titleInkFrame.Height = _titleInk.Title_InkFrame.ActualHeight;
                _titleInkFrame.InkCollector.Mode = InkMode.None;
                _titleInkFrame._inkCanvas.Margin = new Thickness(0);
                double minX = this._titleInk.Title_InkFrame.ActualWidth;
                double minY = this._titleInk.Title_InkFrame.ActualHeight;
                double maxX = 0;
                double maxY = 0;
                if (_titleInk.Title_InkFrame.InkCollector.Sketch.MyStrokes.Count > 0)
                {
                    foreach (MyStroke ms in this._titleInk.Title_InkFrame.InkCollector.Sketch.MyStrokes)
                    {
                        if (ms.VideoPath == _titleInk.Title_InkFrame.InkCollector.VideoPath)
                        {
                            ms.IsExist = false;
                            if (ms.Stroke.GetBounds().X < minX)
                            {
                                minX = ms.Stroke.GetBounds().X;
                            }
                            if (ms.Stroke.GetBounds().Y < minY)
                            {
                                minY = ms.Stroke.GetBounds().Y;
                            }
                            if (ms.Stroke.GetBounds().X + ms.Stroke.GetBounds().Width > maxX)
                            {
                                maxX = ms.Stroke.GetBounds().X + ms.Stroke.GetBounds().Width;
                            }
                            if (ms.Stroke.GetBounds().Y + ms.Stroke.GetBounds().Height > maxY)
                            {
                                maxY = ms.Stroke.GetBounds().Y + ms.Stroke.GetBounds().Height;
                            }

                        }
                        else
                        {
                            ms.IsExist = true;
                        }
                    }
                }

                if (_titleInk.Title_InkFrame.InkCollector.Sketch.Images.Count > 0)
                {
                    foreach (MyImage ms in this._titleInk.Title_InkFrame.InkCollector.Sketch.Images)
                    {
                        if (ms.VideoPath == _titleInk.Title_InkFrame.InkCollector.VideoPath)
                        {
                            ms.IsExist = false;
                            if (ms.Left < minX)
                            {
                                minX = ms.Left;
                            }
                            if (ms.Top < minY)
                            {
                                minY = ms.Top;
                            }
                            if (ms.Left + ms.Width > maxX)
                            {
                                maxX = ms.Left + ms.Width;
                            }
                            if (ms.Top + ms.Height > maxY)
                            {
                                maxY = ms.Top + ms.Height;
                            }
                        }
                        else
                        {
                            ms.IsExist = true;
                        }
                    }
                }

                if (_titleInk.Title_InkFrame.InkCollector.Sketch.MyRichTextBoxs.Count > 0)
                {
                    foreach (MyRichTextBox ms in this._titleInk.Title_InkFrame.InkCollector.Sketch.MyRichTextBoxs)
                    {
                        if (ms.VideoPath == _titleInk.Title_InkFrame.InkCollector.VideoPath)
                        {
                            ms.IsExist = false;
                            if (ms.RichTextBox.Margin.Left < minX)
                            {
                                minX = ms.RichTextBox.Margin.Left;
                            }
                            if (ms.RichTextBox.Margin.Top < minY)
                            {
                                minY = ms.RichTextBox.Margin.Top;
                            }
                            if (ms.RichTextBox.Margin.Left + ms.RichTextBox.Width > maxX)
                            {
                                maxX = ms.RichTextBox.Margin.Left + ms.RichTextBox.Width;
                            }
                            if (ms.RichTextBox.Margin.Top + ms.RichTextBox.Height > maxY)
                            {
                                maxY = ms.RichTextBox.Margin.Top + ms.RichTextBox.Height;
                            }
                        }
                        else
                        {
                            ms.IsExist = true;
                        }
                    }
                }

                Stream streamSave = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Create);
                WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(this._titleInk.Title_InkFrame.InkCollector, streamSave);
                Stream streamOpen = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Open);
                scaling = (_titleButton.Width - 8) / (Math.Abs(maxX - minX));
                InkConstants.InkCanvasTransform(_titleInkFrame._inkCanvas, 1, 1, scaling, scaling);

                //检查要添加的titleButton是否已添加了
                int i = 0;
                foreach (MyButton mb in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
                {
                    //已经存在的情况
                    if (mb.IsGlobal == true && mb.VideoPath == _titleInk.Title_InkFrame.InkCollector.VideoPath)
                    {

                        mb.InkFrame._inkCanvas.Strokes.Clear();
                        mb.InkFrame._inkCanvas.Children.Clear();
                        mb.InkFrame.InkCollector.Sketch.Images.Clear();
                        mb.InkFrame.InkCollector.Sketch.MyStrokes.Clear();
                        mb.InkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
                        WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(mb.InkFrame.InkCollector, streamOpen);
                        //识别笔迹
                        MyInkAnalyzer mia = new MyInkAnalyzer(mb.InkFrame._inkCanvas.Strokes);
                        //_videoOperation._updateRecogntionRusult.Visibility = Visibility.Visible;
                        _videoOperation._updateRecogntionRusult.setMyButtonVideoAnnotation(mb, this, "title");
                        _videoOperation._updateRecogntionRusult._textBox.Text = mia.getAnalyzeResults();
                        break;
                    }
                    i++;
                }
                if (i == _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Count)//不存在则添加
                {
                    WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(_titleInkFrame.InkCollector, streamOpen);
                    MyButton _titleMyButton = new MyButton(_titleButton);
                    Command mmssc = new MoveMyStrokesCommand(_titleInkFrame.InkCollector.Sketch.MyStrokes, -minX, -minY);
                    mmssc.execute();
                    Command mmisc = new MoveMyImagesCommand(_titleInkFrame.InkCollector.Sketch.Images, -minX, -minY);
                    mmisc.execute();
                    Command mtsc = new MoveTextsCommand(_titleInkFrame.InkCollector.Sketch.MyRichTextBoxs, -minX, -minY);
                    mtsc.execute();
                    _titleMyButton.ContentMoveX = -minX;
                    _titleMyButton.ContentMoveY = -minY;
                    _titleMyButton.ContentScaling = scaling;
                    _titleMyButton.InkFrameWidth = _titleInkFrame.Width;
                    _titleMyButton.InkFrameHeight = _titleInkFrame.Height;
                    _titleMyButton.VideoPath = videoPath;
                    _titleMyButton.TimeStart = 0;
                    _titleMyButton.TimeEnd = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                    _titleMyButton.IsGlobal = true;
                    _titleMyButton.Button.Height = Math.Abs(maxY - minY) < Math.Abs(maxX - minX) ?
                    _titleMyButton.Button.Height : _titleMyButton.Button.Width * Math.Abs(maxY - minY) / Math.Abs(maxX - minX);
                    _titleMyButton.InkFrame = _titleInkFrame;
                    AddThumbMyButton(_titleMyButton);
                }
            }
        }

        public void OpenVideoBySource(Uri source)
        {
			Dispose();
            mediaPlayer.Source = source;

        }

        private void SeekToMediaPosition(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
			if (mediaPlayer.Source != null)
			{
				int SliderValue = (int)timelineSlider.Value;
				setPositon(SliderValue);
				if (_thumbInk.Thumb_InkFrame.InkCollector.Mode == InkMode.VideoPlay)
				{
					Annotation_InkFrame.InkCollector.InkCanvas.Children.Clear();
					Annotation_InkFrame.InkCollector.InkCanvas.Strokes.Clear();
					Annotation_InkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
					Annotation_InkFrame.InkCollector.Sketch.Images.Clear();
					Annotation_InkFrame.InkCollector.Sketch.MyStrokes.Clear();
					Annotation_InkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
					Annotation_InkFrame.InkCollector.Mode = InkMode.Ink;
					foreach (MyButton mb in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
					{
						if (mb.IsGlobal == false && mb.TimeStart < SliderValue && mb.TimeEnd > SliderValue && mb.VideoPath == mediaPlayer.Source.ToString())
						{

							//下面是在视频上显示草图
							Stream streamSave = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Create);
							WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(mb.InkFrame.InkCollector, streamSave);
							Stream streamOpen = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Open);
							WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(Annotation_InkFrame.InkCollector, streamOpen);

							mb.InkFrame._inkCanvas.Background = Brushes.LemonChiffon;
						}
						if (mb.IsGlobal == false && (mb.TimeStart > SliderValue || mb.TimeEnd < SliderValue) && mb.VideoPath == mediaPlayer.Source.ToString())
						{
							mb.InkFrame._inkCanvas.Background = Brushes.White;
						}
						if (mb.IsGlobal && mb.VideoPath == mediaPlayer.Source.ToString())
						{
							mb.InkFrame._inkCanvas.Background = Brushes.LemonChiffon;
						}
						if (mb.VideoPath != mediaPlayer.Source.ToString())
						{
							mb.InkFrame._inkCanvas.Background = Brushes.White;
						}
					}
				}
			}
        }

        private void PlayButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mediaPlayer.Source != null)
            {  
                PlayVideo();
            }
            else
            {
                OpenAndPlayVideo();
            }
            
        }

        private void PauseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PauseVideo();
        }

        private void StopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StopVideo();
        }

        #region 视频控制函数

        //定位视频，position的单位是秒
        public void setPositon(int position)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, position);
            mediaPlayer.Position = ts;
            mediaPlayer.Play();
            timer.Start();
			//MathTool.getInstance().getFrameByPosition(mediaPlayer, position, "firstImage");
        }

        //播放视频
        public void PlayVideo()
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Play();
                timer.Start();

            }
        }

        //暂停视频
        public void PauseVideo()
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Pause();
                timer.Stop();
            }
        }

        //停止视频
        public void StopVideo()
        {
            if (mediaPlayer.Source != null)
            {
				foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
				{
					if (myButtonLocal.InkFrame._inkCanvas.Background == Brushes.LemonChiffon)
					{
						myButtonLocal.InkFrame._inkCanvas.Background = Brushes.White;
					}
				}  
                mediaPlayer.Stop();
                timer.Stop();
                timelineSlider.Value = 0;
            }
        }
        #endregion

        private void mediaPlayer_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
			bool isHasNext = false;
			foreach (MyButton mb in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
			{
				if(mb.VideoPath==mediaPlayer.Source.ToString())
				{
					foreach (MyArrow ma in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyArrows)
					{
						if (ma.PreMyButton == mb && ma.NextMyButton != null)
						{
							isHasNext = true;
						}
					}
				}
			}
			if (!isHasNext)
			{
				StopVideo();
			}
        }           
        

        private void ButtonStaticAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mediaPlayer.Source != null)
            {    
                _thumbButton = new Button();
                _thumbButton.Width = _thumbWidth;   
                _videoCollector.ThumbHeightWidthRate = this.Annotation_InkFrame._inkCanvas.ActualHeight / this.Annotation_InkFrame._inkCanvas.ActualWidth;
				_thumbButton.Height = _thumbWidth * (_videoCollector.ThumbHeightWidthRate);
                _thumbMyButton = new MyButton(_thumbButton);
                string videoPath = mediaPlayer.Source.ToString();
                _thumbMyButton.VideoPath = videoPath;
                foreach (MyVideo myVideo in _myVideoList)
                {
                    if (myVideo.VideoPath == videoPath)
                    {
                        _thumbButton.Background = myVideo.Background;
                    }
                }
                _thumbMyButton.TimeStart = mediaPlayer.Position.TotalMilliseconds; //记录当前时间
                _thumbInkFrame = new InkFrame();
                _thumbInkFrame.Width = Annotation_InkFrame.ActualWidth;
                _thumbInkFrame.Height = Annotation_InkFrame.ActualHeight;
				_thumbMyButton.InkFrameWidth = Annotation_InkFrame.ActualWidth;
				_thumbMyButton.InkFrameHeight = Annotation_InkFrame.ActualHeight;
                _thumbInkFrame.InkCollector.Mode = InkMode.None;
                _thumbInkFrame._inkCanvas.Margin = new Thickness(0);
                
                _thumbMyButton.IsStaticAnnotion = true;
                ButtonStaticStart.Visibility = Visibility.Visible;
                ButtonStaticAdd.Visibility = Visibility.Hidden;
				    
                PauseVideo();       //暂停播放视频
                isStaticAnnotation = true;
                ButtonDynamicAdd.IsEnabled = false;
            }
        }   
        

        public void _thumbInkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
			if (_thumbInk.Thumb_InkFrame.InkCollector.Mode == InkMode.AutoMove)
			{
				if (_thumbInk.Thumb_InkFrame.InkCollector.IsAutoMove == false)
				{
					foreach (MyButton mb in _thumbInk.Thumb_InkFrame.InkCollector.SelectButtons)
					{
						mb.TextBoxTime.Background = null;
					}
					_thumbInk.Thumb_InkFrame.InkCollector.SelectButtons.Clear();
					if (_thumbInk.Thumb_InkFrame.InkCollector.SelectButtons.Count == 0)
					{
						InkCanvas inkCanvas = (InkCanvas)sender;
						inkCanvas.CaptureMouse();
						inkCanvas.Cursor = Cursors.ScrollAll;
						int i = 0;      //记录下标
						foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
						{
							if (myButtonLocal.InkFrame._inkCanvas != inkCanvas)
							{
								i++;
							}
							else
							{
								break;
							}
						}
						if (this._thumbInk.Thumb_InkFrame.InkCollector.SelectButtons.IndexOf(_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i]) == -1)
						{
							this._thumbInk.Thumb_InkFrame.InkCollector.SelectButtons.Add(_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i]);

						}
						_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].TextBoxTime.Background = Brushes.CornflowerBlue;

					}
					this._thumbInk.Thumb_InkFrame.InkCollector.Mode = InkMode.AutoMove;
				}
			}
            
        }

        private int _thumbIndex;

        /// <summary>
        /// 缩略图点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  void _thumbInkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this._thumbInk.Thumb_InkFrame.InkCollector.Mode == InkMode.VideoPlay)
            {

                InkCanvas inkCanvas = (InkCanvas)sender;
                int i = 0;      //记录下标
                foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
                {
                    if (myButtonLocal.InkFrame._inkCanvas != inkCanvas)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
				_thumbIndex = i;
				//增加渐变效果
                //Random random = new Random(unchecked((int)DateTime.Now.Ticks));
                //int storyBoardRandomIndex = random.Next(0, StoryboardInList.Count);
                //storyBoardRandomIndex = storyBoardRandomIndex == StoryboardInList.Count ? storyBoardRandomIndex-- : storyBoardRandomIndex;
                //StoryboardInList[storyBoardRandomIndex].Begin();
                //selectInkCanvasStoryboard(storyBoardRandomIndex, "In").Begin(Annotation_InkFrame._inkCanvas);
                OpenVideoBySource(new Uri(_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].VideoPath));
                double TimeStart = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].TimeStart;
                double TimeEnd = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].TimeEnd;
                setPositon((int)TimeStart); //定位视频
				_titleInk.Title_InkFrame.InkCollector.VideoPath = mediaPlayer.Source.ToString();

				Annotation_InkFrame._inkCanvas.Children.Clear();
				Annotation_InkFrame._inkCanvas.Strokes.Clear();
				Annotation_InkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
				Annotation_InkFrame.InkCollector.Sketch.Images.Clear();
				Annotation_InkFrame.InkCollector.Sketch.MyStrokes.Clear();
				Annotation_InkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
				Annotation_InkFrame.InkCollector.Mode = InkMode.Ink;
				if (_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].IsGlobal == false)
				{
					//下面是在视频上显示草图

					Stream streamSave = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Create);
					WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].InkFrame.InkCollector, streamSave);
					Stream streamOpen = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Open);
					WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(Annotation_InkFrame.InkCollector, streamOpen);

				}
                _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].InkFrame._inkCanvas.Background = Brushes.LemonChiffon;
                foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
                {
                    if (myButtonLocal.InkFrame._inkCanvas != inkCanvas && myButtonLocal.InkFrame._inkCanvas.Background == Brushes.LemonChiffon)
                    {
                        myButtonLocal.InkFrame._inkCanvas.Background = Brushes.White;  
                    }
                }                      
            }
            
        }

        private void ButtonStatic()
        {
            if (mediaPlayer.Source != null)
            {
                //记录当前时间代码

                if (this.Annotation_InkFrame._inkCanvas.Strokes.Count > 0 || this.Annotation_InkFrame._inkCanvas.Children.Count > 0)
                {
                    if (mediaPlayer.Position.TotalMilliseconds < _thumbMyButton.TimeStart)
                    {
                        _thumbMyButton.TimeEnd = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        _thumbMyButton.TimeEnd = mediaPlayer.Position.TotalMilliseconds;
                    }
                    if (Annotation_InkFrame._inkCanvas.Children.Count > 1)
                    {
                        Command com = new DeleteImageCommand(this.Annotation_InkFrame.InkCollector, this.Annotation_InkFrame.InkCollector.Sketch.Images[1]);
                        com.execute();
                    }
                    Stream streamSave = new FileStream(GlobalValues.FilesPath + @"\WPFInk\WPFInk\cache\a.xml", FileMode.Create);
                    WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(this.Annotation_InkFrame.InkCollector, streamSave);
                    Stream streamOpen = new FileStream(GlobalValues.FilesPath + @"\WPFInk\WPFInk\cache\a.xml", FileMode.Open);
                    WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(_thumbInkFrame.InkCollector, streamOpen);
                    scaling = (_thumbButton.Width - 6) / this.Annotation_InkFrame.ActualWidth;
                    InkConstants.InkCanvasTransform(_thumbInkFrame._inkCanvas, 1, 1, scaling, scaling);
                    _thumbMyButton.InkFrame = _thumbInkFrame;
                    AddThumbMyButton(_thumbMyButton);
                }
                else
                {
                    _thumbMyButton = null;
                }
                //ButtonStaticOk.Visibility = Visibility.Hidden;
                //ButtonStaticAdd.Visibility = Visibility.Visible;
                isStaticAnnotation = false;
                ButtonDynamicAdd.IsEnabled = true;
            }
        }

        private void ButtonStaticOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonStatic();
            
        }

        private void ButtonStaticStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                ButtonStaticOk.Visibility = Visibility.Visible;
                ButtonStaticStart.Visibility = Visibility.Hidden;
                PlayVideo();
            }

        }
        //添加动态注释
        public void ButtonDynamicAddOk()
        {
            if (mediaPlayer.Source != null)
            {
                //记录当前时间代码   
                if (this.Annotation_InkFrame._inkCanvas.Strokes.Count > 0 || this.Annotation_InkFrame._inkCanvas.Children.Count > 0)
                {
                    dynamicStrokes = 0;
                    if (mediaPlayer.Position.TotalMilliseconds < _thumbMyButtonD.TimeStart)
                    {
                        _thumbMyButtonD.TimeEnd = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        _thumbMyButtonD.TimeEnd = mediaPlayer.Position.TotalMilliseconds;
                    }
                    if (Annotation_InkFrame._inkCanvas.Children.Count > 1)
                    {
                        Command com = new DeleteImageCommand(this.Annotation_InkFrame.InkCollector, this.Annotation_InkFrame.InkCollector.Sketch.Images[1]);
                        com.execute();
                    }
                    Stream streamSave = new FileStream(GlobalValues.FilesPath + @"\WPFInk\WPFInk\cache\a.xml", FileMode.Create);
                    WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(this.Annotation_InkFrame.InkCollector, streamSave);
                    Stream streamOpen = new FileStream(GlobalValues.FilesPath + @"\WPFInk\WPFInk\cache\a.xml", FileMode.Open);
                    WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(_thumbInkFrameD.InkCollector, streamOpen);
                    scaling = (_thumbButtonD.Width - 6) / this.Annotation_InkFrame.ActualWidth;
                    InkConstants.InkCanvasTransform(_thumbInkFrameD._inkCanvas, 1, 1, scaling, scaling);
                    _thumbMyButtonD.InkFrame = _thumbInkFrameD;
                    AddThumbMyButton(_thumbMyButtonD);
                }

                isStaticAnnotation = false;
                ButtonStaticAdd.IsEnabled = true;
            }
        }
        private void ButtonDynamicAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonDynamicAddOk();
        }

        //清除显示的缩略图按钮  清除显示的时间Textbox
        private void RemoveThumbListFromWindow()
        {
            foreach (MyButton myButton in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
            {
                if (_thumbInk.Thumb_InkFrame._inkCanvas.Children.IndexOf(myButton.Button)>-1)
                {
                    _thumbInk.Thumb_InkFrame._inkCanvas.Children.Remove(myButton.Button);
                }
                if (_thumbInk.Thumb_InkFrame._inkCanvas.Children.IndexOf(myButton.TextBoxTime)>-1)
                {
                    _thumbInk.Thumb_InkFrame._inkCanvas.Children.Remove(myButton.TextBoxTime);
                }
               // myButton.MyStrokeList.Clear();
            }
            _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Clear();
        }



        
		
		private bool isUpdatedRecognitionResults = false;//是否修正过识别结果的标志位
		private string updatedRecognitionResults = "";
		public string UpdatedRecognitionResults
		{
			get { return updatedRecognitionResults; }
			set { updatedRecognitionResults = value; }
		}

		public bool IsUpdatedRecognitionResults
		{
			get { return isUpdatedRecognitionResults; }
			set { isUpdatedRecognitionResults = value; }
		}
		//添加mybutton，默认位置
        public void AddThumbMyButton(MyButton myButton)
        {
			//识别笔迹
            //MyInkAnalyzer mia = new MyInkAnalyzer(myButton.InkFrame._inkCanvas.Strokes);
            //if (isUpdatedRecognitionResults == false && myButton.InkFrame._inkCanvas.Strokes.Count > 0)
            //{
            //    _videoOperation._updateRecogntionRusult.Visibility = Visibility.Visible;
            //    _videoOperation._updateRecogntionRusult.setMyButtonVideoAnnotation(myButton,this,"thumb");
            //    _videoOperation._updateRecogntionRusult._textBox.Text = mia.getAnalyzeResults();
            //    return;
            //}
            //else
            //{
            //    isUpdatedRecognitionResults = false;
            //}
			
            int ThumbIndex = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Count;
            myButton.Left = _thumbInterval  + (_thumbWidth + _thumbInterval) * (ThumbIndex % _thumbCountPerLine);
			if (ThumbIndex < _thumbCountPerLine)
			{
                myButton.Top = _thumbVerticalInterval;// +(_thumbWidth * _videoCollector.ThumbHeightWidthRate + _thumbInterval) * (ThumbIndex / 6);
			}
			else
			{
				if (ThumbIndex % _thumbCountPerLine == 0)
				{
					double maxTop = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[ThumbIndex - _thumbCountPerLine].Top + _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[ThumbIndex - _thumbCountPerLine].Height;
					for (int i = ThumbIndex - 5; i < ThumbIndex; i++)
					{
						double newTop = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].Top + _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].Height;
						if (newTop > maxTop)
						{
							maxTop = newTop;
						}
					}
                    myButton.Top = _thumbVerticalInterval + maxTop;
				}
				else
				{
					myButton.Top = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[(ThumbIndex / _thumbCountPerLine)*_thumbCountPerLine].Top;
				}
			}
            myButton.setLocation((int)myButton.Left, (int)myButton.Top);
            myButton.adjustTextBoxTime();
			myButton.VideoAnnotation = this;
			myButton.VideoFileName = System.IO.Path.GetFileName(myButton.VideoPath);
			addItemToVideoList(myButton);
            myButton.InkFrame._inkCanvas.AddHandler(InkCanvas.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this._thumbInkCanvas_MouseLeftButtonDown), true);
            myButton.Button.AddHandler(Button.MouseLeaveEvent, new MouseEventHandler(Button_MouseLeave), true);
			bool isExistGloabalButton = false;
			if (myButton.IsGlobal)
			{
				foreach (MyButton mb in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
				{
					if (mb.IsGlobal && mb.IsDeleted == false && mb.VideoPath == myButton.VideoPath)
					{
						isExistGloabalButton = true;
						break;
					}
				}
				if (!isExistGloabalButton)
				{
					myButton.TextBoxTime.IsReadOnly = true;
					//添加stroke分布条
					myButton.updateRectangles(_thumbInk.Thumb_InkFrame._inkCanvas, _thumbInk.Thumb_InkFrame.InkCollector);
				}
			}
			else
			{
				foreach (MyButton mb in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
				{
					if (mb.IsDeleted == false && mb.IsGlobal && mb.VideoPath == myButton.VideoPath)
					{
						myButton.updateRectangles(_thumbInk.Thumb_InkFrame._inkCanvas, _thumbInk.Thumb_InkFrame.InkCollector);
					}
				}
			}
            myButton.TextBoxTime.KeyDown += new KeyEventHandler(TextBoxTime_KeyDown);
            myButton.addContent();    
            myButton.Id = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Count;
			
			
            AddButtonCommand abc = new AddButtonCommand(_thumbInk.Thumb_InkFrame.InkCollector, myButton);
            abc.execute();
            _thumbInk.Thumb_InkFrame.InkCollector.CommandStack.Push(abc);
            Annotation_InkFrame.InkCollector.InkCanvas.Children.Clear();
            Annotation_InkFrame.InkCollector.InkCanvas.Strokes.Clear();
            Annotation_InkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
            Annotation_InkFrame.InkCollector.Sketch.Images.Clear();
            Annotation_InkFrame.InkCollector.Sketch.MyStrokes.Clear();
			Annotation_InkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
            Annotation_InkFrame.InkCollector.Sketch.MyGraphics.Clear();
            Annotation_InkFrame.InkCollector.Mode = InkMode.Ink;


            //测试相似性算法
            //if (_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Count > 1)
            //{
            //    int maxindex = 0;
            //    float maxresult = 0;
            //    for (int i = 0; i < _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Count - 1;i++ )
            //    {
            //        float result=InkTool.getInstance().getSketchSimilarity(
            //            _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Count-1].InkFrame._inkCanvas.Strokes,
            //            _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].InkFrame._inkCanvas.Strokes);
            //        if (result > maxresult)
            //        {
            //            maxresult = result;
            //            maxindex = i;
            //        }
            //    }

            //    MessageBox.Show(maxindex.ToString()+","+maxresult.ToString());
            //}
        }

		public void smallRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Rectangle r = (Rectangle)sender;
			int i = 0;      //记录下标
			MySmallRectangle mySmallRectangle;
			foreach (MySmallRectangle msr in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.mySmallRectangles)
			{
				if (msr.Rectangle == r)
				{
					mySmallRectangle = msr;
					foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
					{
						if (myButtonLocal != mySmallRectangle.MyButton && myButtonLocal.InkFrame._inkCanvas.Background == Brushes.LemonChiffon)
						{
							myButtonLocal.InkFrame._inkCanvas.Background = Brushes.White;
						}
					} 
					foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
					{
						if (myButtonLocal != mySmallRectangle.MyButton)
						{
							i++;
						}
						else
						{
							break;
						}
					}
					break;
				}
			}
			OpenVideoBySource(new Uri(_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].VideoPath));
			double TimeStart = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].TimeStart;
			double TimeEnd = _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].TimeEnd;
			setPositon((int)TimeStart); //定位视频
			if (_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].IsGlobal == false)
			{
				//下面是在视频上显示草图
				Annotation_InkFrame.InkCollector.InkCanvas.Children.Clear();
				Annotation_InkFrame.InkCollector.InkCanvas.Strokes.Clear();
				Annotation_InkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
				Annotation_InkFrame.InkCollector.Sketch.Images.Clear();
				Annotation_InkFrame.InkCollector.Sketch.MyStrokes.Clear();
				Annotation_InkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
				Annotation_InkFrame.InkCollector.Mode = InkMode.Ink;

				Stream streamSave = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Create);
				WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].InkFrame.InkCollector, streamSave);
				Stream streamOpen = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Open);
				WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(Annotation_InkFrame.InkCollector, streamOpen);

			}
			_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i].InkFrame._inkCanvas.Background = Brushes.LemonChiffon;
			      

			
		}

        

       

        public void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            int i = 0;      //记录下标
            foreach (MyButton myButtonLocal in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
            {
                if (myButtonLocal.Button != button)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            foreach (MyButton myButton in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
            {
				if (_thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[i] != myButton && this._thumbInk.Thumb_InkFrame.InkCollector.SelectButtons.IndexOf(myButton) > -1)
				{
                    myButton.TextBoxTime.Background = null;
                }
            }
			//_thumbInk.Thumb_InkFrame.InkCollector.SelectButtons.Clear();
            
        }

      

        //添加mybutton，指定位置
        public void AddThumbMyButtonByLeftTop(MyButton myButton,double left,double top)
        {
            myButton.Left = left;
            myButton.Top = top;
            myButton.setLocation((int)myButton.Left, (int)myButton.Top);
			myButton.adjustTextBoxTime();
			myButton.updateRectangles(_thumbInk.Thumb_InkFrame._inkCanvas, _thumbInk.Thumb_InkFrame.InkCollector);
			myButton.VideoAnnotation = this;

			bool isExist = false;
			foreach (MyVideo mv in _myVideoList)
			{
				if (mv.VideoPath == myButton.VideoPath)
				{
					isExist = true;
					break;
				}
			}
			if (isExist == false)
			{
				MyVideo mv = new MyVideo();
				mv.VideoPath = myButton.VideoPath;
				_myVideoList.Add(mv);
			}
			addItemToVideoList(myButton);
			AddButtonCommand abc = new AddButtonCommand(_thumbInk.Thumb_InkFrame.InkCollector, myButton);
			abc.execute();
			_thumbInk.Thumb_InkFrame.InkCollector.CommandStack.Push(abc);
        }

        

        public void TextBoxTime_KeyDown(object sender, KeyEventArgs e)
        {
			if (e.Key == Key.Enter)
			{
				string timeStr = ((TextBox)sender).Text.ToString();
				if (timeStr.Length != 17 || !isAllDigitString(timeStr) || (!isAllColon(timeStr)) || (timeStr.Substring(8, 1) != "-"))
				{
					MessageBox.Show("Wrong time format!");
				}
				else
				{
					List<int> hms = new List<int>();
					for (int i = 0; i < timeStr.Length; i = i + 3)
					{
						hms.Add(int.Parse(timeStr.Substring(i, 2)));
					}
					double timeStart = (double)((hms[0] * 3600 + hms[1] * 60 + hms[2]) * 1000);
					double timeEnd = (double)((hms[3] * 3600 + hms[4] * 60 + hms[5]) * 1000);
					foreach (MyButton mb in _thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
					{
						if (mb.TextBoxTime == (TextBox)sender)
						{
							mb.TimeStart = timeStart;
							mb.TimeEnd = timeEnd;
						}
					}

					MessageBox.Show("Time updated successfully!");
				}
				
			}

        }

		public bool isDigitString(String str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				char c = Convert.ToChar(str.Substring(i,1));
				if (!char.IsDigit(c))
				{
					return false;
				}
			}
			return true;
		}

		public bool isAllDigitString(String str)
		{
			for (int i = 0; i < str.Length; i=i + 3)
			{
				if (!isDigitString(str.Substring(i, 2))||(i!=0&&i!=9&&int.Parse(str.Substring(i, 2))>=60))
				{
					return false;
				}
			}
			return true;
		}

		public bool isAllColon(String str)
		{
			for (int i = 2; i < str.Length; i = i + 3)
			{
				if(i!=8&&(str.Substring(i, 1) != ":"))
				{
					return false;
				}
			}
			return true;
		}


		private void timelineSlider_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			timelineSlider.CaptureMouse();
			Point p = e.GetPosition(timelineSlider);
			//MessageBox.Show(p.X.ToString());
			//MessageBox.Show(timelineSlider.ActualWidth.ToString());
			timelineSlider.Value = timelineSlider.Maximum * ((p.X - 5) / (timelineSlider.ActualWidth-10));
			setPositon((int)timelineSlider.Value);
			timelineSlider.ReleaseMouseCapture();
		}

		//清理内存
		public void Dispose()
		{
			
		}

		//GradientIn定义
		private Storyboard InkCanvasvideoGradientStroryBoardIn()
		{
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 0;
			myDoubleAnimation.To = 1;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation,Annotation_InkFrame._inkCanvas.Name);
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(InkCanvas.OpacityProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//RightToLeftIn定义
		private Storyboard InkCanvasRightToLeftStoryboardIn()
		{
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = Annotation_InkFrame._inkCanvas.ActualWidth;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = 0;
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 0;
			myDoubleAnimation.To = 1;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));
			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//LeftToRightIn定义
		private Storyboard InkCanvasLeftToRightStoryboardIn()
		{
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = 0;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = 0;
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 0;
			myDoubleAnimation.To = 1;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//TopToBottomIn定义
		private Storyboard InkCanvasTopToBottomStoryboardIn()
		{
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = 0;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = 0;
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 0;
			myDoubleAnimation.To = 1;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//BottomToTopIn定义
		private Storyboard InkCanvasBottomToTopStoryboardIn()
		{
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			//宽度缩小
			myDoubleAnimation.From = 0;
			myDoubleAnimation.To = 1;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = 0;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = Annotation_InkFrame._inkCanvas.ActualHeight;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//GradientIn定义
		private Storyboard InkCanvasvideoGradientStroryBoardOut()
		{
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = Annotation_InkFrame._inkCanvas.ActualWidth;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = 0;
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 1;
			myDoubleAnimation.To = 0;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, Annotation_InkFrame._inkCanvas.Name);
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(InkCanvas.OpacityProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//RightToLeftOut定义
		private Storyboard InkCanvasRightToLeftStoryboardOut()
		{
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = 0;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = 0;
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			//宽度缩小
			myDoubleAnimation.From = 1;
			myDoubleAnimation.To = 0;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//LeftToRightOut定义
		private Storyboard InkCanvasLeftToRightStoryboardOut()
		{
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 1;
			myDoubleAnimation.To = 0;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = Annotation_InkFrame._inkCanvas.ActualWidth;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = 0;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//TopToBottomOut定义
		private Storyboard InkCanvasTopToBottomStoryboardOut()
		{
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = 0;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = Annotation_InkFrame._inkCanvas.ActualHeight;
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 1;
			myDoubleAnimation.To = 0;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}
		//BottomToTopOut定义
		private Storyboard InkCanvasBottomToTopStoryboardOut()
		{
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = 1;
			myDoubleAnimation.To = 0;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterX = 0;
			Annotation_InkFrame.InkCanvasScaleTrans.CenterY = 0;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));
			Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

			Storyboard myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			return myStoryboard;
		}

		private Storyboard selectInkCanvasStoryboard(int index ,string InOrOut)
		{
			Storyboard sb=null;
			if (InOrOut == "In")
			{
				switch (index)
				{
					case 0:
						sb = InkCanvasvideoGradientStroryBoardIn();
						break;
					case 1:
						sb = InkCanvasRightToLeftStoryboardIn();
						break;
					case 2:
						sb = InkCanvasLeftToRightStoryboardIn();
						break;
					case 3:
						sb = InkCanvasBottomToTopStoryboardIn();
						break;
					case 4:
						sb = InkCanvasTopToBottomStoryboardIn();
						break;
				}
			}
			if (InOrOut == "Out")
			{
				switch (index)
				{
					case 0:
						sb = InkCanvasvideoGradientStroryBoardOut();
						break;
					case 1:
						sb = InkCanvasLeftToRightStoryboardOut();
						break;
					case 2:
						sb = InkCanvasRightToLeftStoryboardOut();
						break;
					case 3:
						sb = InkCanvasTopToBottomStoryboardOut();
						break;
					case 4:
						sb = InkCanvasBottomToTopStoryboardOut();
						break;
				}
			}
			return sb;
		}


		private void addItemToVideoList(MyButton mb)
		{
			bool isExit = false;
			foreach (ListBoxItem li in _videoOperation._videoList.VideoList_ListBox.Items)
			{
				if (li.Content.ToString() == mb.VideoFileName)
				{
					isExit = true;
					break;
				}
			}
			if (!isExit)
			{
				ListBoxItem listBoxItem = new ListBoxItem();
				listBoxItem.Height = 30;
				listBoxItem.Width = 144;
				listBoxItem.Content = mb.VideoFileName;
				listBoxItem.Background = mb.Button.Background;
				listBoxItem.BorderThickness = new Thickness(1);
				listBoxItem.BorderBrush = Brushes.Black;
				listBoxItem.VerticalAlignment = VerticalAlignment.Top;
				listBoxItem.HorizontalAlignment = HorizontalAlignment.Left;
				_videoOperation._videoList.VideoList_ListBox.Items.Add(listBoxItem);
				int count = _videoOperation._videoList.VideoList_ListBox.Items.Count;
				if (count > 0)
				{
                    //_videoOperation._videoList.Visibility = Visibility.Visible;

				}
				if (count > 0 && count < 11)
				{
					_videoOperation._videoList.VideoList_ListBox.Height = 30 * count + 6;
					_videoOperation._videoList.Height = 20 + 30 * count + 6;
					if (_videoOperation._videoList.MinButton.Visibility == Visibility.Visible)
					{
						MyStoryboard.getInstance().HeightStoryboard(_videoOperation._videoList.border, 30 * (count - 1) + 6, 30 * count + 6, 0.5).Begin(_videoOperation._videoList);
					}
				}
			}
			
			
		}

		private void timelineSlider_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (mediaPlayer.Source != null)
			{
				Point p = e.GetPosition(timelineSlider);
				List<string> timeCurr = new List<string>();
				if (p.X < 5 || p.X > timelineSlider.ActualWidth - 5)
				{
					timeLabel.Content = "";
				}
				else
				{
					timeCurr = ConvertClass.getInstance().MsToHMS(timelineSlider.Maximum * ((p.X - 5) / (timelineSlider.ActualWidth-10)));
					timeLabel.Content = timeCurr[0] + ":" + timeCurr[1] + ":" + timeCurr[2];
					timeLabel.Margin = new Thickness(p.X - 10, timeLabel.Margin.Top, 0, 0);
				}
			}
		}

		private void timelineSlider_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			timeLabel.Content = "";
		}
        public void addKeyFrameButton(Image image,int time)
        {
            image.Height = image.Width * Annotation_InkFrame.ActualHeight / Annotation_InkFrame.ActualWidth;
            Button _keyFrameButton = new Button();
            _keyFrameButton.Width = _thumbWidth;
            _videoCollector.ThumbHeightWidthRate = this.Annotation_InkFrame._inkCanvas.ActualHeight / this.Annotation_InkFrame._inkCanvas.ActualWidth;
            _keyFrameButton.Height = _thumbWidth * (_videoCollector.ThumbHeightWidthRate);
            MyButton _keyFrameThumbMyButton = new MyButton(_keyFrameButton);
            string videoPath = mediaPlayer.Source.ToString();
            _keyFrameThumbMyButton.VideoPath = videoPath;
            foreach (MyVideo myVideo in _myVideoList)
            {
                if (myVideo.VideoPath == videoPath)
                {
                    _keyFrameButton.Background = myVideo.Background;
                }
            }
            _keyFrameThumbMyButton.TimeStart = _keyFrameThumbMyButton.TimeEnd=time; //记录当前时间
            InkFrame _keyFrameThumbInkFrame = new InkFrame();
            _keyFrameThumbInkFrame.Width = image.Width;
            _keyFrameThumbInkFrame.Height = image.Height;
            _keyFrameThumbMyButton.InkFrameWidth = image.Width;
            _keyFrameThumbMyButton.InkFrameHeight = image.Height;
            _keyFrameThumbInkFrame.InkCollector.Mode = InkMode.None;
            _keyFrameThumbInkFrame._inkCanvas.Margin = new Thickness(0);
            _keyFrameThumbInkFrame._inkCanvas.Children.Add(image);
            double scaling = (_keyFrameButton.Width - 6) / image.Width;
            InkConstants.InkCanvasTransform(_keyFrameThumbInkFrame._inkCanvas, 1, 1, scaling, scaling);
            _keyFrameThumbMyButton.InkFrame = _keyFrameThumbInkFrame;
            AddThumbMyButton(_keyFrameThumbMyButton);
        }
	}
}