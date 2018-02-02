using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Xml;
using System.IO;
using WPFInk.ink;
using WPFInk.cmd;
using WPFInk.tool;
using Microsoft.Win32;
using WPFInk.video;
using WPFInk.graphic;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WPFInk.PersistenceManager
{
    /// <summary>
    /// PersistenceManager类负责存取程序中产生的笔迹文件
    /// 将笔迹文件存储到服务器上，同时可以将服务器上的笔迹文件下载到客户端
    /// </summary>
    public class PersistenceManager
    {
        private static PersistenceManager single = null;
        private List<MyImage> SketchImages = new List<MyImage>();
        private List<int> ConnectorCounts = new List<int>();                                  // ConnectorCount
        private VideoOperation videoOperation = null;
        private PersistenceManager()
        {

        }
		

        public static PersistenceManager getInstance()
        {
            if (single == null)
                single = new PersistenceManager();
            return single;
        }

        public Sketch GetSketchByName(String FileName)
        {
            return null;
        }

        public void setVideoOperation(VideoOperation v0)
        {
            this.videoOperation = v0;
        }


        //将草图存入到流中
        public void SaveSketchToStream(InkCollector _inkCollector, Stream stream)
        {
            try
            {
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(stream))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    XmlWriter writer = XmlWriter.Create(streamWriter, settings);
                    writer.WriteStartElement("StrokeCollection");
                    Sketch sketch = new Sketch();
                    sketch = _inkCollector.Sketch;
                    foreach (MyStroke mystroke in (sketch.MyStrokes))
                    {
                        if (!mystroke.IsSketchConnector&&mystroke.IsExist==false)
                        {
                            WriteElementProperty(mystroke, writer);
                        }
                    }
                    foreach (MyImage myImage in _inkCollector.Sketch.Images)
                    {
						if (myImage.IsExist == false)
						{
							WriteMyimageProperty(myImage, writer);
						}
                    }
					foreach (MyRichTextBox myRichTextBox in _inkCollector.Sketch.MyRichTextBoxs)
                    {
						TextRange textRange = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);

						if (textRange.Text.Length != 0 && myRichTextBox.IsExist == false)
                        {
							WriteTextProperty(myRichTextBox, writer);
                        }
                    }
                    foreach (MyButton myButton in _inkCollector.Sketch.MyButtons)
                    {
						if (!myButton.IsDeleted)
						{
							WriteButtonProperty(myButton, writer, _inkCollector);
						}
						else
						{
							myButton.Dispose();
						}
                    }
                    //保存图形
                    foreach (MyGraphic mg in _inkCollector.Sketch.MyGraphics)
                    {
                        WriteMyGraphicProperty(mg, writer, _inkCollector);
                    }

                    //保存图形关联
                    foreach (GraphicLinkNode gln in _inkCollector.Sketch.GraphicLinkNodes)
                    {
                        WriteGraphicLindNodeProperty(gln, writer, _inkCollector);
                    }
                    writer.WriteEndElement();
                    writer.Flush();
                    writer.Close();
                    streamWriter.Close();
                }

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 向writer中写入stroke数据
        /// </summary>
        /// <param name="stroke"></param>
        /// <param name="writer"></param>
        private void WriteElementProperty(MyStroke stroke, XmlWriter writer)
        {
            writer.WriteStartElement("Stroke");
            //writer.WriteAttributeString("Color", stroke.DrawingAttributes.Color.ToString());
            writer.WriteAttributeString("Height", stroke.DrawingAttributes.Height.ToString());
            writer.WriteAttributeString("Width", stroke.DrawingAttributes.Width.ToString());
            //write color attributes
            Color c = stroke.DrawingAttributes.Color;
            String s = c.A + "," + c.R + "," + c.G + "," + c.B;
            writer.WriteAttributeString("Color", s);
            writer.WriteAttributeString("StartTime", stroke.StartTime.ToString());
            writer.WriteAttributeString("EndTime", stroke.EndTime.ToString());

            string points = "";
            foreach (StylusPoint sp in stroke.StylusPoints)
            {
                if (points != "")
                    points += ";";

                points += sp.X.ToString() + "," + sp.Y.ToString();
            }
            writer.WriteAttributeString("Points", points);
			writer.WriteAttributeString("VideoPath", stroke.VideoPath);
            writer.WriteEndElement();
        }

        /// <summary>
        /// 写入image和text信息
        /// </summary>
        /// <param name="image"></param>
        /// <param name="writer"></param>
		private void WriteTextProperty(MyRichTextBox myRichTextBox, XmlWriter writer)
        {
            writer.WriteStartElement("Text");
            string margin = "";
			writer.WriteAttributeString("Height", myRichTextBox.RichTextBox.Height.ToString());
			writer.WriteAttributeString("Width", myRichTextBox.RichTextBox.Width.ToString());
			TextRange textRange = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);
			writer.WriteAttributeString("Content", textRange.Text);
			margin = myRichTextBox.RichTextBox.Margin.Left.ToString() + "," + myRichTextBox.RichTextBox.Margin.Top.ToString() + "," +
							myRichTextBox.RichTextBox.Margin.Right.ToString() + "," + myRichTextBox.RichTextBox.Margin.Bottom.ToString();
			writer.WriteAttributeString("Margin", margin);
			writer.WriteAttributeString("VideoPath", myRichTextBox.VideoPath);

			TextRange tr = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);
			writer.WriteAttributeString("FontFamily", tr.GetPropertyValue(RichTextBox.FontFamilyProperty).ToString());
			writer.WriteAttributeString("FontSize", tr.GetPropertyValue(RichTextBox.FontSizeProperty).ToString());
			writer.WriteAttributeString("FontBold", tr.GetPropertyValue(RichTextBox.FontWeightProperty).ToString());
			writer.WriteAttributeString("FontItalic", tr.GetPropertyValue(RichTextBox.FontStyleProperty).ToString());
			writer.WriteAttributeString("FontColor", ((System.Windows.Media.SolidColorBrush)tr.GetPropertyValue(RichTextBox.ForegroundProperty)).Color.R.ToString() + "," + ((System.Windows.Media.SolidColorBrush)tr.GetPropertyValue(RichTextBox.ForegroundProperty)).Color.G.ToString() + "," + ((System.Windows.Media.SolidColorBrush)tr.GetPropertyValue(RichTextBox.ForegroundProperty)).Color.B.ToString());
            writer.WriteEndElement();


        }

        private void WriteMyimageProperty(MyImage myImage, XmlWriter writer)
        {
            writer.WriteStartElement("Image");
            string margin = "";
            writer.WriteAttributeString("Height", myImage.Height.ToString());
            writer.WriteAttributeString("Width", myImage.Width.ToString());
            writer.WriteAttributeString("Source", myImage.Image.Source.ToString());
            margin = myImage.Image.Margin.Left.ToString() + "," + myImage.Image.Margin.Top.ToString() + "," +
                           myImage.Image.Margin.Right.ToString() + "," + myImage.Image.Margin.Bottom.ToString();
            writer.WriteAttributeString("Margin", margin);
			writer.WriteAttributeString("SafeFileName", myImage.SafeFileName);
            writer.WriteAttributeString("connnectorCollectionCount", myImage.ConnectorCollection.Count.ToString());
            if (myImage.ConnectorCollection.Count > 0)
            {
                string aa = myImage.ConnectorCollection[0].Source.PathName;
                writer.WriteAttributeString("connnector0Source", myImage.ConnectorCollection[0].Source.PathName);
                writer.WriteAttributeString("connnector0Target", myImage.ConnectorCollection[0].Target.PathName);
                if (myImage.ConnectorCollection.Count == 2)
                {
                    writer.WriteAttributeString("connnector1Source", myImage.ConnectorCollection[1].Source.PathName);
                    writer.WriteAttributeString("connnector1Target", myImage.ConnectorCollection[1].Target.PathName);
                }
            }
			writer.WriteAttributeString("angle", myImage.Angle.ToString());
			writer.WriteAttributeString("VideoPath", myImage.VideoPath);
            writer.WriteEndElement();

        }

        /// <summary>
        /// 保存Mygraphic
        /// </summary>
        /// <param name="myGraphic"></param>
        /// <param name="writer"></param>
        /// <param name="_inkCollector"></param>
        private void WriteMyGraphicProperty(MyGraphic myGraphic, XmlWriter writer, InkCollector _inkCollector)
        {
            writer.WriteStartElement("MyGraphic");

            writer.WriteAttributeString("MyGraphicId", myGraphic.MyGraphicID.ToString());
            string margin = myGraphic.Shape.Margin.Left.ToString() + "," + myGraphic.Shape.Margin.Top.ToString() + "," +
                           myGraphic.Shape.Margin.Right.ToString() + "," + myGraphic.Shape.Margin.Bottom.ToString();
            writer.WriteAttributeString("Margin", margin);
            writer.WriteAttributeString("StrokeCount", myGraphic.Strokes.Count.ToString());
            int j = 0;
            foreach (Stroke stroke in myGraphic.Strokes)
            {
                writer.WriteAttributeString("Stroke"+j.ToString()+"Height", stroke.DrawingAttributes.Height.ToString());
                writer.WriteAttributeString("Stroke" + j.ToString() + "Width", stroke.DrawingAttributes.Width.ToString());
                Color c = stroke.DrawingAttributes.Color;
                String s = c.A + "," + c.R + "," + c.G + "," + c.B;
                writer.WriteAttributeString("Stroke" + j.ToString() + "Color", s);
                string points = "";
                foreach (StylusPoint sp in stroke.StylusPoints)
                {
                    if (points != "")
                        points += ";";

                    points += sp.X.ToString() + "," + sp.Y.ToString();
                }
                writer.WriteAttributeString("Stroke" + j.ToString() + "Points", points);
                j++;
            }
            writer.WriteAttributeString("textStrokeCount", myGraphic.textStrokeCollection.Count.ToString());
            //保存图形笔迹
            int i = 0;
            foreach (Stroke stroke in myGraphic.textStrokeCollection)
            {
                writer.WriteAttributeString("textStroke" + i.ToString() + "Height", stroke.DrawingAttributes.Height.ToString());
                writer.WriteAttributeString("textStroke" + i.ToString() + "Width", stroke.DrawingAttributes.Width.ToString());
                Color color = stroke.DrawingAttributes.Color;
                String sc = color.A + "," + color.R + "," + color.G + "," + color.B;
                writer.WriteAttributeString("textStroke" + i.ToString() + "Color", sc);
                string ps = "";
                foreach (StylusPoint sp in stroke.StylusPoints)
                {
                    if (ps != "")
                        ps += ";";

                    ps += sp.X.ToString() + "," + sp.Y.ToString();
                }
                writer.WriteAttributeString("textStroke" + i.ToString() + "Points", ps);
                i++;
            }
            writer.WriteAttributeString("Text", myGraphic.Text);
            writer.WriteAttributeString("GraphicLinkNodeID", myGraphic.GraphicLinkNodeID.ToString());
            writer.WriteAttributeString("LastGraphicLinkNodeID", myGraphic.LastGraphicLinkNodeID.ToString());


            writer.WriteEndElement();
        }
        /// <summary>
        /// 保存图形之间的关系
        /// </summary>
        /// <param name="gln"></param>
        /// <param name="writer"></param>
        /// <param name="_inkCollector"></param>
        private void WriteGraphicLindNodeProperty(GraphicLinkNode gln, XmlWriter writer, InkCollector _inkCollector)
        {
            writer.WriteStartElement("GraphicLinkNode");

            writer.WriteAttributeString("GraphicLinkNodeId", gln.GraphicLinkNodeID.ToString());
            writer.WriteAttributeString("PreMyGraphicId", gln.SelfMyGraphicID.ToString());
            writer.WriteAttributeString("NextMyGraphicId", gln.MyGraphicID.ToString());
            writer.WriteAttributeString("NextGraphicLinkNodeID", gln.NextGraphicLinkNodeID.ToString());
            writer.WriteAttributeString("Relation", gln.Rule.ToString());

            writer.WriteEndElement();
        }

        /// <summary>
        /// 保存MyButton
        /// </summary>
        /// <param name="myButton"></param>
        /// <param name="writer"></param>
        /// <param name="_inkCollector"></param>
        private void WriteButtonProperty(MyButton myButton, XmlWriter writer,InkCollector _inkCollector)
        {
            writer.WriteStartElement("Button");
            string margin = "";

            writer.WriteAttributeString("VideoPath", myButton.VideoPath);
            string background = myButton.Button.Background.ToString();
            string background2= ((System.Windows.Media.SolidColorBrush)myButton.Button.Background).Color.R.ToString()+","+ ((System.Windows.Media.SolidColorBrush)myButton.Button.Background).Color.G.ToString()+","+ ((System.Windows.Media.SolidColorBrush)myButton.Button.Background).Color.B.ToString();
            writer.WriteAttributeString("Background", background2);
            writer.WriteAttributeString("Height", myButton.Height.ToString());
            writer.WriteAttributeString("Width", myButton.Width.ToString());
            margin = myButton.Button.Margin.Left.ToString() + "," + myButton.Button.Margin.Top.ToString() + "," +
                           myButton.Button.Margin.Right.ToString() + "," + myButton.Button.Margin.Bottom.ToString();
            writer.WriteAttributeString("Margin", margin);

            writer.WriteAttributeString("angle", myButton.Angle.ToString());
            writer.WriteAttributeString("zoomRate", myButton.ZoomRate.ToString());
            writer.WriteAttributeString("timeStart", myButton.TimeStart.ToString());
            writer.WriteAttributeString("timeEnd", myButton.TimeEnd.ToString());
            writer.WriteAttributeString("Id",myButton.Id.ToString());
			writer.WriteAttributeString("isGlobal", myButton.IsGlobal.ToString());
			writer.WriteAttributeString("InkFrameWidth", myButton.InkFrameWidth.ToString());
			writer.WriteAttributeString("InkFrameHeight", myButton.InkFrameHeight.ToString());
            writer.WriteAttributeString("VideoFileName", myButton.VideoFileName.ToString());
            writer.WriteAttributeString("AnalyzeResults", myButton.AnalyzeResults);
            writer.WriteAttributeString("ContentMoveX", myButton.ContentMoveX.ToString());
            writer.WriteAttributeString("ContentMoveY", myButton.ContentMoveY.ToString());
            writer.WriteAttributeString("ContentScaling", myButton.ContentScaling.ToString());
            foreach (MyArrow myArrow in _inkCollector.Sketch.MyArrows)
            {
                if (myArrow.IsDeleted == false && myArrow.NextMyButton.IsDeleted == false && myArrow.PreMyButton == myButton )
                {
                    writer.WriteAttributeString("nextId", myArrow.NextMyButton.Id.ToString());
                }
            }
            writer.WriteAttributeString("StrokeCount", myButton.InkFrame._inkCanvas.Strokes.Count.ToString());

			//保存笔迹
            int i=0;
            foreach (Stroke stroke in myButton.InkFrame._inkCanvas.Strokes)
            {
                writer.WriteAttributeString("Stroke" + i.ToString() + "Height", stroke.DrawingAttributes.Height.ToString());
                writer.WriteAttributeString("Stroke" + i.ToString() + "Width", stroke.DrawingAttributes.Width.ToString());
                Color c = stroke.DrawingAttributes.Color;
                String s = c.A + "," + c.R + "," + c.G + "," + c.B;
                writer.WriteAttributeString("Stroke" + i.ToString() + "Color", s);
                string points = "";
                foreach (StylusPoint sp in stroke.StylusPoints)
                {
                    if (points != "")
                        points += ";";

                    points += sp.X.ToString() + "," + sp.Y.ToString();
                }
                writer.WriteAttributeString("Stroke" + i.ToString() + "Points", points);
                i++;
            }

			//保存图片
			writer.WriteAttributeString("ImageCount", myButton.InkFrame.InkCollector.Sketch.Images.Count.ToString());
			i = 0;
			foreach (MyImage myImage in myButton.InkFrame.InkCollector.Sketch.Images)
			{
				writer.WriteAttributeString("MyImage" + i.ToString() + "Height", myImage.Height.ToString());
				writer.WriteAttributeString("MyImage" + i.ToString() + "Width", myImage.Width.ToString());
				writer.WriteAttributeString("MyImage" + i.ToString() + "Source", myImage.Image.Source.ToString());
				margin = myImage.Image.Margin.Left.ToString() + "," + myImage.Image.Margin.Top.ToString() + "," +
							   myImage.Image.Margin.Right.ToString() + "," + myImage.Image.Margin.Bottom.ToString();
				writer.WriteAttributeString("MyImage" + i.ToString() + "Margin", margin);
				writer.WriteAttributeString("MyImage" + i.ToString() + "SafeFileName", myImage.SafeFileName);

				writer.WriteAttributeString("MyImage" + i.ToString() + "angle", myImage.Angle.ToString());
				writer.WriteAttributeString("MyImage" + i.ToString() + "VideoPath", myButton.VideoPath);
				writer.WriteAttributeString("MyImage" + i.ToString() + "connnectorCollectionCount", myImage.ConnectorCollection.Count.ToString());
				i++;
			}

			//保存text
			writer.WriteAttributeString("TextCount", myButton.InkFrame.InkCollector.Sketch.MyRichTextBoxs.Count.ToString());
			i = 0;
			foreach (MyRichTextBox myRichTextBox in myButton.InkFrame.InkCollector.Sketch.MyRichTextBoxs)
			{

				TextRange textRange = new TextRange(myRichTextBox.RichTextBox.Document.ContentStart, myRichTextBox.RichTextBox.Document.ContentEnd);
				if (textRange.Text.Length != 0)
				{
					margin = "";
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "Height", myRichTextBox.RichTextBox.Height.ToString());
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "Width", myRichTextBox.RichTextBox.Width.ToString());
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "Content", textRange.Text);
					margin = myRichTextBox.RichTextBox.Margin.Left.ToString() + "," + myRichTextBox.RichTextBox.Margin.Top.ToString() + "," +
									myRichTextBox.RichTextBox.Margin.Right.ToString() + "," + myRichTextBox.RichTextBox.Margin.Bottom.ToString();
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "Margin", margin);
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "VideoPath", myButton.VideoPath);


					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "FontFamily", textRange.GetPropertyValue(RichTextBox.FontFamilyProperty).ToString());
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "FontSize", textRange.GetPropertyValue(RichTextBox.FontSizeProperty).ToString());
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "FontBold", textRange.GetPropertyValue(RichTextBox.FontWeightProperty).ToString());
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "FontItalic", textRange.GetPropertyValue(RichTextBox.FontStyleProperty).ToString());
					writer.WriteAttributeString("MyRichTextBox" + i.ToString() + "FontColor", ((System.Windows.Media.SolidColorBrush)textRange.GetPropertyValue(RichTextBox.ForegroundProperty)).Color.R.ToString() + "," + ((System.Windows.Media.SolidColorBrush)textRange.GetPropertyValue(RichTextBox.ForegroundProperty)).Color.G.ToString() + "," + ((System.Windows.Media.SolidColorBrush)textRange.GetPropertyValue(RichTextBox.ForegroundProperty)).Color.B.ToString());
					i++;
				}
				
			}


            writer.WriteEndElement();

        }



        /// <summary>
        /// 从文件流stream中load笔迹文件
        /// </summary>
        /// <param name="inkCollector"></param>
        /// <param name="stream"></param>
        public void LoadFromStream(InkCollector inkCollector, Stream stream)
        {
            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                XmlReader Reader = XmlReader.Create(streamReader, settings);
                int i = 0;
                while (Reader.Read())
                {
                    if (Reader.NodeType == XmlNodeType.Element)
                    {
                        if (Reader.Name == "StrokeCollection")
                            continue;
                        if (Reader.Name == "Stroke")
                        {
                            ReadElementProperty(inkCollector, Reader);
                        }
                        else if (Reader.Name == "Image")
                        {
                            ReadImageProperty(inkCollector, Reader);
                            CreateSketch(inkCollector, SketchImages, ConnectorCounts);
                        }
                        else if (Reader.Name == "Text")
                            ReadTextProperty(inkCollector, Reader);
                        else if (Reader.Name == "Button")
                        {
                            ReadButtonProperty(inkCollector, Reader,i);
                            i++;
                        }
                        else if (Reader.Name == "MyGraphic")
                        {
                            ReadMyGraphicProperty(inkCollector, Reader);
                        }
                    }
                }
                //以后要加上
                    //inkCollector._mainPage.pointView.pointView.AddNode(inkCollector._mainPage.pointView.pointView.nodeList, inkCollector._mainPage.pointView.pointView.links);
                

                //添加箭头
                if (videoOperation != null)
                {
                    foreach (MyButton myButton in inkCollector.Sketch.MyButtons)
                    {
                        foreach (MyButton myButtonOther in inkCollector.Sketch.MyButtons)
                        {
                            if (myButton.NextId == myButtonOther.Id && myButton != myButtonOther)
                            {
                                ThumbConnector thumbConnector = new ThumbConnector(myButton, myButtonOther);
                                MyArrow ma = new MyArrow(thumbConnector.arrow);
                                ma.PreMyButton = myButton;
                                ma.NextMyButton = myButtonOther;
                                ma.StartPoint = thumbConnector.startPoint;
                                ma.EndPoint = thumbConnector.endPoint;
                                Command aac = new AddArrowCommand(inkCollector, ma);
                                aac.execute();
                                inkCollector.CommandStack.Push(aac);
                            }
                        }
                    }

                    //添加总的ink（右上角的ink区域内容）
                    foreach (MyButton myButton in GlobalMyButtons)
                    {
                        StrokeCollection strokes = myButton.InkFrame._inkCanvas.Strokes.Clone();
                        foreach (Stroke stroke in strokes)
                        {
                            MyStroke myStroke = new MyStroke(stroke);
                            myStroke.VideoPath = myButton.VideoPath;
                            Command asc = new AddStrokeCommand(videoOperation._titleInk.Title_InkFrame.InkCollector, myStroke);
                            asc.execute();
                            videoOperation._titleInk.Title_InkFrame.InkCollector.CommandStack.Push(asc);
                            Command mms = new MoveCommand(myStroke, myButton.ContentMoveX, myButton.ContentMoveY);
                            mms.execute();
                        }
                    }
                }
                Reader.Close();
                streamReader.Close();
            }
        }

        /// <summary>
        /// 响应笔迹读取命令
        /// </summary>
        /// <param name="reader"></param>
        void ReadElementProperty(InkCollector inkCollector, XmlReader reader)
        {

            reader.MoveToAttribute("Height");
            double Height = Convert.ToDouble(reader.Value);
            reader.MoveToAttribute("Width");
            double Width = Convert.ToDouble(reader.Value);
            reader.MoveToAttribute("Color");
            String scolor = reader.Value;
            String[] argb = scolor.Split(',');
            Color Color = Color.FromArgb(Convert.ToByte(argb[0]), Convert.ToByte(argb[1]),
                Convert.ToByte(argb[2]), Convert.ToByte(argb[3]));
            //Color Color = Color.FromArgb(255,0,0,0);
            reader.MoveToAttribute("Points");
            string pointstring = reader.Value;
            StylusPointCollection points = new StylusPointCollection();
            String[] P = pointstring.Split(';');
            foreach (String point in P)
            {
                String[] xy = point.Split(',');
                points.Add(new StylusPoint(Convert.ToDouble(xy[0]), Convert.ToDouble(xy[1])));
            }
            Stroke stroke = new Stroke(points);
            stroke.DrawingAttributes.Height = Height;
            stroke.DrawingAttributes.Width = Width;
            stroke.DrawingAttributes.Color = Color;
            inkCollector.InkCanvas.Strokes.Add(stroke);
            WPFInk.ink.MyStroke mystroke = new WPFInk.ink.MyStroke(stroke);
            reader.MoveToAttribute("StartTime");
            mystroke.StartTime = Convert.ToInt32(reader.Value);
            reader.MoveToAttribute("EndTime");
            mystroke.EndTime = Convert.ToInt32(reader.Value);

            AddStrokeCommand cmd = new AddStrokeCommand(inkCollector, mystroke);
            inkCollector.CommandStack.Push(cmd);
            cmd.execute();
            

            MyTimer.getInstance().setTime(mystroke.EndTime);
        }

        /// <summary>
        /// 从流中读取图片
        /// </summary>
        /// <param name="inkCollector"></param>
        /// <param name="reader"></param>
        void ReadImageProperty(InkCollector inkCollector, XmlReader reader)
        {

            System.Windows.Controls.Image image = new Image();

            reader.MoveToAttribute("Height");
            image.Height = Convert.ToDouble(reader.Value);

            reader.MoveToAttribute("Width");
            image.Width = Convert.ToDouble(reader.Value);

            reader.MoveToAttribute("Margin");
            string[] MarginString = Regex.Split(reader.Value, ",", RegexOptions.IgnoreCase);

            image.Margin = new Thickness(Convert.ToDouble(MarginString[0]), Convert.ToDouble(MarginString[1]), Convert.ToDouble(MarginString[2]), Convert.ToDouble(MarginString[3]));



            BitmapImage bitimage = new BitmapImage();
            reader.MoveToAttribute("Source");
            bitimage.BeginInit();
            bitimage.UriSource = new Uri(reader.Value.ToString());
            bitimage.EndInit();
            image.Source = bitimage;

            MyImage myImage = new MyImage(image);
            myImage.Left = image.Margin.Left;
            myImage.Top = image.Margin.Top;
			reader.MoveToAttribute("SafeFileName");
			string safeFileName = reader.Value;
			myImage.SafeFileName = safeFileName;

            SketchImages.Add(myImage);

            reader.MoveToAttribute("connnectorCollectionCount");
            string ConnectorCount = reader.Value.ToString();
            ConnectorCounts.Add(int.Parse(ConnectorCount));
            InkConstants.AddBound(myImage);

            //旋转角度
            reader.MoveToAttribute("angle");
            myImage.Angle = double.Parse(reader.Value.ToString());
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.RotateAt(myImage.Angle, myImage.Width / 2, myImage.Height / 2);
            myImage.Image.RenderTransform = new MatrixTransform(m);
            myImage.Bound.RenderTransform = new MatrixTransform(m);


            AddImageCommand cmd = new AddImageCommand(inkCollector, myImage);
            inkCollector.CommandStack.Push(cmd);
            cmd.execute();

        }

        //重新生成草图连线
        private void CreateSketch(InkCollector inkCollector, List<MyImage> myImages, List<int> ConnectorCounts)
        {
            int j;
            ImageConnector connector = null;
            for (int i = 0; i < myImages.Count - 1; i++)
            {

                if (ConnectorCounts[i] > 0)
                {
                    j = i + 1;
                    connector = new ImageConnector(myImages[i], myImages[j]);
                    MyStroke myStroke = connector.MYSTROKE;
                    myStroke.Stroke.DrawingAttributes.Width = 3;
                    myStroke.Stroke.DrawingAttributes.Height = 3;
                    myStroke.IsSketchConnector = true;
                    Command sc = new AddStrokeCommand(inkCollector, myStroke);
                    sc.execute();
                }
            }
        }


        /// <summary>
        /// 获取文字
        /// </summary>
        /// <param name="inkCollector"></param>
        /// <param name="reader"></param>
        void ReadTextProperty(InkCollector inkCollector, XmlReader reader)
        {
            reader.MoveToAttribute("Margin");
            String[] margin = reader.Value.Split(',');
            Thickness location = new Thickness(Convert.ToDouble(margin[0]),
                Convert.ToDouble(margin[1]),
                Convert.ToDouble(margin[2]),
                Convert.ToDouble(margin[3]));

            reader.MoveToAttribute("Content");
            string content = reader.Value;

            reader.MoveToAttribute("Height");
            double height = Convert.ToDouble(reader.Value);

            reader.MoveToAttribute("Width");
            double width = Convert.ToDouble(reader.Value);

			RichTextBox tb = new RichTextBox();
			tb.HorizontalAlignment = HorizontalAlignment.Left;
			tb.VerticalAlignment = VerticalAlignment.Top;
            tb.Margin = location;
            tb.Width = width;
            tb.Height = height;
			tb.Padding = new Thickness(0);
			System.Windows.Documents.Paragraph paragraph = new Paragraph();
			paragraph.LineHeight = 1;
			paragraph.Padding = new Thickness(0);
			paragraph.TextAlignment = TextAlignment.Left;
			Run run = new Run();
			run.Text = content;
			paragraph.Inlines.Add(run);
			tb.Document.Blocks.Clear();
			tb.Document.Blocks.Add(paragraph);
            tb.IsHitTestVisible = false;
            tb.BorderBrush = null;
			tb.AcceptsReturn = true;
			tb.Background = new SolidColorBrush(Colors.Transparent);
			//tb.Background = new SolidColorBrush(Colors.Yellow);
			TextRange textRange = new TextRange(tb.Document.ContentStart, tb.Document.ContentEnd);
			reader.MoveToAttribute("FontFamily");
			textRange.ApplyPropertyValue(RichTextBox.FontFamilyProperty, reader.Value);
			reader.MoveToAttribute("FontSize");
			textRange.ApplyPropertyValue(RichTextBox.FontSizeProperty, reader.Value);

			reader.MoveToAttribute("FontBold");
			textRange.ApplyPropertyValue(RichTextBox.FontWeightProperty, reader.Value);

			reader.MoveToAttribute("FontItalic");
			textRange.ApplyPropertyValue(RichTextBox.FontStyleProperty, reader.Value);

			reader.MoveToAttribute("FontColor");
			string foreground = reader.Value;
			String[] rgb = foreground.Split(',');
			SolidColorBrush myBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(rgb[0]), Convert.ToByte(rgb[1]), Convert.ToByte(rgb[2])));
			textRange.ApplyPropertyValue(RichTextBox.ForegroundProperty, myBrush);
			
			
            inkCollector.InkCanvas.Children.Add(tb);
			MyRichTextBox mt = new MyRichTextBox(tb);
            inkCollector.Sketch.AddText(mt);
			//inkCollector.Mode = InkMode.None;
			//tb.Focus();
        }

        /// <summary>
        /// 读取图形
        /// </summary>
        /// <param name="inkCollector"></param>
        /// <param name="reader"></param>
        private void ReadMyGraphicProperty(InkCollector inkCollector, XmlReader reader)
        {
            reader.MoveToAttribute("MyGraphicId");
            string MyGraphicId=reader.Value;
            reader.MoveToAttribute("StrokeCount");
            int strokeCount = int.Parse(reader.Value);
            for (int i = 0; i < strokeCount; i++)
            {
                reader.MoveToAttribute("Stroke" + i.ToString() + "Height");
                double Height = Convert.ToDouble(reader.Value);
                reader.MoveToAttribute("Stroke" + i.ToString() + "Width");
                double Width = Convert.ToDouble(reader.Value);
                reader.MoveToAttribute("Stroke" + i.ToString() + "Color");
                String scolor = reader.Value;
                String[] argb = scolor.Split(',');
                Color Color = Color.FromArgb(Convert.ToByte(argb[0]), Convert.ToByte(argb[1]),
                    Convert.ToByte(argb[2]), Convert.ToByte(argb[3]));
                reader.MoveToAttribute("Stroke" + i.ToString() + "Points");
                string pointstring = reader.Value;
                StylusPointCollection points = new StylusPointCollection();
                String[] P = pointstring.Split(';');
                MultiCustomGesture customGesture = new MultiCustomGesture(inkCollector, inkCollector.InkCanvas);
                foreach (String point in P)
                {
                    String[] xy = point.Split(',');
                    points.Add(new StylusPoint(Convert.ToDouble(xy[0]), Convert.ToDouble(xy[1])));
                }
                Stroke stroke = new Stroke(points);
                stroke.DrawingAttributes.Height = Height;// *zoomRate;
                stroke.DrawingAttributes.Width = Width;// *zoomRate;
                stroke.DrawingAttributes.Color = Color;
                inkCollector.InkCanvas.Strokes.Add(stroke);

                int j = 0;
                foreach (String point in P)
                {
                    String[] xy = point.Split(',');
                    if (j == 0)
                    {
                        //customGesture.Gesture.StartCapture((int)Convert.ToDouble(xy[0]), (int)Convert.ToDouble(xy[1]));
                    }
                    else if (j == P.Length - 1)
                    {
                        //customGesture.Gesture.StopCapture();
                    }
                    else
                    {
                        //customGesture.Gesture.Capturing((int)Convert.ToDouble(xy[0]), (int)Convert.ToDouble(xy[1]));
                    }
                    j++;
                }
            }
            reader.MoveToAttribute("textStrokeCount");
            int textStrokeCount = int.Parse(reader.Value);
            for (int i = 0; i < textStrokeCount; i++)
            {
                reader.MoveToAttribute("textStroke" + i.ToString() + "Height");
                double textHeight = Convert.ToDouble(reader.Value);
                reader.MoveToAttribute("textStroke" + i.ToString() + "Width");
                double textWidth = Convert.ToDouble(reader.Value);
                reader.MoveToAttribute("textStroke" + i.ToString() + "Color");
                String textscolor = reader.Value;
                String[] textargb = textscolor.Split(',');
                Color textColor = Color.FromArgb(Convert.ToByte(textargb[0]), Convert.ToByte(textargb[1]),
                    Convert.ToByte(textargb[2]), Convert.ToByte(textargb[3]));
                reader.MoveToAttribute("textStroke" + i.ToString() + "Points");
                string textpointstring = reader.Value;
                StylusPointCollection textpoints = new StylusPointCollection();
                String[] textP = textpointstring.Split(';');
                foreach (String point in textP)
                {
                    String[] xy = point.Split(',');
                    textpoints.Add(new StylusPoint(Convert.ToDouble(xy[0]), Convert.ToDouble(xy[1])));
                }
                Stroke textstroke = new Stroke(textpoints);
                textstroke.DrawingAttributes.Height = textHeight;// *zoomRate;
                textstroke.DrawingAttributes.Width = textWidth;// *zoomRate;
                textstroke.DrawingAttributes.Color = textColor;
                if (inkCollector.InkCanvas.Strokes.IndexOf(textstroke) == -1)
                {
                    inkCollector.InkCanvas.Strokes.Add(textstroke);
                }
                MyGraphic MyGraphicContrainStroke = inkCollector.Sketch.getMyGraphicContrainStroke(textstroke);
                if (MyGraphicContrainStroke != null)
                {
                    if (MyGraphicContrainStroke.textStrokeCollection.IndexOf(textstroke) == -1)
                    {
                        MyGraphicContrainStroke.addTextStroke(textstroke);
                    }
                }
            }
        }
        /// <summary>
        /// 从流中读取图片
        /// </summary>
        /// <param name="inkCollector"></param>
        /// <param name="reader"></param>
		private List<MyButton> GlobalMyButtons = new List<MyButton>();
        void ReadButtonProperty(InkCollector inkCollector, XmlReader reader,int thumbIndex)
        {

            System.Windows.Controls.Button button = new Button();

            reader.MoveToAttribute("Height");
            button.Height = Convert.ToDouble(reader.Value);

            reader.MoveToAttribute("Width");
            button.Width = Convert.ToDouble(reader.Value);

            reader.MoveToAttribute("Margin");
            string[] MarginString = Regex.Split(reader.Value, ",", RegexOptions.IgnoreCase);

            button.Margin = new Thickness(Convert.ToDouble(MarginString[0]), Convert.ToDouble(MarginString[1]), Convert.ToDouble(MarginString[2]), Convert.ToDouble(MarginString[3]));


            MyButton myButton = new MyButton(button);
            double Left = button.Margin.Left;
            double Top = button.Margin.Top;

			reader.MoveToAttribute("VideoPath");
			myButton.VideoPath = reader.Value;
			reader.MoveToAttribute("VideoFileName");
			myButton.VideoFileName = reader.Value;

            reader.MoveToAttribute("Background");
            string background = reader.Value;
            String[] rgb = background.Split(',');
            SolidColorBrush myBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(rgb[0]), Convert.ToByte(rgb[1]), Convert.ToByte(rgb[2])));

            myButton.Button.Background = (System.Windows.Media.Brush)myBrush;
            reader.MoveToAttribute("AnalyzeResults");
            myButton.AnalyzeResults = reader.Value;
            reader.MoveToAttribute("ContentMoveX");
            myButton.ContentMoveX = -Convert.ToDouble(reader.Value);
            reader.MoveToAttribute("ContentMoveY");
            myButton.ContentMoveY = -Convert.ToDouble(reader.Value);
            reader.MoveToAttribute("ContentScaling");
            myButton.ContentScaling = Convert.ToDouble(reader.Value);
            reader.MoveToAttribute("Id");
            myButton.Id = int.Parse(reader.Value);
			reader.MoveToAttribute("nextId"); 
			if (reader.Value != null)
			{
				myButton.NextId = int.Parse(reader.Value);
			}
            //旋转角度
            reader.MoveToAttribute("angle");
            myButton.Angle = double.Parse(reader.Value.ToString());
            Matrix m = new Matrix(1, 0, 0, 1, 0, 0);
            m.RotateAt(myButton.Angle, myButton.Width / 2, myButton.Height / 2);
            myButton.Button.RenderTransform = new MatrixTransform(m);

            reader.MoveToAttribute("timeStart");
            myButton.TimeStart = double.Parse(reader.Value.ToString());
            reader.MoveToAttribute("timeEnd");
            myButton.TimeEnd = double.Parse(reader.Value.ToString());
            InkConstants.AddTextBoxTime(myButton);

			reader.MoveToAttribute("isGlobal");
			myButton.IsGlobal = bool.Parse(reader.Value.ToString());
			InkFrame _thumbInkFrame = new InkFrame();
			reader.MoveToAttribute("InkFrameWidth");
			double InkFrameWidth = double.Parse(reader.Value.ToString());
			myButton.InkFrameWidth = InkFrameWidth;
			_thumbInkFrame.Width = InkFrameWidth;
			reader.MoveToAttribute("InkFrameHeight");
			double InkFrameHeight = double.Parse(reader.Value.ToString());
			myButton.InkFrameHeight = InkFrameHeight;
			_thumbInkFrame.Height = InkFrameHeight;
			_thumbInkFrame.InkCollector.Mode = InkMode.None;
			_thumbInkFrame._inkCanvas.Margin = new Thickness(0);
			
			_thumbInkFrame._inkCanvas.EditingMode = InkCanvasEditingMode.None;
			

            reader.MoveToAttribute("zoomRate");
            double zoomRate = double.Parse(reader.Value);
            reader.MoveToAttribute("StrokeCount");
            int StrokeCount = int.Parse(reader.Value.ToString());
            for (int i = 0; i < StrokeCount; i++)
            {
                reader.MoveToAttribute("Stroke"+i.ToString()+"Height");
                double Height = Convert.ToDouble(reader.Value);
                reader.MoveToAttribute("Stroke" + i.ToString() + "Width");
                double Width = Convert.ToDouble(reader.Value);
                reader.MoveToAttribute("Stroke" + i.ToString() + "Color");
                String scolor = reader.Value;
                String[] argb = scolor.Split(',');
                Color Color = Color.FromArgb(Convert.ToByte(argb[0]), Convert.ToByte(argb[1]),
                    Convert.ToByte(argb[2]), Convert.ToByte(argb[3]));
                reader.MoveToAttribute("Stroke" + i.ToString() + "Points");
                string pointstring = reader.Value;
                StylusPointCollection points = new StylusPointCollection();
                String[] P = pointstring.Split(';');
                foreach (String point in P)
                {
                    String[] xy = point.Split(',');
                    points.Add(new StylusPoint(Convert.ToDouble(xy[0]), Convert.ToDouble(xy[1])));
                }
                Stroke stroke = new Stroke(points);
				stroke.DrawingAttributes.Height = Height;// *zoomRate;
				stroke.DrawingAttributes.Width = Width;// *zoomRate;
                stroke.DrawingAttributes.Color = Color;
				//_thumbInkFrame._inkCanvas.Strokes.Add(stroke);
				MyStroke myStroke = new MyStroke(stroke);
				Command ams = new AddStrokeCommand(_thumbInkFrame.InkCollector,myStroke);
				ams.execute();
            }
			//读取图片
			reader.MoveToAttribute("ImageCount");
			int ImageCount = int.Parse(reader.Value);
			for (int i = 0; i < ImageCount; i++)
			{
				System.Windows.Controls.Image image = new Image();

				reader.MoveToAttribute("MyImage" + i.ToString() + "Height");
				image.Height = Convert.ToDouble(reader.Value);

				reader.MoveToAttribute("MyImage" + i.ToString() + "Width");
				image.Width = Convert.ToDouble(reader.Value);

				reader.MoveToAttribute("MyImage" + i.ToString() + "Margin");
				MarginString = Regex.Split(reader.Value, ",", RegexOptions.IgnoreCase);

				image.Margin = new Thickness(Convert.ToDouble(MarginString[0]), Convert.ToDouble(MarginString[1]), Convert.ToDouble(MarginString[2]), Convert.ToDouble(MarginString[3]));


				BitmapImage bitimage = new BitmapImage();
				reader.MoveToAttribute("MyImage" + i.ToString() + "Source");
				bitimage.BeginInit();
				bitimage.UriSource = new Uri(reader.Value.ToString());
				bitimage.EndInit();
				image.Source = bitimage;

				MyImage myImage = new MyImage(image);
				myImage.Left = image.Margin.Left;
				myImage.Top = image.Margin.Top;
				reader.MoveToAttribute("MyImage" + i.ToString() + "SafeFileName");
				string safeFileName = reader.Value;
				myImage.SafeFileName = safeFileName;

				reader.MoveToAttribute("MyImage" + i.ToString() + "connnectorCollectionCount");
				string ConnectorCount = reader.Value.ToString();
				ConnectorCounts.Add(int.Parse(ConnectorCount));
				InkConstants.AddBound(myImage);

				SketchImages.Add(myImage);
				//旋转角度
				reader.MoveToAttribute("MyImage" + i.ToString() + "angle");
				myImage.Angle = double.Parse(reader.Value.ToString());
				m = new Matrix(1, 0, 0, 1, 0, 0);
				m.RotateAt(myImage.Angle, myImage.Width / 2, myImage.Height / 2);
				myImage.Image.RenderTransform = new MatrixTransform(m);
				_thumbInkFrame.InkCollector.Sketch.Images.Add(myImage);
				_thumbInkFrame._inkCanvas.Children.Add(myImage.Image);

				if (myButton.IsGlobal == true)
				{
					System.Windows.Controls.Image imageForTitle = new Image();
					imageForTitle.Height = image.Height;
					imageForTitle.Width = image.Width;
					imageForTitle.Margin = image.Margin;
					imageForTitle.Source = bitimage;
					MyImage myImageForTitle = new MyImage(imageForTitle);
					myImageForTitle.Left = image.Margin.Left;
					myImageForTitle.Top = image.Margin.Top;
					myImageForTitle.SafeFileName = myImage.SafeFileName;
					myImageForTitle.Angle = double.Parse(reader.Value.ToString());
					m = new Matrix(1, 0, 0, 1, 0, 0);
					m.RotateAt(myImageForTitle.Angle, myImageForTitle.Width / 2, myImageForTitle.Height / 2);
					myImageForTitle.Image.RenderTransform = new MatrixTransform(m);
					InkConstants.AddBound(myImageForTitle);
					AddImageCommand cmd = new AddImageCommand(videoOperation._titleInk.Title_InkFrame.InkCollector, myImageForTitle);
					videoOperation._titleInk.Title_InkFrame.InkCollector.CommandStack.Push(cmd);
                    cmd.execute();
                    ImageMoveCommand imc = new ImageMoveCommand(myImageForTitle, myButton.ContentMoveX, myButton.ContentMoveY);
                    imc.execute();
				}

			}

			//读取text
			reader.MoveToAttribute("TextCount");
			int TextCount = int.Parse(reader.Value);
			for (int i = 0; i < TextCount; i++)
			{
				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "Margin");
				String[] margin = reader.Value.Split(',');
				Thickness location = new Thickness(Convert.ToDouble(margin[0]),
				Convert.ToDouble(margin[1]),
				Convert.ToDouble(margin[2]),
				Convert.ToDouble(margin[3]));

				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "Content");
				string content = reader.Value;

				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "Height");
				double height = Convert.ToDouble(reader.Value);

				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "Width");
				double width = Convert.ToDouble(reader.Value);

				RichTextBox tb = new RichTextBox();
				tb.Margin = location;
				tb.Width = width;
				tb.Height = height;
				tb.HorizontalAlignment = HorizontalAlignment.Left;
				tb.VerticalAlignment = VerticalAlignment.Top;
				tb.Document.Blocks.Clear();
				System.Windows.Documents.Paragraph paragraph = new Paragraph();
				paragraph.LineHeight = 1;
				paragraph.Padding = new Thickness(0);
				Run run = new Run();
				run.Text = content;
				paragraph.Inlines.Add(run);
				tb.Document.Blocks.Add(paragraph);
				tb.IsHitTestVisible = false;
				tb.BorderBrush = null;
				tb.AcceptsReturn = true;
				tb.Background = new SolidColorBrush(Colors.Transparent);
				tb.HorizontalAlignment = HorizontalAlignment.Left;
				tb.VerticalAlignment = VerticalAlignment.Top;

				TextRange textRange = new TextRange(tb.Document.ContentStart, tb.Document.ContentEnd);
				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "FontFamily");
				textRange.ApplyPropertyValue(RichTextBox.FontFamilyProperty, reader.Value);
				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "FontSize");
				textRange.ApplyPropertyValue(RichTextBox.FontSizeProperty, reader.Value);

				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "FontBold");
				textRange.ApplyPropertyValue(RichTextBox.FontWeightProperty, reader.Value);

				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "FontItalic");
				textRange.ApplyPropertyValue(RichTextBox.FontStyleProperty, reader.Value);

				reader.MoveToAttribute("MyRichTextBox" + i.ToString() + "FontColor");
				string foreground = reader.Value;
				rgb = foreground.Split(',');
				myBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(rgb[0]), Convert.ToByte(rgb[1]), Convert.ToByte(rgb[2])));
				textRange.ApplyPropertyValue(RichTextBox.ForegroundProperty, myBrush);

				MyRichTextBox mt = new MyRichTextBox(tb);
				AddTextCommand at = new AddTextCommand(_thumbInkFrame.InkCollector, mt);
				at.execute();
				_thumbInkFrame.InkCollector.CommandStack.Push(at);
				
				if (myButton.IsGlobal == true)
				{
					RichTextBox tbForTitle = new RichTextBox();
					tbForTitle.Margin = location;
					tbForTitle.Width = width;
					tbForTitle.Height = height;
					tbForTitle.HorizontalAlignment = HorizontalAlignment.Left;
					tbForTitle.VerticalAlignment = VerticalAlignment.Top;
					tbForTitle.Document.Blocks.Clear();
					System.Windows.Documents.Paragraph paragraphForTitle = new Paragraph();
					paragraph.LineHeight = 1;
					paragraph.Padding = new Thickness(0);
					Run runForTitle = new Run();
					runForTitle.Text = content;
					paragraphForTitle.Inlines.Add(runForTitle);
					tbForTitle.Document.Blocks.Add(paragraphForTitle);
					tbForTitle.IsHitTestVisible = false;
					tbForTitle.BorderBrush = null;
					tbForTitle.AcceptsReturn = true;
					tbForTitle.Background = new SolidColorBrush(Colors.Transparent);
					tbForTitle.HorizontalAlignment = HorizontalAlignment.Left;
					tbForTitle.VerticalAlignment = VerticalAlignment.Top;
					TextRange textRangeForTitle = new TextRange(tbForTitle.Document.ContentStart, tbForTitle.Document.ContentEnd);

					textRangeForTitle.ApplyPropertyValue(RichTextBox.FontFamilyProperty, textRange.GetPropertyValue(RichTextBox.FontFamilyProperty));
					textRangeForTitle.ApplyPropertyValue(RichTextBox.FontSizeProperty, textRange.GetPropertyValue(RichTextBox.FontSizeProperty));
					textRangeForTitle.ApplyPropertyValue(RichTextBox.FontWeightProperty, textRange.GetPropertyValue(RichTextBox.FontWeightProperty));
					textRangeForTitle.ApplyPropertyValue(RichTextBox.FontStyleProperty, textRange.GetPropertyValue(RichTextBox.FontStyleProperty));
					textRangeForTitle.ApplyPropertyValue(RichTextBox.ForegroundProperty, textRange.GetPropertyValue(RichTextBox.ForegroundProperty));

					MyRichTextBox myTextForTitle = new MyRichTextBox(tbForTitle);
					AddTextCommand atc = new AddTextCommand(videoOperation._titleInk.Title_InkFrame.InkCollector, myTextForTitle);
					atc.execute();
					videoOperation._titleInk.Title_InkFrame.InkCollector.CommandStack.Push(atc);
                    TextMoveCommand mtc = new TextMoveCommand(myTextForTitle, myButton.ContentMoveX, myButton.ContentMoveY);
                    mtc.execute();

				}
			}
            if (myButton.IsGlobal)
            {
                InkConstants.InkCanvasTransform(_thumbInkFrame._inkCanvas, 1, 1,
                     myButton.ContentScaling, myButton.ContentScaling);
            }
            else
            {
                double scaling = button.Width / InkFrameWidth;
                InkConstants.InkCanvasTransform(_thumbInkFrame._inkCanvas, 1, 1, scaling, scaling);
            }
			myButton.InkFrame = _thumbInkFrame;
			myButton.addContent();
			myButton.InkFrame._inkCanvas.AddHandler(InkCanvas.MouseLeftButtonDownEvent, new MouseButtonEventHandler(_thumbInkCanvas_MouseLeftButtonDown), true);
			//myButton.InkFrame._inkCanvas.AddHandler(InkCanvas.MouseMoveEvent, new MouseEventHandler(_thumbInkCanvas_MouseMove), true);
			myButton.Button.AddHandler(Button.MouseLeaveEvent, new MouseEventHandler(Button_MouseLeave), true);
			myButton.TextBoxTime.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextBoxTime_KeyDown), true);
            if (videoOperation != null)
            {
                videoOperation._videoAnnotation.AddThumbMyButtonByLeftTop(myButton, Left, Top);
            }
			if (myButton.IsGlobal)
			{
				GlobalMyButtons.Add(myButton);
			}
        }

		void _thumbInkCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			videoOperation._videoAnnotation._thumbInkCanvas_MouseLeftButtonDown(sender, e);
		}
		void _thumbInkCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			videoOperation._videoAnnotation._thumbInkCanvas_MouseMove(sender, e);
		}
		void Button_MouseLeave(object sender, MouseEventArgs e)
		{
			videoOperation._videoAnnotation.Button_MouseLeave(sender, e);
		}
		void TextBoxTime_KeyDown(object sender, KeyEventArgs e)
		{
			videoOperation._videoAnnotation.TextBoxTime_KeyDown(sender, e);
		}


        /// <summary>
        /// 保存为图片
        /// </summary>
        /// <param name="inkcollector"></param>
        /// <param name="stream"></param>
        public void SaveInkToImage(InkCollector inkcollector, Stream stream)
        {

            int marg = int.Parse(inkcollector.InkCanvas.Margin.Left.ToString());
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)inkcollector.InkCanvas.ActualWidth - marg,
                            (int)inkcollector.InkCanvas.ActualHeight - marg, 0, 0, PixelFormats.Default);
            rtb.Render(inkcollector.InkCanvas);
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);
            stream.Close();
        }
    }

}
