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

namespace Nuzigor.Extensions.Logging.Memory
{
    /// <summary>
    /// Represents a captured logger state.
    /// </summary>
    public sealed class LogState
    {
        /// <summary>
        /// Empty state.
        /// </summary>
        public static LogState Empty { get; } = new LogState(string.Empty, Array.Empty<KeyValuePair<string, object>>());

        /// <summary>
        /// Public constructor.
        /// </summary>
        public LogState(string message, IReadOnlyCollection<KeyValuePair<string, object>> properties)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            OriginalFormat = Properties.LastOrDefault(x => x.Key == "{OriginalFormat}").Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// The formatted message of the state captured.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// A list of <see cref="KeyValuePair{TKey, TValue}"/> that represents the properties of the captured state.
        /// </summary>
        /// <remarks>Properties are in the same order as in the original state and multiple properties can have the same key.</remarks>
        public IReadOnlyCollection<KeyValuePair<string, object>> Properties { get; }

        /// <summary>
        /// Returns the original format string passed into Log* or BeginScope extension methods
        /// </summary>
        public string OriginalFormat { get; }

        /// <summary>
        /// <see cref="ValueType"/> support.
        /// </summary>
        public void Deconstruct(out string message, out IReadOnlyCollection<KeyValuePair<string, object>> properties)
        {
            message = Message;
            properties = Properties;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is LogState otherState &&
                   Message == otherState.Message &&
                   Properties.SequenceEqual(otherState.Properties);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = -1657424864;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<IReadOnlyCollection<KeyValuePair<string, object>>>.Default.GetHashCode(Properties);
            return hashCode;
        }
    }
}
