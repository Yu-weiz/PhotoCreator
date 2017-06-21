using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Yuweiz.Phone.Media.Imaging.Effects.Sketch
{
    public class PencilEffect : ImageEffect
    {
        private WriteableBitmap bitmap;    //材质
        private int currentTextureIndex;
        private int currentTextureX;
        private int currentTextureY;
        private int lastY;

        public PencilEffect(WriteableBitmap b)
        {
            this.bitmap = b;
        }

        protected override void InitFrame()
        {
            base.InitFrame();
            this.currentTextureX = 0;
            this.currentTextureY = 0;
            this.currentTextureIndex = 0;
            this.lastY = 0;
        }

        protected override int TransformPixel(int[] source, int x, int y, int width, int height, int currentRowIndex)
        {
            int num = ImageEffect.Sobel(source, x, y, width, height, currentRowIndex, currentRowIndex + width, currentRowIndex - width);
            int num2 = source[currentRowIndex + x];
            int num3 = 0;
            if ((num2 & 0xff) > 0x80)
            {
                num3 = this.bitmap.Pixels[this.currentTextureIndex + this.currentTextureX] & 0xff;
            }
            else
            {
                num3 = this.bitmap.Pixels[this.currentTextureIndex + ((this.bitmap.PixelWidth - this.currentTextureX) - 1)] & 0xff;
            }
            num3 = ImageEffect.Mul(num2 ^ 0xffffff, (0xff - num3) & 0xff);
            int num4 = ImageEffect.Sub(num, num3);
            if (y > this.lastY)
            {
                this.lastY = y;
                this.currentTextureY++;
                this.currentTextureIndex += this.bitmap.PixelWidth;
                this.currentTextureX = 0;
                if (this.currentTextureY == this.bitmap.PixelHeight)
                {
                    this.currentTextureY = 0;
                    this.currentTextureIndex = 0;
                }
            }
            this.currentTextureX++;
            if (this.currentTextureX == this.bitmap.PixelWidth)
            {
                this.currentTextureX = 0;
            }
            return num4;
        }


        public WriteableBitmap GetColorSketchEffect(BitmapSource bmp)
        {
            WriteableBitmap wbmp = new WriteableBitmap(bmp);
            PencilEffect pencil = new PencilEffect(bitmap);
            int[] desPencil = new int[wbmp.Pixels.Length];
            pencil.Transform(wbmp.Pixels, desPencil, wbmp.PixelWidth, wbmp.PixelHeight);

            for (int i = 0; i < wbmp.Pixels.Length; i++)
            {
                wbmp.Pixels[i] = desPencil[i];
            }
            return wbmp;
        }

        public int[] GetColorSketchEffect(int[] sourcePixels, int width, int height)
        {
            PencilEffect pencil = new PencilEffect(bitmap);
            int[] desPencil = new int[sourcePixels.Length];
            pencil.Transform(sourcePixels, desPencil, width, height);

            return desPencil;
        }
    }
}
