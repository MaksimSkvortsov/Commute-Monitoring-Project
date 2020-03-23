using System.Threading.Tasks;
using CommuteMonitoring.Notifications.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CommuteMonitoring.Notifications
{
    public static class SendTelegramMessage
    {
        [FunctionName("func-sendtelegrammessage")]
        public static async Task Run([ServiceBusTrigger("sbq-telegram-traffic-bot", Connection = "NotificationQueueConnection")]string message, ILogger log)
        {
            log.LogDebug($"sbq-telegram-traffic-bot queue trigger function processed: {message}");

            var telegramClient = new TelegramClient(Settings.TelegramApiKey, Settings.TelegramChatId);

            var response = await telegramClient.SendMessage(message);
            log.LogInformation($"Sending telegram message - {message}. Response status - {response}.");
        }
    }
}
