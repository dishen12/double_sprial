using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using WPFInk.video;
using System.Windows.Shapes;


namespace WPFInk.ink
{
    public class MyButton
    {
        private Button _button;

        private double left = 0;
        private double top = 0;
        private double angle = 0;
        private TextBox textBoxTime;
        private double timeStart;
        private double timeEnd;
        private bool isStaticAnnotion = false;
        private InkFrame inkFrame;
		private bool isGlobal = false;
		private string _videoPath = null;
		private Brushes background = null;
		private double zoomRate = 1;
		private int id;//用于区别不同的mybutton
		private int nextId;
		private bool isDeleted = false;
		private double inkFrameWidth;
		private double inkFrameHeight;
		private Rectangle rBorder;
		private VideoAnnotation _videoAnnotation;
		private string videoFileName = "";
        private string analyzeResults = "";//笔迹识别结果字符串
        private double contentMoveX = 0;//内容移动的X坐标
        private double contentMoveY = 0;//内容移动的Y坐标
        private double contentScaling = 1;//内容缩放比例

        public double ContentScaling
        {
            get { return contentScaling; }
            set { contentScaling = value; }
        }

        public double ContentMoveY
        {
            get { return contentMoveY; }
            set { contentMoveY = value; }
        }

        public double ContentMoveX
        {
            get { return contentMoveX; }
            set { contentMoveX = value; }
        }

		public string AnalyzeResults
		{
			get { return analyzeResults; }
			set { analyzeResults = value; }
		}

		public string VideoFileName
		{
			get { return videoFileName; }
			set { videoFileName = value; }
		}


		public VideoAnnotation VideoAnnotation
		{
			get { return _videoAnnotation; }
			set { _videoAnnotation = value; }
		}

		public Rectangle RBorder
		{
			get { return rBorder; }
			set { rBorder = value; }
		}

		public double InkFrameHeight
		{
			get { return inkFrameHeight; }
			set { inkFrameHeight = value; }
		}

		public double InkFrameWidth
		{
			get { return inkFrameWidth; }
			set { inkFrameWidth = value; }
		}

		public bool IsGlobal
		{
			get { return isGlobal; }
			set { isGlobal = value; }
		}


        

        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }

        public int NextId
        {
            get { return nextId; }
            set { nextId = value; }
        }
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public double ZoomRate
        {
            get { return zoomRate; }
            set { zoomRate = value; }
        }

        public Brushes Background
        {
            get { return background; }
            set { background = value; }
        }

        

        public MyButton(Button button)
        {
            this._button = button;
            this._button.HorizontalAlignment=HorizontalAlignment.Left;
            this._button.VerticalAlignment=VerticalAlignment.Top;
			this._button.MinHeight = 50;
			this._button.MinWidth = 50;
        }

