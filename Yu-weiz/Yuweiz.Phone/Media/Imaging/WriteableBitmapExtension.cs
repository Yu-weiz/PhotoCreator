using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Media.Imaging
{
    public enum FillPixelTypeEnum { Color, Transparent, Opacity, Original }

    /// <summary>
    /// WriteableBitmap拓展处理类
    /// </summary>
    public partial class WriteableBitmapExt
    {
        private WriteableBitmapExt()
        {

        }

        private static WriteableBitmapExt instance;

        public static WriteableBitmapExt Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WriteableBitmapExt();
                }

                return instance;
            }
        }

        private WriteableBitmap wPNGbp;

        private WriteableBitmap wBmp;

        private FillPixelTypeEnum fillPixelType;

        public int[] WbmpLinePixels { get; set; }


        #region 自己的绘图算法
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="FromPt"></param>
        /// <param name="ToPt"></param>
        /// <param name="wPNGbp"></param>
        /// <param name="wBmp">目标图像</param>
        /// <param name="drawStyle">drawStyle:1，原始图像;2，颜色图像;3，透明图像;4，不透明图像</param>
        public void DrawEachPoint(Point pt, WriteableBitmap wPNGbp, WriteableBitmap wBmp, FillPixelTypeEnum fillPixelType)
        {
            this.wBmp = wBmp;
            this.wPNGbp = wPNGbp;
            this.fillPixelType = fillPixelType;

            int x = (int)(pt.X - wPNGbp.PixelWidth / 2);
            int y = (int)(pt.Y - wPNGbp.PixelHeight / 2);

            this.DrawThePoint(x, y, wPNGbp, wBmp, fillPixelType);
        }

        /// <summary>
        /// 画线,wPNGbp 的像素正方形，像素高必须要为偶数，否则，效果变形，棱角严重
        /// </summary>
        /// <param name="FromPt"></param>
        /// <param name="ToPt"></param>
        /// <param name="wPNGbp">wPNGbp 的像素正方形，像素高必须要为偶数，否则，效果变形，棱角严重</param>
        /// <param name="wBmp">目标图像</param>
        /// <param name="drawStyle">drawStyle:1，原始图像;2，颜色图像;3，透明图像;4，不透明图像</param>
        public void DrawEachPointForLine(Point FromPt, Point ToPt, WriteableBitmap wPNGbp, WriteableBitmap wBmp, FillPixelTypeEnum fillPixelType)
        {
            this.wBmp = wBmp;
            this.wPNGbp = wPNGbp;
            this.fillPixelType = fillPixelType;

            int x1 = (int)(FromPt.X - wPNGbp.PixelWidth / 2);
            int y1 = (int)(FromPt.Y - wPNGbp.PixelHeight / 2);
            int x2 = (int)(ToPt.X - wPNGbp.PixelWidth / 2);
            int y2 = (int)(ToPt.Y - wPNGbp.PixelHeight / 2);

            //DrawEachPointForLine(x1, y1, x2, y2, wPNGbp, wBmp, fillPixelType);
            // BresenhamLine(x1, y1, x2, y2,0);

            //绘始终点
            if (x1 == x2 && y1 == y2)
            {
                this.DrawThePoint(x1, y1, wPNGbp, wBmp, fillPixelType);
                return;
            }

            this.DrawThePoint(x1, y1, wPNGbp, wBmp, fillPixelType);
            this.DrawThePoint(x2, y2, wPNGbp, wBmp, fillPixelType);


            IEnumerable<Point> pts = GetPointsOnLine(x1, y1, x2, y2);
            foreach (Point pt in pts)
            {
                //this.DrawThePoint((int)pt.X, (int)pt.Y, wPNGbp, wBmp, fillPixelType);

                int offX = wPNGbp.PixelWidth / 2;   //如果不加，则从左上角点开始画线，但首点另作绘制，故会覆盖
                int offY = wPNGbp.PixelWidth / 2;   //如果不加，则从左上角点开始画线，但首点另作绘制，故会覆盖

                int[] pixels = GetMiddlePixels(wPNGbp);
                double k = 0;
                if (Math.Abs(x2 - x1) < 10)
                {
                    //垂直方向
                    DrawEachPoint((int)pt.X + 0, (int)pt.Y + offY, pixels, wBmp, 1, fillPixelType);

                }
                else if (Math.Abs(y2 - y1) < 10)
                {
                    //水平方向
                    DrawEachPoint((int)pt.X + offX, (int)pt.Y - 0, pixels, wBmp, 2, fillPixelType);
                }
                else
                {
                    //倾斜方向
                    k = ((double)y2 - (double)y1) / ((double)x2 - (double)x1);
                    double b = y1 - k * x1;
                    double wpngbpK = -1 / k;

                    DrawEachPoint((int)pt.X + offX, (int)pt.Y + offY, pixels, wBmp, wpngbpK, fillPixelType);
                }

            }
        }

        private int[] GetMiddlePixels(WriteableBitmap wPNGbp)
        {
            if (this.WbmpLinePixels != null)
            {
                return this.WbmpLinePixels;
            }

            int[] pixels = new int[wPNGbp.PixelWidth];
            int middleIndex = wPNGbp.PixelWidth * wPNGbp.PixelHeight / 2;

            Array.Copy(wPNGbp.Pixels, middleIndex, pixels, 0, pixels.Length);

            return pixels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="pngWbmp"></param>
        /// <param name="pixels"></param>
        /// <param name="direction">1：横，2：竖，3撇;4，捺</param>
        private void DrawEachPoint(int ptX, int ptY, int[] pixels, WriteableBitmap targetWbmp, byte direction, FillPixelTypeEnum fillPixelType)
        {
            switch (direction)
            {
                case 1:
                    {
                        #region 横:绘图方向正向下
                        int wbmpIndex = (ptY * targetWbmp.PixelWidth + ptX);

                        int pixelIndex = 0;
                        while (pixelIndex < pixels.Length && wbmpIndex < targetWbmp.Pixels.Length && pixelIndex >= 0 && wbmpIndex >= 0)
                        {

                            if (wbmpIndex < (ptY + 1) * targetWbmp.PixelWidth && wbmpIndex > (ptY - 1) * targetWbmp.PixelWidth && ptX > 0 && ptX < targetWbmp.PixelWidth)
                            //if (ptX>0&&ptX<targetWbmp.PixelWidth)
                            {
                                // System.Diagnostics.Debug.WriteLine("pixelIndex:{0}    pixels.Length:{1}   wbmpIndex:{2}    targetWbmp.Pixels:{3}", pixelIndex, pixels.Length, wbmpIndex, targetWbmp.Pixels.Length);
                                targetWbmp.Pixels[wbmpIndex] = this.DrawPixel(targetWbmp.Pixels[wbmpIndex], pixels[pixelIndex], fillPixelType);
                            }

                            wbmpIndex++;
                            pixelIndex++;
                        }

                        break;
                        #endregion
                    }
                case 2:
                    {
                        #region 竖:绘图方向正右
                        int pixelMiddleIndex = pixels.Length / 1;
                        int wbmpIndex = ((ptY + pixelMiddleIndex) * targetWbmp.PixelWidth + ptX);

                        int pixelIndex = 0;
                        while (pixelIndex < pixels.Length && pixelIndex >= 0)
                        {
                            wbmpIndex = ((ptY + pixelMiddleIndex - pixelIndex) * targetWbmp.PixelWidth + ptX);
                            if (wbmpIndex < targetWbmp.Pixels.Length && wbmpIndex >= 0)
                            {
                                targetWbmp.Pixels[wbmpIndex] = this.DrawPixel(targetWbmp.Pixels[wbmpIndex], pixels[pixelIndex], fillPixelType);
                            }

                            pixelIndex++;
                        }

                        break;
                        #endregion
                    }
                case 3:
                    {
                        #region 撇:绘图方向往右下
                        int pixelMiddleIndex = pixels.Length / 2;
                        int wbmpIndex = 0; // ((ptY + pixelMiddleIndex) * targetWbmp.PixelWidth + ptX - pixelMiddleIndex);
                        int pixelIndex = 0;

                        while (pixelIndex < pixels.Length && pixelIndex >= 0)
                        {
                            wbmpIndex = ((ptY + pixelMiddleIndex - pixelIndex) * targetWbmp.PixelWidth + ptX + pixelIndex + 1);
                            if (wbmpIndex < targetWbmp.Pixels.Length && wbmpIndex >= 0)
                            {
                                targetWbmp.Pixels[wbmpIndex] = this.DrawPixel(targetWbmp.Pixels[wbmpIndex], pixels[pixelIndex], fillPixelType);
                            }

                            pixelIndex++;
                        }

                        //填充空隙
                        //pixelIndex =0;
                        //while (pixelIndex < pixels.Length - 1 && wbmpIndex < targetWbmp.Pixels.Length && pixelIndex >= 0 && wbmpIndex >= 0)
                        //{
                        //    wbmpIndex = ((ptY + pixelMiddleIndex - pixelIndex + 1) * targetWbmp.PixelWidth + ptX - pixelMiddleIndex + pixelIndex);
                        //    if (wbmpIndex < targetWbmp.Pixels.Length && wbmpIndex >= 0)
                        //    {
                        //        targetWbmp.Pixels[wbmpIndex] = this.DrawPixel(targetWbmp.Pixels[wbmpIndex], pixels[pixelIndex + 1], fillPixelType);
                        //    }

                        //    pixelIndex++;
                        //}
                        break;
                        #endregion
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="pngWbmp"></param>
        /// <param name="pixels"></param>
        /// <param name="direction">1：横，2：竖，3撇;4，捺</param>
        private void DrawEachPoint(int ptX, int ptY, int[] pixels, WriteableBitmap targetWbmp, double wpngK, FillPixelTypeEnum fillPixelType)
        {
            if (double.IsInfinity(wpngK))
            {
                //
            }
            else if (wpngK == 0)
            {

            }
            else
            {
                double b = ptY - wpngK * ptX;
                double angleA = Math.Atan(wpngK);
                double pixelMiddleIndex = pixels.Length / 2;


                //方向问题 
                //double yMiddleInterval = pixelMiddleIndex * Math.Sin(angleA);
                double xMiddleInterval = pixelMiddleIndex * Math.Cos(angleA);   //中间描绘点X离最边描绘点X的距离

                int startX = ptX - (int)(1 * xMiddleInterval);
                int startY = (int)(wpngK * startX + b);

                int stopX = ptX + (int)xMiddleInterval;
                int stopY = (int)(wpngK * stopX + b);

                IEnumerable<Point> pts = GetPointsOnLine(startX, startY, stopX, stopY);

                int i = 0;
                double ptCount = pts.Count();
                foreach (Point pt in pts)
                {
                    double scale = (double)i / ptCount;
                    int pixelIndex = (int)(scale * pixels.Length);
                    //int pixelIndex = i;

                    int wbmpIndex = (int)(pt.Y * targetWbmp.PixelWidth + pt.X);
                    if (wbmpIndex < targetWbmp.Pixels.Length && wbmpIndex >= 0 && pixelIndex >= 0 && pixelIndex < pixels.Length)
                    {
                        if (pt.X < targetWbmp.PixelWidth && pt.X >= 0)
                        {
                            targetWbmp.Pixels[wbmpIndex] = this.DrawPixel(targetWbmp.Pixels[wbmpIndex], pixels[pixelIndex], fillPixelType);
                        }
                    }

                    wbmpIndex = (int)(pt.Y * targetWbmp.PixelWidth + pt.X) + 1;
                    if (wbmpIndex < targetWbmp.Pixels.Length && wbmpIndex >= 0 && pixelIndex >= 0 && pixelIndex < pixels.Length)
                    {
                        if (pt.X + 1 < targetWbmp.PixelWidth && pt.X >= 0)
                        {
                            targetWbmp.Pixels[wbmpIndex] = this.DrawPixel(targetWbmp.Pixels[wbmpIndex], pixels[pixelIndex], fillPixelType);
                        }
                    }

                    i++;
                }
            }
        }

        /// <summary>
        /// 像素实施着点
        /// </summary>
        /// <param name="destPixel"></param>
        /// <param name="sourcePixel"></param>
        /// <param name="style">着点方式 drawStyle:1，原始图像;2，颜色图像;3，透明图像;4，不透明图像</param>
        private int DrawPixel(int destPixel, int sourcePixel, FillPixelTypeEnum fillPixelType)
        {
            switch (fillPixelType)
            {
                case FillPixelTypeEnum.Transparent:
                    {
                        byte sourcePixelAValue = (byte)(sourcePixel >> 24);
                        byte destPixelAValue = (byte)(destPixel >> 24);

                        ///值越大越不透明
                        if (destPixelAValue > sourcePixelAValue && sourcePixelAValue != 255 && sourcePixelAValue != 0)
                        {
                            destPixel = sourcePixel;
                        }
                        break;
                    }
                case FillPixelTypeEnum.Opacity:
                    {
                        byte sourcePixelAValue = (byte)(sourcePixel >> 24);
                        byte destPixelAValue = (byte)(destPixel >> 24);

                        if (destPixelAValue < sourcePixelAValue)
                        {
                            destPixel = sourcePixel;
                        }

                        break;
                    }
                default:
                    {
                        #region 画笔或原始画刷
                        byte apachValue = (byte)(sourcePixel >> 24);
                        if (apachValue == 255)
                        {
                            destPixel = sourcePixel;
                        }
                        #endregion
                        break;
                    }
            }

            return destPixel;
        }

        /// <summary>
        /// 像素实施着点
        /// </summary>
        /// <param name="destPixel"></param>
        /// <param name="sourcePixel"></param>
        /// <param name="style">着点方式 drawStyle:1，原始图像;2，颜色图像;3，透明图像;4，不透明图像</param>
        private int DrawPixel2(int destPixel, int sourcePixel, FillPixelTypeEnum fillPixelType)
        {
            switch (fillPixelType)
            {
                case FillPixelTypeEnum.Transparent:
                    {
                        byte sourcePixelAValue = (byte)(sourcePixel >> 24);
                        byte destPixelAValue = (byte)(destPixel >> 24);

                        ///值越大越不透明
                        if (destPixelAValue > sourcePixelAValue && sourcePixelAValue != 255 && sourcePixelAValue != 0)
                        {
                            destPixel = sourcePixel;
                        }
                        break;
                    }
                case FillPixelTypeEnum.Opacity:
                    {
                        byte sourcePixelAValue = (byte)(sourcePixel >> 24);
                        byte destPixelAValue = (byte)(destPixel >> 24);

                        if (destPixelAValue < sourcePixelAValue)
                        {
                            destPixel = sourcePixel;
                        }

                        break;
                    }
                default:
                    {
                        byte apachValue = (byte)(sourcePixel >> 24);
                        if (apachValue > 0)
                        {
                            destPixel = sourcePixel;
                        }
                        break;
                    }
            }

            return destPixel;
        }

        /// <summary>
        /// 过期：根据指定的PNG设置蒙版
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="wPNGbp"></param>
        /// <param name="curLayer"></param>
        private void setPicturePngTspBackUp(Point pt, WriteableBitmap wPNGbp, WriteableBitmap wBmp)
        {
            int ptX = Convert.ToInt32(pt.X);    //当前X坐标
            int ptY = Convert.ToInt32(pt.Y);    //当前Y坐标

            int wbWidth = wBmp.PixelWidth;               //当前画布宽度
            int wbHeight = wBmp.PixelHeight;              //当前画布高度

            int recHalfWidth = Convert.ToInt32(wPNGbp.PixelWidth / 2);            //要显示粉刷宽度的一半
            int recHalfHeight = Convert.ToInt32(wPNGbp.PixelHeight / 2);          //要显示粉刷高度的一半

            //i:wPNGbp的列点;i<待绘总像素长度，且i+当前X坐标<画布长度
            for (int i = 0; i < wPNGbp.PixelWidth && ptX - recHalfWidth + i < wbWidth; i++)
            {
                int aimX = ptX - recHalfWidth + i;   //wBmp蒙版画布的待绘X坐标点

                //j:wPNGbp的行点;j<待绘总像素长度，且j+当前Y坐标<画布高度
                for (int j = 0; j < wPNGbp.PixelHeight && ptY - recHalfHeight + j < wbHeight; j++)
                {
                    int aimY = ptY - recHalfHeight + j;   //wBmp蒙版画布的待绘Y坐标点
                    if (aimY * (wbWidth) + aimX < wBmp.Pixels.Length && aimY * (wbWidth) + aimX >= 0 && aimX >= 0 && aimY >= 0)
                    {
                        //如果wBmp蒙版画布的待绘坐标在蒙版内，则可绘制
                        byte[] rgb = BitConverter.GetBytes(wBmp.Pixels[aimY * (wbWidth) + aimX]);    //获取原sRGBA

                        if (j * (wPNGbp.PixelWidth + 1) + i < wPNGbp.Pixels.Length && j * (wPNGbp.PixelWidth) + i >= 0)
                        {
                            int pixelValue = wPNGbp.Pixels[j * (wPNGbp.PixelWidth) + i];
                            wBmp.Pixels[aimY * (wbWidth) + aimX] = pixelValue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 实际描绘像素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wPNGbp">画刷图像</param>
        /// <param name="destPixels">目标蒙版</param>
        /// <param name="destWidth"></param>
        /// <param name="destHeight"></param>
        private void DrawThePoint(int x, int y, WriteableBitmap wPNGbp, WriteableBitmap targetWbmp, FillPixelTypeEnum fillPixelType)
        {
            int width = wPNGbp.PixelWidth;     //画刷宽度
            int height = wPNGbp.PixelHeight;   //画刷高度
            int[] data = wPNGbp.Pixels;
            int xRange = x + width;   //指定X绘制范围
            int yRange = y + height;   //指定Y绘制范围

            int[] destPixels = targetWbmp.Pixels;
            int destWidth = targetWbmp.PixelWidth;
            int destHeight = targetWbmp.PixelHeight;

            //如果待画坐标于图像内则执行
            if ((x < destWidth) && (y < destHeight))
            {
                int xReal;
                int yReal;
                int num7;
                int num8;

                #region 校正X在图像内：Num5;         Num7用于描绘画刷非中心边缘（当中心点不在待绘图像中，但其边缘在时）
                if (x < 0)
                {
                    xReal = 0;
                    num7 = -x;
                }
                else
                {
                    xReal = x;
                    num7 = 0;
                }
                #endregion

                #region 校正Y在图像内：Num6;          Num8用于描绘画刷非中心边缘（当中心点不在待绘图像中，但其边缘在时）
                if (y < 0)
                {
                    yReal = 0;
                    num8 = -y;
                }
                else
                {
                    yReal = y;
                    num8 = 0;
                }
                #endregion

                #region 校正于图像内
                if (xRange > destWidth)
                {
                    xRange = destWidth;
                }

                if (yRange > destHeight)
                {
                    yRange = destHeight;
                }
                #endregion

                int num11 = xRange - xReal;    //==x

                int num12 = width - num11;
                int num13 = destWidth - num11;

                int num14 = yRange - yReal;    //==y

                int num9 = num7 + num11;   //真实的X待绘点
                int num10 = num8 + num14;   //真实的Y待绘点


                if ((num9 >= 0) && (num10 >= 0))
                {
                    int sourcePixelAValue = 0;
                    int index = num7 + (num8 * width);            //画刷像素索引
                    int num18 = xReal + (yReal * destWidth);        //待绘像素索引
                    for (int i = yReal; i < yRange; i++)
                    {
                        for (int j = xReal; j < xRange; j++)
                        {
                            int sourcePixel = data[index];

                            if (fillPixelType == FillPixelTypeEnum.Color || fillPixelType == FillPixelTypeEnum.Original)
                            {
                                #region 画笔或原始画刷
                                byte apachValue = (byte)(sourcePixel >> 24);
                                if (apachValue == 255)
                                {
                                    destPixels[num18] = sourcePixel;
                                }

                                index++;
                                num18++;
                                #endregion
                            }
                            else
                            {
                                #region 透明或不透明处理
                                if (sourcePixel == 0)
                                {
                                    index++;
                                    num18++;
                                }
                                else
                                {
                                    sourcePixelAValue = sourcePixel >> 24;


                                    if (fillPixelType == FillPixelTypeEnum.Transparent && sourcePixelAValue < 255)
                                    {
                                        if ((byte)(destPixels[num18] >> 24) > (byte)sourcePixelAValue)
                                        {
                                            destPixels[num18] = data[index];
                                        }
                                    }
                                    else if (fillPixelType == FillPixelTypeEnum.Opacity)
                                    {
                                        if ((byte)(destPixels[num18] >> 24) < (byte)sourcePixelAValue)
                                        {
                                            destPixels[num18] = data[index];
                                        }
                                    }
                                    index++;
                                    num18++;

                                }
                                #endregion
                            }

                        }
                        index += num12;    //画刷图像下一行
                        num18 += num13;    //待画图像下一行像素
                    }
                }


            }
        }

        #endregion


        /// <summary>
        /// Bresenham像素画线算法
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        private static IEnumerable<Point> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Point((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }

    }
}
