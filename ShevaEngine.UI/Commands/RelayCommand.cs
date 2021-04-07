using System;

namespace ShevaEngine.UI
{
    public class RelayCommand<T> : ICommand
    {
        readonly Action<T> m_execute = null;
        readonly Predicate<T> m_canExecute = null;


        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
        /// </summary>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            m_execute = execute;
            m_canExecute = canExecute;
        }

        ///<summary>
        /// Defines the method that determines whether the command can execute in its current state.
        ///</summary>
        public bool CanExecute(object parameter)
        {
            return m_canExecute == null ? true : m_canExecute((T)parameter);
        }        

        /// <summary>
        /// Execute method.
        /// </summary>        
        public void Execute(object parameter)
        {
            m_execute((T)parameter);
        }
    }
}
