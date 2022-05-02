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
