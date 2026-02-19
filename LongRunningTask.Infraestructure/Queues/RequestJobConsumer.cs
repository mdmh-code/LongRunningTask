using MassTransit;
using LongRunningTask.Domain;
using LongRunningTask.Domain.Interfaces;
using LongRunningTask.Infraestructure.Repositories;
using Microsoft.Extensions.Logging;
using LongRunningTask.Infraestructure.Repositories.Entites;

namespace LongRunningTask.Infraestructure.Queues;

public class RequestJobConsumer(
    ILogger<RequestJobConsumer> logger,
    IPublisher publisher,
    IJobRepository jobRepository,
    StringProcessor processor, IDelayProvider delayProvider) : IConsumer<RequestJob>
{
    public async Task Consume(ConsumeContext<RequestJob> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        logger.LogInformation("Processing {ProcessId}", message.ProcessId);

        var currentJob = Job.CreateNew(message);

        try
        {
            await jobRepository.CreateAsync(currentJob);
        }
        catch (Repositories.JobAlreadyExistsException ex)
        {
            logger.LogInformation(ex, "The job for {ProcessId}, The process will restart", currentJob.ProcessId);

            currentJob = ex.Job;
            currentJob.SetProcessing();

            await jobRepository.UpdateAsync(currentJob);
        }
        catch (JobAlreadyFinishedException ex)
        {
            logger.LogInformation(ex, "The job for {ProcessId} is already finished. No further processing will be done.", message.ProcessId);
            return;
        }

        var processedMessage = processor.Process(message.Input);
        try
        {
            for (int i = 0; i < processedMessage.Length; i++)
            {
                currentJob = (await jobRepository.GetByProcessIdAsync(currentJob?.ProcessId ?? Guid.Empty))!;

                if (currentJob.CancelledOn.HasValue)
                {
                    logger.LogWarning("Job for {ProcessId} was cancelled during processing.", message.ProcessId);
                    return;
                }

                char character = processedMessage[i];

                delayProvider.Delay();
                logger.LogInformation("Publishing result for {ProcessId}: {Character} at position {Position}", message.ProcessId, character, i);

                await publisher.Publish(new ResponseJob(message.UserId, message.ProcessId, character, i, i == processedMessage.Length - 1), cancellationToken);
            }

            currentJob.SetCompleted();
            await jobRepository.UpdateAsync(currentJob);
        }

        catch (OperationCanceledException)
        {
            logger.LogWarning("Processing of {ProcessId} was cancelled.", message.ProcessId);
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing {ProcessId}.", message.ProcessId);
            throw;
        }
    }
}
