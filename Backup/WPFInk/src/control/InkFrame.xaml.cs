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
using WPFInk.ink;
using WPFInk.cmd;
using System.IO;
using WPFInk.tool;
using System.Windows.Ink;
using WPFInk.Global;

namespace WPFInk
{
    /// <summary>
    /// Interaction logic for InkFrame.xaml
    /// </summary>
    public partial class InkFrame : UserControl
    {
        private int zoomMaxSign = 0;
		private string textFontSize;
		private string textFontFamily;
		private SolidColorBrush textFontColor;
		private string textFontWeight = "Normal";
		private string textFontStyle = "Normal";
        private VideoSummarizationControl videoSummarizationControl = null;
        
        #region 封装变量
        public VideoSummarizationControl VideoSummarizationControl
        {
            get { return videoSummarizationControl; }
            set { videoSummarizationControl = value; }
        }

		public string TextFontStyle
		{
			get { return textFontStyle; }
			set { textFontStyle = value; }
		}

		public string TextFontWeight
		{
			get { return textFontWeight; }
			set { textFontWeight = value; }
		}

		public SolidColorBrush TextFontColor
		{
			get { return textFontColor; }
			set { textFontColor = value; }
		}

		public string TextFontFamily
		{
			get { return textFontFamily; }
			set { textFontFamily = value; }
		}

		public string TextFontSize
		{
			get { return textFontSize; }
			set { textFontSize = value; }
		}

        public int ZoomMaxSign
        {
            get
            {
                return zoomMaxSign;
            }
            set
            {
                zoomMaxSign = value;
            }
        }
        #endregion
        //定义一个收集笔迹的数据结构
        private InkCollector _inkCollector = null;

        public InkFrame()
        {
            InitializeComponent();
            InitApp();
        }

        //初始化InkFrame
        private void InitApp()
        {
            try
            {
                _inkCollector = new InkCollector(_inkCanvas, this);
                //_keyFrameAnnotation.setInkCollector(_inkCollector);
                this.OperatePieMenu.OnMove += new PieMenu.ExecuteOperate(OperatePieMenu_OnMove);
                this.OperatePieMenu.OnRotate += new PieMenu.ExecuteOperate(OperatePieMenu_OnRotate);
                this.OperatePieMenu.OnZoom += new PieMenu.ExecuteOperate(OperatePieMenu_OnZoom);
                this.OperatePieMenu.OnClose += new PieMenu.ExecuteOperate(OperatePieMenu_OnClose);

                this._imageSelector.OnClick1 += new ImageSelector.ExecuteClick(ImageSelector_OnClick1);
                this._imageSelector.OnClick2 += new ImageSelector.ExecuteClick(ImageSelector_OnClick2);

                this._peopleImageSelector.OnClick += new PeopleImageSelector.ExecuteClick(PeopleImageSelector_OnClick);


                LoadVideoPathList();
            }
            catch
            {
                MessageBox.Show("InkFrame初始化失败！");
            }
           
        }


        #region 视频摘要
        public void setSummarization(VideoSummarizationControl videoSummarizationControl)
        {
            this.videoSummarizationControl = videoSummarizationControl;
        }
        #endregion

        //以下四个函数是笔迹操作面板上的事件处理函数
        void OperatePieMenu_OnZoom()
        {
            _inkCollector.Mode = InkMode.Zoom;
        }

        void OperatePieMenu_OnRotate()
        {
            _inkCollector.Mode = InkMode.Rotate;
        }

        void OperatePieMenu_OnMove()
        {
            _inkCollector.Mode = InkMode.Move;
        }

        void OperatePieMenu_OnClose()
        {
            _inkCollector.Mode = InkMode.Select;
            //_inkCollector.SelectedImages.Clear();
            //_inkCollector.SelectedStrokes.Clear(); 
			if (_inkCollector.SelectedImages.Count > 0)
			{
				foreach (MyImage myImage in _inkCollector.SelectedImages)
				{
					myImage.Bound.Visibility = Visibility.Collapsed;
				}
			}
			_inkCollector.SelectedImages.Clear();
            if (_inkCollector.SelectButtons.Count > 0)
            {
                foreach (MyButton myButton in _inkCollector.SelectButtons)
                {
                    myButton.TextBoxTime.Background = null;
                }
            }
        }


