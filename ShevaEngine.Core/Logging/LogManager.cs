using System;
using System.Collections.Generic;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Log manager.
	/// </summary>
	public class LogManager
	{
		public static readonly LogManager Instance = new LogManager();
		private static readonly List<ILogReceiver> _receivers = new List<ILogReceiver>();

		/// <summary>
		/// Add log message.
		/// </summary>		
		public void AddLogMessage(LogMessage message)
		{
			lock (_receivers)
				foreach (ILogReceiver receiver in _receivers)
					receiver.OnNewMessage(message);				
		}

		/// <summary>
		/// Add log receiver.
		/// </summary>		
		public static void AddLogReceiver(ILogReceiver receiver)
		{
			lock (_receivers)
			{
				if (!_receivers.Contains(receiver))
					_receivers.Add(receiver);
			}
		}

		/// <summary>
		/// Remove log receiver.
		/// </summary>		
		public static void RemoveLogReceiver(ILogReceiver receiver)
		{
			lock (_receivers)
			{
				if (_receivers.Contains(receiver))
					_receivers.Remove(receiver);
			}
		}
	}
}
