using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Yuweiz.Phone.Media.Imaging;

namespace WPPhotoEditor.ViewModels.Tools
{
    public class BrushSamples
    {     
        private WriteableBitmap[] wbmpArry = new WriteableBitmap[4];

        public WriteableBitmap GetWbmp(FillPixelTypeEnum fillType)
        {
            int index = (int)fillType;
            return wbmpArry[index];
        }

        public void SetWbmp(FillPixelTypeEnum fillType, WriteableBitmap wbmp)
        {
            int index = (int)fillType;
            wbmpArry[index] = wbmp;
        }
    }
}
