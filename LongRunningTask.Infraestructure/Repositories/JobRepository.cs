using Microsoft.EntityFrameworkCore;
using LongRunningTask.Infraestructure.Repositories.Entites;

namespace LongRunningTask.Infraestructure.Repositories;

public class JobRepository(JobDbContext context) : IJobRepository
{
    private readonly JobDbContext _context = context;

    // CREATE
    public async Task CreateAsync(Job job)
    {
        var existingJob = await GetByIdAsync(job.JobId);

        if (existingJob != null)
        {
            if (job.IsFinished)
            {
                throw new JobAlreadyFinishedException($"Job with ID {job.JobId} is already finished.", existingJob);
            }

            throw new JobAlreadyExistsException($"Job with ID {job.JobId} already exists.", existingJob);
        }

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
    }

    // READ (by id)
    public async Task<Job?> GetByIdAsync(Guid jobId)
    {
        return await _context.Jobs.AsNoTracking().FirstOrDefaultAsync(x => x.JobId == jobId);
    }

    public async Task<Job?> GetByProcessIdAsync(Guid processId)
    {
        return await _context.Jobs.AsNoTracking().FirstOrDefaultAsync(x => x.ProcessId == processId);
    }

    public async Task<Job?> GetByStatus(params string[] statuses)
    {
        return await _context.Jobs.AsNoTracking().FirstOrDefaultAsync(x => statuses.Contains(x.Status));
    }

    // READ (all)
    public async Task<List<Job>> GetAllAsync()
    {
        return await _context.Jobs.AsNoTracking().ToListAsync();
    }

    // UPDATE
    public async Task UpdateAsync(Job job)
    {
        await _context.Jobs
            .Where(j => j.ProcessId == job.ProcessId)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(j => j.Status, job.Status)
                    .SetProperty(j => j.ProcessStartedOn, job.ProcessStartedOn)
                    .SetProperty(j => j.ProcessCompletedOn, job.ProcessCompletedOn)
                    .SetProperty(j => j.CancelledOn, job.CancelledOn)
            );

        await _context.SaveChangesAsync();
    }


    // DELETE
    public async Task DeleteAsync(Guid jobId)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
        {
            return;
        }

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
    }
}