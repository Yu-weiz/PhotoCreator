using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Yuweiz.Phone.Gestures
{
    public partial class GestureListener
    {
        private static bool _isInTouch;
        private static Point _gestureOrigin;
        private static TouchFrameEventArgs _curTouchFrameEventArgs;
        private static List<GestureTypeEnum> _enabledGestures;               //启用手势（或需要检测的手势）

        private static List<UIElement> _elements;
        private static UIElement _element;

        public static UIElement Element
        {
            get { return GestureListener._element; }
            set { GestureListener._element = value; }
        }

        /// <summary>
        /// 指示是否监听
        /// </summary>
        private static bool isEnable { get; set; }
        
        /// <summary>
        /// 注意函数中的手执添加次序
        /// 不能随意调换
        /// </summary>
        static GestureListener()
        {
            Touch.FrameReported += Touch_FrameReported;

            _enabledGestures = new List<GestureTypeEnum>();
            _enabledGestures.Add(GestureTypeEnum.Tap);
            _enabledGestures.Add(GestureTypeEnum.DragWith1Finger);           
           _enabledGestures.Add(GestureTypeEnum.DragWith2Finger);
            _enabledGestures.Add(GestureTypeEnum.Pinch);                      
        }


        static void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            //if (!IsEnable)
            //{
            //    return;
            //}

            bool newIsInTouch = false;
            Point gestureOrigin = new Point(0, 0);
            _curTouchFrameEventArgs = e;

            foreach (TouchPoint point in e.GetTouchPoints(null))
            {
                if (point.Action != TouchAction.Up)
                {
                    gestureOrigin = point.Position;
                    newIsInTouch = true;
                    break;
                }
            }

            if (!_isInTouch && newIsInTouch)
            {
                // The user was not in the middle of a gesture, but one has started.
                _gestureOrigin = gestureOrigin;
                TouchStart();
            }
            else if (_isInTouch && !newIsInTouch)
            {
                // The user was in the middle of a gesture, but there are no active 
                // touch points anymore.
                TouchComplete();
            }
            else if (_isInTouch)
            {
                // The state has not changed, and the user was in the middle of a gesture.
                TouchDelta();
            }
            else
            {
                // Possible error condition? The user was not in the middle of a 
                // gesture, but a Touch.FrameReported event was received with no
                // active touch points. We should poll the TouchPanel just to be 
                // safe, but do so in such a way that resets the state.
                TouchStart();
            }

            _isInTouch = newIsInTouch;
        }

        /// <summary>
        /// 手势开始
        ///  ：调用ProcessGestureEvents()完成
        /// </summary>
        private static void TouchStart()
        {
           // if (_elements == null)
            {
                _elements = (List<UIElement>)VisualTreeHelper.FindElementsInHostCoordinates(_gestureOrigin, Application.Current.RootVisual);
            }

            ProcessGestureEvents();
        }

        /// <summary>
        /// 手势持续
        ///  ：调用ProcessGestureEvents()完成
        /// </summary>
        private static void TouchDelta()
        {
            ProcessGestureEvents();
        }

        /// <summary>
        /// 手势完成
        /// ：调用ProcessGestureEvents()完成
        /// </summary>
        private static void TouchComplete()
        {
            ProcessGestureEvents();
        }

        /// <summary>
        /// 检测对应手势，并触发对应事件
        /// </summary>
        private static void ProcessGestureEvents()
        {
            GestureAbstract gestureAbstract = ReadGesture();
            if (gestureAbstract == null)
            {
                return;
            }
            
            RaiseGestureEvent((helper) =>helper.GetGestureEvent(gestureAbstract.GestureType.ToString() + gestureAbstract.GestureState.ToString()), () => { return gestureAbstract.GestureArgs; }, false);
          //  RaiseGestureEvent((helper) =>{return GetEventGetter<EventHandler>(helper,gestureAbstract.GestureType,gestureAbstract.GestureState)}, () => { return gestureAbstract.GestureArgs; }, false);
        }
         
        /// <summary>
        /// 检测当前手势
        /// </summary>
        /// <returns>返回当前检测到的手势</returns>
        private static GestureAbstract ReadGesture()
        {
            GestureAbstract gesture = null;
            List<GestureAbstract> gestureList = GestureFactory.GestureFactoryInstance.GetGestureAbstractList(_enabledGestures);

            foreach (GestureAbstract gestureAbstract in gestureList)
            {
                bool curGestureIsActive = gestureAbstract.GetGestureIsActive(_curTouchFrameEventArgs);
                if (curGestureIsActive)
                {
                    //检测到的手势
                    gesture = gestureAbstract;
                    string gestureEnumItemName = gesture.GetType().Name.Replace("Gesture", "");
                    gesture.GestureType = (GestureTypeEnum)Enum.Parse(typeof(GestureTypeEnum), gestureEnumItemName, true);

                    break;
                    //如果手势为持续则不再检测新手势
                    //if (gesture.GestureState == GestureStateEnum.Delta)
                    //{
                    //    break;
                    //}
                }
            }

            return gesture;
        }


        private static void RaiseGestureEvent<T>(Func<GestureListener, EventHandler<T>> eventGetter, Func<T> argsGetter, bool releaseMouseCapture) where T : GestureEventArgs
        {
            if (eventGetter == null)
            {
                return;
            }

            T args = argsGetter();

            bool handle = false;
            UIElement originalElement = null;

            foreach (UIElement element in _elements)
            {
                if (!handle)
                {
                    if (originalElement == null)
                    {
                        originalElement = element;
                    }
                   
                    GestureListener gestureListener = GestureService.GetGestureListenerInternal(element, false);
                    if (gestureListener != null)
                    {
                        eventGetter(gestureListener);
                        Raise(eventGetter(gestureListener), element, () => { return args; });
                        handle = true;
                        break;
                    }
                }
            }
        }

        public delegate T GetEventArgs<T>() where T : EventArgs;

        private static void Raise<T>(EventHandler<T> eventToRaise, object sender, GetEventArgs<T> getEventArgs) where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, getEventArgs());
            }
        }

        #region 过期函数
        /// <summary>
        /// 检测对应手势，并触发对应事件
        /// </summary>
        private static void ProcessGestureEventsBackup()
        {
            GestureAbstract gestureAbstract = ReadGesture();
            if (gestureAbstract == null)
            {
                return;
            }

            switch (gestureAbstract.GestureType)
            {
                case GestureTypeEnum.DragWith1Finger:
                    {
                        GestureEventArgs dragWith1FingerAgrs = new GestureEventArgs(_curTouchFrameEventArgs.GetTouchPoints(_element));
                        switch (gestureAbstract.GestureState)
                        {
                            case GestureStateEnum.Started: RaiseGestureEvent((helper) => helper.DragWith1FingerStarted, () => { return dragWith1FingerAgrs; }, false);
                                break;
                            case GestureStateEnum.Delta: RaiseGestureEvent((helper) => helper.DragWith1FingerDelta, () => { return dragWith1FingerAgrs; }, false);
                                break;
                            case GestureStateEnum.Completed: RaiseGestureEvent((helper) => helper.DragWith1FingerCompleted, () => { return dragWith1FingerAgrs; }, false);
                                break;
                        }
                        break;
                    }
                case GestureTypeEnum.Pinch:
                    {
                        GestureEventArgs pinchAgrs = new GestureEventArgs(_curTouchFrameEventArgs.GetTouchPoints(_element));
                        switch (gestureAbstract.GestureState)
                        {
                            case GestureStateEnum.Started: RaiseGestureEvent((helper) => helper.PinchStarted, () => { return pinchAgrs; }, false);
                                break;
                            case GestureStateEnum.Delta: RaiseGestureEvent((helper) => helper.PinchDelta, () => { return pinchAgrs; }, false);
                                break;
                            case GestureStateEnum.Completed: RaiseGestureEvent((helper) => helper.PinchCompleted, () => { return pinchAgrs; }, false);
                                break;
                        }
                        break;
                    }
                case GestureTypeEnum.DragWith2Finger:
                    {
                        GestureEventArgs dragWith2FingerAgrs = new GestureEventArgs(_curTouchFrameEventArgs.GetTouchPoints(_element));
                        switch (gestureAbstract.GestureState)
                        {
                            case GestureStateEnum.Started: RaiseGestureEvent((helper) => helper.DragWith2FingerStarted, () => { return dragWith2FingerAgrs; }, false);
                                break;
                            case GestureStateEnum.Delta: RaiseGestureEvent((helper) => helper.DragWith2FingerDelta, () => { return dragWith2FingerAgrs; }, false);
                                break;
                            case GestureStateEnum.Completed: RaiseGestureEvent((helper) => helper.DragWith2FingerCompleted, () => { return dragWith2FingerAgrs; }, false);
                                break;
                        }
                        break;
                    }
                default: break;
            }
        }

        private static EventHandler<T> GetEventGetter<T>(GestureListener lister, GestureTypeEnum gestureType, GestureStateEnum gestureState) where T : GestureEventArgs
        {
            // GestureEventArgs args=new GestureEventArgs (null);
            Assembly assembly = Assembly.Load("Yuweiz.Phone");
            Type typeT = assembly.GetType("Yuweiz.Phone.Gestures." + gestureType.ToString() + "GestureEventArgs");

            MethodInfo getEventHandler = typeof(GestureListener).GetMethod("GetGestureEvent", BindingFlags.Public | BindingFlags.Instance);
            getEventHandler.MakeGenericMethod(typeT);
            object value = getEventHandler.Invoke(lister, new object[] { gestureType.ToString() + gestureState.ToString() });
            return (EventHandler<T>)value;
            // return (T)args;
        }

        #endregion
    }
}
