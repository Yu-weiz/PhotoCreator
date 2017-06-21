using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPPhotoEditor.Models;
using Yuweiz.Phone.Common.OperationHistory;
using Yuweiz.Phone.Common.AppStructure;

namespace WPPhotoEditor
{
    /// <summary>
    /// 应用程序状态及用户会话缓存、全局变量存储类
    /// </summary>
    /// <creator>庾伟荣</creator>
    public partial class AppSession : AppSessionAbstract
    {
        private AppSession():base()
        {
            this.SavedScale = 2;
            this.CanvasBackground = new SolidColorBrush() { Color = Colors.White };

            this.HistoryManager = new HistoryManager();
            this.SettingsModel = Yuweiz.Phone.IO.IsolatedStorageSettingsDAL.OpenSettingsModel<SettingsModel>();

            #region 获取程序集版本
            Version version = new System.Reflection.AssemblyName(
            System.Reflection.Assembly.GetExecutingAssembly().FullName).Version;
            this.Version = version.Major.ToString() + "." + version.Minor.ToString();
            #endregion
        }

        public static readonly AppSession Instance = new AppSession();
        


        public override void UpdateAppSession()
        {
            var license = new Microsoft.Phone.Marketplace.LicenseInformation();
            AppSession.IsTrial = license.IsTrial();
            //AppSession.IsTrial = true;

            //是否显示广告
            IsShowAds = !Common.LicenseManage.NoAdvertisingLicense();

            base.UpdateAppSession();
        }

        public override void SaveAppSession()
        {
            base.SaveAppSession();
        }


        public  bool IsShowAds { get; set; }

        public  HistoryManager HistoryManager { get; set; }

        /// <summary>
        /// 当前跨页面处理的图像
        /// </summary>
        public  WriteableBitmap CurEditingBitmapImage { get; set; }

        public  string Version { get; set; }

        /// <summary>
        /// 应用存储用户设置数据库模型对象
        /// </summary>
        public  SettingsModel SettingsModel { get; set; }

        /// <summary>
        /// 画板背景画刷
        /// </summary>
        public  Brush CanvasBackground { get; set; }

        /// <summary>
        /// 画板保存的图片缩放比例
        /// </summary>
        public  double SavedScale { get; set; }

        public Color? ChosenColor { get; set; }

        public static void ShowToastMessage(string msg, string title = null, string background = null, string foreground = null)
        {
            Coding4Fun.Toolkit.Controls.ToastPrompt toast = new Coding4Fun.Toolkit.Controls.ToastPrompt();
            toast.MillisecondsUntilHidden = 2000;
            toast.Message = msg;

            if (!string.IsNullOrEmpty(background))
            {
                toast.Background = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16(background));
            }

            if (!string.IsNullOrEmpty(foreground))
            {
                toast.Foreground = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16(foreground));
            }

            if (!string.IsNullOrEmpty(title))
            {
                toast.Title = title;
            }

            toast.Show();
        }
    }
}
