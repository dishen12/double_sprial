using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk.src.videoSummarization
{
    class other
    {
        #region 构造函数

        #endregion

        #region 私有变量

        #endregion

        #region 封装变量

        #endregion

        #region 成员函数
        void calcTime()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            //..........
            sw.Stop();
            Console.WriteLine("需要时间：" + sw.ElapsedMilliseconds + "ms");

        }
        #endregion
    }
}
