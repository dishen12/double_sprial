using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using Microsoft.Ink;



namespace WPFInk
{
    public partial class PointView : UserControl
    {
        // 节点列表，用于生成连接线:准备用node类型来替换Point类型，作为节点
        public InkFrame inkFrame;
        public WPFInk.ink.InkCollector _inkCollector;
        public List<Node> nodeList = new List<Node>();
        List<Node> hypernodeList = new List<Node>();
        public List<Link> links = new List<Link>();
        public List<Link> hyperLinks = new List<Link>();//超链接

        public int selectIndex = -1;  //标记在nodeList中结点的位置
        //int selectHyperIndex = -1;
        int sign = 1;  //标记状态：值为1 时，进行结点移动与加边操作；值为－1时，识别手势操作。

        int nodeindex = -1;
        //int hypernodeindex = -1;

        int redLineStartIndex = -1;
        int redLineEndIndex = -1;
        double ministDistance;

        int count = 0;

        public bool WindowsStype = false;
        public bool lineStyle = true;

        private int FindNearestNode(List<Node> list, Point PT, double minDistance)
        {
            //Point resultNode = new Point(0,0);
            int resultIndex = -1;
            double temp;
            for (int i = 0; i < list.Count; i++)
            {
                temp = Math.Sqrt((list[i].point.X - PT.X) * (list[i].point.X - PT.X) + (list[i].point.Y - PT.Y) * (list[i].point.Y - PT.Y));
                if (temp < minDistance)
                {
                    //resultNode = list[i].point;
                    minDistance = temp;
                    resultIndex = i;
                }
            }
            return resultIndex;
        }

        private bool IsPointNull(Point pt)
        {
            if (pt.X == 0 && pt.Y == 0)
                return true;
            return false;
        }


        private Node GetNodeFromLocation(Point location)
        {
            foreach (Node node in nodeList)
            {
                if (Math.Sqrt((node.point.X - presentlocation.X) * (node.point.X - presentlocation.X) + (node.point.Y - presentlocation.Y) * (node.point.Y - presentlocation.Y)) < node.radius)
                {
                    return node;
                }
            }
            return null;
        }
        private Node GetNode(int i, List<Node> list)
        {
            foreach (Node node in list)
            {
                if (node.index == i)
                {
                    return node;
                }
            }
            return null;
        }

        //public void deleteNode(int )

