namespace SMS.Microservice.Service.Models
{
    public class SendSmsCommandModel
    {
        public string MessageId { get; set; }
        public string PhoneNumber { get; set; }
        public string TextMessage { get; set; }
    }
}
