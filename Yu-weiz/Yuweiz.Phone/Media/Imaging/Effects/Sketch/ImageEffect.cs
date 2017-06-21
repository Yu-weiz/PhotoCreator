using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Yuweiz.Phone.Media.Imaging.Effects.Sketch
{
    public abstract class ImageEffect
    {
        protected ImageEffect()
        {
        }

        internal static int GrayScaleToBlueComponent(int source)
        {
            return (((((((source >> 0x10) & 0xff) << 1) + (((source >> 8) & 0xff) << 2)) + (source & 0xff)) >> 3) & 0xff);
        }

        protected virtual void InitFrame()
        {
        }

        internal static int Mul(int source, int v)
        {
            int num = (source >> 0x10) & 0xff;
            int num2 = (source >> 8) & 0xff;
            int num3 = source & 0xff;
            return (int)((((0xff000000) + (((num * v) >> 8) << 0x10)) + (((num2 * v) >> 8) << 8)) + ((num3 * v) >> 8));
        }

        internal static int Sobel(int[] sobelSource, int x, int y, int width, int height, int yModWidth, int yPlusModWidth, int yMinusModWidth)
        {
            int num = 0;
            if (((x <= 1) || (x >= (width - 1))) || ((y <= 1) || (y >= (height - 1))))
            {
                return 0;
            }
            int num4 = GrayScaleToBlueComponent(sobelSource[(x - 1) + yMinusModWidth]);
            int num5 = GrayScaleToBlueComponent(sobelSource[x + yMinusModWidth]);
            int num6 = GrayScaleToBlueComponent(sobelSource[(x - 1) + yMinusModWidth]);
            int num7 = GrayScaleToBlueComponent(sobelSource[(x - 1) + yModWidth]);
            int num8 = GrayScaleToBlueComponent(sobelSource[(x + 1) + yModWidth]);
            int num9 = GrayScaleToBlueComponent(sobelSource[(x - 1) + yPlusModWidth]);
            int num10 = GrayScaleToBlueComponent(sobelSource[x + yPlusModWidth]);
            int num11 = GrayScaleToBlueComponent(sobelSource[(x + 1) + yPlusModWidth]);
            int num2 = ((((((-num4 + num5) - num7) - num7) + num8) + num8) - num9) + num11;
            int num3 = ((((((-num4 - num5) - num5) - num6) + num9) + num10) + num10) + num11;
            int index = (num2 * num2) + (num3 * num3);
            if (index > (SqrtTable.sqrtTable.Length - 1))
            {
                index = SqrtTable.sqrtTable.Length - 1;
            }
            num = SqrtTable.sqrtTable[index];
            if (num < 0)
            {
                num = 0;
            }
            if (num > 0xc0)
            {
                num = 0xff;
            }
            return (int)(((((0xff000000) + (num << 0x10)) + (num << 8)) + num) ^ 0xffffff);
        }

        internal static int Sub(int source1, int source2)
        {
            int num = (source1 >> 0x10) & 0xff;
            int num2 = (source2 >> 0x10) & 0xff;
            int num3 = (source1 >> 8) & 0xff;
            int num4 = (source2 >> 8) & 0xff;
            int num5 = source1 & 0xff;
            int num6 = source2 & 0xff;
            return (int)(((0xff000000 + ((((num - num2) < 0) ? 0 :(num - num2)) << 0x10)) + ((((num3 - num4) < 0) ? 0 : (num3 - num4)) << 8)) + (((num5 - num6) < 0) ? 0:num5 - num6));
        }

        public void Transform(int[] source, int[] destination, int width, int height)
        {
            lock (this)
            {
                this.InitFrame();
                int currentRowIndex = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        destination[currentRowIndex + j] = this.TransformPixel(source, j, i, width, height, currentRowIndex);
                    }
                    currentRowIndex += width;
                }
            }
        }

        protected abstract int TransformPixel(int[] source, int x, int y, int width, int height, int currentRowIndex);
    }


  
}
