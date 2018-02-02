using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFInk.mouseGesture;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Ink;
using WPFInk.ink;
using System.Windows;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using WPFInk.cmd;
using WPFInk.graphic;
using WPFInk.tool;
using WPFInk.Global;

namespace WPFInk.graphic
{

    public class MultiCustomGesture
    {
        #region 私有变量
        private InkCollector _inkCollector;
        private InkCanvas _inkCanvas;
        private System.Windows.Input.StylusPointCollection polyPoints;
        /// <summary>
        /// //根据方向和笔序识别
        /// </summary>
        private MouseGesture gesture;

        public MouseGesture Gesture
        {
            get { return gesture; }
            set { gesture = value; }
        }

        #endregion

        #region 公有变量
        public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public int seconds = 0;
        public StrokeCollection strokes = new StrokeCollection();
        //public 
        #endregion

        #region 常量
        /// <summary>
        /// 五角星提取图形的空白边界，值为5
        /// </summary>
        private const double PentagramZoomStrokePadding = 5;
       /// <summary>
       /// 五角星距离图形最远距离，超过这个距离则不是对图形的重点标注，值为200
       /// </summary>
        private const double PentagramMaxDistance = 200;
        #endregion

        #region 构造函数
        public MultiCustomGesture(InkCollector inkCollector,InkCanvas inkCanvas)
        {
            this._inkCollector = inkCollector;
            this._inkCanvas = inkCanvas;
            timer.Interval = 1000;
            timer.Tick += new System.EventHandler(this.timer_Tick);
            if (GlobalValues.MyGraphic_IsDirectionRecognize)
            {
                createGesture();
            }
        }
        /// <summary>
        /// 识别的时间控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (strokes.Count > 1)
            {
                seconds++;
                if (seconds >= GlobalValues.MyGraphic_PauseTime)
                {
                    if (GlobalValues.MyGraphic_IsDirectionRecognize)
                    {
                        gesture.StopCapture();
                    }
                    else
                    {
                        recognise(strokes);
                    }
                    strokes.Clear();
                    timer.Stop();
                    seconds = 0;
                }
            }
            else
            {
                timer.Stop();
                seconds = 0;
                if (GlobalValues.MyGraphic_IsDirectionRecognize)
                {
                    gesture.StopCapture();
                }
                else
                {
                    recognise(strokes);
                }
                strokes.Clear();
            }
        }
        #endregion

