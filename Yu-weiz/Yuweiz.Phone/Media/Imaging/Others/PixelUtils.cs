using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Media.Imaging.Others
{
    /// <summary>
    /// 像素操作类
    /// </summary>
    public class PixelUtils
    {
        public const int ADD = 4;
        public const int ALPHA = 0x13;
        public const int ALPHA_TO_GRAY = 20;
        public const int AVERAGE = 13;
        public const int CLEAR = 15;
        public const int COLOR = 11;
        public const int DIFFERENCE = 6;
        public const int DISSOLVE = 0x11;
        public const int DST_IN = 0x12;
        public const int EXCHANGE = 0x10;
        public const int HUE = 8;
        public const int MAX = 3;
        public const int MIN = 2;
        public const int MULTIPLY = 7;
        public const int NORMAL = 1;
        public const int OVERLAY = 14;
        private static Random randomGenerator = new Random();
        public const int REPLACE = 0;
        public const int SATURATION = 9;
        public const int SCREEN = 12;
        public const int SUBTRACT = 5;
        public const int VALUE = 10;

        public static int brightness(int rgb)
        {
            int num = (rgb >> 0x10) & 0xff;
            int num2 = (rgb >> 8) & 0xff;
            int num3 = rgb & 0xff;
            return (((num + num2) + num3) / 3);
        }

        public static int clamp(int c)
        {
            if (c < 0)
            {
                return 0;
            }
            if (c > 0xff)
            {
                return 0xff;
            }
            return c;
        }

        public static int CombinePixels(int rgb1, int rgb2, int op)
        {
            return CombinePixels(rgb1, rgb2, op, 0xff);
        }

        public static int CombinePixels(int rgb1, int rgb2, int op, int extraAlpha)
        {
            if (op == 0)
            {
                return rgb1;
            }
            int num = (rgb1 >> 0x18) & 0xff;
            int num2 = (rgb1 >> 0x10) & 0xff;
            int num3 = (rgb1 >> 8) & 0xff;
            int num4 = rgb1 & 0xff;
            int num5 = (rgb2 >> 0x18) & 0xff;
            int num6 = (rgb2 >> 0x10) & 0xff;
            int num7 = (rgb2 >> 8) & 0xff;
            int num8 = rgb2 & 0xff;
            switch (op)
            {
                case 2:
                    num2 = Math.Min(num2, num6);
                    num3 = Math.Min(num3, num7);
                    num4 = Math.Min(num4, num8);
                    break;

                case 3:
                    num2 = Math.Max(num2, num6);
                    num3 = Math.Max(num3, num7);
                    num4 = Math.Max(num4, num8);
                    break;

                case 4:
                    num2 = clamp(num2 + num6);
                    num3 = clamp(num3 + num7);
                    num4 = clamp(num4 + num8);
                    break;

                case 5:
                    num2 = clamp(num6 - num2);
                    num3 = clamp(num7 - num3);
                    num4 = clamp(num8 - num4);
                    break;

                case 6:
                    num2 = clamp(Math.Abs((int)(num2 - num6)));
                    num3 = clamp(Math.Abs((int)(num3 - num7)));
                    num4 = clamp(Math.Abs((int)(num4 - num8)));
                    break;

                case 7:
                    num2 = clamp((num2 * num6) / 0xff);
                    num3 = clamp((num3 * num7) / 0xff);
                    num4 = clamp((num4 * num8) / 0xff);
                    break;

                case 8:
                case 9:
                case 10:
                case 12:
                    num2 = 0xff - (((0xff - num2) * (0xff - num6)) / 0xff);
                    num3 = 0xff - (((0xff - num3) * (0xff - num7)) / 0xff);
                    num4 = 0xff - (((0xff - num4) * (0xff - num8)) / 0xff);
                    break;

                case 13:
                    num2 = (num2 + num6) / 2;
                    num3 = (num3 + num7) / 2;
                    num4 = (num4 + num8) / 2;
                    break;

                case 14:
                    {
                        int num10 = 0xff - (((0xff - num2) * (0xff - num6)) / 0xff);
                        int num9 = (num2 * num6) / 0xff;
                        num2 = ((num10 * num2) + (num9 * (0xff - num2))) / 0xff;
                        num10 = 0xff - (((0xff - num3) * (0xff - num7)) / 0xff);
                        num9 = (num3 * num7) / 0xff;
                        num3 = ((num10 * num3) + (num9 * (0xff - num3))) / 0xff;
                        num10 = 0xff - (((0xff - num4) * (0xff - num8)) / 0xff);
                        num9 = (num4 * num8) / 0xff;
                        num4 = ((num10 * num4) + (num9 * (0xff - num4))) / 0xff;
                        break;
                    }
                case 15:
                    num2 = num3 = num4 = 0xff;
                    break;

                case 0x11:

                    Random rnd1 = new Random();
                    if (rnd1.Next(0x100) <= num)
                    {
                        num2 = num6;
                        num3 = num7;
                        num4 = num8;
                    }
                    break;

                case 0x12:
                    num2 = clamp((num6 * num) / 0xff);
                    num3 = clamp((num7 * num) / 0xff);
                    num4 = clamp((num8 * num) / 0xff);
                    return ((((clamp((num5 * num) / 0xff) << 0x18) | (num2 << 0x10)) | (num3 << 8)) | num4);

                case 0x13:
                    num = (num * num5) / 0xff;
                    return ((((num << 0x18) | (num6 << 0x10)) | (num7 << 8)) | num8);

                case 20:
                    {
                        int num11 = 0xff - num;
                        return ((((num << 0x18) | (num11 << 0x10)) | (num11 << 8)) | num11);
                    }
            }
            if ((extraAlpha != 0xff) || (num != 0xff))
            {
                num = (num * extraAlpha) / 0xff;
                int num12 = ((0xff - num) * num5) / 0xff;
                num2 = clamp(((num2 * num) + (num6 * num12)) / 0xff);
                num3 = clamp(((num3 * num) + (num7 * num12)) / 0xff);
                num4 = clamp(((num4 * num) + (num8 * num12)) / 0xff);
                num = clamp(num + num12);
            }
            return ((((num << 0x18) | (num2 << 0x10)) | (num3 << 8)) | num4);
        }

        public static int CombinePixels(int rgb1, int rgb2, int op, int extraAlpha, int channelMask)
        {
            return ((rgb2 & ~channelMask) | CombinePixels(rgb1 & channelMask, rgb2, op, extraAlpha));
        }

        public static int interpolate(int v1, int v2, float f)
        {
            return clamp(v1 + ((int)(f * (v2 - v1))));
        }

        public static bool nearColors(int rgb1, int rgb2, int tolerance)
        {
            int num = (rgb1 >> 0x10) & 0xff;
            int num2 = (rgb1 >> 8) & 0xff;
            int num3 = rgb1 & 0xff;
            int num4 = (rgb2 >> 0x10) & 0xff;
            int num5 = (rgb2 >> 8) & 0xff;
            int num6 = rgb2 & 0xff;
            return (((Math.Abs((int)(num - num4)) <= tolerance) && (Math.Abs((int)(num2 - num5)) <= tolerance)) && (Math.Abs((int)(num3 - num6)) <= tolerance));
        }
    }
}
