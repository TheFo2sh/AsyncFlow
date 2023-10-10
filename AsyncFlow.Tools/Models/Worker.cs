namespace AsyncFlow.Tools.Models;

public  class Worker
{
    public string Name { get; set; }

    public string[] Queues { get; set; }

    public long Instances { get; set; }
    
}