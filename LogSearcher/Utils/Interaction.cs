using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LogSearcher.Utils
{


    public abstract class BaseCommand : ICommand
    {
        private readonly Action<object> execute;
        readonly Func<bool> canExecute;

        public event EventHandler CanExecuteChanged;
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public virtual void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }
    }

    public class CommandNoParams : BaseCommand
    {        
        readonly Func<bool> canExecute;
        private readonly Action execute;

        public CommandNoParams(Action execute, Func<bool> canExecute = null) 
        {
            if (execute == null)
            {
                throw new NullReferenceException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }        

        public override void Execute(object parameter)
        {
            execute.Invoke(); 
        }
    }

    public class CommandWithParams : BaseCommand
    {
        readonly Func<bool> canExecute;
        private readonly Action<object> execute;

        public CommandWithParams(Action<object> execute, Func<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new NullReferenceException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public override void Execute(object parameter)
        {
            execute.Invoke(parameter);
        }
    }

    public class Command : ICommand
    {
        private readonly Action<object> _action;
        public event EventHandler CanExecuteChanged { add { } remove { } }

        public Command(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }
    }


    public class RelayCommand : ICommand
    {
        readonly Action _execute;
        readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute) //RelayCommand(Action execute, 
        {
            if (execute == null)
            {
                throw new NullReferenceException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute) : this(execute, null) //RelayCommand(Action execute, 
        {
            // this overloaded constructor basically just calls the standard constructor while setting the predicate-parameter to null
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(); //_execute.Invoke();
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
