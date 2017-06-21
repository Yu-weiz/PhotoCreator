using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using WPPhotoEditor.Media;
using Yuweiz.Phone.Media.Imaging;

namespace WPPhotoEditor.Controls
{
    public partial class UCPenStyleBoxControl : UserControl, IBox
    {
        public enum StyleBoxTypeEnum { Pen, Eraser, Transparent, Opacity }

        public UCPenStyleBoxControl()
        {
            InitializeComponent();
            BoxTypeEnum = StyleBoxTypeEnum.Pen;

            foreach (Button btn in this.lsBrushs.Items)
            {
                btn.Click += btnPenHeader_Click;
            }

            hadInitialized = true;

            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel3;
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.StartAds();
            }
        }

        bool hadInitialized = false;

        public byte OpacityValue
        {
            get { return (byte)this.slOpacity.Value; }
        }

        public Size PenSize
        {
            get { return new Size(this.slSize.Value, this.slSize.Value); }
        }

        public Color Color
        {
            get
            {
                return (this.rbtColor.Background as SolidColorBrush).Color;
            }
            set
            {
                this.rbtColor.Background = new SolidColorBrush(value);
            }
        }

        private StyleBoxTypeEnum boxTypeEnum;

        public StyleBoxTypeEnum BoxTypeEnum
        {
            get { return boxTypeEnum; }
            set
            {
                boxTypeEnum = value;
                switch (value)
                {
                    case UCPenStyleBoxControl.StyleBoxTypeEnum.Pen:
                    case UCPenStyleBoxControl.StyleBoxTypeEnum.Eraser:
                        {
                            this.grTransparency.Visibility = Visibility.Collapsed;
                            this.grOpacity.Visibility = Visibility.Collapsed;
                            this.cavIsOpacity.Visibility = Visibility.Collapsed;

                            this.cavIsEraser.Visibility = Visibility.Visible;
                            this.rbtColor.Visibility = value == StyleBoxTypeEnum.Pen ? Visibility.Visible : Visibility.Collapsed;
                        } break;
                    case UCPenStyleBoxControl.StyleBoxTypeEnum.Transparent:
                        {
                            this.grTransparency.Visibility = Visibility.Visible;
                            this.cavIsOpacity.Visibility = Visibility.Visible;

                            this.grOpacity.Visibility = Visibility.Collapsed;
                            this.cavIsEraser.Visibility = Visibility.Collapsed;
                        } break;
                    case UCPenStyleBoxControl.StyleBoxTypeEnum.Opacity:
                        {
                            this.grOpacity.Visibility = Visibility.Visible;
                            this.cavIsOpacity.Visibility = Visibility.Visible;

                            this.grTransparency.Visibility = Visibility.Collapsed;
                            this.cavIsEraser.Visibility = Visibility.Collapsed;
                        } break;
                }

                UpdateBrush();
            }
        }

        public bool IsTransparentOpacityBoxType
        {
            get
            {
                if (boxTypeEnum == StyleBoxTypeEnum.Pen || boxTypeEnum == StyleBoxTypeEnum.Eraser)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (value)
                {
                    BoxTypeEnum = chkIsOpacity.IsChecked.Value ? StyleBoxTypeEnum.Opacity : StyleBoxTypeEnum.Transparent;
                }
                else
                {
                    BoxTypeEnum = chkIsEraser.IsChecked.Value ? StyleBoxTypeEnum.Eraser : StyleBoxTypeEnum.Pen;
                }

            }
        }

        public ImageSource PenHeaderImg
        {
            get
            {
                if (selButton == null)
                {
                    return null;
                }

                ImageBrush imgBrush = selButton.Background as ImageBrush;
                if (imgBrush == null)
                {
                    return null;
                }

                ImageSource imageSource = imgBrush.ImageSource;
                return imageSource;
            }
        }

        private Button selButton
        {
            get
            {
                foreach (Button btn in this.lsBrushs.Items)
                {
                    if ((btn.BorderBrush as SolidColorBrush).Color == Colors.Red)
                    {
                        return btn;
                    }
                }

                return null;
            }
        }

        public void UpdateBrush()
        {
            if (!hadInitialized)
            {
                return;
            }
            switch (boxTypeEnum)
            {
                case StyleBoxTypeEnum.Pen:
                    {
                        BrushFactory.Instance.CreateBrushWbmp(this.Color, this.PenHeaderImg, this.PenSize, this.OpacityValue, FillPixelTypeEnum.Color);
                        BrushFactory.Instance.BrushPenType = FillPixelTypeEnum.Color;
                    } break;
                case StyleBoxTypeEnum.Eraser:
                    {
                        BrushFactory.Instance.CreateBrushWbmp(this.Color, this.PenHeaderImg, this.PenSize, this.OpacityValue, FillPixelTypeEnum.Original);
                        BrushFactory.Instance.BrushPenType = FillPixelTypeEnum.Original;
                    } break;
                case StyleBoxTypeEnum.Transparent:
                    {
                        BrushFactory.Instance.CreateBrushWbmp(this.Color, this.PenHeaderImg, this.PenSize, (byte)(255 - (byte)this.slTransparency.Value), FillPixelTypeEnum.Transparent);
                        BrushFactory.Instance.BrushPenType = FillPixelTypeEnum.Transparent;
                    } break;
                case StyleBoxTypeEnum.Opacity:
                    {
                        BrushFactory.Instance.CreateBrushWbmp(this.Color, this.PenHeaderImg, this.PenSize, (byte)(255 - (byte)this.slOpacity.Value), FillPixelTypeEnum.Opacity);
                        BrushFactory.Instance.BrushPenType = FillPixelTypeEnum.Opacity;
                    } break;
            }
        }

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

        void btnPenHeader_Click(object sender, RoutedEventArgs e)
        {
            foreach (Button bbtn in this.lsBrushs.Items)
            {
                bbtn.BorderBrush = new SolidColorBrush(Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16("#FFC3A0A0"));
            }
            (sender as Button).BorderBrush = new SolidColorBrush(Colors.Red);

            UpdateBrush();
        }

        private void slSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateBrush();
        }

        private void chkIsEraser_Click(object sender, RoutedEventArgs e)
        {
            BoxTypeEnum =chkIsEraser.IsChecked.Value? StyleBoxTypeEnum.Eraser: StyleBoxTypeEnum.Pen;
        }

        private void chkIsOpacity_Click(object sender, RoutedEventArgs e)
        {
            BoxTypeEnum =chkIsOpacity.IsChecked.Value? StyleBoxTypeEnum.Opacity: StyleBoxTypeEnum.Transparent;
        }

        private void rbtColor_Click(object sender, RoutedEventArgs e)
        {
            AppSession.Instance.ChosenColor = (this.rbtColor.Background as SolidColorBrush).Color;
            App.RootFrame.Navigate(new Uri("/Views/ColorChooserPage.xaml", UriKind.Relative));
        }
    }
}
