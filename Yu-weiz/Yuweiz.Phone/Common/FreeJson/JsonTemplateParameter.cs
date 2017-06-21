using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuweiz.Phone.Common.FreeJson
{
   /// <summary>
   /// 键值参数
   /// </summary>
    public class JsonTemplateParameter
    {
        public JsonTemplateParameter()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">键名称</param>
        /// <param name="value">键值</param>
        public JsonTemplateParameter(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
