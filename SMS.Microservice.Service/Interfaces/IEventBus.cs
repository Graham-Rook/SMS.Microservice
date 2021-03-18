using System.Net.Http;

namespace SMS.Microservice.Service.Interfaces
{
    public interface IEventBus
    {
        HttpResponseMessage PublishEvent(IEventNotification notification);
    }
}
