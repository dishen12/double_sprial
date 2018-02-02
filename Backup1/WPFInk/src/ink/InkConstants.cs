using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Ink;
using WPFInk.tool;

namespace WPFInk.ink
{
    public class InkConstants
    {

        /// <summary>
        /// 返回stream所指定的image
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Image getImageFromStream(Stream stream)
        {
            Image newimage = new Image();
            newimage.Stretch = Stretch.Uniform;
            newimage.RenderTransform = new TransformGroup();

            BitmapImage image = new BitmapImage();
            image.StreamSource = stream;
            newimage.Source = image;

            stream.Close();
            return newimage;
        }


        internal static Image getImageFromName(string s)
        {
            Image newimage = new Image();
            newimage.Stretch = Stretch.Uniform;
            newimage.RenderTransform = new TransformGroup();

            BitmapImage image = new BitmapImage(new Uri(s,UriKind.RelativeOrAbsolute));
            newimage.Source = image;
            newimage.Width = image.Width;
            newimage.Height = image.Height;
            newimage.Margin = new System.Windows.Thickness(0,0,0,0);
            return newimage;
        }

        public static Image getImageFromBitmap(System.Drawing.Bitmap bitmap)
        {
            Image newimage = new Image();
            newimage.Stretch = Stretch.Uniform;
            newimage.RenderTransform = new TransformGroup();

            WriteableBitmap image = new WriteableBitmap(bitmap.Width,bitmap.Height,96,96,PixelFormats.Bgr32,null);
           
            List<System.Drawing.Color> pixels = new List<System.Drawing.Color>();
            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    pixels.Add(bitmap.GetPixel(i, j));

            image.WritePixels(new System.Windows.Int32Rect(0, 0, bitmap.Width, bitmap.Height), pixels.ToArray(), 0, 0);
            newimage.Source = image;
            return newimage;
        }

        //为草图加上红边框
        public static void AddBound(MyImage image)
        {
			if (image.Bound == null)
			{
				Rectangle bound = new Rectangle();
				bound.HorizontalAlignment = HorizontalAlignment.Left;
				bound.VerticalAlignment = VerticalAlignment.Top;
				bound.Margin = new Thickness(image.Left, image.Top, 0, 0);
				bound.Width = image.Width;
				bound.Height = image.Height;
				bound.Stroke = new SolidColorBrush(Colors.Red);
				bound.StrokeThickness = 5;
				if (image.Bound != null)
				{
					bound.RenderTransform = image.Bound.RenderTransform;
				}
				image.Bound = bound;
			}
			else
			{
				image.Bound.Margin = new Thickness(image.Left, image.Top, 0, 0);
				image.Bound.Width = image.Width;
				image.Bound.Height = image.Height;
			}
        }

        //为草图缩略图加上时间段
        public static void AddTextBoxTime(MyButton myButton)
        {
            List<string> hmsStart = ConvertClass.getInstance().MsToHMS(myButton.TimeStart);
            List<string> hmsEnd = ConvertClass.getInstance().MsToHMS(myButton.TimeEnd);
            string TimeString = hmsStart[0] + ":" + hmsStart[1] + ":" + hmsStart[2] + "-" + hmsEnd[0] + ":" + hmsEnd[1] + ":" + hmsEnd[2];
            TextBox textBoxTime = new TextBox();
            textBoxTime.Margin = new System.Windows.Thickness(myButton.Left, myButton.Top - 15, 0, 0);
            textBoxTime.Width = 110;
            textBoxTime.FontSize = 10;
            textBoxTime.Height = 15;
            textBoxTime.Background = null;
            textBoxTime.Text = TimeString;
            textBoxTime.BorderThickness = new System.Windows.Thickness(0);
            
            if (myButton.TextBoxTime!=null)
            {
                textBoxTime.RenderTransform = myButton.TextBoxTime.RenderTransform;
                if (myButton.TextBoxTime.Background != null)
                {
                    textBoxTime.Background = myButton.TextBoxTime.Background;
                }
            }			
            myButton.TextBoxTime = textBoxTime;
        }

