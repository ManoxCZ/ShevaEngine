using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ShevaEngine.Core.UserAccounts
{
    public interface IUser : IDisposable
    { 
        Task<bool> ConnectToService(bool silently = false);
    }
}
