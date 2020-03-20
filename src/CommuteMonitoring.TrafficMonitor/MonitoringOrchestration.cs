using CommuteMonitoring.TrafficMonitor.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
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
                    await SendNotification(context, currentTime, parameters.DestinationArrivalTime, timeToDestination);
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

        private static async Task SendNotification(IDurableOrchestrationContext context, TimeSpan currentTime, TimeSpan destinationTime, int timeToDestination)
        {
            var timeToLeave = destinationTime - TimeSpan.FromSeconds(timeToDestination);
            var minutesToLeave = (timeToLeave - currentTime).TotalMinutes;

            var message = $"Leave in {(int)minutesToLeave} minutes to hit 66 route at {destinationTime.ToString(@"hh\:mm")}.";
            await context.CallActivityAsync<int>("func-sendmessage", message);
        }


        [FunctionName("func-sendmessage")]
        public static async Task SendMessage([ActivityTrigger] string message, ILogger log)
        {
            var telegramClient = new TelegramClient(Settings.TelegramApiKey, Settings.TelegramChatId);

            //ToDo: decouple with a message bus
            var response = await telegramClient.SendMessage(message);
            log.LogInformation($"Sending telegram message - {message}. Response status - {response}.");
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
