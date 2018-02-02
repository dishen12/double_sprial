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
using System.Windows.Media.Animation;
using WPFInk.tool;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for VideoList.xaml
	/// </summary>
	public partial class VideoList : UserControl
	{
		//移动
		private bool IsMoving = false;
		private Point CurrentPoint;
		public VideoList()
		{
			this.InitializeComponent();
			this.Height = 20;
		}

		private void MaxButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Height = 326;
			MyStoryboard.getInstance().HeightStoryboard(border, 0, 306 > 30 * VideoList_ListBox.Items.Count + 6 ? 30 * VideoList_ListBox.Items.Count + 6 : 306,1).Begin(this);
			MaxButton.Visibility = Visibility.Collapsed;
			MinButton.Visibility = Visibility.Visible;
		}
		

		private void MinButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			MyStoryboard.getInstance().HeightStoryboard(border, 306 > 30 * VideoList_ListBox.Items.Count + 6 ? 30 * VideoList_ListBox.Items.Count + 6 : 306,0,1).Begin(this);
			MaxButton.Visibility = Visibility.Visible;
			MinButton.Visibility = Visibility.Collapsed;
		}

		private void HeadRectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.HeadRectangle.CaptureMouse();
			IsMoving = true;
			CurrentPoint = e.GetPosition((UIElement)this.Parent);     
		}

		private void HeadRectangle_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (IsMoving)
			{
				IsMoving = false;
				this.HeadRectangle.ReleaseMouseCapture();
			}
		}

		private void HeadRectangle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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

	}
}