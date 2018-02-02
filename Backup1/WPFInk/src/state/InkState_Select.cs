using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.ink;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using WPFInk.video;

namespace WPFInk.state
{
    public class InkState_Select : InkState
    {

        private InkFrame _inkFrame;
        private StrokeCollection selectedStrokes = null;
        private ReadOnlyCollection<UIElement> selectedChildren = null;




        public InkState_Select(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            this._inkFrame = inkCollector._mainPage;
        }


        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (_inkCollector.SelectButtons.Count>0)
            {
                foreach (MyButton myButton in _inkCollector.SelectButtons)
                {
                    myButton.TextBoxTime.Background = null;
                }
            }
            _inkCollector.SelectedImages.Clear();
            _inkCollector.SelectedStrokes.Clear();
            _inkCollector.SelectButtons.Clear();
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {


        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectedStrokes = _inkCanvas.GetSelectedStrokes();
            selectedChildren = _inkCanvas.GetSelectedElements();
            if (selectedStrokes.Count > 0 || selectedChildren.Count > 0)
            {
                Point TopRight = _inkCanvas.GetSelectionBounds().TopRight;
                _inkFrame.OperatePieMenu.Margin = new Thickness(TopRight.X, TopRight.Y - 100, 0, 0);
                Rect rect = _inkCanvas.GetSelectionBounds();
                Rectangle bound = new Rectangle();
                bound.Margin = new System.Windows.Thickness(rect.Left, rect.Top, rect.Right, rect.Bottom);
                bound.Width = rect.Width;
                bound.Height = rect.Height;
                bound.Stroke = new SolidColorBrush(Colors.Red);
                bound.StrokeThickness = 2;
                _inkCollector.BoundSelect = bound;
                _inkCollector.CenterSelect = new StylusPoint(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
               // _inkCanvas.Children.Add(bound);
                if (_inkFrame.OperatePieMenu.Margin.Left < _inkCanvas.Margin.Left)   //左边
                {
                    _inkFrame.OperatePieMenu.Margin = new Thickness(0, TopRight.Y - 100, 0, 0);
                }
                if (_inkFrame.OperatePieMenu.Margin.Top < _inkCanvas.Margin.Top)       //上面
                {
                    _inkFrame.OperatePieMenu.Margin = new Thickness(TopRight.X, 10, 0, 0);
                }
                //右边
                if (_inkFrame.OperatePieMenu.Margin.Left > _inkCanvas.Margin.Left + _inkCanvas.ActualWidth - _inkFrame.OperatePieMenu.ActualWidth)
                {
                    _inkFrame.OperatePieMenu.Margin = new Thickness(_inkCanvas.Margin.Left + _inkCanvas.ActualWidth - _inkFrame.OperatePieMenu.ActualWidth, 10, 0, 0);
                }
                //下面不用处理

                _inkFrame.OperatePieMenu.Visibility = Visibility.Visible;
            }
            if (selectedStrokes.Count > 0)
            {
                foreach (Stroke stroke in selectedStrokes)
                {
					foreach (MyStroke myStroke in _inkCollector.Sketch.MyStrokes)
					{
						if (_inkCollector.SelectedStrokes.IndexOf(myStroke)==-1&&myStroke.Stroke == stroke)
						{
                            _inkCollector.SelectedStrokes.Add(myStroke);
                            //Console.WriteLine(_inkCollector.SelectedStrokes.Count);
						}
					}
                }
            }
            if (selectedChildren.Count > 0)
            {
                foreach (UIElement _uIElement in selectedChildren)
                {
                    if (_uIElement.GetType().ToString() == "System.Windows.Controls.Image")
                    {
                        if (_inkCollector.SelectedImages.IndexOf(new MyImage((System.Windows.Controls.Image)_uIElement)) < 0)
                        {
                            //_inkCollector.SelectedImages.Add(new MyImage((System.Windows.Controls.Image)_uIElement));
                            foreach (MyImage myimage in _inkCollector._sketch.Images)
                            {
                                if ((System.Windows.Controls.Image)_uIElement == myimage.Image)
                                {
                                    _inkCollector.SelectedImages.Add(myimage);
                                    //_inkCanvas.Children.Add(myimage.Bound);
									myimage.Bound.Visibility = Visibility.Visible;

                                }
                            }
                        }
                    }
                    if (_uIElement.GetType().ToString() == "System.Windows.Controls.Button")
                    {
                        if (_inkCollector.SelectButtons.IndexOf(new MyButton((System.Windows.Controls.Button)_uIElement)) < 0)
                        {
                            foreach (MyButton myButton in _inkCollector._sketch.MyButtons)
                            {
                                if ((System.Windows.Controls.Button)_uIElement == myButton.Button)
                                {
                                    myButton.TextBoxTime.Background = Brushes.CornflowerBlue;
                                    _inkCollector.SelectButtons.Add(myButton);

                                }
                            }
                        }
                    }                      
                }

            }
            this._inkCanvas.EditingMode = InkCanvasEditingMode.None;

        }

        public static StrokeCollection getStrokeCollection(StrokeCollection strokeCollection)
        {
            if (strokeCollection != null)
                return strokeCollection;
            else return null;
        }
    }
}
