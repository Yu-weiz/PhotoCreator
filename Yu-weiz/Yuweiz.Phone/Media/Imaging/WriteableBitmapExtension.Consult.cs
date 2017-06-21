using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

///
///此文件函数算法仅供学习参考，不作引用
///

namespace Yuweiz.Phone.Media.Imaging
{
    /// <summary>
    /// WriteableBitmap拓展处理类
    /// </summary>
    public partial class WriteableBitmapExt
    {

        #region 参考的绘图算法

        /// <summary>
        ///  参考的图像式绘图
        /// </summary>
        /// <param name="FromPt"></param>
        /// <param name="ToPt"></param>
        /// <param name="wPNGbp"></param>
        /// <param name="wBmp">目标图像</param>
        /// <param name="drawStyle">drawStyle:1，原始图像;2，颜色图像;3，透明图像;4，不透明图像</param>
        public void DrawBrushFantasia(Point FromPt, Point ToPt, WriteableBitmap wPNGbp, WriteableBitmap wBmp, FillPixelTypeEnum fillPixelType)
        {
            int x1 = (int)(FromPt.X - wPNGbp.PixelWidth / 2);
            int y1 = (int)(FromPt.Y - wPNGbp.PixelHeight / 2);
            int x2 = (int)(ToPt.X - wPNGbp.PixelWidth / 2);
            int y2 = (int)(ToPt.Y - wPNGbp.PixelHeight / 2);

            DrawEachPointForLineFantasia(x1, y1, x2, y2, wPNGbp, wBmp, fillPixelType);
        }

        /// <summary>
        /// 计算要描绘的像素
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="brush"></param>
        /// <param name="pointAction"></param>
        /// <param name="drawStyle">drawStyle:1，原始图像;2，颜色图像;3，透明图像;4，不透明图像</param>
        private void DrawEachPointForLineFantasia(int x1, int y1, int x2, int y2, WriteableBitmap wPNGbp, WriteableBitmap wBmp, FillPixelTypeEnum fillPixelType)
        {
            int num5;
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;// 最大差

            int num = x2 - x1;
            int num2 = y2 - y1;



            int num3 = 0;    //正负1：指示X方向
            if (num < 0)
            {  //左－右
                num = -num;
                num3 = -1;
            }
            else if (num > 0)
            {
                //右－左
                num3 = 1;
            }

            int num4 = 0;    ////正负1：指示Y向
            if (num2 < 0)
            {
                //上－下
                num2 = -num2;
                num4 = -1;
            }
            else if (num2 > 0)
            {
                //下-上
                num4 = 1;
            }


            if (num > num2)    //x差>y差
            {
                //大致横向
                num5 = num3;     //X第次递进
                num6 = 0;        //Y第次递进

                num7 = num3;              //X第次递进
                num8 = num4;
                num9 = num2;
                num10 = num;
            }
            else
            {
                //大致竖向
                num5 = 0;                //X：恒定
                num6 = num4;             //Y每次递进

                num7 = num3;
                num8 = num4;         //Y每次递进
                num9 = num;
                num10 = num2;
            }

            #region 第一个点绘
            int x = x1;
            int y = y1;
            int num13 = num10 >> 1;
            this.DrawTheOpacityFantasia(x, y, wPNGbp, wBmp.Pixels, wBmp.PixelWidth, wBmp.PixelHeight, fillPixelType);
            // setPicturePngTsp(new Point(x, y), wPNGbp, curLayer);
            #endregion


            //num10:Max(X差;Y差)
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

                this.DrawTheOpacityFantasia(x, y, wPNGbp, wBmp.Pixels, wBmp.PixelWidth, wBmp.PixelHeight, fillPixelType);
            }

        }

        /// <summary>
        /// 过期：参考的的描点函数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="brush"></param>
        /// <param name="destPixels"></param>
        /// <param name="destWidth"></param>
        /// <param name="destHeight"></param>
        private static void DrawOpacityBlendUp2(int x, int y, WriteableBitmap brush, int[] destPixels, int destWidth, int destHeight)
        {
            int width = brush.PixelWidth;
            int height = brush.PixelHeight;
            int[] data = brush.Pixels;
            int num3 = x + width;
            int num4 = y + height;
            if ((x < destWidth) && (y < destHeight))
            {
                int num5;
                int num6;
                int num7;
                int num8;
                if (x < 0)
                {
                    num5 = 0;
                    num7 = -x;
                }
                else
                {
                    num5 = x;
                    num7 = 0;
                }
                if (y < 0)
                {
                    num6 = 0;
                    num8 = -y;
                }
                else
                {
                    num6 = y;
                    num8 = 0;
                }
                if (num3 > destWidth)
                {
                    num3 = destWidth;
                }
                if (num4 > destHeight)
                {
                    num4 = destHeight;
                }
                int num11 = num3 - num5;
                int num12 = width - num11;
                int num13 = destWidth - num11;
                int num14 = num4 - num6;
                int num9 = num7 + num11;
                int num10 = num8 + num14;
                if ((num9 >= 0) && (num10 >= 0))
                {
                    int num16 = 0;
                    int index = num7 + (num8 * width);
                    int num18 = num5 + (num6 * destWidth);
                    for (int i = num6; i < num4; i++)
                    {
                        for (int j = num5; j < num3; j++)
                        {
                            int num15 = data[index];
                            if (num15 == 0)
                            {
                                index++;
                                num18++;
                            }
                            else
                            {
                                num16 = (byte)(num15 >> 0x18);
                                if (num16 == 0xff)
                                {
                                    destPixels[num18] = 0x400000ff;
                                    index++;
                                    num18++;
                                }
                                else
                                {
                                    int num21 = destPixels[num18] & -1073741825;
                                    destPixels[num18] = ((num16 + num21) - (((num21 * num16) * 0x8081) >> 0x17)) | 0x40000000;
                                    index++;
                                    num18++;
                                }
                            }
                        }
                        index += num12;
                        num18 += num13;
                    }
                }
            }
        }

