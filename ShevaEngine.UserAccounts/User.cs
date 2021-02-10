using System.Threading.Tasks;

namespace ShevaEngine.UserAccounts
{
	/// <summary>
	/// User.
	/// </summary>
	public abstract class User
	{
		public string Name { get; protected set; }
		


		/// <summary>
		/// Get user data.
		/// </summary>		
		public abstract Task<T> GetUserData<T>() where T: UserData;
	}
}
