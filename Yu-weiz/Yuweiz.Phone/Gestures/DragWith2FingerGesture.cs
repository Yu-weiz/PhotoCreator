using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 两指拖动手势
    /// 类名：一定要与与GestureTypeEnum中的枚举匹配，枚举名+Gesture
    /// </summary>
    public class DragWith2FingerGesture : GestureAbstract
    {
        private const double ZoomDetectAccuracy = 50;                     //缩放检测的精确度（两指距离的变化大小）

        private static TouchPointCollection _originalPoints;               //记录：手指按下时的手指接触信息；用于缩放显示的范围

        private static double _lastFingerDistance = 0;                     //记录上一次两指间的距离

        private static double _firstFingerDistance = 0.0;                  //记录按下时的两指间距离  －增补启用两指移动时，缩放的感应范围(<50时的)

        private DragGestureEventArgs _dragGestureEventArgs;

        public override GestureEventArgs GestureArgs
        {
            get { return _dragGestureEventArgs; }

            protected
            set
            {
                if (value is DragGestureEventArgs)
                {
                    _dragGestureEventArgs = (DragGestureEventArgs)value;
                }
            }
        }

        public override bool GetGestureIsActive(TouchFrameEventArgs e)
        {
            base.IsActive = false;
            if (GestureAbstract._curCheckedGestureType != GestureTypeEnum.DragWith2Finger)
            {
                base.GestureState = GestureStateEnum.None;
            }

            if (base.GestureState == GestureStateEnum.Completed)
            {
                base.GestureState = GestureStateEnum.None;
            }

            TouchPointCollection tPoints = e.GetTouchPoints(null);
            if (tPoints.Count == 2)
            {
                //两指间的距离
                double fingerDistance = Math.Sqrt(Math.Pow(tPoints[0].Position.X - tPoints[1].Position.X, 2) + Math.Pow(tPoints[0].Position.Y - tPoints[1].Position.Y, 2));
                if (tPoints[0].Action == TouchAction.Down || tPoints[1].Action == TouchAction.Down)
                {
                    _firstFingerDistance = fingerDistance;
                    _originalPoints = tPoints;
                    _lastFingerDistance = 0.0;
                }
                if (_lastFingerDistance != 0.0)
                {
                    if (tPoints[0].Action == TouchAction.Move && tPoints[1].Action == TouchAction.Move)
                    {
                        double lastDelta = Math.Abs(fingerDistance - _lastFingerDistance);                 //上一次与此次两指间距离的差绝对值
                        double beginToNowDelta = Math.Abs(fingerDistance - _firstFingerDistance);        //按下时与此次两指间距离的差绝对值
                        if (lastDelta < ZoomDetectAccuracy && beginToNowDelta < ZoomDetectAccuracy)
                        //  if (!(lastDelta > ZoomDetectAccuracy || beginToNowDelta > ZoomDetectAccuracy))
                        {
                            if (base.GestureState == GestureStateEnum.None)
                            {
                                base.GestureState = GestureStateEnum.Started;
                                base.IsActive = true;
                            }
                            else
                            {
                                if (GestureAbstract._curCheckedGestureType == GestureTypeEnum.DragWith2Finger)
                                {
                                    base.GestureState = GestureStateEnum.Delta;
                                    base.IsActive = true;
                                }
                            }
                        }
                    }
                    else if (tPoints[0].Action == TouchAction.Up || tPoints[1].Action == TouchAction.Up)
                    {
                        if (GestureAbstract._curCheckedGestureType == GestureTypeEnum.DragWith2Finger)
                        {
                            if (base.GestureState == GestureStateEnum.Delta)
                            {
                                base.GestureState = GestureStateEnum.Completed;
                                base.IsActive = true;
                            }
                        }
                    }
                }

                //如果手势是活动的，则更新参数
                if (base.IsActive)
                {
                    GestureAbstract._curCheckedGestureType = GestureTypeEnum.DragWith2Finger;

                    Point newMin = new Point((tPoints[0].Position.X + tPoints[1].Position.X) / 2, (tPoints[0].Position.Y + tPoints[1].Position.Y) / 2);
                    Point oldMin = new Point((_lastTouchPoints[0].Position.X + _lastTouchPoints[1].Position.X) / 2, (_lastTouchPoints[0].Position.Y + _lastTouchPoints[1].Position.Y) / 2);
                    double inkPosX = (newMin.X - oldMin.X);
                    double inkPosY = (newMin.Y - oldMin.Y);
                    _dragGestureEventArgs = new DragGestureEventArgs(tPoints);
                    _dragGestureEventArgs.DistanceDelta = new Point(inkPosX, inkPosY);
                    _dragGestureEventArgs.TouchEventArgs = e;

                    if (_lastTouchPoints == null)
                    {
                        _lastTouchPoints = tPoints;
                    }        
                    _dragGestureEventArgs.AngelDelta = getAngle(tPoints, _lastTouchPoints) * 100;
                }
                //////
                else
                {
                    base.GestureState = GestureStateEnum.None;
                }

                _lastFingerDistance = fingerDistance;

            }

            _lastTouchPoints = tPoints;
            return base.IsActive;
        }

        /// <summary>
        /// 获取两次触点的变化角度
        /// </summary>
        /// <param name="newTPoints"></param>
        /// <param name="oldTPoints"></param>
        /// <returns></returns>
        private double getAngle(TouchPointCollection newTPoints, TouchPointCollection oldTPoints)
        {
            //三点
            double angleNew = Math.Atan2(newTPoints[1].Position.Y - oldTPoints[0].Position.Y, newTPoints[1].Position.X - oldTPoints[0].Position.X);
            double angleOld = Math.Atan2(oldTPoints[1].Position.Y - oldTPoints[0].Position.Y, oldTPoints[1].Position.X - oldTPoints[0].Position.X);
            double angleDetal = angleNew - angleOld;
            return angleDetal;
        }
    }
}
