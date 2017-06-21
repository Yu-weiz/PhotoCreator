using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common.AppStructure
{
    /// <summary>
    /// 授权信息模型
    /// </summary>
    public class LicenseModel
    {
        /// <summary>
        /// 上次使用的版本号
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// 导出JPG次数
        /// </summary>
        public int ExportJPGCount
        {
            get;
            set;
        }

        /// <summary>
        /// 导出GIF次数
        /// </summary>
        public int ExportGIFCount
        {
            get;
            set;
        }

        /// <summary>
        /// 导出GIF次数
        /// </summary>
        public int NewFileCount
        {
            get;
            set;
        }

        public int DownLoadSketchCount { get; set; }

        public string ValidDateBegin { get; set; }

        public int ValidDays { get; set; }

        public bool HadReviewed { get; set; }
    }
}
