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
using WPFInk.tool;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
//using LayoutAlgo;
using WPFInk.ink;
using System.Collections.ObjectModel;
using System.Xml;
using WPFInk.graphic;
using WPFInk.Global;
using WPFInk.videoSummarization;

namespace WPFInk
{
	public partial class ControlPanel
    {
        #region 私有常量
        //所控制的InkCollector
        private InkFrame _inkFrame;
        //移动
        private bool IsMoving = false;
        private Point CurrentPoint;
        #endregion

        #region 常量
        private const double ZoomStroke=5;
        #endregion

        #region 构造函数
        public ControlPanel()
		{
            this.InitializeComponent();
            InitApp();
		}

        //初始化InkFrame
        private void InitApp()
        {

            this.OpacitySlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<Double>(OpacitySlider_ValueChanged);
			this.Height =GlobalValues.ControlPanel_OtherModeHeight;            
        }

        public void setInkFrame(InkFrame inkFrame)
        {
            this._inkFrame = inkFrame;
        }
        #endregion

        //返回关联的inkCollector
        public InkCollector InkCollector
        {
            get
            {
                return _inkFrame.InkCollector;
            }
        }

        #region 事件
        /// <summary>
        /// 点击铅笔按钮响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pencil_Click(object sender, RoutedEventArgs e)
        {
            InkCollector.Mode = InkMode.Ink;
			_fontChooser.Visibility = Visibility.Hidden;
			RectangleSlider.Visibility = Visibility.Visible;
			WidthSlider.Visibility = Visibility.Visible;
			OpacitySlider.Visibility = Visibility.Visible;
            this.Height = GlobalValues.ControlPanel_InkModeHeight;
        }

        /// <summary>
        /// 点击点橡皮按钮响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointEraserButton_Click(object sender, RoutedEventArgs e)
        {
			InkCollector.Mode = InkMode.PointErase;
            this.Height = GlobalValues.ControlPanel_OtherModeHeight;
        }

        /// <summary>
        /// 点击大橡皮按钮响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RubberButton_Click(object sender, RoutedEventArgs e)
        {
			InkCollector.Mode = InkMode.StrokeErase;
            this.Height = GlobalValues.ControlPanel_OtherModeHeight;
        }

        /// <summary>
        /// 点击选择按钮，切换到选择模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
			InkCollector.Mode = InkMode.Select;
            this.Height = GlobalValues.ControlPanel_OtherModeHeight;
        }

        //笔迹宽度设置
        private void Slider_ValueChanged(object sender, RoutedEventArgs e)
        {
            int width = (int) this.WidthSlider.Value;
            if (width < DrawingAttributes.MinWidth)
                width = (int)DrawingAttributes.MinWidth+1;
            this.WidthInt.Text = width.ToString();
            InkCollector.DefaultDrawingAttributes.Width = width;
            InkCollector.DefaultDrawingAttributes.Height = width;
        }

        //插入文本
        private void insertText_Click(object sender, RoutedEventArgs e)
		{
			_fontChooser.setInkFrame(_inkFrame);
			InkCollector.Mode = InkMode.InsertText;
			_fontChooser.Visibility = Visibility.Visible;
			RectangleSlider.Visibility = Visibility.Collapsed;
			WidthSlider.Visibility = Visibility.Collapsed;
			OpacitySlider.Visibility = Visibility.Collapsed;
            _fontChooser.FontFamilyCmb.SelectedIndex = 6;
			_fontChooser.FontColor.SelectedIndex = 7;
            _fontChooser.FontSizeCmb.SelectedIndex = 25;
            this.Height = GlobalValues.ControlPanel_InsertTextHeight;
        }

