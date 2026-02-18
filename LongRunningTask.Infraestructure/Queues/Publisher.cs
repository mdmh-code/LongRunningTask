using MassTransit;
using Microsoft.Extensions.Logging;

namespace LongRunningTask.Infraestructure.Queues;

public class Publisher(ILogger<Publisher> logger, IPublishEndpoint publishEndpoint) : IPublisher
{
    public async Task Publish<T>(T message, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));

            await publishEndpoint.Publish(message, cancellationToken);

            logger.LogInformation("Message published successfully: {Message}", message);
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish message: {Message}", message);
            throw;
        }
    }

}

