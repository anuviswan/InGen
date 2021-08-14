
namespace InGen.Generators.Tests.Tests.AutoToString.TestCaseData.TestCase
{
    public partial class ClassWithSinglePublicProperty
    {
        public override string ToString()
        {
            return $"{nameof(UserName)}={UserName}";
        }
    }
}
