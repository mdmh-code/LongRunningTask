namespace LongRunningTask.Infraestructure.Queues;

public record RequestJob(string UserId, string ProcessId, string Input);
public record ResponseJob(string UserId, string ProcessId, char Result, int Position);
