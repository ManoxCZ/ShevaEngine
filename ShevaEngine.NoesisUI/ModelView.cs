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
            NoesisUIWrapper.RunOnUIThread(() =>
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
