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
using System.Threading;
using System.Windows.Ink;
using WPFInk.cmd;
using WPFInk.mouseGesture;
using WPFInk.ink;
using WPFInk.graphic;
using WPFInk.tool;

namespace WPFInk.state
{
	public class InkState_DrawStrokeInGraphic : InkState
    {        
        //private Nullable<Point> _startpoint = null;
        InkAnalyzer analyze;

        public InkState_DrawStrokeInGraphic(InkCollector inkCollector)
            : base(inkCollector)
        {
            this._inkCollector = inkCollector;
            analyze = new InkAnalyzer();
        }
		
        public override void _presenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        public override void _presenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        public override void _presenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InkCanvas inkCanvas = (InkCanvas)sender;
            Stroke lastStroke = inkCanvas.Strokes[inkCanvas.Strokes.Count - 1];
            if (analyze.RootNode.Strokes.IndexOf(lastStroke) == -1)
            {
                analyze.AddStroke(inkCanvas.Strokes[inkCanvas.Strokes.Count - 1]);
            }
            analyze.BackgroundAnalyze();
            string analyzeStr = analyze.GetRecognizedString();
            MyGraphic MyGraphicContrainStroke = _inkCollector.Sketch.getMyGraphicContrainStroke(lastStroke);
            if (MyGraphicContrainStroke != null)
            {
                if (MyGraphicContrainStroke.textStrokeCollection.IndexOf(lastStroke) == -1)
                {

                    MyGraphicContrainStroke.addTextStroke(lastStroke);
                    MyGraphicContrainStroke.Text = analyzeStr;
                    //Console.WriteLine("analyzeStr:" + analyzeStr);
                }
            }
        }


    }
}
