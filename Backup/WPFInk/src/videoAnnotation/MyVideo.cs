using System;
using System.Collections.Generic;
using System.Linq;    
using System.Windows.Media;
using System.Text;

namespace WPFInk.video
{
    public class MyVideo
    {
        private string _videoPath = null;
        private SolidColorBrush _background = null;

        public SolidColorBrush Background
        {
            get { return _background; }
            set { _background = value; }
        }
        public string VideoPath
        {
            get { return _videoPath; }
            set { _videoPath = value; }
        }
		public void Dispose()
		{
			GC.SuppressFinalize(this);

		}
    }
}
