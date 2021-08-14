using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VerifyCS = InGen.Generators.Tests.Utils.CSharpSourceGeneratorVerifier<InGen.Generators.AutoToStringGenerator>;

namespace InGen.Generators.Tests.AutoToString
{
    public class AutoToStringGeneratorsTests
    {
        [TestCaseSource(typeof(AutoToStringGeneratorTestData),nameof(AutoToStringGeneratorTestData.TestCases))]
        public async Task AutoToString_Test(string code,string generatedCode)
        {
            await new VerifyCS.Test
            {
                TestState =
                    {
                        Sources = { code },
                        GeneratedSources =
                        {
                            (typeof(AutoToStringGenerator), "GeneratedFileName", SourceText.From(generatedCode, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                        },
                    },
            }.RunAsync();
        }
    }


    public class AutoToStringGeneratorTestData
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(File.ReadAllText(@"TestCaseData\TestCase\ClassWithSinglePublicProperty.cs"),
                    File.ReadAllText(@"TestCaseData\TestCase\ClassWithSinglePublicProperty_Fixed.cs"));
            }
        } 
    }
}
