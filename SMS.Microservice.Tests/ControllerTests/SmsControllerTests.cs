using SMS.Microservice.Service.Controllers;
using SMS.Microservice.Service.Events;
using SMS.Microservice.Service.Interfaces;
using SMS.Microservice.Service.Models;
using SMS.Microservice.Service.Models.SMSProvider1;
using SMS.Microservice.Service.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Net.Http;

namespace SMS.Microservice.Tests.ControllerTests
{
    [TestFixture]
    class SmsControllerTests
    {
        private class Resources : IDisposable
        {
            public readonly SmsController Controller;
            public readonly Mock<ILogHelper> LogHelper;
            public readonly Mock<ISmsGateway> SmsGateway;
            public readonly Mock<IEventBus> EventBus;

            public Resources()
            {
                LogHelper = new Mock<ILogHelper>();
                SmsGateway = new Mock<ISmsGateway>();
                EventBus = new Mock<IEventBus>();
                Controller = new SmsController(LogHelper.Object, SmsGateway.Object, EventBus.Object);
            }

            public void Dispose()
            { }
        }

        [Test]
        public void SendSms_Success()
        {
            using (var resources = new Resources())
            {
                //Given
                var requestModel = new SendSmsCommandModel
                {
                    MessageId = Guid.NewGuid().ToString(),
                    PhoneNumber = "+44123456789",
                    TextMessage = "This is a test SMS Message"
                };

                //setup SmsGateway SendSms
                resources.SmsGateway.Setup(x => x.SendSms(It.Is<Provider1SmsRequest>(m =>
                    m.UniqueId == requestModel.MessageId &&
                    m.Phone == requestModel.PhoneNumber &&
                    m.Message == requestModel.TextMessage))).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

                //When
                var result = resources.Controller.SendSms(requestModel) as OkObjectResult;

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == 200);

                var validResult = result.Value as ValidationResult;
                Assert.IsNotNull(validResult);
                Assert.IsTrue(validResult.IsValid);

                //verify calls
                resources.LogHelper.Verify(x => x.LogMessage(It.Is<string>(m => 
                    m == $"SMS sent for messageId: { requestModel.MessageId}")), Times.Once);                

                resources.SmsGateway.Verify(x => x.SendSms(It.Is<Provider1SmsRequest>(m =>
                    m.UniqueId == requestModel.MessageId &&
                    m.Phone == requestModel.PhoneNumber &&
                    m.Message == requestModel.TextMessage)), Times.Once);                

                resources.EventBus.Verify(x => x.PublishEvent(It.Is<SmsSentEvent>(m =>
                    m.MessageId == requestModel.MessageId &&
                    m.PhoneNumber == requestModel.PhoneNumber &&
                    m.TextMessage == requestModel.TextMessage)), Times.Once);

                resources.LogHelper.VerifyNoOtherCalls();
                resources.SmsGateway.VerifyNoOtherCalls();
                resources.EventBus.VerifyNoOtherCalls();
            }
        }

        public enum InvalidType
        {
            InvalidMessageId,
            InvalidPhone = 1,
            InvalidMessage = 2
        }

        [TestCase(InvalidType.InvalidMessageId, Constants.SmsInvalid.SmsInvalid_MessageIdInvalid_Code, Constants.SmsInvalid.SmsInvalid_MessageIdInvalid_Message)]
        [TestCase(InvalidType.InvalidPhone, Constants.SmsInvalid.SmsInvalid_PhoneNumberInvalid_Code, Constants.SmsInvalid.SmsInvalid_PhoneNumberInvalid_Message)]
        [TestCase(InvalidType.InvalidMessage, Constants.SmsInvalid.SmsInvalid_TextMessageInvalid_Code, Constants.SmsInvalid.SmsInvalid_TextMessageInvalid_Message)]        
        public void SendSms_Invalid(InvalidType validType, string expectedCode, string expectedMessage)
        {
            using (var resources = new Resources())
            {
                //Given
                var requestModel = new SendSmsCommandModel
                {
                    MessageId = validType == InvalidType.InvalidMessageId ? null : Guid.NewGuid().ToString(),
                    PhoneNumber = validType == InvalidType.InvalidPhone? null : "+27846005192",
                    TextMessage = validType == InvalidType.InvalidMessage ? null : "This is a test SMS Message"
                };

                //setup SmsGateway SendSms
                resources.SmsGateway.Setup(x => x.SendSms(It.Is<Provider1SmsRequest>(m =>
                    m.UniqueId == requestModel.MessageId &&
                    m.Phone == requestModel.PhoneNumber &&
                    m.Message == requestModel.TextMessage))).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK));


