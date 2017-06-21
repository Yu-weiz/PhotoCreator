using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Ad.Model
{
    public class AdUltimateModel
    {
        public AdUltimateModel()
        {
            this.DefaultAdFromEnum = AdSourceTypeEnum.Original;
        }
        
        /// <summary>
        /// 应用客户端名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 应用中的广告位
        /// </summary>
        public string PositionId { get; set; }

        /// <summary>
        /// 当前要显示的广告来源
        /// </summary>
        public AdSourceTypeEnum DefaultAdFromEnum { get; set; }

        public int SmaatoAdSpace { get; set; }

        public bool ShowSmaatoError { get; set; }

        public string AdduplexAppId { get; set; }

        public string InMobiAppId { get; set; }

        public string GoogleAdMobAdUnitID { get; set; }

        public string PubCenterAppID { get; set; }

        public string PubCenterADUnitID { get; set; }

        /// <summary>
        /// 默认广告,当断网的情况下显示的默认广告
        /// </summary>
        public AdCustomModelAbstract OriginalAd { get; set; }

        public bool ShowOriginalAdFirst { get; set; }

        public AdUltimateModel Clone()
        {
            AdUltimateModel model = this.MemberwiseClone() as AdUltimateModel;
            return model;
        }
    }
}
