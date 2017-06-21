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

namespace WPPhotoEditor.OLD.Entity
{
    public enum PenType { Original, Color, Transparent, Opacity }

    public enum ToolType { Edit, Clip, CanvasZoom, LayerZoom }

    public class BrushEntity
    {

        public BrushEntity()
        {
            initializeDefaultBrush();

            initializeDefaultBrush();
        }

        private void initializeDefaultBrush()
        {
            brushSize = new Size(100,100);
            brushOpacityValue = 100;
            brushColor = Color.FromArgb(50,151,241,26);
            brushImg = WPPhotoEditor.Utility.UtilityFunction.GetImgResource("brStar.png");
            brushPenType = PenType.Color;

            CreateTargetBrush();
        }

        private Size brushSize;

        private int brushOpacityValue;

        private Color brushColor;

        private ImageSource brushImg;
        
        private PenType brushPenType;

        private WriteableBitmap targetBrush;

        private ToolType layerToolType;


        public ToolType LayerToolType
        {
            get { return layerToolType; }
            set { layerToolType = value; }
        }
        

        public PenType BrushPenType
        {
            get { return brushPenType; }
            set { brushPenType = value; }
        }

        public Color BrushColor
        {
            get { return brushColor; }
            set { brushColor = value; }
        }

        public ImageSource BrushImg
        {
            get { return brushImg; }
            set { brushImg = value; }
        }

        public Size BrushSize
        {
            get { return brushSize; }
            set { brushSize = value; }
        }

        public WriteableBitmap TargetBrush
        {
            get { return targetBrush; }
        }

        public int BrushOpacityValue
        {
            get { return brushOpacityValue; }
            set { brushOpacityValue = value; }
        }

        public Button selButton
        {
            get;
            set;
        }

        /// <summary>
        /// 生成目標畫刷，並賦值到：TargetBrush
        /// </summary>
        public void CreateTargetBrush()
        {
            if (brushImg != null)
            {
                Image img = new Image();
                img.Width = brushSize.Width;
                img.Height = brushSize.Height;

                img.Stretch = Stretch.Uniform;
                img.Source = brushImg;
                // img.Opacity = 100;
                //ScaleTransform scale = new ScaleTransform() {ScaleX=173/bmp.PixelWidth,ScaleY=173/bmp.PixelHeight };


                // Canvas canvas = new Canvas();
                // canvas.Background = new SolidColorBrush(Colors.Transparent);
                //canvas.Width = brushSize.Width;
                //canvas.Height = brushSize.Height;
                //canvas.Children.Add(img);
                WriteableBitmap wb = new WriteableBitmap(img, null);
                if (PenType.Color == this.brushPenType)
                {
                    wb = colorBmp(wb, brushColor);
                }
                else if (PenType.Transparent == this.brushPenType)
                {
                    int lowestValue = 0;
                    wb = selfEraserBmp(wb, ref lowestValue);
                    int scale = lowestValue - (brushOpacityValue);
                    wb = setTransparetWbmp(wb, scale);
                }
                else if (PenType.Color == this.brushPenType)
                {
                    int HeightValue = getHeightValue(wb);
                    int scale = HeightValue - (255-brushOpacityValue);
                    wb = getOpacityWbmp(wb, scale);
                }

                targetBrush = wb;
            }


        }


        /// <summary>
        /// 生成對應顏色的畫刷
        /// </summary>
        /// <param name="wEraserBmp"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private WriteableBitmap colorBmp(WriteableBitmap wEraserBmp,Color color)
        {
           // return wEraserBmp;
            for (int i = 0; i < wEraserBmp.PixelWidth; i++)
            {
                for (int j = 0; j < wEraserBmp.PixelHeight; j++)
                {
                    byte[] rgb = BitConverter.GetBytes(wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i]);
                    if (rgb[3]== 255)
                    {
                        rgb[0] = color.B;
                        rgb[1] = color.G;
                        rgb[2] = color.R;                      
                    }
                    else
                    {
                        rgb[0] = 255;
                        rgb[1] = 255;
                        rgb[2] =255;
                        rgb[3] = 0;
                    }
                     
                    //rgb[3] = Convert.ToByte(255 - rgb[3]);
                    int pixelValue = BitConverter.ToInt32(rgb, 0);
                    wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i] = BitConverter.ToInt32(rgb, 0);
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
        private WriteableBitmap selfEraserBmp(WriteableBitmap wEraserBmp, ref int lowestValue)
        {
            lowestValue =255;
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
        private WriteableBitmap setTransparetWbmp(WriteableBitmap wEraserBmp, int transpareScale)
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
                    wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i] = BitConverter.ToInt32(rgb, 0);
                }
            }
            return wEraserBmp;
        }


        /// <summary>
        /// 獲取不透明最大值,值越大，越不透明
        /// </summary>
        /// <param name="wEraserBmp"></param>
        /// <returns></returns>
        private int getHeightValue(WriteableBitmap wEraserBmp)
        {
            int  lowestValue = 255;
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

        /// <summary>
        /// 獲取不透明（即橡皮）畫刷圖像
        /// </summary>
        /// <param name="wEraserBmp"></param>
        /// <param name="transpareScale"></param>
        /// <returns></returns>
        private WriteableBitmap getOpacityWbmp(WriteableBitmap wEraserBmp, int transpareScale)
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
                        rgb[3] =0;
                    }
                    int pixelValue = BitConverter.ToInt32(rgb, 0);
                    wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i] = BitConverter.ToInt32(rgb, 0);
                }
            }
            return wEraserBmp;
        }
    }
}
