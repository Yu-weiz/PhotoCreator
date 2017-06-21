using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuweiz.Phone.Common.FreeJson
{
    public class JsonTemplate
    {
        private JsonTemplate()
        {

        }

        private static JsonTemplate instance;

        public static JsonTemplate Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new JsonTemplate();
                }

                return instance;
            }

        }

        /// <summary>
        /// 生成键值对的Json
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>返回生成的Json字符串</returns>
        public string CreateKeyValueJson(JsonTemplateParameter[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
            {
                return "{}";
            }

            StringBuilder jsonStr = new StringBuilder(@"{");

            for (int i = 0; i < parameters.Length; i++)
            {
                string lineStr = @"""" + parameters[i].Key + @"""";
                lineStr += ":";
                lineStr += @"""" + parameters[i].Value + @"""";
                jsonStr.Append(lineStr);

                if (i < parameters.Length - 1)
                {
                    jsonStr.Append(",");
                }
            }

            jsonStr.Append(@"}");

            return jsonStr.ToString();
        }

        /// <summary>
        /// 生成键值对的Json
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>返回生成的Json字符串</returns>
        public string CreateKeyValueJson(List<JsonTemplateParameter> parameters)
        {
            if (parameters == null || parameters.Count < 1)
            {
                return "{}";
            }

            StringBuilder jsonStr = new StringBuilder(@"{");

            for (int i = 0; i < parameters.Count; i++)
            {
                string lineStr = @"""" + parameters[i].Key + @"""";
                lineStr += ":";
                lineStr += @"""" + parameters[i].Value + @"""";
                jsonStr.Append(lineStr);

                if (i < parameters.Count - 1)
                {
                    jsonStr.Append(",");
                }
            }

            jsonStr.Append(@"}");

            return jsonStr.ToString();
        }
    }
}
