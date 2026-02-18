using LongRunningTask.Domain.Configuration;
using LongRunningTask.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace LongRunningTask.Domain;

public class ThreadDelayProvider(IOptions<ThreadDelayConfiguration> configuration) : IDelayProvider
{
    private readonly Random _random = new();

    public void Delay()
    {
        var emissionDelay = _random.Next(0, configuration.Value.MaxDelayInMilliseconds);

        Thread.Sleep(emissionDelay);
    }
}
