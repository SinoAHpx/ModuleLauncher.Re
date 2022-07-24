using Manganese.Array;
using ModuleLauncher.NET.Runtime;

var lines = await LauncherProfileWriter.GeneratePrpfilesAsync();
lines.Output();