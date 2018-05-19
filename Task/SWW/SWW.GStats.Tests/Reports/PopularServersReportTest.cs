using SWW.GStats.BusinessLogic.Services;
using SWW.GStats.DataAccess;
using System;
using System.Linq;
using Xunit;

namespace SWW.GStats.Tests.Reports
{
    public class PopularServersReportTest
    {

        [Fact]
        public async void PopularServersReportShouldReturnRightResults()
        {

            var data = await ReportTestExtentions.GetActualReportResultOnDatabase(
                new[] {
                    new Endpoint { Id = "E1", Name = "Name1", GameModes = "" },
                    new Endpoint { Id = "E2", Name = "Name2", GameModes = "" },
                    new Endpoint { Id = "E3", Name = "Name3", GameModes = "" }
                },
                new[] {
                    // "E1" = 3+1=4 per 2 days = 2 matches per day
                    new Match { EndpointId = "E1", Timestamp = new DateTime(2018,1,1,1,0,0) },
                    new Match { EndpointId = "E1", Timestamp = new DateTime(2018,1,1,2,0,0) },
                    new Match { EndpointId = "E1", Timestamp = new DateTime(2018,1,1,3,0,0) },
                    new Match { EndpointId = "E1", Timestamp = new DateTime(2018,1,2,1,0,0) },

                    // "E2" = 6+0+6=12 per 3 days = 4 matches per day
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,1,1,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,1,2,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,1,3,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,1,4,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,1,5,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,1,6,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,3,1,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,3,2,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,3,3,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,3,4,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,3,5,0,0) },
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,3,6,0,0) },

                    // "E1" = 1 per 1 days = 1 match per day
                    new Match { EndpointId = "E3", Timestamp = new DateTime(2018,1,1,1,0,0) }
                },
                null,

                (db) => (new ReportsService(db)).GetPopularServers(50)
            );


            var actual = data.ToArray();

            Assert.Equal(3, actual.Length);

            Assert.Equal("E2", actual[0].endpoint);
            Assert.Equal("E1", actual[1].endpoint);
            Assert.Equal("E3", actual[2].endpoint);

            Assert.Equal("Name2", actual[0].name);
            Assert.Equal("Name1", actual[1].name);
            Assert.Equal("Name3", actual[2].name);

            Assert.Equal(4.0F, actual[0].averageMatchesPerDay);
            Assert.Equal(2.0F, actual[1].averageMatchesPerDay);
            Assert.Equal(1.0F, actual[2].averageMatchesPerDay);
        }


        [Fact]
        public async void PopularServersReportShouldReturnEmptyArrayIfNoData()
        {

            var data = await ReportTestExtentions.GetActualReportResultOnDatabase(
                null,
                null,
                null,

                (db) => (new ReportsService(db)).GetPopularServers(50)
            );

            var actual = data.ToArray();

            Assert.Empty(actual);
        }


        [Fact]
        public async void PopularServersReportShouldReturnSpecifiedNUmberOfItems()
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
                    new Match { EndpointId = "E2", Timestamp = new DateTime(2018,1,1,1,0,0) },
                    new Match { EndpointId = "E3", Timestamp = new DateTime(2018,1,1,1,0,0) },
                    new Match { EndpointId = "E4", Timestamp = new DateTime(2018,1,1,1,0,0) },
                    new Match { EndpointId = "E5", Timestamp = new DateTime(2018,1,1,1,0,0) }
                },
                null,

                (db) => (new ReportsService(db)).GetPopularServers(expected)
            );


            var actual = data.Count();

            Assert.Equal(expected, actual);
        }
    }
}
