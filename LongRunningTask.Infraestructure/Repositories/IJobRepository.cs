using LongRunningTask.Infraestructure.Repositories.Entites;

namespace LongRunningTask.Infraestructure.Repositories;

public interface IJobRepository
{
    Task CreateAsync(Job job);
    Task DeleteAsync(Guid jobId);
    Task<List<Job>> GetAllAsync();
    Task<Job?> GetByIdAsync(Guid jobId);
    Task UpdateAsync(Job job);
    Task<Job?> GetByProcessIdAsync(Guid processId);

    Task<Job?> GetByStatus(params string[] statuses);
}
