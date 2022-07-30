using Manganese.Text;
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

    public static void RefreshAssetIndex(this MinecraftEntry minecraftEntry)
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    internal static (string AssetIndex, string AssetUrl) GetAssetIndexMetadata(this MinecraftEntry minecraftEntry)
    {
        if (!minecraftEntry.Json.AssetId.IsNullOrEmpty() && !minecraftEntry.Json.AssetIndexUrl.IsNullOrEmpty())
            return (minecraftEntry.Json.AssetId, minecraftEntry.Json.AssetIndexUrl);

        if (minecraftEntry.HasInheritSource())
        {
            var inheritSource = minecraftEntry
                .GetInheritSource()
                .ThrowCorruptedIfNull("Missing inherit Minecraft");
            return (inheritSource.Json.AssetId
                .ThrowCorruptedIfNull(), inheritSource.Json.AssetIndexUrl.ThrowCorruptedIfNull());
        }

        return ("legacy",
            "https://launchermeta.mojang.com/v1/packages/770572e819335b6c0a053f8378ad88eda189fc14/legacy.json");
    }
}