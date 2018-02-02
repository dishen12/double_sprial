using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Drawing.Imaging;
using System.Windows;
using System.IO;
using WPFInk.tool;
using WPFInk.ink;

namespace WPFInk.videoSummarization
{
    /// <summary>
    /// 螺旋视频摘要类
    /// </summary>
    public class SpiralSummarization:VideoSummarization
    {
        #region 私有变量
        private MySpiral mySpiral;//螺旋线
        private StylusPoint center;//螺旋线的中心点
        private List<Point> keyFrameCenterPoints = new List<Point>();//关键帧中心点列表
        private System.Windows.Forms.Timer Timer;
        private StylusPointCollection points;//螺旋线点集合

        #endregion

        #region 构造函数        
        public SpiralSummarization(MySpiral mySpiral, List<KeyFrame> keyFrames)
            :base(mySpiral.SpiralStroke,mySpiral.SpiralWidth,keyFrames,mySpiral.InkCanvas)
        {
            this.mySpiral = mySpiral;
            this.center = mySpiral.Center;
            this.points = mySpiral.SpiralStroke.StylusPoints;
            getKeyPoints((int)mySpiral.SpiralWidth);
            addImages2Spiral();
            //Timer = new System.Windows.Forms.Timer();
            //Timer.Interval = 1;
            //Timer.Tick += new System.EventHandler(Timer_Tick);
            //Timer.Start();
            for (int i = 5; i < keyFrames.Count - 1; i++)
            {                
                MyStoryboard.KeyFrameInSpiralMove(keyFrames[i], keyFrames[i+1], 5);
            }
       }
        int keyFrameIndex=0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (keyFrameIndex < keyFrames.Count)
            {
                addImage2Spiral(keyFrameIndex, keyFrames[keyFrameIndex], true);
                keyFrameIndex++;
            }
            else
            {
                Timer.Stop();
            }
        }
        #endregion

        #region 封装变量
        /// <summary>
        /// 螺旋线
        /// </summary>
        public MySpiral MySpiral
        {
            get { return mySpiral; }
            set { mySpiral = value; }
        }
        /// <summary>
        /// 关键帧中心点列表
        /// </summary>
        public List<Point> KeyFrameCenterPoints
        {
            get { return keyFrameCenterPoints; }
            set { keyFrameCenterPoints = value; }
        }

        #endregion

        #region 普通函数       

