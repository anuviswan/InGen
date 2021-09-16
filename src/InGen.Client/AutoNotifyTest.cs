using InGen.Types.Attributes;

namespace InGen.Client
{
    public partial class DemoForAutoNotify
    {
      //  [AutoNotify]
        //private string _firstName;
        [AutoNotify]
        private string _lastName;

        public void Sample()
        {
            _lastName = "dsfds";
        }
    }
}
