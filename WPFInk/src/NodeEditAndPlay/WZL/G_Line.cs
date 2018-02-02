using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WPFInk.WZL
{
    public class G_Line : G_Geometry
    {
        public Point startPt;
        public Point endPt;
        public double angle;	//X轴正向之间的夹角，以度为单位
        public double angleback;
        public double k;       //斜率
        public double b;       //y = kx + b中的b
        public bool onp2;
        public bool onp1;
        public bool isverticledge; // 是否是一条垂直线
        public bool isangledge;
        public int p1place, p2place;
        public int otherLine;
        public int m_isCircleEle;
        public int m_isTriangleEle;
        public bool formTriangle;
        public int m_isCenterLine;
        public int m_isOneThirdLine1;
        public int m_isOneThirdLine2;
        public int isElement;
        public int m_dRotateAngle;

        public G_Line()
        {
        }

        public G_Line(Point pt1, Point pt2)
        {
            startPt = pt1;
            endPt = pt2;
            onp1 = false;
            onp2 = false;
            isverticledge = false;
            isangledge = false;
            setKBData();
            otherLine = -1;
            formTriangle = false;
            p1place = 0;
            p2place = 0;
            isElement = -1;
            m_isTriangleEle = -1;
            m_isCenterLine = -1;
            m_isOneThirdLine1 = -1;
            m_isOneThirdLine2 = -1;
            m_isCircleEle = -1;
            m_dRotateAngle = 0;
        }

        public void setKBData()
        {
            angleback = angle;
            angle = new ToolForStroke().angleOfPoint(startPt, endPt, 10.0);

            Point tempPt = new Point();
            tempPt.X = endPt.X - startPt.X;
            tempPt.Y = endPt.Y - startPt.Y;

            if (System.Math.Abs(angle) < LINELIMIT)
            {
                b = endPt.Y;
                k = 0.0;
                angle = 0.0;
            }
            else if (90 - System.Math.Abs(angle) < LINELIMIT)
            {
                b = 0.0;
                if (startPt.X - endPt.X < 1)
                    k = 1e+5;  //infinery
                else
                    k = (startPt.Y - endPt.Y) / (startPt.X - endPt.X);
                angle = 90.0;
            }
            else
            {
                k = System.Math.Tan(new ToolForStroke().degreeToRadian(angle));
                b = endPt.Y - k * startPt.X;
            }
        }

        public double LINELIMIT = 0.1;
    }
}
