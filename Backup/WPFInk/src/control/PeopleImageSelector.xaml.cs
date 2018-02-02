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
	/// Interaction logic for PeopleImageSelector.xaml
	/// </summary>
	public partial class PeopleImageSelector : UserControl
	{
		public PeopleImageSelector()
		{
			this.InitializeComponent();
		}

        public delegate void ExecuteClick();
        public event ExecuteClick OnClick;//第一个按钮的事件
		
		private void Nest1Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            OnClick();
            this.Visibility = Visibility.Collapsed;
                
		}

		
	}
}