namespace AsyncFlow.Responses;

public record StatusResponse(string RequestId, string Status, DateTime CreatedAt, ProgressData? ProgressData = null);
public record ProgressData(string Progress,double Percentage);
public record ErrorResponse(string RequestId, Error Error);
public record Error(string Type,string Message, string? StackTrace = null);