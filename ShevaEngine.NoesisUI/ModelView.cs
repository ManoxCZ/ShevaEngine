using Microsoft.Extensions.Logging;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ShevaEngine.NoesisUI
{
    public class ModelView : INotifyPropertyChanged, IDisposable
    {
        protected ShevaGame Game { get; }


        /// <summary>
        /// Constructor.
        /// </summary>        
        public ModelView(ShevaGame game)
        {
            Game = game;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected ILogger Log { get; }

        protected void OnPropertyChanged(string name)
        {
            RunOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }
        protected List<IDisposable> Disposables { get; } = new List<IDisposable>();


        /// <summary>
        /// Constructor.
        /// </summary>
        public ModelView()
        {
            Log = ShevaServices.GetService<ILoggerFactory>().CreateLogger(GetType());
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
                disposable.Dispose();

            Disposables.Clear();
        }

        /// <summary>
        /// Run on UI thread.
        /// </summary>
        protected void RunOnUIThread(Action action)
        {
            ShevaGame.Instance.SynchronizationContext.Send(_ => action(), null);
        }
    }
}
