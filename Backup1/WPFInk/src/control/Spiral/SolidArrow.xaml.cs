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
	/// Interaction logic for SolidArrow.xaml
	/// </summary>
	public partial class SolidArrow : UserControl
	{
		public SolidArrow()
		{
			this.InitializeComponent();
		}
        public void rotate(double angle)
        {
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.RotateAt(angle, this.Width/2,0);
            this.RenderTransform = new MatrixTransform(m);
        }
	}
}