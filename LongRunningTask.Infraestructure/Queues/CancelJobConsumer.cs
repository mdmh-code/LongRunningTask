using LongRunningTask.Infraestructure.Repositories;
using MassTransit;
using LongRunningTask.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LongRunningTask.Infraestructure.Queues
{
    public class CancelJobConsumer(
        ILogger<CancelJobConsumer> logger,
        IJobRepository jobRepository,
        IPublisher publisher
    ) : IConsumer<CancelJob>
    {
        public async Task Consume(ConsumeContext<CancelJob> context)
        {
            var job = await jobRepository.GetByProcessIdAsync(context.Message.ProcessId);

            if (job is null)
            {
                logger.LogWarning("Received cancel request for non-existent job with ProcessId: {ProcessId}", context.Message.ProcessId);
                return;
            }

            if (job.IsFinished)
            {
                logger.LogInformation("Received cancel request for already finished job with ProcessId: {ProcessId}", context.Message.ProcessId);
                return;
            }

            job.SetCancelled();
            
            await jobRepository.UpdateAsync(job);

            await publisher.Publish(new ResponseJob(
                string.Empty, 
                context.Message.ProcessId,
                 ' ', 0, true), context.CancellationToken);
        }
    }
}