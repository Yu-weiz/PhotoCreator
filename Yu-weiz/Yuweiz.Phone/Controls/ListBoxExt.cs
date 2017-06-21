using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Yuweiz.Phone.Controls
{
    public class ListBoxExt : ListBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.MouseMove += CheckScrollTo;
            this.MouseLeftButtonDown += ListBoxExt_MouseLeftButtonDown;
            this.MouseLeftButtonUp += ListBoxExt_MouseLeftButtonUp;
            this.MouseLeave += ListBoxExt_MouseLeave;
        }


        void ListBoxExt_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isCheckingScrollToUp)
            {
                isCheckingScrollToUp = false;
                if (ScrollToUp != null)
                {
                    ScrollToUp();
                }
            }

            if (Math.Abs(movePox) > 130)
            {
                movePox = 0.0d;
                if (ScrollToBottom != null)
                {
                    ScrollToBottom();
                }
            }
        }

        Point downPt;
        bool isCheckingScrollToUp = false;
        double movePox = 0.0d;

        void ListBoxExt_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isCheckingScrollToUp = false;
        }

        void ListBoxExt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ScrollViewer scrollViewer = FindChildOfType<ScrollViewer>(this);//ScrollViewer  scrollBar
            if (scrollViewer == null)
            {
                throw new InvalidOperationException("erro");
            }
            else
            {
                bool isVerticalUp = scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled && scrollViewer.VerticalOffset < 0.001d;
                bool isHorizontalUp = scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled && scrollViewer.HorizontalOffset < 0.001d;

                if (isVerticalUp || isHorizontalUp)
                {
                    isCheckingScrollToUp = true;
                }
            }

            movePox = 0.0d;
            downPt = e.GetPosition(this);
        }

        /// <summary>
        /// 滚动到底部
        /// </summary>
        /// <author>庾伟荣</author>
        public event Action ScrollToBottom;

        public event Action ScrollToUp;

        public double ScrollOffset
        {
            get
            {
                ScrollViewer scrollViewer = FindChildOfType<ScrollViewer>(this);//ScrollViewer  scrollBar
                if (scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled)
                {
                    //垂直
                    return scrollViewer.VerticalOffset;
                }
                else
                {
                    //水平
                    return scrollViewer.HorizontalOffset;
                }
            }
            set
            {
                ScrollViewer scrollViewer = FindChildOfType<ScrollViewer>(this);//ScrollViewer  scrollBar
                if (scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled)
                {
                    //垂直
                    scrollViewer.ScrollToVerticalOffset(value);
                }
                else
                {
                    //水平
                    scrollViewer.ScrollToHorizontalOffset(value);
                }
            }
           
        }

        /// <summary>
        /// 检测是否滚动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <author>庾伟荣</author>
        private void CheckScrollTo(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //获取listbox的子类型ScrollViewer
            ScrollViewer scrollViewer = FindChildOfType<ScrollViewer>(this);//ScrollViewer  scrollBar
            if (scrollViewer == null)
            {
                throw new InvalidOperationException("erro");
            }
            else
            {
                bool isVerticalBottom = scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - scrollViewer.ScrollableHeight * 0.4d && scrollViewer.ScrollableHeight != 0.0d;
                bool isHorizontalBottom = scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled && scrollViewer.HorizontalOffset >= scrollViewer.ScrollableWidth - scrollViewer.ScrollableWidth * 0.4d && scrollViewer.ScrollableWidth != 0.0d;
                //判断当前滚动的高度是否大于或者等于scrollViewer实际可滚动高度，如果等于或者大于就证明到底了
                if (isVerticalBottom || isHorizontalBottom)
                {
                    //处理listbox滚动到底的事情
                    if (ScrollToBottom != null)
                    {
                        ScrollToBottom();
                    }
                }
                //-------------------------

                Point curPt = e.GetPosition(this);
                if ((scrollViewer.VerticalOffset > 0.001d || curPt.Y < downPt.Y) && isCheckingScrollToUp)
                {
                    isCheckingScrollToUp = false;
                }

                #region 检测滚动
                if (scrollViewer.ScrollableHeight > 0.0d)
                {
                    movePox = 0.0d;
                }
                else
                {
                    movePox = curPt.Y - downPt.Y;
                }
                movePox = curPt.Y - downPt.Y;
                #endregion

            }
        }

        //获取子类型
        static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }

        private static List<T> GetVisualChildCollection<T>(object parent) where T : UIElement
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : UIElement
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                else if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }
    }
}
