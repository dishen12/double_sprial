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
using WPFInk.cmd;
using WPFInk.Global;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for TitleInk.xaml
	/// </summary>
	public partial class TitleInk : UserControl
	{
        private InkCollector _inkCollector;
        public InkFrame Title_InkFrame;
        private VideoOperation _videoOperation;
		public TitleInk()
		{
			this.InitializeComponent();
            InitApp();
        }
        public void setVideoOperation(VideoOperation vo)
        {
            this._videoOperation = vo;
        }

        private void InitApp()
        {
            Title_InkFrame = _scrollInkFrame._inkFrame;
            //将controlpanel和inkframe关联
            Title_ControlPanel.Height = 20;
            Title_ControlPanel.MinButton.Visibility = Visibility.Collapsed;
            Title_ControlPanel.MaxButton.Visibility = Visibility.Visible;
            Title_InkFrame.rectangle1.Visibility = Visibility.Collapsed;
            Title_ControlPanel.setInkFrame(Title_InkFrame);
            Title_InkFrame._inkCanvas.DefaultDrawingAttributes.Color = Colors.Blue;
            Title_InkFrame._inkCanvas.DefaultDrawingAttributes.Width = 5;
            Title_InkFrame._inkCanvas.DefaultDrawingAttributes.Height = 5;
            _inkCollector = Title_InkFrame.InkCollector;
            loadSketchBook();
        }

        private const int sketchCountPerRow = 6;//每行草图数量
        private const double VerticalInterval = 15;//垂直间距
        private const double HorizontalInterval = 5;//水平间距
        private const double sketchWidth = 78;
        private const double sketchHeight = 80;
        List<Image> images = new List<Image>();
        private void loadSketchBook()
        {
            TextBox tb = new TextBox();
            tb.Text = "Symbol";            
            tb.Margin = new Thickness(10, 10,0,0);
            tb.BorderThickness = new Thickness(0);
            Title_InkFrame._inkCanvas.Children.Add(tb);
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\bad.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\good.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\music.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\pentagram.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\triangle.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\Malesymbol.png"));

            TextBox tb2 = new TextBox();
            tb2.Text = "Motion";
            tb2.Margin = new Thickness(10, 120, 0, 0);
            tb2.BorderThickness = new Thickness(0);
            Title_InkFrame._inkCanvas.Children.Add(tb2);
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\crash.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\localMove.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\move2.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\move1.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\ray.png"));


            TextBox tb3 = new TextBox();
            tb3.Text = "Human";
            tb3.Margin = new Thickness(10, 220, 0, 0);
            tb3.BorderThickness = new Thickness(0);
            Title_InkFrame._inkCanvas.Children.Add(tb3);
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\skiing2.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\skiing1.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\people3.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\people2.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\people1.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\man1.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\Horse_boy_girl.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\girl7.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\girl6.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\girl5.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\girl4.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\girl3.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\girl2.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\girl1.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\Girl_on_horse.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boygirl.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boy7.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boy6.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boy5.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boy4.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boy3.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boy2.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boy1.png"));
            

            //TextBox tb4 = new TextBox();
            //tb4.Text = "Motion";
            //tb4.Margin = new Thickness(10, 10, 0, 0);
            //tb4.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb4);
            

                
            TextBox tb5 = new TextBox();
            tb5.Text = "Animal";
            tb5.Margin = new Thickness(10, 580, 0, 0);
            tb5.BorderThickness = new Thickness(0);
            Title_InkFrame._inkCanvas.Children.Add(tb5);
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse9.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse8.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse7.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse6.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse5.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse4.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse3.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse2.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\horse1.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\hen.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\dog6.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\dog5.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\dog4.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\dog3.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\dog2.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\dog1.png"));
            images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\cloud2.png"));
            


            //TextBox tb6 = new TextBox();
            //tb6.Text = "Bird";
            //tb6.Margin = new Thickness(10, 10, 0, 0);
            //tb6.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb6);
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\bird4.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\bird3.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\bird2.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\bird1.png"));
            
            //TextBox tb7 = new TextBox();
            //tb7.Text = "Boat";
            //tb7.Margin = new Thickness(10, 10, 0, 0);
            //tb7.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb7);
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boat2.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\boat1.png"));
            
            //TextBox tb8 = new TextBox();
            //tb8.Text = "Building";
            //tb8.Margin = new Thickness(10, 10, 0, 0);
            //tb8.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb8);
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\building3.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\building2.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\building1.png"));
            
            //TextBox tb9 = new TextBox();
            //tb9.Text = "Emotion";
            //tb9.Margin = new Thickness(10, 10, 0, 0);
            //tb9.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb9);
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\smilingface.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\question mark.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\Exclamation Mark.png"));
            
            //TextBox tb10 = new TextBox();
            //tb10.Text = "Scene";
            //tb10.Margin = new Thickness(10, 10, 0, 0);
            //tb10.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb10);
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\sun.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\moon.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\cloud1.png"));
            
            //TextBox tb11 = new TextBox();
            //tb11.Text = "Olympic";
            //tb11.Margin = new Thickness(10, 10, 0, 0);
            //tb11.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb11);
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\tower.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\pavilion.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\oly.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\lake.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\hill.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\birdnest.png"));
            
            //TextBox tb12 = new TextBox();
            //tb12.Text = "Flower";
            //tb12.Margin = new Thickness(10, 10, 0, 0);
            //tb12.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb12);
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\flower8.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\flower7.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\flower6.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\flower5.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\flower4.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\flower3.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\flower2.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\flower1.png"));
            
            //TextBox tb13 = new TextBox();
            //tb13.Text = "Other";
            //tb13.Margin = new Thickness(10, 10, 0, 0);
            //tb13.BorderThickness = new Thickness(0);
            //Title_InkFrame._inkCanvas.Children.Add(tb13);
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\whale.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\water.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\tree1.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\table1.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\strawberry.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\road1.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\plane1.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\mountain1.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\car1.png"));
            //images.Add(InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\bag.png"));
            
            
            images[3].MouseDown+=new MouseButtonEventHandler(TitleInk_MouseDown);
            images[3].MouseUp += new MouseButtonEventHandler(TitleInk_MouseUp);
            int count = images.Count;
            int i=0;
            for (i=0;i<6;i++)
            {
                images[i].Width = sketchWidth;
                images[i].Height = sketchHeight;
                
                images[i].VerticalAlignment = VerticalAlignment.Top;
                images[i].HorizontalAlignment = HorizontalAlignment.Left;
                images[i].Margin = new Thickness(i % sketchCountPerRow * sketchWidth + HorizontalInterval,
                    20 + VerticalInterval, 0, 0);
                MyImage newimage = new MyImage(images[i]);
                newimage.Left = images[i].Margin.Left;
                newimage.Top = images[i].Margin.Top;
                InkConstants.AddBound(newimage);
                AddImageCommand cmd = new AddImageCommand(Title_InkFrame.InkCollector, newimage);
                cmd.execute();
                Title_InkFrame.InkCollector.CommandStack.Push(cmd);
            }
            for (i = 6; i < 11; i++)
            {
                images[i].Width = sketchWidth;
                images[i].Height = sketchHeight;

                images[i].VerticalAlignment = VerticalAlignment.Top;
                images[i].HorizontalAlignment = HorizontalAlignment.Left;
                images[i].Margin = new Thickness((i - 6) % sketchCountPerRow * sketchWidth + HorizontalInterval,
                    130 + VerticalInterval, 0, 0);
                MyImage newimage = new MyImage(images[i]);
                newimage.Left = images[i].Margin.Left;
                newimage.Top = images[i].Margin.Top;
                InkConstants.AddBound(newimage);
                AddImageCommand cmd = new AddImageCommand(Title_InkFrame.InkCollector, newimage);
                cmd.execute();
                Title_InkFrame.InkCollector.CommandStack.Push(cmd);
            }
            for (i = 11; i < 34; i++)
            {
                images[i].Width = sketchWidth;
                images[i].Height = sketchHeight;

                images[i].VerticalAlignment = VerticalAlignment.Top;
                images[i].HorizontalAlignment = HorizontalAlignment.Left;
                images[i].Margin = new Thickness((i-11) % sketchCountPerRow * sketchWidth + HorizontalInterval,
                    (i - 11) / sketchCountPerRow * sketchHeight + VerticalInterval + 240, 0, 0);
                MyImage newimage = new MyImage(images[i]);
                newimage.Left = images[i].Margin.Left;
                newimage.Top = images[i].Margin.Top;
                InkConstants.AddBound(newimage);
                AddImageCommand cmd = new AddImageCommand(Title_InkFrame.InkCollector, newimage);
                cmd.execute();
                Title_InkFrame.InkCollector.CommandStack.Push(cmd);
            }
            for (i = 34; i < 51; i++)
            {
                images[i].Width = sketchWidth;
                images[i].Height = sketchHeight;

                images[i].VerticalAlignment = VerticalAlignment.Top;
                images[i].HorizontalAlignment = HorizontalAlignment.Left;
                images[i].Margin = new Thickness((i - 34) % sketchCountPerRow * sketchWidth + HorizontalInterval,
                    (i - 34) / sketchCountPerRow * sketchHeight + VerticalInterval + 580, 0, 0);
                MyImage newimage = new MyImage(images[i]);
                newimage.Left = images[i].Margin.Left;
                newimage.Top = images[i].Margin.Top;
                InkConstants.AddBound(newimage);
                AddImageCommand cmd = new AddImageCommand(Title_InkFrame.InkCollector, newimage);
                cmd.execute();
                Title_InkFrame.InkCollector.CommandStack.Push(cmd);
            }
            //for (i = 0; i < 6; i++)
            //{
            //    images[i].Width = sketchWidth;
            //    images[i].Height = sketchHeight;

            //    images[i].VerticalAlignment = VerticalAlignment.Top;
            //    images[i].HorizontalAlignment = HorizontalAlignment.Left;
            //    images[i].Margin = new Thickness(i % sketchCountPerRow * sketchWidth + HorizontalInterval,
            //        i / sketchCountPerRow * sketchHeight + VerticalInterval, 0, 0);
            //    MyImage newimage = new MyImage(images[i]);
            //    newimage.Left = images[i].Margin.Left;
            //    newimage.Top = images[i].Margin.Top;
            //    InkConstants.AddBound(newimage);
            //    AddImageCommand cmd = new AddImageCommand(Title_InkFrame.InkCollector, newimage);
            //    cmd.execute();
            //    Title_InkFrame.InkCollector.CommandStack.Push(cmd);
            //}
            //for (i = 0; i < 6; i++)
            //{
            //    images[i].Width = sketchWidth;
            //    images[i].Height = sketchHeight;

            //    images[i].VerticalAlignment = VerticalAlignment.Top;
            //    images[i].HorizontalAlignment = HorizontalAlignment.Left;
            //    images[i].Margin = new Thickness(i % sketchCountPerRow * sketchWidth + HorizontalInterval,
            //        i / sketchCountPerRow * sketchHeight + VerticalInterval, 0, 0);
            //    MyImage newimage = new MyImage(images[i]);
            //    newimage.Left = images[i].Margin.Left;
            //    newimage.Top = images[i].Margin.Top;
            //    InkConstants.AddBound(newimage);
            //    AddImageCommand cmd = new AddImageCommand(Title_InkFrame.InkCollector, newimage);
            //    cmd.execute();
            //    Title_InkFrame.InkCollector.CommandStack.Push(cmd);
            //}
            //for (i = 0; i < 6; i++)
            //{
            //    images[i].Width = sketchWidth;
            //    images[i].Height = sketchHeight;

            //    images[i].VerticalAlignment = VerticalAlignment.Top;
            //    images[i].HorizontalAlignment = HorizontalAlignment.Left;
            //    images[i].Margin = new Thickness(i % sketchCountPerRow * sketchWidth + HorizontalInterval,
            //        i / sketchCountPerRow * sketchHeight + VerticalInterval, 0, 0);
            //    MyImage newimage = new MyImage(images[i]);
            //    newimage.Left = images[i].Margin.Left;
            //    newimage.Top = images[i].Margin.Top;
            //    InkConstants.AddBound(newimage);
            //    AddImageCommand cmd = new AddImageCommand(Title_InkFrame.InkCollector, newimage);
            //    cmd.execute();
            //    Title_InkFrame.InkCollector.CommandStack.Push(cmd);
            //}
            //for (i = 0; i < 6; i++)
            //{
            //    images[i].Width = sketchWidth;
            //    images[i].Height = sketchHeight;

            //    images[i].VerticalAlignment = VerticalAlignment.Top;
            //    images[i].HorizontalAlignment = HorizontalAlignment.Left;
            //    images[i].Margin = new Thickness(i % sketchCountPerRow * sketchWidth + HorizontalInterval,
            //        i / sketchCountPerRow * sketchHeight + VerticalInterval, 0, 0);
            //    MyImage newimage = new MyImage(images[i]);
            //    newimage.Left = images[i].Margin.Left;
            //    newimage.Top = images[i].Margin.Top;
            //    InkConstants.AddBound(newimage);
            //    AddImageCommand cmd = new AddImageCommand(Title_InkFrame.InkCollector, newimage);
            //    cmd.execute();
            //    Title_InkFrame.InkCollector.CommandStack.Push(cmd);
            //}
        }

        void TitleInk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Title_InkFrame.InkCollector.Sketch.Images[3].Bound.Visibility = Visibility.Visible;
        }

        void TitleInk_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Title_InkFrame.InkCollector.Sketch.Images[3].Bound.Visibility = Visibility.Collapsed;
            Image image = InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\pentagram.png");
            image.Width = 60;
            image.Height = 60;
            image.Visibility = Visibility.Collapsed;
            image.Margin = new Thickness(_videoOperation._videoAnnotation.Annotation_InkFrame.ActualWidth/2-30,
                _videoOperation._videoAnnotation.Annotation_InkFrame.ActualHeight/2-30, 0, 0);
            MyImage newimage = new MyImage(image);
            newimage.Left = 240;
            newimage.Top = 240;
            InkConstants.AddBound(newimage);
            AddImageCommand cmd = new AddImageCommand(_videoOperation._videoAnnotation.Annotation_InkFrame.InkCollector, newimage);
            cmd.execute();
            _videoOperation._videoAnnotation.Annotation_InkFrame.InkCollector.CommandStack.Push(cmd);

            Image image2 = InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInkResource\sketchBook\pentagramRed.png");
            image2.Width = 60;
            image2.Height = 60;
            image2.Margin = new Thickness(_videoOperation._videoAnnotation.Annotation_InkFrame.ActualWidth / 2 - 30,
                _videoOperation._videoAnnotation.Annotation_InkFrame.ActualHeight / 2 - 30, 0, 0);
            MyImage newimage2 = new MyImage(image2);
            newimage2.Left = 240;
            newimage2.Top = 240;
            InkConstants.AddBound(newimage2);
            AddImageCommand cmd2 = new AddImageCommand(_videoOperation._videoAnnotation.Annotation_InkFrame.InkCollector, newimage2);
            cmd2.execute();
            _videoOperation._videoAnnotation.Annotation_InkFrame.InkCollector.CommandStack.Push(cmd2);
        }        
	}
}