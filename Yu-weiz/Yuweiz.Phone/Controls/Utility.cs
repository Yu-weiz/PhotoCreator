using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yuweiz.Phone.Controls
{
    public class Utility
    {
        /// <summary>
        /// 判断control1中的controlElem1是否以碰到control2的controlElem2 demo:
        /// </summary>
        /// <param name="control1"></param>
        /// <param name="controlElem1"></param>
        /// <param name="control2"></param>
        /// <param name="controlElem2"></param>
        /// <returns></returns>
        public static bool CheckCollision(FrameworkElement control1, FrameworkElement controlElem1, FrameworkElement control2, FrameworkElement controlElem2)
        {
            // first see if sprite rectangles collide
            Rect rect1 = UserControlBounds(control1);
            Rect rect2 = UserControlBounds(control2);


            rect1.Intersect(rect2);
            if (rect1 == Rect.Empty)
            {
                // no collision - GET OUT!
                return false;
            }
            else
            {
                bool bCollision = false;
                Point ptCheck = new Point();

                // now we do a more accurate pixel hit test
                for (int x = Convert.ToInt32(rect1.X); x < Convert.ToInt32(rect1.X + rect1.Width); x++)
                {
                    for (int y = Convert.ToInt32(rect1.Y); y < Convert.ToInt32(rect1.Y + rect1.Height); y++)
                    {
                        ptCheck.X = x;
                        ptCheck.Y = y;

                        List<UIElement> hits = System.Windows.Media.VisualTreeHelper.FindElementsInHostCoordinates(ptCheck, control1) as List<UIElement>;
                        if (hits.Contains(controlElem1))
                        {
                            // we have a hit on the first control elem, now see if the second elem has a similar hit
                            List<UIElement> hits2 = System.Windows.Media.VisualTreeHelper.FindElementsInHostCoordinates(ptCheck, control2) as List<UIElement>;
                            if (hits2.Contains(controlElem2))
                            {
                                bCollision = true;
                                break;
                            }
                        }
                    }
                    if (bCollision) break;
                }
                return bCollision;
            }


        }

        /// <summary>
        ///  判断Cavans中的elem1是否以碰到elem2:
        /// </summary>
        /// <param name="elem1"></param>
        /// <param name="elem2"></param>
        /// <returns></returns>
        public static bool CheckCollision(FrameworkElement elem1, FrameworkElement elem2)
        {
            // first see if sprite rectangles collide
            Rect rect1 = UserControlBounds(elem1);
            Rect rect2 = UserControlBounds(elem2);

            rect1.Intersect(rect2);
            if (rect1 == Rect.Empty)
            {
                // no collision - GET OUT!
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Rect UserControlBounds(FrameworkElement control)
        {
            Point ptTopLeft = new Point(Convert.ToDouble(control.GetValue(Canvas.LeftProperty)), Convert.ToDouble(control.GetValue(Canvas.TopProperty)));
            Point ptBottomRight = new Point(Convert.ToDouble(control.GetValue(Canvas.LeftProperty)) + control.Width, Convert.ToDouble(control.GetValue(Canvas.TopProperty)) + control.Height);

            return new Rect(ptTopLeft, ptBottomRight);
        }

        public static int GetWordsHeight(double frontSize)
        {
            return 0;
        }

        public static int GetWordsWeight(double frontSize, string words)
        {
            return 0;
        }


        /// <summary>
        /// 获取控件同的显示控件集
        /// </summary>
        /// <typeparam name="T">要获取的元素类型</typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static List<T> GetVisualChildCollection<T>(object parent) where T : UIElement
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
