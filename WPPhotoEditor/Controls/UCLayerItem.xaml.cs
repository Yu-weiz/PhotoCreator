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
using Microsoft.Xna.Framework.Input.Touch;

namespace WPPhotoEditor.UControls
{
    public partial class UCLayerItem : UserControl
    {
        public UCLayerItem()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += UCLayer_MouseLeftButtonDown;
            this.MouseLeftButtonUp += UCLayer_MouseLeftButtonUp;
            // this.MouseLeave += UCLayerItem_MouseLeave;

           
            this.ManipulationDelta += UCLayerItem_ManipulationDelta;
            this.ManipulationCompleted += UCLayerItem_ManipulationCompleted;
        }


        #region 字段

        /// <summary>
        /// 记录图层控件的位置（上一次）
        /// </summary>
        private Point? lastPox;

        private bool hadDown = false;

        private bool isVisibleStatus = true;

        #endregion

        /// <summary>
        /// 获取或设置控件是否激活显示
        /// </summary>
        public bool IsVisibleStatus
        {
            get { return isVisibleStatus; }
            set
            {
                isVisibleStatus = value;
                if (this.isVisibleStatus)
                {
                    //this.brMain.BorderBrush = new SolidColorBrush(Colors.Black);
                    this.Opacity = 1;
                }
                else
                {
                    this.brMain.BorderBrush = new SolidColorBrush(AppSession.LightWhite);
                    this.Opacity = 0.8;
                }
            }
        }

        public bool HadChosen
        {
            get
            {
                SolidColorBrush colorBrush = this.brMain.BorderBrush as SolidColorBrush;
                if (colorBrush != null && colorBrush.Color == Colors.Black)
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
                Color newColor = value ? AppSession.SecondaryColor : Colors.Black;
                if (!value && !this.isVisibleStatus)
                {
                    newColor = AppSession.LightWhite;
                }
                this.brMain.BorderBrush = new SolidColorBrush(newColor);
            }
        }

        /// <summary>
        /// 索引由0开始
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 图层缩略图
        /// </summary>
        public BitmapSource LayerImage
        {
            set
            {
                if (value != null)
                {
                    ImageBrush imgBrush = new ImageBrush();
                    imgBrush.ImageSource = value;
                    imgBrush.Stretch = Stretch.UniformToFill;
                    brMain.Background = imgBrush;
                }
            }
        }

        /// <summary>
        /// e:移动后的位置
        /// </summary>
        public event EventHandler<PoxEventArgs> Moving;

        public event EventHandler MouseReleased;

        public void TurnThinner(int interval)
        {
            if (this.isVisibleStatus)
            {
                this.Opacity = 1 - ((double)interval / 2 / 255);
            }
            else
            {
                this.Opacity += ((double)interval / 50 / 255);
            }
        }

        public void TurnBack()
        {
            if (this.isVisibleStatus)
            {
                this.TurnBackVisibility();
            }
            else
            {
                this.TurnBackCollapse();
            }
        }

        private void TurnBackVisibility()
        {
            this.Opacity = 1;
        }

        private void TurnBackCollapse()
        {
            this.Opacity = 0.8;
        }



        void UCLayer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!hadDown)
            {
                return;
            }

            lastPox = null;
            hadDown = false;

            if (MouseReleased != null)
            {
                MouseReleased(this, null);
            }

            TouchPanel.EnabledGestures =GestureType.None;
        }

        void UCLayer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.DragComplete;
            hadDown = true;
        }

        void UCLayerItem_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {          
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                //获取当前的识别的手势类型
                if (gesture.GestureType == GestureType.DragComplete)
                {
                    UCLayer_MouseLeftButtonUp(null, null);
                }
            }
        }

        void UCLayerItem_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                //获取当前的识别的手势类型
                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    if (!hadDown)
                    {
                        return;
                    }
                    Point pos = new Point(gesture.Position.X, gesture.Position.Y);
                    

                    if (lastPox != null)
                    {
                        double poxX = Canvas.GetLeft((UIElement)sender) + pos.X - lastPox.Value.X;
                        double poxY = Canvas.GetTop((UIElement)sender) + pos.Y - lastPox.Value.Y;
                        Canvas.SetLeft((UIElement)sender, poxX);
                        Canvas.SetTop((UIElement)sender, poxY);

                        if (Moving != null)
                        {
                            Moving(sender, new PoxEventArgs(poxX, poxY));
                        }

                    }

                    lastPox = pos;
                }
                else if (gesture.GestureType == GestureType.DragComplete)
                {                   
                    UCLayer_MouseLeftButtonUp(null, null);
                }
            }

        }


    }


}
