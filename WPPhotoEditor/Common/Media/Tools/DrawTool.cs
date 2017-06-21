using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WPPhotoEditor.Common;
using WPPhotoEditor.UControls;
using WPPhotoEditor.ViewModels;
using Yuweiz.Phone.Media.Imaging;


namespace WPPhotoEditor.Media.Tools
{
    public class DrawTool : ToolAbstract
    {
        //private DrawTool(LayerBase LayerBase)
        //{
        //    base.LayerBase = LayerBase;

        //    this.AllowRotate = true;
        //    this.AllowTwoFingerDrage = true;
        //    this.BindLayer(LayerBase);         
        //}

        public DrawTool()
        {
            this.AllowRotate = true;
            this.AllowTwoFingerDrage = true;
            this.BindLayer(this.LayerBase);
            AppSession.Instance.HistoryManager.SaveCurHistoryItemAction = this.RecordCurHistoryState;
        }

        public static readonly DrawTool Instance = new DrawTool();

        private Point _lastPt;

        /// <summary>
        /// 画笔
        /// 由BrushFactory生成
        /// </summary>
        private WriteableBitmap BrushWbmp
        {
            get { return BrushFactory.Instance.BrushWbmp; }
        }

        private Brush backgroundBrush;

        private Brush opacityMaskBrush;


        /// <summary>
        /// 旋转角度记录
        /// </summary>
        private double initialAngle;

        private CompositeTransform transform;

        public bool AllowRotate { get; set; }
        //-----------


        /// <summary>
        /// 双指拖动
        /// 两指距离保持一定距离+-50时，认为拖动
        /// </summary>
        private bool isDrag2 = false;

        private Point lastCenterPoint;

        public bool AllowTwoFingerDrage { get; set; }
        //----------------
            
        public override void BindLayer(LayerBase layer)
        {
            this.LayerBase = layer;
            if (this.LayerBase == null)
            {
                return;
            }

            this.transform = this.LayerBase.RenderTransform as CompositeTransform;

            this.LayerBase.Tap += LayoutRoot_Tap;
            base.gestureListener2 = new Yuweiz.Phone.Gestures.GestureListener2(this.LayerBase);
            base.gestureListener2.DragStarted += gestureListener2_DragStarted;
            base.gestureListener2.DragCompleted += gestureListener2_DragCompleted;
            base.gestureListener2.DragDelta += gestureListener2_DragDelta;
            base.gestureListener2.PinchStarted += gestureListener2_PinchStarted;
            base.gestureListener2.PinchDelta += gestureListener2_PinchDelta;
            base.gestureListener2.PinchCompleted += gestureListener2_PinchCompleted;
        }

        public override void ReleaseLayer()
        {
            if (this.LayerBase == null)
            {
                return;
            }

            this.LayerBase.Tap -= LayoutRoot_Tap;
            base.gestureListener2.DragStarted -= gestureListener2_DragStarted;
            base.gestureListener2.DragCompleted -= gestureListener2_DragCompleted;
            base.gestureListener2.DragDelta -= gestureListener2_DragDelta;
            base.gestureListener2.PinchStarted -= gestureListener2_PinchStarted;
            base.gestureListener2.PinchDelta -= gestureListener2_PinchDelta;
            base.gestureListener2.PinchCompleted -= gestureListener2_PinchCompleted;


            this.transform = new CompositeTransform();
            this.LayerBase = null;
        }


        /// <summary>
        /// 调用此方法实现将当前的状态保存为重做项
        /// </summary>
        void RecordCurHistoryState()
        {
            //记录历史
            if (BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Color || BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Original)
            {
                AppSession.Instance.HistoryManager.AddHistory(new PEHistoryItem() { OperationType = PEOperationType.Draw | PEOperationType.Undo, Wbmp = new WriteableBitmap(this.GetEditWbmp()), LayerBase = this.LayerBase });
            }
            else
            {
                AppSession.Instance.HistoryManager.AddHistory(new PEHistoryItem() { OperationType = PEOperationType.Mask | PEOperationType.Undo, Wbmp = new WriteableBitmap(this.GetEditWbmp()), LayerBase = this.LayerBase });
            }
        }

