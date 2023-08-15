using AsyncFlow.Core.Cache;

namespace AsyncFlow.ServiceCollection;

public class AsyncFlowOptions
{
    public IAsyncFlowCache Cache { get; set; }
}