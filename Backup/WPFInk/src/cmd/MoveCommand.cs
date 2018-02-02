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
using WPFInk.tool;
namespace WPFInk.cmd
{
    /// <summary>
    /// 移动命令
    /// </summary>
    public class MoveCommand : Command
    {
		private MyStroke _myStroke;
        private double offset_x, offset_y;

        /// <summary>
        /// 构造函数
        /// 初始化要处理的strokecollection和移动的向量
        /// </summary>
        /// <param name="strokeCollection"></param>
        /// <param name="offset_x"></param>
        /// <param name="offset_y"></param>
        public MoveCommand(MyStroke myStroke,double offset_x,double offset_y)
        {
			this._myStroke = myStroke;
            this.offset_x = offset_x;
            this.offset_y = offset_y;
        }

        public void execute()
        {
			MathTool.getInstance().MoveStroke(_myStroke.Stroke, offset_x, offset_y);
        }

        public void undo()
        {           
			MathTool.getInstance().MoveStroke(_myStroke.Stroke, -offset_x, -offset_y);
        }
    }
}
