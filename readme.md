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

Use the package manager [NuGet](https://www.nuget.org/packages/AsyncFlow/) to install AsyncFlow:

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
Certainly! Let's replace the C# record descriptions with JSON examples for the responses:

## The Auto created API Endpoints

When you integrate the `AsyncFlow` library into your application, the following API endpoints are provided for you:

1. **Enqueue Endpoint**:

   - **Path**: `/[flowName]`
   - **HTTP Method**: POST
   - **Purpose**: To initiate the async flow process.
   - **Response**:
     ```json
     {
       "RequestId": "12345-abcd",
       "DateTime": "2023-08-14T15:30:45Z"
     }
     ```

2. **Status Endpoint**:

   - **Path**: `/[flowName]/{jobId}/status`
   - **HTTP Method**: GET
   - **Purpose**: To check the status of a previously enqueued request.
     - **Response**:
       ```json
       {
         "RequestId": "12345-abcd",
         "Status": "Processing",
         "CreatedAt": "2023-08-14T15:30:45Z",
         "progressData": {
             "progress": "Generating Data",
             "percentage": 90
         }
       }
       ```
3. **Result Endpoint**:

   - **Path**: `/[flowName]/{jobId}/result`
   - **HTTP Method**: GET
   - **Purpose**: To retrieve the result of the job.
   - **Response**: JSON that represents the result object.

4. **Delete Endpoint**:

   - **Path**: `/[flowName]/{jobId}`
   - **HTTP Method**: DELETE
   - **Purpose**: To delete the result of the job.
   - **Response**: No content (empty response).

5. **Error Endpoint**:

- **Path**: /[flowName]/{jobId}/error
- **HTTP Method**: GET
- **Purpose**: To retrieve error details if a job fails.
- **Response**:
```json{
  "JobId": "unique-job-id",
  "Error": {
  "Type": "ExceptionType",
  "Message": "Detailed error message",
  "StackTrace": "Stack trace details..."
     }
  }
 ```
---

Remember to replace the placeholders like "*Details about the Result endpoint, including a JSON example.*" with the actual details for those endpoints if they provide responses similar to the ones you've described. If not, describe them as needed.

Also, make sure to guide your users on how to replace the `[flowName]` placeholder appropriately.
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
    public async Task<GenerateDataResponse> ProcessAsync(GenerateDataRequest request ,IProgress<ProgressData> progress , CancellationToken cancellationToken)
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



