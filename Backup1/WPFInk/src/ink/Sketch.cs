using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Ink;
using WPFInk.video;
using WPFInk.graphic;
using WPFInk.tool;

namespace WPFInk.ink
{
    /// <summary>
    /// 封装所有对stroke的操作
    /// 使得没有直接的Stroke类的操作
    /// </summary>
    public class Sketch
    {
        public List<MyStroke> MyStrokes = new List<MyStroke>();
        public List<MyImage> Images = new List<MyImage>();
		public List<MyRichTextBox> MyRichTextBoxs = new List<MyRichTextBox>();
        public List<MyButton> MyButtons = new List<MyButton>();
        public List<MyArrow> MyArrows = new List<MyArrow>();
		public List<MySmallRectangle> mySmallRectangles= new List<MySmallRectangle>();
		public List<MyGraphic> MyGraphics = new List<MyGraphic>();
		public List<GraphicLinkNode> GraphicLinkNodes = new List<GraphicLinkNode>();
        public int UserID;
        public int InkID;

        /// <summary>
        /// 在sketch中增加一个stroke
        /// </summary>
        /// <param name="stroke"></param>
        public void AddStroke(MyStroke stroke)
        {
            ///增加一个stroke，列表，group等等
            MyStrokes.Add(stroke);
        }

        /// <summary>
        /// 删除一个stroke
        /// </summary>
        /// <param name="stroke"></param>
        public void RemoveStroke(MyStroke stroke)
        {
            ///删除一个stroke
            MyStrokes.Remove(stroke);
        }

        /// <summary>
        /// 增加一个图片
        /// </summary>
        /// <param name="image"></param>
        public void AddImage(MyImage image)
        {
            Images.Add(image);
        }

        /// <summary>
        /// 删除一个图片
        /// </summary>
        /// <param name="_image"></param>
        public void RemoveImage(MyImage _image)
        {
            Images.Remove(_image);
        }

        /// <summary>
        /// 增加一个Text
        /// </summary>
        /// <param name="Text">要增加的text</param>
        public void AddText(MyRichTextBox mrtb)
        {
			MyRichTextBoxs.Add(mrtb);
        }

        /// <summary>
        /// 删除一个Text
        /// </summary>
        /// <param name="Text">要删除的Text</param>
		public void RemoveText(MyRichTextBox mrtb)
        {
			MyRichTextBoxs.Remove(mrtb);
        }

        /// <summary>
        /// 增加一个Button
        /// </summary>
        /// <param name="button">要增加的Button</param>
        public void AddButton(MyButton button)
        {
            MyButtons.Add(button);
        }

        /// <summary>
        /// 删除一个Button
        /// </summary>
        /// <param name="button">要删除的button</param>
        public void RemoveButton(MyButton button)
        {
            MyButtons.Remove(button);
        }

		/// <summary>
		/// 增加一个Arrow
		/// </summary>
		/// <param name="button">要增加的Arrow</param>
		public void AddArrow(MyArrow myArrow)
		{
			MyArrows.Add(myArrow);
		}

		/// <summary>
		/// 删除一个Button
		/// </summary>
		/// <param name="button">要删除的Arrow</param>
		public void RemoveArrow(MyArrow myArrow)
		{
			MyArrows.Remove(myArrow);
		}


		/// <summary>
		/// 增加一个rectangle
		/// </summary>
		/// <param name="button">要增加的Arrow</param>
		public void AddMySmallRectangle(MySmallRectangle msr)
		{
			mySmallRectangles.Add(msr);
		}

		/// <summary>
		/// 删除一个rectangle
		/// </summary>
		/// <param name="button">要删除的Arrow</param>
		public void RemoveMySmallRectangle(MySmallRectangle msr)
		{
			mySmallRectangles.Remove(msr);
		}

		/// <summary>
		/// 在sketch中增加一个stroke
		/// </summary>
		/// <param name="stroke"></param>
		public void AddMyGraphic(MyGraphic myGraphic)
		{
			///增加一个stroke，列表，group等等
			MyGraphics.Add(myGraphic);
		}

