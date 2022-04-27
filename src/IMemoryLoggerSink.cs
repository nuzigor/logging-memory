using System.Collections.Generic;

namespace Nuzigor.Extensions.Logging.Memory
{
    /// <summary>
    /// Represents a type used to get access to the captured logs.
    /// </summary>
    public interface IMemoryLoggerSink
    {
        /// <summary>
        /// The captured logs.
        /// </summary>
        IReadOnlyCollection<LogEntry> Logs { get; }

        /// <summary>
        /// Clear the captured logs.
        /// </summary>
        void Clear();
    }
}
