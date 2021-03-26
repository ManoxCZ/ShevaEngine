using Microsoft.Xna.Framework.Graphics;

namespace ShevaEngine.UserAccounts
{
	/// <summary>
	/// User data.
	/// </summary>
	public class UserData
	{
        public static UserData Connecting = new UserData();
        
        public string GamerName { get; internal set; }
        public Texture2D GamerPicture { get; internal set; }
	}
}
