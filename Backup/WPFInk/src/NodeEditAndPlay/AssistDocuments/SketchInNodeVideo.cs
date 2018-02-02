using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk
{
    public class SketchInNodeVideo
    {
        private int startIndex;
        private int endIndex;
        private int startFrame;
        private int endFrame;

        public int _startIndex
        {
            get 
            {
                return startIndex;
            }
            set
            {
                startIndex = value;
            }
        }

        public int _endIndex
        {
            get
            {
                return endIndex;
            }
            set
            {
                endIndex = value;
            }
        }

        public int _startFrame
        {
            get
            {
                return startFrame;
            }
            set
            {
                startFrame = value;
            }
        }

        public int _endFrame
        {
            get
            {
                return endFrame;
            }
            set
            {
                endFrame = value;
            }
        }
    }
}
