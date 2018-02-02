using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WPFInk.ink;
using WPFInk.video;

namespace WPFInk.cmd
{
	class VisibleMyButtonCommand : Command
	{
		private InkCollector _inkcollector;
		private Sketch _sketch;
		private MyButton _myButton;


		public VisibleMyButtonCommand(InkCollector inkcollector, MyButton myButton)
		{
			_inkcollector = inkcollector;
			_sketch = _inkcollector.Sketch;
			_myButton = myButton;
		}



		public void execute()
		{
			_myButton.Button.Visibility = Visibility.Visible;
			_myButton.TextBoxTime.Visibility = Visibility.Visible;
			if (_myButton.IsGlobal)
			{
				foreach (MySmallRectangle msr in _inkcollector.Sketch.mySmallRectangles)
				{
					if (msr.ParentMyButton == _myButton)
					{
						msr.Rectangle.Visibility = Visibility.Visible;
					}
				}
				_myButton.RBorder.Visibility = Visibility.Visible;
			}
			
		}

		public void undo()
		{
			_myButton.Button.Visibility = Visibility.Collapsed;
			_myButton.TextBoxTime.Visibility = Visibility.Collapsed;
			if (_myButton.IsGlobal)
			{
				foreach (MySmallRectangle msr in _inkcollector.Sketch.mySmallRectangles)
				{
					if (msr.ParentMyButton == _myButton)
					{
						msr.Rectangle.Visibility = Visibility.Collapsed;
					}
				}
				_myButton.RBorder.Visibility = Visibility.Collapsed;
			}
		}
	}
}
