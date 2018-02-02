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
using System.Windows.Media.Animation;
using System.Windows.Ink;
using System.Windows.Shapes;
using System.Threading;
using WPFInk.tool;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for VideoOperation.xaml
	/// </summary>
	public partial class VideoOperation : UserControl
	{
        private List<InkCanvas> _thumbInkCanvasList = new List<InkCanvas>();    //草图缩略图列表
        private List<Button> _thumbButtonList = new List<Button>();             //草图缩略图按钮列表    
        private List<Double> _startTimeList = new List<Double>();                 //起始时间列表            
        private List<Double> _endTimeList = new List<Double>();                 //终止时间列表
        private List<TextBox> _timeShowTextBoxList = new List<TextBox>();
        private InkCanvas _thumbInkCanvas;
        private Button _thumbButton;


        private double scaling = 1;//缩放比例
        //Storyboard _videoPalyStoryboard;
        public VideoOperation()
        {
            this.InitializeComponent();
        }



        private void InkCanvasRight_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StrokeCollection strokes = this.InkCanvasRight.Strokes.Clone();
            InkCanvasLeft.Strokes.Clear();
            InkCanvasLeft.Strokes.Add(strokes);
        }

        private void InkCanvasLeft_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //StrokeCollection strokes = this.InkCanvasLeft.Strokes.Clone();
            //InkCanvasRight.Strokes.Clear();
            //InkCanvasRight.Strokes.Add(strokes);
        }



        private void ButtonStaticAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this._videoAnnotation.mediaPlayer.Source != null)
            {

                _startTimeList.Add(this._videoAnnotation.PositionCurr);
                //_startTimeList.Sort();
                _thumbInkCanvas = new InkCanvas();
                _thumbInkCanvas.EditingMode=InkCanvasEditingMode.None;
                _thumbInkCanvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_thumbInkCanvas_MouseLeftButtonDown);
                _thumbButton = new Button();
                System.Windows.Style currStyle = (System.Windows.Style)FindResource("ThumbButtonStyle");
                _thumbButton.Style = currStyle;
                _thumbButton.Width = 164;
                _thumbButton.Height = 164;
                _thumbButtonList.Add(_thumbButton);
                _thumbInkCanvasList.Add(_thumbInkCanvas);
                //记录当前时间
                ButtonStaticStart.Visibility = Visibility.Visible;
                ButtonStaticAdd.Visibility = Visibility.Collapsed;

                //添加草图
                this.InkCanvasLeft.EditingMode = InkCanvasEditingMode.Ink;
                this.InkCanvasLeft.Strokes.Clear();
                this._videoAnnotation.PauseVideo();       //暂停播放视频

            }
        }


        private int _thumbIndex;

        System.Windows.Forms.Timer timePeriodTimer;
        private int timePeriod = 0;   //记录时间间隔
        private void _thumbInkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InkCanvas inkCanvas= (InkCanvas)sender ;
            int i = 0;      //记录下标
            foreach (InkCanvas ic in _thumbInkCanvasList)
            {
                 if (inkCanvas!=ic)
                 {
                     i++;
                 }
                else
                 {
                     break;
                 }
            }
            _thumbIndex = i;
            double startTime = _startTimeList[i];
            double endTime = _endTimeList[i];
            this._videoAnnotation.setPositon((int)startTime); //定位视频
            //MessageBox.Show(i.ToString()+",起始时间："+startTime.ToString()+"ms,结束时间:"+endTime.ToString()+"ms");

            //下面是在视频上显示草图
            this.InkCanvasLeft.Strokes.Clear();
            StrokeCollection strokes = inkCanvas.Strokes.Clone();
            this.InkCanvasLeft.Strokes.Add(strokes);
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.Scale(1/scaling, 1/scaling);
            InkCanvasLeft.Strokes.Transform(m, true);


            //计算草图终止显示时间
            timePeriodTimer = new System.Windows.Forms.Timer();
            timePeriodTimer.Interval = 1000;
            timePeriodTimer.Tick += new System.EventHandler(timer_Tick);
            timePeriodTimer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timePeriod++;
            if ((int)(_endTimeList[_thumbIndex] - _startTimeList[_thumbIndex]) / 1000 == timePeriod)
            {
                InkCanvasLeft.Strokes.Clear();
                timePeriodTimer.Stop();
                timePeriod = 0;
            }
        }

        

        private void ButtonStaticStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this._videoAnnotation.mediaPlayer.Source != null)
            {
                
                ButtonStaticOk.Visibility = Visibility.Visible;
                ButtonStaticStart.Visibility = Visibility.Collapsed;


                this._videoAnnotation.PlayVideo();
            }
        }

        int _PageCount = 0;//缩略图页数
        int _ThumbCount = 0;  //缩略图个数
        int _ThumbCountPerPage = 6;       //每页显示缩略图个数
        int _PageCurr = 0;//当前页
        private void ButtonStaticOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this._videoAnnotation.mediaPlayer.Source != null)
            {
                //记录当前时间代码

                if (this.InkCanvasLeft.Strokes.Count > 0)
                {

                    _endTimeList.Add(_videoAnnotation.PositionCurr);
                    StrokeCollection strokes = this.InkCanvasLeft.Strokes.Clone();
                    _thumbInkCanvas.Width = this.InkCanvasLeft.ActualWidth;
                    _thumbInkCanvas.Height = this.InkCanvasLeft.ActualHeight;
                    _thumbInkCanvas.Strokes.Clear();
                    _thumbInkCanvas.Strokes.Add(strokes);
                    scaling = _thumbButton.Width / this.InkCanvasLeft.ActualWidth;
                    Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
                    m.Scale(scaling, scaling);
                    _thumbInkCanvas.Strokes.Transform(m, true);

                    TextBox timeTextBox = new TextBox();
                    timeTextBox.Height = 30;
                    timeTextBox.Width = 150;
                    timeTextBox.BorderThickness = new System.Windows.Thickness(0);
                    timeTextBox.Background = null;
                    List<string> hmsStart = ConvertClass.getInstance().MsToHMS(_startTimeList[_startTimeList.Count-1]);
                    List<string> hmsEnd = ConvertClass.getInstance().MsToHMS(_endTimeList[_endTimeList.Count-1]);
                    timeTextBox.Text = "Time: " + hmsStart[0] + ":" + hmsStart[1] + ":" + hmsStart[2] + " - " + hmsEnd[0] + ":" + hmsEnd[1] + ":" + hmsEnd[2];
                    _timeShowTextBoxList.Add(timeTextBox);
                    _ThumbCount = _thumbButtonList.Count;
                    if (_ThumbCount % _ThumbCountPerPage == 0)
                    {
                        _PageCount = _ThumbCount / _ThumbCountPerPage;
                    }
                    else
                    {
                        _PageCount = _ThumbCount / _ThumbCountPerPage + 1;
                    }
                    _PageCurr = _PageCount;
                    //清除原来显示的缩略图按钮
                    foreach (Button thumbBtn in _thumbButtonList)
                    {
                        if (this.TableGrid.Children.IndexOf(thumbBtn) > -1)
                        {
                            this.TableGrid.Children.Remove(thumbBtn);
                        }
                    }
                    //清除原来显示的时间Textbox
                    foreach (TextBox textBoxtime in _timeShowTextBoxList)
                    {
                        if (this.TableGrid.Children.IndexOf(textBoxtime) > -1)
                        {
                            this.TableGrid.Children.Remove(textBoxtime);
                        }
                    }
                    for (int i = _ThumbCountPerPage * (_PageCurr - 1); i < _ThumbCount; i++)
                    {

                        Grid.SetColumnSpan(_thumbButtonList[i], 2);
                        Grid.SetRow(_thumbButtonList[i], 1);
                        Grid.SetColumn(_thumbButtonList[i], 0);
                        _thumbButtonList[i].Margin = new System.Windows.Thickness(-1000 + 400 * (i % _ThumbCountPerPage), 70, 0, 70);
                        this.TableGrid.Children.Add(_thumbButtonList[i]);
                        
                        //显示起始终止时间                       
                        Grid.SetColumnSpan(_timeShowTextBoxList[i], 2);
                        Grid.SetRow(_timeShowTextBoxList[i], 1);
                        Grid.SetColumn(_timeShowTextBoxList[i], 0);
                        _timeShowTextBoxList[i].Margin = new System.Windows.Thickness(-1000 + 400 * (i % _ThumbCountPerPage), -110, 0, 70);
                        this.TableGrid.Children.Add(_timeShowTextBoxList[i]);                              

                        _thumbButtonList[i].Content = _thumbInkCanvasList[i];
                        this.InkCanvasLeft.Strokes.Clear();
                    }

                    if (_PageCurr > 1)
                    {
                        this.ButtonPre.Visibility = Visibility.Visible;
                    }


                    this.ButtonNext.Visibility = Visibility.Collapsed;
                    ButtonStaticOk.Visibility = Visibility.Collapsed;
                    ButtonStaticAdd.Visibility = Visibility.Visible;
                    this.InkCanvasLeft.EditingMode = InkCanvasEditingMode.None;
                    //this._videoAnnotation.ResumeVideo();

                }
            }

        }

        private void ButtonPre_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (--_PageCurr > 0)
            {
                foreach (Button thumbBtn in _thumbButtonList)
                {
                    if (this.TableGrid.Children.IndexOf(thumbBtn) > -1)
                    {
                        this.TableGrid.Children.Remove(thumbBtn);
                    }
                }
                //清除原来显示的时间Textbox
                foreach (TextBox textBoxtime in _timeShowTextBoxList)
                {
                    if (this.TableGrid.Children.IndexOf(textBoxtime) > -1)
                    {
                        this.TableGrid.Children.Remove(textBoxtime);
                    }
                }
                if (_ThumbCount % _ThumbCountPerPage == 0)
                {
                    _PageCount = _ThumbCount / _ThumbCountPerPage;
                }
                else
                {
                    _PageCount = _ThumbCount / _ThumbCountPerPage + 1;
                }
                if (_PageCurr == 1)
                {
                    this.ButtonPre.Visibility = Visibility.Collapsed;
                }
                for (int i = _ThumbCountPerPage * (_PageCurr - 1); i < _ThumbCountPerPage * (_PageCurr - 1) + _ThumbCountPerPage; i++)
                {

                    Grid.SetColumnSpan(_thumbButtonList[i], 2);
                    Grid.SetRow(_thumbButtonList[i], 1);
                    Grid.SetColumn(_thumbButtonList[i], 0);
                    _thumbButtonList[i].Margin = new System.Windows.Thickness(-1000 + 400 * (i % _ThumbCountPerPage), 70, 0, 70);
                    this.TableGrid.Children.Add(_thumbButtonList[i]);

                    //显示起始终止时间                       
                    Grid.SetColumnSpan(_timeShowTextBoxList[i], 2);
                    Grid.SetRow(_timeShowTextBoxList[i], 1);
                    Grid.SetColumn(_timeShowTextBoxList[i], 0);
                    _timeShowTextBoxList[i].Margin = new System.Windows.Thickness(-1000 + 400 * (i % _ThumbCountPerPage), -110, 0, 70);
                    this.TableGrid.Children.Add(_timeShowTextBoxList[i]);                              


                    _thumbButtonList[i].Content = _thumbInkCanvasList[i];
                    this.InkCanvasLeft.Strokes.Clear();
                }

            }
            else
            {
                this.ButtonPre.Visibility = Visibility.Collapsed;
            }
            this.ButtonNext.Visibility = Visibility.Visible;

        }

        private void ButtonNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (++_PageCurr <= _PageCount)
            {
                foreach (Button thumbBtn in _thumbButtonList)
                {
                    if (this.TableGrid.Children.IndexOf(thumbBtn) > -1)
                    {
                        this.TableGrid.Children.Remove(thumbBtn);
                    }
                }
                //清除原来显示的时间Textbox
                foreach (TextBox textBoxtime in _timeShowTextBoxList)
                {
                    if (this.TableGrid.Children.IndexOf(textBoxtime) > -1)
                    {
                        this.TableGrid.Children.Remove(textBoxtime);
                    }
                }
                int _thumbButtonCountCurr = 0;    //当前显示页中缩略图个数
                if (_ThumbCount % _ThumbCountPerPage == 0)
                {
                    _PageCount = _ThumbCount / _ThumbCountPerPage;
                }
                else
                {
                    _PageCount = _ThumbCount / _ThumbCountPerPage + 1;
                }
                if (_PageCount > _PageCurr)
                {
                    this.ButtonNext.Visibility = Visibility.Visible;
                    _thumbButtonCountCurr = _ThumbCountPerPage;
                }
                else
                {
                    _thumbButtonCountCurr = _ThumbCount % _ThumbCountPerPage;
                    this.ButtonNext.Visibility = Visibility.Collapsed;
                }
                for (int i = _ThumbCountPerPage * (_PageCurr - 1); i < _ThumbCountPerPage * (_PageCurr - 1) + _thumbButtonCountCurr; i++)
                {

                    Grid.SetColumnSpan(_thumbButtonList[i], 2);
                    Grid.SetRow(_thumbButtonList[i], 1);
                    Grid.SetColumn(_thumbButtonList[i], 0);
                    _thumbButtonList[i].Margin = new System.Windows.Thickness(-1000 + 400 * (i % _ThumbCountPerPage), 70, 0, 70);
                    this.TableGrid.Children.Add(_thumbButtonList[i]);

                    //显示起始终止时间                       
                    Grid.SetColumnSpan(_timeShowTextBoxList[i], 2);
                    Grid.SetRow(_timeShowTextBoxList[i], 1);
                    Grid.SetColumn(_timeShowTextBoxList[i], 0);
                    _timeShowTextBoxList[i].Margin = new System.Windows.Thickness(-1000 + 400 * (i % _ThumbCountPerPage), -110, 0, 70);
                    this.TableGrid.Children.Add(_timeShowTextBoxList[i]);                              


                    _thumbButtonList[i].Content = _thumbInkCanvasList[i];
                    this.InkCanvasLeft.Strokes.Clear();
                }

            }
            else
            {
                this.ButtonNext.Visibility = Visibility.Collapsed;
            }
            this.ButtonPre.Visibility = Visibility.Visible;
        }

#region 变量

        //起始时间列表
        public List<Double> StartTimeList
        {
            get { return _startTimeList; }
        }
        //终止时间列表
        public List<Double> EndTimeList
        {
            get { return _endTimeList; }
        }
#endregion
	}
}