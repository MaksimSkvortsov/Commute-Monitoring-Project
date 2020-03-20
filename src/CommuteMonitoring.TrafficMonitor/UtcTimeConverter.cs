using System;

namespace CommuteMonitoring.TrafficMonitor
{
    internal class UtcTimeConverter
    {
        private const string EasternStandardTime = "Eastern Standard Time";

        public static DateTime ToEasternStandardTime(DateTime dateTime)
        {
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(EasternStandardTime);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, easternZone);
        }
    }
}
