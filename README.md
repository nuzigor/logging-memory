# logging-memory

In-memory implementation of `Microsoft.Extensions.Logging.ILoggerProvider`.

The purpose of this library is to capture and check log messages in tests.

## Design principles

- Full support of `ISupportExternalScope`
- Capture both rendered messages and state properties
- Use the same data structure for both log and scope states
- Capture all scopes and their properties for each log entry
- Expose `OriginalFormat` as an additional property

## Typical usage

First:

```c#
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Nuzigor.Extensions.Logging.Memory;
```

And then in a test method:

```c#
var services = new ServiceCollection();

// Add memory logger provider
services.AddLogging(builder => builder.AddMemory());

var sp = services.BuildServiceProvider();

var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

var logger = loggerFactory.CreateLogger("Category1");
using (var scope = logger.BeginScope("Scope Arg: {ScopeArg}", "ScopeValue"))
{
    logger.LogInformation("{Arg1}, {Arg2}", 15, "SomeText");
}

// Get the service to access captured logs
var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();

Assert.That(memorySink.Logs, Has.Count.EqualTo(1));
var log = memorySink.Logs.ElementAt(0);

Assert.That(log.Message, Is.EqualTo("15, SomeText"));
Assert.That(log.EventId.Id, Is.EqualTo(0));
Assert.That(log.Message, Is.EqualTo("15, SomeText"));
Assert.That(log.State.Message, Is.EqualTo("15, SomeText"));
Assert.That(log.Category, Is.EqualTo("Category1"));
Assert.That(log.LogLevel, Is.EqualTo(LogLevel.Information));
Assert.That(log.Exception, Is.Null);
Assert.That(log.State.Properties, Has.Count.EqualTo(3));

var prop = log.State.Properties.ElementAt(0);
Assert.That(prop.Key, Is.EqualTo("Arg1"));
Assert.That(prop.Value, Is.EqualTo(15));

prop = log.State.Properties.ElementAt(1);
Assert.That(prop.Key, Is.EqualTo("Arg2"));
Assert.That(prop.Value, Is.EqualTo("SomeText"));

prop = log.State.Properties.ElementAt(2);
Assert.That(prop.Key, Is.EqualTo("{OriginalFormat}"));
Assert.That(prop.Value, Is.EqualTo("{Arg1}, {Arg2}"));

Assert.That(log.State.OriginalFormat, Is.EqualTo("{Arg1}, {Arg2}"));

Assert.That(log.Scopes, Has.Exactly(1).Items);

var logScope = log.Scopes.ElementAt(0);
Assert.That(logScope.Message, Is.EqualTo("Scope Arg: ScopeValue"));

Assert.That(logScope.Properties, Has.Exactly(2).Items);

var prop = logScope.Properties.ElementAt(0);
Assert.That(prop.Key, Is.EqualTo("ScopeArg"));
Assert.That(prop.Value, Is.EqualTo("ScopeValue"));

prop = logScope.Properties.ElementAt(1);
Assert.That(prop.Key, Is.EqualTo("{OriginalFormat}"));
Assert.That(prop.Value, Is.EqualTo("Scope Arg: {ScopeArg}"));

Assert.That(logScope.OriginalFormat, Is.EqualTo("Scope Arg: {ScopeArg}"));
```
