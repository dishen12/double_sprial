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

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for UserControlHead.xaml
	/// </summary>
	public partial class UserControlHead : UserControl
	{
        private UserControl parent = null;
        private double parentHeight = 0;
		public UserControlHead()
		{
			this.InitializeComponent();
		}
        public void setParent(UserControl uc,double parentH)
        {
            this.parent = uc;
            parentHeight = parentH;

        }
		private void MaxButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            if (parent != null)
            {
                parent.Height = parentHeight;
                this.MinButton.Visibility = Visibility.Visible;
                this.MaxButton.Visibility = Visibility.Collapsed;
            }
		}

		private void MinButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            if (parent != null)
            {
                parent.Height = this.Height;
                this.MinButton.Visibility = Visibility.Collapsed;
                this.MaxButton.Visibility = Visibility.Visible;
            }
		}


        //控制面板拖动
        private bool IsMoving = false;
        private Point CurrentPoint;
        private void HeadArea_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.HeadArea.CaptureMouse();
            IsMoving = true;
            CurrentPoint = e.GetPosition((UIElement)parent.Parent);
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
                Point p = e.GetPosition(((UIElement)parent.Parent));
                Thickness currentmargin = parent.Margin;
                Point offset = new Point(p.X - CurrentPoint.X, p.Y - CurrentPoint.Y);
                parent.Margin = new Thickness(currentmargin.Left + offset.X, currentmargin.Top + offset.Y, currentmargin.Right - offset.X, currentmargin.Bottom - offset.Y);
                CurrentPoint = p;
            }

        }
	}
}