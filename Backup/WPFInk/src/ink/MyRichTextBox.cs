using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace WPFInk.ink
{
	public class MyRichTextBox
	{


		private RichTextBox _richTextBox;
		private string videoPath = "";
		private bool isExist = false;

		public RichTextBox RichTextBox
		{
			get { return _richTextBox; }
			set { _richTextBox = value; }
		}

		public bool IsExist
		{
			get { return isExist; }
			set { isExist = value; }
		}

		public string VideoPath
		{
			get { return videoPath; }
			set { videoPath = value; }
		}

		public MyRichTextBox(RichTextBox richTextBox)
		{
			this._richTextBox = richTextBox;
			this._richTextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
			this._richTextBox.VerticalContentAlignment = VerticalAlignment.Top;
			this._richTextBox.TextChanged += new TextChangedEventHandler(_richTextBox_TextChanged);
			
		}

		void _richTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextRange textRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
			//Console.WriteLine(textRange.Text);
			Rect rectStart = _richTextBox.Document.ContentStart.GetCharacterRect(LogicalDirection.Forward);
			Rect rectEnd = _richTextBox.Document.ContentEnd.GetCharacterRect(LogicalDirection.Forward);

			//Console.WriteLine(rectStart.TopLeft.X.ToString() + "," + rectStart.TopLeft.Y.ToString());
			//Console.WriteLine(rectEnd.BottomRight.X.ToString() + "," + rectEnd.BottomRight.Y.ToString());
			_richTextBox.Width = Math.Max(rectEnd.BottomRight.X - rectStart.TopLeft.X,_richTextBox.Width);
			_richTextBox.Height = Math.Max(rectEnd.BottomRight.Y-rectStart.TopLeft.Y+10,_richTextBox.Height);
			//Console.WriteLine(_richTextBox.Width);

		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

		}
	}
}
