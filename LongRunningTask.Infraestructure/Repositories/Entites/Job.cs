using System.ComponentModel.DataAnnotations;
using LongRunningTask.Infraestructure.Queues;

namespace LongRunningTask.Infraestructure.Repositories.Entites;

public class Job
{
    public static string PENDING_STATUS = "Pending";
    public static string PROCESSING_STATUS = "Processing";
    public static string COMPLETED_STATUS = "Completed";
    public static string CANCELLED_STATUS = "Cancelled";
    private static string[] FINISHED_STATUSES = [COMPLETED_STATUS, CANCELLED_STATUS];

    [Key]
    public Guid JobId { get; set; }

    public string? UserId { get; set; }

    public Guid? ProcessId { get; set; }

    public string Status { get; set; } = PENDING_STATUS;

    public DateTimeOffset? CancelledOn { get; set; }

    public DateTimeOffset? ProcessStartedOn { get; set; }

    public DateTimeOffset? ProcessCompletedOn { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsFinished => FINISHED_STATUSES.Contains(Status);

    public static Job CreateNew(RequestJob request) => new()
    {
        UserId = request.UserId,
        ProcessId = request.ProcessId,
        Status = PROCESSING_STATUS,
        CreatedAt = DateTime.UtcNow
    };

    public void SetProcessing()
    {
        Status = PROCESSING_STATUS;
        ProcessStartedOn = DateTime.UtcNow;
    }

    public void SetCancelled()
    {
        Status = CANCELLED_STATUS;
        CancelledOn = DateTime.UtcNow;
    }

    public void SetCompleted()
    {
        Status = COMPLETED_STATUS;
        ProcessCompletedOn = DateTime.UtcNow;
    }
}