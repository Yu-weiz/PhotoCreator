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


namespace WPPhotoEditor.Views
{
    public partial class ColorPickerPage : PhoneApplicationPage
    {
        public ColorPickerPage()
        {
            InitializeComponent();
           // btOK.Click += new RoutedEventHandler(btOK_Click);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            AppSession.Instance.CanvasBackground = new SolidColorBrush() { Color = this.cPicker.Color };
        }

        //void btOK_Click(object sender, RoutedEventArgs e)
        //{
        //    AppSession.Instance.CanvasBackground = new SolidColorBrush() { Color=this.cPicker.Color};

        //    NavigationService.GoBack();
        //}
    }
}