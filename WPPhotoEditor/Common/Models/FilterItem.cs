using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPPhotoEditor.Models
{
    public class FilterItem
    {
        public int Index { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 引用当前用户选择的滤镜效果
        /// </summary>
        public static FilterItem CurrentFilterItem { get; set; }

        static StreamImageSource foregroundSource;

        /// <summary>
        /// 创建滤镜效果
        /// </summary>
        /// <param name="index">当 index 不为空时，设置 index 指定的滤镜，如果为空，则指定
        /// CurrentFilterItem.Index 指定的滤镜</param>
        /// <returns></returns>
        public static List<IFilter> CreateInstance(int? index = null)
        {
            var filters = new List<IFilter>();

            int filter_index = index != null ? (int)index : CurrentFilterItem.Index;

            if (foregroundSource == null)
            {
                foregroundSource = new StreamImageSource(App.GetResourceStream(new System.Uri("Assets/Image/foreground.png", System.UriKind.Relative)).Stream, ImageFormat.Png);
            }


            #region 初始化滤镜
            switch (filter_index)
            {
                case 0:
                    //filters.Add(new LomoFilter(0.5, 0.5, LomoVignetting.High, LomoStyle.Yellow));
                    break;
                case 1://古老式
                    filters.Add(new AntiqueFilter()); break;

                case 2://自动提高白平衡、亮度和对比度
                    filters.Add(new AutoEnhanceFilter(true, false)); break;

                case 3://平衡图片的强度级别，如使黑暗的图片更亮，反之亦然
                    filters.Add(new AutoLevelsFilter()); break;

                case 4://在一张原图片上混合另一张图片
                    filters.Add(new BlendFilter { ForegroundSource = foregroundSource, BlendFunction = BlendFunction.Normal });
                    break;

                case 5://指定级别（Range [1, 256]）的模糊，可以指定区域
                    filters.Add(new BlurFilter(10)); break;

                case 6: //调整图片的亮度（Range [-1.0, 1.0]，0 值没有影响）
                    filters.Add(new BrightnessFilter(0.5)); break;

                case 7://卡通样式
                    filters.Add(new CartoonFilter(true)); break;

                case 8: //给指定颜色的像素应用透明度
                    filters.Add(new ChromaKeyFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x00), 0.5d)); break;

                case 9://调整图片 RGB 颜色的组成
                    filters.Add(new ColorAdjustFilter(0.5, 0.5, 0.5)); break;

                case 10://放大图象的颜色。参数：颜色增益的水平（Range [-1.0, 20.0]），建议[-1.0, 1.0]
                    filters.Add(new ColorBoostFilter(0.5)); break;

                case 11://调整色调和亮度/色度的水平,增加一个单一的颜色到一个图像。
                    filters.Add(new ColorizationFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x99), .5, .5)); break;

                case 12://颜色参数的互换滤镜
                    filters.Add(new ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x99), Windows.UI.Color.FromArgb(0xff, 0x00, 0x99, 0x00), .5, true, true)); break;

                case 13://调整图片的对比度（Range [-1.0, 1.0]）
                    filters.Add(new ContrastFilter(0.5)); break;

                case 14://裁切图片的指定区域（Rect范围 range x: [0, min(16383, image
                    //     width)] y: [0, min(16383, image height)]）
                    filters.Add(new CropFilter(new Windows.Foundation.Rect(40, 40, 60, 80)));
                    break;

                case 15://使用颜色曲线转变图片的颜色
                    filters.Add(new CurvesFilter());
                    break;

                case 16://去除图片的噪点
                    filters.Add(new DespeckleFilter(DespeckleLevel.High));
                    break;

                case 17://在灰度级运用一个浮雕效果（Range: [0.0, 1.0]）
                    filters.Add(new EmbossFilter(0.5));
                    break;

                case 18://在一个曝光模式上调整图片的亮度
                    filters.Add(new ExposureFilter(ExposureMode.Natural, 1.0));
                    break;

                case 19://图片翻转
                    filters.Add(new FlipFilter(FlipMode.Both));
                    break;

                case 20://雾化滤镜，使图片变模糊
                    filters.Add(new FogFilter());
                    break;

                case 21://在图片的指定区域运用滤镜，比如面部
                    filters.Add(new FoundationFilter(new Windows.Foundation.Rect(0, 0, 50, 50)));
                    break;

                case 22://把图片转换为灰度模式
                    filters.Add(new GrayscaleFilter());
                    break;

                case 23://把图片转换为负灰度模式
                    filters.Add(new GrayscaleNegativeFilter());
                    break;

                case 24://调整图片的色彩、饱和度
                    filters.Add(new HueSaturationFilter(0.5, 0.5));
                    break;

                case 25://在当前图片上指定的 alpha通道 融合一张图片
                    // 纯色
                    ColorImageSource colorImageSource = new ColorImageSource(new Windows.Foundation.Size(80, 100), Windows.UI.Color.FromArgb(150, 0, 255, 0));
                    // 渐变色
                    var rad = new RadialGradient(new Windows.Foundation.Point(0.5, 0.5), new EllipseRadius(0.3, 0.3));
                    rad.Stops = new GradientStop[] 
                                    {
                                        new GradientStop() { Color = Windows.UI.Color.FromArgb(255, 0, 255, 0), Offset = 0 },
                                        new GradientStop() { Color = Windows.UI.Color.FromArgb(255, 255, 0, 0), Offset = 1 } 
                                    };

                    var grad = new GradientImageSource(new Windows.Foundation.Size(100, 100), rad);
                    filters.Add(new ImageFusionFilter(grad, colorImageSource, true));
                    break;

                case 26://饱和度水平（white、gray、black Range [0.0, 1.0]）
                    filters.Add(new LevelsFilter(0.5, 0.5, 0.5));
                    break;

                case 27://手动提升和增强图像的照明
                    filters.Add(new LocalBoostFilter(0.5, 0.5, 0.5, 0.5));
                    break;

                case 28://运用 Lomo 滤镜
                    filters.Add(new LomoFilter(0.5, 0.5, LomoVignetting.High, LomoStyle.Yellow));
                    break;

                case 29://混合图片的边缘和颜色的处理
                    filters.Add(new MagicPenFilter());
                    break;

                case 30://在图片的表面运用乳白色（浑浊）
                    filters.Add(new MilkyFilter());
                    break;

                case 31://复制图片的左半边，到右半边
                    filters.Add(new MirrorFilter());
                    break;

                case 32://保存一个色调而其他颜色转换为灰度图。
                    filters.Add(new MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.1));
                    break;

                case 33://指定时间（between 17 and 7），运用月色滤镜
                    filters.Add(new MoonlightFilter(10));
                    break;

                case 34://向负极转换图片
                    filters.Add(new NegativeFilter());
                    break;

                case 35://噪点滤镜
                    filters.Add(new NoiseFilter(NoiseLevel.Medium));
                    break;

                case 36://油画滤镜
                    filters.Add(new OilyFilter());
                    break;

                case 37://运用绘画滤镜（Range [1, 4]）
                    filters.Add(new PaintFilter(2));
                    break;

                case 38://应用多色调分色印效果,减少中图像颜色的数量。
                    filters.Add(new PosterizeFilter(8));
                    break;

                case 39://围绕轴心点，重新构造图片指定区域
                    filters.Add(new ReframingFilter(new Windows.Foundation.Rect(10, 10, 60, 60), 60));
                    break;

                case 40://根据指定旋转角度运用旋转滤镜
                    filters.Add(new RotationFilter(60));
                    break;

                case 41://运用棕色基调滤镜
                    filters.Add(new SepiaFilter());
                    break;

                case 42://调整图片锐度（Range [0, 7]）
                    filters.Add(new SharpnessFilter(4));
                    break;

                case 43://草图
                    filters.Add(new SketchFilter(SketchMode.Gray));
                    break;

                case 44://应用曝光过度效果。曝光的阀值（Range [0.0, 1.0]）
                    filters.Add(new SolarizeFilter(0.5));
                    break;

                case 45://在指定的范围应用一个给定颜色的象素亮度值,同时保留一些亮度信息
                    filters.Add(new SplitToneFilter(new List<SplitToneRange> { new SplitToneRange { Color = Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00) } }));
                    break;

                case 46://聚光灯滤镜
                    filters.Add(new SpotlightFilter(new Windows.Foundation.Point(50, 50), 100, 0.5));
                    break;

                case 47://应用邮票效果，产生黑白图片（Smoothness 平滑度：Range [0, 6]）（Threshold 阀值 Range [0.0, 1.0]）
                    filters.Add(new StampFilter(3, 0.5));
                    break;

                case 48://调整图片的色温、色彩（Temperature 色温 Range [-1.0, 1.0]）（Tint 色彩 Range [-1.0, 1.0]）
                    filters.Add(new TemperatureAndTintFilter(0.5, 0.5));
                    break;

                case 49://光晕滤镜（TransitionSize过度区域的大小 Range [0.0, 15.0]）（Color 过度区域的颜色）
                    filters.Add(new VignettingFilter(8, Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00)));
                    break;

                case 50://给图片（或一部分）运用变形滤镜 （WarpEffect：枚举值）（效果范围 Range [0.0, 1.0]）
                    filters.Add(new WarpFilter(WarpEffect.AlienHybrid, 0.5));
                    break;

                case 51://水彩滤镜（Light Intensity 光照强度 Range [0.0, 1.0]）（Color Intensity 颜色强度 Range [0.0, 1.0]）
                    filters.Add(new WatercolorFilter(0.5, 0.5));
                    break;

                case 52://白平衡（3个重载）
                    filters.Add(new WhiteBalanceFilter(WhitePointCalculationMode.Maximum, Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00)));
                    break;

                case 53://在一张白板上增加文字和图纸效果
                    filters.Add(new WhiteboardEnhancementFilter(WhiteboardEnhancementMode.Hard));
                    break;
            }
            #endregion

            return filters;
        }

        /// <summary>
        /// 滤镜列表数据源
        /// </summary>
        public static readonly List<FilterItem> FilterSource = new List<FilterItem> 
            {
                new FilterItem { Index = 0, Name = "NoEffect" },
                new FilterItem { Index = 1, Name = "Antique"},
                new FilterItem { Index = 2, Name = "AutoEnhance"},
                new FilterItem { Index = 3, Name = "AutoLevels"},
                new FilterItem { Index = 4, Name = "Blend"}, 
                new FilterItem { Index = 5, Name = "Blur"},
                new FilterItem { Index = 6, Name = "Brightness"},
                new FilterItem { Index = 7, Name = "Cartoon"},
                new FilterItem { Index = 8, Name = "ChromaKey"},
                new FilterItem { Index = 9, Name = "ColorAdjust"},
                new FilterItem { Index = 10, Name = "ColorBoost" },
                new FilterItem { Index = 11, Name = "Colorization"},
              
                new FilterItem { Index = 12, Name = "ColorSwap"},
                new FilterItem { Index = 13, Name = "Contrast"},
                new FilterItem { Index = 14, Name = "Crop"},
                new FilterItem { Index = 15, Name = "Curves"},
                new FilterItem { Index = 16, Name = "Despeckle"},
                new FilterItem { Index = 17, Name = "Emboss"},
                new FilterItem { Index = 18, Name = "Exposure"},
                new FilterItem { Index = 19, Name = "Flip"},
                new FilterItem { Index = 20, Name = "Fog"},
                new FilterItem { Index = 21, Name = "Foundation"},
                new FilterItem { Index = 22, Name = "Grayscale"},
                new FilterItem { Index = 23, Name = "GrayscaleNegative"},
                new FilterItem { Index = 24, Name = "HueSaturation"},
                new FilterItem { Index = 25, Name = "ImageFusion"},

                new FilterItem { Index = 26, Name = "Levels"},
                new FilterItem { Index = 27, Name = "LocalBoost"},
                new FilterItem { Index = 28, Name = "Lomo"},
                new FilterItem { Index = 29, Name = "MagicPen"},
                new FilterItem { Index = 30, Name = "Milky"},
                new FilterItem { Index = 31, Name = "Mirror"},
                new FilterItem { Index = 32, Name = "Mono"},
                new FilterItem { Index = 33, Name = "Moonlight"},
                new FilterItem { Index = 34, Name = "Negative"},
                new FilterItem { Index = 35, Name = "Noise"},
                new FilterItem { Index = 36, Name = "Oily"},
                new FilterItem { Index = 37, Name = "Paint"},
                new FilterItem { Index = 38, Name = "Posterize"},
                new FilterItem { Index = 39, Name = "Reframing"},
                new FilterItem { Index = 40, Name = "Rotation"},
                new FilterItem { Index = 41, Name = "Sepia"},
                new FilterItem { Index = 42, Name = "Sharpness"},
                new FilterItem { Index = 43, Name = "Sketch"},
                new FilterItem { Index = 44, Name = "Solarize"},

                new FilterItem { Index = 45, Name = "SplitTone"},
                new FilterItem { Index = 46, Name = "Spotlight"},
                new FilterItem { Index = 47, Name = "Stamp"},
                new FilterItem { Index = 48, Name = "TemperatureAndTint"},
                new FilterItem { Index = 49, Name = "Vignetting"},
                new FilterItem { Index = 50, Name = "Warp"},
                new FilterItem { Index = 51, Name = "Watercolor"},
                new FilterItem { Index = 52, Name = "WhiteBalance"},
                new FilterItem { Index = 53, Name = "WhiteboardEnhancement"}
            };
    }
}
