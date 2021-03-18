using System.Net.Http;

namespace SMS.Microservice.Service.Interfaces
{
    public interface ISmsGateway
    {
        HttpResponseMessage SendSms(ISmsSend smsSend);
    }
}
