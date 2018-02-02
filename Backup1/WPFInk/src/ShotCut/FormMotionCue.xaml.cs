using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;
using WPFInk.Global;

namespace WPFInk
{
    /// <summary>
    /// Interaction logic for FormMotionCue.xaml
    /// </summary>
    public partial class FormMotionCue : Window
    {
        public Bitmap curFrameBmp = null;
        public Bitmap nextFrameBmp = null;

        WPFInk.ShotCut.ShotCut shotCut = null;
        // 中心点
        System.Drawing.Point midPoint = new System.Drawing.Point(200, 200);
        int offsetWidth = 100;
        int band = 5;

        int groupCount = 15;

        int innerRadius = 20;//原来为50
        int outerRadius = 80;//原来为150

        public Bitmap bkBmp = null;
        public Bitmap resultBmp = null;

        //private System.ComponentModel.IContainer components = null;

        
        private System.Windows.Forms.TrackBar trackBarOuterRadius;
        private System.Windows.Forms.TrackBar trackBarGroupCount;
        private System.Windows.Forms.TrackBar trackBarInnerRadius;
        private System.Windows.Forms.TrackBar trackBarOffsetWidth;
        private System.Windows.Forms.TrackBar trackBarIntensity;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.PictureBox pictureBoxOrin;
        private System.Windows.Forms.PictureBox pictureBoxSketchResult;


        public WPFInk.ShotCut.ShotCut Shotcut
        {
            get
            {
                return shotCut;
            }
        }

        public void setShotCut(WPFInk.ShotCut.ShotCut sc)
        {
            shotCut = sc;
        }

        public FormMotionCue()
        {
            this.Loaded += new RoutedEventHandler(FormMotionCue_Loaded);
            InitializeComponent();
        }

