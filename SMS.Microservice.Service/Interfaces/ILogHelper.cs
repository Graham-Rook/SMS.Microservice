using SMS.Microservice.Service.Helpers.LogHelper;
using System;

namespace SMS.Microservice.Service.Interfaces
{
    public interface ILogHelper
    {
        void LogMessage(params string[] messages);
        void LogException(Exception ex, Options options = null);
        void LogMessage(Options options, params string[] messages);
    }
}
