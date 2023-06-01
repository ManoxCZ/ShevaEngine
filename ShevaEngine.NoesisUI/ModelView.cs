using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShevaEngine.NoesisUI;

public class ModelView : INotifyPropertyChanged, IDisposable
{        
    public event PropertyChangedEventHandler? PropertyChanged;
    protected ILogger Log { get; }

    protected void OnPropertyChanged(string name)
    {
        RunOnUIThread(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        });
    }
    protected List<IDisposable> Disposables { get; } = new List<IDisposable>();


    public ModelView()
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

    protected void RunOnUIThread(Action action)
    {
        ShevaGame.Instance.SynchronizationContext.Send(_ => action(), null);
    }
}
