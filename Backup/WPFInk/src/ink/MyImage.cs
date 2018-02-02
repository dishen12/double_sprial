using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.tool;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;

namespace WPFInk.ink
{
    public class MyImage
    {
        public static MyImage myImage = null;
		private Rectangle bound;
		private Image _image;
		private int _startTime;
		private string _id;
		private List<ImageConnector> connnectorCollection = new List<ImageConnector>();
		private double left = 0;
		private double top = 0;
		private string pathName;
		private double angle = 0;
		private bool isExist = false;
		private string videoPath = "";
		private string safeFileName = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="image"></param>
        public MyImage(String url)
        {
            _image = InkConstants.getImageFromName(url);
            _id = url;
            _startTime = MyTimer.getInstance().getTime();
            pathName = url;
            _image.HorizontalAlignment = HorizontalAlignment.Left;
			_image.VerticalAlignment = VerticalAlignment.Top;
			_image.MinHeight = 50;
			_image.MinWidth = 50;
        }

        public MyImage(Image image)
        {
            _image = image;
            _startTime = MyTimer.getInstance().getTime();
            _image.HorizontalAlignment = HorizontalAlignment.Left;
			_image.VerticalAlignment = VerticalAlignment.Top;
			_image.MinHeight = 50;
			_image.MinWidth = 50;
        }

        

        //Image属性
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        //StartTime属性
        public int StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public double Width
        {
            get { return _image.Width; }
            set { _image.Width = value; }
        }

        public double Height
        {
            get { return _image.Height; }
            set { _image.Height = value; }
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

        public string PathName
        {
            get { return pathName; }
            set { pathName = value; }
        }

        public Rectangle Bound
        {
            get { return bound; }
            set { bound = value; }
        }

        public void addConnector(ImageConnector connector)
        {
            this.connnectorCollection.Add(connector);
        }


        public List<ImageConnector> ConnectorCollection
        {
            get
            {
                return connnectorCollection;
            }
        }

        public void setLocation(int x, int y)
        {
            Thickness margin = _image.Margin;
            margin.Left = x;
            margin.Right = 0;
            margin.Top = y;
            margin.Bottom = 0;
            _image.Margin = margin;
        }

        public void adjustBound()
        {
            InkConstants.AddBound(this);
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

        
		public string SafeFileName
		{
			get { return safeFileName; }
			set { safeFileName = value; }
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

		}
    }
}
