using System.Threading.Tasks;

namespace ModuleLauncher.Re.Extensions
{
    public static class AsyncExtensions
    {
        public static T GetResult<T>(this Task<T> task)
        {
            return task.GetAwaiter().GetResult();
        }
    }
}