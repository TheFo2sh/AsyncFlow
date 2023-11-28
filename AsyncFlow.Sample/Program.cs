using System.Text.Json;
using AsyncFlow;
using AsyncFlow.Core;
using AsyncFlow.Interfaces;
using AsyncFlow.Sample;
using AsyncFlow.ServiceCollection;
using Correlate.AspNetCore;
using Correlate.DependencyInjection;
using Hangfire;
using Hangfire.Correlate;
using Hangfire.MemoryStorage;
using Hangfire.Storage.SQLite;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = false;
    options.TimestampFormat = "HH:mm:ss ";
    options.JsonWriterOptions = new JsonWriterOptions
    {
        Indented = true
    };
});
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.WriteTo.File(formatter:new JsonFormatter(),@"C:\Users\Ahmed\RiderProjects\Projects\AsyncFlow\AsyncFlow.Sample\bin\Debug\net7.0/log.txt",shared:true);
    configuration.Enrich.FromLogContext();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCorrelate(options => 
    options.RequestHeaders = new []
    {
        "X-Correlation-ID",
    }
);

builder.Services.AddHangfire((serviceProvider, config) =>
{
    config.UseMemoryStorage();
    config.UseCorrelate(serviceProvider);
});

builder.Services.AddHangfireServer(options =>
{
    options.Queues = Flows.All;
});
builder.Services.AddFlows(options => options.UseMemoryCache());
builder.Services.AddMemoryCache();
var app = builder.Build();
app.UseCorrelate();

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