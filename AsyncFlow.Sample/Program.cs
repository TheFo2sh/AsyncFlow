using AsyncFlow;
using AsyncFlow.Sample;
using AsyncFlow.ServiceCollection;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(x => x.UseMemoryStorage());
builder.Services.AddHangfireServer();
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
app.MapGet("/", () => "Hello World!");
app.MapFlow<GenerateDataJob, GenerateDataRequest, GenerateDataResponse>("data");
app.Run();
public partial class Program { }