        //插入图片
        private void insertImage_Click(object sender, RoutedEventArgs e)
        {
            _inkFrame.InkCollector.InkCanvas.EditingMode = InkCanvasEditingMode.None;
            //打开图片选择框
            OpenFileDialog openfile = new OpenFileDialog()
            {
                Filter = "Jpeg Files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp|All Files(*.*)|*.*",
                Multiselect = true
            };

            if (openfile.ShowDialog() == DialogResult.OK)
            {
                string FileName = openfile.FileName;
				string SafeFileName = openfile.SafeFileName;
                MyImage newimage = new MyImage(FileName);
				newimage.SafeFileName = SafeFileName;
                InkConstants.AddBound(newimage);
                AddImageCommand cmd = new AddImageCommand(_inkFrame.InkCollector, newimage);
                cmd.execute();
                _inkFrame.InkCollector.CommandStack.Push(cmd);
                _inkFrame.pointView.pointView.AddNode(_inkFrame.pointView.pointView.nodeList, _inkFrame.pointView.pointView.links);

			}
            this.Height = GlobalValues.ControlPanel_OtherModeHeight;
            InkCollector.Mode = InkMode.AutoMove;
           
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
                //Console.WriteLine(_inkFrame.InkCollector.CommandStack.Count);
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
            //Console.WriteLine(InkCollector.SelectedStrokes.Count);
            if (_inkFrame.InkCollector.InkCanvas.IsGestureRecognizerAvailable)
            {
                _inkFrame.InkCollector.InkCanvas.Gesture += new InkCanvasGestureEventHandler(InkCanvas_Gesture);
                _inkFrame.InkCollector.InkCanvas.SetEnabledGestures(
                    new ApplicationGesture[] { ApplicationGesture.ScratchOut,               //擦除笔迹
                                               ApplicationGesture.Check,                     //对勾
                                               ApplicationGesture.Curlicue,                   //cut
                                               ApplicationGesture.ChevronLeft,                       //insert插入行
											   ApplicationGesture.Up,                       //插入字
											   ApplicationGesture.Down,                 //插入字
                                               ApplicationGesture.Left,               //撤销                           
                                               ApplicationGesture.Right,               //反撤销
											   ApplicationGesture.ArrowRight               //向右箭头



                    
                    });
			}
            this.Height = GlobalValues.ControlPanel_OtherModeHeight;
        }
        int CheckGestureCount = 0;
        //int ArrowRightGestureCount = 0;
        //WPFInk.videoSummarization.MySpiral mySpiral;
        //WPFInk.videoSummarization.SpiralSummarization spiralSummarization;

