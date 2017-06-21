using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common.AppStructure
{
    /// <summary>
    /// 应用会话状态基类，包含应用配置信息及应用使用到的全局变量
    /// </summary>
    public abstract class AppSessionAbstract
    {
        protected AppSessionAbstract()
        {
            LicenseModel = Yuweiz.Phone.IO.IsolatedStorageSettingsDAL.OpenSettingsModel<LicenseModel>();           
        }

        public event Action AppLeaving;

        public event Action AppEnter;

        /// <summary>
        /// 应用刚打开或激活时自动执行
        /// 并引发AppEnter事件
        /// 一般后调用 base
        /// </summary>
        public virtual void UpdateAppSession()
        {
            if (AppEnter != null)
            {
                AppEnter();
            }
        }

        /// <summary>
        /// 应用离开时自动执行,
        /// 并引发AppLeaving事件
        /// 一般先调用base
        /// </summary>
        public virtual void SaveAppSession()
        {
            if (AppLeaving != null)
            {
                AppLeaving();
            }

            Yuweiz.Phone.IO.IsolatedStorageSettingsDAL.SaveSettingsModel<LicenseModel>(LicenseModel);
        }

        /// <summary>
        /// 版本许可证信息
        /// </summary>
        public LicenseModel LicenseModel { get; set; }



    }
}
