using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace Nuzigor.Extensions.Logging.Memory.Tests;

[TestFixture]
public class MemoryLoggerSinkTests
{
    [Test]
    public void Clear()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddMemory());
        var sp = services.BuildServiceProvider();
        var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        var logger = loggerFactory.CreateLogger("Category1");
        logger.LogInformation("{Arg1}, {Arg2}", 15, "SomeText");

        var anotherLogger = loggerFactory.CreateLogger("Category2");
        anotherLogger.LogError(new InvalidOperationException("bla"), "{ArgOne}", 23);

        Assert.That(memorySink, Is.Not.Null);
        Assert.That(memorySink.Logs, Has.Count.EqualTo(2));

        // Act
        memorySink.Clear();

        // Assert
        Assert.That(memorySink.Logs, Is.Empty);
    }

    [Test]
    public void MultipleLogs()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddMemory());
        var sp = services.BuildServiceProvider();
        var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        // Act
        var logger = loggerFactory.CreateLogger("Category1");
        logger.LogInformation("{Arg1}, {Arg2}", 15, "SomeText");

        var anotherLogger = loggerFactory.CreateLogger("Category2");
        anotherLogger.LogError(new InvalidOperationException("bla"), "{ArgOne}", 23);

        // Assert
        Assert.That(memorySink, Is.Not.Null);
        Assert.That(memorySink.Logs, Has.Count.EqualTo(2));
        var log = memorySink.Logs.ElementAt(0);
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

        Assert.That(log.Scopes, Is.Empty);

        log = memorySink.Logs.ElementAt(1);
        Assert.That(log.EventId.Id, Is.EqualTo(0));
        Assert.That(log.Message, Is.EqualTo("23"));
        Assert.That(log.State.Message, Is.EqualTo("23"));
        Assert.That(log.Category, Is.EqualTo("Category2"));
        Assert.That(log.LogLevel, Is.EqualTo(LogLevel.Error));
        Assert.That(log.Exception, Is.InstanceOf<InvalidOperationException>());
        Assert.That(log.State.Properties, Has.Count.EqualTo(2));

        prop = log.State.Properties.ElementAt(0);
        Assert.That(prop.Key, Is.EqualTo("ArgOne"));
        Assert.That(prop.Value, Is.EqualTo(23));

        prop = log.State.Properties.ElementAt(1);
        Assert.That(prop.Key, Is.EqualTo("{OriginalFormat}"));
        Assert.That(prop.Value, Is.EqualTo("{ArgOne}"));

        Assert.That(log.State.OriginalFormat, Is.EqualTo("{ArgOne}"));

        Assert.That(log.Scopes, Is.Empty);
    }

    [Test]
    public void SingleExtensionScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddMemory());
        var sp = services.BuildServiceProvider();
        var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        // Act
        var logger = loggerFactory.CreateLogger("Category1");
        using (var scope = logger.BeginScope("Scope Arg: {ScopeArg}", "ScopeValue"))
        {
            logger.LogInformation("{Arg1}, {Arg2}", 15, "SomeText");
        }

        // Assert
        Assert.That(memorySink, Is.Not.Null);
        Assert.That(memorySink.Logs, Has.Count.EqualTo(1));
        var log = memorySink.Logs.ElementAt(0);
        Assert.That(log.Message, Is.EqualTo("15, SomeText"));
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
    }

    [Test]
    public void MultipleExtensionScopes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddMemory());
        var sp = services.BuildServiceProvider();
        var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        // Act
        var logger = loggerFactory.CreateLogger("Category1");
        using (var scope1 = logger.BeginScope("Scope1 Arg: {Scope1Arg}", "Scope1Value"))
        using (var scope2 = logger.BeginScope("Scope2 Arg: {Scope2Arg}", "Scope2Value"))
        {
            logger.LogInformation("{Arg1}, {Arg2}", 15, "SomeText");
        }

        // Assert
        Assert.That(memorySink, Is.Not.Null);
        Assert.That(memorySink.Logs, Has.Count.EqualTo(1));
        var log = memorySink.Logs.ElementAt(0);
        Assert.That(log.Message, Is.EqualTo("15, SomeText"));
        Assert.That(log.Scopes, Has.Exactly(2).Items);

        var logScope = log.Scopes.ElementAt(0);
        Assert.That(logScope.Message, Is.EqualTo("Scope1 Arg: Scope1Value"));
        Assert.That(logScope.Properties, Has.Exactly(2).Items);

        logScope = log.Scopes.ElementAt(1);
        Assert.That(logScope.Message, Is.EqualTo("Scope2 Arg: Scope2Value"));
        Assert.That(logScope.Properties, Has.Exactly(2).Items);
    }

    [Test]
    public void LoggersShareScopes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddMemory());
        var sp = services.BuildServiceProvider();
        var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        // Act
        var logger1 = loggerFactory.CreateLogger("Category1");
        var logger2 = loggerFactory.CreateLogger("Category2");
        using (var scope = logger1.BeginScope("Scope Arg: {ScopeArg}", "ScopeValue"))
        {
            logger2.LogInformation("{Arg1}, {Arg2}", 15, "SomeText");
        }

        // Assert
        Assert.That(memorySink, Is.Not.Null);
        Assert.That(memorySink.Logs, Has.Count.EqualTo(1));
        var log = memorySink.Logs.ElementAt(0);
        Assert.That(log.Message, Is.EqualTo("15, SomeText"));
        Assert.That(log.Scopes, Has.Exactly(1).Items);

        var logScope = log.Scopes.ElementAt(0);
        Assert.That(logScope.Message, Is.EqualTo("Scope Arg: ScopeValue"));
    }

    [Test]
    public void SingleObjectScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddMemory());
        var sp = services.BuildServiceProvider();
        var memorySink = sp.GetRequiredService<IMemoryLoggerSink>();
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        // Act
        var logger = loggerFactory.CreateLogger("Category1");
        var props = new KeyValuePair<string, object>[]
        {
            new("Prop1", "value1"),
            new("Prop2", 16)
        };

        using (var scope = logger.BeginScope(new ObjectScope(props)))
        {
            logger.LogInformation("{Arg1}, {Arg2}", 15, "SomeText");
        }

        // Assert
        Assert.That(memorySink, Is.Not.Null);
        Assert.That(memorySink.Logs, Has.Count.EqualTo(1));
        var log = memorySink.Logs.ElementAt(0);
        Assert.That(log.Message, Is.EqualTo("15, SomeText"));
        Assert.That(log.Scopes, Has.Exactly(1).Items);

        var logScope = log.Scopes.ElementAt(0);
        Assert.That(logScope.Message, Is.EqualTo("Prop1: value1, Prop2: 16"));

        Assert.That(logScope.Properties, Has.Exactly(2).Items);

        var prop = logScope.Properties.ElementAt(0);
        Assert.That(prop.Key, Is.EqualTo("Prop1"));
        Assert.That(prop.Value, Is.EqualTo("value1"));

        prop = logScope.Properties.ElementAt(1);
        Assert.That(prop.Key, Is.EqualTo("Prop2"));
        Assert.That(prop.Value, Is.EqualTo(16));

        Assert.That(logScope.OriginalFormat, Is.Empty);
    }

    class ObjectScope : IReadOnlyCollection<KeyValuePair<string, object>>
    {
        private readonly KeyValuePair<string, object>[] properties;

        public ObjectScope(IEnumerable<KeyValuePair<string, object>> properties)
        {
            this.properties = properties?.ToArray() ?? throw new ArgumentNullException(nameof(properties));
            if (this.properties.Length == 0)
            {
                throw new ArgumentException("Must have at least one element", nameof(properties));
            }
        }

        public int Count => properties.Length;

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => properties.AsEnumerable().GetEnumerator();

        public override string ToString()
        {
            var message = $"{properties[0].Key}: {properties[0].Value}";
            for (int i = 1; i < properties.Length; i++)
            {
                message += $", {properties[i].Key}: {properties[i].Value}";
            }
            return message;
        }

        IEnumerator IEnumerable.GetEnumerator() => properties.GetEnumerator();
    }
}

