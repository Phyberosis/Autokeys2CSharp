﻿#pragma checksum "..\..\..\Views\MacrosView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A5C0C45B8E630E90919996E870F7BB396DF7E9174FAD159C5D21010754058395"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Autokeys2.Views;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace Autokeys2.Views {
    
    
    /// <summary>
    /// MacrosView
    /// </summary>
    public partial class MacrosView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\Views\MacrosView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Autokeys2.Views.MacrosView brdLeft;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\..\Views\MacrosView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSave;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\Views\MacrosView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnLoad;
        
        #line default
        #line hidden
        
        
        #line 121 "..\..\..\Views\MacrosView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtFileName;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\..\Views\MacrosView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl keyframesControl;
        
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
            System.Uri resourceLocater = new System.Uri("/Autokeys2;component/views/macrosview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\MacrosView.xaml"
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
            this.brdLeft = ((Autokeys2.Views.MacrosView)(target));
            return;
            case 2:
            
            #line 83 "..\..\..\Views\MacrosView.xaml"
            ((System.Windows.Controls.Border)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.traysLostFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnSave = ((System.Windows.Controls.Button)(target));
            
            #line 116 "..\..\..\Views\MacrosView.xaml"
            this.btnSave.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.btnSave_MouseDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnLoad = ((System.Windows.Controls.Button)(target));
            
            #line 118 "..\..\..\Views\MacrosView.xaml"
            this.btnLoad.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.btnLoad_MouseDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtFileName = ((System.Windows.Controls.TextBox)(target));
            
            #line 121 "..\..\..\Views\MacrosView.xaml"
            this.txtFileName.GotFocus += new System.Windows.RoutedEventHandler(this.txtFileName_GotFocus);
            
            #line default
            #line hidden
            return;
            case 6:
            this.keyframesControl = ((System.Windows.Controls.ItemsControl)(target));
            
            #line 141 "..\..\..\Views\MacrosView.xaml"
            this.keyframesControl.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.traysLostFocus);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

