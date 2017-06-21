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
using System.IO;
using Microsoft.Phone;

namespace WPPhotoEditor.Utility
{
    public class UtilityFunction
    {
        public static WriteableBitmap GetImgResource(string imgName)
        {
            Stream stream = App.GetResourceStream(new Uri("/WPPhotoEditor;component/OLD/Brush/" + imgName, UriKind.Relative)).Stream;
            // Decode the JPEG stream.
            WriteableBitmap myBitmap = PictureDecoder.DecodeJpeg(stream);
            return myBitmap;
        }

        public static WriteableBitmap GetICOResource(string imgName)
        {
            Stream stream = App.GetResourceStream(new Uri("/WPPhotoEditor;component/OLD/ICO/" + imgName, UriKind.Relative)).Stream;
            // Decode the JPEG stream.
            WriteableBitmap myBitmap = PictureDecoder.DecodeJpeg(stream);
            return myBitmap;
        }

        /// <summary>
        /// 返回颜色，根据颜色数值字符串
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ReturnColorFromString(string color)
        {
            try
            {
                string alpha = color.Substring(1, 2);
                string red = color.Substring(3, 2);
                string green = color.Substring(5, 2);
                string blue = color.Substring(7, 2);
                byte alphaByte = Convert.ToByte(alpha, 16);
                byte redByte = Convert.ToByte(red, 16);
                byte greenByte = Convert.ToByte(green, 16);
                byte blueByte = Convert.ToByte(blue, 16);
                return Color.FromArgb(alphaByte, redByte, greenByte, blueByte);
            }
            catch
            {
                return Colors.White;
            }
        }



        /// <summary>
        /// 功能：根据容器大小、图象大小,获取最大的图像大小（长宽比例不变）
        /// </summary>
        /// <param name="ContainerSize">容器的大小</param>
        /// <param name="ImgSize">待放事物的大小</param>
        /// <returns>返回适合图像大小</returns>
        public static Size GetImgShowSize(Size ContainerSize, Size ImgSize)
        {
            Size imgShowSize;
            if (ImgSize.Width > ImgSize.Height)
            {
                imgShowSize = new Size(ContainerSize.Width, (int)((double)ImgSize.Height * ((double)ContainerSize.Width / (double)ImgSize.Width)));
                if ((int)((double)ImgSize.Height * ((double)ContainerSize.Width / (double)ImgSize.Width)) > ContainerSize.Height)
                    imgShowSize = new Size((int)((double)ImgSize.Width * ((double)ContainerSize.Height / (double)ImgSize.Height)), ContainerSize.Height);
            }

            else
            {
                imgShowSize = new Size((int)((double)ImgSize.Width * ((double)ContainerSize.Height / (double)ImgSize.Height)), ContainerSize.Height);
                if ((int)((double)ImgSize.Width * ((double)ContainerSize.Height / (double)ImgSize.Height)) > ContainerSize.Width)
                    imgShowSize = new Size(ContainerSize.Width, (int)((double)ImgSize.Height * ((double)ContainerSize.Width / (double)ImgSize.Width)));
            }
            return imgShowSize;
        }
        
        /// <summary>
        /// 根据素描比例获取对应大小
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public static Size GetTheMaxShowSize(Size containerSize,BitmapSource bmp)
        {
            double pro = (double)bmp.PixelWidth / (double)bmp.PixelHeight;
            Size size = new Size(containerSize.Width, containerSize.Height);
            if (pro > (containerSize.Width / containerSize.Height))
            {
                //高一定，宽较长
                size.Height = containerSize.Height;
                size.Width = containerSize.Height * pro;
            }
            else if (pro < (containerSize.Width / containerSize.Height))
            {
                size.Width = containerSize.Width;
                size.Height = containerSize.Width / pro;
            }

            return size;
        }
    }
}
