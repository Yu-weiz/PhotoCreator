using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Ad
{
    public class ADServiceConfig
    {
        static ADServiceConfig()
        {
            Uid = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public const int SmaatoPublisherID = 1100019000;
              
        /// <summary>
        ///  调用例子：http://api.sketchtouch.com/ADServiceHandler.ashx?ClientName=SketchBoard&PositionId=1&RequestType=0
        /// </summary>
        public const string AdServiceHost = @"http://api.sketchtouch.com/ADServiceHandler.ashx";

        public static readonly string Uid;
    }
}
