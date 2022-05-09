using System;
using System.Windows.Input;

namespace ShevaEngine.NoesisUI;

public class RelayCommand<T> : ICommand
{
    private Action<object?> _execute;
    private Func<object?, bool>? _canExecute;
    public event EventHandler? CanExecuteChanged;


    public RelayCommand(Action<object?> execute)
    {
        if (execute == null)
        {
            throw new ArgumentNullException("execute");
        }

        _execute = execute;
    }

    public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute)
        : this(execute)
    {            
        if (canExecute == null)
        {
            throw new ArgumentNullException("canExecute");
        }
        
        _canExecute = canExecute;            
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute((T)parameter!);
    }

    public void Execute(object? parameter)
    {
        _execute((T)parameter!);
    }       

    public void RaiseCanExecuteChanged()
    {
        EventHandler? handler = CanExecuteChanged;

        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
