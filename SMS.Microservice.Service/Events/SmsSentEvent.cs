using SMS.Microservice.Service.Interfaces;

namespace SMS.Microservice.Service.Events
{
    public class SmsSentEvent : IEventNotification
    {
        public string MessageId { get; set; }
        public string PhoneNumber { get; set; }
        public string TextMessage { get; set; }
    }
}
