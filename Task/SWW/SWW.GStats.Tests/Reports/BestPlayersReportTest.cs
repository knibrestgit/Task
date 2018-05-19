using SWW.GStats.BusinessLogic.Services;
using SWW.GStats.DataAccess;
using System.Linq;
using Xunit;

namespace SWW.GStats.Tests.Reports
{
    public class BestPlayersReportTest
    {

        [Fact]
        public async void BestPlayersReportShouldReturnRightResults()
        {

            var data = await ReportTestExtentions.GetActualReportResultOnDatabase(
                null,
                null,
                new[] {
                    
                    // User1 killToDeathRation = 30/60 = 0.5
                    new Scoreboard { Name = "User1", Kills = 10, Deaths = 10 },
                    new Scoreboard { Name = "User1", Kills = 20, Deaths = 20 },
                    new Scoreboard { Name = "User1", Kills =  0, Deaths = 30 },

                    // User2 killToDeathRation = 0/50 = 0
                    new Scoreboard { Name = "User2", Kills =  0, Deaths = 10 },
                    new Scoreboard { Name = "User2", Kills =  0, Deaths = 10 },
                    new Scoreboard { Name = "User2", Kills =  0, Deaths = 30 },

                    // User3 killToDeathRation = 60/10 = 6
                    new Scoreboard { Name = "User3", Kills = 10, Deaths = 0 },
                    new Scoreboard { Name = "User3", Kills = 20, Deaths = 0 },
                    new Scoreboard { Name = "User3", Kills = 30, Deaths = 10 },
                },

                (db) => (new ReportsService(db)).GetBestPlayers(50)

            );

            var actual = data.ToArray();

            Assert.Equal(3, actual.Length);

            Assert.Equal("User3", actual[0].name);
            Assert.Equal("User1", actual[1].name);
            Assert.Equal("User2", actual[2].name);

            Assert.Equal(6.0F, actual[0].killToDeathRatio);
            Assert.Equal(0.5F, actual[1].killToDeathRatio);
            Assert.Equal(0.0F, actual[2].killToDeathRatio);

        }

        [Fact]
        public async void BestPlayersReportShouldEmptyArrayIfNoData()
        {

            var data = await ReportTestExtentions.GetActualReportResultOnDatabase(
                null,
                null,
                null,
                (db) => (new ReportsService(db)).GetBestPlayers(50)
            );

            var actual = data.ToArray();

            Assert.Empty(actual);
        }


        [Fact]
        public async void BestPlayersReportShouldReturnSpecifiedNumberOfItems()
        {
            var expected = 2;

            var data = await ReportTestExtentions.GetActualReportResultOnDatabase(
                null,
                null,
                new[] {
                    
                    // User1 killToDeathRation = 30/60 = 0.5
                    new Scoreboard { Name = "User1", Kills = 10, Deaths = 10 },
                    new Scoreboard { Name = "User1", Kills = 20, Deaths = 20 },
                    new Scoreboard { Name = "User1", Kills =  0, Deaths = 30 },

                    // User2 killToDeathRation = 0/50 = 0
                    new Scoreboard { Name = "User2", Kills =  0, Deaths = 10 },
                    new Scoreboard { Name = "User2", Kills =  0, Deaths = 10 },
                    new Scoreboard { Name = "User2", Kills =  0, Deaths = 30 },

                    // User3 killToDeathRation = 60/10 = 6
                    new Scoreboard { Name = "User3", Kills = 10, Deaths = 0 },
                    new Scoreboard { Name = "User3", Kills = 20, Deaths = 0 },
                    new Scoreboard { Name = "User3", Kills = 30, Deaths = 10 },
                },

                (db) => (new ReportsService(db)).GetBestPlayers(expected)
            );

            var actual = data.Count();

            Assert.Equal(expected, actual);
        }


    }
}
