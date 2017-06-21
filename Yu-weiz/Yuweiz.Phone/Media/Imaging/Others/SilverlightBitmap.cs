using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Media.Imaging.Others
{
    /// <summary>
    /// 注意
    /// Fantasia.Imaging.Silverlight
    /// </summary>
    public class SilverlightBitmap : BitmapBase
    {
        private WriteableBitmap _bitmap;

        public SilverlightBitmap(WriteableBitmap bitmap)
        {
            this._bitmap = bitmap;
            base.Width = bitmap.PixelWidth;
            base.Height = bitmap.PixelHeight;
            base.Pixels = this._bitmap.Pixels;
        }

        public SilverlightBitmap(int width, int height)
        {
            this._bitmap = new WriteableBitmap(width, height);
            base.Width = width;
            base.Height = height;
            base.Pixels = this._bitmap.Pixels;
        }

        public SilverlightBitmap(int width, int height, int[] pixels, bool delayCreateToPreventCrossThreadIssues)
        {
            if (delayCreateToPreventCrossThreadIssues)
            {
                this._bitmap = null;
                base.Width = width;
                base.Height = height;
                base.Pixels = pixels;
            }
            else
            {
                this._bitmap = new WriteableBitmap(width, height);
                Array.Copy(pixels, 0, this._bitmap.Pixels, 0, pixels.Length);
                this._bitmap.Invalidate();
                base.Width = width;
                base.Height = height;
                base.Pixels = pixels;
            }
        }

        public override BitmapBase Clone()
        {
            SilverlightBitmap bitmap = new SilverlightBitmap(base.Width, base.Height);
            Array.Copy(base.Pixels, 0, bitmap.Pixels, 0, base.Pixels.Length);
            bitmap.Bitmap.Invalidate();
            return bitmap;
        }

        public int[] CloneData()
        {
            int[] destinationArray = new int[base.Pixels.Length];
            Array.Copy(base.Pixels, 0, destinationArray, 0, base.Pixels.Length);
            return destinationArray;
        }

        public virtual SilverlightBitmap CloneDataOnly()
        {
            int[] destinationArray = new int[base.Pixels.Length];
            Array.Copy(base.Pixels, 0, destinationArray, 0, base.Pixels.Length);
            return new SilverlightBitmap(base.Width, base.Height, destinationArray, true);
        }

        public virtual SilverlightBitmap CloneDimensionsOnly()
        {
            return new SilverlightBitmap(base.Width, base.Height, new int[base.Pixels.Length], true);
        }

        public override BitmapBase Create(int width, int height)
        {
            return new SilverlightBitmap(width, height);
        }

        /// <summary>
        /// 创建筑缩略图
        /// </summary>
        /// <param name="boundingWidth"></param>
        /// <param name="boundingHeight"></param>
        /// <returns></returns>
        public override BitmapBase CreateThumbnail(int boundingWidth, int boundingHeight)
        {
            double num = ((double)boundingWidth) / ((double)base.Width);
            double num2 = ((double)boundingHeight) / ((double)base.Height);
            if (num > num2)
            {
                num = num2;
            }
            BitmapBase newBitmap = new SilverlightBitmap((int)(base.Width * num), (int)(base.Height * num));
            BitmapHelper.ResizeNearestNeighbour(this, newBitmap);
            return newBitmap;
        }

        public WriteableBitmap Bitmap
        {
            get
            {
                if (this._bitmap == null)
                {
                    this._bitmap = new WriteableBitmap(base.Width, base.Height);
                    Array.Copy(base.Pixels, 0, this._bitmap.Pixels, 0, base.Pixels.Length);
                    base.Pixels = this._bitmap.Pixels;
                    this._bitmap.Invalidate();
                }
                return this._bitmap;
            }
        }
    }
}
