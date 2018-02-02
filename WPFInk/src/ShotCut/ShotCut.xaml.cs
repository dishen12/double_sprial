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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using WPFInk;
using WPFInk.Global;
using WPFInk.tool;

namespace WPFInk.ShotCut
{
    /// <summary>
    /// Interaction logic for ShotCut.xaml
    /// </summary>
    public partial class ShotCut : Window
    {
        const int countPerLine = 10;//每行个数
        const int KeyFrameWidth = 80;
        const int KeyFrameHeight = 50;

        int pause_resume = 1;
        int curpos = 0;
        Bitmap curFrame = null;
        AVIReader reader = null;

        int[] curHist = null;
        int[] lastHist = null;

        int frameCount = 0;

        Bitmap selectedBmp = null;
        public int selectedBmpID = -1;

        int _width;
        int _height;

        int curPage = 0;
        InkFrame inkframe = null;
        List<System.Windows.Media.Imaging.BitmapImage> BImage = new List<System.Windows.Media.Imaging.BitmapImage>();
        List<System.Windows.Point> WPoints = new List<System.Windows.Point>();
        private int videoFramePerSecond = 24;//视频每秒帧数


        private System.Windows.Forms.ToolStripMenuItem converToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem motionCueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sketchGenerationToolStripMenuItem;

        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pictureBoxFrame;
        private System.Windows.Forms.PictureBox pictureBoxShotList;
        private System.Windows.Forms.PictureBox pictureBoxProgress;
        private System.Windows.Forms.NumericUpDown numericUpDownKeyFrameNum;
        private System.Windows.Forms.PictureBox pictureBoxWave;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;

        List<System.Drawing.Rectangle> keyFrameRecList = new List<System.Drawing.Rectangle>();
        List<Frame> keyFrameList = new List<Frame>();
        List<int> keyFrameNoList = new List<int>();

        public int selectedIndex = -1;

        bool bShowBk = false;

        // 初始图像列表
        List<Bitmap> imageList = new List<Bitmap>();
        // resize后的图像列表
        List<Bitmap> resizedImgs = new List<Bitmap>();
        // 节点列表，用于生成连接线
        List<System.Drawing.Point> points = new List<System.Drawing.Point>();//

        List<Link> links = new List<Link>();

        //no use
        private void drawCurve(Graphics g, System.Drawing.Point start, System.Drawing.Point end)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.DrawPath(new System.Drawing.Pen(System.Drawing.Color.Black, this.penWidth), getPoints(start, end));//改过
            g.Flush();
        }

        MathTool math = MathTool.getInstance();//
        //定义弧的跨度
        int DIS_SEP = 80;

        //定义弧的高度
        int H = 50;

        //定义弧的粗细
        float penWidth = 5.0F;

        double LOWidth = 1200;
        double LOHeight = 700;

        double resizeRatio = 0.27;

        enum ArrangeResult
        {
            OK = 0,
            TooLarge = 1,
            TooSmall = -1
        }

        private GraphicsPath getPoints(System.Drawing.Point start, System.Drawing.Point end)
        {
            if (start.X > end.X)
            {
                System.Drawing.Point temp = start;//
                start = end;
                end = temp;
            }

            System.Drawing.Point[] pts = getMidPoints(start, end);//
            GraphicsPath path = new GraphicsPath();//
            for (int i = 1; i < pts.Length; i++)
            {
                System.Drawing.Point p1 = pts[i - 1];
                System.Drawing.Point p2 = pts[i];
                int num = (int)(math.distanceP2P(p1, p2));

                System.Drawing.Point[] ps = new System.Drawing.Point[num];
                System.Drawing.Point b = new System.Drawing.Point();
                System.Drawing.Point temp = new System.Drawing.Point(p1.X + (p2.X - p1.X) / 3, p1.Y + (p2.Y - p1.Y) / 3);

                double angle = math.getAngle(p1, p2);
                double angle1 = 90 + angle;
                double kk = Math.Tan(angle1 / 180);
                b.X = (int)(temp.X - H / kk);
                b.Y = temp.Y + H;

                //panelMain.CreateGraphics().DrawRectangle(new Pen(Color.Black, 3), new Rectangle(b, new Size(4, 4)));

                for (int k = 0; k < num; k++)
                {
                    double t = k * 1.0 / num;
                    double a, e, c;
                    a = (1 - t) * (1 - t);
                    e = 2 * t * (1 - t);
                    c = t * t;
                    int x1 = (int)(p1.X * a + b.X * e + p2.X * c);
                    int y1 = (int)(p1.Y * a + b.Y * e + p2.Y * c);
                    ps[k] = new System.Drawing.Point(x1, y1);
                }
                path.AddLines(ps);
            }
            return path;
        }

        private System.Drawing.Point[] getMidPoints(System.Drawing.Point start, System.Drawing.Point end)
        {
            System.Drawing.Point[] results = { start, end };
            double distance = math.distanceP2P(start, end);

            int numofmidpoints = (int)(distance / DIS_SEP) - 1;

            if (numofmidpoints <= 0)
                return results;
            System.Drawing.Point[] midpoints = new System.Drawing.Point[numofmidpoints];
            int offx = end.X - start.X;
            int offy = end.Y - start.Y;
            for (int i = 0; i < numofmidpoints; i++)
            {
                midpoints[i] = new System.Drawing.Point();
                midpoints[i].X = start.X + offx * (i + 1) / (numofmidpoints + 1);
                midpoints[i].Y = start.Y + offy * (i + 1) / (numofmidpoints + 1);
            }
            results = new System.Drawing.Point[2 + numofmidpoints];
            results[0] = start;
            for (int i = 1; i < numofmidpoints + 1; i++)
                results[i] = midpoints[i - 1];
            results[1 + numofmidpoints] = end;
            return results;
        }

