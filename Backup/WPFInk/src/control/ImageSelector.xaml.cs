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
using System.IO;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for ImageSelector.xaml
	/// </summary>
	public partial class ImageSelector : UserControl
	{

        public delegate void ExecuteClick();
        public event ExecuteClick OnClick1;//第一个按钮的事件
        public event ExecuteClick OnClick2;//第一个按钮的事件
		public ImageSelector()
		{
			this.InitializeComponent();
		}

		private void Nest1Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            OnClick1();
            this.Visibility = Visibility.Collapsed;
                
		}

		private void Nest3Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OnClick2();
            this.Visibility = Visibility.Collapsed;
		}
	}
}