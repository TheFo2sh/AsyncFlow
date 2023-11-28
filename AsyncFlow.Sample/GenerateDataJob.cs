using AsyncFlow.Attributes;
using AsyncFlow.Core;
using AsyncFlow.Interfaces;
using AsyncFlow.Responses;
using Bogus;
using Correlate;

namespace AsyncFlow.Sample;

[Flow(QueueName = "ahmed")]
public class GenerateDataJob:IAsyncFlow<GenerateDataRequest,GenerateDataResponse>
{
    private readonly ILogger<GenerateDataJob> _logger;
    private readonly ICorrelationContextAccessor  _correlationManager;

    public GenerateDataJob(ILogger<GenerateDataJob> logger, ICorrelationContextAccessor  correlationManager)
    {
        _logger = logger;
        _correlationManager = correlationManager;
    }

    public async Task<GenerateDataResponse> ProcessAsync(GenerateDataRequest request, IProgress<ProgressData> progress, CancellationToken cancellationToken)
    {
        _logger.LogInformation(" GenerateDataJob is running");
        _logger.LogError(" GenerateDataJob is error");
        _logger.LogWarning("warning {Time}",DateTime.Now);
        _logger.LogCritical("critical in" ,"GDJ");
        _logger.LogDebug("debug");
        _logger.LogTrace("trace");
        
        if (request.Count == -1)
            return new GenerateDataResponse("Ahmed");
        
        if(request.Count == -2)
            throw new ArgumentException("Count cannot be -2");
        var result = "";
        var faker = new Faker();
        for (var i = 0; i < 10; i++)
        { 
            result = faker.Random.Words(request.Count);
            await Task.Delay(1000, cancellationToken);
            
            progress.Report(new("GenerateDataJob",i*10));
        
        }
        return new GenerateDataResponse(result);
    }
}