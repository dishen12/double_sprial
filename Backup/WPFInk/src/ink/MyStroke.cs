using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows;
//using WPFInk.recognizer;
using WPFInk.tool;

namespace WPFInk.ink
{
    public class MyStroke
    {
        private Stroke _stroke; // 对应的wpf的stroke结构
        private int _startTime, _endTime;//笔迹绘制的开始时间和结束时间

        public MyStroke(Stroke stroke)
        {
            Stroke = stroke;
        }

		//private Linearity _linearity;
		public double m, c;//直线方程的两个参数
		public StylusPointCollection adjustedPoints;//调整过后的点列（规则点列）
		private bool isSketchConnector = false;
		private bool isExist = false;
		private string videoPath = "";
        /// <summary>
        /// Stroke属性
        /// </summary>
        public Stroke Stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
        }

        //属性，记录stroke的颜色宽度等信息
        public DrawingAttributes DrawingAttributes
        {
            get { return Stroke.DrawingAttributes; }
            set { Stroke.DrawingAttributes = value; }
        }

        //属性：记录stroke上的所有点
        public StylusPointCollection StylusPoints
        {
            get { return Stroke.StylusPoints; }
            set { Stroke.StylusPoints = value; }
        }

        

        //属性：记录stroke开始创建时间
        public int StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        //属性：记录stroke创建结束时间
        public int EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        /// <summary>
        /// 属性：stroke起点
        /// </summary>
        public StylusPoint startPoint
        {
            get { return Stroke.StylusPoints[0]; }
        }

        /// <summary>
        /// 属性: stroke终点
        /// </summary>
        public StylusPoint endPoint
        {
            get { return Stroke.StylusPoints.Last(); }
        }

        //记录是否在group中
        public bool IsInGroup
        {
            get;
            set;
        }

        /// <summary>
        ///记录stroke所在的group
        /// </summary>
        public StrokeGroup Group
        {
            get;
            set;
        }

        //记录线性
        //public Linearity linearity
        //{
        //    get { return _linearity; }
        //    set { _linearity = value; }
        //}

        /// <summary>
        /// 返回包围盒
        /// </summary>
        /// <returns></returns>
        public Rect getBounds()
        {
            return Stroke.GetBounds();
        }


        public bool IsSketchConnector
        {
            get { return isSketchConnector; }
            set { isSketchConnector = value; }
         
        }


        /// <summary>
        /// 向笔画中增加一个点集
        /// </summary>
        /// <param name="points">要增加的点集</param>
        public void addPoints(StylusPointCollection points)
        {
            StylusPoints.Add(points);
            //this.endPoint = points[points.Count - 1];
            this.EndTime = MyTimer.getInstance().getTime();
        }

       
        
        

		public string VideoPath
		{
			get { return videoPath; }
			set { videoPath = value; }
		}

		public bool IsExist
		{
			get { return isExist; }
			set { isExist = value; }
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

		}
    }
}