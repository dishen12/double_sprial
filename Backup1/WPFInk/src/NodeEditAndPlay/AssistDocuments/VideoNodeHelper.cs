using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFInk
{
    class VideoNodeHelper
    {
        /// <summary>
        /// 判断某个点是否在某个Node之内
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsPointInNode(Point pt, VideoNode node)
        {
            return IsPointInRectangle(pt, GetVideoNodeRect(node));
        }

        public static bool IsAnyNodeSelected(List<VideoNode> listNode)
        {
            foreach (VideoNode node in listNode)
            {
                if (node.Selected == true)
                    return true;
            }
            return false;
        }

        public static Rectangle GetVideoNodeRect(VideoNode node)
        {
            Rectangle rect = new Rectangle(
                node.ptLocation.X,
                node.ptLocation.Y,
                node.Image.Width,
                node.Image.Height);
            return rect;
        }

        public static bool IsPointInRectangle(Point pt, Rectangle rect)
        {
            if (pt.X >= rect.Left
                && pt.X <= rect.Right
                && pt.Y >= rect.Top
                && pt.Y <= rect.Bottom)
                return true;
            return false;
        }
        /// <summary>
        /// 查找距离某个点最近的Node
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="maxDis">查找的最大距离</param>
        /// <returns></returns>
        public static VideoNode GetNearestNode(List<VideoNode> nodes, Point pt, int maxDis)
        {
            int dis = int.MaxValue;
            VideoNode nearestNode = null;
            foreach (VideoNode node in nodes)
            {
                int X = node.ptLocation.X + node.Image.Width / 2;
                int Y = node.ptLocation.Y + node.Image.Height / 2;
                int curDis = (X - pt.X) * (X - pt.X) + (Y - pt.Y) * (Y - pt.Y);
                if (
                    curDis < dis
                    && curDis < (maxDis + node.Image.Width / 2) * (maxDis + node.Image.Width / 2)
                    )
                {
                    dis = curDis;
                    nearestNode = node;
                }
            }
            return nearestNode;
        }

        /// <summary>
        /// 单位是度
        /// </summary>
        /// <param name="ptStart"></param>
        /// <param name="ptEnd"></param>
        /// <returns></returns>
        public static float GetPtLinkDegree(Point ptStart, Point ptEnd)
        {
            int x = ptEnd.X - ptStart.X;
            int y = ptEnd.Y - ptStart.Y;

            float result = PiToDegree(Math.Atan2(y, x));

            if (result < 0)
                result += 360;

            return result;
        }


        public static float GetPtDistance(Point ptStart, Point ptEnd)
        {
            return (float)Math.Sqrt((ptEnd.X - ptStart.X) * (ptEnd.X - ptStart.X) + (ptEnd.Y - ptStart.Y) * (ptEnd.Y - ptStart.Y));
        }

        public static float PiToDegree(float pi)
        {
            return (float)(pi * 180 / Math.PI);
        }

        public static float PiToDegree(double pi)
        {
            return (float)(pi * 180 / Math.PI);
        }

        public static float DegreeToPi(float degreee)
        {
            return (float)(degreee * Math.PI / 180);
        }
    }
}
