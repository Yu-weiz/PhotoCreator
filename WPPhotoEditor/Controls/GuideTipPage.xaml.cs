using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Yuweiz.Phone.Common.InnerNavigation;

namespace WPPhotoEditor.Controls
{
    public partial class GuideTipPage : UserControl
    {
        public GuideTipPage()
        {
            InitializeComponent();
        }

        private void rbtClose_Click(object sender, RoutedEventArgs e)
        {
            AppSession.Instance.SettingsModel.IsShowHelp = false;
            this.GoBack();
        }
    }
}
