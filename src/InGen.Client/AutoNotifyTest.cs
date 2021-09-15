using InGen.Types.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InGen.Client
{
    [AutoImplmentNotifyChange]
    public partial class AutoNotifyPropertyChangedTest
    {
        [AutoNotify]
        private string _firstName;
        private string _lastName;
    }
}
