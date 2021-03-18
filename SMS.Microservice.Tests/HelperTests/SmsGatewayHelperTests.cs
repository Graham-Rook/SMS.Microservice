using SMS.Microservice.Service.Helpers;
using SMS.Microservice.Service.Models.SMSProvider1;
using SMS.Microservice.Service.Models.SMSProvider2;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Microservice.Tests.HelperTests
{
    [TestFixture]
    class SmsGatewayHelperTests
    {
        private class Resources : IDisposable
        {
            public readonly SmsGatewayHelper SmsGatewayHelper;

            public Resources()
            {
                SmsGatewayHelper = new SmsGatewayHelper();
            }

            public void Dispose()
            { }
        }

        [Test]
        public void SendSms_Provider1()
        {
            using (var resources = new Resources())
            {
                //Given
                var request = new Provider1SmsRequest
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    Phone = "+44123456789",
                    Message = "This is a test SMS Message"
                };

                //When
                var result = resources.SmsGatewayHelper.SendSms(request);

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
            }
        }


        [Test]
        public void SendSms_Provider2()
        {
            using (var resources = new Resources())
            {
                //Given
                var request = new Provider2SmsRequest
                {
                    Idenitifier = Guid.NewGuid().ToString(),
                    Mobile = "+44123456789",
                    MessageText = "This is a test SMS Message"
                };

                //When
                var result = resources.SmsGatewayHelper.SendSms(request);

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
            }
        }

    }
}