        private void DeleteNode(int j, List<Node> list)
        {
            for (int i = links.Count - 1; i >= 0; i--)
            {
                if (links[i].StartPtIndex == j || links[i].EndPtIndex == j)
                {
                    links.RemoveAt(i);
                }
            }
            for (int i = hyperLinks.Count - 1; i >= 0; i--)
            {
                if (hyperLinks[i].StartPtIndex == j)
                {
                    hyperLinks.RemoveAt(i);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].index == j)
                {
                    list.RemoveAt(i);
                }
            }
        }
        private void delehypernode(int j, List<Node> list)
        {
            for (int i = hyperLinks.Count - 1; i >= 0; i--)
            {
                if (hyperLinks[i].EndPtIndex == j)
                {
                    hyperLinks.RemoveAt(i);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].index == j)
                {
                    list.RemoveAt(i);
                }
            }
        }

        //计算任意一点到一般链接的距离
        private double Distance1(Point p, Link link)//此方法的功能是什么？
        {
            Point p1 = GetNode(link.StartPtIndex, nodeList).point;
            Point p2 = GetNode(link.EndPtIndex, nodeList).point;
            if ((p1.X == p2.X) && ((p.Y - p1.Y) * (p.Y - p2.Y) <= 0))
            {
                return Math.Abs(p.X - p1.X);
            }
            else if ((p1.X == p2.X) && ((p.Y - p1.Y) * (p.Y - p2.Y) > 0))
            {
                if (Math.Abs(p1.Y - p.Y) >= Math.Abs(p2.Y - p.Y))
                {
                    return Math.Sqrt((p2.X - p.X) * (p2.X - p.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
                }
                else
                {
                    return Math.Sqrt((p1.X - p.X) * (p1.X - p.X) + (p.Y - p1.Y) * (p.Y - p1.Y));
                }
            }
            else if ((p1.Y == p2.Y) && ((p.X - p1.X) * (p.X - p2.X) <= 0))
            {
                return Math.Abs(p.Y - p1.Y);
            }
            else if ((p1.Y == p2.Y) && ((p.X - p1.X) * (p.X - p2.X) > 0))
            {
                if (Math.Abs(p1.X - p.X) >= Math.Abs(p2.X - p.X))
                {
                    return Math.Sqrt((p2.X - p.X) * (p2.X - p.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
                }
                else
                {
                    return Math.Sqrt((p1.X - p.X) * (p1.X - p.X) + (p.Y - p1.Y) * (p.Y - p1.Y));
                }
            }
            else
            {
                double distance = 0.0;
                //double K, B;

                int n = (p2.X - p1.X) * (p.X - p1.X) + (p2.Y - p1.Y) * (p.Y - p1.Y);
                int m = (p.X - p2.X) * (p1.X - p2.X) + (p.Y - p2.Y) * (p1.Y - p2.Y);
                if (m >= 0 && n >= 0)
                {
                    double l1 = Math.Sqrt((p.X - p1.X) * (p.X - p1.X) + (p.Y - p1.Y) * (p.Y - p1.Y));
                    double l2 = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
                    double l3 = Math.Sqrt((p.X - p2.X) * (p.X - p2.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
                    double s = (l1 + l2 + l3) * (l1 + l2 - l3) * (l1 + l3 - l2) * (l2 + l3 - l1) / 4;
                    double area = Math.Sqrt(s);
                    distance = area * 2 / l2;
                }
                else if (m > 0 && n <= 0)
                {
                    distance = Math.Sqrt((p.X - p2.X) * (p.X - p2.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
                }
                else if (m <= 0 && n > 0)
                {
                    distance = Math.Sqrt((p.X - p1.X) * (p.X - p1.X) + (p.Y - p1.Y) * (p.Y - p1.Y));
                }
                return distance;
            }

        }

        //计算任意一点到超链接的距离
        private double Distance2(Point p, Link link)
        {
            Point p1 = GetNode(link.StartPtIndex, nodeList).point;
            //Point p2 = GetNode(link.EndPtIndex, hypernodeList).point;
            Point p2 = GetNode(link.EndPtIndex, nodeList).point;
            if ((p1.X == p2.X) && ((p.Y - p1.Y) * (p.Y - p2.Y) <= 0))
            {
                return Math.Abs(p.X - p1.X);
            }
            if ((p1.X == p2.X) && ((p.Y - p1.Y) * (p.Y - p2.Y) > 0))
            {
                if (Math.Abs(p1.Y - p.Y) >= Math.Abs(p2.Y - p.Y))
                {
                    return Math.Sqrt((p2.X - p.X) * (p2.X - p.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
                }
                else
                {
                    return Math.Sqrt((p1.X - p.X) * (p1.X - p.X) + (p.Y - p1.Y) * (p.Y - p1.Y));
                }
            }
            if ((p1.Y == p2.Y) && ((p.X - p1.X) * (p.X - p2.X) <= 0))
            {
                return Math.Abs(p.Y - p1.Y);
            }
            if ((p1.Y == p2.Y) && ((p.X - p1.X) * (p.X - p2.X) > 0))
            {
                if (Math.Abs(p1.X - p.X) >= Math.Abs(p2.X - p.X))
                {
                    return Math.Sqrt((p2.X - p.X) * (p2.X - p.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
                }
                else
                {
                    return Math.Sqrt((p1.X - p.X) * (p1.X - p.X) + (p.Y - p1.Y) * (p.Y - p1.Y));
                }
            }
            else
            {
                double distance = 0.0;
                //double K, B;

                int n = (p2.X - p1.X) * (p.X - p1.X) + (p2.Y - p1.Y) * (p.Y - p1.Y);
                int m = (p.X - p2.X) * (p1.X - p2.X) + (p.Y - p2.Y) * (p1.Y - p2.Y);
                if (m >= 0 && n >= 0)
                {
                    double l1 = Math.Sqrt((p.X - p1.X) * (p.X - p1.X) + (p.Y - p1.Y) * (p.Y - p1.Y));
                    double l2 = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
                    double l3 = Math.Sqrt((p.X - p2.X) * (p.X - p2.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
                    double s = (l1 + l2 + l3) * (l1 + l2 - l3) * (l1 + l3 - l2) * (l2 + l3 - l1) / 4;
                    double area = Math.Sqrt(s);
                    distance = area * 2 / l2;
                }
                else if (m >= 0 && n <= 0)
                {
                    distance = Math.Sqrt((p.X - p2.X) * (p.X - p2.X) + (p.Y - p2.Y) * (p.Y - p2.Y));
                }
                else if (m <= 0 && n >= 0)
                {
                    distance = Math.Sqrt((p.X - p1.X) * (p.X - p1.X) + (p.Y - p1.Y) * (p.Y - p1.Y));
                }
                return distance;
            }

        }

        #region 一些变量的声明
        // 半径 引入node类型后该变量可以删除
        //int radius = 20;
        //线型控制变量，确定是加入超链接还是一般链接
        int linechoice = 1;
        //编辑类型选择，包括三种1：无节点操作；2：增加节点；3：删除节点
        int editchoice = 1;

        //缓存当前鼠标点的位置
        Point presentlocation = new Point(0, 0);


        //记录鼠标左键是否被按下，命名存在问题，需要改进
        bool bPointBtnDown = false;

        //记录鼠标右键是否被按下
        bool bRightBtnDown = false;

        //如果被选中的为一般节点，记录鼠标按下事件时选中节点的序号
        int selectedPtIndex = -1;
        //如果被选中的为一般节点，记录鼠标弹起时选中节点的序号
        //int expandPtIndex = -1;

        //如果被选中的为超链接节点，记录被选中节点的序号
        int selectedHyperPoint = -1;

        //缓存链接线的起始点
        Point linkStartPt = new Point(0, 0);

        //缓存链接线的终止点
        Point linkEndPt = new Point(0, 0);

        //用于实现随即曲线的，记录鼠标移动事件的上一个点
        Point OldPoint = new Point(0, 0);

        //用于实现记录绘制圆形曲线的时候的最远点坐标
        Point farthestPoint = new Point(0, 0);

        //用于缓存计算最远距离
        int dis = 0;

        //用于记录链接的权重信息
        //int weight;
        //绘制一般节点的函数

        //public VideoEditor VEForm=null;
        //7.27public SketchPlayer SPForm = null;

        #endregion
        private void DrawHotPoint(Graphics g)
        {
            foreach (Node node in nodeList)
            {
                Point pt = new Point();
                pt = node.point;
                //g.FillEllipse(Brushes.Yellow, pt.X - node.radius, pt.Y - node.radius, 2 * node.radius, 2 * node.radius);
                g.FillEllipse(node.brush, pt.X - node.radius, pt.Y - node.radius, 2 * node.radius, 2 * node.radius);
                g.DrawEllipse(new Pen(Color.Green, 3.0F), pt.X - node.radius, pt.Y - node.radius, 2 * node.radius, 2 * node.radius);
                //radius = 20;
            }
            /*foreach (Node node in this.hypernodeList)
            {
                Point pt = node.point;
                g.FillEllipse(Brushes.Yellow, pt.X - 10, pt.Y - 10, 25, 15);
                g.DrawEllipse(Pens.Green, pt.X - 10, pt.Y - 10, 25, 15);
            }*/

            //if (selectedPtIndex != -1 && selectedPtIndex < nodeList.Count)
            if (selectIndex != -1 && selectIndex < nodeList.Count)
                /*g.DrawEllipse(
                    new Pen(Color.Red, 2.0F),
                    nodeList[selectedPtIndex].point.X - nodeList[selectedPtIndex].radius, nodeList[selectedPtIndex].point.Y - nodeList[selectedPtIndex].radius, 2 * nodeList[selectedPtIndex].radius, 2 * nodeList[selectedPtIndex].radius);*/
                g.DrawEllipse(
                    new Pen(Color.Red, 2.0F),
                    nodeList[selectIndex].point.X - nodeList[selectIndex].radius, nodeList[selectIndex].point.Y - nodeList[selectIndex].radius, 2 * nodeList[selectIndex].radius, 2 * nodeList[selectIndex].radius);

        }

        private void DrawSmallHotPoint(Graphics g)
        {

            foreach (Link link in hyperLinks)
            {
                //double vectorx = (double)(GetNode(link.EndPtIndex, hypernodeList).point.X - GetNode(link.StartPtIndex, nodeList).point.X);
                //double vectory = (double)(GetNode(link.EndPtIndex, hypernodeList).point.Y - GetNode(link.StartPtIndex, nodeList).point.Y);

                double vectorx = (double)(GetNode(link.EndPtIndex, nodeList).point.X - GetNode(link.StartPtIndex, nodeList).point.X);
                double vectory = (double)(GetNode(link.EndPtIndex, nodeList).point.Y - GetNode(link.StartPtIndex, nodeList).point.Y);


                double l = Math.Sqrt(vectorx * vectorx + vectory * vectory);
                vectorx = vectorx / l;
                vectory = vectory / l;

                Node node = GetNode(link.StartPtIndex, nodeList);
                int r = node.radius;
                int x = (int)(vectorx * 15 * r / 20) + GetNode(link.StartPtIndex, nodeList).point.X;
                int y = (int)(vectory * 15 * r / 20) + GetNode(link.StartPtIndex, nodeList).point.Y;
                g.FillEllipse(Brushes.Yellow, x - 5, y - 5, 10, 10);//todo
                g.DrawEllipse(Pens.Red, x - 5, y - 5, 10, 10);//todo
                //Image image = Image.FromFile("F://result.bmp");
                //int width = image.Width / 8;
                //int height = image.Height / 8;
                //g.DrawImage(image, x - width / 2, y - height / 2, width, height);
            }
        }

        private void DrawHyperLinks(Graphics g)
        {
            foreach (Link link in hyperLinks)
            {

                //double vectorx = (double)(GetNode(link.EndPtIndex, hypernodeList).point.X - GetNode(link.StartPtIndex, nodeList).point.X);
                //double vectory = (double)(GetNode(link.EndPtIndex, hypernodeList).point.Y - GetNode(link.StartPtIndex, nodeList).point.Y);

                double vectorx = (double)(GetNode(link.EndPtIndex, nodeList).point.X - GetNode(link.StartPtIndex, nodeList).point.X);
                double vectory = (double)(GetNode(link.EndPtIndex, nodeList).point.Y - GetNode(link.StartPtIndex, nodeList).point.Y);

                double l = Math.Sqrt(vectorx * vectorx + vectory * vectory);
                vectorx = vectorx / l;
                vectory = vectory / l;

                Node node = GetNode(link.StartPtIndex, nodeList);
                int r = node.radius;
                int x = (int)(vectorx * 15 * r / 20) + GetNode(link.StartPtIndex, nodeList).point.X;
                int y = (int)(vectory * 15 * r / 20) + GetNode(link.StartPtIndex, nodeList).point.Y;
                Pen penred = new Pen(Color.Red);
                //penred.DashStyle = DashStyle.Dash;
                penred.Width = 3.0F;

                g.DrawLine(penred, x, y, GetNode(link.EndPtIndex, nodeList).point.X, GetNode(link.EndPtIndex, nodeList).point.Y);
            }
        }

        /*7.27public PointView(SketchPlayer skPlayer)
        {
            this.SPForm = skPlayer;
            InitializeComponent();
        }*/
        public void SetInkFrmae(InkFrame inkF, WPFInk.ink.InkCollector inkCollector)
        {
            inkFrame = inkF;
            _inkCollector = inkCollector;
        }

        public PointView()
        {
            InitializeComponent();
        }

        /**
         * 自定义控件的初始化方法
         * */
        private void PointView_Load(object sender, EventArgs e)
        {
            inkCollector = new Microsoft.Ink.InkCollector(this.inkPictureNode);
            if (inkCollector.Enabled != true)
                inkCollector.Enabled = true;
            inkCollector.CollectionMode = Microsoft.Ink.CollectionMode.InkAndGesture;

            inkCollector.Gesture += new Microsoft.Ink.InkCollectorGestureEventHandler(inkCollector_Gesture);
            inkCollector.Stroke += new Microsoft.Ink.InkCollectorStrokeEventHandler(inkCollector_Stroke);

            inkCollector.SetGestureStatus(Microsoft.Ink.ApplicationGesture.AllGestures, true);

            //7.29改的
            //this.inkPictureNode.SetGestureStatus(ApplicationGesture.Scratchout, true);
            //this.inkPictureNode.SetGestureStatus(ApplicationGesture.Up, true);
            //this.inkPictureNode.SetGestureStatus(ApplicationGesture.Down, true);
            //this.inkPictureNode.SetGestureStatus(ApplicationGesture.Left, true);
            //this.inkPictureNode.SetGestureStatus(ApplicationGesture.Right, true);
        }

        public void initialize_NodeConnectImage()
        {
            int num = inkFrame.InkCollector.Sketch.Images.Count;
            for (int i = 0; i < num; i++)
            {
                this.nodeList.Add(new Node(50 + i * 80, 200, ""));
                nodeindex++;
                nodeList[i].index = nodeindex;
                if (i > 0)
                {
                    links.Add(new Link(i - 1, i, 0));
                    links[i - 1].Color = Color.Black;
                }
            }

        }

        //int deleteCount = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if (linechoice == 1)
            {
                linechoice = 2;
            }
            else if (linechoice == 2)
            {
                linechoice = 1;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (editchoice == 1)
            {
                editchoice = 2;
            }
            else if (editchoice == 2)
            {
                editchoice = 3;
            }
            else if (editchoice == 3)
            {
                editchoice = 1;
            }
        }
        /**
         * force directed graph layout
         * @author sunsnowad
         * @date 20100407
         * @param1 Graphics g-Graphics used to paint 
         * @param2 List<Node> nodeList-NodeList of the graph
         * @param3 List<Link> edgeList-EdgeList of the graph
         * @reference: http://en.wikipedia.org/wiki/Force-based_algorithms
         * @pseudo code :
         *  set up initial node velocities to (0,0)
 set up initial node positions randomly // make sure no 2 nodes are in exactly the same position
 loop
     total_kinetic_energy := 0 // running sum of total kinetic energy over all particles
     for each node
         net-force := (0, 0) // running sum of total force on this particular node
         
         for each other node
             net-force := net-force + Coulomb_repulsion( this_node, other_node )
         next node
         
         for each spring connected to this node
             net-force := net-force + Hooke_attraction( this_node, spring )
         next spring
         
         // without damping, it moves forever
         this_node.velocity := (this_node.velocity + timestep * net-force) * damping
         this_node.position := this_node.position + timestep * this_node.velocity
         total_kinetic_energy := total_kinetic_energy + this_node.mass * (this_node.velocity)^2
     next node
 until total_kinetic_energy is less than some small number  // the simulation has stopped moving
         * */
        /*private void forceDirectedLayout(Graphics g, List<Node> nodeList, List<Link> edgeList)
        {
            const int ITERATOR_TIME = 100;
            const double TIME_STAMP = 0.1;
            const double KINETICENERGYFILTER = 1;//总的动能的阈值，小于该值时停止循环
            const double K_OF_HOOK = 0.001;//const k of hooke attraction
            const double DAMPING = 0.1;//damping of this system, the number may between 0 and 1
            const double MASS = 1;//mass of a node , this var may be in node class
            const int K_TO_ZOOMCOULOMB = 100;//const k of zoomcoulomb
            const int TIME_INTERVAL = 200;//time interval to slow down the speed

            int iterNum = 0;// iterator times
            double totalKineticEnergy = 0;
            do
            {
                totalKineticEnergy = 0;
                foreach (Node currentNode in nodeList)
                {
                    currentNode.NetForce = new Force(0.0, 0.0);
                    //compulate coulomb
                    foreach (Node otherNode in nodeList)
                    {
                        if (otherNode != currentNode)
                        {
                            Force currentForce = new Force(0.0, 0.0);
                            double totalF = K_TO_ZOOMCOULOMB / powDisOfPoints(currentNode.point, otherNode.point);
                            currentForce.X = totalF * (currentNode.point.X - otherNode.point.X);
                            currentForce.Y = totalF * (currentNode.point.Y - otherNode.point.Y);
                            currentNode.netForce.addAForce(currentForce);
                        }
                    }

                    //this comment can be used to modify const k to let this algorithm work better
                    //Console.WriteLine("force:"+currentNode.netForce.X+","+currentNode.netForce.Y);

                    //compulate spring connected
                    List<Node> connectedNodes = getConnectedNodes(currentNode, nodeList, edgeList);
                    foreach (Node connectedNode in connectedNodes)
                    {
                        double totalFSpring = K_OF_HOOK * disOfPoints(currentNode.point, connectedNode.point);
                        Force currentForce = new Force(0.0, 0.0);
                        currentForce.X = totalFSpring * (connectedNode.point.X - currentNode.point.X);
                        currentForce.Y = totalFSpring * (connectedNode.point.Y - currentNode.point.Y);
                        currentNode.netForce.addAForce(currentForce);
                    }

                    //this comment can be used to modify const k to let this algorithm work better
                    //Console.WriteLine("force2:" + currentNode.netForce.X + "," + currentNode.netForce.Y);

                    //damping
                    currentNode.VelocityX = (currentNode.VelocityX + TIME_STAMP * currentNode.netForce.X) * DAMPING;
                    currentNode.VelocityY = (currentNode.VelocityY + TIME_STAMP * currentNode.netForce.Y) * DAMPING;
                    //currentPosition
                    currentNode.point.X = (int)(currentNode.point.X + TIME_STAMP * currentNode.VelocityX);
                    currentNode.point.Y = (int)(currentNode.point.Y + TIME_STAMP * currentNode.VelocityY);
                    //totalKineticEnergy
                    totalKineticEnergy += MASS * Math.Sqrt(currentNode.VelocityX * currentNode.VelocityX + currentNode.VelocityY * currentNode.VelocityY);
                }
                //redraw
                // draw node
                g.FillRectangle(Brushes.White, 0, 0, inkPictureNode.Width, inkPictureNode.Height);
                DrawHyperLinks(g);
                DrawHotPoint(g);
                DrawSmallHotPoint(g);


                Thread.Sleep(TIME_INTERVAL);
                iterNum++;
                ////this comment can be used to modify total kinetic energy to let this algorithm work better
                //Console.WriteLine("totalK" + totalKineticEnergy);
            } while (totalKineticEnergy > KINETICENERGYFILTER && iterNum < ITERATOR_TIME);

        }*/
        ///
        /**
         * get Nodes connected to given node
         * @author sunsnowad
         * @date 20100407
         * @param1 Node givenNode-the node who want to get connected nodes
         * this method has a bug, it will compulate twice of every connected nodes
         * TODO:
         **/
        private List<Node> getConnectedNodes(Node givenNode, List<Node> nodeList, List<Link> edgeList)
        {
            List<Node> list = new List<Node>();
            foreach (Link l in edgeList)
            {
                if (givenNode.index == l.StartPtIndex)// || givenNode.index == l.EndPtIndex)
                {
                    list.Add(GetNode(l.EndPtIndex, nodeList));
                }
                if (givenNode.index == l.EndPtIndex)
                {
                    list.Add(GetNode(l.StartPtIndex, nodeList));
                }
            }
            return list;
        }
        /**
         * get distance of two points' pow
         * */
        private double powDisOfPoints(Point p1, Point p2)
        {
            return ((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        /**
         * get distance of two points
         * */
        private double disOfPoints(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public bool InNode(Point location)
        {
            foreach (Node node in nodeList)
            {
                if (Math.Sqrt((node.point.X - location.X) * (node.point.X - location.X) + (node.point.Y - location.Y) * (node.point.Y - location.Y)) < node.radius)
                {
                    return true;
                }
            }
            return false;

        }

        private void inkPictureNode_Paint(object sender, PaintEventArgs e)
        {

            {
                e.Graphics.FillRectangle(Brushes.White, 0, 0, inkPictureNode.Width, inkPictureNode.Height);
                // draw link

                /*foreach(Link link in hyperLinks)
                {
                    Pen pen = new Pen(link.Color);
                    pen.Width = 3.0F;
                    if (link.StartPtIndex > -1 && link.EndPtIndex > -1 && link.StartPtIndex != link.EndPtIndex)
                        e.Graphics.DrawLine(pen, GetNode(link.StartPtIndex, nodeList).point, GetNode(link.EndPtIndex, nodeList).point);
                    //g.DrawLine(pen, nodeList[redLineStartIndex].point, nodeList[redLineEndIndex].point);
                }*/

                foreach (Link link in links)
                {
                    Pen pen = new Pen(link.Color);
                    pen.Width = 3.0F;
                    if (link.StartPtIndex > -1 && link.EndPtIndex > -1 && link.StartPtIndex != link.EndPtIndex)
                    {
                        e.Graphics.DrawLine(pen, GetNode(link.StartPtIndex, nodeList).point, GetNode(link.EndPtIndex, nodeList).point);
                    }
                }

                Pen penblack = new Pen(Color.Green);

                /*if (false == IsPointNull(linkStartPt)
                    && false == IsPointNull(linkEndPt))
                    e.Graphics.DrawLine(penblack, linkStartPt, linkEndPt);
                foreach (Node node in nodeList)
                {
                    foreach (Link link in hyperLinks)
                    {
                        if (link.StartPtIndex == node.index)
                        {
                            node.radius += 4;
                        }
                    }
                }*/


                DrawHyperLinks(e.Graphics);

                // draw node
                DrawHotPoint(e.Graphics);
                DrawSmallHotPoint(e.Graphics);

                if (-1 != chooseNode)
                {
                    Node node = nodeList[chooseNode];
                    Point pt = new Point();
                    pt = node.point;
                    e.Graphics.DrawEllipse(new Pen(Color.Blue, 3.0F), pt.X - node.radius, pt.Y - node.radius, 2 * node.radius, 2 * node.radius);
                    //chooseNode = -1;
                }


                int selectnode = -1;
                //int selecthypernode = -1;
                foreach (Node node in nodeList)
                {
                    if (Math.Sqrt((node.point.X - presentlocation.X) * (node.point.X - presentlocation.X) + (node.point.Y - presentlocation.Y) * (node.point.Y - presentlocation.Y)) < node.radius)
                    {
                        selectnode = node.index;
                        break;
                    }
                }
                /*foreach (Node node in hypernodeList)
                {
                    if (Math.Sqrt((node.point.X - presentlocation.X) * (node.point.X - presentlocation.X) + (node.point.Y - presentlocation.Y) * (node.point.Y - presentlocation.Y)) < node.radius)
                    {
                        selecthypernode = node.index;
                        break;
                    }
                }*/

                //if (selectnode == -1 && selecthypernode == -1)
                if (selectnode == -1)
                {
                    foreach (Link link in links)
                    {
                        if (Distance1(presentlocation, link) < 20)
                        {
                            double n = Distance1(presentlocation, link);
                            presentlocation.X += 10;
                            presentlocation.Y += 10;
                            e.Graphics.FillRectangle(Brushes.Honeydew, presentlocation.X, presentlocation.Y, 30, 30);
                            Font drawFont = new Font("Arial", 8);
                            e.Graphics.DrawRectangle(Pens.Green, presentlocation.X, presentlocation.Y, 100, 25);
                            e.Graphics.DrawString("Type：Normal link", drawFont, Brushes.Black, presentlocation);
                            string s = link.weight.ToString();
                            e.Graphics.DrawString("Weight：" + s, drawFont, Brushes.Black, new Point(presentlocation.X, presentlocation.Y + 12));
                        }
                    }
                    foreach (Link link in hyperLinks)
                    {
                        if (Distance2(presentlocation, link) < 20)
                        {
                            double n = Distance2(presentlocation, link);
                            presentlocation.X += 10;
                            presentlocation.Y += 10;
                            e.Graphics.FillRectangle(Brushes.Honeydew, presentlocation.X, presentlocation.Y, 30, 30);
                            Font drawFont = new Font("Arial", 8);


                            //e.Graphics.DrawString();

                            e.Graphics.DrawRectangle(Pens.Black, presentlocation.X, presentlocation.Y, 100, 25);
                            e.Graphics.DrawString("Type：Hyperlink", drawFont, Brushes.Black, presentlocation);
                            string s = link.weight.ToString();
                            e.Graphics.DrawString("Weight：" + s, drawFont, Brushes.Black, new Point(presentlocation.X, presentlocation.Y + 12));
                        }
                    }
                }

                else if (selectedPtIndex != -1)//todo
                {
                    presentlocation.X += 10;
                    presentlocation.Y += 10;
                    e.Graphics.FillRectangle(Brushes.Honeydew, presentlocation.X, presentlocation.Y, 30, 30);
                    Font drawFont = new Font("Arial", 8);


                    //e.Graphics.DrawString();

                    e.Graphics.DrawRectangle(Pens.Black, presentlocation.X, presentlocation.Y, 100, 25);
                    e.Graphics.DrawString("Type：Normal node", drawFont, Brushes.Black, presentlocation);



                    string strProperty = string.Empty;
                    if (selectedPtIndex != -1 && selectedPtIndex < nodeList.Count)
                        strProperty = nodeList[selectedPtIndex].Property;
                    e.Graphics.DrawString("Property：" + strProperty, drawFont, Brushes.Black, new Point(presentlocation.X, presentlocation.Y + 12));
                }
                /*else if (selecthypernode > -1)
                {
                    presentlocation.X += 10;
                    presentlocation.Y += 10;
                    e.Graphics.FillRectangle(Brushes.Honeydew, presentlocation.X, presentlocation.Y, 30, 30);
                    Font drawFont = new Font("Arial", 8);


                    //e.Graphics.DrawString();

                    e.Graphics.DrawRectangle(Pens.Black, presentlocation.X, presentlocation.Y, 100, 25);
                    e.Graphics.DrawString("Type：Hyperlink node", drawFont, Brushes.Black, presentlocation);
                    e.Graphics.DrawString("Property：", drawFont, Brushes.Black, new Point(presentlocation.X, presentlocation.Y + 12));
                }*/



                foreach (Node node in this.nodeList)
                {
                    node.DrawHyperImage(e.Graphics);
                }
            }
        }

        private void inkPictureNode_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (null != GetNodeFromLocation(e.Location))
                {
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        Point pt = nodeList[i].point;
                        if (Math.Sqrt((pt.X - e.X) * (pt.X - e.X) + (pt.Y - e.Y) * (pt.Y - e.Y)) < nodeList[i].radius)
                            inkFrame.SelNodeChange(i);
                    }
                }
            }

        }

        private void inkPictureNode_MouseClick(object sender, MouseEventArgs e)
        {

            /*{
                if (e.Button == MouseButtons.Left)
                {
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        Point pt = nodeList[i].point;
                        if (Math.Sqrt((pt.X - e.X) * (pt.X - e.X) + (pt.Y - e.Y) * (pt.Y - e.Y)) < 20)
                            //7.28 13.39expandPtIndex = nodeList[i].index;
                            inkFrame.SelNodeChange(i);
                    }
                    //7.28 13.39this.inkPictureNode.Refresh();
                    //7.28 13.39expandPtIndex = -1;
                }
            }*/
        }

        private void inkPictureNode_MouseMove(object sender, MouseEventArgs e)
        {
            if (sign != -1)
            {
                for (int i = 0; i < nodeList.Count; i++)//求初始点是否在结点上
                {
                    Node node = nodeList[i];
                    Point pt = node.point;
                    if (Math.Sqrt((pt.X - e.X) * (pt.X - e.X) + (pt.Y - e.Y) * (pt.Y - e.Y)) < node.radius)
                    {
                        selectIndex = i;
                        selectedPtIndex = nodeList[i].index;
                        if (inkFrame._inkCanvas.Children.Count / 2 == nodeList.Count || inkFrame._inkCanvas.Children.Count == nodeList.Count)
                        {
							inkFrame.InkCollector.Sketch.Images[i].Bound.Visibility = System.Windows.Visibility.Visible;
                        }

                    }
                    else
					{
						inkFrame.InkCollector.Sketch.Images[i].Bound.Visibility = System.Windows.Visibility.Collapsed;
						_inkCollector.SelectedImages.Clear();
                    }

                }

                if (-1 != selectIndex || true == bRightBtnDown)
                {
                    //7.29改的inkPictureNode.InkEnabled = false;
                    inkCollector.Enabled = false;
                    //inkPictureNode.Refresh();
                    presentlocation = e.Location;
                    if (null != GetNodeFromLocation(e.Location))
                    {
                        if (selectedPtIndex != GetNodeFromLocation(e.Location).index)
                        {
                            selectedPtIndex = GetNodeFromLocation(e.Location).index;
                        }
                    }
                    else
                        selectedPtIndex = -1;
                    if (false == bRightBtnDown && false == bPointBtnDown)
                    {
                        this.inkPictureNode.Refresh();
                    }

                    if (bPointBtnDown == true)
                    {

                        if (selectedPtIndex != -1 && editchoice != 3)
                        {
                            GetNode(selectedPtIndex, nodeList).point = e.Location;
                        }
                        this.inkPictureNode.Refresh();
                    }
                    if (bRightBtnDown == true)
                    {
                        linkEndPt = e.Location;
                        Graphics g = this.inkPictureNode.CreateGraphics();
                        Pen pen = new Pen(Color.Green);
                        pen.Width = 3.0F;
                        if (linechoice == 2)
                        {
                            pen.Color = Color.Red;
                            pen.DashStyle = DashStyle.Dash;
                        }
                        else if (linechoice == 1)
                        {
                            pen.Color = Color.Green;
                        }
                        g.DrawLine(pen, OldPoint, new Point(e.X, e.Y));
                        OldPoint.X = e.X;
                        OldPoint.Y = e.Y;
                        if ((e.X - linkStartPt.X) * (e.X - linkStartPt.X) + (e.Y - linkStartPt.Y) * (e.Y - linkStartPt.Y) > dis)
                        {
                            farthestPoint.X = e.X;
                            farthestPoint.Y = e.Y;
                            dis = (e.X - linkStartPt.X) * (e.X - linkStartPt.X) + (e.Y - linkStartPt.Y) * (e.Y - linkStartPt.Y);
                        }
                    }
                    selectIndex = -1;

                }
                else
                {
                    //7.29inkPictureNode.InkEnabled = true;
                    if (inkCollector.Enabled != true)
                        inkCollector.Enabled = true;
                    inkPictureNode.Refresh();
                }
            }
        }

        private void inkPictureNode_MouseUp(object sender, MouseEventArgs e)
        {
            if (sign != -1)
            {
                //7.29inkPictureNode.InkEnabled = false;
                inkCollector.Enabled = false;
                inkPictureNode.Refresh();
                presentlocation = e.Location;
                bPointBtnDown = false;
                bRightBtnDown = false;
                selectedPtIndex = -1;

                Graphics g = this.inkPictureNode.CreateGraphics();
                Pen arcPen = new Pen(Color.Red);
                arcPen.DashStyle = DashStyle.Dash;

                int startIndex = -1;
                int endIndex = -1;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    Point pt = nodeList[i].point;
                    if (Math.Sqrt((pt.X - linkStartPt.X) * (pt.X - linkStartPt.X) + (pt.Y - linkStartPt.Y) * (pt.Y - linkStartPt.Y)) < nodeList[i].radius)
                    {
                        startIndex = nodeList[i].index;
                        redLineStartIndex = i;
                    }
                }
                if (linechoice == 1)
                {

                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        Point pt = nodeList[i].point;
                        if (Math.Sqrt((pt.X - linkEndPt.X) * (pt.X - linkEndPt.X) + (pt.Y - linkEndPt.Y) * (pt.Y - linkEndPt.Y)) < nodeList[i].radius)
                        {
                            endIndex = nodeList[i].index;
                            redLineEndIndex = i;
                        }
                    }
                }

                if (e.Button == MouseButtons.Right)
                {
                    if ((startIndex != -1 && endIndex != -1) && (startIndex != endIndex))
                    {
                        if (true == lineStyle)
                        {
                            Link alink = new Link(startIndex, endIndex, 0);
                            alink.Color = Color.Black;
                            links.Add(alink);
                            count++;
                        }
                        else if (false == lineStyle)
                        {
                            int i = nodeList[redLineStartIndex].hyper;
                            nodeList[redLineEndIndex].Order = i;
                            Link alink = new Link(nodeList[redLineStartIndex].index, nodeList[redLineEndIndex].index, 0);
                            nodeList[redLineStartIndex]._sketchInnodevideo[i]._endIndex = redLineEndIndex;
                            if (20 == nodeList[redLineStartIndex].radius)
                                nodeList[redLineStartIndex].radius += 8;
                            nodeList[redLineStartIndex].hyper += 1;
                            alink.Color = Color.Red;
                            hyperLinks.Add(alink);
                            lineStyle = true;
                            chooseNode = -1;
                            inkPictureNode.Refresh();

                        }

                    }
                    //else if ((startIndex == endIndex) && (startIndex != -1))
                    //{
                    //    float radius = (float)(0.5 * (Math.Sqrt((linkStartPt.X - farthestPoint.X) * (linkStartPt.X - farthestPoint.X) + (linkStartPt.Y - farthestPoint.Y) * (linkStartPt.Y - farthestPoint.Y))));
                    //    Point centerPoint = new Point((int)((linkStartPt.X + farthestPoint.X) * 0.5), (int)((linkStartPt.Y + farthestPoint.Y) * 0.5));
                    //    g.DrawEllipse(arcPen, centerPoint.X - radius, centerPoint.Y - radius, radius * 2, radius * 2);
                    //    Link alink = new Link(startIndex, endIndex, radius);
                    //    alink.index = startIndex;
                    //    alink.farthestPoint = farthestPoint;


                    //    links.Add(alink);
                    //}
                    //else if (deleteCount < 2)
                    //{
                    //    links.RemoveAt(0);
                    //    deleteCount++;
                    //}

                    //else if (linechoice == 2)
                    //{
                    //    if ((startIndex != -1 && endIndex != -1))
                    //    {
                    //        hyperLinks.Add(new Link(startIndex, endIndex, 0));
                    //    }
                    //}
                }
                linkEndPt = new Point(0, 0);
                linkStartPt = new Point(0, 0);
                selectedHyperPoint = -1;

                this.inkPictureNode.Refresh();

                selectIndex = -1;
            }

            else
            {
                ministDistance = 52;
                redLineEndIndex = FindNearestNode(nodeList, e.Location, ministDistance);
                sign = 1;
                //7.29inkPictureNode.InkEnabled = true;
                inkCollector.Enabled = true;
                inkPictureNode.Refresh();
            }
        }

        private void inkPictureNode_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < nodeList.Count; i++)//求初始点是否在结点上
            {
                Node node = nodeList[i];
                Point pt = node.point;
                if (Math.Sqrt((pt.X - e.X) * (pt.X - e.X) + (pt.Y - e.Y) * (pt.Y - e.Y)) < 20)
                {
                    selectIndex = i;
                }
            }

            if (selectIndex != -1)//初始点在结点上的情况
            {
                //7.29inkPictureNode.InkEnabled = false;
                inkCollector.Enabled = false;
                inkPictureNode.Refresh();
                //清零
                farthestPoint.X = 0;
                farthestPoint.Y = 0;
                dis = 0;

                presentlocation = e.Location;
                #region Left Button
                //如果被按下的是鼠标左键
                if (e.Button == MouseButtons.Left)
                {
                    bPointBtnDown = true;
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        Node node = nodeList[i];
                        Point pt = node.point;
                        if (Math.Sqrt((pt.X - e.X) * (pt.X - e.X) + (pt.Y - e.Y) * (pt.Y - e.Y)) < 20)
                        {
                            selectedPtIndex = node.index;
                        }
                    }
                    selectedPtIndex = nodeList[selectIndex].index;

                }
                #endregion


                #region Right Button
                //如果为右键，记录当前点，设定右键被选中标记为true
                if (e.Button == MouseButtons.Right)
                {
                    linkStartPt = e.Location;
                    OldPoint = linkStartPt;
                    bRightBtnDown = true;
                }
                #endregion

                #region Middle Button
                if (e.Button == MouseButtons.Middle)
                {
                    SketchInNodeVideo snv = new SketchInNodeVideo();
                    nodeList[selectIndex]._sketchInnodevideo.Add(snv);
                    snv._startIndex = selectIndex;


                    //7.27FormAnnotationOld formAnno = new FormAnnotationOld(SPForm,selectIndex);
                    FormAnnotationOld formAnno = new FormAnnotationOld(inkFrame, selectIndex);
                    //formAnno.VideoFilePath = @"H:\SketchVis\用户评估\素材库\视频素材库\For Demo 截取片段\鸟巢-已截取.avi";
                    //formAnno.VideoFilePath = @"F:\lasvegas.avi";
                    //7.27formAnno.VideoFilePath = SPForm.videoPathList[selectIndex];
                    //7.27formAnno.ShowDialog();
                    formAnno.VideoFilePath = inkFrame.videoPathList[selectIndex];
                    formAnno.ShowDialog();
                    if (20 == nodeList[selectedPtIndex].radius)
                        nodeList[selectedPtIndex].radius += 8;//
                    //nodeList[selectIndex].hyper += 1;
                    //nodeList[selectedPtIndex].bmpList.Add(new Bitmap("F://sketch7.bmp"));
                }

                this.Refresh();

                #endregion

                selectIndex = -1;
            }

            else
            {
                ministDistance = 60;
                redLineStartIndex = FindNearestNode(nodeList, e.Location, ministDistance);
                sign = -1;
                //7.29inkPictureNode.InkEnabled = true;
                inkCollector.Enabled = true;
                inkPictureNode.Refresh();
            }
        }

        #region //一些判断的变量

        InkMathTool IMT = new InkMathTool();
        Rectangle tect = new Rectangle();
        int deleteIndex = -1;
        int deleteNodeIndex = -1;
        Point PT = new Point();
        int tempHyper = 0;
        int temp_StartIndex = -1;
        int temp_EndIndex = -1;

        double minValue = Double.MaxValue;//笔迹边框中心到最近的边的中点的距离
        int signIndex = -1;//要删除的边的索引号
        int signLink = 0;
        int _startIndex = -1;//要删除的边的起始点的索引号
        int _endIndex = -1;//要删除的边的终点的索引号
        double AnnotationNode = 50;
        int chooseNode = -1;

        #endregion

        //private void inkPictureNode_Gesture(object sender, Microsoft.Ink.InkCollectorGestureEventArgs e)
        //{
        //    Gesture gesture = e.Gestures[0];
        //    string s = gesture.Id.ToString();

        //    switch (s)
        //    {
        //        case "Scratchout":
        //            {
        //                tect = IMT.InkSpaceToPixelRect(inkPictureNode, inkPictureNode.Ink.Strokes.GetBoundingBox());
        //                PT.X = tect.X + tect.Width / 2;
        //                PT.Y = tect.Y + tect.Height / 2;
        //                for (int i = 0; i < nodeList.Count; i++)
        //                {
        //                    if (Math.Sqrt((PT.X - nodeList[i].point.X) * (PT.X - nodeList[i].point.X) + (PT.Y - nodeList[i].point.Y) * (PT.Y - nodeList[i].point.Y)) < 20)
        //                    {
        //                        deleteNodeIndex = i;
        //                        deleteIndex = nodeList[i].index;
        //                    }
        //                }
        //                if (-1 != deleteIndex && -1 != deleteNodeIndex)
        //                {
        //                    DeleteNodeAndVideo(nodeList, inkFrame.videoPathList, links, hyperLinks, deleteNodeIndex);
        //                    WPFInk.cmd.DeleteImageCommand dic = new WPFInk.cmd.DeleteImageCommand(inkFrame.InkCollector, inkFrame.InkCollector.Sketch.Images[deleteNodeIndex]);
        //                    dic.execute();
        //                }
        //                else
        //                {
        //                    DeleteLink(nodeList,links,hyperLinks);                        
        //                }
        //                inkPictureNode.Refresh();
        //                break;

        //            }

        //        case "Left":
        //        case "Right":
        //        case "Up":
        //        case "Down":
        //            {
        //                int i = nodeList[redLineStartIndex].hyper;
        //                Link alink = new Link(nodeList[redLineStartIndex].index, nodeList[redLineEndIndex].index, 0);
        //                nodeList[redLineStartIndex]._sketchInnodevideo[i]._endIndex = redLineEndIndex;
        //                if (20 == nodeList[redLineStartIndex].radius)
        //                    nodeList[redLineStartIndex].radius += 8;
        //                nodeList[redLineStartIndex].hyper += 1;
        //                alink.Color = Color.Red;
        //                hyperLinks.Add(alink);
        //                inkPictureNode.Refresh();
        //                break;
        //            }
        //        default:
        //            {
        //                inkPictureNode.Refresh();
        //                break;
        //            }

        //    }
        //    //inkPictureNode.Refresh();
        //}

        public void DeleteNodeAndVideo(List<Node> nodeList, List<String> videoList, List<Link> links, List<Link> hyperLinks, int index)
        {
            int i = -1;
            int j = -1;
            int tempHyper = nodeList[index].hyper;
            int deleteIndex = nodeList[index].index;
            nodeList.RemoveAt(index);
            videoList.RemoveAt(index);
            while (-1 == i || -1 == j)
            {
                for (i = 0; i < links.Count; i++)
                {
                    if (deleteIndex == links[i].StartPtIndex || deleteIndex == links[i].EndPtIndex)
                    {
                        links.RemoveAt(i);
                        i = -1;
                        break;
                    }
                }
                for (j = 0; j < hyperLinks.Count; j++)
                {
                    if (deleteIndex == hyperLinks[j].StartPtIndex || deleteIndex == hyperLinks[j].EndPtIndex)
                    {
                        if (0 == tempHyper)//change at time 22.06 in 7.26
                            foreach (Node node in nodeList)
                                if (node.index == hyperLinks[j].StartPtIndex)
                                {
                                    node.hyper -= 1;
                                    if (0 == node.hyper)
                                        node.radius -= 8;
                                }

                        hyperLinks.RemoveAt(j);
                        j = -1;
                        break;
                    }
                }
            }
        }

        public void DeleteLink(List<Node> nodeList, List<Link> links, List<Link> hyperLinks)
        {
            for (int i = 0; i < links.Count; i++)
            {
                for (int j = 0; j < nodeList.Count; j++)
                {
                    if (nodeList[j].index == links[i].StartPtIndex)
                        _startIndex = j;
                    if (nodeList[j].index == links[i].EndPtIndex)
                        _endIndex = j;
                }

                double middleX = (nodeList[_startIndex].point.X + nodeList[_endIndex].point.X) / 2;
                double middleY = (nodeList[_startIndex].point.Y + nodeList[_endIndex].point.Y) / 2;
                double tempValue = Math.Sqrt((PT.X - middleX) * (PT.X - middleX) + (PT.Y - middleY) * (PT.Y - middleY));
                if (tempValue < minValue)
                {
                    minValue = tempValue;
                    signIndex = i;
                    signLink = 1;
                }

            }
            for (int i = 0; i < hyperLinks.Count; i++)
            {
                for (int j = 0; j < nodeList.Count; j++)
                {
                    if (nodeList[j].index == hyperLinks[i].StartPtIndex)
                        _startIndex = j;
                    if (nodeList[j].index == hyperLinks[i].EndPtIndex)
                        _endIndex = j;
                }

                double middleX = (nodeList[_startIndex].point.X + nodeList[_endIndex].point.X) / 2;
                double middleY = (nodeList[_startIndex].point.Y + nodeList[_endIndex].point.Y) / 2;
                double tempValue = Math.Sqrt((PT.X - middleX) * (PT.X - middleX) + (PT.Y - middleY) * (PT.Y - middleY));
                if (tempValue < minValue)
                {
                    temp_StartIndex = _startIndex;
                    temp_EndIndex = _endIndex;
                    minValue = tempValue;
                    signIndex = i;
                    signLink = 2;
                }

            }
            if (signIndex != -1)
            {
                if (1 == signLink)
                    links.RemoveAt(signIndex);
                if (2 == signLink)
                {
                    //nodeList[temp_StartIndex]._sketchInnodevideo.RemoveAt(nodeList[temp_EndIndex].Order);
                    nodeList[temp_StartIndex].hyper -= 1;
                    if (0 == nodeList[temp_StartIndex].hyper)
                        nodeList[temp_StartIndex].radius -= 8;
                    hyperLinks.RemoveAt(signIndex);
                }
            }
        }

        private Microsoft.Ink.InkCollector inkCollector = null;

        //private void PointView_Load(object sender, EventArgs e)
        //{
        //    //inkCollector = new Microsoft.Ink.InkCollector(this.Handle);改动过
        //    inkCollector = new Microsoft.Ink.InkCollector(this.inkPictureNode);
        //    inkCollector.Enabled = true;
        //    inkCollector.CollectionMode = Microsoft.Ink.CollectionMode.InkAndGesture;

        //    inkCollector.Gesture += new Microsoft.Ink.InkCollectorGestureEventHandler(inkCollector_Gesture);
        //    inkCollector.Stroke += new Microsoft.Ink.InkCollectorStrokeEventHandler(inkCollector_Stroke);

        //    inkCollector.SetGestureStatus(Microsoft.Ink.ApplicationGesture.AllGestures, true);
        //}

        void inkCollector_Stroke(object sender, Microsoft.Ink.InkCollectorStrokeEventArgs e)
        {
            // 把未识别为手势的ink笔迹擦除
            inkCollector.Ink.DeleteStrokes();

            this.inkPictureNode.Refresh();//屏蔽了原来类的刷新方法，WPF下识别不出来。
        }

        void inkCollector_Gesture(object sender, Microsoft.Ink.InkCollectorGestureEventArgs e)
        {
            //tect = new Rectangle();
            deleteIndex = -1;
            deleteNodeIndex = -1;
            //Point PT = new Point();
            tempHyper = 0;
            temp_StartIndex = -1;
            temp_EndIndex = -1;

            minValue = Double.MaxValue;//笔迹边框中心到最近的边的中点的距离
            signIndex = -1;//要删除的边的索引号
            signLink = 0;
            _startIndex = -1;//要删除的边的起始点的索引号
            _endIndex = -1;//要删除的边的终点的索引号
            AnnotationNode = 50;

            tect = IMT._InkSpaceToPixelRect(inkPictureNode, inkCollector, inkCollector.Ink.Strokes.GetBoundingBox());
            PT.X = tect.X + tect.Width / 2;
            PT.Y = tect.Y + tect.Height / 2;

            switch (e.Gestures[0].Id.ToString())
            {
                case "Scratchout":
                    {
                        //tect = IMT._InkSpaceToPixelRect(inkPictureNode, inkCollector, inkCollector.Ink.Strokes.GetBoundingBox());
                        //PT.X = tect.X + tect.Width / 2;
                        //PT.Y = tect.Y + tect.Height / 2;
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            if (Math.Sqrt((PT.X - nodeList[i].point.X) * (PT.X - nodeList[i].point.X) + (PT.Y - nodeList[i].point.Y) * (PT.Y - nodeList[i].point.Y)) < 20)
                            {
                                deleteNodeIndex = i;
                                deleteIndex = nodeList[i].index;
                            }
                        }
                        if (-1 != deleteIndex && -1 != deleteNodeIndex)
                        {
                            DeleteNodeAndVideo(nodeList, inkFrame.videoPathList, links, hyperLinks, deleteNodeIndex);
                            WPFInk.cmd.DeleteImageCommand dic = new WPFInk.cmd.DeleteImageCommand(inkFrame.InkCollector, inkFrame.InkCollector.Sketch.Images[deleteNodeIndex]);
                            dic.execute();
                        }
                        else
                        {
                            DeleteLink(nodeList, links, hyperLinks);
                        }
                        inkPictureNode.Refresh();
                        break;

                    }
                case "Circle":
                    {
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            if (Math.Sqrt((PT.X - nodeList[i].point.X) * (PT.X - nodeList[i].point.X) + (PT.Y - nodeList[i].point.Y) * (PT.Y - nodeList[i].point.Y)) < AnnotationNode)
                            {
                                AnnotationNode = Math.Sqrt((PT.X - nodeList[i].point.X) * (PT.X - nodeList[i].point.X) + (PT.Y - nodeList[i].point.Y) * (PT.Y - nodeList[i].point.Y));
                                chooseNode = i;
                                //deleteIndex = nodeList[i].index;
                            }
                        }

                        if (20 == nodeList[chooseNode].radius)
                            nodeList[chooseNode].radius += 8;
                        lineStyle = false;
                        //inkPictureNode.Refresh();

                        SketchInNodeVideo snv = new SketchInNodeVideo();
                        nodeList[chooseNode]._sketchInnodevideo.Add(snv);
                        snv._startIndex = chooseNode;
                        FormAnnotationOld formAnno = new FormAnnotationOld(inkFrame, chooseNode);
                        formAnno.VideoFilePath = inkFrame.videoPathList[chooseNode];
                        formAnno.ShowDialog();
                        //chooseNode = -1;

                        break;
                    }

                case "Left":
                case "Right":
                case "Up":
                case "Down":
                    {
                        if (redLineStartIndex != -1 && redLineEndIndex != -1)
                        {
                            int i = nodeList[redLineStartIndex].hyper;
                            nodeList[redLineEndIndex].Order = i;
                            Link alink = new Link(nodeList[redLineStartIndex].index, nodeList[redLineEndIndex].index, 0);
                            nodeList[redLineStartIndex]._sketchInnodevideo[i]._endIndex = redLineEndIndex;
                            if (20 == nodeList[redLineStartIndex].radius)
                                nodeList[redLineStartIndex].radius += 8;
                            nodeList[redLineStartIndex].hyper += 1;
                            alink.Color = Color.Red;
                            hyperLinks.Add(alink);
                            lineStyle = true;
                            chooseNode = -1;
                            inkPictureNode.Refresh();
                        }
                        break;
                    }


                default:
                    {
                        inkPictureNode.Refresh();
                        break;
                    }
            }
          }

        public void AddNode(List<Node> nodeList, List<Link> links)
        {

            if (1 == inkFrame.ZoomMaxSign)
            {
                int i = nodeList.Count;
                int j = inkFrame.InkCollector.Sketch.Images.Count;
                int k = i;
                if (0 == j)
                    return;
                Node node;
                Link link;

                for (; i < j; i++)
                {
                    node = new Node(i * 80 + 50, 200, "");
                    node.index = i;
                    nodeList.Add(node);
                }
                for (i = k; i < j - 1; i++)
                {
                    link = new Link(i, i + 1, 0);
                    link.Color = Color.Black;
                    links.Add(link);
                }
            }
        }
    }
}
