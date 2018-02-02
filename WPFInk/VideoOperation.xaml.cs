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
using System.Windows.Media.Animation;
using System.Windows.Ink;
using System.Windows.Shapes;
using System.Threading;
using WPFInk.tool;
using WPFInk.video;
using WPFInk.ink;
using WPFInk.cmd;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for VideoOperation.xaml
	/// </summary>
	public partial class VideoOperation : Window
    {
        private bool isFullScreen10 = false;
        private double Column0;
        private double Column1;
        private double Row0;
        private double Row1;
        public InkFrame KeyWordsImagesInkFrame;
		public VideoOperation()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		    InitApp();
        }

        private void InitApp()
        {
            KeyWordsImagesInkFrame = _scrollInkFrame._inkFrame;
            _videoAnnotation.setThumbInk(_thumbInk,_titleInk);
			_videoAnnotation.setVideoOperation(this);
			_titleInk.Title_InkFrame.InkCollector.Mode = InkMode.None;
            _thumbInk.ButtonScreen.Click += new RoutedEventHandler(ButtonScreenThumb_Click);
            WPFInk.PersistenceManager.PersistenceManager.getInstance().setVideoOperation(this);
            _videoAnnotation.Annotation_InkFrame.GotFocus += new RoutedEventHandler(_videoAnnotation_GotFocus);
            _thumbInk.Thumb_InkFrame.GotFocus += new RoutedEventHandler(Thumb_InkFrame_GotFocus);
			_titleInk.Title_InkFrame.GotFocus += new RoutedEventHandler(Title_InkFrame_GotFocus);
            _thumbInk.Thumb_ControlPanel._titleInk = _titleInk;
            _thumbInk.Thumb_ControlPanel._videoAnnotation = _videoAnnotation;
            _thumbInk.Thumb_ControlPanel._videoOperation = this;
            _titleInk.setVideoOperation(this);
			KeyWordsImagesControlPanel.setInkFrame(KeyWordsImagesInkFrame);
			KeyWordsImagesControlPanel.Height = 330;
			_thumbInk.Thumb_ControlPanel.setVideoList(_videoList);

            _searchInkCanvas.DefaultDrawingAttributes.Width = 4;
            _searchInkCanvas.DefaultDrawingAttributes.Height = 4;
            _searchInkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
        }

        void Title_InkFrame_GotFocus(object sender, RoutedEventArgs e)
        {
            _videoAnnotation.Annotation_ControlPanel.Height = 20;
            _videoAnnotation.Annotation_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            _videoAnnotation.Annotation_ControlPanel.MaxButton.Visibility = Visibility.Visible;
            _thumbInk.Thumb_ControlPanel.Height = 20;
            _thumbInk.Thumb_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            _thumbInk.Thumb_ControlPanel.MaxButton.Visibility = Visibility.Visible;

        }

        void Thumb_InkFrame_GotFocus(object sender, RoutedEventArgs e)
        {
            _videoAnnotation.Annotation_ControlPanel.Height = 20;
            _videoAnnotation.Annotation_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            _videoAnnotation.Annotation_ControlPanel.MaxButton.Visibility = Visibility.Visible;
            _titleInk.Title_ControlPanel.Height = 20;
            _titleInk.Title_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            _titleInk.Title_ControlPanel.MaxButton.Visibility = Visibility.Visible;
        }

        void _videoAnnotation_GotFocus(object sender, RoutedEventArgs e)
        {
            _titleInk.Title_ControlPanel.Height = 20;
            _titleInk.Title_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            _titleInk.Title_ControlPanel.MaxButton.Visibility = Visibility.Visible;
            _thumbInk.Thumb_ControlPanel.Height = 20;
            _thumbInk.Thumb_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            _thumbInk.Thumb_ControlPanel.MaxButton.Visibility = Visibility.Visible;
        }     
        

        

        void ButtonScreenThumb_Click(object sender, RoutedEventArgs e)
        {
            if (isFullScreen10 == false)
            {

                Column0 = double.Parse(TableGrid.ColumnDefinitions[0].ActualWidth.ToString());
                Column1 = double.Parse(TableGrid.ColumnDefinitions[1].ActualWidth.ToString());
                Row0 = double.Parse(TableGrid.RowDefinitions[0].ActualHeight.ToString());
                Row1 = double.Parse(TableGrid.RowDefinitions[1].ActualHeight.ToString());
                TableGrid.ColumnDefinitions[0].Width = new GridLength(this.ActualWidth);
                TableGrid.RowDefinitions[0].Height = new GridLength(0);
                TableGrid.ColumnDefinitions[1].Width = new GridLength(0);
                TableGrid.RowDefinitions[1].Height = new GridLength(this.ActualHeight); 
                _videoAnnotation.Annotation_ControlPanel.Visibility = Visibility.Hidden;
                isFullScreen10 = true;
            }
            else
            {

                TableGrid.ColumnDefinitions[0].Width = new GridLength(Column0);
                TableGrid.RowDefinitions[0].Height = new GridLength(Row0);
                TableGrid.ColumnDefinitions[1].Width = new GridLength(Column1);
                TableGrid.RowDefinitions[1].Height = new GridLength(Row1);
                _videoAnnotation.Annotation_ControlPanel.Visibility = Visibility.Visible;
                isFullScreen10 = false;
            }
        }

        private void radioButton_1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _thumbInk.Thumb_InkFrame._inkCanvas.Children.Clear();
            _thumbInk.Thumb_InkFrame._inkCanvas.Strokes.Clear();
            string content="Cat, Sofa, Dog, Fight, Falling Down, Dancing, Outside, Man, Car, Run";
            RichTextBox tb = new RichTextBox();
			tb.HorizontalAlignment = HorizontalAlignment.Left;
			tb.VerticalAlignment = VerticalAlignment.Top;
            tb.Margin = new Thickness(0);
            tb.Width = _thumbInk.ActualWidth-100;
            tb.Height = _thumbInk.ActualHeight-100;
			tb.Padding = new Thickness(0);
			System.Windows.Documents.Paragraph paragraph = new Paragraph();
			paragraph.LineHeight = 1;
			paragraph.Padding = new Thickness(0);
			paragraph.TextAlignment = TextAlignment.Left;
			Run run = new Run();
			run.Text = content;
			paragraph.Inlines.Add(run);
			tb.Document.Blocks.Clear();
			tb.Document.Blocks.Add(paragraph);
            tb.IsHitTestVisible = false;
            tb.BorderBrush = null;
			tb.AcceptsReturn = true;
			tb.Background = new SolidColorBrush(Colors.Transparent);
			TextRange textRange = new TextRange(tb.Document.ContentStart, tb.Document.ContentEnd);
            textRange.ApplyPropertyValue(RichTextBox.FontSizeProperty, "30");
            textRange.ApplyPropertyValue(RichTextBox.FontWeightProperty, "Bold");            
            SolidColorBrush myBrush = new SolidColorBrush(Colors.Blue);
            textRange.ApplyPropertyValue(RichTextBox.ForegroundProperty, myBrush);

            InkCollector ic = _thumbInk.Thumb_InkFrame.InkCollector;
            ic.InkCanvas.Children.Add(tb);
			MyRichTextBox mt = new MyRichTextBox(tb);
            ic.Sketch.AddText(mt);
        }

        private void radioButton_2_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _thumbInk.Thumb_InkFrame._inkCanvas.Children.Clear();
            _thumbInk.Thumb_InkFrame._inkCanvas.Strokes.Clear();
            for(int i=1;i<8;i++)
            {
                Image image = InkConstants.getImageFromName(@"E:\task\WPFInk材料\程序截图\加菲猫草图视频摘要\"+i+".png");
                image.Width = 125;
                image.Height = 80;
                image.Margin=new Thickness((i-1)*135+30,20,0,0);
                _thumbInk.Thumb_InkFrame._inkCanvas.Children.Add(image);
            }
        }

        private void radioButton_3_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _thumbInk.Thumb_InkFrame._inkCanvas.Children.Clear();
            _thumbInk.Thumb_InkFrame._inkCanvas.Strokes.Clear();
            
            Image image = InkConstants.getImageFromName(@"E:\task\WPFInk材料\程序截图\加菲猫草图视频摘要\summarization.png");
            image.Width = _thumbInk.ActualWidth - 100;
            image.Height = _thumbInk.ActualHeight-10;
            image.Margin = new Thickness((_thumbInk.ActualWidth - image.Width)/2, 2, 0, 0);
            _thumbInk.Thumb_InkFrame._inkCanvas.Children.Add(image);
        }

        private void _searchInkCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _searchInkCanvas.Visibility = Visibility.Collapsed;
            TableGrid.ColumnDefinitions[1].Width = new GridLength(TableGrid.ActualWidth* 0.5);
        }

        
	}
}