using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Yuweiz.Phone.Gestures
{
    /// <summary>
    /// 手势工厂：
    /// 根据枚举获取手势对象：GestureAbstract
    /// 单例模式：仅有一个生成检测手势的工厂
    /// </summary>
    internal class GestureFactory
    {
        private GestureFactory()
        { }

        public static readonly GestureFactory GestureFactoryInstance = new GestureFactory();

        /// <summary>
        /// 当前需监听的手势枚举列表
        /// ：可用于对比要监听的手势集合是否更改
        /// </summary>
        private List<GestureTypeEnum> _gestureTypeList;

        /// <summary>
        /// 当前需监听的手势实体列表
        /// ：由GetGestureAbstractList初始化
        /// </summary>
        public List<GestureAbstract> GestureAbstractList
        {
            get;
            private set;
        }

        /// <summary>
        /// 实例化当前需监听的手势实体（列表）：通过手势枚举列表
        /// </summary>
        /// <param name="gestureTypeEnumList">手势枚举列表</param>
        /// <returns></returns>
        public List<GestureAbstract> GetGestureAbstractList(List<GestureTypeEnum> gestureTypeEnumList)
        {
            if (gestureTypeEnumList == null || gestureTypeEnumList.Count < 1)
            {
                return null;
            }

            if (_gestureTypeList != gestureTypeEnumList)
            {
                GestureAbstractList = new List<GestureAbstract>();
                foreach (GestureTypeEnum gestureType in gestureTypeEnumList)
                {
                    #region  GestureAbstract子类为非单例：公共的构造函数
                    GestureAbstract gesture = (GestureAbstract)Assembly.Load("Yuweiz.Phone").CreateInstance("Yuweiz.Phone.Gestures." + gestureType.ToString() + "Gesture");
                    if (gesture != null)
                    {
                        GestureAbstractList.Add(gesture);
                    }
                    #endregion                  
                }
            }

            _gestureTypeList = gestureTypeEnumList;
            return GestureAbstractList;
        }

    }
}
