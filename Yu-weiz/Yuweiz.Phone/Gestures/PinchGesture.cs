using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 捏手势
    /// 类名：一定要与与GestureTypeEnum中的枚举匹配，枚举名+Gesture
    /// </summary>
    public class PinchGesture : GestureAbstract
    {
        private const double ZoomDetectAccuracy = 50;                     //缩放检测的精确度（两指距离的变化大小）

        private TouchPointCollection _originalPoints;                     //记录：手指按下时的手指接触信息；用于缩放显示的范围

        private double _lastFingerDistance = 0;                            //记录上一次两指间的距离

        private double _firstFingerDistance = 0.0;                        //记录按下时的两指间距离  －增补启用两指移动时，缩放的感应范围(<50时的)

        private double _curFingerDistance = 0.0;

        private PinchGestureEventArgs _pinchGestureArgs;

        public override GestureEventArgs GestureArgs
        {
            get
            {
                return _pinchGestureArgs;
            }
            protected set
            {
                if (value is PinchGestureEventArgs)
                {
                    _pinchGestureArgs = (PinchGestureEventArgs)value;
                }
            }
        }


        //public override GestureAbstract Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new PinchGesture();
        //        }

        //        return _instance;
        //    }
        //}

        public override bool GetGestureIsActive(TouchFrameEventArgs e)
        {
            base.IsActive = false;
            if (GestureAbstract._curCheckedGestureType != GestureTypeEnum.Pinch)
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
                _curFingerDistance = Math.Sqrt(Math.Pow(tPoints[0].Position.X - tPoints[1].Position.X, 2) + Math.Pow(tPoints[0].Position.Y - tPoints[1].Position.Y, 2));
                if (tPoints[0].Action == TouchAction.Down || tPoints[1].Action == TouchAction.Down)
                {
                    _firstFingerDistance = _curFingerDistance;
                    _originalPoints = tPoints;
                    _lastFingerDistance = 0.0;

                    //base.GestureState = GestureStateEnum.Started;
                    //base.IsActive = true;         
                }
                if (_lastFingerDistance != 0.0)
                {
                    if (tPoints[0].Action == TouchAction.Move && tPoints[1].Action == TouchAction.Move)
                    {
                        double lastDelta = Math.Abs(_curFingerDistance - _lastFingerDistance);                 //上一次与此次两指间距离的差绝对值
                        double beginToNowDelta = Math.Abs(_curFingerDistance - _firstFingerDistance);          //按下时与此次两指间距离的差绝对值
                        if (lastDelta > ZoomDetectAccuracy || beginToNowDelta > ZoomDetectAccuracy)
                        {
                            if (base.GestureState == GestureStateEnum.None)
                            {
                                base.GestureState = GestureStateEnum.Started;
                                base.IsActive = true;
                            }
                            else
                            {
                                if (GestureAbstract._curCheckedGestureType == GestureTypeEnum.Pinch)
                                {
                                    base.GestureState = GestureStateEnum.Delta;
                                    base.IsActive = true;
                                }
                            }

                        }
                    }
                    ///难以捕捉此状态：受DragWith2FingerGesture影响
                    else if (tPoints[0].Action == TouchAction.Up || tPoints[1].Action == TouchAction.Up)
                    {
                        if (GestureAbstract._curCheckedGestureType == GestureTypeEnum.Pinch)
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
                    GestureAbstract._curCheckedGestureType = GestureTypeEnum.Pinch;
                   
                    _pinchGestureArgs = new PinchGestureEventArgs(tPoints);
                    double delta = _curFingerDistance / this._lastFingerDistance;
                    _pinchGestureArgs.ScaleDelta = new Point(delta, delta);
                    _pinchGestureArgs.OriginalPoints = new Point[] { _originalPoints[0].Position, _originalPoints[1].Position };
                    _pinchGestureArgs.TouchEventArgs = e;

                             
                }
                //////
                else
                {
                    base.GestureState = GestureStateEnum.None;
                }

                _lastFingerDistance = _curFingerDistance;
            }

            _lastTouchPoints = tPoints;

            return base.IsActive;
        }

    }
}
