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
using WPPhotoEditor.UControls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace WPPhotoEditor.Media.Tools
{
    public class TransformTool : ToolAbstract
    {
        public TransformTool()
        {

        }

        public static readonly TransformTool Instance = new TransformTool();

        public override void BindLayer(LayerBase layer)
        {
            this.LayerBase = layer;
            if (this.LayerBase == null)
            {
                return;
            }

            this.transform = this.LayerBase.RenderTransform as CompositeTransform;
            if (this.transform == null)
            {
                throw new System.Exception("LayerBase 图层的变换类型必须先定义成：CompositeTransform");
            }

            base.gestureListener2 = new Yuweiz.Phone.Gestures.GestureListener2(this.LayerBase);
            base.gestureListener2.DragDelta += gestureListener2_DragDelta;
            base.gestureListener2.PinchStarted += gestureListener2_PinchStarted;
            base.gestureListener2.PinchDelta += gestureListener2_PinchDelta;

            base.gestureListener2.DragStarted += gestureListener2_DragStarted;
            base.gestureListener2.DragCompleted += gestureListener2_DragCompleted;
            base.gestureListener2.PinchCompleted += gestureListener2_PinchCompleted;
            base.LayerBase.DoubleTap += LayerBase_DoubleTap;
            base.LayerBase.Tap += LayerBase_Tap;
        }

        void LayerBase_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.CheckShowLayerToolAction != null)
            {
                this.CheckShowLayerToolAction(sender as LayerBase);
            }
        }

        void LayerBase_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.ShowLayerToolAction != null)
            {
                this.ShowLayerToolAction(sender as LayerBase);
            }
        }

        public override void ReleaseLayer()
        {
            base.gestureListener2.DragDelta -= gestureListener2_DragDelta;
            base.gestureListener2.PinchStarted -= gestureListener2_PinchStarted;
            base.gestureListener2.PinchDelta -= gestureListener2_PinchDelta;

            base.gestureListener2.DragStarted -= gestureListener2_DragStarted;
            base.gestureListener2.DragCompleted -= gestureListener2_DragCompleted;
            base.gestureListener2.PinchCompleted -= gestureListener2_PinchCompleted;
            base.LayerBase.DoubleTap -= LayerBase_DoubleTap;
            base.LayerBase.Tap -= LayerBase_Tap;

            this.transform = new CompositeTransform();
            this.LayerBase = null;
        }

        public Action<LayerBase> ShowLayerToolAction { get; set; }

        public Action<LayerBase> CheckShowLayerToolAction { get; set; }

        /// <summary>
        /// 旋转角度记录
        /// </summary>
        private double initialAngle;

        private CompositeTransform transform;


        private Brush backgroundBrush;

        private Brush opacityMaskBrush;

        void gestureListener2_DragDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.gestureListener2.IsGestureOnTarget)
            {
                // HorizontalChange and VerticalChange from DragDeltaGestureEventArgs are now
                // DeltaManipulation.Translation.X and DeltaManipulation.Translation.Y.

                // The translation is given in the coordinate space of e.ManipulationContainer, which in
                // this case is the border control that we're applying transforms to. We need to apply 
                // the the current rotation and scale transforms to the deltas to get back to screen coordinates.
                // Note that if other ancestors of the border control had transforms applied as well, we would
                // need to use UIElement.TransformToVisual to get the aggregate transform between
                // the border control and Application.Current.RootVisual. See GestureListenerStatic.cs in the 
                // WP8 toolkit source for a detailed look at how this can be done.
                Point transformedTranslation = this.gestureListener2.GetTransformNoTranslation(transform).Transform(e.DeltaManipulation.Translation);

                transform.TranslateX += transformedTranslation.X;
                transform.TranslateY += transformedTranslation.Y;
            }
        }

        void gestureListener2_PinchStarted(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.gestureListener2.IsGestureOnTarget)
            {
                initialAngle = transform.Rotation;

                gestureListener2_DragStarted(null, null);
            }
        }

        void gestureListener2_PinchDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.gestureListener2.IsGestureOnTarget)
            {
                // Rather than providing the rotation, the event args now just provide
                // the raw points of contact for the pinch manipulation.
                // However, calculating the rotation from these two points is fairly trivial;
                // the utility method used here illustrates how that's done.
                // Note that we don't have to apply a transform because the angle delta is the
                // same in any non-skewed reference frame.
                double angleDelta = this.gestureListener2.GetAngle(e.PinchManipulation.Current) - this.gestureListener2.GetAngle(e.PinchManipulation.Original);

                transform.Rotation = initialAngle + angleDelta;

                // DistanceRatio from PinchGestureEventArgs is now replaced by
                // PinchManipulation.DeltaScale and PinchManipulation.CumulativeScale,
                // which expose the scale from the pinch directly.
                // Note that we don't have to apply a transform because the distance ratio is the
                // same in any reference frame.
                transform.ScaleX *= e.PinchManipulation.DeltaScale;
                transform.ScaleY *= e.PinchManipulation.DeltaScale;
            }
        }


        void gestureListener2_DragCompleted(Point obj)
        {
            gestureListener2_PinchCompleted();
        }

        void gestureListener2_DragStarted(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.backgroundBrush == this.LayerBase.Background || this.LayerBase.OpacityMask == null)
            {
                return;
            }

            this.backgroundBrush = this.LayerBase.Background;
            this.opacityMaskBrush = this.LayerBase.OpacityMask;

            this.LayerBase.Background = new ImageBrush() { ImageSource = new WriteableBitmap(this.LayerBase, new ScaleTransform() { ScaleX = 2, ScaleY = 2 }), Stretch = Stretch.Fill };
            this.LayerBase.OpacityMask = null;
        }

        void gestureListener2_PinchCompleted()
        {
            Storyboard storyBoard = Yuweiz.Phone.Media.StoryBoards.CreateScaleLimitStoryBoard(this.LayerBase, 0.3);
            transform = this.LayerBase.RenderTransform as CompositeTransform;
            storyBoard.Begin();

            if (this.LayerBase.Background == this.backgroundBrush || this.LayerBase.OpacityMask == this.opacityMaskBrush)
            {
                return;
            }

            this.LayerBase.Background = this.backgroundBrush;
            this.LayerBase.OpacityMask = this.opacityMaskBrush;


        }



    }
}
