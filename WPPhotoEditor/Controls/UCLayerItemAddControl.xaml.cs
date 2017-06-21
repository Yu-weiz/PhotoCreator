using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WPPhotoEditor.Controls
{
    public partial class UCLayerItemAddControl : UserControl
    {
        public UCLayerItemAddControl()
        {
            InitializeComponent();
        }

        public bool IsVisible
        {
            get { return this.Opacity == 0.6; }
            set { this.Opacity = 0.6; }
        }
    }
}
