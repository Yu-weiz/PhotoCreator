using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using WPPhotoEditor.OLD.Entity;

namespace WPPhotoEditor.OLD
{

    public delegate void VoidFunctionP(PenType penType);

	public partial class UCColorChooser : UserControl
	{
		public UCColorChooser()
		{
			// Required to initialize variables
			InitializeComponent();

            this.colPPenColor.MouseLeftButtonDown += new MouseButtonEventHandler(colPPenColor_MouseLeftButtonDown);

            
            rdColor.Checked += new RoutedEventHandler(rdColor_Checked);
            rdColor.Unchecked += new RoutedEventHandler(rdColor_Checked);

            rdOriginal.Checked += new RoutedEventHandler(rdColor_Checked);
            rdOriginal.Unchecked += new RoutedEventHandler(rdColor_Checked);

            rdColor.Checked += new RoutedEventHandler(rdColor_Checked);
            rdColor.Unchecked += new RoutedEventHandler(rdColor_Checked);

            rdTransparent.Checked += new RoutedEventHandler(rdColor_Checked);
            rdTransparent.Unchecked += new RoutedEventHandler(rdColor_Checked);
		}

        void rdColor_Checked(object sender, RoutedEventArgs e)
        {
            if (setICOUI != null)
            {
                setICOUI(ChosenPenType);
            }
        }

        void colPPenColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rdColor.IsChecked = true;
        }

        public VoidFunctionP setICOUI;

        
        public PenType ChosenPenType
        {
            get 
            {
                PenType penType = PenType.Color;               

                if (rdColor.IsChecked.Value)
                {
                    penType = PenType.Color;
                }
                else if (rdOpacity.IsChecked.Value)
                {
                    penType = PenType.Opacity;
                }
                else if (rdOriginal.IsChecked.Value)
                {
                    penType = PenType.Original;
                }
                else
                {
                    penType = PenType.Transparent;
                }
               

                return penType;
            }
            set 
            {
                switch (value)
                {
                    case PenType.Color: rdColor.IsChecked = true; break;
                    case PenType.Opacity: rdOpacity.IsChecked= true; break;
                    case PenType.Original: rdOriginal.IsChecked = true; break;
                    default: rdTransparent.IsChecked = true; break;
                }              
            }
        }

        public Color hadChosenColor
        {
            get { return this.colPPenColor.Color; }
            set { this.colPPenColor.Color = value; }
        }
	}
}