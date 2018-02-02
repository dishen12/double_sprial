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
using System.Windows.Ink;
using WPFInk.ink;
using WPFInk.WindowResize;
using WPFInk.videoSummarization;
using System.Linq;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for KeyFrameAnnotation.xaml
	/// </summary>
	public partial class KeyFrameAnnotation : UserControl
	{
		private InkCollector _inkCollector;
        public KeyFramesAnnotation _keyFramesAnnotation;
        public InkCanvas InkCanvasAnnotation;
		public KeyFrameAnnotation()
		{
			this.InitializeComponent();
            this.MaxWidth = 400;
            this.MaxHeight = 400;
            this.MinHeight = 100;
            this.MinWidth = 100;
            //_controlPanel.setInkFrame(_inkFrame);
            _inkCollector = _inkFrame.InkCollector;
            InkCanvasAnnotation = _inkFrame._inkCanvas;
            _inkFrame.rectangle1.Opacity = 0.5;
            InkCanvasAnnotation.Opacity = 0.5;
            InkCanvasAnnotation.DefaultDrawingAttributes.Color = Colors.Red;
            InkCanvasAnnotation.DefaultDrawingAttributes.Width =
                InkCanvasAnnotation.DefaultDrawingAttributes.Height = 5;
            InkCanvasAnnotation.StrokeCollected += new InkCanvasStrokeCollectedEventHandler(InkCanvasAnnotation_StrokeCollected);
		}
		public void setInkCollector(InkCollector inkCollector)
		{
			this._inkCollector=inkCollector;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyFramesAnnotation"></param>
        /// <param name="isResizeable">是否可以进行缩放</param>
        public void setKeyFramesAnnotation(KeyFramesAnnotation keyFramesAnnotation, bool isResizeable)
        {
            this._keyFramesAnnotation = keyFramesAnnotation;
            if (isResizeable)
            {
                UserControlResizer wr = new UserControlResizer(this, 400, 400, 100, 100);
                wr.addResizerRight(rightSizeGrip);
                wr.addResizerLeft(leftSizeGrip);
                wr.addResizerUp(topSizeGrip);
                wr.addResizerDown(bottomSizeGrip);
                wr.addResizerLeftUp(topLeftSizeGrip);
                wr.addResizerRightUp(topRightSizeGrip);
                wr.addResizerLeftDown(bottomLeftSizeGrip);
                wr.addResizerRightDown(bottomRightSizeGrip);
                InkCanvasAnnotation.EditingMode = InkCanvasEditingMode.Ink;
            }
            else
            {
                InkCanvasAnnotation.EditingMode = InkCanvasEditingMode.None;

            }
        }

        /// <summary>
        /// 每条注释笔迹完成以后的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void InkCanvasAnnotation_StrokeCollected(object sender, System.Windows.Controls.InkCanvasStrokeCollectedEventArgs e)
		{
            if (null != _keyFramesAnnotation)
            {
                if (_keyFramesAnnotation.Strokes.Count == 0)
                {
                    if (_inkCollector.DefaultSummarizationNum == 0)
                    {
                        //关键帧索引
                        int index = _inkCollector.VideoSummarization.ShowKeyFrames.IndexOf(_inkCollector.SelectKeyFrames[0]);
                        ((SpiralSummarization)_inkCollector.VideoSummarization).AddPoints2ShowSpiral(index, Colors.Red, 0);
                    }
                    else if (_inkCollector.DefaultSummarizationNum == 1)
                    {
                        int index = _inkCollector.VideoSummarization.KeyFrames.IndexOf(_inkCollector.SelectKeyFrames[0]);
                        StylusPoint slPoint = _inkCollector.VideoSummarization.KeyPoints[index];

                        StylusPointCollection spc = new StylusPointCollection();
                        StylusPoint currPoint1 = new StylusPoint(slPoint.X - _inkCollector.VideoSummarization.ShowWidth / 2, slPoint.Y + _inkCollector.VideoSummarization.ShowHeight / 2 + 2);
                        StylusPoint currPoint2 = new StylusPoint(slPoint.X + _inkCollector.VideoSummarization.ShowWidth / 2, slPoint.Y + _inkCollector.VideoSummarization.ShowHeight / 2 + 2);
                        spc.Add(currPoint1);
                        spc.Add(currPoint2);
                        Stroke s = new Stroke(spc);
                        s.DrawingAttributes.Color = Colors.Red;
                        s.DrawingAttributes.Width = 3;
                        s.DrawingAttributes.Height = 3;
                        ((InkCanvas)(_inkCollector._mainPage._inkCanvas.Children[1])).Strokes.Add(s);
                    }
                }
                Stroke lastStroke = InkCanvasAnnotation.Strokes[InkCanvasAnnotation.Strokes.Count - 1].Clone();
                _keyFramesAnnotation.Strokes.Add(lastStroke);
            }
		}
        
        /// <summary>
        /// 关闭注释框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void BtnCloseBtn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            this.Visibility = Visibility.Collapsed;
            if (_keyFramesAnnotation != null)
            {
                //记录宽度和高度
                _keyFramesAnnotation.Width = this.Width;
                _keyFramesAnnotation.Height = this.Height;
                foreach (int index in _keyFramesAnnotation.relatedKeyFrameIndexes)
                {
                    KeyValuePair<Stroke, KeyFramesAnnotation> currPair = (from KeyValuePair<Stroke, KeyFramesAnnotation> anno in _inkCollector.VideoSummarization.ShowKeyFrames[index].Annotations
                                                                          where anno.Value == _keyFramesAnnotation
                                                                          select anno).First();
                    Stroke linkline = (Stroke)(currPair.Key);
                    _inkCollector._mainPage._inkCanvas.Strokes.Remove(linkline);
                }
            
                _inkCollector._mainPage.VideoSummarizationControl._keyframeControlPanel.Visibility = Visibility.Collapsed;
                //if (_inkCollector._mainPage.VideoSummarizationControl.IsSpiralScreen)
                //{
                //    _inkCollector._mainPage.VideoSummarizationControl.BtnSpiralScreenBack.Visibility = Visibility.Visible;
                //    _inkCollector._mainPage.VideoSummarizationControl.BtnSpiralScreen.Visibility = Visibility.Collapsed;
                //}
                //else
                //{
                //    _inkCollector._mainPage.VideoSummarizationControl.BtnSpiralScreenBack.Visibility = Visibility.Collapsed;
                //    _inkCollector._mainPage.VideoSummarizationControl.BtnSpiralScreen.Visibility = Visibility.Visible;
                //}
                _inkCollector.Mode = InkMode.VideoSummarization;
            }
		}

        /// <summary>
        /// 注释框宽度和高度发生变化时的操作
        /// </summary>
        public void WidthOrHeightChanged()
        {
            if (this.HorizontalAlignment == HorizontalAlignment.Left)
            {
                double left=_inkCollector.KeyFrameAnnotation.Margin.Left + _inkCollector.KeyFrameAnnotation.Width / 2;
                double top=_inkCollector.KeyFrameAnnotation.Margin.Top + _inkCollector.KeyFrameAnnotation.Height / 2;
                foreach (int index in _keyFramesAnnotation.relatedKeyFrameIndexes)
                {
                    KeyValuePair<Stroke, KeyFramesAnnotation> currPair = (from KeyValuePair<Stroke, KeyFramesAnnotation> anno in _inkCollector.VideoSummarization.ShowKeyFrames[index].Annotations
                                                                          where anno.Value == _keyFramesAnnotation
                                                                          select anno).First();
                    Stroke linkline = (Stroke)(currPair.Key);
                    
                    linkline.StylusPoints[1] = new StylusPoint(left, top);
                    //Console.WriteLine(index+",x,y:" + (int)linkline.StylusPoints[0].X + "," + (int)linkline.StylusPoints[0].Y + "       " +
                    //    (int)linkline.StylusPoints[1].X + "," + (int)linkline.StylusPoints[1].Y);
                }
            }
            else
            {
                double left = _inkCollector._mainPage._inkCanvas.ActualWidth - _inkCollector.KeyFrameAnnotation.Width / 2;
                double top = _inkCollector.KeyFrameAnnotation.Margin.Top + _inkCollector.KeyFrameAnnotation.Height / 2;
                foreach (int index in _keyFramesAnnotation.relatedKeyFrameIndexes)
                {
                    KeyValuePair<Stroke, KeyFramesAnnotation> currPair = (from KeyValuePair<Stroke, KeyFramesAnnotation> anno in _inkCollector.VideoSummarization.ShowKeyFrames[index].Annotations
                                                                          where anno.Value == _keyFramesAnnotation
                                                                          select anno).First();
                    Stroke linkline = (Stroke)(currPair.Key);                    
                    linkline.StylusPoints[1] = new StylusPoint(left, top);
                }
            }
            _keyFramesAnnotation.Width = this.Width;
            _keyFramesAnnotation.Height = this.Height;
        }
	}
}