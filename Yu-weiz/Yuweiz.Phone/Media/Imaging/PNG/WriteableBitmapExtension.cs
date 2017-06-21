using Yuweiz.Phone.Media.Imaging.PNG;

namespace System.Windows.Media.Imaging
{
    /// <summary>
    /// WriteableBitmap Extensions for PNG Writing
    /// </summary>
    public static partial class WriteableBitmapExtPNG
    {
        /// <summary>
        /// Write and PNG file out to a file stream.  Currently compression is not supported.
        /// </summary>
        /// <param name="image">The WriteableBitmap to work on.</param>
        /// <param name="stream">The destination file stream.</param>
        public static void SavePng(this WriteableBitmap image, System.IO.Stream stream)
        {
            SavePng(image, stream, -1);
        }

        /// <summary>
        /// Write and PNG file out to a file stream.  Currently compression is not supported.
        /// </summary>
        /// <param name="image">The WriteableBitmap to work on.</param>
        /// <param name="stream">The destination file stream.</param>
        /// <param name="compression">Level of compression to use (-1=auto, 0=none, 1-100 is percentage).</param>
        public static void SavePng(this WriteableBitmap image, System.IO.Stream stream, int compression)
        {
            PNGWriter.DetectWBByteOrder();
            PNGWriter.WritePNG(image, stream, compression);
        }
    }
}
