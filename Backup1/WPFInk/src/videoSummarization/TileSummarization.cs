using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.IO;
using WPFInk.tool;
using WPFInk.ink;
using System.Drawing.Imaging;
using WPFInk.state;

namespace WPFInk.videoSummarization
{
    public class TileSummarization:VideoSummarization
    {
        const int countInrow = 7;//每行显示关键帧数量
        private int fusePixel = 25;
        private int left;
        private int top; //20
        private Point point;
        private double imageHeight = 60;
        private StrokeCollection lines = new StrokeCollection();
        //private ScrollViewer scrollViewer = new ScrollViewer();
        //private InkCanvas localInkCanvas = new InkCanvas();
        
        //平铺动画效果的一些参数
        private int startInsert = 0;
        private int endInsert = 0;
        private int preSelectedCount = 0;
        private int afterSelectedCount = 0;
        private int sign = 0;
        private int rowWidth;
        private int startDelete = 0;
        private int endDelete = 0;
        private bool protect = true; //防止在动画过程中响应鼠标中键事件的标记
       
        private int distanceOfTrueTimeOut;
        private int distanceOfTrueTimeIn;
        private Thickness magin;
        private Thickness counterPartMagin;
        private System.Windows.Forms.Timer timerOut = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer timerIn = new System.Windows.Forms.Timer();
        private bool isShowHalf = true;
        private InkCollector inkCollector;
        //end


        public InkCollector InkCollector
        {
            get { return inkCollector; }
            set { inkCollector = value; }
        }
        public System.Windows.Forms.Timer TimerOut
        {
            get { return timerOut; }
        }

        public System.Windows.Forms.Timer TimerIn
        {
            get { return timerIn; }
        }

        public void initionTimer()
        {
            timerOut.Interval = 10;
            timerIn.Interval = 10;
            timerOut.Tick += new System.EventHandler(this.timer_ZoomOut);
            timerIn.Tick += new System.EventHandler(this.timer_Zoomin);
        }

        public int PreSelectedCount
        {
            get { return preSelectedCount; }
        }

        public int AfterSelectedCount
        {
            get { return afterSelectedCount; }
        }
        
        #region 封装变量
        public bool Protect
        {
            get { return protect; }
            set { protect = value; }
        }

        public int StartInsert
        {
            get { return startInsert; }
            set { startInsert = value; }
        }

        public int EndInsert
        {
            get { return endInsert; }
            set { endInsert = value; }
        }


        public int StartDelete
        {
            get { return startDelete; }
            set { startDelete = value; }
        }

        public int EndDelete
        {
            get { return endDelete; }
            set { endDelete = value; }
        }
        
        //public ScrollViewer ScrollViewer
        //{
        //    get { return scrollViewer; }
        //    set { scrollViewer = value; }
        //}

        /// <summary>
        /// 平铺摘要的上边距
        /// </summary>
        public int Top
        {
            get { return top; }
            set { top = value; }
        }
        /// <summary>
        /// 平铺摘要的左边距
        /// </summary>
        public int Left
        {
            get { return left; }
            set { left = value; }
        }

