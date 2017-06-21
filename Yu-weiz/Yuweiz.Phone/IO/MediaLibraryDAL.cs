using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.IO
{
    public class MediaLibraryDAL
    {
        private MediaLibraryDAL()
        { }

        private static MediaLibraryDAL instance;

        public static MediaLibraryDAL Instance
        {
            get
            {
                if (instance == null)
                {
                    MediaLibraryDAL.instance = new MediaLibraryDAL();
                }

                return instance;
            }
        }

        public string SavePicture(BitmapSource bmpSource, string fileName = "")
        {
            if (bmpSource == null)
            {
                return null;
            }

            WriteableBitmap wbmp = bmpSource as WriteableBitmap;
            if (wbmp == null)
            {
                wbmp = new WriteableBitmap(bmpSource);
            }

            if (fileName == "")
            {
                fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            wbmp.SaveJpeg(stream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 100);
            Microsoft.Xna.Framework.Media.MediaLibrary library = new Microsoft.Xna.Framework.Media.MediaLibrary();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            library.SavePicture(fileName, stream);
            stream.Close();
            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream">图片流</param>
        /// <param name="fileName"></param>
        [Obsolete("未实现预设功能")]
        public void SavePNGPicture(BitmapSource bmpSource, string fileName = "")
        {
            if (bmpSource == null)
            {
                return;
            }

            WriteableBitmap wbmp = bmpSource as WriteableBitmap;
            if (wbmp == null)
            {
                wbmp = new WriteableBitmap(bmpSource);
            }

            if (fileName == "")
            {
                fileName = DateTime.Now.ToString("yyyy-mm-dd_hh:mm:ss");
            }

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            wbmp.SavePng(stream);
            Microsoft.Xna.Framework.Media.MediaLibrary library = new Microsoft.Xna.Framework.Media.MediaLibrary();
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            library.SavePicture(fileName, stream);
            stream.Close();
        }
    }
}
