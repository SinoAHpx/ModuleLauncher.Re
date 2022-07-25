namespace ModuleLauncher.NET.Tests;

public static class UnitTestsUtils
{
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static T ShouldBe<T>(this T actual, T expected)
    {
        Assert.Equal(actual, expected);
        return actual;
    }
    
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public static T CannotBe<T>(this T actual, T expected)
    {
        Assert.NotEqual(actual, expected);
        return actual;
    }
}