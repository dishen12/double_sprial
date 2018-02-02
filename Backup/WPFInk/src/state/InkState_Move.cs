using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;
using System.Windows;
using WPFInk.cmd;
using System.Windows.Ink;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WPFInk.state
{
    public class InkState_Move : InkState
    {

        private Point _startPoint;
        private Point _prepoint;
		private List<MyStroke> selectedStrokes = null;
        private List<MyImage> selectedImages;
        private List<MyButton> selectedButtons;
        bool pressedMouseLeftButtonDown = false;

        public InkState_Move(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            this._inkCanvas = inkCollector.InkCanvas;
        }

        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.CaptureMouse();
            _startPoint = e.GetPosition(_inkCanvas);
            _prepoint = _startPoint;

            selectedStrokes = _inkCollector.SelectedStrokes;
            selectedImages = _inkCollector.SelectedImages;
            selectedButtons = _inkCollector.SelectButtons;
            pressedMouseLeftButtonDown = true;
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (pressedMouseLeftButtonDown)
            {
                if (_startPoint != null)
                {
                    _inkCanvas.CaptureMouse();
                    if (selectedStrokes.Count > 0 || selectedImages.Count > 0||selectedButtons.Count>0)
                    {
                        //反馈效果
                        Point current = e.GetPosition(_inkCanvas);
                        double offsetx = current.X - _prepoint.X;
                        double offsety = current.Y - _prepoint.Y;

                        if (selectedStrokes.Count > 0)
                        {
							foreach (MyStroke myStroke in selectedStrokes)
							{
								MoveCommand mc = new MoveCommand(myStroke, offsetx, offsety);
								mc.execute();
							}
                        }
                        if (selectedImages.Count > 0)
                        {
                            foreach (MyImage image in selectedImages)
                            {
                                ImageMoveCommand imc = new ImageMoveCommand(image, offsetx, offsety);
                                imc.execute();
                                image.adjustBound();
                                foreach (ImageConnector connector in image.ConnectorCollection)
                                {
                                    connector.adjustConnector();
                                }
                            }
                        }
                        if (selectedButtons.Count > 0)
                        {
                            foreach (MyButton myButton in selectedButtons)
                            {
                                ButtonMoveCommand bmc = new ButtonMoveCommand(myButton,offsetx,offsety,_inkCollector);
                                bmc.execute();
                                //myButton.updateArrow(_inkCanvas);
                            }
                        }
                        _inkCollector._mainPage.OperatePieMenu.Margin = new System.Windows.Thickness(_inkCollector._mainPage.OperatePieMenu.Margin.Left + offsetx, _inkCollector._mainPage.OperatePieMenu.Margin.Top + offsety, 0, 0);
                        //_inkCollector.BoundSelect.Margin = new System.Windows.Thickness(_inkCollector.BoundSelect.Margin.Left + offsetx, _inkCollector.BoundSelect.Margin.Top + offsety, 0, 0);

                        //OperatePieMenu位置校正，防止出界
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
            }
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_startPoint != null)
            {
                Point current = e.GetPosition(_inkCanvas);
                double offsetx = current.X - _startPoint.X;
                double offsety = current.Y - _startPoint.Y;
                _inkCollector.CenterSelect = new System.Windows.Input.StylusPoint(_inkCollector.CenterSelect.X + offsetx, _inkCollector.CenterSelect.Y + offsety);

                if (selectedStrokes.Count > 0)
				{
					foreach (MyStroke myStroke in selectedStrokes)
					{
						MoveCommand mc = new MoveCommand(myStroke, offsetx, offsety);
						_inkCollector.CommandStack.Push(mc);
					}
                }
                if (selectedImages.Count > 0)
                {
                    foreach (MyImage image in selectedImages)
                    {
                        Command imc = new ImageMoveCommand(image, offsetx, offsety);
                        _inkCollector.CommandStack.Push(imc);

                        foreach (ImageConnector connector in image.ConnectorCollection)
                        {
                            connector.adjustConnector();
                        }
                    }
                }
                if (selectedButtons.Count > 0)
                {
                    foreach (MyButton myButton in selectedButtons)
                    {
                        ButtonMoveCommand bmc = new ButtonMoveCommand(myButton, offsetx, offsety, _inkCollector);
                        _inkCollector.CommandStack.Push(bmc);
                    }
                }
            }               
            _inkCanvas.ReleaseMouseCapture();
            pressedMouseLeftButtonDown = false;
        }
    }
}

