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
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for MyprogressBar.xaml
	/// </summary>
	public partial class MyprogressBar : UserControl
	{
        public Storyboard sbProgressBar;
		public MyprogressBar()
		{
			this.InitializeComponent();
            sbProgressBar = this.Resources["StoryboardCicleProgressBar"] as Storyboard;//渐入
		}
        public void Hiden()
        {
            this.Visibility = Visibility.Collapsed;
        }
	}
}