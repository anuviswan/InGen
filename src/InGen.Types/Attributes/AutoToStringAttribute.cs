using System;
using System.Collections.Generic;
using System.Linq;

namespace InGen.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoToStringAttribute:Attribute
    {
        public IEnumerable<string> PropertyNames { get; set;  }
        public AutoToStringAttribute() => PropertyNames = Enumerable.Empty<string>();

        public AutoToStringAttribute(IEnumerable<string> propertyNames)
        {
            PropertyNames = propertyNames;
        }
    }
}
