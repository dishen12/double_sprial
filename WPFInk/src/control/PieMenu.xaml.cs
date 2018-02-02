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
    /// 自定义控件之浮动窗口，事件处理都由使用控件的控件来定义
    /// </summary>

    public enum OperateButton
    {
        Move,
        Rotate,
        Zoom,
        Close
    }
	/// <summary>
	/// Interaction logic for PieMenu.xaml
	/// </summary>
	public partial class PieMenu : UserControl
	{
        public delegate void ExecuteOperate();
        public event ExecuteOperate OnMove;//第一个按钮的事件
        public event ExecuteOperate OnRotate;//第二个按钮的事件
        public event ExecuteOperate OnZoom;//第三个按钮的事件
        public event ExecuteOperate OnClose;//关闭按钮的事件
		public PieMenu()
		{
			this.InitializeComponent();
		}
        //private bool IsMoving = false;
        //private Point CurrentPoint;
        private OperateButton _operateButton;
        public OperateButton OperateButton
        {
            get
            {
                return _operateButton;
            }
            set
            {
                _operateButton = value;   
                SolidColorBrush scb=new  SolidColorBrush(Color.FromArgb(255,240,234,234));
                switch (value)                                     
                {
                    case OperateButton.Move:                         
                        this.MoveButton.Opacity = 0.5;
                        this.RotateButton.Opacity = 1;
                        this.ZoomButton.Opacity = 1;
                        break;
                    case OperateButton.Rotate:
                        this.MoveButton.Opacity = 1;
                        this.RotateButton.Opacity = 0.5;
                        this.ZoomButton.Opacity = 1;
                        break;
                    case OperateButton.Zoom:
                        this.MoveButton.Opacity = 1;
                        this.RotateButton.Opacity = 1;
                        this.ZoomButton.Opacity = 0.5;
                        break;
                    case OperateButton.Close:
                        this.Visibility = Visibility.Collapsed;         
                        break;

                }
            }
        }

       

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnClose();
            this.OperateButton = OperateButton.Close;

        }

        private void ZoomButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnZoom();
            this.OperateButton = OperateButton.Zoom;
        }

        private void RotateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnRotate();
        	// TODO: Add event handler implementation here.
            this.OperateButton = OperateButton.Rotate;
        }

        private void MoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnMove();
            this.OperateButton = OperateButton.Move;
        }

		
	}
}