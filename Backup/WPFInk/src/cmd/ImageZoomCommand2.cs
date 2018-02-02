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
    public class ImageZoomCommand2 : Command
    {
        private Image image;
        private double scaling;
        private int step;
        private ScaleTransform translate;

        public ImageZoomCommand2(WPFInk.ink.MyImage image, double scaling)
        {
            this.image = image.Image;
            this.scaling = scaling;

            TransformGroup transform = (TransformGroup)image.Image.RenderTransform;
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
            translate.CenterX = image.Width / 2;
            translate.CenterY = image.Height / 2;


            //添加到移动的组里面
            TransformGroup transform = (TransformGroup)image.RenderTransform;
            transform.Children.Add(translate);
        }

        public void execute()
        {
            TransformGroup transform = (TransformGroup)image.RenderTransform;
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
            //image.Width *= scaling;
            //image.Height *= scaling;
            //Console.WriteLine("缩放后的宽度和高度：" + image.Width + "," + image.Height);
        }

        public void undo()
        {
            TransformGroup transform = (TransformGroup)image.RenderTransform;
            TransformCollection transformcollection = transform.Children;

            transformcollection.RemoveAt(transformcollection.Count - 1);
        }
    }

}