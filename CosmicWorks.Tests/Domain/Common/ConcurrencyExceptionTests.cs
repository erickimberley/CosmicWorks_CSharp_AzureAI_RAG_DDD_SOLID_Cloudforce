using CosmicWorks.Domain.Common;
using Xunit;

namespace CosmicWorks.Tests.Domain.Common;

public class ConcurrencyExceptionTests
{
    [Fact]
    public void MessageCtor_Sets_Message()
    {
        var ex = new ConcurrencyException("boom");
        Assert.IsAssignableFrom<Exception>(ex);
        Assert.Contains("boom", ex.Message);
    }

    [Fact]
    public void MessageInnerCtor_Sets_Inner()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new ConcurrencyException("wrap", inner);
        Assert.Same(inner, ex.InnerException);
        Assert.Contains("wrap", ex.Message);
    }
}