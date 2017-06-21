using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuweiz.Phone.Common.OperationHistory;

namespace WPPhotoEditor.ViewModels
{
    public class PEOperationType : OperationType
    {
        protected PEOperationType(uint index)
            : base(index)
        {
            base.Index = index;
        }

        ///// <summary>
        ///// OperationType 不能为空
        ///// </summary>
        ///// <param name="operationType"></param>
        //public PEOperationType(OperationType operationType)
        //    : base(operationType.Index)
        //{

        //}

        public static readonly PEOperationType Mask = new PEOperationType(4);

        public static readonly PEOperationType Draw = new PEOperationType(8);
    }
}
