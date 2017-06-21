using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPPhotoEditor.UControls;
using Yuweiz.Phone.Gestures;

namespace WPPhotoEditor.Media.Tools
{
    /// <summary>
    /// 一个工具同一时间只能作用于一个图层（LayerBase）
    /// 所以每一图层，需要有自己的画笔
    /// </summary>
    public abstract class ToolAbstract
    {
        protected GestureListener2 gestureListener2;

        /// <summary>
        /// 工具的操作图层，如果为空，则该图层没有可用工具
        /// </summary>
        public LayerBase LayerBase { get; protected set; }

        public abstract void BindLayer(LayerBase layer);

        public abstract void ReleaseLayer();
    }
}
