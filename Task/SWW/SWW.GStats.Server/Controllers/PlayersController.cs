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
    [Route("players")]
    public class PlayersController : Controller
    {
        readonly ReportsService service;
        readonly IMemoryCache cache;

        public PlayersController(ReportsService service, IMemoryCache memoryCache)
        {
            this.service = service;
            this.cache = memoryCache;
        }

        [Route("{name}/stats")]
        [NullToNotFound]
        public async Task<PlayerStats> GetPlayersStat(string name) {
            return await cache.GetFromCacheOrRunAsync(
                            $"pl-{name}",
                            () => service.GetPlayerStats(name)
                         );
        }
    }
}