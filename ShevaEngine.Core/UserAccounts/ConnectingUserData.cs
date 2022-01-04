using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core.UserAccounts
{
    public class ConnectingUserData : IUserData
    {
        public string GamerName => "Connecting";

        public Texture2D? GamerPicture => null;
    }
}
