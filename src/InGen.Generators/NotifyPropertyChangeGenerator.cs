using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InGen.Generators
{
    [Generator]
    public class NotifyPropertyChangeGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReciever = (AutoNotifyPropertyChangeSyntaxReciever)context.SyntaxReceiver;

            if(syntaxReciever.IdentifiedClass is ClassDeclarationSyntax userClass)
            {
                var model = context.Compilation.GetSemanticModel(userClass.SyntaxTree);
                var namespaceDeclaration = userClass.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
                var nmspc = namespaceDeclaration.Name.ToString();
                var notifySymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged").ToDisplayString();
                var code = GenerateSource(nmspc,userClass.Identifier.Text, notifySymbol);

                context.AddSource(
                    $"{userClass.Identifier.Text}.generated",
                    SourceText.From(code, Encoding.UTF8)
                );
            } 
        }

        private string GenerateSource(string namespc,string className,string notifyInfo)
        {

            var builder = new StringBuilder();
            builder.Append($@"
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace {namespc}
{{
    public partial class {className} : {notifyInfo}
{{
    {GetNotifyPropertyChangeImplementation()}
}}
}}");

            return builder.ToString();
        }


        private string GetNotifyPropertyChangeImplementation()
        {
            return @"
public event PropertyChangedEventHandler PropertyChanged;

public void NotifyPropertyChanged([CallerMemberName] string propertyName = """")
{
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}";
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AutoNotifyPropertyChangeSyntaxReciever());
        }

        private class AutoNotifyPropertyChangeSyntaxReciever : ISyntaxReceiver
        {
            public ClassDeclarationSyntax IdentifiedClass { get; set; }
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if(syntaxNode is ClassDeclarationSyntax classDeclaration)
                {
                    var attributes = classDeclaration.DescendantNodes().OfType<AttributeSyntax>();
                    if (attributes.Any())
                    {
                        var expectedAttribute = attributes.FirstOrDefault(x => ExtractName(x.Name) == "AutoImplmentNotifyChange");
                        if (expectedAttribute != null) IdentifiedClass = classDeclaration;
                    }
                }
            }

            private static string ExtractName(TypeSyntax type)
            {
                while (type != null)
                {
                    switch (type)
                    {
                        case IdentifierNameSyntax ins:
                            return ins.Identifier.Text;

                        case QualifiedNameSyntax qns:
                            type = qns.Right;
                            break;

                        default:
                            return null;
                    }
                }

                return null;
            }
        }
    }
}
