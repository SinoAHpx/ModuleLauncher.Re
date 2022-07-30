using System.Text;
using Flurl.Http;
using Manganese.Text;
using ModuleLauncher.NET.Models.Utils;
using ModuleLauncher.NET.Utilities;

var accessToken =
    "eyJhbGciOiJIUzI1NiJ9.eyJ4dWlkIjoiMjUzNTQ0MDM0MjkwNzc3NyIsImFnZyI6IkFkdWx0Iiwic3ViIjoiNjhkY2MzMDUtMjI1Mi00MzNlLWJjNDktYTY4Mjc0YzM5OWJjIiwibmJmIjoxNjU5MTY1NDk0LCJhdXRoIjoiWEJPWCIsInJvbGVzIjpbXSwiaXNzIjoiYXV0aGVudGljYXRpb24iLCJleHAiOjE2NTkyNTE4OTQsImlhdCI6MTY1OTE2NTQ5NCwicGxhdGZvcm0iOiJPTkVTVE9SRSIsInl1aWQiOiI0NjZkODM3MzEwZWY2OGZiYThkNDdhY2VmYjk3YmJmNCJ9.Fq5yYXee-1OFMo8CNKVv7r1GBXgGFHxT4c63Cr1SycA";

var profile = await SkinUtils.ChangeSkinAsync(accessToken,
    new FileInfo(@"C:\Users\ahpx\Downloads\2022_07_26_badb-chapter-1-1--i-am-a-sinner--you-are-a-saint--20636351.png"),
    SkinVariant.Slim);

profile.ToJsonString().Print();

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}