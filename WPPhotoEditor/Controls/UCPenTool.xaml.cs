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
using Yuweiz.Phone.Media.Imaging;

namespace WPPhotoEditor.UControls
{
    public partial class UCPenTool : UserControl
    {
        public UCPenTool()
        {
            InitializeComponent();
         
        }
                
        public bool HadChosen
        {
            set 
            {
                this.brFrame.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public FillPixelTypeEnum PenType { get; set; }

        public Brush MainBackground
        {
            set { this.brMain.Background = value; }
        }

        public Brush MainOpacityMask
        {
            set { this.brMain.OpacityMask = value; }          
        }
    }
}
