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

namespace WPFInk.tool
{
    public class InkTool
    {
        private static InkTool singleTon = null;
        private InkTool()
        {
        }
        public static InkTool getInstance()
        {
            if (singleTon == null)
                singleTon = new InkTool();
            return singleTon;
        }
        #region 常量
        /// <summary>
        /// 合并两条直线笔迹时的短点距离阈值,默认值为20
        /// </summary>
        private const double merge2StrokesP2PDistanceThreshold = 20;
        /// <summary>
        /// <summary>
        /// 合并两条直线笔迹时的短点距离阈值,默认值为20
        /// </summary>
        private const double merge2LineStrokesL2LDistanceThreshold = 20;
        /// <summary>
        /// 合并两条直线笔迹时的角度阈值,默认值为10
        /// </summary>
        private const double merge2LineStrokesAngleThreshold = 10;
        /// <summary>
        /// 两直线邻接距离阈值，值为20
        /// </summary>
        private const double merge2LineStrokesAbutDistanceThreshold = 20;
        /// <summary>
        /// 全重描情况下的长度差，值为20
        /// </summary>
        private const double merge2LineStrokesRetraceThreshold = 20;
        /// <summary>
        /// 垂直角的阈值，值为20
        /// </summary>
        private const double verticalAngleThreshold = 20;
        /// <summary>
        /// 水平角的阈值，值为20
        /// </summary>
        private const double horizontalAngleThreshold = 20;
        /// <summary>
        /// 圆弧平行圆心距离阈值，值为20
        /// </summary>
        private const double arcParalleCenterDistacneThreshold = 40;
        /// <summary>
        /// 平行圆弧的垂直距离阈值，值为20
        /// </summary>
        private const double arcParalleVerticalDistacneThreshold = 10;
        #endregion
        /// <summary>
        /// 复制一个point
        /// </summary>
        /// <param name="point"></param>
        /// <returns>和参数中的point相同的point</returns>
        public static StylusPoint StylusPointCopy(StylusPoint point)
        {
            return new StylusPoint(point.X, point.Y);
        }

        /// <summary>
        /// 复制一个point collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>和collection相同的复制品</returns>
        public StylusPointCollection StylusPointCollectionCopy(StylusPointCollection collection)
        {
            StylusPointCollection result = new StylusPointCollection();
            foreach (StylusPoint sp in collection)
            {
                result.Add(StylusPointCopy(sp));
            }
            return result;
        }

        /// <summary>
        /// 复制一个drawingattributes
        /// </summary>
        /// <param name="drawingAttributes"></param>
        /// <returns>drawingattributes的复制品</returns>
        public DrawingAttributes DrawingAttributesCopy(DrawingAttributes drawingAttributes)
        {
            DrawingAttributes da = new DrawingAttributes();
            da.Width = drawingAttributes.Width;
            da.Height = drawingAttributes.Height;
            da.Color = drawingAttributes.Color;
            return da;
        }

        /// <summary>
        /// 复制一个stroke
        /// </summary>
        /// <param name="stroke"></param>
        /// <returns>stroke的复制品</returns>
        public Stroke StrokeCopy(Stroke stroke)
        {
            Stroke s = new Stroke(new StylusPointCollection());
            s.DrawingAttributes = DrawingAttributesCopy(stroke.DrawingAttributes);
            s.StylusPoints = StylusPointCollectionCopy(stroke.StylusPoints);
            return s;
        }


        public bool isInCurve(Stroke stroke, Stroke curve)
        {
            StylusPointCollection spc = stroke.StylusPoints;
            StylusPointCollection spccurve = curve.StylusPoints;
            if (spc.Count == 0 || spccurve.Count < 3)
                return false;

            StylusPoint start = spc[0];
            StylusPoint middle = spc[spc.Count / 2];
            StylusPoint end = spc[spc.Count - 1];

            if (isPointInPath(start, spccurve) && isPointInPath(end, spccurve) && isPointInPath(middle, spccurve))
                return true;
            return false;
        }

