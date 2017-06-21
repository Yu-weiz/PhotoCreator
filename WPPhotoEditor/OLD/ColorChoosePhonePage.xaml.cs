using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace WPPhotoEditor.OLD
{
    public partial class ColorChoosePhonePage : PhoneApplicationPage
    {
        public ColorChoosePhonePage()
        {
            InitializeComponent();
            btOK.Click += new RoutedEventHandler(btOK_Click);
        }

        void btOK_Click(object sender, RoutedEventArgs e)
        {
            App.PESettings.CanvasBackgroundColor = cPicker.Color;
            NavigationService.GoBack();
        }
    }
}
