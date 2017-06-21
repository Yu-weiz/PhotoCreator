using AdDuplex;
using GoogleAds;
using SOMAWP8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Yuweiz.Phone.Ad.Model;
using Yuweiz.Phone.Common.FreeJson;
using Yuweiz.Phone.Controls;
using Yuweiz.Phone.Net;

namespace Yuweiz.Phone.Ad.Controls
{
    public class AdUltimateBar : XControl<AdUltimateBar>
    {
        public AdUltimateBar()
            : base(new Uri("/Yuweiz.Phone.Ad;component/themes/AdUltimateBar.xaml", UriKind.Relative))
        {
            this.AdUltimateModel = new AdUltimateModel();
            this.ShowAdFromEnum = AdSourceTypeEnum.Original;
        }

        protected StackPanel LayoutRoot { get; set; }

        public AdUltimateModel AdUltimateModel { get; set; }


        private AdSourceTypeEnum ShowAdFromEnum;

        private int? adInterval;


        protected override void OnApplyXTemplate()
        {
            base.OnApplyXTemplate();

            this.LayoutRoot = this.GetTemplateChild("LayoutRoot") as StackPanel;

            this.LayoutRoot.Background = null;
            this.Background = null;
        }

        /// <summary>
        /// 开始广告
        /// </summary>
        public void StartAds()
        {
            this.LayoutRoot.Children.Clear();

            if (this.AdUltimateModel.ShowOriginalAdFirst && this.AdUltimateModel.OriginalAd != null)
            {
                AdCustomBar adCustomBar = new AdCustomBar();
                adCustomBar.InitializeAd(this.AdUltimateModel.OriginalAd);
                this.LayoutRoot.Children.Add(adCustomBar);
            }

            this.ShowAdFromEnum = this.AdUltimateModel.DefaultAdFromEnum;
            this.RequestAdFrom(this.AdUltimateModel.ClientName, this.AdUltimateModel.PositionId);
        }

        /// <summary>
        /// 请求指广告位的广告
        /// </summary>
        /// <param name="AdPosition"></param>
        private void RequestAdFrom(string adClientName, string adPositionID)
        {
            if (string.IsNullOrEmpty(adPositionID) || string.IsNullOrEmpty(adClientName))
            {
                this.InitializeAd(this.AdUltimateModel.DefaultAdFromEnum);
                return;
            }

            string requestParameter = "Uid=" + ADServiceConfig.Uid + "&ClientName=" + adClientName + "&PositionId=" + adPositionID + "&RequestType=0";
            var http = new HttpRequest();
            http.StartRequest(ADServiceConfig.AdServiceHost + requestParameter,
                (resultData, result) =>
                {
                    if (!result || string.IsNullOrEmpty(resultData))
                    {
                        this.InitializeAd(this.AdUltimateModel.DefaultAdFromEnum);
                        return;
                    }

                    //处理Json
                    object model = JsonUtility.Json2Obj(resultData, typeof(AdSourceTypeDataModel));
                    AdSourceTypeDataModel adFromDataModel = model as AdSourceTypeDataModel;

                    AdSourceTypeModel adSourceTypeModel = new AdSourceTypeModel(adFromDataModel);
                    this.adInterval = adSourceTypeModel.AdInterval;
                    this.InitializeAd(adSourceTypeModel);

                });
        }



