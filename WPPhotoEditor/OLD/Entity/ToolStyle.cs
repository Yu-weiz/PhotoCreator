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

namespace WPPhotoEditor.OLD.Entity
{
    public class Tool
    {

        public Tool()
        {
            CurToolType = ToolType.CanvasZoom;
            CurBrush = new BrushEntity();
        }

        public ToolType CurToolType
        {
            get;
            set;
        }

        public BrushEntity CurBrush
        {
            get;
            set;
        }
    }
}
