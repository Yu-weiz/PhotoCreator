using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Yuweiz.Phone.Ad.Model;
using Yuweiz.Phone.Common.AppStructure;
using Yuweiz.Phone.Common.OperationHistory;

namespace WPPhotoEditor
{
    /// <summary>
    /// 应用程序配置的常量Consts或运行时常量ReadOnly
    /// </summary>
    /// <creator>庾伟荣</creator>
    public partial class AppSession : AppSessionAbstract
    {
        static AppSession()
        {
            switch (LicenseTypeEnum)
            {
               // case Yuweiz.Phone.Common.AppStructure.LicenseTypeEnum.CNPay: UmengAppKey = "5370660c56240b5eb7052a5a"; break;
                case Yuweiz.Phone.Common.AppStructure.LicenseTypeEnum.InternationalAd: UmengAppKey = "537065f356240b68040ea2d7"; break;
                case Yuweiz.Phone.Common.AppStructure.LicenseTypeEnum.InternationalPay: UmengAppKey = "5370660c56240b5eb7052a5a"; break;
                default:UmengAppKey = "537065f356240b68040ea2d7"; break;
            }       
        }
        
        #region 颜色
        /// <summary>
        /// 橙红：APP主色调
        /// </summary>
        public static readonly Color MainColor = Color.FromArgb(255, 255, 152, 13);

        /// <summary>
        /// 蓝色：APP次色调
        /// </summary>
        public static readonly Color SecondaryColor = Color.FromArgb(255, 2, 178, 249);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Color LightGrap = Color.FromArgb(255, 75, 75, 75);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Color LightWhite = Color.FromArgb(255, 220, 220, 220);

        #endregion

        /// <summary>
        /// 启动页背景图默认存储名称
        /// </summary>
        public const string MainPageBackgroundPath = "UserStartupPageBackgroundPath";

        /// <summary>
        /// 画刷样式总数
        /// </summary>
        public const int BrushPNGCount = 15;

        /// <summary>
        /// 图片特效缓存文件路径
        /// </summary>
        public const string TempAffectBitmapPath = "tempEffectOriginal.jpg";

        public const string AdServiceHost = @"http://api.sketchtouch.com/ADServiceHandler.ashx?";

        public static readonly string UmengAppKey;
        
        public static readonly LicenseTypeEnum LicenseTypeEnum = LicenseTypeEnum.InternationalAd;

        public static bool IsTrial
        {
            get;
            set;
        }

        private AdUltimateModel adUltimateModel = new AdUltimateModel() 
        { 
            ClientName =IsChinese?"PhotoEditorZH":"PhotoEditor",
            SmaatoAdSpace = 130087634,
            AdduplexAppId = "95599",
            InMobiAppId = "9dc0a307ce864e4bb6aba408424bf474",
            PubCenterAppID = "f56c5855-41af-4ed5-9bde-32866a873e85",
            PubCenterADUnitID = "211997",
          //OriginalAd = new ImageAdCustomModel() { BackgroundImage = new BitmapImage(new Uri("/Assets/Image/SketchBoardADBar.jpg", UriKind.RelativeOrAbsolute)), TargetUrl = "afbce866-a0fa-4729-b613-c5a5e3dc7e16" } 
        };

        //LayerBox
        public AdUltimateModel AdUltimateModel0
        {
            get
            {
                AdUltimateModel adUltimateModel = this.adUltimateModel;
                adUltimateModel.PositionId = "1";
                adUltimateModel.GoogleAdMobAdUnitID = "ca-app-pub-7377266310690049/4017078210";
                adUltimateModel.DefaultAdFromEnum = AdSourceTypeEnum.Smaato;
                return adUltimateModel;
            }
        }

        //LayerBox
        public AdUltimateModel AdUltimateModel1
        {
            get 
            {
                AdUltimateModel adUltimateModel = this.adUltimateModel;
                adUltimateModel.PositionId = "1";
                adUltimateModel.DefaultAdFromEnum = AdSourceTypeEnum.Smaato;
                return adUltimateModel;
            }
        }

        //LayerToolBar
        public AdUltimateModel AdUltimateModel2
        {
            get
            {
                AdUltimateModel adUltimateModel = this.adUltimateModel.Clone();
                adUltimateModel.PositionId = "2";
                adUltimateModel.PubCenterADUnitID = "211997";
                adUltimateModel.DefaultAdFromEnum = AdSourceTypeEnum.Smaato;
                return adUltimateModel;
            }
        }

        //AffectPage
        public AdUltimateModel AdUltimateModel3
        {
            get
            {
                AdUltimateModel adUltimateModel = this.adUltimateModel.Clone();
                adUltimateModel.PositionId = "3";
                adUltimateModel.DefaultAdFromEnum = AdSourceTypeEnum.Smaato;
                return adUltimateModel;
            }
        }

        //BrushPickerPage
        public AdUltimateModel AdUltimateModel4
        {
            get
            {
                AdUltimateModel adUltimateModel = this.adUltimateModel.Clone();
                adUltimateModel.PositionId = "4";
                adUltimateModel.DefaultAdFromEnum = AdSourceTypeEnum.Smaato;
                return adUltimateModel;
            }
        }

        /// </summary>
        public  bool IsChineseCulture
        {
            get
            {
                return (System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN" || System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-HK");
            }
        }

        public static bool IsChinese
        {
            get
            {
                return (System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN" || System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-HK");
            }
        }
    }
}
