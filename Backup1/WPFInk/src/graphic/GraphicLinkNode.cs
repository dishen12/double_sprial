using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk.graphic
{
	public class GraphicLinkNode
    {
        #region 变量
        private int graphicLinkNodeID;//本节点ID
		private int myGraphicID;//与selfMyGraphic相关的图形ID
		private string rule;//规则
		private int nextGraphicLinkNodeID=0;//下一个结点ID
		private int selfMyGraphicID;//自身指向的MyGraphic
        #endregion

        #region 构造函数
        public GraphicLinkNode(int graphicLinkNodeID,int selfMyGraphicID, int myGraphicID,string rule)
		{
			this.graphicLinkNodeID = graphicLinkNodeID;
			this.selfMyGraphicID = selfMyGraphicID;
			this.myGraphicID = myGraphicID;
			this.rule = rule;
        }
        #endregion

        #region 访问器
        public int SelfMyGraphicID
		{
			get { return selfMyGraphicID; }
			set { selfMyGraphicID = value; }
		}

		public string Rule
		{
			get { return rule; }
			set { rule = value; }
		}

		public int NextGraphicLinkNodeID
		{
			get { return nextGraphicLinkNodeID; }
			set { nextGraphicLinkNodeID = value; }
		}

		public int MyGraphicID
		{
			get { return myGraphicID; }
			set { myGraphicID = value; }
		}


		public int GraphicLinkNodeID
		{
			get { return graphicLinkNodeID; }
			set { graphicLinkNodeID = value; }
        }
        #endregion
    }
}
