using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WPPhotoEditor.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPPhotoEditor.UControls
{
    public partial class UCLayerToolBar : UserControl
    {
        public UCLayerToolBar()
        {
            InitializeComponent();

            this.btnAffect.Click += btnAffect_Click;
            this.btnUp.Click += btnUp_Click;
            this.btnDown.Click += btnDown_Click;
            this.btnDelete.Click += btnDelete_Click;


            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel4;
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.StartAds();
            }         
        }

        /// <summary>
        /// 工具对应的的处理图层
        /// </summary>
        public LayerBase LayerBase { get; set; }

        /// <summary>
        /// 工具对应的操作画板
        /// 画板包含图层
        /// </summary>
        public LayerCanvaslBase LayerCanvaslBase { get; set; }

        public void ShowLayerChangedStoryBoard()
        {
            Storyboard sBoard = this.FindName("StoryboardLayerChange") as Storyboard;
            sBoard.Begin();
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.LayerBase == null || this.LayerCanvaslBase == null)
            {
                return;
            }

        // MessageBoxResult result   MessageBox.Show("Really want to delete this Layer?");

            this.LayerCanvaslBase.DeleteLayer(this.LayerBase);

            this.LayerBase = null;
            this.Visibility = Visibility.Collapsed;
        }

        void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.LayerBase == null || this.LayerCanvaslBase == null)
            {
                return;
            }

            this.LayerCanvaslBase.MoveDown(this.LayerBase);
        }

        void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.LayerBase == null || this.LayerCanvaslBase == null)
            {
                return;
            }

            this.LayerCanvaslBase.MoveUp(this.LayerBase);
        }

        void btnAffect_Click(object sender, RoutedEventArgs e)
        {
            if (this.LayerBase == null || this.LayerCanvaslBase == null)
            {
                return;
            }

            WriteableBitmap bmp = (this.LayerBase.Background as ImageBrush).ImageSource as WriteableBitmap;

            AppSession.Instance.CurEditingBitmapImage = bmp;

            AffectPage.HadNavigated = true;
            PhoneApplicationFrame rootFrame = App.RootFrame;
            rootFrame.Navigate(new Uri("/Views/AffectPage.xaml", UriKind.RelativeOrAbsolute));

        }
    }
}
