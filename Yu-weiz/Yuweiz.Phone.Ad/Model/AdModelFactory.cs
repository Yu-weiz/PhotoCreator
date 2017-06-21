using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Ad.Model
{
    public class AdModelFactory
    {
        private AdModelFactory()
        { }

        private static AdModelFactory intsance;

        public static AdModelFactory Intsance
        {
            get
            {
                if (intsance == null)
                {
                    intsance = new AdModelFactory();
                }
                return AdModelFactory.intsance;
            }           
        }


        public AdCustomModelAbstract CreateAdCustomModel(AdDataModel dataMoel)
        {
            AdCustomModelAbstract adCustomModel = CreateImageAdCustomModel(dataMoel);
            if (adCustomModel == null)
            {
                adCustomModel = CreateTextAdCustomModel(dataMoel);
            }

            return adCustomModel;
        }

        private TextAdCustomModel CreateTextAdCustomModel(AdDataModel dataMoel)
        {
            TextAdCustomModel textModel = null;
            if (dataMoel != null && !string.IsNullOrEmpty(dataMoel.AdWords))
            {
                textModel = new TextAdCustomModel();
                textModel.AdWords = dataMoel.AdWords;
                textModel.TargetUrl = dataMoel.AdUrl;

                if (!string.IsNullOrEmpty(dataMoel.BackgroundColor0x16))
                {
                    textModel.BackgroundColor = Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16(dataMoel.BackgroundColor0x16);
                }

                if (!string.IsNullOrEmpty(dataMoel.ForegroundColor0x16))
                {
                    textModel.ForegroundColor = Yuweiz.Phone.Media.ColorControler.ConvertFrom0x16(dataMoel.ForegroundColor0x16);
                }
            }

            return textModel;
        }


        private ImageAdCustomModel CreateImageAdCustomModel(AdDataModel dataMoel)
        {
            ImageAdCustomModel imageModel = null;

            if (dataMoel != null && !string.IsNullOrEmpty(dataMoel.ImgPath))
            {
                imageModel = new ImageAdCustomModel();
                imageModel.BackgroundImage = new BitmapImage(new Uri(dataMoel.ImgPath, UriKind.RelativeOrAbsolute));
                imageModel.TargetUrl = dataMoel.AdUrl;
            }


            return imageModel;
        }
    }
}
