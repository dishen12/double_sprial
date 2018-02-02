using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;              
using System.Windows.Controls;

namespace WPFInk.video
{
    public class VideoCollector
    {

        
        private double _thumbHeightWidthRate;

        #region  variable
        public double ThumbHeightWidthRate
        {
            set { _thumbHeightWidthRate = value; }
            get { return _thumbHeightWidthRate; }
        }

        public void MaxizeOtherControlPanel(ControlPanel cp)
        {
            cp.Height = 20;
        }

        #endregion
    }
}
