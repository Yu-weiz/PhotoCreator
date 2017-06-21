using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Yuweiz.Phone.Media.Imaging;

namespace WPPhotoEditor.Media
{

    public class BrushFactory
    {

        private BrushFactory()
        {

            InitializeDefaultBrush();
        }

        public static readonly BrushFactory Instance = new BrushFactory();

        private void InitializeDefaultBrush()
        {
            this.BrushSize = new Size(50, 50);
            this.BrushOpacityValue = 100;
            this.BrushColor = Color.FromArgb(50, 151, 241, 26);
            this.BrushImg = new BitmapImage(new Uri("/Assets/Brush/1.png", UriKind.Relative)); // new BitmapImage(new Uri("/Assets/Brush/BrBuffer.png", UriKind.Relative));  //Utility.GetBrushResource("BrFlower.png") brPoint.png
            this.BrushPenType = FillPixelTypeEnum.Color;

            this.BrushWbmp = this.CreateTargetBrush(this.BrushPenType);
        }


        public FillPixelTypeEnum BrushPenType { get; set; }

        public Color BrushColor
        { get; set; }

        public int? SelectedBrushIndex { get; set; }

        public ImageSource BrushImg
        { get; set; }

        public Size BrushSize
        { get; set; }

        public WriteableBitmap BrushWbmp
        { get; private set; }

        /// <summary>
        /// 用于线性绘图时，1*BrushWbmp.PixelWidth 的像素样本
        /// WriteableBitmapExt.DrawEachPoint 使用（替代GetMiddlePixels 方法）
        /// 使非对称画刷同样能够完美线性绘图
        /// 要求：BrushWbmp，宽高相等
        /// </summary>
        public int[] WbmpLinePixels
        {
            get;
            private set;
        }

        public byte BrushOpacityValue
        { get; set; }

        /// <summary>
        /// 生成默认的透明画笔
        /// </summary>
        public void InitializeDefaultTransparencyBrush(FillPixelTypeEnum fillType= FillPixelTypeEnum.Transparent)
        {
           // this.BrushImg = new BitmapImage(new Uri("/Assets/Brush/1.png", UriKind.Relative)); // new BitmapImage(new Uri("/Assets/Brush/BrBuffer.png", UriKind.Relative));  //Utility.GetBrushResource("BrFlower.png") brPoint.png

           // this.BrushImg = new BitmapImage(new Uri(fillType == FillPixelTypeEnum.Transparent ? "/Assets/Brush/5.png" : "/Assets/Brush/1.png", UriKind.Relative)); // new BitmapImage(new Uri("/Assets/Brush/BrBuffer.png", UriKind.Relative));  //Utility.GetBrushResource("BrFlower.png") brPoint.png
            this.BrushPenType = fillType;

            this.BrushWbmp = this.CreateTargetBrush(this.BrushPenType);
        }
             

        /// <summary>
        /// 生成默认的透明画笔
        /// </summary>
        public void InitializeDefaultColorBrush()
        {
            // this.BrushImg = new BitmapImage(new Uri("/Assets/Brush/5.png", UriKind.Relative)); // new BitmapImage(new Uri("/Assets/Brush/BrBuffer.png", UriKind.Relative));  //Utility.GetBrushResource("BrFlower.png") brPoint.png
            this.BrushPenType = FillPixelTypeEnum.Color;

            this.BrushWbmp = this.CreateTargetBrush(this.BrushPenType);
        }

        /// <summary>
        /// 生成目標畫刷，並賦值到：BrushWbmp
        /// </summary>
        /// <param name="fillType">生成的像素画刷类型</param>
        public WriteableBitmap CreateBrushWbmp(Color brushColor, ImageSource brushImg, Size brushSize, byte brushOpacityValue, FillPixelTypeEnum fillType)
        {
            this.BrushColor = brushColor;
            this.BrushSize = brushSize;
            this.BrushOpacityValue = brushOpacityValue;
            this.BrushImg = brushImg;

            this.BrushWbmp = CreateTargetBrush(fillType);
            return this.BrushWbmp;
        }

