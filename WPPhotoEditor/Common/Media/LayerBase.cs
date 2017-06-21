using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPPhotoEditor.Media.Tools;

namespace WPPhotoEditor.Media
{
    /// <summary>
    /// 图层
    /// </summary>
    public class LayerBase : Canvas
    {
        public LayerBase()
        {
            this.compositeTransform = new CompositeTransform();
            this.RenderTransform = this.compositeTransform;
            this.RenderTransformOrigin = new Point(0.5, 0.5);
           
            br = new Border();
            br.HorizontalAlignment = HorizontalAlignment.Stretch;
            br.VerticalAlignment = VerticalAlignment.Stretch;
            br.Margin = new Thickness(0, 0, 0, 0);
            br.BorderThickness = new Thickness(5, 5, 5, 5);          
            br.BorderBrush = new SolidColorBrush() { Color =AppSession.SecondaryColor };           
            this.Children.Add(br);

            this.IsSelected = false;
        }

        private Border br;

        private ToolAbstract tool;

        /// <summary>
        /// 每一图层均有一工具与之相对应
        /// </summary>
        public ToolAbstract Tool
        {
            get { return tool; }
            set
            {
                if (this.tool != null)
                {
                    this.tool.ReleaseLayer();
                }

                this.tool = value;
                if (this.tool != null)
                {
                    tool.BindLayer(this);
                }
            }
        }

        public bool IsSelected
        {
            get { return br.Visibility == Visibility.Visible; }
            set { br.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        private WriteableBitmap backgroundWbmp;

        public WriteableBitmap BackgroundWbmp
        {
            get { return backgroundWbmp; }
            set
            {
                this.backgroundWbmp = value;
                if (this.backgroundWbmp != null)
                {
                    ImageBrush imgBrush = new ImageBrush();
                    imgBrush.ImageSource = this.backgroundWbmp;
                    imgBrush.Stretch = Stretch.UniformToFill;
                    this.Background = imgBrush;

                    Size sz = Yuweiz.Phone.Media.ViewControler.GetTheFitShowSize(new Size(this.Width, this.Height), new Size(this.backgroundWbmp.PixelWidth, this.backgroundWbmp.PixelHeight));
                    this.Width = sz.Width;
                    this.Height = sz.Height;

                    this.br.Width = sz.Width;
                    this.br.Height = sz.Height;
                }
                else
                {
                    //如果没有图像则以白色填充
                    this.Background = new SolidColorBrush() { Color = Colors.White };
                }
            }
        }

        private CompositeTransform compositeTransform;

        /// <summary>
        /// 获取或设置当前图层的显示索引
        /// </summary>
        public int Index { get; set; }
    }
}
