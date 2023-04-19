using PlayFab;
using PlayFab.ClientModels;
using ShevaEngine.Core.UserAccounts;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ShevaEngine.Playfab;

public class PlayfabUser : IUser
{
    public static string TITLE_ID = "";


    public BehaviorSubject<IUserData?> UserData { get; } = new(null);


    public async Task<bool> ConnectToService(bool silently = false)
    {
        UserData.OnNext(ConnectingUserData.Instance);

        PlayFabSettings.staticSettings.TitleId = TITLE_ID;

        LoginWithCustomIDRequest request = new()
        {
            CreateAccount = true,
            CustomId = "User"
        };

        //LoginWithPlayFabRequest request2 = new()
        //{
        //    Username = "ManoxCZ",
        //    TitleId = TITLE_ID,
        //    Password = "passwd",            
        //};

        //if (await PlayFabClientAPI.LoginWithCustomIDAsync(request) is PlayFabResult<LoginResult> result)
        //{
        //    GetUserDataRequest userDataRequest = new()
        //    {
        //        PlayFabId = result.Result.PlayFabId
        //    };

        //    if (await PlayFabClientAPI.GetUserDataAsync(userDataRequest) is PlayFabResult<GetUserDataResult> userDataResult)
        //    {
        //       // UserData.OnNext(new PlayfabUserData(userDataResult));
        //    }

        //    return true;
        //}

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

    public void Dispose()
    {
        
    }
}
