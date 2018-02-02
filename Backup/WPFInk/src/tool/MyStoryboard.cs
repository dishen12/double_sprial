using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WPFInk.videoSummarization;
using System.Windows.Media;
using System.Windows.Input;

namespace WPFInk.tool
{
	public class MyStoryboard
	{
		private static MyStoryboard myStoryboard = null;

		private MyStoryboard()
        {
        
        }

		public static MyStoryboard getInstance()
        {
			if (myStoryboard == null)
				myStoryboard = new MyStoryboard();
			return myStoryboard;
        }
        /// <summary>
        /// 高度变化动画
        /// </summary>
        /// <param name="border"></param>
        /// <param name="fromHeight"></param>
        /// <param name="toHeight"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
		public Storyboard HeightStoryboard(Border border, double fromHeight, double toHeight,double duration)
		{
			DoubleAnimation myDoubleAnimation = new DoubleAnimation();
			myDoubleAnimation.From = fromHeight;
			myDoubleAnimation.To = toHeight;
			myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration));
			myDoubleAnimation.AutoReverse = false;
			Storyboard.SetTargetName(myDoubleAnimation, border.Name);
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Border.HeightProperty));

			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(myDoubleAnimation);
			return storyboard;
		}
        /// <summary>
        /// 螺旋摘要关键帧变化
        /// </summary>
        /// <param name="keyFrameFrom"></param>
        /// <param name="keyFrameTo"></param>
        /// <param name="duration"></param>
        public static Storyboard KeyFrameInSpiralMove(KeyFrame keyFrameFrom,double duration,PathGeometry pathGeometry)
        {
            keyFrameFrom.Image.Margin = new Thickness(0);
            
            MatrixTransform ImageMatrixTransform = new MatrixTransform();
            keyFrameFrom.Image.RenderTransform = ImageMatrixTransform;
            NameScope.SetNameScope(keyFrameFrom.Image, new NameScope());
            keyFrameFrom.Image.RegisterName("ImageMatrixTransform", ImageMatrixTransform);

            MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath();
            matrixAnimation.PathGeometry = pathGeometry;
            matrixAnimation.Duration = TimeSpan.FromSeconds(duration);
            //matrixAnimation.RepeatBehavior = RepeatBehavior.Forever;
           // matrixAnimation.DoesRotateWithTangent = true;
            Storyboard.SetTargetName(matrixAnimation, "ImageMatrixTransform");
            Storyboard.SetTargetProperty(matrixAnimation,
                new PropertyPath(MatrixTransform.MatrixProperty));

            // Create a Storyboard to contain and apply the animation.
            Storyboard pathAnimationStoryboard = new Storyboard();
            pathAnimationStoryboard.Children.Add(matrixAnimation);
            return pathAnimationStoryboard;

        }
        
        /// <summary>
        /// 螺旋摘要关键帧变化
        /// </summary>
        /// <param name="spiralSummarization">螺旋摘要</param>
        /// <param name="keyFrameFrom">旋转的帧</param>
        /// <param name="duration">持续时间</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Storyboard KeyFrameInSpiralMove(SpiralSummarization spiralSummarization, KeyFrame keyFrameFrom, double duration, double angle)
        {
            RotateTransform rotateTransform = new RotateTransform();
            keyFrameFrom.Image.RenderTransform = rotateTransform;
            //旋转中心设置
            rotateTransform.CenterX = spiralSummarization.Center.X - keyFrameFrom.Left;
            rotateTransform.CenterY = spiralSummarization.Center.Y - keyFrameFrom.Top;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = -angle;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration));
            if (keyFrameFrom.Image.FindName("rotateTransform") != null)
            {
                keyFrameFrom.Image.UnregisterName("rotateTransform");
            }
            NameScope.SetNameScope(keyFrameFrom.Image, new NameScope());
            keyFrameFrom.Image.RegisterName("rotateTransform", rotateTransform);
            Storyboard.SetTargetName(myDoubleAnimation, "rotateTransform");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(RotateTransform.AngleProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(myDoubleAnimation);
            return storyboard;
        }
        /// <summary>
        /// 对inkcanvas进行缩放并旋转
        /// </summary>
        /// <param name="inkcanvas">缩放对象</param>
        /// <param name="rate">缩放比率</param>
        public static void InkCanvasZoomRotate(InkCanvas inkCanvas,double duration, double fromZoomRate, double toZoomRate, double centerX, double centerY)
        {
            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform();
            transformGroup.Children.Add(scaleTransform);
            //inkCanvas.RenderTransform = scaleTransform;
            scaleTransform.CenterX = inkCanvas.ActualWidth * centerX;
            scaleTransform.CenterY = inkCanvas.ActualHeight * centerY;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            //宽度缩放
            myDoubleAnimation.From = fromZoomRate;
            myDoubleAnimation.To = toZoomRate;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration));
            if (inkCanvas.FindName("scaleTransform") != null)
            {
                inkCanvas.UnregisterName("scaleTransform");
            }
            inkCanvas.RegisterName("scaleTransform", scaleTransform);
            Storyboard.SetTargetName(myDoubleAnimation, "scaleTransform");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));


            DoubleAnimation myDoubleAnimationHeight = new DoubleAnimation();
            //高度缩放
            myDoubleAnimationHeight.From = fromZoomRate;
            myDoubleAnimationHeight.To = toZoomRate;
            myDoubleAnimationHeight.Duration = myDoubleAnimation.Duration;
            Storyboard.SetTargetName(myDoubleAnimationHeight, "scaleTransform");
            Storyboard.SetTargetProperty(myDoubleAnimationHeight, new PropertyPath(ScaleTransform.ScaleYProperty));

            RotateTransform rotateTransform = new RotateTransform();
            transformGroup.Children.Add(rotateTransform);
            inkCanvas.RenderTransform = transformGroup;
            rotateTransform.CenterX = scaleTransform.CenterX;
            rotateTransform.CenterY = scaleTransform.CenterY;
            DoubleAnimation rotateDoubleAnimation = new DoubleAnimation();
            rotateDoubleAnimation.From = 0;
            rotateDoubleAnimation.To = 720;
            rotateDoubleAnimation.Duration = myDoubleAnimation.Duration;
            if (inkCanvas.FindName("rotateTransform") != null)
            {
                inkCanvas.UnregisterName("rotateTransform");
            }
            inkCanvas.RegisterName("rotateTransform", rotateTransform);
            Storyboard.SetTargetName(rotateDoubleAnimation, "rotateTransform");
            Storyboard.SetTargetProperty(rotateDoubleAnimation, new PropertyPath(RotateTransform.AngleProperty));


            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Children.Add(myDoubleAnimationHeight);
            myStoryboard.Children.Add(rotateDoubleAnimation);
            myStoryboard.Begin(inkCanvas);
        }
        
	}
}
