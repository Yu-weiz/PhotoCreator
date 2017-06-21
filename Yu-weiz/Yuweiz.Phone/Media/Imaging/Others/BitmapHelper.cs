using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Media.Imaging.Others
{
    public class BitmapHelper
    {
        /// <summary>
        /// 生成缩略图到新的图像
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="newBitmap"></param>
        public static void ResizeNearestNeighbour(BitmapBase bitmap, BitmapBase newBitmap)
        {
            int width = newBitmap.Width;
            int height = newBitmap.Height;
            int num3 = bitmap.Width;
            int num4 = bitmap.Height;
            int[] pixels = bitmap.Pixels;
            int[] numArray2 = newBitmap.Pixels;
            double num5 = ((double)num3) / ((double)width);
            double num6 = ((double)num4) / ((double)height);
            double num8 = 0.0;
            int index = 0;
            for (int i = 0; i < height; i++)
            {
                int num11 = (int)num8 * num3;
                double num7 = 0.0;
                for (int j = 0; j < width; j++)
                {
                    numArray2[index] = pixels[num11 + (int)num7];
                    num7 += num5;
                    index++;
                }
                num8 += num6;
            }
        }
    }
}
