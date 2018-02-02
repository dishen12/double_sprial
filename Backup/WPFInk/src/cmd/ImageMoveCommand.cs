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
    /// 图片移动
    /// </summary>
    public class ImageMoveCommand : Command
    {
        private MyImage image;
        private double offset_x, offset_y;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="offsetx">移动x距离</param>
        /// <param name="offsety">移动y距离</param>
        public ImageMoveCommand(WPFInk.ink.MyImage image, double offsetx, double offsety)
        {
            this.image = image;
            this.offset_x = offsetx;
            this.offset_y = offsety;
        }

       

        /// <summary>
        /// 执行移动的过程
        /// </summary>
        public void execute()
        {
            image.Left += offset_x;
            image.Top += offset_y;
            image.setLocation((int)image.Left,(int)image.Top);
			image.adjustBound();
        }

        

        /// <summary>
        /// 撤销
        /// </summary>
        public void undo()
        {
            image.Left -= offset_x;
			image.Top -= offset_y;
			image.setLocation((int)image.Left, (int)image.Top);
			image.adjustBound();
        }
    }
}