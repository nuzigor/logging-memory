using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Nuzigor.Extensions.Logging.Memory.Tests;

[TestFixture]
public class LogEntryTests
{
    [Test]
    public void Equals()
    {
        var entry1 = new LogEntry(
           DateTimeOffset.UtcNow,
           "Category",
           LogLevel.Error,
           new EventId(0),
           null,
           "Message",
           new LogState("state message", new KeyValuePair<string, object>[] { new("key1", "value1"), new("key2", 15) }),
           new[] { new LogState("Scope message 1", new KeyValuePair<string, object>[] { new("key11", "value11"), new("key12", 18) }) });
        var entry2 = new LogEntry(
           DateTimeOffset.UtcNow,
           "Category",
           LogLevel.Error,
           new EventId(0),
           null,
           "Message",
           new LogState("state message", new KeyValuePair<string, object>[] { new("key1", "value1"), new("key2", 15) }),
           new[] { new LogState("Scope message 1", new KeyValuePair<string, object>[] { new("key11", "value11"), new("key12", 18) }) });
        Assert.That(entry1, Is.EqualTo(entry2));
    }
}
