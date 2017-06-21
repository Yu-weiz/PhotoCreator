using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Yuweiz.Phone.Gestures;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Xna.Framework.Input.Touch;
using System.Windows.Media.Imaging;

namespace WPPhotoEditor.UControls
{
    public partial class UCCutBorder : UserControl
    {
        public UCCutBorder()
        {
            InitializeComponent();

            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.DragComplete | GestureType.PinchComplete | GestureType.Pinch;
            this.brPhoto.ManipulationDelta += UCCutBorder_ManipulationDelta;
            this.DoubleTap += UCCutBorder_DoubleTap;
        }

        void UCCutBorder_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.grCrop.Visibility == Visibility.Visible)
            {
                this.ApplyTheCropImage(new Rect(rcCrop.Margin.Left, rcCrop.Margin.Top, rcCrop.Width, rcCrop.Height));
                if (ApplyCroped != null)
                {
                    ApplyCroped();
                }
            }
        }

        public event Action ApplyCroped;

        public Size NewSize { get;protected set; }

        private double cropProportion = 0;

        /// <summary>
        /// 裁剪比例
        /// </summary>
        public double CropProportion
        {
            get { return cropProportion; }
            set 
            {
                cropProportion = value;

                this.grCrop.Visibility =Visibility.Visible;
                this.InitializeCropMask(new Rect(brPhoto.Width * 0.15, brPhoto.Height * 0.15, brPhoto.Width * 0.7, brPhoto.Height * 0.7), cropProportion);
            }
        }

        private const double MinCropWidth = 20;

        private const double MinCropHeight = 20;


        private WriteableBitmap wbmap;
        
        /// <summary>
        /// 含保存的Wbmap，对裁剪旋转等使用
        /// </summary>
        private WriteableBitmap TransWbmp
        {
            set
            {
                this.Wbmap = value;

                //保存临时文件，供特效使用
                Yuweiz.Phone.IO.IsolatedStorageDAL.Instance.SavePicture(wbmap, AppSession.TempAffectBitmapPath);
            }
        }

        private BitmapSource originalBitmapSource;

        private Path phCrop;

        private Shape hadClickEdEllip;

        private Point lastPt;

        bool hadDown = false;



        public WriteableBitmap Wbmap
        {
            get { return wbmap; }
            set
            {
                wbmap = value;
                this.NewSize = new Size(this.wbmap.PixelWidth, this.wbmap.PixelHeight);
                Size sz = Yuweiz.Phone.Media.ViewControler.GetTheFitShowSize(new Size(this.Width, this.Height), new Size(this.wbmap.PixelWidth, this.wbmap.PixelHeight));
                this.brPhoto.Width = sz.Width;
                this.brPhoto.Height = sz.Height;
                this.brPhoto.Background = new ImageBrush() { ImageSource = wbmap, Stretch = Stretch.UniformToFill };
            }
        }

        public BitmapSource BitmapSource
        {
            get
            {
                return wbmap;
            }
            set
            {
                this.originalBitmapSource = value;
                this.TransWbmp = value is WriteableBitmap ? value as WriteableBitmap : new WriteableBitmap(value);
            }
        }

        public bool IsCropState
        {
            get { return this.grCrop.Visibility == Visibility.Visible; }
            set
            {
                this.grCrop.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

                if (value)
                {
                    this.InitializeCropMask(new Rect(brPhoto.Width * 0.15, brPhoto.Height * 0.15, brPhoto.Width * 0.7, brPhoto.Height * 0.7), 1);
                }
            }
        }

        void UCCutBorder_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                //Point ptGesture = new Point(gesture.Position.X, gesture.Position.Y);
                Point ptGesture = e.ManipulationOrigin;

                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    if (!hadDown)
                    {
                        this.setHadClickEdEllip(ptGesture);
                        this.lastPt = ptGesture;
                        hadDown = true;
                    }
                    else
                    {
                        if (this.CropProportion == 0)
                        {
                            setCropMask(ptGesture);
                        }
                        else
                        {
                            setCropMask(ptGesture, this.CropProportion);
                        }
                    }
                }
                else if (gesture.GestureType == GestureType.Pinch)
                {
                    if (grCrop.Visibility == Visibility.Collapsed)
                    {
                        grCrop.Visibility = Visibility.Visible;
                        Rect maskRect = new Rect(brPhoto.Width * 0.15, brPhoto.Height * 0.15, brPhoto.Width * 0.7, brPhoto.Height * 0.7);
                        setCropMask(maskRect);
                    }

                    if (e.PinchManipulation != null && e.PinchManipulation.Current != null)
                    {


                        #region 裁剪相关变量
                        Rect rec = new Rect();
                        //Point[] tPts = { new Point(gesture.Position.X, gesture.Position.Y), new Point(gesture.Position2.X, gesture.Position2.Y) };                   
                        Point[] tPts = { e.PinchManipulation.Current.PrimaryContact, e.PinchManipulation.Current.SecondaryContact };


                        Point pt = locatRec(tPts[0], tPts[1]);              //获取截剪的矩形
                        rec = new Rect(pt.X, pt.Y, Math.Abs(tPts[0].X - tPts[1].X), Math.Abs(tPts[0].Y - tPts[1].Y));

                        if (this.CropProportion == 0)
                        {
                            setCropMask(rec);
                        }
                        else
                        {
                            setCropMask(rec, this.CropProportion);
                        }
                        #endregion
                    }

                    hadDown = false;
                }
                else
                {
                    hadDown = false;
                }
                this.lastPt = ptGesture;
            }
        }


        private void ApplyTheCropImage(Rect rcCrop)
        {
            double proportion = wbmap.PixelWidth / this.brPhoto.Width;
            Rect newRcCrop = new Rect(rcCrop.Left * proportion, rcCrop.Top * proportion, rcCrop.Width * proportion, rcCrop.Height * proportion);

            this.TransWbmp = wbmap.Crop(newRcCrop);
            this.IsCropState = false;
        }

        public void ApplyNewSize(int width, int height)
        {
            this.Wbmap = this.wbmap.Resize(width, height, WriteableBitmapExtensions.Interpolation.NearestNeighbor);
            this.NewSize = new Size(width,height);
        }

        public void ApplyRotate()
        {
            this.TransWbmp = wbmap.Rotate(90);
        }

        public void ApplyFlipH()
        {
            this.TransWbmp = wbmap.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
        }

        public void ApplyFlipV()
        {
            this.TransWbmp = wbmap.Flip(WriteableBitmapExtensions.FlipMode.Vertical);
        }

        public void ResumeBitmapSource()
        {
            this.TransWbmp = this.originalBitmapSource is WriteableBitmap ? this.originalBitmapSource as WriteableBitmap : new WriteableBitmap(originalBitmapSource);
        }


        /// <summary>
        /// 拖拉四角的点，调整矩形
        /// </summary>
        /// <param name="pt"></param>
        private void setCropMask(Point pt)
        {
            Rect rec = new Rect(rcCrop.Margin.Left, rcCrop.Margin.Top, rcCrop.Width, rcCrop.Height);
            Point pos = new Point(pt.X - lastPt.X, pt.Y - lastPt.Y);
            if (hadClickEdEllip != null)
            {
                if (hadClickEdEllip == luEllipse)
                {
                    //左上角                 
                    double newWidth = this.rcCrop.Width - pos.X > MinCropWidth ? this.rcCrop.Width - pos.X : MinCropWidth;
                    double newHeight = this.rcCrop.Height - pos.Y > MinCropHeight ? this.rcCrop.Height - pos.Y : MinCropHeight;

                    if (pt.X < ruEllipse.Margin.Left - 10 && pt.Y < rdEllipse.Margin.Top - 10)
                    {
                        rec = new Rect(this.rcCrop.Margin.Left + pos.X, this.rcCrop.Margin.Top + pos.Y, newWidth, newHeight);
                    }
                }
                else if (hadClickEdEllip == ldEllipse)
                {
                    //左下角
                    double newWidth = rcCrop.Width - pos.X > 20 ? rcCrop.Width - pos.X : 20;
                    double newHeight = rcCrop.Height + pos.Y > 20 ? rcCrop.Height + pos.Y : 20;

                    if (pt.X < ruEllipse.Margin.Left - 10)
                    {
                        rec = new Rect(this.rcCrop.Margin.Left + pos.X, rcCrop.Margin.Top, newWidth, newHeight);
                    }
                }
                else if (hadClickEdEllip == ruEllipse)
                {
                    //右上角
                    double newWidth = rcCrop.Width + pos.X > 20 ? rcCrop.Width + pos.X : 20;
                    double newHeight = rcCrop.Height - pos.Y > 20 ? rcCrop.Height - pos.Y : 20;
                    if (pt.Y < rdEllipse.Margin.Top - 10)
                    {
                        rec = new Rect(rcCrop.Margin.Left, rcCrop.Margin.Top + pos.Y, newWidth, newHeight);
                    }
                }
                else if (hadClickEdEllip == rdEllipse)
                {
                    //右下角
                    double newWidth = rcCrop.Width + pos.X > 20 ? rcCrop.Width + pos.X : 20;
                    double newHeight = rcCrop.Height + pos.Y > 20 ? rcCrop.Height + pos.Y : 20;
                    rec = new Rect(rcCrop.Margin.Left, rcCrop.Margin.Top, newWidth, newHeight);
                }
                else
                {
                    rec = new Rect(rcCrop.Margin.Left + pos.X, rcCrop.Margin.Top + pos.Y, rcCrop.Width, rcCrop.Height);
                }

            }

            setCropMask(rec);
        }

        /// <summary>
        /// 根据给出的矩形，生成剪切蒙版
        /// </summary>
        /// <param name="lRectSize"></param>
        private void setCropMask(Rect lRectSize)
        {
            bool isXOver = lRectSize.X + lRectSize.Width > brPhoto.Width;
            bool isYOver = lRectSize.Y + lRectSize.Height > brPhoto.Height;

            if (lRectSize.X < 0 || lRectSize.Y < 0 || isXOver || isYOver || lRectSize.Width < 20 || lRectSize.Height < 20)
            {
                return;
            }
            setGeometryGroup(phCrop, grCrop, lRectSize);
            setCropRect(lRectSize);
        }

        private void setCropMask(Point pt, double proportion)
        {
            Rect rec = new Rect(rcCrop.Margin.Left, rcCrop.Margin.Top, rcCrop.Width, rcCrop.Height);
            Point pos = new Point(pt.X - lastPt.X, pt.Y - lastPt.Y);
            Point newPt = pt;
            if (hadClickEdEllip != null)
            {
                if (hadClickEdEllip == luEllipse)
                {
                    //左上角
                    double newWidth = rdEllipse.Margin.Left + 10 - pt.X > 20 ? rdEllipse.Margin.Left + 10 - pt.X : 20;
                    double newHeight = rdEllipse.Margin.Top + 10 - pt.Y > 20 ? rdEllipse.Margin.Top + 10 - pt.Y : 20;

                    if (proportion > 0)
                    {
                        if (proportion >= 1)
                        {
                            newHeight = newWidth / proportion;
                            newPt.Y = rdEllipse.Margin.Top + 10 - newHeight;
                        }
                        else if (proportion < 1)
                        {
                            newWidth = newHeight * proportion;
                            newPt.X = rdEllipse.Margin.Left + 10 - newWidth;
                        }
                    }

                    if (newPt.X < ruEllipse.Margin.Left - 10 && newPt.Y < rdEllipse.Margin.Top - 10)
                    {
                        rec = new Rect(newPt.X, newPt.Y, newWidth, newHeight);
                    }
                }
                else if (hadClickEdEllip == ldEllipse)
                {
                    //左下角
                    double newWidth = Math.Abs(ruEllipse.Margin.Left + 10 - pt.X) > 20 ? Math.Abs(ruEllipse.Margin.Left + 10 - pt.X) : 20;
                    double newHeight = Math.Abs(ruEllipse.Margin.Top + 10 - pt.Y) > 20 ? Math.Abs(ruEllipse.Margin.Top + 10 - pt.Y) : 20;

                    if (proportion > 0)
                    {
                        if (proportion >= 1)
                        {
                            newHeight = newWidth / proportion;
                            //newPt.Y = ruEllipse.Margin.Top + 10 + newHeight;
                        }
                        else if (proportion < 1)
                        {
                            newWidth = newHeight * proportion;
                            newPt.X = ruEllipse.Margin.Left + 10 - newWidth;
                        }
                    }

                    if (newPt.X < ruEllipse.Margin.Left - 10)
                    {
                        rec = new Rect(newPt.X, rcCrop.Margin.Top, newWidth, newHeight);
                    }
                }
                else if (hadClickEdEllip == ruEllipse)
                {
                    //右上角
                    double newWidth = Math.Abs(ldEllipse.Margin.Left + 10 - pt.X) > 20 ? Math.Abs(ldEllipse.Margin.Left + 10 - pt.X) : 20;
                    double newHeight = Math.Abs(ldEllipse.Margin.Top + 10 - pt.Y) > 20 ? Math.Abs(ldEllipse.Margin.Top + 10 - pt.Y) : 20;

                    if (proportion > 0)
                    {
                        if (proportion >= 1)
                        {
                            newHeight = newWidth / proportion;
                            newPt.Y = ldEllipse.Margin.Top + 10 - newHeight;
                        }
                        else if (proportion < 1)
                        {
                            newWidth = newHeight * proportion;
                            // newPt.X = ldEllipse.Margin.Left + 10 - newWidth;
                        }
                    }

                    if (pt.Y < rdEllipse.Margin.Top - 10)
                    {
                        rec = new Rect(rcCrop.Margin.Left, newPt.Y, newWidth, newHeight);
                    }
                }
                else if (hadClickEdEllip == rdEllipse)
                {
                    //右下角
                    double newWidth = Math.Abs(luEllipse.Margin.Left + 10 - pt.X) > 20 ? Math.Abs(luEllipse.Margin.Left + 10 - pt.X) : 20;
                    double newHeight = Math.Abs(luEllipse.Margin.Top + 10 - pt.Y) > 20 ? Math.Abs(luEllipse.Margin.Top + 10 - pt.Y) : 20;

                    if (proportion > 0)
                    {
                        if (proportion >= 1)
                        {
                            newHeight = newWidth / proportion;
                        }
                        else if (proportion < 1)
                        {
                            newWidth = newHeight * proportion;
                        }
                    }
                    rec = new Rect(rcCrop.Margin.Left, rcCrop.Margin.Top, newWidth, newHeight);
                }
                else
                {
                    rec = new Rect(rcCrop.Margin.Left + pos.X, rcCrop.Margin.Top + pos.Y, rcCrop.Width, rcCrop.Height);
                }

            }

            setCropMask(rec);
        }

        private void setCropMask(Rect rec, double proportion)
        {

            Rect newRec = rec;
            if (rec.Width > rec.Height)
            {
                newRec.Height = rec.Width / proportion;
            }
            else
            {
                newRec.Width = rec.Height * proportion;
            }
            setCropMask(newRec);
        }

        private void InitializeCropMask(Rect rec, double proportion)
        {
            grCrop.Visibility = Visibility.Visible;
            Rect newRec = rec;
            if (proportion >= 1)
            {
                newRec.Width = Math.Min(rec.Width, rec.Height);
                newRec.Height = newRec.Width / proportion;
            }
            else
            {
                newRec.Height = Math.Min(rec.Width, rec.Height);
                newRec.Width = newRec.Height * proportion;
            }
            setCropMask(newRec);
        }


        //设置剪切矩形
        private void setCropRect(Rect lRectSize)
        {
            rcCrop.Visibility = Visibility.Visible;
            luEllipse.Visibility = Visibility.Visible;
            ldEllipse.Visibility = Visibility.Visible;
            ruEllipse.Visibility = Visibility.Visible;
            rdEllipse.Visibility = Visibility.Visible;

            rcCrop.Margin = new Thickness(lRectSize.X, lRectSize.Y, 0, 0);
            rcCrop.Width = lRectSize.Width;
            rcCrop.Height = lRectSize.Height;

            luEllipse.Margin = new Thickness(lRectSize.X - 10, lRectSize.Y - 10, 0, 0);
            ldEllipse.Margin = new Thickness(lRectSize.X - 10, lRectSize.Y - 10 + lRectSize.Height, 0, 0);
            ruEllipse.Margin = new Thickness(lRectSize.X - 10 + lRectSize.Width, lRectSize.Y - 10, 0, 0);
            rdEllipse.Margin = new Thickness(lRectSize.X - 10 + lRectSize.Width, lRectSize.Y - 10 + lRectSize.Height, 0, 0);


            ///显示裁剪后新的分分辨率
            tSize.Visibility = Visibility.Visible;
            tSize.Margin = new Thickness(lRectSize.X + 10, lRectSize.Y - 30, 0, 0);
            tSize.Text = (this.NewSize.Width * (lRectSize.Width / brPhoto.Width)).ToString("00") + "*" + (this.NewSize.Height * (lRectSize.Height / brPhoto.Height)).ToString("00");
        }

        //设置蒙版层
        private void setGeometryGroup(Path pathCrop, Grid grCrop, Rect lRectSize)
        {
            if (pathCrop == null)
            {
                pathCrop = new Path();
                phCrop = pathCrop;
                grCrop.Children.Insert(0, pathCrop);
            }

            GeometryGroup gmGroup = new GeometryGroup();
            gmGroup.FillRule = FillRule.EvenOdd;
            RectangleGeometry bRect = new RectangleGeometry();       //大的矩形
            RectangleGeometry lRect = new RectangleGeometry();       //小的矩形



            bRect.Rect = new Rect(0, 0, brPhoto.Width, brPhoto.Height);
            lRect.Rect = lRectSize;

            #region ----井线
            LineGeometry[] hLines = new LineGeometry[2];
            LineGeometry[] vLines = new LineGeometry[2];

            double scale = lRectSize.Height / 3;
            for (int i = 0; i < 2; i++)
            {
                hLines[i] = new LineGeometry();
                hLines[i].StartPoint = new Point(lRectSize.X, lRectSize.Y + (i + 1) * scale);
                hLines[i].EndPoint = new Point(lRectSize.X + lRectSize.Width, lRectSize.Y + (i + 1) * scale);
                gmGroup.Children.Add(hLines[i]);
            }

            double scaleV = lRectSize.Width / 3;
            for (int i = 0; i < 2; i++)
            {
                vLines[i] = new LineGeometry();
                vLines[i].StartPoint = new Point(lRectSize.X + (i + 1) * scaleV, lRectSize.Y);
                vLines[i].EndPoint = new Point(lRectSize.X + (i + 1) * scaleV, lRectSize.Y + lRectSize.Height);
                gmGroup.Children.Add(vLines[i]);
            }
            #endregion

            pathCrop.Fill = new SolidColorBrush(Color.FromArgb((byte)110, (byte)170, (byte)170, (byte)170));
            pathCrop.Stroke = (Brush)Application.Current.Resources["PhoneAccentBrush"];

            gmGroup.Children.Add(bRect);
            gmGroup.Children.Add(lRect);

            pathCrop.Data = gmGroup;


        }


        /// <summary>
        /// 设置hadClickEdEllip 字段：为setCropMask指供操作对象
        /// </summary>
        /// <param name="pt"></param>
        private void setHadClickEdEllip(Point pt)
        {
            if (CheckHadClickEllipse(pt, luEllipse, 1))
            {
                hadClickEdEllip = luEllipse;
            }
            else if (CheckHadClickEllipse(pt, ldEllipse, 3))
            {
                hadClickEdEllip = ldEllipse;
            }
            else if (CheckHadClickEllipse(pt, ruEllipse, 2))
            {
                hadClickEdEllip = ruEllipse;
            }
            else if (CheckHadClickEllipse(pt, rdEllipse, 4))
            {
                hadClickEdEllip = rdEllipse;
            }
            else if (pt.X > rcCrop.Margin.Left && pt.X < rcCrop.Margin.Left + rcCrop.Width && pt.Y > rcCrop.Margin.Top && pt.Y < rcCrop.Margin.Top + rcCrop.Height)
            {
                hadClickEdEllip = rcCrop;
            }
            else
            {
                hadClickEdEllip = null;
            }
        }

        /// <summary>
        /// 检测是否点击指定的点（用于指示：矩形的四个角）
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="el"></param>
        /// <param name="position">1:左上，2：右上，3：左下，4：右下</param>
        /// <returns></returns>
        private bool CheckHadClickEllipse(Point pt, Ellipse el, int position)
        {
            int detal = 50;
            switch (position)
            {
                case 1:
                    {
                        bool isXMeet = pt.X < el.Margin.Left + detal;
                        bool isYMeet = pt.Y < el.Margin.Top + detal;

                        bool isVastX = pt.X < el.Margin.Left + el.Width / 2 && isYMeet;
                        bool isVastY = pt.Y < el.Margin.Top + el.Height / 2 && isXMeet;
                        return (isXMeet && isYMeet) || isVastX || isVastY;
                    }
                case 2:
                    {
                        bool isXMeet = pt.X > el.Margin.Left + el.Width - detal;
                        bool isYMeet = pt.Y < el.Margin.Top + detal;

                        bool isVastX = pt.X > el.Margin.Left + el.Width / 2 && isYMeet;
                        bool isVastY = pt.Y < el.Margin.Top + el.Height / 2 && isXMeet;
                        //return isXMeet && isYMeet;        
                        return (isXMeet && isYMeet) || isVastX || isVastY;
                    }
                case 3:
                    {
                        bool isXMeet = pt.X < el.Margin.Left + detal;
                        bool isYMeet = pt.Y > el.Margin.Top + el.Height - detal;

                        bool isVastX = pt.X < el.Margin.Left + this.rcCrop.Width / 2 && isYMeet;
                        bool isVastY = pt.Y > el.Margin.Top - this.rcCrop.Height / 2 && isXMeet;
                        return (isXMeet && isYMeet) || isVastX || isVastY;
                    }
                case 4:
                    {
                        bool isXMeet = pt.X > el.Margin.Left + el.Width - detal;
                        bool isYMeet = pt.Y > el.Margin.Top + el.Height - detal;

                        bool isVastX = pt.X > el.Margin.Left + el.Width - el.Width / 2 && isYMeet;
                        bool isVastY = pt.Y > el.Margin.Top + el.Height - el.Height / 2 && isXMeet;
                        return (isXMeet && isYMeet) || isVastX || isVastY;
                    }
            }
            return false;
        }

        /// <summary>
        /// 获取截剪的矩形
        /// </summary>
        /// <param name="pt01"></param>
        /// <param name="pt02"></param>
        /// <returns></returns>
        private Point locatRec(Point pt01, Point pt02)
        {
            Point pt = new Point();

            //pt为X坐标最小的
            if (pt01.X < pt02.X)
            {
                pt.X = pt01.X;
            }
            else
            {
                pt.X = pt02.X;
            }

            if (pt01.Y < pt02.Y)
            {
                pt.Y = pt01.Y;
            }
            else
            {
                pt.Y = pt02.Y;
            }

            //  pt.X+=  Canvas.GetLeft(this.canLayers.Children[selLayerIndex] as Border);
            // pt.Y += Canvas.GetTop(this.canLayers.Children[selLayerIndex] as Border);
            return pt;
        }

        /// <summary>
        /// 校对点击的点是是否在指定UI中
        /// </summary>
        /// <param name="tPoints"></param>
        /// <returns></returns>
        private Point[] proofreadTPts(Point[] pts, FrameworkElement targetUI)
        {
            if (pts[0].X > targetUI.Width)
            {
                pts[0].X = targetUI.Width;
            }

            if (pts[0].Y > targetUI.Height)
            {
                pts[0].Y = targetUI.Height;
            }

            if (pts[1].X > targetUI.Width)
            {
                pts[1].X = targetUI.Width;
            }

            if (pts[1].Y > targetUI.Height)
            {
                pts[1].Y = targetUI.Height;
            }

            return pts;
        }


    }
}
