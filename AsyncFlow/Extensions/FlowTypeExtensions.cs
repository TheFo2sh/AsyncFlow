namespace AsyncFlow.Extensions;

public static class FlowTypeExtensions
{
    public static string GetQueueName(this Type type)
    {
        return type.Name.ToLower();
    }
}