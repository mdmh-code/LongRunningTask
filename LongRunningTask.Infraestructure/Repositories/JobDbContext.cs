using Microsoft.EntityFrameworkCore;
using LongRunningTask.Infraestructure.Repositories.Entites;
public class JobDbContext(DbContextOptions<JobDbContext> options) : DbContext(options)
{
    public DbSet<Job> Jobs => Set<Job>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(x => x.JobId);
            entity.Property(x => x.UserId).HasMaxLength(64);
            entity.Property(x => x.ProcessId).HasMaxLength(64);
            entity.Property(x => x.Status).HasMaxLength(20);
        });
    }
}