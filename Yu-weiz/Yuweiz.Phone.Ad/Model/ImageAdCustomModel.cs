using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Ad.Model
{
    public class ImageAdCustomModel : AdCustomModelAbstract
    {     
        /// <summary>
        /// 如果BackgroundColor，BackgroundImage同时存在，则取BackgroundImage
        /// </summary>
        public BitmapImage BackgroundImage { get; set; }

    }
}
