using AsyncFlow;
using AsyncFlow.Core;
using AsyncFlow.Interfaces;
using AsyncFlow.Sample;
using AsyncFlow.ServiceCollection;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Storage.SQLite;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});
builder.Services.AddHangfireServer(options =>
{
    options.Queues = Flows.All;
});
builder.Services.AddFlows(options => options.UseMemoryCache());
builder.Services.AddMemoryCache();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0});

app.MapGet("/", () => "Hello World!");
app.MapFlow<GenerateDataJob, GenerateDataRequest, GenerateDataResponse>("data");
app.Run();
public partial class Program { }