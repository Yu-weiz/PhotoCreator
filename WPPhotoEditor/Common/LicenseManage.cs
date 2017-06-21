using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yuweiz.Phone.Common.AppStructure;

namespace WPPhotoEditor.Common
{
    public class LicenseManage
    {
        private const int ExportPictureMaxLimit =6;

        /// <summary>
        /// 导出图片授权
        /// ：保存次数授权
        /// </summary>
        /// <returns></returns>
        public static bool CanExportLicense()
        {
            bool licensed = false;
            switch (AppSession.LicenseTypeEnum)
            {
                case LicenseTypeEnum.InternationalPay:
                    {
                        /// 国际收费：已付：全开,未付仅得导出六次；                       
                        if (AppSession.IsTrial)
                        {
                            if (AppSession.Instance.LicenseModel.ExportJPGCount >= ExportPictureMaxLimit)
                            {
                                MessageBoxResult mbResult = MessageBox.Show("Buy Now!", "Like it?", MessageBoxButton.OKCancel);
                                if (mbResult == MessageBoxResult.OK)
                                {
                                    Microsoft.Phone.Tasks.MarketplaceDetailTask marketplaceDetailTask = new Microsoft.Phone.Tasks.MarketplaceDetailTask();
                                    marketplaceDetailTask.Show();
                                }
                                licensed = false;
                            }
                            else
                            {
                                licensed = true;
                            }
                        }
                        else
                        {
                            licensed = true;
                        }
                        break;
                    }
                case LicenseTypeEnum.InternationalAd:
                    {
                        /// 国际免费：全开；
                        licensed = true;
                        break;
                    }
            }


            #region  检查购买信息
            if (!licensed)
            {
                licensed = GetIsAuthorized();
            }
            #endregion

            return licensed;
        }

        /// <summary>
        /// 关闭广告授权
        /// </summary>
        /// <returns></returns>
        public static bool NoAdvertisingLicense()
        {
            bool licensed = false;
            switch (AppSession.LicenseTypeEnum)
            {
                case LicenseTypeEnum.InternationalPay:
                    {
                        /// 国际收费：永远关闭广告；                
                        licensed = true;
                        break;
                    }
                case LicenseTypeEnum.InternationalAd:
                    {
                        /// 国际免费：永远显示广告；
                        licensed = false;
                        break;
                    }
            }


            #region  检查购买信息
            if (!licensed)
            {
                licensed = GetIsAuthorized();
            }
            #endregion

            return licensed;
        }

        /// <summary>
        /// 检查购买信息
        /// </summary>
        /// <returns></returns>
        private static bool GetIsAuthorized()
        {
            if (AppSession.LicenseTypeEnum == LicenseTypeEnum.CNPay)
            {
                return false;
            }

            bool licensed = false;
            DateTime validDateBegin = new DateTime();
            if (DateTime.TryParse(AppSession.Instance.LicenseModel.ValidDateBegin, out validDateBegin))
            {
                DateTime validDateEnd = validDateBegin.AddDays(AppSession.Instance.LicenseModel.ValidDays);
                if (DateTime.Compare(DateTime.Now, validDateEnd) <= 0)
                {
                    licensed = true;
                }
            }

            return licensed;
        }
    }
}
