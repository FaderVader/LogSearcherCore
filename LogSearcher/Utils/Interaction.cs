using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LogSearcher.Utils
{

    public class RelayCommandNoParams : ICommand
    {        
        readonly Func<bool> canExecute;
        private readonly Action execute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommandNoParams(Action execute, Func<bool> canExecute = null) 
        {
            if (execute == null)
            {
                throw new NullReferenceException("No action provided to execute.");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute();
        }

        public void Execute(object parameter)
        {
            execute.Invoke(); 
        }
    }

    public class RelayCommandWithParams : ICommand
    {
        readonly Func<bool> canExecute;
        private readonly Action<object> execute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommandWithParams(Action<object> execute, Func<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new NullReferenceException("No action provided to execute.");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute();
        }

        public void Execute(object parameter)
        {
            execute.Invoke(parameter);
        }
    }

    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
