using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common.InnerNavigation
{
    public interface IInnerNavigation
    {
        /// <summary>
        /// 导航到此页时页面执行的函数
        /// </summary>
        /// <param name="parameter">导航到此页的参数</param>
        void NavigateTo(object parameter);

        /// <summary>
        /// 离开此页时页面执行的函数
        /// </summary>
        void NavigateFrom();
    }
}
