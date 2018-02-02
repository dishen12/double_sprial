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
using WPFInk.ink;

namespace WPFInk.cmd
{
   

    /// <summary>
    /// 图片旋转
    /// </summary>
    public class ImageRotateCommand : Command
    {
        private MyImage myimage;
        private double preAngle;

        /// <summary>
        /// 图片image旋转angle角度
        /// </summary>
        /// <param name="image">被旋转的图片</param>
        /// <param name="angle">旋转的角度</param>
        public ImageRotateCommand(MyImage image,double preAngle)
        {
            this.myimage = image;
            this.preAngle = preAngle;
        }

       
        /// <summary>
        /// 执行
        /// </summary>
        public void execute()
        {  
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.RotateAt(-myimage.Angle, myimage.Width / 2, myimage.Height / 2);
            this.myimage.Image.RenderTransform = new MatrixTransform(m);
            this.myimage.Bound.RenderTransform = new MatrixTransform(m);
        }

        

        /// <summary>
        /// 撤销
        /// </summary>
        public void undo()
        {
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.RotateAt(preAngle, myimage.Width / 2, myimage.Height / 2);
            this.myimage.Image.RenderTransform = new MatrixTransform(m);
            this.myimage.Bound.RenderTransform = new MatrixTransform(m);
        }
    }
}
