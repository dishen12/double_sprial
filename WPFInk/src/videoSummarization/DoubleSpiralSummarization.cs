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
using System.Windows.Media.Animation;
using WPFInk.Global;

namespace WPFInk.videoSummarization
{
    /// <summary>
    /// 螺旋视频摘要类
    /// </summary>
    public class DoubleSpiralSummarization : VideoSummarization
    {
        #region 私有变量
        private MyDoubleSpiral myDoubleSpiral;//螺旋线        
        private List<Point> keyFrameCenterPoints = new List<Point>();//关键帧中心点列表
        private System.Windows.Forms.Timer Timer;
        private StylusPointCollection points;//螺旋线点集合
        public List<Rect> bounds = new List<Rect>();//关键帧图片位置定位边界
        public List<Rect> boundsForMove = new List<Rect>();//关键帧图片位置定位边界,用作初始化和移动时使用

        private StylusPoint leftCenter;
        private StylusPoint rightCenter;
        //private double extendDuration = 0.5;//插入关键帧动画时间长度
        private double myDoubleSpiralStrokeWidth;//螺旋线宽度
        private bool isClockWise;
        private double myDoubleSpiralWidth;//螺距
        private double centerX;
        private double centerY;
        private double newSpiralWidth;
        private double lastKeyPointAngle = 0;//最后关键帧对应第二个关键点与圆心的角度
        private int showCount = 0;//要显示的关键帧数量，隐藏的不算
        //private List<Point> showKeyFrameCenterPoints = new List<Point>();
        //private int insertIndex = 53;
        private List<int> endIndexes = new List<int>();
        private List<int> keyPointCenterFocusIndexes = new List<int>();//中心聚集的关键点索引列表
        private List<int> keyPointMarginFocusIndexes = new List<int>();//边缘聚集的关键点索引列表

        private Stroke showSpiralStroke;//显示出来的螺旋线
        private int pointsCount = 0;
        private int spiralPointsCount = 0;
        private InkCollector _inkCollector;
        private int minFocusKeyFrameCount = 170;//超过这个值的时候会出现中心聚集和边缘聚集
        private bool isFocus;//是否中心或边缘聚集
        private bool isShowHalf = true;//是否显示一半关键帧，奇数关键帧隐藏
        private bool isZoomOut = false;//是否是边缘聚集,true代表现在处于边缘聚集状态，false代表处于中心聚集状态
        private bool isReadKeyFramesFromCard = false;//在生成螺旋摘要时是否从硬盘读取已经处理好的关键帧
        private static bool isZooming = false;
        #endregion

        #region 构造函数
        public DoubleSpiralSummarization(InkCollector inkCollector, MyDoubleSpiral myDoubleSpiral, List<KeyFrame> keyFrames, bool isShowHalf)
            : base(myDoubleSpiral.SpiralStroke, myDoubleSpiral.SpiralWidth, keyFrames, myDoubleSpiral.InkCanvas)
        {
            this._inkCollector = inkCollector;
            this.myDoubleSpiral = myDoubleSpiral;
            this.Center = myDoubleSpiral.Center;
            this.leftCenter =myDoubleSpiral.LeftCenter;
            this.rightCenter = myDoubleSpiral.RightCenter;
            this.points = myDoubleSpiral.SpiralStroke.StylusPoints;
            this.myDoubleSpiralStrokeWidth = myDoubleSpiral.SpiralStrokeWidth;
            this.isClockWise = myDoubleSpiral.IsClockwise;
            this.myDoubleSpiralWidth = myDoubleSpiral.SpiralWidth;
            this.centerX = Center.X;
            this.centerY = Center.Y;
            this.newSpiralWidth = myDoubleSpiral.NewSpiralWidth;
            this.pointsCount = points.Count;
            this.spiralPointsCount = myDoubleSpiral.SpiralPointsCount;
            this.isShowHalf = isShowHalf;
            //if (keyFrameCount < minFocusKeyFrameCount)
            //{
            //    isFocus = false;
            //}
            //else
            //{
            //    isFocus = true;
            //    inkCollector.IsShowRedPoint = false;
            //}
            //if (isShowHalf || isFocus)
            //{
            //    inkCollector.IsShowRedPoint = false;
            //}
            //else
            //{
            //    inkCollector.IsShowRedPoint = true;
            //}
            isFocus = true;
            inkCollector.IsShowRedPoint = true;

            getAllEndIndexes((int)(myDoubleSpiralWidth*1.1));
            getKeyPoints((int)(myDoubleSpiralWidth));
            getBounds2();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start(); 
            //LoadTemplates();
            addImages2Track(); 
            sw.Stop();
            Console.WriteLine("spiral总需要时间：" + sw.ElapsedMilliseconds + "ms");
            GC.Collect();
            //Timer = new System.Windows.Forms.Timer();
            //Timer.Interval = 1000;
            //Timer.Tick += new System.EventHandler(Timer_Tick);
            //Timer.Start();
        }
        
        #endregion

        #region 封装变量


        public bool IsZooming
        {
            get { return isZooming; }
            set { isZooming = value; }
        }
        public bool IsShowHalf
        {
            get { return isShowHalf; }
            set { isShowHalf = value; }
        }
        public bool IsZoomOut
        {
            get { return isZoomOut; }
            set { isZoomOut = value; }
        }
        public bool IsFocus
        {
            get { return isFocus; }
            set { isFocus = value; }
        }

        public InkCollector InkCollector
        {
            get { return _inkCollector; }
            set { _inkCollector = value; }
        }
        public StylusPointCollection Points
        {
            get { return points; }
            set { points = value; }
        }
        public List<int> KeyPointMarginFocusIndexes
        {
            get { return keyPointMarginFocusIndexes; }
            set { keyPointMarginFocusIndexes = value; }
        }
        public List<int> KeyPointCenterFocusIndexes
        {
            get { return keyPointCenterFocusIndexes; }
            set { keyPointCenterFocusIndexes = value; }
        }


