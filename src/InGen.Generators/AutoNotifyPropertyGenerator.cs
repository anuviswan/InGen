using InGen.Types.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
                var classDeclaration = fieldDeclaration.Ancestors().OfType<ClassDeclarationSyntax>().First();
                var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);

                var hasNotifyPropertyChangedImplemented = InheritsFrom<INotifyPropertyChanged>(classDeclaration.sym)
                var variableDeclaration = fieldDeclaration.Declaration.Variables.First();
                var fieldName = variableDeclaration.Identifier.Text;
                var fieldType = fieldDeclaration.Declaration.Type;

            }
        }

        private bool ImplementsInterface<T>(ClassDeclarationSyntax classDeclaration)
        {
            var interfaceTypeName = 
        }
        private bool InheritsFrom<T>(Type typeInQuestion)
        {
            var interfaceType = typeof(T);
            foreach (var interfaceMember in interfaceType.GetMembers().OfType<IMethodSymbol>())
            {
                var memberFound = false;
                foreach (var typeMember in typeInQuestion.GetMembers().OfType<IMethodSymbol>())
                {
                    if (typeMember.Equals(typeInQuestion.FindImplementationForInterfaceMember(interfaceMember)))
                    {
                        // this member is found
                        memberFound = true;
                        break;
                    }
                }
                if (!memberFound)
                {
                    return false;
                }
            }
            return true;
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
