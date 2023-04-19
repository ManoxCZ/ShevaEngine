using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core.UserAccounts
{
    public class ConnectingUserData : IUserData
    {
        public static ConnectingUserData Instance = new();


        public string GamerName => "Connecting";

        public Texture2D? GamerPicture => null;
    }
}
