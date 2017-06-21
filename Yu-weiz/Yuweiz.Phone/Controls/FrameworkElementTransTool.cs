using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Yuweiz.Phone.Gestures;

namespace Yuweiz.Phone.Controls.InkBoard
{
    public class FrameworkElementTransTool
    {
        public FrameworkElementTransTool(FrameworkElement framework)
        {
            BindSketchBoard(framework);
        }
                
        /// <summary>
        /// 旋转角度记录
        /// </summary>
        private double initialAngle;

        private CompositeTransform transform;

        protected GestureListener2 GestureListener
        {
            get;
            set;
        }


        private FrameworkElement frameworkElement { get; set; }

        public CompositeTransform CompositeTransform
        {
            get { return transform; }
            set { transform = value; }
        }

        public void BindSketchBoard(FrameworkElement framework)
        {
            this.frameworkElement = framework;

            if (frameworkElement == null)
            {
                return;
            }

            this.CompositeTransform = this.frameworkElement.RenderTransform as CompositeTransform;
            if (this.CompositeTransform == null)
            {
                this.CompositeTransform = new CompositeTransform();
                this.frameworkElement.RenderTransform = this.CompositeTransform;
            }


            this.GestureListener = new Yuweiz.Phone.Gestures.GestureListener2(this.frameworkElement);
            this.GestureListener.DragDelta += GestureListener_DragDelta;
            this.GestureListener.PinchDelta += GestureListener_PinchDelta;
            this.GestureListener.PinchStarted += GestureListener_PinchStarted;
            this.GestureListener.PinchCompleted += GestureListener_PinchCompleted;

            this.GestureListener.DragStarted += GestureListener_DragStarted;
            this.GestureListener.DragCompleted += GestureListener_DragCompleted;
        }

        public void Resume()
        {
            if (transform == null)
            {
                return;
            }

            transform.Rotation = 0;
            transform.ScaleX = 1;
            transform.ScaleY = 1;
            transform.TranslateX = 0;
            transform.TranslateY = 0;
        }


        public void ReleaseSketchBoard()
        {
            if (this.frameworkElement == null)
            {
                return;
            }

            this.GestureListener.DragDelta -= GestureListener_DragDelta;
            this.GestureListener.PinchDelta -= GestureListener_PinchDelta;
            this.GestureListener.PinchStarted -= GestureListener_PinchStarted;
            this.GestureListener.PinchCompleted -= GestureListener_PinchCompleted;
            this.GestureListener.DragStarted -= GestureListener_DragStarted;
            this.GestureListener.DragCompleted -= GestureListener_DragCompleted;
        }


        void GestureListener_DragCompleted(Point obj)
        {
            this.frameworkElement.CacheMode = null;
        }

        void GestureListener_DragStarted(object sender, ManipulationDeltaEventArgs e)
        {
            if (this.frameworkElement.CacheMode == null)
            {
                // this.TransingImage.CacheMode = new BitmapCache();
            }
        }

        void GestureListener_DragDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.GestureListener.IsGestureOnTarget)
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
                Point transformedTranslation = this.GestureListener.GetTransformNoTranslation(transform).Transform(e.DeltaManipulation.Translation);

                transform.TranslateX += transformedTranslation.X;
                transform.TranslateY += transformedTranslation.Y;

            }

        }

        void GestureListener_PinchStarted(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.GestureListener.IsGestureOnTarget)
            {
                initialAngle = transform.Rotation;
            }

            if (this.frameworkElement.CacheMode == null)
            {
                this.frameworkElement.CacheMode = new BitmapCache();
            }
        }

        void GestureListener_PinchDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (this.GestureListener.IsGestureOnTarget)
            {
                // Rather than providing the rotation, the event args now just provide
                // the raw points of contact for the pinch manipulation.
                // However, calculating the rotation from these two points is fairly trivial;
                // the utility method used here illustrates how that's done.
                // Note that we don't have to apply a transform because the angle delta is the
                // same in any non-skewed reference frame.
                double angleDelta = this.GestureListener.GetAngle(e.PinchManipulation.Current) - this.GestureListener.GetAngle(e.PinchManipulation.Original);

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

        void GestureListener_PinchCompleted()
        {
            this.frameworkElement.CacheMode = null;
        }

    }
}
