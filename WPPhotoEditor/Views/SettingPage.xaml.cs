using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WPPhotoEditor.Views
{
    public partial class SettingPage : PhoneApplicationPage
    {
        public SettingPage()
        {
            InitializeComponent();


            this.lsStretchPicker.SelectionChanged += lsStretchPicker_SelectionChanged;
            this.switchBtnHelp.Click += switchBtnHelp_Click;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            #region Background
            this.brBackgroundPreview.Background = AppSession.Instance.CanvasBackground;
            if (this.brBackgroundPreview.Background is ImageBrush)
            {
                ImageBrush iBrush = this.brBackgroundPreview.Background as ImageBrush;
                backgroundBmp = iBrush.ImageSource;
                this.grStretch.Visibility = Visibility.Visible;
            }
            else
            {
                this.grStretch.Visibility = Visibility.Collapsed;
            }
            #endregion

            this.tSize.Text = 480 * AppSession.Instance.SavedScale + "*" + 800 * AppSession.Instance.SavedScale;

            this.tOnOff.Text = AppSession.Instance.SettingsModel.IsShowHelp ? WPPhotoEditor.Resources.AppResources.SPON : WPPhotoEditor.Resources.AppResources.SPOFF;
            this.switchBtnHelp.IsChecked = AppSession.Instance.SettingsModel.IsShowHelp;

        }

        #region Background

        private ImageSource backgroundBmp;

        private void btnChooseColor_Click(object sender, RoutedEventArgs e)
        {
            this.backgroundBmp = null;
            this.grStretch.Visibility = Visibility.Collapsed;

            NavigationService.Navigate(new Uri("/Views/ColorPickerPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            this.backgroundBmp = null;
            this.grStretch.Visibility = Visibility.Collapsed; ;

            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += photoChooserTask_Completed;

            photoChooserTask.Show();

        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto); //获取返回的图片     

                ImageBrush imgBrush = new ImageBrush() { ImageSource = bmp, Stretch = Stretch.UniformToFill };
                AppSession.Instance.CanvasBackground = imgBrush;
                this.brBackgroundPreview.Background = imgBrush;

                this.grStretch.Visibility = Visibility.Visible;
            }
        }

        void lsStretchPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (backgroundBmp == null)
            {
                return;
            }

            Stretch stretchType = Stretch.None;
            switch (this.lsStretchPicker.SelectedIndex)
            {
                case 0: stretchType = Stretch.UniformToFill; break;
                case 1: stretchType = Stretch.Uniform; break;
                case 2: stretchType = Stretch.Fill; break;
                case 3: stretchType = Stretch.None; break;
            }

            ImageBrush imgBrush = new ImageBrush() { ImageSource = backgroundBmp, Stretch = stretchType };
            AppSession.Instance.CanvasBackground = imgBrush;
            this.brBackgroundPreview.Background = imgBrush;
        }

        #endregion

        private void slSizeScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.slSizeScale == null)
            {
                return;
            }

            int width = (int)(480 * this.slSizeScale.Value);
            int height = (int)(800 * this.slSizeScale.Value);
            this.tSize.Text = width.ToString() + "*" + height.ToString();
            AppSession.Instance.SavedScale = this.slSizeScale.Value;
        }

        private void switchBtnHelp_Click(object sender, RoutedEventArgs e)
        {
            if (this.switchBtnHelp.IsChecked != null)
            {
                this.tOnOff.Text = this.switchBtnHelp.IsChecked.Value ? WPPhotoEditor.Resources.AppResources.SPON : WPPhotoEditor.Resources.AppResources.SPOFF;
                AppSession.Instance.SettingsModel.IsShowHelp = this.switchBtnHelp.IsChecked.Value;
            }

        }
    }
}