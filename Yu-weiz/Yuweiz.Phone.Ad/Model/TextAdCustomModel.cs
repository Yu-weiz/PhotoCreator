using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Yuweiz.Phone.Ad.Model
{
    public class TextAdCustomModel:AdCustomModelAbstract
    {
        /// <summary>
        /// 如果BackgroundColor，BackgroundImage同时存在，则取BackgroundImage
        /// </summary>
        public Color? BackgroundColor { get; set; }

        public Color? ForegroundColor { get; set; }

        public string AdWords { get; set; }
    }
}
