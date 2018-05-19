using Microsoft.EntityFrameworkCore;
using SWW.GStats.DataAccess;
using System;
using System.IO;
using System.Linq;

namespace SWW.GStats.Tests
{
    public class FillDatabaseRoutine
    {

        // Not actual test. Routine to fill database for perfomance testing

        //[Fact]
        public void FillDatabase()
        {
            var serverCount = 10000;
            var matchesCount = 100*2;
            var daysCount = 14*2;
            var usersCount = 1000000;
            var startDay = new DateTime(2018, 5, 1);
            var random = new Random();

            var fileName = @".\test.sqlite";

            var options = new DbContextOptionsBuilder();
            options.UseSqlite($"Filename={fileName}");

            File.Delete(fileName);

            using (var db = new StatsContext(options.Options)) {
                db.Database.EnsureCreated();
                using (db.Database.BeginTransaction()) {
                    db.Endpoints.AddRange(Enumerable.Range(0, serverCount - 1).Select(i =>
                         new Endpoint {
                             Id = $"Details:{i}",
                             GameModes = "",
                             Name = $"Endpoint Name {i}"
                         }));
                    db.SaveChanges();
                    db.Database.CommitTransaction();
                }
            }

            for (var endpoint = 0; endpoint < serverCount; endpoint++) {
                var days = random.Next(daysCount);
                for (var day = 1; day <= days; day++) {
                    var matches = random.Next(matchesCount);
                    using (var db = new StatsContext(options.Options)) {
                        using (db.Database.BeginTransaction()) {
                            db.Matches.AddRange(Enumerable.Range(0, matches).Select(x =>
                               new Match {
                                   EndpointId = $"Details:{endpoint}",
                                   FragLimit = random.Next(30) + 1,
                                   GameMode = $"DM{random.Next(10)}",
                                   Map = $"Map{random.Next(100)}",
                                   TimeElapsed = 30.2F,
                                   TimeLimit = random.Next(100) + 1,
                                   Timestamp = startDay.AddDays(day).AddMinutes(random.Next(60 * 23)),
                                   Scoreboard = Enumerable.Range(0, random.Next(100)).Select(p =>
                                      new Scoreboard {
                                          Name = $"User{random.Next(usersCount)}",
                                          Deaths = random.Next(100),
                                          Frags = random.Next(100),
                                          Kills = random.Next(100),
                                          Rating = (float)p
                                      }
                                     ).ToArray()
                               }
                            ));
                            db.SaveChanges();
                            db.Database.CommitTransaction();
                        }
                    }
                }
            }
        }
    }
}
