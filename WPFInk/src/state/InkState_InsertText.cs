using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using WPFInk.cmd;
using System.Windows.Documents;
using WPFInk.ink;

namespace WPFInk.state
{
    public class InkState_InsertText : InkState
    {

		private InkFrame _inkFrame;
		private RichTextBox _richTextBox = null;
        private Nullable<Point> _startpoint = null;

        public InkState_InsertText(InkCollector inkCollector)
            : base(inkCollector)
		{
			this._inkFrame = inkCollector._mainPage;

        }

        /// <summary>
        /// 按下左键的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void _presenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {           
			//创建一个richTextBox
			_inkCanvas.CaptureMouse();
			List<MyRichTextBox> emptyMyRichTextBoxs = new List<MyRichTextBox>();
			foreach (MyRichTextBox myRichTextBox in _inkCollector.Sketch.MyRichTextBoxs)
			{
				TextRange textRangeOld = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);
				string textRangeStr1 = textRangeOld.Text.Replace(" ", "");
				string textRangeStr2 = textRangeStr1.Replace("\r\n", "");
				if (textRangeStr2 == "")
				{
					emptyMyRichTextBoxs.Add(myRichTextBox);
				}
			}
			foreach (MyRichTextBox myRichTextBox in emptyMyRichTextBoxs)
			{
				DeleteTextCommand dtc = new DeleteTextCommand(_inkCollector, myRichTextBox);
				dtc.execute();
			}
			_richTextBox = new RichTextBox();
			_startpoint = e.GetPosition(_inkCanvas);
			_richTextBox.Margin = new Thickness(_startpoint.Value.X, _startpoint.Value.Y, 0, 0);
			_richTextBox.Background = new SolidColorBrush(Colors.Transparent);
			_richTextBox.AcceptsReturn = true;
			_richTextBox.BorderBrush = new SolidColorBrush(Colors.Blue);
			_richTextBox.GotFocus += new RoutedEventHandler(_richTextBox_GotFocus);
			_richTextBox.LostFocus += new RoutedEventHandler(_richTextBox_LostFocus);
			_richTextBox.HorizontalAlignment = HorizontalAlignment.Left;
			_richTextBox.VerticalAlignment = VerticalAlignment.Top;
			TextRange textRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
			textRange.ApplyPropertyValue(RichTextBox.FontFamilyProperty, _inkFrame.TextFontFamily);
			textRange.ApplyPropertyValue(RichTextBox.FontSizeProperty, "60");//_inkFrame.TextFontSize);
            textRange.ApplyPropertyValue(RichTextBox.ForegroundProperty, new SolidColorBrush(Colors.Blue));//_inkFrame.TextFontColor);
			textRange.ApplyPropertyValue(RichTextBox.FontWeightProperty, _inkFrame.TextFontWeight);
			textRange.ApplyPropertyValue(RichTextBox.FontStyleProperty, _inkFrame.TextFontStyle);
        }

        public override void _presenter_MouseMove(object sender, MouseEventArgs e)
        {
            if (_startpoint != null)
            {
                Point current = e.GetPosition(_inkCanvas);
                double left, top;
                if (_startpoint.Value.X > current.X)
                    left = current.X;
                else
                    left = _startpoint.Value.X;
                if (_startpoint.Value.Y > current.Y)
                    top = current.Y;
                else
                    top = _startpoint.Value.Y;
                _richTextBox.Margin = new Thickness(left, top, 0, 0);
                _richTextBox.Width = Math.Abs(current.X - _startpoint.Value.X);
                _richTextBox.Height = Math.Abs(current.Y - _startpoint.Value.Y);
            }
        }

        public override void _presenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _inkCanvas.ReleaseMouseCapture();
            if (_startpoint != null)
            {                                     
                _startpoint = null;
            }
			MyRichTextBox myRichTextBox = new MyRichTextBox(_richTextBox);
			AddTextCommand atc = new AddTextCommand(_inkCollector, myRichTextBox);
			atc.execute();
			_inkCollector.CommandStack.Push(atc);			
            _richTextBox.Focus();
        }


		void _richTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("_richTextBox_LostFocus");
			_inkCollector.Mode = InkMode.InsertText;
			_richTextBox.BorderBrush = null;
			
		}

		void _richTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			_richTextBox.BorderBrush = new SolidColorBrush(Colors.Blue);
			//MessageBox.Show("_richTextBox_GotFocus");
			_inkCollector.Mode = InkMode.None;
			_inkCollector.SelectedRichTextBox = _richTextBox;
		}

    }
}
