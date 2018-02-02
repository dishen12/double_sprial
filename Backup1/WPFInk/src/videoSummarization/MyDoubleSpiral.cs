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
using System.Windows;
using System.IO;
using WPFInk.Global;
using WPFInk.tool;
using WPFInk.ink;

namespace WPFInk.videoSummarization
{
    /// <summary>
    /// 螺旋线类
    /// </summary>
    public class MyDoubleSpiral
    {
        #region 螺旋线私有变量
        private double spiralWidth = 0;//螺距        
        private StylusPoint center;//中心点
        private double spiralStrokeWidth = 6;//螺旋线宽度
        private Stroke spiralStroke;//螺旋线笔迹
        private Stroke spiralStroke1;//螺旋线笔迹

       
        private int spiralPointsCount = 0;

        
        private Stroke spiralStrokeRight;//螺旋线笔迹
        private int spiralCount = 0;//螺旋线的半周数，也就是旋转多少个半周
        private InkCanvas _inkCanvas;//显示螺旋线的画板
        private Image spiralStrokeImage;//螺旋线转化的图片
        private bool isClockwise = true;//是否是顺时针
        private double newSpiralWidth = 0;//极坐标下螺旋摘要的螺距
        private double spiralDistance = 0;
        private StylusPoint leftCenter;
        private StylusPoint rightCenter;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="spiralWidth">螺距</param>
        /// <param name="center">中心点</param>
        /// <param name="spiralStrokeWidth">螺旋线宽度</param>
        public MyDoubleSpiral(double spiralWidth, StylusPoint center, double spiralDistance, double spiralStrokeWidth, int spiralCount, InkCanvas _inkCanvas, bool isClockwize)
        {
            this.spiralWidth = spiralWidth;
            this.newSpiralWidth = SpiralWidth / Math.PI / 2;
            this.center = center;
            this.spiralDistance = spiralDistance;
            this.leftCenter = new StylusPoint(center.X-spiralDistance  / 2, center.Y);
            this.rightCenter = new StylusPoint(center.X + spiralDistance / 2, center.Y);
            //this.leftCenter = new StylusPoint(center.X - spiralWidth / 2, center.Y);
            this.spiralStrokeWidth = spiralStrokeWidth;
            this.spiralCount = spiralCount;
            this._inkCanvas = _inkCanvas;
            this.isClockwise = isClockwize;
            if (isClockwise)
            {
                drawSpiralClockwise();
            }
            else
            {
                drawSpiral();
            }
        }
        /// <summary>
        /// 构造函数，螺旋线宽度默认为5
        /// </summary>
        /// <param name="spiralWidth">螺距</param>
        /// <param name="center">中心点</param>
        public MyDoubleSpiral(double spiralWidth, StylusPoint center, double spiralDistance, int spiralCount, InkCanvas _inkCanvas, bool isClockwize)
        {
            this.spiralWidth = spiralWidth;
            this.newSpiralWidth = SpiralWidth / Math.PI / 2;
            this.center = center;
            this.spiralDistance = spiralDistance;
            this.leftCenter = new StylusPoint(center.X - spiralDistance / 2, center.Y);
            this.rightCenter = new StylusPoint(center.X + spiralDistance / 2, center.Y);
            this.spiralCount = spiralCount;
            this._inkCanvas = _inkCanvas;
            this.isClockwise = isClockwize;
            if (isClockwise)
            {
                drawSpiralClockwise();
            }
            else
            {
                drawSpiral();
            }
        }
        #endregion


        #region 封装变量
        /*public Stroke MovePath
        {
            get { return movePath; }
            set { movePath = value; }
        }*/
        /// <summary>
        /// 极坐标下螺旋摘要的螺距
        /// </summary>
        public double NewSpiralWidth
        {
            get { return newSpiralWidth; }
            set { newSpiralWidth = value; }
        }
        /// <summary>
        /// 左中心点
        /// </summary>
        public StylusPoint LeftCenter
        {
            get { return leftCenter; }
            set { leftCenter = value; }
        }

