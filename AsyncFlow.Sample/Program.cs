using AsyncFlow;
using AsyncFlow.Sample;
using AsyncFlow.ServiceCollection;
using Hangfire;
using Hangfire.Storage.SQLite;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(x => x.UseSQLiteStorage());
builder.Services.AddHangfireServer(options =>
{
    options.IsLightweightServer = true;
    options.Queues = Flows.All;
});
builder.Services.AddMemoryCache();
builder.Services.AddTransient<GenerateDataJob>();

builder.Services.AddAsyncFlow(options => options.UseMemoryCache());
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