        public Stroke ShowSpiralStroke
        {
            get { return showSpiralStroke; }
            set { showSpiralStroke = value; }
        }
        public StylusPoint LeftCenter
        {
            get { return leftCenter; }
            set { leftCenter = value; }
        }
        /// <summary>
        /// 螺旋线
        /// </summary>
        public MyDoubleSpiral MyDoubleSpiral
        {
            get { return myDoubleSpiral; }
            set { myDoubleSpiral = value; }
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
        
        private int keyFrameIndex = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (keyFrameIndex < keyFrameCount)
            {
                addImage2Spiral(keyFrameIndex, keyFrames[keyFrameIndex], keyPointIndexes[keyFrameIndex], true);
                showKeyFrameCenterPoints.Add(keyFrameCenterPoints[keyPointIndexes[keyFrameIndex]]);
                showKeyFrames.Add(keyFrames[keyFrameIndex]);
                keyFrameIndex++;
            }
            else
            {
                Timer.Stop();
            }
        }
        private void getAllEndIndexes(int intervalLength)
        {
            double distance=0;
            double[] arcLength = new double[pointsCount];

            int endIndex = 0;
            //(int)(spiralPointsCount+(pointsCount - 2 * spiralPointsCount) / 4)
            int[] length = new int[8] { 270, 630, spiralPointsCount-100, spiralPointsCount+100, intervalLength * 2, (int)(intervalLength * 1.3), intervalLength, (int)(1.8*intervalLength) };
            for (int k = 0; k < pointsCount - 1; k++)
            {
                distance += MathTool.getInstance().distanceP2P(points[k + 1], points[k]);
                if ((int)distance >= length[4])
                {
                    endIndex = k + 1;
                    endIndexes.Add(endIndex);
                    break;
                }
            }
            double preLength;
            for (int i = 1; i < pointsCount - 200; i++)
            {
                if (i < length[0] || (pointsCount - i) < length[0])
                {
                    arcLength[i] = length[4];
                }
                else if (i < length[1] || (pointsCount - i) < length[1])
                {
                    arcLength[i] = length[5];
                }
                //螺旋外围
                else if (i < length[2] || (pointsCount - i) < length[2])
                {
                    arcLength[i] = length[6];
                }
                //中央连线
                else if (i < length[3])
                {
                    arcLength[i] =  (double)i*(length[7] - length[6]) / (length[3] - length[2])  + (length[6] * length[3] - length[7] * length[2]) / (length[3] - length[2]); ;
                        
                }
                else if ((pointsCount  - i) < length[3])
                {
                    arcLength[i] = (double)(pointsCount - i) * (length[7] - length[6]) / (length[3] - length[2]) + (length[6] * length[3] - length[7] * length[2]) / (length[3] - length[2]); ;

                }
                else
                {
                    arcLength[i] = length[7];
                }

                preLength = MathTool.getInstance().distanceP2P(points[i - 1], points[i]);
                distance -= preLength;
                if ((int)distance >= arcLength[i])
                {
                    endIndexes.Add(endIndex);
                }
                else
                {
                    for (int k = endIndex; k < pointsCount - 1; k++)
                    {
                        distance += MathTool.getInstance().distanceP2P(points[k + 1], points[k]);
                        if ((int)distance >= arcLength[i])
                        {
                            endIndex = k + 1;
                            endIndexes.Add(endIndex);
                            break;
                        }
                    }
                }
                //if (283 == i)
               // {
                //InkTool.getInstance().drawPoint(points[endIndexes[i]].X, points[endIndexes[i]].Y, 5, Colors.Red, InkCanvas);
               // }
                if (i == length[0] || i == length[1] || i == length[2] || i == length[3] || (pointsCount - i) == length[0] || (pointsCount - i) == length[1] || (pointsCount - i) == length[2] || (pointsCount - i) == length[3])
                 {
                // InkTool.getInstance().drawPoint(points[i].X, points[i].Y, 15, Colors.Green, InkCanvas);
                 }
            }
        }
        /// <summary>
        /// 获取关键点
        /// </summary>
        /// <param name="intervalLength"></param>
        private void getKeyPoints(int intervalLength)
        {
            keyPointIndexes.Clear();
            double distance = 0;
            showCount = keyFrameCount;
            if (!isFocus)
            {
                keyPointIndexes.Add(0);
                //keyPointCenterFocusIndexes.Add(0);
                //keyPointMarginFocusIndexes.Add(0);
                //InkTool.getInstance().drawPoint(points[0].X, points[0].Y, 10, Colors.Yellow, InkCanvas);
                keyPointIndexes.Add(270);
                //keyPointCenterFocusIndexes.Add(270);
                //keyPointMarginFocusIndexes.Add(270);
                //InkTool.getInstance().drawPoint(points[270].X, points[270].Y, 10, Colors.Yellow, InkCanvas);
                keyPointIndexes.Add(360);
                //keyPointCenterFocusIndexes.Add(360);
                //keyPointMarginFocusIndexes.Add(360);
                //InkTool.getInstance().drawPoint(points[360].X, points[360].Y, 10, Colors.Yellow, InkCanvas);
                int i = 360;
                while (i < pointsCount - 200)
                {
                    keyPointIndexes.Add(endIndexes[i]);
                    keyPointMarginFocusIndexes.Add(endIndexes[i]);
                    InkTool.getInstance().drawPoint(points[endIndexes[i]].X, points[endIndexes[i]].Y, 10, Colors.Yellow, InkCanvas);
                    i=endIndexes[i];
                }
            }
            else
            {
                keyPointIndexes.Add(0);
                //中心聚集
                int i = 0;
                int half;
                for (i=270; i < points.Count - 1; i++)
                {
                    distance += MathTool.getInstance().distanceP2P(points[i + 1], points[i]);
                    half=(int)(showCount *0.05);
                    if (keyPointCenterFocusIndexes.Count <=half )
                    {
                        if (distance >= myDoubleSpiralWidth/20)
                        {
                            if (!isZoomOut)
                            {
                                keyPointIndexes.Add(i + 1);
                            }
                            keyPointCenterFocusIndexes.Add(i + 1);
                            //InkTool.getInstance().drawPoint(points[i + 1].X, points[i + 1].Y, 5, Colors.Blue, InkCanvas);
                            distance = 0;
                            i++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                //int i = 360;
                while (i<points.Count-300)
                {
                    if (!isZoomOut)
                    {
                        keyPointIndexes.Add(endIndexes[i]);
                    }
                    keyPointCenterFocusIndexes.Add(endIndexes[i]);
                    //keyPointIndexes.Add(endIndexes[i]);
                    //keyPointMarginFocusIndexes.Add(endIndexes[i]);
                    //InkTool.getInstance().drawPoint(points[endIndexes[i]].X, points[endIndexes[i]].Y, 10, Colors.Yellow, InkCanvas);
                    i = endIndexes[i];
                }
                //右中心聚集
                distance = 0;
                for(i=points.Count-300;i < points.Count - 10; i++)
                {
                    distance += MathTool.getInstance().distanceP2P(points[i + 1], points[i]);
                    if (distance >= myDoubleSpiralWidth/100)
                    {
                        keyPointIndexes.Add(i + 1);
                        distance = 0;
                    }
                }

                for (i = 1; i < 2000; i++)
                {
                    keyPointIndexes.Add(points.Count-10);
                }

                ////边缘聚集
                //if (isZoomOut)
                //{
                //    keyPointIndexes.Add(270);
                //    keyPointIndexes.Add(360);
                //}
                //i = 360;
                //half = (int)(showCount *0.5);
                //while (keyPointMarginFocusIndexes.Count <= half&&i<2310)
                //{
                //    if (isZoomOut)
                //    {
                //        keyPointIndexes.Add(endIndexes[i]);
                //    }
                //    keyPointMarginFocusIndexes.Add(endIndexes[i]);
                //    i = endIndexes[i];
                //}
                //distance = 0;
                //for (; i < points.Count - 1; i++)
                //{
                //    distance += MathTool.getInstance().distanceP2P(points[i + 1], points[i]);
                //    if (distance >= 2.9)
                //    {
                //        if (isZoomOut)
                //        {
                //            keyPointIndexes.Add(i + 1);
                //        }
                //        keyPointMarginFocusIndexes.Add(i + 1);
                //        //InkTool.getInstance().drawPoint(points[i + 1].X, points[i + 1].Y, 5, Colors.Blue, InkCanvas);
                //        distance = 0;
                //        i++;
                //    }
                //}                

            }
        }

        private const int InitTemplateWidth = 300;//初始模板宽度
        private const int InitTemplateHeight = 200;//初始模板高度
        private const int InitTemplateWidthAndHeight = InitTemplateWidth * InitTemplateHeight;
        private const int ArgbValuesLength = InitTemplateWidthAndHeight * 4;
        private const int InitTemplateWidth4 = InitTemplateWidth * 4;
        private double TemplateWidth = InitTemplateWidth;//模板宽度
        private double TemplateHeight = InitTemplateHeight;//模板高度
        //public static List<List<byte>> templates = new List<List<byte>>();//模板存储空间
        private byte[] ArgbValues = new byte[ArgbValuesLength];
        private WriteableBitmap bitmapTarget;
        private Int32Rect sourceRect;
        /// <summary>
        /// 预载入模板集合,根据计算
        /// </summary>
        private void LoadTemplates()
        {
            FileStream myStream = new FileStream(@"resource\template.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(myStream);
        
            for (int index = 2000; index < 2310; index++)
            {
                //预设值模板宽度和高度，符合bound边界
                MathTool.getInstance().resizeWidthHeight(ref TemplateWidth, ref TemplateHeight, bounds[index].Width, bounds[index].Height);
                double left = bounds[index].X + bounds[index].Width / 2 - TemplateWidth / 2;
                double top = bounds[index].Y + bounds[index].Height / 2 - TemplateHeight / 2;
                List<byte> template = new List<byte>();//初始化一个模板
                //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(keyFrame.ImageName);
                //BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(bitmap);
                double PI15 = Math.PI * 1.5;
                double PID180 = Math.PI / 180;
                //int w = bitmap.Width, h = bitmap.Height;
                int bytes = (int)(InitTemplateWidth * InitTemplateHeight * 4);
                byte[] ArgbValues = new byte[bytes];
                //bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
                double pointWidth = TemplateWidth / InitTemplateWidth, maxDistance = 0, maxi = 0, maxj = 0;
                StylusPoint startPoint = points[index];
                StylusPoint keyFrameCenterPoint = new StylusPoint(keyFrameCenterPoints[index].X, keyFrameCenterPoints[index].Y);
                double angle = MathTool.getInstance().getAngleP2P(Center, keyFrameCenterPoint);
                double centerangle = angle * PID180;
                int keyFrameCenterPointInRing = pointInRing(keyFrameCenterPoint, centerangle, false, Center);
                if (index >= 283 && index < 356)
                {
                    keyFrameCenterPointInRing++;
                }
                double angleSC;//起始关键点与中心点之间的距离和角度
                int startPointInRing, indexBmp;
                //distance1 = MathTool.getInstance().distanceP2P(startPoint, Center);//起始关键点与中心点之间的距离
                angleSC = MathTool.getInstance().getAngleP2P(Center, startPoint);
                startPointInRing = pointInRing(startPoint, angleSC * PID180, true, Center);//,startIndex); 
                double angle1 = (startPointInRing - 1) * 360 + angleSC;//起始关键点与中心点之间的角度
                double x, y, angle2;
                StylusPoint currPoint;
                double currangle, diffAngle, currDistance;
                int currPointInRing; 
                for (int i = 0; i < InitTemplateWidth; i++)
                {
                    for (int j = 0; j < InitTemplateHeight; j++)
                    {
                        indexBmp = 4 * (InitTemplateWidth * j + i);
                        ArgbValues[indexBmp + 3] = 255;
                    }
                }
                for (int i = 0; i < InitTemplateWidth; i++)
                {
                    for (int j = 0; j < InitTemplateHeight; j++)
                    {
                        x = left + i * pointWidth;
                        y = top + j * pointWidth;
                        currPoint = new StylusPoint(x, y);
                        indexBmp = 4 * (InitTemplateWidth * j + i);
                        angle = MathTool.getInstance().getAngleP2P(Center, currPoint);
                        currangle = angle * PID180;
                        currPointInRing = pointInRing(currPoint, currangle, false, Center);
                        diffAngle = centerangle - currangle;
                        if (diffAngle > PI15)
                        {
                            currPointInRing--;
                        }
                        else if (-diffAngle > PI15)
                        {
                            currPointInRing++;
                        }
                        if (currPointInRing == keyFrameCenterPointInRing)
                        {
                            if (diffAngle > PI15)
                            {
                                currPointInRing++;
                            }
                            else if (-diffAngle > PI15)
                            {
                                currPointInRing--;
                            }
                            angle2 = currPointInRing * 360 + angle;
                            if (angle1 > angle2)
                            {
                                currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, Center);
                                if (currDistance > maxDistance)
                                {
                                    maxDistance = currDistance;
                                    maxi = i;
                                    maxj = j;
                                }
                            }
                        }
                        else
                        {
                            ArgbValues[indexBmp + 3] = 0;
                        }
                    }
                }

                double rate;
                for (int i = 0; i < InitTemplateWidth; i++)
                {
                    for (int j = 0; j < InitTemplateHeight; j++)
                    {
                        indexBmp = 4 * (InitTemplateWidth * j + i);
                        if (0 != ArgbValues[indexBmp + 3])
                        {
                            rate = 1;
                            x = left + i * pointWidth;
                            y = top + j * pointWidth;
                            currPoint = new StylusPoint(x, y);
                            angle = MathTool.getInstance().getAngleP2P(Center, currPoint);
                            currangle = angle * PID180;
                            currPointInRing = pointInRing(currPoint, currangle, false, Center);
                            angle2 = currPointInRing * 360 + angle;
                            if (angle1 > angle2)
                            {
                                currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, Center);
                                rate = 1 - currDistance / maxDistance;//透明度比例
                                if (rate > 1) { rate = 1; }
                                ArgbValues[indexBmp + 3] = (byte)(255 * Math.Pow(rate, 3));
                                //对第二、三象限过渡效果进行调整
                                if (j <= 20)
                                {
                                    rate = ((double)j) / 20;
                                    ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                                }
                                //对第一、四象限过渡效果进行调整
                                else if (InitTemplateHeight - j <= 15)
                                {
                                    rate = ((double)(InitTemplateHeight - j)) / 15;
                                    ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                                }

                            }
                        }
                    }
                }
                //if (index == showCount - 1 || index == showCount - 2)
                //{

                StylusPoint endPoint = points[endIndexes[index]+5];
                int endPointInRing;
                //distance1 = MathTool.getInstance().distanceP2P(startPoint, Center);//起始关键点与中心点之间的距离
                double angleEC = MathTool.getInstance().getAngleP2P(Center, endPoint);
                endPointInRing = pointInRing(endPoint, angleEC * PID180, true, Center);//,startIndex); 
                double angleE = (endPointInRing - 1) * 360 + angleEC;//起始关键点与中心点之间的角度
                for (int i = 0; i < InitTemplateWidth; i++)
                {
                    for (int j = 0; j < InitTemplateHeight; j++)
                    {
                        indexBmp = 4 * (InitTemplateWidth * j + i);
                        if (ArgbValues[indexBmp + 3] != 0)
                        {
                            x = left + i * pointWidth; y = top + j * pointWidth;
                            currPoint = new StylusPoint(x, y);

                            angle = MathTool.getInstance().getAngleP2P(Center, currPoint);
                            currangle = angle * PID180;
                            currPointInRing = pointInRing(currPoint, currangle, false, Center);
                            angle2 = currPointInRing * 360 + angle;
                            if (angle2 > angleE)
                            {
                                ArgbValues[indexBmp + 3] = 0;
                            }
                        }
                    }
                }
                //}
                //填入模板
                for (int i = 0; i < InitTemplateWidth; i++)
                {
                    for (int j = 0; j < InitTemplateHeight; j++)
                    {
                        indexBmp = 4 * (InitTemplateWidth * j + i);
                        sWriter.Write(ArgbValues[indexBmp + 3].ToString()+" ");
                        template.Add(ArgbValues[indexBmp + 3]);
                    }
                }
                Console.WriteLine(index);
                sWriter.WriteLine();
                Global.GlobalValues.templates.Add(template);
            }
            sWriter.Close();
            sWriter.Dispose();
        }
        /// <summary>
        /// 向螺旋线中添加所有视频关键帧，即初始化螺旋摘要
        /// </summary>
        public override void addImages2Track()
        {
            if (isFocus||(!isFocus&&!isShowHalf))
            {
                //全部
                showCount = keyFrameCount;
                getLastKeyPointAngle();
                for (int i = 0; i < showCount; i++)
                {
                    addImage2Spiral(i, keyFrames[showCount-1-i], keyPointIndexes[i], true);
                    showKeyFrameCenterPoints.Add(keyFrameCenterPoints[keyPointIndexes[i]]);
                    showKeyFrames.Add(keyFrames[i]);
                }
                //生成模板
                //for (int i = 0; i < points.Count - 300; i++)
                //{
                //    addImage2Spiral(0, keyFrames[0], i, true);
                //    //showKeyFrameCenterPoints.Add(keyFrameCenterPoints[keyPointIndexes[i]]);
                //    //showKeyFrames.Add(keyFrames[i]);
                //}
            }
            else
            {
                //显示偶数
                showCount = keyFrameCount % 2 == 0 ? keyFrameCount / 2 : (keyFrameCount - 1) / 2;
                getLastKeyPointAngle();
                addImage2Spiral(0, keyFrames[0], 0, true);
                showKeyFrames.Add(keyFrames[0]);
                showKeyFrameCenterPoints.Add(keyFrameCenterPoints[0]);
                for (int i = 1; i < keyFrameCount - 1; i += 2)
                {
                    int index = (i + 1) / 2;
                    addImage2Spiral(i, keyFrames[i], keyPointIndexes[index], false);
                    keyFrames[i].IsVisible = false;
                    showKeyFrameCenterPoints.Add(keyFrameCenterPoints[keyPointIndexes[index]]);
                    showKeyFrames.Add(keyFrames[i + 1]);
                    addImage2Spiral(i + 1, keyFrames[i + 1], keyPointIndexes[index], true);
                }
                //showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                //showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                /**/
            }
        }

        private void getLastKeyPointAngle()
        {
            StylusPoint lastKeyPoint = points[keyPointIndexes[showCount]];
            //InkTool.getInstance().drawPoint(lastKeyPoint.X, lastKeyPoint.Y,5, Colors.Blue, InkCanvas);
            lastKeyPointAngle = MathTool.getInstance().getAngleP2P(Center, lastKeyPoint);
            lastKeyPointAngle = (pointInRing(lastKeyPoint, lastKeyPointAngle / 180 * Math.PI, true, keyPointIndexes[showCount], Center)) * 360 + lastKeyPointAngle - 1;
        }
        

        /// <summary>
        /// 向螺旋线中添加视频关键帧
        /// </summary>
        /// <param name="index">在螺旋摘要中的索引值</param>
        /// <param name="keyFrame"></param>
        /// <param name="StartIndex"></param>
        /// <param name="EndIndex"></param>
        /// <param name="bounds"></param>
        public void InsertImage(int index, KeyFrame keyFrame, int StartIndex)
        {
            Rect bound = bounds[StartIndex];
            double boundWidth = bound.Width;
            double boundHeight = bound.Height;
            double boundX = bound.X;
            double boundY = bound.Y;
            Image image;
            if (isReadKeyFramesFromCard)
            {
                image = InkConstants.getImageFromName(GlobalValues.FilesPath + @"\keyFrames\" + index + @"\" + index + "_" + StartIndex + ".png");
                keyFrame.Image = image;
                MathTool.getInstance().resizeImage(keyFrame, boundWidth, boundHeight);
                
            }
            else
            {
                image = InkConstants.getImageFromName(keyFrame.ImageName);
                keyFrame.Image = image;
                keyFrame.BitmapSource =ImageTool.getInstance().BitmapToBitmapSource(new System.Drawing.Bitmap(keyFrame.ImageName));
                MathTool.getInstance().resizeImage(keyFrame, boundWidth, boundHeight);

                //去掉图片多余边界
                // WriteableBitmap bitmapTarget;
                
                // 左端点在左螺旋内
                
                if (StartIndex < spiralPointsCount)
                {
                    
                    //右端点在平直轨道内
                    if (endIndexes[StartIndex] > spiralPointsCount)
                    {

                        //InkTool.getInstance().drawPoint(points[StartIndex].X, points[StartIndex].Y, 15, Colors.Red, InkCanvas);

                        bitmapTarget = RemoveRedundanceLeftTurning(index, keyFrame, StartIndex, endIndexes[StartIndex], boundX + boundWidth / 2 - keyFrame.Width / 2, boundY + boundHeight / 2 - keyFrame.Height / 2);
                    }
                    //右端点在左螺旋内
                    else
                    {
                        bitmapTarget = RemoveRedundanceClockwize(index, keyFrame, StartIndex, endIndexes[StartIndex], boundX + boundWidth / 2 - keyFrame.Width / 2, boundY + boundHeight / 2 - keyFrame.Height / 2);
                    }


                }
                // 左端点在平直轨道
                else if (StartIndex < pointsCount - spiralPointsCount)
                {
                    //右端点在平直轨道内
                    if (endIndexes[StartIndex] < pointsCount - spiralPointsCount)
                    {
                        bitmapTarget = RemoveRedundanceHorizontal(index, keyFrame, StartIndex, endIndexes[StartIndex], boundX + boundWidth / 2 - keyFrame.Width / 2, boundY + boundHeight / 2 - keyFrame.Height / 2);
                    }
                    //右端点在右螺旋内
                    else
                    {
                       // InkTool.getInstance().drawPoint(points[StartIndex].X, points[StartIndex].Y, 15, Colors.Red, InkCanvas);
                        bitmapTarget = RemoveRedundanceRightTurning(index, keyFrame, StartIndex, endIndexes[StartIndex], boundX + boundWidth / 2 - keyFrame.Width / 2, boundY + boundHeight / 2 - keyFrame.Height / 2);
                    }
                }    
                //左端点在右螺旋内    
                else
                {
                    //bitmapTarget = RemoveRedundanceByTemplate( keyFrame, StartIndex);
                    bitmapTarget = RemoveRedundance(index, keyFrame, StartIndex, endIndexes[StartIndex], boundX + boundWidth / 2 - keyFrame.Width / 2, boundY + boundHeight / 2 - keyFrame.Height / 2);
                }




                image.Source = (BitmapSource)bitmapTarget;
            }
            //调整图片大小
            keyFrame.PointIndexInSpiral = StartIndex;
           
            //添加图片
            // if(index==1)
            addPicture(boundX + boundWidth / 2 - keyFrame.Width / 2, boundY + boundHeight / 2 - keyFrame.Height / 2, keyFrame);
            
        }
        
        /// <summary>
        /// 修改视频关键帧图片
        /// </summary>
        /// <param name="index">在螺旋摘要中的索引值</param>
        /// <param name="keyFrame"></param>
        /// <param name="StartIndex"></param>
        /// <param name="EndIndex"></param>
        /// <param name="bounds"></param>
        public void UpdateImage(int index, KeyFrame keyFrame, int StartIndex)
        {
            Rect bound = bounds[StartIndex];
            double boundWidth = bound.Width;
            double boundHeight = bound.Height;
            double boundX = bound.X;
            double boundY = bound.Y; 
            //Image image;
            if (isReadKeyFramesFromCard)
            {
                string s = GlobalValues.FilesPath + @"\keyFrames\" + index + @"\" + index + "_" + StartIndex + ".png";
                BitmapImage image1 = new BitmapImage(new Uri(s, UriKind.RelativeOrAbsolute));
                keyFrame.Image.Source = null;
                keyFrame.Image.Source = image1;
                //调整图片大小
                MathTool.getInstance().resizeImage(keyFrame, boundWidth, boundHeight);
            }
            else
            {
                MathTool.getInstance().resizeImage(keyFrame, boundWidth, boundHeight);

                //去掉图片多余边界
               // WriteableBitmap bitmapTarget;
                if (isClockWise)
                {
                    bitmapTarget = RemoveRedundanceClockwize(index, keyFrame, StartIndex, endIndexes[StartIndex], boundX + boundWidth / 2 - keyFrame.Width / 2, boundY + boundHeight / 2 - keyFrame.Height / 2);
                }
                else
                {
                    bitmapTarget = RemoveRedundanceByTemplate(keyFrame, StartIndex);

                } 
                keyFrame.Image.Source = null;
                keyFrame.Image.Source = (BitmapSource)bitmapTarget;
            }
            //keyFrame.PointIndexInSpiral = StartIndex;
            keyFrame.Image.Margin = new Thickness(boundX + boundWidth / 2 - keyFrame.Width / 2, boundY + boundHeight / 2 - keyFrame.Height / 2, 0, 0);
            
        }
        /// <summary>
        /// 获取对应Index的外包框
        /// </summary>
        /// <param name="index">关键帧位置</param>
        /// <param name="StartIndex"></param>
        /// <param name="EndIndex"></param>
        public Rect getBound(int StartIndex, int EndIndex)
        {
            StylusPointCollection fourPoints = new StylusPointCollection();
            Rect bound;

            StylusPoint pointStart = points[StartIndex];
            StylusPoint pointEnd = points[EndIndex];

            fourPoints.Add(pointStart);
            fourPoints.Add(pointEnd);
            if (StartIndex < 180)
            {
                fourPoints.Add(points[0]);
                fourPoints.Add(points[90]);
                fourPoints.Add(points[180]);
                fourPoints.Add(points[270]);
            }
            //else if(StartIndex>720)
            //{
            //    fourPoints.Add(points[(StartIndex + (StartIndex + EndIndex) / 2) / 2]);
            //    fourPoints.Add(points[(StartIndex + EndIndex) / 2]);
            //    fourPoints.Add(points[((StartIndex + EndIndex) / 2 + EndIndex) / 2]);
            //}
            double pointStartX = pointStart.X;
            double pointStartY = pointStart.Y;
            double pointEndX = pointEnd.X;
            double pointEndY = pointEnd.Y;
            if (pointEndX != pointStartX)
            {
                double k = (pointEndY - pointStartY) / (pointEndX - pointStartX);//斜率
                double kk = k * k;
                double g = myDoubleSpiralWidth * 1.25 * Math.Sqrt(kk / (1 + kk));
                double x31 = pointStartX + g;
                double y31 = k == 0 ? pointStartY + myDoubleSpiralWidth : pointStartY + (pointStartX - x31) / k;
                StylusPoint sp31 = new StylusPoint(x31, y31);
                double x32 = pointStartX - g;
                double y32 = k == 0 ? pointStartY - myDoubleSpiralWidth : pointStartY + (pointStartX - x32) / k;
                StylusPoint sp32 = new StylusPoint(x32, y32);
                StylusPoint sp3;
                sp3 = MathTool.getInstance().distanceP2P(sp31, pointEndX < Center.X ? leftCenter : rightCenter) < MathTool.getInstance().distanceP2P(sp32, pointEndX < Center.X ? leftCenter : rightCenter) ? sp31 : sp32;

                double x41 = pointEndX + g;
                double y41 = k == 0 ? pointEndY + myDoubleSpiralWidth : pointEndY + (pointEndX - x41) / k;
                StylusPoint sp41 = new StylusPoint(x41, y41);
                double x42 = pointEndX - g;
                double y42 = k == 0 ? pointEndY - myDoubleSpiralWidth : pointEndY + (pointEndX - x42) / k;
                StylusPoint sp42 = new StylusPoint(x42, y42);
                StylusPoint sp4;
                sp4 = MathTool.getInstance().distanceP2P(sp41, pointEndX < Center.X ? leftCenter : rightCenter) < MathTool.getInstance().distanceP2P(sp42, pointEndX < Center.X ? leftCenter : rightCenter) ? sp41 : sp42;
                StylusPoint sp5 = new StylusPoint((sp3.X + sp4.X) / 2, (sp3.Y + sp4.Y) / 2);
                if (pointEndY == pointStartY && pointStartY == points[spiralPointsCount].Y)
                {
                    fourPoints.Add(new StylusPoint(sp5.X, sp5.Y + 20));
                }
                else if (StartIndex > spiralPointsCount - 100 && StartIndex < spiralPointsCount)
                {
                    fourPoints.Add(new StylusPoint(sp5.X, sp5.Y + (StartIndex - (spiralPointsCount - 100))*20/100));
                
                }
                else if (StartIndex > pointsCount - spiralPointsCount && StartIndex < pointsCount - spiralPointsCount+100)
                {
                    fourPoints.Add(new StylusPoint(sp5.X, sp5.Y + (StartIndex - (pointsCount-spiralPointsCount)) * 20 / 100));

                }

                //InkTool.getInstance().drawPoint(sp5.X, sp5.Y, 8, Colors.Violet, InkCanvas);
                fourPoints.Add(sp5);
                Stroke s = new Stroke(fourPoints);
                bound = s.GetBounds();
                double boundLeft = bound.Left;
                double boundTop = bound.Top;
                //处理相邻两关键点接近垂直的情况                        
                if (Math.Abs(pointEndX - pointStartX) <= 50)
                {
                    StylusPoint sp6 = new StylusPoint(boundLeft, boundTop - 20);
                    StylusPoint sp7 = new StylusPoint(boundLeft, boundTop + bound.Height + 10);
                    fourPoints.Add(sp6);
                    fourPoints.Add(sp7);
                    s = new Stroke(fourPoints);
                    bound = s.GetBounds();
                }
                

            }
            else
            {
                double x31 = pointStartX + myDoubleSpiralWidth;
                double y3 = pointStartY;
                StylusPoint sp31 = new StylusPoint(x31, y3);
                double x32 = pointStartX - myDoubleSpiralWidth;
                StylusPoint sp32 = new StylusPoint(x32, y3);
                StylusPoint sp3 = new StylusPoint();
                if (MathTool.getInstance().distanceP2P(sp31, pointEndX < Center.X ? leftCenter : rightCenter) < MathTool.getInstance().distanceP2P(sp32, pointEndX < Center.X ? leftCenter : rightCenter))
                {
                    sp3 = sp31;
                }
                else
                {
                    sp3 = sp32;
                }
                fourPoints.Add(sp3);
                Stroke s = new Stroke(fourPoints);
                bound = s.GetBounds();
                //StylusPoint sp4 = new StylusPoint(bound.Left, bound.Top - 10);
                //StylusPoint sp5 = new StylusPoint(bound.Left, bound.Top + bound.Height + 10);
                //fourPoints.Add(sp4);
                //fourPoints.Add(sp5);
                s = new Stroke(fourPoints);
                bound = s.GetBounds();
            }
            bounds.Add(bound);
            keyFrameCenterPoints.Add(new Point(bound.X + bound.Width / 2, bound.Y + bound.Height / 2));
            //if (StartIndex == keyPointIndexes[6])
            //{
            //    InkCanvas.Children.Add(ConvertClass.getInstance().RectToRectangle2(bound));
            //    for (int i = 1; i < fourPoints.Count - 1; i++)
            //    {
            //        InkTool.getInstance().drawPoint(fourPoints[i].X, fourPoints[i].Y, 8, Colors.Blue, InkCanvas);
            //    }
            //}
            return bound;
        }
        private void getBounds2()
        {
            boundsForMove.Clear();
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Reset();
                //sw.Start();
            for (int j = 0; j < points.Count - 220; j++)
            {
                getBound(j, endIndexes[j]);
            }
                //sw.Stop();
                //Console.WriteLine("需要时间：" + sw.ElapsedMilliseconds + "ms");
        }
        /// <summary>
        /// 获取所有外包盒
        /// </summary>
        private void getBounds()
        {
            bounds.Clear();
            for (int i = 0; i < keyFrameCount; i++)
            {
                int startIndex = keyPointIndexes[i];
                int endIndex = keyPointIndexes[i + 1];
                getBound(startIndex, endIndex);
            }
        }

        /// <summary>
        /// 向螺旋线中添加关键帧图片
        /// </summary>
        /// <param name="i">插入位置的索引值</param>
        /// <param name="isNew">是否是螺旋线上已有位置</param>
        public void addImage2Spiral(int i, KeyFrame keyFrame,int startIndex, bool isNew)
        {
            //if (isNew)
            //{
            //    addNewSpiralStroke(i,startIndex, Colors.Black);
            //}
            InsertImage(i, keyFrame, startIndex);
        }
        /// <summary>
        /// 添加对应索引的螺旋线
        /// </summary>
        /// <param name="index"></param>
        private int endIndexesForAddNewSpiralStroke = 0;
        public void addNewSpiralStroke(int index,int startIndex, Color color)
        {
            StylusPointCollection spc = new StylusPointCollection();
            int endPointIndexInSpiral=endIndexes[startIndex];
            //if (index == showCount-1 && startIndex >= 1275 && startIndex <= 1289)
            //{
            //    endPointIndexInSpiral -= 4;
            //}
            if (endIndexesForAddNewSpiralStroke < endPointIndexInSpiral)
            {
                int showSpiralStrokeStylusPointsCount = null==showSpiralStroke?0:showSpiralStroke.StylusPoints.Count;
                for (int i = endIndexesForAddNewSpiralStroke; i <= endPointIndexInSpiral+2; i++)
                {
                    if (null == showSpiralStroke || (showSpiralStroke != null && i >= showSpiralStrokeStylusPointsCount))
                    {
                        spc.Add(new StylusPoint(points[i].X, points[i].Y));
                    }
                }
                endIndexesForAddNewSpiralStroke = endPointIndexInSpiral;
                if (null==showSpiralStroke)
                {
                    showSpiralStroke = new Stroke(spc);
                    showSpiralStroke.DrawingAttributes.Color = color;
                    showSpiralStroke.DrawingAttributes.Width = myDoubleSpiralStrokeWidth;
                    showSpiralStroke.DrawingAttributes.Height = myDoubleSpiralStrokeWidth;
                    InkCanvas.Strokes.Add(showSpiralStroke);
                }
                else
                {
                    showSpiralStroke.StylusPoints.Add(spc);
                }
            }            
        }
        /// <summary>
        /// 向画板中添加图片
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="index"></param>
        private void addPicture(double x, double y, KeyFrame keyFrame)
        {
            Image keyFrameImage = keyFrame.Image;
            if (InkCanvas.Children.IndexOf(keyFrameImage) == -1)
            {
                keyFrameImage.Margin = new Thickness(x, y, 0, 0);
                InkCanvas.Children.Add(keyFrameImage);
            }
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
        
        private static BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmapSource)
        {
            MemoryStream ms = new MemoryStream();
            bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();
            return bitmapImage;
        }
        

        /// <summary>
        /// 隐藏螺旋摘要，用作与自定义摘要的转化
        /// </summary>
        public void hiddenDoubleSpiralSummarization()
        {
            this.InkCanvas.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 显示螺旋摘要，用作与自定义摘要的转化
        /// </summary>
        public void showDoubleSpiralSummarization()
        {
            this.InkCanvas.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 计算点在极坐标螺旋中第几环
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        //public int pointInRing(StylusPoint sp, double angle, bool isKeyPoint, int startIndex)
        //{
        //    if (isKeyPoint)
        //    {
        //        return (startIndex + 90) / 360;
        //    }
        //    double distance = MathTool.getInstance().distanceSP2SP(center, sp);
        //    double minDistance = newSpiralWidth * angle;
        //    double doublering = (distance - minDistance) / myDoubleSpiralWidth;
        //    if (doublering <= 0)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        int ring = (int)(Math.Ceiling(doublering));
        //        double difference = Math.Ceiling(doublering) - doublering;
        //        if (difference <= 0.01 && difference > 0) ring++;
        //        if (isKeyPoint && (difference >= 0.99))
        //            ring--;
        //        return ring;
        //    }
        //}

        /// <summary>
        /// 插入一个关键帧并移动其他的关键帧
        /// </summary>
        /// <param name="insertIndexInKeyFrames"></param>
        public void insertNewKeyFrameAndMoveOther(int insertIndex)
        {
            KeyFrame insertKeyFrame = keyFrames[insertIndex];
            if (insertKeyFrame.Visibility == Visibility.Collapsed)
            {
                //this.insertIndex = insertIndexInKeyFrames;
                //addNewSpiralStroke(keyPointIndexes[showCount], Colors.Yellow);
                showCount++;
                //insertIndex = insertIndexInKeyFrames;
                insertKeyFrame.showImage();
                //查找插入位置
                int i = 0;
                foreach (KeyFrame kf in showKeyFrames)
                {
                    if (kf == keyFrames[insertIndex - 1])
                    {
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                showKeyFrames.Insert(i+1, insertKeyFrame);
                int start;
                List<int> fromIndexes = new List<int>();
                List<int> toIndexes = new List<int>();
                List<KeyFrame> MoveKeyFrames = new List<KeyFrame>();
                for (i = insertIndex; i < keyFrameCount - 1; i++)
                {
                    //index = (i + 1) / 2;
                    //start = keyPointIndexes[index];
                    start = keyFrames[i].PointIndexInSpiral;
                    fromIndexes.Add(start);
                    toIndexes.Add(endIndexes[start]);
                    MoveKeyFrames.Add(keyFrames[i]);
                }
                DMoveKeyFrame dmoveKeyFrame = new DMoveKeyFrame(this, 1,insertIndex, fromIndexes, toIndexes, MoveKeyFrames);
                dmoveKeyFrame.move();
                showKeyFrameCenterPoints.Add(keyFrameCenterPoints[keyPointIndexes[showCount-1]]);
                //InkTool.getInstance().drawPoint(showKeyFrameCenterPoints.Last().X, showKeyFrameCenterPoints.Last().Y,8, Colors.Blue, InkCanvas);
            }
        }
        /// <summary>
        /// 插入多个关键帧并移动其他的关键帧
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        DMoveKeyFrame dmoveKeyFrame;
        public void InsertKeyFrames(int startIndex, int endIndex)
        {
            //检查选定的区域是否有需要插入的关键帧
            int hiddenCount = 0;
            int firstHiddenIndex = startIndex;
            List<int> moveCounts = new List<int>();
            moveCounts.Add(0);
            for (int i = startIndex + 1; i <keyFrameCount - 1; i++)
            {
                if (firstHiddenIndex > startIndex)
                {
                    moveCounts.Add(hiddenCount);
                }
                if (i % 2 == 1 && i < endIndex && !keyFrames[i].IsVisible)//&&keyFrames[i].Visibility == Visibility.Collapsed)
                {
                    hiddenCount++;
                    if (hiddenCount == 1)
                    {
                        firstHiddenIndex = i;
                    }
                }
            }
            if (hiddenCount > 0)
            {
                List<KeyFrame> moveKeyframes = new List<KeyFrame>();
                List<int> fromIndexes = new List<int>();
                List<int> toIndexes = new List<int>();
                int start;
                int end;
                int currStartIndex;
                startIndex = firstHiddenIndex-1;
                for (int i = startIndex + 1; i < keyFrameCount - 1; i++)
                {
                    if (i % 2 == 1 && i < endIndex&&!keyFrames[i].IsVisible)// && keyFrames[i].Visibility == Visibility.Collapsed)
                    {
                        keyFrames[i].IsVisible = true ;
                        //查找插入位置
                        int k = 0;
                        foreach (KeyFrame kf in showKeyFrames)
                        {
                            if (kf == keyFrames[i - 1])
                            {
                                break;
                            }
                            else
                            {
                                k++;
                            }
                        }

                        showKeyFrames.Insert(k + 1, keyFrames[i]);
                        showCount++;
                        showKeyFrameCenterPoints.Add(keyFrameCenterPoints[keyPointIndexes[showCount - 1]]);
                    }
                    start = keyFrames[i].PointIndexInSpiral;
                    currStartIndex = keyPointIndexes.IndexOf(start);
                    fromIndexes.Add(start);
                    end = currStartIndex + moveCounts[i - startIndex - 1];
                    int toIndex = keyPointIndexes[end];
                    keyFrames[i].PointIndexInSpiral = toIndex;
                    toIndexes.Add(toIndex);
                    moveKeyframes.Add(keyFrames[i]);
                }
                isZooming = true;
                dmoveKeyFrame = new DMoveKeyFrame(this, 1, startIndex + 1, fromIndexes, toIndexes, moveKeyframes);
                dmoveKeyFrame.move();
            }
            //showKeyFrameCenterPoints.Add(keyFrameCenterPoints[keyPointIndexes[showCount - 1]]);
        }
        /// <summary>
        /// 隐藏插入的关键帧
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        public void HiddenKeyFrames(int startIndex, int endIndex)
        {
            //检查选定的区域是否有需要隐藏的关键帧
            int hiddenCount = 0;
            int firstHiddenIndex = startIndex;
            List<int> moveCounts = new List<int>();
            moveCounts.Add(0);
            for (int i = startIndex + 1; i < keyFrameCount - 1; i++)
            {
                if (firstHiddenIndex > startIndex)
                {
                    moveCounts.Add(hiddenCount);
                }
                if (i % 2 == 1 && i < endIndex && keyFrames[i].IsVisible)// && keyFrames[i].Visibility == Visibility.Visible)
                {
                    hiddenCount++;
                    if (hiddenCount == 1)
                    {
                        firstHiddenIndex = i;
                    }
                }
            }
            if (hiddenCount > 0)
            {
                List<KeyFrame> moveKeyframes = new List<KeyFrame>();
                List<int> fromIndexes = new List<int>();
                List<int> toIndexes = new List<int>();
                int start;
                int end;
                int currStartIndex;
                startIndex = firstHiddenIndex - 1;
                for (int i = startIndex + 1; i < keyFrameCount - 1; i++)
                {
                    if (i % 2 == 1 && i < endIndex && keyFrames[i].IsVisible)// && keyFrames[i].Visibility == Visibility.Visible)
                    {
                        keyFrames[i].IsVisible = false;
                        //查找插入位置
                        //int k = 0;
                        //foreach (KeyFrame kf in showKeyFrames)
                        //{
                        //    if (kf == keyFrames[i - 1])
                        //    {
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        k++;
                        //    }
                        //}

                        showKeyFrames.Remove(keyFrames[i]);
                        showCount--;
                        showKeyFrameCenterPoints.RemoveAt(showKeyFrameCenterPoints.Count-1);
                    }
                    start = keyFrames[i].PointIndexInSpiral;
                    currStartIndex = keyPointIndexes.IndexOf(start);
                    end = currStartIndex - moveCounts[i - startIndex - 1];
                    if (end > -1)
                    {
                        fromIndexes.Add(start);
                        int toIndex = keyPointIndexes[end];
                        keyFrames[i].PointIndexInSpiral = toIndex;
                        toIndexes.Add(toIndex);
                        moveKeyframes.Add(keyFrames[i]);
                    }
                }
                isZooming = true;
                dmoveKeyFrame = new DMoveKeyFrame(this, 2, startIndex + 1, fromIndexes, toIndexes, moveKeyframes);
                dmoveKeyFrame.move();
            }
        }
        public void ZoomInOut()
        {
            if (isZoomOut)
            {
                isZooming = true;
                DMoveKeyFrame dmoveKeyFrame = new DMoveKeyFrame(this, 3,0, keyPointMarginFocusIndexes, keyPointCenterFocusIndexes, showKeyFrames);
                dmoveKeyFrame.move();
            }
            else
            {
                isZooming = true;
                DMoveKeyFrame dmoveKeyFrame = new DMoveKeyFrame(this, 4, 0, keyPointCenterFocusIndexes, keyPointMarginFocusIndexes, showKeyFrames);
                dmoveKeyFrame.move();
            }

            for (int i = 1; i < showCount; i++)
            {                
                if (isZoomOut)
                {
                    keyPointIndexes[i] = keyPointCenterFocusIndexes[i];
                    showKeyFrameCenterPoints[i] = keyFrameCenterPoints[keyPointCenterFocusIndexes[i]];
                }
                else
                {
                    keyPointIndexes[i] = keyPointMarginFocusIndexes[i];
                    showKeyFrameCenterPoints[i] = keyFrameCenterPoints[keyPointMarginFocusIndexes[i]];
                }
            }
            isZoomOut = !isZoomOut;
        }
        /// <summary>
        /// 获取对应startindex的endindex
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int getEndIndex(int startIndex)
        {
            return endIndexes[startIndex];
        }
        ///// <summary>
        ///// 向显示的螺旋摘要上添加红色笔迹代表该关键帧有注释信息
        ///// </summary>
        ///// <param name="index">对应关键帧索引</param>
        ///// <param name="color">笔迹颜色</param>
        ///// <param name="strokeWidthOffset">相对应螺旋线笔迹的宽度差异,默认为0</param>
        //public Stroke AddPoints2ShowSpiral(int index, Color color,double strokeWidthOffset)
        //{
        //    Stroke stroke;
        //    //关键帧对应关键点在螺旋笔迹中的索引
        //    int startPointIndexInSpiral = _inkCollector.DoubleSpiralSummarization.KeyPointIndexes[index];
        //    //添加蓝色笔迹代表该关键帧有注释
        //    StylusPointCollection spc = new StylusPointCollection();
        //    if (index == 0)
        //    {
        //        for (int i = 1; i <= startPointIndexInSpiral; i++)
        //        {
        //            spc.Add(new StylusPoint(points[i].X, points[i].Y));
        //        }
        //    }
        //    else
        //    {
        //        int endPointIndexInSpiral = _inkCollector.DoubleSpiralSummarization.KeyPointIndexes[index + 1];
        //        for (int i = startPointIndexInSpiral+1; i <= endPointIndexInSpiral; i++)
        //        {
        //            spc.Add(new StylusPoint(points[i].X, points[i].Y));
        //        }
        //    }
        //    if (spc.Count > 0)
        //    {
        //        stroke = new Stroke(spc);
        //        stroke.DrawingAttributes.Color = color;
        //        stroke.DrawingAttributes.Height = stroke.DrawingAttributes.Width = showSpiralStroke.DrawingAttributes.Width+strokeWidthOffset;
        //        ((InkCanvas)(_inkCollector._mainPage._inkCanvas.Children[0])).Strokes.Add(stroke);
        //        return stroke;
        //    }
        //    return null;
        //}
        /// <summary>
        /// 向显示的螺旋摘要上添加圆点代表播放到该关键帧,表明播放进度
        /// </summary>
        /// <param name="index">对应关键帧索引</param>
        /// <param name="color">笔迹颜色</param>
        /// <param name="strokeWidthOffset">相对应螺旋线笔迹的宽度差异,默认为0</param>
        public override Stroke AddPoint2Track(int index, Color color, double strokeWidthOffset)
        {
            Stroke stroke;
            //关键帧对应关键点在螺旋笔迹中的索引
            int startPointIndexInSpiral = KeyPointIndexes[index];
            //添加蓝色笔迹代表该关键帧有注释
            StylusPointCollection spc = new StylusPointCollection();
            int endPointIndexInSpiral = KeyPointIndexes[index + 1];
            int midIndex = (startPointIndexInSpiral + endPointIndexInSpiral) / 2;
            spc.Add(new StylusPoint(points[midIndex].X, points[midIndex].Y));
            if (spc.Count > 0)
            {
                stroke = new Stroke(spc);
                stroke.DrawingAttributes.Color = color;
                stroke.DrawingAttributes.Height = stroke.DrawingAttributes.Width = showSpiralStroke.DrawingAttributes.Width + strokeWidthOffset;
                ((InkCanvas)(_inkCollector._mainPage._inkCanvas.Children[0])).Strokes.Add(stroke);
                return stroke;
            }
            return null;
        }
        /// <summary>
        /// 向显示的螺旋摘要上添加红色笔迹代表该关键帧有注释信息
        /// </summary>
        /// <param name="index">对应关键帧索引</param>
        /// <param name="color">笔迹颜色</param>
        /// <param name="strokeWidthOffset">相对应螺旋线笔迹的宽度差异,默认为0</param>
        public Stroke AddPoints2ShowSpiral(int index, Color color, double strokeWidthOffset)
        {
            Stroke stroke;
            //关键帧对应关键点在螺旋笔迹中的索引
            int startPointIndexInSpiral = KeyPointIndexes[index];
            //添加蓝色笔迹代表该关键帧有注释
            StylusPointCollection spc = new StylusPointCollection();
            if (index == 0)
            {
                for (int i = 1; i <= startPointIndexInSpiral; i++)
                {
                    spc.Add(new StylusPoint(points[i].X, points[i].Y));
                }
            }
            else
            {
                int endPointIndexInSpiral = KeyPointIndexes[index + 1];
                for (int i = startPointIndexInSpiral + 1; i <= endPointIndexInSpiral; i++)
                {
                    spc.Add(new StylusPoint(points[i].X, points[i].Y));
                }
            }
            if (spc.Count > 0)
            {
                stroke = new Stroke(spc);
                stroke.DrawingAttributes.Color = color;
                stroke.DrawingAttributes.Height = stroke.DrawingAttributes.Width = showSpiralStroke.DrawingAttributes.Width + strokeWidthOffset;
                ((InkCanvas)(_inkCollector._mainPage._inkCanvas.Children[0])).Strokes.Add(stroke);
                return stroke;
            }
            return null;
        }
        /// <summary>
        /// 从螺旋摘要中获取选择的关键帧
        /// </summary>
        /// <param name="point"></param>
        public override int getSelectedKeyFrameIndex(Point point)//, DoubleSpiralSummarization spiralSummarization)
        {
            double minDistance = double.MaxValue;
            int minIndex = int.MinValue;
            if (this != null)
            {
                bool isInMyDoubleSpiral = false;
                //int lastPintAngleIndexInSpiral = this.ShowSpiralStroke.StylusPoints.Count + 90;
                int lastPintAngleIndexInSpiral = keyPointIndexes[showKeyFrames.Count] + 90;
                StylusPoint currPoint = new StylusPoint(point.X, point.Y);
                double angle = MathTool.getInstance().getAngleP2P(this.MyDoubleSpiral.Center, currPoint);
                double currangle = angle / 180 * Math.PI;
                int currPointInRing = this.pointInRing(currPoint, currangle, false, 0, Center);
                double angle2 = currPointInRing * 360 + angle;

                if (lastPintAngleIndexInSpiral > angle2)
                {
                    isInMyDoubleSpiral = true;
                }

                if (isInMyDoubleSpiral)
                {
                    for (int i = 0; i < Math.Min(this.KeyPointIndexes.Count, this.ShowKeyFrames.Count); i++)
                    {
                        double distance = MathTool.getInstance().distanceP2P(point, this.ShowKeyFrameCenterPoints[i]);

                        if (distance < this.MyDoubleSpiral.SpiralWidth && distance < minDistance)
                        {
                            minDistance = distance;
                            minIndex = i;
                        }
                    }
                }

                if (minIndex < 0 || minIndex >= keyFrameCount)
                {
                    return int.MinValue;
                }
            }
            return minIndex;
        }
        #endregion
        #region 关键帧处理
        /// <summary>
        /// 去除图片的多余边界
        /// </summary>
        /// <param name="index">在螺旋摘要中的位置</param>
        /// <param name="keyFrame">关键帧</param>
        /// <param name="startIndex">第一个关键点在螺旋线中的点索引值</param>
        /// <param name="endIndex">第二个关键点在螺旋线中的点索引值</param>
        /// <param name="left">关键帧图片的左边距</param>
        /// <param name="top">关键帧图片的上边距</param>
        //WriteableBitmap bitmapTarget;
        private WriteableBitmap RemoveRedundanceByTemplate(KeyFrame keyFrame, int startIndex)
        {
            //BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(keyFrame.Bitmap);            
            keyFrame.BitmapSource.CopyPixels(ArgbValues, InitTemplateWidth4, 0);
            int indexBmp;
            for (int i = 0; i < InitTemplateWidth; i++)
            {
                for (int j = 0; j < InitTemplateHeight; j++)
                {                    
                    indexBmp = 4 * (InitTemplateWidth * j + i);
                    ArgbValues[indexBmp + 3] = Global.GlobalValues.templates[startIndex][i * InitTemplateHeight + j];                    
                }
            }
            bitmapTarget = new WriteableBitmap(InitTemplateWidth, InitTemplateHeight,
                keyFrame.BitmapSource.DpiX, keyFrame.BitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            sourceRect = new Int32Rect(0, 0, InitTemplateWidth, InitTemplateHeight);
            bitmapTarget.WritePixels(sourceRect, ArgbValues, InitTemplateWidth4, 0);
            return bitmapTarget;

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
        /// <summary>
        /// 去除图片的多余边界,顺时针
        /// </summary>
        /// <param name="index">在螺旋摘要中的位置</param>
        /// <param name="keyFrame">关键帧</param>
        /// <param name="startIndex">第一个关键点在螺旋线中的点索引值</param>
        /// <param name="endIndex">第二个关键点在螺旋线中的点索引值</param>
        /// <param name="left">关键帧图片的左边距</param>
        /// <param name="top">关键帧图片的上边距</param>
        private WriteableBitmap RemoveRedundanceClockwize(int index, KeyFrame keyFrame, int startIndex, int endIndex, double left, double top)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(keyFrame.ImageName);
            BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(bitmap);
            int w = bitmap.Width, h = bitmap.Height;
            int bytes = w * h * 4;
            double PI15 = Math.PI * 1.5;
            byte[] ArgbValues = new byte[bytes];
            bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
            double pointWidth = keyFrame.Width / w;
            //去除无用边界
            double maxDistance = 0;
            double maxi = 0;
            double maxj = 0;
            StylusPoint startPoint = points[startIndex];
            StylusPoint keyFrameCenterPoint = new StylusPoint(keyFrameCenterPoints[startIndex].X, keyFrameCenterPoints[startIndex].Y);
            double angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, keyFrameCenterPoint);
            double centerangle = angle / 180 * Math.PI;
            int keyFrameCenterPointInRing = pointInRing(keyFrameCenterPoint, centerangle, false, leftCenter);
            //StylusPoint center = getCenterPoint(startPoint);
            double distance1 = MathTool.getInstance().distanceP2P(startPoint, leftCenter);//起始关键点与中心点之间的距离
            double angleSC = 360 - MathTool.getInstance().getAngleP2P(leftCenter, startPoint);
            int startPointInRing = pointInRing(startPoint, angleSC / 180 * Math.PI, true, startIndex, leftCenter);
            double angle1;
            int indexBmp = 0;
            angle1 = (startPointInRing) * 360 + angleSC;//关键点与中心点连线的夹角,与逆时针不同            
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    double x, y;
                    x = left + i * pointWidth;
                    y = top + j * pointWidth;
                    StylusPoint currPoint = new StylusPoint(x, y);
                    indexBmp = 4 * (w * j + i);
                    angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, currPoint);
                    double currangle = angle / 180 * Math.PI;
                    int currPointInRing = pointInRing(currPoint, currangle, false, leftCenter);
                    if (centerangle - currangle > PI15)
                    {
                        currPointInRing--;
                    }
                    else if (currangle - centerangle > PI15)
                    {
                        currPointInRing++;
                    }
                    if (currPointInRing == keyFrameCenterPointInRing)
                    {
                        if (centerangle - currangle > PI15)
                        {
                            currPointInRing++;
                        }
                        else if (currangle - centerangle > PI15)
                        {
                            currPointInRing--;
                        }
                        double angle2 = currPointInRing * 360 + angle;//与顺时针不同
                        if (angle1 > angle2)
                        {
                            double currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, leftCenter);
                            if (currDistance > maxDistance)
                            {
                                maxDistance = currDistance;
                                maxi = i;
                                maxj = j;

                            }
                        }
                    }
                    else
                    {
                        ArgbValues[indexBmp + 3] = 0;
                    }
                }
            }

            ////对图片连接处进行渐变
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    indexBmp = 4 * (w * j + i);
                    if (ArgbValues[indexBmp + 3] != 0)
                    {
                        double rate = 1;
                        double x, y;
                        x = left + i * pointWidth; y = top + j * pointWidth;
                        StylusPoint currPoint = new StylusPoint(x, y);

                        angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, currPoint);
                        double currangle = angle / 180 * Math.PI;
                        int currPointInRing = pointInRing(currPoint, currangle, false, leftCenter);
                        double angle2 = (currPointInRing) * 360 + angle;
                        if (angle1 > angle2)
                        {
                            double currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, leftCenter);
                            rate = 1 - currDistance / maxDistance;//透明度比例
                            if (rate > 1) { rate = 1; }
                            ArgbValues[indexBmp + 3] = (byte)(255 * Math.Pow(rate, 3));
                            //对第二、三象限过渡效果进行调整
                            if (j <= 20)
                            {
                                rate = ((double)j) / 20;
                                ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                            }
                            //对第一、四象限过渡效果进行调整
                            else if (h - j <= 15)
                            {
                                rate = ((double)(bitmapSource.Height - j)) / 15;
                                ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                            }

                        }
                    }
                }

            }
            if (index == showCount - 1 || index == showCount - 2)
            {
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        indexBmp = 4 * (w * j + i);
                        if (ArgbValues[indexBmp + 3] != 0)
                        {
                            double x, y;
                            x = left + i * pointWidth; y = top + j * pointWidth;
                            StylusPoint currPoint = new StylusPoint(x, y);

                            angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, currPoint);
                            double currangle = angle / 180 * Math.PI;
                            int currPointInRing = pointInRing(currPoint, currangle, false, leftCenter);
                            double angle2 = (currPointInRing) * 360 - angle;
                            if (angle2 > lastKeyPointAngle)
                            {
                                ArgbValues[indexBmp + 3] = 0;
                            }

                        }
                    }

                }
            }
            FileStream myStream = new FileStream(@"resource\template.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(myStream);
            //填入模板
            for (int i = 0; i < InitTemplateWidth; i++)
            {
                for (int j = 0; j < InitTemplateHeight; j++)
                {
                    indexBmp = 4 * (InitTemplateWidth * j + i);
                    //Console.WriteLine(ArgbValues[indexBmp+3]);
                    sWriter.Write(ArgbValues[indexBmp + 3].ToString() + " ");
                    //template.Add(ArgbValues[indexBmp + 3]);
                }
            }
            Console.WriteLine(index);
            sWriter.WriteLine();
            sWriter.Close();
            sWriter = null;
            WriteableBitmap bitmapTarget = new WriteableBitmap(w, h, bitmapSource.DpiX, bitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            Int32Rect sourceRect = new Int32Rect(0, 0, w, h);
            bitmapTarget.WritePixels(sourceRect, ArgbValues, w * 4, 0);
            //keyFrame.Image.Source = null;
            //keyFrame.Image.Source = bitmapTarget;
            ArgbValues = null;
            //保存
            //string savePath = GlobalValues.FilesPath + @"\keyFrames\"+index+@"\" + index+"_"+startIndex + ".png";
            //ImageTool.getInstance().SaveImage(savePath, (BitmapSource)bitmapTarget);
            return bitmapTarget;

        }

        private WriteableBitmap RemoveRedundanceLeftTurning(int index, KeyFrame keyFrame, int startIndex, int endIndex, double left, double top)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(keyFrame.ImageName);
            BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(bitmap);
            int w = bitmap.Width, h = bitmap.Height;
            int bytes = w * h * 4;
            double PI15 = Math.PI * 1.5;
            byte[] ArgbValues = new byte[bytes];
            bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
            double pointWidth = keyFrame.Width / w;
            //去除无用边界
            double maxDistance = 0;
            double maxi = 0;
            double maxj = 0;
            StylusPoint startPoint = points[startIndex];
            StylusPoint keyFrameCenterPoint = new StylusPoint(keyFrameCenterPoints[startIndex].X, keyFrameCenterPoints[startIndex].Y);
            double angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, keyFrameCenterPoint);
            double centerangle = angle / 180 * Math.PI;
            int keyFrameCenterPointInRing = pointInRing(keyFrameCenterPoint, centerangle, false, leftCenter);
            //StylusPoint center = getCenterPoint(startPoint);
            double distance1 = MathTool.getInstance().distanceP2P(startPoint, leftCenter);//起始关键点与中心点之间的距离
            double angleSC = 360 - MathTool.getInstance().getAngleP2P(leftCenter, startPoint);
            int startPointInRing = pointInRing(startPoint, angleSC / 180 * Math.PI, true, startIndex, leftCenter);
            double angle1;
            int indexBmp = 0;
            angle1 = (startPointInRing) * 360 + angleSC;//关键点与中心点连线的夹角,与逆时针不同            
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    double x, y;
                    x = left + i * pointWidth;
                    y = top + j * pointWidth;
                    StylusPoint currPoint = new StylusPoint(x, y);
                    indexBmp = 4 * (w * j + i);
                    angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, currPoint);
                    double currangle = angle / 180 * Math.PI;
                    int currPointInRing = pointInRing(currPoint, currangle, false, leftCenter);
                    if (currPoint.X >  points[spiralPointsCount].X)
                    {
                        if (currPoint.Y > myDoubleSpiral.SpiralStroke1.StylusPoints[0].Y || currPoint.Y < points[spiralPointsCount].Y || currPointInRing < keyFrameCenterPointInRing)
                        {
                            ArgbValues[indexBmp + 3] = 0;
                        }
                    }
                    else
                    {
                        
                        if (currPointInRing == keyFrameCenterPointInRing)
                        {
                            double angle2 = currPointInRing * 360 + angle;//与顺时针不同
                            if (angle1 > angle2)
                            {
                                double currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, leftCenter);
                                if (currDistance > maxDistance)
                                {
                                    maxDistance = currDistance;
                                    maxi = i;
                                    maxj = j;

                                }
                            }
                        }
                        else
                        {
                            ArgbValues[indexBmp + 3] = 0;
                        }
                    }
                }
            }

            ////对图片连接处进行渐变
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    indexBmp = 4 * (w * j + i);
                    if (ArgbValues[indexBmp + 3] != 0)
                    {
                        double rate = 1;
                        double x, y;
                        x = left + i * pointWidth; y = top + j * pointWidth;
                        StylusPoint currPoint = new StylusPoint(x, y);

                        angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, currPoint);
                        double currangle = angle / 180 * Math.PI;
                        int currPointInRing = pointInRing(currPoint, currangle, false, leftCenter);
                        double angle2 = (currPointInRing) * 360 + angle;
                        if (angle1 > angle2)
                        {
                            double currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, leftCenter);
                            rate = 1 - currDistance / maxDistance;//透明度比例
                            if (rate > 1) { rate = 1; }
                            ArgbValues[indexBmp + 3] = (byte)(255 * Math.Pow(rate, 3));
                            //对第二、三象限过渡效果进行调整
                            if (j <= 20)
                            {
                                rate = ((double)j) / 20;
                                ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                            }
                            //对第一、四象限过渡效果进行调整
                            else if (h - j <= 15)
                            {
                                rate = ((double)(bitmapSource.Height - j)) / 15;
                                ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                            }

                        }
                    }
                }

            }
            if (index == showCount - 1 || index == showCount - 2)
            {
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        indexBmp = 4 * (w * j + i);
                        if (ArgbValues[indexBmp + 3] != 0)
                        {
                            double x, y;
                            x = left + i * pointWidth; y = top + j * pointWidth;
                            StylusPoint currPoint = new StylusPoint(x, y);

                            angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, currPoint);
                            double currangle = angle / 180 * Math.PI;
                            int currPointInRing = pointInRing(currPoint, currangle, false, leftCenter);
                            double angle2 = (currPointInRing) * 360 - angle;
                            if (angle2 > lastKeyPointAngle)
                            {
                                ArgbValues[indexBmp + 3] = 0;
                            }

                        }
                    }

                }
            }
            FileStream myStream = new FileStream(@"resource\template.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(myStream);
            //填入模板
            for (int i = 0; i < InitTemplateWidth; i++)
            {
                for (int j = 0; j < InitTemplateHeight; j++)
                {
                    indexBmp = 4 * (InitTemplateWidth * j + i);
                    //Console.WriteLine(ArgbValues[indexBmp+3]);
                    sWriter.Write(ArgbValues[indexBmp + 3].ToString() + " ");
                    //template.Add(ArgbValues[indexBmp + 3]);
                }
            }
            Console.WriteLine(index);
            sWriter.WriteLine();
            sWriter.Close();
            sWriter = null;
            WriteableBitmap bitmapTarget = new WriteableBitmap(w, h, bitmapSource.DpiX, bitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            Int32Rect sourceRect = new Int32Rect(0, 0, w, h);
            bitmapTarget.WritePixels(sourceRect, ArgbValues, w * 4, 0);
            //keyFrame.Image.Source = null;
            //keyFrame.Image.Source = bitmapTarget;
            ArgbValues = null;
            //保存
            //string savePath = GlobalValues.FilesPath + @"\keyFrames\"+index+@"\" + index+"_"+startIndex + ".png";
            //ImageTool.getInstance().SaveImage(savePath, (BitmapSource)bitmapTarget);
            return bitmapTarget;

        }
        private WriteableBitmap RemoveRedundanceHorizontal(int index, KeyFrame keyFrame, int startIndex, int endIndex, double left, double top)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(keyFrame.ImageName);
            BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(bitmap);
            int w = bitmap.Width, h = bitmap.Height;
            int bytes = w * h * 4;
            double PI15 = Math.PI * 1.5;
            byte[] ArgbValues = new byte[bytes];
            bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
            double pointWidth = keyFrame.Width / w;
            //去除无用边界
            double maxDistance = 0;
            double maxi = 0;
            double maxj = 0;
            StylusPoint startPoint = points[startIndex];
            StylusPoint keyFrameCenterPoint = new StylusPoint(keyFrameCenterPoints[startIndex].X, keyFrameCenterPoints[startIndex].Y);
            double angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, keyFrameCenterPoint);
            double centerangle = angle / 180 * Math.PI;
            int keyFrameCenterPointInRing = 0;
            //StylusPoint center = getCenterPoint(startPoint);
            double distance1 = MathTool.getInstance().distanceP2P(startPoint, leftCenter);//起始关键点与中心点之间的距离
            double angleSC = 360 - MathTool.getInstance().getAngleP2P(leftCenter, startPoint);
            int startPointInRing = pointInRing(startPoint, angleSC / 180 * Math.PI, true, startIndex, leftCenter);
            double angle1;
            int indexBmp = 0;
            angle1 = (startPointInRing) * 360 + angleSC;//关键点与中心点连线的夹角,与逆时针不同            
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    double x, y;
                    x = left + i * pointWidth;
                    y = top + j * pointWidth;
                    StylusPoint currPoint = new StylusPoint(x, y);
                    indexBmp = 4 * (w * j + i);

                    if (currPoint.Y > myDoubleSpiral.SpiralStroke1.StylusPoints[0].Y || currPoint.Y < points[spiralPointsCount].Y)
                    {
                        ArgbValues[indexBmp + 3] = 0;
                    }

                    //平直轨道首帧和尾帧的特殊处理：
                    if (currPoint.X > points[spiralPointsCount].X && currPoint.X < myDoubleSpiral.SpiralStroke1.StylusPoints[0].X)
                    {
                        keyFrameCenterPointInRing = pointInRing(keyFrameCenterPoint, centerangle, false, leftCenter);
                        //InkTool.getInstance().drawPoint(points[startIndex].X, points[startIndex].Y, 15, Colors.Bisque, InkCanvas);
                        angle = 360 - MathTool.getInstance().getAngleP2P(leftCenter, currPoint);
                        double currangle = angle / 180 * Math.PI;
                        int currPointInRing = pointInRing(currPoint, currangle, false, leftCenter);

                        if (currPointInRing < keyFrameCenterPointInRing)
                        {
                            ArgbValues[indexBmp + 3] = 0;
                        }
                    
                    }
                    if (currPoint.X > myDoubleSpiral.SpiralStroke1.StylusPoints.Last().X && currPoint.X  < points[pointsCount-spiralPointsCount].X)
                    {
                        //InkTool.getInstance().drawPoint(points[startIndex].X, points[startIndex].Y, 15, Colors.Bisque, InkCanvas);
                        angle = MathTool.getInstance().getAngleP2P(rightCenter, keyFrameCenterPoint);
                        centerangle = angle / 180 * Math.PI;
                        keyFrameCenterPointInRing = pointInRingRightSpiral(keyFrameCenterPoint, centerangle, false, rightCenter);
                        
                        angle = MathTool.getInstance().getAngleP2P(rightCenter, currPoint);
                        double currangle = angle / 180 * Math.PI;
                        int currPointInRing = pointInRingRightSpiral(currPoint, currangle, false, rightCenter);
                        
                        if (currPointInRing < keyFrameCenterPointInRing)
                        {
                            ArgbValues[indexBmp + 3] = 0;
                        }
                     }
                }
            }

            maxDistance = (keyFrame.Width - bounds[startIndex].Width) * 0.9/pointWidth;
            double rate = 1;
            int maxJ=(int)((myDoubleSpiral.SpiralStroke1.StylusPoints[0].Y-top) / pointWidth);
            maxJ=(maxJ>h?h:maxJ);
            int minJ=(int)((points[spiralPointsCount].Y-top)/pointWidth);
            minJ=(minJ<0?0:minJ);
            ////对图片连接处进行渐变
            for (int i = 0; i < maxDistance; i++)
            {
                for (int j = minJ; j < maxJ; j++)
                {
                    indexBmp = 4 * (w * j + i);
                    if (ArgbValues[indexBmp + 3] != 0)
                    {
                        
                        double x, y;
                        x = left + i * pointWidth; y = top + j * pointWidth;
                        StylusPoint currPoint = new StylusPoint(x, y);
                        //double currDistance = points[startIndex].X + maxDistance - currPoint.X;
                        rate = i / maxDistance;//透明度比例
                        ArgbValues[indexBmp + 3] = (byte)(255 * Math.Pow(rate, 3));
                    }
                }

            }
            FileStream myStream = new FileStream(@"resource\template.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(myStream);
            //填入模板
            for (int i = 0; i < InitTemplateWidth; i++)
            {
                for (int j = 0; j < InitTemplateHeight; j++)
                {
                    indexBmp = 4 * (InitTemplateWidth * j + i);
                    //Console.WriteLine(ArgbValues[indexBmp+3]);
                    sWriter.Write(ArgbValues[indexBmp + 3].ToString() + " ");
                    //template.Add(ArgbValues[indexBmp + 3]);
                }
            }
            Console.WriteLine(index);
            sWriter.WriteLine();
            sWriter.Close();
            sWriter = null;
            WriteableBitmap bitmapTarget = new WriteableBitmap(w, h, bitmapSource.DpiX, bitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            Int32Rect sourceRect = new Int32Rect(0, 0, w, h);
            bitmapTarget.WritePixels(sourceRect, ArgbValues, w * 4, 0);
            //keyFrame.Image.Source = null;
            //keyFrame.Image.Source = bitmapTarget;
            ArgbValues = null;
            //保存
            //string savePath = GlobalValues.FilesPath + @"\keyFrames\"+index+@"\" + index+"_"+startIndex + ".png";
            //ImageTool.getInstance().SaveImage(savePath, (BitmapSource)bitmapTarget);
            return bitmapTarget;

        }
        private WriteableBitmap RemoveRedundanceRightTurning(int index, KeyFrame keyFrame, int startIndex, int endIndex, double left, double top)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(keyFrame.ImageName);
            BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(bitmap);
            double PI15 = Math.PI * 1.5;
            double PID180 = Math.PI / 180;
            int w = bitmap.Width, h = bitmap.Height;
            int bytes = w * h * 4;
            byte[] ArgbValues = new byte[bytes];
            bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
            double pointWidth = keyFrame.Width / w, maxDistance = 0, maxi = 0, maxj = 0;
            StylusPoint startPoint = points[startIndex];
            StylusPoint keyFrameCenterPoint = new StylusPoint(keyFrameCenterPoints[startIndex].X, keyFrameCenterPoints[startIndex].Y);

            //////?
            double angle = MathTool.getInstance().getAngleP2P(rightCenter, keyFrameCenterPoint);
            double centerangle = angle * PID180;
            int keyFrameCenterPointInRing = pointInRingRightSpiral(keyFrameCenterPoint, centerangle, false, rightCenter);
            if (startIndex >= 283 && startIndex < 356)
            {
                keyFrameCenterPointInRing++;
            }
            double distance1, angleSC;//起始关键点与中心点之间的距离和角度
            int startPointInRing, indexBmp;
            distance1 = MathTool.getInstance().distanceP2P(startPoint, rightCenter);//起始关键点与中心点之间的距离
            angleSC = MathTool.getInstance().getAngleP2P(rightCenter, startPoint);
            startPointInRing = pointInRingRightSpiral(startPoint, angleSC * PID180, true, rightCenter);//,startIndex); 
            double angle1 = (startPointInRing - 1) * 360 + angleSC;//起始关键点与中心点之间的角度
            double x, y, currangle, currDistance, angle2, diffAngle;
            StylusPoint currPoint;
            int currPointInRing;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    x = left + i * pointWidth;
                    y = top + j * pointWidth;
                    currPoint = new StylusPoint(x, y);
                    indexBmp = 4 * (w * j + i);
                    angle = MathTool.getInstance().getAngleP2P(rightCenter, currPoint);
                    currangle = angle * PID180;
                    currPointInRing = pointInRingRightSpiral(currPoint, currangle, false, rightCenter);
                    diffAngle = centerangle - currangle;
                    if (currPoint.X < points[pointsCount-spiralPointsCount].X)
                    {
                        if (currPoint.Y > myDoubleSpiral.SpiralStroke1.StylusPoints.Last().Y || currPoint.Y < points[spiralPointsCount].Y || currPointInRing < keyFrameCenterPointInRing)
                        {
                            ArgbValues[indexBmp + 3] = 0;
                        }
                    }
                    else
                    {
                        if (currPointInRing == keyFrameCenterPointInRing)
                        {
                            if (diffAngle > PI15)
                            {
                                currPointInRing++;
                            }
                            else if (-diffAngle > PI15)
                            {
                                currPointInRing--;
                            }
                            angle2 = currPointInRing * 360 + angle;
                            if (angle1 < angle2)
                            {
                                currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, rightCenter);
                                if (currDistance > maxDistance)
                                {
                                    maxDistance = currDistance;
                                    maxi = i;
                                    maxj = j;
                                }
                            }
                        }
                        else
                        {
                            ArgbValues[indexBmp + 3] = 0;
                        }
                    }
                }
            }
            
            
            maxDistance = (keyFrame.Width - bounds[startIndex].Width) * 0.9 / pointWidth;
            double rate = 1;
            int maxJ = (int)((myDoubleSpiral.SpiralStroke1.StylusPoints[0].Y - top) / pointWidth);
            maxJ = maxJ < h ? maxJ : h;
            ////对图片连接处进行渐变
            for (int i = 0; i < maxDistance; i++)
            {
                for (int j = (int)((points[spiralPointsCount].Y - top) / pointWidth); j < maxJ; j++)
                {
                    indexBmp = 4 * (w * j + i);
                    if (ArgbValues[indexBmp + 3] != 0)
                    {
                         x = left + i * pointWidth; y = top + j * pointWidth;
                        //double currDistance = points[startIndex].X + maxDistance - currPoint.X;
                        rate = i / maxDistance;//透明度比例
                        ArgbValues[indexBmp + 3] = (byte)(255 * Math.Pow(rate, 3));
                    }
                }

            }

            //if (index == 0)
            //{
            FileStream myStream = new FileStream(@"resource\template.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(myStream);
            //填入模板
            for (int i = 0; i < InitTemplateWidth; i++)
            {
                for (int j = 0; j < InitTemplateHeight; j++)
                {
                    indexBmp = 4 * (InitTemplateWidth * j + i);
                    //Console.WriteLine(ArgbValues[indexBmp+3]);
                    sWriter.Write(ArgbValues[indexBmp + 3].ToString() + " ");
                    //template.Add(ArgbValues[indexBmp + 3]);
                }
            }
            Console.WriteLine(index);
            sWriter.WriteLine();
            sWriter.Close();
            sWriter = null;
            //}
            WriteableBitmap bitmapTarget = new WriteableBitmap(w, h, bitmapSource.DpiX, bitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            Int32Rect sourceRect = new Int32Rect(0, 0, w, h);
            bitmapTarget.WritePixels(sourceRect, ArgbValues, w * 4, 0);
            ArgbValues = null;
            //保存
            //if (index > 75)
            //{
            //    string savePath = GlobalValues.FilesPath + @"\keyFrames\" + index + @"\" + index + "_" + startIndex + ".png";
            //    ImageTool.getInstance().SaveImage(savePath, (BitmapSource)bitmapTarget);
            //}
            return bitmapTarget;
    
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
        private WriteableBitmap RemoveRedundance(int index, KeyFrame keyFrame, int startIndex, int endIndex, double left, double top)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(keyFrame.ImageName);
            BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(bitmap);
            double PI15 = Math.PI * 1.5;
            double PID180 = Math.PI / 180;
            int w = bitmap.Width, h = bitmap.Height;
            int bytes = w * h * 4;
            byte[] ArgbValues = new byte[bytes];
            bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
            double pointWidth = keyFrame.Width / w, maxDistance = 0, maxi = 0, maxj = 0;
            StylusPoint startPoint = points[startIndex];
            StylusPoint keyFrameCenterPoint = new StylusPoint(keyFrameCenterPoints[startIndex].X, keyFrameCenterPoints[startIndex].Y);

            //////?
            double angle = MathTool.getInstance().getAngleP2P(rightCenter, keyFrameCenterPoint);
            double centerangle = angle * PID180;
            int keyFrameCenterPointInRing = pointInRingRightSpiral(keyFrameCenterPoint, centerangle, false, rightCenter);
            if (startIndex >= 283 && startIndex < 356)
            {
                keyFrameCenterPointInRing++;
            }
            double distance1, angleSC;//起始关键点与中心点之间的距离和角度
            int startPointInRing, indexBmp;
            distance1 = MathTool.getInstance().distanceP2P(startPoint, rightCenter);//起始关键点与中心点之间的距离
            angleSC = MathTool.getInstance().getAngleP2P(rightCenter, startPoint);
            startPointInRing = pointInRingRightSpiral(startPoint, angleSC * PID180, true, rightCenter);//,startIndex); 
            double angle1 = (startPointInRing - 1) * 360 + angleSC;//起始关键点与中心点之间的角度
            double x, y, currangle, currDistance, angle2, diffAngle;
            StylusPoint currPoint;
            int currPointInRing;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    x = left + i * pointWidth;
                    y = top + j * pointWidth;
                    currPoint = new StylusPoint(x, y);
                    indexBmp = 4 * (w * j + i);
                    angle = MathTool.getInstance().getAngleP2P(rightCenter, currPoint);
                    currangle = angle * PID180;
                    currPointInRing = pointInRingRightSpiral(currPoint, currangle, false, rightCenter);
                    diffAngle = centerangle - currangle;
                    if (diffAngle > PI15)
                    {
                        currPointInRing--;
                    }
                    else if (-diffAngle > PI15)
                    {
                        currPointInRing++;
                    }
                    if (currPointInRing == keyFrameCenterPointInRing)
                    {
                        if (diffAngle > PI15)
                        {
                            currPointInRing++;
                        }
                        else if (-diffAngle > PI15)
                        {
                            currPointInRing--;
                        }
                        angle2 = currPointInRing * 360 + angle;
                        if (angle1 < angle2)
                        {
                            currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, rightCenter);
                            if (currDistance > maxDistance)
                            {
                                maxDistance = currDistance;
                                maxi = i;
                                maxj = j;
                            }
                        }
                    }
                    else
                    {
                        ArgbValues[indexBmp + 3] = 0;
                    }
                }
            }
            double rate;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    indexBmp = 4 * (w * j + i);
                    if (0 != ArgbValues[indexBmp + 3])
                    {
                        rate = 1;
                        x = left + i * pointWidth;
                        y = top + j * pointWidth;
                        currPoint = new StylusPoint(x, y);
                        angle = MathTool.getInstance().getAngleP2P(rightCenter, currPoint);
                        currangle = angle * PID180;
                        currPointInRing = pointInRingRightSpiral(currPoint, currangle, false, rightCenter);
                        angle2 = currPointInRing * 360 + angle;
                        if (angle1 < angle2)
                        {
                            currDistance = MathTool.getInstance().distanceP2L(currPoint, startPoint, rightCenter);
                            rate = 1 - currDistance / maxDistance;//透明度比例
                            if (rate > 1) { rate = 1; }
                            ArgbValues[indexBmp + 3] = (byte)(255 * Math.Pow(rate, 3));
                            //对第二、三象限过渡效果进行调整
                            if (j <= 20)
                            {
                                rate = ((double)j) / 20;
                                ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                            }
                            //对第一、四象限过渡效果进行调整
                            else if (h - j <= 15)
                            {
                                rate = ((double)(bitmapSource.Height - j)) / 15;
                                ArgbValues[indexBmp + 3] = (byte)(ArgbValues[indexBmp + 3] * rate);
                            }

                        }
                    }
                }
            }
            if (index == showCount - 1 || index == showCount - 2)
            {
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        indexBmp = 4 * (w * j + i);
                        if (ArgbValues[indexBmp + 3] != 0)
                        {
                            x = left + i * pointWidth; y = top + j * pointWidth;
                            currPoint = new StylusPoint(x, y);

                            angle = MathTool.getInstance().getAngleP2P(rightCenter, currPoint);
                            currangle = angle * PID180;
                            currPointInRing = pointInRing(currPoint, currangle, false, rightCenter);
                            angle2 = currPointInRing * 360 + angle;
                            if (angle2 > lastKeyPointAngle)
                            {
                                ArgbValues[indexBmp + 3] = 0;
                            }
                        }
                    }
                }
            }
            //if (index == 0)
            //{
            FileStream myStream = new FileStream(@"resource\template.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(myStream);
            //填入模板
            for (int i = 0; i < InitTemplateWidth; i++)
            {
                for (int j = 0; j < InitTemplateHeight; j++)
                {
                    indexBmp = 4 * (InitTemplateWidth * j + i);
                    //Console.WriteLine(ArgbValues[indexBmp+3]);
                    sWriter.Write(ArgbValues[indexBmp + 3].ToString() + " ");
                    //template.Add(ArgbValues[indexBmp + 3]);
                }
            }
            Console.WriteLine(index);
            sWriter.WriteLine();
            sWriter.Close();
            sWriter = null;
            //}
            WriteableBitmap bitmapTarget = new WriteableBitmap(w, h, bitmapSource.DpiX, bitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            Int32Rect sourceRect = new Int32Rect(0, 0, w, h);
            bitmapTarget.WritePixels(sourceRect, ArgbValues, w * 4, 0);
            ArgbValues = null;
            //保存
            //if (index > 75)
            //{
            //    string savePath = GlobalValues.FilesPath + @"\keyFrames\" + index + @"\" + index + "_" + startIndex + ".png";
            //    ImageTool.getInstance().SaveImage(savePath, (BitmapSource)bitmapTarget);
            //}
            return bitmapTarget;

        } 
        /// <summary>
        /// 计算点在极坐标螺旋中第几环
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private int pointInRing(StylusPoint sp, double angle, bool isKeyPoint, StylusPoint center)
        {
            double distance = MathTool.getInstance().distanceSP2SP(center, sp);
            double minDistance = newSpiralWidth * angle;
            double doublering = (distance - minDistance) / myDoubleSpiralWidth;
            int ring = (int)(Math.Ceiling(doublering));
            if (Math.Ceiling(doublering) - doublering <= 0.01) ring++;
            if (!isKeyPoint && Math.Ceiling(doublering) - doublering >= 0.99)
                ring--;
            return ring;
        }
        private int pointInRingRightSpiral(StylusPoint sp, double angle, bool isKeyPoint, StylusPoint center)
        {
            double distance = MathTool.getInstance().distanceSP2SP(center, sp);
            double minDistance = newSpiralWidth * (angle+Math.PI);
            double doublering = (distance - minDistance) / myDoubleSpiralWidth;
            int ring = (int)(Math.Ceiling(doublering));
            if (Math.Ceiling(doublering) - doublering <= 0.01) ring++;
            if (!isKeyPoint && Math.Ceiling(doublering) - doublering >= 0.99)
                ring--;
            return ring;
        }
        /// <summary>
        /// 计算点在极坐标螺旋中第几环
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private int pointInRing(StylusPoint sp, double angle, bool isKeyPoint, int startIndex, StylusPoint center)
        {
            double distance = MathTool.getInstance().distanceSP2SP(center, sp);
            if (isKeyPoint)
            {
                return (startIndex + 90) / 360;
            }
            double minDistance = newSpiralWidth * angle;
            double doublering = (distance - minDistance) / myDoubleSpiralWidth;
            if (doublering <= 0)
            {
                return 0;
            }
            else
            {
                int ring = (int)(Math.Ceiling(doublering));
                double difference = Math.Ceiling(doublering) - doublering;
                if (difference <= 0.01 && difference > 0) ring++;
                if (isKeyPoint && (difference >= 0.99))
                    ring--;
                return ring;
            }
        }
        /// <summary>
        /// 把最后显示的几个关键帧多余的内容进行处理
        /// </summary>
        private WriteableBitmap RemoveLastKeyFramesRedundance(int index, KeyFrame keyFrame, int startIndex, double left, double top)
        {
            try
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(keyFrame.ImageName);
                BitmapSource bitmapSource = (BitmapSource)keyFrame.Image.Source;
                int w = bitmap.Width, h = bitmap.Height;
                int bytes = w * h * 4;
                byte[] ArgbValues = new byte[bytes];
                bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
                double pointWidth = keyFrame.Width / w;
                StylusPoint startPoint = points[startIndex];
                double angle = 0;
                int indexBmp = 0;
                ////对图片连接处进行渐变
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        indexBmp = 4 * (w * j + i);
                        if (ArgbValues[indexBmp + 3] != 0)
                        {
                            double x, y;
                            x = left + i * pointWidth; y = top + j * pointWidth;
                            StylusPoint currPoint = new StylusPoint(x, y);

                            angle = MathTool.getInstance().getAngleP2P(Center, currPoint);
                            double currangle = angle / 180 * Math.PI;
                            int currPointInRing = pointInRing(currPoint, currangle, false, Center);
                            double angle2 = 0;
                            if (isClockWise)
                            {
                                angle2 = (currPointInRing) * 360 - angle;
                            }
                            else
                            {
                                angle2 = (currPointInRing) * 360 + angle;
                            }
                            if (angle2 > lastKeyPointAngle)
                            {
                                ArgbValues[indexBmp + 3] = 0;
                            }

                        }
                    }

                }
                WriteableBitmap bitmapTarget = new WriteableBitmap(w, h, bitmapSource.DpiX, bitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
                Int32Rect sourceRect = new Int32Rect(0, 0, w, h);
                bitmapTarget.WritePixels(sourceRect, ArgbValues, w * 4, 0);
                //keyFrame.Image.Source = null;
                //keyFrame.Image.Source = bitmapTarget;
                ArgbValues = null;
                return bitmapTarget;
            }
            catch (InvalidCastException e)
            {
                throw (e);    // Rethrowing exception e
            }

        }


        #endregion
    }
}