        /// <summary>
        /// 初始化
        /// </summary>
        protected void InitializeAd(AdSourceTypeModel adSourceTypeModel)
        {
            AdSourceTypeEnum adFromEnum = adSourceTypeModel.BDSourceType;
            if (adFromEnum == AdSourceTypeEnum.Default)
            {
                adFromEnum = this.AdUltimateModel.DefaultAdFromEnum;
            }

            InitializeAd(adFromEnum);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected void InitializeAd(AdSourceTypeEnum adFromEnum)
        {
            if (AdUltimateModel == null)
            //if (AdUltimateModel == null || (this.ShowAdFromEnum == adFromEnum && this.ShowAdFromEnum != AdSourceTypeEnum.Original))
            {
                return;
            }

            if (adFromEnum == AdSourceTypeEnum.Default)
            {
                this.SwitchAd(this.AdUltimateModel.DefaultAdFromEnum);
            }
            else if (adFromEnum == AdSourceTypeEnum.None)
            {
                this.LayoutRoot.Children.Clear();
            }
            else
            {
                this.SwitchAd(adFromEnum);
            }

            this.ShowAdFromEnum = adFromEnum;
        }

        private void SwitchAd(AdSourceTypeEnum adFromEnum)
        {
            switch (adFromEnum)
            {
                case AdSourceTypeEnum.Adduplex: this.CreateAdduplex(); break;
                case AdSourceTypeEnum.Custom: this.CreateAdCustomBar(); break;
                case AdSourceTypeEnum.Smaato: this.CreateSmaatoAd(); break;
                case AdSourceTypeEnum.Pubcenter: this.CreatePubcenter(); break;
                case AdSourceTypeEnum.Original: this.CreateOriginalAd(); break;
                case AdSourceTypeEnum.GoogleAdMob: this.CreateAdMob(); break;
            }
        }

        private void CreateSmaatoAd()
        {
            SomaAdViewer somaAd = new SomaAdViewer();
            somaAd.ShowErrors = this.AdUltimateModel.ShowSmaatoError;
            somaAd.AdInterval = 30000;
            somaAd.PopupAd = true;
            somaAd.PopupAdDuration = 20000;
            somaAd.Adspace = this.AdUltimateModel.SmaatoAdSpace;
            somaAd.Pub = ADServiceConfig.SmaatoPublisherID;

            somaAd.StartAds();
            this.LayoutRoot.Children.Insert(0, somaAd);
        }

        private void CreateAdduplex()
        {
            AdControl adControl = new AdControl();
            adControl.AppId = this.AdUltimateModel.AdduplexAppId;
            adControl.Background = null;
            adControl.BackgroundOpacity = 0;
            this.LayoutRoot.Children.Insert(0, adControl);
        }

        private void CreatePubcenter()
        {
            Microsoft.Advertising.Mobile.UI.AdControl adControl = new Microsoft.Advertising.Mobile.UI.AdControl();
            adControl.Width = 480;
            adControl.Height = 80;
            adControl.ApplicationId =this.AdUltimateModel.PubCenterAppID;
            adControl.AdUnitId = this.AdUltimateModel.PubCenterADUnitID;
            this.LayoutRoot.Children.Insert(0, adControl);
        }
        
        private void CreateAdCustomBar()
        {
            AdCustomBar adCustomBar = new AdCustomBar();
            adCustomBar.AdClientName = this.AdUltimateModel.ClientName;
            adCustomBar.AdPositionID = this.AdUltimateModel.PositionId;

            if (this.adInterval != null)
            {
                adCustomBar.AdInterval = this.adInterval.Value;
            }

            adCustomBar.StartRequestAd();
            this.LayoutRoot.Children.Insert(0, adCustomBar);
        }

        private void CreateOriginalAd()
        {
            if (this.AdUltimateModel.OriginalAd != null)
            {
                AdCustomBar adCustomBar = new AdCustomBar();
                adCustomBar.InitializeAd(this.AdUltimateModel.OriginalAd);
                this.LayoutRoot.Children.Add(adCustomBar);
            }
        }

        private void CreateAdMob()
        {
            AdView bannerAd = new AdView
            {
                Format = AdFormats.Banner,
                AdUnitID = this.AdUltimateModel.GoogleAdMobAdUnitID
            };          
            LayoutRoot.Children.Add(bannerAd);
            AdRequest adRequest = new AdRequest();
            if (string.IsNullOrEmpty(this.AdUltimateModel.GoogleAdMobAdUnitID))
            {
                adRequest.ForceTesting = true;
            }           
            bannerAd.LoadAd(adRequest);
        }

      
    }
}
