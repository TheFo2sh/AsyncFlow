using System.Linq.Expressions;

namespace AsyncFlow;

public interface IFlowEnqueuer
{
    Expression<Func<TRequest, Task>> Enqueue<TRequest>(string queueName);
}