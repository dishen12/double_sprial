using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFInk.WZL
{
    //YHY-090409
    //记录sketch的过程信息，主要是时间、空间邻近关系
    public class SKContextDGNode
    {
        public StrokeGroup strokeGroup;
        public SKContextDGNode tPointer;
        public List<SKContextDGNode> sPointer = new List<SKContextDGNode>();
        public DateTime timeStamp;
        public int groupID;
        public int ID;

        public SKContextDGNode(StrokeGroup sg, DateTime time, int nodeIndex)
        {
            strokeGroup = sg;
            timeStamp = time;
            ID = nodeIndex;
            tPointer = null;
        }
        public SKContextDGNode()
        {
        }

        public SKContextDGNode getTPoint()
        {
            return tPointer;
        }

        public void setTPoint(SKContextDGNode node)
        {
            tPointer = node;
        }

        public List<SKContextDGNode> getSPoint()
        {
            return sPointer;
        }

        public DateTime getTimeStamp()
        {
            return timeStamp;
        }

        public void setTimeStamp(DateTime time)
        {
            timeStamp = time;
        }
    }

    public class SKContextDG
    {
        public SKContextDG()
        {
            firstNode = null;
            lastNode = null;
            nodeIndex = 0;
        }

        public void newHead(SKContextDGNode node)
        {
            DGHeadList.Add(node);
            DGNodeList.Add(node);
        }

        public double P2P(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public double distanceStroketoStroke(MyStroke s1, MyStroke s2)
        {
            double minDist = 1000000;

            foreach (Point pt1 in s1.points)
            {
                foreach (Point pt2 in s2.points)
                {
                    double dist = P2P(pt1, pt2);
                    if (dist < minDist)
                        minDist = dist;
                }

            }


            //以下被封代码的距离计算公式来自于chiu 1998 paper
            /* double dx, dy;
               double a = 1.2; //阈值， 需要确定

               dx = s2.boundingBox.Left - (s1.boundingBox.Left + s1.boundingBox.Width);
               if (dx < 0)
                   dx = 0;
               dy = s2.boundingBox.Top - (s1.boundingBox.Top + s1.boundingBox.Height);
               if (dy < 0)
                   dy = 0;
               dist = dx + a * dy; */

            //YHY-090412
            //Own

            /*    计算包围盒之间的最小距离方法 
                  Point p1 = new Point();
                  Point p2 = new Point();
                  double dist;

                  //================================
                  p1.X = s1.boundingBox.Left;
                  p1.Y = s1.boundingBox.Top;
                  //
                  p2.X = s2.boundingBox.Left;
                  p2.Y = s2.boundingBox.Top;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Right;
                  p2.Y = s2.boundingBox.Top;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Right;
                  p2.Y = s2.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Left;
                  p2.Y = s2.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;

                  //================================
                  p1.X = s1.boundingBox.Right;
                  p1.Y = s1.boundingBox.Top;
                   //
                  p2.X = s2.boundingBox.Left;
                  p2.Y = s2.boundingBox.Top;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Right;
                  p2.Y = s2.boundingBox.Top;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Right;
                  p2.Y = s2.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Left;
                  p2.Y = s2.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;

                  //================================
                  p1.X = s1.boundingBox.Left;
                  p1.Y = s1.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                   //
                  p2.X = s2.boundingBox.Left;
                  p2.Y = s2.boundingBox.Top;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Right;
                  p2.Y = s2.boundingBox.Top;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Right;
                  p2.Y = s2.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Left;
                  p2.Y = s2.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;

                  //================================
                  p1.X = s1.boundingBox.Right;
                  p1.Y = s1.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                   //
                  p2.X = s2.boundingBox.Left;
                  p2.Y = s2.boundingBox.Top;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Right;
                  p2.Y = s2.boundingBox.Top;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Right;
                  p2.Y = s2.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;
                  //
                  p2.X = s2.boundingBox.Left;
                  p2.Y = s2.boundingBox.Bottom;
                  dist = P2P(p1, p2);
                  if (dist < minDist)
                      minDist = dist;*/

            return minDist;

        }

        public double distance(StrokeGroup sg1, StrokeGroup sg2)
        {
            double mindist = 10000;
            double dist;

            foreach (MyStroke stroke1 in sg1.strokeList)
            {
                foreach (MyStroke stroke2 in sg2.strokeList)
                {
                    dist = distanceStroketoStroke(stroke1, stroke2);
                    if (dist < mindist)
                        mindist = dist;
                }
            }

            return mindist;
        }

        public void addNode(StrokeGroup sg)
        {

            /*
             1.	如果SKContextDG为空，创建关于s新节点nodeS，算法结束；
             2.	创建新节点nodeS，计算笔迹s与前一输入笔迹s’的输入时间差deltTime；
             3.	如果deltTime < ThresholdTime，将前一输入笔迹s所对应的节点的tPointer指针指向nodeS；             
             4.	顺次遍历SKContextDG的历史节点，计算nodeS与历史节点的空间距离dist，根据dist’值判断二者之间的空间关系R；
             5.	如果R= ST，在两个节点之间建立空间关联链接，重复步骤4直至遍历完所有节点。
             */

            nodeIndex++;
            SKContextDGNode node = new SKContextDGNode(sg, sg.StartTime, nodeIndex);
            DGNodeList.Add(node);
            node.groupID = sg.groupID;


            double thresholdTime = 1.2;       //TODO::阈值的确定
            double thresholdSpatial = 1000;   //TODO::阈值的确定


            //空间邻近关系建立
            double dist = -1;
            dist = distance(sg, lastNode.strokeGroup);
            if ((dist > thresholdSpatial) && (!DGHeadList.Contains(node)))
                newHead(node);

            foreach (SKContextDGNode preNode in DGNodeList)
            {
                if (preNode != node)
                {
                    StrokeGroup preSg = null;
                    preSg = preNode.strokeGroup;
                    dist = distance(sg, preSg);
                    if ((dist < thresholdSpatial) && (!node.sPointer.Contains(preNode)))
                        node.sPointer.Add(preNode);
                }
            }


            //时间邻近关系建立
            double delta = (long)((TimeSpan)(sg.StartTime - lastNode.timeStamp)).TotalSeconds;
            if (delta < thresholdTime)
            {
                //时间邻近
                lastNode.tPointer = node;
            }
            else if (!DGHeadList.Contains(node))
            {
                newHead(node);
            }

            lastNode = node;
        }


        public void removeNode()
        {

        }

        public void clear()
        {
        }

        public SKContextDGNode findSKContextDGNode()
        {
            return null;
        }

        public bool isEmpty()
        {
            if (nodeIndex == 0)
                return true;

            return false;
        }

        //Attribute
        public SKContextDGNode firstNode;
        public SKContextDGNode lastNode;
        public int nodeIndex;
        public List<SKContextDGNode> DGHeadList = new List<SKContextDGNode>();
        public List<SKContextDGNode> DGNodeList = new List<SKContextDGNode>();
    }
}
