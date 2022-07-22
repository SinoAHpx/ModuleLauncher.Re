using Spectre.Console;

namespace ModuleLauncher.NET.Runtime;

public static class Logger
{
    public static T Log<T>(this T t, string appendix = "", Func<T, string?>? toString = null)
    {
        var tStr = toString == null ? t.ToString() : toString(t);
        AnsiConsole.MarkupLine($"[blue][[LOG]][/] {appendix}{tStr}");
        return t;
    }
    
    public static T Warn<T>(this T t, string appendix = "")
    {
        AnsiConsole.MarkupLine($"[blue][[WARN]][/] {appendix}{t}");
        return t;
    }
    
    public static T Error<T>(this T t, string appendix = "")
    {
        AnsiConsole.MarkupLine($"[blue][[ERROR]][/] {appendix}{t.ToString().EscapeMarkup()}");
        return t;
    }
    
    public static T Error<T>(this T t, Exception ex, string appendix = "")
    {
        AnsiConsole.MarkupLine($"[blue][[ERROR]][/] {appendix}{t}");
        AnsiConsole.WriteException(ex);
        return t;
    }
}