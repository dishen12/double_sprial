using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WPFInk.Global;

namespace WPFInk
{
    public partial class PanelVideo : UserControl
    {
        //SketchInNodeVideo snv = new SketchInNodeVideo();
        public InkFrame inkFrame;

        List<SketchInNodeVideo> snvList = new List<SketchInNodeVideo>();
        
        public int index;
        public int sign;

        public PanelVideo()
        {
            InitializeComponent();
        }

        /*7.27public PanelVideo(SketchPlayer SPlayer)
        {
            this.SPlayer = SPlayer;
            InitializeComponent();
        }*/
        public void SetInkFrame(InkFrame inkF)
        {
            inkFrame = inkF;
        }

        AVIReader reader = null;
        public void Play(string videoPath, int index)
        {
            this.index = index;

            //7.27sign = SPlayer.panelPointView.nodeList[index].hyper;
            sign = inkFrame.pointView.pointView.nodeList[index].hyper;
            if (sign != 0)
                // snv = SPlayer.panelPointView.nodeList[index]._sketchInnodevideo[0];
                //7.27snvList = SPlayer.panelPointView.nodeList[index]._sketchInnodevideo;
                snvList = inkFrame.pointView.pointView.nodeList[index]._sketchInnodevideo;

            reader = new AVIReader();
            reader.Open(videoPath);

            //if (Special == 1)
            //    this.timerVideo.Interval = 110;

            this.timerVideo.Interval = (int)(1000 / this.reader.FrameRate);
            this.timerVideo.Start();
            frameCount = 0;


            if (Special == 2)
            {
                LoadInk(GlobalValues.FilesPath + "/WPFInk/strokes.txt");
                LoadOffsets(GlobalValues.FilesPath + "/WPFInk/offsets.txt");
                frameCount = 22;
            }
        }

        Dictionary<int, Offset> dictionaryOffsets = new Dictionary<int, Offset>();
        Offset curOffset = new Offset(0, 0);
        // just for demo
        public int Special = 0;
        int frameCount = 0;
        private void timerVideo_Tick(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = reader.GetNextFrame();
                Graphics g = Graphics.FromImage(bmp);
                if (sign != 0)
                {
                    foreach (SketchInNodeVideo snv in snvList)
                    {
                        if (frameCount >= snv._startFrame && frameCount <= snv._endFrame)
                        {
                            Bitmap sketchBmp = new Bitmap(@GlobalValues.FilesPath+"/WPFInk/"+index+" "+snv._startFrame+"-"+snv._endFrame+"Sketch.bmp");
                            sketchBmp.MakeTransparent(Color.White);
                            g.DrawImage(sketchBmp, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, sketchBmp.Width, sketchBmp.Height), GraphicsUnit.Pixel);
                        }
                    }
                }

                /*if(Special == 1 && frameCount <= 30 && frameCount >= 10)
                {
                    Bitmap sketchBmp = new Bitmap(@"F://sketchNest.bmp");
                    sketchBmp.MakeTransparent(Color.White);
                    g.DrawImage(sketchBmp, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, sketchBmp.Width, sketchBmp.Height), GraphicsUnit.Pixel);
                }

                if (Special == 2)
                {
                    // chenjia add here 0304
                    if (dictionaryOffsets.ContainsKey(frameCount))
                        this.curOffset = dictionaryOffsets[frameCount];
                    DrawInk(g, curOffset);
                }*/
                this.BackgroundImage = bmp;
                frameCount++;
            }
            catch
            {
                this.BackColor = Color.Black;
                this.timerVideo.Stop();
            }
        }

        internal void Stop()
        {
            this.timerVideo.Stop();
            if (null != reader)
            {
                this.reader.Dispose();
                reader = null;
            }
        }

        private void PanelVideo_Load(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 读取offsets文件
        /// </summary>
        /// <param name="path"></param>
        void LoadOffsets(string path)
        {
            dictionaryOffsets.Clear();
            // 读数据文件（TXT）
            StreamReader sr = new StreamReader(path);
            while (false == sr.EndOfStream)
            {
                string newLine = sr.ReadLine();
                string[] strs = newLine.Split(",".ToCharArray());

                if (strs.Length != 3)
                    throw new Exception("数据有误");

                int key = int.Parse(strs[0]);
                Offset offset = new Offset(0, 0);
                offset.X = double.Parse(strs[1]);
                offset.Y = double.Parse(strs[2]);

                dictionaryOffsets.Add(key, offset);
            }
            Chazhi();
            sr.Close();
        }

        /// <summary>
        /// 对读取的offsets进行插值
        /// </summary>
        void Chazhi()
        {
            // Get the max value of key
            int max = int.MinValue;
            int min = int.MaxValue;
            foreach (KeyValuePair<int, Offset> pair in dictionaryOffsets)
            {
                if (pair.Key > max)
                    max = pair.Key;
                if (pair.Key < min)
                    min = pair.Key;
            }

            Dictionary<int, Offset> newDic = new Dictionary<int, Offset>();
            int last = -1;
            for (int i = 0; i <= max; i++)
            {
                if (dictionaryOffsets.ContainsKey(i))
                {
                    if (last != -1)
                    {
                        for (int m = last + 1; m <= i; m++)
                        {
                            Offset offset = new Offset(0, 0);
                            offset.X =
                                dictionaryOffsets[last].X +
                                (m - last) * (dictionaryOffsets[i].X - dictionaryOffsets[last].X) / (i - last);
                            offset.Y =
                                dictionaryOffsets[last].Y +
                                (m - last) * (dictionaryOffsets[i].Y - dictionaryOffsets[last].Y) / (i - last);
                            newDic.Add(m, offset);
                        }
                    }
                    last = i;
                }
            }
            dictionaryOffsets = newDic;
        }


        List<CJStroke> m_Strokes = new List<CJStroke>();

        void LoadInk(string path)
        {
            if (false == File.Exists(path))
            {
                throw new Exception("wrong path");
            }

            m_Strokes.Clear();
            CJStroke curStroke = null;
            FileStream fs = File.Open(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (false == sr.EndOfStream)
            {
                string newLine = sr.ReadLine();
                if (newLine.StartsWith("NewStroke"))
                {
                    curStroke = new CJStroke();
                    this.m_Strokes.Add(curStroke);
                }
                else
                {
                    string[] strs = newLine.Split(",".ToCharArray());
                    double x = double.Parse(strs[0]);
                    double y = double.Parse(strs[1]);
                    curStroke.AddPoint(x, y);
                }
            }
        }

        void DrawInk(Graphics g)
        {
            foreach (CJStroke stroke in m_Strokes)
            {
                stroke.Draw(g);
            }
        }
        void DrawInk(Graphics g, Offset offset)
        {
            foreach (CJStroke stroke in m_Strokes)
            {
                stroke.DrawWithOffset(g, offset);
            }
        }

        private void PanelVideo_Click(object sender, EventArgs e)
        {
            if(0!=sign)
            {
                foreach (SketchInNodeVideo snv in snvList)
                {
                    if (frameCount >= snv._startFrame && frameCount <= snv._endFrame)
                    {
                        this.Stop();
                        //7.27this.Play(SPlayer.videoPathList[snv._endIndex], snv._endIndex);
                        this.Play(inkFrame.videoPathList[snv._endIndex], snv._endIndex);
                    }
                }
            }
            
        }
    }
}
