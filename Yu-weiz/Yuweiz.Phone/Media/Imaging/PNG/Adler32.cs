/*
 * ToolStack.com C# CRC library by Greg Ross
 * 
 * Homepage: http://ToolStack.com/CRCLib
 * 
 * This library is based upon the examples hosted at the PNG and zlib
 * home pages (www.libpng.org/pub/png/ & zlib.net), ported to C#.
 * 
 * This is public domain software, use and abuse as you see fit.
 * 
 * Version 1.0 - Released Feburary 22, 2012
 *         2.0 - Fixed adler() and addToAdler() to increment the index after the loop instead of before
 *             - Fixed addToAdler() to work with offsets
 *             - Fixed resetAdler() to reset the A & B values correctly
 */

using System;

namespace Yuweiz.Phone.Media.Imaging.PNG
{
    /// <summary>
    /// Computes the Adler32 CRC value for a given data set
    /// </summary>
    public class Adler32
    {
        private const int MOD_ADLER = 65521;
        private UInt32 AdlerA = 1;
        private UInt32 AdlerB = 0;

        /*
         * The Adler32 checksum code for use in zlib compression
        */

        /// <summary>
        /// Returns the current running Adler32 result.
        /// </summary>
        /// <returns>An unsigned 32bit integer representing the current Adler32.</returns>
        public UInt32 adler()
        {
            return (AdlerB << 16) | AdlerA;
        }

        /// <summary>
        /// Returns the current running Adler32 result.
        /// </summary>
        /// <param name="data">A byte[] to process.</param>
        /// <returns>An unsigned 32bit integer representing the current Adler32.</returns>
        public UInt32 adler(byte[] data)
        {
            return adler(data, data.Length, 0);
        }

        /// <summary>
        /// Returns the current running Adler32 result.
        /// </summary>
        /// <param name="data">A byte[] to process.</param>
        /// <param name="len">The length of the byte[].</param>
        /// <returns>An unsigned 32bit integer representing the current Adler32.</returns>
        public UInt32 adler(byte[] data, int len)
        {
            return adler(data, len, 0);
        }
        /// <summary>
        /// Returns the current running Adler32 result.
        /// </summary>
        /// <param name="data">A byte[] to process.</param>
        /// <param name="len">The length of the byte[].</param>
        /// <param name="offset">The offset to start processing byte[] at.</param>
        /// <returns>An unsigned 32bit integer representing the current Adler32.</returns>
        public UInt32 adler(byte[] data, int len, UInt32 offset)
        {
            UInt32 a = 1, b = 0;
            UInt32 index;

            /* Process each byte of the data in order */
            for (index = offset; index < offset + len; index++)
            {
                a = (a + data[index]) % MOD_ADLER;
                b = (b + a) % MOD_ADLER;
            }

            return (b << 16) | a;
        }

        /// <summary>
        /// Adds to the current running Adler32 the bytes from buf[].
        /// </summary>
        /// <param name="data">A byte[] to process.</param>
        public void addToAdler(byte[] data)
        {
            addToAdler(data, data.Length, 0);
        }

        /// <summary>
        /// Adds to the current running Adler32 the bytes from buf[].
        /// </summary>
        /// <param name="data">A byte[] to process.</param>
        /// <param name="len">The length of the byte[].</param>
        public void addToAdler(byte[] data, int len)
        {
            addToAdler(data, len, 0);
        }

        /// <summary>
        /// Adds to the current running Adler32 the bytes from buf[].
        /// </summary>
        /// <param name="data">A byte[] to process.</param>
        /// <param name="len">The length of the byte[].</param>
        /// <param name="offset">The offset to start processing byte[] at.</param>
        public void addToAdler(byte[] data, int len, UInt32 offset)
        {
            UInt32 index;

            /* Process each byte of the data in order */
            for (index = offset; index < offset + len; index++)
            {
                AdlerA = (AdlerA + data[index]) % MOD_ADLER;
                AdlerB = (AdlerB + AdlerA) % MOD_ADLER;
            }
        }

        /// <summary>
        /// Resets the running Adler32.
        /// </summary>
        public void resetAdler()
        {
            AdlerA = 1;
            AdlerB = 0;
        }
    }
}
