using PlayFab;
using PlayFab.ClientModels;
using ShevaEngine.Core.UserAccounts;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ShevaEngine.Playfab;

public class PlayfabUser : IUser
{
    public static string TITLE_ID = "";


    public BehaviorSubject<IUserData?> UserData { get; } = new(null);


    public async Task<bool> ConnectToService(bool silently = false)
    {
        PlayFabSettings.staticSettings.TitleId = TITLE_ID;

        LoginWithCustomIDRequest request = new()
        {
            CreateAccount = true,
            CustomId = "User" 
        };

        if (await PlayFabClientAPI.LoginWithCustomIDAsync(request) is PlayFabResult<LoginResult> result)
        {
            GetUserDataRequest userDataRequest = new()
            {
                PlayFabId = result.Result.PlayFabId
            };

            if (await PlayFabClientAPI.GetUserDataAsync(userDataRequest) is PlayFabResult<GetUserDataResult> userDataResult)
            {

            }

            return true;
        }

        return false;
    }

    public void Dispose()
    {
        
    }
}
