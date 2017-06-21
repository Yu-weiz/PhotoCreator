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
using Yuweiz.Phone.Net;
using Yuweiz.Phone.Ad.Model;

namespace WPPhotoEditor.Views
{
    public partial class OptionPage : PhoneApplicationPage
    {
        public OptionPage()
        {
            InitializeComponent();

            this.btnChange.Click += btnChange_Click;
            this.btnBuy.Click += btnBuy_Click;
            this.btnRevmoeAds.Click += btnRevmoeAds_Click;
            this.btnReview.Click += btnReview_Click;
            this.btnOKRemove.Click += btnOKRemove_Click;
            this.tMoreApps.Tap += tMoreApps_Tap;
            this.btnWeAreHere.Click+=btnWeAreHere_Click;
        }


        void btnWeAreHere_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webTask = new WebBrowserTask();
            string url = AppSession.Instance.IsChineseCulture ? "http://www.weibo.com/5130298741" : "http://www.twitter.com/yu_weiz";
            webTask.Uri = new Uri(url);
            webTask.Show();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //购买信息
            this.tDueDate.Text = "License Code";
            DateTime validDateBegin = new DateTime();
            if (DateTime.TryParse(AppSession.Instance.LicenseModel.ValidDateBegin, out validDateBegin))
            {
                DateTime validDateEnd = validDateBegin.AddDays(AppSession.Instance.LicenseModel.ValidDays);
                this.tDueDate.Text = "Due:" + validDateEnd.ToString("yyyy-MM-dd HH:mm:ss");
            }
            //-------

            #region 更改背景后，返回主页面
            if (this.hadChange && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }

            this.hadChange = false;
            #endregion
        }

        void tMoreApps_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceSearchTask searchTask = new MarketplaceSearchTask();
            searchTask.ContentType = MarketplaceContentType.Applications;
            searchTask.SearchTerms = "Yu-weiz";
            searchTask.Show();
        }

        void btnOKRemove_Click(object sender, RoutedEventArgs e)
        {
            this.btnOKRemove.IsEnabled = false;
            this.proBarRemoving.Visibility = Visibility.Visible;
            var http = new HttpRequest();
            string requestParameter = "ClientName=" + AppSession.Instance.AdUltimateModel1.ClientName + "&OrderNumber=" + this.txtOrderNumber.Text + "";
            http.StartRequest(AppSession.AdServiceHost + requestParameter,
                (resultData, result) =>
                {
                    this.btnOKRemove.IsEnabled = true;
                    this.proBarRemoving.Visibility = Visibility.Collapsed;
                    if (!result || string.IsNullOrEmpty(resultData))
                    {
                        MessageBox.Show(WPPhotoEditor.Resources.AppResources.OPNetWorkErrors);
                        return;
                    }
                    //处理Json
                    object model =Yuweiz.Phone.Common.FreeJson.JsonUtility.Json2Obj(resultData, typeof(AdOrderDataModel));
                    AdOrderModel adOrderModel = new AdOrderModel(model as AdOrderDataModel);
                    if (adOrderModel.IsAuthorization)
                    {
                        #region
                        DateTime validDateBegin = new DateTime();
                        if (DateTime.TryParse(AppSession.Instance.LicenseModel.ValidDateBegin, out validDateBegin))
                        {
                            //如果之前已购买过
                            DateTime validDateEnd = validDateBegin.AddDays(AppSession.Instance.LicenseModel.ValidDays);
                            if (DateTime.Compare(DateTime.Now, validDateEnd) <= 0)
                            {
                                //未过期
                                AppSession.Instance.LicenseModel.ValidDays += adOrderModel.ValidDays;
                            }
                            else
                            {
                                //已过期
                                AppSession.Instance.LicenseModel.ValidDateBegin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                AppSession.Instance.LicenseModel.ValidDays = adOrderModel.ValidDays;
                            }

                        }
                        else
                        {
                            AppSession.Instance.LicenseModel.ValidDateBegin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            AppSession.Instance.LicenseModel.ValidDays = adOrderModel.ValidDays;
                        }
                        #endregion

                        MessageBox.Show(adOrderModel.Message, WPPhotoEditor.Resources.AppResources.OPTakeEffectAfterRestart, MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show(adOrderModel.Message);
                    }
                });
        }

        void btnReview_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask reviewTask = new MarketplaceReviewTask();
            reviewTask.Show();
        }

        void btnRevmoeAds_Click(object sender, RoutedEventArgs e)
        {
            //  throw new NotImplementedException();
        }

        void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceDetailTask detailTask = new MarketplaceDetailTask();
            detailTask.ContentIdentifier = "dd07c6c4-19cc-47c4-b92d-4ce26110f872";
            detailTask.ContentType = MarketplaceContentType.Applications;
            detailTask.Show();
        }

        bool hadChange = false;

        void btnChange_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += (senderP, eP) =>
            {
                if (eP.TaskResult == TaskResult.OK)
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.SetSource(eP.ChosenPhoto); //获取返回的图片     
                    WriteableBitmap wbmpBackground = new WriteableBitmap(bmp);

                    Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.SavePicture(wbmpBackground, AppSession.MainPageBackgroundPath);
                    AppSession.Instance.SettingsModel.MainPageBackgroundPath = AppSession.MainPageBackgroundPath;

                    hadChange = true;

                }
            };
            photoChooserTask.Show();
        }
    }
}