		/// <summary>
		/// 删除一个stroke
		/// </summary>
		/// <param name="stroke"></param>
		public void RemoveMyGraphic(MyGraphic myGraphic)
		{
			///删除一个stroke
			MyGraphics.Remove(myGraphic);
		}
		/// <summary>
		/// 添加一个GraphicLinkNode
		/// </summary>
		/// <param name="gln"></param>
		public void AddGraphicLinkNode(GraphicLinkNode gln)
		{
            if (gln != null)
            {
                GraphicLinkNodes.Add(gln);
            }
		}
		/// <summary>
		/// 删除一个GraphicLinkNode
		/// </summary>
		/// <param name="gln"></param>
		public void RemoveGraphicLinkNode(GraphicLinkNode gln)
		{
            if (gln != null)
            {
                GraphicLinkNodes.Remove(gln);
            }
		}
        /// <summary>
        /// 返回stroke对应的mystroke类型的对象
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public MyStroke GetStroke(Stroke stroke)
        {
            foreach (MyStroke mystroke in MyStrokes)
            {
                if (mystroke.Stroke.Equals(stroke))
                {
                    return mystroke;
                }
            }
            return null;
        }
		/// <summary>
		/// 根据graphicLinkNodeID查找对应的GraphicLinkNode
		/// </summary>
		/// <param name="graphicLinkNodeID"></param>
		/// <returns></returns>
		public GraphicLinkNode getGraphicLinkNodeByID(int graphicLinkNodeID)
		{
			foreach (GraphicLinkNode gln in GraphicLinkNodes)
			{
				if (graphicLinkNodeID == gln.GraphicLinkNodeID)
				{
					return gln;
				}
			}
			return null;
		}
		/// <summary>
		/// 根据myGraphicID和selfMyGraphicID查找对应的GraphicLinkNode
		/// </summary>
		/// <param name="graphicLinkNodeID"></param>
		/// <returns></returns>
		public GraphicLinkNode getGraphicLinkNodeByMyGraphicIDAndSelfMyGraphicID(int myGraphicID, int selfMyGraphicID)
		{
			foreach (GraphicLinkNode gln in GraphicLinkNodes)
			{
				if (myGraphicID == gln.MyGraphicID && selfMyGraphicID==gln.SelfMyGraphicID)
				{
					return gln;
				}
			}
			return null;
		}
		/// <summary>
		/// 根据NextGraphicLinkNodeID查找对应的GraphicLinkNode
		/// </summary>
		/// <param name="graphicLinkNodeID"></param>
		/// <returns></returns>
		public GraphicLinkNode getGraphicLinkNodeByNextGraphicLinkNodeID(int NextGraphicLinkNodeID)
		{
			foreach (GraphicLinkNode gln in GraphicLinkNodes)
			{
				if (NextGraphicLinkNodeID!=0&&NextGraphicLinkNodeID == gln.NextGraphicLinkNodeID)
				{
					return gln;
				}
			}
			return null;
		}
		/// <summary>
		/// 根据SelfMyGraphicID获取与之有关的GraphicLinkNode列表
		/// </summary>
		/// <param name="SelfMyGraphicID"></param>
		/// <returns></returns>
		public List<GraphicLinkNode> getGraphicLinkNodesBySelfMyGraphicID(int SelfMyGraphicID)
		{
			List<GraphicLinkNode> glns = new List<GraphicLinkNode>();
			foreach (GraphicLinkNode gln in GraphicLinkNodes)
			{ 
				if (gln.SelfMyGraphicID==SelfMyGraphicID)
				{
					glns.Add(gln);
				}
			}
			return glns;
		}
		/// <summary>
		/// 根据SelfMyGraphicID从MyGraphics获取与SelfMyGraphicID关联的MyGraphic列表
		/// </summary>
		/// <param name="SelfMyGraphicID"></param>
		/// <returns></returns>
		public List<MyGraphic> getMyGraphicsBySelfMyGraphicID(int SelfMyGraphicID)
		{
			List<MyGraphic> mgs = new List<MyGraphic>();
			foreach (GraphicLinkNode gln in GraphicLinkNodes)
			{
				if (gln.SelfMyGraphicID == SelfMyGraphicID)
				{
					mgs.Add(getMyGraphicByID(gln.MyGraphicID));
				}
			}
			return mgs;
		}
		public MyGraphic getMyGraphicByID(int myGraphicID)
		{
			foreach (MyGraphic mg in MyGraphics)
			{
				if (mg.MyGraphicID == myGraphicID)
				{
					return mg;
				}
			}
			return null;
		}
		/// <summary>
        /// 从MyGraphic列表中查找是arrow图形的Mygraphic
		/// </summary>
		/// <returns></returns>
		public List<MyGraphic> getArrowGraphicFromMyGraphics()
		{
            List<MyGraphic> arrowGraphics = new List<MyGraphic>();
			foreach (MyGraphic mg in MyGraphics)
			{
                if (mg.ShapeType == "arrow" || mg.ShapeType == "loopArc" || mg.ShapeType == "polylineArrow"||mg.ShapeType=="loopArcSelf")
				{
                    arrowGraphics.Add(mg);
				}
			}
            return arrowGraphics;
		}
		/// <summary>
        /// 从MyGraphic列表中非arrow的Mygraphic
		/// </summary>
		/// <returns></returns>
        public List<MyGraphic> getNoArrowGraphicsFromMyGraphics()
		{
            List<MyGraphic> arrowGraphics = new List<MyGraphic>();
			foreach (MyGraphic mg in MyGraphics)
			{
                if (mg.ShapeType != "arrow" && mg.ShapeType != "loopArc" && mg.ShapeType != "polylineArrow" && mg.ShapeType != "loopArcSelf")
				{
                    arrowGraphics.Add(mg);
				}
			}
            return arrowGraphics;
		}
		/// <summary>
		/// 查找笔迹所在的MyGraphic
		/// </summary>
		/// <returns></returns>
		public MyGraphic getMyGraphicContrainStroke(Stroke stroke)
		{
            foreach (MyGraphic mg in getNoArrowGraphicsFromMyGraphics())
			{
				if(MathTool.getInstance().isContainRect(mg.Strokes.GetBounds(),stroke.GetBounds()))
				{
					return mg;
				}
			}
			return null;
		}

        /// <summary>
        /// 根据图形中的内容返回MyGraphic对象
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public MyGraphic getMyGraphicByText(string text)
        {
            foreach (MyGraphic mg in MyGraphics)
            {
                if (mg.Text == text && mg.ShapeType != "arrow" && mg.ShapeType != "loopArc" && mg.ShapeType != "polylineArrow" && mg.ShapeType != "loopArcSelf")
                {
                    return mg;
                }
            }
            return null;
        }
        /// <summary>
        /// 获得mg的下一个相连图形
        /// </summary>
        /// <param name="mg"></param>
        /// <returns></returns>
        public MyGraphic getNextMyGraphic(MyGraphic mg)
        {
            return getMyGraphicByID(getGraphicLinkNodeByID(mg.GraphicLinkNodeID).MyGraphicID);
        }

        
    }
}
