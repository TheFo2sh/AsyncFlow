using AsyncFlow.Responses;
using Refit;

namespace AsyncFlow.Sample.Test;

public interface ISampleApplicationClient
{
    [Post("/data")]
    Task<EnqueueResponse> EnqueueJob([Body] GenerateDataRequest request);

    [Get("/data/{jobId}/status")]
    Task<StatusResponse> GetJobStatus(string jobId);

    [Get("/data/{jobId}/result")]
    Task<GenerateDataResponse> GetJobResult(string jobId);

    [Delete("/data/{jobId}")]
    Task DeleteJob(string jobId);
}