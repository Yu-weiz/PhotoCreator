using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Ad.Model
{
    public abstract class AdCustomModelAbstract
    {
        private string targetUrl;

        /// <summary>
        /// 包含http，则从IE打开，否则检查是否APP GUID
        /// </summary>
        public string TargetUrl
        {
            get { return targetUrl; }
            set 
            {
                targetUrl = value;
                if (!string.IsNullOrEmpty(value) && value.Length > 4 && value.Substring(0, 4).ToLower() == "http")
                {
                    this.HttpUrl = value;
                }
                else
                {
                    this.TargetAppID = value;
                }
            }
        }

        public string HttpUrl { get;protected set; }

        public string TargetAppID { get; protected set; }
    }
}
