using SMS.Microservice.Service.Events;
using SMS.Microservice.Service.Helpers;
using NUnit.Framework;
using System;

namespace SMS.Microservice.Tests.HelperTests
{
    [TestFixture]
    class EventBusHelperTests
    {
        private class Resources : IDisposable
        {
            public readonly EventBusHelper EventBusHelper;

            public Resources()
            {
                EventBusHelper = new EventBusHelper();
            }

            public void Dispose()
            { }
        }

        [Test]
        public void PublishEvent_SmsSent()
        {
            using (var resources = new Resources())
            {
                //Given
                var request = new SmsSentEvent
                {
                    MessageId = Guid.NewGuid().ToString(),
                    PhoneNumber = "+44123456789",
                    TextMessage = "This is a test SMS Message"
                };

                //When
                var result = resources.EventBusHelper.PublishEvent(request);

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
            }
        }

        [Test]
        public void PublishEvent_SmsSuccess()
        {
            using (var resources = new Resources())
            {
                //Given
                var request = new SmsSuccessEvent
                {
                    MessageId = Guid.NewGuid().ToString()
                };

                //When
                var result = resources.EventBusHelper.PublishEvent(request);

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
            }
        }

        [Test]
        public void PublishEvent_SmsFailed()
        {
            using (var resources = new Resources())
            {
                //Given
                var request = new SmsFailedEvent
                {
                    MessageId = Guid.NewGuid().ToString(),
                    FailedReason = "SMS Failed for some reason"
                };

                //When
                var result = resources.EventBusHelper.PublishEvent(request);

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
            }
        }

    }
}
