using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common.OperationHistory
{
    /// <summary>
    /// 继承者以2的n次方来赋值，父类以使用1,2!
    /// </summary>
    public class OperationType
    {
        /// <summary>
        /// 表示，操作为Undo,即可通过重做呈现出来
        /// </summary>
        public static readonly OperationType Undo = new OperationType(1);

        /// <summary>
        /// 表示，操作为Redo,即可通过撤消呈现出来
        /// </summary>
        public static readonly OperationType Redo = new OperationType(2);
             
        protected OperationType(uint index)
        {
            this.Index = index;
        }

        protected uint Index { get; set; }

        /// <summary>
        /// 重载==操作符
        /// </summary>
        /// <param name="p1">Point对象实例</param>
        /// <param name="p2">Point对象实例</</param>
        /// <returns>返回类型：bool; true/p1==p2 ; false/p1!=p2</returns>
        public static bool operator ==(OperationType type1, OperationType type2)
        {
            return type1.Index == type2.Index;
        }

        /// <summary>
        /// 重载!=操作符
        /// </summary>
        /// <param name="p1">Point对象实例</param>
        /// <param name="p2">Point对象实例</</param>
        /// <returns>返回类型：bool; true/p1!=p2 ; false/p1==p2</returns>
        public static bool operator !=(OperationType type1, OperationType type2)
        {
            return type1.Index != type2.Index;
        }

        /// <summary>
        /// 重载!=操作符
        /// </summary>
        /// <param name="p1">Point对象实例</param>
        /// <param name="p2">Point对象实例</</param>
        /// <returns>返回类型：bool; true/p1!=p2 ; false/p1==p2</returns>
        public static OperationType operator |(OperationType type1, OperationType type2)
        {
            uint result = type1.Index | type2.Index;
            return new OperationType(result);
        }


        /// <summary>
        /// 重载!=操作符
        /// </summary>
        /// <param name="p1">Point对象实例</param>
        /// <param name="p2">Point对象实例</</param>
        /// <returns>返回类型：bool; true/p1!=p2 ; false/p1==p2</returns>
        public static OperationType operator &(OperationType type1, OperationType type2)
        {
            uint result = type1.Index & type2.Index;
            return new OperationType(result);
        }

        /// <summary>
        /// 重载!=操作符
        /// </summary>
        /// <param name="p1">Point对象实例</param>
        /// <param name="p2">Point对象实例</</param>
        /// <returns>返回类型：bool; true/p1!=p2 ; false/p1==p2</returns>
        public static OperationType operator ~(OperationType type1)
        {
            uint result = ~type1.Index;
            return new OperationType(result);
        }

        /// <summary>
        /// 重写方法，必须有,后面的111是乱加的，你也可以写其它的
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 重写方法，必须有
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

    }
}
