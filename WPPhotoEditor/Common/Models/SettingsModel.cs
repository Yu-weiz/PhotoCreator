using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPPhotoEditor.Models
{
    public class SettingsModel
    {
        public SettingsModel()
        {
            this.IsShowHelp = true;
        }

        public string MainPageBackgroundPath { get; set; }

        public bool IsShowHelp { get; set; }

        public string Errors { get; set; }
    }
}
