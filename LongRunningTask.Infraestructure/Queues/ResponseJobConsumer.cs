using LongRunningTask.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LongRunningTask.Infraestructure.Queues;

public class ResponseJobConsumer(ILogger<ResponseJobConsumer> logger, ICharacterReceiver characterReceiver) : IConsumer<ResponseJob>
{
    public Task Consume(ConsumeContext<ResponseJob> context)
    {
        var message = context.Message;

        logger.LogInformation("Received response for {ProcessId}: {Character} at position {Position}, isLast: {IsLast}", message.ProcessId, message.Result, message.Position, message.IsLast);

        return characterReceiver.ReceiveCharacter(message.UserId, message.ProcessId, message.Result, message.Position, message.IsLast);
    }
}
