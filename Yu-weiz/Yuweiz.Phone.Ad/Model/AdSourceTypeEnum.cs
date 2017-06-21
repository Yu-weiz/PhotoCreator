using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Ad.Model
{
    /// <summary>
    /// 要显示的广告源
    /// </summary>
    public enum AdSourceTypeEnum
    {
        Default=0,

        /// <summary>
        /// Smaato广告
        /// </summary>
        Smaato=1,

        /// <summary>
        /// https://pubcenter.microsoft.com/Dashboard
        /// </summary>
        Pubcenter = 2,

        /// <summary>
        /// Adduplex交换广告
        /// </summary>
        Adduplex=3,

        /// <summary>
        /// GoogleAdMob
        /// </summary>
        GoogleAdMob=4,

        /// <summary>
        /// 自定义广告
        /// </summary>
        Custom = 10,

        /// <summary>
        /// 原始的断网广告（七个0）
        /// </summary>
        Original = 10000000,

        /// <summary>
        /// 无广告（六个0）
        /// </summary>
        None=10000001
    }
}
