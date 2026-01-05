using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        [SuppressMessage("Major Code Smell", "S2325:Methods and properties that don't access instance data should be static",
            Justification = "Method is intentionally instance-based to allow fluent usage pattern: command.raiseCanExecuteChanged()")]
        public void raiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}