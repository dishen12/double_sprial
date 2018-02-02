using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using WPFInk.videoSummarization;
using WPFInk.tool;
namespace WPFInk.Multitouch
{
    /// <summary>
    /// Managing the manipulation of multiple pictures in the same time
    /// </summary>
    public class PictureTrackerManager
    {
        private int downTime = 0;
        //Cache for re-use of picture trackers 
        private Stack<PictureTracker> _pictureTrackers = new Stack<PictureTracker>();
        //Map between touch ids and picture trackers
        private Dictionary<int, PictureTracker> _pictureTrackerMap = new Dictionary<int, PictureTracker>();
        private InkCanvas _inkCanvas;
        private double zoomRate = 1;
        private int zoomState = 0;//0表示不缩放，1表示放大，2表示缩小
        private bool isFocus;
        private SpiralSummarization _spiralSummarization;
        public int[] indexes = {int.MinValue,int.MinValue};
        private VideoSummarizationControl _videoSummarizationControl;
        public int ZoomState
        {
            get { return zoomState; }
            set { zoomState = value; }
        }

        public InkCanvas InkCanvas
        {
            get { return _inkCanvas; }
            set { _inkCanvas = value; }
        }
        public SpiralSummarization SpiralSummarization
        {
            get { return _spiralSummarization; }
        }


        public PictureTrackerManager(SpiralSummarization spiralSummarization, VideoSummarizationControl _videoSummarizationControl)
        {
            _spiralSummarization = spiralSummarization;
            _inkCanvas = spiralSummarization.InkCanvas;
            this._videoSummarizationControl = _videoSummarizationControl;
            this.isFocus = spiralSummarization.IsFocus;
        }

        public void ProcessDown(object sender, StylusEventArgs args)
        {
            downTime++;
            Console.WriteLine("downTime:"+downTime);
            Point location = args.GetPosition(_inkCanvas);
            int index = _spiralSummarization.getSelectedKeyFrameIndex(location);//, _spiralSummarization);
            //if (pictureTracker == null)
            //    return;
           if (index != int.MinValue)
           {
               index = _spiralSummarization.KeyFrames.IndexOf(_spiralSummarization.ShowKeyFrames[index]);
           }
            if(indexes[0] == int.MinValue)
            {
                indexes[0] = index;
            }
            else 
            { 
                indexes[1] = index; 
            }
            Console.WriteLine("index0:" + indexes[0]); 
            Console.WriteLine("index1:" + indexes[1]);
            PictureTracker pictureTracker = GetPictureTracker(args.StylusDevice.Id, location);
            
            pictureTracker.ProcessDown(args.StylusDevice.Id, location);
        }

        public void ProcessUp(object sender, StylusEventArgs args)
        {
            Point location = args.GetPosition(_inkCanvas);
            PictureTracker pictureTracker = GetPictureTracker(args.StylusDevice.Id, location);

            if (pictureTracker == null)
                return;

            pictureTracker.ProcessUp(args.StylusDevice.Id, location);
        }

        public void ProcessMove(object sender, StylusEventArgs args)
        {
            PictureTracker pictureTracker = GetPictureTracker(args.StylusDevice.Id);

            if (pictureTracker == null)
                return;

            Point location = args.GetPosition(_inkCanvas);
            pictureTracker.ProcessMove(args.StylusDevice.Id, location);
        }

        private PictureTracker GetPictureTracker(int touchId)
        {
            PictureTracker pictureTracker = null;

            _pictureTrackerMap.TryGetValue(touchId, out pictureTracker);

            return pictureTracker;
        }

        private PictureTracker GetPictureTracker(int touchId, Point location)
        {
            PictureTracker pictureTracker;

            //See if we already track the picture with the touchId
            if (_pictureTrackerMap.TryGetValue(touchId, out pictureTracker))
                return pictureTracker;

            //Get the picture under the touch location
            //Picture picture = FindPicture(location);
            int index = _spiralSummarization.getSelectedKeyFrameIndex(location);//, _spiralSummarization);

            //if (index == int.MinValue)
            //    return null;

            //See if we track the picture with other ID
            if (indexes[0] == int.MinValue && indexes[1] == int.MinValue)
            {
                pictureTracker = (from KeyValuePair<int, PictureTracker> entry in _pictureTrackerMap
                                  where entry.Value.index == index
                                  select entry.Value).FirstOrDefault();
            }
            else
            {
                pictureTracker = (from KeyValuePair<int, PictureTracker> entry in _pictureTrackerMap
                                  where entry.Value.index != index
                                  select entry.Value).FirstOrDefault();
            }
            //First time
            if (pictureTracker == null)
            {
                //take from stack
                if (_pictureTrackers.Count > 0)
                    pictureTracker = _pictureTrackers.Pop();
                else //create new
                    pictureTracker = new PictureTracker(this);

                pictureTracker.index = index;
                //BringPictureToFront(picture);
            }
            //remember the corelation between the touch id and the picture
            _pictureTrackerMap[touchId] = pictureTracker;
            //MessageBox.Show(index.ToString());
            return pictureTracker;
        }

