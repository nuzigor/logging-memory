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
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Nuzigor.Extensions.Logging.Memory
{
    /// <summary>
    /// A captured log entry.
    /// </summary>
    public sealed class LogEntry
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public LogEntry(
            DateTimeOffset timeOffset,
            string category,
            LogLevel logLevel,
            EventId eventId,
            Exception? exception,
            string message,
            LogState state,
            IReadOnlyCollection<LogState> scopes)
        {
            TimeOffset = timeOffset;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            LogLevel = logLevel;
            EventId = eventId;
            Exception = exception;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            State = state ?? throw new ArgumentNullException(nameof(state));
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }

        /// <summary>
        /// The time when the entry was created.
        /// </summary>
        /// <remarks> Does not participate in Equals and GetHashCode. </remarks>
        public DateTimeOffset TimeOffset { get; }

        /// <summary>
        /// The category name for messages produced by the corresponding logger.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Entry was written on this level.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// Id of the event.
        /// </summary>
        public EventId EventId { get; }

        /// <summary>
        /// The exception related to this entry.
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// The formatted message for this log entry.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// State for this log entry.
        /// </summary>
        public LogState State { get; }

        /// <summary>
        /// The scopes for this log entry.
        /// </summary>
        public IReadOnlyCollection<LogState> Scopes { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is LogEntry entry &&
                   Category == entry.Category &&
                   LogLevel == entry.LogLevel &&
                   EqualityComparer<EventId>.Default.Equals(EventId, entry.EventId) &&
                   EqualityComparer<Exception?>.Default.Equals(Exception, entry.Exception) &&
                   Message == entry.Message &&
                   EqualityComparer<LogState>.Default.Equals(State, entry.State) &&
                   Scopes.SequenceEqual(entry.Scopes);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = -78854121;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Category);
            hashCode = hashCode * -1521134295 + LogLevel.GetHashCode();
            hashCode = hashCode * -1521134295 + EventId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Exception?>.Default.GetHashCode(Exception);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<LogState>.Default.GetHashCode(State);
            hashCode = hashCode * -1521134295 + EqualityComparer<IReadOnlyCollection<LogState>>.Default.GetHashCode(Scopes);
            return hashCode;
        }
    }
}

