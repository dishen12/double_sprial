using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Shapes;


namespace WPFInk.mouseGesture
{
    public class MouseGesture
    {
        #region 常量
        /// <summary>
        /// 鼠标方向扇区
        /// </summary>
        public const int DEFAULT_SECTORS = 8;
        /// <summary>
        /// 最小精确度,以像素表示
        /// </summary>
        public const int DEFAULT_PRECISION = 8;
        /// <summary>
        /// 有效等级,越小越精确,但是对于画出来的图形相似度要求较高
        /// </summary>
        public const int DEFAULT_FIABILITY = 20;
        #endregion

        #region 成员变量
        private ArrayList moves;//鼠标手势     int
        private System.Windows.Input.StylusPoint lastPoint;//最后鼠标位置
        private ArrayList gestures;//需要匹配的手势  GestureProperties
        private GenericRect rect;//鼠标手势轨迹大小
        private ArrayList points;//鼠标轨迹点   Point

        public ArrayList Points
        {
            get { return points; }
            set { points = value; }
        }

        private double sectorRad;//一个扇区的弧度
        private ArrayList anglesMap;//弧度向量表  int
        #endregion

        #region 事件
        public delegate void GestureMatchDelegate(MouseGestureEventArgs args);
        public event GestureMatchDelegate GestureMatchEvent;

        //public delegate void GestureNoMatchDelegate();
        //public event GestureNoMatchDelegate GestureNoMatchEvent;
        public System.Windows.Input.StylusPointCollection polyPoints = new System.Windows.Input.StylusPointCollection();
        #endregion

