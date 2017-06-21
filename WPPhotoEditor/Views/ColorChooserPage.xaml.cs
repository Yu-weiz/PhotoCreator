using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WPPhotoEditor.Views
{
    public partial class ColorChooserPage : PhoneApplicationPage
    {
        public ColorChooserPage()
        {
            InitializeComponent();
            this.Loaded += ColorChooserPage_Loaded;
        }

        void ColorChooserPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSession.Instance.ChosenColor != null)
            {
                this.cPicker.Color =AppSession.Instance.ChosenColor.Value;
                AppSession.Instance.ChosenColor = null;
            }
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            AppSession.Instance.ChosenColor = this.cPicker.Color;
            this.NavigationService.GoBack();
        }
    }
}