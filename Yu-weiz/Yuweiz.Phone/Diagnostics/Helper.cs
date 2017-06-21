using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Yuweiz.Phone.Diagnostics
{
    public class Helper
    {

        /// <summary>
        /// 获取当前系统是否中文
        /// </summary>
        public bool IsChineseCulture
        {
            get
            {
                return (System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN" || System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-HK");
            }
        }

        /// <summary>
        /// BUG收集器
        /// </summary>
        public void BugCollect(string subject, string errors)
        {
            string message = "It seems appear some errors just now!Do you want to  send it in an E-mail to us to fix them?\r\nThank you!";
            string title = "Hellp us";

            if (this.IsChineseCulture)
            {
                 message = "刚才似乎出现了一点问题，你能用邮件发送给我们进行修复吗?\r\n谢谢！";
                 title = "帮助我们";
            }

            MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Microsoft.Phone.Tasks.EmailComposeTask task = new Microsoft.Phone.Tasks.EmailComposeTask();
                task.To = "Yu-weiz@live.com";
                task.Subject = subject;
                task.Body = errors;
                task.Show();
            }
        }
    }
}
