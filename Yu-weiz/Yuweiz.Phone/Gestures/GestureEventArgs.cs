using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace Yuweiz.Phone.Gestures
{
    public class GestureEventArgs : EventArgs
    {
        public GestureEventArgs(TouchPointCollection positions)
        {
            PositionList = positions;
        }

        /// <summary>
        /// 手指的点击位
        /// 如果只有一个手指点击，击列表仅有一行记录
        /// </summary>
        public TouchPointCollection PositionList { get; set; }

        public TouchFrameEventArgs TouchEventArgs { get; set; }
    }

    public class PinchGestureEventArgs : GestureEventArgs
    {
        public PinchGestureEventArgs(TouchPointCollection positions)
            : base(positions)
        {

        }

        public Point ScaleDelta { get; set; }

        public Point[] OriginalPoints { get; set; }

        /// <summary>
        /// 自接触至此总共变化的角度
        /// </summary>
       // public int TotalAngel { get; set; }
    }

    public class DragGestureEventArgs : GestureEventArgs
    {
        public DragGestureEventArgs(TouchPointCollection positions)
            : base(positions)
        {

        }

        /// <summary>
        /// 触点变化的角度
        /// </summary>
        public double AngelDelta { get; set; }

        /// <summary>
        /// 手指变化的移动距离
        /// </summary>
        public Point DistanceDelta { get; set; }
    }
}
