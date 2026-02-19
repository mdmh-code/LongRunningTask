using LongRunningTask.Infraestructure.Repositories.Entites;

namespace LongRunningTask.Infraestructure.Repositories;

public class JobAlreadyExistsException(string message, Job job) : Exception(message)
{
    public Job Job { get; } = job;
}