        /// <summary>
        /// 获取关键点
        /// </summary>
        /// <param name="intervalLength"></param>
        private void getKeyPoints(int intervalLength)
        {
            keyPointIndexes.Clear();
            int firstIndex = (int)mySpiral.SpiralWidth * 2 + 2;
            //drawPoint(points[firstIndex].X, points[firstIndex].Y, Colors.Blue);
            keyPointIndexes.Add(firstIndex);
            keyPointIndexes.Add(firstIndex);
            int secondIndex = firstIndex + (int)mySpiral.SpiralWidth;
            keyPointIndexes.Add(secondIndex);
            //drawPoint(points[secondIndex].X, points[secondIndex].Y, Colors.Blue);
            double distance = 0;
            for (int i = secondIndex; i < points.Count - 1; i++)
            {
                distance += MathTool.getInstance().distanceP2P(points[i + 1], points[i]);
                double arcLength = keyPointIndexes.Count <= 10 ? intervalLength + 10 : intervalLength;
                if ((int)distance >= arcLength)
                {
                    keyPointIndexes.Add(i + 1);
                    //drawPoint(points[i + 1].X, points[i + 1].Y, Colors.Blue);
                    distance = 0;
                    i++;
                }
            }
        }
        /// <summary>
        /// 在画板上画点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void drawPoint(double x, double y, Color color)
        {
            StylusPointCollection spc = new StylusPointCollection();
            StylusPoint currPoint = new StylusPoint(x, y);
            spc.Add(currPoint);
            Stroke s = new Stroke(spc);
            s.DrawingAttributes.Color = color;
            s.DrawingAttributes.Width = 8;
            s.DrawingAttributes.Height = 8;
            InkCanvas.Strokes.Add(s);
        }
        /// <summary>
        /// 向螺旋线中添加所有视频关键帧
        /// </summary>
        private void addImages2Spiral()
        {
            for (int i = 0; i < keyFrames.Count; i++)
            {
                addImage2Spiral(i, keyFrames[i],true);
            }            
        }
        /// <summary>
        /// 移动关键帧
        /// </summary>
        /// <param name="image">要移动的关键帧图片</param>
        /// <param name="zoomOut">是否是向外移动，true代表是向外移动，false代表是向内移动</param>
        /// <param name="movePointCount">每次移动的点数</param>
        /// <param name="currKeyPointIndex">现在的索引值</param>
        //int startIndex = 0;
        //int endIndex = 0;
        //public void moveKeyFrame(bool zoomOut,int movePointCount,int currKeyPointIndex)
        //{
        //    //if (isNew)
        //    //{
        //    //    addNewSpiralStroke(i);
        //    //}
        //    if (startIndex == 0 && endIndex == 0)
        //    {
        //        startIndex = keyPointIndexes[currKeyPointIndex];
        //        endIndex = keyPointIndexes[currKeyPointIndex + 1];
        //    }
        //    else
        //    {
        //        startIndex += movePointCount;
        //        endIndex += movePointCount; 
        //    }
        //    if (currKeyPointIndex> 0)
        //    {
        //        if (currKeyPointIndex == keyFrames.Count - 1)
        //        {
        //            StylusPointCollection spc = new StylusPointCollection();
        //            for (int i = startIndex+1; i <= endIndex; i++)
        //            {
        //                spc.Add(new StylusPoint(points[i].X, points[i].Y));
        //            }

        //            Stroke stroke = new Stroke(spc);
        //            stroke.DrawingAttributes.Color = Colors.Yellow;
        //            stroke.DrawingAttributes.Width = mySpiral.SpiralStroke.DrawingAttributes.Width;
        //            stroke.DrawingAttributes.Height = mySpiral.SpiralStroke.DrawingAttributes.Width;
        //            mySpiral.InkCanvas.Strokes.Add(stroke);
        //        }
        //        List<Rectangle> bounds = new List<Rectangle>();//图片边界列表
        //        if(InkCanvas.Children.IndexOf(keyFrames[currKeyPointIndex].Image)!=-1)
        //        {
        //            InkCanvas.Children.Remove(keyFrames[currKeyPointIndex].Image);
        //        }
        //        Image newKeyFrame = InkConstants.getImageFromName(keyFrames[currKeyPointIndex].ImageName);
        //        keyFrames[currKeyPointIndex].Image = newKeyFrame;
        //        InsertImage(newKeyFrame, startIndex, endIndex, bounds);
        //    }
        //}
        
