using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.Core.UserAccounts
{
    public interface IUserData
    {        
        public string GamerName { get; }
        public Texture2D? GamerPicture { get; }
    }
}
