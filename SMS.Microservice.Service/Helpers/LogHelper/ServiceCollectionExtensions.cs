using SMS.Microservice.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace SMS.Microservice.Service.Helpers.LogHelper
{
    public static class ServiceCollectionExtensions
    {

        public static void AddLogHelper(this IServiceCollection services, IConfiguration configuration, LogHelperTypes logHelperType = LogHelperTypes.ProfileLogHelper)
        {
            switch (logHelperType)
            {
                case LogHelperTypes.ProfileLogHelper:
                    AddProfileLogHelper(services, configuration);
                    break;
            }
        }

        private static void AddProfileLogHelper(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = services.BuildServiceProvider();            
            var profiles = new List<Profile>();
            configuration.Bind("Logging:Profiles", profiles);
            services.AddSingleton(profiles);

            var logHelper = new ProfileLogHelper(profiles);

            services.AddTransient<ILogHelper>(x => logHelper);
        }
    }
}
