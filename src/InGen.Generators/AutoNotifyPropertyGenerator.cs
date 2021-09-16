using InGen.Types.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InGen.Generators
{
    [Generator]
    public class AutoNotifyPropertyGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReciever = (FieldSyntaxReciever<AutoNotifyAttribute>)context.SyntaxReceiver;
            
            if(syntaxReciever.IdentifiedField is FieldDeclarationSyntax fieldDeclaration)
            {
                var sourceBuilder = new StringBuilder();
                var classDeclaration = fieldDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().First();
                var namespaceDeclaration = classDeclaration.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
                var nmspc = namespaceDeclaration.Name.ToString();
                var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var classInterfaces = model.GetDeclaredSymbol(classDeclaration).AllInterfaces;
                var notifySymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");
                var implementINotifyPropertyChanged = classInterfaces.Contains(notifySymbol);

                var variableDeclaration = fieldDeclaration.Declaration.Variables.First();
                var fieldName = variableDeclaration.Identifier.Text;
                var fieldType = fieldDeclaration.Declaration.Type.ToString();

                sourceBuilder.Append($@"using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace {nmspc}
{{
    public partial class {classDeclaration.Identifier.Text} {(implementINotifyPropertyChanged == false? $": {notifySymbol.Name}" : string.Empty)}
{{
    {(implementINotifyPropertyChanged == false? $"{GetNotifyPropertyChangeImplementation()}" : string.Empty)}
    {GetPropertyDeclaration(fieldName,fieldType)}
}}
}}");

                context.AddSource(
                    $"{classDeclaration.Identifier.Text}_{fieldName}.generated",
                    SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));


            }
        }



        private string GetPropertyDeclaration(string fieldName,string fieldType)
        {
            var str = new StringBuilder();
            str.Append($"public {fieldType} {NormalizePropertyName(fieldName)}");
            str.Append("{");
            str.Append($"get=> {fieldName};");
            str.Append($"set");
            str.Append("{");
            str.Append($"if({fieldName}==value) return;");
            str.Append($"{fieldName}=value;");
            str.Append($"NotifyPropertyChanged();");
            str.Append("}");
            str.Append("}");
            return str.ToString();
        }

        private string NormalizePropertyName(string fieldName)
        {
            return Regex.Replace(fieldName, "_[a-z]", delegate (Match m) {
                return m.ToString().TrimStart('_').ToUpper();
            });
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
            context.RegisterForSyntaxNotifications(() => new FieldSyntaxReciever<AutoNotifyAttribute>());
        }

        private class FieldSyntaxReciever<TAttribute> : ISyntaxReceiver where TAttribute:Attribute
        {
            public FieldDeclarationSyntax IdentifiedField { get; set; }
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if(syntaxNode is FieldDeclarationSyntax fieldDeclaration)
                {
                    var attributes = fieldDeclaration.DescendantNodes().OfType<AttributeSyntax>();
                    if (attributes.Any())
                    {
                        var expectedAttribute = attributes.FirstOrDefault(x => ExtractName(x.Name) == typeof(TAttribute).Name.Replace("Attribute", "")) ;
                        if (expectedAttribute != null) IdentifiedField = fieldDeclaration;
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
