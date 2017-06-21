using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Yuweiz.Phone.Diagnostics
{
    /// <summary>
    /// 获取当前分辨率
    /// </summary>
    public static class ResolutionHelper
    {
        private static bool IsWvga
        {
            get
            {
                return Application.Current.Host.Content.ScaleFactor == 100;
            }
        }

        private static bool IsWxga
        {
            get
            {
                return Application.Current.Host.Content.ScaleFactor == 160;
            }
        }

        private static bool IsHD
        {
            get
            {
                return Application.Current.Host.Content.ScaleFactor == 150;
            }
        }

        /// <summary>
        /// 默认：WVGA
        /// </summary>
        public static ResolutionsEnum CurrentResolution
        {
            get
            {
                if (IsWvga) return ResolutionsEnum.WVGA;
                else if (IsWxga) return ResolutionsEnum.WXGA;
                else if (IsHD) return ResolutionsEnum.HD;
                //else throw new InvalidOperationException("Unknown resolution");
                else
                {
                    return ResolutionsEnum.WVGA; 
                }
            }
        }
    }
}
