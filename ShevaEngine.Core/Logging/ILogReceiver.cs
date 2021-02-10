namespace ShevaEngine.Core
{
	public interface ILogReceiver
	{
		/// <summary>
		/// On new message.
		/// </summary>		
		void OnNewMessage(LogMessage message);		
	}
}
