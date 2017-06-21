using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using WPPhotoEditor.Media;
using Coding4Fun.Toolkit.Controls;
using System.Threading;
using System.Windows.Media;
using System.IO;
using WPPhotoEditor.Resources;

namespace WPPhotoEditor.Views
{
    public partial class CollagePage : PhoneApplicationPage
    {
        public CollagePage()
        {
            InitializeComponent();

            this.InitializeLayerToolBar();

            BuildLocalizedApplicationBar();

            this.ucPenStyleBox.IsTransparentOpacityBoxType = true;
        }


        // 用于生成本地化 ApplicationBar 的示例代码      
        private void BuildLocalizedApplicationBar()
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.CPUndo;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.CPPen;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.CPDrag;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.CPAdd;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.CPRedo;
            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.CPSave;
            (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).Text = AppResources.CPSetting;
        }


        private bool isWaitingDeparture;

        public static bool HadNavigated { get; set; }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            #region 退出画笔选项

            if (this.ucPenStyleBox.Visibility == Visibility.Visible)
            {
                this.ucPenStyleBox.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }

            #endregion

            #region 退出图片编辑工具
            bool quit = this.QuitLayerToolBar();
            if (quit)
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
                    ToastPrompt toast = new ToastPrompt();
                    toast.MillisecondsUntilHidden = 2000;
                    toast.Message = WPPhotoEditor.Resources.AppResources.GlPressTheBackKeyAgain;
                    toast.Title = WPPhotoEditor.Resources.AppResources.GlLeaveCollage;
                    toast.Show();

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
            UmengSDK.UmengAnalytics.TrackPageEnd("CollagePage");
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackPageStart("CollagePage");

            base.OnNavigatedTo(e);

            if (!HadNavigated)
            {
                this.ApplicationBarMenuItemTakePhoto_Click(null, null);
                BrushFactory.Instance.InitializeDefaultTransparencyBrush();
            }
            HadNavigated = true;


            if (AppSession.Instance.CurEditingBitmapImage != null)
            {
                if (this.ucLayerToolBar.LayerBase != null)
                {
                    //this.ucLayerToolBar.LayerBase.Background = new ImageBrush() { ImageSource = AppSession.Instance.CurEditingBitmapImage, Stretch = Stretch.UniformToFill };
                    this.ucLayerToolBar.LayerBase.BackgroundWbmp = AppSession.Instance.CurEditingBitmapImage;
                }
                AppSession.Instance.CurEditingBitmapImage = null;
            }


            this.layersCanvas.Background = AppSession.Instance.CanvasBackground;
        }

        private void InitializeLayerToolBar()
        {
            this.ucLayerToolBar.Visibility = Visibility.Collapsed;
            this.ucLayerToolBar.LayerCanvaslBase = this.layersCanvas;
            this.layersCanvas.ShowLayerToolAction = (layer) =>
            {
                if (this.ucLayerToolBar.Visibility == Visibility.Visible)
                {
                    this.ucLayerToolBar.ShowLayerChangedStoryBoard();
                }
                else
                {
                    this.ucLayerToolBar.Visibility = Visibility.Visible;
                }

                if (this.ucLayerToolBar.LayerBase != null)
                {
                    this.ucLayerToolBar.LayerBase.IsSelected = false;
                }
                this.ucLayerToolBar.LayerBase = layer;
                layer.IsSelected = true;
            };
            this.layersCanvas.CheckToShowLayerToolAction = (layer) =>
            {
                if (this.ucLayerToolBar.Visibility == Visibility.Visible)
                {
                    this.ucLayerToolBar.ShowLayerChangedStoryBoard();
                    if (this.ucLayerToolBar.LayerBase != null)
                    {
                        this.ucLayerToolBar.LayerBase.IsSelected = false;
                    }
                    this.ucLayerToolBar.LayerBase = layer;
                    layer.IsSelected = true;
                }
            };
        }

        /// <summary>
        /// 退出图层编辑状态
        /// </summary>
        /// <returns>True:如果有执行退出图层操作</returns>
        private bool QuitLayerToolBar()
        {
            if (this.ucLayerToolBar.Visibility == Visibility.Visible)
            {
                if (this.ucLayerToolBar.LayerBase != null)
                {
                    this.ucLayerToolBar.LayerBase.IsSelected = false;
                    this.ucLayerToolBar.LayerBase = null;
                }

                this.ucLayerToolBar.Visibility = Visibility.Collapsed;
                return true;
            }

            return false;
        }

        private void ApplicationBarMenuItemRedo_Click(object sender, EventArgs e)
        {
            this.layersCanvas.Redo();
        }
               
        private void ApplicationBarMenuItemSave_Click(object sender, EventArgs e)
        {
            bool canSaved = Common.LicenseManage.CanExportLicense();
            if (!canSaved)
            {
                return;
            }

            AppSession.Instance.LicenseModel.ExportJPGCount++;

            this.QuitLayerToolBar();

            WriteableBitmap wbmp = new WriteableBitmap(this.layersCanvas, new ScaleTransform() { ScaleX = AppSession.Instance.SavedScale, ScaleY = AppSession.Instance.SavedScale });
            Yuweiz.Phone.IO.MediaLibraryDAL.Instance.SavePicture(wbmp);

            //Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.SavePngPicture(wbmp);
            //Stream stream = Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.OpenFile();
            //BitmapImage bmp = new BitmapImage();
            //bmp.SetSource(stream);
            //wbmp = new WriteableBitmap(bmp);

            this.layersCanvas.AddLayer(wbmp);

            MessageBox.Show(WPPhotoEditor.Resources.AppResources.GlImageHaveSavedTo);
        }

        private void ApplicationBarMenuItemSetting_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SettingPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ApplicationBarMenuItemTakePhoto_Click(object sender, EventArgs e)
        {
            bool hadMemory = Yuweiz.Phone.Diagnostics.PerformanceAnalyzer.CheckMemoryAllow(5);
            if (!hadMemory)
            {
                MessageBox.Show("Sorry!! Memory Limited!");
                return;
            }

            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += photoChooserTask_Completed;

            photoChooserTask.Show();

        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //MessageBox.Show(e.OriginalFileName);

                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto); //获取返回的图片     
                WriteableBitmap wbmp = new WriteableBitmap(bmp);
                Size newSize = Yuweiz.Phone.Media.ViewControler.GetTheFitShowSize(new Size(1000, 1000), new Size(wbmp.PixelWidth, wbmp.PixelHeight));
                wbmp = WriteableBitmapExtensions.Resize(wbmp, (int)newSize.Width, (int)newSize.Height, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

                this.layersCanvas.AddLayer(wbmp);

                WPPhotoEditor.ViewModels.Utility.ShowHelp(WPPhotoEditor.Resources.AppResources.CPDoubleTapToShowToolBar,7000);
            }

            

        }

        private void ApplicationBarMenuItemUndo_Click(object sender, EventArgs e)
        {
            this.layersCanvas.Undo();
        }

        private void ApplicationBarMenuItemPenTool_Click(object sender, EventArgs e)
        {
            this.ucPenStyleBox.Visibility = this.ucPenStyleBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            this.layersCanvas.BindDrawTool();
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;     
        }

        private void ApplicationBarMenuItemTransfromTool_Click(object sender, EventArgs e)
        {
            this.ucPenStyleBox.Visibility =  Visibility.Collapsed;

            this.layersCanvas.BindTransformTool();

            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
        }

        private void layersCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.ucPenStyleBox.Visibility = Visibility.Collapsed;
        }
    }
}