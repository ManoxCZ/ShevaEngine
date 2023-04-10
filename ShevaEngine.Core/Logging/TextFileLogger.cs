using Microsoft.Extensions.Logging;
using System;

namespace ShevaEngine.Core
{

    public class TextFileLogger : ILogger
    {
        private readonly TextFileLogReceiver _receiver;
        private readonly string _category;


        public TextFileLogger(TextFileLogReceiver receiver, string category)
        {
            _receiver = receiver;

            string[] parts = category.Split('.');
            _category = parts[^1];
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel) => true;


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _receiver.OnNewMessage(logLevel, _category, eventId, state, exception, formatter);
        }
    }
}
