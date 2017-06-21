using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common.FreeJson
{
    /// <summary>
    /// JSON工具类
    /// 字符串实体转换类
    /// </summary>
    /// <creator>庾伟荣</creator>
    public static class JsonUtility
    {
        public static T Parse<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        /// <summary>  
        /// 将对象序列化为Json字符串。  
        /// </summary>  
        /// <param name="app">对象</param>  
        /// <returns>Json字符串</returns>  
        public static string ToJson<T>(T t)
        {
            var serializer = new DataContractJsonSerializer(t.GetType());
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, t);
                var array = ms.ToArray();
                return Encoding.UTF8.GetString(array, 0, array.Length);
            }
        }

        /// <summary>
        /// 将Json字符串反序列化为对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonString">字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static T FromJson<T>(this string jsonString)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T jsonObject = (T)ser.ReadObject(ms);
                return jsonObject;
            }
        }

        public static Object Json2Obj(String json, Type t)
        {
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(t);
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    return serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }
        
        public static string Stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                byte[] byteArray = ms.ToArray();
                return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            }
        }
    }
}
