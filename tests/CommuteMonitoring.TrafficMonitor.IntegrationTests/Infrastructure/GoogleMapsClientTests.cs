using CommuteMonitoring.TrafficMonitor.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace CommuteMonitoring.TrafficMonitor.IntegrationTests.Infrastructure
{
    public class GoogleMapsClientTests
    {
        public GoogleMapsClientTests()
        {
            Configuration.InitializeEnvironmentVariables();
        }

        [Fact]
        public async Task GetDistance_ValidInput_PositiveDuration()
        {
            var target = new GoogleMapsClient(Settings.GoogleMapsKey);
            var duration = await target.GetDistnace(Settings.DirectionOrigin, Settings.DirectionDestination);

            Assert.True(duration > 0);
        }

        [Fact]
        public async Task GetDistance_InvalidKey_Exception()
        {
            var target = new GoogleMapsClient("12345");
            await Assert.ThrowsAsync<GoogleApiException>(() => target.GetDistnace(Settings.DirectionOrigin, Settings.DirectionDestination));
        }
    }
}