        /// <summary>
        /// 右中心点
        /// </summary>
        public StylusPoint RightCenter
        {
            get { return rightCenter; }
            set { rightCenter = value; }
        }
        public bool IsClockwise
        {
            get { return isClockwise; }
            set { isClockwise = value; }
        }
        public Image SpiralStrokeImage
        {
            get { return spiralStrokeImage; }
            set { spiralStrokeImage = value; }
        }
        /// <summary>
        /// 螺旋线宽度
        /// </summary>
        public double SpiralStrokeWidth
        {
            get { return spiralStrokeWidth; }
            set { spiralStrokeWidth = value; }
        }

        /// <summary>
        /// 螺距
        /// </summary>
        public double SpiralWidth
        {
            get { return spiralWidth; }
            set { spiralWidth = value; }
        }

        /// <summary>
        /// 螺旋线笔迹,自动由drawSpiral函数生成，无法修改
        /// </summary>
        public Stroke SpiralStroke
        {
            get { return spiralStroke; }
        }
        /// <summary>
        /// 螺旋线笔迹,平直轨道下边界
        /// </summary>
        public Stroke SpiralStroke1
        {
            get { return spiralStroke1; }
            set { spiralStroke1 = value; }
        }
       
        /// <summary>
        /// 螺旋线的中心点
        /// </summary>
        public StylusPoint Center
        {
            get { return center; }
            set { center = value; }
        }

        /// <summary>
        /// 螺旋线的半周数
        /// </summary>
        public int SpiralCount
        {
            get { return spiralCount; }
            set { spiralCount = value; }
        }

        /// <summary>
        /// 显示螺旋线的画板
        /// </summary>
        public InkCanvas InkCanvas
        {
            get { return _inkCanvas; }
            set { _inkCanvas = value; }
        }


        /// <summary>
        /// 两螺旋间距
        /// </summary>
        public double SpiralDistance
        {
            get { return spiralDistance; }
            set { spiralDistance = value; }
        }

        /// <summary>
        /// 螺旋部分点数
        /// </summary>
        public int SpiralPointsCount
        {
            get { return spiralPointsCount; }
            set { spiralPointsCount = value; }
        }
        // private Stroke movePath;//和螺旋线平行的轨迹，用作移动轨迹

        #endregion

        #region 普通函数

        /// <summary>
        /// 画对逆时针称螺旋线
        /// </summary>
        private void drawSpiral()
        {
            StylusPointCollection spc = new StylusPointCollection();//螺旋线上点集合
            //StylusPointCollection spcMoveTrack = new StylusPointCollection();//螺旋线上点集合

            double x = 0;
            double y = 0;
            double r = 0;
            double max = Math.PI * spiralCount;
            double interval = Math.PI / 180;

            StylusPointCollection spcCenter = new StylusPointCollection();
            for (double i = 0; i < Math.PI / 2; i += interval)
            {
                r = newSpiralWidth * i;
                x = center.X + r * Math.Cos(-i);
                y = center.Y + r * Math.Sin(-i);
                StylusPoint sp = new StylusPoint(x, y);
                spcCenter.Add(sp);
            }
            Stroke sCenter = new Stroke(spcCenter);
            sCenter.DrawingAttributes.Color = Color.FromArgb(255, 0, 0, 0);
            sCenter.DrawingAttributes.Width = SpiralStrokeWidth;
            sCenter.DrawingAttributes.Height = SpiralStrokeWidth;
            _inkCanvas.Strokes.Add(sCenter);
            

            for (double i = Math.PI / 2; i < max; i += interval)
            {
                r = newSpiralWidth * i;
                x = center.X + r * Math.Cos(-i);
                y = center.Y + r * Math.Sin(-i);
                StylusPoint sp = new StylusPoint(x, y);
                //InkTool.getInstance().drawPoint();
                spc.Add(sp);
                /*r -= spiralWidth / 2;
                x = center.X + r * Math.Cos(-i);
                y = center.Y + r * Math.Sin(-i);
                StylusPoint sp2 = new StylusPoint(x, y);
                spcMoveTrack.Add(sp2);*/
            }

            Stroke s = new Stroke(spc);
            s.DrawingAttributes.Color = Color.FromArgb(255, 0, 0, 0);
            s.DrawingAttributes.Width = SpiralStrokeWidth;
            s.DrawingAttributes.Height = SpiralStrokeWidth;
            spiralStroke = s;
            _inkCanvas.Strokes.Add(spiralStroke);

            //movePath = new Stroke(spcMoveTrack);
            //movePath.DrawingAttributes.Color = Color.FromArgb(50, 0, 255, 0);
            //movePath.DrawingAttributes.Width = SpiralStrokeWidth;
            //movePath.DrawingAttributes.Height = SpiralStrokeWidth;
            //_inkCanvas.Strokes.Add(movePath);
        }
        /// <summary>
        /// 画对顺时针称螺旋线
        /// </summary>
        /// 
        
