using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace ConfigurationExtensions.Tests;
public static class NotNullExtensions
{
    [return: NotNull]
    public static T NotNull<T>(this T self)
    {
        Assert.NotNull(self);
        return self!;
    }
}

public static class Ignore
{
    public static void IfThrows(Action operation, string reason)
    {
        try
        {
            operation.Invoke();
        }
        catch (XunitException)
        {
            throw;
        }
        catch (Exception e)
        {
            // We could do a lot to massage the exception reason/stack trace, but it's fine.
            throw new SkipException($"Known Bug: {reason}\n{e.GetType().Name}: {e}");
        }

        throw new XunitException($"Did not encounter expected bug {reason}. Update this test");
    }
}
