using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using WPFInk.ink;
using WPFInk.tool;
using System.Windows;
using System.Windows.Media;
using System.Windows.Ink;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFInk.state;

namespace WPFInk.videoSummarization
{
    public class TapestrySummarization:VideoSummarization
    {
        private int fuseLeftRight = 30;
        private int fuseTopBottom = 50;
        private double secondLineTop;
        private double firstLineTop;
        private int zoomLayout = 0;//表示显示的摘要信息具体到第几层，0代表显示的帧数出初始状态24帧，1代表zoomIn进去一层48帧，2代表ZoomIn进去两层96帧
        private TimeBar timebar;
        private double width;// 整个摘要的宽度
        private InkCanvas parentInkcanvas;
        private InkCollector inkCollector;
        bool isZoomIng = false;//是否在zoom过程中
        private double MoveTime = 2;//zoomIn和zoomOut动画时间长度
        public bool IsZoomIng
        {
            get { return isZoomIng; }
            set { isZoomIng = value; }
        }
        public InkCanvas ParentInkcanvas
        {
            get { return parentInkcanvas; }
            set { parentInkcanvas = value; }
        }


        public InkCollector InkCollector
        {
            get { return inkCollector; }
            set { inkCollector = value; }
        }

        public TimeBar Timebar
        {
            get { return timebar; }
            set { timebar = value; }
        }
        public double Width
        {
            get { return width; }
            set { width = value; }
        }
        public TapestrySummarization(InkCanvas _inkCanvas, List<KeyFrame> keyframes)
            : base(null, 0, keyframes, _inkCanvas)
        {
            for (int i = 0; i < keyFrames.Count; i++)
            {
                keyFrames[i].Image = InkConstants.getImageFromName(keyframes[i].ImageName);
                showWidth = 150;
                showHeight = 100;
                MathTool.getInstance().resizeImage(keyFrames[i], showWidth, showHeight);
            }
            firstLineTop = 0;
            secondLineTop = firstLineTop+showHeight - fuseTopBottom/2;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            addImages2Track();
            sw.Stop();
            Console.WriteLine("Tapestry总需要时间：" + sw.ElapsedMilliseconds + "ms");
            GC.Collect();
            this.width = 13 * (showWidth-fuseLeftRight)+fuseLeftRight;
        }
        public void setTimebar(TimeBar t)
        {
            this.timebar = t;
        }
        public override void addImages2Track()
        {
            fuseImages(0, 0);
            showKeyFrames.Add(keyFrames[0]);
            int i;
            int j;
            for (i = 0; i < 96; i +=8)
            {
                for (j = i + 1; j <= i + 8; j++)
                {
                    if (j % 2 == 0)
                    {
                        fuseImages(j, i/4 + 2);
                    }
                    else
                    {
                        fuseImages(j, i/4 + 1);
                    }
                }
                showKeyFrames.Add(keyFrames[i+7]);
                showKeyFrames.Add(keyFrames[i+8]);
            }
        }
        //图片的融合
        public void fuseFirstImages()
        {
            System.Drawing.Bitmap bitmapSource = new System.Drawing.Bitmap(keyFrames[0].ImageName);
            BitmapData data = bitmapSource.LockBits(new System.Drawing.Rectangle(0, 0, bitmapSource.Width, bitmapSource.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int w = bitmapSource.Width, h = bitmapSource.Height;
            int bytes = w * h * 4;
            byte[] ArgbValues = new byte[bytes];
            IntPtr ptr = data.Scan0;
            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, ArgbValues, 0, bytes);
            //融合右边
            for (int i = w-fuseLeftRight; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int indexBmp = 4 * (data.Width * j + i);
                    double rate = (double)(w - i) / fuseLeftRight;
                    ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                }
            }
            //融合下边
            for (int i = 0; i < w; i++)
            {
                for (int j = h-fuseTopBottom; j < h; j++)
                {
                    int indexBmp = 4 * (data.Width * j + i);
                    double rate = (double)(h - j) / fuseTopBottom;
                    ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(ArgbValues, 0, ptr, bytes);
            System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height, bitmapSource.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
            BitmapImage bitmapImage = ImageTool.getInstance().BitmapToImageSource(bitmapTarget);
            keyFrames[0].Image.Source = bitmapImage;
            bitmapSource.UnlockBits(data);
            keyFrames[0].Image.Margin = new Thickness(0, firstLineTop, 0, 0);
            InkCanvas.Children.Add(keyFrames[0].Image);

        }
        //图片的融合
        public void fuseSecondImages(int index,bool isExist)
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
            //融合右边
            for (int i = w - fuseLeftRight; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int indexBmp = 4 * (data.Width * j + i);
                    double rate = (double)(w - i) / fuseLeftRight;
                    ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                }
            }
            //融合上边
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < fuseTopBottom; j++)
                {
                    int indexBmp = 4 * (data.Width * j + i);
                    double rate = (double)j / fuseTopBottom;
                    ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(ArgbValues, 0, ptr, bytes);
            System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height, bitmapSource.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
            BitmapImage bitmapImage = ImageTool.getInstance().BitmapToImageSource(bitmapTarget);
            keyFrames[index].Image.Source = bitmapImage;
            bitmapSource.UnlockBits(data);
            //keyFrames[index].Image.Margin = new Thickness((showWidth - fuseLeftRight) / 2, secondLineTop, 0, 0);
            if (!isExist)
            {
                InkCanvas.Children.Add(keyFrames[index].Image);
            }


        }
        /// <summary>
        /// //图片的融合
        /// </summary>
        /// <param name="index">此帧在关键帧列表中的索引</param>
        /// <param name="currPositonIndex">此帧在织锦摘要中的位置索引</param>
        public void fuseImages(int index,int currPositonIndex)
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
            //融合左边
            if (currPositonIndex != 0 && currPositonIndex != 1)
            {
                for (int i = 0; i < fuseLeftRight; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        int indexBmp = 4 * (data.Width * j + i);
                        double rate = (double)i / (fuseLeftRight - 1);
                        ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                    }
                }
            }
            //融合右边
            if (currPositonIndex != keyFrameCount - 2 && currPositonIndex != keyFrameCount - 1)
            {
                for (int i = w - fuseLeftRight; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        int indexBmp = 4 * (data.Width * j + i);
                        double rate = (double)(w - i) / fuseLeftRight;
                        ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                    }
                }
            }
            if (index % 2 == 0)
            {
                //融合下面
                for (int i = 0; i < w; i++)
                {
                    for (int j = h - fuseTopBottom; j < h; j++)
                    {
                        int indexBmp = 4 * (data.Width * j + i);
                        double rate = (double)(h - j) / fuseTopBottom;
                        ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                    }
                }
            }
            else
            {
                //融合上面
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < fuseTopBottom; j++)
                    {
                        int indexBmp = 4 * (data.Width * j + i);
                        double rate = (double)j / fuseTopBottom;
                        ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                    }
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(ArgbValues, 0, ptr, bytes);
            System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height, bitmapSource.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
            BitmapImage bitmapImage = ImageTool.getInstance().BitmapToImageSource(bitmapTarget);
            keyFrames[index].Image.Source = bitmapImage;
            bitmapSource.UnlockBits(data);
            setPosition(index,currPositonIndex);
            InkCanvas.Children.Add(keyFrames[index].Image);

        }
        /// <summary>
        /// 把索引为index的关键帧放在的currPositionIndex的位置上
        /// </summary>
        /// <param name="index"></param>
        /// <param name="currPositonIndex"></param>
        private void setPosition(int index, int currPositonIndex)
        {
            if (currPositonIndex % 2 == 0)
            {
                keyFrames[index].Image.Margin = new Thickness(currPositonIndex / 2 * (showWidth - fuseLeftRight), firstLineTop, 0, 0);
            }
            else
            {
                keyFrames[index].Image.Margin = new Thickness(currPositonIndex / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2, secondLineTop, 0, 0);
            }
        }
        /// <summary>
        /// 从平铺摘要获取选择的关键帧
        /// </summary>
        /// <param name="point"></param>
        public override int getSelectedKeyFrameIndex(Point point)
        {
            if (point.Y > InkCanvas.Margin.Top && point.Y <= InkCanvas.Margin.Top + InkCanvas.Height)
            {
                double left = point.X - InkCanvas.Margin.Left;
                int index=int.MinValue;
                if (point.Y <= InkCanvas.Margin.Top + InkCanvas.Height / 2)
                {
                    index = (int)left / (int)(showWidth - fuseLeftRight) * 2;
                }
                else if (point.X > (showWidth - fuseLeftRight) / 2) 
                {
                    left += (showWidth - fuseLeftRight) / 2;
                    index = (int)left / (int)(showWidth - fuseLeftRight) * 2-1;
                }
                if (index < 0 || index >= showKeyFrames.Count)
                {
                    return int.MinValue;
                }
                return index;
            }
            return int.MinValue;
        }
        /// <summary>
        /// 向轨迹要上添加圆点代表播放到该关键帧,表明播放进度
        /// </summary>
        /// <param name="index">对应关键帧索引</param>
        /// <param name="color">笔迹颜色</param>
        /// <param name="strokeWidthOffset">相对应螺旋线笔迹的宽度差异,默认为0</param>
        public override Stroke AddPoint2Track(int index, Color color, double strokeWidthOffset)
        {
            return null; 
        }
        public void ZoomIn(int currIndex)
        {
            if (!isZoomIng)
            {
                if (0 == zoomLayout)
                {
                    showKeyFrames.Insert(1, keyFrames[3]);
                    showKeyFrames.Insert(2, keyFrames[4]);
                    fuseSecondImages(7, true);
                    for (int i = 5; i < 97; i++)
                    {
                        if (i % 2 == 0)
                        {
                            if ((i - 2) % 4 == 0)
                            {
                                moveStoryBoard(i, 0, (i / 2 + 1) / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left, currIndex*4);

                            }
                            else
                            {
                                moveStoryBoard(i, 0, (i / 2) / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left, currIndex*4);
                                if (i / 4 % 2 == 1)
                                {
                                    showKeyFrames.Insert(i / 2, keyFrames[i]);
                                }
                            }
                        }
                        else
                        {
                            if ((i - 1) % 4 == 0)
                            {
                                moveStoryBoard(i, 0, (i / 2 + 1) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left, (currIndex+1)*4);
                            }
                            else
                            {
                                moveStoryBoard(i, 0, (i / 2) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left, (currIndex + 1) * 4);
                                if ((i + 1) / 4 % 2 == 1)
                                {
                                    showKeyFrames.Insert(i / 2, keyFrames[i]);
                                }
                            }
                        }
                    }
                    updateTimebarStoryBoardIn();
                    isZoomIng = true;
                }
                else if (1 == zoomLayout)
                {
                    showKeyFrames.Insert(1, keyFrames[1]);
                    showKeyFrames.Insert(2, keyFrames[2]);
                    fuseSecondImages(3, true);
                    for (int i = 3; i < 97; i++)
                    {
                        if (i % 2 == 0)
                        {
                            if ((i - 2) % 4 == 0)
                            {
                                moveStoryBoard(i, (i / 2 + 1) / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left,
                                    i / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left, currIndex*2);
                                showKeyFrames.Insert(i, keyFrames[i]);
                            }
                            else
                            {
                                moveStoryBoard(i,  (i / 2) / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left,
                                    i / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left, currIndex*2);
                                
                            }
                        }
                        else
                        {
                            if ((i - 1) % 4 == 0)
                            {
                                moveStoryBoard(i, (i / 2 + 1) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left
                                    , (i) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left, currIndex*2+1);
                                showKeyFrames.Insert(i, keyFrames[i]);
                            }
                            else
                            {
                                moveStoryBoard(i, (i / 2) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left,
                                    (i) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left, currIndex * 2 + 1);
                                
                            }
                        }
                    }
                    updateTimebarStoryBoardIn();
                    isZoomIng = true;
                }
            }
        }
        public void ZoomOut(int currIndex)
        {
            if (!isZoomIng)
            {
                if (1 == zoomLayout)
                {
                    fuseSecondImages(7, true);
                    for (int i = 96; i >= 3; i--)
                    {
                        if (i % 2 == 0)
                        {
                            if ((i - 2) % 4 == 0)
                            {
                                moveStoryBoard(i, (i / 2 + 1) / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left,
                                    0, currIndex*2);
                            }
                            else
                            {
                                moveStoryBoard(i, (i / 2) / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left,
                                    0, currIndex*2);

                            } 
                            if (i % 4 == 0 && i % 8 != 0)
                            {
                                showKeyFrames.RemoveAt(i / 2);
                            }
                        }
                        else
                        {
                            if ((i - 1) % 4 == 0)
                            {
                                moveStoryBoard(i, (i / 2 + 1) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left,
                                    0, currIndex * 2 + 1);
                            }
                            else
                            {
                                moveStoryBoard(i, (i / 2) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left,
                                    0, currIndex * 2 + 1);
                                
                            }
                            if ((i + 1) % 4 == 0 && (i + 1) % 8 != 0)
                            {
                                showKeyFrames.RemoveAt((i - 1) / 2);
                            }
                        }
                    }
                    updateTimebarStoryBoardOut();
                    isZoomIng = true;
                }
                else if (2 == zoomLayout)
                {
                    fuseSecondImages(3, true);
                    for (int i = 96; i >= 5; i--)
                    {
                        if (i % 2 == 0)
                        {
                            if ((i - 2) % 4 == 0)
                            {
                                moveStoryBoard(i,i / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left,
                                    (i / 2 + 1) / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left, currIndex);
                                if (i % 4 != 0)
                                {
                                    showKeyFrames.RemoveAt(i);
                                }
                            }
                            else
                            {
                                moveStoryBoard(i, i / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left,
                                    (i / 2) / 2 * (showWidth - fuseLeftRight) - keyFrames[i].Image.Margin.Left, currIndex);

                            }
                        }
                        else
                        {
                            if ((i - 1) % 4 == 0)
                            {
                                moveStoryBoard(i, i / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left
                                    , (i / 2 + 1) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left, currIndex);
                                if ((i + 1) % 4 != 0)
                                {
                                    showKeyFrames.RemoveAt(i);
                                }
                            }
                            else
                            {
                                moveStoryBoard(i, i / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left,
                                    (i / 2) / 2 * (showWidth - fuseLeftRight) + (showWidth - fuseLeftRight) / 2 - keyFrames[i].Image.Margin.Left, currIndex);

                            }
                        }
                    }
                    showKeyFrames.RemoveAt(2);
                    showKeyFrames.RemoveAt(1);
                    updateTimebarStoryBoardOut();
                    isZoomIng = true;
                }
            }
        }
        double InkCanvasOffsetX=0;
        double preInkCanvasOffsetX = 0;
        public void moveStoryBoard(int index,double startLeft, double endLeft,int currIndex)
        {
            var tg = new TransformGroup();
            var translation = new TranslateTransform();
            var translationName = "myTranslation" + translation.GetHashCode();
            InkCanvas.RegisterName(translationName, translation);
            tg.Children.Add(translation);
            keyFrames[index].Image.RenderTransform = tg;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = startLeft;// keyFrames[index].Image.Margin.Left;
            myDoubleAnimation.To = endLeft;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(MoveTime));

            var s = new Storyboard();
            Storyboard.SetTargetName(s, translationName);
            Storyboard.SetTargetProperty(s, new PropertyPath(TranslateTransform.XProperty));
            var storyboardName = "s" + s.GetHashCode();
            s.Children.Add(myDoubleAnimation);
            s.Begin(InkCanvas);

            //摘要左边距动画
            if (index == currIndex)
            {
                var tgInkcanvas = new TransformGroup();
                var translationInkcanvas = new TranslateTransform();
                var translationNameInkcanvas = "myTranslationInkcanvas" + translationInkcanvas.GetHashCode();
                parentInkcanvas.RegisterName(translationNameInkcanvas, translationInkcanvas);
                tgInkcanvas.Children.Add(translationInkcanvas);
                InkCanvas.RenderTransform = tgInkcanvas;
                DoubleAnimation myDoubleAnimationInkcanvas = new DoubleAnimation();
                myDoubleAnimationInkcanvas.From = 0;
                InkCanvasOffsetX = startLeft - endLeft;
                //InkCanvasOffsetX = ((startLeft==360||startLeft==240)&&InkCanvasOffsetX > 0 ? 0 : InkCanvasOffsetX);
                myDoubleAnimationInkcanvas.To = InkCanvasOffsetX;

                myDoubleAnimationInkcanvas.Duration = new Duration(TimeSpan.FromSeconds(MoveTime));
                Storyboard.SetTargetName(myDoubleAnimationInkcanvas, translationNameInkcanvas);
                Storyboard.SetTargetProperty(myDoubleAnimationInkcanvas, new PropertyPath(TranslateTransform.XProperty));
                var sInkcanvas = new Storyboard();
                var storyboardNameInkcanvas = "s" + s.GetHashCode();
                sInkcanvas.Children.Add(myDoubleAnimationInkcanvas);
                sInkcanvas.Completed += new EventHandler(sInkcanvas_Completed);
                sInkcanvas.Begin(parentInkcanvas);
            }
        }
        

        void sInkcanvas_Completed(object sender, EventArgs e)
        {
            TransformGroup tg2 = (TransformGroup)(InkCanvas.RenderTransform);
            if (tg2.Children.Count > 0)
            {
                tg2.Children.Clear();
                InkCanvas.Margin = new Thickness(InkCanvas.Margin.Left + InkCanvasOffsetX, InkCanvas.Margin.Top, 0, 0);
            }
            ((InkState_TapestrySummarization)inkCollector._state).MoveTimerSecond = 0;
            ((InkState_TapestrySummarization)inkCollector._state).CurrIndex = int.MinValue;
        }
        double ShowTimerbarOffsetX = 0;
        public void updateTimebarStoryBoardIn()
        {
            //时间轴动画
            var tg = new TransformGroup();
            var translation = new TranslateTransform();
            var translationName = "myTranslation" + translation.GetHashCode();
            timebar.RegisterName(translationName, translation);
            tg.Children.Add(translation);
            timebar.ShowTimeBar.RenderTransform = tg;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;// keyFrame.Image.Margin.Left;
            double rate = (-(0 == zoomLayout ? InkCanvasOffsetX : InkCanvasOffsetX - preInkCanvasOffsetX) - InkCanvas.Margin.Left) / ((0 == zoomLayout ? 25 : 37) * 120 + 30);
            rate=(rate>=1?0.9:rate);
            ShowTimerbarOffsetX =  rate* timebar.Width - timebar.ShowTimeBar.Margin.Left;
            preInkCanvasOffsetX=(0==zoomLayout?InkCanvasOffsetX:0);
            ShowTimerbarOffsetX = (ShowTimerbarOffsetX < 0 ? -timebar.ShowTimeBar.Margin.Left : ShowTimerbarOffsetX);
            myDoubleAnimation.To = ShowTimerbarOffsetX;

            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(MoveTime));
            Storyboard.SetTargetName(myDoubleAnimation, translationName);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(TranslateTransform.XProperty));

            timebar.RegisterName("showTimebar", timebar.ShowTimeBar);
            DoubleAnimation myDoubleAnimationWidth = new DoubleAnimation();
            myDoubleAnimationWidth.From = timebar.ShowTimeBar.Width;
            double toWidth = parentInkcanvas.ActualWidth / ((0 == zoomLayout ? 25 : 37) * 120 + 30) * timebar.Width;
            toWidth = (ShowTimerbarOffsetX + timebar.ShowTimeBar.Margin.Left+toWidth>timebar.Width?
                timebar.Width-timebar.ShowTimeBar.Margin.Left-ShowTimerbarOffsetX:toWidth);
            toWidth = (toWidth < 0 ? 10 : toWidth);
            toWidth = (ShowTimerbarOffsetX + timebar.ShowTimeBar.Margin.Left + toWidth > timebar.TotalTimeBar.Width ?
                timebar.TotalTimeBar.Width - ShowTimerbarOffsetX - timebar.ShowTimeBar.Margin.Left : toWidth);
            myDoubleAnimationWidth.To = toWidth;

            myDoubleAnimationWidth.Duration = new Duration(TimeSpan.FromSeconds(MoveTime));
            Storyboard.SetTargetName(myDoubleAnimationWidth, timebar.ShowTimeBar.Name);
            Storyboard.SetTargetProperty(myDoubleAnimationWidth, new PropertyPath(Rectangle.WidthProperty));
            var s = new Storyboard();
            var storyboardName = "s" + s.GetHashCode();
            s.Children.Add(myDoubleAnimation);
            s.Children.Add(myDoubleAnimationWidth);
            s.Completed += new EventHandler(sIn_Completed);
            s.Begin(timebar);
        }
        public void updateTimebarStoryBoardOut()
        {
            var tg = new TransformGroup();
            var translation = new TranslateTransform();
            var translationName = "myTranslation" + translation.GetHashCode();
            timebar.RegisterName(translationName, translation);
            tg.Children.Add(translation);
            timebar.ShowTimeBar.RenderTransform = tg;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;// keyFrame.Image.Margin.Left;
            double rate = -((2 == zoomLayout ? 0 : InkCanvasOffsetX) + InkCanvas.Margin.Left) / ((1 == zoomLayout ? 25 : 37) * 120 + 30);
            rate = (rate >= 1 ? 0.9 : rate);
            preInkCanvasOffsetX = (2 == zoomLayout ? InkCanvasOffsetX : 0);
            ShowTimerbarOffsetX =  rate* timebar.Width - timebar.ShowTimeBar.Margin.Left;
            //对左边界进行特殊处理
            ShowTimerbarOffsetX = (InkCanvas.Margin.Left > 0 || InkCanvas.Margin.Left + InkCanvasOffsetX > 0 ? -timebar.ShowTimeBar.Margin.Left : ShowTimerbarOffsetX);
            //对右边界进行特殊处理

            myDoubleAnimation.To = ShowTimerbarOffsetX;

            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(MoveTime));
            Storyboard.SetTargetName(myDoubleAnimation, translationName);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(TranslateTransform.XProperty));

            timebar.RegisterName("showTimebar", timebar.ShowTimeBar);
            DoubleAnimation myDoubleAnimationWidth = new DoubleAnimation();
            myDoubleAnimationWidth.From = parentInkcanvas.ActualWidth / ((1 == zoomLayout ? 25 : 37) * 120 + 30) * timebar.Width;
            double toWidth = parentInkcanvas.ActualWidth / ((1 == zoomLayout ? 13 : 25) * 120 + 30) * timebar.Width;
            toWidth=(toWidth<0?10:toWidth);
            toWidth = (ShowTimerbarOffsetX + timebar.ShowTimeBar.Margin.Left+toWidth>timebar.TotalTimeBar.Width?
                timebar.TotalTimeBar.Width-ShowTimerbarOffsetX - timebar.ShowTimeBar.Margin.Left:toWidth);
            myDoubleAnimationWidth.To = toWidth;

            myDoubleAnimationWidth.Duration = new Duration(TimeSpan.FromSeconds(MoveTime));
            Storyboard.SetTargetName(myDoubleAnimationWidth, timebar.ShowTimeBar.Name);
            Storyboard.SetTargetProperty(myDoubleAnimationWidth, new PropertyPath(Rectangle.WidthProperty));
            var s = new Storyboard();
            var storyboardName = "s" + s.GetHashCode();
            s.Children.Add(myDoubleAnimation);
            s.Children.Add(myDoubleAnimationWidth);
            s.Completed += new EventHandler(sOut_Completed);
            s.Begin(timebar);

        }
        void sIn_Completed(object sender, EventArgs e)
        {
            width = (showKeyFrames.Count + 1) / 2 * 120 + 30;
            timebar.TotalTime = width;
            TransformGroup tg2 = (TransformGroup)(timebar.ShowTimeBar.RenderTransform);
            if (tg2.Children.Count > 0)
            {
                tg2.Children.Clear();
                timebar.ShowTimeBar.Margin = new Thickness(timebar.ShowTimeBar.Margin.Left + ShowTimerbarOffsetX, timebar.ShowTimeBar.Margin.Top, 0, 0);
            }
            isZoomIng = false;

            zoomLayout++;
            

        }
        void sOut_Completed(object sender, EventArgs e)
        {
            width = (showKeyFrames.Count + 1) / 2 * 120 + 30;
            timebar.TotalTime = width;
            TransformGroup tg2 = (TransformGroup)(timebar.ShowTimeBar.RenderTransform);
            if (tg2.Children.Count > 0)
            {
                tg2.Children.Clear();
                timebar.ShowTimeBar.Margin = new Thickness(timebar.ShowTimeBar.Margin.Left + ShowTimerbarOffsetX, timebar.ShowTimeBar.Margin.Top, 0, 0);
            }
            isZoomIng = false;
            zoomLayout--;
        }
    }
}
