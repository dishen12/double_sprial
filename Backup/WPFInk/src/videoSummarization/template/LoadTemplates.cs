using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.Global;
using System.Windows;

namespace WPFInk.template
{
    public class LoadTemplates
    {
        ShareMemory sm;
        public LoadTemplates()
        {
            try
            {
                unsafe
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Reset();
                    sw.Start();
                    int size = 60000;// 2310 * 60000;
                    for (int i = 0; i < 2310; i++)
                    {
                        sm = new ShareMemory();
                        sm.Init("template"+i.ToString(), size);
                        byte[] bs = new byte[size];
                        sm.Read(ref bs, 0, size);
                        GlobalValues.templates.Add(new List<byte>(bs));
                    }
                    sw.Stop();
                    Console.WriteLine("LoadTemplates总需要时间：" + sw.ElapsedMilliseconds + "ms");
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("LoadTemplates初始化失败！ "+e.Message);
            }
        }
    }
}
