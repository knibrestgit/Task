using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace SWW.GStats.Server.Controllers
{
    public static class ControllersExtentions
    {
        private const int DefaultMachesCount = 5;
        private const int MaxMatchesCount = 50;
        private static readonly TimeSpan CacheExpirationPeriod = TimeSpan.FromMinutes(1);

        public static int ConfigureCount(this int? count)
        {
            if (!count.HasValue) return DefaultMachesCount;
            if (count > MaxMatchesCount) return MaxMatchesCount;
            if (count <= 0) return 0;
            return count.Value;
        }

        public static async Task<T> GetFromCacheOrRunAsync<T>(this IMemoryCache cache, string key, Func<Task<T>> function)
        {
            var entry = await cache.GetOrCreateAsync(key, e => {
                e.AbsoluteExpirationRelativeToNow = CacheExpirationPeriod;
                return function();
            });
            return entry;
        }
    }
}
