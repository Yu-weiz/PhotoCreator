using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common.OperationHistory
{
    public class HistoryManager
    {
        public HistoryManager()
        {
            this.historyItemList = new List<HistoryItemAbstract>();
        }

        private List<HistoryItemAbstract> historyItemList;

        private bool isFirstUndo = true;

        private HistoryItemAbstract recentUndoHistoryItem;

        private const int HistoryItemCountLimit = 5;

        public int CanUndoCount { get; set; }

        public int CanRedoCount { get; set; }

        /// <summary>
        /// 调用此函数实现将当前的状态保存为重做项    
        /// </summary>
        public Action SaveCurHistoryItemAction;

        /// <summary>
        /// 记录历史记录
        /// 操作前，调用
        /// </summary>
        /// <param name="item"></param>
        public void AddHistory(HistoryItemAbstract item)
        {
            if ((item.OperationType & OperationType.Undo) != OperationType.Undo)
            {
                isFirstUndo = true;
            }
            this.historyItemList.Add(item);

            if (this.historyItemList.Count > HistoryItemCountLimit)
            {
                this.historyItemList.RemoveAt(0);
            }
        }

        /// <summary>
        /// 撤销当前操作
        /// </summary>
        /// <param name="item">当前的状态操作,已转换OperationType</param>
        public HistoryItemAbstract Undo()
        {
            #region 第一次调用撤消，应先将当前的状态保存为重做项
            if (isFirstUndo && SaveCurHistoryItemAction != null)
            {
                //foreach (HistoryItemAbstract historyItemAbstract in this.historyItemList)
                //{
                //    if ((historyItemAbstract.OperationType & OperationType.Undo) == OperationType.Undo)
                //    {
                //        historyItemAbstract.OperationType = historyItemAbstract.OperationType & ~OperationType.Undo | OperationType.Redo;
                //    }

                //}


                this.SaveCurHistoryItemAction();

            }
            isFirstUndo = false;
            #endregion

            IEnumerable<HistoryItemAbstract> iItems = GetCanUndoOperationItems();

            if (iItems == null)
            {
                return null;
            }

            int count = iItems.Count();
            if (count < 1)
            {
                return null;
            }

            HistoryItemAbstract operationItem = iItems.ElementAt(count - 1);
            for (int i = 1; i < count + 1; i++)
            {
                HistoryItemAbstract item = iItems.ElementAt(count - i);
                if (this.recentUndoHistoryItem != item)
                {
                    operationItem = item;
                    this.historyItemList.Remove(operationItem);
                    operationItem.OperationType = operationItem.OperationType & ~OperationType.Redo | OperationType.Undo;
                    this.historyItemList.Add(operationItem);

                    this.recentUndoHistoryItem = item;
                    break;
                }
                else
                {
                    item.OperationType = item.OperationType & ~OperationType.Redo | OperationType.Undo;
                }
            }

            this.CanRedoCount++;
            this.CanUndoCount = count-1;

            return operationItem;
        }

        /// <summary>
        /// 撤销当前操作
        /// </summary>
        /// <param name="item">当前的状态操作</param>
        public HistoryItemAbstract Redo()
        {
            isFirstUndo = false;

            IEnumerable<HistoryItemAbstract> iItems = GetRevokedOperationItems();

            if (iItems == null)
            {
                return null;
            }

            int count = iItems.Count();
            if (count < 1)
            {
                return null;
            }

            HistoryItemAbstract operationItem = null;
            for (int i = 1; i < count + 1; i++)
            {
                HistoryItemAbstract item = iItems.ElementAt(count - i);
                if (this.recentUndoHistoryItem != item)
                {
                    operationItem = item;
                    this.historyItemList.Remove(operationItem);
                    operationItem.OperationType = operationItem.OperationType & ~OperationType.Undo | OperationType.Redo;
                    this.historyItemList.Add(operationItem);

                    this.recentUndoHistoryItem = item;
                    break;
                }
                else
                {
                    item.OperationType = item.OperationType & ~OperationType.Undo | OperationType.Redo;
                }
            }

            this.CanRedoCount = count-1;
            this.CanUndoCount++;

            return operationItem;
        }

        public void Clear()
        {
            this.historyItemList.Clear();
        }

        /// <summary>
        /// 获取状dd
        /// </summary>
        /// <returns></returns>
        private IEnumerable<HistoryItemAbstract> GetRevokedOperationItems()
        {
            IEnumerable<HistoryItemAbstract> iItems = this.historyItemList.Where((item) => ((item.OperationType & OperationType.Undo) == OperationType.Undo));
            return iItems;
        }

        private IEnumerable<HistoryItemAbstract> GetCanUndoOperationItems()
        {
            IEnumerable<HistoryItemAbstract> iItems = this.historyItemList.Where((item) => ((item.OperationType & OperationType.Undo) != OperationType.Undo));
            return iItems;
        }
    }
}
