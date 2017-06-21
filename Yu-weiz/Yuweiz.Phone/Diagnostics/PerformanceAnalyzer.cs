using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Diagnostics
{
    public class PerformanceAnalyzer
    {
        public static void ShowMemory()
        {
            long deviceTotalMemory = Microsoft.Phone.Info.DeviceStatus.DeviceTotalMemory / 1024 / 1024;
            long applicationMemoryUsageLimit = Microsoft.Phone.Info.DeviceStatus.ApplicationMemoryUsageLimit / 1024 / 1024;
            long applicationCurrentMemoryUsage = Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage / 1024 / 1024;
            long applicationPeakMemoryUsage = Microsoft.Phone.Info.DeviceStatus.ApplicationPeakMemoryUsage / 1024 / 1024;

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString());
            System.Diagnostics.Debug.WriteLine("设置总内存:M ", deviceTotalMemory.ToString());
            System.Diagnostics.Debug.WriteLine("应用可用内存:M " + applicationMemoryUsageLimit.ToString());
            System.Diagnostics.Debug.WriteLine("当前内存使用:M " + applicationCurrentMemoryUsage.ToString());
            System.Diagnostics.Debug.WriteLine("高峰内存使用:M " + applicationPeakMemoryUsage.ToString());
        }

        /// <summary>
        /// 检查是否存在足够内存
        /// </summary>
        /// <param name="remainMemoryM">保留的内存M</param>
        /// <returns>足够：ture</returns>
        public static bool CheckMemoryAllow(int remainMemoryM)
        {
            long applicationMemoryUsageLimit = Microsoft.Phone.Info.DeviceStatus.ApplicationMemoryUsageLimit / 1024 / 1024;
            long applicationCurrentMemoryUsage = Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage / 1024 / 1024;

            if (applicationCurrentMemoryUsage <= applicationMemoryUsageLimit - remainMemoryM)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