        public double ImageHeight
        {
            get { return imageHeight; }
            set { imageHeight = value; }
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_inkCanvas">摘要所在画布</param>
        /// <param name="keyframes">摘要包含的关键帧</param>
        /// <param name="center">摘要的左上角点的坐标</param>

        public TileSummarization(InkCanvas _inkCanvas, List<KeyFrame> keyframes, StylusPoint center,bool isShowHalf)
            : base(null, 0, keyframes, _inkCanvas)
        {
            try
            {
                this.initionTimer();

                ShowWidth = 150;
                keyFrames[0].Image = InkConstants.getImageFromName(keyframes[0].ImageName);
                showHeight = ShowWidth * keyFrames[0].Image.Height / keyFrames[0].Image.Width;
                keyFrames[0].Image.Width = ShowWidth;
                keyFrames[0].Image.Height = ShowHeight;
                for (int i = 1; i < keyFrames.Count; i++)
                {
                    keyFrames[i].Image = InkConstants.getImageFromName(keyframes[i].ImageName);

                    //MathTool.resizeImage(keyFrames[i], 150, 100);
                    keyFrames[i].Image.Width = ShowWidth;
                    keyFrames[i].Image.Height = ShowHeight;
                }
                this.top = (int)center.Y;
                this.left = (int)center.X;
                this.Center = center;
                this.fusePixel = (int)(showWidth * 0.1);
                this.isShowHalf = isShowHalf;
                //scrollViewer.Width = InkCanvas.Width;
                //scrollViewer.Height = InkCanvas.Height;
                //InkCanvas.Children.Add(scrollViewer);
                //scrollViewer.Content = localInkCanvas;
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Reset();
                sw.Start();
                addImages2Track();
                sw.Stop();
                Console.WriteLine("tile总需要时间：" + sw.ElapsedMilliseconds + "ms");
                GC.Collect();
                rowWidth = Convert.ToInt32(left + (showWidth - fusePixel) * countInrow);
                drawLine();
            }
            catch
            {
                MessageBox.Show("TileSummarization构造失败！");
            }
        }

        //public void initializeAllVariable()
        //{
        //    startInsert = 0;
        //    endInsert = 0;
        //    preSelectedCount = 0;
        //    afterSelectedCount = 0;
        //    distanceOfTrueTime = 0;

        //}

        private void drawLine()
        {
            if (0 != keyFrames.Count)
            {
                int line = (keyFrames.Count - 1) / countInrow + 1;
                for (int i = 1; i <= line; i++)
                {
                    Point startPoint = new Point(Center.X, Center.Y + i * showHeight + 2);
                    Point endPoint = new Point(Center.X + (showWidth - fusePixel) * countInrow + fusePixel, Center.Y + i * showHeight + 2);
                    InkTool.getInstance().DrawLine(startPoint.X, startPoint.Y,endPoint.X, endPoint.Y, InkCanvas, Colors.Black);
                }
            }
        }

        public override void addImages2Track()
        {
            double j = 1;
            double k = 0;
            if (isShowHalf)
            {
                for (int i = 0; i <= keyFrames.Count - 1; i++)
                {
                    Image image = keyFrames[i].Image;
                    if (j == countInrow)
                    {
                        j = 0;
                        k++;
                    }
                    if (0 == i)  //处理第一个关键帧
                    {
                        point = new Point(left, top + k * ShowHeight + 2);
                        showKeyFrames.Add(keyFrames[i]);
                        keyFrames[i].Level = true;
                        keyFrames[i].IsVisibleInTile = true;
                        showKeyFrames[showKeyFrames.Count - 1].ID = i;

                    }
                    else  //处理后续的关键帧
                    {
                        if (0 == j)
                        {
                            point = new Point(left, top + k * ShowHeight + 2);
                            if (0 == i % 2)
                            {
                                j++;
                                showKeyFrames.Add(keyFrames[i]);
                                showKeyFrames[showKeyFrames.Count - 1].ID = i;
                                keyFrames[i].Level = true;
                                keyFrames[i].IsVisibleInTile = true;
                            }
                            else
                                keyFrames[i].Image.Visibility = Visibility.Hidden;

                        }
                        else
                        {
                            if (1 == i % 2)
                            {
                                point = new Point(keyFrames[i - 1].Image.Margin.Left + ShowWidth - fusePixel, keyFrames[i - 1].Image.Margin.Top);
                                keyFrames[i].Image.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                point = new Point(keyFrames[i - 1].Image.Margin.Left, keyFrames[i - 1].Image.Margin.Top);
                                j++;
                                showKeyFrames.Add(keyFrames[i]);
                                showKeyFrames[showKeyFrames.Count - 1].ID = i;
                                keyFrames[i].Level = true;
                                keyFrames[i].IsVisibleInTile = true;
                            }

                        }
                    }


                    ///<start> 为每一帧创建一个备份，以用于动画效果
                    Image imageCounterPart = InkConstants.getImageFromName(keyFrames[i].ImageName);
                    keyFrames[i].CounterPartImage = imageCounterPart;
                    keyFrames[i].CounterPartImage.Width = showWidth;
                    keyFrames[i].CounterPartImage.Height = showHeight;
                    //keyFrames[i].CounterPartImage.Visibility = Visibility.Hidden;


                    InkCanvas.Children.Add(keyFrames[i].CounterPartImage);
                    InkCanvas.Children.Add(keyFrames[i].Image);

                    ///<end>
                    image.Margin = new Thickness(point.X, point.Y, 0, 0);
                    //if (j != 0)
                    fuseImages(i);
                    keyFrames[i].CounterPartImage.Visibility = Visibility.Hidden;

                    //lines.Add(InkTool.getInstance().DrawLine(point.X, keyFrames[i].Image.Height + point.Y - 2, keyFrames[i].Image.Width + point.X, keyFrames[i].Image.Height + point.Y - 2, localInkCanvas, Colors.Yellow));
                    //keyFrames[i].Line.Add(InkTool.getInstance().DrawLine(keyFrames[i].Image.Margin.Left, keyFrames[i].Image.Height + keyFrames[i].Image.Margin.Top - 2,
                    //    keyFrames[i].Image.Width + keyFrames[i].Image.Margin.Left, keyFrames[i].Image.Height + keyFrames[i].Image.Margin.Top - 2, localInkCanvas, Colors.Yellow));
                    StylusPoint stylusPoint = new StylusPoint(point.X + 50, point.Y + 29);
                    keyPoints.Add(stylusPoint);

                }
                if (0 == keyFrames.Count % 2)
                {
                    keyFrames[keyFrames.Count - 1].IsVisibleInTile = true;
                    keyFrames[keyFrames.Count - 1].Image.Visibility = Visibility.Visible;
                    showKeyFrames.Add(keyFrames[keyFrames.Count - 1]);
                }
            }
            else
            {
                j = 0;
                for (int i = 0; i <= keyFrames.Count - 1; i++)
                {
                    Image image = keyFrames[i].Image;
                    if (j >= countInrow)
                    {
                        j = 0; ++k;
                    }
                    if (j == 0)
                    {
                        point = new Point(left, top + k * ShowHeight + 2);
                    }
                    else
                    {
                        point = new Point(keyFrames[i - 1].Image.Margin.Left + ShowWidth - fusePixel, keyFrames[i - 1].Image.Margin.Top);
                    }

                    image.Margin = new Thickness(point.X, point.Y, 0, 0);
                    //if (j != 0)
                    fuseImages(i);
                    InkCanvas.Children.Add(keyFrames[i].Image);
                    //lines.Add(InkTool.getInstance().DrawLine(point.X, keyFrames[i].Image.Height + point.Y - 2, keyFrames[i].Image.Width + point.X, keyFrames[i].Image.Height + point.Y - 2, localInkCanvas, Colors.Black));
                    j++;
                    StylusPoint stylusPoint = new StylusPoint(point.X + 50, point.Y + 29);
                    keyPoints.Add(stylusPoint);
                    showKeyFrames.Add(keyFrames[i]);
                }
            }
        }

        //图片的融合
        public void fuseImages(int index)
        {
            System.Drawing.Bitmap bitmapSource = new System.Drawing.Bitmap(keyFrames[index].ImageName);
            BitmapData data = bitmapSource.LockBits(new System.Drawing.Rectangle(0, 0, bitmapSource.Width, bitmapSource.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int w = bitmapSource.Width, h = bitmapSource.Height;
            int bytes = w * h * 4;
            byte[] ArgbValues = new byte[bytes];
            IntPtr ptr = data.Scan0;
            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, ArgbValues, 0, bytes);
            for (int i = 0; i < fusePixel; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int indexBmp = 4 * (data.Width * j + i);
                    double rate = (double)i / (fusePixel - 1);
                    ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(ArgbValues, 0, ptr, bytes);
            System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height, bitmapSource.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
            BitmapImage bitmapImage = ImageTool.getInstance().BitmapToImageSource(bitmapTarget);
            keyFrames[index].Image.Source = bitmapImage;
            if (isShowHalf)
            {
                keyFrames[index].CounterPartImage.Source = bitmapImage.Clone();
            }
            bitmapSource.UnlockBits(data);

            //System.Drawing.Bitmap bitmapCounterPartSource = new System.Drawing.Bitmap(keyFrames[index].ImageName);
            //BitmapData dataCounterPart = bitmapCounterPartSource.LockBits(new System.Drawing.Rectangle(0, 0, bitmapCounterPartSource.Width, bitmapCounterPartSource.Height),
            //                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            ////int w = bitmapSource.Width, h = bitmapSource.Height;
            //int bytesCounterPart = w * h * 4;
            //byte[] ArgbValuesCounterPart = new byte[bytes];
            //IntPtr ptrCounterPart = dataCounterPart.Scan0;
            //// Copy the RGB values into the array.
            //System.Runtime.InteropServices.Marshal.Copy(ptr, ArgbValues, 0, bytes);
            //for (int i = 0; i < fusePixel; i++)
            //{
            //    for (int j = 0; j < h; j++)
            //    {
            //        int indexBmp = 4 * (data.Width * j + i);
            //        double rate = (double)i / (fusePixel - 1);
            //        ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
            //    }
            //}
            //System.Runtime.InteropServices.Marshal.Copy(ArgbValues, 0, ptr, bytes);
            //System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height, bitmapSource.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
            //BitmapImage bitmapImage = ImageTool.getInstance().BitmapToImageSource(bitmapTarget);
            //keyFrames[index].Image.Source = bitmapImage;
            //bitmapSource.UnlockBits(data);

        }

        /// <summary>
        /// 从平铺摘要获取选择的关键帧
        /// </summary>
        /// <param name="point"></param>
        public override int getSelectedKeyFrameIndex(Point point)//, TileSummarization tileSummarization)
        {
            //double minDistance = double.MaxValue;
            //point.Y += scrollViewer.VerticalOffset;
            point.Y -= InkCanvas.Margin.Top;
            int minIndex = int.MinValue;
            if (this != null)
            {
                if (point.X < Center.X || point.Y < Center.Y || point.X > (showWidth - fusePixel) * countInrow)
                {
                    return int.MinValue;
                }

                try
                {
                    int i = (int)((point.X - Center.X) / (showWidth - fusePixel));
                    int j = (int)((point.Y - Center.Y) / (showHeight));
                    minIndex = j * countInrow + i;
                    if (minIndex < 0 || minIndex >= keyFrameCount)
                    {
                        return int.MinValue;
                    }
                }
                catch
                {
                    MessageBox.Show("TileSummarization 除0错误！");
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
            //    ((localInkCanvas)(_inkCollector._mainPage.InkCanvas.Children[0])).Strokes.Add(stroke);
            //    return stroke;
            //}
            return null;
        }

        #region 平铺展开代码集
        public void SelectInsertKeyFrames(int selectedIndex)
        {
            preSelectedCount = afterSelectedCount = 0;
            startInsert = endInsert = -1;
            int idInKeyFrames = showKeyFrames[selectedIndex].ID;
            if (false == keyFrames[idInKeyFrames].Level)
                return;

            int j = 0;
            int k = 1;
            for (int i = 1; i != 4; i++)
            {

                if ((idInKeyFrames - 2 * i + 1) > 0 && selectedIndex > 0 && false == keyFrames[idInKeyFrames - 2 * i + 1].Level
                    && false == keyFrames[idInKeyFrames - 2 * i + 1].IsVisibleInTile)
                {
                    keyFrames[idInKeyFrames - 2 * i + 1].Image.Visibility = Visibility.Visible;
                    showKeyFrames.Insert(selectedIndex - j, keyFrames[idInKeyFrames - 2 * i + 1]);
                    showKeyFrames[selectedIndex - j].ID = idInKeyFrames - 2 * i + 1;
                    keyFrames[idInKeyFrames - 2 * i + 1].IsVisibleInTile = true;
                    startInsert = selectedIndex - j;
                    j += 2;
                    preSelectedCount = i;
                    selectedIndex++;
                }
                else
                    break;
            }

            for (int i = 1; i != 4; i++)
            {
                if ((idInKeyFrames + 2 * i - 1) < (keyFrames.Count - 1) && selectedIndex < (keyFrames.Count - 1) 
                    && false == keyFrames[idInKeyFrames + 2 * i - 1].Level && false == keyFrames[idInKeyFrames + 2 * i - 1].IsVisibleInTile)
                {
                    keyFrames[idInKeyFrames + 2 * i - 1].Image.Visibility = Visibility.Visible;
                    showKeyFrames.Insert(selectedIndex + k, keyFrames[idInKeyFrames + 2 * i - 1]);
                    showKeyFrames[selectedIndex + k].ID = idInKeyFrames + 2 * i - 1;
                    keyFrames[idInKeyFrames + 2 * i - 1].IsVisibleInTile = true;
                    endInsert = selectedIndex + k;
                    k += 2;
                    afterSelectedCount = i;
                }
                else
                    break;
            }
            if (-1 == startInsert && -1 != endInsert)
            {
                startInsert = selectedIndex + 1;
            }
            if (-1 != startInsert && -1 == endInsert)
            {
                endInsert = selectedIndex - 1;
            }

        }

        public void timer_ZoomOut(object sender, EventArgs e)
        {
            distanceOfTrueTimeOut = preSelectedCount + afterSelectedCount;
            //int rowWidth = Convert.ToInt32(left + (showWidth - fusePixel) * countInrow);
            int left_D_Value = 1;
            int left_D_Value_BEF = 1;
            ((InkState_Summarization)inkCollector._state).MoveTimerSecond = 0;
            for (int i = showKeyFrames.Count - 1; i > startInsert; i--)
            {
                if (i > endInsert)
                {
                    left_D_Value = Convert.ToInt32((showKeyFrames[i].Image.Margin.Left + preSelectedCount + afterSelectedCount)
                    - rowWidth);

                    if (left_D_Value >= -(showWidth - fusePixel))
                    {
                        if (left_D_Value >= 0)
                        {
                            magin = new Thickness(left_D_Value + left, showKeyFrames[i].Image.Margin.Top + ShowHeight, 0, 0);
                            showKeyFrames[i].CounterPartImage.Visibility = Visibility.Hidden;
                        }
                        else if (left_D_Value < 0)
                        {
                            magin = new Thickness(showKeyFrames[i].Image.Margin.Left + preSelectedCount + afterSelectedCount,
                                showKeyFrames[i].Image.Margin.Top, 0, 0);
                            counterPartMagin = new Thickness(left_D_Value + left, showKeyFrames[i].Image.Margin.Top + showHeight, 0, 0);

                            if (showKeyFrames[i].CounterPartImage.Visibility == Visibility.Hidden)
                            {
                                showKeyFrames[i].CounterPartImage.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                        magin = new Thickness(showKeyFrames[i].Image.Margin.Left + preSelectedCount + afterSelectedCount,
                            showKeyFrames[i].Image.Margin.Top, 0, 0);
                }
                else
                {
                    if (0 == (endInsert - i) % 2)
                        distanceOfTrueTimeOut--;

                    left_D_Value_BEF = Convert.ToInt32((showKeyFrames[i].Image.Margin.Left + distanceOfTrueTimeOut)
                        - rowWidth);

                    if (left_D_Value_BEF >= -(showWidth - fusePixel))
                    {
                        if (left_D_Value_BEF >= 0)
                        {
                            magin = new Thickness(left_D_Value_BEF + left, showKeyFrames[i].Image.Margin.Top + ShowHeight, 0, 0);
                            showKeyFrames[i].CounterPartImage.Visibility = Visibility.Hidden;
                        }
                        else if (left_D_Value_BEF < 0)
                        {
                            magin = new Thickness(showKeyFrames[i].Image.Margin.Left + distanceOfTrueTimeOut, showKeyFrames[i].Image.Margin.Top, 0, 0);
                            counterPartMagin = new Thickness(left_D_Value_BEF + left, showKeyFrames[i].Image.Margin.Top + showHeight, 0, 0);
                            if (showKeyFrames[i].CounterPartImage.Visibility == Visibility.Hidden)
                            {
                                showKeyFrames[i].CounterPartImage.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                        magin = new Thickness(showKeyFrames[i].Image.Margin.Left + distanceOfTrueTimeOut, showKeyFrames[i].Image.Margin.Top, 0, 0);
                }

                if ((left_D_Value >= -(showWidth - fusePixel) && left_D_Value < 0) || (left_D_Value_BEF >= -(showWidth - fusePixel) && left_D_Value_BEF < 0))
                {
                    showKeyFrames[i].CounterPartImage.Margin = counterPartMagin;
                }
                showKeyFrames[i].Image.Margin = magin;

            }

            sign++;
            if ((showWidth - fusePixel) == sign)
            {
                timerOut.Stop();
                for (int i = 1; i < keyFrames.Count; i += 2)
                {
                    if (keyFrames[i].Level == false && keyFrames[i].IsVisibleInTile == false)
                    {
                        keyFrames[i].Image.Margin = new Thickness(keyFrames[i + 1].Image.Margin.Left,
                            keyFrames[i + 1].Image.Margin.Top, 0, 0);
                    }
                }
                sign = 0;
                protect = true;
                //startInsert = endInsert = 0;
            }

        }

        #endregion

        #region 平铺收缩代码集
        //查找要收缩的关键帧
        public void selectedDeleteKeyFrames(int selectedIndex)
        {
            preSelectedCount = afterSelectedCount = 0;
            startDelete = endDelete = -1;
            int idInKeyFrames = showKeyFrames[selectedIndex].ID;
            if (false == keyFrames[idInKeyFrames].Level)
                return;
            //int j = 0;
            //int k = 1;
            for (int i = 1; i != 4; i++)
            {

                if ((selectedIndex - 2 * i + 1) > 0 && false == showKeyFrames[selectedIndex - 2 * i + 1].Level 
                    && true == showKeyFrames[selectedIndex - 2 * i + 1].IsVisibleInTile)
                {
                    //keyFrames[idInKeyFrames - 2 * i + 1].Image.Visibility = Visibility.Visible;
                    //showKeyFrames.Insert(selectedIndex - j, keyFrames[idInKeyFrames - 2 * i + 1]);
                    showKeyFrames[selectedIndex - 2 * i + 1].IsVisibleInTile = false;
                    //showKeyFrames[selectedIndex - 2 * i + 1].Image.Visibility = Visibility.Hidden;
                    startDelete = selectedIndex - 2 * i + 1;
                    preSelectedCount = i;
                }
                else
                    break;
            }

            for (int i = 1; i != 4; i++)
            {
                if ((selectedIndex + 2 * i - 1) < (showKeyFrames.Count - 1) && false == showKeyFrames[selectedIndex + 2 * i - 1].Level
                    && true == showKeyFrames[selectedIndex + 2 * i - 1].IsVisibleInTile)
                {
                    //keyFrames[idInKeyFrames + 2 * i - 1].Image.Visibility = Visibility.Visible;
                    //showKeyFrames.Insert(selectedIndex + k, keyFrames[idInKeyFrames + 2 * i - 1]);
                    showKeyFrames[selectedIndex + 2 * i - 1].IsVisibleInTile = false;
                    //showKeyFrames[selectedIndex + 2 * i - 1].Image.Visibility = Visibility.Hidden;
                    endDelete = selectedIndex + 2 * i - 1;
                    //k += 2;
                    afterSelectedCount = i;
                }
                else
                    break;
            }
            if (startDelete == -1 && endDelete != -1)
            {
                startDelete = selectedIndex + 1;
            }

            if (startDelete != -1 && endDelete == -1)
            {
                endDelete = selectedIndex - 1;
            }

        }

        //时钟响应事件，用来实现收缩动画
        public void timer_Zoomin(object sender, EventArgs e)
        {
            distanceOfTrueTimeIn = 0;
            //int rowWidth = Convert.ToInt32(left + (showWidth - fusePixel) * countInrow);

            ((InkState_Summarization)inkCollector._state).MoveTimerSecond = 0;
            for (int i = startDelete + 1; i < showKeyFrames.Count; i++)
            {
                int left_D_Value = 1;
                int left_D_Value_BEF = 1;

                if (i > endDelete)
                {
                    left_D_Value = Convert.ToInt32((showKeyFrames[i].Image.Margin.Left - preSelectedCount - afterSelectedCount)
                    - left);

                    if (left_D_Value < 0)
                    {
                        if (left_D_Value <= -(showWidth - fusePixel))
                        {
                            magin = new Thickness(left_D_Value + rowWidth, showKeyFrames[i].Image.Margin.Top - ShowHeight, 0, 0);
                            showKeyFrames[i].CounterPartImage.Visibility = Visibility.Hidden;
                        }
                        else if (left_D_Value > -(showWidth - fusePixel))
                        {
                            magin = new Thickness(showKeyFrames[i].Image.Margin.Left - preSelectedCount - afterSelectedCount,
                                showKeyFrames[i].Image.Margin.Top, 0, 0);
                            counterPartMagin = new Thickness(left_D_Value + rowWidth, showKeyFrames[i].Image.Margin.Top - showHeight, 0, 0);

                            if (showKeyFrames[i].CounterPartImage.Visibility == Visibility.Hidden)
                            {
                                showKeyFrames[i].CounterPartImage.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                        magin = new Thickness(showKeyFrames[i].Image.Margin.Left - preSelectedCount - afterSelectedCount,
                            showKeyFrames[i].Image.Margin.Top, 0, 0);
                }
                else
                {
                    if (1 == (i - startDelete) % 2)
                        distanceOfTrueTimeIn++;

                    left_D_Value_BEF = Convert.ToInt32((showKeyFrames[i].Image.Margin.Left - distanceOfTrueTimeIn) - left);

                    if (left_D_Value_BEF < 0)
                    {
                        if (left_D_Value_BEF <= -(showWidth - fusePixel))
                        {
                            magin = new Thickness(left_D_Value_BEF + rowWidth, showKeyFrames[i].Image.Margin.Top - ShowHeight, 0, 0);
                            showKeyFrames[i].CounterPartImage.Visibility = Visibility.Hidden;
                        }
                        else if (left_D_Value_BEF > -(showWidth - fusePixel))
                        {
                            magin = new Thickness(showKeyFrames[i].Image.Margin.Left - distanceOfTrueTimeIn, showKeyFrames[i].Image.Margin.Top, 0, 0);
                            counterPartMagin = new Thickness(left_D_Value_BEF + rowWidth, showKeyFrames[i].Image.Margin.Top - showHeight, 0, 0);
                            if (showKeyFrames[i].CounterPartImage.Visibility == Visibility.Hidden)
                            {
                                showKeyFrames[i].CounterPartImage.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                        magin = new Thickness(showKeyFrames[i].Image.Margin.Left - distanceOfTrueTimeIn, showKeyFrames[i].Image.Margin.Top, 0, 0);
                }
                if ((left_D_Value_BEF > -(showWidth - fusePixel) && left_D_Value_BEF < 0)|| (left_D_Value < 0&&left_D_Value > -(showWidth - fusePixel)))
                {
                    showKeyFrames[i].CounterPartImage.Margin = counterPartMagin;
                    
                }
                showKeyFrames[i].Image.Margin = magin;
            }

            sign++;
            if ((showWidth - fusePixel) == sign)
            {
                timerIn.Stop();
                for (int i = 0; i < keyFrames.Count; i++)
                {
                    if (keyFrames[i].Level == false && keyFrames[i].IsVisibleInTile == false)
                    {
                        keyFrames[i].Image.Visibility = Visibility.Hidden;
                        keyFrames[i].Image.Margin = new Thickness(keyFrames[i + 1].Image.Margin.Left,
                            keyFrames[i + 1].Image.Margin.Top, 0, 0);
                    }
                }
                int k = startDelete;
                int j = preSelectedCount + afterSelectedCount;
                for (int i = 0; i != j; i++)
                {
                    showKeyFrames.RemoveAt(k++);
                }
                sign = 0;
                protect = true;
                //startInsert = endInsert = 0;
            }
        }

        //删除收缩动画时的隐藏帧
        //public void deleteZoomInKeyFrames(int deleteIndex, int deleteCount)  
        //{
        //    for (int i = 0; i != deleteCount; i++)
        //    {
        //        showKeyFrames.RemoveAt(deleteIndex++);
        //    }
        //}
        #endregion

    }
}
