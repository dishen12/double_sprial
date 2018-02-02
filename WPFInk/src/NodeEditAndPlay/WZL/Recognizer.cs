using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace WPFInk.WZL
{
    public class Recognizer
    {
        //YHY-090421
        public void bottomUpRecognize(SemanticGroup smsg)
        {
            //bool flagContain = false;

            //get geo compositon of all templates
            GetGeoInfoOfTemplateNodeList();

            foreach (StrokeGroup sg in smsg.groupList)
            {
                Graphic geoType = sg.GRAPH;
                switch (geoType)
                {
                    case Graphic.Line:
                        smsg.nCountLine++;
                        break;
                    case Graphic.Circle:
                        smsg.nCountCircle++;
                        break;
                    case Graphic.Curve:
                        smsg.nCountCurve++;
                        break;
                    default:
                        break;
                }
            }

            List<GeoInfoOfTemplateNode> copyOfgeoOfTemplateNodeList = new List<GeoInfoOfTemplateNode>();
            foreach (GeoInfoOfTemplateNode DomNode in geoOfTemplateNodeList)
            {
                GeoInfoOfTemplateNode copyofDomainNode = new GeoInfoOfTemplateNode();
                copyofDomainNode.nCountCircle = DomNode.nCountCircle;
                copyofDomainNode.nCountLine = DomNode.nCountLine;
                copyofDomainNode.nCountCurve = DomNode.nCountCurve;
                copyofDomainNode.templateName = DomNode.templateName;
                foreach (string sss in DomNode.geoComposition)
                {
                    copyofDomainNode.geoComposition.Add(sss);
                }
                copyOfgeoOfTemplateNodeList.Add(copyofDomainNode);
            }

            geoOfTemplateNodeList.Clear();

            foreach (GeoInfoOfTemplateNode domainNode in copyOfgeoOfTemplateNodeList)
            {
                if (domainNode.nCountLine == smsg.nCountLine && domainNode.nCountCircle == smsg.nCountCircle)
                {
                    geoOfTemplateNodeList.Add(domainNode);
                }

            }

            /*
            foreach (StrokeGroup sg in smsg.groupList)
            {
                Graphic geoType = sg.GRAPH;
                switch (geoType)
                {
                    case Graphic.Line:
                        flagContain = false;

                        foreach (GeoInfoOfTemplateNode domainNode in geoOfTemplateNodeList)
                        {
                            GeoInfoOfTemplateNode domainNode1 = new GeoInfoOfTemplateNode();
                            domainNode1.templateName = domainNode.templateName;
                            foreach (string ss in domainNode.geoComposition)
                            {
                                domainNode1.geoComposition.Add(ss);
                            }

                            foreach (string s in domainNode.geoComposition)
                            {
                                if (s == "line")
                                {
                                    flagContain = true;
                                    domainNode.geoComposition.Remove(s);
                                }
                            }
                            if (flagContain == false)
                                geoOfTemplateNodeList.Remove(domainNode);
                        }
                        
                        break;
                    case Graphic.Circle:
                        flagContain = false;

                        foreach (GeoInfoOfTemplateNode domainNode in geoOfTemplateNodeList)
                        {
                            GeoInfoOfTemplateNode domainNode1 = new GeoInfoOfTemplateNode();
                            domainNode1.templateName = domainNode.templateName;
                            foreach (string ss in domainNode.geoComposition)
                            {
                                domainNode1.geoComposition.Add(ss);
                            }
                            
                            foreach (string s in domainNode1.geoComposition)
                            {
                                if (s == "circle")
                                {
                                    flagContain = true;
                                    domainNode.geoComposition.Remove(s);
                                }
                            }
                            if (flagContain == false)
                                geoOfTemplateNodeList.Remove(domainNode);
                        }
                        break;
                    case Graphic.Curve:
                        flagContain = false;

                        foreach (GeoInfoOfTemplateNode domainNode in geoOfTemplateNodeList)
                        {
                            GeoInfoOfTemplateNode domainNode1 = new GeoInfoOfTemplateNode();
                            domainNode1.templateName = domainNode.templateName;
                            foreach (string ss in domainNode.geoComposition)
                            {
                                domainNode1.geoComposition.Add(ss);
                            }

                            foreach (string s in domainNode.geoComposition)
                            {
                                if (s == "curve")
                                {
                                    flagContain = true;
                                    domainNode.geoComposition.Remove(s);
                                }
                            }
                            if (flagContain == false)
                                geoOfTemplateNodeList.Remove(domainNode);
                        }
                        break;
                    default:
                        break;
                }
            }*/
        }

        public XmlDocument loadTemplateXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            String filePath = "E:\\草图通用编辑器\\DomainTemplate.xml";
            xmlDoc.Load(filePath);

            //XmlNode root = xmlDoc.SelectSingleNode("Template");

            return xmlDoc;
        }

        public void GetGeoInfoOfTemplateNodeList()
        {
            //XmlDocument xmlDoc = new XmlDocument();
            ////String filePath = "E:\\DomainTemplate.xml";
            ////xmlDoc.Load(filePath);

            //XmlNodeList nodeList = xmlDoc.SelectSingleNode("Template").ChildNodes;//获取Template的所有子节点

            //foreach (XmlNode xn in nodeList)
            //{
            //    GeoInfoOfTemplateNode geoNode = new GeoInfoOfTemplateNode();

            //    XmlElement xe = (XmlElement)xn; //DomainObject tag
            //    geoNode.templateName = xe.GetAttribute("name");

            //    XmlNodeList xelist = xe.ChildNodes;
            //    xe = (XmlElement)xelist.Item(0);//GeoComposition

            //    xelist = xe.ChildNodes;
            //    foreach (XmlNode xnLeaf in xelist)  //GeoObject
            //    {
            //        XmlElement xeleaf = (XmlElement)xnLeaf;  
            //        string stype = xeleaf.GetAttribute("type");
            //        geoNode.geoComposition.Add(stype);

            //        //方式2
            //        switch (stype)
            //        {
            //            case "line":
            //                geoNode.nCountLine++;
            //                break;
            //            case "circle":
            //                geoNode.nCountCircle++;
            //                break;
            //            case "curve":
            //                geoNode.nCountCurve++;
            //                break;
            //            default:
            //                break;
            //        }
            //    }

            //    geoOfTemplateNodeList.Add(geoNode);

            //}

        }

        public List<GeoInfoOfTemplateNode> geoOfTemplateNodeList = new List<GeoInfoOfTemplateNode>();

    }

    public class GeoInfoOfTemplateNode
    {
        public string templateName;
        //方式1
        public List<string> geoComposition = new List<string>();

        //方式2
        public int nCountCircle = 0;
        public int nCountLine = 0;
        public int nCountCurve = 0;
    }
}
