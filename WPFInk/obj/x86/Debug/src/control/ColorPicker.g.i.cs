﻿#pragma checksum "..\..\..\..\..\src\control\ColorPicker.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "44D2968948FA47DFD1768E4FF971B9EE"
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
    /// ColorPicker
    /// </summary>
    public partial class ColorPicker : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WPFInk.ColorPicker UserControl;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border RootControl;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RowDefinition colorRow;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition colorColumn;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition rightColumn;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas BackgroundCanvas;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas ColorCanvas;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid FinalColor;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border resultCanvas;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border RainbowBorder;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas HueCanvas;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid RainbowHandle;
        
        #line default
        #line hidden
        
        
        #line 136 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel CompactPanel;
        
        #line default
        #line hidden
        
        
        #line 139 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CompactRGBText;
        
        #line default
        #line hidden
        
        
        #line 147 "..\..\..\..\..\src\control\ColorPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CompactHexText;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFInk;component/src/control/colorpicker.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\src\control\ColorPicker.xaml"
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
            this.UserControl = ((WPFInk.ColorPicker)(target));
            
            #line 10 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.UserControl.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.TurnEverythingOff);
            
            #line default
            #line hidden
            return;
            case 2:
            this.RootControl = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.colorRow = ((System.Windows.Controls.RowDefinition)(target));
            return;
            case 5:
            this.colorColumn = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 6:
            this.rightColumn = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 7:
            this.BackgroundCanvas = ((System.Windows.Controls.Canvas)(target));
            return;
            case 8:
            this.ColorCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 52 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.ColorCanvas.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Canvas_MouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 53 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.ColorCanvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Canvas_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 54 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.ColorCanvas.MouseMove += new System.Windows.Input.MouseEventHandler(this.Canvas_MouseMove);
            
            #line default
            #line hidden
            
            #line 55 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.ColorCanvas.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Canvas_MouseLeave);
            
            #line default
            #line hidden
            
            #line 56 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.ColorCanvas.SizeChanged += new System.Windows.SizeChangedEventHandler(this.ColorCanvas_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.FinalColor = ((System.Windows.Controls.Grid)(target));
            return;
            case 10:
            this.resultCanvas = ((System.Windows.Controls.Border)(target));
            
            #line 75 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.resultCanvas.Loaded += new System.Windows.RoutedEventHandler(this.resultCanvas_Loaded);
            
            #line default
            #line hidden
            return;
            case 11:
            this.RainbowBorder = ((System.Windows.Controls.Border)(target));
            
            #line 81 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.RainbowBorder.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.RainbowBorder_TurnOn);
            
            #line default
            #line hidden
            
            #line 82 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.RainbowBorder.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.RainbowBorder_TurnOff);
            
            #line default
            #line hidden
            
            #line 83 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.RainbowBorder.MouseMove += new System.Windows.Input.MouseEventHandler(this.RainbowBorder_UpdateHue);
            
            #line default
            #line hidden
            return;
            case 12:
            this.HueCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 96 "..\..\..\..\..\src\control\ColorPicker.xaml"
            this.HueCanvas.SizeChanged += new System.Windows.SizeChangedEventHandler(this.HueCanvas_SizeChanged);
            
            #line default
            #line hidden
            return;
            case 13:
            this.RainbowHandle = ((System.Windows.Controls.Grid)(target));
            return;
            case 14:
            this.CompactPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 15:
            this.CompactRGBText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 16:
            this.CompactHexText = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
