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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFInk.ink;
using WPFInk.video;
using WPFInk.cmd;
using WPFInk.ChineseWordSegmentation;
using System.Windows.Ink;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for ThumbInk.xaml
	/// </summary>
	public partial class ThumbInk : UserControl
	{
		private InkCollector _inkCollector;
		private double _thumbWidth = 120; //缩略图宽度
		private double _thumbInterval = 80;//两个缩略图之间的间隙
		public ThumbInk()
		{
			this.InitializeComponent();
            InitApp();
        }

        private void InitApp()
        {
            //将controlpanel和inkframe关联
            Thumb_InkFrame.rectangle1.Visibility = Visibility.Collapsed;
            Thumb_InkFrame._inkCanvas.Background = null;
            Thumb_ControlPanel.setInkFrame(Thumb_InkFrame);
            Thumb_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            //Thumb_ControlPanel.MaxButton.Visibility = Visibility.Visible;
            _inkCollector = Thumb_InkFrame.InkCollector;
            //_inkCollector.Mode = InkMode.None;
            _inkCollector.Mode = InkMode.GestureOnly;
            if (_inkCollector.InkCanvas.IsGestureRecognizerAvailable)
            {
                _inkCollector.InkCanvas.Gesture += new InkCanvasGestureEventHandler(Thumb_ControlPanel.InkCanvas_Gesture);
                _inkCollector.InkCanvas.SetEnabledGestures(
                    new ApplicationGesture[] { ApplicationGesture.ScratchOut ,              //擦除      
                        ApplicationGesture.Down
                    });
            }
        }

        private void KeyWordsSentence_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
			if (e.Key == Key.Enter)
			{
				String keyWordsSentence = KeyWordsSentence.Text;
				keyWordsSentence.Trim();

				//显示所有的mybutton
				foreach (MyButton mb in _inkCollector.Sketch.MyButtons)
				{
					if (mb.IsDeleted == false)
					{
						Command vmbc = new VisibleMyButtonCommand(_inkCollector, mb);
						vmbc.execute();
						_inkCollector.CommandStack.Push(vmbc);
					}
				}

				//MessageBox.Show(keyWordsSentence.Length.ToString());
				List<MyButton> myButtonList = new List<MyButton>();
				string[] strArray = autoStoryBoardByString(keyWordsSentence);
				for (int i = 0; i < strArray.Length; i++)
				{
					string CharString = strArray[i];
					string otherWords = "的了吗，,。.着";
					if (otherWords.IndexOf(CharString) == -1)
					{
						foreach (MyButton myButton in _inkCollector.Sketch.MyButtons)
						{
							bool findFlag = false;
							//第一步：检测文本
							foreach (MyRichTextBox myRichTextBox in myButton.InkFrame.InkCollector.Sketch.MyRichTextBoxs)
							{
								TextRange textRange = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);
								if (textRange.Text.IndexOf(CharString) >-1 && myButtonList.IndexOf(myButton) == -1)
								{
									myButtonList.Add(myButton);
									MessageBox.Show("MyRichTextBox:" + CharString);
									findFlag = true;
									break;
								}
							} 
							if (findFlag)
							{
								break;
							}

							//第二步：检测图片名称
							foreach (MyImage mi in myButton.InkFrame.InkCollector.Sketch.Images)
							{
								if (mi.SafeFileName.IndexOf(CharString) > -1 && myButtonList.IndexOf(myButton) == -1)
								{
									myButtonList.Add(myButton);
									MessageBox.Show("MyImage:" + CharString);
									findFlag = true;
									break;
								}
							}
							if (findFlag)
							{
								break;
							}	

							//第三步：检测视频名称,全局Mybutton较优先
							if (myButton.VideoFileName.IndexOf(CharString) > -1 && myButtonList.IndexOf(myButton) == -1&&myButton.IsGlobal)
							{
								myButtonList.Add(myButton);
								MessageBox.Show("VideoFileName:" + CharString);
								break;
							}

						}

					}
				}

				//第一步：删除不需要的Mybutton
				List<MyButton> myButtonOtherList = new List<MyButton>();
				foreach (MyButton mb in _inkCollector.Sketch.MyButtons)
				{
					if (myButtonList.IndexOf(mb) == -1)
					{
						myButtonOtherList.Add(mb);
					}
				}
				//MessageBox.Show(Thumb_InkFrame._inkCanvas.Children.Count.ToString());
				foreach (MyButton mb in myButtonOtherList)
				{
					Command hmbc = new HiddenMyButtonCommand(_inkCollector, mb);
					hmbc.execute();
					_inkCollector.CommandStack.Push(hmbc);
				}
				//MessageBox.Show(Thumb_InkFrame._inkCanvas.Children.Count.ToString());
				foreach(MyArrow ma in _inkCollector.Sketch.MyArrows)
				{
					DeleteArrowCommand dac=new DeleteArrowCommand(_inkCollector,ma);
					dac.execute();
					_inkCollector.CommandStack.Push(dac);
				}

				//第二步：移动需要的Mybutton并添加连线
				int ThumbIndex = 0;
				foreach (MyButton mb in myButtonList)
				{
					double Left = _thumbInterval + (_thumbWidth + _thumbInterval) * (ThumbIndex % 6);
					double Top = _thumbInterval + (_thumbWidth * (mb.Height / mb.Width) + _thumbInterval) * (ThumbIndex / 6);
					ButtonMoveCommand bmc = new ButtonMoveCommand(mb, Left - mb.Left, Top - mb.Top, _inkCollector);
					bmc.execute();
					_inkCollector.CommandStack.Push(bmc);
					if (ThumbIndex != myButtonList.Count - 1)
					{
						ThumbConnector thumbConnector = new ThumbConnector(myButtonList[ThumbIndex], myButtonList[ThumbIndex + 1]);
						MyArrow ma = new MyArrow(thumbConnector.arrow);
						ma.PreMyButton = myButtonList[ThumbIndex];
						ma.NextMyButton = myButtonList[ThumbIndex + 1];
						ma.StartPoint = thumbConnector.startPoint;
						ma.EndPoint = thumbConnector.endPoint;
						Command aac = new AddArrowCommand(_inkCollector, ma);
						aac.execute();
						_inkCollector.CommandStack.Push(aac);
					} 
					ThumbIndex++;
				}
				

			}
        }

        private void KeyWordsSentence_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
			Thumb_ControlPanel.Height = 20;
			Thumb_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
			Thumb_ControlPanel.MaxButton.Visibility = Visibility.Visible;
        }

		private string[] autoStoryBoardByString(String s)
		{
			string textRangeStr1 = s.Replace(" ", "");
			string textRangeStr2 = textRangeStr1.Replace("\r\n", "");

			ChineseParse chineseParse = new ChineseParse();
			string[] strArray = chineseParse.ParseChinese(textRangeStr2);
			return strArray;
		}

		private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            Thumb_ControlPanel.Visibility = Visibility.Visible;
		}


		

	}
}