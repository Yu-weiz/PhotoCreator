using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common
{
    /// <summary>
    /// 信息验证
    /// </summary>
    /// <creator>肖斌</creator>
    public static class RegexUtil
    {
        private static string pattern = null;

        /// <summary>
        /// 验证Short类型数字
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsSortNum(string sort)
        {
            pattern = @"^\d{1,4}$";

            bool result = Regex.IsMatch(sort, pattern);

            return result;
        }

        /// <summary>
        /// 验证密码数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsNum(string num)
        {
            pattern = @"([0-9])+";

            bool result = Regex.IsMatch(num, pattern);

            return result;
        }

        /// <summary>
        /// 验证密码字母
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsWord(string word)
        {
            pattern = @"([a-zA-Z])+";

            bool result = Regex.IsMatch(word, pattern);

            return result;
        }

        /// <summary>
        /// 验证手机号码
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsMobile(string Mobile)
        {
            pattern = @"^[1]+\d{10}$";

            bool result = Regex.IsMatch(Mobile, pattern);

            return result;
        }

        /// <summary>
        /// 验证储值卡卡号
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsCardNo(string cardNo)
        {
            pattern = @"^[1-9][0-9]{0,10}$";

            bool result = Regex.IsMatch(cardNo, pattern);

            return result;
        }

        /// <summary>
        /// 验证身份证
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsIDcard(string idCard)
        {
            pattern = @"(^\d{17}(\d|X|x)$)|(^\d{14}(\d|X|x)$)";

            bool result = Regex.IsMatch(idCard, pattern);

            return result;
        }

        /// <summary>
        /// 验证车牌号
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsCarNo(string carNo)
        {
            pattern = @"(^[\u4e00-\u9fa5]{1}[A-Za-z]{1}[A-Za-z0-9]{5}$)";

            bool result = Regex.IsMatch(carNo, pattern);

            return result;
        }

        /// <summary>
        /// 验证车量情况（车长，车宽，车重）
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsCarSituation(string carSituation)
        {
            pattern = @"^[1-9][0-9]?(\.[0-9]{1,2})?$";

            bool result = Regex.IsMatch(carSituation, pattern);

            return result;
        }

        public static bool IsCarWidth(string width)
        {
            pattern = @"^[1-9][0-9]{2,3}[Xx][1-9][0-9]{2,3}$";

            bool result = Regex.IsMatch(width, pattern);

            return result;
        }

        /// <summary>
        /// 验证会员号
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsUserId(string userId)
        {
            pattern = @"^\d{1,9}$";

            bool result = Regex.IsMatch(userId, pattern);

            return result;
        }

        /// <summary>
        /// 根据图片路径，找出图片名
        /// </summary>
        /// <param name="PicturePath"></param>
        /// <returns></returns>
        /// <author>黄海柱</author>
        public static string SelectPictureName(string PicturePath)
        {
            int index = PicturePath.LastIndexOf("/");
            string pictureName = PicturePath.Substring(index + 1);
            return pictureName;

        }

        /// <summary>
        /// 验证数字
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <author>黄海柱</author>
        public static bool IsNumber(string number)
        {
            pattern = @"^(\d+)$";

            bool result = Regex.IsMatch(number, pattern);

            return result;

        }

        /// <summary>
        /// 验证钱
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsMoney(string number)
        {
            pattern = @"^(\d+)(.)?(\d*)$";

            bool result = Regex.IsMatch(number, pattern);

            return result;

        }

        /// <summary>
        /// 验证Decimal
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsDecimal(string reg)
        {
            Decimal intResult = 0;
            return decimal.TryParse(reg, out intResult);
        }

        /// <summary>
        /// 同时验证电话号码
        /// </summary>
        /// <param name="tel"></param>
        /// <returns></returns>
        /// <author>黄海柱</author>
        public static bool IsMobileTel(string tel)
        {
            pattern = @"^\d{11,12}$";
            bool result = Regex.IsMatch(tel, pattern);

            if (!result)
            {
                string reg = @"^\d{3}-\d{8}|\d{4}-\d{7} $";

                return Regex.IsMatch(tel, reg);
            }
            return result;
        }

        /// <summary>
        /// 验证http地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            pattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            bool result = Regex.IsMatch(url, pattern);

            return result;
        }

        /// <summary>
        /// 验证广告宽高
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        /// <author>黄海柱</author>
        public static bool IsADSituation(string carSituation)
        {
            pattern = @"^[1-9][0-9]{1,5}?(\.[0-9]{1,2})?$";

            bool result = Regex.IsMatch(carSituation, pattern);

            return result;
        }

        /// <summary>
        /// 验证营业执照号
        /// </summary>
        /// <param name="businessLicenceNumber"></param>
        /// <returns></returns>
        /// <author>庾伟荣</author>
        public static bool IsBusinessLicenceNumber(string businessLicenceNumber)
        {
            pattern = @"^[0-9]{1,15}$";
            bool result = Regex.IsMatch(businessLicenceNumber, pattern);

            return result;
        }

        /// <summary>
        /// 验证Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            bool result = Regex.IsMatch(email, pattern);

            return result;
        }

        /// <summary>
        /// 验证电话或传真号码
        /// </summary>
        /// <param name="telephone"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsTelephone(string telephone)
        {
            pattern = @"^(\d{3}-|\d{4}-)?(\d{8}|\d{7})$";
            bool result = Regex.IsMatch(telephone, pattern);

            return result;
        }

        /// <summary>
        /// 验证电话号码
        /// </summary>
        /// <param name="telephone"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsTel(string telephone)
        {
            pattern = @"^\d{5,11}$";
            bool result = Regex.IsMatch(telephone, pattern);
            if (!result)
            {
                string reg = @"^\d{3}-\d{8}|\d{4}-\d{7} $";

                return Regex.IsMatch(telephone, reg);
            }
            return result;
        }

        /// <summary>
        /// 验证QQ
        /// </summary>
        /// <param name="QQ"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool isQQ(string QQ)
        {
            pattern = @"^[1-9]\d{1,10}$";
            bool result = Regex.IsMatch(QQ, pattern);

            return result;
        }

        /// <summary>
        /// 验证中文
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        /// <author>joze</author>
        public static bool IsChinese(string reg)
        {
            pattern = @"^[\u4e00-\u9fa5]$";
            char[] words = reg.ToCharArray();
            foreach (char word in words)
            {
                bool result = Regex.IsMatch(word.ToString(), pattern);
                if (!result)
                {
                    return result;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查是否版本格式的字符串
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public static bool IsVersion(string reg)
        {
            pattern = @"^\d+\.\d+\.\d+\.\d+$";
            bool result = Regex.IsMatch(reg, pattern);

            return result;
        }

        /// <summary>
        /// 验证价格
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static bool IsPrice(string reg)
        {
            pattern = @"^[0-9]*(.)?([0-9]{1,2})?$";
            bool result = Regex.IsMatch(reg, pattern);

            return result;
        }

        /// <summary>
        /// 扑捉转化INT32异常
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static int ConvertToInt32(string reg)
        {
            int intResult = 0;
            try
            {
                intResult = Convert.ToInt32(reg);
            }
            catch
            {
                intResult = 0;
            }
            return intResult;
        }

        /// <summary>
        /// 扑捉转化INT64异常
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static long ConvertToInt64(string reg)
        {
            long intResult = 0;
            try
            {
                intResult = Convert.ToInt64(reg);
            }
            catch
            {
                intResult = 0;
            }
            return intResult;
        }

        /// <summary>
        /// 扑捉转化Decimal异常
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        /// <author>肖斌</author>
        public static Decimal ConvertToDecimal(string reg)
        {
            Decimal intResult = 0;
            try
            {
                intResult = Convert.ToDecimal(reg);
            }
            catch
            {
                intResult = 0;
            }
            return intResult;
        }
        
        /// <summary>
        /// 判段是否为中文字符
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="index">要判断的字符位置索引</param>
        /// <returns></returns>
        public static bool IsChineseLetter(string input, int index)
        {
            int code = 0;
            int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chend = Convert.ToInt32("9fff", 16);
            if (input != "")
            {
                code = Char.ConvertToUtf32(input, index);    //获得字符串input中指定索引index处字符unicode编码

                if (code >= chfrom && code <= chend)
                {
                    return true;     //当code在中文范围内返回true

                }
                else
                {
                    return false;    //当code不在中文范围内返回false
                }
            }
            return false;
        }

    }
}
