using System;

namespace ShevaEngine.Core
{
	/// <summary>
	/// Log.
	/// </summary>
	public class Log
	{
		public string _origin;


		/// <summary>
		/// Constructor.
		/// </summary>		
		public Log(Type type)
		{
			_origin = type.Name;
		}

		/// <summary>
		/// Constructor.
		/// </summary>		
		public Log(Type type, string name)
		{
			_origin = type.Name + "-" + name;
		}

		/// <summary>
		/// Debug.
		/// </summary>		
		public void Debug(string message, Exception exception = null)
		{
#if DEBUG
			LogManager.Instance.AddLogMessage(new LogMessage
			{
				DateTime = DateTime.Now,
				Exception = exception,
				Message = message,
				Origin = _origin,
				Severity = LogSeverity.Debug
			});
#endif
		}

		/// <summary>
		/// Info.
		/// </summary>		
		public void Info(string message, Exception exception = null)
		{
			LogManager.Instance.AddLogMessage(new LogMessage
			{
				DateTime = DateTime.Now,
				Exception = exception,
				Message = message,
				Origin = _origin,
				Severity = LogSeverity.Info
			});
		}

		/// <summary>
		/// Warning.
		/// </summary>		
		public void Warning(string message, Exception exception = null)
		{
			LogManager.Instance.AddLogMessage(new LogMessage
			{
				DateTime = DateTime.Now,
				Exception = exception,
				Message = message,
				Origin = _origin,
				Severity = LogSeverity.Warning
			});
		}

		/// <summary>
		/// Error.
		/// </summary>		
		public void Error(string message, Exception exception = null)
		{
			LogManager.Instance.AddLogMessage(new LogMessage
			{
				DateTime = DateTime.Now,
				Exception = exception,
				Message = message,
				Origin = _origin,
				Severity = LogSeverity.Error
			});
		}
	}
}
