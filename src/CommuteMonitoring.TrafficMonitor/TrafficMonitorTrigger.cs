using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CommuteMonitoring.TrafficMonitor
{
    public static class TrafficMonitorTrigger
    {
        [FunctionName("func-trafficmonitortrigger")]
        public static async Task Run([TimerTrigger("0 10 2 * * 1-5")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Start traffic monitoring");
            using (var client = new HttpClient())
            {
                var functionUrl = $"{Settings.MonitoringStartUrl}?code={Settings.FunctionAuthenticationCode}";
                var response = await client.GetAsync(functionUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    log.LogInformation("Traffic monitoring was triggered.");
                }
                else
                {
                    log.LogError($"Traffic monitoring trigger failed with code {response.StatusCode}.");
                }
            }
        }
    }
}
