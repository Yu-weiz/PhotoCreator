using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 指示手势的状态：未开始，刚开始，持续中，刚完成 
    /// GestureListener 中声明的事件，要后缀要与此一致
    /// </summary>
    public enum GestureStateEnum
    {
        /// <summary>
        /// 未开始
        /// </summary>
        None = 0,
        /// <summary>
        /// 刚开始
        /// </summary>
        Started = 1,
        /// <summary>
        ///  持续中
        /// </summary>
        Delta = 2,
        /// <summary>
        ///  刚完成
        /// </summary>
        Completed = 3
    }
}
