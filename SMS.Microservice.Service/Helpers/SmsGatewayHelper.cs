using SMS.Microservice.Service.Interfaces;
using System.Net.Http;

namespace SMS.Microservice.Service.Helpers
{
    public class SmsGatewayHelper : ISmsGateway
    {
        public HttpResponseMessage SendSms(ISmsSend smsSend)
        {
            //TODO: Implementation required
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
    }
}
