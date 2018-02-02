using System;
using System.Collections.Generic;
using Microsoft.Ink;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace WPFInk.WZL
{
    // <summary>
    /// 负责stroke的tidy up
    /// 实现2D Tidy-up of Freehand Sketch with Overtracing上的sketch处理步骤
    /// </summary>
    public class TidyUp
    {
        public TidyUp(Sketch sketch, Panel panel, InkOverlay drawing)
        {
            this.sketch = sketch;
            this.inkPanel = panel;
            this.drawingInk = drawing;

            ///初始化skeleton stroke的属性
            this.Skeleton_Attibute.Color = Skeleton_Color;
            this.Skeleton_Attibute.Width = Skeleton_Width;
        }

        public void addStroke(Stroke stroke)
        {
            List<Stroke> changes = new List<Stroke>();//需要刷新的stroke
            MyStroke newstroke = new MyStroke(stroke);
            newstroke.setBoundingBox();//YHY-090412
            Sketch currentpage = sketch.currentpage.Value.content; //当前页

            ///线性测试
            if (tool.LinearityTest(newstroke) == Linearity.Line)
            {
                //新增加的stroke是直线
                ///straight-line segment fitting
                foreach (StrokeGroup strokegroup in currentpage.groupList)
                {
                    if (strokegroup.TYPE == GroupType.DEFAULT)
                    {
                        MyStroke mstroke = strokegroup.strokeList.First.Value;
                        if ((tool.DistanceTest(newstroke, mstroke) == TESTRESULT.GROUP)
                        && (tool.AngleTest(newstroke, mstroke) == TESTRESULT.GROUP))
                        {
                            //通过了距离测试和角度测试
                            strokegroup.addStroke(newstroke);
                            drawingInk.Ink.DeleteStroke(strokegroup.Skeleton);
                            strokegroup.Skeleton = drawingInk.Ink.CreateStroke(strokegroup.SkeletonPoints);
                            strokegroup.Skeleton.DrawingAttributes = this.Skeleton_Attibute;
                            strokegroup.setBoundingBox();//YHY-090412
                            changes.Add(strokegroup.Skeleton);
                            break;
                        }
                    }
                }
                ///如果新增加的笔画没有找到组织，那么将给它单独建一个组
                if (!newstroke.isInGroup)
                {
                    //如果没有通过距离测试和角度测试
                    //Seperate Line
                    StrokeGroup sg = new StrokeGroup(GroupType.DEFAULT);
                    sketch.groupIndex++;
                    sg.groupID = sketch.groupIndex; //YHY-090413 增加一个标志
                    sg.addStroke(newstroke);
                    sg.Skeleton = drawingInk.Ink.CreateStroke(sg.SkeletonPoints);
                    sg.Skeleton.DrawingAttributes = this.Skeleton_Attibute;
                    //YHY-090410
                    sg.GRAPH = Graphic.Line;
                    sg.setBoundingBox();
                    sg.geometry = new G_Line(newstroke.startPoint, newstroke.endPoint);//YHY-090415

                    currentpage.addGroup(sg);

                    //判断新加入的strokegroup代表的图元和历史图元之间的约束关系
                    refreshConstraint(sg);

                    //yhy-090409
                    //将新建的group加入到skechContextDG中去
                    if (sketch.sketchContextDG.isEmpty())
                    {
                        sketch.sketchContextDG.nodeIndex++;
                        SKContextDGNode node = new SKContextDGNode(sg, sg.StartTime, sketch.sketchContextDG.nodeIndex);
                        node.groupID = sg.groupID;
                        sketch.sketchContextDG.newHead(node);
                        sketch.sketchContextDG.firstNode = node;
                        sketch.sketchContextDG.lastNode = node;
                    }
                    else
                    {
                        sketch.sketchContextDG.addNode(sg);
                    }
                }

                //Groupping: add curve object to strokeGroup and sketch 
                //YHY-090409
                ////////////////////////////////////////////////////////////////////////////////
                //YHY-090414
                //Just for test, ReGroup
                //sketchReGroupping();
                //drawBoundingBoxtoTestGrouppingResult();
                //////////////////////////////////////////////////////////////////////////////////


                /* StrokeGroup ngroup = newstroke.group;

                foreach (StrokeGroup group in currentpage.groupList)
                {
                    if (group == ngroup)
                        continue;

                    if (tool.distanceP2P(ngroup.startPoint, group.startPoint) < Threshold_Distance)
                    {
                        ngroup.startPoint = group.startPoint;
                        ngroup.computeSkeletonFromEnds();
                    }
                    else if (tool.distanceP2P(ngroup.startPoint, group.endPoint) < Threshold_Distance)
                    {
                        ngroup.startPoint = group.endPoint;
                        ngroup.computeSkeletonFromEnds();
                    }
                    else if (tool.distanceP2P(ngroup.endPoint, group.startPoint) < Threshold_Distance)
                    {
                        ngroup.endPoint = group.startPoint;
                        ngroup.computeSkeletonFromEnds();
                    }
                    else if (tool.distanceP2P(ngroup.endPoint, group.endPoint) < Threshold_Distance)
                    {
                        ngroup.endPoint = group.endPoint;
                        ngroup.computeSkeletonFromEnds();
                    }
                }*/

            }

            else if (tool.LinearityTest(newstroke) == Linearity.Curve)
            {
                //如果线性测试的结果是曲线

                //TODO::ToolForStroke.circle(et al).judge function
                //YHY-090408
                //先不考虑曲线补笔等草图特效的问题，假设所有曲线相关的sketch都是单笔的
                //判断是否为circle
                bool isCircle = false;
                isCircle = tool.JudgeCircle(newstroke);

                //YHY-090410
                //创建新的group，先不考虑曲线补笔等情况
                if (!newstroke.isInGroup)
                {
                    StrokeGroup sg = new StrokeGroup(GroupType.DEFAULT);
                    sketch.groupIndex++;
                    sg.groupID = sketch.groupIndex;
                    sg.addStroke(newstroke);
                    sg.setBoundingBox();

                    if (isCircle)
                    {
                        //TODO::暂时统一都认为是圆，以后需要改成椭圆
                        sg.GRAPH = Graphic.Circle;
                        Point p = new Point();
                        double r;
                        p.X = sg.boundingBox.Left + sg.boundingBox.Width / 2;
                        p.Y = sg.boundingBox.Top + sg.boundingBox.Height / 2;
                        r = System.Math.Min(sg.boundingBox.Height, sg.boundingBox.Width) / 2;
                        sg.geometry = new G_Cricle(p, r);//YHY-090415                        
                    }
                    else
                    {
                        sg.GRAPH = Graphic.Curve;
                    }

                    //sg.Skeleton = inkpicture.Ink.CreateStroke(sg.SkeletonPoints); //TODO: Get curve skeleton
                    //sg.Skeleton.DrawingAttributes = this.Skeleton_Attibute;
                    currentpage.addGroup(sg);

                    //捕捉约束
                    //判断新加入的strokegroup代表的图元和历史图元之间的约束关系
                    refreshConstraint(sg);


                    //yhy-090409
                    //将新建的group加入到skechContextDG中去
                    if (sketch.sketchContextDG.isEmpty())
                    {
                        sketch.sketchContextDG.nodeIndex++;
                        SKContextDGNode node = new SKContextDGNode(sg, sg.StartTime, sketch.sketchContextDG.nodeIndex);
                        node.groupID = sg.groupID;
                        sketch.sketchContextDG.newHead(node);
                        sketch.sketchContextDG.firstNode = node;
                        sketch.sketchContextDG.lastNode = node;
                    }
                    else
                    {
                        sketch.sketchContextDG.addNode(sg);
                    }

                }

                //Groupping: add curve object to strokeGroup and sketch 
                //YHY-090408
                ////////////////////////////////////////////////////////////////////////////////
                //YHY-090414
                //Just for test, ReGroup
                //   sketchReGroupping();
                //   drawBoundingBoxtoTestGrouppingResult();
                //////////////////////////////////////////////////////////////////////////////////



                //TODO::constraint capture function
                //如果未归为prior group，则需判断新添笔迹(group)与原来笔迹的约束关系
                //YHY-090416
                //判断新添加strokegroup里的图元和其余历史图元之间的关系


            }

            currentpage.addStroke(newstroke);
            ///对纸张进行局部更新
            if (stroke != null)
            {
                inkPanel.Invalidate(tool.InkSpaceToPixelRect(inkPanel.Handle, drawingInk.Renderer, stroke.GetBoundingBox()));
                foreach (Stroke st in changes)
                    inkPanel.Invalidate(tool.InkSpaceToPixelRect(inkPanel.Handle, drawingInk.Renderer, st.GetBoundingBox()));
            }

        }

        public void refreshConstraint(StrokeGroup sg)
        {
            bool isL_L_Vertical = false;
            bool isL_L_Intersect = false;
            bool isL_L_Parallel = false;
            bool isL_C_Tangent = false;
            bool isL_C_Intersect = false;
            bool isL_C_LineInCircle = false;
            //TODO::加入更多的约束关系


            if (sg.GRAPH == Graphic.Line)
            {
                G_Line geo1 = (G_Line)sg.geometry;

                foreach (SKContextDGNode nd in sketch.sketchContextDG.DGNodeList)
                {
                    Graphic type = nd.strokeGroup.GRAPH;

                    switch (type)
                    {
                        case Graphic.Line:  //L_L
                            G_Line geo2 = (G_Line)nd.strokeGroup.geometry;
                            if (geo2 != null)
                            {
                                isL_L_Vertical = new G_L_L_Constraints().L_L_Vertical(geo1, geo2);
                                isL_L_Intersect = new G_L_L_Constraints().L_L_Intersect(geo1, geo2);
                                isL_L_Parallel = new G_L_L_Constraints().L_L_Parallel(geo1, geo2);
                                //保存约束
                                ConstraintElement elem = new ConstraintElement();
                                elem.strokeGroup1 = sg;
                                elem.strokeGroup2 = nd.strokeGroup;
                                if (isL_L_Intersect)
                                {
                                    elem.ctype = ConstraintType.Intersect;
                                    sketch.m_pConsList.Add(elem);
                                    // MessageBox.Show("Intersecting");
                                }
                                if (isL_L_Parallel)
                                {
                                    elem.ctype = ConstraintType.Parallel;
                                    sketch.m_pConsList.Add(elem);
                                    //  MessageBox.Show("Parallel");
                                }
                                if (isL_L_Vertical)
                                {
                                    elem.ctype = ConstraintType.Vertical;
                                    sketch.m_pConsList.Add(elem);
                                    //  MessageBox.Show("Vertical");
                                }
                            }
                            break;
                        case Graphic.Circle:  //L_C
                            G_Cricle geo3 = (G_Cricle)nd.strokeGroup.geometry;
                            if (geo3 != null)
                            {
                                isL_C_Tangent = new G_L_C_Constraints().L_C_Tangent();
                                isL_C_Intersect = new G_L_C_Constraints().L_C_Intesect(geo1, geo3);
                                isL_C_LineInCircle = new G_L_C_Constraints().L_C_LineInCircle(geo1, geo3);
                                //保存约束
                                ConstraintElement elem1 = new ConstraintElement();
                                elem1.strokeGroup1 = sg;
                                elem1.strokeGroup2 = nd.strokeGroup;
                                if (isL_C_Tangent)
                                {
                                    elem1.ctype = ConstraintType.Tangent;
                                    sketch.m_pConsList.Add(elem1);
                                    // MessageBox.Show("Intersecting");
                                }
                                if (isL_C_Intersect)
                                {
                                    elem1.ctype = ConstraintType.Intersect;
                                    sketch.m_pConsList.Add(elem1);
                                    //  MessageBox.Show("Parallel");
                                }
                                if (isL_C_LineInCircle)
                                {
                                    elem1.ctype = ConstraintType.In;
                                    sketch.m_pConsList.Add(elem1);
                                    //  MessageBox.Show("Vertical");
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
                if (sg.GRAPH == Graphic.Circle)//Circle
                {
                    G_Cricle geo4 = (G_Cricle)sg.geometry;

                    foreach (SKContextDGNode nd in sketch.sketchContextDG.DGNodeList)
                    {
                        Graphic type = nd.strokeGroup.GRAPH;
                        switch (type)
                        {
                            case Graphic.Line: //L_C
                                G_Line geo5 = (G_Line)nd.strokeGroup.geometry;
                                if (geo5 != null)
                                {
                                    isL_C_Tangent = new G_L_C_Constraints().L_C_Tangent();
                                    isL_C_Intersect = new G_L_C_Constraints().L_C_Intesect(geo5, geo4);
                                    isL_C_LineInCircle = new G_L_C_Constraints().L_C_LineInCircle(geo5, geo4);
                                    //保存约束
                                    ConstraintElement elem1 = new ConstraintElement();
                                    elem1.strokeGroup1 = sg;
                                    elem1.strokeGroup2 = nd.strokeGroup;
                                    if (isL_C_Tangent)
                                    {
                                        elem1.ctype = ConstraintType.Tangent;
                                        sketch.m_pConsList.Add(elem1);
                                        // MessageBox.Show("Intersecting");
                                    }
                                    if (isL_C_Intersect)
                                    {
                                        elem1.ctype = ConstraintType.Intersect;
                                        sketch.m_pConsList.Add(elem1);
                                        //  MessageBox.Show("Parallel");
                                    }
                                    if (isL_C_LineInCircle)
                                    {
                                        elem1.ctype = ConstraintType.In;
                                        sketch.m_pConsList.Add(elem1);
                                        //  MessageBox.Show("Vertical");
                                    }
                                }
                                break;
                            case Graphic.Circle: //C_C
                                break;
                            default:
                                break;
                        }
                    }
                }

        }

        public void sketchReGroupping()
        {
            SKContextDGNode head = sketch.sketchContextDG.firstNode;
            SKContextDGNode nextNode = new SKContextDGNode();
            SKContextDGNode curNode = new SKContextDGNode();
            Sketch currentpage = sketch.currentpage.Value.content; //当前页

            /*
              1.	从SKContextDG中取出输入时间戳最早的节点nodeCur，标记为Head；
              2.	从nodeCur出发，找到其tPointer指针所指向的下一个节点nodeNext；
              3.	如果当前nodeNext不为空(TT)，执行步骤3.1
            */
            curNode = head;


            for (nextNode = curNode.tPointer; ; nextNode = nextNode.tPointer)
            {
                if (nextNode != null) //TT
                {
                    /*
                       3.1	对于当前的nodeNext，判断其sPointer指针是否为空；
                       3.2	如果sPointer非空(ST)，判断sPointer所指节点nodePre是否与nodeCur属于同一个group。
                            如果是，则nodeCur和nodeNext之间是强关联类型(TT&ST)，nodeNext与nodeCur属于同一个group，
                            将nodeNext设为nodeCur，重复步骤2；
                            如果nodePre与nodeCur不属于同一个group，则将nodeNext与nodePre归为同一个group，
                            将当前nodeCur标记为Tail，从SKContextDG中寻找输入时间戳大于nodeNext的下一个节点记为nodeNext；
                       3.3	如果sPointer为空(SF)，则nodeCur与nodeNext之间是弱关联II类型(TT&SF)，
                            视nodeNext为新group的头节点，将其标记为Head，同时，将nodeCur标记为Tail；
                     */
                    int tmp = nextNode.sPointer.Count;
                    if (nextNode.sPointer.Count > 0)
                    {
                        //如果nextnode和curnode空间邻近，则它们属于同一group
                        bool isContain = nextNode.sPointer.Contains(curNode);
                        if (isContain) // curNode 和 nextNode是强关联(TT && ST)
                        {
                            nextNode.groupID = curNode.groupID;//属于同一group
                            curNode = nextNode;
                        }
                        else
                        {
                            //nodeCur与nodeNext之间是弱关联II类型(TT&SF)
                            //检测nextnode和历史node的关系，判断它应该归属于哪一个group
                            //先处理简单情况，nextNode只和属于同一个group的历史节点有空间邻近的关系
                            foreach (SKContextDGNode snode in nextNode.sPointer)
                            {
                                nextNode.groupID = snode.groupID;
                            }
                            curNode = nextNode;

                        }
                    }
                    else//spointer = null 没有与之空间邻近的节点，独立成为一个group
                    {
                        //nodeNext为新group的头节点，将其标记为Head
                        sketch.sketchContextDG.DGHeadList.Add(nextNode);
                        curNode = nextNode;

                    }
                }
                else //TF
                {
                    /*
                       4. 如果当前nodeNext为空(TF)，从SKContextDG中寻找输入时间戳大于nodeCur节点的最早输入节点，
                          令其为nodeNext，
                          4.1 如果nodeNext非空，判断其sPointer指针是否为空；
                          4.2 如果sPointer非空(ST)，判断sPointer所指节点nodePre是否与nodeCur属于同一个group，
                              如果是，则nodeCur和nodeNext之间是弱关联I类型(TF&ST)，nodeNext与nodeCur属于同一个group，
                              将nodeNext设为nodeCur，重复步骤2；
                          4.3 如果sPointer为空(SF)，则nodeCur与nodeNext之间无关联，视nodeNext为新group的头节点，将其标记为Head，同时，将nodeCur标记为Tail
                    */
                    //先找到nextNdoe节点
                    foreach (SKContextDGNode nd in sketch.sketchContextDG.DGHeadList)
                    {
                        if (nd.timeStamp > curNode.timeStamp)//HeadList都是按序进入的，故只需找到第一个比curNode时间戳大的节点即可
                            nextNode = nd;

                        if (nextNode != null)
                            break;
                    }
                    if (nextNode == null)
                        break;

                    if (nextNode.sPointer != null)
                    {
                        bool isContain = nextNode.sPointer.Contains(curNode);
                        if (isContain)
                        {
                            nextNode.groupID = curNode.groupID;
                            curNode = nextNode;
                        }
                        else
                        {
                            foreach (SKContextDGNode snode in nextNode.sPointer)
                            {
                                nextNode.groupID = snode.groupID;
                            }
                            curNode = nextNode;
                        }
                    }
                    else
                    {
                        //spointer = null 没有与之空间邻近的节点，独立成为一个group
                        //nodeNext为新group的头节点，将其标记为Head
                        sketch.sketchContextDG.DGHeadList.Add(nextNode);
                        curNode = nextNode;
                    }
                }
            }

            //更新semantic group
            int nGroupCount = -1;
            foreach (SKContextDGNode nd in sketch.sketchContextDG.DGNodeList)
            {
                if (nd.groupID > nGroupCount)
                    nGroupCount = nd.groupID;
            }

            sketch.semanticGroupList.Clear();

            for (int i = 1; i <= nGroupCount; i++)
            {
                SemanticGroup semanticSg = new SemanticGroup();
                sketch.semanticGroupList.Add(semanticSg);
                semanticSg.semanticGroupID = i;
            }

            foreach (SKContextDGNode nn in sketch.sketchContextDG.DGNodeList)
            {
                foreach (SemanticGroup smsg in sketch.semanticGroupList)
                {
                    if ((nn.groupID == smsg.semanticGroupID) && (!smsg.groupList.Contains(nn.strokeGroup)))
                    {
                        smsg.groupList.Add(nn.strokeGroup);
                        smsg.setBoundingBox();
                    }
                }
            }

        }

        public void drawBoundingBoxtoTestGrouppingResult()
        {
            //画出ReGroup后的group的包围盒，测试group是否正确

            //统计有多少个group
            SKContextDGNode head = sketch.sketchContextDG.firstNode;
            SKContextDGNode nextNode = new SKContextDGNode();
            SKContextDGNode curNode = new SKContextDGNode();

            curNode = head;
            nextNode = curNode.tPointer;

            int nGroupCount = -1;

            foreach (SKContextDGNode nd in sketch.sketchContextDG.DGNodeList)
            {
                if (nd.groupID > nGroupCount)
                    nGroupCount = nd.groupID;
            }

            Rectangle[] boundingBoxArray;
            boundingBoxArray = new Rectangle[nGroupCount];
            for (int i = 0; i < nGroupCount; i++)
            {
                boundingBoxArray[i] = new Rectangle(1000000, 1000000, -2000000, -2000000);
            }
            int index = 0;

            foreach (SKContextDGNode nd in sketch.sketchContextDG.DGNodeList)
            {
                index = nd.groupID;
                int Left = boundingBoxArray[index - 1].Left;
                int Right = boundingBoxArray[index - 1].Right;
                int Top = boundingBoxArray[index - 1].Top;
                int Bottom = boundingBoxArray[index - 1].Bottom;


                Rectangle rect = nd.strokeGroup.boundingBox;
                Console.Write("node" + nd.ID + "boundingbox:" + "(" + rect.Left + "," + rect.Top + "," + rect.Width + "," + rect.Height + ")");

                if (rect.Left < Left)
                    Left = rect.Left;
                if (rect.Right > Right)
                    Right = rect.Right;
                if (rect.Top < Top)
                    Top = rect.Top;
                if (rect.Bottom > Bottom)
                    Bottom = rect.Bottom;

                boundingBoxArray[index - 1] = new Rectangle(Left, Top, System.Math.Abs(Right - Left), System.Math.Abs(Bottom - Top));

            }

            //绘制出所有的group的包围盒
            Graphics g = inkPanel.CreateGraphics();
            Pen mypen = new Pen(Color.Blue);
            for (int i = 0; i < nGroupCount; i++)
            {
                Rectangle rt = new ToolForStroke().InkSpaceToPixelRect(inkPanel.Handle, drawingInk.Renderer, boundingBoxArray[i]);
                g.DrawRectangle(mypen, rt);
                Console.Write("Group" + i + "boundingbox:" + "(" + boundingBoxArray[i].Left + "," + boundingBoxArray[i].Top + "," + boundingBoxArray[i].Width + "," + boundingBoxArray[i].Height + ")");
            }

        }



        /// <summary>
        /// elements
        /// </summary>
        private ToolForStroke tool = new ToolForStroke();
        public Sketch sketch;
        public Panel inkPanel;
        public InkOverlay drawingInk;



        ///constants
        private const int Skeleton_Width = 50;
        private Color Skeleton_Color = Color.Green;
        private DrawingAttributes Skeleton_Attibute = new DrawingAttributes();
    }
}
