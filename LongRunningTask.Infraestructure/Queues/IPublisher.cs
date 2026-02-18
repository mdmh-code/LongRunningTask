namespace LongRunningTask.Infraestructure.Queues;
    public interface IPublisher
    {
        Task Publish<T>(T message, CancellationToken cancellationToken = default);
    }
