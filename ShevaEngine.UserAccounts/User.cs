using System;
using System.Reactive.Subjects;

namespace ShevaEngine.UserAccounts
{
    /// <summary>
    /// User.
    /// </summary>
    public class User : IDisposable
    {
#if WINDOWS_UAP
        private Microsoft.Xbox.Services.System.XboxLiveUser _xboxLiveUser;
#endif
        public BehaviorSubject<UserData> Data { get; private set; } = new BehaviorSubject<UserData>(null);


        /// <summary>
        /// Constructor.
        /// </summary>
        public User()
        {
            ConnectToService();            
        }

        /// <summary>
        /// Dispose.s
        /// </summary>
        public void Dispose()
        {
            Data?.Dispose();
            Data = null;
        }

        /// <summary>
        /// Connect to service.
        /// </summary>
        public void ConnectToService()
        {
#if WINDOWS_UAP
            Windows.Foundation.IAsyncOperation<System.Collections.Generic.IReadOnlyList<Windows.System.User>> usersTask = Windows.System.User.FindAllAsync();

            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().CoreWindow.Dispatcher;

            usersTask.Completed += 
                (Windows.Foundation.IAsyncOperation<System.Collections.Generic.IReadOnlyList<Windows.System.User>> asyncInfo, Windows.Foundation.AsyncStatus asyncStatus) =>
            {
                foreach (Windows.System.User user in asyncInfo.GetResults())
                {
                    _xboxLiveUser = new Microsoft.Xbox.Services.System.XboxLiveUser(user);

                    if (_xboxLiveUser.IsSignedIn)
                        Data.OnNext(GetUserData(_xboxLiveUser));                                            
                    else
                    {
                        _xboxLiveUser.SignInSilentlyAsync(dispatcher).Completed +=
                            (Windows.Foundation.IAsyncOperation<Microsoft.Xbox.Services.System.SignInResult> asyncInfoSignin, Windows.Foundation.AsyncStatus asyncStatusSignin) =>
                        {
                            if (asyncInfoSignin.GetResults().Status == Microsoft.Xbox.Services.System.SignInStatus.Success)
                                Data.OnNext(GetUserData(_xboxLiveUser));
                            else
                                Data.OnNext(null);
                        };
                    }
                }       
            };
#endif
        }

#if WINDOWS_UAP
        /// <summary>
        /// Get user data.
        /// </summary>
        private UserData GetUserData(Microsoft.Xbox.Services.System.XboxLiveUser user)
        {
            return new UserData()
            {
                Name = user.Gamertag,                
            };
        }
#endif      
    }
}

