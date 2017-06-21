using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Yuweiz.Phone.Media
{
    public class ViewControler
    {      
        /// <summary>
        /// 获得取图像指定容器中不失真地显示并匹配容器的显示大小
        /// Size 应为浮点数
        /// </summary>
        /// <param name="ContainerSize">容器的大小</param>
        /// <param name="ImgSize">待放事物的大小</param>
        /// <returns>返回适合图像大小</returns>
        public static Size GetTheFitShowSize(Size containerSize, Size imgSize)
        {
            Size resultSize = imgSize;

            if (imgSize.Width > imgSize.Height)
            {
                double proportion = containerSize.Width / imgSize.Width;
                resultSize = new Size(containerSize.Width, imgSize.Height * proportion);
                if (imgSize.Height * proportion > containerSize.Height)
                {
                    proportion = containerSize.Height / imgSize.Height;
                    resultSize = new Size(imgSize.Width * proportion, containerSize.Height);
                }

            }
            else
            {
                double proportion = containerSize.Height / imgSize.Height;
                resultSize = new Size(imgSize.Width * proportion, containerSize.Height);
                if (imgSize.Width * proportion > containerSize.Width)
                {
                    proportion = containerSize.Width / imgSize.Width;
                    resultSize = new Size(containerSize.Width, imgSize.Height * proportion);
                }

            }
            return resultSize;
        }

        /// <summary>
        /// 根据素描比例获取匹配屏幕的大小
        /// </summary>
        /// <param name="pro">宽/高</param>
        /// <returns></returns>
        public static Size GetTheUniformSize(Size containerSize, Size imgSize)
        {
            double pro = imgSize.Width / imgSize.Height;
            double screenWidth = containerSize.Width;
            double screenHeight = containerSize.Height;
            Size size = new Size(screenWidth, screenHeight);
            if (pro < (screenWidth / screenHeight))
            {               
                size.Width = screenWidth;
                size.Height = screenWidth / pro;
            }
            else if (pro > (screenWidth / screenHeight))
            {
                //宽度较短
                size.Height = screenHeight;
                size.Width = screenHeight * pro;
            }

            return size;
        }

        /// <summary>
        /// 根据素描比例获取匹配屏幕的大小
        /// </summary>
        /// <param name="pro">宽/高</param>
        /// <returns></returns>
        public static Size GetTheUniformToFillSize(Size containerSize, Size imgSize)
        {
            double pro = imgSize.Width / imgSize.Height;
            double screenWidth = containerSize.Width;
            double screenHeight = containerSize.Height;
            Size size = new Size(screenWidth, screenHeight);
            if (pro > (screenWidth / screenHeight))
            {
                //高一定，宽较长
                size.Height = screenHeight;
                size.Width = screenHeight * pro;
            }
            else if (pro < (screenWidth / screenHeight))
            {
                size.Width = screenWidth;
                size.Height = screenWidth / pro;
            }

            return size;
        }

    }
}
