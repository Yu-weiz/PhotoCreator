using System;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 包含呈现不同多点触控手势的值，GestureListener.ReadGesture 可检测到这些手势。
    /// 对应手势类名要与此枚举匹配(以便工厂生成对象)：类名＝枚举项+Gesture
    /// </summary>
    //[Flags]
    public enum GestureTypeEnum
    {
        /// <summary>
        ///  呈现无手势。
        /// </summary>
        None = 0,
        /// <summary>
        /// 用户触控屏幕一点，然后执行自由格式的拖动手势。
        /// </summary>
        DragWith1Finger = 1,
        /// <summary>
        ///  用户触控屏幕两点，然后执行自由格式的拖动手势。
        /// </summary>
        DragWith2Finger = 2,
        /// <summary>
        /// 捏手势
        /// </summary>
        Pinch = 3,
        /// <summary>
        /// 轻弹
        /// </summary>
        Tap = 4,
       

    }
}
