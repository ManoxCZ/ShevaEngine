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
        protected void OnPropertyChanged(string name)
        {
            Game.TasksManager.RunTaskOnMainThread(new Task(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name))));            
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
