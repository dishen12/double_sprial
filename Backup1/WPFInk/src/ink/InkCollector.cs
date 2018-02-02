using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Ink;
using System.Windows.Shapes;
using System.Windows;
using WPFInk.state;
using WPFInk.tool;
using WPFInk.cmd;
using WPFInk.ink;
using System.IO;
using WPFInk.video;
using WPFInk.graphic;
using WPFInk.Global;
using WPFInk.videoSummarization;
using WPFInk.template;

namespace WPFInk.ink
{

    /// <summary>
    /// InkMode定义了当前的编辑模式
    /// </summary>
    public enum InkMode
    {
        Ink,
        StrokeErase,
        PointErase,
        Select,
        Move,
        Rotate,
        Zoom,
        ImageMove,
        ImageRotate,
        ImageZoom,
        InsertText,
        InkPlay,
        GestureOnly,
        DrawArrow,
        VideoPlay,
        AutoMove,
        DrawGraphic,
        DrawStrokeInGraphic,
        VideoSummarization,
        MergeSummarization,
        HyperLinkSummarization,
        AddKeyFrameAnnotation,
        TapestrySummarization,
        MergeDoubleSummarization,
        //HyperLinkSummarization,
        AddKeyFrameAnnotationDouble,
        TapestryDoubleSummarization,
        DoubleSpiralSummarization,
        None
    }

    /// <summary>
    /// InkCollector保存笔迹的数据结构
    /// </summary>
    public class InkCollector
    {

