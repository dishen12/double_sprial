﻿#pragma checksum "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6503009A6B591FF2A5D6B0DF5B1710F4"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.4963
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFInk;


namespace WPFInk {
    
    
    /// <summary>
    /// VideoAnnotation
    /// </summary>
    public partial class VideoAnnotation : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal WPFInk.VideoAnnotation UserControl;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Border border;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.MediaElement mediaPlayer;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Media.ScaleTransform mediaPlayer_scale;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Media.RotateTransform mediaPlayer_rotate;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Media.TranslateTransform mediaPlayer_Translate;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Button OpenFile;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Button PlayButton;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Button PauseButton;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Button StopButton;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Button ButtonDynamicAdd;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Button ButtonStaticAdd;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Button ButtonStaticOk;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Button ButtonStaticStart;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.TextBox VideoProgress;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Slider timelineSlider;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal WPFInk.InkFrame Annotation_InkFrame;
        
        #line default
        #line hidden
        
        
        #line 125 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal WPFInk.ControlPanel Annotation_ControlPanel;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
        internal System.Windows.Controls.Label timeLabel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPFInk;component/src/control/videoannotation/videoannotation.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UserControl = ((WPFInk.VideoAnnotation)(target));
            return;
            case 2:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.border = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            this.mediaPlayer = ((System.Windows.Controls.MediaElement)(target));
            
            #line 87 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.mediaPlayer.MediaFailed += new System.EventHandler<System.Windows.ExceptionRoutedEventArgs>(this.Element_MediaOpenedFailed);
            
            #line default
            #line hidden
            
            #line 87 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.mediaPlayer.MediaOpened += new System.Windows.RoutedEventHandler(this.Element_MediaOpened);
            
            #line default
            #line hidden
            
            #line 88 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.mediaPlayer.MediaEnded += new System.Windows.RoutedEventHandler(this.mediaPlayer_MediaEnded);
            
            #line default
            #line hidden
            return;
            case 5:
            this.mediaPlayer_scale = ((System.Windows.Media.ScaleTransform)(target));
            return;
            case 6:
            this.mediaPlayer_rotate = ((System.Windows.Media.RotateTransform)(target));
            return;
            case 7:
            this.mediaPlayer_Translate = ((System.Windows.Media.TranslateTransform)(target));
            return;
            case 8:
            this.OpenFile = ((System.Windows.Controls.Button)(target));
            
            #line 101 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.OpenFile.Click += new System.Windows.RoutedEventHandler(this.OpenFile_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.PlayButton = ((System.Windows.Controls.Button)(target));
            
            #line 102 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.PlayButton.Click += new System.Windows.RoutedEventHandler(this.PlayButton_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.PauseButton = ((System.Windows.Controls.Button)(target));
            
            #line 103 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.PauseButton.Click += new System.Windows.RoutedEventHandler(this.PauseButton_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.StopButton = ((System.Windows.Controls.Button)(target));
            
            #line 104 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.StopButton.Click += new System.Windows.RoutedEventHandler(this.StopButton_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.ButtonDynamicAdd = ((System.Windows.Controls.Button)(target));
            
            #line 105 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.ButtonDynamicAdd.Click += new System.Windows.RoutedEventHandler(this.ButtonDynamicAdd_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.ButtonStaticAdd = ((System.Windows.Controls.Button)(target));
            
            #line 106 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.ButtonStaticAdd.Click += new System.Windows.RoutedEventHandler(this.ButtonStaticAdd_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            this.ButtonStaticOk = ((System.Windows.Controls.Button)(target));
            
            #line 109 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.ButtonStaticOk.Click += new System.Windows.RoutedEventHandler(this.ButtonStaticOk_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            this.ButtonStaticStart = ((System.Windows.Controls.Button)(target));
            
            #line 110 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.ButtonStaticStart.Click += new System.Windows.RoutedEventHandler(this.ButtonStaticStart_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            this.VideoProgress = ((System.Windows.Controls.TextBox)(target));
            return;
            case 17:
            this.timelineSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 114 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.timelineSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.SeekToMediaPosition);
            
            #line default
            #line hidden
            
            #line 114 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.timelineSlider.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.timelineSlider_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 114 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.timelineSlider.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.timelineSlider_PreviewMouseMove);
            
            #line default
            #line hidden
            
            #line 114 "..\..\..\..\..\src\control\VideoAnnotation\VideoAnnotation.xaml"
            this.timelineSlider.MouseLeave += new System.Windows.Input.MouseEventHandler(this.timelineSlider_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 18:
            this.Annotation_InkFrame = ((WPFInk.InkFrame)(target));
            return;
            case 19:
            this.Annotation_ControlPanel = ((WPFInk.ControlPanel)(target));
            return;
            case 20:
            this.timeLabel = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
