using Epic.OnlineServices.UserInfo;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core.UserAccounts;

namespace ShevaEngine.EOS
{
    public class EOSUserData : IUserData
    {
        public string GamerName { get; }
        public Texture2D? GamerPicture { get; } = null;


        public EOSUserData(UserInfoData userInfoData)
        {
            GamerName = userInfoData.DisplayName;
            
        }
    }
}
