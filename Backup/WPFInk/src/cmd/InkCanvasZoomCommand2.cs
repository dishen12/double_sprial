using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFInk.cmd
{
    /// <summary>
    /// 图像缩放命令
    /// </summary>
    public class InkCanvasZoomCommand2 : Command
    {
        private InkCanvas _inkCanvas=new InkCanvas();
        private double scaling;
        private double offsetX;
        //private int step;
        //private ScaleTransform translate;

        public InkCanvasZoomCommand2(InkCanvas _inkCanvas, double scaling,double offsetX)
        {
            this._inkCanvas = _inkCanvas;
            this.scaling = scaling;
            this.offsetX = offsetX;
        }

       

        public void execute()
        {
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.ScaleAt(scaling, scaling, 0, 0);
            m.OffsetX = offsetX;
            this._inkCanvas.RenderTransform = new MatrixTransform(m);

        }

        public InkCanvas InkCanvas
        {
            get { return this._inkCanvas; }
        }

        public void undo()
        {
            TransformGroup transform = (TransformGroup)_inkCanvas.RenderTransform;
            TransformCollection transformcollection = transform.Children;

            transformcollection.RemoveAt(transformcollection.Count - 1);
        }
    }

}