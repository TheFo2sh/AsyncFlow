namespace AsyncFlow.Responses;

public record EnqueueResponse(string RequestId, DateTime DateTime);
public record StatusResponse(string RequestId,string Status, DateTime CreatedAt);

