using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AsyncFlow.Queues.Generator.Util;

public static class ClassDeclarationExt
{
    public static string? GetNamespaceFromClass(this ClassDeclarationSyntax classSyntax)
    {
        // For traditional namespaces
        var traditionalNamespace = classSyntax.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        if (traditionalNamespace != null)
        {
            return traditionalNamespace.Name.ToString();
        }
        var parentCompilationUnit = classSyntax.SyntaxTree.GetCompilationUnitRoot();
        return parentCompilationUnit
            .ChildNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .First()
            .Name
            .ToString();
    }
    public static string? GetAttributeValue(this ClassDeclarationSyntax classDeclarationSyntax, string attributeName,
        string propertyName)
    {
        return classDeclarationSyntax.AttributeLists
            .SelectMany(attrList => attrList.Attributes)
            .Where(attr => attr.Name.ToString() == attributeName)
            .Select(attr =>
                attr.ArgumentList?.Arguments.FirstOrDefault(arg =>
                    arg.NameEquals?.Name.Identifier.Text == propertyName))
            .Where(arg => arg != null)
            .Select(arg => arg.Expression.ToString().Trim('"'))
            .FirstOrDefault();
    }

    public static IEnumerable<string> GetBaseInterfaceGenericParameters(
        this ClassDeclarationSyntax classDeclarationSyntax, string interfaceName)
    {
        
        if (classDeclarationSyntax.BaseList?.Types.Count > 0)
        {
            foreach (var baseType in
                     classDeclarationSyntax.BaseList.Types.Select(baseTypeSyntax => baseTypeSyntax.Type))
            {
                if (baseType is not GenericNameSyntax genericName || genericName.Identifier.Text != interfaceName)
                    continue;

                var typeArguments = genericName.TypeArgumentList.Arguments;

                foreach (var typeArgument in typeArguments)
                {
                    yield return typeArgument.ToString();
                }

                break;
            }
        }
        else
            throw new Exception($"class does not implement interface {interfaceName}");
    }
}
