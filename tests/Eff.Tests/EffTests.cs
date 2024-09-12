namespace Nessos.Effects.Tests;

using Xunit;

public static class EffTests
{
    [Fact]
    public static void ConfigureAwait_ShouldAddCallerInfo()
    {
        var eff = Test();

        var awaiter = eff.ConfigureAwait();

        Assert.True(awaiter.CallerMemberName?.Length > 0);
        Assert.True(awaiter.CallerFilePath?.Length > 0);
        Assert.True(awaiter.CallerLineNumber > 0);

        async Eff<int> Test() => 42;
    }

    [Fact]
    public static void ConfigureAwait_Untyped_ShouldAddCallerInfo()
    {
        var eff = Test();

        var awaiter = eff.ConfigureAwait();

        Assert.True(awaiter.CallerMemberName?.Length > 0);
        Assert.True(awaiter.CallerFilePath?.Length > 0);
        Assert.True(awaiter.CallerLineNumber > 0);

        async Eff Test() { };
    }
}
