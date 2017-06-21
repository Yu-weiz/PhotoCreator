using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Yuweiz.Phone.Gestures
{
    public abstract class GestureAbstract
    {
        protected GestureAbstract()
        {

        }

        /// <summary>
        /// 记录上接触点的信息
        /// </summary>
        protected  TouchPointCollection _lastTouchPoints;
        protected static TouchPointCollection _originalPoints;               //记录：手指按下时的手指接触信息；用于缩放显示的范围
        //private static Point lastPox = new Point();                       //记录上一次X、Y轴上的偏移       
        //private static double lastFingerDistance = 0.0;                   //记录移动时上一次的两指间距离
        //private static double firstFingerDistance = 0.0;                  //记录按下时上一次的两指间距离  －增补启用两指移动时，缩放的感应范围(<50时的)     
        //private static TouchPointCollection originalTouchPoints;          //记录两指初始按下时的点：用于旋转背景图 

        protected static GestureTypeEnum _curCheckedGestureType;

        public GestureStateEnum GestureState
        {
            get;
            protected set;
        }

        /// <summary>
        /// 手势中的参数信息
        /// </summary>
        public abstract GestureEventArgs GestureArgs
        {
            get;
            protected set;
        }

        /// <summary>
        /// 指示此手势是否活动
        /// </summary>
        protected bool IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// 获取手势类型
        /// </summary>
        public GestureTypeEnum GestureType
        {
            get;
            set;
        }

        /// <summary>
        /// 检查是否此手势是否触发
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public abstract bool GetGestureIsActive(TouchFrameEventArgs e);

    }
}
