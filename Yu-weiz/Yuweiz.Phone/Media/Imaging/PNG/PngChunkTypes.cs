/*
 * ToolStack.com C# PNG Writer library by Greg Ross
 * 
 * Homepage: http://ToolStack.com/PNGWriter
 * 
 * This library is inspired by the examples hosted at the forums on WriteableBitmapEx
 * project at the codeplex site (http://writeablebitmapex.codeplex.com/discussions/274445), however
 * there's not really any of that code left, just some constants.
 * 
 * Compression is currently not supported but I am looking at adding it in.
 * 
 * This is public domain software, use and abuse as you see fit.
 * 
 * Version 1.0 - Released Feburary 22, 2012
 *         2.0 - Rewrote WriteDataChunksUncompressed() pretty much from the ground up to reduce the 3
 *               copies of the image in memory down to just a single copy.  This also reduced the 
 *               number of loops performed to manipulate the data from 2 to 1.
 *         2.1 - De-multiplied alpha channel, thanks to Jan for supplying the fix!
 */

using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Media.Imaging.PNG
{
    static class PngChunkTypes
    {
        /// <summary>
        /// The first chunk in a png file. Can only exists once. Contains 
        /// common information like the width and the height of the image or
        /// the used compression method.
        /// </summary>
        public const string Header = "IHDR";
        /// <summary>
        /// The PLTE chunk contains from 1 to 256 palette entries, each a three byte
        /// series in the RGB format.
        /// </summary>
        public const string Palette = "PLTE";
        /// <summary>
        /// The IDAT chunk contains the actual image data. The image can contains more
        /// than one chunk of this type. All chunks together are the whole image.
        /// </summary>
        public const string Data = "IDAT";
        /// <summary>
        /// This chunk must appear last. It marks the end of the PNG datastream. 
        /// The chunk's data field is empty. 
        /// </summary>
        public const string End = "IEND";
        /// <summary>
        /// This chunk specifies that the image uses simple transparency: 
        /// either alpha values associated with palette entries (for indexed-color images) 
        /// or a single transparent color (for grayscale and truecolor images). 
        /// </summary>
        public const string PaletteAlpha = "tRNS";
        /// <summary>
        /// Textual information that the encoder wishes to record with the image can be stored in 
        /// tEXt chunks. Each tEXt chunk contains a keyword and a text string.
        /// </summary>
        public const string Text = "tEXt";
        /// <summary>
        /// This chunk specifies the relationship between the image samples and the desired 
        /// display output intensity.
        /// </summary>
        public const string Gamma = "gAMA";
        /// <summary>
        /// The pHYs chunk specifies the intended pixel size or aspect ratio for display of the image. 
        /// </summary>
        public const string Physical = "pHYs";
    }
}
