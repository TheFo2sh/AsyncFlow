using Hangfire.Storage;
using Hangfire.Storage.Monitoring;

namespace AsyncFlow.Helpers;

public static class MonitoringApiExtention
{
    public static IEnumerable<KeyValuePair<string, FailedJobDto>> GetFailedJobs(this IMonitoringApi api, int pullby = 1000)
    {
        var page = 0;
        while (true)
        {
            var jobs = api.FailedJobs(page, pullby);
            if (jobs.Count == 0)
                yield break;
            foreach (var job in jobs)
                yield return job;
            page++;
        }
    }
  
}