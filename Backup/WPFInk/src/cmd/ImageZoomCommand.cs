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
    /// 图像缩放命令
    /// </summary>
    public class ImageZoomCommand : Command
    {
        private MyImage myImage;
        private double scaling;

        public ImageZoomCommand(MyImage image, double scaling)
        {
            this.myImage = image;
            this.scaling = scaling;
        }

        
        public void execute()
        {
            myImage.Width *= scaling;
            myImage.Height *= scaling;
        }

        public void undo()
        {
            myImage.Width *= 1/scaling;
            myImage.Height *= 1/scaling;
        }
    }

}