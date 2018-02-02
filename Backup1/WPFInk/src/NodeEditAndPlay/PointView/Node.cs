using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFInk
{
    public class Node
    {
        //velocity of node, used to do force-directed layout,added by sunsnowad
        private double velocityX = 0.0f;
        private double velocityY = 0.0f;
        public Force netForce = new Force(0.0, 0.0);

        // public Point point;
        //private int x;
        //private int y;
        public Point point;
        private int degree = 0;
        public int index;
        public int radius = 20;
        public Point linkcenterPoint;
        public int inDegree = 0;
        public int outDegree = 0;
        public int hyper = 0;
        private int order = -1;

        public int Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
            }
        }

        public string videoPath = null;//视频的路径

        public Brush brush = Brushes.Yellow;//初始化时的结点颜色
        
        public Node(Point pt)
        {
            this.point = pt;
        }
        public Node(int x, int y)
        {
            this.point.X = x;
            this.point.Y = y;
        }

        public Node(int x, int y, string strProperty)
        {
            this.point.X = x;
            this.point.Y = y;
            this.Property = strProperty;
        }
        //public int X
        //{
        //    get
        //    {
        //        return x;
        //    }
        //    set
        //    {
        //        x = value;
        //    }
        //}
        //public int Y
        //{
        //    get
        //    {
        //        return y;
        //    }
        //    set
        //    {
        //        y = value;
        //    }
        //}

        public Force NetForce
        {
            get
            {
                return netForce;
            }
            set
            {
                netForce = value;
            }
        }
        public int Degree
        {
            get
            {
                return degree;
            }
            set
            {
                degree = value;
            }
        }

        public double VelocityX
        {
            get
            {
                return velocityX;
            }
            set
            {
                velocityX = value;
            }
        }
        public double VelocityY
        {
            get
            {
                return velocityY;
            }
            set
            {
                velocityY = value;
            }
        }

        public List<SketchInNodeVideo> _sketchInnodevideo = new List<SketchInNodeVideo>(); 

        public string Property = string.Empty;

        public List<Bitmap> bmpList = new List<Bitmap>();

        public void DrawHyperImage(Graphics g)
        {
            //foreach(Bitmap bmp in bmpList)
            //{
            //    bmp.MakeTransparent(Color.White);
            //    g.DrawImage(
            //        bmp,
            //        new Rectangle(point.X - radius / 2, point.Y - radius / 2, radius * 3 / 2, radius * 3 / 2),
            //        new Rectangle(0,0,bmp.Width,bmp.Height),
            //        GraphicsUnit.Pixel);
            //}
            if (bmpList.Count > 0)
                g.DrawEllipse(Pens.Red, point.X - radius / 2, point.Y - radius / 2, radius / 2, radius / 2);
        }
    }
}