        //We remove the touchID from the tracking map since the fingers are no longer touch
        //the picture
        public void InInertia(PictureTracker pictureTracker)
        {
            //remove all touch id from the map
            foreach (int id in
                (from KeyValuePair<int, PictureTracker> entry in _pictureTrackerMap
                 where entry.Value == pictureTracker
                 select entry.Key).ToList())
            {
                _pictureTrackerMap.Remove(id);
            }
        }

        //Inertia is completed, we can reuse the object
        public void Completed(PictureTracker pictureTracker)
        {
            Console.WriteLine("zoomState:" + zoomState);
            if (zoomState !=0)
            {
                if (indexes[0] != int.MinValue && indexes[1] != int.MinValue
                   && indexes[0] != indexes[1])
                {                    
                    if(indexes[0] > indexes[1])
                    {
                        int tem=indexes[1];
                        indexes[1]=indexes[0];
                        indexes[0]=tem;
                    }
                    if (zoomState == 1)
                    {
                        if (isFocus)
                        {
                            if (!SpiralSummarization.IsZoomOut)
                            {
                                SpiralSummarization.ZoomInOut();
                            }
                        }
                        else
                        {
                            SpiralSummarization.InsertKeyFrames(indexes[0], indexes[1]);
                        }
                    }
                    else if (zoomState == 2)
                    {
                        if (isFocus)
                        {
                            if (SpiralSummarization.IsZoomOut)
                            {
                                SpiralSummarization.ZoomInOut();
                            }
                        }
                        else
                        {
                            SpiralSummarization.HiddenKeyFrames(indexes[0], indexes[1]);
                        }
                    }
                }
                else if (indexes[0] == int.MinValue && indexes[1] == int.MinValue)
                {
                    if (zoomState == 1)
                    {
                        InkTool.getInstance().InkCanvasZoom(InkCanvas, zoomRate, zoomRate + 0.1 <= 3 ? zoomRate = zoomRate + 0.1 : zoomRate, 0.5, 0.5);
                        _videoSummarizationControl.InkCanvasShowRate.Content = (zoomRate * 100).ToString() + "%";
                    }
                    else if (zoomState == 2)
                    {
                        InkTool.getInstance().InkCanvasZoom(InkCanvas, zoomRate, zoomRate > 0.15 ? zoomRate = zoomRate - 0.1 : zoomRate, 0.5, 0.5);
                        _videoSummarizationControl.InkCanvasShowRate.Content = (zoomRate * 100).ToString() + "%";
                    }
                }
            }
           
            else if (indexes[0] != int.MinValue && indexes[1] == int.MinValue)
            {
                VideoSummarizationTool.locateMediaPlayer(
                    _spiralSummarization.InkCollector._mainPage.VideoSummarizationControl.mediaPlayer,
                    _spiralSummarization.KeyFrames[indexes[0]]);
            }
            pictureTracker.index = int.MinValue;
            indexes[0] = int.MinValue;
            indexes[1] = int.MinValue;
            zoomState = 0;
            _pictureTrackers.Push(pictureTracker);
        }

        /// <summary>
        /// Find the picture in the touch location
        /// </summary>
        /// <param name="pointF">touch location</param>
        /// <returns>The picture or null if no picture exists in the touch location</returns>
        //private Picture FindPicture(Point location)
        //{
        //    HitTestResult result = VisualTreeHelper.HitTest(_inkCanvas, location);

        //    if (result == null)
        //        return null;

        //    Image image = result.VisualHit as Image;

        //    if (image == null)
        //        return null;

        //    return image.Parent as Picture;
        //}

        //private void BringPictureToFront(Picture picture)
        //{
        //    if (picture == null)
        //        return;

        //    var children = (from UIElement child in _inkCanvas.Children
        //                    where child != picture
        //                    orderby Canvas.GetZIndex(child)
        //                    select child).ToArray();

        //    for (int i = 0; i < children.Length; ++i)
        //    {
        //        Canvas.SetZIndex(children[i], i);
        //    }
        //    Canvas.SetZIndex(picture, children.Length);
        //}
    }
}