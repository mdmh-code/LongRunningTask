using LongRunningTask.Infraestructure.Repositories.Entites;

namespace LongRunningTask.Infraestructure.Repositories;

public class JobAlreadyFinishedException(string message, Job job) : Exception(message)
{
    public Job Job { get; } = job;
}
