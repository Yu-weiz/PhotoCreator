using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 手势监听
    /// 手势事件命名由GestureTypeEnum、GestureStateEnum枚举元素组合而成
    /// </summary>
    public partial class GestureListener
    {
        public GestureListener()
        {
           
        }

        public event EventHandler<GestureEventArgs> DigStarted;

        public event EventHandler<GestureEventArgs> DigDelta;

        public event EventHandler<GestureEventArgs> DigComplete;


        public event EventHandler<GestureEventArgs> PinchStarted;

        public event EventHandler<GestureEventArgs> PinchDelta;

        public event EventHandler<GestureEventArgs> PinchCompleted;


        public event EventHandler<GestureEventArgs> DragWith2FingerStarted;

        public event EventHandler<GestureEventArgs> DragWith2FingerDelta;

        public event EventHandler<GestureEventArgs> DragWith2FingerCompleted;


        public event EventHandler<GestureEventArgs> DragWith1FingerStarted;

        public event EventHandler<GestureEventArgs> DragWith1FingerDelta;

        public event EventHandler<GestureEventArgs> DragWith1FingerCompleted;


        public event EventHandler<GestureEventArgs> DoubleTap;

        public event EventHandler<GestureEventArgs> TapStarted;

        public event EventHandler<GestureEventArgs> TapCompleted;

        public event EventHandler<GestureEventArgs> Rotate;

        bool _enableEvent;
        /// <summary>
        /// 获取或设置是否允许触发事件
        /// </summary>
        public bool EnableEvent
        {
            get { return _enableEvent; }

            set
            {
                _enableEvent = value;
            }
        }

        /// <summary>
        /// 获取手势事件：根据事件名称
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        private EventHandler<GestureEventArgs> GetGestureEvent(string eventName)
        {           
            if (!EnableEvent)
            {
                return null;
            }

            Type type = this.GetType();
            object value = type.InvokeMember(eventName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, this, new object[] { }); ;
            return (EventHandler<GestureEventArgs>)value;
        }

        #region 过期的
        /// <summary>
        /// 获取手势事件：根据事件名称
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        [Obsolete("此方法仅供学习使用！")]
        private EventHandler<T> GetGestureEvent<T>(string eventName) where T : GestureEventArgs
        {
            Type type = this.GetType();
            object value = type.InvokeMember(eventName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, this, new object[] { }); ;
            return (EventHandler<T>)value;
        }
        #endregion
    }
}