        private void DrawTheOpacityFantasia(int x, int y, WriteableBitmap wPNGbp, int[] destPixels, int destWidth, int destHeight, FillPixelTypeEnum fillPixelType)
        {
            int width = wPNGbp.PixelWidth;     //画刷宽度
            int height = wPNGbp.PixelHeight;   //画刷高度
            int[] data = wPNGbp.Pixels;
            int xRange = x + width;   //指定X绘制范围
            int yRange = y + height;   //指定Y绘制范围

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
                    int num16 = 0;
                    int index = num7 + (num8 * width);            //画刷像素索引
                    int num18 = xReal + (yReal * destWidth);        //待绘像素索引
                    for (int i = yReal; i < yRange; i++)
                    {
                        for (int j = xReal; j < xRange; j++)
                        {
                            int num15 = data[index];
                            if (num15 == 0)
                            {
                                index++;
                                num18++;
                            }
                            else
                            {
                                num16 = num15 >> 24;


                                if (fillPixelType == FillPixelTypeEnum.Transparent)
                                {
                                    if ((byte)(destPixels[num18] >> 24) > (byte)num16)
                                    {
                                        destPixels[num18] = data[index];
                                    }
                                }
                                else if (fillPixelType == FillPixelTypeEnum.Opacity)
                                {
                                    if ((byte)(destPixels[num18] >> 24) < (byte)num16)
                                    {
                                        destPixels[num18] = data[index];
                                    }
                                }
                                else
                                {
                                    if ((byte)num16>0)
                                    {
                                        destPixels[num18] = data[index];
                                    }
                                }
                                index++;
                                num18++;


                            }

                        }
                        index += num12;    //画刷图像下一行
                        num18 += num13;    //待画图像下一行像素
                    }
                }


            }
        }
        #endregion


        #region 拓展

        void BresenhamLine(int x0, int y0, int x1, int y1, long color)
        {

            int x, y, dx, dy;

            float k, e;

            dx = x1 - x0;

            dy = y1 - y0;

            e = -dx;

            x = x0;

            y = y0;

            if (dx == 0)
            {

                for (int i = 0; i <= dy; i++)
                {

                    //DrawPixel(x, y + i, color);//画像素（x,y+i）
                    this.DrawThePoint(x, y, wPNGbp, wBmp, fillPixelType);

                }

                return;

            }

            for (int i = 0; i <= dx; i++)
            {

                //DrawPixel(x, y, color);//画像素（x,y）
                this.DrawThePoint(x1, y1, wPNGbp, wBmp, fillPixelType);

                x++;

                e += e + 2 * dy;

                if (e >= 0)
                {

                    y++;

                    e -= 2 * dx;

                }

            }

        }

        void IntegerBresenhamlin(int x0, int y0, int x1, int y1, int color)
        {


            int x, y, dx, dy, unitx, unity, fabs_dx, fabs_dy, e;
            dx = x1 - x0;
            dy = y1 - y0;
            fabs_dx = (int)Math.Abs(dx);
            fabs_dy = (int)Math.Abs(dy);

            if (fabs_dx == 0 || fabs_dy == 0)
            {
                return;
            }


            unitx = dx / fabs_dx;
            unity = dy / fabs_dy;
            x = x0;
            y = y0;


            if (fabs_dx > fabs_dy)
            {
                e = -dx;
                for (int i = 0; i <= fabs_dx; i++)
                {
                    //drawpixel(x,y,color);
                    this.DrawThePoint(x, y, wPNGbp, wBmp, fillPixelType);
                    x += unitx;
                    e = e + 2 * dy;
                    if (e >= 0)
                    {
                        y += unity; e = e - 2 * dx;
                    }
                } // for end
            }
            else
            {
                e = -dy;
                for (int i = 0; i <= fabs_dy; i++)
                {
                    //drawpixel(x,y,color);
                    this.DrawThePoint(x, y, wPNGbp, wBmp, fillPixelType);
                    y += unity;
                    e = e + 2 * dx;
                    if (e >= 0)
                    {
                        x += unitx; e = e - 2 * dy;
                    }
                } // for end
            }// if end
        } //:~

        void BresenhamLine(int x0, int y0, int x1, int y1)
        {
            int deltax, deltay, delta1, delta2, d, x, y;
            deltax = x1 - x0;
            deltay = y1 - y0;
            d = 2 * deltay - deltax;
            delta1 = 2 * deltay;
            delta2 = 2 * (deltay - deltax);
            x = x0; y = y0;

            this.DrawThePoint(x, y, wPNGbp, wBmp, fillPixelType);

            while (x < x1)
            {
                if (d < 0)
                {
                    x++; d += delta1;
                }
                else { x++; y++; d += delta2; }
                {
                    this.DrawThePoint(x, y, wPNGbp, wBmp, fillPixelType);
                }
            }
        }


        #endregion

    }
}
