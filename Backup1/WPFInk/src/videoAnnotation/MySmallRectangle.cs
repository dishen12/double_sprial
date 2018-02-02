using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using WPFInk.ink;
namespace WPFInk.video
{
	public class MySmallRectangle
	{
		private Rectangle _rectangle;
		private MyButton _myButton;
		private MyButton _ParentMyButton;

		
		public MySmallRectangle(Rectangle r)
		{
			_rectangle = r;
		}



		public Rectangle Rectangle
		{
			get { return _rectangle; }
			set { _rectangle = value; }
		}
		public MyButton MyButton
		{
			get { return _myButton; }
			set { _myButton = value; }
		}
		public MyButton ParentMyButton
		{
			get { return _ParentMyButton; }
			set { _ParentMyButton = value; }
		}
		public void Dispose()
		{
			GC.SuppressFinalize(this);

		}
	}
}
