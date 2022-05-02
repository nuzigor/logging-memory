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
