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
using System.Windows.Media.Imaging;
using System.Windows.Data;
using WPPhotoEditor.ViewModels.Tools;
using Yuweiz.Phone.Media.Imaging;
using WPPhotoEditor.UControls;
using WPPhotoEditor.Media;

namespace WPPhotoEditor.Controls
{
    public partial class UCBrushStyleBox : PhoneApplicationPage
    {
        public UCBrushStyleBox()
        {
            InitializeComponent();

            this.Loaded += BrushPickerPage_Loaded;
            this.colorPicker.ColorChanged += colorPicker_ColorChanged;

            if (AppSession.Instance.IsShowAds)
            {
                // this.grAd.Visibility = Visibility.Visible;
                // this.somaAd.StartAds();
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel4;
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.StartAds();
            }

            #region 笔工具
            this.ucPTColor.Tap += UCPenTool_Tap;
            this.ucPTOpacity.Tap += UCPenTool_Tap;
            this.ucPTTransparency.Tap += UCPenTool_Tap;
            this.ucPTOriginal.Tap += UCPenTool_Tap;
            #endregion
        }
        private static BrushSamples brushSamples;

        private bool needReflesh = false;

        private Button selButton;

        private double OpacityValue
        {
            get { return (255 - this.slOpacity.Value); }
            set { this.slOpacity.Value = (255 - value); }
        }

        private double SizeValue
        {
            get
            {
                int sizeValue = (int)this.slSize.Value;
                sizeValue = sizeValue % 2 == 0 ? sizeValue : sizeValue + 1;

                return sizeValue;
            }
            set
            {
                int sizeValue = (int)value;
                sizeValue = sizeValue % 2 == 0 ? sizeValue : sizeValue + 1;

                this.slSize.Value = sizeValue;
            }

        }


        void BrushPickerPage_Loaded(object sender, RoutedEventArgs e)
        {
            selButton = null;

            this.OpacityValue = BrushFactory.Instance.BrushOpacityValue;
            this.SizeValue = BrushFactory.Instance.BrushSize.Width;
            this.colorPicker.Color = BrushFactory.Instance.BrushColor;

            this.LoadBrushList();
            this.SelectUCPenTool(BrushFactory.Instance.BrushPenType);


            if (brushSamples == null)
            {
                brushSamples = new BrushSamples();
                this.needReflesh = true;
                this.UpdatePenStatus();
            }
            else
            {
                this.ucPTColor.MainBackground = new ImageBrush() { ImageSource = brushSamples.GetWbmp(FillPixelTypeEnum.Color), Stretch = Stretch.Fill };
                this.ucPTTransparency.MainOpacityMask = new ImageBrush() { ImageSource = brushSamples.GetWbmp(FillPixelTypeEnum.Transparent), Stretch = Stretch.Fill };
                this.ucPTOpacity.MainOpacityMask = new ImageBrush() { ImageSource = brushSamples.GetWbmp(FillPixelTypeEnum.Opacity), Stretch = Stretch.Fill };
                this.ucPTOriginal.MainBackground = new ImageBrush() { ImageSource = brushSamples.GetWbmp(FillPixelTypeEnum.Original), Stretch = Stretch.Fill };
                this.needReflesh = true;
            }


        }

