using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFInk.ink;
using WPFInk.tool;

namespace WPFInk.cmd
{
    /// <summary>
    /// 删除stroke
    /// 要删除sketch和inkpresenter中相对应的stroke
    /// 同时，还要删除stroke所在group中的所有stroke
    /// </summary>
    public class DeleteButtonCommand:Command
    {
        private InkCollector _inkcollector;
        private Sketch _sketch;
        private MyButton _myButton;
		private VideoList _videoList;

		public DeleteButtonCommand(InkCollector inkcollector, MyButton myButton, VideoList videoList)
        {
            _inkcollector = inkcollector;
            _sketch = _inkcollector.Sketch;
            _myButton = myButton;
			_videoList = videoList;
        }

        

        public void execute()
        {

            _inkcollector.RemoveButton(_myButton);
			//删除对应VideoList
			bool isHaveOther = false;
			foreach (MyButton mb in _inkcollector.Sketch.MyButtons)
			{
				if (mb.IsDeleted==false&&mb!=_myButton&&mb.VideoPath == _myButton.VideoPath)
				{
					isHaveOther = true;
					break;
				}
			}
			if (!isHaveOther)
			{
				ListBoxItem deleteLBI = null;
				foreach (ListBoxItem lbi in _videoList.VideoList_ListBox.Items)
				{
					if (lbi.Content.ToString() == _myButton.VideoFileName)
					{
						deleteLBI = lbi;
					}
				}
				if (deleteLBI != null)
				{
					_videoList.VideoList_ListBox.Items.Remove(deleteLBI);
					int count = _videoList.VideoList_ListBox.Items.Count;
					if (count == 0)
					{
						_videoList.Visibility = Visibility.Collapsed;
					}
					if (count > 0 && count < 11)
					{
						_videoList.VideoList_ListBox.Height = 30 * count + 6;
						_videoList.Height = 30 * count + 26;
						if (_videoList.MinButton.Visibility == Visibility.Visible)
						{
							MyStoryboard.getInstance().HeightStoryboard(_videoList.border, 30 * (count + 1) + 6, 30 * count + 6, 0.5).Begin(_videoList);
						}
					}
				}
			}
        }

        public void undo()
        {
            _inkcollector.AddButton(_myButton);

			//添加对应VideoList
			bool isHaveOther = false;
			foreach (ListBoxItem lbi in _videoList.VideoList_ListBox.Items)
			{
				if (lbi.Content.ToString() == _myButton.VideoFileName)
				{
					isHaveOther = true;
				}
			}
			if (!isHaveOther)
			{
				ListBoxItem listBoxItem = new ListBoxItem();
				listBoxItem.Height = 30;
				listBoxItem.Width = 144;
				listBoxItem.Content = _myButton.VideoFileName;
				listBoxItem.Background = _myButton.Button.Background;
				listBoxItem.BorderThickness = new Thickness(1);
				listBoxItem.BorderBrush = Brushes.Black;
				listBoxItem.VerticalAlignment = VerticalAlignment.Top;
				listBoxItem.HorizontalAlignment = HorizontalAlignment.Left;
				_videoList.VideoList_ListBox.Items.Add(listBoxItem);
				int count = _videoList.VideoList_ListBox.Items.Count;
				if (count > 0)
				{
                    _videoList.Visibility = Visibility.Visible;
				}
				if (count > 0 && count < 11)
				{
					_videoList.VideoList_ListBox.Height = 30 * count + 6;
					_videoList.Height = 20 + 30 * count + 6;
					if (_videoList.MinButton.Visibility == Visibility.Visible)
					{
						MyStoryboard.getInstance().HeightStoryboard(_videoList.border, 30 * (count - 1) + 6, 30 * count + 6, 0.5).Begin(_videoList);
					}
				}
				
			}
        }
    }
}
