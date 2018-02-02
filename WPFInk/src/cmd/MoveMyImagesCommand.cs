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
using System.Collections.Generic;
using WPFInk.ink;


namespace WPFInk.cmd
{
    /// <summary>
    /// 图片移动
    /// </summary>
    public class MoveMyImagesCommand : Command
    {
        private List<MyImage> _myImages;
        private double offset_x, offset_y;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="offsetx">移动x距离</param>
        /// <param name="offsety">移动y距离</param>
        public MoveMyImagesCommand(List<MyImage> myImages, double offsetx, double offsety)
        {
            this._myImages = myImages;
            this.offset_x = offsetx;
            this.offset_y = offsety;
        }

       

        /// <summary>
        /// 执行移动的过程
        /// </summary>
        public void execute()
        {
            foreach (MyImage mi in _myImages)
            {
                mi.Left += offset_x;
                mi.Top += offset_y;
                mi.setLocation((int)mi.Left, (int)mi.Top);
                mi.adjustBound();
            }
        }

        

        /// <summary>
        /// 撤销
        /// </summary>
        public void undo()
        {
            foreach (MyImage mi in _myImages)
            {
                mi.Left -= offset_x;
                mi.Top -= offset_y;
                mi.setLocation((int)mi.Left, (int)mi.Top);
                mi.adjustBound();
            }
        }
    }
}