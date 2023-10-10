using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.XPath;
using AsyncFlow.Queues.Generator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AsyncFlow.Queues.Generator;
using FlowData=ImmutableArray<(string? classNamespace, string className, IEnumerable<UsingDirectiveSyntax> namespaces, string queueName, ImmutableArray<string> args)>;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class ExecutorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var sourceProvider = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, ct) => node.IsKind(SyntaxKind.ClassDeclaration),
            static (context, ct) => (ClassDeclarationSyntax)context.Node)
            .Where(classDeclaration => classDeclaration.AttributeLists.Any(x => x.Attributes.Any(y => y.Name.ToString() == "Flow")))
            .Collect();


        context.RegisterSourceOutput(sourceProvider, GenerateSource);
    }

    private void GenerateSource(SourceProductionContext context, ImmutableArray<ClassDeclarationSyntax> values)
    {
       
        var classData = GetTemplate("ExecutorTemplate");

        var flowData = values.Select(@class => {
            var className = @class.Identifier.Text;
            var queueName = @class.GetAttributeValue("Flow","QueueName")??className;
            var args=@class.GetBaseInterfaceGenericParameters("IAsyncFlow").ToImmutableArray();
            var namespaces = @class.Ancestors().OfType<UsingDirectiveSyntax>();
            var classNamespace = @class.GetNamespaceFromClass();
            return (classNamespace,className,namespaces,queueName, args);
        }).ToImmutableArray();
        
        GenerateExecutorClasses(context, flowData, classData);
        GenerateFlowRegistrationClass(context, flowData);
    }

    private static void GenerateExecutorClasses(SourceProductionContext context, ImmutableArray<(string? classNamespace, string className, IEnumerable<UsingDirectiveSyntax> namespaces, string queueName, ImmutableArray<string> args)> flowData, string classData)
    {
        foreach (var item in flowData)
        {
            var sourceBuilder = new System.Text.StringBuilder();
            foreach (var @namespace in item.namespaces)
            {
                sourceBuilder.AppendLine($"using {@namespace.Name};");
            }

            if (item.classNamespace != null)
                sourceBuilder.AppendLine($"using {item.classNamespace};");

            var source = classData
                .Replace("TRequest", item.args[0])
                .Replace("TResult", item.args[1])
                .Replace("TQueueName", item.queueName);
            sourceBuilder.Append(source);
            var sourceText = SourceText.From(sourceBuilder.ToString(), System.Text.Encoding.UTF8);
            context.AddSource($"{item.args[0]}_Executor.g.cs", sourceText);
        }
    }

    private string GetTemplate(string name)
    {
        using var classDataStream = this.GetType().Assembly
            .GetManifestResourceStream($"AsyncFlow.Queues.Generator.{name}.txt");
        using var streamReader = new StreamReader(classDataStream);
        var classData = streamReader.ReadToEnd();
        return classData;
    }

    public void GenerateFlowRegistrationClass(SourceProductionContext context,FlowData flowData)
    {
        var classData=GetTemplate("FlowsRegistrationExt");
        var sourceBuilder = new StringBuilder();
        var usingDirectiveSyntaxes = flowData
            .SelectMany(item=>item.namespaces)
            .Select(itm=>$"using {itm.Name};");

        var namespaces = flowData.Select(item => $"using {item.classNamespace};");
        sourceBuilder.Append( classData.Replace("{usings}", string.Join("\n", usingDirectiveSyntaxes.Concat(namespaces))));
       foreach (var item in flowData)
       {
           AddFlowRegistration(sourceBuilder,item.args[0],item.args[1],item.className);
       }

       sourceBuilder.AppendLine("}");
       sourceBuilder.AppendLine("}");
       
       var sourceText = SourceText.From(sourceBuilder.ToString(), System.Text.Encoding.UTF8);
       context.AddSource("ServiceCollectionExt.g.cs", sourceText);
    }
    public void AddFlowRegistration(StringBuilder builder,string requestType, string resultType, string flowType)
    {
        builder.AppendLine(
            $"services.AddTransient<IAsyncFlow<{requestType},{resultType}>,{flowType}>();");
        builder.AppendLine(
            $"services.AddTransient<IExecutor<{requestType}>, {requestType}_Executor>();");
    }
}
