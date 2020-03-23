using CommuteMonitoring.TrafficMonitor.Infrastructure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommuteMonitoring.TrafficMonitor
{
    public static class MonitoringOrchestration
    {
        [FunctionName("func-monitoringorchestration")]
        public static async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation("Traffic monitoring started.");

            var parameters = context.GetInput<MonitoringOrchestrationParameters>();
            TimeSpan currentTime = GetCurrentTime(context);

            while (currentTime < parameters.TimerExpirationTime)
            {
                var timeToDestination = await context.CallActivityAsync<int>("func-getcommutetime", null);
                var timeToLeave = parameters.DestinationArrivalTime - TimeSpan.FromSeconds(timeToDestination + parameters.NotificationPeriod);
                log.LogInformation("Current time {currentTime}; time to leave {timeToLeave}.", currentTime, timeToLeave);

                if (currentTime > timeToLeave)
                {
                    var input = new NotificationParameters
                    {
                        CurrentTime = currentTime,
                        DestinationTime = parameters.DestinationArrivalTime,
                        TimeToDestination = timeToDestination
                    };
                    await context.CallActivityAsync<NotificationParameters>("func-sendnotification", input);
                    
                    return;
                }

                var nextCheckTime = context.CurrentUtcDateTime.AddSeconds(parameters.CheckInterval);
                log.LogInformation("Schedule next timer job at {nextCheckTime}.", nextCheckTime);
                await context.CreateTimer(nextCheckTime, CancellationToken.None);

                currentTime = GetCurrentTime(context);
            }

            log.LogError("Orchestration finished due expiration time. Current time = {currentTime}, Timeout = {timeout}.", currentTime, parameters.TimerExpirationTime);
        }

        private static TimeSpan GetCurrentTime(IDurableOrchestrationContext context)
        {
            return UtcTimeConverter.ToEasternStandardTime(context.CurrentUtcDateTime).TimeOfDay;
        }

        [FunctionName("func-sendnotification")]
        public static async Task SendNotification([ActivityTrigger]NotificationParameters parameters)
        {
            var timeToLeave = parameters.DestinationTime - TimeSpan.FromSeconds(parameters.TimeToDestination);
            var minutesToLeave = (timeToLeave - parameters.CurrentTime).TotalMinutes;

            var message = $"Leave in {(int)minutesToLeave} minutes to hit 66 route at {parameters.DestinationTime.ToString(@"hh\:mm")}.";
            var queueClient = new QueueClient(Settings.NotificationQueueConnection, Settings.NotificationQueueName);
            await queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(message)));

            await queueClient.CloseAsync();
        }

        [FunctionName("func-getcommutetime")]
        public static async Task<int> GetCommuteTime([ActivityTrigger] object input)
        {
            var key = Settings.GoogleMapsKey;
            var origin = Settings.DirectionOrigin;
            var destination = Settings.DirectionDestination;
            
            var client = new GoogleMapsClient(key);
            return await client.GetDistnace(origin, destination);
        }
    }
}
