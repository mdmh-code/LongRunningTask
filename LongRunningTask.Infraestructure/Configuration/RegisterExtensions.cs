using MassTransit;
using Microsoft.Extensions.DependencyInjection;

using LongRunningTask.Infraestructure.Queues;
using LongRunningTask.Domain.Interfaces;

namespace LongRunningTask.Infraestructure.Configuration;

public static class RegisterExtensions
{
    public static void RegisterQueuePublisher(this IServiceCollection services)
    {
        services.AddScoped<IPublisher, Publisher>();
    }

    public static void RegisterQueueConsumer<T>(this IServiceCollection services) where T : class, IConsumer
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<T>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConcurrentMessageLimit = 8;

                cfg.Host("queues", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                cfg.UseDelayedRedelivery(r => r.Intervals(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromMinutes(1)
                ));

                cfg.ConfigureEndpoints(context);
                
            });
        });
    }

    public static void RegisterAsyncMessaging(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddScoped<ICharacterReceiver, SignalCharacterReceiver>();
    }
}