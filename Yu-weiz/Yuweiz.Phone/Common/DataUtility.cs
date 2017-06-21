using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common
{
    public static class DataUtility
    {
        public static string ConvertRelativeTimeConverter(DateTime dt, bool isChinese = true)
        {
            string dTime = dt.ToString("M/d H:m:s yyyy");
            if (DateTime.Compare(dt.AddYears(1), DateTime.Now) > 0 && DateTime.Compare(dt.AddMonths(1), DateTime.Now) < 0)
            {
                dTime = dt.ToString("M/d H:m:s");
            }
            else if (DateTime.Compare(dt.AddYears(3), DateTime.Now) > 0 && (DateTime.Compare(dt.AddYears(1), DateTime.Now) < 0))
            {
                var m = DateTime.Now - dt;
                dTime = m.Days.ToString() + (isChinese ? "" : "");
            }

            return string.Empty;
        }

        public static int ConvertToIntWithDefault(string data)
        {
            int value = 0;
            int.TryParse(data,out value);
            return value;
        }

        public static T2 ShallowCopy<T1, T2>(T1 father, T2 child)
            where T1 : new()
            where T2 : T1
        {
            System.Type childT = typeof(T2);


            System.Type t = typeof(T1);
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                object value = property.GetValue(father, null);

                var t2Pro = childT.GetProperty(property.Name);
                t2Pro.SetValue(child, value);
            }

            return child;
        }

        #region Reflection

        /// <summary>
        /// 使用反射复制
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T2 CopyTo<T1, T2>(T1 source)
        {

            if (source == null)
            {
                return default(T2);
            }

            #region CopyTo2

            var targetType = typeof(T2);
            T2 target = default(T2);
            target = (T2)targetType.Assembly.CreateInstance(targetType.FullName);
            CopyTo(source, target);

            #endregion

            return target;

        }

        /// <summary>
        /// 复制到指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>返回目标类型实体</returns>
        public static T To<T>(this object obj)
        {

            if (obj == null)
            {
                return default(T);
            }

            #region CopyTo2

            var targetType = typeof(T);
            T target = default(T);
            obj.CopyTo<T>(target);

            #endregion

            return target;

        }

        /// <summary>
        /// 克隆一个对象出来,不用能用于构造函数存在方法的对象模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this object source)
        {
            if (source == null)
            {
                return default(T);
            }

            #region CopyTo2

            var targetType = typeof(T);
            T target = default(T);
            target = (T)targetType.Assembly.CreateInstance(targetType.FullName);
            CopyTo(source, target);

            #endregion

            return target;
        }

        /// <summary>
        /// 使用反射复制：主方法
        /// </summary>    
        /// <returns></returns>
        public static void CopyTo<T>(this object source, T target)
        {
            if (source == null)
            {
                return;
            }

            var targetType = typeof(T);
            if (target == null)
            {
                target = (T)targetType.Assembly.CreateInstance(targetType.FullName);
            }

            if (target == null)
            {
                throw new ApplicationException("target 未实例化！");
            }

            #region 属性

            foreach (PropertyInfo info in target.GetType().GetProperties())
            {
                try
                {
                    var sourceProperty = source.GetType().GetProperty(info.Name);
                    if (sourceProperty == null)
                    {
                        continue;
                    }
                    object obj2 = sourceProperty.GetValue(source, null);
                    if (obj2 == null)
                    {
                        continue;
                    }

                    var sourceProType = obj2.GetType();

                    if (info.PropertyType.IsGenericType)
                    {
                        #region 特殊处理 枚举/布尔

                        if (info.PropertyType.GetGenericArguments()[0].IsEnum)
                        {
                            //枚举转换                              
                            int enumValue = 0;
                            if ((obj2 != null) && !sourceProType.IsEnum)
                            {
                                if (int.TryParse(obj2.ToString(), out enumValue))
                                {
                                    info.SetValue(target, Enum.ToObject(info.PropertyType.GetGenericArguments()[0], enumValue), null);
                                }
                            }
                            else
                            {
                                enumValue = (int)obj2;
                                info.SetValue(target, Enum.ToObject(info.PropertyType.GetGenericArguments()[0], enumValue), null);
                            }
                            continue;
                        }
                        else if (info.PropertyType.GetGenericArguments()[0].Name == "Boolean")
                        {
                            var stName = sourceProType.Name;
                            if (sourceProType.IsGenericType)
                            {
                                stName = sourceProType.GetGenericArguments()[0].Name;
                            }

                            if (stName == info.PropertyType.GetGenericArguments()[0].Name)
                            {
                                targetType.InvokeMember(info.Name, BindingFlags.SetProperty, null, target, new object[] { obj2 });
                                continue;
                            }

                            int bValue = 0;
                            int.TryParse(obj2.ToString(), out bValue);
                            targetType.InvokeMember(info.Name, BindingFlags.SetProperty, null, target, new object[] { bValue == 0 ? false : true });

                            continue;
                        }

                        #endregion
                    }

                    if (((!info.PropertyType.IsGenericType || !info.PropertyType.GetGenericArguments()[0].IsEnum) && !info.PropertyType.IsArray) && ((!info.PropertyType.IsGenericType || (info.PropertyType.GetInterface("System.Collections.IEnumerable",false) == null)) && (source.GetType().GetProperty(info.Name) != null)))
                    {
                        //列表不可复制                          
                        if (!sourceProType.IsEnum || info.PropertyType == sourceProType)
                        {
                            info.SetValue(target, obj2, null);
                        }
                        #region 枚举
                        else if (info.PropertyType.Name == "String")
                        {
                            targetType.InvokeMember(info.Name, BindingFlags.SetProperty, null, target, new object[] { ((int)obj2).ToString() });
                        }
                        else
                        {
                            targetType.InvokeMember(info.Name, BindingFlags.SetProperty, null, target, new object[] { (int)obj2 });
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {

                }
            }

            #endregion

            #region 字段

            foreach (FieldInfo info2 in target.GetType().GetFields())
            {
                var sourceField = source.GetType().GetField(info2.Name);
                if (sourceField != null && info2.FieldType == sourceField.FieldType)
                {
                    info2.SetValue(target, sourceField.GetValue(source));
                }
            }

            #endregion
        }

        #endregion
    }
}

namespace System.Linq
{
    public static class Queryable
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
