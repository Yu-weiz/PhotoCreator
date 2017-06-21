using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 单指自由拖动手势
    /// 类名：一定要与与GestureTypeEnum中的枚举匹配，枚举名+Gesture
    /// </summary>
    public class DragWith1FingerGesture : GestureAbstract
    {
        public DragWith1FingerGesture()
        {} 

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
            //throw new NotImplementedException();
            base.IsActive = false;

            if (GestureAbstract._curCheckedGestureType != GestureTypeEnum.DragWith1Finger)
            {
                base.GestureState = GestureStateEnum.None;
            }

            if (base.GestureState == GestureStateEnum.Completed)
            {
                base.GestureState = GestureStateEnum.None;
            }

            TouchPointCollection tPoints = e.GetTouchPoints(null);
            if (tPoints.Count == 1)
            {                
                if (tPoints[0].Action == TouchAction.Move)
                {
                    if (base.GestureState == GestureStateEnum.None)
                    {
                        base.GestureState = GestureStateEnum.Started;                      
                        base.IsActive = true;
                    }
                    else
                    {
                        base.GestureState = GestureStateEnum.Delta;
                        base.IsActive = true;
                    }
                }
                else if (tPoints[0].Action == TouchAction.Up)
                {
                    if (base.GestureState == GestureStateEnum.Delta)
                    {
                        base.GestureState = GestureStateEnum.Completed;
                        base.IsActive = true;
                    }
                }

                //如果手势是活动的，则更新参数
                if (base.IsActive)
                {
                    GestureAbstract._curCheckedGestureType = GestureTypeEnum.DragWith1Finger;

                    GestureArgs = new DragGestureEventArgs(tPoints);

                    if (_lastTouchPoints == null)
                    {
                        _lastTouchPoints = tPoints;
                    }

                    double inkPosX = (tPoints[0].Position.X - _lastTouchPoints[0].Position.X);
                    double inkPosY = (tPoints[0].Position.Y - _lastTouchPoints[0].Position.Y);
                    _dragGestureEventArgs.DistanceDelta = new System.Windows.Point(inkPosX, inkPosY);
                    _dragGestureEventArgs.TouchEventArgs = e;
                }
                /////
            }

            _lastTouchPoints = tPoints;
            return base.IsActive;
        }
    }
}
