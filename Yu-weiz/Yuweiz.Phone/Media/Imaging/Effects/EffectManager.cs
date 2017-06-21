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
using System.Windows.Media.Imaging;
using Yuweiz.Phone.Media.Imaging.Effects.Sketch;

namespace Yuweiz.Phone.Media.Imaging.Effects
{
    public class EffectManager
    {
        public static WriteableBitmap GetSketchEffect(BitmapSource bmp, int containerMaxWidthHeight = 2000)
        {
            WriteableBitmap wbmp = bmp as WriteableBitmap;
            if (wbmp == null)
            {
                wbmp = new WriteableBitmap(bmp);

                if (wbmp.PixelWidth > containerMaxWidthHeight || wbmp.PixelHeight > containerMaxWidthHeight)
                {
                    long applicationMemoryUsageLimit = Microsoft.Phone.Info.DeviceStatus.ApplicationMemoryUsageLimit / 1024 / 1024;
                    if (applicationMemoryUsageLimit > 400)
                    {
                        containerMaxWidthHeight = 3000;
                    }

                    if (wbmp.PixelWidth > containerMaxWidthHeight || wbmp.PixelHeight > containerMaxWidthHeight)
                    {
                        Size size = Media.ViewControler.GetTheUniformToFillSize(new Size(containerMaxWidthHeight, containerMaxWidthHeight), new Size(wbmp.PixelWidth, wbmp.PixelHeight));
                        wbmp = wbmp.Resize((int)size.Width, (int)size.Height, WriteableBitmapExtensions.Interpolation.Bilinear);
                    }
                }
            }


            SobelEffect pencil = new SobelEffect();
            int[] desPencil = new int[wbmp.Pixels.Length];
            pencil.Transform(wbmp.Pixels, desPencil, wbmp.PixelWidth, wbmp.PixelHeight);

            for (int i = 0; i < wbmp.Pixels.Length; i++)
            {
                wbmp.Pixels[i] = desPencil[i];
            }
            return wbmp;
        }

        public static int[] GetSketchEffect(int[] sourcePixels, int width, int height)
        {
            SobelEffect pencil = new SobelEffect();
            int[] desPencil = new int[sourcePixels.Length];
            pencil.Transform(sourcePixels, desPencil, width, height);

            return desPencil;
        }

        public static WriteableBitmap GetGrayEffect(WriteableBitmap wb)
        {
            if (wb == null)
            {
                return null;
            }

            for (int y = 0; y < wb.PixelHeight; y++)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {
                    int pixel = wb.Pixels[y * wb.PixelWidth + x];
                    byte[] dd = BitConverter.GetBytes(pixel);
                    double R = dd[2];
                    double G = dd[1];
                    double B = dd[0];
                    byte gray = (byte)(0.299 * R + 0.587 * G + 0.114 * B);
                    dd[0] = dd[1] = dd[2] = gray;

                    wb.Pixels[y * wb.PixelWidth + x] = BitConverter.ToInt32(dd, 0);
                }
            }
            return wb;
        }

    }
}
