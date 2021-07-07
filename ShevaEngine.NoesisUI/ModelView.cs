using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShevaEngine.NoesisUI
{
    public class ModelView : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }
        protected List<IDisposable> Disposables { get; } = new List<IDisposable>();


        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
                disposable.Dispose();

            Disposables.Clear();
        }

    }
}