        void FormMotionCue_Loaded(object sender,RoutedEventArgs e)
        {
            

            trackBarInnerRadius = new System.Windows.Forms.TrackBar();
            trackBarOuterRadius = new System.Windows.Forms.TrackBar();
            trackBarGroupCount = new System.Windows.Forms.TrackBar();
            trackBarOffsetWidth = new System.Windows.Forms.TrackBar();
            trackBarIntensity = new System.Windows.Forms.TrackBar();
            comboBoxType = new System.Windows.Forms.ComboBox();

            pictureBoxSketchResult=new System.Windows.Forms.PictureBox();
            pictureBoxOrin= new System.Windows.Forms.PictureBox();


            windowsFormsHostGroupCount.Child = trackBarGroupCount;
            windowsFormsHostInnerRadius.Child = trackBarInnerRadius;
            windowsFormsHostIntensity.Child = trackBarIntensity;
            windowsFormsHostOffsetWidth.Child = trackBarOffsetWidth;
            windowsFormsHostOuterRadius.Child = trackBarOuterRadius;
            windowsFormsHostType.Child = comboBoxType;

            windowsFormsHostSketchResult.Child = pictureBoxSketchResult;
            //windowsFormsHostOrin.Child = pictureBoxOrin;

            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSketchResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOuterRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGroupCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarInnerRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOffsetWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarIntensity)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.pictureBoxOFlow)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.pictureBoxDirect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOrin)).BeginInit();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSketchResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOuterRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGroupCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarInnerRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOffsetWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarIntensity)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.pictureBoxOFlow)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.pictureBoxDirect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOrin)).EndInit();
            

            this.pictureBoxSketchResult.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBoxSketchResult.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);

            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "straight",
            "radial",
            "bang",
            "curve"});
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.TabIndex = 3;
            this.comboBoxType.Text = "None";
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);

            this.trackBarOuterRadius.Maximum = 100;//原来为1000
            this.trackBarOuterRadius.Value = 40;//原来为1000
            this.trackBarOuterRadius.ValueChanged += new System.EventHandler(this.trackBarOuterRadius_ValueChanged);

            this.trackBarGroupCount.Maximum = 50;
            this.trackBarGroupCount.Minimum = 1;
            this.trackBarGroupCount.Value = 15;
            this.trackBarGroupCount.ValueChanged += new System.EventHandler(this.trackBarGroupCount_ValueChanged);

            this.trackBarInnerRadius.Maximum = 50;//原来为200
            this.trackBarInnerRadius.Minimum = 10;//原来为100
            this.trackBarInnerRadius.Value = 20;
            this.trackBarInnerRadius.ValueChanged += new System.EventHandler(this.trackBarInnerRadius_ValueChanged);

            this.trackBarOffsetWidth.Maximum = 200;
            this.trackBarOffsetWidth.Minimum = -200;
            this.trackBarOffsetWidth.ValueChanged += new System.EventHandler(this.trackBarOffsetWidth_ValueChanged);

            this.trackBarIntensity.Maximum = 30;
            this.trackBarIntensity.Minimum = 1;
            this.trackBarIntensity.Value = 5;
            this.trackBarIntensity.ValueChanged += new System.EventHandler(this.trackBarIntensity_ValueChanged);

            this.pictureBoxOrin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            //this.pictureBoxOFlow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
            //this.pictureBoxOFlow.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
        }

        int angle = 30;
        private System.Drawing.Point Tran(double angle, System.Drawing.Point pt1, System.Drawing.Point ptcenter)
        {
            System.Drawing.Point pt2 = new System.Drawing.Point();
            System.Drawing.Point temp = new System.Drawing.Point();
            temp.X = pt1.Y;
            temp.Y = pt1.X;
            pt2.X = ptcenter.X + (int)((double)(temp.X - ptcenter.X) * Math.Cos(angle * Math.PI / 180) - (double)(temp.Y - ptcenter.Y) * Math.Sin(angle * Math.PI / 180));
            pt2.Y = ptcenter.Y + (int)((double)(temp.Y - ptcenter.Y) * Math.Cos(angle * Math.PI / 180) + (double)(temp.X - ptcenter.X) * Math.Sin(angle * Math.PI / 180));
            return pt2;
        }

        List<System.Drawing.Point> pts = new List<System.Drawing.Point>();
        //int direct = -1;
        private void DrawCurve(Graphics g)
        {
            if (pts.Count != 2)
                return;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(2 * pts[0].X, 2 * pts[0].Y, 2 * pts[1].X - 2 * pts[0].X, 2 * pts[1].Y - 2 * pts[0].Y);
            Random rand = new Random();
            for (int i = 0; i < trackBarIntensity.Value; i++)
            {
                int offset = rand.Next(-10, 10);
                System.Drawing.Point[] npts = new System.Drawing.Point[4];
                System.Drawing.Point[] npts2 = new System.Drawing.Point[4];
                npts[0] = new System.Drawing.Point(rect.Right - offset, rect.Top);
                npts[1] = new System.Drawing.Point(rect.Right - this.trackBarOffsetWidth.Value - rand.Next(-2, 2) - offset, rect.Top + rect.Height / 3);
                npts[2] = new System.Drawing.Point(rect.Right - this.trackBarOffsetWidth.Value - rand.Next(-2, 2) - offset, rect.Top + rect.Height * 2 / 3);
                npts[3] = new System.Drawing.Point(rect.Right - offset, rect.Bottom);
                for (int j = 0; j < 4; j++)
                {
                    npts2[j] = Tran(angle, npts[j], new System.Drawing.Point(rect.Right - this.offsetWidth / 2, rect.Bottom + rect.Top / 2));
                }
                g.DrawCurve(Pens.Black, npts2);
            }
        }

        private void DrawStraight(Graphics g)
        {
            Random rand = new Random();

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(2 * pts[0].X, 2 * pts[0].Y, 2 * pts[1].X - 2 * pts[0].X, 2 * pts[1].Y - 2 * pts[0].Y);
            for (int i = rect.Top; i < rect.Bottom; i += 20 + rand.Next(-10, 10))
            {
                int yOffset = 0, xOffset = 0;
                for (int j = 0; j < band; j++)
                {
                    yOffset = rand.Next(-3, 3);
                    xOffset = rand.Next(-10, 10);
                    if (j == 0)
                        g.DrawLine(Pens.Black, new System.Drawing.Point(rect.Left + rand.Next(-10, 10), i + yOffset), new System.Drawing.Point(rect.Right + xOffset + 20, i + yOffset));
                    else
                        g.DrawLine(Pens.Black, new System.Drawing.Point(rect.Left + rand.Next(-10, 10), i + yOffset), new System.Drawing.Point(rect.Right + xOffset, i + yOffset));
                }
            }
        }

        private void DrawRadial(Graphics g)
        {
            Random rand = new Random();
            // draw radial lines
            int outerRadius = 1000;
            for (int i = 0; i < groupCount; i++)
            {
                for (int j = 0; j < band; j++)
                {
                    double offset = (double)rand.Next(-offsetWidth, offsetWidth) / 1000;
                    int innerOffset = rand.Next(-10, 10);
                    int x1 = (int)(midPoint.X + (innerRadius + innerOffset) * Math.Sin(i * 2 * Math.PI / groupCount + offset));
                    int y1 = (int)(midPoint.Y + (innerRadius + innerOffset) * Math.Cos(i * 2 * Math.PI / groupCount + offset));
                    int x2 = (int)(midPoint.X + outerRadius * Math.Sin(i * 2 * Math.PI / groupCount + offset));
                    int y2 = (int)(midPoint.Y + outerRadius * Math.Cos(i * 2 * Math.PI / groupCount + offset));
                    System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Brushes.Black, 3);
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }

        private void DrawBang(Graphics g)
        {
            Random rand = new Random();
            GraphicsPath path = new GraphicsPath();

            List<System.Drawing.Point> pts = new List<System.Drawing.Point>();
            int lastX = (int)(midPoint.X + (outerRadius + rand.Next(-Math.Abs(offsetWidth), Math.Abs(offsetWidth))) * Math.Sin(0 * 2 * Math.PI / groupCount));
            int lastY = (int)(midPoint.Y + (outerRadius + rand.Next(-Math.Abs(offsetWidth), Math.Abs(offsetWidth))) * Math.Cos(0 * 2 * Math.PI / groupCount));
            pts.Add(new System.Drawing.Point(lastX, lastY));


            for (int i = 1; i < groupCount; i++)
            {
                int tempOutRadius = outerRadius + rand.Next(-Math.Abs(offsetWidth), Math.Abs(offsetWidth));
                int x1 = (int)(midPoint.X + (tempOutRadius) * Math.Sin(i * 2 * Math.PI / groupCount));
                int y1 = (int)(midPoint.Y + (tempOutRadius) * Math.Cos(i * 2 * Math.PI / groupCount));
                for (int j = 0; j < band; j++)
                {
                    int tempInnerRadius = innerRadius + rand.Next(-Math.Abs(offsetWidth), Math.Abs(offsetWidth));
                    int x = (int)(midPoint.X + (tempInnerRadius) * Math.Sin((i - 1) * 2 * Math.PI / groupCount + j * 2 * Math.PI / groupCount / band));
                    int y = (int)(midPoint.Y + (tempInnerRadius) * Math.Cos((i - 1) * 2 * Math.PI / groupCount + j * 2 * Math.PI / groupCount / band));
                    pts.Add(new System.Drawing.Point(x, y));
                }
                pts.Add(new System.Drawing.Point(x1, y1));
                lastX = x1;
                lastY = y1;
            }
            for (int j = 0; j < band; j++)
            {
                int x = (int)(midPoint.X + (innerRadius + rand.Next(-Math.Abs(offsetWidth), Math.Abs(offsetWidth))) * Math.Sin((0 - 1) * 2 * Math.PI / groupCount + j * 2 * Math.PI / groupCount / band));
                int y = (int)(midPoint.Y + (innerRadius + rand.Next(-Math.Abs(offsetWidth), Math.Abs(offsetWidth))) * Math.Cos((0 - 1) * 2 * Math.PI / groupCount + j * 2 * Math.PI / groupCount / band));
                pts.Add(new System.Drawing.Point(x, y));
            }

            for (int i = 1; i < pts.Count; i++)
                path.AddLine(pts[i - 1], pts[i]);
            path.AddLine(pts[pts.Count - 1], pts[0]);

            // 填充内部为白色
            g.FillPath(System.Drawing.Brushes.Transparent, path);
            //g.DrawPath(Pens.Black, path);
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Brushes.Black, 3);
            g.DrawPath(pen, path);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.midPoint = e.Location;
                this.pictureBoxSketchResult.Refresh();
            }                                                                                                               
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (null == bkBmp)
                return;
            resultBmp = new Bitmap(bkBmp.Width, bkBmp.Height);
            Graphics g = Graphics.FromImage(resultBmp);

            this.pictureBoxSketchResult.Width =  bkBmp.Width;
            this.pictureBoxSketchResult.Height =  bkBmp.Height;
            //this.pictureBoxOFlow.Top = this.pictureBoxSketchResult.Height;
            //this.pictureBoxOFlow.Width = this.pictureBoxSketchResult.Width / 2;
            //this.pictureBoxOFlow.Height = this.pictureBoxSketchResult.Height / 2;


            this.pictureBoxOrin.Width = this.pictureBoxSketchResult.Width / 2;
            this.pictureBoxOrin.Height = this.pictureBoxSketchResult.Height / 2;
            this.pictureBoxOrin.Top = this.pictureBoxSketchResult.Height;
            this.pictureBoxOrin.Left = this.pictureBoxSketchResult.Width / 2;
            if (null != curFrameBmp)
                this.pictureBoxOrin.BackgroundImage = curFrameBmp;
            g.DrawImage(bkBmp, new System.Drawing.Rectangle(0, 0, this.pictureBoxSketchResult.Width, this.pictureBoxSketchResult.Height), new System.Drawing.Rectangle(0, 0, bkBmp.Width, bkBmp.Height), GraphicsUnit.Pixel);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            switch (this.comboBoxType.Text)
            {
                case "straight":
                    DrawStraight(g);
                    break;
                case "radial":
                    DrawRadial(g);
                    break;

                case "bang":
                    DrawBang(g);
                    break;
                case "curve":
                    DrawCurve(g);
                    break;
            }
            g.Dispose();
            e.Graphics.DrawImage(resultBmp, new System.Drawing.Rectangle(0, 0, this.pictureBoxSketchResult.Width, this.pictureBoxSketchResult.Height), new System.Drawing.Rectangle(0, 0, resultBmp.Width, resultBmp.Height), GraphicsUnit.Pixel);
            resultBmp.Save(GlobalValues.FilesPath+"/WPFInk/result.bmp");
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxType.Text)
            {
                case "bang":
                    this.trackBarInnerRadius.Value = 47;
                    this.trackBarOuterRadius.Value = 64;
                    this.trackBarIntensity.Value = 7;
                    this.trackBarOffsetWidth.Value = 10;
                    this.trackBarGroupCount.Value = 28;
                    break;
                case "curve":
                    this.pictureBoxSketchResult.Refresh();
                    break;
            }
        }

        private void trackBarInnerRadius_ValueChanged(object sender, EventArgs e)
        {
            this.innerRadius = this.trackBarInnerRadius.Value;
            this.pictureBoxSketchResult.Refresh();
        }

        private void trackBarOuterRadius_ValueChanged(object sender, EventArgs e)
        {
            this.outerRadius = this.trackBarOuterRadius.Value;
            this.pictureBoxSketchResult.Refresh();
        }

        private void trackBarIntensity_ValueChanged(object sender, EventArgs e)
        {
            this.band = this.trackBarIntensity.Value;
            this.pictureBoxSketchResult.Refresh();
        }

        private void trackBarOffsetWidth_ValueChanged(object sender, EventArgs e)
        {
            this.offsetWidth = this.trackBarOffsetWidth.Value;
            this.pictureBoxSketchResult.Refresh();
        }

        private void trackBarGroupCount_ValueChanged(object sender, EventArgs e)
        {
            this.groupCount = this.trackBarGroupCount.Value;
            this.pictureBoxSketchResult.Refresh();
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)   
        {
            if (pts.Count == 2)
            {
                e.Graphics.DrawRectangle(Pens.Red, pts[0].X, pts[0].Y, pts[1].X - pts[0].X, pts[1].Y - pts[0].Y);


            }
        }

        /*private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (pts.Count >= 2)
                {
                    pts.Clear();
                }
                pts.Add(e.Location);
                if (pts.Count == 2)
                {
                    direct = CalculateDirection(pts[0], pts[1], this.pictureBoxOFlow.BackgroundImage);

                }
                this.pictureBoxOFlow.Refresh();
                //this.pictureBoxDirect.Refresh();
            }
        }*/

        private int CalculateDirection(System.Drawing.Point pt1, System.Drawing.Point pt2, System.Drawing.Image image)
        {
            int left = pt1.X < pt2.X ? pt1.X : pt2.X;
            int right = pt1.X > pt2.X ? pt1.X : pt2.X;

            int top = pt1.Y < pt2.Y ? pt1.Y : pt2.Y;
            int bottom = pt1.Y > pt2.Y ? pt1.Y : pt2.Y;

            int[] frequency = new int[361];
            for (int i = 0; i < frequency.Length; i++)
                frequency[i] = 0;

            Bitmap bmp = new Bitmap(image);
            for (int i = left; i < right; i++)
                for (int j = top; j < bottom; j++)
                {
                    frequency[(int)bmp.GetPixel(i, j).GetHue()]++;
                }

            int max = int.MinValue;
            int id = 0;
            for (int i = 1; i < frequency.Length; i++)
            {
                if (frequency[i] > max)
                {
                    max = frequency[i];
                    id = i;
                }
            }
            return id;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            this.pictureBoxSketchResult.Refresh();
            Random rand = new Random();
            //resultBmp.Save("F://" + rand.Next() + ".bmp");
            resultBmp.Save(GlobalValues.FilesPath+"//WPFInk/" + shotCut.selectedBmpID + ".bmp");
            this.Close();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            this.pts.Clear();
            this.pictureBoxSketchResult.Refresh();
        }


    }
}