        /// <summary>
        /// 向螺旋线中添加视频关键帧，不是第一帧的情况
        /// </summary>
        /// <param name="index">在螺旋摘要中的索引值</param>
        /// <param name="keyFrame"></param>
        /// <param name="StartIndex"></param>
        /// <param name="EndIndex"></param>
        /// <param name="bounds"></param>
        private void InsertImage(int index,KeyFrame keyFrame,int StartIndex, int EndIndex, List<Rectangle> bounds)
        {
            StylusPointCollection fourPoints = new StylusPointCollection();
            fourPoints.Add(points[StartIndex]);
            fourPoints.Add(points[EndIndex]);

            if (points[EndIndex].X - points[StartIndex].X != 0)
            {
                double k = (points[EndIndex].Y - points[StartIndex].Y) / (points[EndIndex].X - points[StartIndex].X);//斜率
                double x31 = points[StartIndex].X + mySpiral.SpiralWidth * Math.Sqrt(k * k / (1 + k * k));
                double y31 = k == 0 ? points[StartIndex].Y + mySpiral.SpiralWidth : points[StartIndex].Y + (points[StartIndex].X - x31) / k;
                StylusPoint sp31 = new StylusPoint(x31, y31);
                double x32 = points[StartIndex].X - mySpiral.SpiralWidth * Math.Sqrt(k * k / (1 + k * k));
                double y32 = k == 0 ? points[StartIndex].Y - mySpiral.SpiralWidth : points[StartIndex].Y + (points[StartIndex].X - x32) / k;
                StylusPoint sp32 = new StylusPoint(x32, y32);
                StylusPoint sp3 = new StylusPoint();
                if (MathTool.getInstance().distanceP2P(sp31, center) < MathTool.getInstance().distanceP2P(sp32, center))
                {
                    sp3 = sp31;
                }
                else
                {
                    sp3 = sp32;
                }

                double x41 = points[EndIndex].X + mySpiral.SpiralWidth * Math.Sqrt(k * k / (1 + k * k));
                double y41 = k == 0 ? points[EndIndex].Y + mySpiral.SpiralWidth : points[EndIndex].Y + (points[EndIndex].X - x41) / k;
                StylusPoint sp41 = new StylusPoint(x41, y41);
                double x42 = points[EndIndex].X - mySpiral.SpiralWidth * Math.Sqrt(k * k / (1 + k * k));
                double y42 = k == 0 ? points[EndIndex].Y - mySpiral.SpiralWidth : points[EndIndex].Y + (points[EndIndex].X - x42) / k;
                StylusPoint sp42 = new StylusPoint(x42, y42);
                StylusPoint sp4 = new StylusPoint();
                if (MathTool.getInstance().distanceP2P(sp41, center) < MathTool.getInstance().distanceP2P(sp42, center))
                {
                    sp4 = sp41;
                }
                else
                {
                    sp4 = sp42;
                }
                StylusPoint sp5 = new StylusPoint((sp3.X + sp4.X) / 2, (sp3.Y + sp4.Y) / 2);
                fourPoints.Add(sp5);
                Stroke s = new Stroke(fourPoints);
                Rect bound = s.GetBounds();

                //处理相邻两关键点接近垂直的情况                        
                if (Math.Abs(points[EndIndex].X - points[StartIndex].X) <= 10)
                {
                    StylusPoint sp6 = new StylusPoint(bound.Left, bound.Top - 20);
                    StylusPoint sp7 = new StylusPoint(bound.Left, bound.Top + bound.Height + 10);
                    fourPoints.Add(sp6);
                    fourPoints.Add(sp7);
                    s = new Stroke(fourPoints);
                    bound = s.GetBounds();
                }
                else
                {
                    if (points[StartIndex].Y > center.Y)
                    {
                        double angle = MathTool.getInstance().getAngleP2P(center, points[StartIndex]);
                        if ((angle > 180 && angle < 250) || (angle > 280 && angle < 360))
                        {
                            StylusPoint sp6 = new StylusPoint(bound.Left, bound.Top - 30);
                            fourPoints.Add(sp6);
                            s = new Stroke(fourPoints);
                            bound = s.GetBounds();
                        }
                    }
                    else
                    {
                        double angle = MathTool.getInstance().getAngleP2P(new StylusPoint(center.X - mySpiral.SpiralWidth / 2, center.Y), points[StartIndex]);
                        if ((angle >= 0 && angle < 60) || (angle > 120 && angle < 180) || (angle > 316 && angle <= 360))
                        {
                            StylusPoint sp6 = new StylusPoint(bound.Left, bound.Top + bound.Height + 20);
                            fourPoints.Add(sp6);
                        }
                        if (angle > 155 && angle < 205)
                        {
                            StylusPoint sp6 = new StylusPoint(bound.Left, bound.Top - 20);
                            fourPoints.Add(sp6);
                        }
                        s = new Stroke(fourPoints);
                        bound = s.GetBounds();
                    }
                }
                bounds.Add(ConvertClass.getInstance().RectToRectangle2(bound));
                keyFrameCenterPoints.Add(new Point(bound.X + bound.Width / 2, bound.Y + bound.Height / 2));

                //调整图片大小
                MathTool.getInstance().resizeImage(keyFrame, bound.Width, bound.Height);

                //去掉图片多余边界
                //RemoveRedundance(index, keyFrame, StartIndex, EndIndex, bound.X + bound.Width / 2 - keyFrame.Image.Width / 2, bound.Y + bound.Height / 2 - keyFrame.Image.Height / 2);

                //添加图片
                addPicture(bound.X + bound.Width / 2 - keyFrame.Image.Width / 2, bound.Y + bound.Height / 2 - keyFrame.Image.Height / 2, keyFrame);
            }
            else
            {
                double x31 = points[StartIndex].X + mySpiral.SpiralWidth;
                double y3 = points[StartIndex].Y;
                StylusPoint sp31 = new StylusPoint(x31, y3);
                double x32 = points[StartIndex].X - mySpiral.SpiralWidth;
                StylusPoint sp32 = new StylusPoint(x32, y3);
                StylusPoint sp3 = new StylusPoint();
                if (MathTool.getInstance().distanceP2P(sp31, center) < MathTool.getInstance().distanceP2P(sp32, center))
                {
                    sp3 = sp31;
                }
                else
                {
                    sp3 = sp32;
                }
                fourPoints.Add(sp3);
                Stroke s = new Stroke(fourPoints);
                Rect bound = s.GetBounds();
                StylusPoint sp4 = new StylusPoint(bound.Left, bound.Top - 10);
                StylusPoint sp5 = new StylusPoint(bound.Left, bound.Top + bound.Height + 10);
                fourPoints.Add(sp4);
                fourPoints.Add(sp5);
                s = new Stroke(fourPoints);
                bound = s.GetBounds();
                bounds.Add(ConvertClass.getInstance().RectToRectangle2(bound));
                keyFrameCenterPoints.Add(new Point(bound.X + bound.Width / 2, bound.Y + bound.Height / 2));

                //调整图片大小
                MathTool.getInstance().resizeImage(keyFrame, bound.Width, bound.Height);

                //去掉图片多余边界
                RemoveRedundance(index, keyFrame, StartIndex, EndIndex, bound.X + bound.Width / 2 - keyFrame.Image.Width / 2, bound.Y + bound.Height / 2 - keyFrame.Image.Height / 2);

                //添加图片
                addPicture(bound.X + bound.Width / 2 - keyFrame.Image.Width / 2, bound.Y + bound.Height / 2 - keyFrame.Image.Height / 2, keyFrame);
            }
        }
        /// <summary>
        /// 删除多余螺旋线，现在没有用
        /// </summary>
        private void deleteRedundantStrokePoints()
        {
            
        }

