using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFInk
{
    /// <summary>
    /// 这个类主要描述连接线的几何特征，逻辑关系放在ConnectLine类里面
    /// </summary>
    class MyLine
    {
        Point startPoint;

        public Point StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }
        Point endPoint;

        public Point EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        public MyLine(Point startPt, Point endPt)
        {
            this.startPoint = startPt;
            this.endPoint = endPt;
        }
    }
}
