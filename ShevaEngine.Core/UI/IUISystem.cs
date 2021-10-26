using System;
using System.Threading.Tasks;

namespace ShevaEngine.Core.UI
{
    public interface IUISystem
    {
        public Task<ILayer> GetLayer(string xamlFilename);

        void RunOnUIThread(Action action);

        public Task<T> RunFuncOnUIThread<T>(Func<T> function);
    }
}
