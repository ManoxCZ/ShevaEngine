using Microsoft.Xna.Framework.Graphics;
using PlayFab;
using PlayFab.ClientModels;
using ShevaEngine.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShevaEngine.Playfab;

public class PlayfabUserData : IUserData
{
    private readonly PlayFabAuthenticationContext _authenticationContext;
    private Dictionary<string, UserDataRecord>? _userData;
    
    public string GamerName 
    {
        get => _userData?[PlayfabUserDataType.GamerName.ToString()]?.Value?.ToString() ?? "No data";
        set
        {
            UpdateUserInfoAsync(PlayfabUserDataType.GamerName, value);            
        } 
    }

    public Texture2D? GamerPicture 
    { 
        get => throw new NotImplementedException(); 
        set => throw new NotImplementedException(); 
    }


    public PlayfabUserData(PlayFabAuthenticationContext authenticationContext)
    {
        _authenticationContext = authenticationContext;        
    }

    public async Task<bool> GetData()
    {
        GetUserDataRequest userDataRequest = new()
        {
            PlayFabId = _authenticationContext.PlayFabId
        };
        
        if (await PlayFabClientAPI.GetUserDataAsync(userDataRequest) is PlayFabResult<GetUserDataResult> userDataResult)
        {
            _userData = userDataResult.Result.Data;

            return true;
        }

        return false;
    }

    private async Task<bool> UpdateUserInfoAsync(PlayfabUserDataType dataType, string value)
    {
        UpdateUserDataRequest request = new()
        {
            AuthenticationContext = _authenticationContext,
            Data = new Dictionary<string, string>()
            {
                {dataType.ToString(), value}
            }
        };

        if (await PlayFabClientAPI.UpdateUserDataAsync(request) is PlayFabResult<UpdateUserDataResult> result)
        {
            if (_userData is not null &&
                _userData.ContainsKey(dataType.ToString()))
            {
                _userData[dataType.ToString()] = new UserDataRecord()
                {
                    LastUpdated = DateTime.Now,
                    Value = value
                };
            }

            return result.Error is null;
        }
        
        return false;
    }
}
