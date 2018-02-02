using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace WPFInk.ShotCut
{
    class ColorHelper
    {
        public static Bitmap ColorToGray(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            int bytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[bytes];

            IntPtr ptr = data.Scan0;
            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int i = 0; i < bytes; i += 3)
            {
                if (rgbValues[i] <= 245 && rgbValues[i + 1] <= 245 && rgbValues[i + 2] <= 245)
                    rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = 0;
                else
                    rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = 255;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(data);
            return bmp;
        }

        static float MAX(float a, float b)
        {
            return a > b ? a : b;
        }
        static float MIN(float a, float b)
        {
            return a < b ? a : b;
        }
        static int MAX(int a, int b)
        {
            return a > b ? a : b;
        }
        static int MIN(int a, int b)
        {
            return a < b ? a : b;
        }
        public static HSV RGB2HSV(float r, float g, float b)
        {
            float max = MAX(r, MAX(g, b)), min = MIN(r, MIN(g, b));
            float delta = max - min;
            float h = 0, s, v;
            v = max;
            if (max != 0.0)
                s = delta / max;
            else
                s = 0.0F;
            if (s == 0.0)
                h = 0;
            else
            {
                if (r == max)
                    h = (g - b) / delta;
                else if (g == max)
                    h = 2 + (b - r) / delta;
                else if (b == max)
                    h = 4 + (r - g) / delta;
                h *= 60.0F;
                if (h < 0) h += 360.0F;
                h /= 360.0F;
            }
            HSV hsv = new HSV(h, s, v);
            return hsv;
        }
        HSV RGB2HSV(int R, int G, int B)
        {
            float dR, dG, dB;
            dR = (float)(0.0 + R) / 255;
            dG = (float)(0.0 + G) / 255;
            dB = (float)(0.0 + B) / 255;
            return RGB2HSV(dR, dG, dB);
        }
        void _HSV2RGB(float h, float s, float v, out float r, out float g, out float b)
        {
            int i;
            float aa, bb, cc, f;
            r = g = b = 0;
            if (s == 0) /* Grayscale */
                r = g = b = v;
            else
            {
                if (h == 1.0) h = 0;
                h *= 6.0F;
                i = (int)Math.Floor(h);
                f = h - i;
                aa = v * (1 - s);
                bb = v * (1 - (s * f));
                cc = v * (1 - (s * (1 - f)));
                switch (i)
                {
                    case 0: r = v; g = cc; b = aa; break;
                    case 1: r = bb; g = v; b = aa; break;
                    case 2: r = aa; g = v; b = cc; break;
                    case 3: r = aa; g = bb; b = v; break;
                    case 4: r = cc; g = aa; b = v; break;
                    case 5: r = v; g = aa; b = bb; break;
                }
            }
        }

        Color HSV2RGB(float h, float s, float v)
        {
            float dR, dG, dB;

            _HSV2RGB(h, s, v, out dR, out dG, out dB);
            Color clr = Color.FromArgb((int)(255 * dR),
                (int)(255 * dG),
                (int)(255 * dB));
            return clr;
        }
    }
}
