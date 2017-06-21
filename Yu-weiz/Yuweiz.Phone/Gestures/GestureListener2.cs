using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 使用Manipulation的手执检测
    /// 从Windows phone toolkit中提取
    /// </summary>
    public class GestureListener2
    {
        public GestureListener2()
        {
            isPinch = false;
            isDrag = false;
            isGestureOnTarget = false;
        }

        public GestureListener2(FrameworkElement element)
        {
            isPinch = false;
            isDrag = false;
            isGestureOnTarget = false;

            this.border = element;

            if (element != null)
            {
                //border.Tap += OnTap;
                //border.DoubleTap += OnDoubleTap;
                //border.Hold += OnHold;
                border.ManipulationStarted += OnManipulationStarted;
                border.ManipulationDelta += OnManipulationDelta;
                border.ManipulationCompleted += OnManipulationCompleted;

                //border.CacheMode=new BitmapCache();
                //border.RenderTransformOrigin=new Point (0.5,0.5);             
            }
        }

        bool isPinch;
        bool isDrag;
        bool isGestureOnTarget;
        FrameworkElement border;

        /// <summary>
        /// 提示手势是否作用在目标上
        /// </summary>
        public bool IsGestureOnTarget
        {
            get { return isGestureOnTarget; }
        }


        public event EventHandler<ManipulationDeltaEventArgs> DragStarted;
        public event EventHandler<ManipulationDeltaEventArgs> DragDelta;
        public event Action<Point> DragCompleted;
        public event EventHandler<ManipulationDeltaEventArgs> PinchStarted;
        public event EventHandler<ManipulationDeltaEventArgs> PinchDelta;
        public event Action PinchCompleted;
        public event EventHandler<ManipulationCompletedEventArgs> Flick;
        public event EventHandler<FlickEventArgs> FlickExt;

        #region UIElement touch event handlers

        #region 待删
        //手势开始
        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {

        }

        // UIElement.Tap is used in place of GestureListener.Tap.
        private void OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        // UIElement.DoubleTap is used in place of GestureListener.DoubleTap.
        private void OnDoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        // UIElement.Hold is used in place of GestureListener.Hold.
        private void OnHold(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
        #endregion


        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            bool oldIsPinch = isPinch;
            bool oldIsDrag = isDrag;
            isPinch = e.PinchManipulation != null;

            // The origin of the first manipulation after a pinch is completed always corresponds to the
            // primary touch point from the pinch, even if the secondary touch point is the one that 
            // remains active. In this sample we only want a drag to affect the rectangle if the finger
            // on the screen falls inside the rectangle's bounds, so if we've just finished a pinch,
            // we have to defer until the next ManipulationDelta to determine whether or not a new 
            // drag has started.
            isDrag = e.PinchManipulation == null && !oldIsPinch;

            // check for ending gestures
            if (oldIsDrag && !isDrag)
            {
                //执行拖完成事件
                if (this.DragCompleted != null)
                {
                    this.DragCompleted(e.ManipulationOrigin);
                }
                /////
            }
            if (oldIsPinch && !isPinch)
            {
                //执行捏拉完成事件
                if (this.PinchCompleted != null)
                {
                    this.PinchCompleted();
                }
                /////
            }

            // check for continuing gestures
            if (oldIsDrag && isDrag)
            {
                //执行拖持续事件
                if (this.DragDelta != null)
                {
                    this.DragDelta(sender, e);
                }
                /////
            }
            if (oldIsPinch && isPinch)
            {
                //执行捏拉完成事件
                if (this.PinchDelta != null)
                {
                    this.PinchDelta(sender, e);
                }
                /////
            }

            // check for starting gestures
            if (!oldIsDrag && isDrag)
            {
                // Once a manipulation has started on the UIElement, that element will continue to fire ManipulationDelta
                // events until all fingers have left the screen and we get a ManipulationCompleted. In this sample
                // however, we treat each transition between pinch and drag as a new gesture, and we only want to 
                // apply effects to our border control if the the gesture begins within the bounds of the border.
                isGestureOnTarget = e.ManipulationContainer == border &&
                        new Rect(0, 0, border.ActualWidth, border.ActualHeight).Contains(e.ManipulationOrigin);

                //执行拖开始事件
                if (this.DragStarted != null)
                {
                    this.DragStarted(sender, e);
                }
                /////
            }
            if (!oldIsPinch && isPinch)
            {
                isGestureOnTarget = e.ManipulationContainer == border &&
                        new Rect(0, 0, border.ActualWidth, border.ActualHeight).Contains(e.PinchManipulation.Original.PrimaryContact);

                //执行捏拉开始事件
                if (this.PinchStarted != null)
                {
                    this.PinchStarted(sender, e);
                }
                /////
            }
        }

        // UIElement.ManipulationCompleted indicates the end of a touch interaction. It tells us that
        // we went from having at least one finger on the screen to having no fingers on the screen.
        // If e.IsInertial is true, then it's also the same thing as GestureListener.Flick,
        // although the event args API for the flick case are different, as will be noted inside that method.
        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (isDrag)
            {
                isDrag = false;

                //执行拖完成事件
                if (this.DragCompleted != null)
                {
                    this.DragCompleted(e.ManipulationOrigin);
                }
                /////

                if (e.IsInertial)
                {
                    //执行弹开事件
                    if (this.Flick != null)
                    {
                        this.Flick(sender, e);
                    }
                    /////
                }
            }

            if (isPinch)
            {
                isPinch = false;

                //执行捏拉结束事件
                if (this.PinchCompleted != null)
                {
                    this.PinchCompleted();
                }
                /////
            }
        }

        #endregion

        #region Gesture events inferred from UIElement.Manipulation* touch events

        /// <summary>
        ///拖开始
        /// </summary>
        private void OnDragStarted()
        {
            if (isGestureOnTarget)
            {

            }
        }

        /// <summary>
        /// 拖持续
        /// </summary>
        private void OnDragDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (isGestureOnTarget)
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

            }
        }

        /// <summary>
        /// 拖结束
        /// </summary>
        private void OnDragCompleted()
        {
            if (isGestureOnTarget)
            {

            }
        }

        /// <summary>
        /// 捏拉开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPinchStarted(object sender, ManipulationDeltaEventArgs e)
        {
            if (isGestureOnTarget)
            {

            }
        }

        /// <summary>
        /// 捏拉持续
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPinchDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (isGestureOnTarget)
            {
                // Rather than providing the rotation, the event args now just provide
                // the raw points of contact for the pinch manipulation.
                // However, calculating the rotation from these two points is fairly trivial;
                // the utility method used here illustrates how that's done.
                // Note that we don't have to apply a transform because the angle delta is the
                // same in any non-skewed reference frame.
                double angleDelta = this.GetAngle(e.PinchManipulation.Current) - this.GetAngle(e.PinchManipulation.Original);

                //transform.Rotation = initialAngle + angleDelta;

                // DistanceRatio from PinchGestureEventArgs is now replaced by
                // PinchManipulation.DeltaScale and PinchManipulation.CumulativeScale,
                // which expose the scale from the pinch directly.
                // Note that we don't have to apply a transform because the distance ratio is the
                // same in any reference frame.

                //transform.ScaleX *= e.PinchManipulation.DeltaScale;
                //transform.ScaleY *= e.PinchManipulation.DeltaScale;
            }
        }

        /// <summary>
        /// 捏拉结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPinchCompleted()
        {
            if (isGestureOnTarget)
            {

            }
        }

        /// <summary>
        ///  弹开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFlick(object sender, ManipulationCompletedEventArgs e)
        {
            if (isGestureOnTarget)
            {
                // All of the properties on FlickGestureEventArgs have been replaced by the single property
                // FinalVelocities.LinearVelocity.  This method shows how to retrieve from FinalVelocities.LinearVelocity
                // the properties that used to be in FlickGestureEventArgs. Also, note that while the GestureListener
                // provided fairly precise directional information, small linear velocities here are rounded
                // to 0, resulting in flick vectors that are often snapped to one axis.

                //  Point transformedVelocity = GetTransformNoTranslation(transform).Transform(e.FinalVelocities.LinearVelocity);

                // double horizontalVelocity = transformedVelocity.X;
                //  double verticalVelocity = transformedVelocity.Y;

                // flickData.Text = string.Format("{0} Flick: Angle {1} Velocity {2},{3}",
                //   this.GetDirection(horizontalVelocity, verticalVelocity), Math.Round(this.GetAngle(horizontalVelocity, verticalVelocity)), horizontalVelocity, verticalVelocity);
            }
        }
        #endregion

        #region Helpers
        public GeneralTransform GetTransformNoTranslation(CompositeTransform transform)
        {
            CompositeTransform newTransform = new CompositeTransform();
            newTransform.Rotation = transform.Rotation;
            newTransform.ScaleX = transform.ScaleX;
            newTransform.ScaleY = transform.ScaleY;
            newTransform.CenterX = transform.CenterX;
            newTransform.CenterY = transform.CenterY;
            newTransform.TranslateX = 0;
            newTransform.TranslateY = 0;

            return newTransform;
        }

        /// <summary>
        /// 获取角度
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public double GetAngle(PinchContactPoints points)
        {
            Point directionVector = new Point(points.SecondaryContact.X - points.PrimaryContact.X, points.SecondaryContact.Y - points.PrimaryContact.Y);
            return GetAngle(directionVector.X, directionVector.Y);
        }

        /// <summary>
        /// 获取方向
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Orientation GetDirection(double x, double y)
        {
            return Math.Abs(x) >= Math.Abs(y) ? System.Windows.Controls.Orientation.Horizontal : System.Windows.Controls.Orientation.Vertical;
        }

        /// <summary>
        /// 获取角度
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double GetAngle(double x, double y)
        {
            // Note that this function works in xaml coordinates, where positive y is down, and the
            // angle is computed clockwise from the x-axis. 
            double angle = Math.Atan2(y, x);

            // Atan2() returns values between pi and -pi.  We want a value between
            // 0 and 2 pi.  In order to compensate for this, we'll add 2 pi to the angle
            // if it's less than 0, and then multiply by 180 / pi to get the angle
            // in degrees rather than radians, which are the expected units in XAML.
            if (angle < 0)
            {
                angle += 2 * Math.PI;
            }

            return angle * 180 / Math.PI;
        }
        #endregion

        protected FlickEventArgs TranslateFlickEventArgs(ManipulationCompletedEventArgs e)
        {
            FlickEventArgs flickEventArgs = new Gestures.FlickEventArgs();
            Point transformedVelocity = e.FinalVelocities.LinearVelocity;

            double horizontalVelocity = transformedVelocity.X;
            double verticalVelocity = transformedVelocity.Y;

            Orientation orientation = this.GetDirection(horizontalVelocity, verticalVelocity);
            double angle = Math.Round(this.GetAngle(horizontalVelocity, verticalVelocity));

            if (angle > 220 && angle < 320)
            {
                flickEventArgs.OrientationEnum = OrientationEnum.Up;
            }
            else if (angle > 130 && angle < 220)
            {
                flickEventArgs.OrientationEnum = OrientationEnum.Left;
            }
            else if (angle > 40 && angle < 130)
            {
                flickEventArgs.OrientationEnum = OrientationEnum.Down;
            }
            else
            {
                flickEventArgs.OrientationEnum = OrientationEnum.Right;
            }

            return flickEventArgs;
        }
    }

    public enum OrientationEnum {Up,Down,Left,Right }

    public class FlickEventArgs : EventArgs
    {
        public OrientationEnum OrientationEnum { get; set; }

        /// <summary>
        /// 水平速度
        /// </summary>
        public double HorizontalVelocity { get; set; }

        /// <summary>
        /// 垂直速度
        /// </summary>
        public double VerticalVelocity { get; set; }
    }
}
