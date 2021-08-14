﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InGen.Generators
{
    [Generator]
    public class AutoToStringGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (AutoToStringSyntaxReciever)context.SyntaxReceiver;

            var userClass = syntaxReceiver.IdentifiedClass;
            if (userClass is null)
            {
                return;
            }


            var properties = userClass.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            var code = GetSource(userClass.Identifier.Text, properties);

            context.AddSource(
                $"{userClass.Identifier.Text}.generated",
                SourceText.From(code, Encoding.UTF8)
            );
        }

        private string GetSource(string className, IEnumerable<PropertyDeclarationSyntax> properties)
        {
            var toStringContend = string.Join(",", properties.Select(x => $"{x.Identifier.Text}={{{x.Identifier.Text}}}"));
            var code = $@"
                        namespace InGen.Client {{
                            public partial class {className}
                            {{
                                public override string ToString()
                                {{
                                    return $""{toStringContend}"";
                                }}
                            }}
                        }}";
            return code;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AutoToStringSyntaxReciever());
        }

        private class AutoToStringSyntaxReciever : ISyntaxReceiver
        {
            public ClassDeclarationSyntax IdentifiedClass { get; private set; }
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclaration)
                {
                    var attributes = classDeclaration.DescendantNodes().OfType<AttributeSyntax>();
                    if (attributes.Any())
                    {
                        var autoToStringAttribute = attributes.FirstOrDefault(x => ExtractName(x.Name) == "AutoToString");
                        if (autoToStringAttribute != null) IdentifiedClass = classDeclaration;
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
