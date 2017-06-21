using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Coding4Fun.Toolkit.Controls;

namespace WPPhotoEditor.UControls
{
    public partial class UCLayersBox : UserControl
    {
        public UCLayersBox()
        {
            InitializeComponent();

            this.layersControler = new LayersBoxControler(this.cavLayers);
            this.layersControler.LayerExchaged += layersControler_LayerExchaged;
            this.layersControler.LayerSelected += layersControler_LayerSelected;

            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel1;
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.StartAds();

                Canvas.SetLeft(this.imgTrash, 40);
                Canvas.SetTop(this.imgTrash, 526);
            }         

            this.InitializeDefault();
        }

        private void InitializeDefault()
        {
            this.AddLayer(null);
        }

        void layersControler_LayerExchaged(int arg1, int arg2)
        {
            if (this.LayerExchaged != null)
            {
                this.LayerExchaged(arg1, arg2);
            }
        }

        private LayersBoxControler layersControler;

        //指示当前的层是否碰到可视栏
        private bool hadHitVisiableBar;

        //指示当前的层是否碰到垃圾桶
        private bool hadHitTrash;

        public event Action<int> LayerDeleted;

        public event Action<int> LayerVisibilityChanged;

        public event Action<int, int> LayerExchaged;

        public event Action<int> LayerSelected;

        public int LayersCount
        {
            get { return this.layersControler.LayersCount; }
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <returns>返回新增图层的显示索引</returns>
        public int AddLayer(BitmapSource bmp)
        {
            UCLayerItem layer = new UCLayerItem();
            layer.LayerImage = bmp;

            layer.Moving += layer_Moving;
            layer.MouseReleased += layer_MouseReleased;
            layer.MouseLeftButtonDown += layer_MouseLeftButtonDown;

            int index = this.layersControler.AddLayer(layer);
            return index;
        }

        /// <summary>
        /// 合并可见图像
        /// </summary>
        /// <param name="bmp">合并后的图像</param>
        public void MergeLayers(BitmapSource bmp)
        {
            List<UCLayerItem> delList = new List<UCLayerItem>();
            foreach (FrameworkElement element in this.cavLayers.Children)
            {
                if (element is UCLayerItem)
                {
                    UCLayerItem item = element as UCLayerItem;
                    if (item.IsVisibleStatus)
                    {
                        delList.Add(item);
                    }                   
                }
            }

            this.AddLayer(bmp);

            foreach (UCLayerItem item in delList)
            {
                this.layersControler.DeleteLayer(item);
            }

        }

        /// <summary>
        /// 更新指定图层项的缩略图
        /// </summary>
        /// <param name="bmpSource"></param>
        public void UpdateLayerItemPreview(BitmapSource bmpSource, int index)
        {
            this.layersControler.UpdateLayerItemPreview(bmpSource, index);
        }


        void layersControler_LayerSelected(UCLayerItem obj)
        {
            if (obj == null)
            {
                return;
            }

            #region 设置图层可视状态栏的状态
            Color newColor = obj.IsVisibleStatus ? AppSession.MainColor : AppSession.LightGrap;
            this.grVisiableBar.Background = new SolidColorBrush(newColor);
            #endregion

            if (this.LayerSelected != null)
            {
                this.LayerSelected(obj.Index);
            }
        }

        void layer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hadHitVisiableBar = false;
            hadHitTrash = false;

            UCLayerItem ucLayerItem = sender as UCLayerItem;
            this.layersControler.SelectLayer(ucLayerItem);

            if (this.LayerSelected != null)
            {
                this.LayerSelected(ucLayerItem.Index);
            }
        }

