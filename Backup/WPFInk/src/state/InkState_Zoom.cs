using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using WPFInk.ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPFInk.tool;
using WPFInk.cmd;
using System.Collections.Generic;

namespace WPFInk.state
{
    public class InkState_Zoom : InkState
    {
        private StylusPoint _startPoint;
        private StylusPoint center;
        StylusPoint _prepoint;
        bool pressedMouseLeftButtonDown = false;
        private List<MyStroke> selectedStrokes = null;
        private List<MyButton> selectedButtons;
        List<MyImage> selectedImages = null;
        private List<double> preMyButtonInkFrameList = new List<double>();

        public InkState_Zoom(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            this._inkCanvas = inkCollector.InkCanvas;
        }

        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.CaptureMouse();//捕捉鼠标
            Point start = e.GetPosition(_inkCanvas);
            _startPoint = new StylusPoint(start.X, start.Y);
            center = _inkCollector.CenterSelect;
            selectedStrokes = _inkCollector.SelectedStrokes;
            selectedImages = _inkCollector.SelectedImages;
            selectedButtons = _inkCollector.SelectButtons;
            _prepoint = _startPoint;
            foreach (MyButton myButton in selectedButtons)
            {
                preMyButtonInkFrameList.Add(myButton.Width / myButton.InkFrame.Width);
            }
            pressedMouseLeftButtonDown = true;
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (pressedMouseLeftButtonDown)
            {
                Point cur = e.GetPosition(_inkCanvas);
                StylusPoint current = new StylusPoint(cur.X, cur.Y);
                double dist1 = MathTool.getInstance().distanceP2P(center, current);
                double dist2 = MathTool.getInstance().distanceP2P(center, _prepoint);
                if (dist2 == 0)
                {
                    dist2 = 1;
                }
                double scaling = dist1 / dist2;
                if (selectedStrokes.Count > 0)   //笔迹处理
                {
					foreach (MyStroke myStroke in selectedStrokes)
					{
						ZoomCommand zc = new ZoomCommand(myStroke, scaling);
						zc.execute();
					}
                }
                if (selectedImages.Count > 0)    //图片处理
                {
                    foreach (MyImage image in selectedImages)
                    {
                        ImageZoomCommand izc = new ImageZoomCommand(image, scaling);
                        izc.execute();
                        image.adjustBound();
                        foreach (ImageConnector connector in image.ConnectorCollection)
                        {
                            connector.adjustConnector();
                        }
                    }
                }
                if (selectedButtons.Count > 0)    //button处理
                {
                    foreach (MyButton myButton in selectedButtons)
                    {
                        ButtonZoomCommand bzc = new ButtonZoomCommand(myButton, scaling,_inkCollector,1);
                        bzc.execute();
                    }
                }
                //piemenu移动
                double dist3 = MathTool.getInstance().distanceP2P(center, current);
                double dist4 = MathTool.getInstance().distanceP2P(center, _startPoint);
                if (dist4 == 0)
                {
                    dist4 = 1;
                }
                double scaling2 = dist3 / dist4;
                _inkCollector._mainPage.OperatePieMenu.Margin = new System.Windows.Thickness(_inkCollector.BoundSelect.Margin.Left + _inkCollector.BoundSelect.Width * scaling2, _inkCollector._mainPage.OperatePieMenu.Margin.Top, 0, 0);
                if (_inkCollector._mainPage.OperatePieMenu.Margin.Top < _inkCanvas.Margin.Top)       //上面
                {
                    _inkCollector._mainPage.OperatePieMenu.Margin = new Thickness(_inkCollector._mainPage.OperatePieMenu.Margin.Left, 10, 0, 0);
                }
                //右边
                if (_inkCollector._mainPage.OperatePieMenu.Margin.Left > _inkCanvas.Margin.Left + _inkCanvas.ActualWidth - _inkCollector._mainPage.OperatePieMenu.ActualWidth)
                {
                    _inkCollector._mainPage.OperatePieMenu.Margin = new Thickness(_inkCanvas.Margin.Left + _inkCanvas.ActualWidth - _inkCollector._mainPage.OperatePieMenu.ActualWidth, _inkCollector._mainPage.OperatePieMenu.Margin.Top, 0, 0);
                }
                _prepoint = current;
            }
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_startPoint != null)
            {
                Point cur = e.GetPosition(_inkCanvas);
                StylusPoint current = new StylusPoint(cur.X, cur.Y);
                double dist1 = MathTool.getInstance().distanceP2P(center, current);
                double dist2 = MathTool.getInstance().distanceP2P(center, _startPoint);
                double scaling = dist1 / dist2;
                if (selectedStrokes.Count > 0)   //笔迹处理
				{
					foreach (MyStroke myStroke in selectedStrokes)
					{
						ZoomCommand zc = new ZoomCommand(myStroke, scaling);
						_inkCollector.CommandStack.Push(zc);
					}
                }
                if (selectedImages.Count > 0)    //图片处理
                {
                    foreach (MyImage image in selectedImages)
                    {
                        ImageZoomCommand izc = new ImageZoomCommand(image, scaling);
                        _inkCollector.CommandStack.Push(izc);
                        foreach (ImageConnector connector in image.ConnectorCollection)
                        {
                            connector.adjustConnector();
                        }
                    }
                }
                if (selectedButtons.Count > 0)    //button处理
                {
                    int i = 0;
                    foreach (MyButton myButton in selectedButtons)
                    {
                        ButtonZoomCommand bzc = new ButtonZoomCommand(myButton, scaling, _inkCollector,preMyButtonInkFrameList[i]);
                        _inkCollector.CommandStack.Push(bzc);
                        i++;
                    }
                }
            }
            _inkCanvas.ReleaseMouseCapture();
            pressedMouseLeftButtonDown = false;
        }
    }
}
