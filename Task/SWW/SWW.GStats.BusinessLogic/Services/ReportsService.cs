using Microsoft.EntityFrameworkCore;
using SWW.GStats.BusinessLogic.DTO;
using SWW.GStats.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWW.GStats.BusinessLogic.Services
{
    public class ReportsService
    {
        private StatsContext db { get; set; }


        public ReportsService(StatsContext context)
        {
            db = context;
        }

        public async Task<PlayerStats> GetPlayerStats(string name)
        {
            var result = new PlayerStats();
            var totalKills = 0;
            var totalDeath = 0;
            var totalRating = 0F;
            var favServer = new Dictionary<string, int>(10000);
            var favGameModes = new Dictionary<string, int>(10);
            var favMaps = new Dictionary<string, int>(10000);
            var daysStats = new Dictionary<DateTime, int>(10000);


            var query = db.Scoreboards.Where(x => EF.Functions.Like(x.Name, $"%{name}%"))
                          .Join(db.Matches, x => x.Match.Id, y => y.Id, (x, y) => new {
                              MatchEndpoint = y.EndpointId,
                              MatchTimestamp = y.Timestamp,
                              MatchMap = y.Map,
                              MatchGameMode = y.GameMode,
                              PlayerRating = x.Rating,
                              PlayersKills = x.Kills,
                              PlayerDeaths = x.Deaths
                          });

            await query.ForEachAsync(x => {
                result.totalMatchesPlayed++;
                if (x.PlayerRating == 1F) {
                    result.totalMatchesWon++;
                }
                totalKills += x.PlayersKills;
                totalDeath += x.PlayerDeaths;
                totalRating += x.PlayerRating;
                if (x.MatchTimestamp > result.lastMatchPlayed) {
                    result.lastMatchPlayed = x.MatchTimestamp;
                }
                favServer.CollectGroupStats(x.MatchEndpoint);
                favMaps.CollectGroupStats(x.MatchMap);
                favGameModes.CollectGroupStats(x.MatchGameMode);
                daysStats.CollectGroupStats(x.MatchTimestamp.Date);
            });

            if (result.totalMatchesPlayed == 0) return null;

            result.favoriteServer = favServer.Aggregate((r, x) => x.Value > r.Value ? x : r).Key;
            result.uniqueServers = favServer.Count();
            result.favoriteGameMode = favGameModes.Aggregate((r, x) => x.Value > r.Value ? x : r).Key;
            result.maximumMatchesPerDay = daysStats.Max(x => x.Value);
            result.averageMatchesPerDay = (float)daysStats.Average(x => x.Value);
            result.averageScoreboardPercent = totalRating / result.totalMatchesPlayed;
            result.killToDeathRatio = totalDeath > 0 ? totalKills / totalDeath : 1;

            return result;
        }



        public async Task<IEnumerable<MatchReportItem>> GetRecentMatches(int count)
        {
            if (count <= 0) return Enumerable.Empty<MatchReportItem>();

            var query = await db.Matches
                                .Include(x => x.Scoreboard)
                                .OrderByDescending(x => x.Timestamp)
                                .Take(count)
                                .ToArrayAsync();

            return query.Select(x => x.ToMatchReportItem());
        }

        public async Task<IEnumerable<PopularServer>> GetPopularServers(int count) {
            if (count <= 0) return Enumerable.Empty<PopularServer>();

            return await db.RunRawSqlAsync(
                  $@"SELECT m.[EndpointId] AS [endpoint],
                           [Endpoints].Name AS [name],
                           m.[AvgM] AS [averageMatchesPerDay]
                    FROM (SELECT [EndpointId], COUNT() / CAST(julianday(MAX([Timestamp])) - julianday(MIN([Timestamp]))+1 AS INT) AS [AvgM]
                          FROM [Matches]
                          GROUP BY [EndpointId]
                          ORDER BY COUNT() / CAST(julianday(MAX([Timestamp])) - julianday(MIN([Timestamp]))+1 AS INT) DESC
                          LIMIT {count}) AS m 
                          INNER JOIN [Endpoints] ON m.[EndpointId] = Endpoints.Id
                   ",
                  null,
                  r => new PopularServer {
                      endpoint = r.GetString(0),
                      name = r.GetString(1),
                      averageMatchesPerDay = r.IsDBNull(2) ? 0 : r.GetFloat(2)
                  }
            );
        }


        public async Task<IEnumerable<BestPlayer>> GetBestPlayers(int count) {
            if (count <= 0) return Enumerable.Empty<BestPlayer>();

            var query = db.Scoreboards
                          .GroupBy(x => x.Name,
                                  (key, values) => new BestPlayer {
                                      name = key,
                                      killToDeathRatio = values.Sum(x => (float) x.Kills) /  values.Sum(x => (float) x.Deaths)
                                  })
                          .OrderByDescending(x => x.killToDeathRatio)
                          .Take(count);

            return await query.ToArrayAsync();
        }
    }
}
