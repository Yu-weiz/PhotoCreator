using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Foundation;

namespace WPPhotoEditor.Models
{
    public class FilterItemModel : INotifyPropertyChanged
    {
        public FilterItem FilterItem { get; set; }

        private WriteableBitmap filterSampleWbmp;

        public WriteableBitmap FilterSampleWbmp
        {
            get { return this.filterSampleWbmp; }

            set
            {
                this.filterSampleWbmp = value;
                this.NotifyPropertyChanged("FilterSampleWbmp");

                this.WbmpBg = new ImageBrush() { ImageSource = this.filterSampleWbmp, Stretch = Stretch.UniformToFill };
                this.NotifyPropertyChanged("WbmpBg");
            }
        }

        private int index;

        public ImageBrush WbmpBg { get; set; }

        public int Index
        {
            get { return index; }
            set
            {
                index = value;
            }
        }

        public async Task<bool> UpdateSample(string picPath)
        {
            if (this.FilterItem == null || picPath == null)
            {
                return false;
            }

            //Stream _imageSource = App.GetResourceStream(new Uri("Sample.jpg", UriKind.Relative)).Stream;
            //BitmapImage bmp = new BitmapImage();
            //bmp.SetSource(_imageSource);
            //this.FilterSampleWbmp = new WriteableBitmap(bmp);

            using (Stream stream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(picPath))
            {
                WriteableBitmap writeableBitmap = new WriteableBitmap(100, 100);

                // 为示例图片添加相应的滤镜效果
                using (var source = new StreamImageSource(stream))
                using (var filterEffect = new FilterEffect(source) { Filters = FilterItem.CreateInstance(FilterItem.Index).ToArray() })
                using (var renderer = new WriteableBitmapRenderer(filterEffect, writeableBitmap))
                {
                    await renderer.RenderAsync();
                }

                stream.Close();
                stream.Dispose();

                this.FilterSampleWbmp = new WriteableBitmap(writeableBitmap);
            }
            return true;
        }

        [Obsolete("过时，无效！仅保留参考")]
        private async Task<WriteableBitmap> Affect(string picPath)
        {
            WriteableBitmap writeableBitmap = null;

            if (this.FilterItem == null || picPath == null)
            {
                return writeableBitmap;
            }

            using (Stream stream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(picPath))
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(stream);
                writeableBitmap = new WriteableBitmap(bmp);

                // 为示例图片添加相应的滤镜效果
                using (var source = new StreamImageSource(stream))
                using (var filterEffect = new FilterEffect(source) { Filters = FilterItem.CreateInstance(FilterItem.Index).ToArray() })
                using (var renderer = new WriteableBitmapRenderer(filterEffect, writeableBitmap))
                {
                    await renderer.RenderAsync();
                }
            }
            return writeableBitmap;
        }

        #region 实现接口
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
