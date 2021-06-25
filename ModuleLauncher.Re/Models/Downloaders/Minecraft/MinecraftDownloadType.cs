using System.ComponentModel;
using System.Runtime.Serialization;

namespace ModuleLauncher.Re.Models.Downloaders.Minecraft
{
    public enum MinecraftDownloadType
    {
        [Description("release")]
        [EnumMember(Value = "release")]
        Release,
        [Description("snapshot")]
        [EnumMember(Value = "snapshot")]
        Snapshot,
        [Description("old_alpha")]
        [EnumMember(Value = "old_alpha")]
        OldAlpha,
        [Description("old_beta")]
        [EnumMember(Value = "old_beta")]
        OldBeta
    }
}