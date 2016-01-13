using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EyePad.UI
{
    public class RelayCommand : ICommand
    {
        Action _targetExecuteMethod;
        Func<bool> _targetCanExecuteMethod;

        public RelayCommand(Action executeMethod)
        {
            _targetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _targetExecuteMethod = executeMethod;
            _targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
         
        public bool CanExecute(object parameter)
        {
            if (_targetCanExecuteMethod != null)
            {
                return _targetCanExecuteMethod();
            }
            else
            {
                return (_targetExecuteMethod != null);
            }
        }

        public void Execute(object parameter)
        {
            if (_targetExecuteMethod != null)
            {
                _targetExecuteMethod();
            }
        }

        // Warning: Use weak references if command instance 
        // lifetime exceeds lifetime of their corresponding UI objects 
        public event EventHandler CanExecuteChanged = delegate {};
    }


    public class RelayCommand<T> : ICommand
    {
        Action<T> _targetExecuteMethod;
        Func<T, bool> _targetCanExecuteMethod;

        public RelayCommand(Action<T> executeMethod)
        {
            _targetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            _targetExecuteMethod = executeMethod;
            _targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_targetCanExecuteMethod != null)
            {
                return _targetCanExecuteMethod((T) parameter);
            }
            else
            {
                return (_targetExecuteMethod != null);
            }
        }

        public void Execute(object parameter)
        {
            if (_targetExecuteMethod != null)
            {
                _targetExecuteMethod((T) parameter);
            }
        }

        // Warning: Use weak references if command instance 
        // lifetime exceeds lifetime of their corresponding UI objects 
        public event EventHandler CanExecuteChanged = delegate { };
    }
}
