using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;

namespace WPFInk.tool
{

    public enum TimerMode
    {
        Running,
        Pause,
    }

    /// <summary>
    /// 为系统提供计时器服务
    /// </summary>
    public class MyTimer
    {
        private static MyTimer mytimer = null;
        private TimerMode mode = TimerMode.Running;
        private Timer Timer;
        private int seconds = 0;

        public delegate void TimeChanged(string time);
        //public event TimeChanged OnTimeChanged;

        private MyTimer()
        {
            Timer = new Timer(addOneSecond, null, 1000, 1000);
        }

        /// <summary>
        /// 只能获得一个计时器的实例
        /// </summary>
        /// <returns></returns>
        public static MyTimer getInstance()
        {
            if (mytimer == null)
                mytimer = new MyTimer();
            return mytimer;
        }

        /// <summary>
        /// 时间属性
        /// </summary>
        public int Seconds
        {
            set
            {
                seconds = value;
                //int hours = seconds / 3600;
                //int minutes = seconds / 60 - hours * 60;
                //int secs = seconds % 60;
                //string time = hours.ToString() + ":" + minutes.ToString() + ":" + secs.ToString();
                //OnTimeChanged(time);
            }
            get
            {
                return seconds;
            }
        }

        /// <summary>
        /// 计时器处理函数，每过一秒钟，时间增加一秒
        /// </summary>
        /// <param name="state"></param>
        private void addOneSecond(object state)
        {
            if (mode == TimerMode.Running)
                Seconds++;
        }

        /// <summary>
        /// 重新开始计时，时间清零
        /// </summary>
        public void ReStart()
        {
            Timer.Dispose();
            Timer = new Timer(addOneSecond, null, 1000, 1000);
            Seconds = 0;
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        public void Pause()
        {
            mode = TimerMode.Pause;
        }

        /// <summary>
        /// 恢复计时
        /// </summary>
        public void Resume()
        {
            mode = TimerMode.Running;
        }

        /// <summary>
        /// 获得当前的时间
        /// </summary>
        /// <returns>当前时间</returns>
        public int getTime()
        {
            return Seconds;
        }

        /// <summary>
        /// 设置当前的时间
        /// </summary>
        /// <param name="Time">将当前时间设置为Time</param>
        public void setTime(int Time)
        {
            Seconds = Time;
        }
    }
}