                //When
                var result = resources.Controller.SendSms(requestModel) as BadRequestObjectResult;

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == 400);

                var validResult = result.Value as ValidationResult;

                Assert.False(validResult.IsValid);
                Assert.IsNotEmpty(validResult.Messages);
                
                var validationMessage = validResult.Messages[0];
                Assert.AreEqual(expectedCode, validationMessage.Code);
                Assert.AreEqual(expectedMessage, validationMessage.MessageText);

                //verify no calls were made                                
                resources.LogHelper.Verify(x => x.LogMessage(It.Is<string>(m =>
                    m == $"SMS sent for messageId: { requestModel.MessageId}")), Times.Never);                

                resources.SmsGateway.Verify(x => x.SendSms(It.Is<Provider1SmsRequest>(m =>
                    m.UniqueId == requestModel.MessageId &&
                    m.Phone == requestModel.PhoneNumber &&
                    m.Message == requestModel.TextMessage)), Times.Never);                

                resources.EventBus.Verify(x => x.PublishEvent(It.Is<SmsSentEvent>(m =>
                    m.MessageId == requestModel.MessageId &&
                    m.PhoneNumber == requestModel.PhoneNumber &&
                    m.TextMessage == requestModel.TextMessage)), Times.Never);

                resources.LogHelper.VerifyNoOtherCalls();
                resources.SmsGateway.VerifyNoOtherCalls();
                resources.EventBus.VerifyNoOtherCalls();
            }
        }

        [Test]
        public void SendSms_BadRequest()
        {
            using (var resources = new Resources())
            {
                //Given
                var requestModel = new SendSmsCommandModel
                {
                    MessageId = Guid.NewGuid().ToString(),
                    PhoneNumber = "+27846005192",
                    TextMessage = "This is a test SMS Message"
                };

                //setup SmsGateway SendSms
                resources.SmsGateway.Setup(x => x.SendSms(It.Is<Provider1SmsRequest>(m =>
                    m.UniqueId == requestModel.MessageId &&
                    m.Phone == requestModel.PhoneNumber &&
                    m.Message == requestModel.TextMessage))).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest));


                //When
                var result = resources.Controller.SendSms(requestModel) as BadRequestObjectResult;

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == 400);

                var responseMessage = result.Value as HttpResponseMessage;
                Assert.IsTrue(responseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest);

                //verify calls
                resources.SmsGateway.Verify(x => x.SendSms(It.Is<Provider1SmsRequest>(m =>
                    m.UniqueId == requestModel.MessageId &&
                    m.Phone == requestModel.PhoneNumber &&
                    m.Message == requestModel.TextMessage)), Times.Once);                

                //verify no calls were made to log and event bus
                resources.LogHelper.Verify(x => x.LogMessage(It.Is<string>(m =>
                    m == $"SMS sent for messageId: { requestModel.MessageId}")), Times.Never);               

                resources.EventBus.Verify(x => x.PublishEvent(It.Is<SmsSentEvent>(m =>
                    m.MessageId == requestModel.MessageId &&
                    m.PhoneNumber == requestModel.PhoneNumber &&
                    m.TextMessage == requestModel.TextMessage)), Times.Never);

                resources.SmsGateway.VerifyNoOtherCalls();
                resources.LogHelper.VerifyNoOtherCalls();
                resources.EventBus.VerifyNoOtherCalls();
            }
        }

        [Test]
        public void RecieveSuccess()
        {
            using (var resources = new Resources())
            {
                //Given
                var requestModel = new SmsSuccessModel
                {
                    MessageId = Guid.NewGuid().ToString()
                };                                

                //When
                var result = resources.Controller.RecieveSuccess(requestModel) as OkResult;

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == 200);

                //verify calls
                resources.LogHelper.Verify(x => x.LogMessage(It.Is<string>(m =>
                    m == $"SMS was successful for messageId: {requestModel.MessageId}")), Times.Once);                

                resources.EventBus.Verify(x => x.PublishEvent(It.Is<SmsSuccessEvent>(m =>
                    m.MessageId == requestModel.MessageId)), Times.Once);

                resources.LogHelper.VerifyNoOtherCalls();
                resources.EventBus.VerifyNoOtherCalls();
                resources.SmsGateway.VerifyNoOtherCalls();
            }
        }

        [Test]
        public void RecieveFailed()
        {
            using (var resources = new Resources())
            {
                //Given
                var requestModel = new SmsFailedModel
                {
                    MessageId = Guid.NewGuid().ToString(),
                    FailedReason = "SMS Failed for some reason"
                };

                //When
                var result = resources.Controller.RecieveFailed(requestModel) as OkResult;

                //Then
                Assert.IsNotNull(result);
                Assert.IsTrue(result.StatusCode == 200);

                //verify calls were made
                resources.LogHelper.Verify(x => x.LogMessage(It.Is<string>(m =>
                    m == $"SMS failed for messageId: {requestModel.MessageId}, reason: {requestModel.FailedReason}")), Times.Once);              
                    
                resources.EventBus.Verify(x => x.PublishEvent(It.Is<SmsFailedEvent>(m =>
                    m.MessageId == requestModel.MessageId &&
                    m.FailedReason == requestModel.FailedReason)), Times.Once);

                resources.LogHelper.VerifyNoOtherCalls();
                resources.EventBus.VerifyNoOtherCalls();
                resources.SmsGateway.VerifyNoOtherCalls();
            }
        }
    }
}
