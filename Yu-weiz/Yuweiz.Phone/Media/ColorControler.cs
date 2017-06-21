using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Yuweiz.Phone.Media
{
    public class ColorControler
    {
        /// <summary>
        /// 将十六进制字符串转换成颜色值，一般情况都应为长度为八的十六进制数值字符串，如果不足八，程序会在前面补0
        /// </summary>
        /// <returns></returns>
        public static Color ConvertFrom0x16(string value0x16)
        {
            Color color = Colors.Transparent;
            if (string.IsNullOrEmpty(value0x16))
            {
                return color;
            }
            value0x16 = value0x16.Replace("#", "");

            int lenght = value0x16.Length - 8;
            string valueStr = value0x16;
            if (lenght > 0)
            {                
                valueStr = value0x16.Substring(0, 8);
            }
            else if (lenght < 0)
            {
                value0x16.Insert(0, "000000".Substring(0, Math.Abs(lenght)));
            }

            byte a = Convert.ToByte(valueStr.Substring(0, 2), 16);
            byte r = Convert.ToByte(valueStr.Substring(2, 2), 16);
            byte g = Convert.ToByte(valueStr.Substring(4, 2), 16);
            byte b = Convert.ToByte(valueStr.Substring(6, 2), 16);
            color = Color.FromArgb(a, r, g, b);

            return color;
        }

        public static string ConvertToOx16(Color color)
        {
            string str = color.ToString().Replace("#","");
            return str;
        }
    }
}
