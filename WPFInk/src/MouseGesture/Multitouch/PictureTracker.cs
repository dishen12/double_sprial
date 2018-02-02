using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using Windows7.Multitouch.Manipulation;
using Windows7.Multitouch.WPF;
using System.Windows;
using WPFInk.tool;
using System.Windows.Controls;

namespace WPFInk.Multitouch
{
    /// <summary>
    /// Track a single picture
    /// </summary>
    public class PictureTracker
    {
        //Calculate the inertia start velocity
        private readonly InertiaParam _inertiaParam = new InertiaParam();

        private readonly ManipulationInertiaProcessor _processor =
            new ManipulationInertiaProcessor(ProcessorManipulations.ALL, Factory.CreateTimer());

        private PictureTrackerManager _pictureTrackerManager;

        public PictureTracker(PictureTrackerManager pictureTrackerManager)
        {
            _pictureTrackerManager = pictureTrackerManager;

            //Start inertia velocity calculations
            _processor.ManipulationStarted += (s, e) =>
            {
                _inertiaParam.Reset();
            };

            //All completed, inform the tracker manager that the current tracker can be reused
            _processor.ManipulationCompleted += (s, e) => { _inertiaParam.Stop(); pictureTrackerManager.Completed(this); };
            _processor.ManipulationDelta += ProcessManipulationDelta;
            _processor.BeforeInertia += OnBeforeInertia;
        }

        public int index { get; set; }

        public void ProcessDown(int id, System.Windows.Point location)
        {
            _processor.ProcessDown((uint)id, location.ToDrawingPointF());
        }

        public void ProcessMove(int id, System.Windows.Point location)
        {
            _processor.ProcessMove((uint)id, location.ToDrawingPointF());
        }

        public void ProcessUp(int id, System.Windows.Point location)
        {
            _processor.ProcessUp((uint)id, location.ToDrawingPointF());
        }

        //Update picture state
        private void ProcessManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            //if (index == int.MinValue)
            //    return;
            //System.Windows.Controls.Image image = _pictureTrackerManager.SpiralSummarization.KeyFrames[index].Image;
           // Console.WriteLine(index);

            //image.Margin = new Thickness(image.Margin.Left + e.TranslationDelta.Width, image.Margin.Top + e.TranslationDelta.Height,0,0);
            if (e.ScaleDelta > 1)
            {
                _pictureTrackerManager.ZoomState = 1;
            }
            else if (e.ScaleDelta < 1)
            {
                _pictureTrackerManager.ZoomState = 2;
            }
            //Console.WriteLine("index:" + index);
            //Console.WriteLine("scaledelda:" + e.ScaleDelta);
           // Console.WriteLine("zoomState:" + _pictureTrackerManager.ZoomState);
            
        }

        //Fingers removed, start inertia
        void OnBeforeInertia(object sender, BeforeInertiaEventArgs e)
        {
            //Tell the tracker manager that the user removed the fingers
            _pictureTrackerManager.InInertia(this);

            _processor.InertiaProcessor.InertiaTimerInterval = 15;
            _processor.InertiaProcessor.MaxInertiaSteps = 500;
            _processor.InertiaProcessor.InitialVelocity = _inertiaParam.InitialVelocity;
            _processor.InertiaProcessor.DesiredDisplacement = _inertiaParam.InitialVelocity.Magnitude * 250;
            _processor.InertiaProcessor.InitialAngularVelocity = _inertiaParam.InitialAngularVelocity * 20F / (float)Math.PI;
            _processor.InertiaProcessor.DesiredRotation = Math.Abs(_inertiaParam.InitialAngularVelocity * _processor.InertiaProcessor.InertiaTimerInterval * 540F / (float)Math.PI);
            _processor.InertiaProcessor.InitialExpansionVelocity = _inertiaParam.InitialExpansionVelocity * 15;
            _processor.InertiaProcessor.DesiredExpansion = Math.Abs(_inertiaParam.InitialExpansionVelocity * 4F);
        }

        //Keep track of object velocities
        private class InertiaParam
        {
            public VectorF InitialVelocity { get; set; }
            public float InitialAngularVelocity { get; set; }
            public float InitialExpansionVelocity { get; set; }
            public System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
            public void Reset()
            {
                InitialVelocity = new VectorF(0, 0);
                InitialAngularVelocity = 0;
                InitialExpansionVelocity = 0;
                _stopwatch.Reset();
                _stopwatch.Start();
            }

            public void Stop()
            {
                _stopwatch.Stop();
            }

            //update velocities, velocity = distance/time
            public void Update(ManipulationDeltaEventArgs e, float history)
            {
                float elappsedMS = (float)_stopwatch.ElapsedMilliseconds;
                if (elappsedMS == 0)
                    elappsedMS = 1;

                InitialVelocity = InitialVelocity * history + ((VectorF)e.TranslationDelta * (1F - history)) / elappsedMS;
                InitialAngularVelocity = InitialAngularVelocity * history + (e.RotationDelta * (1F - history)) / elappsedMS;
                InitialExpansionVelocity = InitialExpansionVelocity * history + (e.ExpansionDelta * (1F - history)) / elappsedMS;
                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }
    }
}