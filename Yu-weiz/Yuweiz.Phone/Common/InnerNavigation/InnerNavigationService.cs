using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Yuweiz.Phone.Common.InnerNavigation
{
    /// <summary>
    /// 使用前必须要给RootFrame赋非空值
    /// </summary>
    public static class InnerNavigationService
    {
        private static PhoneApplicationFrame rootFrame;

        /// <summary>
        /// 使用前必须要给RootFrame赋非空值
        /// </summary>
        public static PhoneApplicationFrame RootFrame
        {
            get
            {
                if (InnerNavigationService.rootFrame == null)
                {
                    InnerNavigationService.rootFrame = new PhoneApplicationFrame();
                }
                return InnerNavigationService.rootFrame; 
            }
            set { InnerNavigationService.rootFrame = value; }
        }

        public static readonly List<Type> ExceptControlsTypeList = new List<Type>();
       
        public static T NavigateTo<T>(this UserControl page, bool isTop = true, bool canGoBack = true) where T : Control
        {
            object obj = page.NavigateTo<T>(null, isTop, canGoBack);
            return obj as T;
        }

        public static T NavigateTo<T>(this UserControl page, object parameter, bool isTop = true, bool canGoBack = true) where T : Control
        {
            object obj = Activator.CreateInstance<T>();//根据类型创建实例
            NavigateTo(page, obj, parameter, isTop, canGoBack);
            return obj as T;
        }

        public static void GoBack(this UserControl userControl)
        {
            try
            {
                RootFrame.BackKeyPress -= RootFrame_BackKeyPress;

                Panel panel = userControl.Parent as Panel;
                if (panel == null)
                {
                    // return;
                }

                IInnerNavigation iInnerNavigation = userControl as IInnerNavigation;
                if (iInnerNavigation != null)
                {
                    iInnerNavigation.NavigateFrom();
                }
                panel.Children.Remove(userControl);
            }
            catch (Exception err)
            {
#if DEBUG
              //  throw err;
#endif
            }
        }


        public static void NavigateTo(this UserControl page, Type userControlType, object parameter, bool isTop = true)
        {
            object obj = Activator.CreateInstance(userControlType);//根据类型创建实例
            NavigateTo(page, obj, parameter, isTop);
        }

        public static void NavigateTo(this UserControl page, Control userControl, object parameter, bool isTop = true)
        {
            NavigateTo(page, userControl, parameter, isTop);
        }


        static void NavigateTo(UserControl page, object obj, object parameter, bool isTop = true, bool canGoBack = true)
        {
            Panel panel = page.Content as Panel;
            if (panel == null)
            {
                return;
            }

            Control userControl = obj as Control;
            if (userControl == null)
            {
#if DEBUG
                throw new Exception("userControlType 生成的对象为非控件对象");
#else
                return;
#endif
            }

            //上一导航记录
            userControl.Tag = new InnerNavigationInfo() { IsTop = isTop, CanGoBack = canGoBack };

            #region 添加到可视化树
            if (isTop)
            {
                panel.Children.Add(userControl);
            }
            else
            {
                int index = CheckExistAppBar(panel);
                if (index > -1)
                {
                    panel.Children.Insert(index, userControl);
                }
                else
                {
                    panel.Children.Add(userControl);
                }
            }
            #endregion

            #region 如果继承了导航接口，则执行

            IInnerNavigation iInnerNavigation = obj as IInnerNavigation;
            if (iInnerNavigation != null)
            {
                iInnerNavigation.NavigateTo(parameter);
            }

            #endregion

            RootFrame.BackKeyPress += RootFrame_BackKeyPress;
        }

        static void RootFrame_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Page page = RootFrame.Content as Page;
                UserControl obj = page.GetCurrrentInnerPage();
                if (obj != null)
                {
                    InnerNavigationInfo info = obj.Tag as InnerNavigationInfo;
                    if (info.CanGoBack)
                    {
                        e.Cancel = true;
                        obj.GoBack();
                    }
                }
            }
            catch (Exception err)
            {
#if DEBUG
                throw err;
#endif
            }

        }

        static int CheckExistAppBar(Panel panel)
        {
            if (ExceptControlsTypeList == null || ExceptControlsTypeList.Count < 1)
            {
                return -1;
            }

            int index = -1;
            for (int i = panel.Children.Count - 1; i > -1; i--)
            {
                foreach (var t in ExceptControlsTypeList)
                {
                    if (panel.Children[i].GetType()==t)
                    {
                        index = i;
                        break;
                    }
                }
                
            }
            return index;
        }

        static UserControl GetCurrrentInnerPage(this UserControl page)
        {
            Stack<UserControl> innerPageStack = new Stack<UserControl>();
            page.GetCurrrentInnerPage(innerPageStack);
            if (innerPageStack.Count < 1)
            {
                return null;
            }
            UserControl obj = innerPageStack.Pop();
            return obj;
        }

        static void GetCurrrentInnerPage(this UserControl page, Stack<UserControl> innerPageStack)
        {
            if (innerPageStack == null)
            {
                innerPageStack = new Stack<UserControl>();
            }

            Panel panel = page.Content as Panel;
            if (panel == null)
            {
                // return;
            }

            foreach (var item in panel.Children)
            {
                UserControl control = item as UserControl;
                if (control != null && control.Tag is InnerNavigationInfo)
                {
                    innerPageStack.Push(control);
                    control.GetCurrrentInnerPage(innerPageStack);
                    break;
                }
            }
        }
    }
}
