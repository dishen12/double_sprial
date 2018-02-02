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
	/// Interaction logic for UpdateRecognitionRusult.xaml
	/// </summary>
	public partial class UpdateRecognitionRusult : UserControl
	{
		private MyButton _myButton;
		private MainWindow _mainWindow;
		private VideoAnnotation _videoAnnotation;
		private string _buttonStyle;
		public UpdateRecognitionRusult()
		{
			this.InitializeComponent();
		}

		public void setMyButtonVideoAnnotation(MyButton myButton, VideoAnnotation va,string buttonStyle)
		{
			_myButton = myButton;
			_videoAnnotation = va;
			_buttonStyle = buttonStyle;
		}

		public void setMainWindow(MainWindow w)
		{
			_mainWindow = w;
		}
		
		private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			//updateResult();
		}

		private void _textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				//updateResult();
			}
		}
        /*
		private void updateResult()
		{
			if (_myButton != null && _videoAnnotation != null && _buttonStyle == "thumb")
			{
				_videoAnnotation.IsUpdatedRecognitionResults = true;
				_myButton.AnalyzeResults = _textBox.Text;
				_videoAnnotation.AddThumbMyButton(_myButton);
			}
			if (_myButton != null && _videoAnnotation != null && _buttonStyle == "title")
			{
				_myButton.AnalyzeResults = _textBox.Text;
			}
			if (_mainWindow != null)
			{
				_mainWindow.IsUpdatedRecognitionResults = true;
				_mainWindow.UpdatedRecognitionResults = _textBox.Text;
				_mainWindow.AutoGenerationStoryBoard();
			}
			this.Visibility = Visibility.Collapsed;
		}
         * */
	}
}