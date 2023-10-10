using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AsyncFlow.Queues.Generator;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class FlowGenerator : IIncrementalGenerator
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
        var sourceBuilder = new System.Text.StringBuilder();

        sourceBuilder.AppendLine("public static class Flows");
        sourceBuilder.AppendLine("{");
        var classNames = values.Select(v => {
            var queueName = v.AttributeLists
                .SelectMany(attrList => attrList.Attributes)
                .Where(attr => attr.Name.ToString() == "Flow")
                .Select(attr => attr.ArgumentList?.Arguments.FirstOrDefault(arg => arg.NameEquals is { Name.Identifier.Text: "QueueName" }))
                .Where(arg => arg != null)
                .Select(arg => arg.Expression.ToString().Trim('"'))
                .FirstOrDefault();
                
            return string.IsNullOrEmpty(queueName) ? v.Identifier.Text : queueName;
        }).Distinct().ToImmutableArray();
        
        var combined = string.Join(", ", classNames.Select(name=>"\"" + name + "\""));
        foreach (var className in classNames)
        {
            sourceBuilder.AppendLine($"    public static readonly string {className} = \"{className.ToLower()}\";");
        }
        sourceBuilder.AppendLine($"    public static string[] All = new[] {{ {combined.ToLower()} , \"default\" }};");
        sourceBuilder.AppendLine($"    public static string[] FromArgs(string[] args) => All.Intersect(args).ToArray();");

        sourceBuilder.AppendLine("}");
        
        var sourceText = SourceText.From(sourceBuilder.ToString(), System.Text.Encoding.UTF8);
        context.AddSource("Flows.g.cs", sourceText);
    }
    
}
