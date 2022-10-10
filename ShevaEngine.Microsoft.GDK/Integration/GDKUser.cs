using ShevaEngine.Core.UserAccounts;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ShevaEngine.Microsoft.GDK.Integration;

public class GDKUser : IUser
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public BehaviorSubject<IUserData?> UserData => new (null);


    public GDKUser()
    {
        StartAndUpdate();
    }


    public Task<bool> ConnectToService(bool silently = false)
    {
        XGamingRuntime.SDK.XUserAddAsync(XGamingRuntime.XUserAddOptions.AddDefaultUserAllowingUI, (hr, userHandle) =>
        {
            //LOG("XUserAddAsync", hr);
            if (hr >= 0)
            {
              //  this.userHandle = userHandle;
                hr = XGamingRuntime.SDK.XBL.XblContextCreateHandle(userHandle, out XGamingRuntime.XblContextHandle? xblContextHandle);
                //LOG("XblContextCreateHandle", hr);

                hr = XGamingRuntime.SDK.XUserGetId(userHandle, out ulong userId);
                //LOG("XUserGetId", hr);

                if (hr >= 0)
                {
                    //GetUserProfiles.Enabled = true;
                    //GetUserProfile.Enabled = true;
                    //GetSocialUserProfiles.Enabled = true;
                    //AddPlayerXuid.Enabled = true;
                }
            }
        });

        return Task.FromResult(true);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }

    private Task StartAndUpdate()
    {
        XGamingRuntime.SDK.XGameRuntimeInitialize();

        return Task.Run(() =>
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                XGamingRuntime.SDK.XTaskQueueDispatch();
            }
        },
        _cancellationTokenSource.Token);
    }
}
