using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using WPFInk.ink;

namespace WPFInk.video
{
    public class MyArrow
    {
        private Path arrow = null;
        private MyButton preMyButton = null;
        private MyButton nextMyButton = null;
        private Point startPoint;
        private Point endPoint;
        private bool isDeleted = false;

        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }

        public Point EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }


        public Point StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }
 

        public MyArrow(Path arrow)
        {
            this.arrow = arrow;
        }

        public MyButton NextMyButton
        {
            get { return nextMyButton; }
            set { nextMyButton = value; }
        }

        public MyButton PreMyButton
        {
            get { return preMyButton; }
            set { preMyButton = value; }
        }

        public Path Arrow
        {
            get { return arrow; }
            set { arrow = value; }
        }

		public void Dispose()
		{
			GC.SuppressFinalize(this);

		}
    }
}
