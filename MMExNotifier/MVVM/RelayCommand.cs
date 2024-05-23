using System;
using System.Windows.Input;

namespace MMExNotifier.MVVM
{
    internal class RelayCommand : ICommand
    {
        private readonly Action? _execute = null;
        private readonly Func<bool>? _canExecute = null;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action actionToExecute)
        {
            _execute = actionToExecute ?? throw new ArgumentNullException(nameof(actionToExecute));
        }

        public RelayCommand(Action actionToExecute, Func<bool> executionEvaluator)
            : this(actionToExecute)
        {
            _canExecute = executionEvaluator ?? throw new ArgumentNullException(nameof(executionEvaluator));
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute.Invoke();
        }

        public void Execute(object? parameter)
        {
            _execute?.Invoke();
        }
    }

    internal class RelayCommand<T> : ICommand
    {
        private readonly Action<T?>? _execute = null;
        private readonly Predicate<T?>? _canExecute = null;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<T?> actionToExecute)
        {
            _execute = actionToExecute ?? throw new ArgumentNullException(nameof(actionToExecute));
        }

        public RelayCommand(Action<T?> actionToExecute, Predicate<T?> executionEvaluator)
            : this(actionToExecute)
        {
            _canExecute = executionEvaluator ?? throw new ArgumentNullException(nameof(executionEvaluator));
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
                return true;

            if (parameter == null && typeof(T?).IsValueType)
                return _canExecute.Invoke(default);

            if (parameter == null || parameter is T?)
                return _canExecute.Invoke((T?)parameter);

            return false;
        }

        public void Execute(object? parameter)
        {
            _execute?.Invoke((T?)parameter);
        }
    }
}
