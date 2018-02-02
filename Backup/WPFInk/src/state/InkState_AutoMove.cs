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
using WPFInk.graphic;
using WPFInk.Global;

namespace WPFInk.state
{
    public class InkState_AutoMove : InkState
    {

        private Point _startPoint;
        private Point _prepoint;
		private Point current;
        bool pressedMouseLeftButtonDown = false;
		string MoveOrZoom = "";
        private List<MyGraphic> SelectedMyGraphics = new List<MyGraphic>();
        public InkState_AutoMove(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            this._inkCanvas = inkCollector.InkCanvas;
        }

        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCollector.IsAutoMove = true;
			_inkCanvas.CaptureMouse();
            _startPoint = e.GetPosition(_inkCanvas);
            _prepoint = _startPoint; 
			//button
			foreach (MyButton myButton in _inkCollector.Sketch.MyButtons)
			{
				if (myButton.IsDeleted == false&&myButton.Button.Visibility==Visibility.Visible)
				{
					if (MathTool.getInstance().isCloseMyButton(_startPoint, myButton, -2) == true)
					{
						_inkCollector.SelectButtons.Add(myButton);
						myButton.InkFrame._inkCanvas.CaptureMouse();
						myButton.InkFrame._inkCanvas.Cursor = Cursors.ScrollAll;
						myButton.TextBoxTime.Background = Brushes.CornflowerBlue;
						MoveOrZoom = "Move";
						break;
					}
					if (MathTool.getInstance().isEncircleMyButton(_startPoint, myButton, -2, 20) == true)
					{
						_inkCollector.SelectButtons.Add(myButton);
						_inkCanvas.Cursor = new Cursor(GlobalValues.FilesPath+"/WPFInk/WPFInk/src/cursor/Zoom.cur");
						myButton.TextBoxTime.Background = Brushes.CornflowerBlue;
						MoveOrZoom = "Zoom";
						break;
					}
				}
			}
			//image
			foreach (MyImage myImage in _inkCollector.Sketch.Images)
			{
				if (MathTool.getInstance().isCloseRectangle(_startPoint, myImage.Bound, -2))
				{
					_inkCollector.SelectedImages.Add(myImage);
					_inkCanvas.Cursor = Cursors.ScrollAll;
					myImage.Bound.Visibility = Visibility.Visible;
					MoveOrZoom = "Move";
				}
				if (MathTool.getInstance().isEncircleRectangle(_startPoint, myImage.Bound, -2,20))
				{
					_inkCollector.SelectedImages.Add(myImage);
					_inkCanvas.Cursor = new Cursor(GlobalValues.FilesPath+"/WPFInk/WPFInk/src/cursor/Zoom.cur");
					myImage.Bound.Visibility = Visibility.Visible;
					MoveOrZoom = "Zoom";
				}
			}
			//stroke
			StrokeCollection strokes = new StrokeCollection();
			foreach (MyStroke myStroke in _inkCollector.Sketch.MyStrokes)
			{
                //myStroke.Stroke.DrawingAttributes.Color = Colors.Red;
				strokes.Add(myStroke.Stroke);
			}
			StrokeCollection hitStrokes = strokes.HitTest(_startPoint, 10);
			if (hitStrokes.Count > 0)
			{
				foreach (Stroke stroke in hitStrokes)
				{
					foreach (MyStroke myStroke in _inkCollector.Sketch.MyStrokes)
					{
						if (myStroke.Stroke == stroke)
						{
							_inkCollector.SelectedStrokes.Add(myStroke);
						}
					}
				}
				_inkCanvas.Cursor = Cursors.ScrollAll;
				MoveOrZoom = "Move";
			}
					
			
			//myGraphic
            foreach (MyGraphic myGraphic in _inkCollector.Sketch.MyGraphics)
            {
                if (myGraphic.ShapeType!="loopArc"&&MathTool.getInstance().isPointInRectangle(MathTool.getInstance().RectToRectangle(myGraphic.Strokes.GetBounds()), _startPoint))
                {
                    //myGraphic.Stroke.DrawingAttributes.Color = Colors.Red;
                    SelectedMyGraphics = GraphicMathTool.getInstance().getDirectRelativeMyGraphicLineHasSelf(myGraphic, _inkCollector, new List<MyGraphic>());
                    //_inkCollector._mainPage.message.Text= SelectedMyGraphics.Count.ToString();
                    _inkCanvas.Cursor = Cursors.ScrollAll;
                    MoveOrZoom = "Move";
                }
            }
			//Text,移动text，要求鼠标的位置在text周围，而不是上面
			foreach (MyRichTextBox myRichTextBox in _inkCollector.Sketch.MyRichTextBoxs)
			{
				TextRange textRange = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);
				string textRangeStr1 = textRange.Text.Replace(" ", "");
				string textRangeStr2 = textRangeStr1.Replace("\r\n", "");
				if (textRangeStr2 != "" && MathTool.getInstance().isEncircleRectangle(_startPoint, MathTool.getInstance().RectToRectangle(new Rect(new Point(myRichTextBox.RichTextBox.Margin.Left, myRichTextBox.RichTextBox.Margin.Top), new Point(myRichTextBox.RichTextBox.Margin.Left + myRichTextBox.RichTextBox.Width, myRichTextBox.RichTextBox.Margin.Top + myRichTextBox.RichTextBox.Height))), 0, 20))
				{
					_inkCollector.SelectedMyRichTextBoxs.Add(myRichTextBox);
					myRichTextBox.RichTextBox.BorderBrush = new SolidColorBrush(Colors.Blue);
					_inkCanvas.Cursor = Cursors.ScrollAll;
					MoveOrZoom = "Move";
				}
			}

