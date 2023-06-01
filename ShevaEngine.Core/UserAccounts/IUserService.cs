using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ShevaEngine.Core.UserAccounts
{
    public interface IUserService : IDisposable
    {
        BehaviorSubject<IUserData?> UserData { get; }

        Task<bool> ConnectToService(bool silently = false);

        Task<bool> RegisterToServiceAsync(string username);
    }
}
