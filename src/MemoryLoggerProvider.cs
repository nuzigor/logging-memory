// Copyright 2022 Igor Nuzhnov
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
