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
using WPFInk.ink;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for InkControl.xaml
	/// </summary>
	public partial class InkControl : UserControl
    {
        private InkCollector _inkCollector;
		public InkControl()
		{
            this.InitializeComponent();
            _controlPanel.setInkFrame(_inkFrame);
            _inkCollector = _inkFrame.InkCollector;

		}

	}
}