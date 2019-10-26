using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogSearcher.Domain
{
    public static class Utils
    {
        public static bool ValidateDirectory(string path)
        {
            if (path == null) return false; 

            DirectoryInfo DirInfo = new DirectoryInfo(path);
            if (DirInfo.Exists == false) return false;
            return true;
        }

        public static DirectoryInfo GetDirInfo(this string path)
        {            
            DirectoryInfo DirInfo = new DirectoryInfo(path);
            if (DirInfo.Exists == false) return null; 
            return DirInfo;
        }

    }
    
    public class TextPosition
    {
        public int Line;
        public int Column;
        public string Text = "";
    }

    public class RelayCommand : ICommand
    {
        readonly Action _execute;
        readonly Func<bool> _canExecute;


        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new NullReferenceException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute) : this(execute, null)
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
            _execute.Invoke();
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
