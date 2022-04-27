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
