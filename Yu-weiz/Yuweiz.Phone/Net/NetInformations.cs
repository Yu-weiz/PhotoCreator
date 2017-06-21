using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Net
{
    public class NetInformations
    {
        /// <summary>
        /// 线程检测网络类型:
        /// </summary>        
        public static async Task<NetworkInterfaceType> GetNetStatusType()
        {
            Task<NetworkInterfaceType> task = new Task<NetworkInterfaceType>(() =>
            {
                return Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
            });
            task.Start();
            NetworkInterfaceType faceType = await task;
            return faceType;
        }

        /// <summary>
        /// 获取当前可用的IP地址列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIPAddressList()
        {
            List<string> arrayIPAddress = new List<string>();
            // Windows.Networking.Connectivity.  
            var hostNames = Windows.Networking.Connectivity.NetworkInformation.GetHostNames();
            foreach (var hn in hostNames)
            {
                if (hn.IPInformation != null)
                {
                    string ipAddress = hn.DisplayName;
                    arrayIPAddress.Add(ipAddress);
                }
            }

            return arrayIPAddress;
        }

        public static string GetTopIPAddress()
        {
            string strIPAddress = null;
            List<string> arrayIPAddress = GetIPAddressList();

            if (arrayIPAddress.Count == 1)
            {
                strIPAddress = arrayIPAddress[0];
            }
            if (arrayIPAddress.Count > 1)
            {
                strIPAddress = arrayIPAddress[arrayIPAddress.Count - 1];
                foreach (string str in arrayIPAddress)
                {
                    if (str.Substring(0, 7) == "192.168")
                    {
                        strIPAddress = str;
                        break;
                    }
                }
            }

            return strIPAddress;
        }
    }
}
