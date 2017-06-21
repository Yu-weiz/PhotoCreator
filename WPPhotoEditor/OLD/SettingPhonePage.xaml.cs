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
    public partial class SettingPhonePage : PhoneApplicationPage
    {
        public SettingPhonePage()
        {
            InitializeComponent();
            slSavedSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slSavedSize_ValueChanged);

            this.txtBoardWidth.GotFocus += new RoutedEventHandler(txtBoardWidth_GotFocus);
            this.txtBoardWidth.LostFocus += new RoutedEventHandler(txtBoardWidth_LostFocus);

            this.txtBoardHeight.GotFocus += new RoutedEventHandler(txtBoardWidth_GotFocus);
            this.txtBoardHeight.LostFocus += new RoutedEventHandler(txtBoardWidth_LostFocus);

            this.recBackground.MouseLeftButtonUp += new MouseButtonEventHandler(recBackground_MouseLeftButtonUp);

            this.chkMatchFirstPic.Checked += new RoutedEventHandler(chkMatchFirstPic_Checked);
            this.chkMatchFirstPic.Unchecked += new RoutedEventHandler(chkMatchFirstPic_Unchecked);


            this.btDefaultValue.Click += new RoutedEventHandler(btDefaultValue_Click);
        }

        void btDefaultValue_Click(object sender, RoutedEventArgs e)
        {
            slSavedSize.Value = 1;
        }

        void chkMatchFirstPic_Unchecked(object sender, RoutedEventArgs e)
        {
            txtBoardWidth.IsEnabled = !chkMatchFirstPic.IsChecked.Value;
            txtBoardHeight.IsEnabled = !chkMatchFirstPic.IsChecked.Value;
        }

        void chkMatchFirstPic_Checked(object sender, RoutedEventArgs e)
        {
            txtBoardWidth.IsEnabled = !chkMatchFirstPic.IsChecked.Value;
            txtBoardHeight.IsEnabled = !chkMatchFirstPic.IsChecked.Value;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            slSavedSize.Value = App.SaveScale;
            tSaveSized.Text = (int)(480 * App.SaveScale) + "*" + (int)(640 * App.SaveScale);

            txtBoardHeight.Text = App.PESettings.CanvasSize.Height.ToString();
            txtBoardWidth.Text = App.PESettings.CanvasSize.Width.ToString();

            recBackground.Fill = new SolidColorBrush(App.PESettings.CanvasBackgroundColor);
            chkMatchFirstPic.IsChecked = App.PESettings.MatchTheFirstLayerPic;

            txtBoardWidth.IsEnabled = !chkMatchFirstPic.IsChecked.Value;
            txtBoardHeight.IsEnabled = !chkMatchFirstPic.IsChecked.Value;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {

            App.PESettings.CanvasSize = new Size(Convert.ToDouble(txtBoardWidth.Text),Convert.ToDouble(txtBoardHeight.Text));
            App.PESettings.MatchTheFirstLayerPic = chkMatchFirstPic.IsChecked.Value;
            base.OnNavigatedFrom(e);
        }





        void recBackground_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/OLD/ColorChoosePhonePage.xaml",UriKind.RelativeOrAbsolute));
        }

        void txtBoardWidth_LostFocus(object sender, RoutedEventArgs e)
        {
           TextBox tbox=(sender as TextBox);
           (sender as TextBox).Text = checkReachLimit(Convert.ToInt32(tbox.Text)).ToString();
        }

        void txtBoardWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

       
     

        private int checkReachLimit(int value)
        {
            int checkedValue = value;
            if (value > 1200)
            {
                checkedValue = 1200;
            }
            else if(value < 200)
            {
                checkedValue = 200;
            }
            return checkedValue;
        }


      

        void slSavedSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            App.SaveScale = slSavedSize.Value;
            tSaveSized.Text = (int)(480 * App.SaveScale) + "*" + (int)(640 * App.SaveScale);
        }
    }
}
