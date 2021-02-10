using System;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Log severity.
	/// </summary>
	public enum LogSeverity
	{
		Debug,
		Info,
		Warning,
		Error
	}

	/// <summary>
	/// Log message.
	/// </summary>
	public class LogMessage
	{
		public DateTime DateTime { get; set; }
		public LogSeverity Severity { get; set; }
		public string Origin { get; set; }
		public string Message { get; set; }
		public Exception Exception { get; set; }
	}
}
