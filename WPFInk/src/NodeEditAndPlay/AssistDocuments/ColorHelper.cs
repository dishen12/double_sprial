using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFInk
{
    class HSVColorHelper
    {
        public int H, S, V;
        public int R, G, B;
        public int C, M, Y;// K;
        /**/
        /// <summary>
        /// 处理Rgb到Hsv的转换
        /// </summary>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        /// 
        public void RgbToHsv(int R, int G, int B)
        {
            Color MyColor = Color.FromArgb(R, G, B);
            H = Convert.ToInt32(MyColor.GetHue());

            //奇怪——用微软自己的方法获得的S值和V值居然不对
            //S=Convert.ToInt32(MyColor.GetSaturation()/255*100);
            //V=Convert.ToInt32(MyColor.GetBrightness()/255*100);

            decimal min;
            decimal max;
            decimal delta;

            decimal R1 = Convert.ToDecimal(R) / 255;
            decimal G1 = Convert.ToDecimal(G) / 255;
            decimal B1 = Convert.ToDecimal(B) / 255;

            min = Math.Min(Math.Min(R1, G1), B1);
            max = Math.Max(Math.Max(R1, G1), B1);
            V = Convert.ToInt32(max * 100);
            delta = (max - min) * 100;

            if (max == 0 || delta == 0)
                S = 0;
            else
                S = Convert.ToInt32(delta / max);

        }


        public void RgbToHsv(Color MyColor)
        {
            int R = MyColor.R;
            int G = MyColor.G;
            int B = MyColor.B;
            H = Convert.ToInt32(MyColor.GetHue());

            //奇怪——用微软自己的方法获得的S值和V值居然不对
            //S=Convert.ToInt32(MyColor.GetSaturation()/255*100);
            //V=Convert.ToInt32(MyColor.GetBrightness()/255*100);

            decimal min;
            decimal max;
            decimal delta;

            decimal R1 = Convert.ToDecimal(R) / 255;
            decimal G1 = Convert.ToDecimal(G) / 255;
            decimal B1 = Convert.ToDecimal(B) / 255;

            min = Math.Min(Math.Min(R1, G1), B1);
            max = Math.Max(Math.Max(R1, G1), B1);
            V = Convert.ToInt32(max * 100);
            delta = (max - min) * 100;

            if (max == 0 || delta == 0)
                S = 0;
            else
                S = Convert.ToInt32(delta / max);

        }

        /**/
        /// <summary>
        /// 处理Hsv到Rgb的转换
        /// </summary>
        /// <param name="H"></param>
        /// <param name="S"></param>
        /// <param name="V"></param>
        public void HsvToRgb(int H, int S, int V)
        {

            H = Convert.ToInt32(Convert.ToDecimal(H) / 360 * 255);
            S = Convert.ToInt32(Convert.ToDecimal(S) / 100 * 255);
            V = Convert.ToInt32(Convert.ToDecimal(V) / 100 * 255);

            if (S == 0)
            {
                R = 0;
                G = 0;
                B = 0;
            }

            decimal fractionalSector;
            decimal sectorNumber;
            decimal sectorPos;
            sectorPos = (Convert.ToDecimal(H) / 255 * 360) / 60;
            sectorNumber = Convert.ToInt32(Math.Floor(Convert.ToDouble(sectorPos)));
            fractionalSector = sectorPos - sectorNumber;

            decimal p;
            decimal q;
            decimal t;

            decimal r = 0;
            decimal g = 0;
            decimal b = 0;
            decimal ss = Convert.ToDecimal(S) / 255;
            decimal vv = Convert.ToDecimal(V) / 255;


            p = vv * (1 - ss);
            q = vv * (1 - (ss * fractionalSector));
            t = vv * (1 - (ss * (1 - fractionalSector)));

            switch (Convert.ToInt32(sectorNumber))
            {
                case 0:

                    r = vv;
                    g = t;
                    b = p;
                    break;

                case 1:
                    r = q;
                    g = vv;
                    b = p;
                    break;
                case 2:

                    r = p;
                    g = vv;
                    b = t;
                    break;
                case 3:

                    r = p;
                    g = q;
                    b = vv;
                    break;
                case 4:

                    r = t;
                    g = p;
                    b = vv;
                    break;
                case 5:

                    r = vv;
                    g = p;
                    b = q;
                    break;
            }
            R = Convert.ToInt32(r * 255);
            G = Convert.ToInt32(g * 255);
            B = Convert.ToInt32(b * 255);

        }
        /**/
        /// <summary>
        /// RgbtoCmy
        /// </summary>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        public void RgbToCmyk(decimal R, decimal G, decimal B)
        {
            C = Convert.ToInt32((255 - Math.Abs(R)) * 100 / 255);

            M = Convert.ToInt32((255 - Math.Abs(G)) * 100 / 255);

            Y = Convert.ToInt32((255 - Math.Abs(B)) * 100 / 255);
        }
    }
}