        private void Resize()
        {
            List<Bitmap> cleanImgs = new List<Bitmap>();
            // 剔除空白部分
            foreach (Bitmap bmp in this.imageList)
            {
                cleanImgs.Add(bmp);
            }


            //double alpha = 0.5;
            double totalSize = 0;
            foreach (Bitmap bmp in cleanImgs)
            {
                totalSize += bmp.Width * bmp.Height;
            }

            // 这个contrib本来是应该每个map node都不一样的

            double ratio = Math.Sqrt(LOWidth * LOHeight / totalSize * resizeRatio);

            this.resizedImgs.Clear();

            foreach (Bitmap bmp in cleanImgs)
            {
                int width = (int)(bmp.Width * ratio);
                int height = (int)(bmp.Height * ratio);
                _width=width;
                _height=height;
                Bitmap newBmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(newBmp);
                g.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();

                this.resizedImgs.Add(newBmp);
            }

            // 禁用添加图片
            //this.buttonAddThis.Enabled = false;
        }

        private void Arrange()
        {
            if (points.Count == 0)
            {
                // 0621
                // GarField Orin
                this.points.Add(new System.Drawing.Point(406, 142));
                this.points.Add(new System.Drawing.Point(216, 288));
                this.points.Add(new System.Drawing.Point(486, 348));
                this.points.Add(new System.Drawing.Point(458, 626));
                this.points.Add(new System.Drawing.Point(1016, 262));
                this.points.Add(new System.Drawing.Point(990, 602));
                // 0621
                // 2
                this.points.Add(new System.Drawing.Point(243, 295));
                this.points.Add(new System.Drawing.Point(361, 116));
                this.points.Add(new System.Drawing.Point(580, 219));
                this.points.Add(new System.Drawing.Point(403, 432));
                this.points.Add(new System.Drawing.Point(733, 692));
                this.points.Add(new System.Drawing.Point(1060, 440));
                this.points.Add(new System.Drawing.Point(1030, 202));



                // 3 
                this.points.Add(new System.Drawing.Point(196, 324));
                this.points.Add(new System.Drawing.Point(374, 137));
                this.points.Add(new System.Drawing.Point(619, 365));
                this.points.Add(new System.Drawing.Point(243, 591));
                this.points.Add(new System.Drawing.Point(582, 729));
                this.points.Add(new System.Drawing.Point(869, 698));
                this.points.Add(new System.Drawing.Point(986, 390));
                this.points.Add(new System.Drawing.Point(979, 186));
                // 0621 start


                this.points.Add(new System.Drawing.Point(198, 191));
                this.points.Add(new System.Drawing.Point(557, 82));
                this.points.Add(new System.Drawing.Point(985, 178));
                this.points.Add(new System.Drawing.Point(592, 317));
                this.points.Add(new System.Drawing.Point(213, 435));
                this.points.Add(new System.Drawing.Point(232, 681));
                this.points.Add(new System.Drawing.Point(711, 524));
                this.points.Add(new System.Drawing.Point(950, 422));
                this.points.Add(new System.Drawing.Point(1020, 574));
                this.points.Add(new System.Drawing.Point(767, 701));
                // 0621 end
            }
            int span = 0;
            ArrangeResult result = IsArrangeOK();
            Random rand = new Random();
            System.Drawing.Point curPt = new System.Drawing.Point();


            while (result != ArrangeResult.OK)
            {
                this.points.Clear();
                //direction 1: Go Right, -1: Go LeftDown
                int direction = 1;
                if (result == ArrangeResult.TooSmall)
                    span += 20;
                if (result == ArrangeResult.TooLarge)
                {
                    resizeRatio -= 0.05;
                    this.Resize();
                }
                if (this.resizedImgs.Count > 0)
                {
                    curPt = new System.Drawing.Point(resizedImgs[0].Width / 2 + rand.Next(0, 50), resizedImgs[0].Height / 2 + rand.Next(0, 50));
                    this.points.Add(curPt);
                }
                for (int i = 1; i < resizedImgs.Count; i++)
                {
                    // 修订方向
                    if (direction == 1)
                        if (curPt.X + resizedImgs[i].Width + span + 100 > LOWidth)
                        {
                            direction = -1;
                            curPt.Y += resizedImgs[i].Height + span;
                        }
                    if (direction == -1)
                        if (curPt.X - resizedImgs[i].Width - span < 0)
                        {
                            direction = 1;
                            curPt.Y += resizedImgs[i].Height + span;
                        }

                    // Go
                    if (direction == 1)
                    {
                        curPt.X = curPt.X + resizedImgs[i].Width + span + rand.Next(-30, 30);
                        curPt.Y = curPt.Y + rand.Next(-30, 30);
                    }
                    if (direction == -1)
                    {
                        curPt.X = curPt.X - resizedImgs[i].Width - span + rand.Next(-30, 30);
                        curPt.Y = curPt.Y + rand.Next(-30, 30);
                    }
                    this.points.Add(curPt);
                }
                result = IsArrangeOK();
            }
        }

        private ArrangeResult IsArrangeOK()
        {
            if (this.resizedImgs.Count == 0)
                return ArrangeResult.OK;
            int left = int.MaxValue;
            int right = int.MinValue;
            int top = int.MaxValue;
            int bottom = int.MinValue;
            foreach (System.Drawing.Point pt in points)
            {
                if (pt.X < left)
                    left = pt.X;
                if (pt.X > right)
                    right = pt.X;
                if (pt.Y < top)
                    top = pt.Y;
                if (pt.Y > bottom)
                    bottom = pt.Y;
            }
            double ratio = ((double)(right - left)) * (bottom - top) / LOWidth / LOHeight;
            if (ratio < 0.4)
                return ArrangeResult.TooSmall;
            if (ratio > 1.0)
                return ArrangeResult.TooLarge;
            return ArrangeResult.OK;
        }