        #region 成员函数
        /// <summary>
        /// 识别图形
        /// </summary>
        /// <param name="strokeCollection"></param>
        void recognise(StrokeCollection strokeCollection)
        {
            try
            {
                Stroke stroke = strokes[strokeCollection.Count - 1];
                StrokeCollection myGraphicStrokes = new StrokeCollection(strokes);
                Rect bound = strokes.GetBounds();
                int myGraphicId = _inkCollector.Sketch.MyGraphics.Count == 0 ? 1 :
                    _inkCollector.Sketch.MyGraphics[_inkCollector.Sketch.MyGraphics.Count - 1].MyGraphicID + 1;

                //根据拐点拆分线段
                StrokeCollection sctmp = new StrokeCollection();
                foreach (Stroke stro in myGraphicStrokes)
                {
                    //根据拐点拆分线段
                    StrokeCollection sc = GraphicMathTool.getInstance().inciseStrokeByPolyPoints(stro);
                    for (int i = sc.Count - 1; i >= 0; i--)
                    {
                        sctmp.Add(sc[i]);
                    }
                }
                myGraphicStrokes.Add(sctmp);
                _inkCanvas.Strokes.Add(sctmp);

                //折点显示
                polyPoints = GraphicMathTool.getInstance().getPolyPointsByStrokes(myGraphicStrokes);
                for (int i = 0; i < polyPoints.Count; i++)
                {
                    // GraphicMathTool.getInstance().drawPoint(polyPoints[i], 5, Colors.Red, _inkCanvas);
                }

                //识别各段图元序列
                List<string> recogniseResults = recogniseEveryStrokes(myGraphicStrokes);

                mergeStrokes(myGraphicStrokes, recogniseResults);//合并相关直线和弧线
                recogniseResults = recogniseEveryStrokes(myGraphicStrokes);//重新识别
                //折点显示
                System.Windows.Input.StylusPointCollection polyPoints2 = GraphicMathTool.getInstance().getPolyPointsByStrokes(myGraphicStrokes);
                for (int i = 0; i < polyPoints2.Count; i++)
                {
                    //GraphicMathTool.getInstance().drawPoint(polyPoints2[i], 3, Colors.Yellow, _inkCanvas);
                }
                //根据圆弧序列识别图形
                recognizeGraphic(recogniseResults, myGraphicStrokes);
                if (_inkCollector.Sketch.MyGraphics.Count > 0)
                {
                    List<int> ids = GraphicMathTool.getInstance().getGraphicStructure(_inkCollector.Sketch.MyGraphics[0], _inkCollector, new List<int>());
                    //foreach (int id in ids)
                    //{
                    //    _inkCollector._mainPage.message.Content += id.ToString() + ",";
                    //}
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return;
            }
        }
        /// <summary>
        /// 识别图形
        /// </summary>
        /// <param name="recogniseResults"></param>
        private void recognizeGraphic(List<string> recogniseResults,StrokeCollection strokes)
        {
            if (recogniseResults.Count > 0)
            {
                int myGraphicId = _inkCollector.Sketch.MyGraphics.Count == 0 ? 1 :
                _inkCollector.Sketch.MyGraphics[_inkCollector.Sketch.MyGraphics.Count - 1].MyGraphicID + 1;
                int arcCount = 0;
                foreach (string str in recogniseResults)
                {
                    if (str.Equals("arc"))
                    {
                        arcCount++;
                    }
                }
                double arcPercent = (double)arcCount / (double)recogniseResults.Count;
                if (recogniseResults.IndexOf("ellipse") != -1 || arcPercent == 1)//椭圆
                {
                    _inkCollector._mainPage.message.Content = "椭圆，";
                    ellipse(strokes, myGraphicId);
                    return;
                }
                else if (arcPercent >= 0.5)//圆弧
                {
                    _inkCollector._mainPage.message.Content = "圆弧，";
                    _inkCanvas.Strokes.Remove(strokes);
                    return;
                }
                else if (arcPercent == 0)//折线
                {
                    _inkCollector._mainPage.message.Content = "都是直线，";
                    if (GraphicMathTool.getInstance().isRectangle(strokes))
                    {
                        _inkCollector._mainPage.message.Content = "矩形，";
                        rectangle(strokes, myGraphicId);
                        return;
                    }
                    else if (GraphicMathTool.getInstance().isArrow(strokes).Count == 2)
                    {
                        _inkCollector._mainPage.message.Content = "箭头，";
                        arrow(strokes, myGraphicId, GraphicMathTool.getInstance().isArrow(strokes)[0],
                            GraphicMathTool.getInstance().isArrow(strokes)[1]);
                        return;
                    }
                    else
                    {
                        _inkCollector._mainPage.message.Content = "没有识别出来";
                        _inkCanvas.Strokes.Remove(strokes);
                        return;
                    }
                }
                else if (arcPercent > 0 && arcPercent < 0.5)
                {
                    _inkCollector._mainPage.message.Content = "没有识别出来arcPercent";
                    _inkCanvas.Strokes.Remove(strokes);
                    return;

                }
                else
                {
                    _inkCollector._mainPage.message.Content = "没有识别出来";
                    _inkCanvas.Strokes.Remove(strokes);
                    return;
                }
            }
            else
            {
                _inkCollector._mainPage.message.Content = "没有识别出来";
                _inkCanvas.Strokes.Remove(strokes);
                return;

            }
        }

        /// <summary>
        /// 合并相关笔迹
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        /// <param name="recogniseResults"></param>
        public void mergeStrokes(StrokeCollection myGraphicStrokes, List<string> recogniseResults)
        {
            for (int i = 0; i <= recogniseResults.Count - 1; i++)
            {
                if (recogniseResults[i] == "line")
                {
                    for (int k = i + 1; k <= recogniseResults.Count - 1; k++)
                    {
                        if (recogniseResults[k] == "line")
                        {
                            StrokeCollection MergeResults = InkTool.getInstance().merge2LineStrokes(myGraphicStrokes[i], myGraphicStrokes[k]);
                            if (MergeResults != null)
                            {
                                if (MergeResults.Count == 1)
                                {
                                    myGraphicStrokes[i] = MergeResults[0];
                                    myGraphicStrokes.Remove(myGraphicStrokes[k]);
                                    recogniseResults[i] = "line";
                                    recogniseResults.Remove(recogniseResults[k]);
                                    i--;
                                    break;
                                }

                            }
                        }
                    }
                }
                else if (recogniseResults[i] == "arc")
                {
                    for (int k = i + 1; k <= recogniseResults.Count - 1; k++)
                    {
                        if (recogniseResults[k] == "arc")
                        {
                            StrokeCollection MergeResults = InkTool.getInstance().merge2ArcStrokes(myGraphicStrokes[i], myGraphicStrokes[k]);
                            if (MergeResults != null)
                            {
                                if (MergeResults.Count == 1)
                                {
                                    myGraphicStrokes[i] = MergeResults[0];
                                    myGraphicStrokes.Remove(myGraphicStrokes[k]);
                                    recogniseResults[i] = "arc";
                                    recogniseResults.Remove(recogniseResults[k]);
                                    i--;
                                    break;
                                }

                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 识别各段图元序列
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        /// <returns></returns>
        private List<string> recogniseEveryStrokes(StrokeCollection myGraphicStrokes)
        {
            int j = 1;
            List<string> recogniseResults = new List<string>();
            foreach (Stroke str in myGraphicStrokes)
            {
                if (GraphicMathTool.getInstance().isEllipse(str))
                {
                    //_inkCollector._mainPage.message.Content += j.ToString() + "椭圆";
                    recogniseResults.Add("ellipse");
                    //GraphicMathTool.getInstance().drawPoint(MathTool.getInstance().getArcCenter(str), 10, Colors.Blue, _inkCanvas);
                }
                //判断是否是直线
                else if (GraphicMathTool.getInstance().isLine(str))
                {
                    //_inkCollector._mainPage.message.Content += j.ToString() + "直线";
                    recogniseResults.Add("line");
                }

                else if (GraphicMathTool.getInstance().isArc(str))
                {
                    //_inkCollector._mainPage.message.Content += j.ToString() + "圆弧";
                    recogniseResults.Add("arc");
                    //GraphicMathTool.getInstance().drawPoint(MathTool.getInstance().getArcCenter(str), 10, Colors.Blue, _inkCanvas);

                }

                j++;
            }
            return recogniseResults;
        }
       /// <summary>
       /// 折线箭头
       /// </summary>
       /// <param name="myGraphicStrokes"></param>
       /// <param name="myGraphicId"></param>
       /// <param name="pointFirst"></param>
        private void polylineArrow(StrokeCollection myGraphicStrokes, int myGraphicId, Point pointFirst)
        {
            if (myGraphicStrokes.Count == 2)//如果笔迹由两条笔迹组成则拆成3条笔迹
            {
                StrokeCollection splitStrokes = MathTool.getInstance().ProcessPointErase(myGraphicStrokes[0], polyPoints[0]);
                if (splitStrokes.Count == 2)
                {
                    splitStrokes.Add(myGraphicStrokes[1]);
                    _inkCollector._mainPage._inkCanvas.Strokes.Remove(myGraphicStrokes[0]);
                    myGraphicStrokes.Clear();
                    myGraphicStrokes = splitStrokes;
                    _inkCollector._mainPage._inkCanvas.Strokes.Add(myGraphicStrokes[0]);
                    _inkCollector._mainPage._inkCanvas.Strokes.Add(myGraphicStrokes[1]);
                }
            }
            if (myGraphicStrokes.Count == 3)
            {
                foreach (System.Windows.Input.StylusPoint sp in myGraphicStrokes[0].StylusPoints)
                {
                    if ((int)sp.X == (int)polyPoints[1].X && (int)sp.Y == (int)polyPoints[1].Y)
                    {
                        StrokeCollection splitStrokes = MathTool.getInstance().ProcessPointErase(myGraphicStrokes[0], polyPoints[1]);
                        if (splitStrokes.Count == 2)
                        {
                            splitStrokes.Add(myGraphicStrokes[1]);
                            splitStrokes.Add(myGraphicStrokes[2]);
                            _inkCollector._mainPage._inkCanvas.Strokes.Remove(myGraphicStrokes[0]);
                            myGraphicStrokes.Clear();
                            myGraphicStrokes = splitStrokes;
                            _inkCollector._mainPage._inkCanvas.Strokes.Add(myGraphicStrokes[0]);
                            _inkCollector._mainPage._inkCanvas.Strokes.Add(myGraphicStrokes[1]);
                        }
                    }
                }
            }
            System.Windows.Shapes.Line connLine = ShapeLineBy2Points(pointFirst.X, pointFirst.Y
                                 , myGraphicStrokes[1].StylusPoints[myGraphicStrokes[1].StylusPoints.Count - 1].X
                                 , myGraphicStrokes[1].StylusPoints[myGraphicStrokes[1].StylusPoints.Count - 1].Y);
            MyGraphic myGraphicPolylineArrow = new MyGraphic(myGraphicId, myGraphicStrokes, connLine);
            myGraphicPolylineArrow.ShapeType = "polylineArrow";
            myGraphicPolylineArrow.PolyPoints = polyPoints;

            Command amgcPolylineArrow = new AddMyGraphicCommand(_inkCollector, myGraphicPolylineArrow);
            amgcPolylineArrow.execute();
            _inkCollector.CommandStack.Push(amgcPolylineArrow);
            polyPoints.Clear();

            GraphicMathTool.getInstance().SearchRelationByPosition(myGraphicPolylineArrow, _inkCollector.Sketch.MyGraphics, _inkCollector);
        }
        /// <summary>
        /// 箭头结果处理
        /// </summary>
        /// <param name="myGraphicStrokes">笔迹集合</param>
        /// <param name="myGraphicId">图形ID</param>
        /// <param name="pointFirst">箭尾起点</param>
        /// <param name="pointLast">箭尾尾点</param>
        private void arrow(StrokeCollection myGraphicStrokes, int myGraphicId, System.Windows.Input.StylusPoint pointFirst, 
            System.Windows.Input.StylusPoint pointLast)
        {
            if (myGraphicStrokes.Count > 1)
            {
                System.Windows.Shapes.Line line = ShapeLineBy2Points(pointFirst.X, pointFirst.Y, pointLast.X, pointLast.Y);                
                MyGraphic myGraphicLine = new MyGraphic(myGraphicId, myGraphicStrokes, line);
                myGraphicLine.ShapeType = "arrow";
                myGraphicLine.PolyPoints = polyPoints;
                Command amgcLine = new AddMyGraphicCommand(_inkCollector, myGraphicLine);
                amgcLine.execute();
                _inkCollector.CommandStack.Push(amgcLine);
                //查找关联Ellipse和rectangle
                GraphicMathTool.getInstance().SearchRelationByPosition(myGraphicLine, _inkCollector.Sketch.MyGraphics, _inkCollector);
            }
        }
        /// <summary>
        /// 接触图形之间的关系
        /// </summary>
        /// <param name="strokes"></param>
        private void severRelationGesture(StrokeCollection strokes)
        {
            Point firstPoint = new Point(strokes[0].StylusPoints[0].X, strokes[0].StylusPoints[0].Y);
            Point lastPoint = new Point(strokes[strokes.Count - 1].StylusPoints[strokes[strokes.Count - 1].StylusPoints.Count - 1].X
                                        , strokes[strokes.Count - 1].StylusPoints[strokes[strokes.Count - 1].StylusPoints.Count - 1].Y);
            MyGraphic myGraphic2SeverRelation = null;//要解除关系的矩形
            foreach (MyGraphic mg in _inkCollector.Sketch.MyGraphics)//找出要解除关系的矩形
            {
                if (MathTool.getInstance().isPointXYinRectangle(MathTool.getInstance().RectToRectangle(mg.Strokes.GetBounds())
                    , strokes[0].StylusPoints[0].X, strokes[0].StylusPoints[0].Y))
                {
                    myGraphic2SeverRelation = mg;
                    break;
                }
            }
            if (myGraphic2SeverRelation != null)
            {                
                _inkCollector.Sketch.RemoveGraphicLinkNode(_inkCollector.Sketch.getGraphicLinkNodeByNextGraphicLinkNodeID(myGraphic2SeverRelation.MyGraphicID));
                foreach(GraphicLinkNode gln in _inkCollector.Sketch.getGraphicLinkNodesBySelfMyGraphicID(myGraphic2SeverRelation.MyGraphicID))
                {
                    _inkCollector.Sketch.RemoveGraphicLinkNode(gln);
                }

                Command mgmc = new MyGraphicMoveCommand(myGraphic2SeverRelation, lastPoint.X - firstPoint.X, lastPoint.Y - firstPoint.Y, _inkCollector);
                mgmc.execute();
                _inkCollector.CommandStack.Push(mgmc);
            }
            _inkCanvas.Strokes.Remove(strokes);
        }
        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="stroke"></param>
        private void inserImageGesture(Stroke stroke)
        {
            _inkCanvas.Strokes.Remove(stroke);
            OpenFileDialog openfileImg = new OpenFileDialog()
            {
                Filter = "Jpeg Files (*.jpg)|*.jpg|Bitmap files (*.bmp)|*.bmp|All Files(*.*)|*.*",
                Multiselect = true
            };

            if (openfileImg.ShowDialog() == DialogResult.OK)
            {
                string FileName = openfileImg.FileName;
                string SafeFileName = openfileImg.SafeFileName;
                MyImage newimage = new MyImage(FileName);
                newimage.SafeFileName = SafeFileName;
                InkConstants.AddBound(newimage);
                AddImageCommand cmd = new AddImageCommand(_inkCollector, newimage);
                cmd.execute();
                _inkCollector.CommandStack.Push(cmd);
                _inkCollector._mainPage.pointView.pointView.AddNode(_inkCollector._mainPage.pointView.pointView.nodeList
                                                                    , _inkCollector._mainPage.pointView.pointView.links);

            }
        }
        /// <summary>
        /// 打开xml文件
        /// </summary>
        /// <param name="stroke"></param>
        private void openGesture(Stroke stroke)
        {
            _inkCanvas.Strokes.Remove(stroke);
            //打开文件
            OpenFileDialog openfile = new OpenFileDialog()
            {
                Filter = "Xml Files (*.xml)|*.xml|All Files(*.*)|*.*",
                Multiselect = false
            };

            if (openfile.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = openfile.OpenFile())
                {
                    WPFInk.PersistenceManager.PersistenceManager.getInstance().LoadFromStream(_inkCollector, stream);
                    stream.Close();
                }
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stroke"></param>
        private void saveGesture(Stroke stroke)
        {
            _inkCanvas.Strokes.Remove(stroke);
            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "xml",
                Filter = "Xml Files (*.xml)|*.xml|Jpeg Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|" +
                         "Bitmap files (*.bmp)|*.bmp|All　files　(*.*)|*.*",
                FilterIndex = 1,
                FileName = "未命名"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    if (sfd.FilterIndex == 1)
                    {
                        WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveSketchToStream(_inkCollector, stream);
                    }
                    else
                    {
                        WPFInk.PersistenceManager.PersistenceManager.getInstance().SaveInkToImage(_inkCollector, stream);
                    }
                    stream.Close();
                }
            }
        }
        /// <summary>
        /// 撤销
        /// </summary>
        /// <param name="stroke"></param>
        private void redoGesture(Stroke stroke)
        {
            if (_inkCollector.UndoCommandStack.Count > 0)
            {
                Command cmd = _inkCollector.UndoCommandStack.Pop();
                _inkCollector.CommandStack.Push(cmd);
                cmd.execute();
            }
            _inkCanvas.Strokes.Remove(stroke);
        }
        /// <summary>
        /// 反撤销
        /// </summary>
        /// <param name="stroke"></param>
        private void undoGesture(Stroke stroke)
        {
            if (_inkCollector.CommandStack.Count > 0)
            {
                Command cmd = _inkCollector.CommandStack.Pop();
                _inkCollector.UndoCommandStack.Push(cmd);
                cmd.undo();
            }
            _inkCanvas.Strokes.Remove(stroke);
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="stroke"></param>
        private void clearAllGesture(Stroke stroke)
        {
            _inkCanvas.Strokes.Remove(stroke);
            if (_inkCollector._mainPage._inkCanvas.Children.Count > 0 || _inkCollector._mainPage._inkCanvas.Strokes.Count > 0)
            {

                DialogResult MsgBoxResult;//设置对话框的返回值
                MsgBoxResult = System.Windows.Forms.MessageBox.Show("删除以后将不能恢复，是否删除", "提示", MessageBoxButtons.YesNo);
                if (MsgBoxResult == DialogResult.Yes)//如果对话框的返回值是YES（按"Y"按钮）
                {
                    MathTool.getInstance().ClearAllStrokesAndChildren(_inkCollector._mainPage);
                }
            }
        }
        /// <summary>
        /// 删除所有元素
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        private void deleteGesture(StrokeCollection myGraphicStrokes)
        {
            _inkCollector.removeHitStrokes(myGraphicStrokes);//删除笔迹                    
            _inkCollector.removeHitImages(myGraphicStrokes);//删除图片                    
            _inkCollector.removeHitTextBoxes(myGraphicStrokes);//删除文本
            _inkCollector.removeHitMyGraphics(myGraphicStrokes);//删除图形
            _inkCanvas.Strokes.Remove(myGraphicStrokes);
        }
        /// <summary>
        /// 向有关联的矩形之间添加矩形手势，手势为>
        /// </summary>
        /// <param name="stroke"></param>
        /// <param name="myGraphicStrokes"></param>
        private void insertRectangleLeft(Stroke stroke, StrokeCollection myGraphicStrokes)
        {
            MyGraphic insertMyGraphicLeft = null;
            //查找要插入的图形
            foreach (MyGraphic mg in _inkCollector.Sketch.MyGraphics)
            {
                if (mg.ShapeType == "rectangle" && MathTool.getInstance().isHitRects(new Rect(stroke.GetBounds().Left
                    , stroke.GetBounds().Top, stroke.GetBounds().Width / 2, stroke.GetBounds().Height), mg.Strokes.GetBounds()))
                {
                    insertMyGraphicLeft = mg;
                    break;
                }
            }
            //查找插入位置
            if (insertMyGraphicLeft != null)
            {
                List<MyGraphic> mg2InsertList = new List<MyGraphic>();
                foreach (MyGraphic mg in _inkCollector.Sketch.MyGraphics)
                {
                    if (mg != insertMyGraphicLeft && mg.ShapeType == "rectangle"
                        && MathTool.getInstance().isHitRects(new Rect(myGraphicStrokes.GetBounds().Left + myGraphicStrokes.GetBounds().Width / 2
                            , myGraphicStrokes.GetBounds().Top, myGraphicStrokes.GetBounds().Width / 2, myGraphicStrokes.GetBounds().Height), mg.Strokes.GetBounds()))
                    {
                        mg2InsertList.Add(mg);
                        if (mg2InsertList.Count == 2)
                        {
                            break;
                        }
                    }
                }
                //查处需要移动的图形
                if (mg2InsertList.Count == 2)
                {
                    if (mg2InsertList[0].Strokes.GetBounds().Top < mg2InsertList[1].Strokes.GetBounds().Top)
                    {
                        InsertRectangleMoveOther(insertMyGraphicLeft, mg2InsertList, 1);
                    }
                    else
                    {
                        InsertRectangleMoveOther(insertMyGraphicLeft, mg2InsertList, 0);
                    }
                }
                GraphicMathTool.getInstance().SearchRelationByPosition(insertMyGraphicLeft, _inkCollector.Sketch.MyGraphics, _inkCollector);
            }


            _inkCanvas.Strokes.Remove(stroke);
        }
        /// <summary>
        /// 向有关联的矩形之间添加矩形手势，手势为<
        /// </summary>
        /// <param name="stroke"></param>
        /// <param name="myGraphicStrokes"></param>
        private void insertRectangleRight(Stroke stroke, StrokeCollection myGraphicStrokes)
        {
            MyGraphic insertMyGraphicRight = null;

            //查找要插入的图形
            foreach (MyGraphic mg in _inkCollector.Sketch.MyGraphics)
            {
                if (mg.ShapeType == "rectangle" && MathTool.getInstance().isHitRects(new Rect(myGraphicStrokes.GetBounds().Left
                    + myGraphicStrokes.GetBounds().Width / 2, myGraphicStrokes.GetBounds().Top, myGraphicStrokes.GetBounds().Width / 2, myGraphicStrokes.GetBounds().Height),
                    mg.Strokes.GetBounds()))
                {
                    insertMyGraphicRight = mg;
                    break;
                }
            }

            //查找插入位置
            if (insertMyGraphicRight != null)
            {
                List<MyGraphic> mg2InsertList = new List<MyGraphic>();
                foreach (MyGraphic mg in _inkCollector.Sketch.MyGraphics)
                {
                    if (mg != insertMyGraphicRight && mg.ShapeType == "rectangle"
                        && MathTool.getInstance().isHitRects(new Rect(myGraphicStrokes.GetBounds().Left, myGraphicStrokes.GetBounds().Top,
                            myGraphicStrokes.GetBounds().Width / 2, myGraphicStrokes.GetBounds().Height), mg.Strokes.GetBounds()))
                    {
                        mg2InsertList.Add(mg);
                        if (mg2InsertList.Count == 2)
                        {
                            break;
                        }
                    }
                }
                //查处需要移动的图形
                if (mg2InsertList.Count == 2)
                {
                    if (mg2InsertList[0].Strokes.GetBounds().Top < mg2InsertList[1].Strokes.GetBounds().Top)
                    {
                        InsertRectangleMoveOther(insertMyGraphicRight, mg2InsertList, 1);
                    }
                    else
                    {
                        InsertRectangleMoveOther(insertMyGraphicRight, mg2InsertList, 0);
                    }
                }
                GraphicMathTool.getInstance().SearchRelationByPosition(insertMyGraphicRight, _inkCollector.Sketch.MyGraphics, _inkCollector);
            }
            _inkCanvas.Strokes.Remove(stroke);
        }
        /// <summary>
        /// 五角星
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        /// <param name="myGraphicId"></param>
        /// <param name="pointFirst"></param>
        private void pentagram( StrokeCollection myGraphicStrokes, int myGraphicId, Point pointFirst)
        {
            System.Windows.Shapes.Polygon pentagram = new Polygon();
            pentagram.Points.Add(new Point(pointFirst.X, pointFirst.Y));
            pentagram.Points.Add(new Point(polyPoints[0].X, polyPoints[0].Y));
            pentagram.Points.Add(new Point(polyPoints[2].X, polyPoints[2].Y));
            pentagram.Points.Add(new Point(polyPoints[4].X, polyPoints[4].Y));
            if (polyPoints.Count > 7)
            {
                pentagram.Points.Add(new Point(polyPoints[6].X, polyPoints[6].Y));
            }
            MyGraphic myGraphicPentagram = new MyGraphic(myGraphicId, myGraphicStrokes, pentagram);
            myGraphicPentagram.ShapeType = "pentagram";
            myGraphicPentagram.PolyPoints = polyPoints;
            Command amgcPentagram = new AddMyGraphicCommand(_inkCollector, myGraphicPentagram);
            amgcPentagram.execute();
            _inkCollector.CommandStack.Push(amgcPentagram);
            polyPoints.Clear();
            MyGraphic nearestMyGraphic = null;//距离五角星最近的图形
            double nearestDistance = PentagramMaxDistance;           
            foreach (MyGraphic mg in _inkCollector.Sketch.MyGraphics)
            {
                if (mg.ShapeType == "rectangle" || mg.ShapeType == "ellipse")
                {
                    double distance = MathTool.getInstance().distanceR2R(mg.Strokes.GetBounds(), myGraphicStrokes.GetBounds());
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestMyGraphic = mg;
                    }
                }
            }
            if (nearestMyGraphic != null)
            {
                nearestMyGraphic.PentagramStrokes = myGraphicStrokes;
            }
            //五角星图形提取
            List<MyGraphic> PentagramGraphics = new List<MyGraphic>();
            _inkCollector._mainPage._pentagramExtractive.pentagramListBox.Items.Clear();
            foreach (MyGraphic mg in _inkCollector._mainPage.InkCollector.Sketch.MyGraphics)
            {
                if (mg.PentagramStrokes != null)
                {
                    _inkCollector._mainPage._pentagramExtractive.Visibility = Visibility.Visible;
                    PentagramGraphics.Add(mg);
                    _inkCollector._mainPage.message.Content = "五角星" + mg.MyGraphicID.ToString() + "五角星";
                    ListBoxItem listBoxItem = new ListBoxItem();
                    listBoxItem.Height = 80;
                    listBoxItem.Width = 195;
                    //笔迹显示
                    InkCanvas ic = new InkCanvas();
                    ic.EditingMode = InkCanvasEditingMode.None;
                    ic.Width = listBoxItem.Width;
                    ic.Height = listBoxItem.Height;
                    ic.Background = _inkCollector._mainPage._pentagramExtractive.pentagramListBox.Background;
                    StrokeCollection icStrokes = new StrokeCollection();
                    foreach (Stroke s in _inkCollector._mainPage.InkCollector.Sketch.getMyGraphicByID(mg.MyGraphicID).Strokes)
                    {
                        System.Windows.Input.StylusPointCollection spc = new System.Windows.Input.StylusPointCollection();
                        foreach (System.Windows.Input.StylusPoint sp in s.StylusPoints)
                        {
                            spc.Add(sp);
                        }
                        Stroke ics = new Stroke(spc);
                        icStrokes.Add(ics);
                    }
                    foreach (Stroke s in _inkCollector._mainPage.InkCollector.Sketch.getMyGraphicByID(mg.MyGraphicID).textStrokeCollection)
                    {
                        System.Windows.Input.StylusPointCollection spc = new System.Windows.Input.StylusPointCollection();
                        foreach (System.Windows.Input.StylusPoint sp in s.StylusPoints)
                        {
                            spc.Add(sp);
                        }
                        Stroke ics = new Stroke(spc);
                        icStrokes.Add(ics);
                    }
                    //笔迹位置规整
                    Rect bound = _inkCollector._mainPage.InkCollector.Sketch.getMyGraphicByID(mg.MyGraphicID).Strokes.GetBounds();
                    double moveX = bound.Left;
                    double moveY = bound.Top;
                    ic.Strokes.Add(icStrokes);
                    MathTool.getInstance().MoveStrokes(ic.Strokes, -moveX + PentagramZoomStrokePadding, -moveY + PentagramZoomStrokePadding);
                    //笔迹大小规整
                    double scaling = (listBoxItem.Height - PentagramZoomStrokePadding * 2) / bound.Height;
                    InkTool.getInstance().ZoomStrokes(ic.Strokes, scaling, new System.Windows.Input.StylusPoint(0, 0));

                    listBoxItem.Content = ic;
                    _inkCollector._mainPage._pentagramExtractive.pentagramListBox.Items.Add(listBoxItem);
                }
            }
        }
        /// <summary>
        /// 三角形，暂时没用
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        /// <param name="myGraphicId"></param>
        /// <param name="pointFirst"></param>
        private void triangle(StrokeCollection myGraphicStrokes, int myGraphicId, Point pointFirst)
        {
            System.Windows.Shapes.Polygon triangle = new Polygon();
            triangle.Points.Add(new Point(pointFirst.X, pointFirst.Y));
            triangle.Points.Add(new Point(polyPoints[0].X, polyPoints[0].Y));
            triangle.Points.Add(new Point(polyPoints[2].X, polyPoints[2].Y));

            MyGraphic myGraphicTriangle = new MyGraphic(myGraphicId, myGraphicStrokes, triangle);
            myGraphicTriangle.ShapeType = "triangle";
            myGraphicTriangle.PolyPoints = polyPoints;
            Command amgcTriangle = new AddMyGraphicCommand(_inkCollector, myGraphicTriangle);
            amgcTriangle.execute();
            _inkCollector.CommandStack.Push(amgcTriangle);
            polyPoints.Clear();
        }
        /// <summary>
        /// 矩形
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        /// <param name="myGraphicId"></param>
        private void rectangle(StrokeCollection myGraphicStrokes, int myGraphicId)
        {            
            Rectangle rectangle = new Rectangle();
            Rect bound = myGraphicStrokes.GetBounds();
            rectangle.Margin = new Thickness(bound.Left, bound.Top, 0, 0);
            rectangle.Width = bound.Width;
            rectangle.Height = bound.Height;
            rectangle.StrokeThickness = 2;
            MyGraphic myGraphicRectangle = new MyGraphic(myGraphicId, myGraphicStrokes, rectangle);
            
            myGraphicRectangle.ShapeType = "rectangle";
            Command amgcRectangle = new AddMyGraphicCommand(_inkCollector, myGraphicRectangle);
            amgcRectangle.execute();
            _inkCollector.CommandStack.Push(amgcRectangle);
            //查找关联直线
            GraphicMathTool.getInstance().SearchRelationByPosition(myGraphicRectangle, _inkCollector.Sketch.MyGraphics, _inkCollector);
        }
        /// <summary>
        /// 椭圆
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        /// <param name="myGraphicId"></param>
        private void ellipse(StrokeCollection myGraphicStrokes, int myGraphicId)
        {
            System.Windows.Shapes.Ellipse ellipse = new Ellipse();
            Rect bound=myGraphicStrokes.GetBounds();
            ellipse.Margin = new Thickness(bound.Left, bound.Top, 0, 0);
            ellipse.Width = bound.Width;
            ellipse.Height = bound.Height;
            MyGraphic myGraphicCircle = new MyGraphic(myGraphicId, myGraphicStrokes, ellipse);
            myGraphicCircle.ShapeType = "ellipse";
            Command amgcCircle = new AddMyGraphicCommand(_inkCollector, myGraphicCircle);
            amgcCircle.execute();
            _inkCollector.CommandStack.Push(amgcCircle);
            //查找关联直线
            GraphicMathTool.getInstance().SearchRelationByPosition(myGraphicCircle, _inkCollector.Sketch.MyGraphics, _inkCollector);
        }
        /// <summary>
        /// 循环
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        /// <param name="myGraphicId"></param>
        /// <param name="pointFirst"></param>
        /// <param name="pointLast"></param>
        private void loopArc(StrokeCollection myGraphicStrokes, int myGraphicId, Point pointFirst, Point pointLast)
        {
            System.Windows.Shapes.Line lineLoopArc =
                    ShapeLineBy2Points(pointLast.X
                                       , pointLast.Y
                                       , pointFirst.X
                                       , pointFirst.Y);
            Point Stroke1BoundCenter_loop = new Point(myGraphicStrokes[1].GetBounds().Left
                , myGraphicStrokes[1].GetBounds().Top + myGraphicStrokes[1].GetBounds().Height / 2);
            double distanceToLinePoint1_loop = MathTool.getInstance().distanceP2P(Stroke1BoundCenter_loop, pointFirst);
            double distanceToLinePoint2_loop = MathTool.getInstance().distanceP2P(Stroke1BoundCenter_loop, pointLast);
            if (distanceToLinePoint1_loop > distanceToLinePoint2_loop)
            {
                lineLoopArc =
                    ShapeLineBy2Points(pointFirst.X
                                         , pointFirst.Y
                                         , pointLast.X
                                         , pointLast.Y);
            }
            MyGraphic myGraphicLoopArc = new MyGraphic(myGraphicId, myGraphicStrokes, lineLoopArc);
            myGraphicLoopArc.ShapeType = "loopArc";
            Command amgcLoopArc = new AddMyGraphicCommand(_inkCollector, myGraphicLoopArc);
            amgcLoopArc.execute();
            _inkCollector.CommandStack.Push(amgcLoopArc);
            GraphicMathTool.getInstance().SearchRelationByPosition(myGraphicLoopArc, _inkCollector.Sketch.MyGraphics, _inkCollector);
        }
        /// <summary>
        /// 自循环
        /// </summary>
        /// <param name="myGraphicStrokes"></param>
        /// <param name="myGraphicId"></param>
        /// <param name="pointFirst"></param>
        /// <param name="pointLast"></param>
        private void loopArcSelf(StrokeCollection myGraphicStrokes, int myGraphicId, Point pointFirst, Point pointLast)
        {
            System.Windows.Shapes.Line lineLoopArcSelf = ShapeLineBy2Points(pointFirst.X
                                                         , pointFirst.Y
                                                         , pointLast.X
                                                         , pointLast.Y);
            MyGraphic myGraphicLoopArcSelf = new MyGraphic(myGraphicId, myGraphicStrokes, lineLoopArcSelf);
            myGraphicLoopArcSelf.ShapeType = "loopArcSelf";
            Command amgcLoopArcSelf = new AddMyGraphicCommand(_inkCollector, myGraphicLoopArcSelf);
            amgcLoopArcSelf.execute();
            _inkCollector.CommandStack.Push(amgcLoopArcSelf);
            GraphicMathTool.getInstance().SearchRelationByPosition(myGraphicLoopArcSelf, _inkCollector.Sketch.MyGraphics, _inkCollector);            
        }
        

        /// <summary>
        /// 在层次图中插入矩形，插入位置以下的图形，向下移动
        /// </summary>
        /// <param name="insertMyGraphicRight"></param>
        /// <param name="mg2InsertList"></param>
        /// <param name="index"></param>
        private void InsertRectangleMoveOther(MyGraphic insertMyGraphic, List<MyGraphic> mg2InsertList,int index)
        {
            List<MyGraphic> mg2MoveList = GraphicMathTool.getInstance().getDirectDownRelativeMyGraphicNoSelf(mg2InsertList[index]
                , _inkCollector, new List<MyGraphic>());
            mg2MoveList.Add(mg2InsertList[index]);
            Command mgsmc = new MyGraphicsMoveCommand(mg2MoveList, 0, insertMyGraphic.Strokes.GetBounds().Height + 15
                , _inkCollector);
            mgsmc.execute();
            _inkCollector.CommandStack.Push(mgsmc);
            Command mgmc = new MyGraphicMoveCommand(insertMyGraphic
                , mg2InsertList[index == 0 ? 1 : 0].Strokes.GetBounds().Left - insertMyGraphic.Strokes.GetBounds().Left
                , mg2InsertList[index == 0 ? 1 : 0].Strokes.GetBounds().Top
                + mg2InsertList[index == 0 ? 1 : 0].Strokes.GetBounds().Height + 15 - insertMyGraphic.Strokes.GetBounds().Top, _inkCollector);
            mgmc.execute();
            _inkCollector.CommandStack.Push(mgmc);
        }

        /// <summary>
        /// 根据给定点坐标形成直线
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
         private static System.Windows.Shapes.Line ShapeLineBy2Points(double x1,double y1,double x2, double y2)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            return line;
        }
        /// <summary>
        /// 创建手势库
        /// </summary>
         public void createGesture()
         {
             gesture = new MouseGesture();

             //圆形
             gesture.AddGesture("ellipse", "076543210", null);//逆时针
             gesture.AddGesture("ellipse", "123456701", null);//顺时针
             gesture.AddGesture("ellipse", "234567012", null);//顺时针
             gesture.AddGesture("ellipse", "345670123", null);//顺时针
             gesture.AddGesture("ellipse", "456701234", null);//顺时针
             gesture.AddGesture("ellipse", "567012345", null);//顺时针
             gesture.AddGesture("ellipse", "670123456", null);//顺时针
             gesture.AddGesture("ellipse", "701234567", null);//顺时针
             gesture.AddGesture("ellipse", "432107654", null);//逆时针
             gesture.AddGesture("ellipse", "321076543", null);//逆时针
             gesture.AddGesture("ellipse", "210765432", null);//逆时针
             gesture.AddGesture("ellipse", "107654321", null);//逆时针
             gesture.AddGesture("ellipse", "076543210", null);//逆时针
             gesture.AddGesture("ellipse", "765432107", null);//逆时针
             gesture.AddGesture("ellipse", "654321076", null);//逆时针
             gesture.AddGesture("ellipse", "543210765", null);//逆时针

             //矩形
             gesture.AddGesture("rectangle", "0246", null);//顺时针
             gesture.AddGesture("rectangle", "2460", null);//顺时针
             gesture.AddGesture("rectangle", "4602", null);//顺时针
             gesture.AddGesture("rectangle", "6024", null);//顺时针
             gesture.AddGesture("rectangle", "02460", null);//顺时针
             gesture.AddGesture("rectangle", "24602", null);//顺时针
             gesture.AddGesture("rectangle", "46024", null);//顺时针
             gesture.AddGesture("rectangle", "60246", null);//顺时针
             gesture.AddGesture("rectangle", "4206", null);//逆时针
             gesture.AddGesture("rectangle", "2064", null);//逆时针
             gesture.AddGesture("rectangle", "0642", null);//逆时针
             gesture.AddGesture("rectangle", "6420", null);//逆时针
             gesture.AddGesture("rectangle", "42064", null);//逆时针
             gesture.AddGesture("rectangle", "20642", null);//逆时针
             gesture.AddGesture("rectangle", "06420", null);//逆时针
             gesture.AddGesture("rectangle", "64206", null);//逆时针
             gesture.AddGesture("rectangle", "260240", null);//逆时针

             //三角形
             gesture.AddGesture("triangle", "147", null);//顺时针
             gesture.AddGesture("triangle", "471", null);//顺时针
             gesture.AddGesture("triangle", "714", null);//顺时针
             gesture.AddGesture("triangle", "035", null);//顺时针
             gesture.AddGesture("triangle", "350", null);//顺时针
             gesture.AddGesture("triangle", "530", null);//顺时针
             gesture.AddGesture("triangle", "417", null);//逆时针
             gesture.AddGesture("triangle", "174", null);//逆时针
             gesture.AddGesture("triangle", "417", null);//逆时针
             gesture.AddGesture("triangle", "305", null);//逆时针
             gesture.AddGesture("triangle", "053", null);//逆时针
             gesture.AddGesture("triangle", "530", null);//逆时针
             gesture.AddGesture("triangle", "37140", null);//逆时针

             //五角星
             gesture.AddGesture("pentagram", "03715", null);
             gesture.AddGesture("pentagram", "73715", null);
             gesture.AddGesture("pentagram", "37150", null);
             gesture.AddGesture("pentagram", "71503", null);
             gesture.AddGesture("pentagram", "15037", null);
             gesture.AddGesture("pentagram", "50371", null);

             //插入矩形手势<
             gesture.AddGesture("insertRectangleRight", "31", null);//右边插入手势

             //插入矩形手势>
             gesture.AddGesture("insertRectangleLeft", "13", null);//左边插入手势

             //删除图形手势
             gesture.AddGesture("deleteGesture", "03030", null);//删除手势
             gesture.AddGesture("deleteGesture", "030", null);//删除手势
             gesture.AddGesture("deleteGesture", "0303030", null);//删除手势
             gesture.AddGesture("deleteGesture", "030303030", null);//删除手势
             gesture.AddGesture("deleteGesture", "04040", null);//删除手势
             gesture.AddGesture("deleteGesture", "163", null);//删除手势X
             gesture.AddGesture("deleteGesture", "361", null);//删除手势X

             //清空面板手势
             gesture.AddGesture("clearAllGesture", "432107654203030", null);//清空面板手势

             //undo手势
             gesture.AddGesture("undoGesture", "654", null);//undo手势

             //redo手势
             gesture.AddGesture("redoGesture", "670", null);//redo手势

             //check手势
             gesture.AddGesture("checkGesture", "17", null);//check手势

             //save手势:S
             //gesture.AddGesture("saveGesture", "54321012345", null);//save手势

             //open手势:N
             //gesture.AddGesture("openGesture", "616", null);//open手势

             //inserImage手势:M
             //gesture.AddGesture("inserImageGesture", "7171", null);//插入图片手势
             //gesture.AddGesture("inserImageGesture", "6172", null);//插入图片手势

             //inserText手势:W
             //gesture.AddGesture("inserTextGesture", "1717", null);//插入Text手势

             //pencil手势:P
             //gesture.AddGesture("pencilGesture", "2667012345", null);//pencil手势

             //severRelationGesture解除图形关系手势
             gesture.AddGesture("severRelationGesture", "670123456770", null);//解除图形关系手势
             gesture.AddGesture("severRelationGesture", "012345670012", null);//解除图形关系手势

             //弧形循环线loopArc
             gesture.AddGesture("loopArc", "67012517", null);//向右
             gesture.AddGesture("loopArc", "21076371", null);//向右
             gesture.AddGesture("loopArc", "65432017", null);//向右
             gesture.AddGesture("loopArc", "23456071", null);//向右
             //gesture.AddGesture("loopArc", "0124731", null);//向右下
             gesture.AddGesture("loopArc", "0124731", null);//向下
             gesture.AddGesture("loopArc", "43210513", null);//向下
             gesture.AddGesture("loopArc", "07654231", null);//向下
             gesture.AddGesture("loopArc", "45670213", null);//向下
             //gesture.AddGesture("loopArc", "0124731", null);//向左下
             gesture.AddGesture("loopArc", "65432517", null);//向左
             gesture.AddGesture("loopArc", "23456371", null);//向左
             gesture.AddGesture("loopArc", "67012417", null);//向左
             gesture.AddGesture("loopArc", "21076471", null);//向左
             //gesture.AddGesture("loopArc", "0124731", null);//向左上
             gesture.AddGesture("loopArc", "07654731", null);//向上
             gesture.AddGesture("loopArc", "45670513", null);//向上
             gesture.AddGesture("loopArc", "43210653", null);//向上
             gesture.AddGesture("loopArc", "01234631", null);//向上
             //gesture.AddGesture("loopArc", "0124731", null);//向右上


             //自循环loopArcSelf
             gesture.AddGesture("loopArcSelf", "45670123731", null);//顺时针
             gesture.AddGesture("loopArcSelf", "67012345371", null);//顺时针
             gesture.AddGesture("loopArcSelf", "23456710517", null);//顺时针
             gesture.AddGesture("loopArcSelf", "01234567513", null);//顺时针
             gesture.AddGesture("loopArcSelf", "07654321513", null);//逆时针
             gesture.AddGesture("loopArcSelf", "21076543517", null);//逆时针
             gesture.AddGesture("loopArcSelf", "43210765731", null);//逆时针
             gesture.AddGesture("loopArcSelf", "65432107371", null);//逆时针

             //箭头
             gesture.AddGesture("arrow", "0513", null);//->水平向右 双笔迹
             gesture.AddGesture("arrow", "4013", null);//->水平向右 双笔迹
             gesture.AddGesture("arrow", "4713", null);//->水平向右 双笔迹
             gesture.AddGesture("arrow", "40137", null);//->水平向右 三笔迹
             gesture.AddGesture("arrow", "47137", null);//->水平向右 三笔迹
             gesture.AddGesture("arrow", "05137", null);//->水平向右 三笔迹
             gesture.AddGesture("arrow", "4731", null);//->水平向左 双笔迹和三笔迹
             gesture.AddGesture("arrow", "0431", null);//水平向左 双笔迹和三笔迹
             gesture.AddGesture("arrow", "0531", null);//水平向左 双笔迹和三笔迹
             gesture.AddGesture("arrow", "04315", null);//水平向左 双笔迹和三笔迹
             gesture.AddGesture("arrow", "05315", null);//水平向左 双笔迹和三笔迹
             gesture.AddGesture("arrow", "2517", null);//垂直向下 双笔迹
             gesture.AddGesture("arrow", "25173", null);//垂直向下 三笔迹
             gesture.AddGesture("arrow", "62517", null);//垂直向下 双笔迹
             gesture.AddGesture("arrow", "625173", null);//垂直向下 三笔迹
             gesture.AddGesture("arrow", "6371", null);//垂直向上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "26371", null);//垂直向上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "1624", null);//右下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "16240", null);//右下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "51624", null);//右下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "516240", null);//右下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "1406", null);//右下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "14062", null);//右下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "51406", null);//右下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "514062", null);//右下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "3620", null);//左下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "36204", null);//左下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "73620", null);//左下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "736204", null);//左下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "3046", null);//左下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "30462", null);//左下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "73046", null);//左下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "730462", null);//左下 双笔迹和三笔迹
             gesture.AddGesture("arrow", "5260", null);//左上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "52604", null);//左上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "15260", null);//左上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "152604", null);//左上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "5042", null);//左上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "50426", null);//左上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "15042", null);//左上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "150429", null);//左上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "7402", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "74026", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "37402", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "374026", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "7264", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "72640", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "37264", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "372640", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "7372", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "73726", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "3726", null);//右上 双笔迹和三笔迹
             gesture.AddGesture("arrow", "372", null);//右上 双笔迹和三笔迹


             //折线箭头
             //gesture.AddGesture("polylineArrow", "205137", null);//L> 向右 四笔
             //gesture.AddGesture("polylineArrow", "20513", null);//L> 向右 三笔
             //gesture.AddGesture("polylineArrow", "025173", null);//-|\/ 向下 四笔
             //gesture.AddGesture("polylineArrow", "02517", null);//-|\/ 向下 三笔
             //gesture.AddGesture("polylineArrow", "247315", null);//-|\/ 向左 四笔
             //gesture.AddGesture("polylineArrow", "24731", null);//-|\/ 向左 三笔

             gesture.GestureMatchEvent += new MouseGesture.GestureMatchDelegate(gesture_GestureMatchEvent);
         }

        /// <summary>
        /// 方向和笔序识别结果匹配和处理
        /// </summary>
        /// <param name="args"></param>
         void gesture_GestureMatchEvent(MouseGestureEventArgs args)
         {

             Stroke stroke = strokes[strokes.Count - 1];
             StrokeCollection myGraphicStrokes = new StrokeCollection(strokes);
             Rect bound = myGraphicStrokes.GetBounds();
             int myGraphicId = _inkCollector.Sketch.MyGraphics.Count == 0 ? 1 :
                 _inkCollector.Sketch.MyGraphics[_inkCollector.Sketch.MyGraphics.Count - 1].MyGraphicID + 1;
             Point pointFirst = new Point(myGraphicStrokes[0].StylusPoints[0].X, myGraphicStrokes[0].StylusPoints[0].Y);//第一笔的首点
             Point pointLast = new Point(myGraphicStrokes[0].StylusPoints[myGraphicStrokes[0].StylusPoints.Count - 1].X
                                         , myGraphicStrokes[0].StylusPoints[myGraphicStrokes[0].StylusPoints.Count - 1].Y);//第一笔的尾点            
             polyPoints = gesture.polyPoints;
             switch (args.Present)
             {

                 case "loopArcSelf"://自循环
                     loopArcSelf(myGraphicStrokes, myGraphicId, pointFirst, pointLast);
                     _inkCollector._mainPage.message.Content = "自循环";
                     break;

                 case "loopArc"://循环
                     _inkCollector._mainPage.message.Content = "循环";
                     loopArc(myGraphicStrokes, myGraphicId, pointFirst, pointLast);
                     break;

                 case "ellipse"://圆形
                     ellipse(myGraphicStrokes, myGraphicId);
                     _inkCollector._mainPage.message.Content = "椭圆";
                     break;

                 case "rectangle"://矩形
                     rectangle(myGraphicStrokes, myGraphicId);
                     _inkCollector._mainPage.message.Content = "矩形";
                     break;

                 //case "triangle"://三角形
                 //    triangle(myGraphicStrokes, myGraphicId, pointFirst);
                 //    Console.WriteLine("三角形");
                 //    _inkCollector._mainPage.message.Content += "三角形";                    
                 //    break;

                 case "pentagram"://五角星
                     _inkCollector._mainPage.message.Content = "五角星";
                     pentagram(myGraphicStrokes, myGraphicId, pointFirst);
                     break;

                 case "insertRectangleRight"://右边插入矩形手势
                     _inkCollector._mainPage.message.Content = "右边插入矩形";
                     insertRectangleRight(stroke, myGraphicStrokes);
                     break;

                 case "insertRectangleLeft"://左边插入矩形手势
                     _inkCollector._mainPage.message.Content = "左边插入矩形";
                     insertRectangleLeft(stroke, myGraphicStrokes);
                     break;

                 case "deleteGesture"://删除手势
                     _inkCollector._mainPage.message.Content = "删除手势";
                     deleteGesture(myGraphicStrokes);
                     break;

                 case "clearAllGesture"://清空画板手势
                     _inkCollector._mainPage.message.Content = "清空画板手势";
                     clearAllGesture(stroke);
                     break;

                 case "undoGesture"://撤销
                     undoGesture(stroke);
                     _inkCollector._mainPage.message.Content = "撤销";
                     break;

                 case "redoGesture"://反撤销
                     redoGesture(stroke);
                     _inkCollector._mainPage.message.Content = "redoGesture";
                     break;

                 case "saveGesture"://保存手势
                     saveGesture(stroke);
                     _inkCollector._mainPage.message.Content = "保存";
                     break;

                 case "openGesture"://打开文件手势
                     _inkCollector._mainPage.message.Content = "打开";
                     openGesture(stroke);
                     break;

                 case "inserImageGesture"://插入图片
                     _inkCollector._mainPage.message.Content = "inserImageGesture";
                     inserImageGesture(stroke);
                     break;

                 case "inserTextGesture"://插入文本
                     _inkCollector._mainPage.message.Content = "inserImageGesture";
                     _inkCanvas.Strokes.Remove(myGraphicStrokes);
                     break;

                 case "pencilGesture"://画笔状态
                     _inkCollector.Mode = InkMode.Ink;
                     _inkCollector._mainPage.message.Content = "pencilGesture";
                     _inkCanvas.Strokes.Remove(myGraphicStrokes);
                     break;

                 case "checkGesture":
                     _inkCollector._mainPage.message.Content = "checkGesture";
                     _inkCanvas.Strokes.Remove(myGraphicStrokes);
                     break;

                 case "severRelationGesture"://解除图形关系
                     _inkCollector._mainPage.message.Content = "解除图形关系";
                     severRelationGesture(myGraphicStrokes);
                     break;

                 case "arrow"://箭头
                     _inkCollector._mainPage.message.Content = "arrow";
                     arrow(myGraphicStrokes, myGraphicId, new System.Windows.Input.StylusPoint(pointFirst.X,pointFirst.Y),
                         new System.Windows.Input.StylusPoint(pointLast.X, pointLast.Y));
                     break;

                 //case "polylineArrow"://折线箭头
                 //    _inkCollector._mainPage.message.Content += "折线箭头";
                 //    polylineArrow(myGraphicStrokes, myGraphicId, pointFirst);
                 //    break;

                 default:
                     _inkCollector._mainPage.message.Content = "default";
                     _inkCanvas.Strokes.Remove(myGraphicStrokes);
                     break;
             }
             polyPoints.Clear();
             if (_inkCollector.Sketch.MyGraphics.Count > 0)
             {
                 List<int> ids = GraphicMathTool.getInstance().getGraphicStructure(_inkCollector.Sketch.MyGraphics[0], _inkCollector, new List<int>());
                 //foreach (int id in ids)
                 //{
                 //    _inkCollector._mainPage.message.Content += id.ToString() + ",";
                 //}
             }
         }
        #endregion
    }
}
