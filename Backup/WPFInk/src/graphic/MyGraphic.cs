using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
namespace WPFInk.graphic
{
	public class MyGraphic
	{
        public MyGraphic(int myGraphicID, StrokeCollection strokes, Shape shape)
		{
			this._myGraphicID = myGraphicID;
			this._strokes = strokes;
			this._shape = shape;
			this._shape.VerticalAlignment = VerticalAlignment.Top;
			this._shape.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			this._shape.Stroke = new SolidColorBrush(Color.FromArgb(0,255,0,0));
            this._shape.StrokeThickness = 2;
		}
		private int _myGraphicID;//标识
		private StrokeCollection _strokes;//表达图形的笔迹
		private Shape _shape;//图形
        private string _shapeType;//图形类型，如椭圆、矩形
		private int _graphicLinkNodeID=0;
		private int _lastGraphicLinkNodeID = 0;
		public StrokeCollection textStrokeCollection = new StrokeCollection();
		private string text = null;//图形中的文本
        private StrokeCollection pentagramStrokes = null;//五角星批注
        private StylusPointCollection polyPoints = new StylusPointCollection();

        public StrokeCollection PentagramStrokes
        {
            get { return pentagramStrokes; }
            set { pentagramStrokes = value; }
        }

        public StylusPointCollection PolyPoints
        {
            get { return polyPoints; }
            set { polyPoints = value; }
        }

        


        public string ShapeType
        {
            get { return _shapeType; }
            set { _shapeType = value; }
        }
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public int LastGraphicLinkNodeID
		{
			get { return _lastGraphicLinkNodeID; }
			set { _lastGraphicLinkNodeID = value; }
		}


		public int GraphicLinkNodeID
		{
			get { return _graphicLinkNodeID; }
			set { _graphicLinkNodeID = value; }
		}

		public int MyGraphicID
		{
			get { return _myGraphicID; }
			set { _myGraphicID = value; }
		}
		

		public StrokeCollection Strokes
		{
			get { return _strokes; }
			set { _strokes = value; }
		}

		public Shape Shape
		{
			get { return _shape; }
			set { _shape = value; }
		}

		public void addTextStroke(Stroke s)
		{
			textStrokeCollection.Add(s);
		}
        /// <summary>
        /// 修改Stroke
        /// </summary>
        /// <param name="s"></param>
        public void updateStrokes(StrokeCollection s)
        {
            this.Strokes = s;
        }
        public void updateShape(Shape shape)
        {
            this.Shape = shape;
        }
	}
}
