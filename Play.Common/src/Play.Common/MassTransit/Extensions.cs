using System.Reflection;
using System.Runtime.CompilerServices;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extentions
    {
        public static IServiceCollection AddMassTransitwithRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(config =>
          {
              config.AddConsumers(Assembly.GetEntryAssembly());

              config.UsingRabbitMq((context, configurator) =>
              {
                  var configuration = context.GetService<IConfiguration>();

                  var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

                  var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                  configurator.Host(rabbitMQSettings.Host);
                  configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
              });
          });
            services.AddMassTransitHostedService();
            return services;
        }
    }
}