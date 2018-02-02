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
using WPFInk.graphic;
using WPFInk.ink;
using WPFInk.tool;
using WPFInk.Global;

namespace WPFInk.cmd
{
    /// <summary>
    /// 移动命令
    /// </summary>
	public class MyGraphicsMoveCommand : Command
    {
        private List<MyGraphic> _myGraphicsMove;
        private double offset_x, offset_y;
		private List<MyGraphic> MyGraphics;
		private InkCollector _inkCollector;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strokeCollection"></param>
        /// <param name="offset_x"></param>
        /// <param name="offset_y"></param>
		public MyGraphicsMoveCommand(List<MyGraphic> myGraphicsMove, double offset_x, double offset_y, InkCollector inkCollector)
        {
			this._myGraphicsMove = myGraphicsMove;
            this.offset_x = offset_x;
            this.offset_y = offset_y;
            MyGraphics = inkCollector.Sketch.MyGraphics;
			this._inkCollector = inkCollector;
        }

        public void execute()
        {
            //移动需要移动的graphic
			foreach (MyGraphic mg in _myGraphicsMove)
			{
				MyGraphicMoveCommand mgmc = new MyGraphicMoveCommand(mg, offset_x, offset_y, _inkCollector);
				mgmc.execute();
			}
            //找出要移动的graphic中不是箭头的graphic
            List<MyGraphic> MyGraphicsNoArrow=new List<MyGraphic>();
            StrokeCollection strokesMove=new StrokeCollection();
            foreach (MyGraphic mg in _myGraphicsMove)
            {
                if (mg.ShapeType != "arrow" && mg.ShapeType != "loopArc" && mg.ShapeType != "polylineArrow" && mg.ShapeType != "loopArcSelf")
                {
                    strokesMove.Add(mg.Strokes);
                    MyGraphicsNoArrow.Add(mg);
                }
            }
            Rect boundMove = strokesMove.GetBounds();//获取需要移动的graphic的矩形边框
            //检查其他不需要移动的graphic与移动的graphic之间的距离是否小到了最小阈值
            Rect boundOther;//不需要移动的graphic的矩形边框
            foreach (MyGraphic mg in MyGraphics)
            {
                if (_myGraphicsMove.IndexOf(mg) == -1)
                {
                    boundOther = mg.Strokes.GetBounds();
                    if (MathTool.getInstance().distanceR2R(boundOther,boundMove)<= GlobalValues.MyGraphic_SpacingDistance)
                    {
                        //MessageBox.Show("嘿，太近了");
                        List<MyGraphic> MyGraphicsOther = new List<MyGraphic>();
                        MyGraphicsOther.Add(mg);
                        MyGraphicsMoveCommand mgmc = new MyGraphicsMoveCommand(MyGraphicsOther, offset_x, offset_y,  _inkCollector);
                        mgmc.execute();
                    }
                }
            }
			
        }
		
        public void undo()
        {
			foreach (MyGraphic mg in _myGraphicsMove)
			{
				MyGraphicMoveCommand mgmc = new MyGraphicMoveCommand(mg, -offset_x, -offset_y,  _inkCollector);
				mgmc.execute();
			}
        }
		
    }
}
