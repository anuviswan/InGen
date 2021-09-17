using InGen.Types.Attributes;

namespace InGen.Client
{
    public partial class AutoNotifyGeneratorTests
    {
        [AutoNotify]
        private string _fullName;
        [AutoNotify]
        private string _displayName;

        private string _userName;

       
    }
}
