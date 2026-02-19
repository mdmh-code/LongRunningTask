namespace LongRunningTask.API.Models;

public record StringProcessRequest(string Message);
public record StringCancelRequest(Guid processId);