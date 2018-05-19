using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SWW.GStats.BusinessLogic.DTO;
using SWW.GStats.BusinessLogic.Services;
using SWW.GStats.Server.ActionFilters;

namespace SWW.GStats.Server.Controllers
{
    [Produces("application/json")]
    [JsonError]
    [Route("reports")]
    public class ReportsController : Controller
    {
        readonly ReportsService service;
        readonly IMemoryCache cache;

        public ReportsController(ReportsService service, IMemoryCache memoryCache)
        {
            this.service = service;
            this.cache = memoryCache;
        }

        [HttpGet("recent-matches/{count:int?}")]
        public async Task<IEnumerable<MatchReportItem>> GetRecentMatches(int? count) {
            var cnt = count.ConfigureCount();
            return await cache.GetFromCacheOrRunAsync(
                            $"rm-{cnt}",
                            () => service.GetRecentMatches(cnt)
                         );
        }

        [HttpGet("best-players/{count:int?}")]
        public async Task<IEnumerable<BestPlayer>> GetBestPlayers(int? count)
        {
            var cnt = count.ConfigureCount();
            return await cache.GetFromCacheOrRunAsync(
                            $"bp-{cnt}",
                            () => service.GetBestPlayers(cnt)
                         );
        }

        
        [HttpGet("popular-servers/{count:int?}")]
        public async Task<IEnumerable<PopularServer>> GetPopularServers(int? count)
        {
            var cnt = count.ConfigureCount();
            return await cache.GetFromCacheOrRunAsync(
                            $"ps-{cnt}",
                            () => service.GetPopularServers(cnt)
                         );
        }



    }
}