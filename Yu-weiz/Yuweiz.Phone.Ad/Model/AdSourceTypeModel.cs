using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuweiz.Phone.Ad.Model
{
    public class AdSourceTypeModel
    {
        public AdSourceTypeModel()
        {
            this.BDSourceType = AdSourceTypeEnum.Default;
            this.AdInterval = 60000;        
        }

        public AdSourceTypeModel(AdSourceTypeDataModel dataModel)
        {
            if (dataModel == null)
            {
                this.BDSourceType = AdSourceTypeEnum.Default;
                this.AdInterval = 60000;        
                return;
            }

            this.BDSourceType = this.ConvertFrom(dataModel.BDSourceType);

            int adInterval = 0;
            int.TryParse(dataModel.AdInterval, out adInterval);
            this.AdInterval = adInterval;
        }

        public AdSourceTypeEnum BDSourceType { get; set; }

        public int AdInterval { get; set; }


        private AdSourceTypeEnum ConvertFrom(string adFromEnumValue)
        {
            AdSourceTypeEnum adFromEnum = AdSourceTypeEnum.Default;
            if (string.IsNullOrEmpty(adFromEnumValue))
            {
                return adFromEnum;
            }

            switch (adFromEnumValue)
            {
                case "1": adFromEnum = AdSourceTypeEnum.Smaato; break;
                case "2": adFromEnum = AdSourceTypeEnum.Pubcenter; break;
                case "3": adFromEnum = AdSourceTypeEnum.Adduplex; break;
                case "4": adFromEnum = AdSourceTypeEnum.GoogleAdMob; break;
                case "10": adFromEnum = AdSourceTypeEnum.Custom; break;
                case "10000000": adFromEnum = AdSourceTypeEnum.Original; break;
                case "10000001": adFromEnum = AdSourceTypeEnum.None; break;
            }

            return adFromEnum;
        }
    }
}