        public MouseGesture()
        {
            gestures = new ArrayList();
			Init();
        }
        public void Init()
        {
            BuildAngleMap();
        }
        /// <summary>
        /// 添加定义的手势
        /// </summary>
        /// <param name="present">手势代表的字符</param>
        /// <param name="gesture">手势数据，8个方向，从右开始为0，顺时针指定</param>
        /// <param name="match">匹配的回调,可以为null</param>
        public void AddGesture(string present, string gesture, MatchHandler.matchHandler match)
        {
            int[] g = new int[gesture.Length];
            char[] gestureArray = gesture.ToCharArray();
            for (int i = 0; i < gesture.Length; i++)
            {
                g[i] = gestureArray[i].ToString().Equals(".") ? -1 : int.Parse(gestureArray[i].ToString());
            }
            gestures.Add(new GestureProperties(present, g, match));
        }
        /// <summary>
        /// 建立弧度表
        /// </summary>
        private void BuildAngleMap()
        {
            sectorRad = Math.PI * 2 / DEFAULT_SECTORS;
            anglesMap = new ArrayList();

            //精度步进，100
            double step = Math.PI * 2 / 100;

            int sector;
            for (double i = -sectorRad / 2; i <= Math.PI * 2 - sectorRad / 2; i += step)
            {
                sector = (int)Math.Floor((i + sectorRad / 2) / sectorRad);
                anglesMap.Add(sector);
            }
        }
        /// <summary>
        /// 开始手势捕获
        /// </summary>
        /// <param name="mx">鼠标相对于绘图区的X坐标</param>
        /// <param name="my">鼠标相对于绘图区的Y坐标</param>
        public void StartCapture(int mx,int my)
        {
            moves = new ArrayList();
            points = new ArrayList();

            rect = new GenericRect(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);

            lastPoint = new System.Windows.Input.StylusPoint(mx, my);
        }
        /// <summary>
        /// 鼠标手势捕获中
        /// </summary>
        /// <param name="mx">鼠标相对于绘图区的X坐标</param>
        /// <param name="my">鼠标相对于绘图区的Y坐标</param>
        public void Capturing(int mx,int my)
        {
            int difx = mx - (int)lastPoint.X;
			int dify = my - (int)lastPoint.Y;

            int sqDist = difx * difx + dify * dify;
            int sqPrec = (int)(DEFAULT_PRECISION * DEFAULT_PRECISION);
            if (sqDist > sqPrec)
            {
                points.Add(new System.Windows.Input.StylusPoint(mx, my));
                AddMove(difx, dify);
                lastPoint.X = mx;
                lastPoint.Y = my;

                if (mx < rect.MinX)
                    rect.MinX = mx;
                if (mx > rect.MaxX)
                    rect.MaxX = mx;
                if (my < rect.MinY)
                    rect.MinY = my;
                if (my > rect.MaxY)
                    rect.MaxY = my;
            }
        }
        /// <summary>
        /// 添加手势数据
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        private void AddMove(int dx, int dy)
        {
            double angle = Math.Atan2(dy, dx) + sectorRad / 2;
            if (angle < 0)
                angle += Math.PI * 2;
            int no = (int)Math.Floor(angle / (Math.PI * 2) * 100);//计算在弧度表中对应的向量编号
            moves.Add(anglesMap[no]);
        }
        /// <summary>
        /// 停止鼠标手势捕获
        /// </summary>
        public void StopCapture()
        {
            MatchGesture();
        }
        private void MatchGesture()
        {
            int bestCost = 1000000;
            int cost=0;
            int[] gest;
            int[] imoves = new int[moves.Count];
            for (int i = 0; i < moves.Count; i++)
            {
                //Console.WriteLine((int)moves[i]);
                if (i>0&&i + 1 < imoves.Length)
                {
                    if (((int)moves[i] == 7 && (int)moves[i + 1] != 6 && (int)moves[i + 1] != 0 && (int)moves[i + 1] != 7) || ((int)moves[i + 1] == 7 && (int)moves[i] != 6 && (int)moves[i] != 0 && (int)moves[i] != 7))
                    {
                        polyPoints.Add((System.Windows.Input.StylusPoint)points[i]);
                        polyPoints.Add((System.Windows.Input.StylusPoint)points[i + 1]);
                    }
                    else if((int)moves[i] != 7&&(int)moves[i+1] != 7&&Math.Abs(((int)moves[i + 1] + 1) % 8 - ((int)moves[i] + 1) % 8) > 1)
                    {
                        polyPoints.Add((System.Windows.Input.StylusPoint)points[i]);
                        polyPoints.Add((System.Windows.Input.StylusPoint)points[i + 1]);
                    }
                }

            }
            for (int i = 0; i < imoves.Length; i++)
            {
                imoves[i] = (int)moves[i];
            }
            System.Windows.Input.StylusPoint[] ppoints = new System.Windows.Input.StylusPoint[points.Count];
            for (int i = 0; i < ppoints.Length; i++)
            {
                ppoints[i] = (System.Windows.Input.StylusPoint)points[i];
            }
            string bestGesture = string.Empty;
            Rectangle irect = new Rectangle();
			irect.HorizontalAlignment = HorizontalAlignment.Left;
			irect.VerticalAlignment = VerticalAlignment.Top;
			irect.Margin = new Thickness(rect.MinX, rect.MinY, 0, 0);
			irect.Width = rect.MaxX - rect.MinX;
			irect.Height = rect.MaxY - rect.MinY;
            GestureInfos infos = new GestureInfos(new GestureData(imoves, ppoints, lastPoint, irect));
            for (int i = 0; i < gestures.Count; i++)
            {
                gest = (gestures[i] as GestureProperties).Moves;
                infos.Present = (gestures[i] as GestureProperties).Present;
                cost = CostLeven(gest, imoves);
                if (cost <= DEFAULT_FIABILITY)
                {
                    if ((gestures[i] as GestureProperties).match != null)
                    {
                        infos.Cost = cost;
                        cost = (gestures[i] as GestureProperties).match(infos);
                    }
                    if (cost < bestCost)
                    {
                        bestCost = cost;
                        bestGesture = (gestures[i] as GestureProperties).Present;
                    }
                }
            }
            MouseGestureEventArgs args = new MouseGestureEventArgs();
            args.Present = bestGesture;
            args.Fiability = cost;
            if (GestureMatchEvent != null)
                GestureMatchEvent(args);
        }
        private int DifAngle(int a, int b)
        {
            int dif = (int)Math.Abs(a - b);
            if (dif > DEFAULT_SECTORS / 2)
                dif = DEFAULT_SECTORS - dif;
            return dif;
        }
        /// <summary>
        /// 采用Levenshtein算法计算数组相似度
        /// </summary>
        /// <param name="cgest"></param>
        /// <param name="cmoves"></param>
        /// <returns></returns>
        private int CostLeven(int[] cgest, int[] cmoves)
        {
            if (cgest[0] == -1)
            {
                return (int)(cmoves.Length == 0 ? 0 : 100000);
            }
            int[,] d = new int[cgest.Length + 1, cmoves.Length + 1];
            int[,] w = new int[cgest.Length + 1, cmoves.Length + 1];
            int x = 0, y = 0;
            for (x = 1; x <= cgest.Length; x++)
            {
                for (y = 1; y < cmoves.Length; y++)
                {
                    d[x, y] = DifAngle((int)cgest[x - 1], (int)cmoves[y - 1]);
                }
            }

            for (y = 1; y <= cmoves.Length; y++)
                w[0, y] = 100000;
            for (x = 1; x <= cgest.Length; x++)
                w[x, 0] = 100000;
            w[0,0] = 0;

            int cost = 0;
            int above;
            int left;
            int diag;

            for (x = 1; x <= cgest.Length; x++)
            {
                for (y = 1; y < cmoves.Length; y++)
                {
                    cost = d[x, y];
                    above = w[x - 1,y] + cost;
                    left = w[x, y - 1] + cost;
                    diag = w[x - 1, y - 1] + cost;
                    w[x, y] = Math.Min(Math.Min(above, left), diag);
                }
            }
            return w[x - 1, y - 1];
        }
    }
}
