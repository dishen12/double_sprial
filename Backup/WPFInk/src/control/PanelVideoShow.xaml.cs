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
	/// Interaction logic for PanelVideoShow.xaml
	/// </summary>
	public partial class PanelVideoShow : UserControl
	{
        public PanelVideo panelVideo;
		public PanelVideoShow()
		{
			this.InitializeComponent();
            panelVideo = new PanelVideo();
            this.HostPanelVideoShow.Child = panelVideo;
		}

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
           
		}
	}
}