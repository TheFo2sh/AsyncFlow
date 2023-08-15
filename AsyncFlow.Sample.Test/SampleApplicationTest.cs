using AsyncFlow.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Refit;

namespace AsyncFlow.Sample.Test;

public class SampleApplicationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ISampleApplicationClient _client;

    public SampleApplicationTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client=RestService.For<ISampleApplicationClient>(_factory.CreateClient());
    }

    [Fact]
    public async Task ShouldProcessJobCorrectly()
    {
        var enqueueResponse = await _client.EnqueueJob(new GenerateDataRequest(-1));
        var statusResponse = await StatusResponse(enqueueResponse).WaitAsync(TimeSpan.FromMinutes(1));
       
        statusResponse.Status.Should().Be("Succeeded");
        var resultResponse = await _client.GetJobResult(enqueueResponse.RequestId);
        resultResponse.Data.Should().Be("Ahmed");
    }

    private async Task<StatusResponse> StatusResponse(EnqueueResponse enqueueResponse)
    {
        StatusResponse statusResponse;
        do
        {
            statusResponse = await _client.GetJobStatus(enqueueResponse.RequestId);
        } 
        while (statusResponse.Status == "Processing");

        return statusResponse;
    }
}