using PlayFab;
using PlayFab.ClientModels;
using ShevaEngine.Core.UserAccounts;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ShevaEngine.Playfab;

public class PlayfabUserService : IUserService
{
    public BehaviorSubject<IUserData?> UserData { get; } = new(null);


    public PlayfabUserService(string titleId)
    {
        PlayFabSettings.staticSettings.TitleId = titleId;
    }

    public void Dispose()
    {

    }

    public async Task<bool> ConnectToService(bool silently = false)
    {
        UserData.OnNext(ConnectingUserData.Instance);

        LoginWithCustomIDRequest request = new()
        {
            CreateAccount = true,
            CustomId = "User"
        };
        
        if (await PlayFabClientAPI.LoginWithCustomIDAsync(request) is PlayFabResult<LoginResult> result)
        {
            PlayfabUserData userData = new(result.Result.AuthenticationContext);

            if (await userData.GetData())
            {
                UserData.OnNext(userData);                

                return true;
            }
        }

        UserData.OnNext(null);

        return false;
    }

    public async Task<bool> RegisterToServiceAsync(string username)
    {
        RegisterPlayFabUserRequest request = new()
        {
            Username = username,
            
        };

        if (await PlayFabClientAPI.RegisterPlayFabUserAsync(request) is PlayFabResult<RegisterPlayFabUserResult> result)
        {
            
        }

        return false;
    }
}
