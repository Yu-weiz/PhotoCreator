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
using WPPhotoEditor.Resources;

namespace WPPhotoEditor.Views
{
    public partial class MovieFramePage : PhoneApplicationPage
    {
        public MovieFramePage()
        {
            InitializeComponent();
            this.Loaded += MovieFramePage_Loaded;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.APChangeThePicture;
            hadInitialized = true;

            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel2;
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.StartAds();
            }
        }

        bool hadInitialized = false;

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UmengSDK.UmengAnalytics.TrackPageEnd("MovieFramePage");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UmengSDK.UmengAnalytics.TrackPageStart("MovieFramePage");
        }

        void MovieFramePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSession.Instance.CurEditingBitmapImage != null)
            {
                this.img.Source = AppSession.Instance.CurEditingBitmapImage;
                AppSession.Instance.CurEditingBitmapImage = null;
            }
        }

        private void ApplicationBarIconButtonLike_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask task = new MarketplaceReviewTask();
            task.Show();
        }

        private void ApplicationBarIconButtonSave_Click(object sender, EventArgs e)
        {
            bool canSaved = Common.LicenseManage.CanExportLicense();
            if (!canSaved)
            {
                return;
            }

            AppSession.Instance.LicenseModel.ExportJPGCount++;

            WriteableBitmap wbmp = new WriteableBitmap(this.grOutputImage, new ScaleTransform() { ScaleX = 3, ScaleY = 3 });
            Yuweiz.Phone.IO.MediaLibraryDAL.Instance.SavePicture(wbmp, "PhotoEditor_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            MessageBox.Show(WPPhotoEditor.Resources.AppResources.GlImageHaveSavedTo);
        }

        private void rbtWords_Click(object sender, RoutedEventArgs e)
        {
            //#FF767676
            var sb = this.rbtWords.Foreground as SolidColorBrush;
            Color newColor = Colors.White;
            newColor = sb.Color == Colors.White ? Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#FF767676") : Colors.White;
            this.rbtWords.Foreground = new SolidColorBrush(newColor);

            this.recScreen.Visibility = this.recScreen.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            this.grWords.Visibility = this.grWords.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void rbtCropping_Click(object sender, RoutedEventArgs e)
        {
            var sb = this.rbtCropping.Foreground as SolidColorBrush;
            Color newColor = Colors.White;
            newColor = sb.Color == Colors.White ? Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#FF767676") : Colors.White;
            this.rbtCropping.Foreground = new SolidColorBrush(newColor);

            this.recCropping1.Visibility = sb.Color == Colors.White ? Visibility.Collapsed : Visibility.Visible;
            this.recCropping2.Visibility = sb.Color == Colors.White ? Visibility.Collapsed : Visibility.Visible;

            this.grWords.Margin = sb.Color == Colors.White ? new Thickness(10, 0, 10, 5) : new Thickness(10, 0, 10, 35);
        }

        private void rbtFilter_Click(object sender, RoutedEventArgs e)
        {
            AppSession.Instance.CurEditingBitmapImage = this.img.Source as WriteableBitmap;
            if (AppSession.Instance.CurEditingBitmapImage == null)
            {
                return;
            }
            AffectPage.HadNavigated = true;
            PhoneApplicationFrame rootFrame = App.RootFrame;
            rootFrame.Navigate(new Uri("/Views/AffectPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
          //  string line1 = this.txtLine1.Text;
          //var request=  new Yuweiz.Phone.Net.HttpRequest();
          //string reqString =string.Format(@"https://www.googleapis.com/language/translate/v2?key=INSERT-YOUR-KEY&q=hello%20world&source=en&target=zh-CN";
          //request.GetUrl();

            //http://fanyi.baidu.com/#zh/en/%E4%BD%A0%E9%95%BF%E5%A4%A7%E4%BA%86.
              string line1 = this.txtLine1.Text;
            var request=  new Yuweiz.Phone.Net.HttpRequest();
            string reqString = @"http://fanyi.baidu.com/#zh/en/%E4%BD%A0%E9%95%BF%E5%A4%A7%E4%BA%86.";
            request.GetUrl(reqString,(str)=>{
                int a = 0;
                a++;

            });
        }

        private void txtLine1_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.tLine1.Text = this.txtLine1.Text;
        }

        private void txtLine2_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.tLine2.Text = this.txtLine2.Text;
        }

        private void slWordsSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!hadInitialized)
            { return; }

            this.tLine1.FontSize = slWordsSize.Value;
            this.tLine2.FontSize = slWordsSize.Value;
        }

        private void ApplicationBarMenuItemPicture_Click(object sender1, EventArgs e1)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.PixelWidth = 1600;
            photoChooserTask.PixelHeight = 900;
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += (sender, ePhoto) =>
            {
                WriteableBitmap bmp = new WriteableBitmap(1, 1);
                if (ePhoto.TaskResult == TaskResult.OK)
                {
                    bmp.SetSource(ePhoto.ChosenPhoto); //获取返回的图片 
                    this.img.Source = bmp;
                }
            };

            photoChooserTask.Show();
        }
    }
}