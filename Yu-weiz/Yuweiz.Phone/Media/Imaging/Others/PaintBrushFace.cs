using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Yuweiz.Phone.Media.Imaging.Others
{
    public class PaintBrushFace
    {
        private float _angleDeg;
        private Color _color;
        private byte _hardness;
        private byte _opacity;
        private byte[] _originalAlpha;
        public readonly int[] Data;
        public readonly int Height;
        public readonly bool IsEraser;
        public int JitterAbs;
        public int PixelCountSoFar;
        public int PixelCountToDrawOn;
      
        public readonly int Width;

        public int[] Pixels { get; set; }
      

        public void ChangeColor(Color newColor)
        {
            if (!this.IsEraser)
            {
                int length = this.Data.Length;
                uint r = newColor.R;
                uint g = newColor.G;
                uint b = newColor.B;
                int num5 = (int)((((newColor.A << 0x18) | (r << 0x10)) | (g << 8)) | b);
                if (newColor.A == 0xff)
                {
                    for (int i = 0; i < length; i++)
                    {
                        byte num7 = this._originalAlpha[i];
                        switch (num7)
                        {
                            case 0:
                                break;

                            case 0xff:
                                this.Data[i] = num5;
                                break;

                            default:
                                this.Data[i] = (int)((((num7 << 0x18) | ((((r * num7) * 0x8081) >> 0x17) << 0x10)) | ((((g * num7) * 0x8081) >> 0x17) << 8)) | (((b * num7) * 0x8081) >> 0x17));
                                break;
                        }
                    }
                }
                else
                {
                    uint a = newColor.A;
                    for (int j = 0; j < length; j++)
                    {
                        byte num10 = this._originalAlpha[j];
                        switch (num10)
                        {
                            case 0:
                                break;

                            case 0xff:
                                this.Data[j] = num5;
                                break;

                            default:
                                {
                                    uint num11 = (uint)(((a * num10) * 0x8081) >> 0x17);
                                    this.Data[j] = (int)((((num11 << 0x18) | ((((r * num11) * 0x8081) >> 0x17) << 0x10)) | ((((g * num11) * 0x8081) >> 0x17) << 8)) | (((b * num11) * 0x8081) >> 0x17));
                                    break;
                                }
                        }
                    }
                }
            }
        }

        public void ChangeColorDirty(Color newColor)
        {
            if (!this.IsEraser)
            {
                int length = this.Data.Length;
                uint r = newColor.R;
                uint g = newColor.G;
                uint b = newColor.B;
                int num5 = (int)((((newColor.A << 0x18) | (r << 0x10)) | (g << 8)) | b);
                if (newColor.A == 0xff)
                {
                    for (int i = 0; i < length; i++)
                    {
                        byte num7 = this._originalAlpha[i];
                        switch (num7)
                        {
                            case 0:
                                break;

                            case 0xff:
                                this.Data[i] = num5;
                                break;

                            default:
                                this.Data[i] = (int)((((num7 << 0x18) | (((r * num7) >> 8) << 0x10)) | (((g * num7) >> 8) << 8)) | ((b * num7) >> 8));
                                break;
                        }
                    }
                }
                else
                {
                    uint a = newColor.A;
                    for (int j = 0; j < length; j++)
                    {
                        byte num10 = this._originalAlpha[j];
                        switch (num10)
                        {
                            case 0:
                                break;

                            case 0xff:
                                this.Data[j] = num5;
                                break;

                            default:
                                {
                                    uint num11 = (a * num10) >> 8;
                                    this.Data[j] = (int)((((num11 << 0x18) | (((r * num11) >> 8) << 0x10)) | (((g * num11) >> 8) << 8)) | ((b * num11) >> 8));
                                    break;
                                }
                        }
                    }
                }
            }
        }



        private void CopyAlpha(int[] data, byte[] alphaData)
        {
            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                alphaData[i] = (byte)((data[i] >> 0x18) & 0xff);
            }
        }

        public Color GetColor()
        {
            return this._color;
        }

        public float AngleDeg
        {
            get
            {
                return this._angleDeg;
            }
        }

        public byte Hardness
        {
            get
            {
                return this._hardness;
            }
        }

        public float Jitter
        {
            set
            {
                this.JitterAbs = (int)(this.Width * value);
            }
        }

        public byte Opacity
        {
            get
            {
                return this._opacity;
            }
        }

        public float Spacing
        {
            set
            {
                this.PixelCountToDrawOn = (int)(this.Width * value);
                this.PixelCountSoFar = this.PixelCountToDrawOn;
            }
        }
    }
}
