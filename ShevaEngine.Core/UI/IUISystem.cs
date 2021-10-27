using System;
using System.Threading.Tasks;

namespace ShevaEngine.Core.UI
{
    public interface IUISystem
    {
        void RunOnUIThread(Action action);

        public Task<T> RunFuncOnUIThread<T>(Func<T> function);
    }
}
