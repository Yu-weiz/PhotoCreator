using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using System.Windows.Media.Imaging;
using System.Windows.Ink;
using System.Threading;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.IO;

using WPPhotoEditor.OLD.Entity;

namespace WPPhotoEditor.OLD
{
    public enum ToolStyle { Brush, ColorDraw, Zoom, Clip, CanvasZoom }

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            rbtAddLay.Click += new RoutedEventHandler(rbtAddLay_Click);
            rbtDelLayer.Click += new RoutedEventHandler(rbtDelLayer_Click);

           ///指定Brush事件
            foreach (Button bt in lsBrushs.Items)
            {
                bt.Click += new RoutedEventHandler(btBrBuffer_Click);
            }
         


            //初始化工具栏：
            initializeTools();

            #region 手执辅助操作：载剪，旋转
            Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);
            #endregion

            slAngle.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slAngle_ValueChanged);
            rbtCancleClip.Click += new RoutedEventHandler(rbtCancleClip_Click);
            rbtOKClip.Click += new RoutedEventHandler(rbtOKClip_Click);
            tAngle.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(tAngle_Tap);
            this.rbtClip.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(rbtClip_Hold);
            
            this.grCanContainer.MouseLeftButtonDown += new MouseButtonEventHandler(canLayers_MouseLeftButtonDown);
            this.grCanContainer.LostMouseCapture += new MouseEventHandler(MainPage_LostMouseCapture);

            initializeBrushes();

            this.canLayers.MouseLeftButtonUp += new MouseButtonEventHandler(MainPage_MouseLeftButtonUp);
            this.grLayers.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(grLayers_Hold);



            this.grCrop.DoubleTap += new EventHandler<System.Windows.Input.GestureEventArgs>(grCrop_DoubleTap);

            initializeDrawing();

            if (AppSession.Instance.IsShowAds)
            {
                this.adUltimateBar.Visibility = Visibility.Visible;
                this.adUltimateBar.AdUltimateModel = AppSession.Instance.AdUltimateModel1;
                this.adUltimateBar.StartAds();

                this.adUltimateBar2.Visibility = Visibility.Visible;
                this.adUltimateBar2.AdUltimateModel = AppSession.Instance.AdUltimateModel3;
                this.adUltimateBar2.StartAds();
            }
        }

        void MainPage_LostMouseCapture(object sender, MouseEventArgs e)
        {
            screenTouch = true;
        }

        void canLayers_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.canLayers.CaptureMouse();
            screenTouch = false;
        }



        void grLayers_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            return;
            inkPosX = 0;
            inkPosY = 0;

            Canvas.SetLeft(grCanContainer, inkPosX);
            Canvas.SetTop(grCanContainer, inkPosY);
        }

        void MainPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (screenTouch || (curTool != ToolStyle.Brush && curTool != ToolStyle.ColorDraw))
            {
                closeTool();
                closePenDetailSetting();
            }
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.canLayers.Background = new SolidColorBrush(App.PESettings.CanvasBackgroundColor);

            if (App.PESettings.MatchTheFirstLayerPic)
            {
                matchTheFirstPic();
            }
            else
            {
                resizeTheCanvas(App.PESettings.CanvasSize);
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (colorPicker != null && colorPicker.Visibility == Visibility.Visible)
            {
                closeColorPiker();
                e.Cancel = true;
            }


            if (!e.Cancel)
            {
                MessageBoxResult result = MessageBox.Show(WPPhotoEditor.Resources.AppResources.tLeaveContent, "", MessageBoxButton.OKCancel);

                if (result != MessageBoxResult.OK)
                {
                    e.Cancel = true;
                }
            }

            base.OnBackKeyPress(e);
        }


        bool screenTouch = true;
        bool hadCollapsed = false;   //指示是否已隐藏

        TouchPointCollection lastTouchPoints;          //用于移动画布

        TouchPointCollection originalPoints;                   //记录：手指按下时的中点坐标；用于绽放手指所在的范围
        double lastFingerDistance = 0.0;                   //记录移动时上一次的两指间距离
        double firstFingerDistance = 0.0;                  //记录按下时上一次的两指间距离  －增补启用两指移动时，缩放的感应范围(<50时的)
        double inkPosX = 0.0;
        double inkPosY = 0;
        TransformGroup transFormGp = new TransformGroup();
        ScaleTransform scForm = new ScaleTransform();     //用于变换画板图层



        UnReDoManage unReManager = new UnReDoManage();    //撤消、重做管理对象     



        bool isLayerAddHandle = true;               //指示是添加图层，还是修改图层图片

        int selLayerIndex = -1;                     //当前编辑的图层索引

        ToolStyle curTool = ToolStyle.Brush;        //指示当前鼠标的工具类型

        bool isMove = false;                         //指示是缩放还是移动



        #region 手势：放大/缩小/移动/剪切矩形画取

        void rbtOKClip_Click(object sender, RoutedEventArgs e)
        {
            if (selLayerIndex > -1 && selLayerIndex < this.lsBrushs.Items.Count && recClip.Visibility == Visibility.Visible)
            {
                RectangleGeometry recGem = new RectangleGeometry();
                recGem.Rect = new Rect(new Point(this.recClip.Margin.Left - Canvas.GetLeft(this.canLayers.Children[selLayerIndex] as Border), this.recClip.Margin.Top - Canvas.GetTop(this.canLayers.Children[selLayerIndex] as Border)), new Size(this.recClip.Width, this.recClip.Height));
                this.canLayers.Children[selLayerIndex].Clip = recGem;
                this.recClip.Visibility = Visibility.Collapsed;
            }
        }

        void rbtCancleClip_Click(object sender, RoutedEventArgs e)
        {
            recClip.Visibility = Visibility.Collapsed;
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
        /// 校对点击的点是是否有屏幕中
        /// </summary>
        /// <param name="tPoints"></param>
        /// <returns></returns>
        private Point[] proofreadTPts(TouchPointCollection tPoints)
        {
            Point[] pts = new Point[2];
            pts[0] = tPoints[0].Position;
            pts[1] = tPoints[1].Position;

            if (pts[0].X > this.grLayers.Width)
            {
                pts[0].X = this.grLayers.Width;
            }

            if (pts[0].Y > this.grLayers.Height)
            {
                pts[0].Y = this.grLayers.Height;
            }

            if (pts[1].X > this.grLayers.Width)
            {
                pts[1].X = this.grLayers.Width;
            }

            if (pts[1].Y > this.grLayers.Height)
            {
                pts[1].Y = this.grLayers.Height;
            }

            return pts;
        }

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            if (screenTouch)
            {
                return;
            }

            if (curTool == ToolStyle.Brush)
            {
                if (curBrush.BrushPenType == PenType.Color || curBrush.BrushPenType == PenType.Original)
                {
                    Touch_FrameDrawing(e);
                }
                else
                {
                    Touch_FrameTransparent(e);
                }
                return;
            }

            if (curTool == ToolStyle.Clip)
            {
                Touch_FrameClip(e);
                return;
            }



            if (selLayerIndex > -1 && selLayerIndex < lsLayers.Items.Count && this.canLayers.Children[selLayerIndex].Visibility == Visibility.Visible)
            {
                TouchPointCollection tPoints = e.GetTouchPoints(this.grLayers);
                if (tPoints.Count == 2)
                {
                    //MessageBox.Show("11");
                    #region  双指操作
                    if (tPoints[0].Action == TouchAction.Down || tPoints[1].Action == TouchAction.Down)
                    {
                        lastFingerDistance = 0.0;
                        firstFingerDistance = 0.0;
                        //manipulationType = FingerManipulationType.Null;

                        //记录按下
                        originalPoints = e.GetTouchPoints(this.canLayers);
                        // MessageBox.Show(recClip.Width.ToString());

                        if (curTool == ToolStyle.Clip)
                        {
                            #region 裁剪相关变量
                            // tPoints = e.GetTouchPoints(this.canLayers.Children[selLayerIndex] as Border);
                            Point[] tPts = proofreadTPts(tPoints);              //校对点击的点是是否有屏幕中
                            Point pt = locatRec(tPts[0], tPts[1]);              //获取截剪的矩形
                            recClip.Margin = new Thickness(pt.X, pt.Y, 0, 0);
                            recClip.Width = Math.Abs(tPts[0].X - tPts[1].X);
                            recClip.Height = Math.Abs(tPts[0].Y - tPts[1].Y);
                            recClip.Visibility = Visibility.Visible;
                            #endregion
                        }
                        else if (curTool == ToolStyle.Zoom)
                        {
                            #region 图层缩放相关变量
                            scForm = new ScaleTransform();     //用于变换画板图层
                            if (canLayers.Children[selLayerIndex].RenderTransform is TransformGroup)
                            {
                                transFormGp = canLayers.Children[selLayerIndex].RenderTransform as TransformGroup;
                                foreach (Transform trans in transFormGp.Children)
                                {
                                    if (trans is ScaleTransform)
                                    {
                                        scForm = trans as ScaleTransform;
                                        transFormGp.Children.Remove(trans);
                                        break;

                                    }
                                }
                            }
                            else
                            {
                                transFormGp = new TransformGroup();
                            }

                            //TouchPointCollection tP = e.GetTouchPoints(canLayers.Children[selLayerIndex] as Border);
                            // if ((tP[0].Position.X + tP[1].Position.X) / 2 > 0 && (tP[0].Position.X + tP[1].Position.X) / 2 < (canLayers.Children[selLayerIndex] as Border).Width && (tP[0].Position.Y + tP[1].Position.Y) / 2 > 0 && (tP[0].Position.Y + tP[1].Position.Y) / 2 < (canLayers.Children[selLayerIndex] as Border).Height)
                            {
                                //   scForm.CenterX = (tPoints[0].Position.X + tPoints[1].Position.X) / 2;
                                //  scForm.CenterY = (tPoints[0].Position.Y + tPoints[1].Position.Y) / 2;
                            }
                            //  else
                            {
                                scForm.CenterX = (canLayers.Children[selLayerIndex] as Border).Width / 2;
                                scForm.CenterY = (canLayers.Children[selLayerIndex] as Border).Height / 2;
                            }
                            transFormGp.Children.Add(scForm);

                            canLayers.Children[selLayerIndex].RenderTransform = transFormGp;
                            #endregion
                        }
                        else if (curTool == ToolStyle.CanvasZoom)
                        {
                            scForm = new ScaleTransform();     //用于变换画板
                            if (this.canLayers.RenderTransform is ScaleTransform)
                            {
                                scForm = this.canLayers.RenderTransform as ScaleTransform;
                            }
                            else
                            {
                                this.canLayers.RenderTransform = scForm;
                            }
                            scForm.CenterX = canLayers.Width / 2;
                            scForm.CenterY = canLayers.Height / 2;
                        }
                    }

                    else if (tPoints[0].Action == TouchAction.Move && tPoints[1].Action == TouchAction.Move)
                    {
                        //当前两指间距离  
                        double fingerDistance = Math.Sqrt(Math.Pow(tPoints[0].Position.X - tPoints[1].Position.X, 2) + Math.Pow(tPoints[0].Position.Y - tPoints[1].Position.Y, 2));

                        //#region 两指缩放                         
                        if (lastFingerDistance != 0.0)
                        {
                            if (curTool == ToolStyle.Zoom || curTool == ToolStyle.CanvasZoom)
                            {
                                #region ---缩放
                                scForm.ScaleX *= (fingerDistance) / (lastFingerDistance);
                                scForm.ScaleY *= (fingerDistance) / (lastFingerDistance);
                                isMove = false;
                                #endregion
                            }
                            else if (curTool == ToolStyle.Clip)
                            {
                                #region 裁剪相关变量
                                // tPoints = e.GetTouchPoints(this.canLayers.Children[selLayerIndex] as Border);
                                Point[] tPts = proofreadTPts(tPoints);
                                Point pt = locatRec(tPts[0], tPts[1]);
                                recClip.Margin = new Thickness(pt.X, pt.Y, 0, 0);
                                recClip.Width = Math.Abs(tPts[0].X - tPts[1].X);
                                recClip.Height = Math.Abs(tPts[0].Y - tPts[1].Y);
                                //recClip.RadiusX = Math.Abs(tPoints[0].Position.X - tPoints[1].Position.X);
                                //recClip.RadiusY = Math.Abs(tPoints[0].Position.Y - tPoints[1].Position.Y);
                                // MessageBox.Show(recClip.Width.ToString());
                                #endregion
                            }

                        }
                        else
                        {
                            //两手指第一次按下时距离
                            firstFingerDistance = fingerDistance;
                        }

                        //最后两手指间间的距离:用于指示缩放
                        lastFingerDistance = fingerDistance;
                    }
                    // else if (tPoints[0].Action == TouchAction.Up || tPoints[1].Action == TouchAction.Up)
                    {
                        // if (curTool == ToolStyle.Clip)
                        {
                            //rbtOKClip.Visibility = Visibility.Visible;
                            //rbtCancleClip.Visibility = Visibility.Visible;
                        }

                    }
                    #endregion
                    // #endregion
                }
                else if (tPoints.Count == 1 && (curTool == ToolStyle.Zoom || curTool == ToolStyle.CanvasZoom))
                {
                    TouchPoint tp = e.GetPrimaryTouchPoint(this.canLayers.Children[selLayerIndex]);
                    TouchPoint tpls = e.GetPrimaryTouchPoint(grToolDetail);
                    bool hadClickedLS = grToolDetail.Visibility == Visibility.Visible && tpls.Position.X > 0 && tpls.Position.Y > 0 && tpls.Position.X < grToolDetail.Width && tpls.Position.Y < grToolDetail.Height;
                    bool hadClickedBorder = !hadClickedLS && tp.Position.X > 0 && tp.Position.Y > 0 && tp.Position.X < (this.canLayers.Children[selLayerIndex] as Border).Width && tp.Position.Y < (this.canLayers.Children[selLayerIndex] as Border).Height;

                    if (curTool == ToolStyle.CanvasZoom)
                    {
                        tp = e.GetPrimaryTouchPoint(this.grCanContainer);
                        hadClickedBorder = !hadClickedLS && tp.Position.X > 0 && tp.Position.Y > 0 && tp.Position.X < grCanContainer.Width && tp.Position.Y < grCanContainer.Height;
                    }

                    tp = e.GetPrimaryTouchPoint(this.canLayers);    //防止放大后移动工具栏，发生移动事件
                    hadClickedBorder = hadClickedBorder && tp.Position.X > 0 && tp.Position.Y > 0 && tp.Position.X < this.canLayers.Width && tp.Position.Y < this.canLayers.Height;


                    if (hadClickedBorder)
                    {
                        if (curTool == ToolStyle.Zoom)
                        {
                            #region 图层移动
                            if (tPoints[0].Action == TouchAction.Down)
                            {
                                lastTouchPoints = tPoints;
                                inkPosX = Canvas.GetLeft(canLayers.Children[selLayerIndex]);
                                inkPosY = Canvas.GetTop(canLayers.Children[selLayerIndex]);
                                isMove = true;
                            }
                            else if (isMove && tPoints[0].Action == TouchAction.Move)
                            {
                                if (lastTouchPoints != null && lastTouchPoints.Count > 0)
                                {

                                    inkPosX += (tPoints[0].Position.X - lastTouchPoints[0].Position.X);
                                    inkPosY += (tPoints[0].Position.Y - lastTouchPoints[0].Position.Y);
                                    Canvas.SetLeft(canLayers.Children[selLayerIndex], inkPosX);
                                    Canvas.SetTop(canLayers.Children[selLayerIndex], inkPosY);

                                }
                            }
                            #endregion

                        }
                        else
                        {
                            #region 画布移动

                            if (tPoints[0].Action == TouchAction.Down)
                            {
                                lastTouchPoints = tPoints;
                                inkPosX = Canvas.GetLeft(grCanContainer);
                                inkPosY = Canvas.GetTop(grCanContainer);
                                isMove = true;
                            }
                            else if (isMove && tPoints[0].Action == TouchAction.Move)
                            {
                                if (lastTouchPoints != null && lastTouchPoints.Count > 0)
                                {

                                    inkPosX += (tPoints[0].Position.X - lastTouchPoints[0].Position.X);
                                    inkPosY += (tPoints[0].Position.Y - lastTouchPoints[0].Position.Y);

                                    if (Math.Abs(inkPosX) < 380 && Math.Abs(inkPosY) < 550)
                                    {
                                        Canvas.SetLeft(grCanContainer, inkPosX);
                                        Canvas.SetTop(grCanContainer, inkPosY);
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    if (tPoints.Count == 1 && tPoints[0].Action == TouchAction.Up)
                    {
                        lastTouchPoints = null;
                        //isMove = false;
                    }
                    else
                    {
                        lastTouchPoints = tPoints;
                    }

                }


            }
        }


        #endregion


        #region 工具栏
        UCColorChooser colorPicker;
        private void initializeTools()
        {

            rbtLayers.Click += new RoutedEventHandler(rbtLayers_Click);
            rbtBrush.Click += new RoutedEventHandler(rbtBrush_Click);
            rbtUndo.Click += new RoutedEventHandler(rbtUndo_Click);
            rbtRedo.Click += new RoutedEventHandler(rbtRedo_Click);
            rbtZoom.Click += new RoutedEventHandler(rbtZoom_Click);
            rbtClip.Click += new RoutedEventHandler(rbtClip_Click);
            rbtSave.Click += new RoutedEventHandler(rbtSave_Click);
            rbtSetting.Click += new RoutedEventHandler(rbtSetting_Click);
            rbtEraser.Click += new RoutedEventHandler(rbtEraser_Click);
            rbtClear.Click += new RoutedEventHandler(rbtClear_Click);

            rbtSave.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(rbtSave_Hold);

            rbtBrush.MouseLeave += new MouseEventHandler(rbtBrush_MouseLeave);

            rbtRefeshTrans.Click += new RoutedEventHandler(rbtRefeshTrans_Click);
        }

        void rbtRefeshTrans_Click(object sender, RoutedEventArgs e)
        {

            // slAngle.Value = 0;

            inkPosX = 0;
            inkPosY = 0;
            Canvas.SetLeft(canLayers.Children[selLayerIndex], inkPosX);
            Canvas.SetTop(canLayers.Children[selLayerIndex], inkPosY);

            canLayers.Children[selLayerIndex].RenderTransform = null;
        }

        void rbtSave_Click(object sender, RoutedEventArgs e)
        {
            if (hadExecHold)
            {
                hadExecHold = false;
                return;
            }

            adGoogle.Visibility = Visibility.Collapsed;

            if (this.canLayers.Children.Count < 1)
            {
                MessageBox.Show(WPPhotoEditor.Resources.AppResources.HaveNoLayer);
                return;
            }
            try
            {
                ScaleTransform saveST = new ScaleTransform();
                if (this.canLayers.Children.Count == 1)
                {
                    #region  无效：2012/6/7
                    /*
                    //double slope = bmp.PixelHeight / newLayer.Height;
                    BitmapImage bImg = ((this.canLayers.Children[0] as Border).Background as ImageBrush).ImageSource as BitmapImage;
                    double slope = bImg.PixelHeight / (this.canLayers.Children[0] as Border).Height;

                    if (slope < 1)
                    {
                        slope = 1;
                    }

                    saveST.ScaleX = slope;
                    saveST.ScaleY = slope;
                    */
                    #endregion

                    saveST.ScaleX = App.SaveScale;
                    saveST.ScaleY = App.SaveScale;
                }
                else
                {
                    saveST.ScaleX = App.SaveScale;
                    saveST.ScaleY = App.SaveScale;
                }

                WriteableBitmap bmp;
                bmp = new WriteableBitmap(this.canLayers, saveST);
                MemoryStream stream = new MemoryStream();
                bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);

                MediaLibrary library = new MediaLibrary();
                string fileName = "WPPEditor" + DateTime.Now.ToString("yyyy-mm-dd_hh:mm:ss");

                stream.Seek(0, SeekOrigin.Begin);
                library.SavePicture(fileName, stream);
                stream.Close();
                MessageBox.Show(WPPhotoEditor.Resources.AppResources.HadSaveTo);
            }
            catch
            {
                MessageBox.Show(WPPhotoEditor.Resources.AppResources.SaveFailed);
            }


        }



        void rbtClip_Click(object sender, RoutedEventArgs e)
        {
            setChosenToolUI(sender);
            closeEditTool();
            closeColorPiker();

            adGoogle.Visibility = Visibility.Collapsed;


            setToolState(rbtClip);
            curTool = ToolStyle.Clip;

            closeTool();
            grCrop.Visibility = Visibility.Visible;
        }

        void rbtZoom_Click(object sender, RoutedEventArgs e)
        {
            setChosenToolUI(sender);
            closeEditTool();
            closeColorPiker();
            showZoomTool();


            curTool = ToolStyle.Zoom;
            grCrop.Visibility = Visibility.Collapsed;
            adGoogle.Visibility = Visibility.Visible;

        }

        void rbtBrush_Click(object sender, RoutedEventArgs e)
        {
            setChosenToolUI(sender);

            showEidtTool();
            closeColorPiker();

            curTool = ToolStyle.Brush;

            //   updateCurBrush();        

            adGoogle.Visibility = Visibility.Visible;
            grCrop.Visibility = Visibility.Collapsed;

        }

        void rbtLayers_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show(WPPhotoEditor.Resources.AppResources.tMergeLayers, "", MessageBoxButton.OKCancel))
            {
                combineLayers();


            }
            grCrop.Visibility = Visibility.Collapsed;
            return;
            curTool = ToolStyle.CanvasZoom;
            setChosenToolUI(sender);
            closeEditTool();
            closeColorPiker();
            closeTool();

            adGoogle.Visibility = Visibility.Visible;


        }

        void rbtEraser_Click(object sender, RoutedEventArgs e)
        {
            setChosenToolUI(rbtBrush);
            curTool = ToolStyle.Brush;

            rbtUndo.Visibility = Visibility.Visible;
            rbtRedo.Visibility = Visibility.Visible;
            rbtClear.Visibility = Visibility.Visible;

            if (colorPicker == null)
            {
                try
                {
                    colorPicker = new UCColorChooser();
                    colorPicker.setICOUI = setPenTypeUI;
                    this.LayoutRoot.Children.Add(colorPicker);
                    colorPicker.ChosenPenType = curBrush.BrushPenType;
                }
                catch { }
            }
            else
            {
                if (colorPicker.Visibility == Visibility.Visible)
                {
                    closeColorPiker();
                }
                else
                {
                    colorPicker.Visibility = Visibility.Visible;
                    colorPicker.ChosenPenType = curBrush.BrushPenType;
                }
            }

            grCrop.Visibility = Visibility.Collapsed;
        }

        void rbtRedo_Click(object sender, RoutedEventArgs e)
        {
            closeColorPiker();
            LayerState lState = unReManager.GetRedoCollection();
            if (lState != null)
            {
                if (lState.isDrawing)
                {
                    unReManager.AddUndoCollection(createDrawingUndoRecord());
                }
                else
                {
                    unReManager.AddUndoCollection(createUndoRecord());
                }
            }
            setLayerStateRecord(lState);

            if (unReManager.redoLayerStateCollection.Count < 1)
            {
                rbtRedo.IsEnabled = false;
            }

            rbtUndo.IsEnabled = true;
        }

        void rbtUndo_Click(object sender, RoutedEventArgs e)
        {
            closeColorPiker();
            LayerState lState = unReManager.GetUndoCollection();
            if (lState != null)
            {
                if (lState.isDrawing)
                {
                    unReManager.AddRedoCollection(createDrawingUndoRecord());
                }
                else
                {
                    unReManager.AddRedoCollection(createUndoRecord());
                }
            }
            setLayerStateRecord(lState);

            if (unReManager.undoLayerStateCollection.Count < 1)
            {
                rbtUndo.IsEnabled = false;
            }
            rbtRedo.IsEnabled = true;
        }

        void rbtClear_Click(object sender, RoutedEventArgs e)
        {
            closeColorPiker();
            adGoogle.Visibility = Visibility.Collapsed;

            if (curTool == ToolStyle.Brush && getCurLayerControl() == btDrawingBoardControl && (curBrush.BrushPenType == PenType.Color || curBrush.BrushPenType == PenType.Original))
            {
                brDrawingBoard.Background = null;
                return;
            }
            //清空画笔     
            grPenDetail.Visibility = Visibility.Collapsed;
            if (selLayerIndex < lsLayers.Items.Count && selLayerIndex > -1)
            {
                this.canLayers.Children[selLayerIndex].OpacityMask = null;
            }


        }

        void rbtSetting_Click(object sender, RoutedEventArgs e)
        {
            useMine = !this.useMine;
            // closeColorPiker();
            NavigationService.Navigate(new Uri("/OLD/SettingPhonePage.xaml",UriKind.RelativeOrAbsolute));
        }




        void rbtSave_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            adGoogle.Visibility = Visibility.Collapsed;
            CaptureScreen(sender, e);

            hadExecHold = true;
            return;

        }

        void rbtClip_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            adGoogle.Visibility = Visibility.Collapsed;
            this.canLayers.Clip = null;

        }



        void rbtBrush_MouseLeave(object sender, MouseEventArgs e)
        {
            brPen.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// 设置已选择的UI状态
        /// </summary>
        /// <param name="rButton"></param>
        private void setToolState(Coding4Fun.Phone.Controls.RoundButton rButton)
        {

        }



        private void showEidtTool()
        {
            // grPenDetail.Visibility = Visibility.Visible;
            grToolDetail.Visibility = Visibility.Visible;

            lsBrushs.Visibility = Visibility.Visible;
            grAngle.Visibility = Visibility.Collapsed;
            grClip.Visibility = Visibility.Collapsed;

            rbtUndo.Visibility = Visibility.Visible;
            rbtRedo.Visibility = Visibility.Visible;
            rbtClear.Visibility = Visibility.Visible;

            isEnableEdit = false;       //用于点击画板，关闭编辑栏，但先不绘图,
        }

        private void showZoomTool()
        {
            grPenDetail.Visibility = Visibility.Collapsed;
            grToolDetail.Visibility = Visibility.Visible;

            lsBrushs.Visibility = Visibility.Collapsed;
            grAngle.Visibility = Visibility.Visible;
            grClip.Visibility = Visibility.Collapsed;
        }


        //关闭编辑（绘图）的UI元素(含closeTool())
        private void closeEditTool()
        {
            if (curTool == ToolStyle.Brush)
            {
                rbtUndo.Visibility = Visibility.Collapsed;
                rbtRedo.Visibility = Visibility.Collapsed;
                rbtClear.Visibility = Visibility.Collapsed;

                closeTool();
            }
        }


        //关闭工具栏选项、画刷选项（操作画板时总调用）
        private void closeTool()
        {
            if (grToolDetail.Visibility == Visibility.Visible)
            {
                grPenDetail.Visibility = Visibility.Collapsed;
                grToolDetail.Visibility = Visibility.Collapsed;

                if (curTool == ToolStyle.Brush)
                {

                    isEnableEdit = true;

                    updateCurBrush();

                    brPen.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 标记工具选中状态
        /// </summary>
        /// <param name="sender"></param>
        private void setChosenToolUI(object sender)
        {
            if (sender != null)
            {
                foreach (Coding4Fun.Phone.Controls.RoundButton rButton in lsTools.Items)
                {
                    rButton.Foreground = App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
                    rButton.BorderBrush = App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
                }

                Coding4Fun.Phone.Controls.RoundButton curButton = sender as Coding4Fun.Phone.Controls.RoundButton;
                curButton.Foreground = App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                curButton.BorderBrush = App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
            }

        }

        /// <summary>
        /// 合并图层
        /// </summary>
        private void combineLayers()
        {
            WriteableBitmap bmp;
            ScaleTransform saveST = new ScaleTransform();
            saveST.ScaleX = 3;
            saveST.ScaleY = 3;
            bmp = new WriteableBitmap(this.canLayers, saveST);

            List<Border> delBrList = new List<Border>();
            List<Button> delBtList = new List<Button>();
            for (int j = 0; j < lsLayers.Items.Count; j++)
            {
                selLayerIndex = j;
                Border selBr = getCurLayer();
                Button selBt = getCurLayerControl();

                if (selBr == brDrawingBoard)
                {
                    brDrawingBoard.Background = null;
                    brDrawingBoard.OpacityMask = null;
                }
                else
                {
                    delBrList.Add(selBr);
                    delBtList.Add(selBt);
                }
            }

            foreach (Border br in delBrList)
            {
                this.canLayers.Children.Remove(br);
            }

            foreach (Button bt in delBtList)
            {
                this.lsLayers.Items.Remove(bt);
            }

            addPicture(bmp);
            selLayerIndex = 0;

        }

        #endregion


        #region 图层管理
        void rbtDelLayer_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show(WPPhotoEditor.Resources.AppResources.tDelLayerConfirm, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (getCurLayer() == brDrawingBoard)
                {
                    brDrawingBoard.Background = null;
                    brDrawingBoard.OpacityMask = null;
                    return;
                }


                if (selLayerIndex > -1 && selLayerIndex < lsLayers.Items.Count)
                {

                    lsLayers.Items.RemoveAt(selLayerIndex);
                    canLayers.Children.RemoveAt(selLayerIndex);

                    #region 更新撤消/重做的状态
                    unReManager.DelUnDoOnLayerIndex(selLayerIndex);
                    if (unReManager.undoLayerStateCollection.Count < 1)
                    {
                        rbtUndo.IsEnabled = false;
                    }

                    if (unReManager.redoLayerStateCollection.Count < 1)
                    {
                        rbtRedo.IsEnabled = false;
                    }
                    #endregion


                    selLayerIndex = 0;

                    for (int i = canLayers.Children.Count - 1; i > -1; i--)
                    {
                        if (canLayers.Children[i].Visibility == Visibility.Visible)
                        {
                            newLayerControl_Click(lsLayers.Items[i], null);
                            selLayerIndex = i;
                            break;
                        }
                    }

                }


                return;
                Border layer = getCurLayer();
                if (layer != null)
                {
                    this.canLayers.Children.Remove(layer);
                }

                Button layerControl = getCurLayerControl();
                if (layerControl != null)
                {
                    this.lsLayers.Items.Remove(layerControl);
                }
            }



        }

        void rbtAddLay_Click(object sender, RoutedEventArgs e)
        {
            if (lsLayers.Items.Count > 4)
            {
                MessageBox.Show(WPPhotoEditor.Resources.AppResources.OnlyLayer);
                return;
            }
            isLayerAddHandle = true;
            ImageChooserClicked(null, null);
        }

        //图片选择按钮点击
        private void ImageChooserClicked(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);

            try
            {
                photoChooserTask.Show();
            }
            catch
            {
                MessageBox.Show("An error occurred.");
            }
        }

        //图片选择后返回并执行
        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //MessageBox.Show(e.OriginalFileName);

                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);//获取返回的图片                           


                if (isLayerAddHandle)
                {

                    // bmp = new BitmapImage(new Uri("3TMRNDOS008V0008.jpg", UriKind.RelativeOrAbsolute));
                    addPicture(new WriteableBitmap(bmp));
                }
                else
                {
                    if (selLayerIndex > -1 && selLayerIndex < lsLayers.Items.Count)
                    {
                        Border br = canLayers.Children[selLayerIndex] as Border;
                        addPicture(bmp, ref br, this.canLayers);
                    }
                }

                rbtZoom_Click(rbtZoom, null);
                grToolDetail.Visibility = Visibility.Visible;
                // addPicture(bmp);
                lsTools.IsEnabled = true;
                rbtDelLayer.IsEnabled = true;
            }
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="inkPDrawing"></param>
        private void addPicture(WriteableBitmap bmp)
        {
            // showMemory();

            Border newLayer = new Border();
            newLayer.Width = 480;
            newLayer.Height = 640;
            newLayer.DoubleTap += new EventHandler<System.Windows.Input.GestureEventArgs>(newLayer_DoubleTap);


            double slope = bmp.PixelHeight / newLayer.Height;
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = bmp;
            imgBrush.Stretch = Stretch.Fill;
            newLayer.Background = imgBrush;


            if (bmp.PixelWidth != 0)
            {
                Size sz = WPPhotoEditor.Utility.UtilityFunction.GetImgShowSize(new Size(newLayer.Width, newLayer.Height), new Size(bmp.PixelWidth, bmp.PixelHeight));
                newLayer.Width = sz.Width;
                newLayer.Height = sz.Height;
            }

            addLayerToCanvas(newLayer);

            addLayerControl(new WriteableBitmap(bmp));

            //showMemory();
            // curBrush.BrushPenType = PenType.Transparent;
            ;
            brPen.Visibility = Visibility.Collapsed;
        }

        private void addLayerControl(WriteableBitmap bmp)
        {
            Button newLayerControl = new Button();
            newLayerControl.Width = 129;
            newLayerControl.Height = 70;
            newLayerControl.Margin = new Thickness(-20, 0, 0, 0);
            newLayerControl.Click += new RoutedEventHandler(newLayerControl_Click);                //标识当前点击（选择）的图层
            newLayerControl.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(newLayerControl_Hold);           //显示/隐藏图层
            newLayerControl.DoubleTap += new EventHandler<System.Windows.Input.GestureEventArgs>(newLayerControl_DoubleTap);
            newLayerControl.MouseLeftButtonDown += new MouseButtonEventHandler(grToolDetail_MouseLeftButtonDown);
            newLayerControl.Tag = lsLayers.Items.Count;


            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = bmp;
            imgBrush.Stretch = Stretch.Fill;
            newLayerControl.Background = imgBrush;


            if (lsLayers.Items[lsLayers.Items.Count - 1] != btDrawingBoardControl)
            {
                lsLayers.Items.Add(newLayerControl);
            }
            else
            {
                lsLayers.Items.Insert(lsLayers.Items.Count - 1, newLayerControl);
            }
            newLayerControl_Click(newLayerControl, null);
        }



        void newLayer_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (curTool == ToolStyle.Zoom)
            {
                inkPosX = 0;
                inkPosY = 0;
                Canvas.SetLeft(canLayers.Children[selLayerIndex], inkPosX);
                Canvas.SetTop(canLayers.Children[selLayerIndex], inkPosY);
            }

        }

        /// <summary>
        /// 修改对应图层的图像
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="backgroundImg"></param>
        /// <param name="inkPDrawing"></param>
        private void addPicture(BitmapImage bmp, ref Border backgroundImg, FrameworkElement inkPDrawing)
        {

            double slope = bmp.PixelHeight / backgroundImg.Height;
            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = bmp;
            imgBrush.Stretch = Stretch.Fill;
            backgroundImg.Background = imgBrush;

            Size sz = WPPhotoEditor.Utility.UtilityFunction.GetImgShowSize(new Size(backgroundImg.Width, backgroundImg.Height), new Size(bmp.PixelWidth, bmp.PixelHeight));
            backgroundImg.Width = sz.Width;
            backgroundImg.Height = sz.Height;
            backgroundImg.Visibility = Visibility.Visible;

            (lsLayers.Items[selLayerIndex] as Button).Background = imgBrush;

            // Canvas.SetLeft(backgroundImg,0);
            // Canvas.SetTop(backgroundImg, 0);         
        }


        //显示/隐藏图层
        void newLayerControl_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selLayerIndex = this.lsLayers.Items.IndexOf(sender);

            Border br = getCurLayer();
            Button bt = getCurLayerControl();
            if (br.Visibility == Visibility.Visible)
            {
                //bt.BorderBrush = App.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
                bt.BorderThickness = new Thickness(0, 0, 0, 0);
                br.Visibility = Visibility.Collapsed;

                for (int i = canLayers.Children.Count - 1; i > -1; i--)
                {
                    if (canLayers.Children[i].Visibility == Visibility.Visible)
                    {
                        newLayerControl_Click(lsLayers.Items[i], null);
                        break;
                    }
                }
            }
            else
            {
                // bt.BorderBrush = App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                bt.BorderThickness = new Thickness(3, 3, 3, 3);
                br.Visibility = Visibility.Visible;
                hadExecHold = false;
            }


            hadExecHold = true;
        }

        //选择点击的图层
        void newLayerControl_Click(object sender, RoutedEventArgs e)
        {
            if (hadExecHold)
            {
                hadExecHold = false;
                return;
            }

            setLayerControlState(sender as Button);

            if (sender != btDrawingBoardControl)
            {
                if (curBrush.BrushPenType == PenType.Color || curBrush.BrushPenType == PenType.Original)
                {
                    curBrush.BrushPenType = PenType.Transparent;
                    setPenTypeUI(curBrush.BrushPenType);
                    updateCurBrush();
                }
            }

        }

        void newLayerControl_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            moveLayerToTheTop();
            // isLayerAddHandle = false;
            // ImageChooserClicked(null, null);
        }

        private Border getCurLayer()
        {
            if (selLayerIndex >= 0 && selLayerIndex < this.canLayers.Children.Count)
            {
                Border layer = this.canLayers.Children[selLayerIndex] as Border;
                return layer;
            }
            else
            {
                return null;
            }
        }

        private Button getCurLayerControl()
        {
            if (selLayerIndex >= 0 && selLayerIndex < this.lsLayers.Items.Count)
            {
                Button bt = this.lsLayers.Items[selLayerIndex] as Button;

                return bt;
            }
            return null;
        }


        private void setLayerControlState(Button selLayerControl)
        {
            foreach (Button bt in lsLayers.Items)
            {
                if (bt != selLayerControl)
                {
                    bt.BorderBrush = App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
                }
                else
                {
                    bt.BorderBrush = App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                }
            }

            selLayerIndex = lsLayers.Items.IndexOf(selLayerControl);


            if (selLayerControl == btDrawingBoardControl)
            {
                rbtZoom.Visibility = Visibility.Collapsed;

                setChosenToolUI(rbtBrush);
                curTool = ToolStyle.Brush;

                rbtUndo.Visibility = Visibility.Visible;
                rbtRedo.Visibility = Visibility.Visible;
                rbtClear.Visibility = Visibility.Visible;

                rbtClip.Visibility = Visibility.Collapsed;
                grCrop.Visibility = Visibility.Collapsed;
            }
            else
            {
                rbtZoom.Visibility = Visibility.Visible;
                rbtClip.Visibility = Visibility.Visible;
                //grCrop.Visibility = Visibility.Visible;


            }
        }


        private void moveLayerToTheTop()
        {
            Border layer = getCurLayer();
            Button btLayer = this.lsLayers.Items[this.canLayers.Children.IndexOf(layer)] as Button;

            this.canLayers.Children.Remove(layer);
            this.lsLayers.Items.Remove(btLayer);

            canLayers.Children.Add(layer);
            this.lsLayers.Items.Add(btLayer);

            setLayerControlState(btLayer);


            selLayerIndex = this.canLayers.Children.Count - 1;
        }


        private void addLayerToCanvas(Border br)
        {
            if (canLayers.Children.Count < 1 || canLayers.Children[canLayers.Children.Count - 1] != brDrawingBoard)
            {
                canLayers.Children.Add(br);
            }
            else
            {
                canLayers.Children.Insert(canLayers.Children.Count - 1, br);
            }
        }

        #region 在缩放时过滤无效的移动
        void grToolDetail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            screenTouch = false;
        }

        void grToolDetail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            screenTouch = true;
        }

        void grToolDetail_MouseLeave(object sender, MouseEventArgs e)
        {
            screenTouch = false;
        }

        #endregion
        #endregion


        #region 图层对象（字段与方法）
        private bool hadExecHold = false;   //刚执行完隐藏操作，则在点击事件中，禁止显示

        Point lastPt;


        private void Touch_FrameTransparent(TouchFrameEventArgs e)
        {

            if (curTool == ToolStyle.Brush && curBrush.BrushPenType == PenType.Transparent || curBrush.BrushPenType == PenType.Opacity)
            {
                Border curBorder = getCurLayer();
                if (curBorder != null)
                {
                    TouchPoint tPoint = e.GetPrimaryTouchPoint(curBorder);
                    if (tPoint.Action == TouchAction.Down)
                    {
                        newLayer_MouseLeftButtonDown(tPoint.Position);
                    }
                    else if (tPoint.Action == TouchAction.Move)
                    {
                        newLayer_MouseMove(tPoint.Position);
                    }
                    else
                    {
                        newLayer_MouseLeftButtonUp(tPoint.Position);

                        closeTool();
                        closePenDetailSetting();
                    }
                }
            }
        }



        #region 动作
        void newLayer_MouseLeftButtonDown(Point pt)
        {
            if (!isEnableEdit)
            {
                return;
            }

            if (curTool == ToolStyle.Brush)
            {
                unReManager.AddUndoCollection(createUndoRecord());
                rbtUndo.IsEnabled = true;
            }

            lastPt = pt;
        }

        void newLayer_MouseMove(Point pt)
        {
            // tTest.Text = e.GetPosition(this).ToString();

            if (!isEnableEdit)
            {
                return;
            }

            if (curTool != ToolStyle.Brush)
            {
                return;
            }

            if (selLayerIndex > -1 && selLayerIndex < lsLayers.Items.Count)
            {


                Border curBr = this.canLayers.Children[selLayerIndex] as Border;
                WriteableBitmap wBmp;
                if (curBr.OpacityMask != null)
                {
                    wBmp = (curBr.OpacityMask as ImageBrush).ImageSource as WriteableBitmap;
                }
                else
                {
                    if (curBr == this.brDrawingBoard)
                    {
                        wBmp = new WriteableBitmap(this.grCanContainer, null);
                    }
                    else
                    {
                        wBmp = new WriteableBitmap(curBr, null);
                    }
                }
                // Point pt = e.GetPosition(curBr);

                if (curBrush.BrushPenType == PenType.Color)
                {
                    drawBrushFhColor(lastPt, pt, curBrush.TargetBrush, curBr, new WriteableBitmap((curBr.Background as ImageBrush).ImageSource as BitmapSource));
                }
                else
                {
                    drawBrushFh(lastPt, pt, curBrush.TargetBrush, curBr, wBmp);

                    ImageBrush ibrush = new ImageBrush();
                    ibrush.ImageSource = wBmp;
                    ibrush.Stretch = Stretch.Uniform;
                    curBr.OpacityMask = ibrush;

                }

                lastPt = pt;
            }


        }

        bool useMine = false;

        void newLayer_MouseLeftButtonUp(Point pt)
        {
            if (!isEnableEdit)
            {
                return;
            }

            if (hadCollapsed)
            {
                hadCollapsed = false;
                return;
            }

            if (curTool != ToolStyle.Brush)
            {
                return;
            }



            if (selLayerIndex > -1 && selLayerIndex < lsLayers.Items.Count)
            {

                Border curBr = this.canLayers.Children[selLayerIndex] as Border;
                //  Point pt = e.GetPosition(curBr);

                WriteableBitmap wBmp;
                if (curBr.OpacityMask != null)
                {
                    wBmp = (curBr.OpacityMask as ImageBrush).ImageSource as WriteableBitmap;
                }
                else
                {
                    if (curBr == this.brDrawingBoard)
                    {
                        wBmp = new WriteableBitmap(this.grCanContainer, null);
                    }
                    else
                    {
                        wBmp = new WriteableBitmap(curBr, null);
                    }
                }

                // if (curBrush.BrushPenType == PenType.Color)
                {
                    //   drawBrushFhColor(lastPt, pt, curBrush.TargetBrush, curBr, new WriteableBitmap((curBr.Background as ImageBrush).ImageSource as BitmapSource));
                }
                // else
                {
                    drawBrushFh(lastPt, pt, curBrush.TargetBrush, curBr, wBmp);
                }



                lastPt = pt;
            }
        }
        #endregion


        private void selfEraserBmp(WriteableBitmap wEraserBmp)
        {
            for (int i = 0; i < wEraserBmp.PixelWidth; i++)
            {
                for (int j = 0; j < wEraserBmp.PixelHeight; j++)
                {
                    byte[] rgb = BitConverter.GetBytes(wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i]);
                    rgb[3] = Convert.ToByte(255 - rgb[3]);    //获取粉刷的不透明度
                    int pixelValue = BitConverter.ToInt32(rgb, 0);
                    wEraserBmp.Pixels[j * (wEraserBmp.PixelWidth) + i] = BitConverter.ToInt32(rgb, 0);
                }
            }
        }

        /// <summary>
        /// 画
        /// </summary>
        /// <param name="FromPt"></param>
        /// <param name="ToPt"></param>
        /// <param name="wPNGbp"></param>
        /// <param name="curLayer"></param>
        /// <param name="wBmp"></param>
        private void drawBrushFh(Point FromPt, Point ToPt, WriteableBitmap wPNGbp, Border curLayer, WriteableBitmap wBmp)
        {
            int x1 = (int)(FromPt.X - wPNGbp.PixelWidth / 2);
            int y1 = (int)(FromPt.Y - wPNGbp.PixelHeight / 2);
            int x2 = (int)(ToPt.X - wPNGbp.PixelWidth / 2);
            int y2 = (int)(ToPt.Y - wPNGbp.PixelHeight / 2);
            drawEachPointForLine(x1, y1, x2, y2, wPNGbp, curLayer, wBmp);

            ImageBrush ibrush = new ImageBrush();
            ibrush.ImageSource = wBmp;
            ibrush.Stretch = Stretch.Uniform;

            curLayer.OpacityMask = ibrush;
        }


        /// <summary>
        /// 计算要描述的像素
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="brush"></param>
        /// <param name="pointAction"></param>
        public void drawEachPointForLine(int x1, int y1, int x2, int y2, WriteableBitmap wPNGbp, Border curLayer, WriteableBitmap wBmp)
        {
            int num5;
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;// 最大差

            int num = x2 - x1;
            int num2 = y2 - y1;



            int num3 = 0;    //正负1：指示X方向
            if (num < 0)
            {  //左－右
                num = -num;
                num3 = -1;
            }
            else if (num > 0)
            {
                //右－左
                num3 = 1;
            }

            int num4 = 0;    ////正负1：指示Y向
            if (num2 < 0)
            {
                //上－下
                num2 = -num2;
                num4 = -1;
            }
            else if (num2 > 0)
            {
                //下-上
                num4 = 1;
            }


            if (num > num2)    //x差>y差
            {
                //大致横向
                num5 = num3;     //X第次递进
                num6 = 0;        //Y第次递进

                num7 = num3;              //X第次递进
                num8 = num4;
                num9 = num2;
                num10 = num;
            }
            else
            {
                //大致竖向
                num5 = 0;                //X：恒定
                num6 = num4;             //Y每次递进

                num7 = num3;
                num8 = num4;         //Y每次递进
                num9 = num;
                num10 = num2;
            }

            #region 第一个点绘
            int x = x1;
            int y = y1;
            int num13 = num10 >> 1;
            DrawTheOpacity(x, y, wPNGbp, wBmp.Pixels, wBmp.PixelWidth, wBmp.PixelHeight);
            // setPicturePngTsp(new Point(x, y), wPNGbp, curLayer);
            #endregion


            //num10:Max(X差；Y差)
            for (double i = 0; i < num10; i++)
            {
                num13 -= num9;
                if (num13 < 0)
                {
                    num13 += num10;
                    x += num7;
                    y += num8;
                }
                else
                {
                    x += num5;
                    y += num6;
                }

                i += slSpacing.Value;
                DrawTheOpacity(x, y, wPNGbp, wBmp.Pixels, wBmp.PixelWidth, wBmp.PixelHeight);
            }

        }

        /// <summary>
        /// 实际描绘像素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wPNGbp">画刷图像</param>
        /// <param name="destPixels">目标蒙版</param>
        /// <param name="destWidth"></param>
        /// <param name="destHeight"></param>
        public void DrawTheOpacity(int x, int y, WriteableBitmap wPNGbp, int[] destPixels, int destWidth, int destHeight)
        {
            int width = wPNGbp.PixelWidth;     //画刷宽度
            int height = wPNGbp.PixelHeight;   //画刷高度
            int[] data = wPNGbp.Pixels;
            int xRange = x + width;   //指定X绘制范围
            int yRange = y + height;   //指定Y绘制范围

            //如果待画坐标于图像内则执行
            if ((x < destWidth) && (y < destHeight))
            {
                int xReal;
                int yReal;
                int num7;
                int num8;

                #region 校正X在图像内：Num5；         Num7用于描绘画刷非中心边缘（当中心点不在待绘图像中，但其边缘在时）
                if (x < 0)
                {
                    xReal = 0;
                    num7 = -x;
                }
                else
                {
                    xReal = x;
                    num7 = 0;
                }
                #endregion

                #region 校正Y在图像内：Num6;          Num8用于描绘画刷非中心边缘（当中心点不在待绘图像中，但其边缘在时）
                if (y < 0)
                {
                    yReal = 0;
                    num8 = -y;
                }
                else
                {
                    yReal = y;
                    num8 = 0;
                }
                #endregion

                #region 校正于图像内
                if (xRange > destWidth)
                {
                    xRange = destWidth;
                }

                if (yRange > destHeight)
                {
                    yRange = destHeight;
                }
                #endregion

                int num11 = xRange - xReal;    //==x

                int num12 = width - num11;
                int num13 = destWidth - num11;

                int num14 = yRange - yReal;    //==y

                int num9 = num7 + num11;   //真实的X待绘点
                int num10 = num8 + num14;   //真实的Y待绘点


                if ((num9 >= 0) && (num10 >= 0))
                {
                    int num16 = 0;
                    int index = num7 + (num8 * width);            //画刷像素索引
                    int num18 = xReal + (yReal * destWidth);        //待绘像素索引
                    for (int i = yReal; i < yRange; i++)
                    {
                        for (int j = xReal; j < xRange; j++)
                        {
                            int num15 = data[index];
                            if (num15 == 0)
                            {
                                index++;
                                num18++;
                            }
                            else
                            {
                                num16 = num15 >> 24;


                                if (curBrush.BrushPenType == PenType.Transparent)
                                {
                                    if ((byte)(destPixels[num18] >> 24) > (byte)num16)
                                    {
                                        destPixels[num18] = data[index];
                                    }
                                }
                                else
                                {
                                    if ((byte)(destPixels[num18] >> 24) < (byte)num16)
                                    {
                                        destPixels[num18] = data[index];
                                    }
                                }
                                index++;
                                num18++;


                            }

                        }
                        index += num12;    //画刷图像下一行
                        num18 += num13;    //待画图像下一行像素
                    }
                }


            }
        }
        #endregion



        #region 画刷
        WPPhotoEditor.OLD.Entity.BrushEntity curBrush = new Entity.BrushEntity();

        private bool isEnableEdit = true;

        private void initializeBrushes()
        {
            curBrush.selButton = btDefaultPen;

            updateCurBrush();


            slPenSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slPenSize_ValueChanged);
            slTransparency.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slTransparency_ValueChanged);
            slEclosion.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slEclosion_ValueChanged);

        }

        // 画笔
        void btBrBuffer_Click(object sender, RoutedEventArgs e)
        {
            showPenDetailSetting(sender as Button);



            #region 设置画刷点选状态
            SolidColorBrush sbr = new SolidColorBrush();
            sbr.Color = Color.FromArgb(255, 31, 31, 31);

            if (curBrush.selButton != null)
            {
                curBrush.selButton.BorderBrush = sbr;
            }

            (sender as Button).BorderBrush = App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
            curBrush.selButton = sender as Button;
            #endregion

            curBrush.selButton = sender as Button;

        }


        void slEclosion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setPenPreview();
        }

        void slTransparency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setPenPreview();
        }

        void slPenSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setPenPreview();
        }


        /// <summary>
        /// 关闭颜色选择：并生成目标画刷（颜色/透明/不透明）
        /// </summary>
        protected void closeColorPiker()
        {
            if (colorPicker != null && colorPicker.Visibility == Visibility.Visible)
            {
                colorPicker.Visibility = Visibility.Collapsed;
                curBrush.BrushPenType = colorPicker.ChosenPenType;
                curBrush.BrushColor = colorPicker.hadChosenColor;

                setPenTypeUI(curBrush.BrushPenType);

                updateCurBrush();

                if (curBrush.BrushPenType == PenType.Color || curBrush.BrushPenType == PenType.Original)
                {
                    newLayerControl_Click(btDrawingBoardControl, null);
                }

                if (grPenDetail.Visibility == Visibility.Visible)
                {
                    showPenDetailSetting(curBrush.selButton);
                }
            }
        }

        /// <summary>
        /// 生成目标画刷（颜色/透明/不透明）
        /// </summary>
        private void updateCurBrush()
        {
            curBrush.BrushSize = new Size(slPenSize.Value, slPenSize.Value);
            curBrush.BrushOpacityValue = Convert.ToInt32(255 - slTransparency.Value);

            if (curBrush.selButton == btDefaultPen)
            {
                curBrush.BrushImg = createDefaultPen(slEclosion.Value, slPenSize.Value);
            }
            else
            {
                curBrush.BrushImg = (curBrush.selButton.Background as ImageBrush).ImageSource;
            }
            curBrush.CreateTargetBrush();
        }


        /// <summary>
        /// 创建默认画刷的PNG图像(黑色透明底)
        /// </summary>
        /// <param name="hardness"></param>
        /// <param name="sizeWidth"></param>
        /// <returns></returns>
        private WriteableBitmap createDefaultPen(double hardness, double sizeWidth)
        {
            Border brPenView = new Border();
            Ellipse elPenView = new Ellipse();

            RadialGradientBrush rGrBrush = new RadialGradientBrush();
            GradientStop grCenter = new GradientStop();
            GradientStop grEnd = new GradientStop();

            grCenter.Color = Colors.Black;
            grCenter.Offset = hardness;

            grEnd.Color = Colors.Transparent;
            grEnd.Offset = 1;

            rGrBrush.GradientStops.Add(grCenter);
            rGrBrush.GradientStops.Add(grEnd);


            brPenView.Width = sizeWidth;
            brPenView.Height = sizeWidth;
            elPenView.Width = sizeWidth;
            elPenView.Height = sizeWidth;
            elPenView.Fill = rGrBrush;
            brPenView.Child = elPenView;

            WriteableBitmap wbmp = new WriteableBitmap(brPenView, null);
            return wbmp;
        }


        private void setPenTypeUI(PenType pentype)
        {
            switch (pentype)
            {
                case PenType.Color: rbtEraser.ImageSource = WPPhotoEditor.Utility.UtilityFunction.GetICOResource("Color.png"); break;
                case PenType.Original: rbtEraser.ImageSource = WPPhotoEditor.Utility.UtilityFunction.GetICOResource("ColorOr.png"); break;
                case PenType.Transparent: rbtEraser.ImageSource = WPPhotoEditor.Utility.UtilityFunction.GetICOResource("transParent.png"); break;
                default: rbtEraser.ImageSource = WPPhotoEditor.Utility.UtilityFunction.GetICOResource("Opacity.png"); break;
            }
        }


        /// <summary>
        /// 生成画笔预览图像
        /// </summary>
        /// <param name="sizeWidth">画笔宽度</param>
        /// <param name="opacity">画笔透明度</param>
        /// <param name="hardness">画笔软硬特性</param>
        /// <param name="brPenView">显示容器</param>
        /// <param name="elOrButton">指示PNG画笔还是自定义画笔</param>
        void setPenView(int sizeWidth, double opacity, double hardness, Border brPenView, object elOrButton)
        {
            if (elOrButton is Ellipse)
            {
                Ellipse elPenView = elOrButton as Ellipse;

                RadialGradientBrush rGrBrush = new RadialGradientBrush();
                GradientStop grCenter = new GradientStop();
                GradientStop grEnd = new GradientStop();

                grCenter.Color = Colors.Black;
                grCenter.Offset = hardness;

                grEnd.Color = Colors.Transparent;
                grEnd.Offset = 1;

                rGrBrush.GradientStops.Add(grCenter);
                rGrBrush.GradientStops.Add(grEnd);


                brPenView.Width = sizeWidth;
                brPenView.Height = sizeWidth;
                elPenView.Width = sizeWidth;
                elPenView.Height = sizeWidth;
                elPenView.Fill = rGrBrush;
                elPenView.Opacity = opacity / 255;
            }
            else
            {
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = ((elOrButton as Button).Background as ImageBrush).ImageSource;
                imgBrush.Stretch = Stretch.Fill;
                imgBrush.Opacity = opacity;
                brPenView.Width = sizeWidth;
                brPenView.Height = sizeWidth;
                brPenView.Background = imgBrush;
            }
        }


        //显示画笔预览
        void setPenPreview()
        {
            if (curBrush.selButton == null)
            {
                curBrush.selButton = btDefaultPen;
            }

            if (curBrush.BrushPenType == PenType.Transparent)
            {
                brPen.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                brPen.Background = null;
            }
            brPen.BorderThickness = new Thickness(1, 1, 1, 1);
            brPen.Visibility = Visibility.Visible;
            if (curBrush.selButton == btDefaultPen)
            {
                elPen.Visibility = Visibility.Visible;
                setPenView((int)slPenSize.Value, slTransparency.Value, slEclosion.Value, brPen, elPen);
            }
            else
            {
                elPen.Visibility = Visibility.Collapsed;
                setPenView((int)slPenSize.Value, slTransparency.Value, slEclosion.Value, brPen, curBrush.selButton);
            }
        }





        /// <summary>
        /// 显示指定画刷样式的画刷详细设置
        /// </summary>
        /// <param name="bt"></param>
        private void showPenDetailSetting(Button bt)
        {
            if (grPenDetail.Visibility == Visibility.Collapsed)
            {
                grPenDetail.Visibility = Visibility.Visible;               
            }

            if (bt == btDefaultPen)
            {
                grHardness.Visibility = Visibility.Visible;
            }
            else
            {
                grHardness.Visibility = Visibility.Collapsed; ;
            }

            if (curBrush.BrushPenType == PenType.Transparent)
            {
                grTransparency.Visibility = Visibility.Visible;
                tOpacity.Text = WPPhotoEditor.Resources.AppResources.Transparency;
            }
            else if (curBrush.BrushPenType == PenType.Opacity)
            {
                grTransparency.Visibility = Visibility.Visible;
                tOpacity.Text = WPPhotoEditor.Resources.AppResources.tOpacity;
            }
            else
            {
                grTransparency.Visibility = Visibility.Collapsed;

                if (bt == btDefaultPen)
                {
                    grHardness.Visibility = Visibility.Collapsed;
                }
            }

        }

        /// <summary>
        /// 关闭画刷详细设置并生成目标画刷图像
        /// </summary>
        private void closePenDetailSetting()
        {
            if (grPenDetail.Visibility == Visibility.Visible)
            {
                grPenDetail.Visibility = Visibility.Collapsed;

                updateCurBrush();
            }

        }
        #endregion



        #region 裁剪
        private void Touch_FrameClip(TouchFrameEventArgs e)
        {
            TouchPointCollection tPoints = e.GetTouchPoints(this);
            if (tPoints.Count == 2)
            {
                #region 裁剪相关变量
                Rect rec = new Rect();
                Point[] tPts = proofreadTPts(tPoints, this);              //校对点击的点是是否有屏幕中
                Point pt = locatRec(tPts[0], tPts[1]);              //获取截剪的矩形
                rec = new Rect(pt.X, pt.Y, Math.Abs(tPts[0].X - tPts[1].X), Math.Abs(tPts[0].Y - tPts[1].Y));

                if (cropProportion == 0)
                {
                    setCropMask(rec);
                }
                else
                {
                    setCropMask(rec, cropProportion);
                }
                #endregion
            }
            else if (tPoints.Count == 1 && grCrop.Visibility == Visibility.Visible)
            {

                if (tPoints[0].Action == TouchAction.Down)
                {
                    setHadClickEdEllip(tPoints[0].Position);

                    //取消裁剪
                    if (grCrop.Visibility == Visibility.Visible)
                    {
                        Point pt = e.GetPrimaryTouchPoint(this).Position;
                        if (pt.X < 0 || pt.Y < 0 || pt.X > this.Width || pt.Y > this.Height)
                        {
                            grCrop.Visibility = Visibility.Collapsed;
                        }
                    }

                }
                else if (tPoints[0].Action == TouchAction.Move)
                {
                    if (cropProportion == 0)
                    {
                        setCropMask(tPoints[0].Position);
                    }
                    else
                    {
                        setCropMask(tPoints[0].Position, cropProportion);
                    }
                }
                lastPt = tPoints[0].Position;
            }
        }


        void grCrop_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (getCurLayer() == brDrawingBoard)
            {
                return;
            }
            RectangleGeometry recGem = new RectangleGeometry();
            recGem.Rect = new Rect(rcCrop.Margin.Left - Canvas.GetLeft(getCurLayer()), rcCrop.Margin.Top - Canvas.GetTop(getCurLayer()), rcCrop.Width, rcCrop.Height);
            getCurLayer().Clip = recGem;
            this.grCrop.Visibility = Visibility.Collapsed;
        }


        #region 裁切

        #region -----根据指定矩形生成截切UI层
        /// <summary>
        /// 根据给出的矩形，生成剪切蒙版
        /// </summary>
        /// <param name="lRectSize"></param>
        private void setCropMask(Rect lRectSize)
        {
            if (lRectSize.X < 0 || lRectSize.Y < 0 || lRectSize.X + lRectSize.Width > this.Width || lRectSize.Y + lRectSize.Height > this.Height || lRectSize.Width < 20 || lRectSize.Height < 20)
            {
                return;
            }
            setGeometryGroup(phCrop, grCrop, lRectSize);
            setCropRect(lRectSize);
        }

        //设置剪切矩形
        private void setCropRect(Rect lRectSize)
        {
            grCrop.Visibility = Visibility.Visible;
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

            //tSize.Visibility = Visibility.Visible;
            //tSize.Margin = new Thickness(lRectSize.X + 10, lRectSize.Y - 30, 0, 0);
            //  BitmapSource curBp = (iEditPhoto.Background as ImageBrush).ImageSource as BitmapSource;

            //  tSize.Text = (slSizeWidth.Value * (lRectSize.Width / this.Width)).ToString("00") + "*" + (slSizeHeight.Value * (lRectSize.Height / this.Height)).ToString("00");
        }

        //设置蒙版层
        private void setGeometryGroup(System.Windows.Shapes.Path pathCrop, Grid grCrop, Rect lRectSize)
        {


            if (pathCrop == null)
            {
                pathCrop = new System.Windows.Shapes.Path();
                phCrop = pathCrop;
                grCrop.Children.Insert(0, pathCrop);
            }


            GeometryGroup gmGroup = new GeometryGroup();
            gmGroup.FillRule = FillRule.EvenOdd;
            RectangleGeometry bRect = new RectangleGeometry();       //大的矩形
            RectangleGeometry lRect = new RectangleGeometry();       //小的矩形



            bRect.Rect = new Rect(0, 0, this.Width, this.Height);
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
        #endregion

        #region -----根据指定点拉挤截切UI
        private System.Windows.Shapes.Path phCrop;

        private Shape hadClickEdEllip;

        //private Point lastPt;   //用于移动时记录上一个点

        private double cropProportion = 0.0;
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
                    double newWidth = rcCrop.Width - pos.X > 20 ? rcCrop.Width - pos.X : 20;
                    double newHeight = rcCrop.Height - pos.Y > 20 ? rcCrop.Height - pos.Y : 20;
                    if (pt.X < ruEllipse.Margin.Left - 10 && pt.Y < rdEllipse.Margin.Top - 10)
                    {
                        rec = new Rect(pt.X, pt.Y, newWidth, newHeight);
                    }
                }
                else if (hadClickEdEllip == ldEllipse)
                {
                    //左下角
                    double newWidth = rcCrop.Width - pos.X > 20 ? rcCrop.Width - pos.X : 20;
                    double newHeight = rcCrop.Height + pos.Y > 20 ? rcCrop.Height + pos.Y : 20;

                    if (pt.X < ruEllipse.Margin.Left - 10)
                    {
                        rec = new Rect(pt.X, rcCrop.Margin.Top, newWidth, newHeight);
                    }
                }
                else if (hadClickEdEllip == ruEllipse)
                {
                    //右上角
                    double newWidth = rcCrop.Width + pos.X > 20 ? rcCrop.Width + pos.X : 20;
                    double newHeight = rcCrop.Height - pos.Y > 20 ? rcCrop.Height - pos.Y : 20;
                    if (pt.Y < rdEllipse.Margin.Top - 10)
                    {
                        rec = new Rect(rcCrop.Margin.Left, pt.Y, newWidth, newHeight);
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
        /// 设置hadClickEdEllip 字段：为setCropMask指供操作对象
        /// </summary>
        /// <param name="pt"></param>
        private void setHadClickEdEllip(Point pt)
        {
            if (hadClickEllipse(pt, luEllipse))
            {
                hadClickEdEllip = luEllipse;
            }
            else if (hadClickEllipse(pt, ldEllipse))
            {
                hadClickEdEllip = ldEllipse;
            }
            else if (hadClickEllipse(pt, ruEllipse))
            {
                hadClickEdEllip = ruEllipse;
            }
            else if (hadClickEllipse(pt, rdEllipse))
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
        /// <returns></returns>
        private bool hadClickEllipse(Point pt, Ellipse el)
        {
            if (pt.X > el.Margin.Left - 50 && pt.X < el.Margin.Left + 50 && pt.Y < el.Margin.Top + 50 && pt.Y > el.Margin.Top - 50)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取截剪的矩形
        /// </summary>
        /// <param name="pt01"></param>
        /// <param name="pt02"></param>
        /// <returns></returns>
        private Point locatRec2(Point pt01, Point pt02)
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
        private Point[] proofreadTPts(TouchPointCollection tPoints, FrameworkElement targetUI)
        {
            Point[] pts = new Point[2];
            pts[0] = tPoints[0].Position;
            pts[1] = tPoints[1].Position;

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
        #endregion

        #region ------根据指定点或指定最大矩形，指定比例生成截切UI
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

        private void initializeCropMask(Rect rec, double proportion)
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

        #endregion


        #endregion

        #endregion



        #region 图层旋转与撤销重做
        void tAngle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            double value = (Math.Floor(slAngle.Value / 90) + 1) * 90;
            if (value >= 450)
            {
                value = 0;
            }
            slAngle.Value = value;
        }

        void slAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (curTool == ToolStyle.Rotate)
            {
                if (selLayerIndex > -1 && selLayerIndex < lsLayers.Items.Count)
                {
                    Border br = this.canLayers.Children[selLayerIndex] as Border;
                    RotateTransform roTrans = new RotateTransform();
                    if (br.RenderTransform is TransformGroup)
                    {
                        transFormGp = br.RenderTransform as TransformGroup;
                        foreach (Transform trans in transFormGp.Children)
                        {
                            if (trans is RotateTransform)
                            {
                                roTrans = trans as RotateTransform;
                                transFormGp.Children.Remove(trans);
                                break;
                            }
                        }
                    }
                    else
                    {
                        transFormGp = new TransformGroup();
                    }
                    roTrans.Angle = slAngle.Value;
                    roTrans.CenterX = br.Width / 2;
                    roTrans.CenterY = br.Height / 2;
                    transFormGp.Children.Add(roTrans);
                    br.RenderTransform = transFormGp;

                    if (br.Clip != null)
                    {
                        RotateTransform croTrans = new RotateTransform();
                        croTrans.CenterX = (br.Clip as RectangleGeometry).Rect.Width / 2;
                        croTrans.CenterY = (br.Clip as RectangleGeometry).Rect.Height / 2;
                        br.Clip.Transform = croTrans;
                    }
                }
            }
        }

        //创建绘图撤消
        private LayerState createDrawingUndoRecord()
        {

            if (curTool == ToolStyle.Brush)
            {


                if (selLayerIndex >= 0 && selLayerIndex < this.canLayers.Children.Count)
                {
                    LayerState lastLayerState = new LayerState();
                    lastLayerState.isDrawing = true;
                    lastLayerState.LayerIndex = selLayerIndex;
                    rbtUndo.IsEnabled = true;

                    Border layer = this.canLayers.Children[selLayerIndex] as Border;
                    if ((layer.Background is ImageBrush))
                    {
                        WriteableBitmap wbmp = new WriteableBitmap((layer.Background as ImageBrush).ImageSource as BitmapSource);
                        lastLayerState.LastLayerWbp = wbmp;
                    }
                    else
                    {
                        WriteableBitmap wbmp = new WriteableBitmap(layer, null);
                        lastLayerState.LastLayerWbp = wbmp;
                    }
                    return lastLayerState;
                }
            }

            return null;
        }

        /// <summary>
        /// 创建透明、非透明撤消
        /// </summary>
        /// <returns></returns>
        private LayerState createUndoRecord()
        {
            if (curTool == ToolStyle.Brush)
            {
                if (selLayerIndex >= 0 && selLayerIndex < this.canLayers.Children.Count)
                {
                    Border layer = this.canLayers.Children[selLayerIndex] as Border;
                    if ((layer.OpacityMask is ImageBrush))
                    {
                        WriteableBitmap wbmp = new WriteableBitmap((layer.OpacityMask as ImageBrush).ImageSource as BitmapSource);
                        return setUndoRecord(wbmp);
                    }
                    else
                    {
                        WriteableBitmap wbmp = new WriteableBitmap(layer, null);
                        return setUndoRecord(wbmp);
                    }
                }
            }

            return null;
        }

        LayerState setUndoRecord(WriteableBitmap wBmp)
        {
            LayerState lastLayerState = new LayerState();
            lastLayerState.LastLayerWbp = new WriteableBitmap(wBmp);
            lastLayerState.LayerIndex = selLayerIndex;
            lastLayerState.isDrawing = false;
            rbtUndo.IsEnabled = true;
            return lastLayerState;
        }


        void setLayerStateRecord(LayerState lState)
        {
            if (lState != null && (lState.LayerIndex >= 0 && lState.LayerIndex < this.canLayers.Children.Count))
            {

                Border layer = this.canLayers.Children[lState.LayerIndex] as Border;
                ImageBrush ibrush = new ImageBrush();
                ibrush.ImageSource = lState.LastLayerWbp;

                if (lState.isDrawing)
                {
                    layer.Background = ibrush;
                }
                else
                {
                    layer.OpacityMask = ibrush;
                }

            }
        }
        #endregion


        #region 绘画
        Button btDrawingBoardControl;
        WriteableBitmap drawingWbmp;
    
        private void initializeDrawing()
        {
            // drawingWbmpOpacity = WPPhotoEditor.Utility.UtilityFunction.GetImgResource("WbmpOpacity.png");
            //  this.canLayers.Children.Remove(this.brDrawingBoard);
            // return;
            //drawingWbmp = new WriteableBitmap(this.brDrawingBoard, null);
            //   drawingWbmpOpacity = new WriteableBitmap(this.grCanContainer, null);
            //   
            curBrush.BrushPenType = PenType.Color;

            btDrawingBoardControl = new Button();
            addDrawingControl(btDrawingBoardControl);
        }

        private void Touch_FrameDrawing(TouchFrameEventArgs e)
        {
            if (curTool == ToolStyle.Brush && curBrush.BrushPenType == PenType.Color || curBrush.BrushPenType == PenType.Original)
            {
                if (brDrawingBoard != null)
                {
                    TouchPoint tPoint = e.GetPrimaryTouchPoint(brDrawingBoard);
                    if (tPoint.Action == TouchAction.Down)
                    {
                        canLayers_MouseLeftButtonDown(tPoint.Position);
                    }
                    else if (tPoint.Action == TouchAction.Move)
                    {
                        canLayers_MouseMove(tPoint.Position);
                    }
                    else
                    {
                        canLayers_MouseLeftButtonUp(tPoint.Position);
                        closeTool();
                        closePenDetailSetting();
                    }
                }
            }
        }


        void canLayers_MouseLeftButtonUp(Point pt)
        {
            if (!isEnableEdit)
            {
                return;
            }

            if (curTool == ToolStyle.Brush && curBrush.BrushPenType == PenType.Color || curBrush.BrushPenType == PenType.Original)
            {
                if (this.brDrawingBoard.Background is ImageBrush)
                {
                    drawingWbmp = (this.brDrawingBoard.Background as ImageBrush).ImageSource as WriteableBitmap;
                }
                else
                {

                    drawingWbmp = new WriteableBitmap(this.brDrawingBoard, null);
                }

                //  pt = new Point(pt.X * 3.0, pt.Y * 3.0);
                drawBrushFhColor(drawingLastPt, pt, curBrush.TargetBrush, this.brDrawingBoard, drawingWbmp);
                drawingLastPt = pt;
            }
        }

        void canLayers_MouseLeftButtonDown(Point pt)
        {
            if (!isEnableEdit)
            {
                return;
            }

            // pt = new Point(pt.X * 3.0, pt.Y * 3.0);
            drawingLastPt = pt;


            if (curTool == ToolStyle.Brush)
            {
                unReManager.AddUndoCollection(createDrawingUndoRecord());
                rbtUndo.IsEnabled = true;
            }

        }

        void canLayers_MouseMove(Point pt)
        {

            if (!isEnableEdit)
            {
                return;
            }
            if (curTool == ToolStyle.Brush && curBrush.BrushPenType == PenType.Color || curBrush.BrushPenType == PenType.Original)
            {
                if (this.brDrawingBoard.Background is ImageBrush)
                {
                    drawingWbmp = (this.brDrawingBoard.Background as ImageBrush).ImageSource as WriteableBitmap;
                }
                else
                {

                    drawingWbmp = new WriteableBitmap(this.brDrawingBoard, null);
                }

                // pt = new Point(pt.X * 3.0, pt.Y * 3.0);
                drawBrushFhColor(drawingLastPt, pt, curBrush.TargetBrush, this.brDrawingBoard, drawingWbmp);
                drawingLastPt = pt;
            }


        }


        private Point drawingLastPt = new Point();

        private void addDrawingControl(Button newLayerControl)
        {
            newLayerControl.Width = 129;
            newLayerControl.Height = 70;
            newLayerControl.Margin = new Thickness(-20, 0, 0, 0);
            newLayerControl.Click += new RoutedEventHandler(newLayerControl_Click);                //标识当前点击（选择）的图层
            newLayerControl.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(newLayerControl_Hold);           //显示/隐藏图层
            newLayerControl.DoubleTap += new EventHandler<System.Windows.Input.GestureEventArgs>(newLayerControl_DoubleTap);
            newLayerControl.MouseLeftButtonDown += new MouseButtonEventHandler(grToolDetail_MouseLeftButtonDown);
            newLayerControl.Tag = lsLayers.Items.Count;


            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = new WriteableBitmap(this.canLayers, null);
            imgBrush.Stretch = Stretch.Fill;
            newLayerControl.Background = imgBrush;


            lsLayers.Items.Add(newLayerControl);

            newLayerControl_Click(newLayerControl, null);
        }







        #region 绘图函数
        // private WriteableBitmap brush = WPPhotoEditor.Utility.UtilityFunction.GetImgResource("brGrass.png");
        private void drawBrushFhColor(Point FromPt, Point ToPt, WriteableBitmap wPNGbp, Border curBoard, WriteableBitmap wBmp)
        {
            int x1 = (int)(FromPt.X - wPNGbp.PixelWidth / 2);
            int y1 = (int)(FromPt.Y - wPNGbp.PixelHeight / 2);
            int x2 = (int)(ToPt.X - wPNGbp.PixelWidth / 2);
            int y2 = (int)(ToPt.Y - wPNGbp.PixelHeight / 2);
            drawEachPointForLine(x1, y1, x2, y2, wPNGbp, wBmp);

            ImageBrush ibrush = new ImageBrush();
            ibrush.Stretch = Stretch.None;
            ibrush.ImageSource = wBmp;
            curBoard.Background = ibrush;
        }


        /// <summary>
        /// 计算要描述的像素
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="brush"></param>
        /// <param name="pointAction"></param>
        public void drawEachPointForLine(int x1, int y1, int x2, int y2, WriteableBitmap wPNGbp, WriteableBitmap wBmp)
        {
            int num5;
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;// 最大差

            int num = x2 - x1;
            int num2 = y2 - y1;



            int num3 = 0;    //正负1：指示X方向
            if (num < 0)
            {  //左－右
                num = -num;
                num3 = -1;
            }
            else if (num > 0)
            {
                //右－左
                num3 = 1;
            }

            int num4 = 0;    ////正负1：指示Y向
            if (num2 < 0)
            {
                //上－下
                num2 = -num2;
                num4 = -1;
            }
            else if (num2 > 0)
            {
                //下-上
                num4 = 1;
            }


            if (num > num2)    //x差>y差
            {
                //大致横向
                num5 = num3;     //X第次递进
                num6 = 0;        //Y第次递进

                num7 = num3;              //X第次递进
                num8 = num4;
                num9 = num2;
                num10 = num;
            }
            else
            {
                //大致竖向
                num5 = 0;                //X：恒定
                num6 = num4;             //Y每次递进

                num7 = num3;
                num8 = num4;         //Y每次递进
                num9 = num;
                num10 = num2;
            }

            #region 第一个点绘
            int x = x1;
            int y = y1;
            int num13 = num10 >> 1;
            DrawTheWbmp(x, y, wPNGbp, wBmp.Pixels, wBmp.PixelWidth, wBmp.PixelHeight);
            // setPicturePngTsp(new Point(x, y), wPNGbp, curLayer);
            #endregion


            //num10:Max(X差；Y差)
            for (double i = 0; i < num10; i++)
            {
                num13 -= num9;
                if (num13 < 0)
                {
                    num13 += num10;
                    x += num7;
                    y += num8;
                }
                else
                {
                    x += num5;
                    y += num6;
                }


                i += slSpacing.Value;
                DrawTheWbmp(x, y, wPNGbp, wBmp.Pixels, wBmp.PixelWidth, wBmp.PixelHeight);


            }


        }

        /// <summary>
        /// 实际描绘像素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wPNGbp">画刷图像</param>
        /// <param name="destPixels">目标蒙版</param>
        /// <param name="destWidth"></param>
        /// <param name="destHeight"></param>
        public void DrawTheWbmp(int x, int y, WriteableBitmap wPNGbp, int[] destPixels, int destWidth, int destHeight)
        {
            int width = wPNGbp.PixelWidth;     //画刷宽度
            int height = wPNGbp.PixelHeight;   //画刷高度
            int[] data = wPNGbp.Pixels;
            int xRange = x + width;   //指定X绘制范围
            int yRange = y + height;   //指定Y绘制范围

            //如果待画坐标于图像内则执行
            if ((x < destWidth) && (y < destHeight))
            {
                int xReal;
                int yReal;
                int num7;
                int num8;

                #region 校正X在图像内：Num5；         Num7用于描绘画刷非中心边缘（当中心点不在待绘图像中，但其边缘在时）
                if (x < 0)
                {
                    xReal = 0;
                    num7 = -x;
                }
                else
                {
                    xReal = x;
                    num7 = 0;
                }
                #endregion

                #region 校正Y在图像内：Num6;          Num8用于描绘画刷非中心边缘（当中心点不在待绘图像中，但其边缘在时）
                if (y < 0)
                {
                    yReal = 0;
                    num8 = -y;
                }
                else
                {
                    yReal = y;
                    num8 = 0;
                }
                #endregion

                #region 校正于图像内
                if (xRange > destWidth)
                {
                    xRange = destWidth;
                }

                if (yRange > destHeight)
                {
                    yRange = destHeight;
                }
                #endregion

                int num11 = xRange - xReal;    //==x

                int num12 = width - num11;
                int num13 = destWidth - num11;

                int num14 = yRange - yReal;    //==y

                int num9 = num7 + num11;   //真实的X待绘点
                int num10 = num8 + num14;   //真实的Y待绘点


                if ((num9 >= 0) && (num10 >= 0))
                {
                    //int num16 = 0;
                    int index = num7 + (num8 * width);            //画刷像素索引
                    int num18 = xReal + (yReal * destWidth);        //待绘像素索引
                    for (int i = yReal; i < yRange; i++)
                    {
                        for (int j = xReal; j < xRange; j++)
                        {
                            if ((byte)(data[index] >> 24) > 0)
                            {
                                destPixels[num18] = data[index];
                            }

                            index++;
                            num18++;
                        }
                        index += num12;    //画刷图像下一行
                        num18 += num13;    //待画图像下一行像素
                    }
                }


            }
        }
        #endregion
        #endregion


        private void matchTheFirstPic()
        {
            if (App.PESettings.MatchTheFirstLayerPic && (selLayerIndex == 0 || lsLayers.Items.Count == 2))
            {
                Border brLayer = brDrawingBoard;
                for (int i = 0; i < this.canLayers.Children.Count; i++)
                {
                    if (this.canLayers.Children[i] != brDrawingBoard)
                    {
                        brLayer = this.canLayers.Children[i] as Border;
                        break;
                    }
                }

                if (brLayer != brDrawingBoard)
                {
                    BitmapSource bmps = (brLayer.Background as ImageBrush).ImageSource as BitmapSource;
                    Size newSize = WPPhotoEditor.Utility.UtilityFunction.GetTheMaxShowSize(new Size(480, 640), bmps);



                    // if (Math.Max(bmps.PixelWidth, bmps.PixelHeight) > 1200)
                    {
                        // double scale = 1200 / Math.Max(bmps.PixelWidth, bmps.PixelHeight);
                        //  newSize = new Size(bmps.PixelWidth * scale, bmps.PixelHeight * scale);
                    }
                    App.PESettings.CanvasSize = new Size(bmps.PixelWidth, bmps.PixelHeight);

                    resizeTheCanvas(newSize);

                    (this.canLayers.Children[0] as Border).Width = newSize.Width;
                    (this.canLayers.Children[0] as Border).Height = newSize.Height;
                    // (this.canLayers.Children[0] as CavLayer).Width = bmps.PixelWidth;
                    // (this.canLayers.Children[0] as CavLayer).Height = bmps.PixelHeight;

                    Border br = canLayers.Children[0] as Border;
                    addPicture((br.Background as ImageBrush).ImageSource as BitmapImage, ref br, this.canLayers);

                    setLayerControlState(this.lsLayers.Items[0] as Button);
                }
            }
        }

        private void resizeTheCanvas(Size newSize)
        {

            this.grCanContainer.Width = newSize.Width;
            this.grCanContainer.Height = newSize.Height;

            this.canLayers.Width = newSize.Width;
            this.canLayers.Height = newSize.Height;

            this.brDrawingBoard.Width = newSize.Width;
            this.brDrawingBoard.Height = newSize.Height;
        }


        private void CaptureScreen(object sender, EventArgs e)
        {
            ScaleTransform scaleTs = new ScaleTransform();
            scaleTs.ScaleY = 1.0;
            scaleTs.ScaleX = 1.0;
            WriteableBitmap bmp = new WriteableBitmap(App.Current.RootVisual, scaleTs);
            //  WriteableBitmap bmp = new WriteableBitmap(480, 800);
            // bmp.Render(App.Current.RootVisual, null);
            // bmp.Invalidate();

            MemoryStream stream = new MemoryStream();
            bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
            stream.Seek(0, SeekOrigin.Begin);

            MediaLibrary library = new MediaLibrary();
            string fileName = "ScreenShot_" + DateTime.Now.ToString("yyyy-mm-dd_hh:mm:ss");
            library.SavePicture(fileName, stream);
            stream.Close();
        }

        private void FillTheScreen(BitmapImage bmp, InkPresenter iPresenter, Border brChild)
        {
            #region 使图片能不失真地最大化显示在画布上
            if (bmp.PixelWidth > bmp.PixelHeight)
            {
                brChild.Width = iPresenter.Height;      //注意.width=.Height
                brChild.Height = iPresenter.Width;

                RotateTransform rTransFrom = new RotateTransform();
                rTransFrom.Angle = 90;
                rTransFrom.CenterX = iPresenter.Width / 2;
                rTransFrom.CenterY = iPresenter.Width / 2;
                brChild.RenderTransform = rTransFrom;
            }
            else
            {
                brChild.Width = iPresenter.Width;      //注意.width=.Height
                brChild.Height = iPresenter.Height;
            }
            #endregion

            brChild.Visibility = Visibility.Visible;
        }

        private void showMemory()
        {
            long deviceTotalMemory = (long)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceTotalMemory") / 1024 / 1024;
            long applicationCurrentMemoryUsage = (long)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage") / 1024 / 1024;
            long applicationPeakMemoryUsage = (long)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage") / 1024 / 1024;
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString());
            System.Diagnostics.Debug.WriteLine("Device Total : " + deviceTotalMemory.ToString());
            System.Diagnostics.Debug.WriteLine("App Current : " + applicationCurrentMemoryUsage.ToString());
            System.Diagnostics.Debug.WriteLine("App Peak : " + applicationPeakMemoryUsage.ToString());
        }
    }

    #region ------撤销

    /// <summary>
    /// 维护撤消/重做的历史资源
    /// </summary>
    public class UnReDoManage
    {
        public List<LayerState> undoLayerStateCollection = new List<LayerState>();
        public List<LayerState> redoLayerStateCollection = new List<LayerState>();

        public void AddUndoCollection(LayerState addLState)
        {
            if (undoLayerStateCollection.Count > 5)
            {
                undoLayerStateCollection.RemoveAt(0);
            }
            undoLayerStateCollection.Add(addLState);
        }

        public void AddRedoCollection(LayerState addLState)
        {
            if (redoLayerStateCollection.Count > 5)
            {
                redoLayerStateCollection.RemoveAt(0);
            }
            redoLayerStateCollection.Add(addLState);
        }


        public LayerState GetUndoCollection()
        {
            LayerState getLStae = null;
            if (undoLayerStateCollection.Count > 0)
            {
                getLStae = undoLayerStateCollection[undoLayerStateCollection.Count - 1];
                undoLayerStateCollection.RemoveAt(undoLayerStateCollection.Count - 1);
                // AddRedoCollection(getLStae);
            }

            return getLStae;
        }

        public LayerState GetRedoCollection()
        {
            LayerState getLStae = null;
            if (redoLayerStateCollection.Count > 0)
            {
                getLStae = redoLayerStateCollection[redoLayerStateCollection.Count - 1];
                redoLayerStateCollection.RemoveAt(redoLayerStateCollection.Count - 1);
                // AddUndoCollection(getLStae);
            }

            return getLStae;
        }

        public void DelUnDoOnLayerIndex(int selIndex)
        {
            List<LayerState> delUnRedo = new List<LayerState>();

            #region 更新撤消
            foreach (LayerState lState in undoLayerStateCollection)
            {
                if (lState.LayerIndex == selIndex)
                {
                    delUnRedo.Add(lState);
                }
            }

            foreach (LayerState lState in delUnRedo)
            {
                undoLayerStateCollection.Remove(lState);
            }
            #endregion

            delUnRedo = new List<LayerState>();


            #region 更新重做
            foreach (LayerState lState in redoLayerStateCollection)
            {
                if (lState.LayerIndex == selIndex)
                {
                    delUnRedo.Add(lState);
                }
            }

            foreach (LayerState lState in delUnRedo)
            {
                redoLayerStateCollection.Remove(lState);
            }
            #endregion
        }
    }

    public class LayerState
    {

        public WriteableBitmap LastLayerWbp
        {
            get;
            set;
        }

        public int LayerIndex
        {
            get;
            set;
        }

        public bool isDrawing
        {
            get;
            set;
        }

        public bool isUndo
        {
            get;
            set;
        }

    }

    #endregion
}