        string secondsToMinSec(int seconds)
        {
            if (reader != null)
            {
                int min = (int)(seconds / reader.FrameRate / 60);
                int sec = (int)(seconds / reader.FrameRate % 60);
                return "" + min + ":" + sec;
            }
            return string.Empty;
        }

        public ShotCut(InkFrame inkframe)
        {
            this.inkframe = inkframe;
            this.Loaded += new RoutedEventHandler(ShotCut_Loaded);
            InitializeComponent();
        }


        void ShotCut_Loaded(object sender, RoutedEventArgs e)
        {

            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.motionCueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sketchGenerationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.converToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            timer1 = new System.Windows.Forms.Timer();
            pictureBoxFrame = new System.Windows.Forms.PictureBox();
            pictureBoxShotList = new System.Windows.Forms.PictureBox();
            pictureBoxWave = new System.Windows.Forms.PictureBox();
            pictureBoxProgress = new System.Windows.Forms.PictureBox();
            numericUpDownKeyFrameNum = new System.Windows.Forms.NumericUpDown();
            windowsFormsHostFrame.Child = pictureBoxFrame;
            windowsFormsHostShotList.Child = pictureBoxShotList;
            windowsFormsHostWave.Child = pictureBoxWave;
            windowsFormsHostProgress.Child = pictureBoxProgress;
            windowsFormsHostNumeric.Child = numericUpDownKeyFrameNum;

            this.numericUpDownKeyFrameNum.TabIndex = 1;
            this.numericUpDownKeyFrameNum.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownKeyFrameNum.Maximum = 100000;
            this.pictureBoxFrame.Width = (int)windowsFormsHostFrame.Width;
            this.pictureBoxFrame.Height = (int)windowsFormsHostFrame.Height;
            this.pictureBoxFrame.TabIndex = 0;
            this.pictureBoxFrame.TabStop = false;

            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

            this.pictureBoxShotList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxShotList_MouseDown);
            this.pictureBoxShotList.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxShotList_Paint);

            this.pictureBoxWave.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBoxWave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            this.pictureBoxProgress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxProgress_MouseDown);
            this.pictureBoxProgress.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxProgress_Paint);

            //方便删除！
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.motionCueToolStripMenuItem,
            this.sketchGenerationToolStripMenuItem,
            this.converToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(179, 92);

            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,converToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(179, 47);

            // 
            // motionCueToolStripMenuItem
            // 
            this.motionCueToolStripMenuItem.Name = "motionCueToolStripMenuItem";
            this.motionCueToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.motionCueToolStripMenuItem.Text = "MotionCue";
            this.motionCueToolStripMenuItem.Click += new System.EventHandler(this.motionCueToolStripMenuItem_Click);
            // 
            // sketchGenerationToolStripMenuItem
            // 
            this.sketchGenerationToolStripMenuItem.Name = "sketchGenerationToolStripMenuItem";
            this.sketchGenerationToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.sketchGenerationToolStripMenuItem.Text = "SketchGeneration";
            this.sketchGenerationToolStripMenuItem.Click += new System.EventHandler(this.sketchGenerationToolStripMenuItem_Click);
            // 
            // converToolStripMenuItem
            // 
            this.converToolStripMenuItem.Name = "converToolStripMenuItem";
            this.converToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.converToolStripMenuItem.Text = "ConvertAllToSketch";
            this.converToolStripMenuItem.Click += new System.EventHandler(this.converToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);



            this.pictureBoxFrame.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
            {
                this.textBox1.Text = dlg.FileName;
            }
        }

        private void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            if (string.Empty != this.textBox1.Text)
            {
                reader = new AVIReader();
                reader.Open(this.textBox1.Text);
                Console.WriteLine(this.textBox1.Text);
                this.videoFramePerSecond = ((int)reader.FrameRate == reader.FrameRate ? (int)reader.FrameRate : (int)reader.FrameRate + 1);

                this.timer1.Start();
            }
        }

        private void pictureBoxShotList_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < keyFrameList.Count; i++)
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(keyFrameRecList[i].X, keyFrameRecList[i].Y - curPage * 720, keyFrameRecList[i].Width, keyFrameRecList[i].Height);
                e.Graphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue, 5.0F), rect);
                if (null != keyFrameList[i].sketchBmp)
                    e.Graphics.DrawImage(keyFrameList[i].sketchBmp, rect, new System.Drawing.Rectangle(0, 0, keyFrameList[i].frameBmp.Width, keyFrameList[i].frameBmp.Height), GraphicsUnit.Pixel);
                else
                    e.Graphics.DrawImage(keyFrameList[i].frameBmp, rect, new System.Drawing.Rectangle(0, 0, keyFrameList[i].frameBmp.Width, keyFrameList[i].frameBmp.Height), GraphicsUnit.Pixel);
               //e.Graphics.DrawString(secondsToMinSec(keyFrameNoList[i]), new System.Drawing.Font("", 12, System.Drawing.FontStyle.Bold), System.Drawing.Brushes.Black, new System.Drawing.Point(rect.Left, rect.Bottom + 10));
            }
        }
        /// <summary>
        /// 生成关键帧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (null == reader)
                return;

            Bitmap nextFrame = null;
            try
            {
                curFrame = reader.GetNextFrame();
                nextFrame = reader.GetNextFrame();
            }
            catch { return; }
            frameCount++;
            curpos = reader.CurrentPosition;
            if (null == curFrame)
                return;



            this.pictureBoxFrame.BackgroundImage = curFrame;
            this.pictureBoxFrame.Refresh();

            if (frameCount % 2 != 0)
            {
                return;
            }

            // 取thumbnail去生成hist，但keyFrame里存的还是原来的bmp
            Bitmap resizedCurFrame = new Bitmap(curFrame.GetThumbnailImage(200, 200, null, IntPtr.Zero));
            curHist = Histogram.CalHSVHis(resizedCurFrame);
            if (null != lastHist)
            {
                double diff = Histogram.CalHisDiff(curHist, lastHist);

                // 这里暂时取像素值的四分之一作为阈值
                if (diff > resizedCurFrame.Width * resizedCurFrame.Height * 0.25)
                {
                    this.keyFrameList.Add(new Frame(curFrame, nextFrame, (int)((double)curpos-1)*1000/videoFramePerSecond));
                    AddRect(curFrame);
                    this.keyFrameNoList.Add((int)(reader.CurrentPosition));
                    this.pictureBoxShotList.Refresh();
                }

                if (curpos == 128 || curpos == 432 || curpos == 604 || curpos == 680 || curpos == 1216 || curpos == reader.Length - 1)
                {
                    this.keyFrameList.Add(new Frame(curFrame, nextFrame, (int)((double)curpos - 1) * 1000 / videoFramePerSecond));
                    AddRect(curFrame);
                    this.keyFrameNoList.Add((int)(reader.CurrentPosition));
                    this.pictureBoxShotList.Refresh();
                }
            }
            lastHist = curHist;
            this.pictureBoxWave.Refresh();
            this.pictureBoxFrame.Refresh();
            this.pictureBoxProgress.Refresh();
        }

        private void AddRect(Bitmap bmp)
        {
            int count = keyFrameRecList.Count;
            //int x = count % 5 * 150 + 20; //改动过，原来为5  11.01.19
            //int y = count / 5 * 150 + 20; //改动过，原来为5

            int width = KeyFrameWidth;
            int height = KeyFrameHeight;//bmp.Height * width / bmp.Width-20;
            int x = count % countPerLine * (width+2) + 10;//2011.1.19号改过，原来为5
            int y = count / countPerLine * (height+10) + 20;//2011.1.19号改过，原来为5
            this.keyFrameRecList.Add(new System.Drawing.Rectangle(x, y, width, height));
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // 画格子
            e.Graphics.FillRectangle(System.Drawing.Brushes.Black, new System.Drawing.Rectangle(0, 0, this.pictureBoxWave.Width, this.pictureBoxWave.Height));
            e.Graphics.DrawLine(Pens.Green, 0, this.pictureBoxWave.Height / 2, this.pictureBoxWave.Width, this.pictureBoxWave.Height / 2);
            for (int i = 0; i < 100; i++)
            {
                e.Graphics.DrawLine(Pens.Blue, i * this.pictureBoxWave.Width / 100, 0, i * this.pictureBoxWave.Width / 100, this.pictureBoxWave.Height);
            }

            for (int i = 0; i < 10; i++)
            {
                e.Graphics.DrawLine(Pens.Green, i * this.pictureBoxWave.Width / 10, 0, i * this.pictureBoxWave.Width / 10, this.pictureBoxWave.Height);
            }


            if (null != this.curHist)
            {
                int max = int.MinValue;
                int min = int.MaxValue;
                for (int i = 0; i < curHist.Length; i++)
                {
                    if (max < curHist[i])
                        max = curHist[i];
                    if (min > curHist[i])
                        min = curHist[i];
                }

                int[] hist = new int[curHist.Length];

                for (int i = 0; i < curHist.Length; i++)
                {
                    hist[i] = (curHist[i] - min) * 100 / (max - min);
                }

                for (int i = 1; i < hist.Length; i++)
                {
                    e.Graphics.DrawLine(Pens.Red, 6 * (i - 1), -hist[i - 1] + this.pictureBoxWave.Height / 2, 6 * i, -hist[i] + this.pictureBoxWave.Height / 2);
                }
            }
        }


        private int getKeyFrameSelected(System.Drawing.Point pt)
        {
            for (int i = curPage*64; i < keyFrameRecList.Count; i++)
            {
                System.Drawing.Rectangle rect = keyFrameRecList[i];
                if (pt.X <= rect.Right && pt.X >= rect.Left
                    && pt.Y >= (rect.Top-curPage*720) && pt.Y <= (rect.Bottom-curPage*720))
                {
                    this.selectedBmp = keyFrameList[i].frameBmp;
                    this.selectedBmpID = i;
                    return i;
                }
            }
            // 没有找到就返回-1，否则返回id值
            return -1;
        }

        private void pictureBoxShotList_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && getKeyFrameSelected(e.Location) != -1)
            {
                System.Drawing.Point pt = this.pictureBoxShotList.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                this.contextMenuStrip1.Show(pt.X, pt.Y);
            }
            else if (e.Button == MouseButtons.Left)
            {
                selectedBmpID = getKeyFrameSelected(new System.Drawing.Point(e.X, e.Y));
                if (selectedBmpID != -1)
                {
                    deleteSelectFrame(selectedBmpID);
                    rearrangeFrameRec();
                    this.pictureBoxShotList.Refresh();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                // temp chenjia
                for (int i = 1; i < 7; i++)
                    this.keyFrameList[i - 1].sketchBmp = new Bitmap(@"E:\e" + i + ".bmp");
                this.pictureBoxShotList.Refresh();
            }
        }

        private void motionCueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMotionCue form = new FormMotionCue();
            form.setShotCut(this);
            if (this.keyFrameList[selectedBmpID].sketchBmp != null)
                form.bkBmp = this.keyFrameList[selectedBmpID].sketchBmp;
            else
                form.bkBmp = this.keyFrameList[selectedBmpID].frameBmp;

            form.curFrameBmp = keyFrameList[selectedBmpID].frameBmp;
            form.nextFrameBmp = keyFrameList[selectedBmpID].nextFrameBmp;
            //form.MdiParent = this.MdiParent;
            //这里我作了改动
            //if (System.Windows.Forms.DialogResult.OK == form.ShowDialog())
            //{
            form.ShowDialog();
            this.keyFrameList[this.selectedBmpID].sketchBmp = form.resultBmp;
            //}
            this.pictureBoxWave.Refresh();
            this.pictureBoxShotList.Refresh();
        }


        private void ExecuteCmd(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.StandardInput.WriteLine(GlobalValues.FilesPath+"/WPFInk");
            p.StandardInput.WriteLine(command);
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();
            p.Close();
        }

        private void sketchGenerationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == selectedBmp)
                return;
            this.timer1.Stop();
            selectedBmp.Save(GlobalValues.FilesPath+"/WPFInk/temp.bmp");
            ExecuteCmd(GlobalValues.FilesPath+"/WPFInk/stroke.exe "+GlobalValues.FilesPath+"/WPFInk/temp.bmp");

            this.keyFrameList[selectedBmpID].sketchBmp = ColorHelper.ColorToGray(new Bitmap(GlobalValues.FilesPath+"/WPFInk/FinalImage.jpg"));

            this.pictureBoxShotList.Refresh();
        }

     /*   private void button2_Click(object sender, EventArgs e)
        {
            FormLayout form = new FormLayout();
            form.MdiParent = this.MdiParent;
            foreach (Frame frame in this.keyFrameList)
            {
                if (frame.sketchBmp != null)
                    form.AddBmp(frame.sketchBmp);
            }
            form.Show();
        }*/

        private void pictureBoxProgress_Paint(object sender, PaintEventArgs e)
        {
            if (reader == null)
                return;

            if (this.keyFrameNoList.Count > 0)
                e.Graphics.FillRectangle(new SolidBrush(getColor(0)), 0, 0, (keyFrameNoList[0]) * this.pictureBoxProgress.Width / reader.Length, this.pictureBoxProgress.Height);
            for (int i = 1; i < keyFrameNoList.Count; i++)
            {
                int width = (keyFrameNoList[i] - keyFrameNoList[i - 1]) * this.pictureBoxProgress.Width / reader.Length;
                width = width < 1 ? 1 : width;
                e.Graphics.FillRectangle(new SolidBrush(getColor(i % 5)), keyFrameNoList[i - 1] * this.pictureBoxProgress.Width / reader.Length, 0, width, this.pictureBoxProgress.Height);
            }

            // drawTrackLine
            e.Graphics.DrawLine(Pens.Red, curpos * this.pictureBoxProgress.Width / reader.Length, 0, curpos * this.pictureBoxProgress.Width / reader.Length, this.pictureBoxProgress.Height);
        }

        private System.Drawing.Color getColor(int i)
        {
            switch (i)
            {
                case 0:
                    return System.Drawing.Color.Red;
                    //break;
                case 1:
                    return System.Drawing.Color.Green;
                    //break;
                case 2:
                    return System.Drawing.Color.Blue;
                    //break;
                case 3:
                    return System.Drawing.Color.Gray;
                    //break;
            }
            return System.Drawing.Color.Yellow;
        }

        private void buttonFilter_Click(object sender, EventArgs e)
        {
            filterFrame((int)this.numericUpDownKeyFrameNum.Value);
            rearrangeFrameRec();
            this.pictureBoxShotList.Refresh();
        }

        private void deleteSelectFrame(int id)
        {
            keyFrameNoList.RemoveAt(id);
            keyFrameList.RemoveAt(id);
            keyFrameRecList.RemoveAt(id);
        }

        private void rearrangeFrameRec()
        {
            for (int i = 0; i < keyFrameRecList.Count; i++)
            {
                int width = KeyFrameWidth;
                int height = 70;// keyFrameList[i].frameBmp.Height * width / keyFrameList[i].frameBmp.Width - 20;
                int x = i % countPerLine * (width+2) + 10;//2011.1.19号改过，原来为5
                int y = i / countPerLine * (height+10) + 20;//2011.1.19号改过，原来为5
                this.keyFrameRecList[i] = new System.Drawing.Rectangle(x, y, width, height);
            }
        }

        private bool isCheat(int i)
        {
            if (i == 128 || i == 432 || i == 604 || i == 680 || i == 1216 || i == reader.Length - 1)
                return true;
            return false;
        }

        private void filterFrame(int frameNo)
        {
            while (keyFrameNoList.Count > frameNo)
            {
                int min = int.MaxValue;
                int minindex = -1;
                for (int i = 1; i < keyFrameNoList.Count; i++)
                {
                    if (keyFrameNoList[i] - keyFrameNoList[i - 1] < min && false == isCheat(keyFrameNoList[i - 1]))
                    {
                        min = keyFrameNoList[i] - keyFrameNoList[i - 1];
                        minindex = i - 1;
                    }
                }
                if (minindex == -1)
                    return;
                keyFrameNoList.RemoveAt(minindex);
                keyFrameList.RemoveAt(minindex);
                keyFrameRecList.RemoveAt(minindex);
            }


        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteSelectFrame(selectedBmpID);
            rearrangeFrameRec();
            this.pictureBoxShotList.Refresh();
        }

        private void pictureBoxProgress_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //e.Location.X, e.Location.Y;
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            switch (pause_resume)
            {
                case 1:
                    this.timer1.Stop();
                    this.buttonPause.Content = "Resume";
                    pause_resume = 2;
                    break;
                case 2:
                    this.timer1.Start();
                    this.buttonPause.Content = "Pause";
                    pause_resume = 1;
                    break;
            }
        }
        /// <summary>
        /// 保存关键帧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            int count = 0;
            FileStream myStream = new FileStream(GlobalValues.FilesPath + @"\WPFInk\cache\keyFrames\time.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sWriter=new StreamWriter(myStream);
            foreach (Frame frame in this.keyFrameList)
            {
                frame.frameBmp.Save(GlobalValues.FilesPath+ @"\WPFInk\cache\keyFrames\" + (++count) + ".png");
                sWriter.WriteLine(count.ToString() + " " + (frame.time).ToString());
            }
            sWriter.Close();
            myStream.Close();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(GlobalValues.FilesPath+"/WPFInk//out.bmp");
            AutoDetectArea(bmp);
        }

        private void AutoDetectArea(Bitmap bmp)
        {
            Bitmap newBMP = new Bitmap(bmp.Width, bmp.Height);

            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    System.Drawing.Color clr = bmp.GetPixel(j, i);
                    float s = ColorHelper.RGB2HSV(clr.R, clr.G, clr.B).s;
                    if (min > s)
                        min = s;
                    if (max < s)
                        max = s;
                }

            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    System.Drawing.Color clr = bmp.GetPixel(j, i);
                    float s = ColorHelper.RGB2HSV(clr.R, clr.G, clr.B).s;
                    int gray = (int)((s - min) * 255 / (max - min));
                    newBMP.SetPixel(j, i, System.Drawing.Color.FromArgb(gray, gray, gray));
                }
            newBMP.Save(GlobalValues.FilesPath + "//keyframes/gray.bmp");


            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (newBMP.GetPixel(j, i).R > 150)
                        newBMP.SetPixel(j, i, System.Drawing.Color.Red);
                    else
                        newBMP.SetPixel(j, i, System.Drawing.Color.Green);
                }
            newBMP.Save(GlobalValues.FilesPath + "//keyframes/Area.bmp");
        }

        private void converToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //for (int i = 1; i <= 7; i++)
            //{
            // this.keyFrameList[i - 1].frameBmp = new Bitmap(@"C:\Documents and Settings\Administrator\桌面\TODO_0127\2\" + i + ".bmp");
            //}
            //this.pictureBoxShotList.Refresh();
            for (int i = 0; i < keyFrameList.Count; i++)
            {
                //ROI(i);
                CLD(i);
            }
        }
        private void CLD(int i)
        {
            string CldInput = GlobalValues.FilesPath + @"\WPFInk\cache\CldInput" + i + ".bmp";
            //string soursePath = GlobalValues.FilesPath + "/WPFInk/cache/temp" + i + ".bmp";
            this.keyFrameList[i].frameBmp.Save(CldInput);
            string CldOutput = GlobalValues.FilesPath + @"\WPFInk\cache\CldOutput" + i + ".bmp";
            CmdUtility.ExeCommand(GlobalValues.FilesPath + "/WPFInk/CLD.exe " + CldInput + " " + CldOutput);
            this.keyFrameList[i].frameBmp = new Bitmap(CldOutput);
            this.pictureBoxShotList.Refresh();
        }
        private void ROI(int i)
        {
            string soursePath = GlobalValues.FilesPath + "/WPFInk/cache/temp" + i + ".bmp";
            this.keyFrameList[i].frameBmp.Save(soursePath);
            string RoiOutPutDir = GlobalValues.FilesPath + "/WPFInk/cache/";
            CmdUtility.ExeCommand(@"attcut\AttCut.exe " + soursePath + " " + RoiOutPutDir + " 70");
            string RoiOutput = GlobalValues.FilesPath + "/WPFInk/cache/temp" + i + "_RCC.png";
            using (BinaryReader binaryReader = new BinaryReader(File.Open(RoiOutput, FileMode.Open)))
            {
                FileInfo fi = new FileInfo(RoiOutput);
                byte[] bytes = binaryReader.ReadBytes((int)fi.Length);
                binaryReader.Close();
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(bytes);
                bitmapImage.EndInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                getWhiteAreaBound(ImageTool.getInstance().BitmapImageToBitMap(bitmapImage), soursePath, i);
            }
            string CldInput = GlobalValues.FilesPath + @"\WPFInk\cache\CldInput" + i + ".bmp";
            this.keyFrameList[i].frameBmp = new Bitmap(CldInput);
            this.pictureBoxShotList.Refresh();
        }
        private void ROICLD(int i)
        {
            string soursePath = GlobalValues.FilesPath + "/WPFInk/cache/temp" + i + ".bmp";
            this.keyFrameList[i].frameBmp.Save(soursePath);
            string RoiOutPutDir = GlobalValues.FilesPath + "/WPFInk/cache/";
            CmdUtility.ExeCommand(@"attcut\AttCut.exe " + soursePath + " " + RoiOutPutDir + " 70");
            string RoiOutput = GlobalValues.FilesPath + "/WPFInk/cache/temp" + i + "_RCC.png";
            using (BinaryReader binaryReader = new BinaryReader(File.Open(RoiOutput, FileMode.Open)))
            {
                FileInfo fi = new FileInfo(RoiOutput);
                byte[] bytes = binaryReader.ReadBytes((int)fi.Length);
                binaryReader.Close();
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(bytes);
                bitmapImage.EndInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                getWhiteAreaBound(ImageTool.getInstance().BitmapImageToBitMap(bitmapImage), soursePath, i);
            }
            string CldInput = GlobalValues.FilesPath + @"\WPFInk\cache\CldInput" + i + ".bmp";
            string CldOutput = GlobalValues.FilesPath + @"\WPFInk\cache\CldOutput" + i + ".bmp";
            CmdUtility.ExeCommand(GlobalValues.FilesPath + "/WPFInk/CLD.exe " + CldInput + " " + CldOutput);
            this.keyFrameList[i].frameBmp = new Bitmap(CldOutput);
            this.pictureBoxShotList.Refresh();
        }
        public void removeBlackAreaBound(Bitmap roiImage, string soursePath,int index)
        {
            BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(roiImage);
            int w = roiImage.Width, h = roiImage.Height;
            int bytes = w * h * 4;
            byte[] ArgbValues = new byte[bytes];
            bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
            //Bitmap bitmapSource = new Bitmap(bitmap.Width, bitmap.Height);
            int i, j;
            int indexBmp = 0;
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(soursePath);
            BitmapSource bitmapSource2 = ImageTool.getInstance().BitmapToBitmapSource(bitmap);
            byte[] ArgbValuesTarget = new byte[bytes];
            bitmapSource2.CopyPixels(ArgbValuesTarget, w * 4, 0);

            for (i = 0; i < w; i++)
            {
                for (j = 0; j < h; j++)
                {
                    indexBmp = 4 * (w * j + i);
                    if (ArgbValues[indexBmp + 3] == 255 && ArgbValues[indexBmp] == 0
                        && ArgbValues[indexBmp + 1] == 0 && ArgbValues[indexBmp + 2] == 0)
                    {
                        ArgbValuesTarget[indexBmp + 3] = ArgbValuesTarget[indexBmp] =
                        ArgbValuesTarget[indexBmp + 1] = ArgbValuesTarget[indexBmp + 2] = 255;
                    }
                }
            }
            WriteableBitmap bitmapTarget = new WriteableBitmap(w, h, bitmapSource.DpiX, bitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            Int32Rect sourceRect = new Int32Rect(0, 0, w, h);
            bitmapTarget.WritePixels(sourceRect, ArgbValuesTarget, w * 4, 0);
            ArgbValues = null;
            //保存
            string savePath = GlobalValues.FilesPath + @"\WPFInk\cache\CldInput" + index + ".bmp";
            ImageTool.getInstance().SaveImage(savePath, (BitmapSource)bitmapTarget);
            //return bitmapTarget;
        }
        private void getWhiteAreaBound(Bitmap roiImage, string soursePath, int index)
        {
            BitmapSource bitmapSource = ImageTool.getInstance().BitmapToBitmapSource(roiImage);
            int w = roiImage.Width, h = roiImage.Height;
            int bytes = w * h * 4;
            byte[] ArgbValues = new byte[bytes];
            bitmapSource.CopyPixels(ArgbValues, w * 4, 0);
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            //Bitmap bitmapSource = new Bitmap(bitmap.Width, bitmap.Height);
            int i, j;
            int indexBmp = 0;
            //计算left
            for (i = 0; i < w; i++)
            {
                for (j = 0; j < h; j++)
                {
                    indexBmp = 4 * (w * j + i);
                    if (ArgbValues[indexBmp + 3] == 255 && ArgbValues[indexBmp] == 255
                        && ArgbValues[indexBmp + 1] == 255 && ArgbValues[indexBmp + 2] == 255)
                    {
                        left = i;
                        break;
                    }
                }
                if (j < h)
                    break;
            }
            //计算top
            for (i = 0; i < h; i++)
            {
                for (j = left; j < w; j++)
                {
                    indexBmp = 4 * (w * i + j);
                    if (ArgbValues[indexBmp + 3] == 255 && ArgbValues[indexBmp] == 255
                        && ArgbValues[indexBmp + 1] == 255 && ArgbValues[indexBmp + 2] == 255)
                    {
                        top = i;
                        break;
                    }
                }
                if (j < w)
                    break;
            }
            //计算right
            for (i = (int)(w - 1); i >= left; i--)
            {
                for (j = top; j < h; j++)
                {
                    indexBmp = 4 * (w * j + i);
                    if (ArgbValues[indexBmp + 3] == 255 && ArgbValues[indexBmp] == 255
                        && ArgbValues[indexBmp + 1] == 255 && ArgbValues[indexBmp + 2] == 255)
                    {
                        right = i;
                        break;
                    }
                }
                if (j < h)
                    break;
            }
            //计算bottom
            for (i = (int)(h - 1); i >= top; i--)
            {
                for (j = left; j <= right; j++)
                {
                    indexBmp = 4 * (w * i + j);
                    if (ArgbValues[indexBmp + 3] == 255 && ArgbValues[indexBmp] == 255
                        && ArgbValues[indexBmp + 1] == 255 && ArgbValues[indexBmp + 2] == 255)
                    {
                        bottom = i;
                        break;
                    }
                }
                if (j <= right)
                    break;
            }
            Thickness tn = new Thickness(left, top, right, bottom);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(soursePath);
            BitmapSource bitmapSource2 = ImageTool.getInstance().BitmapToBitmapSource(bitmap);
            byte[] ArgbValuesSource = new byte[w*h* 4];
            byte[] ArgbValuesTarget = new byte[(right - left) * (bottom - top) * 4];
            bitmapSource2.CopyPixels(ArgbValuesSource, w * 4, 0);
            int indexBmpTarget = 0;
            int widthTarget = right - left;
            int heightTarget = bottom - top;
            for (i = left; i < right; i++)
            {
                for (j = top; j < bottom; j++)
                {
                    //if (ArgbValues[indexBmp + 3] == 255 && ArgbValues[indexBmp] == 0
                    //    && ArgbValues[indexBmp + 1] == 0 && ArgbValues[indexBmp + 2] == 0)
                    //{
                    //    ArgbValuesTarget[indexBmp + 3] = ArgbValuesTarget[indexBmp] =
                    //    ArgbValuesTarget[indexBmp + 1] = ArgbValuesTarget[indexBmp + 2] = 255;
                    //}
                    indexBmp = 4 * (w * j + i);
                    indexBmpTarget = 4 * (widthTarget * (j - top) + i - left);
                    ArgbValuesTarget[indexBmpTarget + 3] = ArgbValuesSource[indexBmp + 3];
                    ArgbValuesTarget[indexBmpTarget] = ArgbValuesSource[indexBmp];
                    ArgbValuesTarget[indexBmpTarget + 1] = ArgbValuesSource[indexBmp + 1];
                    ArgbValuesTarget[indexBmpTarget + 2] = ArgbValuesSource[indexBmp + 2];
                }
            }
            WriteableBitmap bitmapTarget = new WriteableBitmap(widthTarget == 0 ? widthTarget + 1 : widthTarget, heightTarget == 0 ? heightTarget + 1 : heightTarget, bitmapSource.DpiX, bitmapSource.DpiY, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.Halftone125);
            Int32Rect sourceRect = new Int32Rect(0, 0, widthTarget, heightTarget);
            bitmapTarget.WritePixels(sourceRect, ArgbValuesTarget, widthTarget * 4, 0);
            ArgbValues = null;
            //保存
            string savePath = GlobalValues.FilesPath + @"\WPFInk\cache\CldInput" + index + ".bmp";
            ImageTool.getInstance().SaveImage(savePath, (BitmapSource)bitmapTarget);
        }
        private void buttonLast_Click(object sender, RoutedEventArgs e)
        {
            if (curPage > 0)
                this.curPage--;
            this.pictureBoxShotList.Refresh();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this.curPage++;
            this.pictureBoxShotList.Refresh();
        }

        private void buttonLayout_Click(object sender, RoutedEventArgs e)
        {            
            if (-1 != selectedBmpID)
                keyFrameList[selectedBmpID].frameBmp = new Bitmap(GlobalValues.FilesPath+"/WPFInk/cache/CldOutput" + selectedBmpID + ".bmp");
            foreach (Frame frame in this.keyFrameList)
            {
                if (frame.frameBmp != null)
                    this.imageList.Add(frame.frameBmp);
                //AddBmp(frame.frameBmp);
            }

            this.bShowBk = true;
            //this.pictureBox1.Width = int.Parse(this.textBoxCanvasWidth.Text);
            //this.pictureBox1.Height = int.Parse(this.textBoxCanvasHeight.Text);

            Resize();
            Arrange();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "dat|*.dat";
            //if (DialogResult.OK == dlg.ShowDialog())

            // 获取目录名
            //FileInfo info = new FileInfo(dlg.FileName);
            FileInfo info = new FileInfo(GlobalValues.FilesPath + "//keyframes/temporary.dat");

            string strDirectoryName = info.DirectoryName;

            // 写入文件
            StreamWriter writer = new StreamWriter(new FileStream(dlg.FileName, FileMode.Create));
            //StreamWriter writer = new StreamWriter(new FileStream("f://temporary.dat", FileMode.Create));
            for (int i = 0; i < this.resizedImgs.Count; i++)
            {
                string bmpFileName = strDirectoryName + i + ".bmp";
                resizedImgs[i].Save(bmpFileName);
                writer.WriteLine(i + ".bmp");
                writer.WriteLine(points[i].X);
                writer.WriteLine(points[i].Y);
            }
            writer.Close();

            for (int i = 0; i < this.resizedImgs.Count; i++)
            {
                System.Windows.Point p = new System.Windows.Point();
                p.X = points[i].X;
                p.Y = points[i].Y;
                WPoints.Add(p);
            }

            this.Close();
            inkframe._myprogressBar.Visibility = Visibility.Visible;
            inkframe._myprogressBar.sbProgressBar.Completed += new EventHandler(sbProgressBar_Completed);
            inkframe._myprogressBar.sbProgressBar.Begin();
            //inkframe.LoadSketch(resizedImgs, WPoints, _width, _height);

        }

        void sbProgressBar_Completed(object sender, EventArgs e)
        {
            inkframe._myprogressBar.Visibility = Visibility.Collapsed;
            inkframe.LoadSketch(resizedImgs, WPoints, _width, _height);
        }

        private void buttonAddThis_Click(object sender, RoutedEventArgs e)
        {
            //this.pictureBoxFrame.BackgroundImage
            System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog()
                {
                    Multiselect=true
                };
            if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
            {
                this.keyFrameList.Add(new Frame(new Bitmap(dlg.FileName), new Bitmap(dlg.FileName),0));
                this.keyFrameNoList.Add(0);
                AddRect(new Bitmap(dlg.FileName));
            }
            this.pictureBoxShotList.Refresh();
        }

        private void buttonPause_Click(object sender, RoutedEventArgs e)
        {
            switch (pause_resume)
            {
                case 1:
                    this.timer1.Stop();
                    this.buttonPause.Content = "Resume";
                    pause_resume = 2;
                    break;
                case 2:
                    this.timer1.Start();
                    this.buttonPause.Content = "Pause";
                    pause_resume = 1;
                    break;
            }
        }

        private void buttonFilter_Click(object sender, RoutedEventArgs e)
        {
            filterFrame((int)this.numericUpDownKeyFrameNum.Value);
            rearrangeFrameRec();
            this.pictureBoxShotList.Refresh();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            FileStream myStream = new FileStream(GlobalValues.FilesPath + @"\WPFInk\cache\keyFrames\time.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(myStream);
            foreach (Frame frame in this.keyFrameList)
            {
                frame.frameBmp.Save(GlobalValues.FilesPath + @"\WPFInk\cache\keyFrames\" + (++count) + ".png");
                sWriter.WriteLine(count.ToString() + " " + (frame.time).ToString());
            }
            sWriter.Close();
            myStream.Close();
        }


    }
}
