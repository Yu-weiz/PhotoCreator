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

using System.Windows.Media.Imaging;

namespace WPPhotoEditor.OLD.Entity
{
    public delegate void VoidFunction();

    public class LayerState
    {
        public WriteableBitmap LastLayerWbp
        {
            get;
            set;
        }

        public int LayerIndex
        {
            get;
            set;
        }

        public bool isUndo
        {
            get;
            set;
        }
    }
}
