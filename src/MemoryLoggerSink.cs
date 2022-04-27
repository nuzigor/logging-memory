using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Nuzigor.Extensions.Logging.Memory
{
    internal sealed class MemoryLoggerSink : IMemoryLoggerSink
    {
        private ConcurrentQueue<LogEntry> _logs = new ConcurrentQueue<LogEntry>();

        public void Write<TState>(LogLevel logLevel, string category, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter, IExternalScopeProvider? scopeProvider)
        {
            string message = formatter(state, exception);
            var now = DateTimeOffset.Now;
            var logState = state == null ? LogState.Empty : CreateLogState(state);
            var scopesArray = Array.Empty<LogState>();
            if (scopeProvider != null)
            {
                var scopes = new List<LogState>();
                scopeProvider.ForEachScope((scope, passedState) => passedState.Add(CreateLogState(scope)), scopes);
                if (scopes.Count > 0)
                {
                    scopesArray = scopes.ToArray();
                }
            }

            var logEntry = new LogEntry(now, category, logLevel, eventId, exception, message, logState, scopesArray);
            _logs.Enqueue(logEntry);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _logs = new ConcurrentQueue<LogEntry>();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<LogEntry> Logs => _logs.ToArray();

        private LogState CreateLogState(object state)
        {
            var message = Convert.ToString(state, CultureInfo.InvariantCulture) ?? string.Empty;
            var itemsArray = Array.Empty<KeyValuePair<string, object>>();
            if (state is IEnumerable<KeyValuePair<string, object>> items)
            {
                itemsArray = items.ToArray();
            }

            return new LogState(message, itemsArray);
        }
    }
}
