using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace WPPhotoEditor.Controls
{
    public partial class UCLayerItemControl : UserControl
    {
        public UCLayerItemControl()
        {
            InitializeComponent();
        }
             
        public ImageSource ImageSource
        {
            get { return this.imgPreview.Source; }
            set
            {
                this.imgPreview.Source = value;
            
            }
        }

        public new Brush BorderBrush
        {
            get { return this.LayoutRoot.BorderBrush;}
            set { this.LayoutRoot.BorderBrush = value; }
        }

        public new Thickness BorderThickness
        {
            get { return this.LayoutRoot.BorderThickness; }
            set { this.LayoutRoot.BorderThickness = value; }
        }

        public bool IsVisible
        {
            get { return this.LayoutRoot.Opacity == 1.0; }
            set { this.LayoutRoot.Opacity =value?1: 0.2; }
        }
    }
}
