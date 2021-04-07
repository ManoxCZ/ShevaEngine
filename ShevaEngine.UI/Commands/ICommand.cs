namespace ShevaEngine.UI
{
    public interface ICommand
    {
        bool CanExecute(object parameter);

        void Execute(object parameter);
    }
}
