using SMS.Microservice.Service.Interfaces;

namespace SMS.Microservice.Service.Models.SMSProvider2
{
    public class Provider2SmsRequest : ISmsSend
    {
        public string Idenitifier { get; set; }
        public string Mobile { get; set; }
        public string MessageText { get; set; }
    }
}
