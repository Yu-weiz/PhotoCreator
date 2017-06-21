using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Common
{
    public class AppInfo
    {
        /// <summary>
        ///  获取程序集版本
        /// </summary>
        /// <returns></returns>
        public static string GetAppVersion()
        {
            Version version = new System.Reflection.AssemblyName(
            System.Reflection.Assembly.GetExecutingAssembly().FullName).Version;
            string versionStr = version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Revision + "." + version.Build;
            return versionStr;
        }
    }
}
