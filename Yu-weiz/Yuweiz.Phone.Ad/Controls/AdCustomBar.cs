using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Yuweiz.Phone.Ad.Model;
using Yuweiz.Phone.Controls;
using Microsoft.Phone.Tasks;
using Yuweiz.Phone.Net;
using System.Windows.Media.Imaging;
using System.Threading;
using Yuweiz.Phone.Common.FreeJson;

namespace Yuweiz.Phone.Ad.Controls
{
    /// <summary>
    /// 自定义广告
    /// </summary>
    public class AdCustomBar : XControl<AdCustomBar>
    {
        public AdCustomBar()
            : base(new Uri("/Yuweiz.Phone.Ad;component/themes/AdCustomBar.xaml", UriKind.Relative))
        {
            AdClientName = "SketchBoard";
            AdPositionID = "1";
        }

        public string AdClientName { get; set; }

        public string AdPositionID { get; set; }

        /// <summary>
        /// 毫秒
        /// </summary>
        public int AdInterval { get; set; }

        protected string RequestParameter
        {
            get
            {
                if (string.IsNullOrEmpty(AdClientName) || string.IsNullOrEmpty(AdPositionID))
                {
                    return string.Empty;
                }
                else
                {
                    return AdClientName + AdPositionID;
                }
            }
        }

        protected Grid LayoutRoot { get; set; }

        protected TextBlock TextBlockWords { get; set; }

        protected AdCustomModelAbstract AdCustomModelAbstract { get; set; }

        protected override void OnApplyXTemplate()
        {
            base.OnApplyXTemplate();

            this.LayoutRoot = this.GetTemplateChild("LayoutRoot") as Grid;
            this.TextBlockWords = this.GetTemplateChild("TextBlockWords") as TextBlock;

            this.LayoutRoot.Background = null;
            this.Background = null;
        }

        public void StartRequestAd()
        {
            if (AdInterval > 0)
            {
                Action action = () =>
                {
                    while (AdInterval > 0)
                    {
                        this.RequestAd(AdClientName, AdPositionID);
                        Thread.Sleep(AdInterval);
                    }
                };

                Thread thread = new Thread(new ThreadStart(action));
                thread.Start();
            }
            else
            {
                this.RequestAd(AdClientName, AdPositionID);
            }

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void InitializeAd(AdCustomModelAbstract model)
        {
            this.AdCustomModelAbstract = model;

            if (model == null)
            {
                return;
            }

            if (model is ImageAdCustomModel)
            {
                ImageAdCustomModel adModel = model as ImageAdCustomModel;
                if (adModel.BackgroundImage != null)
                {
                    this.LayoutRoot.Background = new ImageBrush() { ImageSource = adModel.BackgroundImage, Stretch = Stretch.Fill };
                    this.TextBlockWords.Visibility = Visibility.Collapsed;
                }
            }
            else if (model is TextAdCustomModel)
            {
                TextAdCustomModel adModel = model as TextAdCustomModel;
                if (adModel.BackgroundColor != null)
                {
                    this.LayoutRoot.Background = new SolidColorBrush(adModel.BackgroundColor.Value);
                }

                this.TextBlockWords.Visibility = Visibility.Visible;
                this.TextBlockWords.Text = adModel.AdWords;
            }

            this.Tap += AdCustomBar_Tap;
        }

        /// <summary>
        /// 请求指广告位的广告
        /// </summary>
        /// <param name="AdPosition"></param>
        private void RequestAd(string adClientName, string adPositionID)
        {
            if (string.IsNullOrEmpty(adPositionID) || string.IsNullOrEmpty(adClientName))
            {
                return;
            }

            var http = new HttpRequest();
            string requestParameter = "Uid=" + ADServiceConfig.Uid + "&ClientName=" + adClientName + "&PositionId=" + adPositionID + "&RequestType=1";
            http.StartRequest(ADServiceConfig.AdServiceHost + requestParameter,
                (resultData, result) =>
                {
                    if (!result || string.IsNullOrEmpty(resultData))
                    {
                        return;
                    }
                    //处理Json
                    object model = JsonUtility.Json2Obj(resultData, typeof(AdDataModel));
                    AdCustomModelAbstract adCustomModel = AdModelFactory.Intsance.CreateAdCustomModel(model as AdDataModel);
                    this.InitializeAd(adCustomModel);
                });
        }

        private void AdCustomBar_Tap(object sender, GestureEventArgs e)
        {
            if (AdCustomModelAbstract == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(AdCustomModelAbstract.TargetAppID))
            {
                MarketplaceDetailTask detailTask = new MarketplaceDetailTask();
                detailTask.ContentIdentifier = this.AdCustomModelAbstract.TargetAppID;
                detailTask.Show();
            }
            else
            {
                WebBrowserTask webTask = new WebBrowserTask();
                webTask.Uri = new Uri(AdCustomModelAbstract.HttpUrl);
                webTask.Show();
            }
        }

    }
}
