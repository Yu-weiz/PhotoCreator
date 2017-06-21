using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Windows.Devices.Input;

namespace Yuweiz.Phone.Controls
{
    /// <summary>
    /// 分布加载滚动列表控件
    /// </summary>
    public class LongListScrollSelector : LongListSelector
    {
        public LongListScrollSelector()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            List<ScrollBar> scrollBarList = GetVisualChildCollection<ScrollBar>(this);
            foreach (ScrollBar scrollBar in scrollBarList)
            {
                if (scrollBar.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {

                }
                else
                {
                    this.MouseMove += CheckScrollToBottom;
                }
            }
        }

        /// <summary>
        /// 滚动到底部
        /// </summary>
        /// <author>庾伟荣</author>
        public event Action ScrollToBottom;

        public event Action<double, double> Scroll;

        /// <summary>
        /// 检测是否滚动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <author>庾伟荣</author>
        private void CheckScrollToBottom(object sender, System.Windows.Input.MouseEventArgs e)
        {
            List<ScrollBar> scrollBarList = GetVisualChildCollection<ScrollBar>(this);
            if (scrollBarList != null)
            {
                ScrollBar scrollBar = scrollBarList[0];
                object valueObj = scrollBar.GetValue(ScrollBar.ValueProperty);
                object maxObj = scrollBar.GetValue(ScrollBar.MaximumProperty);
                if (valueObj != null && maxObj != null)
                {
                    double value = (double)valueObj;
                    double max = (double)maxObj - 200.0d;
                                       
                    if (value >= max)
                    {
                        //读取下一页的数据
                        if (ScrollToBottom != null)
                        {
                            ScrollToBottom();
                        }
                    }

                    if (Scroll != null)
                    {
                        Scroll(value, max + 200.0d);
                    }

                }
            }
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
