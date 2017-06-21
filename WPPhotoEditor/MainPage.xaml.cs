using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using System.Threading;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using Windows.ApplicationModel;
using System.Windows.Media.Animation;

namespace WPPhotoEditor
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            // this.btnSketchBoard.Click += btnSketchBoard_Click;
            //this.imgYuweizLogo.Tap += (sender, e) =>
            //{
            //    WebBrowserTask webTask = new WebBrowserTask();
            //    string url = AppSession.Instance.IsChineseCulture ? "http://www.weibo.com/5130298741" : "http://www.twitter.com/yu_weiz";
            //    webTask.Uri = new Uri(url);
            //    webTask.Show();
            //};

            this.Loaded += (sender, e) =>
            {
                Storyboard storyBoard = this.Resources["StoryboardSketchBoard"] as Storyboard;
                storyBoard.AutoReverse = true;
                storyBoard.RepeatBehavior = RepeatBehavior.Forever;
                storyBoard.Begin();
            };

            this.brSketchMask.Tap += (sender, e) =>
                {
                    btnSketchBoard_Click(null, null);
                };

            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel0;
                this.adUltimateBar.StartAds();
            }
        }

        void btnSketchBoard_Click(object sender, RoutedEventArgs e)
        {
            //查询本机其他与当前应用发行者ID相同的其他应用
            IEnumerable<Package> apps = Windows.Phone.Management.Deployment.InstallationManager.FindPackagesForCurrentPublisher();

            Package pagckage = null;
            foreach (Package app in apps)
            {
                string appName = AppSession.Instance.IsChineseCulture ? "素描画板" : "Sketch Board";
                if (app.Id.Name == appName)
                {
                    pagckage = app;
                    break;
                }
            }

            if (pagckage == null)
            {
                MessageBoxResult mResult = MessageBox.Show(WPPhotoEditor.Resources.AppResources.MYouHaveNotInstalledSketchBoard, WPPhotoEditor.Resources.AppResources.MInstallNow, MessageBoxButton.OKCancel);
                if (mResult == MessageBoxResult.OK)
                {
                    string appId = AppSession.Instance.IsChineseCulture ? "d2b90019-dec6-49a2-bb0c-a11b2b464357" : "afbce866-a0fa-4729-b613-c5a5e3dc7e16";
                    MarketplaceDetailTask detailTask = new MarketplaceDetailTask();
                    detailTask.ContentIdentifier = appId;
                    detailTask.Show();
                }               
            }
            else
            {
                pagckage.Launch(string.Empty);
            }

        }

        //private bool isWaitingDeparture = false;

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            #region 退出提醒
            //e.Cancel = true;
            //if (!isWaitingDeparture)
            //{
            //    isWaitingDeparture = true;
            //    ToastPrompt toast = new ToastPrompt();
            //    toast.MillisecondsUntilHidden = 2000;
            //    toast.Message = "Press the back key again ! =^_^=";
            //    toast.Title = "Leave?";
            //    toast.Show();

            //    Thread leaveThread = new Thread(new ThreadStart(() =>
            //    {
            //        Thread.Sleep(2000);
            //        isWaitingDeparture = false;

            //    }));//创建一个线程  
            //    leaveThread.Start();//启动  
            //}
            //else
            //{
            //    e.Cancel = false;
            //}
            #endregion
        }

        // 为 ViewModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.tVersion.Text = "Version " + AppSession.Instance.Version;

            #region 加载自定义背景
            this.chkDefaultBackground.IsEnabled = false;
            if (!string.IsNullOrEmpty(AppSession.Instance.SettingsModel.MainPageBackgroundPath))
            {
                using (Stream stream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(AppSession.MainPageBackgroundPath))
                {
                    if (stream != null)
                    {
                        BitmapImage bmp = new BitmapImage();
                        bmp.SetSource(stream); //获取返回的图片                       

                        this.chkDefaultBackground.IsChecked = false;
                        this.panoramaMain.Title = "Photo Creator";
                        this.panoramaMain.Background = new ImageBrush() { ImageSource = bmp, Stretch = Stretch.None };
                        this.chkDefaultBackground.IsEnabled = true;

                        stream.Close();
                        stream.Dispose();
                    }
                }
            }
            #endregion

            string sketchPath = "/Assets/Image/SketchFlipCycleTileLarge" + (AppSession.Instance.IsChineseCulture ? "ZH.png" : ".jpg");
            this.imgSketchBoard.Source = new BitmapImage(new Uri(sketchPath, UriKind.Relative));

            this.ReviewMe();
        }

        private void Collage_Click(object sender, RoutedEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackEvent("");
            NavigationService.Navigate(new Uri("/Views/CollagePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Paint_Click(object sender, RoutedEventArgs e)
        {
            WPPhotoEditor.Media.BrushFactory.Instance.InitializeDefaultColorBrush();
            NavigationService.Navigate(new Uri("/Views/EditBasePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Magic_Click(object sender, RoutedEventArgs e)
        {
            //PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            //photoChooserTask.ShowCamera = true;
            //photoChooserTask.Completed += AffectPhotoChooserTask_Completed;
            //photoChooserTask.Show();
            AffectPage.HadNavigated = false;
            NavigationService.Navigate(new Uri("/Views/AffectPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void IDontKnow_Click(object sender, RoutedEventArgs e)
        {
            //MarketplaceSearchTask searchTask = new MarketplaceSearchTask();
            //searchTask.SearchTerms = "Yu-weiz";
            //searchTask.Show();
            NavigationService.Navigate(new Uri("/OLD/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void AffectPhotoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //MessageBox.Show(e.OriginalFileName);

                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto); //获取返回的图片     
                AppSession.Instance.CurEditingBitmapImage = new WriteableBitmap(bmp);

                NavigationService.Navigate(new Uri("/Views/AffectPage.xaml", UriKind.RelativeOrAbsolute));
            }

        }

        private void btnFeedback_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailTask = new EmailComposeTask();
            emailTask.To = "Yu-weiz@live.com";
            emailTask.Subject = "Photo Creator(1.6) Feedback";
            emailTask.Show();
        }

        private void btnEncourageUs_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask reviewTask = new MarketplaceReviewTask();
            reviewTask.Show();

        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            //PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            //photoChooserTask.ShowCamera = true;
            //photoChooserTask.Completed += photoChooserTask_Completed;
            //photoChooserTask.Show();

            NavigationService.Navigate(new Uri("/Views/OptionPage.xaml", UriKind.RelativeOrAbsolute));
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto); //获取返回的图片     
                WriteableBitmap wbmpBackground = new WriteableBitmap(bmp);

                Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.SavePicture(wbmpBackground, AppSession.MainPageBackgroundPath);
                AppSession.Instance.SettingsModel.MainPageBackgroundPath = AppSession.MainPageBackgroundPath;

                this.chkDefaultBackground.IsEnabled = true;
                this.chkDefaultBackground.IsChecked = false;
                this.panoramaMain.Title = "Photo Creator";
                this.panoramaMain.Background = new ImageBrush() { ImageSource = wbmpBackground, Stretch = Stretch.None };
            }
        }

        private void chkDefaultBackground_Click(object sender, RoutedEventArgs e)
        {
            if (this.chkDefaultBackground.IsChecked != null && this.chkDefaultBackground.IsChecked.Value)
            {
                //this.panoramaMain.Title = "";
                //WriteableBitmap wbmp = WPPhotoEditor.ViewModels.Utility.GetImageResource("PhotoEditorDefault480X800.png");
                //this.panoramaMain.Background = new ImageBrush() { ImageSource = wbmp, Stretch = Stretch.Uniform };

                if (!string.IsNullOrEmpty(AppSession.Instance.SettingsModel.MainPageBackgroundPath))
                {
                    Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.DeleteFile(AppSession.Instance.SettingsModel.MainPageBackgroundPath);
                    AppSession.Instance.SettingsModel.MainPageBackgroundPath = string.Empty;

                    ToastPrompt toast = new ToastPrompt();
                    toast.MillisecondsUntilHidden = 2000;
                    toast.Message = WPPhotoEditor.Resources.AppResources.MItWillTakeEffectAtTheNextStart;
                    toast.Show();

                    this.chkDefaultBackground.IsEnabled = false;
                }
            }

        }

        private void MenuItemPinCollage_Click(object sender, RoutedEventArgs e)
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("CollagePage.xaml"));

            // Create the Tile if we didn't find that it already exists.
            if (TileToFind == null)
            {
                var NewTileData = new FlipTileData()
                {
                    BackgroundImage = new Uri("/Assets/Tiles/Collage.png", UriKind.Relative),
                };

                ShellTile.Create(new Uri("/Views/CollagePage.xaml", UriKind.Relative), NewTileData, true);
            }
            else
            {
                MessageBox.Show("Have pined!");
            }
        }

        private void MenuItemPinAffect_Click(object sender, RoutedEventArgs e)
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("AffectPage"));

            // Create the Tile if we didn't find that it already exists.
            if (TileToFind == null)
            {
                var NewTileData = new FlipTileData()
                {
                    BackgroundImage = new Uri("/Assets/Tiles/Affect.png", UriKind.Relative),
                };

                ShellTile.Create(new Uri("/Views/AffectPage.xaml", UriKind.Relative), NewTileData, true);
            }
            else
            {
                MessageBox.Show("Have pined!");
            }
        }

        private void MenuItemPinPaint_Click(object sender, RoutedEventArgs e)
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("EditBasePage.xaml"));

            // Create the Tile if we didn't find that it already exists.
            if (TileToFind == null)
            {
                var NewTileData = new StandardTileData()
                {
                    BackgroundImage = new Uri("/Assets/Tiles/Paint.png", UriKind.Relative),
                };

                ShellTile.Create(new Uri("/Views/EditBasePage.xaml", UriKind.Relative), NewTileData);
            }
            else
            {
                MessageBox.Show("Have pined!");
            }
        }

        private void MenuItemPinClassical_Click(object sender, RoutedEventArgs e)
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("/OLD/MainPage.xaml"));

            // Create the Tile if we didn't find that it already exists.
            if (TileToFind == null)
            {
                var NewTileData = new StandardTileData()
                {
                    BackgroundImage = new Uri("/Assets/Tiles/OLDLogo.jpg", UriKind.Relative),
                };

                ShellTile.Create(new Uri("/OLD/MainPage.xaml", UriKind.Relative), NewTileData);
            }
            else
            {
                MessageBox.Show("Have pined!");
            }
        }

        private void ReviewMe()
        {
            if (AppSession.Instance.LicenseModel.HadReviewed || AppSession.Instance.LicenseModel.ExportJPGCount < 3 || AppSession.Instance.LicenseModel.ExportJPGCount % 2 != 1)
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

        private void btnMoreApps_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceSearchTask searchTask = new MarketplaceSearchTask();
            searchTask.ContentType = MarketplaceContentType.Applications;
            searchTask.SearchTerms = "Yu-weiz";
            searchTask.Show();
        }


        #region  Bug捕捉与发送
        /// <summary>
        /// 异常时不能保存，故此方法无效
        /// </summary>
        private void ErrorsCatchAndSend()
        {
            if (string.IsNullOrEmpty(AppSession.Instance.SettingsModel.Errors))
            {
                return;
            }

            #region 获取版本号
            var assemblyInfo = new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName);
            Version version = assemblyInfo.Version;
            string subject = assemblyInfo.Name + "-" + assemblyInfo.Version.ToString() + "：Errors";
            #endregion

            #region 发送邮件
            string message = "It seems appear some errors last time!Do you want to  send it in an E-mail to us to fix them?\r\nThank you!";
            string title = "Hellp us";

            if (this.IsChineseCulture)
            {
                message = "上次启动似乎出现了一点问题，你能用邮件发送给我们进行修复吗?\r\n谢谢！";
                title = "帮助我们";
            }

            MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Microsoft.Phone.Tasks.EmailComposeTask task = new Microsoft.Phone.Tasks.EmailComposeTask();
                task.To = "Yu-weiz@hotmail.com";
                task.Subject = subject;
                task.Body = AppSession.Instance.SettingsModel.Errors;
                task.Show();
            }
            #endregion

            AppSession.Instance.SettingsModel.Errors = null;
        }

        /// <summary>
        /// 获取当前系统是否中文
        /// </summary>
        private bool IsChineseCulture
        {
            get
            {
                return (System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN" || System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-HK");
            }
        }
        #endregion
    }
}