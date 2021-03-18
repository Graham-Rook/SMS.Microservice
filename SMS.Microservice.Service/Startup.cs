using SMS.Microservice.Service.Helpers;
using SMS.Microservice.Service.Helpers.LogHelper;
using SMS.Microservice.Service.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace SMS.Microservice.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddControllers();
                services.AddLogHelper(Configuration);
                services.AddTransient<IEventBus, EventBusHelper>();
                services.AddTransient<ISmsGateway, SmsGatewayHelper>();

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SMS Microservice", Version = "v1" });
                });
            }
            catch (Exception ex)
            {
                ProfileLogHelper.LogDesperate(ex);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SMS Microservice v1"));
                }

                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            catch (Exception ex)
            {
                ProfileLogHelper.LogDesperate(ex);
                throw;
            }

        }
    }
}
