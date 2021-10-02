using Microsoft.CodeAnalysis;
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
            var syntaxReceiver = (AutoToStringSyntaxReciever)context.SyntaxContextReceiver;

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
                       #if DEBUG
                                public override string ToString()
                                {{
                                    return $""{toStringContend}"";
                                }}
                       #endif
                            }}
                        }}";
            return code;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AutoToStringSyntaxReciever());
        }

        private class AutoToStringSyntaxReciever : ISyntaxContextReceiver //where TAttribute:Attribute
        {
            public ClassDeclarationSyntax IdentifiedClass { get; private set; } 

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Any())
                {
                    var classDeclarationSemantics = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

                    if(classDeclarationSemantics.GetAttributes().Any(x=>x.AttributeClass.ToDisplayString() == "InGen.Types.Attributes.AutoToStringAttribute"))
                    {
                        IdentifiedClass = classDeclarationSyntax;
                    }
                }
            }
        }
        
    }
}
