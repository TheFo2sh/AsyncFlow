using System.Text;
using AsyncFlow.Tools.Helpers;
using AsyncFlow.Tools.Models;
using CliWrap;
using CliWrap.Exceptions;
using Typin;
using Typin.Attributes;
using Typin.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AsyncFlow.Tools.Commands;

[Command("run", Description = "Run a project")]
public class RunCommand:ICommand
{
    [CommandOption("project", 'p', Description = "The CSharp project file to run")]
    public string Project { get; set; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var token = console.GetCancellationToken();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(new CamelCaseNamingConvention())
            .Build();

        var directory = new FileInfo(Project).DirectoryName;
        var manifaistData = await File.ReadAllTextAsync(Path.Combine(directory, "asyncflow.yaml"), token);
        var manifest = deserializer.Deserialize<AsyncFlowModel>(manifaistData);

        await Task.WhenAll(GetTasks(manifest,token));

    }

    private IEnumerable<Task> GetTasks(AsyncFlowModel manifest, CancellationToken cancellationToken)
    {
        foreach (var worker in manifest.Workers)
        {
            for (var _ = 0; _ < worker.Instances; _++)
            {
                yield return Run(worker, cancellationToken);
            }
        }
    }
    private async Task Run( Worker worker, CancellationToken token)
    {
        var port = PortHelper.GetAvailablePort();
        var dotnetCall = Cli.Wrap(@"dotnet")
            .WithArguments(
                $"run --project {Project} --configuration Release --urls=http://localhost:{port} {string.Join(",", worker.Queues)}");

        Console.WriteLine($"http://localhost:{port}");
        await dotnetCall.WithValidation(CommandResultValidation.None).ExecuteAsync(CancellationToken.None,token);
    }
}