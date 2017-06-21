using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using WPPhotoEditor.Resources;

namespace WPPhotoEditor.Controls
{
    public partial class UCLayersBoxControl : UserControl, IBox
    {
        public UCLayersBoxControl()
        {
            InitializeComponent();
            this.ucLayerAdd.rbtLayerAdd.Click += rbtLayerAdd_Click;

            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel2;
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.StartAds();
            }
        }

        void rbtLayerAdd_Click(object sender1, RoutedEventArgs e1)
        {
            bool hadMemory = Yuweiz.Phone.Diagnostics.PerformanceAnalyzer.CheckMemoryAllow(5);
            if (!hadMemory)
            {
                MessageBox.Show("Sorry!! Memory Limited!");
                return;
            }

            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += (sender, e) =>
            {
                if (e.TaskResult == TaskResult.OK)
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.SetSource(e.ChosenPhoto); //获取返回的图片     
                    WriteableBitmap wbmp = new WriteableBitmap(bmp);
                    Size newSize = Yuweiz.Phone.Media.ViewControler.GetTheFitShowSize(new Size(1000, 1000), new Size(wbmp.PixelWidth, wbmp.PixelHeight));
                    wbmp = WriteableBitmapExtensions.Resize(wbmp, (int)newSize.Width, (int)newSize.Height, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

                    AddImageLayer(wbmp);
                }
            };

            photoChooserTask.Show();
        }

        public event Action<int, object> LayerAdded;

        public event Action<int> LayerDeleted;

        public event Action<int> LayerSetTop;

        public event Action<int> LayerSelected;

        public event Action<int, bool> LayerVisialbeChanged;

        public void AddImageLayer(BitmapSource bmp)
        {
            UCLayerItemControl layerItem = new UCLayerItemControl();
            layerItem.Hold += layerItem_Hold;
            layerItem.Tap += layerItem_Tap;
            layerItem.DoubleTap += layerItem_DoubleTap;
            layerItem.ImageSource = bmp;
            this.lsLayers.Items.Insert(this.lsLayers.Items.Count - 1, layerItem);
            if (LayerAdded != null)
            {
                LayerAdded(this.lsLayers.Items.Count - 2, bmp);
            }

            layerItem_Tap(layerItem, null);
        }

        void layerItem_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var msgResult = MessageBox.Show(AppResources.tDelLayerConfirm, string.Empty, MessageBoxButton.OKCancel);
            if (msgResult != MessageBoxResult.OK)
            {
                return;
            }

            if (this.lsLayers.Items.Count<3)
            {
                MessageBox.Show(AppResources.UCOneLayerShouldBeKept);
                return;
            }
           

            UCLayerItemControl item = sender as UCLayerItemControl;
            int index = this.lsLayers.Items.IndexOf(item);
            this.lsLayers.Items.RemoveAt(index);
            if (LayerDeleted != null)
            {
                LayerDeleted(index);
            }

             UCLayerItemControl layer =this.lsLayers.Items[this.lsLayers.Items.Count-2] as UCLayerItemControl;
             layerItem_Tap(layer, null);
        }

        void layerItem_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           // layerItem_Tap(sender, e);
            UCLayerItemControl item = sender as UCLayerItemControl;
            int index = this.lsLayers.Items.IndexOf(item);
            if (LayerSetTop != null)
            {
                LayerSetTop(index);
            }

            this.lsLayers.Items.Remove(item);
            this.lsLayers.Items.Insert(this.lsLayers.Items.Count - 1, item);
        }

        void layerItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            foreach (UserControl control in this.lsLayers.Items)
            {
                UCLayerItemControl item = control as UCLayerItemControl;
                if (item!=null)
                {
                    item.BorderBrush = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#D7FF980D"));
                    item.BorderThickness = new Thickness(1);
                }
            }

            UCLayerItemControl layer = sender as UCLayerItemControl;
            layer.BorderBrush = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#FFFF0D0D"));
            layer.BorderThickness = new Thickness(3);
            if (LayerSelected != null)
            {
                int index = this.lsLayers.Items.IndexOf(layer);
                LayerSelected(index);
            }

            this.rbtLayerVisibility.Background = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16(layer.IsVisible ? "#7E0C0C0C" : "#7EFFFFFF"));

        }

        public UCLayerItemControl GetSelectedItem()
        {
            foreach (UserControl control in this.lsLayers.Items)
            {
                UCLayerItemControl item = control as UCLayerItemControl;
                if (item != null && item.BorderThickness.Bottom==3)
                {
                    return item;
                }
            }
            return null;
        }

        public void UpdateCurLayerPreview(ImageSource imgSource)
        {
            foreach (UserControl control in this.lsLayers.Items)
            {
                UCLayerItemControl item = control as UCLayerItemControl;
                if (item != null&&item.BorderThickness.Bottom==3)
                {
                    item.ImageSource = imgSource;
                }
            }
        }

        //public void AddColorLayer(Color color)
        //{

        //}

        #region IBox 接口

        public void ChangeVisibility()
        {
            this.Visibility = this.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public bool Show()
        {
            bool hadShown = false;
            if (this.Visibility == Visibility.Visible)
            {
                hadShown = true;
            }

            this.Visibility = Visibility.Visible;

            return !hadShown;
        }

        public bool Hide()
        {
            bool hadHid = false;
            if (this.Visibility == Visibility.Collapsed)
            {
                hadHid = true;
            }

            this.Visibility = Visibility.Collapsed;

            return !hadHid;
        }

        #endregion

        private void rbtLayerVisibility_Click(object sender, RoutedEventArgs e)
        {
            var item = GetSelectedItem();
            item.IsVisible = !item.IsVisible;
            if (this.LayerVisialbeChanged != null)
            {
                this.LayerVisialbeChanged(this.lsLayers.Items.IndexOf(item), item.IsVisible);
            }
            this.rbtLayerVisibility.Background = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16(item.IsVisible ? "#7E0C0C0C" : "#7EFFFFFF"));

        }

        /// <summary>
        /// 合并可见图像
        /// </summary>
        /// <param name="bmp">合并后的图像</param>
        public void MergeLayers(BitmapSource bmp)
        {
            List<UCLayerItemControl> delList = new List<UCLayerItemControl>();
            foreach (FrameworkElement element in this.lsLayers.Items)
            {
                if (element is UCLayerItemControl)
                {
                    UCLayerItemControl item = element as UCLayerItemControl;
                   // if (item.IsVisible)
                    {
                        delList.Add(item);
                    }
                }
            }

            foreach (UCLayerItemControl item in delList)
            {
                this.lsLayers.Items.Remove(item);
            }

            this.AddImageLayer(bmp);
        }
    }
}
