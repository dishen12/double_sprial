using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFInk.videoSummarization
{
    public class MyStrokeData
    {
        private Point point;
        private string currentTime;
        private string operateType;

        public Point Point
        {
            get { return point; }
            set { point = value; }
        }

        public string CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        public string OperateType
        {
            get { return operateType;}
            set { operateType = value; }
        }

    }
}
