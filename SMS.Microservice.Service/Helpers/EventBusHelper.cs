using SMS.Microservice.Service.Interfaces;
using System.Net.Http;

namespace SMS.Microservice.Service.Helpers
{
    public class EventBusHelper : IEventBus
    {
        public HttpResponseMessage PublishEvent(IEventNotification notification)
        {
            //TODO: Implementation required
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
    }
}