		//为草图缩略图加上时间段
		public static void AddTextBoxTime2(MyButton myButton)
		{//colon是冒号的意思

			List<MaskedTextBox> maskedTextBoxList = new List<MaskedTextBox>();
			List<string> hmsStart = ConvertClass.getInstance().MsToHMS(myButton.TimeStart);
			List<string> hmsEnd = ConvertClass.getInstance().MsToHMS(myButton.TimeEnd);
			string TimeString = hmsStart[0] + ":" + hmsStart[1] + ":" + hmsStart[2] + " - " + hmsEnd[0] + ":" + hmsEnd[1] + ":" + hmsEnd[2];

			MaskedTextBox shMaskedTextBoxTime = new MaskedTextBox();
			shMaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left, myButton.Top - 40, 0, 0);
			shMaskedTextBoxTime.Width = 20;
			shMaskedTextBoxTime.Height = 20;
			shMaskedTextBoxTime.Background = null;
			shMaskedTextBoxTime.Text = hmsStart[0];
			shMaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			shMaskedTextBoxTime.InputMask = "II";
			maskedTextBoxList.Add(shMaskedTextBoxTime);

			MaskedTextBox colon1MaskedTextBoxTime = new MaskedTextBox();
			colon1MaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 15, myButton.Top - 40, 0, 0);
			colon1MaskedTextBoxTime.Width = 10;
			colon1MaskedTextBoxTime.Height = 20;
			colon1MaskedTextBoxTime.Background = null;
			//colon1MaskedTextBoxTime.Text = hmsStart[0];
			colon1MaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			colon1MaskedTextBoxTime.InputMask = ":";
			maskedTextBoxList.Add(colon1MaskedTextBoxTime);


