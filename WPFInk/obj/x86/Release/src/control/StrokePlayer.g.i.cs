﻿#pragma checksum "..\..\..\..\..\src\control\StrokePlayer.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C320120CFDFCB47155AEB476DADA88DD"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.33440
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
using System.Windows.Shell;


namespace WPFInk {
    
    
    /// <summary>
    /// StrokePlayer
    /// </summary>
    public partial class StrokePlayer : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 106 "..\..\..\..\..\src\control\StrokePlayer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\..\..\src\control\StrokePlayer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle StartBackground;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\..\..\src\control\StrokePlayer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider ProgressSlider;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\..\..\src\control\StrokePlayer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button EndButton;
        
        #line default
        #line hidden
        
        
        #line 163 "..\..\..\..\..\src\control\StrokePlayer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StopButton;
        
        #line default
        #line hidden
        
        
        #line 164 "..\..\..\..\..\src\control\StrokePlayer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image StopImg;
        
        #line default
        #line hidden
        
        
        #line 166 "..\..\..\..\..\src\control\StrokePlayer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PlayButton;
        
        #line default
        #line hidden
        
        
        #line 169 "..\..\..\..\..\src\control\StrokePlayer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button GoOnButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPFInk;component/src/control/strokeplayer.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\src\control\StrokePlayer.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.StartBackground = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            this.ProgressSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 110 "..\..\..\..\..\src\control\StrokePlayer.xaml"
            this.ProgressSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.ProgressButton_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.EndButton = ((System.Windows.Controls.Button)(target));
            
            #line 111 "..\..\..\..\..\src\control\StrokePlayer.xaml"
            this.EndButton.Click += new System.Windows.RoutedEventHandler(this.EndButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.StopButton = ((System.Windows.Controls.Button)(target));
            
            #line 163 "..\..\..\..\..\src\control\StrokePlayer.xaml"
            this.StopButton.Click += new System.Windows.RoutedEventHandler(this.StopButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.StopImg = ((System.Windows.Controls.Image)(target));
            return;
            case 7:
            this.PlayButton = ((System.Windows.Controls.Button)(target));
            
            #line 166 "..\..\..\..\..\src\control\StrokePlayer.xaml"
            this.PlayButton.Click += new System.Windows.RoutedEventHandler(this.PlayButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.GoOnButton = ((System.Windows.Controls.Button)(target));
            
            #line 169 "..\..\..\..\..\src\control\StrokePlayer.xaml"
            this.GoOnButton.Click += new System.Windows.RoutedEventHandler(this.GoonButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