        void textBoxTime_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MessageBox.Show(((TextBox)sender).Text.ToString());
            } 
            
        }
        

        public string VideoPath
        {
            get { return _videoPath; }
            set { _videoPath = value; }
        }
        public Button Button
        {
            set {_button=value;}
            get{return _button;}
        }

        public double Width
        {
            get { return _button.Width; }
            set { _button.Width = value; }
        }

        public double Height
        {
            get { return _button.Height; }
            set { _button.Height = value; }
        }

        public double Left
        {
            get { return left; }
            set { left = value; }
        }

        public double Top
        {
            get { return top; }
            set { top = value; }
        }

        //旋转角度
        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public TextBox TextBoxTime
        {
            get { return textBoxTime; }
            set { textBoxTime=value;}
        }

        public double TimeStart
        {
            get { return timeStart; }
            set { timeStart = value; }
        }

        public double TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = value; }
        }


        public bool IsStaticAnnotion
        {
            set { isStaticAnnotion = value; }
            get { return isStaticAnnotion; }
        }
        public void adjustTextBoxTime()
        {
            InkConstants.AddTextBoxTime(this);
			//InkConstants.AddTextBoxTime2(this);
        }


        public InkFrame InkFrame
        {
            get { return inkFrame; }
            set { inkFrame = value; }
        }

        public void updateArrow(InkCanvas ic,InkCollector _inkCollector)
        {
            List<MyArrow> maList = new List<MyArrow>();
            foreach (MyArrow myArrow in _inkCollector.Sketch.MyArrows)
            {
                if (myArrow.IsDeleted == false && myArrow.NextMyButton == this && ic.Children.IndexOf(myArrow.Arrow) > -1 && myArrow.PreMyButton.isDeleted == false)
                {
                    ic.Children.Remove(myArrow.Arrow);
                    ThumbConnector tc = new ThumbConnector(myArrow.PreMyButton, this);
                    MyArrow ma = new MyArrow(tc.arrow);
                    ma.PreMyButton = myArrow.PreMyButton;
                    ma.NextMyButton = this;
                    ma.StartPoint = tc.startPoint;
                    ma.EndPoint = tc.endPoint;
                    maList.Add(ma);
                    continue;
                }
                if (myArrow.IsDeleted == false && myArrow.PreMyButton == this && ic.Children.IndexOf(myArrow.Arrow) > -1 && myArrow.NextMyButton.isDeleted == false)
                {
                    ic.Children.Remove(myArrow.Arrow);
                    ThumbConnector tc = new ThumbConnector(this, myArrow.NextMyButton);
                    MyArrow ma = new MyArrow(tc.arrow);
                    ma.PreMyButton = this;
                    ma.NextMyButton = myArrow.NextMyButton;
                    ma.StartPoint = tc.startPoint;
                    ma.EndPoint = tc.endPoint;
                    maList.Add(ma);
                    continue;
                }
                maList.Add(myArrow);
            }

            for (int i = 0; i < maList.Count; i++)
            {
                if (_inkCollector.Sketch.MyArrows[i] != maList[i])
                {
                    _inkCollector.Sketch.MyArrows[i] = maList[i];
                    ic.Children.Add(_inkCollector.Sketch.MyArrows[i].Arrow);
                }                
            }

            
        }

        public void setLocation(int x, int y)
        {
            Thickness margin = _button.Margin;
            margin.Left = x;
            margin.Right = 0;
            margin.Top = y;
            margin.Bottom = 0;
            _button.Margin = margin;
        }

        public void addContent()
        {
            this.Button.Content = inkFrame;
        }

		public void updateRectangles(InkCanvas inkCanvas, InkCollector inkCollector)
		{
			if (isGlobal)
			{
				if (rBorder != null)
				{
					inkCanvas.Children.Remove(rBorder);
				}
				Rectangle rBorderRectangle = new Rectangle();
				rBorderRectangle.HorizontalAlignment = HorizontalAlignment.Left;
				rBorderRectangle.VerticalAlignment = VerticalAlignment.Top;
				rBorderRectangle.Margin = new Thickness(Left, Top + Height, 0, 0);
				rBorderRectangle.Height = 10;
				rBorderRectangle.Width = Width;
				rBorderRectangle.Stroke = new SolidColorBrush(Colors.Black);
				if (this.Button.Visibility == Visibility.Visible)
				{
					inkCanvas.Children.Add(rBorderRectangle);
				}
				rBorder = rBorderRectangle;
				List<MySmallRectangle> msrs = new List<MySmallRectangle>();
				foreach (MySmallRectangle msr in inkCollector.Sketch.mySmallRectangles)
				{
					if (msr.MyButton.VideoPath == this.VideoPath)
					{
						msrs.Add(msr);
						inkCanvas.Children.Remove(msr.Rectangle);
					}
				}
				foreach (MySmallRectangle msr in msrs)
				{
					inkCollector.Sketch.mySmallRectangles.Remove(msr);
				}
				List<MyButton> strokeCountMyButtonList = new List<MyButton>();
				foreach (MyButton mb in inkCollector.Sketch.MyButtons)
				{
					if (mb.IsDeleted == false && mb.IsGlobal == false && mb.VideoPath == _videoPath&&mb.Button.Visibility==Visibility.Visible)
					{
						strokeCountMyButtonList.Add(mb);
					}
				}
				int smallRectangleCount = strokeCountMyButtonList.Count;
				if (smallRectangleCount > 0)
				{
					strokeCountMyButtonList.Sort(new ComparerStrokeCount());
					double totalTime = timeEnd;
					int mostStrokeCount = strokeCountMyButtonList[smallRectangleCount - 1].InkFrame._inkCanvas.Children.Count + strokeCountMyButtonList[smallRectangleCount - 1].InkFrame._inkCanvas.Strokes.Count; //最大的stroke count
					double rate;
					if (mostStrokeCount != 0)
					{
						for (int i = smallRectangleCount - 1; i >= 0; i--)
						{
							Rectangle r = new Rectangle();
							r.Stroke = null;
							r.HorizontalAlignment = HorizontalAlignment.Left;
							r.VerticalAlignment = VerticalAlignment.Top;
							int strokeCount = strokeCountMyButtonList[i].InkFrame._inkCanvas.Children.Count + strokeCountMyButtonList[i].InkFrame._inkCanvas.Strokes.Count;
							r.Margin = new Thickness(rBorder.Margin.Left+1 + rBorder.Width * (strokeCountMyButtonList[i].TimeStart / totalTime), rBorder.Margin.Top+1, 0, 0);



							rate = (double)strokeCount / (double)mostStrokeCount;

							r.Fill = new SolidColorBrush(Color.FromRgb(255, Convert.ToByte(255 - 255 * rate), Convert.ToByte(255 - 255 * rate)));
							r.Height = rBorder.Height-2;
							r.Width = rBorder.Width * (strokeCountMyButtonList[i].TimeEnd - strokeCountMyButtonList[i].TimeStart) / totalTime;
							r.MouseLeftButtonUp += new MouseButtonEventHandler(r_MouseLeftButtonUp);
							MySmallRectangle msr = new MySmallRectangle(r);
							inkCollector.Sketch.mySmallRectangles.Add(msr);
							msr.MyButton = strokeCountMyButtonList[i];
							msr.ParentMyButton = this;
							if (this.Button.Visibility == Visibility.Visible)
							{
								inkCanvas.Children.Add(r);
							}
						}
					}
				}
			}
			else
			{
				foreach (MyButton mb in inkCollector.Sketch.MyButtons)
				{
					if (mb.isDeleted == false && mb.isGlobal && mb.VideoPath == VideoPath)
					{
						if (mb.rBorder != null)
						{
							inkCanvas.Children.Remove(mb.rBorder);
						}
						Rectangle rBorderRectangle = new Rectangle();
						rBorderRectangle.HorizontalAlignment = HorizontalAlignment.Left;
						rBorderRectangle.VerticalAlignment = VerticalAlignment.Top;
						rBorderRectangle.Margin = new Thickness(mb.Left, mb.Top + mb.Height, 0, 0);
						rBorderRectangle.Height = 10;
						rBorderRectangle.Width = mb.Width;
						rBorderRectangle.Stroke = new SolidColorBrush(Colors.Black);
						if (mb.Button.Visibility == Visibility.Visible)
						{
							inkCanvas.Children.Add(rBorderRectangle);
						}
						mb.rBorder = rBorderRectangle;
						List<MySmallRectangle> msrs = new List<MySmallRectangle>();
						foreach (MySmallRectangle msr in inkCollector.Sketch.mySmallRectangles)
						{
							if (msr.MyButton.VideoPath == this.VideoPath)
							{
								msrs.Add(msr);
								inkCanvas.Children.Remove(msr.Rectangle);
							}
						}
						foreach (MySmallRectangle msr in msrs)
						{
							inkCollector.Sketch.mySmallRectangles.Remove(msr);
						}
						List<MyButton> strokeCountMyButtonList = new List<MyButton>();
						foreach (MyButton m in inkCollector.Sketch.MyButtons)
						{
							if (m.IsDeleted == false && m.IsGlobal == false && m.VideoPath == _videoPath&&m.Button.Visibility==Visibility.Visible)
							{
								strokeCountMyButtonList.Add(m);
							}
						}
						strokeCountMyButtonList.Add(this);
						int smallRectangleCount = strokeCountMyButtonList.Count;
						if (smallRectangleCount > 0)
						{
							strokeCountMyButtonList.Sort(new ComparerStrokeCount());
							double totalTime = mb.TimeEnd;
							int mostStrokeCount = strokeCountMyButtonList[smallRectangleCount - 1].InkFrame._inkCanvas.Children.Count + strokeCountMyButtonList[smallRectangleCount - 1].InkFrame._inkCanvas.Strokes.Count; //最大的stroke count
							
							if (mostStrokeCount != 0)
							{
								for (int i = smallRectangleCount - 1; i >= 0; i--)
								{
									Rectangle r = new Rectangle();
									r.Stroke = null;
									r.HorizontalAlignment = HorizontalAlignment.Left;
									r.VerticalAlignment = VerticalAlignment.Top;
									int strokeCount = strokeCountMyButtonList[i].InkFrame._inkCanvas.Children.Count + strokeCountMyButtonList[i].InkFrame._inkCanvas.Strokes.Count;
									r.Margin = new Thickness(rBorderRectangle.Margin.Left +1+ rBorderRectangle.Width * (strokeCountMyButtonList[i].TimeStart / totalTime), rBorderRectangle.Margin.Top+1, 0, 0);
									r.Fill = new SolidColorBrush(Color.FromRgb(255, Convert.ToByte(255 - 255 * (double)strokeCount / (double)mostStrokeCount), Convert.ToByte(255 - 255 * (double)strokeCount / (double)mostStrokeCount)));
									r.Height = rBorderRectangle.Height - 2;
									r.Width = rBorderRectangle.Width * (strokeCountMyButtonList[i].TimeEnd - strokeCountMyButtonList[i].TimeStart) / totalTime;
									r.MouseLeftButtonUp += new MouseButtonEventHandler(r_MouseLeftButtonUp);
									MySmallRectangle msr = new MySmallRectangle(r);
									inkCollector.Sketch.mySmallRectangles.Add(msr);
									msr.MyButton = strokeCountMyButtonList[i];
									msr.ParentMyButton = mb;
									if (mb.Button.Visibility == Visibility.Visible)
									{
										inkCanvas.Children.Add(r);
									}
								}
							}
						}
						break;
					}
				}
			}

		}

		void r_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_videoAnnotation.smallRectangle_MouseLeftButtonUp(sender, e);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		
    }
}
