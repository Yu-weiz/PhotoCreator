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
using Coding4Fun.Toolkit.Controls;
using System.Threading;
using WPPhotoEditor.UControls;
using WPPhotoEditor.Resources;

namespace WPPhotoEditor
{
    public partial class EditBasePage : PhoneApplicationPage
    {
        public EditBasePage()
        {
            InitializeComponent();

            this.ucLayersBox.LayerExchaged += ucLayersBox_LayerExchaged;
            this.ucLayersBox.LayerDeleted += ucLayersBox_LayerDeleted;
            this.ucLayersBox.LayerVisibilityChanged += ucLayersBox_LayerVisibilityChanged;
            this.ucLayersBox.LayerSelected += ucLayersBox_LayerSelected;

            this.isShowingLayersBox = false;

            if (this.layersCanvas.Children.Count < 1)
            {
                this.layersCanvas.AddLayer();
                this.layersCanvas.BindDrawTool();
                this.SetApplicationBarIconButtonEditingStatus(true);
            }

            BuildLocalizedApplicationBar();
        }


        // 用于生成本地化 ApplicationBar 的示例代码      
        private void BuildLocalizedApplicationBar()
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.PPLayers;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.CPUndo;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.CPStyle;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.CPDrag;

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.CPRedo;
            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.CPSave;
            (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).Text = AppResources.CPSetting;

        }



        private bool isShowingLayersBox
        {
            get { return this.ucLayersBox.Visibility == Visibility.Visible; }
            set
            {
                this.ucLayersBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

                string appBarKey = value ? "appBarLayerControl" : "appBarMain";
                this.ApplicationBar = this.Resources[appBarKey] as IApplicationBar;
            }
        }

        private bool isWaitingDeparture;

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (this.isShowingLayersBox)
            {
                this.isShowingLayersBox = false;
                e.Cancel = true;
                return;
            }

            if (ucBrushStyleBox.Visibility == Visibility.Visible)
            {
                ucBrushStyleBox.Visibility = Visibility.Collapsed;
                e.Cancel = true;
                this.ApplicationBar.IsVisible = true;
                return;
            }

            #region 退出提醒
            e.Cancel = true;
            if (!isWaitingDeparture)
            {
                //MessageBoxResult mResult = MessageBox.Show("确定要退出编辑？", "", MessageBoxButton.OKCancel);
                //{
                //    if (mResult == MessageBoxResult.Cancel)
                //    {
                //        e.Cancel = true;
                //    }
                //}

                isWaitingDeparture = true;
                ToastPrompt toast = new ToastPrompt();
                toast.MillisecondsUntilHidden = 2000;
                toast.Message = WPPhotoEditor.Resources.AppResources.GlPressTheBackKeyAgain;
                toast.Title = WPPhotoEditor.Resources.AppResources.GlLeavePaint;
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
            #endregion
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackPageEnd("EditBasePage");
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackPageStart("EditBasePage");
            base.OnNavigatedTo(e);
            this.layersCanvas.Background = AppSession.Instance.CanvasBackground;
        }


        void ApplicationBarIconButtonLayers_Click(object sender, EventArgs e)
        {
            this.isShowingLayersBox = !this.isShowingLayersBox;
            if (this.isShowingLayersBox)
            {
                this.layersCanvas.UpdateLayerItemsPreview(this.ucLayersBox.UpdateLayerItemPreview);

                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.ULMergeLayers;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.ULAddPicture;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.ULAddLayer;
            }
        }

        void ApplicationBarIconButtonPen_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn0 = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            if (btn0.Text == "Undo")
            {
                this.layersCanvas.Undo();
            }
            else
            {
                this.layersCanvas.BindDrawTool();
            }
                      