        void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //记录历史
            if (BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Color || BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Original)
            {
                AppSession.Instance.HistoryManager.AddHistory(new PEHistoryItem() { OperationType = PEOperationType.Draw, Wbmp = new WriteableBitmap(this.GetEditWbmp()), LayerBase = this.LayerBase });
            }
            else
            {
                AppSession.Instance.HistoryManager.AddHistory(new PEHistoryItem() { OperationType = PEOperationType.Mask, Wbmp = new WriteableBitmap(this.GetEditWbmp()), LayerBase = this.LayerBase });
            }

            Point pt = e.GetPosition(sender as UIElement);
            this.DrawPoint(pt);
        }

        void gestureListener2_DragCompleted(Point pt)
        {
            this.DrawLine(pt);


        }

        void gestureListener2_DragStarted(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.gestureListener2.IsGestureOnTarget)
            {
                //记录历史
                if (BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Color || BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Original)
                {
                    AppSession.Instance.HistoryManager.AddHistory(new PEHistoryItem() { OperationType = PEOperationType.Draw, Wbmp = new WriteableBitmap(this.GetEditWbmp()), LayerBase = this.LayerBase });
                }
                else
                {
                    AppSession.Instance.HistoryManager.AddHistory(new PEHistoryItem() { OperationType = PEOperationType.Mask, Wbmp = new WriteableBitmap(this.GetEditWbmp()), LayerBase = this.LayerBase });
                }

                Point pt = new Point(e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
                this.DrawPoint(pt);
            }
        }

