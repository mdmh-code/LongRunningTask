using LongRunningTask.Domain;
using LongRunningTask.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LongRunningTask.Infraestructure.Queues;

public class RequestJobConsumer(ILogger<RequestJobConsumer> logger, IPublisher publisher, StringProcessor processor, IDelayProvider delayProvider) : IConsumer<RequestJob>
{
    public async Task Consume(ConsumeContext<RequestJob> context)
    {
        var message = context.Message;

        logger.LogInformation("Processing {ProcessId}", message.ProcessId);

        var processedMessage = processor.Process(message.Input);

        for (int i = 0; i < processedMessage.Length; i++)
        {
            char character = processedMessage[i];

            delayProvider.Delay();
            logger.LogInformation("Publishing result for {ProcessId}: {Character} at position {Position}", message.ProcessId, character, i);

            await publisher.Publish(new ResponseJob(message.UserId, message.ProcessId, character, i));
        }
    }
}
