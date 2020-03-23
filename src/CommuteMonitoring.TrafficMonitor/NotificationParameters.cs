using System;
using System.Collections.Generic;
using System.Text;

namespace CommuteMonitoring.TrafficMonitor
{
    [Serializable]
    public class NotificationParameters
    {
        public TimeSpan CurrentTime { get; set; } 
        public TimeSpan DestinationTime { get; set; } 
        public int TimeToDestination { get; set; }
    }
}