        void gestureListener2_DragDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.gestureListener2.IsGestureOnTarget)
            {
                Point pt = new Point(e.ManipulationOrigin.X, e.ManipulationOrigin.Y);

                this.DrawLine(pt);

            }
        }

        void gestureListener2_PinchStarted(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.gestureListener2.IsGestureOnTarget)
            {
                if (this.AllowRotate)
                {
                    initialAngle = transform.Rotation;
                }

                this.backgroundBrush = this.LayerBase.Background;
                this.opacityMaskBrush = this.LayerBase.OpacityMask;

                this.LayerBase.Background = new ImageBrush() { ImageSource = new WriteableBitmap(this.LayerBase, new ScaleTransform() { ScaleX = 2, ScaleY = 2 }), Stretch = Stretch.Fill };
                this.LayerBase.OpacityMask = null;
            }
        }

        void gestureListener2_PinchDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.gestureListener2.IsGestureOnTarget)
            {
                if (this.AllowRotate)
                {
                    double angleDelta = this.gestureListener2.GetAngle(e.PinchManipulation.Current) - this.gestureListener2.GetAngle(e.PinchManipulation.Original);
                    transform.Rotation = initialAngle + angleDelta;
                }

                if (this.AllowTwoFingerDrage)
                {
                    #region 两指拖动 及缩放
                    //两指距离保持一定距离+-50时，认为拖动
                    double curDistance = Math.Sqrt(Math.Pow(e.PinchManipulation.Current.PrimaryContact.X - e.PinchManipulation.Current.SecondaryContact.X, 2) + Math.Pow(e.PinchManipulation.Current.PrimaryContact.Y - e.PinchManipulation.Current.SecondaryContact.Y, 2));
                    double lastDistance = Math.Sqrt(Math.Pow(e.PinchManipulation.Original.PrimaryContact.X - e.PinchManipulation.Original.SecondaryContact.X, 2) + Math.Pow(e.PinchManipulation.Original.PrimaryContact.Y - e.PinchManipulation.Original.SecondaryContact.Y, 2));
                    if (Math.Abs(curDistance - lastDistance) < 50 / transform.ScaleX)
                    {
                        if (!isDrag2)
                        {
                            //开始拖动
                            isDrag2 = true;
                            lastCenterPoint = e.PinchManipulation.Current.Center;
                        }
                        else
                        {
                            //拖动中
                            isDrag2 = true;
                            Point pt = new Point(e.PinchManipulation.Current.Center.X - this.lastCenterPoint.X, e.PinchManipulation.Current.Center.Y - this.lastCenterPoint.Y);

                            Point transformedTranslation = this.gestureListener2.GetTransformNoTranslation(transform).Transform(pt);
                            transform.TranslateX += transformedTranslation.X;
                            transform.TranslateY += transformedTranslation.Y;
                        }
                    }
                    else
                    {
                        isDrag2 = false;

                        // DistanceRatio from PinchGestureEventArgs is now replaced by
                        // PinchManipulation.DeltaScale and PinchManipulation.CumulativeScale,
                        // which expose the scale from the pinch directly.
                        // Note that we don't have to apply a transform because the distance ratio is the
                        // same in any reference frame.
                        transform.ScaleX *= e.PinchManipulation.DeltaScale;
                        transform.ScaleY *= e.PinchManipulation.DeltaScale;
                    }
                    #endregion
                }
                else
                {
                    //仅缩放
                    transform.ScaleX *= e.PinchManipulation.DeltaScale;
                    transform.ScaleY *= e.PinchManipulation.DeltaScale;
                }
            }
        }

        void gestureListener2_PinchCompleted()
        {
            isDrag2 = false;

            if (this.LayerBase == null)
            {
                return;
            }

            this.LayerBase.Background = this.backgroundBrush;
            this.LayerBase.OpacityMask = this.opacityMaskBrush;

            Storyboard storyBoard = Yuweiz.Phone.Media.StoryBoards.CreateScaleLimitStoryBoard(this.LayerBase, 0.3);
            transform = this.LayerBase.RenderTransform as CompositeTransform;
            storyBoard.Begin();
        }

        //ScaleTransform scaleTransform = new ScaleTransform() { ScaleX = 2, ScaleY = 2 };  //清晰两倍,但慢
        private WriteableBitmap GetEditWbmp()
        {
            LayerBase curBr = base.LayerBase;
            WriteableBitmap wBmp;

            if (curBr == null)
            {
                return null;
            }

            #region 获取绘图对象
            if (BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Color || BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Original)
            {
                if (!(curBr.Background is ImageBrush))
                {
                    wBmp = new WriteableBitmap(curBr, null);
                }
                else
                {
                    wBmp = (curBr.Background as ImageBrush).ImageSource as WriteableBitmap;
                    if ((curBr.Background as ImageBrush).ImageSource as WriteableBitmap == null || wBmp.PixelWidth > App.Current.Host.Content.ActualWidth)
                    {
                        wBmp = new WriteableBitmap(curBr, null);
                    }
                }
            }
            else
            {
                if (curBr.OpacityMask != null)
                {
                    wBmp = (curBr.OpacityMask as ImageBrush).ImageSource as WriteableBitmap;
                }
                else
                {
                    //wBmp = new WriteableBitmap(curBr, scaleTransform);  //清晰两倍
                    wBmp = new WriteableBitmap(curBr, null);
                }
            }
            #endregion

            return wBmp;
        }

        void DrawPoint(Point pt)
        {
            _lastPt = pt;
            //_lastPt = scaleTransform.Transform(pt);   //清晰两倍

            this.DrawLine(pt);
        }

        void DrawLine(Point pt)
        {
            //pt = scaleTransform.Transform(pt);  //清晰两倍

            if (this.LayerBase == null)
            {
                return;
            }

            WriteableBitmap wBmp = this.GetEditWbmp();

            if (BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Color || BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Original)
            {
 
            }
            //if (_lastPt == pt)
            //{
            //    // Yuweiz.Phone.Media.Imaging.WriteableBitmapExt.Instance.DrawEachPoint(pt, this.BrushWbmp, wBmp, BrushFactory.Instance.BrushPenType);         
            //    Yuweiz.Phone.Media.Imaging.WriteableBitmapExt.Instance.DrawEachPointForLine(_lastPt, pt, this.BrushWbmp, wBmp, BrushFactory.Instance.BrushPenType);
            //}
            //else
            //{
            //    Yuweiz.Phone.Media.Imaging.WriteableBitmapExt.Instance.WbmpLinePixels = BrushFactory.Instance.WbmpLinePixels;
              // Yuweiz.Phone.Media.Imaging.WriteableBitmapExt.Instance.DrawEachPointForLine(_lastPt, pt, this.BrushWbmp, wBmp, BrushFactory.Instance.BrushPenType);
            //}
           Yuweiz.Phone.Media.Imaging.WriteableBitmapExt.Instance.DrawBrushFantasia(_lastPt, pt, this.BrushWbmp, wBmp, BrushFactory.Instance.BrushPenType);

            #region 呈现绘图对象
            ImageBrush ibrush = new ImageBrush();
            ibrush.ImageSource = wBmp;
            if (BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Color || BrushFactory.Instance.BrushPenType == Yuweiz.Phone.Media.Imaging.FillPixelTypeEnum.Original)
            {
                base.LayerBase.Background = ibrush;
            }
            else
            {
                base.LayerBase.OpacityMask = ibrush;
                //curBr.Background = ibrush;
            }
            #endregion

            _lastPt = pt;
        }

    }
}
