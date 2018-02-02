using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using WPFInk.videoSummarization;

namespace WPFInk.tool
{
    public class ImageTool
    {
        private static ImageTool singleTon = null;
        private ImageTool()
        {

        }

        public static ImageTool  getInstance()
        {
            if(singleTon==null)
            {
                singleTon=new ImageTool();
            }
            return singleTon;
        }
        public Image newImage(string imageUri)
        {
            Image newimage = new Image();
            newimage.Stretch = Stretch.Uniform;
            newimage.RenderTransform = new TransformGroup();

            BitmapImage image = new BitmapImage(new Uri("file:///" + imageUri, UriKind.RelativeOrAbsolute));
            newimage.Source = image;
            newimage.Width = image.Width;
            newimage.Height = image.Height;
            newimage.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            newimage.VerticalAlignment = VerticalAlignment.Top;
            return newimage;
        }

        public BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmapSource)
        {
            MemoryStream ms = new MemoryStream();
            bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();
            return bitmapImage;
        }
        /// <summary>
        /// byte[]转换为BitmapImage
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public BitmapImage ByteArrayToBitmapImage(byte[] byteArray) 
         { 
             BitmapImage bmp = null; 
         
            try 
             { 
                 bmp = new BitmapImage(); 
                 bmp.BeginInit(); 
                 bmp.StreamSource = new MemoryStream(byteArray); 
                 bmp.EndInit(); 
             } 
             catch 
             { 
                 bmp = null; 
             } 
         
            return bmp; 
         }
 
 
 
　　    /// <summary>
            /// BitmapImage转换为byte[]
　　    /// </summary>
　　    /// <param name="bmp"></param>
　　    /// <returns></returns>
 
        public byte[] BitmapImageToByteArray(BitmapImage bmp) 
         { 
             byte[] byteArray = null; 
         
            try 
             { 
                 Stream sMarket = bmp.StreamSource; 
         
                if (sMarket != null && sMarket.Length > 0) 
                 { 
                     //很重要，因为Position经常位于Stream的末尾，导致下面读取到的长度为0。 
                     sMarket.Position = 0; 
         
                    using (BinaryReader br = new BinaryReader(sMarket)) 
                     { 
                         byteArray = br.ReadBytes((int)sMarket.Length); 
                     } 
                 } 
             } 
             catch 
             { 
                 //other exception handling 
             } 
         
            return byteArray; 
            }
        /// <summary>
        /// 保存System.Windows.Controls.Image
        /// </summary>
        /// <param name="myImage"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public void saveImage(System.Windows.Controls.Image myImage, string savePath)
        {
            System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            bitmapImage = ((System.Windows.Media.Imaging.BitmapImage)myImage.Source);
            System.Windows.Media.Imaging.PngBitmapEncoder pngBitmapEncoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            System.IO.FileStream stream = new System.IO.FileStream(savePath, FileMode.Create);

            pngBitmapEncoder.Interlace = PngInterlaceOption.On;
            pngBitmapEncoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmapImage));
            pngBitmapEncoder.Save(stream);
            stream.Flush();
            stream.Close();
        }

        public void SaveImage(string file, BitmapSource img)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            //BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Interlace = PngInterlaceOption.On;
            encoder.Frames.Add(BitmapFrame.Create(img));

            FileStream bitmap = new FileStream(file, FileMode.Create, FileAccess.Write);
            encoder.Save(bitmap);
            bitmap.Close();
        }

        public BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap bitmap) 
        { 
            BitmapSource destination; 
            IntPtr hBitmap = bitmap.GetHbitmap(); 
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions(); 
            destination = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            destination.Freeze(); 
            return destination; 
        }

        public System.Drawing.Bitmap BitmapSourceToBitmap(BitmapSource bitmapsource) 
        {     
            System.Drawing.Bitmap bitmap;    
            using (MemoryStream outStream = new MemoryStream())     
            {        
                // from System.Media.BitmapImage to System.Drawing.Bitmap   
                
                BitmapEncoder enc = new BmpBitmapEncoder();       
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));        
                enc.Save(outStream);        
                bitmap = new System.Drawing.Bitmap(outStream); 
            }    
            return bitmap;
        }

        public System.Drawing.Bitmap BitmapImageToBitMap(BitmapImage bitmapImage)
        {
            var ms = new MemoryStream();
            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(ms);
            var bmp = new System.Drawing.Bitmap(ms);
            return bmp;
        }
        /// <summary>
        /// 彩色关键帧转黑白关键帧
        /// </summary>
        /// <param name="image"></param>
        public BitmapSource convertToGray(KeyFrame keyFrame)
        {
            const int InitTemplateWidth = 300;//初始模板宽度
            const int InitTemplateHeight = 200;//初始模板高度
            const int InitTemplateWidthAndHeight = InitTemplateWidth * InitTemplateHeight;
            const int ArgbValuesLength = InitTemplateWidthAndHeight * 4;
            const int InitTemplateWidth4 = InitTemplateWidth * 4;
            byte[] ArgbValues = new byte[ArgbValuesLength];
            WriteableBitmap bitmapTarget;
            Int32Rect sourceRect;
            int PointIndexInSpiral = keyFrame.PointIndexInSpiral;
            keyFrame.BitmapSource.CopyPixels(ArgbValues, InitTemplateWidth4, 0);
            int indexBmp;
            for (int i = 0; i < InitTemplateWidth; i++)
            {
                for (int j = 0; j < InitTemplateHeight; j++)
                {
                    indexBmp = 4 * (InitTemplateWidth * j + i);
                    //Console.WriteLine("keyFrame.PointIndexInSpiral:" + PointIndexInSpiral);
                    byte a=Global.GlobalValues.templates[PointIndexInSpiral][i * InitTemplateHeight + j];
                    ArgbValues[indexBmp + 3] = a;
                    float gray = (float)(ArgbValues[indexBmp] * 0.299 + ArgbValues[indexBmp + 1] * 0.587 + ArgbValues[indexBmp + 2] * 0.114);
                    ArgbValues[indexBmp] = ArgbValues[indexBmp + 1] = ArgbValues[indexBmp + 2] = (byte)gray;

                }
            }
            bitmapTarget = new WriteableBitmap(InitTemplateWidth, InitTemplateHeight,
                keyFrame.BitmapSource.DpiX, keyFrame.BitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            sourceRect = new Int32Rect(0, 0, InitTemplateWidth, InitTemplateHeight);
            bitmapTarget.WritePixels(sourceRect, ArgbValues, InitTemplateWidth4, 0);
            //keyFrame.Image.Source = (BitmapSource)bitmapTarget;
            return (BitmapSource)bitmapTarget;
        }

    }
}
