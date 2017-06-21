using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPPhotoEditor.Resources;

namespace WPPhotoEditor
{
    public class LocalizedStrings
    {
        private static AppResources localizedResource = new AppResources();

        public AppResources LocalizedResource
        {
            get { return LocalizedStrings.localizedResource; }

        }
    }
}
