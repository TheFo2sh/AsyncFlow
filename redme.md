# AsyncFlow Library

Integrate asynchronous job flows with ease in your .NET applications. AsyncFlow is designed for web applications that use a pattern where users call an invoke API, receive a job ID, and can then track and retrieve the job's results using separate endpoints.

## Features

- **Asynchronous Job Invocation**: Easily handle long-running jobs by providing an immediate response with a job ID.
- **Status Tracking**: Allow users to track the status of their job.
- **Flexible Result Retrieval**: Once the job is complete, users can retrieve the result.
- **Integration with Hangfire**: Make use of the robust background processing library, Hangfire, to handle job execution.
- **Extensible Cache Options**: Store job results in-memory or use a distributed cache.

## Getting Started

### Installation

Use the package manager [NuGet](https://www.nuget.org/) to install AsyncFlow:

```bash
dotnet add package AsyncFlow
```

### Configuration

1. **Setup Hangfire (or any supported background job processor)**:

   ```csharp
   builder.Services.AddHangfire(x => x.UseMemoryStorage());
   builder.Services.AddHangfireServer();
   ```

2. **Add AsyncFlow with Desired Cache**:

   For in-memory cache:

   ```csharp
   builder.Services.AddAsyncFlow(options => options.UseMemoryCache());
   ```

   For distributed cache:

   ```csharp
   builder.Services.AddAsyncFlow(options => options.UseDistributedCache(yourDistributedCacheInstance));
   ```

3. **Map Endpoints**:

   ```csharp
   app.MapFlow<YourJobClass, YourRequestType, YourResponseType>("endpointName");
   ```

### Custom Endpoint Configuration

You can customize the behavior of the AsyncFlow endpoints using the `AsyncFlowEndpointConfigurator`:

```csharp
var configurator = new YourConfiguratorSubClass();
app.MapFlow<YourJobClass, YourRequestType, YourResponseType>("endpointName", configurator);
```

## Usage Example

Define a job:

```csharp
public class GenerateDataJob : IAsyncFlow<GenerateDataRequest, GenerateDataResponse>
{
    public async Task<GenerateDataResponse> ProcessAsync(GenerateDataRequest request)
    {
        // Your logic here
    }
}
```

Invoke it:

```csharp
app.MapFlow<GenerateDataJob, GenerateDataRequest, GenerateDataResponse>("data");
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License.



