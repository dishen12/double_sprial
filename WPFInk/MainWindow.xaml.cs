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
using WPFInk.PersistenceManager;
using WPFInk.ink;
using WPFInk.cmd;
using Microsoft.Win32;
using System.Windows.Ink;
using WPFInk.tool;
using System.Windows.Media.Animation;
using WPFInk.ChineseWordSegmentation;
using WPFInk.video;
using WPFInk.graphic;
using WPFInk.Global;

namespace WPFInk
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InkCollector _inkCollector;

        private int zoomMaxSign = 0;//詹启Add
		private bool isUpdatedRecognitionResults = false;//是否修正过识别结果的标志位
		private string updatedRecognitionResults="";
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

        public MainWindow()
        {
            InitializeComponent();
            InitApp();
        }

        private void InitApp()
        {
           
            //将controlpanel和inkframe关联
            _controlPanel.setInkFrame(_inkFrame);
            _inkCollector = _inkFrame.InkCollector;

            //时间轴
            this.timeBar.OnStart += new WPFInk.StrokePlayer.startPlay(timeBar_OnStart);
            this.timeBar.OnEnd += new WPFInk.StrokePlayer.startPlay(timeBar_OnEnd);
            this.timeBar.OnStop += new WPFInk.StrokePlayer.startPlay(timeBar_OnStop);
            

        }

        

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	this.Close();
        } 
        

        private void SaveAsDrawFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "xml",
                Filter = "xml　files　(*.xml)|*.xml|All　files　(*.*)|*.*",
                FilterIndex = 1
            };
            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(this._inkFrame.InkCollector, stream);
                    stream.Close();
                }
            }
                       
        }


        //打开Dat文件
        private void OpenDat_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "dat|*.dat";
            if (dlg.ShowDialog()==true)
            {
                // 获取dat文件所在目录名
                FileInfo info = new FileInfo(dlg.FileName);
                string strDirectoryName = info.DirectoryName;
                // 自己写文件的方式
                StreamReader reader = new StreamReader(dlg.FileName);

                while (false == reader.EndOfStream)
                {
                    MyImage mi = new MyImage(strDirectoryName + "\\" + reader.ReadLine());
                    AddImageCommand cmd = new AddImageCommand(_inkFrame.InkCollector, mi);
                    _inkFrame.InkCollector.CommandStack.Push(cmd);
                    cmd.execute();
                    mi.setLocation(int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine()));
                }

            }

        }                                                                                                         


        private void OpenDrawFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this._inkFrame._inkCanvas.Children.Count != 0 || this._inkFrame._inkCanvas.Strokes.Count != 0)
            {
                System.Windows.Forms.DialogResult MsgBoxResult;//设置对话框的返回值
                MsgBoxResult = System.Windows.Forms.MessageBox.Show("删除以后将不能恢复，是否删除", "提示", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (MsgBoxResult == System.Windows.Forms.DialogResult.Yes)//如果对话框的返回值是YES（按"Y"按钮）
                {
                    //先清空Inkcanvas
                    _inkFrame.InkCollector.InkCanvas.Children.Clear();
                    _inkFrame.InkCollector.InkCanvas.Strokes.Clear();
                    //打开文件
                    OpenFileDialog openfile = new OpenFileDialog()
                    {
                        Filter = "Xml Files (*.xml)|*.xml|All Files(*.*)|*.*",
                        Multiselect = false
                    };

                    if (openfile.ShowDialog() == true)
                    {
                        //FileInfo info = openfile.OpenFile();;

                        using (Stream stream = openfile.OpenFile())
                        {
                            WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(_inkFrame.InkCollector, stream);
                            stream.Close();
                        }
                    }
                }
            }
            else
            {
                //打开文件
                OpenFileDialog openfile = new OpenFileDialog()
                {
                    Filter = "Xml Files (*.xml)|*.xml|All Files(*.*)|*.*",
                    Multiselect = false
                };

                if (openfile.ShowDialog() == true)
                {
                    //FileInfo info = openfile.OpenFile();;

                    using (Stream stream = openfile.OpenFile())
                    {
                        WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(_inkFrame.InkCollector, stream);
                        stream.Close();
                    }
                }
            }
        }



        #region playback        

        //笔迹回放

        System.Windows.Forms.Timer Timer;
        private int playbackPointIndex = 0;
        private int playbackStrokeIndex = 0;
        private Stroke strokeToPlayback = null;
        private List<MyStroke> strokesForPlayback = null;
        /// <summary>
        /// 开始播放笔迹动画时的事件处理函数
        /// </summary>
        private void timeBar_OnStart()
        {
            this._controlPanel.Visibility = Visibility.Collapsed;
            if (_inkCollector.SelectedImages.Count > 0)
            {
				foreach (MyImage myImage in _inkCollector.SelectedImages)
                {
					myImage.Bound.Visibility = Visibility.Collapsed;
                }
            }
			_inkCollector.SelectedImages.Clear();
            strokesForPlayback = _inkCollector.Sketch.MyStrokes;        //所有要播放的笔迹
            if (strokesForPlayback.Count == 0)//没有需要回放的笔迹
                return;

            playbackPointIndex = 0;
            playbackStrokeIndex = 0;
            _inkFrame.InkCollector.InkCanvas.Strokes.Clear();
            _inkCollector.Mode = InkMode.InkPlay;
            _inkCollector.IsPlaying = true;
            Timer = new System.Windows.Forms.Timer();
            Timer.Interval = 50;
            Timer.Tick += new System.EventHandler(Timer_Tick);
            Timer.Start();
        }

        /// <summary>
        /// 笔迹回放
        /// </summary>
        private void strokePlay()
        {
            if ((strokesForPlayback.Count == 0) || (_inkCollector.IsPlaying == false))//没有要播放的笔迹或者不在播放状态
                return;
            MyStroke currentStroke = strokesForPlayback[playbackStrokeIndex];
            if (playbackPointIndex == 0)
            {
                StylusPointCollection spc = new StylusPointCollection();
                spc.Add(currentStroke.Stroke.StylusPoints[playbackPointIndex]);
                strokeToPlayback = new Stroke(spc);
                strokeToPlayback.DrawingAttributes = currentStroke.DrawingAttributes;
                _inkFrame._inkCanvas.Strokes.Add(strokeToPlayback);
                double progress = (double)(playbackStrokeIndex + 1) / (double)(strokesForPlayback.Count);
                this.timeBar.setProgress(progress);
            }

            strokeToPlayback.StylusPoints.Add(currentStroke.StylusPoints[playbackPointIndex]);
            playbackPointIndex++;
            if (playbackPointIndex < currentStroke.StylusPoints.Count)
            {
                strokeToPlayback.StylusPoints.Add(currentStroke.StylusPoints[playbackPointIndex]);
                playbackPointIndex++;
            }
            this.timeBar.StrokePlayButton = StrokePlayButton.StopPlay;
            if (playbackPointIndex == currentStroke.StylusPoints.Count)
            {
                playbackPointIndex = 0;
                playbackStrokeIndex++;
                if (playbackStrokeIndex >= strokesForPlayback.Count)
                {
                    //结束播放
                    _inkCollector.Mode = InkMode.Ink;
                    _inkFrame._inkCanvas.Strokes.Clear();

                    for (int i = 0; i < _inkCollector.Sketch.MyStrokes.Count; i++)
                    {
                        _inkFrame._inkCanvas.Strokes.Add(_inkCollector.Sketch.MyStrokes[i].Stroke);
                    }

                    this.timeBar.setProgress(0);
                    _inkCollector.IsPlaying = false;
                    _inkCollector.Mode = InkMode.Ink;
                    MyTimer.getInstance().Resume();
                    this.timeBar.StrokePlayButton = StrokePlayButton.StartPlay;
                    this._controlPanel.Visibility = Visibility.Visible;
                    return;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            strokePlay();
        }

        /// <summary>
        /// 停止播放时的事件处理函数
        /// </summary>
        private void timeBar_OnEnd()
        {
            _inkCollector.Mode = InkMode.Ink;
            _inkFrame._inkCanvas.Strokes.Clear();

            for (int i = 0; i < _inkCollector.Sketch.MyStrokes.Count; i++)
            {
                _inkFrame._inkCanvas.Strokes.Add(_inkCollector.Sketch.MyStrokes[i].Stroke);
            }
            this.timeBar.setProgress(0);
            _inkCollector.IsPlaying = false;
            _inkCollector.Mode = InkMode.Ink;
            this._controlPanel.Visibility = Visibility.Visible;
            //Timer.Stop();
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        private void timeBar_OnStop()
        {
            _inkCollector.IsPlaying = !_inkCollector.IsPlaying;
            if (_inkCollector.IsPlaying)
                strokePlay();
            this.timeBar.StrokePlayButton = StrokePlayButton.GoOnPlay;
            //Timer.Stop();
        }
        #endregion playback

        #region 缩放画板
        private double scaling;

        private void InkCanvasZoomButton_Click(object sender, System.Windows.RoutedEventArgs e)
        { 

            #region 詹启加的代码 
            if (0 == zoomMaxSign)
            {
                
                
                if (0 != _inkFrame.InkCollector.Sketch.Images.Count)
                {
                    _inkFrame.pointView.pointView.initialize_NodeConnectImage();
                    zoomMaxSign = 1;
                    _inkFrame.ZoomMaxSign = zoomMaxSign;
                }

            }
            #endregion
            double windowWidth = this.ActualWidth; //获取窗口宽度
            double windwoHeight = this.ActualHeight; //获取窗口高度
            _inkFrame.panelVideoShow.Width = windowWidth / 2;
            double inkCanvasZoomWidth = windowWidth - _inkFrame.panelVideoShow.Width;//计算画板缩小后的宽度
            scaling = (inkCanvasZoomWidth - 22) / _inkFrame._inkCanvas.ActualWidth;  //计算缩放比例

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //宽度缩小
            myDoubleAnimation.From = 1;
            myDoubleAnimation.To = scaling;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

            DoubleAnimation myDoubleAnimationHeight = new DoubleAnimation();
            //高度缩小
            myDoubleAnimationHeight.From = 1;
            myDoubleAnimationHeight.To = scaling;
            myDoubleAnimationHeight.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetName(myDoubleAnimationHeight, "InkCanvasScaleTrans");
            Storyboard.SetTargetProperty(myDoubleAnimationHeight, new PropertyPath(ScaleTransform.ScaleYProperty));


            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Children.Add(myDoubleAnimationHeight);
            myStoryboard.Completed += new EventHandler(myStoryboard_Completed);
            myStoryboard.Begin(_inkFrame._inkCanvas);

            _inkFrame.pointView.Width = windowWidth - 20;
            _inkFrame.pointView.Height = windwoHeight - _inkFrame._inkCanvas.ActualHeight * scaling;
            _inkFrame.pointView.Margin = new Thickness(4, _inkFrame._inkCanvas.ActualHeight * scaling - 4, 4, 4);
            _inkFrame.panelVideoShow.Height = _inkFrame._inkCanvas.ActualHeight * scaling;
            _inkFrame.pointView.pointView.inkPictureNode.Width = (int)_inkFrame.pointView.Width - 10;
            _inkFrame.pointView.pointView.inkPictureNode.Height = (int)_inkFrame.pointView.Height - 10;
			if (_inkCollector.SelectedImages.Count > 0)
			{
				foreach (MyImage myImage in _inkCollector.SelectedImages)
				{
					myImage.Bound.Visibility = Visibility.Collapsed;
				}
			}
			_inkCollector.SelectedImages.Clear();
            this._controlPanel.Visibility = Visibility.Collapsed;
            this._inkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
            this.InkCanvasZoomButton.Visibility = Visibility.Collapsed;
        }

        void myStoryboard_Completed(object sender, EventArgs e)
        {
            this._inkFrame.panelVideoShow.Visibility = Visibility.Visible;
            this._inkFrame.pointView.Visibility = Visibility.Visible;
            this.InkCanvasZoomBackButton.Visibility = Visibility.Visible;
        }

        private void InkCanvasZoomBackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //宽度
            myDoubleAnimation.From = scaling;
            myDoubleAnimation.To = 1;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetName(myDoubleAnimation, "InkCanvasScaleTrans");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

            DoubleAnimation myDoubleAnimationHeight = new DoubleAnimation();
            //宽度
            myDoubleAnimationHeight.From = scaling;
            myDoubleAnimationHeight.To = 1;
            myDoubleAnimationHeight.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetName(myDoubleAnimationHeight, "InkCanvasScaleTrans");
            Storyboard.SetTargetProperty(myDoubleAnimationHeight, new PropertyPath(ScaleTransform.ScaleYProperty));


            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Children.Add(myDoubleAnimationHeight);
            myStoryboard.Begin(_inkFrame._inkCanvas);
			if (_inkCollector.SelectedImages.Count > 0)
			{
				foreach (MyImage myImage in _inkCollector.SelectedImages)
				{
					myImage.Bound.Visibility = Visibility.Collapsed;
				}
			}
			_inkCollector.SelectedImages.Clear();
            this._inkFrame.panelVideoShow.Visibility = Visibility.Collapsed;
            this._inkFrame.pointView.Visibility = Visibility.Collapsed;
            this._controlPanel.Visibility = Visibility.Visible;
            this.InkCanvasZoomButton.Visibility = Visibility.Visible;
            this.InkCanvasZoomBackButton.Visibility = Visibility.Collapsed;
        }
        #endregion
        private void ShotCut_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            WPFInk.ShotCut.ShotCut fm = new WPFInk.ShotCut.ShotCut(_inkFrame);
            fm.Show();
        }
        /*
		//打开视频操作的界面
        private void VideoOperation_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.VideoOperation.Visibility = Visibility.Visible;
            this.timeBar.Visibility = Visibility.Collapsed;
            this._inkFrame.Visibility = Visibility.Collapsed;

        }
        #region AutoGenerationStoryBoard
        
        //自动生成storyboard
        public void AutoGenerationStoryBoard_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            AutoGenerationStoryBoard();
            VideoOperation._videoList.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 自动生成StoryBoard
        /// </summary>
		public void AutoGenerationStoryBoard()
		{
			double _thumbWidth = 120; //缩略图宽度
			double _thumbInterval = 80;//两个缩略图之间的间隙
			if (VideoOperation.KeyWordsImagesInkFrame._inkCanvas.Children.Count > 0 || VideoOperation.KeyWordsImagesInkFrame._inkCanvas.Strokes.Count > 0)
			{
				foreach (MyArrow ma in VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyArrows)
				{
					VideoOperation._thumbInk.Thumb_InkFrame._inkCanvas.Children.Remove(ma.Arrow);
					ma.Dispose();
				}
				VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyArrows.Clear();
				string keyWordsSentence = null;
				foreach (MyRichTextBox myRichTextBox in VideoOperation.KeyWordsImagesInkFrame.InkCollector.Sketch.MyRichTextBoxs)
				{
					TextRange textRange = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);
					keyWordsSentence += textRange.Text;
				}
				string strRecognitonResults = (new MyInkAnalyzer(VideoOperation.KeyWordsImagesInkFrame._inkCanvas.Strokes)).getAnalyzeResults();
                
                
				if (isUpdatedRecognitionResults == false)
				{
                    if (strRecognitonResults != null)
                    {
                        VideoOperation._updateRecogntionRusult.setMainWindow(this);
                        VideoOperation._updateRecogntionRusult.Visibility = Visibility.Visible;
                        VideoOperation._updateRecogntionRusult._textBox.Text = strRecognitonResults;
                        return;
                    }
				}
				else
				{
					strRecognitonResults = updatedRecognitionResults;
					isUpdatedRecognitionResults = false;
				}
				//MessageBox.Show(strRecognitonResults);
				if (strRecognitonResults != "Recognition Failed")
				{
					keyWordsSentence += strRecognitonResults;
				}
				foreach (MyImage myImage in VideoOperation.KeyWordsImagesInkFrame.InkCollector.Sketch.Images)
				{
					keyWordsSentence += myImage.SafeFileName;
				}
                //检测图形（流程图）
                foreach (MyGraphic mg in VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyGraphics)
                {

                }
				//MessageBox.Show(keyWordsSentence);
				VideoOperation.KeyWordsImagesGrid.Visibility = Visibility.Hidden;
				VideoOperation.TableGrid.Visibility = Visibility.Visible;
				List<string> strArray = autoStoryBoardByString(keyWordsSentence);
				List<MyButton> myButtonList = new List<MyButton>();
				for (int i = 0; i < strArray.Count; i++)
				{
					string CharString = strArray[i];

                    List<string> otherWordsList=new List<string>();
                    if (GlobalValues.InkAnalyzerLanguageId == 1033)
                    {
                        otherWordsList.Add("the");
                        otherWordsList.Add("am");
                        otherWordsList.Add("for");
                        otherWordsList.Add("to");
                        otherWordsList.Add("was");
                        otherWordsList.Add("in");
                        otherWordsList.Add("that");
                        otherWordsList.Add("a");
                        otherWordsList.Add("an");
                        otherWordsList.Add("were");
                        otherWordsList.Add("are");
                        otherWordsList.Add("");
                        otherWordsList.Add(" ");
                    }
                    else
                    {
                        otherWordsList.Add("的");
                        otherWordsList.Add("了");
                        otherWordsList.Add("吗");
                        otherWordsList.Add("在");
                        otherWordsList.Add("是");
                        otherWordsList.Add("");
                        otherWordsList.Add(" ");
                    }
                    if (otherWordsList.IndexOf(CharString) == -1)
					{
						foreach (MyButton myButton in VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
						{
                            //if (!myButton.IsGlobal)
                            //{
								bool findFlag = false;
								//第一步：检测文本
								foreach (MyRichTextBox myRichTextBox in myButton.InkFrame.InkCollector.Sketch.MyRichTextBoxs)
								{
									TextRange textRange = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);
                                    List<string> strArrayText = autoStoryBoardByString(textRange.Text);
                                    if (strArrayText.IndexOf(CharString) > -1 && myButtonList.IndexOf(myButton) == -1)
									{
										myButtonList.Add(myButton);
										//MessageBox.Show("MyRichTextBox:" + CharString);
										findFlag = true;
										break;
									}
								}
								if (findFlag)
								{
									break;
								}

								//第二步：检测笔迹识别结果
                                List<string> strArrayStroke = autoStoryBoardByString(myButton.AnalyzeResults);
								if (strArrayStroke.IndexOf(CharString) > -1 && myButtonList.IndexOf(myButton) == -1)
								{
									myButtonList.Add(myButton);
									//MessageBox.Show("AnalyzeResults:" + CharString);
									break;
								}


								//第三步：检测图片名称
								foreach (MyImage mi in myButton.InkFrame.InkCollector.Sketch.Images)
                                {
                                    List<string> strArrayImage = autoStoryBoardByString(mi.SafeFileName);
                                    if (strArrayImage.IndexOf(CharString) > -1 && myButtonList.IndexOf(myButton) == -1)
									{
										myButtonList.Add(myButton);
										//MessageBox.Show("MyImage:" + CharString);
										findFlag = true;
										break;
									}
								}
								if (findFlag)
								{
									break;
								}

								//第四步：检测视频名称,全局Mybutton较优先
                                //if (myButton.VideoFileName.IndexOf(CharString) > -1 && myButtonList.IndexOf(myButton) == -1 && myButton.IsGlobal)
                                //{
                                //    myButtonList.Add(myButton);
                                //    //MessageBox.Show("VideoFileName:" + CharString);
                                //    break;
                                //}
                            //}

						}

					}
				}
                if (VideoOperation.KeyWordsImagesInkFrame.InkCollector.Sketch.MyGraphics.Count > 0)
                {
                    List<int> ids = GraphicMathTool.getInstance().getGraphicStructure(VideoOperation.KeyWordsImagesInkFrame.InkCollector.Sketch.MyGraphics[0], VideoOperation.KeyWordsImagesInkFrame.InkCollector, new List<int>());
                    foreach (int id in ids)
                    {
                        myButtonList.Add(VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons[id - 1]);
                    }
                }
				//第一步：删除不需要的Mybutton
				List<MyButton> myButtonOtherList = new List<MyButton>();
				foreach (MyButton mb in VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
				{
					if (myButtonList.IndexOf(mb) == -1)
					{
						myButtonOtherList.Add(mb);
					}
				}
				//MessageBox.Show(Thumb_InkFrame._inkCanvas.Children.Count.ToString());
				foreach (MyButton mb in myButtonOtherList)
				{
					Command hmbc = new HiddenMyButtonCommand(VideoOperation._thumbInk.Thumb_InkFrame.InkCollector, mb);
					hmbc.execute();
					_inkCollector.CommandStack.Push(hmbc);
				}
				//MessageBox.Show(Thumb_InkFrame._inkCanvas.Children.Count.ToString());
				foreach (MyArrow ma in _inkCollector.Sketch.MyArrows)
				{
					DeleteArrowCommand dac = new DeleteArrowCommand(VideoOperation._thumbInk.Thumb_InkFrame.InkCollector, ma);
					dac.execute();
					_inkCollector.CommandStack.Push(dac);
				}


				//第二步：移动需要的Mybutton并添加连线
				int ThumbIndex = 0;
				foreach (MyButton mb in myButtonList)
				{
					double Left = _thumbInterval + (_thumbWidth + _thumbInterval) * (ThumbIndex % 6);
					double Top = _thumbInterval + (_thumbWidth * (mb.Height / mb.Width) + _thumbInterval) * (ThumbIndex / 6);
					ButtonMoveCommand bmc = new ButtonMoveCommand(mb, Left - mb.Left, Top - mb.Top,  VideoOperation._thumbInk.Thumb_InkFrame.InkCollector);
					bmc.execute();
					_inkCollector.CommandStack.Push(bmc);
					//添加连线
					if (ThumbIndex != myButtonList.Count - 1)
					{
						ThumbConnector thumbConnector = new ThumbConnector(myButtonList[ThumbIndex], myButtonList[ThumbIndex + 1]);
						MyArrow ma = new MyArrow(thumbConnector.arrow);
						ma.PreMyButton = myButtonList[ThumbIndex];
						ma.NextMyButton = myButtonList[ThumbIndex + 1];
						ma.StartPoint = thumbConnector.startPoint;
						ma.EndPoint = thumbConnector.endPoint;
						Command aac = new AddArrowCommand(VideoOperation._thumbInk.Thumb_InkFrame.InkCollector, ma);
						aac.execute();
						VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.CommandStack.Push(aac);
					}
					
					if (ThumbIndex == 0)
					{
						VideoOperation._videoAnnotation.OpenVideoBySource(new Uri(myButtonList[0].VideoPath));
						VideoOperation._videoAnnotation.setPositon((int)myButtonList[0].TimeStart);
						VideoOperation._videoAnnotation.StoryboardInList[0].Begin();
					}

					ThumbIndex++;
				}

			}

		}
        
		void listBoxItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			foreach (MyButton mb in VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons)
			{
				if (mb.IsGlobal && mb.VideoFileName == ((ListBoxItem)sender).Content.ToString())
				{
					//MessageBox.Show("name:" + mb.VideoFileName + ",path:" + mb.VideoPath);
					VideoOperation._videoAnnotation.OpenVideoBySource(new Uri(mb.VideoPath));
					double TimeStart = mb.TimeStart;
					double TimeEnd = mb.TimeEnd;
					VideoOperation._videoAnnotation.setPositon((int)TimeStart); //定位视频
					if (mb.IsGlobal == false)
					{
						//下面是在视频上显示草图
						VideoOperation._videoAnnotation.Annotation_InkFrame.InkCollector.InkCanvas.Children.Clear();
						VideoOperation._videoAnnotation.Annotation_InkFrame.InkCollector.InkCanvas.Strokes.Clear();
						VideoOperation._videoAnnotation.Annotation_InkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
						VideoOperation._videoAnnotation.Annotation_InkFrame.InkCollector.Sketch.Images.Clear();
						VideoOperation._videoAnnotation.Annotation_InkFrame.InkCollector.Sketch.MyStrokes.Clear();
						VideoOperation._videoAnnotation.Annotation_InkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
						VideoOperation._videoAnnotation.Annotation_InkFrame.InkCollector.Mode = InkMode.Ink;

						Stream streamSave = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Create);
						WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(mb.InkFrame.InkCollector, streamSave);
						Stream streamOpen = new FileStream(GlobalValues.FilesPath+@"\WPFInk\WPFInk\cache\a.xml", FileMode.Open);
						WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(VideoOperation._videoAnnotation.Annotation_InkFrame.InkCollector, streamOpen);

					}
					mb.InkFrame._inkCanvas.Background = Brushes.LemonChiffon;
				}
			}
		}


		//弹出绘制关键字和关键图片的界面
        private void DrawKeyWordsImages_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (VideoOperation._thumbInk.Thumb_InkFrame.InkCollector.Sketch.MyButtons.Count > 0)
			{
				VideoOperation.KeyWordsImagesGrid.Visibility = Visibility.Visible;
				VideoOperation.TableGrid.Visibility = Visibility.Hidden;
				VideoOperation._videoList.Visibility = Visibility.Hidden;
			}
			else
			{
				MessageBox.Show("You cannot draw keywords and keyimages. Because there is no video information!");
			}
        }

        /// <summary>
        /// 对字符串的格式进行处理
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private List<string> autoStoryBoardByString(String s)
		{
			string textRangeStr2 = s.Replace("\r\n", " ");
            string textRangeStr3 = textRangeStr2;
            String[] strArray;
            List<string> strList = new List<string>();
            if (GlobalValues.InkAnalyzerLanguageId == 1033)//如果是英文，
            {
                textRangeStr3 = textRangeStr2.ToLower();//全部转换成小写
                string textRangeStr4 = textRangeStr3.Replace(",", " ");//替换逗号
                string textRangeStr5 = textRangeStr4.Replace(".", " ");//替换句号
                string textRangeStr6 = textRangeStr5.Replace(":", " ");//替换分号
                string textRangeStr7 = textRangeStr6.Replace(";", " ");//替换冒号
                string textRangeStr8 = textRangeStr7.Replace("?", " ");//替换问号
                string textRangeStr9 = textRangeStr8.Replace("!", " "); //替换感叹号
                string textRangeStr10 = textRangeStr9.Replace("\"", " "); //替换双引号
                string textRangeStr11 = textRangeStr10.Replace("\'", " "); //替换单引号
                strArray = textRangeStr11.Split(' ');
                for (int i = 0; i < strArray.Length; i++)
                {
                    strList.Add(strArray[i]);
                }
                return strList;
            }
            else
            {
                string textRangeStr4 = textRangeStr3.Replace(",", " ");//替换逗号
                string textRangeStr5 = textRangeStr4.Replace("，", " ");//替换逗号
                string textRangeStr6 = textRangeStr5.Replace(".", " ");//替换句号
                string textRangeStr7 = textRangeStr6.Replace("。", " ");//替换句号
                string textRangeStr8 = textRangeStr7.Replace(":", " ");//替换冒号
                string textRangeStr9 = textRangeStr8.Replace("：", " ");//替换冒号
                string textRangeStr10 = textRangeStr9.Replace(";", " ");//替换分号
                string textRangeStr11 = textRangeStr10.Replace("；", " ");//替换分号
                string textRangeStr12 = textRangeStr11.Replace("?", " ");//替换问号
                string textRangeStr13 = textRangeStr12.Replace("？", " ");//替换问号
                string textRangeStr14 = textRangeStr13.Replace("!", " "); //替换感叹号
                string textRangeStr15 = textRangeStr14.Replace("！", " "); //替换感叹号
                string textRangeStr16 = textRangeStr15.Replace("“", " "); //替换双引号
                string textRangeStr17 = textRangeStr16.Replace("”", " "); //替换双引号
                string textRangeStr18 = textRangeStr17.Replace("’", " "); //替换单引号
                string textRangeStr19 = textRangeStr18.Replace("‘", " "); //替换单引号
                ChineseParse chineseParse = new ChineseParse();
                strArray = chineseParse.ParseChinese(textRangeStr19);
                for (int i = 0; i < strArray.Length; i++)
                {
                    strList.Add(strArray[i]);
                }
                return strList;
            }
        }
        #endregion
        */
        #region 选项
        /// <summary>
        /// //选项事件，用于修改全局变量设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Option_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalValues.MyGraphic_IsDirectionRecognize)
            {
                GlobalValues.MyGraphic_IsDirectionRecognize = false;
            }
            else
            {
                GlobalValues.MyGraphic_IsDirectionRecognize = true;

            }
        }
        #endregion
    }
}
