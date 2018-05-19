using Microsoft.EntityFrameworkCore;
using SWW.GStats.BusinessLogic.Services;
using SWW.GStats.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SWW.GStats.Tests.Reports
{
    public class ReportTestExtentions
    {

        public static async Task<T> GetActualReportResultOnDatabase<T>(
            IEnumerable<Endpoint> endpoints,
            IEnumerable<Match> matches,
            IEnumerable<Scoreboard> scoreboards,
            Func<StatsContext, Task<T>> function
        ) {
            var fileName = Path.GetTempFileName();
            var options = new DbContextOptionsBuilder();
            options.UseSqlite($"Filename={fileName}");

            T actual;

            using (var db = new StatsContext(options.Options)) {
                db.Database.EnsureCreated();

                if (endpoints !=null)    db.Endpoints.AddRange(endpoints);
                if (matches != null)     db.Matches.AddRange(matches);
                if (scoreboards != null) db.Scoreboards.AddRange(scoreboards);

                if (db.ChangeTracker.HasChanges()) {
                    await db.SaveChangesAsync();
                }

                actual = await function(db);
            }
            File.Delete(fileName);
            return actual;
        }


        public static Scoreboard[] GenerateScoreboard(int playersCount) {
            Random random = new Random();
            return Enumerable.Range(1, playersCount).Select(ind => new Scoreboard {
                Name = $"Player{ind}",
                Deaths = random.Next(20),
                Frags = random.Next(20),
                Kills = random.Next(20),
                Rating = ServicesExtentions.CalcScoreboardPercent(ind-1,playersCount)
            }).ToArray();
        }
    }
}
