using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SWW.GStats.BusinessLogic.DTO;
using SWW.GStats.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWW.GStats.BusinessLogic.Services
{
    public class ServerService
    {
        private StatsContext db { get; set; }

        public ServerService(StatsContext context)
        {
            db = context;
        }

        public async Task PutEndpointInfo(string endpoint, EndpointAdvertising dto) {
            var data = await db.Endpoints.FindAsync(endpoint);
            if (data == null) {
                data = new Endpoint { Id = endpoint };
                db.Endpoints.Add(data);
            }

            data.FromDto(dto);

            await db.SaveChangesAsync();
        }

        public async Task<EndpointAdvertising> GetEndpointInfo(string endpoint) {
            var data = await db.Endpoints.FindAsync(endpoint);
            return data.ToDto();
        }

        public async Task<IEnumerable<EndpointItem>> GetAllEndpointsInfo() {
            var data = await db.Endpoints.ToListAsync();

            return data.Select(x=> new EndpointItem {
                endpoint = x.Id,
                info = x.ToDto()
            });
        }

        public async Task PutMatch(string endpoint, DateTime timestamp, MatchItem dto)
        {
            var endpointData = await db.Endpoints.FindAsync(endpoint);
            if (endpoint == null) throw new Exception("Bad endpoint");

            var data = dto.ToData(endpointData, timestamp);

            await db.SaveChangesAsync();
        }

        public async Task<EndpointStats> GetEndpointStats(string endpoint)
        {
            var parameter = new[] { new SqliteParameter("endpoint", endpoint) };
            try {
                var data = await db.RunRawSqlAsync(
                      $@"SELECT SUM(Cnt),
	                        MAX(Cnt),
                            AVG(Cnt),
	                        MAX(MaxPlayers),
	                        TotalPlayers / Sum(Cnt)
                     FROM 
                            (SELECT COUNT() AS Cnt, 
                                    SUM(PlayersCount) AS TotalPlayers, 
                                    MAX(PlayersCount) AS MaxPlayers
	                         FROM Matches 
	                         WHERE [EndpointId] = @endpoint 
                             GROUP BY CAST(julianday([Timestamp]) AS INT))
                   ",
                      parameter,
                      r => new EndpointStats {
                          totalMatchesPlayed = r.IsDBNull(0) ? throw new NoDataException() : r.GetInt32(0),
                          maximumMatchesPerDay = r.GetInt32(1),
                          averageMatchesPerDay = r.GetFloat(2),
                          maximumPopulation = r.GetInt32(3),
                          averagePopulation = r.GetFloat(4),
                      }
                );
                var result = data.FirstOrDefault();

                var topGameModes = await db.RunRawSqlAsync(
                      $@"SELECT GameMode, COUNT(*)
                     FROM Matches 
                     WHERE [EndpointId] = @endpoint 
                     GROUP BY GameMode
                     ORDER BY COUNT(*) DESC
                     LIMIT 5
                   ",
                      parameter,
                      r => r.GetString(0)
                );
                result.top5GameModes = topGameModes.ToArray();

                var topMaps = await db.RunRawSqlAsync(
                      $@"SELECT Map, COUNT(*)
                     FROM Matches 
                     WHERE [EndpointId] = @endpoint 
                     GROUP BY Map
                     ORDER BY COUNT(*) DESC
                     LIMIT 5
                   ",
                      parameter,
                      r => r.GetString(0)
                );
                result.top5Maps = topMaps.ToArray();

                return result;
            } catch (NoDataException) {
                return null;
            }
        }

        public async Task<MatchItem> GetMatch(string endpoint, DateTime timestamp)
        {
            var data = await db.Matches
                               .Include(x => x.Scoreboard)
                               .FirstOrDefaultAsync(x => 
                                     x.EndpointId == endpoint && 
                                     x.Timestamp == timestamp
                                );

            return data?.ToDto();
        }

    }

}
