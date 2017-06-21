using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 单指轻弹手势
    /// 只有开始与结束两状态，中间状态则为：DragWith1FingerGesture
    /// 类名：一定要与与GestureTypeEnum中的枚举匹配，枚举名+Gesture
    /// </summary>
    public class TapGesture : GestureAbstract
    {
        public TapGesture()
        { }

        public override GestureEventArgs GestureArgs
        {
            get;
            protected set;
        }

        public override bool GetGestureIsActive(TouchFrameEventArgs e)
        {
            //throw new NotImplementedException();
            base.IsActive = false;
            if (base.GestureState == GestureStateEnum.Completed)
            {
                base.GestureState = GestureStateEnum.None;
            }

            TouchPointCollection tPoints = e.GetTouchPoints(null);
            if (tPoints.Count == 1)
            {
                if (tPoints[0].Action == TouchAction.Down)
                {
                    base.GestureState = GestureStateEnum.Started;
                    base.IsActive = true;
                }
                else if (tPoints[0].Action == TouchAction.Up)
                {
                    base.GestureState = GestureStateEnum.Completed;
                    base.IsActive = true;
                }

                //如果手势是活动的，则更新参数
                if (base.IsActive)
                {
                    GestureAbstract._curCheckedGestureType = GestureTypeEnum.Tap;

                    GestureArgs = new GestureEventArgs(tPoints);
                    GestureArgs.TouchEventArgs = e;
                }
                /////
            }
            else
            {
                base.GestureState = GestureStateEnum.None;
            }


            return base.IsActive;
        }
    }
}
