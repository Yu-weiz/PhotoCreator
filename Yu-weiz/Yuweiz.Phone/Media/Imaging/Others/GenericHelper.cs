using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Media.Imaging.Others
{
    public class GenericHelper
    {
        public PaintBrushFace PreviewSurface { get; set; }

        public static void LineForEach(int x1, int y1, int x2, int y2, PaintBrushFace brush, Action<int, int, PaintBrushFace> pointAction)
        {
            int num5;
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;
            int num = x2 - x1;
            int num2 = y2 - y1;
            int num3 = 0;
            if (num < 0)
            {
                num = -num;
                num3 = -1;
            }
            else if (num > 0)
            {
                num3 = 1;
            }
            int num4 = 0;
            if (num2 < 0)
            {
                num2 = -num2;
                num4 = -1;
            }
            else if (num2 > 0)
            {
                num4 = 1;
            }
            if (num > num2)
            {
                num5 = num3;
                num6 = 0;
                num7 = num3;
                num8 = num4;
                num9 = num2;
                num10 = num;
            }
            else
            {
                num5 = 0;
                num6 = num4;
                num7 = num3;
                num8 = num4;
                num9 = num;
                num10 = num2;
            }
            int x = x1;
            int y = y1;
            int num13 = num10 >> 1;
            brush.PixelCountSoFar++;
            if (brush.PixelCountSoFar >= brush.PixelCountToDrawOn)
            {
                pointAction(x, y, brush);
                brush.PixelCountSoFar = 0;
            }
            for (int i = 0; i < num10; i++)
            {
                num13 -= num9;
                if (num13 < 0)
                {
                    num13 += num10;
                    x += num7;
                    y += num8;
                }
                else
                {
                    x += num5;
                    y += num6;
                }
                brush.PixelCountSoFar++;
                if (brush.PixelCountSoFar >= brush.PixelCountToDrawOn)
                {
                    pointAction(x, y, brush);
                    brush.PixelCountSoFar = 0;
                }
            }
        }

        public void DrawBrushOnOpacityLayerBlendUpNoInvalidateTransparency(int fromX, int fromY, int toX, int toY, PaintBrushFace brush)
        {
            int num3;
            int length;
            fromX -= brush.Width / 2;
            fromY -= brush.Height / 2;
            toX -= brush.Width / 2;
            toY -= brush.Height / 2;
            GenericHelper.LineForEach(fromX, fromY, toX, toY, brush, null);
            int num = (fromY * this.PreviewSurface.Width) + fromX;
            int num2 = (toY * this.PreviewSurface.Width) + toX;
            if (num < num2)
            {
                num3 = num;
                length = num2;
            }
            else
            {
                num3 = num2;
                length = num;
            }
            int num5 = brush.JitterAbs >> 1;
            int num6 = ((brush.Height + num5) * this.PreviewSurface.Width) + (brush.Width + num5);
            num3 -= num6;
            length += num6;
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (length > this.PreviewSurface.Pixels.Length)
            {
                length = this.PreviewSurface.Pixels.Length;
            }

           /// GenericHelper.ApplyOpacityPostAddOpaque(this.OpacityMask, this._updatedR, this._updatedG, this._updatedB, this._original, this.PreviewSurface, num3, length);
        }

        public static void ApplyOpacityPostAddOpaque(int[] opacityMask, int ur, int ug, int ub, int[] origBitmap, SilverlightBitmap target, int from, int to)
        {
            int[] pixels = target.Pixels;
            int num6 = ((-16777216 | (ur << 0x10)) | (ug << 8)) | ub;
            for (int i = from; i < to; i++)
            {
                int num = opacityMask[i];
                if ((num & 0x40000000) != 0)
                {
                    num &= -1073741825;
                    if (num >= 0xff)
                    {
                        opacityMask[i] = 0xff;
                        pixels[i] = num6;
                    }
                    else
                    {
                        opacityMask[i] = num;
                        int num2 = origBitmap[i];
                        int num3 = (num2 >> 0x10) & 0xff;
                        int num4 = (num2 >> 8) & 0xff;
                        int num5 = num2 & 0xff;
                        pixels[i] = ((-16777216 | ((num3 + ((((ur - num3) * num) * 0x8081) >> 0x17)) << 0x10)) | ((num4 + ((((ug - num4) * num) * 0x8081) >> 0x17)) << 8)) | (num5 + ((((ub - num5) * num) * 0x8081) >> 0x17));
                    }
                }
            }
        }
    }
}