        private void ApplyCache()
        {
            #region 画刷样式
            foreach (Button btn in this.lstBrush.Items)
            {
                btn.BorderBrush = btn.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

            selButton.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            BrushFactory.Instance.SelectedBrushIndex = (int)selButton.Tag;
            #endregion

            #region 工具
            UCPenTool tool = null;
            if (tool == null)
            {
                return;
            }

            foreach (UCPenTool item in this.grTools.Children)
            {
                item.HadChosen = false;
            }
            tool.HadChosen = true;

            BrushFactory.Instance.BrushPenType = tool.PenType;

            switch (tool.PenType)
            {
                case FillPixelTypeEnum.Color:
                    {
                        this.tPenToolName.Text = WPPhotoEditor.Resources.AppResources.PSPColorPen;
                        this.colorPicker.Visibility = Visibility.Visible;
                        this.grOpacityOption.Visibility = Visibility.Collapsed;
                        break;
                    }
                case FillPixelTypeEnum.Original:
                    {
                        this.tPenToolName.Text = WPPhotoEditor.Resources.AppResources.PSPOriginalPen;
                        this.colorPicker.Visibility = Visibility.Collapsed;
                        this.grOpacityOption.Visibility = Visibility.Collapsed;
                        break;
                    }
                case FillPixelTypeEnum.Transparent:
                    {
                        this.tPenToolName.Text = WPPhotoEditor.Resources.AppResources.PSPTransparentPen;
                        this.tOptionName.Text = WPPhotoEditor.Resources.AppResources.PSPTransparency;
                        this.colorPicker.Visibility = Visibility.Collapsed;
                        this.grOpacityOption.Visibility = Visibility.Visible;
                        break;
                    }
                case FillPixelTypeEnum.Opacity:
                    {
                        this.tPenToolName.Text = WPPhotoEditor.Resources.AppResources.PSPOpaquePen;
                        this.tOptionName.Text = WPPhotoEditor.Resources.AppResources.PSPOpacity;
                        this.colorPicker.Visibility = Visibility.Collapsed;
                        this.grOpacityOption.Visibility = Visibility.Visible;
                        break;
                    }
            }
            #endregion

            this.UpdatePenStatus();
        }

        private void LoadBrushList()
        {
            // List<ImageSource> imgSourceList = new List<ImageSource>();
            for (int i = 1; i <= AppSession.BrushPNGCount; i++)
            {
                ImageSource imageSource = new BitmapImage(new Uri("/Assets/Brush/" + i.ToString() + ".png", UriKind.Relative));

                Button btn = new Button();
                btn.Margin = new Thickness(-10, -8, 0, 0);
                btn.BorderThickness = new Thickness(2, 2, 2, 2);
                btn.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

                ImageBrush imgBrush = new ImageBrush();
                imgBrush.Stretch = Stretch.Fill;
                imgBrush.ImageSource = imageSource;
                btn.Width = 90;
                btn.Height = 90;
                btn.Background = imgBrush;
                btn.Tag = i;
                //Image img = new Image();
                //img.Width = 60;
                //img.Height = 60;
                //img.Source = imageSource;
                //this.lstBrush.Items.Add(img);
                btn.Click += btn_Click;
                this.lstBrush.Items.Add(btn);

                if (BrushFactory.Instance.SelectedBrushIndex != null)
                {
                    if (BrushFactory.Instance.SelectedBrushIndex.Value == i)
                    {
                        selButton = btn;
                    }
                }
                else if (i == 1)
                {
                    selButton = btn;
                }
            }


            this.btn_Click(selButton, null);

        }

        private void UpdatePenStatus()
        {
            if (!needReflesh)
            {
                return;
            }

            if (selButton == null)
            {
                return;
            }

            ImageBrush imgBrush = selButton.Background as ImageBrush;
            if (imgBrush == null)
            {
                return;
            }

            ImageSource imageSource = imgBrush.ImageSource;

            foreach (UCPenTool item in this.grTools.Children)
            {
                if (item.PenType == BrushFactory.Instance.BrushPenType)
                {
                    #region 生成当前画刷
                    WriteableBitmap brushImg = BrushFactory.Instance.CreateBrushWbmp(this.colorPicker.Color, imageSource, new Size(this.SizeValue, this.SizeValue), (byte)this.OpacityValue, BrushFactory.Instance.BrushPenType);
                    ImageBrush imgWbmpBrush = new ImageBrush() { ImageSource = brushImg, Stretch = Stretch.Fill };

                    if (item.PenType == FillPixelTypeEnum.Opacity || item.PenType == FillPixelTypeEnum.Transparent)
                    {
                        item.MainOpacityMask = imgWbmpBrush;
                    }
                    else
                    {
                        item.MainBackground = imgWbmpBrush;
                    }
                    #endregion

                    brushSamples.SetWbmp(item.PenType, brushImg);
                }

                else
                {
                    #region 生成所有画刷缩略图
                    WriteableBitmap brushImg = BrushFactory.Instance.CreateBrush(this.colorPicker.Color, imageSource, new Size(this.SizeValue, this.SizeValue), (byte)this.OpacityValue, item.PenType);
                    ImageBrush imgWbmpBrush = new ImageBrush() { ImageSource = brushImg, Stretch = Stretch.Fill };

                    if (item.PenType == FillPixelTypeEnum.Opacity || item.PenType == FillPixelTypeEnum.Transparent)
                    {
                        item.MainOpacityMask = imgWbmpBrush;
                    }
                    else
                    {
                        item.MainBackground = imgWbmpBrush;
                    }
                    #endregion

                    brushSamples.SetWbmp(item.PenType, brushImg);
                }

            }
        }

        private void SelectUCPenTool(UCPenTool tool)
        {
            if (tool == null)
            {
                return;
            }

            foreach (UCPenTool item in this.grTools.Children)
            {
                item.HadChosen = false;
            }
            tool.HadChosen = true;

            BrushFactory.Instance.BrushPenType = tool.PenType;

            switch (tool.PenType)
            {
                case FillPixelTypeEnum.Color:
                    {
                        this.tPenToolName.Text = WPPhotoEditor.Resources.AppResources.PSPColorPen;
                        this.colorPicker.Visibility = Visibility.Visible;
                        this.grOpacityOption.Visibility = Visibility.Collapsed;
                        break;
                    }
                case FillPixelTypeEnum.Original:
                    {
                        this.tPenToolName.Text = WPPhotoEditor.Resources.AppResources.PSPOriginalPen;
                        this.colorPicker.Visibility = Visibility.Collapsed;
                        this.grOpacityOption.Visibility = Visibility.Collapsed;
                        break;
                    }
                case FillPixelTypeEnum.Transparent:
                    {
                        this.tPenToolName.Text = WPPhotoEditor.Resources.AppResources.PSPTransparentPen;
                        this.tOptionName.Text = WPPhotoEditor.Resources.AppResources.PSPTransparency;
                        this.colorPicker.Visibility = Visibility.Collapsed;
                        this.grOpacityOption.Visibility = Visibility.Visible;
                        break;
                    }
                case FillPixelTypeEnum.Opacity:
                    {
                        this.tPenToolName.Text = WPPhotoEditor.Resources.AppResources.PSPOpaquePen;
                        this.tOptionName.Text = WPPhotoEditor.Resources.AppResources.PSPOpacity;
                        this.colorPicker.Visibility = Visibility.Collapsed;
                        this.grOpacityOption.Visibility = Visibility.Visible;
                        break;
                    }
            }

            this.UpdatePenStatus();
        }

        private void SelectUCPenTool(FillPixelTypeEnum toolType)
        {
            UCPenTool selTool = null;
            foreach (UCPenTool item in this.grTools.Children)
            {
                if (item.PenType == toolType)
                {
                    selTool = item;
                }
            }

            this.SelectUCPenTool(selTool);
        }

        private void UCPenTool_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SelectUCPenTool(sender as UCPenTool);
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            foreach (Button btn in this.lstBrush.Items)
            {
                btn.BorderBrush = btn.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }

            selButton = sender as Button;
            selButton.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            BrushFactory.Instance.SelectedBrushIndex = (int)selButton.Tag;

            this.UpdatePenStatus();

        }

        private void colorPicker_ColorChanged(object sender, Color color)
        {
            this.UpdatePenStatus();
        }

        private void slSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdatePenStatus();
        }
    }
}