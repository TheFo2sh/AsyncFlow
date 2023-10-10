using System.Reflection;
using AsyncFlow.Attributes;

namespace AsyncFlow.Extensions;

public static class FlowTypeExtensions
{
    public static string GetQueueName(this Type type)
    {
        var flowAttribute = type.GetCustomAttribute<Flow>();
        if(flowAttribute == null)
        {
            throw new ArgumentException($"Type {type.Name} is not a flow");
        }
        return !string.IsNullOrEmpty(flowAttribute.QueueName) ? flowAttribute.QueueName : type.Name.ToLower();
    }
}