using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WPFInk.tool;
using System.IO;
using WPFInk.Global;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFInk.videoSummarization
{
    public class GernarateKeyFrame
    {
        private List<KeyFrame> keyFrames = new List<KeyFrame>();
        private SpiralSummarization spiralSummarization;
        private MySpiral mySpiral;
        private StylusPointCollection points;
        private int intervalLength;
        private List<int> endIndexes = new List<int>();
        public GernarateKeyFrame(List<KeyFrame> keyFrames,SpiralSummarization spiralSummarization)
        {
            this.keyFrames = keyFrames;
            this.spiralSummarization = spiralSummarization;
            this.mySpiral = spiralSummarization.MySpiral;
            this.points = mySpiral.SpiralStroke.StylusPoints;
            this.intervalLength = (int)(mySpiral.SpiralWidth * 1.1);
        }
        public void saveKeyFrames()
        {
            getKeyPoints();
            getAllBounds();
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    string savePath = GlobalValues.FilesPath + @"\WPFInk\WPFInk\cache\keyFrames\" + i + "_" + j + ".png";
                    //WriteableBitmap wb= spiralSummarization.InsertImage(i, keyFrames[i], j, endIndexes[j]);
                    //try
                    //{
                       // ImageTool.getInstance().SaveImage(savePath,(BitmapSource)wb);
                    //}
                    //catch
                    //{
                    //    Console.WriteLine("保存发生错误");
                    //}
                }
            }

            //test           
            //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(@"E:\task\WPFInk材料\螺旋摘要录制\顺时针螺旋摘要.png");
            //bitmap.Save(@"E:\task\WPFInk材料\螺旋摘要录制\test.png", System.Drawing.Imaging.ImageFormat.Png);
            //bitmap.Dispose();
        }

        private void getAllBounds()
        {
            List<Rect> bounds = new List<Rect>();
            for (int j = 0; j < points.Count-300; j++)
            {
               
                Rect bound = spiralSummarization.getBound(j, endIndexes[j]);
                spiralSummarization.KeyFrameCenterPoints.Add(new System.Windows.Point(bound.X + bound.Width / 2, bound.Y + bound.Height / 2));
                bounds.Add(bound);

                //spiralSummarization.InkCanvas.Children.Add(ConvertClass.getInstance().RectToRectangle2(bound));


            }
            
            spiralSummarization.bounds = bounds;
        }
        private void getKeyPoints()
        {
            double arcLength = 0;
            for (int i = 0; i < 1; i++)
            {
                if (i < intervalLength * 5)
                {
                    arcLength = intervalLength * 3;
                }
                else if (i < intervalLength * 10)
                {
                    arcLength = intervalLength * 1.3;
                }
                else
                {
                    arcLength = intervalLength;

                }
                for (int j = 0; j < 10; j++)
                {
                    double distance = 0;
                    int endIndex = 0;
                    for (int k = j; k < points.Count - 1; k++)
                    {
                        distance += MathTool.getInstance().distanceP2P(points[k + 1], points[k]);

                        if ((int)distance >= arcLength)
                        {
                            endIndex = k + 1;
                            endIndexes.Add(endIndex);
                            distance = 0;
                            break;
                        }
                    }
                }
            }
        }
    }
}
