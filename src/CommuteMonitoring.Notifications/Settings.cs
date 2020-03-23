using System;
using System.Collections.Generic;
using System.Text;

namespace CommuteMonitoring.Notifications
{
    public static class Settings
    {
        public static string TelegramApiKey => GetEnvironmentVariable("TelegramApiKey");

        public static string TelegramChatId => GetEnvironmentVariable("TelegramChatId");

        public static string NotificationQueueConnection => GetEnvironmentVariable("NotificationQueueConnection");

        public static string NotificationQueueName => GetEnvironmentVariable("NotificationQueueName");


        private static string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
