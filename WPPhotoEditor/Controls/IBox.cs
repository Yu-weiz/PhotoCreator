using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPPhotoEditor.Controls
{
    interface IBox
    {
        /// <summary>
        /// 更新当前的显示状态
        /// </summary>
        void ChangeVisibility();

        /// <summary>
        /// 显示
        /// </summary>
        /// <returns>如果本来就已经显示，则返回False，如果进行了由隐藏到显示的操作，则返回True</returns>
        bool Show();

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <returns>如果本来就已经显示，则返回False，如果进行了由显示到隐藏的操作，则返回True</returns>
        bool Hide();
    }
}
