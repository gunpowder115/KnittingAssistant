using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace KnittingAssistant.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = " ")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public static SynchronizationContext SynchronizationContext;

        public ViewModelBase()
        {
            if (SynchronizationContext == null)
                SynchronizationContext = SynchronizationContext.Current;
        }
    }
}