        /// <summary>
        /// 生成目標畫刷，並賦不值到：BrushWbmp
        /// </summary>
        /// <param name="fillType">生成的像素画刷类型</param>
        public WriteableBitmap CreateBrush(Color brushColor, ImageSource brushImg, Size brushSize, byte brushOpacityValue, FillPixelTypeEnum fillType)
        {
            this.BrushColor = brushColor;
            this.BrushSize = brushSize;
            this.BrushOpacityValue = brushOpacityValue;
            this.BrushImg = brushImg;

            WriteableBitmap wbmp = CreateTargetBrush(fillType);
            return wbmp;
        }

        /// <summary>
        /// 生成目標畫刷，並賦值到：TargetBrush
        /// </summary>
        private WriteableBitmap CreateTargetBrush(FillPixelTypeEnum fillType)
        {
            if (this.BrushImg == null)
            {
                throw new Exception("尝试生成绘画画刷，但是画刷样本为空，无法生成！");
            }

            Image img = new Image();
            img.Width = this.BrushSize.Width;
            img.Height = this.BrushSize.Height;

            img.Stretch = Stretch.Fill;
            img.Source = this.BrushImg;

            WriteableBitmap wb = new WriteableBitmap(img, null);
            if (FillPixelTypeEnum.Color == fillType)
            {
                wb = GetColorWbmp(wb, this.BrushColor, this.BrushOpacityValue);
            }
            else if (FillPixelTypeEnum.Transparent == fillType)
            {
                int lowestValue = 0;
                wb = GetOriginalWbmp(wb, ref lowestValue);
                int scale = lowestValue - (this.BrushOpacityValue);
                wb = GetTransparetWbmp(wb, scale);
            }
            else if (FillPixelTypeEnum.Opacity == fillType)
            {
                int HeightValue = GetHeightValue(wb);
                int scale = HeightValue - (255 - this.BrushOpacityValue);
                wb = GetOpacityWbmp(wb, scale);
            }

            return wb;
        }

        /// <summary>
        /// 生成對應顏色的畫刷
        /// </summary>
        /// <param name="wEraserBmp"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private WriteableBitmap GetColorWbmp(WriteableBitmap wEraserBmp, Color color, byte opacityValue)
        {
            // return wEraserBmp;
            //   this.WbmpLinePixels = new int[wEraserBmp.PixelWidth];

            for (int i = 0; i < wEraserBmp.PixelWidth; i++)
            {
                for (int j = 0; j < wEraserBmp.PixelHeight; j++)
                {
                    int index = j * wEraserBmp.PixelWidth + i;
                    byte[] rgb = BitConverter.GetBytes(wEraserBmp.Pixels[index]);

                    if (rgb[3] == 255)
                    {
                        rgb[0] = color.B;
                        rgb[1] = color.G;
                        rgb[2] = color.R;
                    }
                    else
                    {
                        rgb[0] = 255;
                        rgb[1] = 255;
                        rgb[2] = 255;
                        rgb[3] = 0;
                    }

                    //rgb[3] = Convert.ToByte(255 - rgb[3]);
                    int pixelValue = BitConverter.ToInt32(rgb, 0);
                    wEraserBmp.Pixels[index] = BitConverter.ToInt32(rgb, 0);

                    if (rgb[3] == 255)
                    {
                        // this.WbmpLinePixels[i] = wEraserBmp.Pixels[index];
                    }

                }
            }
            return wEraserBmp;
        }

