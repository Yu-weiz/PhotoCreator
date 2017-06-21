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
using Yuweiz.Phone.Common.AppStructure;

namespace WPPhotoEditor
{
    public partial class StartupMainPage : PhoneApplicationPage
    {
        public StartupMainPage()
        {
            InitializeComponent();

            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel3;
                this.adUltimateBar.StartAds();
            }

            #region 版本选择
            this.brProVersion.Visibility = AppSession.LicenseTypeEnum == LicenseTypeEnum.InternationalPay ? Visibility.Visible : Visibility.Collapsed;
            #endregion
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.ReviewMe();
        }

        private void btnCollage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/CollagePage.xaml", UriKind.Relative));
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/AffectPage.xaml", UriKind.Relative));
        }

        private void btnMovie_Click(object sender1, RoutedEventArgs e1)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.PixelWidth = 1600;
            photoChooserTask.PixelHeight = 900;
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += (sender, ePhoto) =>
            {
                WriteableBitmap bmp = new WriteableBitmap(1,1);
                if (ePhoto.TaskResult == TaskResult.OK)
                {
                    bmp.SetSource(ePhoto.ChosenPhoto); //获取返回的图片 
                    AppSession.Instance.CurEditingBitmapImage = bmp;
                    this.NavigationService.Navigate(new Uri("/Views/MovieFramePage.xaml", UriKind.Relative));
                }
            };

            photoChooserTask.Show();           
        }

        private void btnFreeBegin_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/EditPage.xaml", UriKind.Relative));
        }

        private void btnClassical_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/OLD/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ReviewMe()
        {
            if (AppSession.Instance.LicenseModel.HadReviewed || AppSession.Instance.LicenseModel.ExportJPGCount <2 || AppSession.Instance.LicenseModel.ExportJPGCount % 2 != 1)
            {
                return;
            }

            CheckBox checkBox = new CheckBox()
            {
                Content = WPPhotoEditor.Resources.AppResources.GlDontAskMeAgain,
                Margin = new Thickness(0, 14, 0, -2)
            };

            Microsoft.Phone.Controls.TiltEffect.SetIsTiltEnabled(checkBox, true);

            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = WPPhotoEditor.Resources.AppResources.MEncourageUs,
                Message = WPPhotoEditor.Resources.AppResources.GlRateViewString,
                Content = checkBox,
                LeftButtonContent = WPPhotoEditor.Resources.AppResources.APOK,
                RightButtonContent = WPPhotoEditor.Resources.AppResources.GlCancel
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        MarketplaceReviewTask reviewTask = new MarketplaceReviewTask();
                        reviewTask.Show();
                        break;
                    case CustomMessageBoxResult.RightButton:
                        if ((bool)checkBox.IsChecked)
                        {
                            AppSession.Instance.LicenseModel.HadReviewed = true;
                        }
                        AppSession.Instance.LicenseModel.ExportJPGCount++;
                        break;
                    case CustomMessageBoxResult.None:
                        if ((bool)checkBox.IsChecked)
                        {
                            AppSession.Instance.LicenseModel.HadReviewed = true;
                        }
                        AppSession.Instance.LicenseModel.ExportJPGCount++;
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }
    }
}