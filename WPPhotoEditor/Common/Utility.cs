using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPPhotoEditor.ViewModels
{
    public class Utility
    {
        public static WriteableBitmap GetBrushResource(string imgName)
        {
            Stream stream = App.GetResourceStream(new Uri("/WPPhotoEditor;component/Assets/Brush/" + imgName, UriKind.Relative)).Stream;

            // Decode the JPEG stream.
            WriteableBitmap myBitmap = PictureDecoder.DecodeJpeg(stream);
            return myBitmap;
        }

        public static WriteableBitmap GetImageResource(string imgName)
        {
            Stream stream = App.GetResourceStream(new Uri("/WPPhotoEditor;component/Assets/Image/" + imgName, UriKind.Relative)).Stream;
            // Decode the JPEG stream.
            WriteableBitmap myBitmap = PictureDecoder.DecodeJpeg(stream);
            return myBitmap;
        }

        public static void ShowHelp(string message,int ms=2000)
        {
            if (!AppSession.Instance.SettingsModel.IsShowHelp)
            {
                return;
            }

            ToastPrompt toast = new ToastPrompt();
            toast.MillisecondsUntilHidden = ms;
            toast.Message = message;
            toast.Show();
        }
    }
}
