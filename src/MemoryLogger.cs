using System;
using Microsoft.Extensions.Logging;

namespace Nuzigor.Extensions.Logging.Memory
{
    internal sealed class MemoryLogger : ILogger
    {
        private readonly string _name;
        private readonly MemoryLoggerSink _memorySink;

        public MemoryLogger(string name, MemoryLoggerSink memorySink, IExternalScopeProvider? scopeProvider)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _memorySink = memorySink ?? throw new ArgumentNullException(nameof(memorySink));
            ScopeProvider = scopeProvider;
        }

        internal IExternalScopeProvider? ScopeProvider { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            _memorySink.Write(logLevel, _name, eventId, state, exception, formatter, ScopeProvider);
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}