        //check手势 第一幅图
        void ImageSelector_OnClick1()
        {
            //Console.WriteLine("_inkCollector.SelectedStrokes.count:" + _inkCollector.SelectedStrokes.Count);
			if (_inkCollector.SelectedImages.Count > 0)
			{
				foreach (MyImage myImage in _inkCollector.SelectedImages)
				{
					myImage.Bound.Visibility = Visibility.Collapsed;
				}
			}
			_inkCollector.SelectedImages.Clear();
            FileStream file = new FileStream(GlobalValues.FilesPath+"/WPFInkResource/example/Sketch1.xml",FileMode.Open);                
            WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(this.InkCollector, file);
            file.Close();
            _inkCollector.Mode = InkMode.Ink;
            //this.pointView.pointView.AddNode(this.pointView.pointView.nodeList, this.pointView.pointView.links);

        }

        //check手势 第二幅图
        void PeopleImageSelector_OnClick()
        {
            //Console.WriteLine("_inkCollector.SelectedStrokes.count:" + _inkCollector.SelectedStrokes.Count);
			if (_inkCollector.SelectedImages.Count > 0)
			{
				foreach (MyImage myImage in _inkCollector.SelectedImages)
				{
					myImage.Bound.Visibility = Visibility.Collapsed;
				}
			}
			_inkCollector.SelectedImages.Clear();
            FileStream file = new FileStream(GlobalValues.FilesPath+"/WPFInkResource/example/Sketch3.xml", FileMode.Open);
            WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(this.InkCollector, file);
            file.Close();
            _inkCollector.Mode = InkMode.Ink;
            //this.pointView.pointView.AddNode(this.pointView.pointView.nodeList, this.pointView.pointView.links);
        }

        //check手势 第二幅图   (people)
        void ImageSelector_OnClick2()
        {
            //Console.WriteLine("_inkCollector.SelectedStrokes.count:" + _inkCollector.SelectedStrokes.Count);
			if (_inkCollector.SelectedImages.Count > 0)
			{
				foreach (MyImage myImage in _inkCollector.SelectedImages)
				{
					myImage.Bound.Visibility = Visibility.Collapsed;
				}
			}
			_inkCollector.SelectedImages.Clear();
            FileStream file = new FileStream(GlobalValues.FilesPath+"/WPFInkResource/example/Sketch2.xml", FileMode.Open);
            WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(this.InkCollector, file);
            file.Close();
            _inkCollector.Mode = InkMode.Ink;
            //this.pointView.pointView.AddNode(this.pointView.pointView.nodeList, this.pointView.pointView.links);

        }

        //InkCollector 属性
        public InkCollector InkCollector
        {
            get
            {
                return _inkCollector;
            }
            set
            {
                _inkCollector = value;
            }
        }

     
        //导入草图事件，测试用
        private void LoadSketchButton_Click(object sender, RoutedEventArgs e)
        {
            List<System.Drawing.Bitmap> bmp = new List<System.Drawing.Bitmap>();

            //System.Drawing.Bitmap bitimage0 = new System.Drawing.Bitmap(@GlobalValues.FilesPath+"\out0.bmp", true);
            System.Drawing.Bitmap bitimage0 = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(GlobalValues.FilesPath+"//WPFink/out0.bmp");
            bmp.Add(bitimage0);

            System.Drawing.Bitmap bitimage1 = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(GlobalValues.FilesPath+"//WPFink/out1.bmp");
            bmp.Add(bitimage1);
            System.Drawing.Bitmap bitimage2 = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(GlobalValues.FilesPath+"//WPFink/out2.bmp");
            bmp.Add(bitimage2);
            System.Drawing.Bitmap bitimage3 = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(GlobalValues.FilesPath+"//WPFink/out3.bmp");
            bmp.Add(bitimage3);
            System.Drawing.Bitmap bitimage4 = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(GlobalValues.FilesPath+"//WPFink/out4.bmp");
            bmp.Add(bitimage4);
            System.Drawing.Bitmap bitimage5 = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(GlobalValues.FilesPath+"//WPFink/out5.bmp");
            bmp.Add(bitimage5);
            System.Drawing.Bitmap bitimage6 = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(GlobalValues.FilesPath+"//WPFink/out6.bmp");
            bmp.Add(bitimage6);




            List<Point> points = new List<Point>();
            Point point0 = new Point(120, 20);
            points.Add(point0);
            Point point1 = new Point(50, 200);
            points.Add(point1);
            Point point2 = new Point(120, 500);
            points.Add(point2);
            Point point3 = new Point(500, 500);
            points.Add(point3);
            Point point4 = new Point(900, 300);
            points.Add(point4);
            Point point5 = new Point(800, 150);
            points.Add(point5);
            Point point6 = new Point(500, 0);
            points.Add(point6);

            int width = 100;
            int height = 100;

            LoadSketch(bmp, points,width,height);
        }




