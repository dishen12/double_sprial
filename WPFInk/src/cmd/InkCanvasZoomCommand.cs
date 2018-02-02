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
    public class InkCanvasZoomCommand : Command
    {
        private InkCanvas _inkCanvas=new InkCanvas();
        private double scaling;
        private int step;
        private ScaleTransform translate;

        public InkCanvasZoomCommand(InkCanvas _inkCanvas, double scaling)
        {
            foreach (UIElement _uIElement in _inkCanvas.Children)
            {
                this._inkCanvas.Children.Add(_uIElement);
            }
            foreach (Stroke stroke in _inkCanvas.Strokes)
            {
                this._inkCanvas.Strokes.Add(stroke);
            }
            this.scaling = scaling;

            TransformGroup transform = (TransformGroup)this._inkCanvas.RenderTransform;
            step = transform.Children.Count;
        }

        /// <summary>
        /// 当鼠标左键单击时，创建一个transform
        /// </summary>
        public void CreateTransform()
        {
            //创建一个移动的transform
            translate = new ScaleTransform();
            translate.ScaleX = scaling;
            translate.ScaleY = scaling;
            translate.CenterX = _inkCanvas.Width / 2;
            translate.CenterY = _inkCanvas.Height / 2;


            //添加到移动的组里面
            TransformGroup transform = (TransformGroup)_inkCanvas.RenderTransform;
            transform.Children.Add(translate);
        }

        public void execute()
        {
            TransformGroup transform = (TransformGroup)_inkCanvas.RenderTransform;
            TransformCollection transformcollection = transform.Children;

            if (step > transform.Children.Count)
            {
                translate = new ScaleTransform();

                transform.Children.Add(translate);
            }
            else
            {
                //找到最后一个translate，把它移动
                translate = (ScaleTransform)transformcollection[transform.Children.Count - 1];
            }
            translate.ScaleX = scaling;
            translate.ScaleY = scaling;

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