            this.SetApplicationBarIconButtonEditingStatus(true);          
        }

        void ApplicationBarIconButtonStyle_Click(object sender, EventArgs e)
        {
            this.layersCanvas.BindDrawTool();
            this.SetApplicationBarIconButtonEditingStatus(true);
            //NavigationService.Navigate(new Uri("/Views/BrushPickerPage.xaml", UriKind.RelativeOrAbsolute));
            ucBrushStyleBox.Visibility = Visibility.Visible;
            this.ApplicationBar.IsVisible = false;
        }

        void ApplicationBarIconButtonHand_Click(object sender, EventArgs e)
        {
            this.layersCanvas.BindTransformTool();
            this.SetApplicationBarIconButtonEditingStatus(false);
        }

        private void ApplicationBarMenuItemRedo_Click(object sender, EventArgs e)
        {
            this.layersCanvas.Redo();
        }
        
        private void SetApplicationBarIconButtonEditingStatus(bool isDrawing)
        {
            if (isDrawing)
            {
                ApplicationBarIconButton btn1 = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                btn1.IsEnabled = true;
                ApplicationBarIconButton btn2 = ApplicationBar.Buttons[3] as ApplicationBarIconButton;
                btn2.IsEnabled = true;

                ApplicationBarIconButton btn0 = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
                btn0.IconUri = new Uri("Assets/AppBar/Undo.png", UriKind.Relative);
                btn0.Text = "Undo";
            }
            else
            {
                ApplicationBarIconButton btn0 = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
                btn0.IconUri = new Uri("Assets/AppBar/BrushTool.png", UriKind.Relative);
                btn0.Text = "Pen";

                ApplicationBarIconButton btn1 = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
                btn1.IsEnabled = true;
                ApplicationBarIconButton btn2 = ApplicationBar.Buttons[3] as ApplicationBarIconButton;
                btn2.IsEnabled = false;
            }
        }
        
        void AddLayer_Click(object sender, EventArgs e)
        {
            bool hadMemory = Yuweiz.Phone.Diagnostics.PerformanceAnalyzer.CheckMemoryAllow(5);
            if (!hadMemory)
            {
                MessageBox.Show("Sorry!! Memory Limited!");
                return;
            }

            if (this.ucLayersBox.LayersCount > 6)
            {
                MessageBox.Show("Sorry!! We can only show 7 layers!");
                return;
            }

            this.ucLayersBox.AddLayer(null);
            this.layersCanvas.AddLayer();
        }

        void AddPicture_Click(object sender, EventArgs e)
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

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //MessageBox.Show(e.OriginalFileName);

                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto); //获取返回的图片     
                WriteableBitmap wbmp = new WriteableBitmap(bmp);
                Size newSize = Yuweiz.Phone.Media.ViewControler.GetTheFitShowSize(new Size(2000, 2000), new Size(wbmp.PixelWidth, wbmp.PixelHeight));
                wbmp = WriteableBitmapExtensions.Resize(wbmp, (int)newSize.Width, (int)newSize.Height, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

                this.ucLayersBox.AddLayer(wbmp);
                this.layersCanvas.AddLayer(wbmp);

                //this.isShowingLayersBox = false;
            }
        }

        private void ApplicationBarMenuItemSave_Click(object sender, EventArgs e)
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

        private void ApplicationBarMenuItemSetting_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SettingPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void UpdateAppBarStatus()
        {
            if (AppSession.Instance.HistoryManager.CanUndoCount > 0)
            {
                (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
            }
            else
            {
                (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
            }

            if (AppSession.Instance.HistoryManager.CanRedoCount > 0)
            {
                (this.ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = true;
            }
            else
            {
                (this.ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = false;
            }
        }

        #region 图层管理

        void ucLayersBox_LayerSelected(int obj)
        {
           // this.layersCanvas.SelectLayer(obj);
        }

        void ucLayersBox_LayerVisibilityChanged(int obj)
        {
            this.layersCanvas.ChangeLayerVisibility(obj);
        }

        void ucLayersBox_LayerDeleted(int obj)
        {
            this.layersCanvas.DeleteLayer(obj);
        }

        void ucLayersBox_LayerExchaged(int arg1, int arg2)
        {
            if (this.layersCanvas == null)
            {
                return;
            }

            this.layersCanvas.ExchageLayer(arg1, arg2);
        }

        void MergeLayers_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Merge the layers?", "Confirm", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                WriteableBitmap wbmp = this.layersCanvas.MergeVisibleLayers();
                this.ucLayersBox.MergeLayers(wbmp);
            }
        }

        #endregion

        
    }
}