        //导入草图方法
        public void LoadSketch(List<System.Drawing.Bitmap> BitmapList, List<Point> points,int width,int height)
        {
            List<BitmapImage> bmpList=new List<BitmapImage>();
            int i=0;
            foreach (System.Drawing.Bitmap bit in BitmapList)
            {
                bmpList.Add(WPFInk.tool.ConvertClass.getInstance().BitmapToBitmapImage(bit)); 
            }



            int j;
            string pathName = GlobalValues.FilesPath+"//WPFink/0.bmp";
            BitmapList[0].Save(pathName);
            MyImage preMyImage = new MyImage(pathName);
            MyImage MyImage0 = preMyImage;
            preMyImage.Width = width;
            preMyImage.Height = height;
            preMyImage.Left = points[0].X;
            preMyImage.Top = points[0].Y;
            preMyImage.PathName = pathName;
            preMyImage.setLocation((int)points[0].X, (int)points[0].Y);
            InkConstants.AddBound(preMyImage);
            
            AddImageCommand cmd = new AddImageCommand(InkCollector, preMyImage);
            cmd.execute();
            ImageConnector connector = null;
            for (i = 0; i < bmpList.Count-1; i++)
            {
                j = i + 1;
                pathName = GlobalValues.FilesPath+"//WPFink/" + j + ".bmp";

                BitmapList[j].Save(pathName);
                MyImage newimage = new MyImage(pathName);
                newimage.Width = width;
                newimage.Height = height;
                newimage.setLocation((int)points[j].X, (int)points[j].Y);
                newimage.Left = points[j].X;
                newimage.Top = points[j].Y;
                newimage.PathName = pathName;
                InkConstants.AddBound(newimage);
                AddImageCommand cmd2 = new AddImageCommand(InkCollector, newimage);
                cmd2.execute();
                connector = new ImageConnector(preMyImage, newimage);
                preMyImage = newimage;
                MyStroke myStroke = connector.MYSTROKE;
                myStroke.Stroke.DrawingAttributes.Width = 4;
                myStroke.Stroke.DrawingAttributes.Height = 4;
                myStroke.IsSketchConnector = true;
                Command sc = new AddStrokeCommand(InkCollector, myStroke);
                sc.execute();
            }
        }

#region //zhanqi添加的代码

        public List<string> videoPathList = new List<string>();

        private void LoadVideoPathList()
        {
            this.videoPathList.Add(GlobalValues.FilesPath+ @"\WPFInkResource\example\玲珑塔-1.avi");
            this.videoPathList.Add(GlobalValues.FilesPath + @"\WPFInkResource\example\五环.avi");
            this.videoPathList.Add(GlobalValues.FilesPath+ @"\WPFInkResource\example\鸟巢.avi");
            this.videoPathList.Add(GlobalValues.FilesPath+ @"\WPFInkResource\example\水立方-已截取.avi");
            this.videoPathList.Add(GlobalValues.FilesPath+ @"\WPFInkResource\example\山-已截取.avi");
            this.videoPathList.Add(GlobalValues.FilesPath+ @"\WPFInkResource\example\亭子.avi");
            this.videoPathList.Add(GlobalValues.FilesPath+ @"\WPFInkResource\example\NewestHorse.avi");
            this.videoPathList.Add(GlobalValues.FilesPath+ @"\WPFInkResource\example\滑雪.avi");
            this.videoPathList.Add(GlobalValues.FilesPath+ @"\WPFInkResource\example\跑步.avi");
        }

        internal void SelNodeChange(int index)
        {
            this.panelVideoShow.panelVideo.Stop();
            this.panelVideoShow.panelVideo.Play(videoPathList[index], index);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
			this.pointView.pointView.SetInkFrmae(this, _inkCollector);
            this.panelVideoShow.panelVideo.SetInkFrame(this);
        }

        private void ClearMessage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.message.Content = "";
        }
#endregion
        

    }
}
