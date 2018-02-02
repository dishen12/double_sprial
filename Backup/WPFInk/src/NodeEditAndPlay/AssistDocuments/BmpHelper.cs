using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace WPFInk
{
    class BmpHelper
    {
        public static Bitmap DeleteBlank(Bitmap bmp)
        {
            int left = 0;
            int right = bmp.Width;
            int top = 0;
            int bottom = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            int bytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[bytes];

            IntPtr ptr = data.Scan0;
            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // detect left boundary
            for (int i = 0; i < bmp.Width; i++)
            {
                bool bBlank = true;
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = 3 * (data.Width * j + i);
                    //if (rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255)
                    //    ;
                    //else
                    //{
                    //    bBlank = false;
                    //    break;
                    //}
                    if (!(rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255))
                    {
                        bBlank = false;
                        break;
                    }
                }
                if (true == bBlank)
                    left = i;
                else
                    break;
            }

            // detect right boundary
            for (int i = bmp.Width - 1; i >= 0; i--)
            {
                bool bBlank = true;
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = 3 * (data.Width * j + i);
                    //if (rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255)
                    //    ;
                    //else
                    //{
                    //    bBlank = false;
                    //    break;
                    //}
                    if (!(rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255))
                    {
                        bBlank = false;
                        break;
                    }
                }

                if (true == bBlank)
                    right = i;
                else
                    break;
            }


            // detect top boundary
            for (int j = 0; j < bmp.Height; j++)
            {
                bool bBlank = true;
                for (int i = 0; i < bmp.Width; i++)
                {
                    int index = 3 * (data.Width * j + i);
                    //if (rgbValues[index] > 255 && rgbValues[index + 1] > 255 && rgbValues[index + 2] > 255)
                    //    ;
                    //else
                    //{
                    //    bBlank = false;
                    //    break;
                    //}
                    if (!(rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255))
                    {
                        bBlank = false;
                        break;
                    }
                }

                if (true == bBlank)
                    top = j;
                else
                    break;
            }

            // detect bottom boundary
            for (int j = bmp.Height - 1; j >= 0; j--)
            {
                bool bBlank = true;
                for (int i = 0; i < bmp.Width; i++)
                {
                    int index = 3 * (data.Width * j + i);
                    //if (rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255)
                    //    ;
                    //else
                    //{
                    //    bBlank = false;
                    //    break;
                    //}
                    if (!(rgbValues[index] == 255 && rgbValues[index + 1] == 255 && rgbValues[index + 2] == 255))
                    {
                        bBlank = false;
                        break;
                    }
                }

                if (true == bBlank)
                    bottom = j;
                else
                    break;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(data);

            Bitmap newBmp = new Bitmap(right - left, bottom - top);
            Graphics g = Graphics.FromImage(newBmp);
            g.DrawImage(bmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height), new Rectangle(left, top, right - left, bottom - top), GraphicsUnit.Pixel);
            g.Dispose();
            return newBmp;
        }
    }
}
