using SMS.Microservice.Service.Interfaces;

namespace SMS.Microservice.Service.Events
{
    public class SmsFailedEvent : IEventNotification
    {
        public string MessageId { get; set; }
        public string FailedReason { get; set; }
    }
}
