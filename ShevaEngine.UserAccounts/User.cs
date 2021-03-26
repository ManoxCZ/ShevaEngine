using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

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
            ConnectToService(true);
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
        public Task<bool> ConnectToService(bool silently = false)
        {
            TaskCompletionSource<bool> taskSource = new TaskCompletionSource<bool>();

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
                        GetUserData(_xboxLiveUser).ContinueWith(item => Data.OnNext(item.Result));                                         
                    else
                    {
                        Data.OnNext(UserData.Connecting);

                        if (silently)
                        {
                            _xboxLiveUser.SignInSilentlyAsync(dispatcher).Completed +=
                                (Windows.Foundation.IAsyncOperation<Microsoft.Xbox.Services.System.SignInResult> asyncInfoSignin, Windows.Foundation.AsyncStatus asyncStatusSignin) =>
                            {
                                if (asyncInfoSignin.GetResults().Status == Microsoft.Xbox.Services.System.SignInStatus.Success)
                                {
                                    GetUserData(_xboxLiveUser).ContinueWith(item => Data.OnNext(item.Result));

                                    taskSource.SetResult(true);
                                }
                                else
                                {
                                    Data.OnNext(null);

                                    taskSource.SetResult(false);
                                }
                            };
                        }
                        else
                        {
                            _xboxLiveUser.SignInAsync(dispatcher).Completed +=
                                (Windows.Foundation.IAsyncOperation<Microsoft.Xbox.Services.System.SignInResult> asyncInfoSignin, Windows.Foundation.AsyncStatus asyncStatusSignin) =>
                                {
                                    if (asyncInfoSignin.GetResults().Status == Microsoft.Xbox.Services.System.SignInStatus.Success)
                                    {
                                        GetUserData(_xboxLiveUser).ContinueWith(item => Data.OnNext(item.Result));

                                        taskSource.SetResult(true);
                                    }
                                    else
                                    {
                                        Data.OnNext(null);

                                        taskSource.SetResult(false);
                                    }
                                };
                        }
                    }
                }       
            };
#endif

            return taskSource.Task;
        }

#if WINDOWS_UAP
        /// <summary>
        /// Get user data.
        /// </summary>
        private Task<UserData> GetUserData(Microsoft.Xbox.Services.System.XboxLiveUser user)
        {
            Microsoft.Xbox.Services.XboxLiveContext context = new Microsoft.Xbox.Services.XboxLiveContext(user);
            Task<Microsoft.Xbox.Services.Social.XboxUserProfile> getUserProfileTask = context.ProfileService.GetUserProfileAsync(user.XboxUserId).AsTask();

            return getUserProfileTask.ContinueWith(userProfileTask => 
            {
                Microsoft.Xbox.Services.Social.XboxUserProfile userProfile = userProfileTask.Result;

                GetGamerPicture(user, context, userProfile);

                //Microsoft.Xbox.Services.XboxLiveHttpCall httpCall = Microsoft.Xbox.Services.XboxLiveHttpCall.CreateXboxLiveHttpCall(
                //    context.Settings, "GET", )

                return new UserData()
                {
                    GamerName = userProfile.GameDisplayName,
                };
            });                                  
        }

        private void GetGamerPicture(Microsoft.Xbox.Services.System.XboxLiveUser user,
            Microsoft.Xbox.Services.XboxLiveContext context, Microsoft.Xbox.Services.Social.XboxUserProfile userProfile)
        {
            Uri uri = userProfile.GameDisplayPictureResizeUri;

            Microsoft.Xbox.Services.XboxLiveHttpCall liveHttpCall = Microsoft.Xbox.Services.XboxLiveHttpCall.CreateXboxLiveHttpCall(
                context.Settings,
                "GET",
                $@"{uri.Scheme}://{uri.Host}",
                uri.Query);                        

            liveHttpCall.GetResponseWithoutAuth(Microsoft.Xbox.Services.HttpCallResponseBodyType.VectorBody)
                .AsTask()
                .ContinueWith(responseTask =>
                {
                    try
                    {
                        Microsoft.Xbox.Services.XboxLiveHttpCallResponse response = responseTask.Result;

                        if (response.ErrorCode != 0)
                            return;

                        //auto image = response->response_body_vector();
                        //SetGamerPic(image.data(), image.size());
                    }
                    catch (Exception exception)
                    {
                        //    OutputDebugString(L"Getting the http_call_response when getting a gamerpic threw an exception\n");
                    }
                });
        }
#endif      
    }
}

