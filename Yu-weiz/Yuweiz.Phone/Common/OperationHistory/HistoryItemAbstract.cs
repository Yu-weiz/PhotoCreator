using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Common.OperationHistory
{
    public abstract class HistoryItemAbstract
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public OperationType OperationType { get; set; }

      //  public abstract HistoryItemAbstract Clone();

    }
}