        /// <summary>
        /// 生成透明的圖形畫刷圖像，並返回不透明度最低的值(即最透明的值)
        /// </summary>
        /// <param name="wEraserBmp"></param>
        /// <param name="lowestValue"></param>
        /// <returns></returns>
        private WriteableBitmap GetOriginalWbmp(WriteableBitmap wEraserBmp, ref int lowestValue)
        {
            lowestValue = 255;
            for (int i = 0; i < wEraserBmp.PixelWidth; i++)
            {
                for (int j = 0; j < wEraserBmp.PixelHeight; j++)
                {
                    byte[] rgb = BitConverter.GetBytes(wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i]);
                    rgb[3] = Convert.ToByte(255 - rgb[3]);    //获取粉刷的不透明度255 - rgb[3]
                    int pixelValue = BitConverter.ToInt32(rgb, 0);
                    wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i] = BitConverter.ToInt32(rgb, 0);

                    if (lowestValue > rgb[3])
                    {
                        lowestValue = rgb[3];
                    }
                }
            }
            return wEraserBmp;
        }

        /// <summary>
        /// 根據"透明的圖形畫刷圖像"與設置的最大值的透明度參數，生成對應的---"透明的圖形畫刷圖像"
        /// </summary>
        /// <param name="wEraserBmp">透明的圖形畫刷圖像</param>
        /// <param name="transpareScale"></param>
        /// <returns></returns>
        private WriteableBitmap GetTransparetWbmp(WriteableBitmap wEraserBmp, int transpareScale)
        {
            for (int i = 0; i < wEraserBmp.PixelWidth; i++)
            {
                for (int j = 0; j < wEraserBmp.PixelHeight; j++)
                {
                    byte[] rgb = BitConverter.GetBytes(wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i]);
                    // rgb[3] = Convert.ToByte(255 - rgb[3]);    //获取粉刷的不透明度255 - rgb[3]
                    if ((rgb[3] - transpareScale) >= 0 && (rgb[3] - transpareScale) < 256)
                    {
                        rgb[3] = (byte)(rgb[3] - transpareScale);
                    }
                    else
                    {
                        rgb[3] = 255;
                    }
                    int pixelValue = BitConverter.ToInt32(rgb, 0);
                    wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i] = pixelValue;
                }
            }
            return wEraserBmp;
        }

        /// <summary>
        /// 獲取不透明（即橡皮）畫刷圖像
        /// </summary>
        /// <param name="wEraserBmp"></param>
        /// <param name="transpareScale"></param>
        /// <returns></returns>
        private WriteableBitmap GetOpacityWbmp(WriteableBitmap wEraserBmp, int transpareScale)
        {
            for (int i = 0; i < wEraserBmp.PixelWidth; i++)
            {
                for (int j = 0; j < wEraserBmp.PixelHeight; j++)
                {
                    byte[] rgb = BitConverter.GetBytes(wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i]);
                    // rgb[3] = Convert.ToByte(255 - rgb[3]);    //获取粉刷的不透明度255 - rgb[3]
                    if ((rgb[3] - transpareScale) >= 0 && (rgb[3] - transpareScale) < 256)
                    {
                        rgb[3] = (byte)(rgb[3] - transpareScale);
                    }
                    else
                    {
                        rgb[3] = 0;
                    }
                    int pixelValue = BitConverter.ToInt32(rgb, 0);
                    wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i] = pixelValue;
                }
            }
            return wEraserBmp;
        }

        /// <summary>
        /// 獲取不透明最大值,值越大，越不透明
        /// </summary>
        /// <param name="wEraserBmp"></param>
        /// <returns></returns>
        private int GetHeightValue(WriteableBitmap wEraserBmp)
        {
            int lowestValue = 255;
            for (int i = 0; i < wEraserBmp.PixelWidth; i++)
            {
                for (int j = 0; j < wEraserBmp.PixelHeight; j++)
                {
                    byte[] rgb = BitConverter.GetBytes(wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i]);
                    if (lowestValue < rgb[3])
                    {
                        lowestValue = rgb[3];
                    }
                }
            }

            return lowestValue;
        }

    }
}
