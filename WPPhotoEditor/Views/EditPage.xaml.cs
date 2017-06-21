using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WPPhotoEditor.Controls;
using WPPhotoEditor.Media;
using Yuweiz.Phone.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using WPPhotoEditor.Resources;
using Yuweiz.Phone.Common.InnerNavigation;

namespace WPPhotoEditor.Views
{
    public partial class EditPage : PhoneApplicationPage
    {
        public enum ToolTypeEnum { Move, Pen, TranspareTool }

        static bool hadNavigated = false;

        public EditPage()
        {
            InitializeComponent();
            this.Loaded += EditPage_Loaded;
            this.layersCanvas.MouseLeftButtonDown += layersCanvas_MouseLeftButtonDown;
            this.ucLayersBox.LayerAdded += ucLayersBox_LayerAdded;
            this.ucLayersBox.LayerDeleted += ucLayersBox_LayerDeleted;
            this.ucLayersBox.LayerSelected += ucLayersBox_LayerSelected;
            this.ucLayersBox.LayerSetTop += ucLayersBox_LayerSetTop;
            this.ucLayersBox.LayerVisialbeChanged += ucLayersBox_LayerVisialbeChanged;
            this.ucLayersBox.rbtLayerMerge.Click += rbtLayerMerge_Click;

            if (AppSession.Instance.IsShowAds)
            {             
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel1;
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.StartAds();
            }

            toolTypeEnum = ToolTypeEnum.Pen;
        }

        void EditPage_Loaded(object sender, RoutedEventArgs e)
        {
            Yuweiz.Phone.Common.InnerNavigation.InnerNavigationService.RootFrame = App.RootFrame;

            if (AppSession.Instance.ChosenColor != null)
            {
                this.ucPenStyleBox.rbtColor.Background = new SolidColorBrush(AppSession.Instance.ChosenColor.Value);
                this.ucPenStyleBox.UpdateBrush();
                AppSession.Instance.ChosenColor = null;                   
            }
        }

        void ucLayersBox_LayerVisialbeChanged(int arg1, bool arg2)
        {
            this.layersCanvas.ChangeLayerVisibility(arg1);
        }

