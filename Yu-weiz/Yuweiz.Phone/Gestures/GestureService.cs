using System;
using System.Windows;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// GestureService 类帮助获取或设置元素的GestureListener
    /// </summary>
    public static class GestureService
    {
        /// <summary>
        /// 获取新元素的一个GestureListener，如有必要将创建一个新的
        /// DependencyObject----->Visual------>UIElement------>FrameworkElement
        /// </summary>
        /// <param name="obj">要获取GestureListener的obj</param>
        /// <returns>已存在或新的GestureListener</returns>
        public static GestureListener GetGestureListener(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return GetGestureListenerInternal(obj, true);
        }

        /// <summary>
        /// 获取一个GestureListener，如createIfMissing为True,则在它为空时创建一个新的
        /// </summary>
        /// <param name="obj">要获取GestureListener的obj</param>
        /// <param name="createIfMissing">若为真：如果没有在元素上设置附加属性则创建一个新的，并绑定到obj</param>
        /// <returns></returns>
        internal static GestureListener GetGestureListenerInternal(DependencyObject obj, bool createIfMissing)
        {
            GestureListener gestureListener = (GestureListener)obj.GetValue(GestureListenerProperty);
            if (gestureListener == null && createIfMissing)
            {
                gestureListener = new GestureListener();
                SetGestureListenerInternal(obj, gestureListener);
            }
            return gestureListener;
        }
        
        /// <summary>
        /// 设置GestureListener到元素，仅能在XAML中使用，如需要代码中使用，则使用GetGestureListener
        /// </summary>
        /// <param name="obj">要设置GestureListener的obj</param>
        /// <param name="value">GestureListener值</param>
        [Obsolete("Do not add handlers using this method. Instead, use GetGestureListener, which will create a new instance if one is not already set, to add your handlers to an element.", true)]
        public static void SetGestureListener(DependencyObject obj, GestureListener value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            SetGestureListenerInternal(obj, value);
        }

        /// <summary>
        /// 内部设置GestureListener到元素
        /// </summary>
        /// <param name="obj">要设置GestureListener的obj</param>
        /// <param name="value">GestureListener值</param>
        private static void SetGestureListenerInternal(DependencyObject obj, GestureListener value)
        {
            obj.SetValue(GestureListenerProperty, value);
        }

        /// <summary>
        /// 附加依赖项属性的GestureListener的定义。
        /// </summary>
        public static readonly DependencyProperty GestureListenerProperty =
           DependencyProperty.RegisterAttached("GestureListener", typeof(GestureListener), typeof(GestureService), new PropertyMetadata(null));
    }
}
