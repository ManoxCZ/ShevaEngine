using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShevaEngine.Core.UI;

public class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected readonly ILogger Log;
    protected readonly List<IDisposable> Disposables = new();

    public ViewModel()
    {
        Log = ShevaServices.GetService<ILoggerFactory>().CreateLogger(GetType());    
    }

    public virtual void Dispose()
    {
        foreach (IDisposable disposable in Disposables)
        {
            disposable.Dispose();
        }

        Disposables.Clear();
    }

    protected void OnPropertyChanged(string name)
    {
        ShevaGame.Instance.SynchronizationContext.Send(_ =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }, null);
    }
}


public class ViewModel<T> : ViewModel, INotifyPropertyChanged where T : ShevaGameComponent
{
    public T GameComponent { get; }
    
    public ViewModel(T gameComponent)
        : base()
    {       
        GameComponent = gameComponent;
    }    
}
