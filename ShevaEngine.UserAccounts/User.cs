using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ShevaEngine.UserAccounts
{
    /// <summary>
    /// User.
    /// </summary>
    public class User : IDisposable
    {        
        private const int CANCEL_TIMEOUT = 5000;

        private readonly Log _log = new Log(typeof(User));
        private readonly ShevaGame _game;

#if WINDOWS_UAP
        private Microsoft.Xbox.Services.System.XboxLiveUser _xboxLiveUser;
        private Microsoft.Xbox.Services.Statistics.Manager.StatisticManager _statisticManager => Microsoft.Xbox.Services.Statistics.Manager.StatisticManager.SingletonInstance;
#endif
        public BehaviorSubject<UserData> Data { get; private set; } 
        public CancellationTokenSource _cancellationTokenSource;


        /// <summary>
        /// Constructor.
        /// </summary>
        internal User(ShevaGame game)
        {
            _game = game;
            Data = new BehaviorSubject<UserData>(null);            

#if WINDOWS_UAP
            Microsoft.Xbox.Services.System.XboxLiveUser.SignOutCompleted += XboxLiveUser_SignOutCompleted;
#endif
        }        

        /// <summary>
        /// Dispose.s
        /// </summary>
        public void Dispose()
        {
#if WINDOWS_UAP
            Microsoft.Xbox.Services.System.XboxLiveUser.SignOutCompleted -= XboxLiveUser_SignOutCompleted;
#endif

            Data?.Dispose();
            Data = null;
        }

        /// <summary>
        /// Connect to service.
        /// </summary>
        public Task<bool> ConnectToService(bool silently = false)
        {
            _cancellationTokenSource?.Cancel();
            
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(CANCEL_TIMEOUT);

            TaskCompletionSource<bool> taskSource = new TaskCompletionSource<bool>();

#if WINDOWS_UAP
            _log.Info("Xbox Live service");

            Windows.Foundation.IAsyncOperation<IReadOnlyList<Windows.System.User>> usersTask = Windows.System.User.FindAllAsync();

            Windows.UI.Core.CoreDispatcher dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().CoreWindow.Dispatcher;

            usersTask.Completed += 
                (Windows.Foundation.IAsyncOperation<IReadOnlyList<Windows.System.User>> asyncInfo, Windows.Foundation.AsyncStatus asyncStatus) =>
            {
                foreach (Windows.System.User user in asyncInfo.GetResults())
                {
                    _xboxLiveUser = new Microsoft.Xbox.Services.System.XboxLiveUser(user);                    

                    if (silently && _xboxLiveUser.IsSignedIn)
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
                                        AddLocalUser(_xboxLiveUser, _cancellationTokenSource.Token).ContinueWith(addedToStat =>
                                        {
                                            if (addedToStat.Result)
                                                GetUserData(_xboxLiveUser).ContinueWith(item => Data.OnNext(item.Result));
                                        });

                                        _cancellationTokenSource = null;
                                        taskSource.SetResult(true);
                                    }
                                    else
                                    {
                                        Data.OnNext(null);

                                        _cancellationTokenSource = null;
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
                                        AddLocalUser(_xboxLiveUser, _cancellationTokenSource.Token).ContinueWith(addedToStat =>
                                        {
                                            if (addedToStat.Result)
                                                GetUserData(_xboxLiveUser).ContinueWith(item => Data.OnNext(item.Result));
                                        });

                                        _cancellationTokenSource = null;
                                        taskSource.SetResult(true);
                                    }
                                    else
                                    {
                                        Data.OnNext(null);

                                        _cancellationTokenSource = null;
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

        /// <summary>
        /// Disconnect.
        /// </summary>        
        public Task<bool> Disconnect()
        {
#if WINDOWS_UAP
            
#endif
            return Task.FromResult(true);
        }

#if WINDOWS_UAP     
        /// <summary>
        /// Xbox Live user signed out.
        /// </summary>
        private void XboxLiveUser_SignOutCompleted(object sender, Microsoft.Xbox.Services.System.SignOutCompletedEventArgs e)
        {
            RemoveLocalUser(_xboxLiveUser, CancellationToken.None);

            Data.OnNext(null);
        }

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

                Task<Texture2D> pictureTask = GetGamerPicture(context, userProfile);
                pictureTask.Wait();

                return new UserData()
                {
                    GamerName = userProfile.GameDisplayName,
                    GamerPicture = pictureTask.Result,
                    XboxLiveUser = _xboxLiveUser,
                };
            });                                  
        }

        /// <summary>
        /// Get gamer picture.
        /// </summary>
        private Task<Texture2D> GetGamerPicture(string xboxLiveId)
        {
            Microsoft.Xbox.Services.XboxLiveContext context = new Microsoft.Xbox.Services.XboxLiveContext(_xboxLiveUser);
            Task<Microsoft.Xbox.Services.Social.XboxUserProfile> getUserProfileTask = context.ProfileService.GetUserProfileAsync(xboxLiveId).AsTask();

            return getUserProfileTask.ContinueWith(userProfileTask =>
            {
                Microsoft.Xbox.Services.Social.XboxUserProfile userProfile = userProfileTask.Result;

                Task<Texture2D> pictureTask = GetGamerPicture(context, userProfile);
                pictureTask.Wait();

                return pictureTask.Result;
            });
        }

        /// <summary>
        /// Get gamer picture.
        /// </summary>
        private Task<Texture2D> GetGamerPicture(
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

        /// <summary>
        /// Add local user.
        /// </summary>
        private Task<bool> AddLocalUser(Microsoft.Xbox.Services.System.XboxLiveUser user, CancellationToken cancellationToken)
        {           
            return Task.Run(() =>
            {
                try
                {
                    _statisticManager.AddLocalUser(user);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        foreach (Microsoft.Xbox.Services.Statistics.Manager.StatisticEvent statEvent in _statisticManager.DoWork())
                        {
                            if (statEvent.EventType == Microsoft.Xbox.Services.Statistics.Manager.StatisticEventType.LocalUserAdded)
                                return statEvent.ErrorCode == 0;
                        }

                        Task.Delay(500).Wait();
                    }
                }
                catch (Exception exception)
                {
                    _log.Error("Can't add local player to statistic manager", exception);
                }

                return false;
            });
        }

        /// <summary>
        /// Add local user.
        /// </summary>
        private Task<bool> RemoveLocalUser(Microsoft.Xbox.Services.System.XboxLiveUser user, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _statisticManager.RemoveLocalUser(user);

                while (!cancellationToken.IsCancellationRequested)
                {
                    foreach (Microsoft.Xbox.Services.Statistics.Manager.StatisticEvent statEvent in _statisticManager.DoWork())
                    {
                        if (statEvent.EventType == Microsoft.Xbox.Services.Statistics.Manager.StatisticEventType.LocalUserRemoved)
                            return statEvent.ErrorCode == 0;
                    }

                    Task.Delay(500).Wait();
                }

                return false;
            });
        }

        /// <summary>
        /// Get leaderboard.
        /// </summary>
        public Task<IEnumerable<LeaderboardItem<T>>> GetLeaderboard<T>(string name)
        {
            if (_xboxLiveUser == null || !_xboxLiveUser.IsSignedIn)
                return Task.FromResult<IEnumerable<LeaderboardItem<T>>>(null);
            
            Microsoft.Xbox.Services.Leaderboard.LeaderboardQuery query = new Microsoft.Xbox.Services.Leaderboard.LeaderboardQuery()
            {                
                MaxItems = 10,
                SkipResultToMe = false,                                                
            };

            _statisticManager.GetLeaderboard(_xboxLiveUser, name, query);
            
            return Task.Run(() =>
            {
                while (true)
                    foreach (Microsoft.Xbox.Services.Statistics.Manager.StatisticEvent statEvent in _statisticManager.DoWork())
                    {
                        if (statEvent.EventType == Microsoft.Xbox.Services.Statistics.Manager.StatisticEventType.GetLeaderboardComplete && statEvent.ErrorCode == 0)
                        {
                            Microsoft.Xbox.Services.Statistics.Manager.LeaderboardResultEventArgs leaderArgs =
                                (Microsoft.Xbox.Services.Statistics.Manager.LeaderboardResultEventArgs)statEvent.EventArgs;

                            Microsoft.Xbox.Services.Leaderboard.LeaderboardResult leaderboardResult = leaderArgs.Result;

                            return leaderboardResult.Rows.ToList().Select(item =>
                            {
                                Task<Texture2D> pictureTask = GetGamerPicture(item.XboxUserId);
                                pictureTask.Wait();

                                return new LeaderboardItem<T>()
                                {
                                    GamerName = item.Gamertag,
                                    Rank = item.Rank,
                                    Score = GetScore<T>(item.Values[0]),                                    
                                    Picture = pictureTask.Result,
                                };
                            });
                        }
                    }
            });
        }

        /// <summary>
        /// Get score.
        /// </summary>
        private T GetScore<T>(string value)
        {
            if (typeof(T) == typeof(byte) ||
               typeof(T) == typeof(short) ||
               typeof(T) == typeof(ushort) ||
               typeof(T) == typeof(int) ||
               typeof(T) == typeof(uint) ||
               typeof(T) == typeof(long) ||
               typeof(T) == typeof(ulong))
                return (T)(object)int.Parse(value);
            else if (typeof(T) == typeof(float) ||
                     typeof(T) == typeof(double))
                return (T)(object)double.Parse(value, CultureInfo.InvariantCulture);

            return (T)(object)value;
        }
#endif
    }
}

