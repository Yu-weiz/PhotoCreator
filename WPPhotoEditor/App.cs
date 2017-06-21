using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPPhotoEditor.OLD.Entity;

namespace WPPhotoEditor
{
    public partial class App : Application
    {


        private static double _saveScale;

        /// <summary>
        /// 保存图片的缩放比率
        /// </summary>
        public static double SaveScale
        {
            get { return _saveScale; }
            set
            {
                _saveScale = value;
                if (_saveScale < 1)
                {
                    _saveScale = 1;
                }
            }
        }

        public static SettingState PESettings
        {
            get;
            set;
        }

        #region 配置信息
        private void openSettings()
        {
            App.PESettings = new SettingState();
            App.PESettings.CanvasSize = new Size(480, 640);
            App.PESettings.CanvasBackgroundColor = Colors.White;
            App.PESettings.MatchTheFirstLayerPic = false;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("SaveScale"))
            {
                App.SaveScale = Convert.ToDouble(settings["SaveScale"]);
            }
            else
            {
                App.SaveScale = 2;
            }


            if (settings.Contains("CanvasBackgroundColor"))
            {
                App.PESettings.CanvasBackgroundColor = WPPhotoEditor.Utility.UtilityFunction.ReturnColorFromString(settings["CanvasBackgroundColor"].ToString());
            }


            if (settings.Contains("CanvasSizeW") && settings.Contains("CanvasSizeH"))
            {
                App.PESettings.CanvasSize = new Size(Convert.ToDouble(settings["CanvasSizeW"]), Convert.ToDouble(settings["CanvasSizeH"]));

                //    App.PESettings.CanvasSize = new Size(720,960);
            }

            if (settings.Contains("MatchTheFirstLayerPic"))
            {
                App.PESettings.MatchTheFirstLayerPic = Convert.ToBoolean(settings["MatchTheFirstLayerPic"].ToString());
            }

        }

        private void saveSetting()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("SaveScale"))
            {
                settings["SaveScale"] = App.SaveScale;
            }
            else
            {
                settings.Add("SaveScale", App.SaveScale);
            }


            if (settings.Contains("CanvasSizeW"))
            {
                settings["CanvasSizeW"] = App.PESettings.CanvasSize.Width;
            }
            else
            {
                settings.Add("CanvasSizeW", App.PESettings.CanvasSize.Width);
            }

            if (settings.Contains("CanvasSizeH"))
            {
                settings["CanvasSizeH"] = App.PESettings.CanvasSize.Height;
            }
            else
            {
                settings.Add("CanvasSizeH", App.PESettings.CanvasSize.Height);
            }



            if (settings.Contains("CanvasBackgroundColor"))
            {
                settings["CanvasBackgroundColor"] = App.PESettings.CanvasBackgroundColor;
            }
            else
            {
                settings.Add("CanvasBackgroundColor", App.PESettings.CanvasBackgroundColor);
            }

            if (settings.Contains("MatchTheFirstLayerPic"))
            {
                settings["MatchTheFirstLayerPic"] = App.PESettings.MatchTheFirstLayerPic;
            }
            else
            {
                settings.Add("MatchTheFirstLayerPic", App.PESettings.MatchTheFirstLayerPic);
            }

        }
        #endregion
    }
}