        void rbtLayerMerge_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(AppResources.EMergeLayersConfirm, AppResources.CLClearImageConfirm, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                WriteableBitmap wbmp = this.layersCanvas.MergeVisibleLayers();
                this.ucLayersBox.MergeLayers(wbmp);
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            #region 关闭选项
            bool hadHide = ShowUCControl(null);
            if (hadHide)
            {
                e.Cancel = true;
            }
            #endregion

            #region 退出提醒
            if (!e.Cancel)
            {
                e.Cancel = true;
                if (!isWaitingDeparture)
                {
                    isWaitingDeparture = true;
                    AppSession.ShowToastMessage(AppResources.GlLeaveCollage, AppResources.GlPressTheBackKeyAgain);

                    Thread leaveThread = new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(2000);
                        isWaitingDeparture = false;

                    }));//创建一个线程  
                    leaveThread.Start();//启动  
                }
                else
                {
                    e.Cancel = false;
                }
            }
            #endregion
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            UmengSDK.UmengAnalytics.TrackPageEnd("EditBasePage");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UmengSDK.UmengAnalytics.TrackPageStart("EditBPage");

            if (AppSession.Instance.CurEditingBitmapImage != null)
            {
                if (this.layersCanvas.CurrentLayer != null)
                {
                    //this.ucLayerToolBar.LayerBase.Background = new ImageBrush() { ImageSource = AppSession.Instance.CurEditingBitmapImage, Stretch = Stretch.UniformToFill };
                    this.layersCanvas.CurrentLayer.BackgroundWbmp = AppSession.Instance.CurEditingBitmapImage;
                }
                AppSession.Instance.CurEditingBitmapImage = null;
            }
            else
            {
                BrushFactory.Instance.InitializeDefaultTransparencyBrush(FillPixelTypeEnum.Color);

                if (!hadNavigated)
                {
                    this.ucLayersBox.AddImageLayer(null);
                    hadNavigated = true;
                }
            }

            this.layersCanvas.Background = AppSession.Instance.CanvasBackground;
        }

        bool isWaitingDeparture = false;

        ToolTypeEnum toolTypeEnum {

            get
            {
                //#7E898989
                if ((this.rbtMove.Background as SolidColorBrush).Color.R == 137)
                {
                    return ToolTypeEnum.Move;
                }
                else if ((this.rbtPen.Background as SolidColorBrush).Color.R == 137)
                {
                    return ToolTypeEnum.Pen;
                }
                else 
                {
                    return ToolTypeEnum.TranspareTool;
                }
            }
            set
            {
                //#7E0C0C0C
                this.rbtMove.Background = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#7E0C0C0C"));
                this.rbtPen.Background = this.rbtMove.Background;
                this.rbtTransparentPen.Background = this.rbtMove.Background;
                switch (value)
                {
                    case ToolTypeEnum.Move:
                        this.rbtMove.Background = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#7E898989"));
                        break;
                    case ToolTypeEnum.Pen:
                        this.rbtPen.Background = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#7E898989"));
                        break;
                    case ToolTypeEnum.TranspareTool:
                        this.rbtTransparentPen.Background = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#7E898989"));
                        break;
                }
            }
        }

        void ucLayersBox_LayerSetTop(int obj)
        {
            if (this.layersCanvas == null)
            {
                return;
            }

            this.layersCanvas.SetTopLayer(obj);
        }

        void ucLayersBox_LayerSelected(int obj)
        {
            this.layersCanvas.SelectLayer(obj);
        }

        void ucLayersBox_LayerDeleted(int obj)
        {
            this.layersCanvas.DeleteLayer(obj);
        }

        void ucLayersBox_LayerAdded(int arg1, object arg2)
        {
            this.layersCanvas.AddLayer(arg2 as BitmapSource);
            this.layersCanvas.BindDrawToolInstance();

            if (this.layersCanvas.Children.Count == 2&&AppSession.Instance.SettingsModel.IsShowHelp)
            {
                this.NavigateTo<Controls.GuideTipPage>();
                AppSession.Instance.SettingsModel.IsShowHelp = false;
            }
        }


        /// <summary>
        /// 如果有关闭控件，则返回True
        /// </summary>
        /// <param name="ucontrol"></param>
        /// <returns></returns>
        private bool ShowUCControl(UserControl ucontrol)
        {
            bool hadHide = false;
            foreach (object control in this.LayoutRoot.Children)
            {
                if (control is IBox)
                {
                    IBox iBox = control as IBox;
                    if (iBox != null)
                    {
                        if (iBox == ucontrol as IBox)
                        {
                            iBox.ChangeVisibility();
                        }
                        else
                        {
                            bool b = iBox.Hide();
                            if (!hadHide)
                            {
                                hadHide = b;
                            }
                        }
                    }
                }
            }

            return hadHide;
        }

        void layersCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.ucLayersBox.Visibility == Visibility.Collapsed)
            {
                ShowUCControl(null);
            }

        }

        #region 工具

        private void rbtLayer_Click(object sender, RoutedEventArgs e)
        {
            this.ShowUCControl(this.ucLayersBox);
            if (this.ucLayersBox.Visibility == Visibility.Visible)
            {
                this.ucLayersBox.UpdateCurLayerPreview(this.layersCanvas.CurrentLayer.BackgroundWbmp);
            }
        }


        private void rbtTransparentPen_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.ucPenStyleBox.Visibility == Visibility.Visible && !this.ucPenStyleBox.IsTransparentOpacityBoxType))
            {
                ShowUCControl(this.ucPenStyleBox);
            }
            this.ucPenStyleBox.IsTransparentOpacityBoxType = true;
            toolTypeEnum = ToolTypeEnum.TranspareTool;

            this.layersCanvas.BindDrawToolInstance();
        }

        private void rbtPen_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.ucPenStyleBox.Visibility == Visibility.Visible && this.ucPenStyleBox.IsTransparentOpacityBoxType))
            {
                ShowUCControl(this.ucPenStyleBox);
            }
            this.ucPenStyleBox.IsTransparentOpacityBoxType = false;

            toolTypeEnum = ToolTypeEnum.Pen;
        }


        #endregion

        private void rbtClear_Click(object sender, RoutedEventArgs e)
        {
            if (this.ucPenStyleBox.IsTransparentOpacityBoxType)
            {
                var msgResult = MessageBox.Show(AppResources.CLClearOpacityMaskConfirm, string.Empty, MessageBoxButton.OKCancel);
                if (msgResult != MessageBoxResult.OK)
                {
                    return;
                }

                this.layersCanvas.ClearOpacityMask();
            }
            else
            {
                var msgResult = MessageBox.Show(AppResources.CLClearImageConfirm, string.Empty, MessageBoxButton.OKCancel);
                if (msgResult != MessageBoxResult.OK)
                {
                    return;
                }

                this.layersCanvas.ClearDrawed();
            }
        }

        private void rbtOverflow_Click(object sender, RoutedEventArgs e)
        {
            this.lsTools.Height = this.lsTools.Height == 63 ? 83 : 63;
        }

        private void rbtRedo_Click(object sender, RoutedEventArgs e)
        {
            this.layersCanvas.Redo();
        }

        private void rbtUndo_Click(object sender, RoutedEventArgs e)
        {
            this.layersCanvas.Undo();
        }

        private void rbtFilter_Click(object sender, RoutedEventArgs e)
        {
            if (this.layersCanvas.CurrentLayer == null)
            {
                return;
            }

            ImageBrush ib = this.layersCanvas.CurrentLayer.Background as ImageBrush;
            if (ib == null || ib.ImageSource == null)
            {
                MessageBox.Show("No image.");
                return;
            }

            WriteableBitmap bmp = (this.layersCanvas.CurrentLayer.Background as ImageBrush).ImageSource as WriteableBitmap;

            AppSession.Instance.CurEditingBitmapImage = bmp;

            AffectPage.HadNavigated = true;
            PhoneApplicationFrame rootFrame = App.RootFrame;
            rootFrame.Navigate(new Uri("/Views/AffectPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void rbtMove_Click(object sender, RoutedEventArgs e)
        {
            this.layersCanvas.BindTransformTool();
            toolTypeEnum = ToolTypeEnum.Move;
        }

        private void rbtSave_Click(object sender, RoutedEventArgs e)
        {
            bool canSaved = Common.LicenseManage.CanExportLicense();
            if (!canSaved)
            {
                return;
            }

            AppSession.Instance.LicenseModel.ExportJPGCount++;

            WriteableBitmap wbmp = new WriteableBitmap(this.layersCanvas, new System.Windows.Media.ScaleTransform() { ScaleX = AppSession.Instance.SavedScale, ScaleY = AppSession.Instance.SavedScale });
            Yuweiz.Phone.IO.MediaLibraryDAL.Instance.SavePicture(wbmp);

            MessageBox.Show(WPPhotoEditor.Resources.AppResources.GlImageHaveSavedTo);
        }

        private void rbtWords_Click(object sender, RoutedEventArgs e)
        {
          
            WriteableBitmap wbmp = new WriteableBitmap(this.layersCanvas, new System.Windows.Media.ScaleTransform() { ScaleX = AppSession.Instance.SavedScale, ScaleY = AppSession.Instance.SavedScale });
            
            AppSession.Instance.CurEditingBitmapImage = wbmp;
            PhoneApplicationFrame rootFrame = App.RootFrame;
            rootFrame.Navigate(new Uri("/Views/MovieFramePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void rbtSetting_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SettingPage.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}