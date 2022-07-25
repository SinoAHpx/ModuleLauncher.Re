using Manganese.Text;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Tests;

public class NetworkingTests
{
    [Fact]
    public async Task GetString()
    {
        var url = "https://httpbin.org/get?awd=1&aw3=2";
        var result = await url.GetStringAsync();
        
        result.Fetch("args.awd").ShouldBe("1");
        result.Fetch("args.aw3").CannotBe("1");
    }
}