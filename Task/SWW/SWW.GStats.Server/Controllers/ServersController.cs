using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Route("servers")]
    public class ServersController : Controller
    {
        readonly ServerService service;
        readonly IMemoryCache cache;

        public ServersController(ServerService service, IMemoryCache memoryCache)
        {
            this.service = service;
            this.cache = memoryCache;
        }

        [HttpPut("{endpoint}/info")]
        [NullToNotFound]
        public async void PutEndpointInfo(string endpoint, [FromBody] EndpointAdvertising dto)
        {
            TryValidateModel(dto);
            await service.PutEndpointInfo(endpoint, dto);
        }


        [HttpGet("{endpoint}/info")]
        [NullToNotFound]
        public async Task<EndpointAdvertising> GetEndpointInfo(string endpoint)
        {
            return await service.GetEndpointInfo(endpoint);
        }

        [HttpGet("info")]
        public async Task<IEnumerable<EndpointItem>> GetAllEndpointsInfo()
        {
            return await cache.GetFromCacheOrRunAsync(
                $"all-ep",
                () => service.GetAllEndpointsInfo()
            );
        }


        [HttpPut("{endpoint}/matches/{timestamp}")]
        [NullToNotFound]
        public async void PutMatch(string endpoint, DateTime timestamp, [FromBody] MatchItem dto)
        {
            TryValidateModel(dto);
            await service.PutMatch(endpoint, timestamp, dto);
        }

        [HttpGet("{endpoint}/matches/{timestamp}")]
        [NullToNotFound]
        public async Task<MatchItem> GetMatch(string endpoint, DateTime timestamp)
        {
            return await service.GetMatch(endpoint, timestamp);
        }

        [HttpGet("{endpoint}/stats")]
        [NullToNotFound]
        public async Task<EndpointStats> GetEndpointStats(string endpoint)
        {
            return await cache.GetFromCacheOrRunAsync(
                    $"es-{endpoint}",
                    () => service.GetEndpointStats(endpoint)
                );
        }

    }
}
