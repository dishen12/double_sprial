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
    /// 图片旋转
    /// </summary>
    public class ImageRotateCommand2 : Command
    {
        private Image image;
        private double angle;
        private int step;
        private RotateTransform translate; 
        StylusPoint center;

        /// <summary>
        /// 图片image旋转angle角度
        /// </summary>
        /// <param name="image">被旋转的图片</param>
        /// <param name="angle">旋转的角度</param>
        public ImageRotateCommand2(WPFInk.ink.MyImage image, double angle,StylusPoint center)
        {
            this.image = image.Image;
            this.angle = angle;
            this.center = center; 
            TransformGroup transform = (TransformGroup)image.Image.RenderTransform;
            step = transform.Children.Count;
        }

        /// <summary>
        /// 当鼠标左键单击时，创建一个transform
        /// </summary>
        public void CreateTransform()
        {
            //创建一个旋转的transform
            translate = new RotateTransform();
            translate.Angle = angle;
            translate.CenterX = center.X;// -image.Width;
            translate.CenterY = center.Y;// -image.Height;

            //添加到移动的组里面
            TransformGroup transform = (TransformGroup)image.RenderTransform;
            transform.Children.Add(translate);
            image.RenderTransform = transform;
            //Console.WriteLine("旋转中心："+translate.CenterX+","+translate.CenterY);
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void execute()
        {
            TransformGroup transform = (TransformGroup)image.RenderTransform;
            TransformCollection transformcollection = transform.Children;
            
            if (step > transformcollection.Count)
            {
                translate = new RotateTransform();
                translate.CenterX = center.X;
                translate.CenterY = center.Y;
                transform.Children.Add(translate);
            }
            else
            {
                //找到最后一个translate，把它旋转
                translate = (RotateTransform)transformcollection[transform.Children.Count - 1];
            }
            translate.Angle += angle;
        }

        /// <summary>
        /// 是否真的旋转
        /// </summary>
        /// <returns></returns>
        public bool isRealRotate()
        {
            TransformGroup transform = (TransformGroup)image.RenderTransform;
            TransformCollection transformcollection = transform.Children;

            //找到最后一个translate，判断是否发生旋转
            RotateTransform translate = (RotateTransform)transformcollection[transform.Children.Count - 1];
            if (Math.Abs(translate.Angle) < 0.0005)
            {
                transformcollection.RemoveAt(transformcollection.Count - 1);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void undo()
        {
            TransformGroup transform = (TransformGroup)image.RenderTransform;
            TransformCollection transformcollection = transform.Children;

            transformcollection.RemoveAt(transformcollection.Count - 1);
        }
    }
}
