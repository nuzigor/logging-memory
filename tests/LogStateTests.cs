using System.Collections.Generic;
using NUnit.Framework;

namespace Nuzigor.Extensions.Logging.Memory.Tests;

[TestFixture]
public class LogStateTests
{
    [Test]
    public void Deconstruct()
    {
        var state = new LogState("text", new[] { new KeyValuePair<string, object>("name", "value") });
        var (message, properties) = state;
        Assert.That(message, Is.EqualTo("text"));
        Assert.That(properties, Has.Exactly(1).Items);
    }

    [Test]
    public void OriginalFormat_When_NotPresent()
    {
        var state = new LogState("text", new[] { new KeyValuePair<string, object>("name", "value") });
        Assert.That(state.OriginalFormat, Is.Empty);
    }

    [Test]
    public void OriginalFormat()
    {
        var state = new LogState("text", new[] { new KeyValuePair<string, object>("name", "value"), new KeyValuePair<string, object>("{OriginalFormat}", "format") });
        Assert.That(state.Properties, Has.Exactly(2).Items);
        Assert.That(state.OriginalFormat, Is.EqualTo("format"));
    }

    [Test]
    public void Equals()
    {
        var state1 = new LogState("text", new[] { new KeyValuePair<string, object>("name", "value"), new KeyValuePair<string, object>("name2", 15) });
        var state2 = new LogState("text", new[] { new KeyValuePair<string, object>("name", "value"), new KeyValuePair<string, object>("name2", 15) });
        Assert.That(state1, Is.EqualTo(state2));
    }

    [Test]
    public void NotEquals()
    {
        var state1 = new LogState("text", new[] { new KeyValuePair<string, object>("name", "value"), new KeyValuePair<string, object>("name2", 15) });
        var state2 = new LogState("text1", new[] { new KeyValuePair<string, object>("name", "value"), new KeyValuePair<string, object>("name2", 15) });
        var state3 = new LogState("text", new[] { new KeyValuePair<string, object>("name", "value"), new KeyValuePair<string, object>("name3", 15) });
        var state4 = new LogState("text", new[] { new KeyValuePair<string, object>("name", "value") });
        Assert.That(state1, Is.Not.EqualTo(state2));
        Assert.That(state1, Is.Not.EqualTo(state3));
        Assert.That(state1, Is.Not.EqualTo(state4));
    }
}
