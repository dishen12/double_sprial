using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WPFInk
{
    class FileHelper
    {
        public static void SavePanoSketch(List<CJStroke> strokes, string path)
        {
            StreamWriter sw = File.CreateText(path);
            foreach (CJStroke stroke in strokes)
            {
                sw.WriteLine("NewStroke" + "," + stroke.color.ToString());
                foreach (CJPoint pt in stroke)
                {
                    sw.WriteLine("" + pt.X + "," + pt.Y);
                }
            }
            sw.Close();
        }
    }
}
