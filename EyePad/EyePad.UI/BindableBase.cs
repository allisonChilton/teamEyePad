using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EyePad.UI.Annotations;

namespace EyePad.UI
{
    class BindableBase : INotifyPropertyChanged
    {
        protected virtual void SetProperty<T>(ref T member, T val, 
            [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val))
            {
                return;
            }
            else
            {
                member = val;
                OnPropertyChanged(propertyName);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

    }
}
