using Flurl.Http;
using Manganese.Data;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET.Resources;

public class AssetsResolver
{
    /// <summary>
    /// Minecraft resolver, you'll need to provide this only when you need to get libraries by minecraft id
    /// </summary>
    public MinecraftResolver? MinecraftResolver { get; set; }

    /// <summary>
    /// Construct resolver via minecraft resolver
    /// </summary>
    /// <param name="minecraftResolver"></param>
    public AssetsResolver(MinecraftResolver? minecraftResolver)
    {
        MinecraftResolver = minecraftResolver;
    }

    /// <summary>
    /// Construct assets resolver via minecraft root path
    /// </summary>
    /// <param name="minecraftRootPath"></param>
    public AssetsResolver(string? minecraftRootPath)
    {
        MinecraftResolver =
            minecraftRootPath.ThrowIfNullOrEmpty<NullReferenceException>("Root path of resolver could not be null");
    }

    /// <summary>
    /// Just an empty constructor
    /// </summary>
    public AssetsResolver()
    {
    }

    /// <summary>
    /// Get assets, will automatically download if no assets index exists 
    /// </summary>
    /// <returns></returns>
    public async Task<List<AssetEntry>> GetAssetsAsync(string minecraftId)
    {
        var mc = MinecraftResolver.ThrowIfNull(new InvalidOperationException("Minecraft resolver is not initialized"))
            .GetMinecraft(minecraftId);

        return await GetAssetsAsync(mc);
    }

    /// <summary>
    /// Get assets without automatic download
    /// </summary>
    /// <returns></returns>
    /// <exception cref="CorruptedStuctureException">If assets index does not exist</exception>
    public List<AssetEntry> GetAssets(string minecraftId)
    {
        var mc = MinecraftResolver.ThrowIfNull(new InvalidOperationException("Minecraft resolver is not initialized"))
            .GetMinecraft(minecraftId);

        return GetAssets(mc);
    }

    /// <summary>
    /// Get assets, will automatically download if no assets index exists 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static async Task<List<AssetEntry>> GetAssetsAsync(MinecraftEntry minecraftEntry)
    {
        var assetIndexMetadata = GetAssetIndexMetadata(minecraftEntry);
        var assetIndexFile = minecraftEntry.Tree.AssetsIndexes.DiveToFile($"{assetIndexMetadata.AssetIndex}.json");
        if (!assetIndexFile.Exists)
        {
            var remoteAssetIndex = await assetIndexMetadata.AssetUrl.GetStringAsync();
            assetIndexFile.Directory?.Create();

            await assetIndexFile.WriteAllTextAsync(remoteAssetIndex);
        }

        return GetAssets(minecraftEntry);
    }

    /// <summary>
    /// Get assets without automatic download
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    /// <exception cref="CorruptedStuctureException">If assets index does not exist</exception>
    public static List<AssetEntry> GetAssets(MinecraftEntry minecraftEntry)
    {
        //sync: no automatic download
        //async: automatically download when index file missing
        var assetIndex = GetAssetIndexMetadata(minecraftEntry).AssetIndex;
        var assetIndexFile = minecraftEntry.Tree.AssetsIndexes.DiveToFile($"{assetIndex}.json");

        if (!assetIndexFile.Exists)
            throw new CorruptedStuctureException("Missing assets index file, you may download it first");

        var assetsIndexJson = assetIndexFile.ReadAllText().ToJObject();
        var objects = assetsIndexJson
            .FetchJToken("objects").ThrowCorruptedIfNull()
            .ToObject<JObject>().ThrowCorruptedIfNull();

        var assets = new List<AssetEntry>();
        foreach (var (key, value) in objects)
        {
            AssetEntry assetEntry;
            if (assetIndex == "legacy")
                assetEntry = ProcessLegacy(minecraftEntry, (key, value.ThrowCorruptedIfNull()));
            else if (assetIndex == "pre-1.6")
                assetEntry = ProcessLegacy(minecraftEntry, (key, value.ThrowCorruptedIfNull()), false);
            else
                assetEntry = Process(minecraftEntry, (key, value.ThrowCorruptedIfNull()));

            assetEntry.RelativeUrl = $"{assetEntry.Hash[..2]}/{assetEntry.Hash}";
            
            assets.Add(assetEntry);
        }
        
        return assets;
    }

    private static AssetEntry Process(MinecraftEntry minecraftEntry, (string key, JToken value) rawAsset)
    {
        var parentDir = minecraftEntry.Tree.Assets.Dive("objects");
        var hash = rawAsset.value.Fetch("hash").ThrowCorruptedIfNull();
        var file = parentDir.DiveToFile($"{hash[..2]}/{hash}");

        return new AssetEntry
        {
            File = file,
            Hash = hash,
            IsLegacy = false,
            MapToResource = false
        };
    }

    /// <summary>
    /// Process assets for legacy and pre-1.6 versions
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="rawAsset"></param>
    /// <param name="isLegacy"></param>
    /// <returns></returns>
    private static AssetEntry ProcessLegacy(MinecraftEntry minecraftEntry, (string key, JToken value) rawAsset, bool isLegacy = true)
    {
        //minecraftEntry.Tree.Assets -> .minecraft\assets\virtual\legacy
        var file = minecraftEntry.Tree.Assets.DiveToFile(rawAsset.key);
        var hash = rawAsset.value.Fetch("hash").ThrowCorruptedIfNull();

        return new AssetEntry
        {
            File = file,
            Hash = hash,
            IsLegacy = isLegacy,
            MapToResource = !isLegacy
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    private static (string AssetIndex, string AssetUrl) GetAssetIndexMetadata(MinecraftEntry minecraftEntry)
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