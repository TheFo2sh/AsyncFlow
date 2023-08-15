using AsyncFlow.Interfaces;
using Bogus;

namespace AsyncFlow.Sample;

public record GenerateDataRequest(int Count);
public record GenerateDataResponse(string Data);
public class GenerateDataJob:IAsyncFlow<GenerateDataRequest,GenerateDataResponse>
{
    public async Task<GenerateDataResponse> ProcessAsync(GenerateDataRequest request)
    {
        if (request.Count == -1)
            return new GenerateDataResponse("Ahmed");
        var faker = new Faker();
        var data = faker.Random.Words(request.Count);
        return new GenerateDataResponse(data);
    }
}