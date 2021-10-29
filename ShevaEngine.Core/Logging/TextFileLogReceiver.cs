using Microsoft.Extensions.Logging;
using System;

namespace ShevaEngine.Core
{
    public class TextFileLogReceiver : ILoggerProvider, IDisposable
    {        
#if WINDOWS_UAP
        private Windows.Foundation.Diagnostics.FileLoggingSession _session;
        private Windows.Foundation.Diagnostics.LoggingChannel _channel;
#else
        private object _lock = new object();
        private string _filename = null!;
#endif


        /// <summary>
        /// Constructor.
        /// </summary>		
        public TextFileLogReceiver()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            if (ShevaServices.GetService<IFileSystemService>() is IFileSystemService service)
            {                
#if WINDOWS_UAP
                _channel = new Windows.Foundation.Diagnostics.LoggingChannel("General", new Windows.Foundation.Diagnostics.LoggingChannelOptions());

                _session = new Windows.Foundation.Diagnostics.FileLoggingSession("Game");
#if DEBUG
                _session.AddLoggingChannel(_channel, Windows.Foundation.Diagnostics.LoggingLevel.Verbose);
#else
                _session.AddLoggingChannel(_channel, Windows.Foundation.Diagnostics.LoggingLevel.Information);
#endif
#else
                _filename = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    service.UserDataPath,
                    $"game.log");

                if (service.FileExists(_filename))
                    service.DeleteFile(_filename);
#endif
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
#if WINDOWS_UAP     
            _channel?.Dispose();
            _channel = null;

            _session.CloseAndSaveToFileAsync().AsTask().Wait();

            _session?.Dispose();
            _session = null;
#endif
        }


        /// <summary>
        /// On new message.
        /// </summary>		
        public void OnNewMessage<TState>(LogLevel logLevel, string category, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (_lock)
            {
#if WINDOWS_UAP
                Windows.Foundation.Diagnostics.LoggingFields fields = new Windows.Foundation.Diagnostics.LoggingFields();
                fields.AddString("Source", message.Origin);            
                fields.AddString("Message", message.Message);
            
                if (message.Exception != null)
                    fields.AddString("Exception", message.Exception.ToString());
            
                _channel.LogEvent("Game", fields, GetLoggingLevel(message));
#else
                string formattedMessage = $"{DateTime.Now}\t{logLevel}\t{category}\t{formatter(state, exception)}\t{exception?.ToString()}\n";

                System.IO.File.AppendAllText(_filename, formattedMessage);
#endif
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TextFileLogger(this, categoryName);
        }

#if WINDOWS_UAP
        /// <summary>
        /// Get correct logging level.
        /// </summary>
        private Windows.Foundation.Diagnostics.LoggingLevel GetLoggingLevel(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Debug:
                    return Windows.Foundation.Diagnostics.LoggingLevel.Verbose;
                case LogSeverity.Info:
                    return Windows.Foundation.Diagnostics.LoggingLevel.Information;
                case LogSeverity.Warning:
                    return Windows.Foundation.Diagnostics.LoggingLevel.Warning;
                case LogSeverity.Error:
                    return Windows.Foundation.Diagnostics.LoggingLevel.Error;
                default:
                    return Windows.Foundation.Diagnostics.LoggingLevel.Verbose;
            }
        }
#endif
    }
}
