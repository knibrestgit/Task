using SWW.GStats.BusinessLogic.Services;
using SWW.GStats.DataAccess;
using System;
using System.Linq;
using Xunit;

namespace SWW.GStats.Tests.Reports
{
    public class RecentMatchesReportTest
    {

        [Fact]
        public async void RecentMatchesReportShouldReturnRightResults()
        {
            // TODO 
            var data = await ReportTestExtentions.GetActualReportResultOnDatabase(
                new[] {
                    new Endpoint { Id = "E1", Name = "Name1", GameModes = "" },
                    new Endpoint { Id = "E2", Name = "Name2", GameModes = "" },
                    new Endpoint { Id = "E3", Name = "Name3", GameModes = "" }
                },
                new[] {
                    new Match { EndpointId = "E1", Timestamp = new DateTime(2018,1,1,1,00,0), Map = "M1", Scoreboard = ReportTestExtentions.GenerateScoreboard(10) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,2,1,30,0), Map = "M2", Scoreboard = ReportTestExtentions.GenerateScoreboard(20) },
                    new Match { EndpointId = "E3", Timestamp = new DateTime(2018,1,1,2,00,0), Map = "M3", Scoreboard = ReportTestExtentions.GenerateScoreboard(30) },
                },
                null,

                (db) => (new ReportsService(db)).GetRecentMatches(50)
            );


            var actual = data.ToArray();

            Assert.Equal(3, actual.Length);

            Assert.Equal("E2", actual[0].server);
            Assert.Equal("E3", actual[1].server);
            Assert.Equal("E1", actual[2].server);

            Assert.Equal(new DateTime(2018,1,2,1,30,0), actual[0].timestamp);
            Assert.Equal(new DateTime(2018,1,1,2,00,0), actual[1].timestamp);
            Assert.Equal(new DateTime(2018,1,1,1,00,0), actual[2].timestamp);

            Assert.Equal("M2", actual[0].results.map);
            Assert.Equal("M3", actual[1].results.map);
            Assert.Equal("M1", actual[2].results.map);

            Assert.Equal(20, actual[0].results.scoreboard.Count());
            Assert.Equal(30, actual[1].results.scoreboard.Count());
            Assert.Equal(10, actual[2].results.scoreboard.Count());
        }


        [Fact]
        public async void RecentMatchesReportShouldReturnEmptyArrayIfNoData()
        {

            var data = await ReportTestExtentions.GetActualReportResultOnDatabase(
                null,
                null,
                null,

                (db) => (new ReportsService(db)).GetRecentMatches(50)
            );

            var actual = data.ToArray();

            Assert.Empty(actual);
        }


        [Fact]
        public async void RecentMatchesReportShouldReturnSpecifiedNumberOfItems()
        {
            var expected = 2;

            var data = await ReportTestExtentions.GetActualReportResultOnDatabase(
                new[] {
                    new Endpoint { Id = "E1", Name = "Name1", GameModes = "" },
                    new Endpoint { Id = "E2", Name = "Name2", GameModes = "" },
                    new Endpoint { Id = "E3", Name = "Name3", GameModes = "" },
                    new Endpoint { Id = "E4", Name = "Name3", GameModes = "" },
                    new Endpoint { Id = "E5", Name = "Name3", GameModes = "" }
                },
                new[] {
                    new Match { EndpointId = "E1", Timestamp = new DateTime(2018,1,1,1,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,1,2,0,0) },
                    new Match { EndpointId = "E3", Timestamp = new DateTime(2018,1,1,3,0,0) },
                    new Match { EndpointId = "E4", Timestamp = new DateTime(2018,1,1,4,0,0) },
                    new Match { EndpointId = "E5", Timestamp = new DateTime(2018,1,1,5,0,0) }
                },
                null,

                (db) => (new ReportsService(db)).GetRecentMatches(expected)
            );


            var actual = data.Count();

            Assert.Equal(expected, actual);
        }
    }
}
