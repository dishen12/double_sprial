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
    public class MySpiral
    {
        #region 螺旋线私有变量
        private double spiralWidth = 0;//螺距        
        private StylusPoint center;//中心点
        private StylusPoint leftCenter;//左中心点
        private double spiralStrokeWidth = 6;//螺旋线宽度
        private Stroke spiralStroke;//螺旋线笔迹
        private int spiralCount = 0;//螺旋线的半周数，也就是旋转多少个半周
        private InkCanvas _inkCanvas;//显示螺旋线的画板
        private Image spiralStrokeImage;//螺旋线转化的图片
        private bool isClockwise = true;//是否是顺时针
        private double newSpiralWidth = 0;//极坐标下螺旋摘要的螺距
        private Color color;
       // private Stroke movePath;//和螺旋线平行的轨迹，用作移动轨迹

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="spiralWidth">螺距</param>
        /// <param name="center">中心点</param>
        /// <param name="spiralStrokeWidth">螺旋线宽度</param>
        public MySpiral(double spiralWidth, Color color, StylusPoint center, double spiralStrokeWidth, int spiralCount, InkCanvas _inkCanvas, bool isClockwize)
        {
            this.spiralWidth = spiralWidth;
            this.newSpiralWidth = SpiralWidth / Math.PI / 2;
            this.center = center;
            this.leftCenter = new StylusPoint(center.X - spiralWidth / 2, center.Y);
            this.spiralStrokeWidth = spiralStrokeWidth;
            this.spiralCount = spiralCount;
            this._inkCanvas = _inkCanvas;
            this.isClockwise = isClockwize;
            this.color = color;
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
        public MySpiral(double spiralWidth,double color, StylusPoint center, int spiralCount, InkCanvas _inkCanvas, bool isClockwize)
        {
            this.spiralWidth = spiralWidth;
            this.newSpiralWidth = SpiralWidth / Math.PI / 2;
            this.center = center;
            this.leftCenter = new StylusPoint(center.X - spiralWidth / 2, center.Y);
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

        public StylusPoint LeftCenter
        {
            get { return leftCenter; }
            set { leftCenter = value; }
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
            double max = Math.PI * 15;
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
            sCenter.DrawingAttributes.Color = color;
            sCenter.DrawingAttributes.Width = SpiralStrokeWidth;
            sCenter.DrawingAttributes.Height = SpiralStrokeWidth;
            _inkCanvas.Strokes.Add(sCenter);
            for (double i = Math.PI / 2; i < max; i += interval)
            {
                r = newSpiralWidth * i;
                x = center.X + r * Math.Cos(-i);
                y = center.Y + r * Math.Sin(-i);
                StylusPoint sp = new StylusPoint(x, y);
                //InkTool.getInstance().getInstance().drawPoint();
                spc.Add(sp);
                /*r -= spiralWidth / 2;
                x = center.X + r * Math.Cos(-i);
                y = center.Y + r * Math.Sin(-i);
                StylusPoint sp2 = new StylusPoint(x, y);
                spcMoveTrack.Add(sp2);*/
            }

            Stroke s = new Stroke(spc);
            s.DrawingAttributes.Color = color;
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
        private void drawSpiralClockwise()
        {
            StylusPointCollection spc = new StylusPointCollection();//螺旋线上点集合
            double x = 0;
            double y = 0;
            double r = 0;
            double max = Math.PI * 15;
            double interval = Math.PI / 180;

            StylusPointCollection spcCenter = new StylusPointCollection();
            for (double i = 0; i < Math.PI / 2; i += interval)
            {
                r = newSpiralWidth * i;
                x = center.X + r * Math.Cos(i);
                y = center.Y + r * Math.Sin(i);
                StylusPoint sp = new StylusPoint(x, y);
                spcCenter.Add(sp);
            }
            Stroke sCenter = new Stroke(spcCenter);
            sCenter.DrawingAttributes.Color = Color.FromArgb(255, 255, 255, 0);
            sCenter.DrawingAttributes.Width = SpiralStrokeWidth;
            sCenter.DrawingAttributes.Height = SpiralStrokeWidth;
            _inkCanvas.Strokes.Add(sCenter);
            for (double i = Math.PI/2; i < max; i += interval)
            {
                r = newSpiralWidth * i;
                x = center.X + r * Math.Cos(i);
                y = center.Y + r * Math.Sin(i);
                StylusPoint sp = new StylusPoint(x, y);
                spc.Add(sp);
            }
            Stroke s = new Stroke(spc);
            //s.DrawingAttributes.FitToCurve = true;
            s.DrawingAttributes.Color = Color.FromArgb(0, 255, 255, 0);
            s.DrawingAttributes.Width = SpiralStrokeWidth;
            s.DrawingAttributes.Height = SpiralStrokeWidth;
            spiralStroke = s;
            _inkCanvas.Strokes.Add(spiralStroke);
        }
        
        #endregion
    }
}
