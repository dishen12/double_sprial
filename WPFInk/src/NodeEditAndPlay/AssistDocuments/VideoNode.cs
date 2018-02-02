using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WPFInk
{
    class VideoNode
    {
        #region
        Point _ptLocation;
        // 指示代表视频的节点在画布上的位置
        public Point ptLocation
        {
            get { return _ptLocation; }
            set { _ptLocation = value; }
        }
        string _strImagePath = null;
        // 指示代表视频的节点的图像的路径
        public string strImagePath
        {
            get { return _strImagePath; }
            set { _strImagePath = value; }
        }

        Image _Image = null;
        // 指示代表视频的节点的图像
        public Image Image
        {
            get { return _Image; }
            set { _Image = value; }
        }
        bool _Selected = false;

        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }

        string videoFilePath = string.Empty;

        public string VideoFilePath
        {
            get { return videoFilePath; }
            set { videoFilePath = value; }
        }
        #endregion

        Rectangle boundingBox;

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        public VideoNode(int x, int y, Image image, string filePath)
        {
            this.ptLocation = new Point(x, y);
            this.Image = image;
            this.videoFilePath = filePath;
        }

        public List<Bitmap> annoBmps = new List<Bitmap>();
        /// <summary>
        ///  指的是视频上面不动的那种草图注释
        /// </summary>
        public List<CJVideoStaticStroke> listStaticStrokes = new List<CJVideoStaticStroke>();
    }
}
