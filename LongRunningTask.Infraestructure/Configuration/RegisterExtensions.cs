using LongRunningTask.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

using LongRunningTask.Infraestructure.Queues;

namespace LongRunningTask.Infraestructure.Configuration
{
    public static class RegisterExtensions
    {
        public static void RegisterInfraestructure(this IServiceCollection services)
        {
            services.AddSignalR();

            // services.AddMassTransit(x =>
            // {
            //     x.AddConsumer<ProcessStringJobConsumer>();

            //     x.UsingRabbitMq((context, cfg) =>
            //     {
            //         cfg.Host("queues", "/", h =>
            //         {
            //             h.Username("guest");
            //             h.Password("guest");
            //         });

            //         cfg.ConfigureEndpoints(context);
            //     });
            // });
            services.AddScoped<IPublisher, Publisher>();
        }

    }
}
