using System;
using System.Threading.Tasks;

namespace ShevaEngine.UserAccounts
{
	/// <summary>
	/// Local User.
	/// </summary>
	public class LocalUser : User
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		public LocalUser()
			: base()
		{			
			//Name = Environment.UserName;			
		}

		/// <summary>
		/// Get user data.
		/// </summary>		
		public override Task<T> GetUserData<T>() 
		{
			return null;
		}
	}
}
