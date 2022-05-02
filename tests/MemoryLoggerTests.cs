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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace Nuzigor.Extensions.Logging.Memory.Tests;

[TestFixture]
public class MemoryLoggerTests
{
    [Test]
    public void IsEnabled()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddMemory());
        var sp = services.BuildServiceProvider();
        var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        var logger = loggerFactory.CreateLogger("Category1");

        Assert.That(logger.IsEnabled(LogLevel.Error), Is.True);
        Assert.That(logger.IsEnabled(LogLevel.None), Is.False);
    }
}
