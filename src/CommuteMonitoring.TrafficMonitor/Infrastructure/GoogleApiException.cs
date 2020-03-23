using System;

namespace CommuteMonitoring.TrafficMonitor.Infrastructure
{
    public class GoogleApiException : Exception
    {
        public GoogleApiException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
