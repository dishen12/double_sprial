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
	/// Interaction logic for NodeEditorAndPlayShow.xaml
	/// </summary>
	public partial class NodeEditorAndPlayShow : UserControl
	{
		public PointView pointView;
		public NodeEditorAndPlayShow()
		{
			this.InitializeComponent();
            pointView = new PointView();
            this.HostPointView.Child = pointView;
		}

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
            
		}
	}
}