using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.UserAccounts;
using System;

namespace ShevaEngine.Playfab;

public class PlayfabUserData : IUserData
{
    public string GamerName => throw new NotImplementedException();

    public Texture2D? GamerPicture => throw new NotImplementedException();
}
