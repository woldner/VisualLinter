using System;
using System.Windows.Input;

namespace jwldnr.VisualLinter.Helpers
{
    internal class RelayCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        internal RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        internal RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ??
                throw new ArgumentNullException(nameof(execute));

            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        internal void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }

    internal class RelayCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        internal RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        internal RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ??
                throw new ArgumentNullException(nameof(execute));

            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T) parameter);
        }

        internal void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
