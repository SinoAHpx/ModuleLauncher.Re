using Manganese.Text;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Resources;

namespace ModuleLauncher.NET.Example.Utils;

public static class DataBus
{
    //use this backing-field style property, in case we need to do data persist
    
    private static AuthenticateResult _authenticateResult;

    public static AuthenticateResult AuthenticateResult
    {
        get => _authenticateResult;
        set => _authenticateResult = value;
    }

    public static string? MinecraftRootPath { get; set; }

    public static string? MinecraftWorkingPath { get; set; }

    public static MinecraftResolver? MinecraftResolver =>
        MinecraftRootPath.IsNullOrEmpty() ? null : new MinecraftResolver(MinecraftRootPath);
}