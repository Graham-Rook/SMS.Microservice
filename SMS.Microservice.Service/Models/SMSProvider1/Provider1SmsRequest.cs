using SMS.Microservice.Service.Interfaces;

namespace SMS.Microservice.Service.Models.SMSProvider1
{
    public class Provider1SmsRequest : ISmsSend
    {
        public string UniqueId { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
    }
}
