using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk.WZL
{
    /// <summary>
    /// 保存一页的笔迹
    /// </summary>
    public class Page
    {
        public Page()
        {
            content = new Sketch();
        }

        public Sketch content = null;
    }
}
