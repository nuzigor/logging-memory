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
