using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.IO
{
    public class IsolatedStorageSettingsDAL
    {
        static IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        /// 保存设置状态
        /// </summary>
        /// <param name="penModel"></param>
        public static void SaveSettingsModel<T>(T model)
        {
            System.Type t = typeof(T);
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                object value = property.GetValue(model, null);
               
                if (settings.Contains(property.Name))
                {
                    settings[property.Name] = value;
                }
                else
                {
                    settings.Add(property.Name, value);
                }
            }
        }

        /// <summary>
        /// 载入设置状态
        /// </summary>
        /// <returns></returns>
        public static T OpenSettingsModel<T>() where T : new()
        {
            T model = new T();
            System.Type t = typeof(T);
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                if (settings.Contains(property.Name))
                {
                    object value = settings[property.Name];                  
                    if (value != null)
                    {
                        property.SetValue(model, value, null);
                    }
                }

            }
            return model;
        }
    }
}
