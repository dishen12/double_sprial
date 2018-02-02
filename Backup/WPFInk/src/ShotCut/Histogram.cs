using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace WPFInk.ShotCut
{
    class Histogram
    {
        static int BIN_NUMBER = 4;


        public static int[] CalHSVHis(Bitmap bmp)
        {
            int[] histMap = new int[BIN_NUMBER * BIN_NUMBER * BIN_NUMBER];
            for (int i = 0; i < BIN_NUMBER * BIN_NUMBER * BIN_NUMBER; i++)
            {
                histMap[i] = 0;
            }

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            int bytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[bytes];

            IntPtr ptr = data.Scan0;
            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                {
                    int index = 3 * (data.Width * j + i);
                    HSV hsv = ColorHelper.RGB2HSV(rgbValues[index], rgbValues[index + 1], rgbValues[index + 2]);

                    int indexH = (int)(hsv.h * BIN_NUMBER);
                    if (indexH >= BIN_NUMBER)
                        indexH = BIN_NUMBER - 1;
                    int indexS = (int)(hsv.s * BIN_NUMBER);
                    if (indexS >= BIN_NUMBER)
                        indexS = BIN_NUMBER - 1;
                    int indexV = (int)(hsv.v * BIN_NUMBER);
                    if (indexV >= BIN_NUMBER)
                        indexV = BIN_NUMBER - 1;

                    histMap[indexH * BIN_NUMBER * BIN_NUMBER + indexS * BIN_NUMBER + indexV]++;
                }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(data);
            return histMap;
        }


        public static double CalHisDiff(int[] histMap1, int[] histMap2)
        {
            double sum = 0;
            for (int i = 0; i < BIN_NUMBER; i++)
                for (int j = 0; j < BIN_NUMBER; j++)
                    for (int k = 0; k < BIN_NUMBER; k++)
                    {
                        sum += Math.Abs(histMap1[i * BIN_NUMBER * BIN_NUMBER + j * BIN_NUMBER + k] - histMap2[i * BIN_NUMBER * BIN_NUMBER + j * BIN_NUMBER + k]);
                    }
            return sum;
        }
    }
}
