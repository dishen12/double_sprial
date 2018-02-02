using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFInk
{
    // <summary>
    /// 代表了视频节点之间的连接，和Line类不同之处在于，Line类主要是连接线的几何属性
    /// 而ConnectLine更关注视频节点间的连接逻辑关系
    /// </summary>
    class ConnectLine
    {
        public ConnectLine(VideoNode start, VideoNode end)
        {
            this.startNode = start;
            this.endNode = end;
        }
        VideoNode startNode = null;

        /// <summary>
        /// 连接线连接的起始节点
        /// </summary>
        public VideoNode StartNode
        {
            get { return startNode; }
            set { startNode = value; }
        }

        VideoNode endNode = null;

        /// <summary>
        /// 连接线连接的结束节点
        /// </summary>
        public VideoNode EndNode
        {
            get { return endNode; }
            set { endNode = value; }
        }

    }
}
