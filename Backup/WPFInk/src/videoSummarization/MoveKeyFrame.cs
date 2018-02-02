using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPFInk.Global;
using System.Windows.Input;
using System.Windows.Ink;
using WPFInk.state;
using System.Windows.Forms;


namespace WPFInk.videoSummarization
{
    public class MoveKeyFrame
    {
        //private KeyFrame keyFrame;
        //private double duration;
        private int currIndex = 0;
        private System.Windows.Forms.Timer extendTimer;

        public System.Windows.Forms.Timer ExtendTimer
        {
            get { return extendTimer; }
            set { extendTimer = value; }
        }
        private SpiralSummarization spiralSummarization;
        private int operationId;//操作类型id，1代表插入帧，2代表隐藏帧，3代表向中心聚集zoomIn，4代表向边缘聚集zoomOut
        //private System.Diagnostics.Stopwatch sw;
        private int showCount;
        private List<int> movePointCountPerTimes = new List<int>();
        private int maxMovePointCount=0;
        private int maxMovePointCountIndex = 0;
        private int minMovePointCount = int.MaxValue;
        private List<int> fromKeyPointIndexes = new List<int>();
        private List<int> toKeyPointIndexes = new List<int>();
        private List<KeyFrame> keyFrames = new List<KeyFrame>();
        private int maxMoveTime = 0;//最大移动次数
        private List<int> restPointCountList = new List<int>();
        private int insertIndex = 0;
        private Stroke showSpiralStroke;//显示出来的螺旋线
        private StylusPointCollection points;
        /// <summary>
        /// 移动批量关键帧动画类
        /// </summary>
        /// <param name="spiralSummarization"></param>
        /// <param name="operationId"></param>
        /// <param name="insertIndex"></param>
        /// <param name="fromKeyPointIndexes"></param>
        /// <param name="toKeyPointIndexes"></param>
        /// <param name="keyFrames"></param>
        public MoveKeyFrame(SpiralSummarization spiralSummarization, int operationId,int insertIndex, List<int> fromKeyPointIndexes, List<int> toKeyPointIndexes,List<KeyFrame> keyFrames)//, int index, KeyFrame keyFrame, int startIndex, int endIndex, double duration,bool isInsert)
        {
            if (spiralSummarization != null)
            {
                spiralSummarization.InkCollector.IsShowUnbrokenKeyFrame = false;
            }
            this.operationId = operationId;
            this.fromKeyPointIndexes = fromKeyPointIndexes;
            this.toKeyPointIndexes = toKeyPointIndexes;
            this.keyFrames = keyFrames;
            this.spiralSummarization = spiralSummarization;
            this.insertIndex = insertIndex;
            showCount = keyFrames.Count;
            this.showSpiralStroke = spiralSummarization.ShowSpiralStroke;
            this.points = spiralSummarization.Points;
            for (int i = 1; i < showCount; i++)
            {
                int movePointCount = Math.Abs(fromKeyPointIndexes[i] - toKeyPointIndexes[i]);
                if (movePointCount > maxMovePointCount)
                {
                    maxMovePointCount = movePointCount;
                    maxMovePointCountIndex = i;
                }
                else if (movePointCount < minMovePointCount && movePointCount > 0)
                {
                    minMovePointCount = movePointCount;
                }
            }
            
            for (int i = 1; i < showCount; i++)
            {
                int movePointCount = Math.Abs(fromKeyPointIndexes[i] - toKeyPointIndexes[i]);
                int movePointCountPerTime = (int)Math.Floor((double)movePointCount / minMovePointCount);
                int restPointCount = movePointCount % minMovePointCount;
                restPointCountList.Add(restPointCount);
                int moveTime=(int)Math.Ceiling((double)movePointCount / movePointCountPerTime);
                if ( moveTime> maxMoveTime)
                {
                    maxMoveTime = moveTime;
                }
                movePointCountPerTimes.Add(movePointCountPerTime);
            }
        }
        /// <summary>
        /// 启动timer
        /// </summary>
        public void move()
        {
            extendTimer = new System.Windows.Forms.Timer();
            extendTimer.Interval = 45;
            switch (operationId)
            {
                case 1:
                    extendTimer.Tick += delegate(object o, EventArgs args)
                    {
                        extendTimerInsert_Tick();
                    };
                    break;
                case 2:
                    extendTimer.Tick += delegate(object o, EventArgs args)
                    {
                        extendTimerHidden_Tick();
                    };
                    break;
                case 3:
                    extendTimer.Tick += delegate(object o, EventArgs args)
                    {
                        extendTimerZoomIn_Tick();
                    };
                    break;
                case 4:
                    extendTimer.Tick += delegate(object o, EventArgs args)
                    {
                        extendTimerZoomOut_Tick();
                    };
                    break;
            }
            extendTimer.Start();
        }
        int tickCount = 0;
        /// <summary>
        /// zoomIn
        /// </summary>
        void extendTimerZoomIn_Tick()
        {            
            tickCount++;            
            for (int i = 1; i < showCount; i++)
            {
                currIndex = fromKeyPointIndexes[i] - tickCount * movePointCountPerTimes[i - 1];
                if (tickCount <= restPointCountList[i - 1])
                {
                    currIndex -= tickCount;
                }
                else
                {
                    currIndex -= restPointCountList[i - 1];
                }
                if (currIndex >= toKeyPointIndexes[i])
                {
                    spiralSummarization.UpdateImage(i, keyFrames[i], currIndex);
                    //if (i == showCount - 1)
                    //{
                    //    int end = spiralSummarization.getEndIndex(currIndex);
                    //    end += 1;
                    //    while (showSpiralStroke.StylusPoints.Count > end)
                    //    {
                    //        showSpiralStroke.StylusPoints.RemoveAt(showSpiralStroke.StylusPoints.Count - 1);
                    //    }
                    //}
                }
                else if (i == showCount - 1 && tickCount >= minMovePointCount)
                {
                    extendTimer.Stop();
                    extendTimer.Dispose();
                    spiralSummarization.InkCollector.IsShowUnbrokenKeyFrame = true;
                    spiralSummarization.IsZooming = false;
                    tickCount = 0;
                    break;
                }

            }
            ((InkState_Summarization)spiralSummarization.InkCollector._state).MoveTimerSecond = 0;
        }
        /// <summary>
        /// zoomOut
        /// </summary>
        void extendTimerZoomOut_Tick()
        {
            tickCount++;
            for (int i = 1; i < showCount; i++)
            {
                currIndex = fromKeyPointIndexes[i] + tickCount * movePointCountPerTimes[i - 1];
                if (tickCount <= restPointCountList[i - 1])
                {
                    currIndex += tickCount;
                }
                else
                {
                    currIndex += restPointCountList[i - 1];
                }
                if (currIndex <= toKeyPointIndexes[i])
                {
                    spiralSummarization.UpdateImage(i, keyFrames[i], currIndex);
                    //if (i == showCount - 1)
                    //{
                    //    int end = spiralSummarization.getEndIndex(currIndex);
                    //    end += 1;
                    //    while (showSpiralStroke.StylusPoints.Count < end)
                    //    {
                    //        showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                    //    }
                    //    //if (currIndex == 1290)
                    //    //{
                    //    //    showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                    //    //    showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                    //    //    showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                    //    //    showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                    //    //}
                    //    //else if (currIndex == 1310)
                    //    //{
                    //    //    showSpiralStroke.StylusPoints.RemoveAt(showSpiralStroke.StylusPoints.Count - 1);
                    //    //    //showSpiralStroke.StylusPoints.RemoveAt(showSpiralStroke.StylusPoints.Count - 1);
                    //    //}
                    //    //else if (currIndex == 1382)
                    //    //{
                    //    //    showSpiralStroke.StylusPoints.RemoveAt(showSpiralStroke.StylusPoints.Count - 1);
                    //    //    //showSpiralStroke.StylusPoints.RemoveAt(showSpiralStroke.StylusPoints.Count - 1);
                    //    //}
                    //}
                }
                else if (i == showCount - 1 && tickCount >= minMovePointCount)
                {
                    extendTimer.Stop();
                    extendTimer.Dispose();
                    spiralSummarization.InkCollector.IsShowUnbrokenKeyFrame = true;
                    spiralSummarization.IsZooming = false;
                    tickCount = 0;
                    break;
                }

            }
            ((InkState_Summarization)spiralSummarization.InkCollector._state).MoveTimerSecond = 0;
        }
        /// <summary>
        /// 插入多关键帧
        /// </summary>
        void extendTimerInsert_Tick()
        {
            tickCount++;
            for (int i = 1; i < showCount; i++)
            {
                currIndex = fromKeyPointIndexes[i] + tickCount * movePointCountPerTimes[i - 1];
                if (tickCount <= restPointCountList[i - 1])
                {
                    currIndex += tickCount;
                }
                else
                {
                    currIndex += restPointCountList[i - 1];
                }
                if (currIndex <= toKeyPointIndexes[i]+1)
                {
                    spiralSummarization.UpdateImage(i + insertIndex, keyFrames[i], currIndex);
                    //if (i == showCount - 1)
                    //{
                    //    try
                    //    {
                    //        int end = spiralSummarization.getEndIndex(currIndex);
                    //        end += 6;
                    //        int removeCount = end - showSpiralStroke.StylusPoints.Count;
                    //        while (removeCount > 0)
                    //        {
                    //            showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                    //            removeCount--;

                    //        }
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        MessageBox.Show(e.Message);
                    //    }
                    //    //while (showSpiralStroke.StylusPoints.Count < end)
                    //    //{
                    //    //    showSpiralStroke.StylusPoints.Add(points[showSpiralStroke.StylusPoints.Count]);
                    //    //}
                    //}
                }
                else if (i == showCount - 1 && tickCount >= minMovePointCount)
                {
                    spiralSummarization.IsZooming = false;
                    Console.WriteLine("spiralSummarization.IsZooming = false;");
                    //MessageBox.Show("tickCount:" + tickCount.ToString() + ",minMovePointCount:" + minMovePointCount);
                    Console.WriteLine("i:" + i.ToString() + "tickCount:" + tickCount.ToString() + ",minMovePointCount" + minMovePointCount);
                    extendTimer.Stop();
                    extendTimer.Dispose();
                    extendTimer = null;
                    spiralSummarization.InkCollector.IsShowUnbrokenKeyFrame = true;
                    tickCount = 0;
                    break;
                }

            }
            ((InkState_Summarization)spiralSummarization.InkCollector._state).MoveTimerSecond = 0;
        }
        /// <summary>
        /// 隐藏插入的关键帧
        /// </summary>
        void extendTimerHidden_Tick()
        {
            tickCount++;
            for (int i = 1; i < showCount; i++)
            {
                currIndex = fromKeyPointIndexes[i] - tickCount * movePointCountPerTimes[i - 1];
                if (tickCount <= restPointCountList[i - 1])
                {
                    currIndex -= tickCount;
                }
                else
                {
                    currIndex -= restPointCountList[i - 1];
                }
                if (currIndex >= toKeyPointIndexes[i])
                {
                    spiralSummarization.UpdateImage(i + insertIndex, keyFrames[i], currIndex);
                    //if (i == showCount - 1)
                    //{
                    //    try
                    //    {
                    //        int end = spiralSummarization.getEndIndex(currIndex);
                    //        end += 6;
                    //        //while (showSpiralStroke.StylusPoints.Count > end)
                    //        //{
                    //        //    showSpiralStroke.StylusPoints.RemoveAt(showSpiralStroke.StylusPoints.Count - 1);
                    //        //} 
                    //        int removeCount = showSpiralStroke.StylusPoints.Count - end;
                    //        while (removeCount > 0)
                    //        {
                    //            showSpiralStroke.StylusPoints.RemoveAt(showSpiralStroke.StylusPoints.Count - 1);
                    //            removeCount--;
                    //        }
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        MessageBox.Show(e.Message);
                    //    }
                    //}
                }
                else if (i == showCount - 1&&tickCount >= minMovePointCount)
                {
                    spiralSummarization.IsZooming = false;
                    //MessageBox.Show("tickCount:" + tickCount.ToString() + ",minMovePointCount" + minMovePointCount);
                    Console.WriteLine("i:"+i.ToString()+"tickCount:" + tickCount.ToString() + ",minMovePointCount" + minMovePointCount);
                    extendTimer.Stop();
                    extendTimer.Dispose();
                    spiralSummarization.InkCollector.IsShowUnbrokenKeyFrame = true;
                    tickCount = 0;
                    break;
                }

            }
            ((InkState_Summarization)spiralSummarization.InkCollector._state).MoveTimerSecond = 0;
        }
    }
}
