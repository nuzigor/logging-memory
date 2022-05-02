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
public class MemoryLoggerProviderTests
{
    [Test]
    public void CreateLogger_CachesSameCategory()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddMemory());
        var sp = services.BuildServiceProvider();
        var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        var logger1 = loggerFactory.CreateLogger("Category1");
        var logger2 = loggerFactory.CreateLogger("Category1");
        var logger3 = loggerFactory.CreateLogger("Category2");

        Assert.That(logger1, Is.EqualTo(logger2));
        Assert.That(logger2, Is.Not.EqualTo(logger3));
    }
}
