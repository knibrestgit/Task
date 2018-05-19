using SWW.GStats.BusinessLogic.Services;
using Xunit;

namespace SWW.GStats.Tests.Services
{
    public class ServiceExtentionsTest
    {

        [Theory]

        // One players
        [InlineData(0, 1,  100F)]

        // 4 players
        [InlineData(0, 4,  100F           )]
        [InlineData(1, 4,  66.66666666667F)]
        [InlineData(2, 4,  33.33333333333F)]
        [InlineData(3, 4,  0F             )]

        // 3 players
        [InlineData(0, 3, 100F)]
        [InlineData(1, 3, 50F )]
        [InlineData(2, 3, 0F  )]

        public void CalcScoreboardPercentTests(int position, int total, float expected)
        {
            var actual = ServicesExtentions.CalcScoreboardPercent(position, total);
            Assert.Equal(expected, actual, precision: 10);
        }
    }
}
