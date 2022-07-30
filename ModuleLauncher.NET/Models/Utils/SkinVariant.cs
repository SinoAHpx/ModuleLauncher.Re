using System.ComponentModel;
using System.Runtime.Serialization;

namespace ModuleLauncher.NET.Models.Utils;

public enum SkinVariant
{
    [Description("classic")]
    [EnumMember(Value = "classic")]
    Classic,
    [Description("slim")]
    [EnumMember(Value = "slim")]
    Slim
}