using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;

namespace MMExNotifier.MVVM
{
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event EventHandler? OnClose;
        public event EventHandler? OnOpen;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Close()
        {
            OnClose?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Open()
        {
            OnOpen?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Activate();
    }
}