        void InkCanvas_Gesture(object sender, InkCanvasGestureEventArgs e)
        {
            ReadOnlyCollection<GestureRecognitionResult> gestureResults =
            e.GetGestureRecognitionResults();

            if (gestureResults[0].RecognitionConfidence == RecognitionConfidence.Strong)
            {
                switch (gestureResults[0].ApplicationGesture)
                {
                    case ApplicationGesture.ScratchOut:
                        InkCollector.removeHitStrokes(e.Strokes);//删除笔迹                    
                        InkCollector.removeHitImages(e.Strokes);//删除图片                    
                        InkCollector.removeHitTextBoxes(e.Strokes);//删除文本
                        InkCollector.removeHitMyGraphics(e.Strokes);//删除图形	
                        _inkFrame.OperatePieMenu.Visibility = Visibility.Collapsed;
                        //Console.WriteLine("擦除");
                        break;
                    case ApplicationGesture.Left:
						if (_inkFrame.InkCollector.CommandStack.Count > 0)
						{
							Command cmd = _inkFrame.InkCollector.CommandStack.Pop();
							_inkFrame.InkCollector.UndoCommandStack.Push(cmd);
							cmd.undo();
							//Console.WriteLine(_inkFrame.InkCollector.CommandStack.Count);
						}
                        break;
                    case ApplicationGesture.Right:
                        if (_inkFrame.InkCollector.UndoCommandStack.Count > 0)
                        {
                            Command cmd = _inkFrame.InkCollector.UndoCommandStack.Pop();
                            _inkFrame.InkCollector.CommandStack.Push(cmd);
                            cmd.execute();
                        }
                        //Console.WriteLine("反撤销");
                        break;
                    case ApplicationGesture.Check:
                        foreach (MyStroke myStroke in InkCollector.SelectedStrokes)
                        {
							DeleteStrokeCommand dsc = new DeleteStrokeCommand(_inkFrame.InkCollector, myStroke);
                            dsc.execute();
                        }
                        InkCollector.SelectedStrokes.Clear();
                        if (CheckGestureCount++ < 3)
                        {
                            _inkFrame._imageSelector.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            _inkFrame._peopleImageSelector.Visibility = Visibility.Visible;
                        }
                        //Console.WriteLine("对勾");
                        break;
                    case ApplicationGesture.Curlicue:
                        //Console.WriteLine("清除文字");
                        break;
                    case ApplicationGesture.ChevronLeft:
                        //System.Windows.MessageBox.Show("插入行");
						Rect ChevronLeftRect=e.Strokes.GetBounds();
						Point keyPoint = new Point(ChevronLeftRect.TopLeft.X, ChevronLeftRect.TopLeft.Y + ChevronLeftRect.Height / 2);
						Rect LeftDownRect = new Rect(new Point(0, keyPoint.Y), new Point(ChevronLeftRect.BottomRight.X, _inkFrame._inkCanvas.ActualHeight));
						foreach (MyStroke ms in _inkFrame.InkCollector.Sketch.MyStrokes)
						{
							if (MathTool.getInstance().isHitRects(LeftDownRect, ms.Stroke.GetBounds()) == true)
							{
								MoveCommand mc = new MoveCommand(ms, 0, 100);
								mc.execute();
								_inkFrame.InkCollector.CommandStack.Push(mc);
							}
						}
						
						break;
					case ApplicationGesture.Up:
						Rect UpRect = e.Strokes.GetBounds();
						Rect RightRectUp = new Rect(new Point(UpRect.TopLeft.X, UpRect.TopLeft.Y), new Point(_inkFrame._inkCanvas.ActualWidth, UpRect.BottomLeft.Y));
						
						foreach (MyStroke ms in _inkFrame.InkCollector.Sketch.MyStrokes)
						{
							if (MathTool.getInstance().isHitRects(RightRectUp, ms.Stroke.GetBounds()) == true)
							{
								MoveCommand mcs = new MoveCommand(ms, 50, 0);
								mcs.execute();
								_inkFrame.InkCollector.CommandStack.Push(mcs);
							}
						}
						break;
					case ApplicationGesture.Down:
						Rect DownRect = e.Strokes.GetBounds();
						Rect RightRectDown = new Rect(new Point(DownRect.TopLeft.X, DownRect.TopLeft.Y), new Point(_inkFrame._inkCanvas.ActualWidth, DownRect.BottomLeft.Y));
						
						foreach (MyStroke ms in _inkFrame.InkCollector.Sketch.MyStrokes)
						{
							if (MathTool.getInstance().isHitRects(RightRectDown, ms.Stroke.GetBounds()) == true)
							{
								MoveCommand mcs = new MoveCommand(ms, 50, 0);
								mcs.execute();
								_inkFrame.InkCollector.CommandStack.Push(mcs);
							}
						}
						break;
					case ApplicationGesture.ArrowRight://向右箭头
                        
                        //_inkFrame._inkCanvas.Children.Clear();
                        ///*测试螺旋式摘要*/
                        //if (ArrowRightGestureCount == 0)
                        //{
                        //    //mySpiral = new MySpiral(70, new StylusPoint(400, 400), 5, 2, _inkFrame._inkCanvas);
                        //    mySpiral = new MySpiral(80, new StylusPoint(_inkFrame._inkCanvas.ActualWidth / 2, _inkFrame._inkCanvas.ActualHeight / 2), 5, 2, _inkFrame._inkCanvas);
                        //}
                       
                        //spiralSummarization = new SpiralSummarization(@"E:\task\idea\螺旋式摘要\麋鹿王1_clip.avi", mySpiral, keyFrames);
                        
                        //InkCollector.SpiralSummarization = spiralSummarization;
                        //InkCollector.Mode = InkMode.SpiralSummarization;
                        //ArrowRightGestureCount++;
						break;
                }

            }
        }

        //List<KeyFrame> keyFrames = new List<KeyFrame>();
        //private void addImages()
        //{
        //    List<int> videoTimeList = new List<int>();
        //    string path = GlobalValues.FilesPath + @"\WPFInk\WPFInk\src\videoSummarization\VideoTime.txt" ;
        //    if (File.Exists(path))
        //    {
        //        using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
        //        {
        //            string s = "";
        //           // int i = 0;
        //            while ((s = sr.ReadLine()) != null)
        //            {
        //                //i++;
        //                //if (i % 2 == 1)
        //                //{
        //                    string[] line = s.Split(' ');
        //                    videoTimeList.Add(Int32.Parse(line[1]));
        //                //}
                        
        //            }
        //        }
        //    }  
        //    for (int i = 1; i <= 76; i++)
        //    {
        //        //BlackAndWhite,mlw_clip
        //        //KeyFrame keyFrame = new KeyFrame("E:/task/idea/螺旋式摘要/BlackAndWhite/mlw (" + i + ").png", 1000);
        //        KeyFrame keyFrame = new KeyFrame("E:/task/idea/螺旋式摘要/mlw_clip/mlw (" + i + ").png", videoTimeList[i - 1]);
        //        keyFrames.Add(keyFrame);
        //    }
        //}
        

