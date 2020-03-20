using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace CommuteMonitoring.TrafficMonitor
{
    public static class MonitoringOrchestrationStart
    {
        [FunctionName("func-monitoringorchestrationstart")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, [DurableClient]IDurableOrchestrationClient starter, ILogger log)
        {
            var parameters = GetParameters();
            var instanceId = await starter.StartNewAsync("func-monitoringorchestration", parameters);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        private static MonitoringOrchestrationParameters GetParameters()
        {
            return new MonitoringOrchestrationParameters
            {
                CheckInterval = Settings.TrafficCheckIntervalInSeconds,
                NotificationPeriod = Settings.NotificationPeriodInSeconds,
                DestinationArrivalTime = Settings.DestinationArrivalTime,
                TimerExpirationTime = Settings.TimerExpirationTime
            };
        }
    }
}
