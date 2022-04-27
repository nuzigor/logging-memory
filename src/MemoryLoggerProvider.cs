using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Nuzigor.Extensions.Logging.Memory
{
    [ProviderAlias("Memory")]
#pragma warning disable CA1812
    internal sealed class MemoryLoggerProvider : ILoggerProvider, ISupportExternalScope
#pragma warning restore CA1812
    {
        private readonly MemoryLoggerSink _memorySink;
        private readonly ConcurrentDictionary<string, MemoryLogger> _loggers = new ConcurrentDictionary<string, MemoryLogger>();
        private IExternalScopeProvider? _scopeProvider;

        public MemoryLoggerProvider(IMemoryLoggerSink memorySink)
        {
            if (memorySink == null)
            {
                throw new ArgumentNullException(nameof(memorySink));
            }

            if (memorySink is MemoryLoggerSink internalMemorySink)
            {
                _memorySink = internalMemorySink;
            }
            else
            {
                throw new ArgumentException("Must be an instance of an internal MemoryLoggerSink", nameof(memorySink));
            }
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.TryGetValue(categoryName, out MemoryLogger? logger)
              ? logger
              : _loggers.GetOrAdd(categoryName, new MemoryLogger(categoryName, _memorySink, _scopeProvider));
        }

        /// <inheritdoc />
        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
            foreach (var logger in _loggers)
            {
                logger.Value.ScopeProvider = scopeProvider;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
