using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows;
using System.IO;
using WPFInk.Global;

namespace WPFInk.tool
{
    public class ConvertClass
    {
        private static ConvertClass convert = null;

        private ConvertClass()
        {}

        public static ConvertClass getInstance()
        {
            if (convert == null)
                convert = new ConvertClass();
            return convert;
        }

        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            Bitmap bitmapSource = new Bitmap(bitmap.Width,bitmap.Height);
            int i,j;
            for(i=0;i<bitmap.Width;i++)
              for (j = 0; j < bitmap.Height; j++)
              {
                  Color pixelColor = bitmap.GetPixel(i, j);
                  Color newColor = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
                  bitmapSource.SetPixel(i, j, newColor);
              }
            MemoryStream ms = new MemoryStream();
            bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();

            //var imageStreamSource = bitmapImage;
            //var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            //var bitmapFrame = decoder.Frames[0]; 
            //var encoder = new JpegBitmapEncoder();
            //encoder.Frames.Add(bitmapFrame);
            //encoder.Save(File.Create(@GlobalValues.FilesPath+"\out1.jpg")); 

            return bitmapImage;
        }
        public static System.Drawing.Bitmap BitmapImageToBitMap(BitmapImage bitmapImage)
        {
            var ms = new MemoryStream();
            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(ms);
            var bmp = new System.Drawing.Bitmap(ms);
            return bmp;

        }

        //毫秒转化为小时：分钟：秒的形式
        public List<string> MsToHMS(double ms)
        {
            List<string> hms = new List<string>();
            int sTotal = (int)(ms / 1000); //转化为秒
            int mTotal=sTotal/60;//转化为分
            int h = sTotal / 3600;//算出小时
            int m = mTotal - h * 60;
            int s = sTotal - m * 60 - h * 60;

			if (h < 10)
			{
				hms.Add("0" + h.ToString());
			}
			else
			{
				hms.Add(h.ToString());
			}
			if (m < 10)
			{
				hms.Add("0" + m.ToString());
			}
			else
			{
				hms.Add(m.ToString());
			}
			if (s < 10)
			{
				hms.Add("0" + s.ToString());
			}
			else
			{
				hms.Add(s.ToString());
			}
            return hms;
        }

        public System.Drawing.Rectangle RectToRectangle(Rect value) 
        { 
            System.Drawing.Rectangle result = new System.Drawing.Rectangle(); 
            result.X = (int)value.X; 
            result.Y = (int)value.Y; 
            result.Width = (int)value.Width; 
            result.Height = (int)value.Height;
            return result; 
        }
        public System.Windows.Shapes.Rectangle RectToRectangle2(Rect value)
        {
            System.Windows.Shapes.Rectangle bound = new System.Windows.Shapes.Rectangle();
            bound.HorizontalAlignment = HorizontalAlignment.Left;
            bound.VerticalAlignment = VerticalAlignment.Top;
            bound.Margin = new Thickness(value.X, value.Y, 0, 0);
            bound.Width = value.Width;
            bound.Height = value.Height;
            bound.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            bound.StrokeThickness = 2;
            return bound;
        }
        public Rect RectangleToRect(System.Drawing.Rectangle value) 
        { 
            Rect result = new Rect(); 
            result.X = value.X; 
            result.Y = value.Y; 
            result.Width = value.Width; 
            result.Height = value.Height; 
            return result; 
        }
        /// <summary>
        /// 复制Bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap CopyBitmap(Bitmap bitmap)
        {
            Bitmap bitmapSource = new Bitmap(bitmap.Width, bitmap.Height);
            int i, j;
            for (i = 0; i < bitmap.Width; i++)
                for (j = 0; j < bitmap.Height; j++)
                {
                    Color pixelColor = bitmap.GetPixel(i, j);
                    Color newColor = Color.FromArgb(pixelColor.A,pixelColor.R, pixelColor.G, pixelColor.B);
                    bitmapSource.SetPixel(i, j, newColor);
                    pixelColor = bitmapSource.GetPixel(i, j);
                    Console.Write("aa");
                }
            return bitmapSource;
        }

        //public static System.Drawing.Image ControlImage2DrawingImage(System.Windows.Controls.Image image)
        //{
        //    System.Drawing.Bitmap bitmap = null;
        //    var width = bitmapImage.PixelWidth;        
        //    var height = bitmapImage.PixelHeight;        
        //    var stride = width * ((bitmapImage.Format.BitsPerPixel + 7) / 8);        
        //    var bits = new byte[height * stride];        
        //    bitmapImage.CopyPixels(bits, stride, 0);        
        //    unsafe        
        //    {            
        //        fixed (byte* pB = bits)            
        //        {                
        //            var ptr = new IntPtr(pB);                
        //            bitmap = new System.Drawing.Bitmap(width, height, stride,
        //                System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
        //                ptr);
        //        }        
        //    }        
        //    return Icon.FromHandle(bitmap.GetHicon());    
        //}

    }
}
