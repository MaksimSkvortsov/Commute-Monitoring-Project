using System;

namespace CommuteMonitoring.TrafficMonitor
{
    public class MonitoringOrchestrationParameters
    {
        public int CheckInterval { get; set; }

        public int NotificationPeriod { get; set; }

        public TimeSpan TimerExpirationTime { get; set; }

        public TimeSpan DestinationArrivalTime { get; set; }
    }
}
