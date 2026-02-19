namespace LongRunningTask.Infraestructure.Queues;

public record RequestJob(string UserId, Guid ProcessId, string Input);
public record CancelJob(Guid ProcessId);
public record ResponseJob(string UserId, Guid ProcessId, char Result, int Position, bool IsLast
);
