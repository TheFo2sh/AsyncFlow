using AsyncFlow;
using AsyncFlow.Sample;
using AsyncFlow.ServiceCollection;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHangfire(x => x.UseMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<GenerateDataJob>();

builder.Services.AddAsyncFlow(options => options.UseMemoryCache());

var app = builder.Build();


app.UseHangfireDashboard();
app.MapGet("/", () => "Hello World!");
app.MapFlow<GenerateDataJob, GenerateDataRequest, GenerateDataResponse>("data");
app.Run();
public partial class Program { }