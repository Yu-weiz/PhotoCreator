using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Yuweiz.Phone.Media.Imaging.Effects.Sketch
{
    internal class SobelEffect : ImageEffect
    {
        protected override int TransformPixel(int[] source, int x, int y, int width, int height, int currentRowIndex)
        {
            return ImageEffect.Sobel(source, x, y, width, height, currentRowIndex, currentRowIndex + width, currentRowIndex - width);
        }
    }
}
