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
using Ink = Microsoft.Ink;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using WPFInk.Global;



namespace WPFInk
{
    /// <summary>
    /// Interaction logic for FormAnnotationOld.xaml
    /// </summary>
    /// 
    public partial class FormAnnotationOld : Window
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.NumericUpDown numericUpDownPenWidth;
        private System.Windows.Forms.Button buttonPenColor;

        private PanelMainFrame pictureBoxFrameMain;
        private PanelNormalFrame pictureBoxFrameLast;
        private PanelNormalFrame pictureBoxFrameNext;
        private PictureBox pictureBox;

        //7.27private SketchPlayer sketchPl;
        private InkFrame inkFrame;
        private int selectIndex;

        public FormAnnotationOld()
        {
            this.Loaded += new RoutedEventHandler(FormAnnotationOld_Loaded);
            this.Loaded += new RoutedEventHandler(FormAnnotationOld_Load);
            InitializeComponent();
        }

        /*7.27public FormAnnotationOld(SketchPlayer sketchPl,int index)
        {
            this.sketchPl = sketchPl;
            selectIndex = index;
            this.Loaded += new RoutedEventHandler(FormAnnotationOld_Loaded);
            this.Loaded += new RoutedEventHandler(FormAnnotationOld_Load);
            InitializeComponent();
        }*/

        public FormAnnotationOld(InkFrame inkF,int index)
        {
            this.inkFrame = inkF;
            selectIndex = index;
            this.Loaded += new RoutedEventHandler(FormAnnotationOld_Loaded);
            this.Loaded += new RoutedEventHandler(FormAnnotationOld_Load);
            InitializeComponent();
        }