        /// <summary>
        /// 选择颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPicker_ColorChanged(object sender, WPFInk.ColorChangedEventArgs e)
        {
            InkCollector.DefaultDrawingAttributes.Color = e.newColor.Color;
            OpacitySlider.Value = 1.0;
        }


        //清空画板
		private void ClearAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			if (this._inkFrame._inkCanvas.Children.Count > 0 || this._inkFrame._inkCanvas.Strokes.Count > 0)
			{

				DialogResult MsgBoxResult;//设置对话框的返回值
				MsgBoxResult = System.Windows.Forms.MessageBox.Show("删除以后将不能恢复，是否删除", "提示", MessageBoxButtons.YesNo);
				if (MsgBoxResult == DialogResult.Yes)//如果对话框的返回值是YES（按"Y"按钮）
				{
                    MathTool.getInstance().ClearAllStrokesAndChildren(_inkFrame);
				}
			}
            this.Height = GlobalValues.ControlPanel_OtherModeHeight;
        }

        //关闭控制面板
        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
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
                //FileInfo info = openfile.OpenFile();;

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

        private void OpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            //取小数点后一位

            string penOpacityStr = this.OpacitySlider.Value.ToString();
            double penOpacity;
            if (penOpacityStr.Length > 3)
            {
                penOpacity = double.Parse(penOpacityStr.Substring(0, 3));
            }
            else
            {
                penOpacity = double.Parse(penOpacityStr);
            }
            this.penOpacityTextBlock.Text = penOpacityStr;
            InkCollector.DefaultDrawingAttributes.Color = Color.FromArgb((byte)(255 * penOpacity), InkCollector.DefaultDrawingAttributes.Color.R, InkCollector.DefaultDrawingAttributes.Color.G, InkCollector.DefaultDrawingAttributes.Color.B);

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
			if (InkCollector.Mode == InkMode.Ink)
			{
                this.Height = GlobalValues.ControlPanel_InkModeHeight;
				_fontChooser.Visibility = Visibility.Collapsed;
				RectangleSlider.Visibility = Visibility.Visible;
				WidthSlider.Visibility = Visibility.Visible;
				OpacitySlider.Visibility = Visibility.Visible;
			}
			else if (InkCollector.Mode == InkMode.InsertText)
			{
                this.Height = GlobalValues.ControlPanel_InsertTextHeight;
				_fontChooser.Visibility = Visibility.Visible;
				RectangleSlider.Visibility = Visibility.Collapsed;
				WidthSlider.Visibility = Visibility.Collapsed;
				OpacitySlider.Visibility = Visibility.Collapsed;
			}
			else
			{
                this.Height = GlobalValues.ControlPanel_OtherModeHeight;
			}
            this.MinButton.Visibility = Visibility.Visible;
            this.MaxButton.Visibility = Visibility.Collapsed;
        }        
        private void MoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	InkCollector.Mode = InkMode.AutoMove;
        }

		InkAnalyzer analyze;
        private void StrokeToImageButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			_inkFrame._inkCanvas.StrokeCollected += new InkCanvasStrokeCollectedEventHandler(_inkCanvas_StrokeCollected_StrokeToImage);
			analyze = new InkAnalyzer();
			InkCollector.Mode = InkMode.Ink;
			_fontChooser.Visibility = Visibility.Hidden;
			RectangleSlider.Visibility = Visibility.Visible;
			WidthSlider.Visibility = Visibility.Visible;
			OpacitySlider.Visibility = Visibility.Visible;
            this.Height = GlobalValues.ControlPanel_InkModeHeight;

        }

		void _inkCanvas_StrokeCollected_StrokeToImage(object sender, InkCanvasStrokeCollectedEventArgs e)
		{
			InkCanvas inkCanvas = (InkCanvas)sender;
			if (analyze.RootNode.Strokes.IndexOf(inkCanvas.Strokes[inkCanvas.Strokes.Count - 1]) == -1)
			{
				analyze.AddStroke(inkCanvas.Strokes[inkCanvas.Strokes.Count - 1]);
			}
			analyze.BackgroundAnalyze();
			string analyzeStr = analyze.GetRecognizedString();
			//Stream streamOpen = new FileStream("pack://siteoforigin:,,,/xml/StrokeToImage.xml", FileMode.Open);//提示“不支持指定格式”
			Stream streamOpen = new FileStream(GlobalValues.FilesPath+"/WPFInk/WPFInk/bin/Debug/xml/StrokeToImage.xml", FileMode.Open);
			using (System.IO.StreamReader streamReader = new System.IO.StreamReader(streamOpen))
			{
				XmlReaderSettings settings = new XmlReaderSettings();
				XmlReader Reader = XmlReader.Create(streamReader, settings);
				while (Reader.Read())
				{
					if (Reader.NodeType == XmlNodeType.Element)
					{
						if (Reader.Name == "Image")
						{
							Reader.MoveToAttribute("stroke");
							string strokeStr = Reader.Value;
							Reader.MoveToAttribute("imageName");
							searchStrokes(strokeStr, Reader.Value, inkCanvas);
						}
					}
				}
			}
			
		}
		void _inkCanvas_StrokeCollected_StrokeInGraphic(object sender, InkCanvasStrokeCollectedEventArgs e)
		{
			InkCanvas inkCanvas = (InkCanvas)sender;
			Stroke lastStroke = inkCanvas.Strokes[inkCanvas.Strokes.Count - 1];
			if (analyze.RootNode.Strokes.IndexOf(lastStroke) == -1)
			{
				analyze.AddStroke(inkCanvas.Strokes[inkCanvas.Strokes.Count - 1]);
			}
			analyze.BackgroundAnalyze();
			string analyzeStr = analyze.GetRecognizedString();
			MyGraphic MyGraphicContrainStroke = InkCollector.Sketch.getMyGraphicContrainStroke(lastStroke);
			if (MyGraphicContrainStroke != null)
			{
				if (MyGraphicContrainStroke.textStrokeCollection.IndexOf(lastStroke) == -1)
				{

					MyGraphicContrainStroke.addTextStroke(lastStroke);
					MyGraphicContrainStroke.Text = analyzeStr;
					//Console.WriteLine("analyzeStr" + analyzeStr);
				}
			}
			
		}
		private void searchStrokes(string searchString, string imageName,InkCanvas inkCanvas)
		{
			StrokeCollection[] strokes = analyze.Search(searchString);
			foreach (StrokeCollection sc in strokes)
			{
				foreach (Stroke s in sc)
				{
					if (inkCanvas.Strokes.IndexOf(s) != -1)
					{
						inkCanvas.Strokes.Remove(sc);
					}
					if (analyze.RootNode.Strokes.IndexOf(s) != -1)
					{
						analyze.RemoveStrokes(sc);
					}
				}
				addImage(imageName, new Thickness(sc[0].StylusPoints[0].X, sc[0].StylusPoints[0].Y, 0, 0),inkCanvas);
			}
		}

		private void addImage(string name, Thickness margin, InkCanvas inkCanvas)
		{
			BitmapImage bitmapImage = new BitmapImage(new Uri("pack://siteoforigin:,,,/images/" + name + ".jpg", UriKind.RelativeOrAbsolute));
			Image image = new Image();
			image.Source = bitmapImage;
			image.Width = bitmapImage.Width;
			image.Height = bitmapImage.Height;
			image.Margin = margin;
			MyImage myImage = new MyImage(image);
			myImage.Left = image.Margin.Left;
			myImage.Top = image.Margin.Top;
			myImage.Height = image.Height;
			myImage.Width = image.Width;
			InkConstants.AddBound(myImage);
			Command aic = new AddImageCommand(_inkFrame.InkCollector, myImage);
			aic.execute(); 
			_inkFrame.InkCollector.CommandStack.Push(aic);
		}

		private void DrawGraphicButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			InkCollector.Mode = InkMode.DrawGraphic;
			_fontChooser.Visibility = Visibility.Hidden;
			RectangleSlider.Visibility = Visibility.Visible;
			WidthSlider.Visibility = Visibility.Visible;
			OpacitySlider.Visibility = Visibility.Visible;
            this.Height = GlobalValues.ControlPanel_InkModeHeight;
		}
		private void StrokeInGraphicButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			InkCollector.Mode = InkMode.DrawStrokeInGraphic;
			_fontChooser.Visibility = Visibility.Hidden;
			RectangleSlider.Visibility = Visibility.Visible;
			WidthSlider.Visibility = Visibility.Visible;
			OpacitySlider.Visibility = Visibility.Visible;
            this.Height = GlobalValues.ControlPanel_InkModeHeight;
        }

        private void Hidden_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            this.Visibility = Visibility.Collapsed;
		}

        #endregion
    }
}