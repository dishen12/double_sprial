using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Ink;
using System.Windows.Forms;
using WPFInk.cmd;
using System.Windows.Input;
using System.Windows.Shapes;
using WPFInk.tool;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
//using LayoutAlgo;
using WPFInk.ink;
using WPFInk.video;
using System.Collections.ObjectModel;
using WPFInk.Global;

namespace WPFInk
{
    public partial class VideoThumbControlPanel : System.Windows.Controls.UserControl
	{
        //所控制的InkCollector
        private InkFrame _inkFrame;

        //移动
        private bool IsMoving = false;
        private Point CurrentPoint;
		private VideoList _videoList = null;
        public VideoOperation _videoOperation = null;

		public VideoThumbControlPanel()
		{
            this.InitializeComponent();
            //InitApp();
		}

        

        public void setInkFrame(InkFrame inkFrame)
        {
            this._inkFrame = inkFrame;
        }
		public void setVideoList(VideoList v)
		{
			this._videoList = v;
		}

        //返回关联的inkCollector
        public InkCollector InkCollector
        {
            get
            {
                return _inkFrame.InkCollector;
            }
        }

        /// <summary>
        /// 点击选择按钮，切换到选择模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            InkCollector.Mode = InkMode.Select;
        }
        /// <summary>
        /// 撤销操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_inkFrame.InkCollector.CommandStack.Count > 0)
            {
                Command cmd = _inkFrame.InkCollector.CommandStack.Pop();
                _inkFrame.InkCollector.UndoCommandStack.Push(cmd);
                cmd.undo();
                //System.Windows.MessageBox.Show(_inkFrame.InkCollector.CommandStack.Count.ToString());
            }
        }

        /// <summary>
        /// 重做操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_inkFrame.InkCollector.UndoCommandStack.Count > 0)
            {
                Command cmd = _inkFrame.InkCollector.UndoCommandStack.Pop();
                _inkFrame.InkCollector.CommandStack.Push(cmd);
                cmd.execute();
            }
        }

        //启用手势按钮
        private void Gesture_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InkCollector.Mode = InkMode.GestureOnly;
            if (_inkFrame.InkCollector.InkCanvas.IsGestureRecognizerAvailable)
            {
                _inkFrame.InkCollector.InkCanvas.Gesture += new InkCanvasGestureEventHandler(InkCanvas_Gesture);
                _inkFrame.InkCollector.InkCanvas.SetEnabledGestures(
                    new ApplicationGesture[] { ApplicationGesture.ScratchOut ,              //擦除      
                        ApplicationGesture.Down
                    });
            }
        }
        int downcount = 0;
        public void InkCanvas_Gesture(object sender, InkCanvasGestureEventArgs e)
        {
            ReadOnlyCollection<GestureRecognitionResult> gestureResults =
            e.GetGestureRecognitionResults();
            Rect bound = e.Strokes.GetBounds();
            System.Drawing.Rectangle rectangleBound = ConvertClass.getInstance().RectToRectangle(bound);
            if (gestureResults[0].RecognitionConfidence == RecognitionConfidence.Strong)
            {
                switch (gestureResults[0].ApplicationGesture)
                {
                    case ApplicationGesture.ScratchOut:
                        
                        foreach (MyButton myButton in _inkFrame.InkCollector.Sketch.MyButtons)
                        {
                            Rect rectMyButton=new Rect(new Point(myButton.Left,myButton.Top),new Point(myButton.Left+myButton.Width,myButton.Top+myButton.Height));
                            if (MathTool.getInstance().isHitRects(e.Strokes.GetBounds(), rectMyButton) == true)
                            {
                                DeleteButtonCommand dbc = new DeleteButtonCommand(_inkFrame.InkCollector, myButton,_videoList);
                                dbc.execute();
                                InkCollector.CommandStack.Push(dbc);
                            }
                        }
                        foreach (MyArrow myArrow in _inkFrame.InkCollector.Sketch.MyArrows)
                        {
                            Rect rectMyButton = new Rect(myArrow.StartPoint,myArrow.EndPoint);
                            if (MathTool.getInstance().isHitRects(e.Strokes.GetBounds(), rectMyButton) == true)
                            {
                                Command dac = new DeleteArrowCommand(_inkFrame.InkCollector, myArrow);
                                dac.execute();
                                InkCollector.CommandStack.Push(dac);
                            }
                        }
                        break;
                    case ApplicationGesture.Down:
                        if (downcount == 1)
                        {
                            getKeyFramesInGivenVideoClip(_inkFrame.InkCollector.Sketch.MyButtons[0].VideoPath.Substring(8),
                                (int)_inkFrame.InkCollector.Sketch.MyButtons[0].TimeEnd,
                               (int)_inkFrame.InkCollector.Sketch.MyButtons[1].TimeStart);
                        }
                        else if (downcount == 0)
                        {
                            getKeyFramesInGivenVideoClip(_inkFrame.InkCollector.Sketch.MyButtons[0].VideoPath.Substring(8),
                                (int)_inkFrame.InkCollector.Sketch.MyButtons[5].TimeEnd,
                               (int)_inkFrame.InkCollector.Sketch.MyButtons[6].TimeStart);
                        }
                        else if (downcount == 2)
                        {
                            getKeyFramesInGivenVideoClip(_inkFrame.InkCollector.Sketch.MyButtons[0].VideoPath.Substring(8),
                                (int)_inkFrame.InkCollector.Sketch.MyButtons[6].TimeEnd,
                               (int)_inkFrame.InkCollector.Sketch.MyButtons[7].TimeStart);
                            InkCollector.Mode = InkMode.VideoPlay;
                        }
                        //else if (downcount == 3)
                        //{
                        //    getKeyFramesInGivenVideoClip(_inkFrame.InkCollector.Sketch.MyButtons[0].VideoPath.Substring(8),
                        //        (int)_inkFrame.InkCollector.Sketch.MyButtons[8].TimeEnd,
                        //       (int)_inkFrame.InkCollector.Sketch.MyButtons[9].TimeStart);
                        //}
                        downcount++;
                        //getKeyFramesInGivenVideoClip(GlobalValues.FilesPath + "/麋鹿王.avi", 0,
                        //       1069000);
                        break;
                }

            }
        }

        

        
        //控制面板拖动
        private void HeadArea_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.HeadArea.CaptureMouse();
            IsMoving = true;
            CurrentPoint = e.GetPosition((UIElement)this.Parent);     
        }

        private void HeadArea_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsMoving)
            {
                IsMoving = false;
                this.HeadArea.ReleaseMouseCapture();
            }

        }

        private void HeadArea_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsMoving)
            {
                Point p = e.GetPosition((UIElement)this.Parent);
                Thickness currentmargin = this.Margin;
                Point offset = new Point(p.X - CurrentPoint.X, p.Y - CurrentPoint.Y);
                this.Margin = new Thickness(currentmargin.Left + offset.X, currentmargin.Top + offset.Y, currentmargin.Right - offset.X, currentmargin.Bottom - offset.Y);
                CurrentPoint = p;
            }

        }

        //打开xml文件
        private void Open_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
                //打开文件
                OpenFileDialog openfile = new OpenFileDialog()
                {
                    Filter = "Xml Files (*.xml)|*.xml|All Files(*.*)|*.*",
                    Multiselect = false
                };

                if (openfile.ShowDialog() == DialogResult.OK)
                {
					_inkFrame.InkCollector.InkCanvas.Children.Clear();
					_inkFrame.InkCollector.InkCanvas.Strokes.Clear();
					_inkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
					_inkFrame.InkCollector.Sketch.Images.Clear();
					_inkFrame.InkCollector.Sketch.MyStrokes.Clear();
					_inkFrame.InkCollector.Sketch.MyRichTextBoxs.Clear();
					_inkFrame.InkCollector.Sketch.MyButtons.Clear();
                    MathTool.getInstance().ClearAllStrokesAndChildren(_titleInk.Title_InkFrame);
                    using (Stream stream = openfile.OpenFile())
                    {
                        WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(_inkFrame.InkCollector, stream);
                        stream.Close();
                    }
                }
            
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "xml",
                Filter = "Xml Files (*.xml)|*.xml|Jpeg Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|" +
                         "Bitmap files (*.bmp)|*.bmp|All　files　(*.*)|*.*",
                FilterIndex = 1,
                FileName = "未命名"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    if (sfd.FilterIndex == 1)
                    {
                        WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(this._inkFrame.InkCollector, stream);
                    }
                    else
                    {
                        WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveInkToImage(_inkFrame.InkCollector, stream);
                    }
                    stream.Close();
                }
            }

        }

        

        //最小化控制面板
        private void MinButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Height = 20;
            this.MinButton.Visibility = Visibility.Collapsed;
            this.MaxButton.Visibility = Visibility.Visible;
        }

        private void MaxButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Height = 262;
            this.MinButton.Visibility = Visibility.Visible;
            this.MaxButton.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 添加箭头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InkCollector.Mode = InkMode.DrawArrow;
        }
        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArrowButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //保存Title inkFrame
            //_videoAnnotation.SaveTitleInkFrame();

            InkCollector.Mode = InkMode.VideoPlay;
            
        }

        private void MoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InkCollector.Mode = InkMode.AutoMove;
        }
		public TitleInk _titleInk = null;
		public VideoAnnotation _videoAnnotation = null;
        private void ClearAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			if (this._inkFrame._inkCanvas.Children.Count > 0 || this._inkFrame._inkCanvas.Strokes.Count > 0)
			{

				DialogResult MsgBoxResult;//设置对话框的返回值
				MsgBoxResult = System.Windows.Forms.MessageBox.Show("删除以后将不能恢复，是否删除", "提示", MessageBoxButtons.YesNo);
				if (MsgBoxResult == DialogResult.Yes)//如果对话框的返回值是YES（按"Y"按钮）
				{
                    MathTool.getInstance().ClearAllStrokesAndChildren(_inkFrame);
                    MathTool.getInstance().ClearAllStrokesAndChildren(_titleInk.Title_InkFrame);
                    MathTool.getInstance().ClearAllStrokesAndChildren(_videoAnnotation.Annotation_InkFrame);
					_videoList.Visibility = Visibility.Collapsed;
					_videoList.VideoList_ListBox.Items.Clear();
					foreach (MyVideo mv in _videoAnnotation._myVideoList)
					{
						mv.Dispose();
					}
					_videoAnnotation._myVideoList.Clear();
				}
			}
        }


        private void AscByVideoPathButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ascSort(new ComparerVideoPath());
        }

        private void ascSort(System.Collections.Generic.IComparer<MyButton> comp)
        {
            List<MyButton> myButtonListNoArrow = new List<MyButton>();
            List<MyButton> myButtonListHaveArrow = new List<MyButton>();
            double _thumbInterval = _videoAnnotation.ThumbInterval;
            double _thumbWidth = _videoAnnotation.ThumbWidth;
            foreach (MyButton mb in _inkFrame.InkCollector.Sketch.MyButtons)
            {
                foreach (MyArrow ma in _inkFrame.InkCollector.Sketch.MyArrows)
                {
                    if (ma.IsDeleted == false && (ma.PreMyButton == mb || ma.NextMyButton == mb) && mb.IsDeleted == false)
                    {
                        myButtonListHaveArrow.Add(mb);
                    }
                }
            }
            double maxTop = 0;
            double maxHeight = 0;
            if (myButtonListHaveArrow.Count > 0)
            {
                maxTop = myButtonListHaveArrow[0].Top;
                maxHeight = myButtonListHaveArrow[0].Height;
            }
            foreach (MyButton mb in myButtonListHaveArrow)
            {
                maxTop = Math.Max(maxTop, mb.Top);
                maxHeight = Math.Max(maxHeight, mb.Height);
            }
            foreach (MyButton mb in _inkFrame.InkCollector.Sketch.MyButtons)
            {
                if (mb.IsDeleted == false && mb.Button.Visibility == Visibility.Collapsed)
                {
                    Command vmbc = new VisibleMyButtonCommand(_inkFrame.InkCollector, mb);
                    vmbc.execute();
                }
                if (myButtonListHaveArrow.IndexOf(mb) == -1 && mb.IsDeleted == false)
                {
                    myButtonListNoArrow.Add(mb);
                }
            }
            myButtonListNoArrow.Sort(comp);
            int ThumbIndex = 0;
            double _thumbVerticalInterval = _videoAnnotation.ThumbVerticalInterval;
            //foreach (MyButton mb in myButtonListNoArrow)
            int _thumbCountPerLine = _videoAnnotation.ThumbCountPerLine;
            for (int i = 0; i < myButtonListNoArrow.Count; i++)
            {
                double Left = _thumbInterval + (_thumbWidth + _thumbInterval) * (i % _thumbCountPerLine);
                double Top;
                if (i < _thumbCountPerLine)
                {
                    Top = _thumbVerticalInterval;
                }
                else
                {
                    if (i % _thumbCountPerLine == 0)
                    {
                        double maxTopButtonListNoArrow = myButtonListNoArrow[i - _thumbCountPerLine].Top + myButtonListNoArrow[i - _thumbCountPerLine].Height;
                        for (int j = i - 5; j < i; j++)
                        {
                            double newTop = myButtonListNoArrow[j].Top + myButtonListNoArrow[j].Height;
                            if (newTop > maxTopButtonListNoArrow)
                            {
                                maxTopButtonListNoArrow = newTop;
                            }
                        }
                        Top = _thumbVerticalInterval + maxTopButtonListNoArrow;
                    }
                    else
                    {
                        Top = myButtonListNoArrow[(i / _thumbCountPerLine) * _thumbCountPerLine].Top;
                    }
                }
                //double Top = _thumbInterval + maxTop + maxHeight + (_thumbWidth * (mb.Height / mb.Width) + _thumbInterval) * (ThumbIndex / 6);
                ButtonMoveCommand bmc = new ButtonMoveCommand(myButtonListNoArrow[i], Left - myButtonListNoArrow[i].Left, Top - myButtonListNoArrow[i].Top, _inkFrame.InkCollector);
                bmc.execute();
                _inkFrame.InkCollector.CommandStack.Push(bmc);
                ThumbIndex++;

            }
        }

        private void DescByVideoPathButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			List<MyButton> myButtonListNoArrow = new List<MyButton>();
			List<MyButton> myButtonListHaveArrow = new List<MyButton>();
			double _thumbInterval = 80;
			double _thumbWidth = 120;
			foreach (MyButton mb in _inkFrame.InkCollector.Sketch.MyButtons)
			{
				foreach (MyArrow ma in _inkFrame.InkCollector.Sketch.MyArrows)
				{
					if (ma.IsDeleted == false && (ma.PreMyButton == mb || ma.NextMyButton == mb) && mb.IsDeleted == false)
					{
						myButtonListHaveArrow.Add(mb);
					}
				}
			}
			double maxTop = 0;
			double maxHeight = 0;
			if (myButtonListHaveArrow.Count > 0)
			{
				maxTop = myButtonListHaveArrow[0].Top;
				maxHeight = myButtonListHaveArrow[0].Height;
			}
			foreach (MyButton mb in myButtonListHaveArrow)
			{
				maxTop = Math.Max(maxTop, mb.Top);
				maxHeight = Math.Max(maxHeight, mb.Height);
			}
			foreach (MyButton mb in _inkFrame.InkCollector.Sketch.MyButtons)
			{
				if (mb.IsDeleted == false && mb.Button.Visibility == Visibility.Collapsed)
				{
					Command vmbc = new VisibleMyButtonCommand(_inkFrame.InkCollector, mb);
					vmbc.execute();
				}
				if (myButtonListHaveArrow.IndexOf(mb) == -1 && mb.IsDeleted == false)
				{
					myButtonListNoArrow.Add(mb);
				}
			}
			myButtonListNoArrow.Sort(new ComparerVideoPath());
			for (int k = myButtonListNoArrow.Count - 1; k >= 0; k--)
			{
				double Left = _thumbInterval + (_thumbWidth + _thumbInterval) * ((myButtonListNoArrow.Count-1-k) % 6);
				int i = myButtonListNoArrow.Count-k-1;
				//double Top = _thumbInterval + maxTop + maxHeight + (_thumbWidth * (myButtonListNoArrow[i].Height / myButtonListNoArrow[i].Width) + _thumbInterval) * ((myButtonListNoArrow.Count-1 - i) / 6);
				double Top;
				if (i < 6)
				{
					Top = _thumbInterval + maxTop + maxHeight;
				}
				else
				{
					if (i % 6 == 0)
					{
						double maxTopButtonListNoArrow = myButtonListNoArrow[myButtonListNoArrow.Count - i].Top + myButtonListNoArrow[myButtonListNoArrow.Count - i].Height;
						for (int j = myButtonListNoArrow.Count - i + 1; j < myButtonListNoArrow.Count - i+5; j++)
						{
							double newTop = myButtonListNoArrow[j].Top + myButtonListNoArrow[j].Height;
							if (newTop > maxTopButtonListNoArrow)
							{
								maxTopButtonListNoArrow = newTop;
							}
						}
						Top = _thumbInterval + maxTopButtonListNoArrow;
					}
					else
					{
						Top = myButtonListNoArrow[myButtonListNoArrow.Count-1-(i / 6) * 6].Top;
					}
				}
				ButtonMoveCommand bmc = new ButtonMoveCommand(myButtonListNoArrow[k], Left - myButtonListNoArrow[k].Left, Top - myButtonListNoArrow[k].Top, _inkFrame.InkCollector);
				bmc.execute();
				_inkFrame.InkCollector.CommandStack.Push(bmc);
			}
        }
        int hiddenClickCount = 0;
        private void Hidden_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (hiddenClickCount % 2 == 0)
            {
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
           // _videoOperation.RadioButtonGrid.Visibility = Visibility.Visible;
        }
        private void getKeyFramesInGivenVideoClip(string videoPathName,int startPosition,int endPosition)
        {
            /*
            ////实时提取关键帧，此段要永久保留
            //AVIReader reader = new AVIReader();
            //reader.Open(videoPathName);
            //int videoFramePerSecond = 24;
            //int position = (int)(((double)startPosition /1000)* videoFramePerSecond);
            //reader.Position = position;
            //System.Drawing.Bitmap curFrame = null;
            //System.Drawing.Bitmap nextFrame = null;
            //int frameCount = 0;
            //int[] curHist = null;
            //int[] lastHist = null;
            //List<Frame> keyFrameList = new List<Frame>();
            //List<int> keyFrameNoList = new List<int>();
            //int curpos = 0;
            //endPosition = (int)(((double)endPosition / 1000) * videoFramePerSecond);
            //System.Drawing.Bitmap resizedCurFrame;
            //double diff;
            //while (position < endPosition)
            //{
            //    curFrame = reader.GetNextFrame();
            //    nextFrame = reader.GetNextFrame();
            //    frameCount++;
            //    curpos = reader.CurrentPosition;
            //    if (null != curFrame)
            //    {
            //        // 取thumbnail去生成hist，但keyFrame里存的还是原来的bmp
            //        resizedCurFrame = new System.Drawing.Bitmap(curFrame.GetThumbnailImage(200, 200, null, IntPtr.Zero));
            //        curHist = WPFInk.ShotCut.Histogram.CalHSVHis(resizedCurFrame);
            //        if (null != lastHist)
            //        {
            //            diff = WPFInk.ShotCut.Histogram.CalHisDiff(curHist, lastHist);

            //            // 这里暂时取像素值的四分之一作为阈值
            //            if (diff > resizedCurFrame.Width * resizedCurFrame.Height * 0.25)
            //            {
            //                keyFrameList.Add(new Frame(curFrame, nextFrame, (int)((double)curpos - 1) * 1000 / videoFramePerSecond));
            //                keyFrameNoList.Add((int)(reader.CurrentPosition));
            //            }
            //            else if (curpos == 128 || curpos == 432 || curpos == 604 || curpos == 680 || curpos == 1216 || curpos == reader.Length - 1)
            //            {
            //                keyFrameList.Add(new Frame(curFrame, nextFrame, (int)((double)curpos - 1) * 1000 / videoFramePerSecond));
                            
            //                keyFrameNoList.Add((int)(reader.CurrentPosition));                            
            //            }                           
            //        }
            //    }
            //    lastHist = curHist;
            //    position++;
            //    resizedCurFrame = null;
            //    curFrame = null;
            //    nextFrame = null;

            //}
            ////过滤关键帧
            //int filterNo = 5;
            //while (keyFrameNoList.Count > filterNo)
            //{
            //    int min = int.MaxValue;
            //    int minindex = -1;
            //    for (int i = 1; i < keyFrameNoList.Count; i++)
            //    {
            //        if (keyFrameNoList[i] - keyFrameNoList[i - 1] < min &&
            //            !(i == 128 || i == 432 || i == 604 || i == 680 || i == 1216 || i == reader.Length - 1))
            //        {
            //            min = keyFrameNoList[i] - keyFrameNoList[i - 1];
            //            minindex = i - 1;
            //        }
            //    }
            //    if (minindex == -1)
            //        return;
            //    keyFrameNoList.RemoveAt(minindex);
            //    keyFrameList.RemoveAt(minindex);
            //    //keyFrameRecList.RemoveAt(minindex);
            //}
            ////保存关键帧
            //int count = 0;
            //foreach (Frame frame in keyFrameList)
            //{
            //    frame.frameBmp.Save(GlobalValues.FilesPath + @"\WPFInk\WPFInk\resource\CachekeyFrames\" + (++count) + ".png");
            //}
             * */
           string path = @"resource\"+GlobalValues.videoName+@"\time.txt";
          // string path = @"resource\" + "大雄兔" + @"\time.txt";
            int minIndex = 0;
            int maxIndex=0;
            bool isFindMin=false;
            List<int> videoTimeList = new List<int>();
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string s = "";
                    int index = 1;
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] line = s.Split(' ');
                        if(!isFindMin)
                        {
                            if (Int32.Parse(line[1]) > startPosition && Int32.Parse(line[1]) < endPosition)
                            {
                                minIndex=index;
                                videoTimeList.Add(Int32.Parse(line[1]));
                                isFindMin=true;
                            }
                        }
                        else
                        {
                            if (Int32.Parse(line[1]) > startPosition && Int32.Parse(line[1]) < endPosition)
                            {
                                videoTimeList.Add(Int32.Parse(line[1]));
                            }
                        }
                        if (Int32.Parse(line[1]) >= endPosition)
                        {
                            maxIndex=index;
                            break;
                        }
                        index++;
                    }
                }
            }
            if (minIndex > 0 && maxIndex > minIndex)
            {
                for (int i = minIndex; i < maxIndex; i++)
                {
                    _videoAnnotation.addKeyFrameButton(InkConstants.getImageFromName(@"resource\" + GlobalValues.videoName + @"_source\" + i + ".png"),
                   // _videoAnnotation.addKeyFrameButton(InkConstants.getImageFromName(@"resource\" + "大雄兔" + @"_source\" + i + ".png"),
                        videoTimeList[i - minIndex]);
                }
            }
            ascSort(new ComparerTimeStart());
        }
        private void getKeyFramesInGivenVideoClipOlympic(string videoPathName, int startPosition, int endPosition)
        {
            string path = GlobalValues.FilesPath + @"\task\WPFInk材料\程序截图\加菲猫草图视频摘要\VideoTime.txt";
            int minIndex = 1;
            int maxIndex = 8;
            List<int> videoTimeList = new List<int>();
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string s = "";
                    //int index = 1;
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] line = s.Split(' ');
                        videoTimeList.Add(Int32.Parse(line[1]));
                    }
                }
            }

            for (int i = minIndex; i < maxIndex; i++)
            {
                //_videoAnnotation.addKeyFrameButton(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\task\WPFInk材料\程序截图\加菲猫草图视频摘要\" + i + ".png"),
                //    videoTimeList[i - 1]);
                _videoAnnotation.addKeyFrameButton(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\task\WPFInk材料\程序截图\加菲猫草图视频摘要\" + i + ".png"),
                    videoTimeList[i - 1]);
            }
        }

	}
}