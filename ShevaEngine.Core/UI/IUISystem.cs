using System;

namespace ShevaEngine.Core.UI
{
    public interface IUISystem
    {
        void RunOnUIThread(Action action);        
    }
}
