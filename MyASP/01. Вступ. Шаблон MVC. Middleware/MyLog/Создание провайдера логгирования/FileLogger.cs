using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Создание_провайдера_логгирования
{
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string filePath;
        private static readonly object _lock = new object();

        public FileLogger(string path)
        {
            filePath = path;
        }

        public IDisposable BeginScope<TState>(TState state) => this;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (formatter == null) return;

            string message = formatter(state, exception);

            lock (_lock)
            {
                File.AppendAllText(filePath, message + Environment.NewLine);
            }
        }

        public void Dispose() { }
    }
}
