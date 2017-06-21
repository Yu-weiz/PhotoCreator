using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;

using Nokia.Graphics.Imaging;
using WPPhotoEditor.ViewModels;
using WPPhotoEditor.Models;
using System.IO;
using Yuweiz.Phone.Controls;
using System.Threading.Tasks;
using System.Threading;
using WPPhotoEditor.Resources;

namespace WPPhotoEditor
{
    public partial class AffectPage : PhoneApplicationPage
    {
        public AffectPage()
        {
            InitializeComponent();


            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel4;
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.StartAds();
            }

            this.Loaded += AffectPage_Loaded;

            InitializeSizeAndProBox();

            // 用于本地化 ApplicationBar 的示例代码
            BuildLocalizedApplicationBar();
        }


        // 用于生成本地化 ApplicationBar 的示例代码      
        private void BuildLocalizedApplicationBar()
        {   
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.APResume;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.APCrop;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.CPSave;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.APChangeThePicture;
            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.APCropProportion;
            (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).Text = AppResources.APNewSize;
            (ApplicationBar.MenuItems[3] as ApplicationBarMenuItem).Text = AppResources.APRotate;
            (ApplicationBar.MenuItems[4] as ApplicationBarMenuItem).Text = AppResources.APFlipVertical;
            (ApplicationBar.MenuItems[5] as ApplicationBarMenuItem).Text = AppResources.APFlipHorizontal;
         }

        private AffectPageViewModel viewModel = new AffectPageViewModel();

        public static bool HadNavigated { get; set; }

        private static bool? isNeedToLeave;

        private bool isAffecting = false;

        private bool isWaitingDeparture;

        protected async override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (this.grNewSize.Visibility == Visibility.Visible || this.grProportion.Visibility == Visibility.Visible)
            {
                this.grProportion.Visibility = Visibility.Collapsed;
                this.grNewSize.Visibility = Visibility.Collapsed;
                e.Cancel = true;
                return;
            }


            if (this.ucCutBorder.IsCropState)
            {
                this.ucCutBorder.IsCropState = false;
                e.Cancel = true;
                return;
            }

            if (!e.Cancel && !isWaitingDeparture)
            {
                e.Cancel = true;
                MessageBoxExt.Instance.Message = WPPhotoEditor.Resources.AppResources.GlDoYouWant;
                MessageBoxExt.Instance.SaveString = WPPhotoEditor.Resources.AppResources.CPSave;
                MessageBoxExt.Instance.CancelString = WPPhotoEditor.Resources.AppResources.GlCancel;
                MessageBoxExt.Instance.LeaveString = WPPhotoEditor.Resources.AppResources.GlLeave;

                isWaitingDeparture = true;
                MessageBoxExtResult result = await MessageBoxExt.Instance.Show();
                if (result == MessageBoxExtResult.OK)
                {
                    AppSession.Instance.CurEditingBitmapImage = null;
                    NavigationService.GoBack();
                }
                else if (result == MessageBoxExtResult.DO)
                {
                    if (AppSession.Instance.CurEditingBitmapImage != null)
                    {
                        AppSession.Instance.CurEditingBitmapImage = this.ucCutBorder.Wbmap;
                    }
                    else
                    {
                        this.ApplicationBarMenuItemSave_Click(null, null);
                    }
                    NavigationService.GoBack();
                }
                else
                {
                    isWaitingDeparture = false;
                }
            }
            else
            {
                e.Cancel = true;
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UmengSDK.UmengAnalytics.TrackPageEnd("AffectPage");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackPageStart("AffectPage");

            base.OnNavigatedTo(e);
            FirstLoadCheckAddPicture();

            ///编辑其它页面过来的图像
            if (AppSession.Instance.CurEditingBitmapImage != null)
            {
                DealWithOtherPageBitmapImage(AppSession.Instance.CurEditingBitmapImage);
            }
            ///-----------

        }

        private void AffectPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (isNeedToLeave == null)
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    while (isNeedToLeave == null)
                    {
                        Thread.Sleep(100);
                    }