        public InkCollector(InkCanvas canvas, InkFrame inkframe)
        {
            _inkCanvas = canvas;
            _mainPage = inkframe;

            //初始化state
            //init all the states:一定要在presenter之后init
            _state_ink = new InkState_Ink(this);
            _state_erase = new InkState_Erase(this);
            _state_pointerase = new InkState_PointErase(this);
            _state_move = new InkState_Move(this);
            _state_select = new InkState_Select(this);
            _state_rotate = new InkState_Rotate(this);
            _state_zoom = new InkState_Zoom(this);
            _state_inserttext = new InkState_InsertText(this);
            _state_inkplay = new InkState_Inkplay(this);
            _state_Gesture = new InkState_Gesture(this);
            _state_DrawArrow = new InkState_DrawArrow(this);
            _state_VideoPlay = new InkState_VideoPlay(this);
            _state_AutoMove = new InkState_AutoMove(this);
            _state_DrawGraphic = new InkState_DrawGraphic(this);
            _state_None = new InkState_None(this);
            _state_DrawStrokeInGraphic = new InkState_DrawStrokeInGraphic(this);
            _state_Summarization = new InkState_Summarization(this);
            _state_MergeSummarization = new InkState_MergeSummarization(this);
            _state_HyperLinkSummarization = new InkState_HyperLinkSummarization(this);
            _state_AddKeyFrameAnnotation = new InkState_AddKeyFrameAnnotation(this);
            _state_TapestrySummarization = new InkState_TapestrySummarization(this);
            _state_DoubleSpiralSummarization = new InkState_DoubleSpiralSummarization(this);
            _state = _state_ink;//默认state

            _inkCanvas.AddHandler(InkCanvas.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this._inkCanvas_MouseDown), true);
            _inkCanvas.AddHandler(InkCanvas.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this._inkCanvas_MouseUp), true);
            //初始化事件
            _inkCanvas.MouseMove += new MouseEventHandler(_inkCanvas_MouseMove);

            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Reset();
            //sw.Start();
            //LoadTemplates loadTemplates = new LoadTemplates();
            //sw.Stop();
            //Console.WriteLine("loadTemplates总需要时间：" + sw.ElapsedMilliseconds + "ms");
        }

        #region MouseEvent
        void _inkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _state._presenter_MouseLeftButtonUp(sender, e);
        }

        void _inkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            _state._presenter_MouseMove(sender, e);
        }

        void _inkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _state._presenter_MouseLeftButtonDown(sender, e);
        }
        #endregion

        #region inkoperation

        /// <summary>
        /// 增加一个笔画，添加到sketch中
        /// wpf中Stroke在InkCanvas中的添加不需要程序员来做
        /// </summary>
        /// <param name="stroke"></param>
        public void AddStroke(MyStroke stroke)
        {
            if (videoPath != "")
            {
                stroke.VideoPath = videoPath;
            }
            if (this._inkCanvas.Strokes.Contains(stroke.Stroke))
                _sketch.AddStroke(stroke);
            else
            {
                _inkCanvas.Strokes.Add(stroke.Stroke);
                _sketch.AddStroke(stroke);
            }
        }

        /// <summary>
        /// 在sketch中删除掉mystroke
        /// </summary>
        /// <param name="stroke"></param>
        public void RemoveStroke(MyStroke stroke)
        {
            if (stroke != null)
            {
                if (_inkCanvas.Strokes.Contains(stroke.Stroke))
                {
                    _inkCanvas.Strokes.Remove(stroke.Stroke);
                }

            }
            _sketch.RemoveStroke(stroke);
        }

        /// <summary>
        /// 增加一个image
        /// </summary>
        /// <param name="image"></param>
        public void AddImage(MyImage image)
        {
            if (videoPath != "")
            {
                image.VideoPath = videoPath;
            }
            int index = Sketch.Images.Count;
            _inkCanvas.Children.Insert(index, image.Image);
            image.Bound.Visibility = Visibility.Collapsed;
            _inkCanvas.Children.Add(image.Bound);
            Sketch.AddImage(image);
        }

        /// <summary>
        /// 删除graphic
        /// </summary>
        /// <param name="image"></param>
        public void RemoveMyGraphic(MyGraphic myGraphic)
        {
            _inkCanvas.Children.Remove(myGraphic.Shape);
            foreach (Stroke s in myGraphic.Strokes)
            {
                if (_inkCanvas.Strokes.IndexOf(s) != -1)
                {
                    _inkCanvas.Strokes.Remove(s);
                }
            }
            foreach (Stroke s in myGraphic.textStrokeCollection)
            {
                if (_inkCanvas.Strokes.IndexOf(s) != -1)
                {
                    _inkCanvas.Strokes.Remove(s);
                }
            }
            if (myGraphic.PentagramStrokes != null)
            {
                _inkCanvas.Strokes.Remove(myGraphic.PentagramStrokes);
            }
            GraphicMathTool.getInstance().searchExistRelationAndRemove(myGraphic, Sketch.MyGraphics, this);
            Sketch.RemoveMyGraphic(myGraphic);
        }
        /// <summary>
        /// 增加一个graphic
        /// </summary>
        /// <param name="image"></param>
        public void AddMyGraphic(MyGraphic myGraphic)
        {
            foreach (Stroke s in myGraphic.Strokes)
            {
                if (_inkCanvas.Strokes.IndexOf(s) == -1)
                {
                    _inkCanvas.Strokes.Add(s);
                }
            }
            foreach (Stroke s in myGraphic.textStrokeCollection)
            {
                if (_inkCanvas.Strokes.IndexOf(s) == -1)
                {
                    _inkCanvas.Strokes.Add(s);
                }
            }
            if (myGraphic.PentagramStrokes != null)
            {
                _inkCanvas.Strokes.Add(myGraphic.PentagramStrokes);
            }
            _inkCanvas.Children.Add(myGraphic.Shape);
            Sketch.AddMyGraphic(myGraphic);
        }
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="image"></param>
        public void RemoveImage(MyImage image)
        {
            _inkCanvas.Children.Remove(image.Image);
            _inkCanvas.Children.Remove(image.Bound);
            Sketch.RemoveImage(image);
        }
        /// <summary>
        /// 增加一个button
        /// </summary>
        /// <param name="button"></param>
        public void AddButton(MyButton myButton)
        {
            myButton.IsDeleted = false;
            if (_inkCanvas.Children.IndexOf(myButton.Button) == -1)
            {
                _inkCanvas.Children.Add(myButton.Button);
            }
            if (_inkCanvas.Children.IndexOf(myButton.TextBoxTime) == -1)
            {
                _inkCanvas.Children.Add(myButton.TextBoxTime);
            }
            int i = 0;//为了找到最后一个符合条件的NextMyArrow
            int j = 0;//记录最后一个符合条件的NextMyArrow的下标
            foreach (MyArrow myArrow in Sketch.MyArrows)
            {

                i++;
                if (myArrow.NextMyButton == myButton && myArrow.PreMyButton.IsDeleted == false && myArrow.IsDeleted == true && _inkCanvas.Children.IndexOf(myArrow.Arrow) == -1)
                {
                    _inkCanvas.Children.Add(myArrow.Arrow);
                    myArrow.IsDeleted = false;
                }
                if (myArrow.PreMyButton == myButton && myArrow.NextMyButton.IsDeleted == false && myArrow.IsDeleted == true)
                {
                    j = i;
                }
            }

            if (j != 0 && Sketch.MyArrows.Count >= j && _inkCanvas.Children.IndexOf(Sketch.MyArrows[j - 1].Arrow) == -1)
            {
                Sketch.MyArrows[j - 1].IsDeleted = false;
                _inkCanvas.Children.Add(Sketch.MyArrows[j - 1].Arrow);

            }
            myButton.updateRectangles(_inkCanvas, this);
            if (Sketch.MyButtons.IndexOf(myButton) == -1)
            {
                Sketch.AddButton(myButton);
            }


        }

        /// <summary>
        /// 删除button
        /// </summary>
        /// <param name="myButton"></param>
        public void RemoveButton(MyButton myButton)
        {
            myButton.IsDeleted = true;
            foreach (MyArrow myArrow in Sketch.MyArrows)
            {
                if (myArrow.IsDeleted == false && (myArrow.NextMyButton == myButton || myArrow.PreMyButton == myButton))
                {
                    myArrow.IsDeleted = true;
                    _inkCanvas.Children.Remove(myArrow.Arrow);
                }
            }

            if (myButton.IsGlobal)
            {
                foreach (MySmallRectangle msr in Sketch.mySmallRectangles)
                {
                    if (msr.ParentMyButton == myButton)
                    {
                        _inkCanvas.Children.Remove(msr.Rectangle);
                    }
                }
            }
            else
            {
                foreach (MyButton mb in Sketch.MyButtons)
                {
                    if (mb.IsGlobal && mb.VideoPath == myButton.VideoPath)
                    {
                        mb.updateRectangles(_inkCanvas, this);
                    }
                }
            }
            _inkCanvas.Children.Remove(myButton.Button);
            _inkCanvas.Children.Remove(myButton.TextBoxTime);
            _inkCanvas.Children.Remove(myButton.RBorder);


        }

        /// <summary>
        /// 增加一个Arrow
        /// </summary>
        /// <param name="button"></param>
        public void AddArrow(MyArrow myArrow)
        {
            if (_inkCanvas.Children.IndexOf(myArrow.Arrow) == -1)
            {
                myArrow.Arrow.Stroke = System.Windows.Media.Brushes.Black;
                _inkCanvas.Children.Add(myArrow.Arrow);
                myArrow.IsDeleted = false;
                Sketch.AddArrow(myArrow);
            }
        }

        /// <summary>
        /// 删除Arrow
        /// </summary>
        /// <param name="myButton"></param>
        public void RemoveArrow(MyArrow myArrow)
        {
            if (_inkCanvas.Children.IndexOf(myArrow.Arrow) > -1)
            {
                _inkCanvas.Children.Remove(myArrow.Arrow);
                myArrow.IsDeleted = true;
            }

        }

        /// <summary>
        /// 增加一个文本
        /// </summary>
        /// <param name="image"></param>
        public void AddText(MyRichTextBox _myRichTextBox)
        {
            if (videoPath != "")
            {
                _myRichTextBox.VideoPath = videoPath;
            }
            if (_inkCanvas.Children.IndexOf(_myRichTextBox.RichTextBox) == -1)
            {
                int index = Sketch.Images.Count;
                _inkCanvas.Children.Insert(index, _myRichTextBox.RichTextBox);
                Sketch.AddText(_myRichTextBox);
            }
        }

        /// <summary>
        /// 删除文本
        /// </summary>
        /// <param name="image"></param>
        public void RemoveText(MyRichTextBox _myRichTextBox)
        {
            _inkCanvas.Children.Remove(_myRichTextBox.RichTextBox);
            Sketch.RemoveText(_myRichTextBox);
        }
        #endregion

        #region 模式

        //笔迹编辑模式
        public InkMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case InkMode.Ink:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        _inkCanvas.Cursor = Cursors.Pen;
                        clearSelectedElement();
                        _state = _state_ink;
                        break;
                    case InkMode.StrokeErase:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        Cursor StrokeEraserCursor = new Cursor(GlobalValues.FilesPath + "/WPFInk/WPFInk/src/cursor/StrokeEraser.cur");
                        //Cursor StrokeEraserCursor = new Cursor("pack://siteoforigin:,,,/cursor/StrokeEraser.cur");
                        _inkCanvas.Cursor = StrokeEraserCursor;
                        clearSelectedElement();
                        _state = _state_erase;
                        break;
                    case InkMode.PointErase:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        Cursor PointEraserCursor = new Cursor(GlobalValues.FilesPath + "/WPFInk/WPFInk/src/cursor/PointEraser.cur");
                        _inkCanvas.Cursor = PointEraserCursor;
                        clearSelectedElement();
                        _state = _state_pointerase;
                        break;
                    case InkMode.Select:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Select;
                        Cursor SelectCursor = new Cursor(GlobalValues.FilesPath + "/WPFInk/WPFInk/src/cursor/Select.cur");

                        _inkCanvas.Cursor = SelectCursor;
                        _state = _state_select;
                        break;
                    case InkMode.InsertText:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        Cursor TextCursor = Cursors.Cross;
                        _inkCanvas.Cursor = TextCursor;
                        clearSelectedElement();
                        _state = _state_inserttext;
                        break;
                    case InkMode.Rotate:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        Cursor RotateCursor = new Cursor(GlobalValues.FilesPath + "/WPFInk/WPFInk/src/cursor/Rotate.cur");

                        _inkCanvas.Cursor = RotateCursor;
                        if (this.selectedStrokes.Count > 0 || this.SelectedImages.Count > 0 || this.selectButtons.Count > 0)
                        {
                            _state = _state_rotate;
                        }
                        break;
                    case InkMode.Zoom:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        Cursor ZoomCursor = new Cursor(GlobalValues.FilesPath + "/WPFInk/WPFInk/src/cursor/Zoom.cur");
                        _inkCanvas.Cursor = ZoomCursor;
                        _state = _state_zoom;
                        break;
                    case InkMode.Move:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;

                        _inkCanvas.Cursor = Cursors.SizeAll;
                        _state = _state_move;
                        break;
                    case InkMode.InkPlay:
                        _inkCanvas.Cursor = Cursors.Wait;
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        clearSelectedElement();
                        _state = _state_inkplay;
                        break;
                    case InkMode.GestureOnly:
                        _inkCanvas.Cursor = Cursors.Hand;
                        //clearSelectedElement();
                        _inkCanvas.EditingMode = InkCanvasEditingMode.GestureOnly;
                        _state = _state_Gesture;
                        break;
                    case InkMode.DrawArrow:
                        _inkCanvas.Cursor = Cursors.Hand;
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        clearSelectedElement();
                        _state = _state_DrawArrow;
                        break;
                    case InkMode.VideoPlay:
                        _inkCanvas.Cursor = Cursors.Arrow;
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        clearSelectedElement();
                        _state = _state_VideoPlay;
                        break;
                    case InkMode.AutoMove:
                        _inkCanvas.Cursor = Cursors.Arrow;
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        clearSelectedElement();
                        _state = _state_AutoMove;
                        break;
                    case InkMode.DrawGraphic:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        _inkCanvas.Cursor = Cursors.Pen;
                        clearSelectedElement();
                        _state = _state_DrawGraphic;
                        break;

                    case InkMode.DrawStrokeInGraphic:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        _inkCanvas.Cursor = Cursors.Pen;
                        clearSelectedElement();
                        _state = _state_DrawStrokeInGraphic;
                        break;

                    case InkMode.VideoSummarization:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        DefaultDrawingAttributes.Color = System.Windows.Media.Colors.Red;
                        DefaultDrawingAttributes.Width = 5;
                        DefaultDrawingAttributes.Height = 5;
                        _inkCanvas.Cursor = Cursors.Cross;
                        clearSelectedElement();
                        //VideoSummarizationTool.locateMediaPlayer(_mainPage.VideoSummarizationControl.mediaPlayer, videoSummarization.KeyFrames[0]);
                        //_state_Summarization.VideoPlayTimer.Start();
                        _state_Summarization.videoSource = videoSummarization.KeyFrames[0].VideoName;
                        _state_Summarization.VideoSummarization = videoSummarization;
                        List<string> timeTotal = ConvertClass.getInstance().MsToHMS(_mainPage.VideoSummarizationControl._timeBar.Maximum);
                        _state_Summarization.TimeTotalString = timeTotal[0] + ":" + timeTotal[1] + ":" + timeTotal[2];
                        _state = _state_Summarization;
                        break;

                    case InkMode.MergeSummarization:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        DefaultDrawingAttributes.Color = System.Windows.Media.Colors.Red;
                        DefaultDrawingAttributes.Width = 5;
                        DefaultDrawingAttributes.Height = 5;
                        _inkCanvas.Cursor = Cursors.Cross;
                        clearSelectedElement();
                        _state = _state_MergeSummarization;
                        break;
                    case InkMode.HyperLinkSummarization:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        DefaultDrawingAttributes.Color = System.Windows.Media.Colors.Red;
                        DefaultDrawingAttributes.Width = 3;
                        DefaultDrawingAttributes.Height = 3;
                        _inkCanvas.Cursor = Cursors.Cross;
                        //clearSelectedElement();
                        _state = _state_HyperLinkSummarization;
                        break;
                    case InkMode.AddKeyFrameAnnotation:
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        DefaultDrawingAttributes.Color = System.Windows.Media.Colors.Red;
                        DefaultDrawingAttributes.Width = 3;
                        DefaultDrawingAttributes.Height = 3;
                        _inkCanvas.Cursor = Cursors.Hand;
                        _state_AddKeyFrameAnnotation.VideoSummarization = videoSummarization;
                        _state = _state_AddKeyFrameAnnotation;
                        break;
                    case InkMode.TapestrySummarization:
                        _inkCanvas.Cursor = Cursors.Hand;
                        _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                        DefaultDrawingAttributes.Color = System.Windows.Media.Colors.Transparent;
                        VideoSummarizationTool.locateMediaPlayer(_mainPage.VideoSummarizationControl.mediaPlayer, videoSummarization.KeyFrames[0]);
                        if (_mainPage.VideoSummarizationControl.mediaPlayer.NaturalDuration.HasTimeSpan)
                        {
                            _mainPage.VideoSummarizationControl._timeBar.Maximum = _mainPage.VideoSummarizationControl.mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                        }
                        List<string> timeTotal2 = ConvertClass.getInstance().MsToHMS(_mainPage.VideoSummarizationControl._timeBar.Maximum);
                        _state_Summarization.TimeTotalString = timeTotal2[0] + ":" + timeTotal2[1] + ":" + timeTotal2[2];
                        //_state_Summarization.VideoPlayTimer.Start();
                        _state_TapestrySummarization.videoSource = videoSummarization.KeyFrames[0].VideoName;
                        _state_TapestrySummarization.VideoSummarization = videoSummarization;
                        _state_TapestrySummarization.TapestrySummarization = (TapestrySummarization)videoSummarization;
                        _state_TapestrySummarization.Timebar = _state_TapestrySummarization.TapestrySummarization.Timebar;
                        _state = _state_TapestrySummarization;
                        break;
                    case InkMode.DoubleSpiralSummarization:
                        _state_DoubleSpiralSummarization.videoSource = videoSummarization.KeyFrames[0].VideoName;
                        _state_DoubleSpiralSummarization.VideoSummarization = videoSummarization;
                        _state = _state_DoubleSpiralSummarization;
                        break;
                    case InkMode.None:
                        _inkCanvas.Cursor = Cursors.Arrow;
                        _inkCanvas.EditingMode = InkCanvasEditingMode.None;
                        _mainPage.OperatePieMenu.Visibility = Visibility.Collapsed;
                        _state = _state_None;
                        break;
                }
            }
        }
        #endregion
        #region 事件和方法
        private void clearSelectedElement()
        {
            selectedStrokes.Clear();
            SelectedImages.Clear();

            _mainPage.OperatePieMenu.Visibility = Visibility.Collapsed;

            if (SelectedImages.Count > 0)
            {
                foreach (MyImage myImage in SelectedImages)
                {
                    myImage.Bound.Visibility = Visibility.Collapsed;
                }
            }
            if (SelectButtons.Count > 0)
            {
                foreach (MyButton myButton in SelectButtons)
                {
                    myButton.TextBoxTime.Background = null;
                }
            }
            SelectButtons.Clear();
        }

        /// <summary>
        /// 删除与笔迹strokes相交的图形
        /// </summary>
        /// <param name="strokes"></param>
        public void removeHitMyGraphics(StrokeCollection strokes)
        {
            List<MyGraphic> myGraphics = new List<MyGraphic>();

            foreach (MyGraphic myGraphic in Sketch.MyGraphics)
            {
                if (MathTool.getInstance().isHitRects(myGraphic.Strokes.GetBounds(), strokes.GetBounds()))
                {
                    myGraphics.Add(myGraphic);
                }
            }
            foreach (MyGraphic myGraphic in myGraphics)
            {
                GraphicMathTool.getInstance().deleteMyGraphic(myGraphic, this);

            }
        }
        /// <summary>
        /// 删除与笔迹strokes相交的文本
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        public void removeHitTextBoxes(StrokeCollection myGraphicStrokes)
        {
            List<MyRichTextBox> myRichTextBoxs = new List<MyRichTextBox>();
            foreach (MyRichTextBox myRichTextBox in Sketch.MyRichTextBoxs)
            {
                Rect rectMyRichTextBox = new Rect(new Point(myRichTextBox.RichTextBox.Margin.Left, myRichTextBox.RichTextBox.Margin.Top),
                                         new Point(myRichTextBox.RichTextBox.Margin.Left + myRichTextBox.RichTextBox.Width,
                                             myRichTextBox.RichTextBox.Margin.Top + myRichTextBox.RichTextBox.Height));
                if (MathTool.getInstance().isHitRects(myGraphicStrokes.GetBounds(), rectMyRichTextBox) == true)
                {
                    myRichTextBoxs.Add(myRichTextBox);
                }
            }
            foreach (MyRichTextBox myRichTextBox in myRichTextBoxs)
            {
                Command dtc = new DeleteTextCommand(this, myRichTextBox);
                dtc.execute();
                CommandStack.Push(dtc);
            }
        }
        /// <summary>
        /// 删除与笔迹strokes相交的图像
        /// </summary>
        /// <param name="strokes"></param>
        public void removeHitImages(StrokeCollection strokes)
        {
            List<MyImage> myImages = new List<MyImage>();
            foreach (MyImage myImage in Sketch.Images)
            {
                Rect rectMyImage = new Rect(new Point(myImage.Left, myImage.Top),
                                            new Point(myImage.Left + myImage.Width, myImage.Top + myImage.Height));
                if (MathTool.getInstance().isHitRects(strokes.GetBounds(), rectMyImage) == true)
                {
                    myImages.Add(myImage);
                }
            }
            foreach (MyImage myImage in myImages)
            {
                Command dbc = new DeleteImageCommand(this, myImage);
                dbc.execute();
                CommandStack.Push(dbc);
            }
        }
        /// <summary>
        /// 删除与笔迹strokes相交的笔迹
        /// </summary>
        /// <param name="strokes"></param>
        public void removeHitStrokes(StrokeCollection strokes)
        {
            StrokeCollection hitStrokes = new StrokeCollection();
            foreach (MyStroke myStroke in Sketch.MyStrokes)
            {
                if (MathTool.getInstance().isHitRects(myStroke.Stroke.GetBounds(), strokes.GetBounds()))
                {
                    hitStrokes.Add(myStroke.Stroke);
                }
            }
            //删除笔迹
            if (hitStrokes.Count > 0)
            {
                List<MyStroke> myStrokes = new List<MyStroke>();
                foreach (Stroke s in hitStrokes)
                {
                    foreach (MyStroke myStroke in Sketch.MyStrokes)
                    {
                        if (myStroke.Stroke == s)
                        {
                            myStrokes.Add(myStroke);
                        }
                    }
                }
                foreach (MyStroke ms in myStrokes)
                {
                    DeleteStrokeCommand dsc = new DeleteStrokeCommand(this, ms);
                    dsc.execute();
                    CommandStack.Push(dsc);
                }
            }
        }
        /// <summary>
        /// 预加载视频关键帧信息，用于视频摘要
        /// </summary>
        public void addImages()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            if (GlobalValues.templates.Count == 0)
                //if (GlobalValues.isShowHalf && GlobalValues.templates.Count == 0)
            {
                LoadTemplates loadTemplates = new LoadTemplates();
                
                if (GlobalValues.templates.Count == 0)
                {
                    LoadTemplatesFromCard();
                }
            }
            sw.Stop();
            Console.WriteLine("LoadTemplates总需要时间：" + sw.ElapsedMilliseconds + "ms");
            List<string> videoNameList = new List<string>();
            //videoNameList.Add(GlobalValues.videoName);
            videoNameList.Add("麋鹿王");
            //videoNameList.Add("小熊维尼与跳跳虎");
            videoNameList.Add("憨豆先生1");
            videoNameList.Add("绝望主妇1");
            //videoNameList.Add("绝望主妇2");
            //videoNameList.Add("大雄兔");
           // videoNameList.Add("大头儿子小头爸爸");
            for (int i = 0; i < videoNameList.Count; i++)
            {
                loadKeyFrames(videoNameList[i]);
            }
        }
        /// <summary>
        /// 预载入模板集合,从硬盘文件读取
        /// </summary>
        private void LoadTemplatesFromCard()
        {
            string path = @"resource\template.txt";
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        List<byte> template = new List<byte>();
                        string[] line = s.Split(' ');
                        for (int i = 0; i < line.Length - 1; i++)
                        {
                            template.Add(Byte.Parse(line[i]));
                        }
                        Global.GlobalValues.templates.Add(template);
                    }
                }
            }
        }
        private void loadKeyFrames(string videoName)
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
            List<KeyFrame> keyFrames1 = new List<KeyFrame>();
            List<KeyFrame> keyFramesMerge = new List<KeyFrame>();
            List<KeyFrame> keyFramesLink = new List<KeyFrame>();
            List<KeyFrame> keyFrames2 = new List<KeyFrame>();
            List<KeyFrame> keyFrames3 = new List<KeyFrame>();
            for (int j = 1; j <= videoTimeList.Count; j++)
            {
                KeyFrame keyFrame = new KeyFrame(GlobalValues.FilesPath + "/WPFInkResource/" + videoName + ".avi", @"resource\" + videoName + @"\" + j + ".png", videoTimeList[j - 1]);
                keyFrames1.Add(keyFrame);
            }
            for (int j = 1; j <= videoTimeList.Count *0.6; j++)
            {
                KeyFrame keyFrame = new KeyFrame(GlobalValues.FilesPath + "/WPFInkResource/" + videoName + ".avi", @"resource\" + videoName + @"\" + j + ".png", videoTimeList[j - 1]);
                keyFramesMerge.Add(keyFrame);
            }
            for (int j = 1; j <= videoTimeList.Count *0.58; j++)
            {
                KeyFrame keyFrame = new KeyFrame(GlobalValues.FilesPath + "/WPFInkResource/" + videoName + ".avi", @"resource\" + videoName + @"\" + j + ".png", videoTimeList[j - 1]);
                keyFramesLink.Add(keyFrame);
            }
            for (int j = 1; j <= videoTimeList.Count; j++)
            {
                KeyFrame keyFrame = new KeyFrame(GlobalValues.FilesPath + "/WPFInkResource/" + videoName + ".avi", @"resource\" + videoName + @"_source\" + j + ".png", videoTimeList[j - 1]);
                keyFrames2.Add(keyFrame);
            }
            for (int j = 1; j <= videoTimeList.Count; j++)
            {
                KeyFrame keyFrame = new KeyFrame(GlobalValues.FilesPath + "/WPFInkResource/" + videoName + ".avi", @"resource\" + videoName + @"_source\" + j + ".png", videoTimeList[j - 1]);
                keyFrames3.Add(keyFrame);
            }
            if (GlobalValues.isShowHalf&& keyFrames1.Count % 2 == 1)
            {
                keyFrames1.RemoveAt(keyFrames1.Count - 1);
            }
            keyFramesSpiralMerge.Add(keyFramesMerge);
            keyFramesSpiralLink.Add(keyFramesLink);
            keyFramesSpiral.Add(keyFrames1);
            keyFramesTile.Add(keyFrames2);
            keyFramesTapestry.Add(keyFrames3);
        }
        #endregion
        #region Property
        //设置笔迹画板
        public InkCanvas InkCanvas
        {
            get { return _inkCanvas; }
            set
            {
                _inkCanvas = value;
            }
        }

        //提取画板选中的笔迹
        public List<MyStroke> SelectedStrokes
        {
            get { return selectedStrokes; }
            set { selectedStrokes = value; }
        }
        //选中区域的中心点
        public StylusPoint CenterSelect
        {
            get { return _centerSelect; }
            set { _centerSelect = value; }
        }
        //选中区域的边框
        public Rectangle BoundSelect
        {
            get { return _boundSelect; }
            set { _boundSelect = value; }
        }
        //提取画板中选中的Image
        public List<MyImage> SelectedImages
        {
            get { return selectImages; }
            set { selectImages = value; }
        }

        //提取画板中选中的Button
        public List<MyButton> SelectButtons
        {
            get { return selectButtons; }
            set { selectButtons = value; }
        }

        //提取画板中选中的Button的边框
        public List<Rectangle> SelectedButtonsBound
        {
            get { return selectedButtonsBound; }
            set { selectedButtonsBound = value; }
        }

        //提取画板中选中的textBox
        public RichTextBox SelectedRichTextBox
        {
            get { return selectedRichTextBox; }
            set { selectedRichTextBox = value; }
        }

        //选中的MyRichTextBox
        public List<MyRichTextBox> SelectedMyRichTextBoxs
        {
            get { return selectedMyRichTextBoxs; }
            set { selectedMyRichTextBoxs = value; }
        }
        //选中的MyGraphic
        public List<MyGraphic> SelectedMyGraphics
        {
            get { return selectedMyGraphics; }
            set { selectedMyGraphics = value; }
        }

        /// <summary>
        /// 当前的drawingAttribute
        /// </summary>
        public DrawingAttributes DefaultDrawingAttributes
        {
            get { return _inkCanvas.DefaultDrawingAttributes; }
            set { _inkCanvas.DefaultDrawingAttributes = value; }
        }

        /// <summary>
        /// 已经完成的命令栈
        /// </summary>
        public Stack<Command> CommandStack
        {
            get { return _commandStack; }
            set { _commandStack = value; }
        }

        /// <summary>
        /// 已经撤销的命令栈
        /// </summary>
        public Stack<Command> UndoCommandStack
        {
            get { return _undoCommandStack; }
            set { _undoCommandStack = value; }
        }

        /// <summary>
        /// 当前的sketch
        /// </summary>
        public Sketch Sketch
        {
            get { return _sketch; }
        }


        public bool IsAutoMove
        {
            get { return isAutoMove; }
            set { isAutoMove = value; }
        }

        public string VideoPath
        {
            get { return videoPath; }
            set { videoPath = value; }
        }

        //public List<KeyFrame> KeyFrames
        //{
        //    get { return keyFrames; }
        //    set { keyFrames = value; }
        //}


        //public List<KeyFrame> KeyFrames1
        //{
        //    get { return keyFrames1; }
        //    set { keyFrames1 = value; }
        //}
        //public List<KeyFrame> KeyFrames2
        //{
        //    get { return keyFrames2; }
        //    set { keyFrames2 = value; }
        //}

        //public List<KeyFrame> KeyFrames3
        //{
        //    get { return keyFrames3; }
        //    set { keyFrames3 = value; }
        //}


        //public SpiralSummarization SpiralSummarization
        //{
        //    get { return spiralSummarization; }
        //    set { spiralSummarization = value; }
        //}

        public List<KeyFrame> SelectKeyFrames
        {
            get { return selectKeyFrames; }
            set { selectKeyFrames = value; }
        }


        public int DefaultSummarizationNum
        {
            get { return defaultSummarizationNum; }
            set { defaultSummarizationNum = value; }
        }
        //public List<string> KeyFrameNames
        //{
        //    get { return keyFrameNames; }
        //    set { keyFrameNames = value; }
        //}

        public List<Image> SelectKeyFrameImages
        {
            get { return selectKeyFrameImages; }
            set { selectKeyFrameImages = value; }
        }

        //public List<KeyFrame> TileKeyFrames
        //{
        //    get { return tileKeyFrames; }
        //    set { tileKeyFrames = value; }
        //}

        //public TileSummarization TileSummarization
        //{
        //    get { return tileSummarization; }
        //    set { tileSummarization = value; }
        //}

        public InkState_Ink State_Ink
        {
            get { return _state_ink; }
        }

        public InkState_Summarization InkState_Summarization
        {
            get { return _state_Summarization; }
            set { _state_Summarization = value; }
        }


        public InkState_TapestrySummarization InkState_TapestrySummarization
        {
            get { return _state_TapestrySummarization; }
            set { _state_TapestrySummarization = value; }
        }

        public List<KeyFrame> HyperLinkKeyFrames
        {
            get { return hyperLinkKeyFrames; }
            set { hyperLinkKeyFrames = value; }

        }
        /// <summary>
        /// 螺旋摘要是否MouseMove事件会触发显示完整关键帧
        /// </summary>
        public bool IsShowUnbrokenKeyFrame
        {
            get { return isShowUnbrokenKeyFrame; }
            set { isShowUnbrokenKeyFrame = value; }
        }

        public KeyFrameAnnotation KeyFrameAnnotation
        {
            get { return _keyFrameAnnotation; }
            set { _keyFrameAnnotation = value; }
        }
        public VideoSummarization VideoSummarization
        {
            get { return videoSummarization; }
            set { videoSummarization = value; }
        }

        public KeyFramesAnnotation KeyFramesAnnotation
        {
            get { return keyFramesAnnotation; }
            set { keyFramesAnnotation = value; }
        }

        //public List<KeyFrame> KeyFramesMerge1
        //{
        //    get { return keyFramesMerge1; }
        //    set { keyFramesMerge1 = value; }
        //}
        //public List<KeyFrame> KeyFramesMerge2
        //{
        //    get { return keyFramesMerge2; }
        //    set { keyFramesMerge2 = value; }
        //}
        public List<List<KeyFrame>> KeyFramesSpiral
        {
            get { return keyFramesSpiral; }
            set { keyFramesSpiral = value; }
        }
        public List<List<KeyFrame>> KeyFramesTapestry
        {
            get { return keyFramesTapestry; }
            set { keyFramesTapestry = value; }
        }

        public List<List<KeyFrame>> KeyFramesTile
        {
            get { return keyFramesTile; }
            set { keyFramesTile = value; }
        }
        public bool IsShowRedPoint
        {
            get { return isShowRedPoint; }
            set { isShowRedPoint = value; }
        }

        public int VideoNum
        {
            get { return videoNum; }
            set { videoNum = value; }
        }
        #endregion
        #region 
        //以下为笔迹编辑状态属性
        ///以下为ink的编辑状态
        public InkState _state;
        private InkState_Ink _state_ink;
        private InkState_Erase _state_erase;
        private InkState_PointErase _state_pointerase;
        private InkState_Select _state_select;
        private InkState_Move _state_move;
        private InkState_Rotate _state_rotate;
        private InkState_Zoom _state_zoom;
        private InkState_InsertText _state_inserttext;
        private InkState_Inkplay _state_inkplay;
        private InkState_Gesture _state_Gesture;
        private InkState_DrawArrow _state_DrawArrow;
        private InkState_VideoPlay _state_VideoPlay;
        private InkState_AutoMove _state_AutoMove;
        private InkState_DrawGraphic _state_DrawGraphic;
        private InkState_None _state_None;
        private InkState_DrawStrokeInGraphic _state_DrawStrokeInGraphic;
        private InkState_Summarization _state_Summarization;
        private InkState_MergeSummarization _state_MergeSummarization;
        private InkState_HyperLinkSummarization _state_HyperLinkSummarization;
        private InkState_AddKeyFrameAnnotation _state_AddKeyFrameAnnotation;
        private InkState_TapestrySummarization _state_TapestrySummarization;
        private InkState_DoubleSpiralSummarization _state_DoubleSpiralSummarization;

        private InkCanvas _inkCanvas = null; //ink 画板
        public InkFrame _mainPage = null;//ink 画板界面
        private InkMode _mode = InkMode.Ink;//模式

        public Sketch _sketch = new Sketch();
        private Stack<Command> _commandStack = new Stack<Command>();
        private Stack<Command> _undoCommandStack = new Stack<Command>();
        private List<MyStroke> selectedStrokes = new List<MyStroke>();
        private StylusPoint _centerSelect;
        private Rectangle _boundSelect;
        private List<MyImage> selectImages = new List<MyImage>();
        private List<System.Drawing.Bitmap> sketchImages = new List<System.Drawing.Bitmap>();  //关键帧草图    
        private List<Point> sketchImagesPositions = new List<Point>();  //关键帧草图
        private RichTextBox selectedRichTextBox = null;
        private List<MyButton> selectButtons = new List<MyButton>();
        private List<Rectangle> selectedButtonsBound = new List<Rectangle>();
        private List<MyRichTextBox> selectedMyRichTextBoxs = new List<MyRichTextBox>();
        private List<MyGraphic> selectedMyGraphics = new List<MyGraphic>();

        //螺旋式摘要变量
        private List<List<KeyFrame>> keyFramesSpiral = new List<List<KeyFrame>>();//关键帧列表,用作螺旋摘要
        public List<List<KeyFrame>> keyFramesSpiralMerge = new List<List<KeyFrame>>();//关键帧列表,用作螺旋摘要
        public List<List<KeyFrame>> keyFramesSpiralLink = new List<List<KeyFrame>>();//关键帧列表,用作螺旋摘要
        private List<List<KeyFrame>> keyFramesTapestry = new List<List<KeyFrame>>();//关键帧列表，用作织锦摘要        
        private List<List<KeyFrame>> keyFramesTile = new List<List<KeyFrame>>();//关键帧列表
        private int videoNum = 0;//当前视频编号
        //private List<KeyFrame> keyFrames3 = new List<KeyFrame>();//关键帧列表
        //private List<KeyFrame> keyFramesMerge1 = new List<KeyFrame>();//关键帧列表
        //private List<KeyFrame> keyFramesMerge2 = new List<KeyFrame>();//关键帧列表
        //private List<KeyFrame> tileKeyFrames = new List<KeyFrame>();//平铺关键帧列表
        private VideoSummarization videoSummarization;
        private List<KeyFrame> selectKeyFrames = new List<KeyFrame>();//选中的关键帧
        private List<KeyFrame> hyperLinkKeyFrames = new List<KeyFrame>();//有超链接的关键帧
        private List<Image> selectKeyFrameImages = new List<Image>();//选中的关键帧图片
        private int defaultSummarizationNum = 0;//摘要类型，0代表螺旋摘要，1代表平铺摘要，2代表织锦摘要
        private bool isShowUnbrokenKeyFrame = true;//螺旋摘要是否MouseMove事件会触发显示完整关键帧
        private KeyFrameAnnotation _keyFrameAnnotation;
        private KeyFramesAnnotation keyFramesAnnotation;
        private bool isShowRedPoint = false;//是否显示摘要中播放进度点



        private bool isAutoMove = false;
        public MyTimer timer = MyTimer.getInstance();
        public bool IsPlaying = false;
        private string videoPath = "";

        #endregion


    }
}
