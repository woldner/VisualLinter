using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace jwldnr.VisualLinter.ViewModels
{
    internal abstract class ViewModelBase : UserControl, INotifyPropertyChanged
    {
        protected readonly IVisualLinterOptions Options;

        protected ViewModelBase()
        {
            Options = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>() ??
                throw new Exception("exception: unable to retrieve options");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected T GetPropertyValue<T>(DependencyProperty property)
        {
            var value = (T)GetValue(property);

            return value;
        }

        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetPropertyValue<T>(DependencyProperty property, T newValue, [CallerMemberName] string propertyName = null)
        {
            var oldValue = GetPropertyValue<T>(property);

            if (EqualityComparer<T>.Default.Equals(newValue, oldValue))
                return false;

            SetValue(property, newValue);
            RaisePropertyChanged(propertyName);

            return true;
        }
    }
}