                    if (isNeedToLeave.Value)
                    {
                        Action action = delegate()  //后台中要更改主线程中的UI，于是我们还是用委托来实现，再创建一个实例
                        {
                            NavigationService.GoBack();
                        };//要调用的过程
                        this.Dispatcher.BeginInvoke(action); //使用分发器让这个委托等待执行
                    }
                }));
                thread.Start();
            }
            else if (isNeedToLeave.Value)
            {
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// 处理其它页面过来的图像
        /// </summary>
        /// <param name="bmp"></param>
        async void DealWithOtherPageBitmapImage(WriteableBitmap bmp)
        {
            this.SetAppBarEnable(false);
            this.lsFilterItem.ItemsSource = this.viewModel.FilterItemModelCollection;
            this.ucCutBorder.BitmapSource = bmp;
            int updatedCount = await this.viewModel.UpdateFilterSampleList();
            if (updatedCount >= AffectPageViewModel.FiltersCount)
            {
                this.SetAppBarEnable(true);
            }
        }

        async void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto); //获取返回的图片    

                this.progressOverlay.Show();
                this.ucCutBorder.BitmapSource = bmp;
                int updatedCount = await this.viewModel.UpdateFilterSampleList();
                if (updatedCount >= AffectPageViewModel.FiltersCount)
                {
                    this.SetAppBarEnable(true);
                    this.progressOverlay.Hide();
                }

                this.txtSizeWidth.Text = this.ucCutBorder.NewSize.Width.ToString();
                this.txtSizeHeight.Text = this.ucCutBorder.NewSize.Height.ToString();
            }
            else
            {
                NavigationService.GoBack();
            }
        }


        /// <summary>
        ///  用户单击滤镜列表中的滤镜项
        /// </summary>
        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            FilterItemModel model = (sender as Border).DataContext as FilterItemModel;
            if (model == null)
            {
                return;
            }

            this.viewModel.CurFilterItemModel = model;
            this.Affect(model);
        }

        /// <summary>
        /// 实施特效
        /// </summary>
        /// <param name="index">FilterItem.Index</param>
        private async void Affect(FilterItemModel model)
        {
            if (model == null || model.FilterItem == null || isAffecting)
            {
                return;
            }

            isAffecting = true;
            this.progressOverlay.Show();
            this.SetAppBarEnable(false);
            WriteableBitmap writeableBitmap = null;
            using (Stream _imageSource = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(AppSession.TempAffectBitmapPath))
            {
                writeableBitmap = new WriteableBitmap(this.ucCutBorder.BitmapSource.PixelWidth, this.ucCutBorder.BitmapSource.PixelHeight);

                // 为示例图片添加相应的滤镜效果
                using (var source = new StreamImageSource(_imageSource))
                using (var filterEffect = new FilterEffect(source) { Filters = FilterItem.CreateInstance(model.FilterItem.Index).ToArray() })
                using (var renderer = new WriteableBitmapRenderer(filterEffect, writeableBitmap))
                {
                    await renderer.RenderAsync();
                }

                _imageSource.Close();
                _imageSource.Dispose();
            }

            GC.Collect();
            // 显示到页面中      
            this.ucCutBorder.Wbmap = writeableBitmap;
            this.progressOverlay.Hide();
            isAffecting = false;
            this.SetAppBarEnable(true);
        }

        /// <summary>
        /// 实施特效
        /// </summary>
        /// <param name="index">FilterItem.Index</param>
        private async Task<WriteableBitmap> affect(string picPath, int effectIndex)
        {
            if (string.IsNullOrEmpty(picPath))
            {
                return null;
            }

            this.progressOverlay.Show();
            using (Stream _imageSource = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(picPath))
            {
                WriteableBitmap writeableBitmap = new WriteableBitmap(this.ucCutBorder.BitmapSource.PixelWidth, this.ucCutBorder.BitmapSource.PixelHeight);

                // 为示例图片添加相应的滤镜效果
                using (var source = new StreamImageSource(_imageSource))
                using (var filterEffect = new FilterEffect(source) { Filters = FilterItem.CreateInstance(effectIndex).ToArray() })
                using (var renderer = new WriteableBitmapRenderer(filterEffect, writeableBitmap))
                {
                    await renderer.RenderAsync();
                }

                // 显示到页面中             
                this.progressOverlay.Hide();
                return writeableBitmap;
            }

        }


        private void rbtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.lsFilterItem.ItemsSource = null;
            this.lsFilterItem.ItemsSource = this.viewModel.FilterItemModelCollection;
            using (Stream stream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile(AppSession.TempAffectBitmapPath))
            {
                BitmapImage img = new BitmapImage();
                img.SetSource(stream);
                //this.brMain.Background = new ImageBrush() { ImageSource=img};
                this.imgShow.Source = this.viewModel.FilterItemModelCollection[2].FilterSampleWbmp;
            }
        }

        private void ApplicationBarMenuItemResume_Click(object sender, EventArgs e)
        {
            this.ucCutBorder.ResumeBitmapSource();

            this.txtSizeWidth.Text = this.ucCutBorder.NewSize.Width.ToString();
            this.txtSizeHeight.Text = this.ucCutBorder.NewSize.Height.ToString();
        }

        private void ApplicationBarMenuItemCrop_Click(object sender, EventArgs e)
        {
            this.ucCutBorder.IsCropState = !this.ucCutBorder.IsCropState;
        }

        private void ApplicationBarMenuItemRotate_Click(object sender, EventArgs e)
        {
            this.progressOverlay.Show();

            this.ucCutBorder.ApplyRotate();

            this.progressOverlay.Hide();

        }

        private void ApplicationBarMenuItemFlipH_Click(object sender, EventArgs e)
        {
            this.progressOverlay.Show();

            this.ucCutBorder.ApplyFlipH();

            this.progressOverlay.Hide();

        }

        private void ApplicationBarMenuItemFlipV_Click(object sender, EventArgs e)
        {
            this.progressOverlay.Show();
            this.ucCutBorder.ApplyFlipV();
            this.progressOverlay.Hide();
        }

        private void ApplicationBarMenuItemTakePhoto_Click(object sender, EventArgs e)
        {
            AppSession.Instance.CurEditingBitmapImage = null;

            this.SetAppBarEnable(false);
            this.lsFilterItem.ItemsSource = this.viewModel.FilterItemModelCollection;
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.Show();
        }

        private void ApplicationBarMenuItemSave_Click(object sender, EventArgs e)
        {
            bool canSaved = Common.LicenseManage.CanExportLicense();
            if (!canSaved)
            {
                return;
            }

            AppSession.Instance.LicenseModel.ExportJPGCount++;

            //保存原图文件
            //WriteableBitmap wbmpOriginal = await affect(AppSession.TempAffectBitmapPath.Replace(".jpg", ""), this.viewModel.CurFilterItemModel.FilterItem.Index);
            //Yuweiz.Phone.IO.MediaLibraryDAL.Instance.SavePicture(wbmpOriginal);
            Yuweiz.Phone.IO.MediaLibraryDAL.Instance.SavePicture(this.ucCutBorder.Wbmap);
            MessageBox.Show(WPPhotoEditor.Resources.AppResources.GlImageHaveSavedTo);
                       
        }

        private void ApplicationBarMenuItemPro_Click(object sender, EventArgs e)
        {
            this.grProportion.Visibility = Visibility.Visible;
        }

        private void ApplicationBarMenuItemSize_Click(object sender, EventArgs e)
        {
            this.grNewSize.Visibility = Visibility.Visible;
        }


        private void SetAppBarEnable(bool isEnable)
        {
            foreach (ApplicationBarIconButton btn in this.ApplicationBar.Buttons)
            {
                btn.IsEnabled = isEnable;
            }

            foreach (ApplicationBarMenuItem menuItem in this.ApplicationBar.MenuItems)
            {
                menuItem.IsEnabled = isEnable;
            }
        }

        private void FirstLoadCheckAddPicture()
        {
            if (!HadNavigated)
            {
                isNeedToLeave = null;
                this.SetAppBarEnable(false);
                this.lsFilterItem.ItemsSource = this.viewModel.FilterItemModelCollection;
                PhotoChooserTask photoChooserTask = new PhotoChooserTask();
                photoChooserTask.ShowCamera = true;
                photoChooserTask.Completed += async (sender, ePhoto) =>
                {
                    BitmapImage bmp = new BitmapImage();
                    if (ePhoto.TaskResult == TaskResult.OK)
                    {
                        isNeedToLeave = false;
                        bmp.SetSource(ePhoto.ChosenPhoto); //获取返回的图片 
                        this.progressOverlay.Show();
                        this.ucCutBorder.BitmapSource = bmp;
                        int updatedCount = await this.viewModel.UpdateFilterSampleList();
                        if (updatedCount >= AffectPageViewModel.FiltersCount)
                        {
                            this.SetAppBarEnable(true);
                            this.progressOverlay.Hide();
                        }

                        this.txtSizeWidth.Text = this.ucCutBorder.NewSize.Width.ToString();
                        this.txtSizeHeight.Text = this.ucCutBorder.NewSize.Height.ToString();
                    }
                    else
                    {
                        isNeedToLeave = true;
                    }


                };

                photoChooserTask.Show();


            }
            HadNavigated = true;
        }


        private void InitializeSizeAndProBox()
        {
            this.btnProOK.Click += btnProOK_Click;
            this.btnSizeOK.Click += btnSizeOK_Click;
            this.ucCutBorder.ApplyCroped += () =>
            {
                this.txtSizeWidth.Text = this.ucCutBorder.NewSize.Width.ToString();
                this.txtSizeHeight.Text = this.ucCutBorder.NewSize.Height.ToString();
            };

            this.txtSizeWidth.LostFocus += (sender, e) =>
                {
                    if (this.chkKeepProportion.IsChecked.Value)
                    {
                        int sizeWidth = 10;
                        int sizeHeith = 10;
                        int.TryParse(this.txtSizeWidth.Text, out sizeWidth);
                        int.TryParse(this.txtSizeHeight.Text, out sizeHeith);

                        this.txtSizeHeight.Text = (this.ucCutBorder.NewSize.Height * sizeWidth / this.ucCutBorder.NewSize.Width).ToString("0");
                    }
                };

            this.txtSizeHeight.LostFocus += (sender, e) =>
            {
                if (this.chkKeepProportion.IsChecked.Value)
                {
                    int sizeWidth = 10;
                    int sizeHeith = 10;
                    int.TryParse(this.txtSizeWidth.Text, out sizeWidth);
                    int.TryParse(this.txtSizeHeight.Text, out sizeHeith);

                    this.txtSizeWidth.Text = (this.ucCutBorder.NewSize.Width * sizeHeith / this.ucCutBorder.NewSize.Height).ToString("0");
                }
            };


            this.ucCutBorder.Tap += (sender, e) =>
            {
                this.grProportion.Visibility = Visibility.Collapsed;
                this.grNewSize.Visibility = Visibility.Collapsed;
            };
        }

        void btnSizeOK_Click(object sender, RoutedEventArgs e)
        {
            int sizeWidth = 10;
            int sizeHeith = 10;

            int.TryParse(this.txtSizeWidth.Text, out sizeWidth);
            int.TryParse(this.txtSizeHeight.Text, out sizeHeith);

            if (sizeWidth > 10 && sizeHeith > 10)
            {
                this.grNewSize.Visibility = Visibility.Collapsed;
                this.ucCutBorder.ApplyNewSize(sizeWidth, sizeHeith);
            }
            else
            {
                this.txtSizeWidth.Text = this.ucCutBorder.NewSize.Width.ToString("0");
                this.txtSizeHeight.Text = this.ucCutBorder.NewSize.Height.ToString("0");
            }
        }

        void btnProOK_Click(object sender, RoutedEventArgs e)
        {
            double proWidth = 0;
            double proHeith = 0;

            double.TryParse(this.txtProWidth.Text, out proWidth);
            double.TryParse(this.txtProHeight.Text, out proHeith);

            if (proWidth == 0 || proHeith == 0)
            {
                this.ucCutBorder.CropProportion = 0.0;
                this.txtProWidth.Text = "";
                this.txtProHeight.Text = "";

            }
            else
            {
                this.ucCutBorder.CropProportion = proWidth / proHeith;
                this.grProportion.Visibility = Visibility.Collapsed;
            }

        }

    }
}