			MaskedTextBox smMaskedTextBoxTime = new MaskedTextBox();
			smMaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left+30, myButton.Top - 40, 0, 0);
			smMaskedTextBoxTime.Width = 20;
			smMaskedTextBoxTime.Height = 20;
			smMaskedTextBoxTime.Background = null;
			smMaskedTextBoxTime.Text = hmsStart[1];
			smMaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			smMaskedTextBoxTime.InputMask = "II";
			maskedTextBoxList.Add(smMaskedTextBoxTime);

			MaskedTextBox colon2MaskedTextBoxTime = new MaskedTextBox();
			colon2MaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 50, myButton.Top - 40, 0, 0);
			colon2MaskedTextBoxTime.Width = 5;
			colon2MaskedTextBoxTime.Height = 20;
			colon2MaskedTextBoxTime.Background = null;
			//colon1MaskedTextBoxTime.Text = hmsStart[0];
			colon2MaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			colon2MaskedTextBoxTime.InputMask = ":";
			maskedTextBoxList.Add(colon2MaskedTextBoxTime);

			MaskedTextBox ssMaskedTextBoxTime = new MaskedTextBox();
			ssMaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 47, myButton.Top - 40, 0, 0);
			ssMaskedTextBoxTime.Width = 20;
			ssMaskedTextBoxTime.Height = 20;
			ssMaskedTextBoxTime.Background = null;
			ssMaskedTextBoxTime.Text = hmsStart[2];
			ssMaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			ssMaskedTextBoxTime.InputMask = "II";
			maskedTextBoxList.Add(ssMaskedTextBoxTime);

			MaskedTextBox intervalMaskedTextBoxTime = new MaskedTextBox();
			intervalMaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 62, myButton.Top - 40, 0, 0);
			intervalMaskedTextBoxTime.Width = 20;
			intervalMaskedTextBoxTime.Height = 20;
			intervalMaskedTextBoxTime.Background = null;
			//colon1MaskedTextBoxTime.Text = hmsStart[0];
			intervalMaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			intervalMaskedTextBoxTime.InputMask = " - ";
			maskedTextBoxList.Add(intervalMaskedTextBoxTime);

			MaskedTextBox ehMaskedTextBoxTime = new MaskedTextBox();
			ehMaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 82, myButton.Top - 40, 0, 0);
			ehMaskedTextBoxTime.Width = 20;
			ehMaskedTextBoxTime.Height = 20;
			ehMaskedTextBoxTime.Background = null;
			ehMaskedTextBoxTime.Text = hmsEnd[0];
			ehMaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			ehMaskedTextBoxTime.InputMask = "II";
			maskedTextBoxList.Add(ehMaskedTextBoxTime);

			MaskedTextBox colon3MaskedTextBoxTime = new MaskedTextBox();
			colon3MaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 102, myButton.Top - 40, 0, 0);
			colon3MaskedTextBoxTime.Width = 5;
			colon3MaskedTextBoxTime.Height = 20;
			colon3MaskedTextBoxTime.Background = null;
			//colon1MaskedTextBoxTime.Text = hmsStart[0];
			colon3MaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			colon3MaskedTextBoxTime.InputMask = ":";
			maskedTextBoxList.Add(colon3MaskedTextBoxTime);


			MaskedTextBox emMaskedTextBoxTime = new MaskedTextBox();
			emMaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 104, myButton.Top - 40, 0, 0);
			emMaskedTextBoxTime.Width = 20;
			emMaskedTextBoxTime.Height = 20;
			emMaskedTextBoxTime.Background = null;
			emMaskedTextBoxTime.Text = hmsEnd[1];
			emMaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			emMaskedTextBoxTime.InputMask = "II";
			maskedTextBoxList.Add(emMaskedTextBoxTime);

			MaskedTextBox colon4MaskedTextBoxTime = new MaskedTextBox();
			colon4MaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 124, myButton.Top - 40, 0, 0);
			colon4MaskedTextBoxTime.Width = 5;
			colon4MaskedTextBoxTime.Height = 20;
			colon4MaskedTextBoxTime.Background = null;
			//colon1MaskedTextBoxTime.Text = hmsStart[0];
			colon4MaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			colon4MaskedTextBoxTime.InputMask = ":";
			maskedTextBoxList.Add(colon4MaskedTextBoxTime);

			MaskedTextBox esMaskedTextBoxTime = new MaskedTextBox();
			esMaskedTextBoxTime.Margin = new System.Windows.Thickness(myButton.Left + 126, myButton.Top - 40, 0, 0);
			esMaskedTextBoxTime.Width = 20;
			esMaskedTextBoxTime.Height = 20;
			esMaskedTextBoxTime.Background = null;
			esMaskedTextBoxTime.Text = hmsEnd[2];
			esMaskedTextBoxTime.BorderThickness = new System.Windows.Thickness(0);
			esMaskedTextBoxTime.InputMask = "II";
			maskedTextBoxList.Add(esMaskedTextBoxTime);

			//myButton.MaskedTextBoxList = maskedTextBoxList;
		}

        public static void StrokesTransform(StrokeCollection strokes,double scalingX,double scalingY)
        {
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.Scale(scalingX, scalingY);
            strokes.Transform(m, true);
        }

        public static void UIElementCollectionTransform(UIElementCollection uiec, double scalingX, double scalingY)
        {
            foreach (UIElement uie in uiec)
            {
                ScaleTransform scaleTransform = new ScaleTransform();
                //scaleTransform.SetValue(ScaleTransform.ScaleXProperty,_videoAnnotation.Annotation_InkFrame._inkCanvas.RenderTransform.GetValue(ScaleTransform.ScaleXProperty));
                scaleTransform.ScaleX = scalingX;
                scaleTransform.ScaleY = scalingY;
                uie.RenderTransform = scaleTransform;
            }
        }

        public static void InkCanvasTransform(InkCanvas inkCanvas, double fromScalingX, double fromScalingY, double toScalingX, double toScalingY)
        {
            ScaleTransform scaleTransform = new ScaleTransform(1, 1);
            scaleTransform.CenterX = 0;
            scaleTransform.CenterY = 0;
            inkCanvas.RenderTransform = scaleTransform;

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //宽度
            myDoubleAnimation.From = fromScalingX;
            myDoubleAnimation.To = toScalingX;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));
            Storyboard.SetTarget(myDoubleAnimation, inkCanvas);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)")); 

            DoubleAnimation myDoubleAnimationHeight = new DoubleAnimation();
            //宽度
            myDoubleAnimationHeight.From = fromScalingY;
            myDoubleAnimationHeight.To = toScalingY;
            myDoubleAnimationHeight.Duration = new Duration(TimeSpan.FromSeconds(0));
            Storyboard.SetTarget(myDoubleAnimationHeight, inkCanvas);
            Storyboard.SetTargetProperty(myDoubleAnimationHeight, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)")); 


            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Children.Add(myDoubleAnimationHeight);
            myStoryboard.Begin(inkCanvas);
            
        }

    }
}
