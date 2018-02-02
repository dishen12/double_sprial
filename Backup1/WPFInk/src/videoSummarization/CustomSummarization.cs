using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using WPFInk.tool;
using WPFInk.ink;
using System.Windows;
using System.Windows.Media;

namespace WPFInk.videoSummarization
{
    /// <summary>
    /// 自定义视频摘要轨迹
    /// </summary>
    public class CustomSummarization : VideoSummarization
    {
        #region 构造函数
        public CustomSummarization(Stroke trackStroke, List<KeyFrame> keyFrames, InkCanvas _inkCanvas)
            : base(trackStroke,80, keyFrames,_inkCanvas)
        {
            this.imageHeight = 80;
            addImages2Track();
        }
        #endregion

        #region 私有变量

        private double imageHeight;//每帧高度,同时也是自定义摘要轨迹的宽度
        private const double imageWidth = 120;//每帧宽度
        private const double GradientWidth = 10;//重叠即渐变宽度

        #endregion

        #region 封装变量     
        public double ImageHeight
        {
            get { return imageHeight; }
            set { imageHeight = value; }
        }
        #endregion

        #region 成员函数
        /// <summary>
        /// 获取关键点
        /// </summary>
        /// <param name="intervalLength"></param>
        private void getKeyPoints(int intervalLength)
        {
            StylusPointCollection spc = trackStroke.StylusPoints;
            int firstIndex = 0;
            keyPoints.Add(spc[firstIndex]);
            double distance = 0;
            for (int i = firstIndex; i < spc.Count - 1; i++)
            {
                double distanc2P = MathTool.getInstance().distanceP2P(spc[i + 1], spc[i]);
                distance += distanc2P;
                int offsetDistance = (int)distance - intervalLength;
                if (offsetDistance > 3)//处理两点间隔距离过大的情况，向中间添加点
                {
                    StylusPoint addPoint = MathTool.getInstance().getPointInLine(spc[i + 1], spc[i], distanc2P - offsetDistance);
                    spc.Add(new StylusPoint(0, 0));
                    for (int j = spc.Count - 2; j > i; j--)
                    {
                        spc[j + 1] = new StylusPoint(spc[j].X, spc[j].Y);
                    }
                    spc[i + 1] = new StylusPoint(addPoint.X, addPoint.Y);
                    //drawPoint(spc[i+1].X, spc[i+1].Y,Colors.Green);
                    keyPoints.Add(spc[i + 1]);
                    distance = 0;
                }
                else if (Math.Abs(offsetDistance) <= 3)
                {
                    //drawPoint(spc[i + 1].X, spc[i + 1].Y, Colors.Blue);
                    keyPoints.Add(spc[i + 1]);
                    distance = 0;
                }
            }
        }