        void FormAnnotationOld_Loaded(object sender, RoutedEventArgs e)
        {
            this.components = new System.ComponentModel.Container();
            this.numericUpDownPenWidth = new System.Windows.Forms.NumericUpDown();
            this.buttonPenColor = new System.Windows.Forms.Button();
            this.pictureBoxFrameMain = new PanelMainFrame(this);
            this.pictureBoxFrameLast = new PanelNormalFrame();
            this.pictureBoxFrameNext = new PanelNormalFrame();
            this.pictureBox = new PictureBox();

            windowsFormsHostFrameMain.Child = pictureBoxFrameMain;
            windowsFormsHostFrameLast.Child = pictureBoxFrameLast;
            windowsFormsHostFrameNext.Child = pictureBoxFrameNext;
            windowsFormsHostPenWidth.Child = numericUpDownPenWidth;
            windowsFormsHostPenColor.Child = buttonPenColor;
            windowsFormsHostpictureBox.Child = pictureBox;
            // 
            // buttonPenColor
            // 
            this.buttonPenColor.BackColor = System.Drawing.Color.Red;
            this.buttonPenColor.TabIndex = 3;
            this.buttonPenColor.UseVisualStyleBackColor = false;
            this.buttonPenColor.Click += new System.EventHandler(this.buttonPenColor_Click);
            // 
            // numericUpDownPenWidth
            // 
            this.numericUpDownPenWidth.TabIndex = 4;
            this.numericUpDownPenWidth.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // pictureBoxFrameNext
            // 
            this.pictureBoxFrameNext.BackColor = System.Drawing.SystemColors.ControlText;
            this.pictureBoxFrameNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxFrameNext.CurFrameNum = 0;
            this.pictureBoxFrameNext.TabIndex = 2;
            this.pictureBoxFrameNext.TabStop = false;
            // 
            // pictureBoxFrameMain
            // 
            this.pictureBoxFrameMain.BackColor = System.Drawing.SystemColors.ControlText;
            this.pictureBoxFrameMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxFrameMain.CurFrameNum = 0;
            this.pictureBoxFrameMain.TabIndex = 1;
            this.pictureBoxFrameMain.TabStop = false;
            // 
            // pictureBoxFrameLast
            // 
            this.pictureBoxFrameLast.BackColor = System.Drawing.SystemColors.ControlText;
            this.pictureBoxFrameLast.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxFrameLast.CurFrameNum = 0;
            this.pictureBoxFrameLast.TabIndex = 0;
            this.pictureBoxFrameLast.TabStop = false;
            //
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPenWidth)).EndInit();
            //
        }

        #region variables
        /// <summary>
        /// ink collector
        /// </summary>
        private Ink.InkCollector inkCollector = null;

        /// <summary>
        /// avi reader
        /// </summary>
        private AVIReader reader = new WPFInk.AVIReader();

        /// <summary>
        /// 视频路径
        /// </summary>
        private string videoFilePath = string.Empty;

        /// <summary>
        /// 当前帧号
        /// </summary>
        private int currentFrameNumber = 1;

        public List<Annotation> GetListAnnos()
        {
            return this.pictureBoxFrameMain.ListAnnos;
        }

        public string VideoFilePath
        {
            get { return videoFilePath; }
            set { videoFilePath = value; }
        }
        #endregion

        #region init
        private void FormAnnotationOld_Load(object sender, EventArgs e)
        {
            //inkCollector = new Microsoft.Ink.InkCollector(this.Handle);改动过
            inkCollector = new Microsoft.Ink.InkCollector(this.pictureBox);
            inkCollector.Enabled = true;
            inkCollector.CollectionMode = Microsoft.Ink.CollectionMode.InkAndGesture;

            inkCollector.Gesture += new Microsoft.Ink.InkCollectorGestureEventHandler(inkCollector_Gesture);
            inkCollector.Stroke += new Microsoft.Ink.InkCollectorStrokeEventHandler(inkCollector_Stroke);

            inkCollector.SetGestureStatus(Microsoft.Ink.ApplicationGesture.AllGestures,true);
            if (videoFilePath != string.Empty)
            {
                reader.Open(this.videoFilePath);
                DrawFrames();
            }
        }
        #endregion

        #region ink
        void inkCollector_Stroke(object sender, Microsoft.Ink.InkCollectorStrokeEventArgs e)
        {
            // 把未识别为手势的ink笔迹擦除
            inkCollector.Ink.DeleteStrokes();
            
            this.pictureBox.Refresh();//屏蔽了原来类的刷新方法，WPF下识别不出来。
        }

        void inkCollector_Gesture(object sender, Microsoft.Ink.InkCollectorGestureEventArgs e)
        {
            switch(e.Gestures[0].Id.ToString())
            {
                case "Left"://向前一帧
                    last();
                    break;
                case "Right":// 向后一帧
                    Next();
                    break;
                case "SemiCircleRight"://向后十帧
                    NextTen();
                    break;
                case "SemiCircleLeft": // 向前十帧
                    LastTen();
                    break;
                case "ChevronDown":
                    this.pictureBoxFrameMain.CurAnno.EndFrame = this.pictureBoxFrameMain.CurFrameNum;
                    //int i = sketchPl.panelPointView.nodeList[selectIndex].hyper;
                    int i = inkFrame.pointView.pointView.nodeList[selectIndex].hyper;
                    this.pictureBoxFrameMain.SaveSketchToImg(GlobalValues.FilesPath+"/WPFInk/" + selectIndex + " " + pictureBoxFrameMain.CurAnno.StartFrame + "-" + pictureBoxFrameMain.CurAnno.EndFrame + "Sketch.bmp");

                    //7.27sketchPl.panelPointView.nodeList[selectIndex]._sketchInnodevideo[i]._startFrame = pictureBoxFrameMain.CurAnno.StartFrame;
                    //7.27sketchPl.panelPointView.nodeList[selectIndex]._sketchInnodevideo[i]._endFrame = pictureBoxFrameMain.CurAnno.EndFrame;

                    inkFrame.pointView.pointView.nodeList[selectIndex]._sketchInnodevideo[i]._startFrame = pictureBoxFrameMain.CurAnno.StartFrame;
                    inkFrame.pointView.pointView.nodeList[selectIndex]._sketchInnodevideo[i]._endFrame = pictureBoxFrameMain.CurAnno.EndFrame;

                    break;
            }
        }
        #endregion

        #region Frame Number
        private void Next()
        {
            if(currentFrameNumber < reader.Length - 1 )
            {
                currentFrameNumber++;
                DrawFrames();
            } 
        }

        private void NextTen()
        {
            if(currentFrameNumber + 10 < reader.Length)
            {
                currentFrameNumber += 10;
                DrawFrames();
            }
        }

        private void last()
        {
            if (currentFrameNumber > 1)
            {
                currentFrameNumber--;
                DrawFrames();
            }
        }

        private void LastTen()
        {
            if (currentFrameNumber > 11)
            {
                currentFrameNumber -= 10;
                DrawFrames();
            }
        }
        #endregion

        #region Draw
        public void DrawFrames()
        {
            reader.CurrentPosition = currentFrameNumber - 1;
            
            this.pictureBoxFrameLast.BackgroundImage = reader.GetNextFrame();
            pictureBoxFrameLast.CurFrameNum = currentFrameNumber-1;
            pictureBoxFrameLast.Annos = pictureBoxFrameMain.ListAnnos;

            this.pictureBoxFrameMain.BackgroundImage = reader.GetNextFrame();
            pictureBoxFrameMain.CurFrameNum = currentFrameNumber;
            
            this.pictureBoxFrameNext.BackgroundImage = reader.GetNextFrame();
            pictureBoxFrameNext.CurFrameNum = currentFrameNumber + 1;
            pictureBoxFrameNext.Annos = pictureBoxFrameMain.ListAnnos;
        }
        #endregion

        private void buttonPenColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
            {
                buttonPenColor.BackColor = dlg.Color;
            }
        }

        public System.Drawing.Color GetPenColor()
        {
            return this.buttonPenColor.BackColor;
        }

        public double GetPenWidth()
        {
            return Convert.ToDouble(this.numericUpDownPenWidth.Value);
        }

        private void RefreshAll()
        {
            this.pictureBoxFrameLast.Refresh();
            this.pictureBoxFrameMain.Refresh();
        }

        protected override void OnClosed(EventArgs e)
        {
            //int i = sketchPl.panelPointView.nodeList[selectIndex].hyper;
            //this.pictureBoxFrameMain.SaveSketchToImg("F://"+selectIndex+" "+pictureBoxFrameMain.CurAnno.StartFrame + "-" + pictureBoxFrameMain.CurAnno.EndFrame + "Sketch.bmp");

            //sketchPl.panelPointView.nodeList[selectIndex]._sketchInnodevideo[i]._startFrame = pictureBoxFrameMain.CurAnno.StartFrame;
            //sketchPl.panelPointView.nodeList[selectIndex]._sketchInnodevideo[i]._endFrame = pictureBoxFrameMain.CurAnno.EndFrame;
 	        base.OnClosed(e);
            //inkFrame.pointView.pointView.lineStyle = false;

            //inkFrame.pointView.pointView.inkPictureNode.Refresh();

        }

    }
}