        /// <summary>
        /// 向螺旋线中添加关键帧图片
        /// </summary>
        /// <param name="i">插入位置的索引值</param>
        /// <param name="isNew">是否是螺旋线上已有位置</param>
        public void addImage2Spiral(int i,KeyFrame keyFrame,bool isNew)
        {
            if (isNew)
            {
                keyFrame.SpiralStrokeClip=addNewSpiralStroke(i);
            }
            List<Rectangle> bounds = new List<Rectangle>();//图片边界列表
            if (i == 0)
            {
                StylusPointCollection fourPoints = new StylusPointCollection();
                fourPoints.Add(points[0]);
                fourPoints.Add(points[(int)(mySpiral.SpiralWidth / 2)]);
                fourPoints.Add(points[(int)(mySpiral.SpiralWidth)]);
                fourPoints.Add(points[(int)(mySpiral.SpiralWidth * 2)]);
                Stroke s = new Stroke(fourPoints);
                Rect bound = s.GetBounds();
                bounds.Add(ConvertClass.getInstance().RectToRectangle2(bound));
                keyFrameCenterPoints.Add(new Point(bound.X + bound.Width / 2, bound.Y + bound.Height / 2));

                //调整图片大小
                MathTool.getInstance().resizeImage(keyFrame, bound.Width, bound.Height);

                //去掉图片多余边界
                RemoveRedundanceFirst(keyFrame, bound.X + bound.Width / 2 - keyFrame.Image.Width / 2, bound.Y + bound.Height / 2 - keyFrame.Image.Height / 2);

                //添加图片
                addPicture(bound.X + bound.Width / 2 - keyFrame.Image.Width / 2, bound.Y + bound.Height / 2 - keyFrame.Image.Height / 2, keyFrame);
            }
            else
            {
                int startIndex = keyPointIndexes[i];
                int endIndex = keyPointIndexes[i + 1];
                InsertImage(i,keyFrame, startIndex, endIndex, bounds);
            }
        }
        /// <summary>
        /// 添加对应索引的螺旋线
        /// </summary>
        /// <param name="index"></param>
        private Stroke addNewSpiralStroke(int index)
        {
            //关键帧对应关键点在螺旋笔迹中的索引
            int startPointIndexInSpiral = keyPointIndexes[index];

            //添加蓝色笔迹代表该关键帧有注释
            StylusPointCollection spc = new StylusPointCollection();
            if (index == 0)
            {
                for (int i = 0; i <= startPointIndexInSpiral; i++)
                {
                    spc.Add(new StylusPoint(points[i].X, points[i].Y));
                }
            }
            else
            {
                int endPointIndexInSpiral = keyPointIndexes[index + 1];
                for (int i = startPointIndexInSpiral; i <= endPointIndexInSpiral; i++)
                {
                    spc.Add(new StylusPoint(points[i].X, points[i].Y));
                }
            }
            Stroke stroke = new Stroke(spc);
            stroke.DrawingAttributes.Color = Colors.Yellow;
            stroke.DrawingAttributes.Width = mySpiral.SpiralStroke.DrawingAttributes.Width;
            stroke.DrawingAttributes.Height = mySpiral.SpiralStroke.DrawingAttributes.Width;
            mySpiral.InkCanvas.Strokes.Add(stroke);
            return stroke;
        }
        /// <summary>
        /// 向画板中添加图片
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="index"></param>
        private void addPicture(double x, double y, KeyFrame keyFrame)
        {
            if (InkCanvas.Children.IndexOf(keyFrame.Image) == -1)
            {
                keyFrame.Image.Margin = new Thickness(x, y, 0, 0);
                InkCanvas.Children.Add(keyFrame.Image);
            }
        }
        /// <summary>
        /// 对第一幅图的边界进行修复
        /// </summary>
        /// <param name="image"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        private void RemoveRedundanceFirst(KeyFrame keyFrame, double left, double top)
        {
            System.Drawing.Bitmap bitmapSource = new System.Drawing.Bitmap(keyFrame.ImageName);
            BitmapData data = bitmapSource.LockBits(new System.Drawing.Rectangle(0, 0, bitmapSource.Width, bitmapSource.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int bytes = bitmapSource.Width * bitmapSource.Height * 4;
            byte[] ArgbValues = new byte[bytes];
            IntPtr ptr = data.Scan0;
            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, ArgbValues, 0, bytes);
            double pointWidth = keyFrame.Image.Width / bitmapSource.Width;
            for (int i = 0; i < bitmapSource.Width; i++)
            {
                for (int j = 0; j < bitmapSource.Height; j++)
                {
                    StylusPoint currPoint = new StylusPoint(left + i * pointWidth, top + j * pointWidth);
                    StylusPoint currCenterPoint = getCenterPoint(currPoint);
                    double bigRadius = currPoint.Y > center.Y ? mySpiral.SpiralWidth : mySpiral.SpiralWidth / 2;
                    double x2y2 = (left + i * pointWidth - currCenterPoint.X) * (left + i * pointWidth - currCenterPoint.X)
                        + (top + j * pointWidth - currCenterPoint.Y) * (top + j * pointWidth - currCenterPoint.Y);
                    if (x2y2 <= bigRadius * bigRadius)
                    {
                        if (!((mySpiral.IsClockwise && currPoint.X > center.X) || (!mySpiral.IsClockwise && currPoint.X < center.X)))
                        {
                            int indexBmp = 4 * (data.Width * j + i);
                            ArgbValues[indexBmp + 3] = 0;
                        }
                    }
                    else if (x2y2 > bigRadius * bigRadius)
                    {
                        int indexBmp = 4 * (data.Width * j + i);
                        ArgbValues[indexBmp + 3] = 0;
                    }
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(ArgbValues, 0, ptr, bytes);
            System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height,bitmapSource.Width* 4,System.Drawing.Imaging.PixelFormat.Format32bppArgb,ptr);
            BitmapImage bitmapImage = ImageTool.BitmapToImageSource(bitmapTarget);
            keyFrame.Image.Source = bitmapImage;
            bitmapSource.UnlockBits(data);
        }
        /// <summary>
        /// 去除图片的多余边界
        /// </summary>
        /// <param name="index">在螺旋摘要中的位置</param>
        /// <param name="keyFrame">关键帧</param>
        /// <param name="startIndex">第一个关键点在螺旋线中的点索引值</param>
        /// <param name="endIndex">第二个关键点在螺旋线中的点索引值</param>
        /// <param name="left">关键帧图片的左边距</param>
        /// <param name="top">关键帧图片的上边距</param>
        private void RemoveRedundance(int index,KeyFrame keyFrame, int startIndex,int endIndex, double left, double top)
        {
            System.Drawing.Bitmap bitmapSource = new System.Drawing.Bitmap(keyFrame.ImageName);
            BitmapData data = bitmapSource.LockBits(new System.Drawing.Rectangle(0, 0, bitmapSource.Width, bitmapSource.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int bytes = bitmapSource.Width * bitmapSource.Height * 4;
            byte[] ArgbValues = new byte[bytes];
            IntPtr ptr = data.Scan0;
            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, ArgbValues, 0, bytes);
            double pointWidth = keyFrame.Image.Width / bitmapSource.Width;
            //去除无用边界
            double maxDistance = 0;
            double maxi = 0;
            double maxj = 0;
            StylusPoint CenterPoint = getCenterPoint(points[startIndex]);
            double distance1 = MathTool.getInstance().distanceP2P(points[startIndex], CenterPoint);//关键点与中心点之间的距离
            double angle1 = 0;
            if (mySpiral.IsClockwise)
            {
                angle1 = ((int)(distance1 + mySpiral.SpiralStrokeWidth) / (int)mySpiral.SpiralWidth ) * 360 -MathTool.getInstance().getAngleP2P(points[startIndex], CenterPoint);//关键点与中心点连线的夹角
            }
            else
            {
                angle1 = ((int)(distance1 + mySpiral.SpiralStrokeWidth) / (int)mySpiral.SpiralWidth - 1) * 360 + MathTool.getInstance().getAngleP2P(points[startIndex], CenterPoint);//关键点与中心点连线的夹角
            }
            //找到渐变的最远点
            for (int i = 0; i < bitmapSource.Width; i++)
            {
                for (int j = 0; j < bitmapSource.Height; j++)
                {
                    StylusPoint currPoint = new StylusPoint(left + i * pointWidth, top + j * pointWidth);
                    StylusPoint currCenterPoint = getCenterPoint(currPoint);
                    double bigRadius = MathTool.getInstance().distanceP2P(currPoint.Y > center.Y ? points[startIndex] : points[endIndex], currCenterPoint);
                    double smallRadius = bigRadius - mySpiral.SpiralWidth;//小圆半径
                    double x2y2 = (left + i * pointWidth - currCenterPoint.X) * (left + i * pointWidth - currCenterPoint.X)
                        + (top + j * pointWidth - currCenterPoint.Y) * (top + j * pointWidth - currCenterPoint.Y);
                    if (!(x2y2 < smallRadius * smallRadius || x2y2 > bigRadius * bigRadius))
                    {
                        double distance2 = MathTool.getInstance().distanceP2P(currPoint, currCenterPoint);//关键点与中心点之间的距离
                        double currDistance = MathTool.getInstance().distanceP2L(currPoint, points[startIndex], currCenterPoint);
                        double angle2 = 0;
                        if (currCenterPoint.X == center.X)
                        {
                            if (mySpiral.IsClockwise)
                            {
                                angle2 = (((int)(distance2) / (int)mySpiral.SpiralWidth) + 1) * 360 - MathTool.getInstance().getAngleP2P(currPoint, currCenterPoint);
                            }
                            else
                            {
                                angle2 = (((int)(distance2) / (int)mySpiral.SpiralWidth)) * 360 + MathTool.getInstance().getAngleP2P(currPoint, currCenterPoint);

                            }
                        }
                        else
                        {
                            if (mySpiral.IsClockwise)
                            {
                                angle2 = (((int)(distance2 - mySpiral.SpiralWidth / 2) / (int)mySpiral.SpiralWidth) + 1) * 360 - MathTool.getInstance().getAngleP2P(currPoint, currCenterPoint);
                            }
                            else
                            {
                                angle2 = (((int)(distance2 - mySpiral.SpiralWidth / 2) / (int)mySpiral.SpiralWidth)) * 360 + MathTool.getInstance().getAngleP2P(currPoint, currCenterPoint);

                            }
                        }
                        double angle = angle1 - angle2;
                        if (angle > 0 && currDistance > maxDistance)
                        {
                            maxDistance = currDistance;
                            maxi = i;
                            maxj = j;
                        }
                    }
                    else
                    {
                        int indexBmp = 4 * (data.Width * j + i); 
                        ArgbValues[indexBmp + 3] = 0;
                    }
                    if (index == 2 && currPoint.Y > center.Y)//纠正索引为2的图片
                    {
                        int indexBmp = 4 * (data.Width * j + i);
                        ArgbValues[indexBmp + 3] = 0;
                    }
                }
            }
            double angle3 = 0;
            if (index == keyFrames.Count - 2)
            {
                if (mySpiral.IsClockwise)
                {
                    angle3 = ((int)(distance1 + mySpiral.SpiralStrokeWidth) / (int)mySpiral.SpiralWidth) * 360 - MathTool.getInstance().getAngleP2P(points[keyPointIndexes[index + 2]], CenterPoint);//关键点与中心点连线的夹角
                }
                else
                {
                    angle3 = ((int)(distance1 + mySpiral.SpiralStrokeWidth) / (int)mySpiral.SpiralWidth - 1) * 360 + MathTool.getInstance().getAngleP2P(points[keyPointIndexes[index + 2]], CenterPoint);//关键点与中心点连线的夹角
                }
            }
            else if (index == keyFrames.Count - 1)
            {
                if (mySpiral.IsClockwise)
                {
                    angle3 = ((int)(distance1 + mySpiral.SpiralStrokeWidth) / (int)mySpiral.SpiralWidth) * 360 - MathTool.getInstance().getAngleP2P(points[endIndex], CenterPoint);//关键点与中心点连线的夹角
                }
                else
                {
                    angle3 = ((int)(distance1 + mySpiral.SpiralStrokeWidth) / (int)mySpiral.SpiralWidth - 1) * 360 + MathTool.getInstance().getAngleP2P(points[endIndex], CenterPoint);//关键点与中心点连线的夹角
                }
            }
            //对图片连接处进行渐变
            for (int i = 0; i < bitmapSource.Width; i++)
            {
                for (int j = 0; j < bitmapSource.Height; j++)
                {
                    int indexBmp = 4 * (data.Width * j + i); 
                    if (ArgbValues[indexBmp + 3] != 0)
                    {
                        double rate = 1;
                        StylusPoint currPoint = new StylusPoint(left + i * pointWidth, top + j * pointWidth);
                        StylusPoint currCenterPoint = getCenterPoint(currPoint);
                        double distance2 = MathTool.getInstance().distanceP2P(currPoint, currCenterPoint);//关键点与中心点之间的距离
                        double angle2 = 0;
                        if (currCenterPoint.X == center.X)
                        {
                            if (mySpiral.IsClockwise)
                            {
                                angle2 = ((int)(distance2) / (int)mySpiral.SpiralWidth + 1) * 360 - MathTool.getInstance().getAngleP2P(currPoint, currCenterPoint);
                            }
                            else
                            {
                                angle2 = ((int)(distance2) / (int)mySpiral.SpiralWidth) * 360 + MathTool.getInstance().getAngleP2P(currPoint, currCenterPoint);
                            }
                        }
                        else
                        {
                            if (mySpiral.IsClockwise)
                            {
                                angle2 = ((int)(distance2 - mySpiral.SpiralWidth / 2) / (int)mySpiral.SpiralWidth + 1) * 360 - MathTool.getInstance().getAngleP2P(currPoint, currCenterPoint);
                            }
                            else
                            {
                                angle2 = ((int)(distance2 - mySpiral.SpiralWidth / 2) / (int)mySpiral.SpiralWidth) * 360 + MathTool.getInstance().getAngleP2P(currPoint, currCenterPoint);
                            }
                        }
                        double angle = angle1 - angle2;
                        if (angle > 0)
                        {
                            double currDistance = MathTool.getInstance().distanceP2L(currPoint, points[startIndex], currCenterPoint);
                            rate = 1 - currDistance / maxDistance;//透明度比例
                            if (rate > 1) { rate = 1; }
                            ArgbValues[indexBmp + 3] = (byte)(255 * Math.Pow(rate,3));
                            //对第二、三象限过渡效果进行调整
                            if (((angle1 > 720 && angle1 % 360 >= 0 && angle1 % 360 < 90) || (angle1 > 630 && angle1 % 360 > 270 && angle1 % 360 < 360)) && j <= 20)
                            {
                                rate = ((double)j) / 20;
                                ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                            }
                            //对第一、四象限过渡效果进行调整
                            else if (angle1 > 450 && angle1 % 360 >= 90 && angle1 % 360 < 270 && bitmapSource.Height - j <= 15)
                            {
                                rate = ((double)(bitmapSource.Height - j)) / 15;
                                ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                             }

                        }
                        if (index == keyFrames.Count - 2 || index == keyFrames.Count - 1)
                        {
                            if (angle3 - angle2 < 0)
                            {
                                ArgbValues[indexBmp + 3] = 0;
                            }
                        }
                    }
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(ArgbValues, 0, ptr, bytes);
            System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height, bitmapSource.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
            BitmapImage bitmapImage = ImageTool.BitmapToImageSource(bitmapTarget);
            keyFrame.Image.Source = bitmapImage;
            bitmapSource.UnlockBits(data);
        }

        /// <summary>
        /// 获取圆心
        /// </summary>
        /// <param name="currPoint"></param>
        /// <returns></returns>
        private StylusPoint getCenterPoint(StylusPoint currPoint)
        {
            StylusPoint centerPoint = center;
            if (currPoint.Y < center.Y)
            {
                if (mySpiral.IsClockwise)
                {
                    centerPoint = new StylusPoint(center.X + mySpiral.SpiralWidth / 2, center.Y);
                }
                else
                {
                    centerPoint = new StylusPoint(center.X - mySpiral.SpiralWidth / 2, center.Y);
                }
            }
            return centerPoint;
        }

        /// <summary>
        /// 隐藏螺旋摘要，用作与自定义摘要的转化
        /// </summary>
        public void hiddenSpiralSummarization()
        {
            this.InkCanvas.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 显示螺旋摘要，用作与自定义摘要的转化
        /// </summary>
        public void showSpiralSummarization()
        {
            this.InkCanvas.Visibility = Visibility.Visible;
        }

        #endregion
    }
}
