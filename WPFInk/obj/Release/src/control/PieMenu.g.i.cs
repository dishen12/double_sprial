﻿#pragma checksum "..\..\..\..\src\control\PieMenu.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F847ACD9A8D8C3AD1EE15D05EFF6B0A6"
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
using WPFInk;


namespace WPFInk {
    
    
    /// <summary>
    /// PieMenu
    /// </summary>
    public partial class PieMenu : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\src\control\PieMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WPFInk.PieMenu UserControl;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\src\control\PieMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\src\control\PieMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ZoomButton;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\src\control\PieMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path path;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\src\control\PieMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RotateButton;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\..\src\control\PieMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MoveButton;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\src\control\PieMenu.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CloseButton;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFInk;component/src/control/piemenu.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\src\control\PieMenu.xaml"
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
            this.UserControl = ((WPFInk.PieMenu)(target));
            return;
            case 2:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.ZoomButton = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\..\..\src\control\PieMenu.xaml"
            this.ZoomButton.Click += new System.Windows.RoutedEventHandler(this.ZoomButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.path = ((System.Windows.Shapes.Path)(target));
            return;
            case 5:
            this.RotateButton = ((System.Windows.Controls.Button)(target));
            
            #line 21 "..\..\..\..\src\control\PieMenu.xaml"
            this.RotateButton.Click += new System.Windows.RoutedEventHandler(this.RotateButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.MoveButton = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\..\..\src\control\PieMenu.xaml"
            this.MoveButton.Click += new System.Windows.RoutedEventHandler(this.MoveButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CloseButton = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\..\src\control\PieMenu.xaml"
            this.CloseButton.Click += new System.Windows.RoutedEventHandler(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

