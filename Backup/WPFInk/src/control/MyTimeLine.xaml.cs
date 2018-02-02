using System;
using System.Collections.Generic;
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
using WPFInk.videoSummarization;
using WPFInk.Global;
using System.IO;
using System.Windows.Ink;
using WPFInk.tool;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for MyTimeLine.xaml
	/// </summary>
	public partial class MyTimeLine : UserControl
    {
        private List<int> bunchCenterNo = new List<int>();//聚类中心帧
        private List<List<int>> BunchNoList = new List<List<int>>();//聚类编号列表
        private List<KeyFrame> keyFrames = new List<KeyFrame>();//视频摘要关键帧列表
        private List<Color> colors = new List<Color>();
        private double perKeyframeLineLength = 5;// 1000 / keyFrames.Count;
        private double distanceLayer = 10;
        private List<Line> lines = new List<Line>();
        private int currIndex=1;
        private int preIndex=0;
        private MediaElement videoPlayer = null;
        private int layerCount = 5;//层次数量
        private double lineUnit = 20000;//子线长度对应时间长度单位
        private double keyframeLineUnit;//子线长度单位，this.Width/totalTime
        private double totalTime;//视频时间长度,以毫秒为单位
        private int timeLineType = 3;//时间轴类型,1代表版本1.0,2代表版本2.0,3代表版本3.0

        public double KeyframeLineUnit
        {
            get { return keyframeLineUnit; }
            set { keyframeLineUnit = value; }
        }
        public MyTimeLine()
		{
			this.InitializeComponent();
            this.Width = 1200;
            LoadKeyFrames(GlobalValues.videoName);
            LoadTxt();
            InitColors();
		}

        public void exec()
        {
            totalTime = videoPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            switch (timeLineType)
            {
                case 1:
                    DrawTimeline1();//版本1.0，无底线，所有子线都等长
                    break;
                case 2:
                    DrawTimeline2();//版本2.0，有底线，所有子线都等长
                    break;
                case 3:
                    DrawTimeline3();//版本3.0，有底线，子线长度和关键帧占用时间长度成比例
                    break;
            }
        }
        private void InitColors()
        {
            //colors.Add(Colors.Yellow);
            colors.Add(Colors.Blue);
            colors.Add(Colors.Red);
            colors.Add(Colors.Green);
            colors.Add(Colors.Purple);
            colors.Add(Colors.Plum);
        }
        public void setVideoPlayer(MediaElement VideoPlayer)
        {
            this.videoPlayer = VideoPlayer;
            locateMediaPlayer(videoPlayer, keyFrames[0]);
        }
        /// <summary>
        /// 加载关键帧
        /// </summary>
        /// <param name="videoName"></param>
        private void LoadKeyFrames(string videoName)
        {
            List<int> videoTimeList = new List<int>();
            string path = @"resource\" + videoName + @"\time.txt";
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] line = s.Split(' ');
                        videoTimeList.Add(Int32.Parse(line[1]));
                    }
                }
            }
            for (int j = 1; j <= videoTimeList.Count; j++)
            {
                KeyFrame keyFrame = new KeyFrame(GlobalValues.FilesPath + "/WPFInkResource/" + videoName + ".avi", @"resource\" + videoName + @"\" + j + ".png", videoTimeList[j - 1]);
                keyFrames.Add(keyFrame);
            }
        }
        /// <summary>
        /// 加载聚类结果
        /// </summary>
        private void LoadTxt()
        {
            try
            {
                List<KeyFrame> newKeyFrames = new List<KeyFrame>();
                string path = GlobalValues.FilesPath + @"\WPFInkResource\聚类结果\" + GlobalValues.videoName + @"\clstRst2.txt";
                int bigBunchCount = 0;//已经归为大类的帧的编号
                if (File.Exists(path))
                {
                    using (StreamReader stream = new StreamReader(path, System.Text.Encoding.Default))
                    {
                        int lineNo = 0;//行号，从0开始计算
                        //int bunchCount = 0;//聚类数量，比如100，代表10X10，共分100个类
                        // int maxBunchKeyFrmesCount = 0;//聚类中关键帧数量最多的聚类中含有的聚类数量
                        string s = "";
                        while ((s = stream.ReadLine()) != null)
                        {
                            string[] line = s.Split(' ');
                            if (lineNo == 0)//处理第0行的情况
                            {
                                //bunchCount = int.Parse(line[0]);//读取聚类数量
                                //maxBunchKeyFrmesCount = int.Parse(line[2]);//读取聚类中关键帧数量最多的聚类中含有的聚类数量
                                for (int i = 3; i < line.Length; i++)
                                {
                                    bunchCenterNo.Add(int.Parse(line[i]));
                                    //List<int> BunchNo = new List<int>();
                                    //BunchNoList.Add(BunchNo);
                                }
                                lineNo++;
                            }
                            else//处理其他行的情况
                            {
                                for (int i = 0; i < line.Length - 1; i++)
                                {
                                    int ii = int.Parse(line[i]) - 1;
                                    if (ii > -1)
                                    {
                                        keyFrames[ii].BunchNo = i + 1;
                                        int bigBunchNo = bunchCenterNo.IndexOf(i + 1);
                                        if (bigBunchNo != -1)
                                        {
                                            keyFrames[ii].BunchLayer = 1;
                                            keyFrames[ii].BigBunchNo = bigBunchNo + 1;
                                            bigBunchCount++;
                                        }
                                    }
                                }
                            }
                        }
                        //把距离为1的聚类添加到各个聚类中
                        if (bigBunchCount < keyFrames.Count)
                        {
                            try
                            {
                                for (int i = 0; i < bunchCenterNo.Count; i++)
                                {
                                    List<int> distance1No = getDistanceBunchNo(bunchCenterNo[i]);
                                    for (int j = 0; j < distance1No.Count; j++)
                                    {
                                        for (int k = 0; k < keyFrames.Count; k++)
                                        {
                                            if (keyFrames[k].BunchNo == distance1No[j] && keyFrames[k].BunchLayer == 0)
                                            {
                                                keyFrames[k].BunchLayer = 2;
                                                keyFrames[k].BigBunchNo = i + 1;
                                                bigBunchCount++;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                            //把距离为2的聚类添加到各个聚类中
                            if (bigBunchCount < keyFrames.Count)
                            {
                                try
                                {
                                    for (int i = 0; i < keyFrames.Count; i++)
                                    {
                                        if (keyFrames[i].BunchLayer == 2)
                                        {
                                            List<int> distance2No = getDistanceBunchNo(i);
                                            for (int j = 0; j < distance2No.Count; j++)
                                            {
                                                for (int k = 0; k < keyFrames.Count; k++)
                                                {
                                                    if (keyFrames[k].BunchNo == distance2No[j] && keyFrames[k].BunchLayer == 0)
                                                    {
                                                        keyFrames[k].BunchLayer = 3;
                                                        keyFrames[k].BigBunchNo = keyFrames[i].BigBunchNo;
                                                        //BunchNoList[i].Add(k);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message);
                                }
                                
                                //把距离为3的聚类添加到各个聚类中
                                if (bigBunchCount < keyFrames.Count)
                                {
                                    try
                                    {
                                        for (int i = 0; i < keyFrames.Count; i++)
                                        {
                                            if (keyFrames[i].BunchLayer == 3)
                                            {
                                                List<int> distance2No = getDistanceBunchNo(i);
                                                for (int j = 0; j < distance2No.Count; j++)
                                                {
                                                    for (int k = 0; k < keyFrames.Count; k++)
                                                    {
                                                        if (keyFrames[k].BunchNo == distance2No[j] && keyFrames[k].BunchLayer == 0)
                                                        {
                                                            keyFrames[k].BunchLayer = 4;
                                                            keyFrames[k].BigBunchNo = keyFrames[i].BigBunchNo;
                                                            //BunchNoList[i].Add(k);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        MessageBox.Show(e.Message);
                                    }
                                    //把其他的聚类都放到第五层
                                    if (bigBunchCount < keyFrames.Count)
                                    {
                                        for (int i = 0; i < keyFrames.Count; i++)
                                        {
                                            if (keyFrames[i].BunchLayer == 0)
                                            {
                                                keyFrames[i].BunchLayer = 5;
                                                keyFrames[i].BigBunchNo = bunchCenterNo.Count+1;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        
        }
        /// <summary>
        /// 绘制时间轴,版本1.0，无底线，所有子线都等长
        /// </summary>
        private void DrawTimeline1()
        {
            //StylusPointCollection points = new StylusPointCollection();
            if (perKeyframeLineLength > 0)
            {
                for (int i = 0; i < keyFrames.Count; i++)
                {
                    Line line = new Line();
                    line.X1 = i * perKeyframeLineLength;
                    line.X2 = (i + 1) * perKeyframeLineLength;
                    line.Y1 = line.Y2 = keyFrames[i].BunchLayer * distanceLayer;
                    //line.Y1 = line.Y2 = 5;
                    line.Stroke = new SolidColorBrush(keyFrames[i].BigBunchNo > 0?colors[keyFrames[i].BigBunchNo - 1]:Colors.Yellow);
                    line.StrokeThickness = 4;
                    LayoutRoot.Children.Add(line);
                    lines.Add(line);
                    //绘制垂直连线
                    if (i < keyFrames.Count - 1 && keyFrames[i].BunchLayer != keyFrames[i + 1].BunchLayer)
                    {
                        Line lineVertical = new Line();
                        lineVertical.X1 = lineVertical.X2 = (i + 1) * perKeyframeLineLength;
                        lineVertical.Y1 = keyFrames[i].BunchLayer * distanceLayer;
                        lineVertical.Y2 = keyFrames[i + 1].BunchLayer * distanceLayer;
                        lineVertical.Stroke = new SolidColorBrush(Colors.Gray);
                        lineVertical.StrokeThickness = 2;
                        LayoutRoot.Children.Add(lineVertical);
                    }
                    
                }
            }
            //Stroke timeline = new Stroke(points);
        }
        /// <summary>
        /// 绘制时间轴,版本2.0，有底线，所有子线都等长
        /// </summary>
        private void DrawTimeline2()
        {
            DrawBaseLine();
            //StylusPointCollection points = new StylusPointCollection();
            if (perKeyframeLineLength > 0)
            {
                for (int i = 0; i < keyFrames.Count; i++)
                {
                    Line line = new Line();
                    line.X1 = i * perKeyframeLineLength;
                    line.X2 = (i + 1) * perKeyframeLineLength;
                    line.Y1 = line.Y2 = keyFrames[i].BunchLayer * distanceLayer;
                    //line.Y1 = line.Y2 = 5;
                    line.Stroke = new SolidColorBrush(keyFrames[i].BigBunchNo > 0 ? colors[keyFrames[i].BigBunchNo - 1] : Colors.Yellow);
                    line.StrokeThickness = 4;
                    LayoutRoot.Children.Add(line);
                    lines.Add(line);
                    //绘制垂直连线
                    if (i < keyFrames.Count - 1 && keyFrames[i].BunchLayer != keyFrames[i + 1].BunchLayer)
                    {
                        Line lineVertical = new Line();
                        lineVertical.X1 = lineVertical.X2 = (i + 1) * perKeyframeLineLength;
                        lineVertical.Y1 = keyFrames[i].BunchLayer * distanceLayer;
                        lineVertical.Y2 = keyFrames[i + 1].BunchLayer * distanceLayer;
                        lineVertical.Stroke = new SolidColorBrush(Colors.Gray);
                        lineVertical.StrokeThickness = 2;
                        LayoutRoot.Children.Add(lineVertical);
                    }

                }
            }
            //Stroke timeline = new Stroke(points);
        }
        /// <summary>
        /// 绘制时间轴,版本2.0，有底线，子线长度和关键帧占用时间长度成比例
        /// </summary>
        private void DrawTimeline3()
        {
            DrawBaseLine();   
            if (perKeyframeLineLength > 0)
            {
                drawLineFirst();
                for (int i = 0; i < keyFrames.Count - 1; i++)//keyframes.count - 1
                {
                    DrawLine(i);
                }
                drawLineLast();
            }
        }
        /// <summary>
        /// 绘制直线
        /// </summary>
        /// <param name="index">关键帧索引</param>
        private void DrawLine(int index)
        {
            //如果和前面同层次且同颜色，就无法区分，因此添加间隔点
            if (index>0&&keyFrames[index].BigBunchNo == keyFrames[index-1].BigBunchNo&&keyFrames[index].BunchLayer == keyFrames[index - 1].BunchLayer)
            {
                Line intervalLine = new Line();
                intervalLine.X1 = intervalLine.X2 = lines[index].X2;
                intervalLine.Y1 = lines[index].Y2 - 2;
                intervalLine.Y2 = lines[index].Y2+2;
                intervalLine.Stroke = new SolidColorBrush(Colors.White);
                intervalLine.StrokeThickness = 4;
                LayoutRoot.Children.Add(intervalLine);
            }

            //添加水平时间抽子段
            Line line = new Line();
            line.X1 = lines[index].X2;
            line.X2 = line.X1 + (keyFrames[index + 1].Time - keyFrames[index].Time) * KeyframeLineUnit;
            line.Y1 = line.Y2 = keyFrames[index].BunchLayer * distanceLayer;
            line.Stroke = new SolidColorBrush(keyFrames[index].BigBunchNo > 0 ? colors[keyFrames[index].BigBunchNo - 1] : Colors.Yellow);
            line.StrokeThickness = 4;
            LayoutRoot.Children.Add(line);
            lines.Add(line);

            //绘制垂直连线
            if (keyFrames[index].BunchLayer != keyFrames[index + 1].BunchLayer)
            {
                Line lineVertical = new Line();
                lineVertical.X1 = lineVertical.X2 = line.X2;
                lineVertical.Y1 = line.Y2;
                lineVertical.Y2 = keyFrames[index + 1].BunchLayer * distanceLayer;
                lineVertical.Stroke = new SolidColorBrush(Colors.Gray);
                lineVertical.StrokeThickness = 2;
                LayoutRoot.Children.Add(lineVertical);
            }
        }
        /// <summary>
        /// //第0帧，第一帧前面的一帧，用来定位视频开头
        /// </summary>
        private void drawLineFirst()
        {
            Line line = new Line();
            line.X1 = 0;
            line.X2 = keyFrames[1].Time * KeyframeLineUnit;
            line.Y1 = line.Y2 = distanceLayer;
            //line.Y1 = line.Y2 = 5;
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 4;
            LayoutRoot.Children.Add(line);
            lines.Add(line);

            Line lineVertical = new Line();
            lineVertical.X1 = lineVertical.X2 = line.X2;
            lineVertical.Y1 = line.Y2;
            lineVertical.Y2 = keyFrames[0].BunchLayer * distanceLayer;
            lineVertical.Stroke = new SolidColorBrush(Colors.Gray);
            lineVertical.StrokeThickness = 2;
            LayoutRoot.Children.Add(lineVertical);
        }
        /// <summary>
        /// //绘制最后一帧，没有垂直折线
        /// </summary>
        private void drawLineLast()
        {
            Line line = new Line();
            line.X1 = lines[keyFrames.Count-1].X2;
            line.X2 = line.X1 + (totalTime - keyFrames[keyFrames.Count - 1].Time) * KeyframeLineUnit;
            line.Y1 = line.Y2 = keyFrames[keyFrames.Count-1].BunchLayer * distanceLayer;
            //line.Y1 = line.Y2 = 5;
            line.Stroke = new SolidColorBrush(keyFrames[keyFrames.Count - 1].BigBunchNo > 0 ? colors[keyFrames[keyFrames.Count - 1].BigBunchNo - 1] : Colors.Yellow);
            line.StrokeThickness = 4;
            LayoutRoot.Children.Add(line);
            lines.Add(line);
        }
        /// <summary>
        /// 绘制底线
        /// </summary>
        private void DrawBaseLine()
        {
            for (int i = 0; i < layerCount; i++)
            {
                Line baseLine = new Line();
                baseLine.X1 = 0;
                baseLine.X2 = this.Width;
                baseLine.Y1=baseLine.Y2 = distanceLayer * (i + 1);
                baseLine.Stroke = new SolidColorBrush(Colors.Gray);
                baseLine.StrokeThickness = 1;
                DoubleCollection dc = new DoubleCollection();
                dc.Add(4);
                dc.Add(5);
                baseLine.StrokeDashArray = dc;
                LayoutRoot.Children.Add(baseLine);
            }
        }
        /// <summary>
        /// 获取距离为1的聚类编号,关键帧编号
        /// </summary>
        /// <param name="currIndex"></param>
        /// <returns></returns>
        public List<int> getDistanceBunchNo(int currIndex)
        {
            List<int> distance1BunchNo = new List<int>();//距离为1的聚类编号
            //计算距离为1的聚类编号
            int row = keyFrames[currIndex].BunchNo / 10;//行号
            int col = keyFrames[currIndex].BunchNo % 10;//列号
            if (row % 2 == 0)//奇数行
            {
                if (row - 1 >= 0)
                {
                    if (col - 1 >= 0)
                    {
                        distance1BunchNo.Add((row - 1) * 10 + col - 1);
                    }
                    distance1BunchNo.Add((row - 1) * 10 + col);
                }
                if (col - 1 >= 0)
                {
                    distance1BunchNo.Add(row * 10 + col - 1);
                }
                if (col + 1 < 10)
                {
                    distance1BunchNo.Add(row * 10 + col + 1);
                }
                if (row + 1 < 10)
                {
                    if (col - 1 > 0)
                    {
                        distance1BunchNo.Add((row + 1) * 10 + col - 1);
                    }
                    distance1BunchNo.Add((row + 1) * 10 + col);
                }
            }
            else
            {
                if (row - 1 >= 0)
                {
                    if (col + 1 < 10)
                    {
                        distance1BunchNo.Add((row - 1) * 10 + col + 1);
                    }

                    distance1BunchNo.Add((row - 1) * 10 + col);
                }
                if (col - 1 >= 0)
                {
                    distance1BunchNo.Add(row * 10 + col - 1);
                }
                if (col + 1 < 10)
                {
                    distance1BunchNo.Add(row * 10 + col + 1);
                }
                if (row + 1 < 10)
                {
                    if (col + 1 < 10)
                    {
                        distance1BunchNo.Add((row + 1) * 10 + col + 1);
                    }
                    distance1BunchNo.Add((row + 1) * 10 + col);
                }
            }
            return distance1BunchNo;
        }

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            switch (timeLineType)
            {
                case 1:
                case 2:
                    Point mouseCurrPoint = e.GetPosition(LayoutRoot);
                    currIndex = (int)(mouseCurrPoint.X / perKeyframeLineLength);
                    if (currIndex < keyFrames.Count && currIndex != preIndex)
                    {
                        lines[preIndex].StrokeThickness = 4;
                        lines[currIndex].StrokeThickness = 10;
                        preIndex = currIndex;
                    }
                    break;
                case 3:
                    //aaaa
                    break;
            }
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (currIndex < keyFrames.Count)
            {
                locateMediaPlayer(videoPlayer, keyFrames[currIndex]);
            }
        }

        private void LayoutRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            lines[preIndex].StrokeThickness = 4;
        }

        /// <summary>
        /// 定位视频
        /// </summary>
        /// <param name="videoTime"></param>
        public void locateMediaPlayer(MediaElement videoPlayer, KeyFrame keyFrame)
        {
            videoPlayer.Source = new Uri(keyFrame.VideoName);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, keyFrame.Time);
            videoPlayer.Position = ts;
            videoPlayer.Play();
        }
	}
}