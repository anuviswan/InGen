using InGen.Types.Attributes;

namespace InGen.Generators.Tests.Tests.AutoToString.TestCaseData.TestCase
{
    [AutoToString]
    public partial class ClassWithSinglePublicProperty
    {
        public string UserName { get; set; }
    }
}
