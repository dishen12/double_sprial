using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;

namespace WPFInk.videoSummarization
{
    public class KeyFramesAnnotation
    {
        #region 构造函数

        #endregion

        #region 私有变量
        private StrokeCollection strokes=new StrokeCollection();//annotation
        private double width=300;//视频注释框宽度
        private double height=300;//视频注释框高度
        public List<int> relatedKeyFrameIndexes = new List<int>();//拥有该注释的关键帧
        #endregion

        #region 封装变量
        /// <summary>
        /// 视频注释笔迹
        /// </summary>
        public StrokeCollection Strokes
        {
            get { return strokes; }
            set { strokes = value; }
        }
        /// <summary>
        /// 视频注释框宽度
        /// </summary>
        public double Width
        {
            get { return width; }
            set { width = value; }
        }
        /// <summary>
        /// 视频注释框高度
        /// </summary>
        public double Height
        {
            get { return height; }
            set { height = value; }
        }
        #endregion

        #region 成员函数
        
        #endregion
    }
}
