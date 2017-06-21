using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Media.Imaging.Others
{
    public abstract class BitmapBase
    {
        public int Height;
        public int[] Pixels;
        public int Width;

        protected BitmapBase()
        {
        }

        public abstract BitmapBase Clone();
        public abstract BitmapBase Create(int width, int height);
        public abstract BitmapBase CreateThumbnail(int boundingWidth, int boundingHeight);
    }
}
