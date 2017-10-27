using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace jwldnr.VisualLinter.ViewModels
{
    internal abstract class ViewModelBase : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected T GetPropertyValue<T>(DependencyProperty property)
        {
            return (T)GetValue(property);
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