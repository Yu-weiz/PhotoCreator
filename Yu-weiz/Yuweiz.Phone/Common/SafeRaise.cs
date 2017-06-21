// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Yuweiz.Phone.Common
{
    /// <summary>
    /// 安全触发事件辅助类
    /// </summary>
    public  static class SafeRaise
    {
        /// <summary>
        /// 以安全的方式触发事件，并作null值判断
        /// </summary>
        /// <param name="eventToRaise">要触发的事件</param>
        /// <param name="sender">事件的发送者</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping existing implementation.")]
        public static void Raise(EventHandler eventToRaise, object sender)
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 以安全的方式触发事件，并作null值判断
        /// </summary>
        /// <param name="eventToRaise">要触发的事件</param>
        /// <param name="sender">事件的发送者</param>
        public static void Raise(EventHandler<EventArgs> eventToRaise, object sender)
        {
            Raise(eventToRaise, sender, EventArgs.Empty);
        }

        /// <summary>
        /// 以安全的方式触发事件，并作null值判断
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="eventToRaise">要触发的事件</param>
        /// <param name="sender">事件的发送者</param>
        /// <param name="args">事件参数</param>
        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, T args) where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, args);
            }
        }
    }
}
