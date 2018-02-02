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
	/// Interaction logic for PentagramExtractive.xaml
	/// </summary>
	public partial class PentagramExtractive : UserControl
	{
		public PentagramExtractive()
		{
			this.InitializeComponent();
            InitApp();
        }

        //初始化InkFrame
        private void InitApp()
        {
            this._head.setParent(this, 400);

        }
	}
}