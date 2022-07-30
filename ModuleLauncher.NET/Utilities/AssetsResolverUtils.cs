using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;

namespace ModuleLauncher.NET.Utilities;

public static class AssetsResolverUtils
{
    /// <summary>
    /// Get assets, will automatically download if no assets index exists 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static List<AssetEntry> GetAssets(this MinecraftEntry minecraftEntry)
    {
        return AssetsResolver.GetAssets(minecraftEntry);
    }

    /// <summary>
    /// Get assets without automatic download
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static async Task<List<AssetEntry>> GetAssetsAsync(this MinecraftEntry minecraftEntry)
    {
        return await AssetsResolver.GetAssetsAsync(minecraftEntry);
    } 
}