        /// <summary>
        /// 向轨道上添加图片
        /// </summary>
        public override void addImages2Track()
        {
            getKeyPoints((int)(imageHeight - GradientWidth));
            for (int i = 0; i < keyPoints.Count - 1 && i < keyFrames.Count; i++)
            {
                //去掉图片多余边界
                RemoveRedundance(i, keyPoints[i].X - keyFrames[i].Image.Width / 2, keyPoints[i].Y - keyFrames[i].Image.Height / 2);

                //添加图片
                addPicture(keyPoints[i].X - keyFrames[i].Image.Width / 2, keyPoints[i].Y - keyFrames[i].Image.Height / 2, i);                
            }
        }
        /// <summary>
        /// 去除图片的多余边界
        /// </summary>
        /// <param name="index"></param>
        private void RemoveRedundance(int index, double left, double top)
        {
            System.Drawing.Bitmap bitmapSource = ConvertClass.BitmapImageToBitMap((BitmapImage)keyFrames[index].Image.Source);
            System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height);
            StylusPointCollection points = trackStroke.StylusPoints;
            double pointWidth = keyFrames[index].Image.Width / bitmapSource.Width;
            unsafe
            {
                if (index == 0)
                {
                    for (int i = 0; i < bitmapSource.Width; i++)
                    {
                        for (int j = 0; j < bitmapSource.Height; j++)
                        {
                            System.Drawing.Color pixelColor = bitmapSource.GetPixel(i, j);
                            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(255, pixelColor.R, pixelColor.G, pixelColor.B);
                            StylusPoint currPoint = new StylusPoint(left + i * pointWidth, top + j * pointWidth);
                            int preIndexInStroke = points.IndexOf(keyPoints[index > 0 ? index - 1 : 0]);
                            int nextIndexInStroke = trackStroke.StylusPoints.IndexOf(keyPoints[index < keyPoints.Count - 1 ? index + 1 : keyPoints.Count - 1]);
                            double distance = distanceP2Points(currPoint, points, preIndexInStroke, nextIndexInStroke);
                            if (distance > imageHeight / 2)
                            {
                                newColor = System.Drawing.Color.FromArgb(0, pixelColor.R, pixelColor.G, pixelColor.B);
                            }
                            bitmapTarget.SetPixel(i, j, newColor);
                        }
                    }
                }
                else
                {
                    //去除无用边界
                    int preIndexInStroke = points.IndexOf(keyPoints[index > 0 ? index - 1 : 0]);
                    int nextIndexInStroke = trackStroke.StylusPoints.IndexOf(keyPoints[index < keyPoints.Count - 1 ? index + 1 : keyPoints.Count - 1]);

                    StylusPoint middleKeyPoint = new StylusPoint(points[(preIndexInStroke + nextIndexInStroke) / 2].X, points[(preIndexInStroke + nextIndexInStroke) / 2].Y);
                    double maxDistanceCurr2MiddleKP = 0;
                    for (int i = 0; i < bitmapSource.Width; i++)
                    {
                        for (int j = 0; j < bitmapSource.Height; j++)
                        {
                            System.Drawing.Color pixelColor = bitmapSource.GetPixel(i, j);
                            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(255, pixelColor.R, pixelColor.G, pixelColor.B);
                            StylusPoint currPoint = new StylusPoint(left + i * pointWidth, top + j * pointWidth);
                            double distance = distanceP2Points(currPoint, points, preIndexInStroke, nextIndexInStroke);
                            if (distance > imageHeight / 2)
                            {
                                newColor = System.Drawing.Color.FromArgb(0, pixelColor.R, pixelColor.G, pixelColor.B);
                            }
                            else
                            {
                                double DistanceCurr2PreKP = MathTool.getInstance().distanceP2P(currPoint, keyPoints[index - 1]);
                                double DistanceCurr2CurrKP = MathTool.getInstance().distanceP2P(currPoint, keyPoints[index]);
                                if (DistanceCurr2PreKP < DistanceCurr2CurrKP)
                                {
                                    double DistanceCurr2MiddleKP = MathTool.getInstance().distanceP2P(currPoint, middleKeyPoint);
                                    if (DistanceCurr2MiddleKP > maxDistanceCurr2MiddleKP)
                                    {
                                        maxDistanceCurr2MiddleKP = DistanceCurr2MiddleKP;
                                    }
                                }
                            }

                            bitmapTarget.SetPixel(i, j, newColor);
                        }
                    }
                    //添加渐变效果
                    for (int i = 0; i < bitmapSource.Width; i++)
                    {
                        for (int j = 0; j < bitmapSource.Height; j++)
                        {
                            System.Drawing.Color pixelColor = bitmapTarget.GetPixel(i, j);
                            if (pixelColor.A != 0)
                            {
                                System.Drawing.Color newColor = System.Drawing.Color.FromArgb(255, pixelColor.R, pixelColor.G, pixelColor.B);
                                StylusPoint currPoint = new StylusPoint(left + i * pointWidth, top + j * pointWidth);
                                double DistanceCurr2PreKP = MathTool.getInstance().distanceP2P(currPoint, keyPoints[index - 1]);
                                double DistanceCurr2CurrKP = MathTool.getInstance().distanceP2P(currPoint, keyPoints[index]);
                                double distance = distanceP2Points(currPoint, points, preIndexInStroke, nextIndexInStroke);

                                //为了衔接效果好，没有在前一副图片的点，不修改透明度

                                bool isInPreImage=MathTool.getInstance().isCloseRectangle(new Point(currPoint.X, currPoint.Y),
                                    MathTool.getInstance().RectToRectangle(new Rect(keyFrames[index-1].Image.Margin.Left,keyFrames[index-1].Image.Margin.Top, keyFrames[index-1].Image.Width, keyFrames[index-1].Image.Height)), 0);

                                if (isInPreImage&&maxDistanceCurr2MiddleKP != 0 && DistanceCurr2PreKP < DistanceCurr2CurrKP)
                                {
                                    double DistanceCurr2MiddleKP = MathTool.getInstance().distanceP2P(currPoint, middleKeyPoint);
                                    double rate = 1 - DistanceCurr2MiddleKP / maxDistanceCurr2MiddleKP;
                                    newColor = System.Drawing.Color.FromArgb((byte)(255 * rate), pixelColor.R, pixelColor.G, pixelColor.B);

                                }
                                //if (distance <= imageHeight / 2 && distance >= imageHeight / 3)
                                //{
                                //    newColor = System.Drawing.Color.FromArgb(255, pixelColor.R, pixelColor.G, pixelColor.B);
                                //}
                                bitmapTarget.SetPixel(i, j, newColor);
                            }
                        }
                    }

                }
                MemoryStream ms = new MemoryStream();
                bitmapTarget.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
                bitmapImage.EndInit();
                keyFrames[index].Image.Source = bitmapImage;
            }
        }
        /// <summary>
        /// 计算点到点集的最短距离
        /// </summary>
        /// <param name="p"></param>
        /// <param name="points"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private double distanceP2Points(StylusPoint p, StylusPointCollection points, int startIndex, int endIndex)
        {
            double min = MathTool.getInstance().distanceP2P(points[0], p);
            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                double distance = MathTool.getInstance().distanceP2P(points[i], p);
                if (distance < min)
                {
                    min = distance;
                }
            }
            return min;
        }
        /// <summary>
        /// 向画板中添加图片
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="index"></param>
        private void addPicture(double x, double y, int index)
        {
            keyFrames[index].Image.Margin = new Thickness(x, y, 0, 0);
            if (InkCanvas.Children.IndexOf(keyFrames[index].Image) == -1)
            {
                InkCanvas.Children.Add(keyFrames[index].Image);
            }
        }
        /// <summary>
        /// 从自定义摘要获取选择的关键帧
        /// </summary>
        /// <param name="point"></param>
        public override int getSelectedKeyFrameIndex(Point point)//, CustomSummarization customSummarization)
        {
            double minDistance = double.MaxValue;
            int minIndex = int.MinValue;
            if (this != null)
            {
                for (int i = 0; i < Math.Min(this.KeyFrames.Count, this.KeyPoints.Count); i++)
                {
                    double distance = MathTool.getInstance().distanceP2P(point, new Point(this.KeyPoints[i].X, this.KeyPoints[i].Y));

                    if (distance < this.ImageHeight / 2 && distance < minDistance)
                    {
                        minDistance = distance;
                        minIndex = i;
                    }
                }
            }
            return minIndex;
        }
        /// <summary>
        /// 向轨迹要上添加圆点代表播放到该关键帧,表明播放进度
        /// </summary>
        /// <param name="index">对应关键帧索引</param>
        /// <param name="color">笔迹颜色</param>
        /// <param name="strokeWidthOffset">相对应螺旋线笔迹的宽度差异,默认为0</param>
        public override Stroke AddPoint2Track(int index, Color color, double strokeWidthOffset)
        {
            //Stroke stroke;
            ////关键帧对应关键点在螺旋笔迹中的索引
            //int startPointIndexInSpiral = _inkCollector.SpiralSummarization.KeyPointIndexes[index];
            ////添加蓝色笔迹代表该关键帧有注释
            //StylusPointCollection spc = new StylusPointCollection();
            //int endPointIndexInSpiral = _inkCollector.SpiralSummarization.KeyPointIndexes[index + 1];
            //int midIndex = (startPointIndexInSpiral + endPointIndexInSpiral) / 2;
            //spc.Add(new StylusPoint(points[midIndex].X, points[midIndex].Y));
            //if (spc.Count > 0)
            //{
            //    stroke = new Stroke(spc);
            //    stroke.DrawingAttributes.Color = color;
            //    stroke.DrawingAttributes.Height = stroke.DrawingAttributes.Width = mySpiralStrokeWidth + strokeWidthOffset;
            //    ((InkCanvas)(_inkCollector._mainPage._inkCanvas.Children[0])).Strokes.Add(stroke);
            //    return stroke;
            //}
            return null;
        }
        #endregion
    }
}