            pressedMouseLeftButtonDown = true;
        }

        
        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (pressedMouseLeftButtonDown)
            {
                if (_startPoint != null)
                {                               
                    current = e.GetPosition(_inkCanvas);
						switch (MoveOrZoom)
						{
							case "Move":
								double offsetx = current.X - _prepoint.X;
								double offsety = current.Y - _prepoint.Y;
								_inkCollector.IsAutoMove = true;
										
								//移动Mybutton
								foreach (MyButton myButton in _inkCollector.SelectButtons)
								{
									myButton.InkFrame._inkCanvas.CaptureMouse();
									ButtonMoveCommand bmc = new ButtonMoveCommand(myButton, offsetx, offsety, _inkCollector);
									bmc.execute();
								}
								
								//移动MyImage
								foreach (MyImage image in _inkCollector.SelectedImages)
								{
									ImageMoveCommand imc = new ImageMoveCommand(image, offsetx, offsety);
									imc.execute();
									image.adjustBound();
									foreach (ImageConnector connector in image.ConnectorCollection)
									{
										connector.adjustConnector();
									}
								}

								//移动笔迹
								if (_inkCollector.SelectedStrokes.Count > 0)
								{
									foreach (MyStroke myStroke in _inkCollector.SelectedStrokes)
									{
										MoveCommand mc = new MoveCommand(myStroke, offsetx, offsety);
										mc.execute();
									}
								}

								//移动图形
								if (SelectedMyGraphics.Count > 0)
								{
                                    
									MyGraphicsMoveCommand mgsmc = new MyGraphicsMoveCommand(SelectedMyGraphics, offsetx, offsety,  _inkCollector);
									mgsmc.execute();
								}

								//移动文本
								foreach (MyRichTextBox myRichTextBox in _inkCollector.SelectedMyRichTextBoxs)
								{
									Command tmc = new TextMoveCommand(myRichTextBox, offsetx, offsety);
									tmc.execute();
								}
								break;
							case "Zoom":
								
								_inkCollector.IsAutoMove = false;
								StylusPoint curr = new StylusPoint(current.X, current.Y);
								StylusPoint pre = new StylusPoint(_prepoint.X, _prepoint.Y);
								
								foreach (MyButton myButton in _inkCollector.SelectButtons)
								{
									double dist1 = MathTool.getInstance().distanceP2P(MathTool.getInstance().getMyButtonCenter(myButton), curr);
									double dist2 = MathTool.getInstance().distanceP2P(MathTool.getInstance().getMyButtonCenter(myButton), pre);
									if (dist2 == 0)
									{
										dist2 = 1;
									}
									double scaling = dist1 / dist2;
									ButtonZoomCommand bmc = new ButtonZoomCommand(myButton, scaling, _inkCollector,myButton.Angle);
									bmc.execute();
								}
								foreach (MyImage image in _inkCollector.SelectedImages)
								{
									double dist1 = MathTool.getInstance().distanceP2P(MathTool.getInstance().getImageCenter(image), curr);
									double dist2 = MathTool.getInstance().distanceP2P(MathTool.getInstance().getImageCenter(image), pre);
									if (dist2 == 0)
									{
										dist2 = 1;
									}
									double scaling = dist1 / dist2;
									ImageZoomCommand izc = new ImageZoomCommand(image, scaling);
									izc.execute();
									image.adjustBound();
									foreach (ImageConnector connector in image.ConnectorCollection)
									{
										connector.adjustConnector();
									}
								}
								break;
						}
                        
                    
                    _prepoint = current;

                }
            }          
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (pressedMouseLeftButtonDown)
            {
                if (_startPoint != null)
                {
                    current = e.GetPosition(_inkCanvas);

                    
					switch (MoveOrZoom)
					{
						case "Move":
							double offsetx = current.X - _startPoint.X;
							double offsety = current.Y - _startPoint.Y;
							_inkCollector.IsAutoMove = true;
							foreach (MyButton myButton in _inkCollector.SelectButtons)
							{
								myButton.InkFrame._inkCanvas.ReleaseMouseCapture();
								myButton.InkFrame._inkCanvas.Cursor = Cursors.Arrow;
								myButton.TextBoxTime.Background = null;
								ButtonMoveCommand bmc = new ButtonMoveCommand(myButton, offsetx, offsety, _inkCollector);
								_inkCollector.CommandStack.Push(bmc);
							}
							if (_inkCollector.SelectedImages.Count > 0)
							{
								foreach (MyImage image in _inkCollector.SelectedImages)
								{
									Command imc = new ImageMoveCommand(image, offsetx, offsety);
									_inkCollector.CommandStack.Push(imc);
									image.Bound.Visibility = Visibility.Collapsed;
									_inkCanvas.Cursor = Cursors.Arrow;
									image.Image.ReleaseMouseCapture();

									foreach (ImageConnector connector in image.ConnectorCollection)
									{
										connector.adjustConnector();
									}
								}
								
							}
							if (_inkCollector.SelectedStrokes.Count > 0)
							{
								foreach (MyStroke myStroke in _inkCollector.SelectedStrokes)
								{
									MoveCommand mc = new MoveCommand(myStroke, offsetx, offsety);
									_inkCollector.CommandStack.Push(mc);
								}
							}
							//移动图形
							if (SelectedMyGraphics.Count > 0)
							{
                                foreach (MyGraphic myGraphic in SelectedMyGraphics)
                                {
                                    MyGraphicMoveCommand mgmc = new MyGraphicMoveCommand(myGraphic, offsetx, offsety, _inkCollector);
                                    _inkCollector.CommandStack.Push(mgmc);
                                    mgmc.searchRelation();
                                }
                                //MyGraphicsMoveCommand mgsmc = new MyGraphicsMoveCommand(SelectedMyGraphics, offsetx, offsety, _inkCollector.Sketch.MyGraphics, _inkCollector);
                                //_inkCollector.CommandStack.Push(mgsmc);
                                List<int> ids = GraphicMathTool.getInstance().getGraphicStructure(_inkCollector.Sketch.MyGraphics[0], _inkCollector, new List<int>());
                                foreach (int id in ids)
                                {
                                    _inkCollector._mainPage.message.Content += id.ToString() + ",";
                                }
							}
							//移动文本
							foreach (MyRichTextBox myRichTextBox in _inkCollector.SelectedMyRichTextBoxs)
							{
								Command tmc = new TextMoveCommand(myRichTextBox, offsetx, offsety);
								myRichTextBox.RichTextBox.BorderBrush = null;
								_inkCollector.CommandStack.Push(tmc);
							}
							MoveOrZoom = "";
							break;
						case "Zoom":
							_inkCollector.IsAutoMove = false;
							StylusPoint curr = new StylusPoint(current.X, current.Y);
							StylusPoint sta = new StylusPoint(_startPoint.X, _startPoint.Y);
							foreach (MyButton myButton in _inkCollector.SelectButtons)
							{
								double dist1 = MathTool.getInstance().distanceP2P(MathTool.getInstance().getMyButtonCenter(myButton), curr);
								double dist2 = MathTool.getInstance().distanceP2P(MathTool.getInstance().getMyButtonCenter(myButton), sta);
								if (dist2 == 0)
								{
									dist2 = 1;
								}
								double scaling = dist1 / dist2;
								_inkCanvas.Cursor = Cursors.Arrow;
								myButton.TextBoxTime.Background = null;
								ButtonZoomCommand bmc = new ButtonZoomCommand(myButton, scaling, _inkCollector, myButton.Angle);
								_inkCollector.CommandStack.Push(bmc);
							}
							foreach (MyImage image in _inkCollector.SelectedImages)
							{
								double dist1 = MathTool.getInstance().distanceP2P(MathTool.getInstance().getImageCenter(image), curr);
								double dist2 = MathTool.getInstance().distanceP2P(MathTool.getInstance().getImageCenter(image), sta);
								if (dist2 == 0)
								{
									dist2 = 1;
								}
								double scaling = dist1 / dist2;
								_inkCanvas.Cursor = Cursors.Arrow;
								image.Bound.Visibility = Visibility.Collapsed;
								ImageZoomCommand izc = new ImageZoomCommand(image, scaling);
								_inkCollector.CommandStack.Push(izc);
								foreach (ImageConnector connector in image.ConnectorCollection)
								{
									connector.adjustConnector();
								}
							}
							MoveOrZoom = "";
							break;
					}



					_inkCollector.SelectButtons.Clear();
					_inkCollector.SelectedImages.Clear();
					_inkCollector.SelectedStrokes.Clear();
					_inkCollector.SelectedMyRichTextBoxs.Clear();
					SelectedMyGraphics.Clear();

                }

				_inkCanvas.ReleaseMouseCapture();
                _inkCollector.IsAutoMove = false;
                pressedMouseLeftButtonDown = false; 
                
            }
        }
    }
}
