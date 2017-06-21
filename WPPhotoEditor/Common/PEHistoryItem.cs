using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPPhotoEditor.Media;
using WPPhotoEditor.UControls;
using Yuweiz.Phone.Common.OperationHistory;

namespace WPPhotoEditor.Common
{
    public class PEHistoryItem : HistoryItemAbstract
    {
        /// <summary>
        /// 操作相关的图像
        /// </summary>
        public WriteableBitmap Wbmp { get; set; }
           
        public LayerBase LayerBase { get; set; }

        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <returns></returns>
        public PEHistoryItem Clone()
        {
            PEHistoryItem item = new PEHistoryItem();
            item.Wbmp = new WriteableBitmap(this.Wbmp);          
            item.LayerBase = LayerBase;
            item.OperationType = base.OperationType;

            return item;
        }
    }
}
