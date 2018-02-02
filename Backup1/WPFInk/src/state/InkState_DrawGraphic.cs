using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Input;
using System.Threading;
using System.Windows.Ink;
using WPFInk.cmd;
using WPFInk.mouseGesture;
using WPFInk.ink;
using WPFInk.graphic;
using WPFInk.tool;
using WPFInk.Global;

namespace WPFInk.state
{
	public class InkState_DrawGraphic : InkState
    {
        #region 常量
        private const int DWELLTIME = 2;//图形识别停顿时间阈值
        #endregion

        #region 私有变量
        private Point lastPoint;
        private MultiCustomGesture multiCustomGesture;
        #endregion

        #region 构造函数
        public InkState_DrawGraphic(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            multiCustomGesture = new MultiCustomGesture(inkCollector, _inkCanvas);
        }
        #endregion

        #region 事件
        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _inkCanvas.CaptureMouse();//捕捉鼠标
            lastPoint = e.GetPosition(_inkCanvas);
            if (multiCustomGesture.seconds < DWELLTIME)
            {
                multiCustomGesture.seconds = 0;
                multiCustomGesture.timer.Stop();
                if (GlobalValues.MyGraphic_IsDirectionRecognize && multiCustomGesture.strokes.Count == 0)//画的是第一条笔迹时开始捕捉笔迹
                {
                    if (multiCustomGesture.Gesture == null)
                    {
                        multiCustomGesture.createGesture();
                    }
                    multiCustomGesture.Gesture.StartCapture((int)lastPoint.X, (int)lastPoint.Y);
                }
            }            
            
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
			if (lastPoint.X != 0 && lastPoint.Y != 0)
			{
				_inkCanvas.CaptureMouse();//捕捉鼠标
                Point currentPoint = e.GetPosition(_inkCanvas); 
                //GraphicMathTool.getInstance().drawPoint(new StylusPoint(currentPoint.X, currentPoint.Y), 5, Colors.Red, _inkCanvas);
                if (GlobalValues.MyGraphic_IsDirectionRecognize)
                {
                    multiCustomGesture.Gesture.Capturing((int)currentPoint.X, (int)currentPoint.Y);
                }
				lastPoint = currentPoint;
			}
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Stroke lastStroke = _inkCanvas.Strokes.Last();
            if (lastPoint.X != 0 && lastPoint.Y != 0)
            {
                multiCustomGesture.strokes.Add(lastStroke);                
                //_inkCollector._mainPage.message.Content += "@"+multiCustomGesture.strokes.Count.ToString()+"@";
                lastPoint.X = 0;
                lastPoint.Y = 0;
                _inkCanvas.ReleaseMouseCapture();
                multiCustomGesture.timer.Start();
            }
        }
        #endregion
    }
}

                