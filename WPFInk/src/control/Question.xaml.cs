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
	/// Interaction logic for Question.xaml
	/// </summary>
	public partial class Question : UserControl
	{
        private int selectIndex = int.MinValue;

        public int SelectIndex
        {
            get { return selectIndex; }
            set { selectIndex = value; }
        }
		public Question()
		{
			this.InitializeComponent();
		}
        public void setQuestion(string question)
        {
            this.TBQuestion.Text = question;
        }
        public void addRadio(string radioContent,string groupName)
        {
            RadioButton rb = new RadioButton();
            rb.Content = radioContent;
            rb.FontSize = 15;
            rb.Foreground = new SolidColorBrush(Colors.White);
            rb.IsEnabled = true;
            rb.GroupName = groupName;
            rb.IsChecked = false;
            rb.Checked += new RoutedEventHandler(rb_Checked);
            RadioStackPanel.Children.Add(rb);
        }

        void rb_Checked(object sender, RoutedEventArgs e)
        {
            selectIndex = RadioStackPanel.Children.IndexOf((RadioButton)sender);
        }
        private void Option1_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //SingleTaskResult[0].SingleCheck = "YES";
            //SingleTaskResult[0].sign = 1;
            //if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
            //    this.S_StartB.IsEnabled = true;
        }

        private void Option2_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //SingleTaskResult[0].SingleCheck = "NO";
            //SingleTaskResult[0].sign = 1;
            //if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
            //    this.S_StartB.IsEnabled = true;
        }

        private void Option3_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //SingleTaskResult[1].SingleCheck = "YES";
            //SingleTaskResult[1].sign = 1;
            //if (1 == SingleTaskResult[0].sign && 1 == SingleTaskResult[1].sign && 1 == SingleTaskResult[2].sign)
            //    this.S_StartB.IsEnabled = true;
        }

	}
}