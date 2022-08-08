using Flurl.Http;
using Manganese.Array;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;

namespace ModuleLauncher.NET.Utilities;

public static class AssetsResolverUtils
{
    /// <summary>
    /// Get assets without automatic download
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static List<AssetEntry> GetAssets(this MinecraftEntry minecraftEntry)
    {
        return AssetsResolver.GetAssets(minecraftEntry);
    }

    /// <summary>
    /// Get assets, will automatically download if no assets index exists 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static async Task<List<AssetEntry>> GetAssetsAsync(this MinecraftEntry minecraftEntry)
    {
        return await AssetsResolver.GetAssetsAsync(minecraftEntry);
    }

    /// <summary>
    /// Refresh assets file since it might be updated someday
    /// </summary>
    /// <param name="minecraftEntry"></param>
    public static async Task RefreshAssetIndexAsync(this MinecraftEntry minecraftEntry)
    {
        var assetIndex = minecraftEntry.GetAssetIndexMetadata();
        var assetIndexJson = await assetIndex.AssetUrl.GetStringAsync();

        var assetIndexFile = minecraftEntry.Tree.AssetsIndexes.DiveToFile($"{assetIndex.AssetIndex}.json");
        await assetIndexFile.WriteAllTextAsync(assetIndexJson);
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

    /// <summary>
    /// Get assets and map them to resources or virtual/legacy
    /// <remarks>If certain entry missing, it will be skipped, this method will be invoked in Launcher class</remarks>
    /// </summary>
    /// <param name="minecraftEntry"></param>
    public static async Task MapAssetsAsync(this MinecraftEntry minecraftEntry)
    {
        var assets = await minecraftEntry.GetAssetsAsync();
        MapAssets(assets, minecraftEntry);
    }

    /// <summary>
    /// Get assets and map them to resources or virtual/legacy
    /// <remarks>If certain entry missing, it will be skipped, this method will be invoked in Launcher class</remarks>
    /// </summary>
    /// <param name="minecraftEntry"></param>
    public static void MapAssets(this MinecraftEntry minecraftEntry)
    {
        var assets = minecraftEntry.GetAssets();
        MapAssets(assets, minecraftEntry);
    }


    /// <summary>
    /// Internal implementation of MapAssets
    /// </summary>
    /// <param name="assetEntries"></param>
    /// <param name="minecraftEntry"></param>
    private static void MapAssets(List<AssetEntry> assetEntries, MinecraftEntry minecraftEntry)
    {
        if (assetEntries.All(a => !a.IsLegacy && !a.MapToResource))
            return;

        foreach (var assetEntry in assetEntries)
        {
            if (!assetEntry.File.Exists)
                continue;

            var originalName = assetEntry.Raw.Key;
            if (assetEntry.MapToResource)
            {
                var resourceFile = minecraftEntry.Tree.WorkingDirectory.DiveToFile($"resources/{originalName}");
                resourceFile.Directory?.Create();

                if (!resourceFile.Exists)
                    assetEntry.File.CopyTo(resourceFile.FullName);
            }

            var legacyFile = minecraftEntry.Tree.Assets.DiveToFile(originalName);
            legacyFile.Directory?.Create();

            if (!legacyFile.Exists)
                assetEntry.File.CopyTo(legacyFile.FullName);
        }
    }
}