        void layer_MouseReleased(object sender, EventArgs e)
        {
            UCLayerItem item = sender as UCLayerItem;
            if (!hadHitVisiableBar)
            {
                item.TurnBack();
            }
            else
            {
                #region 显示/隐藏图层
                item.IsVisibleStatus = !item.IsVisibleStatus;
                if (this.LayerVisibilityChanged != null)
                {
                    this.LayerVisibilityChanged(item.Index);
                }
                #endregion
            }

            #region 选择图层
            if (!item.IsVisibleStatus)
            {
                //智能选择非隐藏图层
                this.layersControler.SelectUnderLayer(item);
            }
            #endregion

            if (!hadHitTrash)
            {
                item.RenderTransform = null;
                this.layersControler.ReNewLayerPosition(sender as UCLayerItem);
            }
            else
            {
                #region 删除图层
                bool result = this.layersControler.DeleteLayer(item);
                if (!result)
                {
                    item.RenderTransform = null;
                    this.layersControler.ReNewLayerPosition(sender as UCLayerItem);
                }
                else
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Message = WPPhotoEditor.Resources.AppResources.UCALayerHadBeenDeleted;
                    toast.Show();

                    this.layersControler.SelectUnderLayer(item);

                    if (LayerDeleted != null)
                    {
                        LayerDeleted(item.Index);
                    }
                }
                #endregion
            }

            hadHitVisiableBar = false;
            hadHitTrash = false;
        }

        void layer_Moving(object sender, PoxEventArgs e)
        {
            UCLayerItem item = sender as UCLayerItem;
            Point? original = this.layersControler.GetAbsolutePosition(item);
            if (original != null)
            {
                double differenceY = e.Y - original.Value.Y;        //当前Y偏离原图层座位的距离
                double differenceStander = GetDistanceFromTrash(original.Value);
                double differenceVar = GetDistanceFromTrash(new Point(e.X, e.Y));
                double differenceScale = differenceVar / differenceStander * 1.5;
                differenceScale = differenceScale > 1 ? 1 : differenceScale;
                if (differenceY > 0)
                {
                    //向下移动：检测垃圾筒删除效果
                    hadHitTrash = Yuweiz.Phone.Controls.Utility.CheckCollision(this.imgTrash, item);
                    if (!hadHitTrash)
                    {
                        this.layersControler.CheckLayerExchageNeedWhenMoving(item, e);
                    }


                    //放在回收站，缩小效果
                    CompositeTransform compTransform = new CompositeTransform();
                    compTransform.ScaleX = differenceScale;
                    compTransform.ScaleY = differenceScale;
                    item.RenderTransform = compTransform;
                }
                else
                {
                    //向上移动：检测可视栏显示效果
                    hadHitVisiableBar = Yuweiz.Phone.Controls.Utility.CheckCollision(this.grVisiableBar, item);
                    if (!hadHitVisiableBar)
                    {
                        this.layersControler.CheckLayerExchageNeedWhenMoving(item, e);

                        #region 设置图层可视状态栏的原来颜色
                        Color newColor = item.IsVisibleStatus ? AppSession.MainColor : AppSession.LightGrap;
                        this.grVisiableBar.Background = new SolidColorBrush(newColor);
                        #endregion
                    }
                    else
                    {
                        #region 交换图层可视状态栏的原来颜色
                        Color newColor = item.IsVisibleStatus ? AppSession.LightGrap : AppSession.MainColor;
                        this.grVisiableBar.Background = new SolidColorBrush(newColor);
                        #endregion
                    }

                    //显示或隐藏，减淡或加深效果
                    int interval = (int)((Math.Abs(e.Y - original.Value.Y)) / (original.Value.Y + item.Height / 3) * 255);
                    interval = interval > 255 ? 255 : interval;
                    item.TurnThinner(interval);
                }

            }


        }

        private double GetDistanceFromTrash(Point curPt)
        {
            double distance = 0.0d;
            Point trashPt = new Point(Canvas.GetLeft(this.imgTrash) + this.imgTrash.Width / 2, Canvas.GetTop(this.imgTrash) + this.imgTrash.Height / 2);
            distance = Math.Sqrt(Math.Pow(curPt.X - trashPt.X, 2) + Math.Pow(curPt.Y - trashPt.Y, 2));
            return distance;
        }


    }

}
