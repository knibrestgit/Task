using SWW.GStats.Server.Controllers;
using Xunit;

namespace SWW.GStats.Tests.Controllers
{
    public class ControllersExtentionsTest
    {
        [Theory]
        [InlineData(null,  5)]
        [InlineData( -10,  0)]
        [InlineData(  -1,  0)]
        [InlineData(   0,  0)]
        [InlineData(   1,  1)]
        [InlineData(   2,  2)]
        [InlineData(  20, 20)]
        [InlineData(  40, 40)]
        [InlineData(  49, 49)]
        [InlineData(  50, 50)]
        [InlineData(  51, 50)]
        [InlineData( 100, 50)]
        [InlineData( 200, 50)]
        public void ConfigureCountTests(int? count, int expected)
        {
            var actual = ControllersExtentions.ConfigureCount(count);
            Assert.Equal(expected, actual);
        }
    }
}
