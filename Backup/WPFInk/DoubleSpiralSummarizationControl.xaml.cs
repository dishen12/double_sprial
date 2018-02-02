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
using System.Windows.Shapes;
using WPFInk.videoSummarization;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for DoubleSpiralSummarization.xaml
	/// </summary>
    public partial class DoubleSpiralSummarizationControl : Window
	{
		public DoubleSpiralSummarizationControl()
		{
			this.InitializeComponent();
			// Insert code required on object creation below this point.
            _inkFrame.InkCollector.addImages();
            _inkFrame._inkCanvas.Background = new SolidColorBrush(Colors.Black);
            InkCanvas inkCansLeft = new InkCanvas();
            inkCansLeft.Width = 1440;// _inkFrame._inkCanvas.ActualWidth;
            inkCansLeft.Height = 960;// _inkFrame._inkCanvas.ActualHeight;
            inkCansLeft.Background = new SolidColorBrush(Colors.White);
            MyDoubleSpiral myDoubleSpiral = new MyDoubleSpiral(40, new StylusPoint(635,500), 770, 3, 3, inkCansLeft, true);
           // MySpiral mySpiral = new MySpiral(30, new StylusPoint(200, 200), 3, 10, inkCansLeft, false);

            WPFInk.videoSummarization.DoubleSpiralSummarization spiralSummarization = new WPFInk.videoSummarization.DoubleSpiralSummarization(_inkFrame.InkCollector, myDoubleSpiral, _inkFrame.InkCollector.KeyFramesSpiral[_inkFrame.InkCollector.VideoNum],false);
            _inkFrame._inkCanvas.Children.Add(inkCansLeft);
                
		}
	}
}