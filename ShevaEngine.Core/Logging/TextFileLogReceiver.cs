using Microsoft.Extensions.Logging;
using System;

namespace ShevaEngine.Core
{
    public class TextFileLogReceiver : ILoggerProvider
    {        
        private object _lock = new();
        private string _filename = null!;


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
                _filename = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    service.UserDataPath,
                    $"game.log");

                if (service.FileExists(_filename))
                {
                    service.DeleteFile(_filename);
                }
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// On new message.
        /// </summary>		
        public void OnNewMessage<TState>(LogLevel logLevel, string category, EventId eventId, 
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            lock (_lock)
            {
                string formattedMessage = $"{DateTime.Now}\t{logLevel}\t{category}\t{formatter(state, exception)}\t{exception?.ToString()}\n";

                System.IO.File.AppendAllText(_filename, formattedMessage);
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TextFileLogger(this, categoryName);
        }
    }
}