        public bool isInCurve(Image imagei, Stroke curve)
        {
            StylusPointCollection spccurve = curve.StylusPoints;
            if (spccurve.Count < 3)
                return false;
            Rect bounds = new Rect(imagei.Margin.Left, imagei.Margin.Top, imagei.ActualWidth, imagei.ActualHeight);
            Rect r = imagei.RenderTransform.TransformBounds(bounds);

            Point s1 = imagei.RenderTransform.Transform(new Point(bounds.Left, bounds.Top)),
                s2 = imagei.RenderTransform.Transform(new Point(bounds.Left, bounds.Bottom)),
                s3 = imagei.RenderTransform.Transform(new Point(bounds.Right, bounds.Top)),
                s4 = imagei.RenderTransform.Transform(new Point(bounds.Right, bounds.Bottom));

            if (isPointInPath(s1, spccurve) & isPointInPath(s2, spccurve) & isPointInPath(s3, spccurve) & isPointInPath(s4, spccurve))
                return true;
            return false;
        }

        /// <summary>
        /// 判断point是否在trace中
        /// </summary>
        /// <param name="point"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public bool isPointInPath(StylusPoint point, StylusPointCollection trace)
        {
            StylusPoint p1, p2;
            bool inside = false;
            double x = point.X, y = point.Y;

            if (trace.Count < 3)
            {
                return inside;
            }

            StylusPoint oldPoint = new StylusPoint(trace[trace.Count - 1].X, trace[trace.Count - 1].Y);

            for (int i = 0; i < trace.Count; i++)
            {
                StylusPoint newPoint = trace[i];
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < x) == (x <= oldPoint.X) &&
                    ((double)y - (double)p1.Y) * (double)(p2.X - p1.X) <
                    ((double)p2.Y - (double)p1.Y) * (double)(x - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }

        /// <summary>
        /// 重载方法
        /// </summary>
        /// <param name="point"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public bool isPointInPath(Point point, StylusPointCollection trace)
        {
            StylusPoint sp = new StylusPoint(point.X, point.Y);
            return isPointInPath(sp, trace);
        }

        /// <summary>
        /// 合并两条直线笔迹
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public StrokeCollection merge2LineStrokes(Stroke s1, Stroke s2)
        {
            StrokeCollection strokes = new StrokeCollection();
            if (s1 == null)
            {
                Stroke result = new Stroke(s2.StylusPoints);
                strokes.Add(result);
                return strokes;
            }
            if (s2 == null)
            {
                Stroke result = new Stroke(s1.StylusPoints);
                strokes.Add(result);
                return strokes;
            }
            int s1PointCount = s1.StylusPoints.Count;
            int s2PointCount = s2.StylusPoints.Count;
            if (s1PointCount > 1 && s1PointCount > 1)
            {
                //计算两条直线的角度

                double angle = MathTool.getInstance().getAngleL2L(s1, s2);

                if (angle <= merge2LineStrokesAngleThreshold
                    || (angle >= 180 - merge2LineStrokesAngleThreshold && angle <= 180 + merge2LineStrokesAngleThreshold))//平行
                {
                    //两条直线之间的距离
                    double distanceL2L = MathTool.getInstance().distanceL2L(s1, s2);
                    if (distanceL2L < merge2LineStrokesL2LDistanceThreshold)//两条直线的垂直距离足够小
                    {
                        Rect bound1 = s1.GetBounds();
                        Rect bound2 = s2.GetBounds();
                        double distanceR2R = MathTool.getInstance().distanceR2R(bound1, bound2);
                        double distance11 = MathTool.getInstance().distanceP2P(s1.StylusPoints[0], s2.StylusPoints[0]);
                        double distance12 = MathTool.getInstance().distanceP2P(s1.StylusPoints[0], s2.StylusPoints[s2PointCount - 1]);
                        double distance21 = MathTool.getInstance().distanceP2P(s1.StylusPoints[s1PointCount - 1], s2.StylusPoints[0]);
                        double distance22 = MathTool.getInstance().distanceP2P(s1.StylusPoints[s1PointCount - 1]
                            , s2.StylusPoints[s2PointCount - 1]);
                        if (distanceR2R < merge2StrokesP2PDistanceThreshold)//直线没有分开
                        {
                            double length1 = MathTool.getInstance().distanceP2P(s1.StylusPoints[0], s1.StylusPoints[s1PointCount - 1]);//直线1的长度
                            double length2 = MathTool.getInstance().distanceP2P(s2.StylusPoints[0], s2.StylusPoints[s2PointCount - 1]);//直线2的长度
                            double angel1 = MathTool.getInstance().getAngleP2P(s1.StylusPoints[0], s1.StylusPoints[s1PointCount - 1]) % 90;//直线1的角度
                            double angel2 = MathTool.getInstance().getAngleP2P(s2.StylusPoints[0], s2.StylusPoints[s2PointCount - 1]) % 90;//直线2的角度
                            if (MathTool.getInstance().isContainRect(bound1, bound2))//直线2是直线1的子重描=
                            {
                                Stroke result = new Stroke(s1.StylusPoints);
                                strokes.Add(result);
                                return strokes;
                            }
                            else if (MathTool.getInstance().isContainRect(bound2, bound1))//直线1是直线2的子重描=
                            {
                                Stroke result = new Stroke(s2.StylusPoints);
                                strokes.Add(result);
                                return strokes;
                            }
                            else if (distanceR2R < 0
                                || ((Math.Abs(angel1) <= verticalAngleThreshold || Math.Abs(90 - angel1) <= verticalAngleThreshold)
                                    && (Math.Abs(angel2) <= verticalAngleThreshold || Math.Abs(90 - angel2) <= verticalAngleThreshold))//都是垂线
                                || ((Math.Abs(angel1) <= horizontalAngleThreshold || Math.Abs(90 - angel1) <= horizontalAngleThreshold)
                                    && (Math.Abs(angel2) <= horizontalAngleThreshold || Math.Abs(90 - angel2) <= horizontalAngleThreshold))//都是水平线
                                )//交叉重描
                            {
                                //找相距最远的两个点
                                double maxDistacne = Math.Max(Math.Max(distance11, distance12), Math.Max(distance21, distance22));
                                if (distance11 == maxDistacne)
                                {
                                    Stroke result = new Stroke(s1.StylusPoints);
                                    int index = getNearestPointInStroke(s2, s1.StylusPoints[s1PointCount - 1]);
                                    for (int i = index; i >= 0; i--)
                                    {
                                        result.StylusPoints.Add(s2.StylusPoints[i]);

                                    }
                                    strokes.Add(result);
                                    return strokes;
                                }
                                else if (distance12 == maxDistacne)
                                {
                                    StylusPointCollection spc = new StylusPointCollection();
                                    for (int i = 0; i < s1PointCount - 1; i++)
                                    {
                                        spc.Add(s1.StylusPoints[i]);
                                    }
                                    Stroke result = new Stroke(spc);
                                    int index = getNearestPointInStroke(s2, s1.StylusPoints[s1PointCount - 1]);
                                    for (int i = index; i < s2PointCount; i++)
                                    {
                                        result.StylusPoints.Add(s2.StylusPoints[i]);

                                    }
                                    strokes.Add(result);
                                    return strokes;
                                }
                                else if (distance21 == maxDistacne)
                                {
                                    StylusPointCollection spc = new StylusPointCollection();
                                    for (int i = s1PointCount - 1; i >= 0; i--)
                                    {
                                        spc.Add(s1.StylusPoints[i]);
                                    }
                                    Stroke result = new Stroke(spc);
                                    int index = getNearestPointInStroke(s2, s1.StylusPoints[0]);
                                    for (int i = index; i >= 0; i--)
                                    {
                                        result.StylusPoints.Add(s2.StylusPoints[i]);

                                    }
                                    strokes.Add(result);
                                    return strokes;
                                }
                                else if (distance22 == maxDistacne)
                                {
                                    StylusPointCollection spc = new StylusPointCollection();
                                    for (int i = s1PointCount - 1; i >= 0; i--)
                                    {
                                        spc.Add(s1.StylusPoints[i]);
                                    }
                                    Stroke result = new Stroke(spc);
                                    int index = getNearestPointInStroke(s2, s1.StylusPoints[0]);
                                    for (int i = index; i < s2PointCount; i++)
                                    {
                                        result.StylusPoints.Add(s2.StylusPoints[i]);

                                    }
                                    strokes.Add(result);
                                    return strokes;
                                }
                            }
                            else if (distanceR2R <= merge2LineStrokesAbutDistanceThreshold && distanceR2R >= 0)//两直线邻接
                            {
                                //查找邻接点                                
                                double minDistacne = Math.Min(Math.Min(distance11, distance12), Math.Min(distance21, distance22));
                                if (distance11 == minDistacne)
                                {
                                    StylusPointCollection spc = new StylusPointCollection();
                                    for (int i = s1PointCount - 1; i >= 0; i--)
                                    {
                                        spc.Add(s1.StylusPoints[i]);
                                    }
                                    Stroke result = new Stroke(spc);
                                    result.StylusPoints.Add(s2.StylusPoints);
                                    strokes.Add(result);
                                    return strokes;
                                }
                                else if (distance12 == minDistacne)
                                {
                                    Stroke result = new Stroke(s2.StylusPoints);
                                    result.StylusPoints.Add(s1.StylusPoints);
                                    strokes.Add(result);
                                    return strokes;
                                }
                                else if (distance21 == minDistacne)
                                {
                                    Stroke result = new Stroke(s1.StylusPoints);
                                    result.StylusPoints.Add(s2.StylusPoints);
                                    strokes.Add(result);
                                    return strokes;
                                }
                                else if (distance22 == minDistacne)
                                {
                                    Stroke result = new Stroke(s1.StylusPoints);
                                    for (int i = s2PointCount - 1; i >= 0; i--)
                                    {
                                        result.StylusPoints.Add(s2.StylusPoints[i]);
                                    }
                                    strokes.Add(result);
                                    return strokes;
                                }

                            }

                        }
                    }
                }
                strokes.Add(s1);
                strokes.Add(s2);
                return strokes;
            }
            return null;
        }
        /// <summary>
        /// 合并两条弧线笔迹
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public StrokeCollection merge2ArcStrokes(Stroke s1, Stroke s2)
        {
            StrokeCollection strokes = new StrokeCollection();
            if (s1 == null)
            {
                Stroke result = new Stroke(s2.StylusPoints);
                strokes.Add(result);
                return strokes;
            }
            if (s2 == null)
            {
                Stroke result = new Stroke(s1.StylusPoints);
                strokes.Add(result);
                return strokes;
            }
            int s1PointCount = s1.StylusPoints.Count;
            int s2PointCount = s2.StylusPoints.Count;
            if (s1PointCount > 1 && s1PointCount > 1)
            {
                if (isParallel2ArcStrokes(s1, s2))//平行
                {
                    if (MathTool.getInstance().distance2ParallelArc(s1, s2) <= arcParalleVerticalDistacneThreshold)//垂直距离足够小
                    {
                        Rect bound1 = s1.GetBounds();
                        Rect bound2 = s2.GetBounds();
                        double distanceR2R = MathTool.getInstance().distanceR2R(bound1, bound2);
                        if (distanceR2R < merge2StrokesP2PDistanceThreshold)//弧线没有分开
                        {
                            if (MathTool.getInstance().isContainRect(bound1, bound2))//直线2是直线1的子重描=
                            {
                                Stroke result = new Stroke(s1.StylusPoints);
                                strokes.Add(result);
                                return strokes;
                            }
                            else if (MathTool.getInstance().isContainRect(bound2, bound1))//直线1是直线2的子重描=
                            {
                                Stroke result = new Stroke(s2.StylusPoints);
                                strokes.Add(result);
                                return strokes;
                            }
                            else
                            {
                                double distance102 = MathTool.getInstance().distanceP2S(s1.StylusPoints[0], s2);
                                double distance112 = MathTool.getInstance().distanceP2S(s1.StylusPoints[s1.StylusPoints.Count - 1], s2);
                                double distance201 = MathTool.getInstance().distanceP2S(s2.StylusPoints[0], s1);
                                double distance211 = MathTool.getInstance().distanceP2S(s2.StylusPoints[s2.StylusPoints.Count - 1], s1);
                                double minDistance = Math.Min(Math.Min(distance102, distance112), Math.Min(distance201, distance211));
                                if (minDistance < merge2StrokesP2PDistanceThreshold)
                                {
                                    if (distance102 == minDistance)
                                    {
                                        int index = getNearestPointInStroke(s2, s1.StylusPoints[0]);

                                    }
                                }
                            }
                        }

                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 在直线中查找里直线外一点最近的点下标
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s1PointCount"></param>
        /// <param name="s2PointCount"></param>
        public int getNearestPointInStroke(Stroke s, StylusPoint sp)
        {
            double nearestDistance = MathTool.getInstance().distanceP2P(sp, s.StylusPoints[0]);
            int nearestIndex = 0;//记录最近点的索引值
            int i = 0;
            for (i = 1; i <= s.StylusPoints.Count - 1; i++)
            {
                double tempDistance = MathTool.getInstance().distanceP2P(sp, s.StylusPoints[i]);
                if (tempDistance < nearestDistance)
                {
                    nearestDistance = tempDistance;
                    nearestIndex = i;
                }
            }
            return nearestIndex;
        }
        /// <summary>
        ///移动Stroke
        /// </summary>
        /// <param name="stroke"></param>
        public void MoveStroke(Stroke stroke, double offset_x, double offset_y)
        {
            if (stroke != null)
            {
                Matrix moveMatrix = new Matrix(1, 0, 0, 1, offset_x, offset_y);

                stroke.Transform(moveMatrix, false);
            }
        }
        /// <summary>
        /// 移动StrokeCollection
        /// </summary>
        /// <param name="strokes"></param>
        public void MoveStrokes(StrokeCollection strokes, double offset_x, double offset_y)
        {
            if (strokes != null && strokes.Count > 0)
            {
                Matrix moveMatrix = new Matrix(1, 0, 0, 1, offset_x, offset_y);
                strokes.Transform(moveMatrix, false);
            }
        }
        /// <summary>
        /// 缩放stroke
        /// </summary>
        /// <param name="stroke"></param>
        /// <param name="offset_x"></param>
        /// <param name="scaling"></param>
        /// <param name="centerPoint"></param>
        public void ZoomStroke(Stroke stroke, double scaling, StylusPoint centerPoint)
        {
            if (stroke != null)
            {
                for (int j = 0; j < stroke.StylusPoints.Count; j++)
                {
                    StylusPoint point = stroke.StylusPoints[j];
                    double offsetx = point.X - centerPoint.X;
                    double offsety = point.Y - centerPoint.Y;

                    point.X = centerPoint.X + offsetx * scaling;
                    point.Y = centerPoint.Y + offsety * scaling;

                    stroke.StylusPoints[j] = point;
                }
            }
        }

        public void ZoomStrokes(StrokeCollection strokes, double scaling, StylusPoint centerPoint)
        {
            if (strokes != null)
            {
                foreach (Stroke s in strokes)
                {
                    ZoomStroke(s, scaling, centerPoint);
                }
            }
        }
        /// <summary>
        /// 判断两条笔迹是否平行，同圆心即可
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public bool isParallel2ArcStrokes(Stroke s1, Stroke s2)
        {
            if (s1 != null && s2 != null)
            {
                StylusPoint centerPoint1 = MathTool.getInstance().getArcCenter(s1);
                StylusPoint centerPoint2 = MathTool.getInstance().getArcCenter(s2);
                double c2cDistance = MathTool.getInstance().distanceP2P(centerPoint1, centerPoint2);
                if (centerPoint1.X != 0 && centerPoint1.Y != 0 && centerPoint2.X != 0 && centerPoint2.Y != 0
                    && c2cDistance < arcParalleCenterDistacneThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 计算草图相似性
        /// </summary>
        /// <param name="sc1">草图1笔迹簇</param>
        /// <param name="sc2">草图2笔迹簇</param>
        /// <returns></returns>
        private const int AreasCountCol = 4;//草图区域数的开方，这里是3，代表把草图分为9个区域
        private const float similarityThreshold = 0.5F;//草图相似性比较的阈值
        public float getSketchSimilarity(StrokeCollection sc1, StrokeCollection sc2)
        {
            float result = 0;
            int[] P1 = getAreasCount(sc1);//草图1落在各区域点数的数组
            int[] P2 = getAreasCount(sc2);//草图2落在各区域点数的数组
            int AreasCount = AreasCountCol * AreasCountCol;
            float[] A = new float[AreasCount];
            Rect bound1 = sc1.GetBounds();
            Rect bound2 = sc2.GetBounds();
            int similarityCount = 0;
            for (int i = 0; i < AreasCount; i++)
            {
                A[i] = (float)P1[i] / (float)P2[i];//求出对应区域点数的比值数组
            }
            float B = (float)(bound1.Width * bound1.Height / (float)bound2.Width / (float)bound2.Height);//计算两个草图包围盒的面积比
            for (int i = 0; i < AreasCount; i++)
            {
                if (Math.Abs(A[i] - B) <= similarityThreshold)
                {
                    similarityCount++;
                }
            }
            result = (float)similarityCount / (float)AreasCount;
            return result;
        }

        private int[] getAreasCount(StrokeCollection sc)
        {
            int[] areasPointCount = new int[AreasCountCol * AreasCountCol];//草图落在各区域点数的数组

            Rect bound = sc.GetBounds();
            double strideX = bound.Width / AreasCountCol;//每一小格的宽度
            double strideY = bound.Height / AreasCountCol;//每一小格的高度
            foreach (Stroke s in sc)
            {
                foreach (StylusPoint sp in s.StylusPoints)
                {
                    int i = (int)((sp.X - bound.Left) / strideX);//计算每个点所在列
                    int j = (int)((sp.Y - bound.Top) / strideY);//计算每个点所在行
                    int index = i + j * AreasCountCol;//计算每个点所在区域的下标值
                    areasPointCount[index]++;
                }
            }
            return areasPointCount;
        }
        /// <summary>
        /// 对inkcanvas进行缩放
        /// </summary>
        /// <param name="inkcanvas">缩放对象</param>
        /// <param name="rate">缩放比率</param>
        public void InkCanvasZoom(InkCanvas inkCanvas, double fromRate, double toRate, double centerX, double centerY)
        {
            //((ScaleTransform)inkCanvas.RenderTransform).CenterX = centerX;
            //((ScaleTransform)inkCanvas.RenderTransform).CenterY = centerY;
            ScaleTransform scaleTransform = new ScaleTransform();
            inkCanvas.RenderTransform = scaleTransform;
            scaleTransform.CenterX = inkCanvas.ActualWidth*centerX;
            scaleTransform.CenterY = inkCanvas.ActualHeight * centerY;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //宽度缩放
            myDoubleAnimation.From = fromRate;
            myDoubleAnimation.To = toRate;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            if (inkCanvas.FindName("scaleTransform") != null)
            {
                inkCanvas.UnregisterName("scaleTransform");
            }
            inkCanvas.RegisterName("scaleTransform", scaleTransform);
            Storyboard.SetTargetName(myDoubleAnimation, "scaleTransform");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));


            DoubleAnimation myDoubleAnimationHeight = new DoubleAnimation();
            //高度缩放
            myDoubleAnimationHeight.From = fromRate;
            myDoubleAnimationHeight.To = toRate;
            myDoubleAnimationHeight.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            Storyboard.SetTargetName(myDoubleAnimationHeight, "scaleTransform");
            Storyboard.SetTargetProperty(myDoubleAnimationHeight, new PropertyPath(ScaleTransform.ScaleYProperty));


            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Children.Add(myDoubleAnimationHeight);
            myStoryboard.Begin(inkCanvas);
        }
        /// <summary>
        /// 在画板上画点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Stroke drawPoint(double x, double y, double width, Color color, InkCanvas inkCanvas)
        {
            StylusPointCollection spc = new StylusPointCollection();
            StylusPoint currPoint = new StylusPoint(x, y);
            spc.Add(currPoint);
            Stroke s = new Stroke(spc);
            s.DrawingAttributes.Color = color;
            s.DrawingAttributes.Width = width;
            s.DrawingAttributes.Height = width;
            inkCanvas.Strokes.Add(s);
            return s;
        }
        /// <summary>
        /// 根据两点画笔迹直线
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public Stroke DrawLine(double x1, double y1, double x2, double y2,InkCanvas inkCanvas,Color color)
        {
            StylusPointCollection spc = new StylusPointCollection();
            StylusPoint currPoint1 = new StylusPoint(x1, y1);
            StylusPoint currPoint2 = new StylusPoint(x2, y2);
            spc.Add(currPoint1);
            spc.Add(currPoint2);
            Stroke s = new Stroke(spc);
            s.DrawingAttributes.Color = color;
            s.DrawingAttributes.Width = 3;
            s.DrawingAttributes.Height = 3;
            inkCanvas.Strokes.Add(s);
            return s;
        }

        //public Path DrawArrow(Point point1,Point point2)
        //{
            //int arrowHeadWidth = 20;
            //int arrowHeadAngle = 15;
            //double angleStartEnd=0;
            //GeometryGroup lineGroup = new GeometryGroup();
            //LineGeometry line = new LineGeometry();
            //LineGeometry line2 = new LineGeometry();
            //LineGeometry line3 = new LineGeometry();

            //line = new LineGeometry(point1, point2);
            //if ((angle >=0 && angle < 45) || (angle >= 315 && angle <= 360))
            //{
            //    startPoint = RightPoint1;
            //    endPoint = LeftPoint2;
            //    angleStartEnd = MathTool.getInstance().getAngleP2P(new StylusPoint(RightPoint1.X, RightPoint1.Y), new StylusPoint(LeftPoint2.X, LeftPoint2.Y));
                
            //    Point point = getMiddlePoint(angleStartEnd, LeftPoint2, arrowHeadWidth);
            //    line2 = new LineGeometry(LeftPoint2, point);
            //    RotateTransform xform2 = new RotateTransform();
            //    xform2.CenterX= LeftPoint2.X;
            //    xform2.CenterY=LeftPoint2.Y;
            //    xform2.Angle = arrowHeadAngle;
            //    line2.Transform = xform2;

            //    line3 = new LineGeometry(LeftPoint2, point);
            //    RotateTransform xform3 = new RotateTransform();
            //    xform3.CenterX = LeftPoint2.X;
            //    xform3.CenterY = LeftPoint2.Y;
            //    xform3.Angle = -arrowHeadAngle;
            //    line3.Transform = xform3;
            //}
            //lineGroup.Children.Add(line);
            //lineGroup.Children.Add(line2);
            //lineGroup.Children.Add(line3);
            //Path arrow=new Path();
            //arrow.Data=lineGroup;
        //}
    }
}
