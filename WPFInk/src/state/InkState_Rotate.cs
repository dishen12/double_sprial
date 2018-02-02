using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;
using System.Windows;
using System.Windows.Ink;           
using System.Windows.Input;
using WPFInk.tool;
using WPFInk.cmd;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WPFInk.state
{
    public class InkState_Rotate : InkState
    {
        StylusPoint _center;
        StylusPoint _startPoint;
        StylusPoint _prepoint;
        private List<double> preImageAngleList = new List<double>();
        private List<double> preButtonAngleList = new List<double>();

        private List<MyStroke> selectedStrokes = new List<MyStroke>();
        private List<MyButton> selectedButtons=new List<MyButton>();
        bool pressedMouseLeftButtonDown = false;
        List<MyImage> selectImages = new List<MyImage>();

        public InkState_Rotate(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            this._inkCanvas = inkCollector.InkCanvas;
            
        }
        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.CaptureMouse();
            selectedStrokes = _inkCollector.SelectedStrokes;
            selectImages = _inkCollector.SelectedImages;
            selectedButtons = _inkCollector.SelectButtons;

            if (selectedStrokes.Count > 0 || selectImages.Count > 0||selectedButtons.Count>0)
            {
                _center = _inkCollector.CenterSelect;
                Point start = e.GetPosition(_inkCanvas);
                _startPoint = new StylusPoint(start.X, start.Y);
                _prepoint = _startPoint;
                if (selectImages.Count > 0)
                {
                    foreach (MyImage myimage in selectImages)
                    {
                        preImageAngleList.Add(myimage.Angle);
                    }
                }
                if (selectedButtons.Count > 0)
                {
                    foreach (MyButton myButton in selectedButtons)
                    {
                        preButtonAngleList.Add(myButton.Angle);
                    }
                }
            }
            pressedMouseLeftButtonDown = true;
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(pressedMouseLeftButtonDown)
            {
                if (_startPoint != null)
                {
                    Point cur = e.GetPosition(_inkCanvas);
                    StylusPoint current = new StylusPoint(cur.X, cur.Y);
                    double angle1 = MathTool.getInstance().getAngleP2P(_center, _startPoint);
                    double angle2 = WPFInk.tool.MathTool.getInstance().getAngleP2P(_center, current);
                    double angle = angle2 - angle1;
                    if(selectedStrokes.Count>0)
                    {
						foreach (MyStroke myStroke in selectedStrokes)
						{
							Command rcInk = new RotateCommand(myStroke, angle, _center);
							rcInk.execute();
						}
                    }

                    if (selectImages.Count > 0)
                    {
                        int i = 0;
                        foreach (MyImage image in selectImages)
                        {
                            
                            //double angleImage1 = MathTool.getInstance().getAngle(_center, _startPoint);
                            //double angleImage2 = WPFInk.tool.MathTool.getInstance().getAngle(_center, current);
                            //double angleImage = angleImage2 - angleImage1;
                            image.Angle = angle;
                            Command rcImage = new ImageRotateCommand(image, preImageAngleList[i]);
                            rcImage.execute();
                            i++;
                            
                        }

                    }
                    if (selectedButtons.Count > 0)
                    {
                        int i = 0;
                        foreach (MyButton myButton in selectedButtons)
                        {

                            //double angleButton1 = MathTool.getInstance().getAngle(_center, _startPoint);
                            //double angleButton2 = WPFInk.tool.MathTool.getInstance().getAngle(_center, current);
                            //double angleButton = angleButton2 - angleButton1;
                            myButton.Angle = angle;
                            Command brc = new ButtonRotateCommand(myButton, preButtonAngleList[i]);
                            brc.execute();
                            //myButton.updateArrow(_inkCanvas,_inkCollector);
                            i++;

                        }

                    }
                    
                    _prepoint = current;
                }
            }
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_startPoint != null)
            {
                Point cur = e.GetPosition(_inkCanvas);
                StylusPoint current = new StylusPoint(cur.X, cur.Y);
                double angle1 = MathTool.getInstance().getAngleP2P(_center, _startPoint);
                double angle2 = MathTool.getInstance().getAngleP2P(_center, current);

                double angle = angle2 - angle1;
                if (selectedStrokes.Count > 0)
                {
					foreach (MyStroke myStroke in selectedStrokes)
					{
						Command rcInk = new RotateCommand(myStroke, angle, _center);
						_inkCollector.CommandStack.Push(rcInk);
					}
                }
                
                if (selectImages.Count > 0)
                {
                    int j = 0;
                    foreach (MyImage image in selectImages)
                    {
                        double angleImage1 = MathTool.getInstance().getAngleP2P(_center, _startPoint);
                        double angleImage2 = WPFInk.tool.MathTool.getInstance().getAngleP2P(_center, current);
                        double angleImage = angleImage2 - angleImage1;
                        Command rcImage = new ImageRotateCommand(image, preImageAngleList[j]);
                        _inkCollector.CommandStack.Push(rcImage);
                        j++;
                    }

                }
                if (selectedButtons.Count > 0)
                {
                    int j = 0;
                    foreach (MyButton myButton in selectedButtons)
                    {
                        double angleButton1 = MathTool.getInstance().getAngleP2P(_center, _startPoint);
                        double angleButton2 = WPFInk.tool.MathTool.getInstance().getAngleP2P(_center, current);
                        double angleButton = angleButton2 - angleButton1;
                        myButton.Angle = angleButton;
                        Command brc = new ButtonRotateCommand(myButton, preButtonAngleList[j]);

                        _inkCollector.CommandStack.Push(brc);
                        j++;
                    }

                }
            }

            _inkCanvas.ReleaseMouseCapture();
            pressedMouseLeftButtonDown = false;

        }
    }
}
