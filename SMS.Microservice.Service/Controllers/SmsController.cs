using SMS.Microservice.Service.Events;
using SMS.Microservice.Service.Interfaces;
using SMS.Microservice.Service.Models;
using SMS.Microservice.Service.Models.SMSProvider1;
using SMS.Microservice.Service.Shared;
using Microsoft.AspNetCore.Mvc;
using System;

namespace SMS.Microservice.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly ILogHelper _logHelper;
        private readonly ISmsGateway _smsGateway;
        private readonly IEventBus _eventBus;

        public SmsController(ILogHelper logHelper, ISmsGateway smsGateway, IEventBus eventBus)
        {
            _logHelper = logHelper;
            _smsGateway = smsGateway;
            _eventBus = eventBus;
        }

        [HttpPost("SendSms")]
        public IActionResult SendSms([FromBody] SendSmsCommandModel requestModel)
        {
            try
            {
                //validate request
                var validResult = IsSmsValidToSend(requestModel);

                if (validResult.IsValid)
                {
                    var smsRequest = new Provider1SmsRequest
                    {
                        Message = requestModel.TextMessage,
                        Phone = requestModel.PhoneNumber,
                        UniqueId = requestModel.MessageId
                    };

                    //send sms to provider
                    var result = _smsGateway.SendSms(smsRequest);

                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //publish event
                        _eventBus.PublishEvent(new SmsSentEvent
                        {
                            MessageId = requestModel.MessageId,
                            PhoneNumber = requestModel.PhoneNumber,
                            TextMessage = requestModel.TextMessage
                        });
                        
                        _logHelper.LogMessage($"SMS sent for messageId: {requestModel.MessageId}");

                        return Ok(validResult);
                    }
                    return BadRequest(result);
                }
                return BadRequest(validResult);
            }
            catch (Exception ex)
            {
                _logHelper.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost("ReceiveSuccess")]
        public IActionResult RecieveSuccess([FromBody] SmsSuccessModel response)
        {
            try
            {
                //publish success event
                _eventBus.PublishEvent(new SmsSuccessEvent
                {
                    MessageId = response.MessageId
                });

                _logHelper.LogMessage($"SMS was successful for messageId: {response.MessageId}");

                return Ok();
            }
            catch (Exception ex)
            {
                _logHelper.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost("ReceiveFailed")]
        public IActionResult RecieveFailed([FromBody] SmsFailedModel response)
        {
            try
            {
                //publish failed event
                _eventBus.PublishEvent(new SmsFailedEvent
                {
                    MessageId = response.MessageId,
                    FailedReason = response.FailedReason
                });

                _logHelper.LogMessage($"SMS failed for messageId: {response.MessageId}, reason: {response.FailedReason}");

                return Ok();
            }
            catch (Exception ex)
            {
                _logHelper.LogException(ex);
                return BadRequest(ex);
            }
        }

        #region private

        private ValidationResult IsSmsValidToSend(SendSmsCommandModel requestModel)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(requestModel.MessageId))
                validationResult.Invalidate(Constants.SmsInvalid.SmsInvalid_MessageIdInvalid_Code, Constants.SmsInvalid.SmsInvalid_MessageIdInvalid_Message);

            if (string.IsNullOrEmpty(requestModel.PhoneNumber))
                validationResult.Invalidate(Constants.SmsInvalid.SmsInvalid_PhoneNumberInvalid_Code, Constants.SmsInvalid.SmsInvalid_PhoneNumberInvalid_Message);

            if (string.IsNullOrEmpty(requestModel.TextMessage))
                validationResult.Invalidate(Constants.SmsInvalid.SmsInvalid_TextMessageInvalid_Code, Constants.SmsInvalid.SmsInvalid_TextMessageInvalid_Message);

            return validationResult;
        }

        #endregion
    }
}
