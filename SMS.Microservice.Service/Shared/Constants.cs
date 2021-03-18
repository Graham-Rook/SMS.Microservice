namespace SMS.Microservice.Service.Shared
{
    public class Constants
    {
        public static class SmsInvalid
        {
            public const string SmsInvalid_MessageIdInvalid_Code = "MessageId";
            public const string SmsInvalid_MessageIdInvalid_Message = "MessageId is missing";

            public const string SmsInvalid_PhoneNumberInvalid_Code = "PhoneNumber";
            public const string SmsInvalid_PhoneNumberInvalid_Message = "Phone Number is missing";

            public const string SmsInvalid_TextMessageInvalid_Code = "TextMessage";
            public const string SmsInvalid_TextMessageInvalid_Message = "Text message is missing";
        }
    }
}
