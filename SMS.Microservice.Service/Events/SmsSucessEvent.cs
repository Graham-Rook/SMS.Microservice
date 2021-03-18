using SMS.Microservice.Service.Interfaces;

namespace SMS.Microservice.Service.Events
{
    public class SmsSuccessEvent : IEventNotification
    {
        public string MessageId { get; set; }
    }
}
