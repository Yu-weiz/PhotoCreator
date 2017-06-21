using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Yuweiz.Phone.Common
{
    public class NotifyPropertyBase:INotifyPropertyChanged
    {       
        public event PropertyChangedEventHandler PropertyChanged;
       
        public void DoPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
