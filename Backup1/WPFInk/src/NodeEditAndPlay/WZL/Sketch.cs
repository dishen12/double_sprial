using System;
using System.Collections.Generic;
using Microsoft.Ink;
using System.Text;

namespace WPFInk.WZL
{
    /// <summary>
    /// 定义Sketch的类
    /// </summary>
    public class Sketch
    {
        public Sketch()
        {

        }

        //////////////////////////////////////////////////////////////////////////////////
        ///operations
        ///

        ///<summary>
        /// 向sketch中增加stroke
        /// </summary>
        /// <param name="mystroke">需要增加的stroke</param>
        public void addStroke(MyStroke mystroke)
        {
            this.strokeList.Add(mystroke);
            this.laststroke = mystroke;
        }

        /// <summary>
        /// 将stroke从sketch中删除
        /// </summary>
        /// <param name="mystroke"></param>
        public void removeStroke(MyStroke mystroke)
        {
            this.strokeList.Remove(mystroke);
        }

        /// <summary>
        /// 向sketch中增加strokegroup
        /// </summary>
        /// <param name="strokegroup"></param>
        public void addGroup(StrokeGroup strokegroup)
        {
            groupList.Add(strokegroup);
        }

        ///<summary>
        ///将group删除掉
        ///</summary>
        public void deleteGroup(StrokeGroup strokegroup)
        {
            foreach (MyStroke stroke in strokegroup.strokeList)
            {
                stroke.inkstroke.Ink.DeleteStroke(stroke.inkstroke);
                strokeList.Remove(stroke);
            }
            groupList.Remove(strokegroup);
        }

        /// <summary>
        /// 将mystroke从草图中删除
        /// 当stroke在某个strokegroup中的时候，将整个strokegroup删除
        /// </summary>
        /// <param name="mystroke"></param>
        public void deleteStroke(MyStroke mystroke)
        {
            if (!mystroke.isInGroup)
            {
                mystroke.inkstroke.Ink.DeleteStroke(mystroke.inkstroke);
                strokeList.Remove(mystroke);
            }
            else
            {
                if (mystroke.group != null)
                {
                    deleteGroup(mystroke.group);
                }
            }
            if (mystroke == laststroke)
                laststroke = strokeList.ToArray()[strokeList.Count - 1];
        }

        /// <summary>
        /// 找到和tablet pc上相对应的mystroke
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns></returns>
        public MyStroke findStroke(Stroke stroke)
        {
            foreach (MyStroke ms in strokeList)
            {
                if (ms.inkstroke.Id.Equals(stroke.Id))
                {
                    return ms;
                }
            }
            return null;
        }

        //////////////////////////////////////////////////////////////////////////////////
        ///对于草图页面的操作
        ///

        ///<summary>
        /// 向sketch中增加一页
        ///</summary>
        public Page addPage()
        {
            Page page = new Page();
            pageList.AddLast(page);
            currentpage = pageList.Last;
            pageNo = pageList.Count;
            return currentpage.Value;
        }

        /// <summary>
        /// 从中删除掉一页
        /// </summary>
        /// <param name="page">需要删掉的page</param>
        public void deletePage(Page page)
        {
            if (pageList.Count >= 1)
            {
                pageList.Remove(page);
                currentpage = currentpage.Previous;
                pageNo--;
            }
        }

        /// <summary>
        /// 返回下一页
        /// </summary>
        /// <returns>返回下一页</returns>
        public Page getNextPage()
        {
            if (currentpage.Next != null)
            {
                pageNo++;
                return (currentpage = currentpage.Next).Value;
            }
            else
                return null;
        }

        /// <summary>
        /// 返回前一页
        /// </summary>
        /// <returns></returns>
        public Page getPreviousPage()
        {
            if (currentpage.Previous != null)
            {
                pageNo--;
                return (currentpage = currentpage.Previous).Value;
            }
            else
                return null;
        }

        /// <summary>
        /// 将page页面在InkPicture中打开
        /// </summary>
        /// <param name="inkpicture"></param>
        /// <param name="page"></param>
        public void LoadPage(InkPicture inkpicture, Page page)
        {
            foreach (MyStroke mystroke in page.content.strokeList)
            {
                Stroke s = inkpicture.Ink.CreateStroke(mystroke.points);
                s.DrawingAttributes = mystroke.DrawingAttributes;
                mystroke.inkstroke = s;
            }
        }


        //////////////////////////////////////////////////////////////////////////////////
        ///attributes
        //保存strokegroup和stroke
        public List<Sketch> sketchList = new List<Sketch>();
        public List<StrokeGroup> groupList = new List<StrokeGroup>();
        public List<MyStroke> strokeList = new List<MyStroke>();
        public InkPicture inkpicture;
        public TidyUp tidyup;

        //YHY-090415
        //临时，保存semantic group
        public List<SemanticGroup> semanticGroupList = new List<SemanticGroup>();

        //YHY-090410
        //保存sketch的绘制过程信息SketchContextDG
        public SKContextDG sketchContextDG = new SKContextDG();
        public int groupIndex = 0;

        //YHY-090415
        //保存sketch的几何约束信息
        public List<ConstraintElement> m_pConsList = new List<ConstraintElement>();

        //分页的列表，当前页
        public LinkedList<Page> pageList = new LinkedList<Page>();
        public LinkedListNode<Page> currentpage = null;
        public int pageNo = 0;
        public MyStroke laststroke = null;
    }
}
