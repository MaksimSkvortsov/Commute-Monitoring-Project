using System;

namespace CommuteMonitoring.TrafficMonitor
{
    public static class Settings
    {
        public static int TrafficCheckIntervalInSeconds => GetIntEnvironmentVariable("TrafficCheckIntervalInSeconds");

        public static int NotificationPeriodInSeconds => GetIntEnvironmentVariable("NotificationPeriodInSeconds");

        public static TimeSpan DestinationArrivalTime => GetTimeEnvironmentVariable("DestinationArrivalTime");

        public static TimeSpan TimerExpirationTime => GetTimeEnvironmentVariable("TimerExpirationTime");

        public static string GoogleMapsKey => GetEnvironmentVariable("GoogleMapsKey");

        public static string DirectionOrigin => GetEnvironmentVariable("DirectionOrigin");

        public static string DirectionDestination => GetEnvironmentVariable("DirectionDestination");

        public static string TelegramApiKey => GetEnvironmentVariable("TelegramApiKey");

        public static string TelegramChatId => GetEnvironmentVariable("TelegramChatId");

        public static string MonitoringStartUrl => GetEnvironmentVariable("MonitoringStartUrl");

        public static string FunctionAuthenticationCode => GetEnvironmentVariable("FunctionAuthenticationCode");



        private static TimeSpan GetTimeEnvironmentVariable(string key)
        {
            return TimeSpan.Parse(GetEnvironmentVariable(key));
        }

        private static int GetIntEnvironmentVariable(string key)
        {
            return Convert.ToInt32(GetEnvironmentVariable(key));
        }

        private static string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
