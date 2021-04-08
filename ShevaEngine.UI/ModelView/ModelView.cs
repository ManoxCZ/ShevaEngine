using System;
using System.Collections.Generic;

namespace ShevaEngine.UI
{
    /// <summary>
    /// Model view.
    /// </summary>
    public class ModelView : PropertiesClass, IDisposable
    {
        protected List<IDisposable> Disposables { get; }        


        /// <summary>
        /// Constructor.
        /// </summary>
        public ModelView()
        {
            Disposables = new List<IDisposable>();            
        }
        
        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            foreach (IDisposable item in Disposables)
                item.Dispose();

            Disposables.Clear();

            base.Dispose();
        }
    }
}