        private void drawSpiralClockwise()
        {
            
            double x = 0;
            double y = 0;
            double r = 0;
            double endLeftX=0;
            double endRightX=0;
            double endLeftY=0;
            double endRightY=0;
            double max = Math.PI * (spiralCount*2-0.5);
            double interval = Math.PI / 180;
            
            //求平直轨道下边缘与螺旋交点所需变量
            double minDistance = 100;
            double distance = 0;
            double intersectionX = 0;
            double intersectionY = 0;

             //计算两螺旋外端点
            endLeftX = leftCenter.X  + newSpiralWidth * max * Math.Cos(max);
            endLeftY=center.Y + newSpiralWidth * max * Math.Sin(max);
            endRightX = rightCenter.X  + newSpiralWidth * max * Math.Cos(-(max + Math.PI));
            endRightY=center.Y + newSpiralWidth * max * Math.Sin(-(max+Math.PI));


            //螺旋线上点集合
            //绘制左螺旋中心
            StylusPointCollection spcCenterLeft = new StylusPointCollection();
            for (double i = 0; i < Math.PI / 2; i += interval)
            {
                r = newSpiralWidth * i;
                x = leftCenter.X + r * Math.Cos(i);
                y = center.Y + r * Math.Sin(i);
                StylusPoint sp = new StylusPoint(x, y);
                spcCenterLeft.Add(sp);
            }
            Stroke sCenterLeft = new Stroke(spcCenterLeft);
            sCenterLeft.DrawingAttributes.Color = Color.FromArgb(255, 255, 255, 255);
            sCenterLeft.DrawingAttributes.Width = SpiralStrokeWidth;
            sCenterLeft.DrawingAttributes.Height = SpiralStrokeWidth;
            _inkCanvas.Strokes.Add(sCenterLeft);

            //绘制左螺旋外围
            StylusPointCollection spcLeft = new StylusPointCollection();
            for (double i = Math.PI/2; i < max; i += interval)
            {
                r = newSpiralWidth * i;
                x = leftCenter.X + r * Math.Cos(i);
                y = center.Y + r * Math.Sin(i);
                StylusPoint sp = new StylusPoint(x, y);
                spcLeft.Add(sp);
                if (i > max - Math.PI * 2 && i < max - Math.PI)
                {
                   distance = MathTool.getInstance().distanceP2L(sp, new StylusPoint(endLeftX, endLeftY + 2 * spiralWidth), new StylusPoint(endRightX, endRightY + 2 * spiralWidth));
                   if(minDistance>distance)
                   {
                       intersectionX = x;
                       intersectionY = y;
                       minDistance = distance;
                   } 
                    
                }

            }
            Stroke sLeft = new Stroke(spcLeft);
            spiralPointsCount = spcLeft.Count;
            //s.DrawingAttributes.FitToCurve = true;
            sLeft.DrawingAttributes.Color = Color.FromArgb(255, 255, 255, 255);
            sLeft.DrawingAttributes.Width = SpiralStrokeWidth;
            sLeft.DrawingAttributes.Height = SpiralStrokeWidth;
            _inkCanvas.Strokes.Add(sLeft);
            spiralStroke = sLeft;


            //绘制平铺轨道
           
            StylusPointCollection spcHorizontal = new StylusPointCollection();
            StylusPointCollection spcHorizontal1 = new StylusPointCollection();
            //////为什么往左移动两个单位才能接上？
            for (double i = endLeftX - 2; i < endRightX - 2; i += SpiralStrokeWidth/2)
            { 
                x = i;
                y = endLeftY;
                StylusPoint sp = new StylusPoint(x, y);
                spcHorizontal.Add(sp);
                //绘制下边缘
                if (x > intersectionX && x < Center.X + (Center.X - intersectionX))
                {
                    StylusPoint sp1 = new StylusPoint(x, intersectionY);
                    spcHorizontal1.Add(sp1);
                }
            }
            spiralStroke.StylusPoints.Add(spcHorizontal);
            spiralStroke1=new Stroke(spcHorizontal1);
            spiralStroke1.DrawingAttributes.Color = Color.FromArgb(255, 255, 255, 255);
            spiralStroke1.DrawingAttributes.Width = SpiralStrokeWidth;
            spiralStroke1.DrawingAttributes.Height = SpiralStrokeWidth;
            _inkCanvas.Strokes.Add(spiralStroke1);


            //绘制右螺旋外围
            StylusPointCollection spcRight = new StylusPointCollection();
            //for (double i = max + Math.PI; i > Math.PI * 0.5; i -= interval)
            //{
            //    //r = newSpiralWidth * (i - Math.PI);
            //    //x = rightCenter.X + r * Math.Cos(-i);
            //    //y = center.Y + r * Math.Sin(-i);
            //    //StylusPoint sp = new StylusPoint(x, y);
            //    ////InkTool.getInstance().drawPoint();
            //    //spcRight.Add(sp);
            //    /*r -= spiralWidth / 2;
            //    x = center.X + r * Math.Cos(-i);
            //    y = center.Y + r * Math.Sin(-i);
            //    StylusPoint sp2 = new StylusPoint(x, y);
            //    spcMoveTrack.Add(sp2);*/



            //}
            for (double i = max; i > Math.PI * 0.5; i -= interval)
            {
                r = newSpiralWidth * i;
                x = rightCenter.X + r * Math.Cos(-i + Math.PI);
                y = rightCenter.Y + r * Math.Sin(-i + Math.PI);
                StylusPoint sp = new StylusPoint(x, y);
                //InkTool.getInstance().drawPoint();
                spcRight.Add(sp);
            }
            //Stroke sRight = new Stroke(spcRight);
            //sRight.DrawingAttributes.Color = Color.FromArgb(255, 0, 0, 0);
            //sRight.DrawingAttributes.Width = SpiralStrokeWidth;
            //sRight.DrawingAttributes.Height = SpiralStrokeWidth;
            //_inkCanvas.Strokes.Add(sRight);
            spiralStroke.StylusPoints.Add(spcRight);

            //绘制右螺旋中心
            //StylusPointCollection spcCenterRight = new StylusPointCollection();
            //for (double i = Math.PI * 1.5; i >= Math.PI; i -= interval)
            //{
            //    r = newSpiralWidth * (i - Math.PI);
            //    x = rightCenter.X  + r * Math.Cos(-i);
            //    y = center.Y + r * Math.Sin(-i);
            //    StylusPoint sp = new StylusPoint(x, y);
            //    spcCenterRight.Add(sp);
            //}
            //Stroke sCenterRight = new Stroke(spcCenterRight);
            //sCenterRight.DrawingAttributes.Color = Color.FromArgb(255, 0, 0, 0);
            //sCenterRight.DrawingAttributes.Width = SpiralStrokeWidth;
            //sCenterRight.DrawingAttributes.Height = SpiralStrokeWidth;
            //_inkCanvas.Strokes.Add(sCenterRight);

            StylusPointCollection spcCenterRight = new StylusPointCollection();
            for (double i = 0; i < Math.PI / 2; i += interval)
            {
                r = newSpiralWidth * i;
                x = rightCenter.X + r * Math.Cos(-i + Math.PI);
                y = rightCenter.Y + r * Math.Sin(-i + Math.PI);
                StylusPoint sp = new StylusPoint(x, y);
                spcCenterRight.Add(sp);
            }
            Stroke sCenterRight = new Stroke(spcCenterRight);
            sCenterRight.DrawingAttributes.Color = Color.FromArgb(255, 255, 255, 255);
            sCenterRight.DrawingAttributes.Width = SpiralStrokeWidth;
            sCenterRight.DrawingAttributes.Height = SpiralStrokeWidth;
            _inkCanvas.Strokes.Add(sCenterRight);

        }
        /// <summary>
        /// 向螺旋线中添加半圆
        /// </summary>
        public void addHalfCircle()
        {
            int pointX = 0;
            int pointY = 0;
            double pointX0 = 0;
            spiralCount++;
            double radius = spiralWidth + spiralCount * spiralWidth / 2;
            if (spiralCount % 2 == 0)//偶数,下半圆
            {
                pointX0 = center.X;
                for (pointX = (int)(pointX0 - radius) + 1; pointX <= (int)(pointX0 + radius) - 1; pointX++)
                {
                    pointY = (int)(center.Y + Math.Sqrt(radius * radius - (pointX - pointX0) * (pointX - pointX0)));
                    StylusPoint currPoint = new StylusPoint(pointX, pointY);
                    spiralStroke.StylusPoints.Add(currPoint);
                }
            }

            else//奇数，上半圆
            {
                pointX0 = center.X - spiralWidth / 2;
                for (pointX = (int)(pointX0 + radius); pointX >= Math.Ceiling(pointX0 - radius); pointX--)
                {
                    pointY = (int)(center.Y - Math.Sqrt(radius * radius - (pointX - pointX0) * (pointX - pointX0)));
                    StylusPoint currPoint = new StylusPoint(pointX, pointY);
                    spiralStroke.StylusPoints.Add(currPoint);
                }
            }
        }
        /// <summary>
        /// 向螺旋线中添加一个长度为arcLength的弧线,该函数不能用
        /// </summary>
        public void addArc(double arcLength)
        {
            double length = 0;
            StylusPoint lastPoint = spiralStroke.StylusPoints.Last();
            double radius = MathTool.getInstance().distanceP2P(lastPoint, center);
            double leftRadius = MathTool.getInstance().distanceP2P(lastPoint, leftCenter);
            StylusPoint prePoint = lastPoint;
            double pointX = lastPoint.X;
            double pointY = lastPoint.Y;
            double currentLength = 0;
            if (lastPoint.Y > center.Y)//原始尾点在下半圆
            {
                while ((int)pointY > center.Y && length < arcLength)
                {
                    pointX++;
                    pointY = (int)(center.Y + Math.Sqrt(radius * radius - (pointX - center.X) * (pointX - center.X)));
                    if (pointY == int.MinValue)
                    {
                        break;
                    }
                    StylusPoint currPoint = new StylusPoint(pointX, pointY);
                    spiralStroke.StylusPoints.Add(currPoint);
                    currentLength = MathTool.getInstance().distanceP2P(currPoint, prePoint);
                    length += currentLength;
                    prePoint = currPoint;
                }
                if (length < arcLength)//中间遇到由下半圆进入上半圆的情况
                {
                    spiralStroke.StylusPoints.Remove(spiralStroke.StylusPoints.Last());
                    StylusPoint turningPoint = new StylusPoint(pointX - 1, center.Y);
                    length -= currentLength;
                    length += MathTool.getInstance().distanceP2P(turningPoint, prePoint);
                    spiralStroke.StylusPoints.Add(turningPoint);
                    double currRadius = MathTool.getInstance().distanceP2P(turningPoint, leftCenter);

                    while (length < arcLength)
                    {
                        pointX--;
                        pointY = (int)(center.Y - Math.Sqrt(currRadius * currRadius - (pointX - leftCenter.X) * (pointX - leftCenter.X)));
                        StylusPoint currPoint = new StylusPoint(pointX, pointY);
                        spiralStroke.StylusPoints.Add(currPoint);
                        length += MathTool.getInstance().distanceP2P(currPoint, prePoint);
                        prePoint = currPoint;
                    }
                }
            }
            else//原始尾点在上半圆
            {
                while ((int)pointY < center.Y && length < arcLength)
                {
                    pointX--;
                    pointY = (int)(center.Y - Math.Sqrt(radius * radius - (pointX - leftCenter.X) * (pointX - leftCenter.X)));
                    if (pointY == int.MinValue)
                    {
                        break;
                    }
                    StylusPoint currPoint = new StylusPoint(pointX, pointY);
                    spiralStroke.StylusPoints.Add(currPoint);
                    length += MathTool.getInstance().distanceP2P(currPoint, prePoint);
                    currentLength = MathTool.getInstance().distanceP2P(currPoint, prePoint);
                    prePoint = currPoint;
                }
                if (length < arcLength)//中间遇到由上半圆进入下半圆的情况
                {
                    spiralStroke.StylusPoints.Remove(spiralStroke.StylusPoints.Last());
                    StylusPoint turningPoint = new StylusPoint(pointX + 1, center.Y);
                    length -= currentLength;
                    length += MathTool.getInstance().distanceP2P(turningPoint, prePoint);
                    spiralStroke.StylusPoints.Add(turningPoint);
                    double currRadius = MathTool.getInstance().distanceP2P(turningPoint, center);
                    while (length < arcLength)
                    {
                        pointX++;
                        pointY = (int)(center.Y + Math.Sqrt(currRadius * currRadius - (pointX - center.X) * (pointX - center.X)));
                        if (pointY == int.MinValue)
                        {
                            break;
                        }
                        StylusPoint currPoint = new StylusPoint(pointX, pointY);
                        spiralStroke.StylusPoints.Add(currPoint);
                        length += MathTool.getInstance().distanceP2P(currPoint, prePoint);
                        prePoint = currPoint;
                    }
                }
            }
        }
        public void ConvertSpiraltoPNG()
        {
            //InkCanvas inkCanvas = new InkCanvas();
            //inkCanvas.Strokes.Add(spiralStroke);
            //RenderTargetBitmap rtb = new RenderTargetBitmap((int)_inkCanvas.Width,
            //                (int)_inkCanvas.Height, 0, 0, PixelFormats.Default);
            //rtb.Render(inkCanvas);
            //BmpBitmapEncoder encoder = new BmpBitmapEncoder();            
            //encoder.Frames.Add(BitmapFrame.Create(rtb));

            //Stream stream = new FileStream(GlobalValues.FilesPath + @"\WPFInk\WPFInk\cache\spiral.png", FileMode.Create);
            //encoder.Save(stream);
            //stream.Close();
            //Image image = InkConstants.getImageFromName(GlobalValues.FilesPath + @"\WPFInk\WPFInk\cache\spiral.png");
            //System.Drawing.Bitmap bitmapSource = ConvertClass.getInstance().BitmapImageToBitMap((BitmapImage)image.Source);
            //System.Drawing.Bitmap bitmapTarget = new System.Drawing.Bitmap(bitmapSource.Width, bitmapSource.Height);
            //for (int i = 0; i < bitmapSource.Width; i++)
            //{
            //    for (int j = 0; j < bitmapSource.Height; j++)
            //    {
            //        System.Drawing.Color pixelColor = bitmapSource.GetPixel(i, j);
            //        System.Drawing.Color newColor;
            //        if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 0)
            //        {
            //            bitmapTarget.SetPixel(i, j, pixelColor);
            //        }
            //        else
            //        {
            //            newColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            //            bitmapTarget.SetPixel(i, j, newColor);
            //        }
            //    }
            //}
            //MemoryStream ms = new MemoryStream();
            //bitmapTarget.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //BitmapImage bitmapImage = new BitmapImage();
            //bitmapImage.BeginInit();
            //bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            //bitmapImage.EndInit();
            //image.Source = bitmapImage;
            //spiralStroke.DrawingAttributes.Color = Colors.Transparent;     
            //_inkCanvas.Children.Add(image);    

        }
        #endregion
    }
}
