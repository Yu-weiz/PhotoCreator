using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.IO
{
    public class StreamConvert
    {
        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public static byte[] StreamToBytes(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary> 
        /// 将 byte[] 转成 Stream 
        /// </summary> 
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        public static byte[] WriteableBitmapToBytes(WriteableBitmap wbmp)
        {
            MemoryStream stream = new MemoryStream();
            wbmp.SaveJpeg(stream, wbmp.PixelWidth,
            wbmp.PixelHeight, 0, 100);
            byte[] rgbBytes = stream.ToArray();
            stream.Dispose();
            return rgbBytes;
        }
    }
}
