﻿using InGen.Types.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InGen.Client
{
    [AutoToString]
    public partial class AutoToStringDemo
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
    }
}
