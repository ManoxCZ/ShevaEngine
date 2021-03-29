using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ShevaEngine.UserAccounts
{
    /// <summary>
    /// User.
    /// </summary>
    public class User : IDisposable
    {
        private readonly Log _log = new Log(typeof(User));
        private readonly ShevaGame _game;

#if WINDOWS_UAP
        private Microsoft.Xbox.Services.System.XboxLiveUser _xboxLiveUser;
#endif
        public BehaviorSubject<UserData> Data { get; private set; } = new BehaviorSubject<UserData>(null);


        /// <summary>
        /// Constructor.
        /// </summary>
        internal User(ShevaGame game)
        {
            _game = game;

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
            _log.Info("Xbox Live service");

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

                Task<Texture2D> pictureTask = GetGamerPicture(user, context, userProfile);
                pictureTask.Wait();

                return new UserData()
                {
                    GamerName = userProfile.GameDisplayName,
                    GamerPicture = pictureTask.Result
                };
            });                                  
        }

        /// <summary>
        /// Get gamer picture.
        /// </summary>
        private Task<Texture2D> GetGamerPicture(Microsoft.Xbox.Services.System.XboxLiveUser user,
            Microsoft.Xbox.Services.XboxLiveContext context, Microsoft.Xbox.Services.Social.XboxUserProfile userProfile)
        {
            TaskCompletionSource<Texture2D> taskSource = new TaskCompletionSource<Texture2D>();

            Uri uri = userProfile.GameDisplayPictureResizeUri;
            
            System.Collections.Specialized.NameValueCollection queryParts = System.Web.HttpUtility.ParseQueryString(uri.Query);
            
            string pathAndQuery = string.Join(string.Empty, uri.LocalPath, "?url=", queryParts["url"], "&format=", queryParts["format"]);

            Microsoft.Xbox.Services.XboxLiveHttpCall liveHttpCall = Microsoft.Xbox.Services.XboxLiveHttpCall.CreateXboxLiveHttpCall(
                context.Settings,
                "GET",
                $@"{uri.Scheme}://{uri.Host}",
                pathAndQuery);

            liveHttpCall.GetResponseWithoutAuth(Microsoft.Xbox.Services.HttpCallResponseBodyType.VectorBody)
                .AsTask()
                .ContinueWith(responseTask =>
                {
                    try
                    {
                        Microsoft.Xbox.Services.XboxLiveHttpCallResponse response = responseTask.Result;

                        if (response.ErrorCode != 0)
                        {
                            _log.Warning("Can't get gamer picture!");

                            taskSource.SetResult(null);

                            return;
                        }

                        _log.Info("Gamer picture received");

                        taskSource.SetResult(GetTexture(response.ResponseBodyVector));                        
                    }
                    catch (Exception exception)
                    {
                        _log.Error("Failed to get Xbox Live gamer picture", exception);
                    }
                });


            return taskSource.Task;
        }        

        /// <summary>
        /// Get Texture2D.
        /// </summary>
        private Texture2D GetTexture(byte[] data)
        {            
            using (MemoryStream stream = new MemoryStream(data))
                return Texture2D.FromStream(_game.GraphicsDevice, stream);            
        }
#endif      
    }
}

