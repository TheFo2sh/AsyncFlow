namespace AsyncFlow.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class Flow : Attribute
{
    public string QueueName { get; set; }
}