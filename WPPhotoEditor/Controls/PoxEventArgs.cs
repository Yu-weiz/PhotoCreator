using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPPhotoEditor.UControls
{  
    public class PoxEventArgs : EventArgs
    {
        public PoxEventArgs(double x, double y)
            : base()
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }
}
